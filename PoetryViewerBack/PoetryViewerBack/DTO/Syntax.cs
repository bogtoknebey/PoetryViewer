using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using PoetryViewerBack.Models;

namespace PoetryViewerBack.DTO
{
    public static class Syntax
    {
        public static Models.Syntax? GetIncludes(string author, int minLen)
        {
            Dictionary<string, int> inclusions = new Dictionary<string, int>();
            string dir = $"data/{author}";
            if (!Directory.Exists(dir)) return null;
            var subDirs = Directory.GetDirectories(dir);

            foreach (var subDir in subDirs)
            {
                string[] textFiles = Directory.GetFiles(subDir, "*.txt");
                string text = File.ReadAllText(textFiles[0]);
                string[] words = text.Split(
                    '\n', ' ', '"', '\'', 
                    '!', '-', '(', ')', 
                    '*', '.', '?', ']', 
                    '[', ',', ':', ';', 
                    '\r', '\t', '0', '1',
                    '2', '3', '4', '5',
                    '6', '7', '8', '9'
                );
                string currWord;
                foreach (var word in words)
                {
                    currWord = word.ToLower();
                    if (currWord.Length < minLen)
                        continue;
                    if (!inclusions.ContainsKey(currWord))
                        inclusions[currWord] = 0;
                    inclusions[currWord]++;
                }
            }
            Models.Syntax res = new Models.Syntax() { Author = author, Inclusions = inclusions };
            return res;
        }
        public static List<string>? GetWordsLocal(string author, int minLen = 1, int wordsCount = 1)
        {
            List<string> res = new List<string>();
            Models.Syntax syntax = DTO.Syntax.GetIncludes(author, minLen);
            List<string> words = new List<string>(syntax.Inclusions.Keys);
            Random rnd = new Random();
            
            int num;
            for (int i = 0; i < wordsCount; i++)
            {
                num = rnd.Next(0, words.Count);
                res.Add(words[num]);
            }

            return res;
        }
        public static async Task<List<string>?> GetWordsGlobal(int minLen = 1, int wordsCount = 1)
        {
            List<string> res = new List<string>();
            string externalURL = "http://free-generator.ru/generator.php?action=word&type=0";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    int cycleLimit = 100;
                    int i = 0, j = 0;
                    while (i < wordsCount && j < cycleLimit)
                    {
                        HttpResponseMessage response = await client.GetAsync(externalURL);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        response.EnsureSuccessStatusCode();

                        var responseBody = await response.Content.ReadAsStringAsync();
                        dynamic jsonObject = JsonConvert.DeserializeObject(responseBody);

                        string word = jsonObject?.word?.word;

                        if (word is not null && word.Length >= minLen)
                        {
                            res.Add(word);
                            i++;
                        }
                        j++;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error making API request: {ex.Message}");
                    return null;
                }
            }

            return res;
        }
    }
}
