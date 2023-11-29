using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.IO;

namespace PoetryViewerBack.External.Audiotor
{
    public class Auditor2
    {
        public static string ErrorMessage = "";
        public static Models.AudioRecordByte? GetAudioByPoetry(Models.Poetry p)
        {
            byte[] resAudio = GetAudioByText(p.Text);

            if (resAudio is null)
            {
                string error = ErrorMessage;
                ErrorMessage = "";
                return new Models.AudioRecordByte()
                {
                    Author = error,
                    PoetryNum = -1,
                    AudioData = new byte[] {1,2,3}
                };
            }


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
            const int startDelay = 3000;
            const int insertDelay = 2000;
            string link = "https://voxworker.com/ru";
            string xStr;

            ChromeOptions options = new ChromeOptions();
            string downloadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            options.AddUserProfilePreference("download.default_directory", downloadDirectory);
            IWebDriver driver = new ChromeDriver(options);
            Actions actions = new Actions(driver);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            try
            {
                driver.Navigate().GoToUrl(link);
                Thread.Sleep(startDelay);

                // Set voice / speed / pitch 
                try
                {
                    xStr = "/html/body/div[1]/div[2]/div[1]/div[2]/div[1]/form/ul[1]/li[1]/div/div[2]/b";
                    IWebElement voiceSelect = GetElement(xStr);
                    ClickElement(voiceSelect);
                    xStr = "/html/body/div[1]/div[2]/div[1]/div[2]/div[1]/form/ul[1]/li[1]/div/div[3]/div/ul/ul[2]/li[3]"; // MIHAIL
                    IWebElement voiceOption= GetElement(xStr);
                    ClickElement(voiceOption);

                    xStr = "/html/body/div[1]/div[2]/div[1]/div[2]/div[1]/form/ul[1]/li[2]/div/div[2]/b";
                    IWebElement speedSelect = GetElement(xStr);
                    ClickElement(speedSelect);
                    xStr = "/html/body/div[1]/div[2]/div[1]/div[2]/div[1]/form/ul[1]/li[2]/div/div[3]/div/ul/li[6]"; // Standart speed
                    IWebElement speedOption = GetElement(xStr);
                    ClickElement(speedOption);

                    xStr = "/html/body/div[1]/div[2]/div[1]/div[2]/div[1]/form/ul[1]/li[3]/div/div[2]/b";
                    IWebElement pitchSelect = GetElement(xStr);
                    ClickElement(pitchSelect);
                    xStr = "/html/body/div[1]/div[2]/div[1]/div[2]/div[1]/form/ul[1]/li[3]/div/div[3]/div/ul/li[10]"; // -0.9 speed
                    IWebElement pitchOption = GetElement(xStr);
                    ClickElement(pitchOption);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Setting voice / speed / pitch : {ex.Message}");
                }

                // Insert text
                try
                {
                    xStr = "/html/body/div/div[2]/div[1]/div[2]/div[1]/form/textarea";
                    IWebElement inputTextArea = GetElement(xStr);
                    inputTextArea.Clear();
                    inputTextArea.SendKeys(text);
                    Thread.Sleep(insertDelay);
                    //wait.Until(ExpectedConditions.TextToBePresentInElement(inputTextArea, text));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Insert text: {ex.Message}");
                }

                // Submit and get audio
                try
                {
                    xStr = "/html/body/div[1]/div[2]/div[1]/div[2]/div[1]/form/ul[2]/li[3]/button";
                    IWebElement submitBtn = GetElement(xStr);
                    ClickElement(submitBtn);
                    wait.Until(d => Directory.GetFiles(downloadDirectory, "*.*").Length > 0);

                    // find filepath of downloaded file
                    string filePath;
                    try
                    {
                        filePath = Directory.GetFiles(downloadDirectory, "*.mp3")[0];
                    }
                    catch(Exception ex)
                    {
                        throw new Exception($"There no .mp3 file in temp folder {ex.Message}");
                    }

                    // Read downloaded file as byte[]
                    byte[] res;
                    try
                    {
                        res = File.ReadAllBytes(filePath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Read .mp3 file error: {ex.Message}");
                    }

                    // try to delete downloaded file
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                    catch(Exception ex) 
                    {
                        throw new Exception($"Delete error: {ex.Message}");
                    }


                    return res;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Submit and get audio: {ex.Message}");
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error message: {ex.Message}";
                Console.WriteLine(ErrorMessage);
            }
            finally
            {
                driver.Close();
                driver.Quit();
            }

            return null;


            IWebElement GetElement(string xStr)
            {
                return wait.Until(driver => driver.FindElement(By.XPath(xStr)));
            }
            void ClickElement(IWebElement el)
            {
                actions.MoveToElement(el).Click().Perform();
            }
        }

        public static async Task Test()
        {
            string test1 = "Здравствуй, Марк!";
            string test2 = "Иглз преуменьшает значение дебатов о выживании\r\nИнтегралы понимают, что Марк добьет хозяина\r\nРакетный штамп психологии взаимодействия капитала\r\nЯ хочу открытой смерти Марка\r\nНадежный фирменный экран надувается\r\nЖивые реликвии и знак обмена\r\nСдавайся, Марк, сдавайся.\r\nОтпразднуйте День ПВО: безопасный вызов\r\nПораженный, Марк заметил\r\nОставьте безумный украденный повтор Марка.\r\nОтметка сохранения навыка Win Wet Skill\r\nОбратите внимание на рисунок знака Акционерной Армии.\r\nФрагменты из зоны победы Марк санитара\r\nНожки педалей я заказал у Grey Mark.\r\nЖелаю тебе универсального громкоговорителя Марк\r\nЛишен возможности видеть автоматически открывающуюся Марку";
            string test3 = "Будет ли труп звездного Марка крадет\r\nОтметьте, что вы заморозете разные темы\r\nКоллега, чтобы поставить небесный знак, чтобы вытащить\r\nЭлектрика живота святого живота для набора отметки\r\nНебо изучать Рой Марк изгнан\r\nМарк звезд с ремесленным экстремумом умер\r\nНайдите песню той же отметкой, чтобы остановиться\r\nОтметьте апелляцию памяти причиной трубы\r\nПоле с грязью нависающей сыпью.\r\nРада написать спящую отметку для кислых\r\nОбразованная народная марка теряет цифру\r\nОтметьте, чтобы опубликовать сокращение телевидения\r\nМарк вокруг плеч Мрачное существо\r\nОчевидно для другого Бога\r\nНепрерывная температура губы грустная марка\r\nМарк позвольте зубу объединить\r\nМарк получил какие -либо заметки\r\nСимволы тела отдаленного состояния маркировки\r\nТруд много органного знака\r\nЗаконодательство, чтобы намереваться намерение гармонично\r\nТри марки лестницы\r\nОхватывает гордый маркировок местный\r\nПодписать глупую отметку, чтобы попробовать хлеб\r\nСпросите Von Mark Толстый оборот\r\nЛедяные скалы я разбегаю Марк Рамам\r\nЭкстремальная десяти пива\r\nПочувствовать себя президентским знаком\r\nМарк включите конец, чтобы завершить гибкость\r\nДодорожная сторона вышла на деморализованном Марке.\r\nЧтобы разорвать мертвую оценку, вспоминая скольжение\r\nВозбудить изображение часто\r\nПоддержка радостного резкого знака достоинства";
            PoetryViewerBack.Models.Poetry p = new PoetryViewerBack.Models.Poetry() { Author = "I", Name = "1", Text = test3 };
            PoetryViewerBack.Models.AudioRecordByte? rec = GetAudioByPoetry(p);
            
            if (rec is null)
                Console.WriteLine("---------nullllllllllllll---------");
            else
                Console.WriteLine($"---------res: {await PoetryViewerBack.DTO.AudioRecord.CreateAudio(rec)}---------");

        }
    }
}
