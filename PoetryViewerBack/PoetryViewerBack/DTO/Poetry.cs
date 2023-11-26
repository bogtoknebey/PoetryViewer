namespace PoetryViewerBack.DTO;
using PoetryViewerBack.Models;
public static class Poetry
{
    public static List<string> GetAuthorsList()
    {
        List<string> res = new List<string>();
        try
        {
            var dirs = Directory.GetDirectories("data");
            foreach (var dir in dirs)
                res.Add(Path.GetFileName(dir));
        }
        catch 
        {
            res.Add("Something went wrong.");
        }
        return res;
    }

    public static string GetNextFilePath(string path, string extention, string type)
    {
        string[] subdirectories = {};
        if (type == "directory")
        {
            subdirectories = Directory.GetDirectories(path);
        }
        else if (type == "file")
        {
            subdirectories = Directory.GetFiles(path);
        }

        int highestNumber = 0;
        if (subdirectories.Length > 0)
        {
            highestNumber = subdirectories
                .Select(dir => Path.GetFileNameWithoutExtension(dir))
                .Select(name => int.TryParse(name, out int number) ? number : 0)
                .Max();
        }
        highestNumber++;

        return path + $"/{highestNumber}.{extention}";
    }

    public static int GetPoetryCount(string author)
    {
        string path = $"data/{author}";
        return Directory.GetDirectories(path).Length;
    }

    public static Models.Poetry? GetRandomPoetry(string author)
    {
        string dir = $"data/{author}";
        if (!Directory.Exists(dir)) return null;

        var subdirectories = Directory.GetDirectories(dir);
        if(subdirectories.Length == 0) return null;

        int highestNumber = subdirectories
            .Select(dir => int.TryParse(new DirectoryInfo(dir).Name, out int number) ? number : 0)
            .Max();

        Random random = new Random();
        return GetPoetry(author, random.Next(1, highestNumber + 1));
    }

    public static Models.Poetry? GetPoetry(string author, int number)
    {
        string directoryPath = $"data/{author}/{number}";
        if (!Directory.Exists(directoryPath)) return null;

        string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
        if (textFiles.Length != 1) return null;

        Models.Poetry res = new Models.Poetry() 
        { 
            Author = author, 
            Name = number.ToString(), 
            Text = File.ReadAllText(textFiles[0]) 
        };
        return res;
    }

    public static CreatePoetryResponse CreatePoetry(string author, string text)
    {
        string dir = $"data/{author}";
        if(!Directory.Exists(dir)) return new CreatePoetryResponse(true, "Failed to add, empty text.");

        var subdirectories = Directory.GetDirectories(dir);
        int highestNumber = 0;
        if (subdirectories.Length != 0) 
        {
            highestNumber = subdirectories
                .Select(dir => int.TryParse(new DirectoryInfo(dir).Name, out int number) ? number : 0)
                .Max();
        }
        highestNumber++;

        dir += $"/{highestNumber}";
        Directory.CreateDirectory(dir);
        Directory.CreateDirectory(dir + "/audio");

        File.WriteAllText(dir + $"/{highestNumber}.txt", text);
        return new CreatePoetryResponse(false, $"Your poetry have been added with {highestNumber} number", $"{highestNumber}");
    }

    public static bool UpdatePoetry(Models.Poetry p)
    {
        string path = $"data/{p.Author}/{p.Name}";
        if (!Directory.Exists(path)) return false;

        string[] textFiles = Directory.GetFiles(path, "*.txt");
        if (textFiles.Length != 1) return false;

        try
        {
            File.WriteAllText(textFiles[0], p.Text);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public static bool DeletePoetry(Models.Poetry p)
    {
        string path = $"data/{p.Author}/{p.Name}";
        if (!Directory.Exists(path)) return false;

        try
        {
            Directory.Delete(path, true);
        }
        catch
        {
            return false;
        }
        return true;
    }
}
