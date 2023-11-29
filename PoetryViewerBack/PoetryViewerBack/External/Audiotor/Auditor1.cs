using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace PoetryViewerBack.External.Audiotor;

public class Auditor1 
{
    private const int charLimit = 500;

    public static List<string> TextInPieces(string text)
    {
        List<string> res = new List<string>();
        int rest;
        string currText;
        for (int i = 0; i < text.Length; i += charLimit)
        {
            rest = (text.Length - 1) - i;
            currText = text.Substring(i, charLimit > rest ? rest : charLimit);
            res.Add(currText);
        }
        return res;
    }

    public static Models.AudioRecordByte GetAudioByPoetry(Models.Poetry p)
    {
        // 1 - devide into 500chars pieces
        // 2 - use GetAudioByText
        // 3 - unite responses
        // 4 - form result
        // 5* - get beat(by link or stored(using DTO))
        // 6* - unite audio with beat
        List<string> texts = TextInPieces(p.Text);
        List<byte[]> audios = new List<byte[]>();
        byte[] currAudio;
        foreach (var text in texts)
        {
            currAudio = GetAudioByText(text);
            if (currAudio is null)
                return null;
            audios.Add(currAudio);
        }

        byte[] resAudio = CombineWavFiles(audios);
        Models.AudioRecordByte res = new Models.AudioRecordByte()
        {
            Author = p.Author,
            PoetryNum = Convert.ToInt32(p.Name),
            AudioData = resAudio
        };

        return res;
    }

    public static byte[]? GetAudioByText(string text)
    {
        if (text.Length > charLimit)
            return null;

        const int startDelay = 10000;
        string link = "https://www.narakeet.com/app/text-to-audio";
        IWebDriver driver = new ChromeDriver();
        Actions actions = new Actions(driver);
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

        try
        {
            driver.Navigate().GoToUrl(link);
            // Authentification delay
            Thread.Sleep(startDelay);

            // Set language and announcer
            try
            {
                string xStrLanguage = "/html/body/main/div[5]/div/div[1]/div[1]/select";
                IWebElement language = wait.Until(driver => driver.FindElement(By.XPath(xStrLanguage)));
                actions.MoveToElement(language).Click().Perform();
                SelectElement selectLanguage = new SelectElement(language);
                selectLanguage.SelectByText("Russian");

                string xStrAnnouncer = "/html/body/main/div[5]/div/div[1]/div[2]/select";
                IWebElement announcer = wait.Until(driver => driver.FindElement(By.XPath(xStrAnnouncer)));
                actions.MoveToElement(announcer).Click().Perform();
                SelectElement selectAnnouncer = new SelectElement(announcer);
                selectAnnouncer.SelectByValue("boris");
            }
            catch (Exception ex)
            {
                throw new Exception($"Choose language and announcer: {ex.Message}");
            }

            // Insert text
            try
            {
                string xStrInputTextArea = "/html/body/main/div[5]/div/div[1]/div[3]/div[6]/div[2]";
                IWebElement inputTextArea = wait.Until(driver => driver.FindElement(By.XPath(xStrInputTextArea)));
                inputTextArea.SendKeys(text);
                wait.Until(ExpectedConditions.TextToBePresentInElement(inputTextArea, text));
            }
            catch (Exception ex)
            {
                throw new Exception($"Insert text: {ex.Message}");
            }

            // Submit
            try
            {
                string xStrSubmitBtn = "/html/body/main/div[5]/div/div[1]/div[6]/button[4]";
                IWebElement submitBtn = wait.Until(driver => driver.FindElement(By.XPath(xStrSubmitBtn)));
                actions.MoveToElement(submitBtn).Click().Perform();
            }
            catch (Exception ex)
            {
                throw new Exception($"Submit: {ex.Message}");
            }

            // Get Audio
            try
            {
                string xStrAudio = "/html/body/main/div[4]/div/div/audio";
                IWebElement audioElement = wait.Until(driver => driver.FindElement(By.XPath(xStrAudio)));
                wait.Until(driver => !string.IsNullOrEmpty(audioElement.GetAttribute("src")));
                string audioSourceUrl = audioElement.GetAttribute("src");
                using (WebClient client = new WebClient())
                { 
                    byte[] audioBytes = client.DownloadData(audioSourceUrl);
                    return audioBytes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Get Audio: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");
        }
        finally
        {
            driver.Close();
            driver.Quit();
        }

        return null;
    }

    public static byte[] CombineWavFiles(List<byte[]> audioDataList)
    {
        int headerSize = BitConverter.ToInt32(audioDataList.First(), 40);
        int totalDataSize = audioDataList.Sum(data => data.Length - headerSize);
        byte[] combinedAudio = new byte[headerSize + totalDataSize];

        Array.Copy(audioDataList.First(), combinedAudio, headerSize);
        int offset = headerSize;
        foreach (byte[] audioData in audioDataList)
        {
            Array.Copy(audioData, headerSize, combinedAudio, offset, audioData.Length - headerSize);
            offset += audioData.Length - headerSize;
        }

        return combinedAudio;
    }

    public static async Task Test()
    {
        string test1 = "Здравствуй, Марк!";
        string test2 = "Иглз преуменьшает значение дебатов о выживании\r\nИнтегралы понимают, что Марк добьет хозяина\r\nРакетный штамп психологии взаимодействия капитала\r\nЯ хочу открытой смерти Марка\r\nНадежный фирменный экран надувается\r\nЖивые реликвии и знак обмена\r\nСдавайся, Марк, сдавайся.\r\nОтпразднуйте День ПВО: безопасный вызов\r\nПораженный, Марк заметил\r\nОставьте безумный украденный повтор Марка.\r\nОтметка сохранения навыка Win Wet Skill\r\nОбратите внимание на рисунок знака Акционерной Армии.";
        string test3 = "Иглз преуменьшает значение дебатов о выживании\r\nИнтегралы понимают, что Марк добьет хозяина\r\nРакетный штамп психологии взаимодействия капитала\r\nЯ хочу открытой смерти Марка\r\nНадежный фирменный экран надувается\r\nЖивые реликвии и знак обмена\r\nСдавайся, Марк, сдавайся.\r\nОтпразднуйте День ПВО: безопасный вызов\r\nПораженный, Марк заметил\r\nОставьте безумный украденный повтор Марка.\r\nОтметка сохранения навыка Win Wet Skill\r\nОбратите внимание на рисунок знака Акционерной Армии.\r\nФрагменты из зоны победы Марк санитара\r\nНожки педалей я заказал у Grey Mark.\r\nЖелаю тебе универсального громкоговорителя Марк\r\nЛишен возможности видеть автоматически открывающуюся Марку";
        PoetryViewerBack.Models.Poetry p = new PoetryViewerBack.Models.Poetry() { Author = "I", Name = "1", Text = test3 };
        PoetryViewerBack.Models.AudioRecordByte? rec = Auditor1.GetAudioByPoetry(p);
        if (rec is null)
            Console.WriteLine("---------nullllllllllllll---------");
        else
            Console.WriteLine($"---------res: {await PoetryViewerBack.DTO.AudioRecord.CreateAudio(rec)}---------");
    }
}
