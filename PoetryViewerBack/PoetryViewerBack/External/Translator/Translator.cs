namespace PoetryViewerBack.External.Translator;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static System.Net.WebRequestMethods;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public static class Translator
{
    public static string GetTranslate(string text, int switchTimes)
    {
        const int smallDelay = 500;
        const int largeDelay = 1500;
        string res = "";
        string errorStack = "";
        string link = "https://translate.google.com.ua/?sl=ru&tl=en";
        IWebDriver driver = new ChromeDriver();
        Actions actions = new Actions(driver);
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        try
        {
            driver.Navigate().GoToUrl(link);

            // Concent
            try
            {
                string xStrConsentBtn = "/html/body/c-wiz/div/div/div/div[2]/div[1]/div[3]/div[1]/div[1]/form[2]/div/div/button/div[3]";
                IWebElement consentBtn = wait.Until(driver => driver.FindElement(By.XPath(xStrConsentBtn)));
                actions.MoveToElement(consentBtn).Click().Perform();
            }
            catch
            {
                errorStack += "concent";
            }

            // Find input text area
            IWebElement inputTextArea = driver.FindElement(By.XPath("/html"));
            try
            {
                string xStrInputTextArea = "/html/body/c-wiz/div/div[2]/c-wiz/div[2]/c-wiz/div[1]/div[2]/div[3]/c-wiz[1]/span/span/div/textarea";
                inputTextArea = wait.Until(driver => driver.FindElement(By.XPath(xStrInputTextArea)));
                // Pass input text
                inputTextArea.SendKeys(text);
                wait.Until(ExpectedConditions.TextToBePresentInElementValue(inputTextArea, text));
            }
            catch
            {
                errorStack += "Find input text area OR Pass input text";
            }

            //// Setting languages(using link parameters for it)
            //try
            //{
            //    string xStrInputArrow = "/html/body/c-wiz/div/div[2]/c-wiz/div[2]/c-wiz/div[1]/div[1]/c-wiz/div[1]/c-wiz/div[2]/button/div[3]";
            //    IWebElement inputArrow = wait.Until(driver => driver.FindElement(By.XPath(xStrInputArrow)));
            //    actions.MoveToElement(inputArrow).Click().Perform();
            //    string xStrInputLanguage = "/html/body/c-wiz/div/div[2]/c-wiz/div[2]/c-wiz/div[1]/div[1]/c-wiz/div[2]/c-wiz/div[1]/div/div[3]/div/div[3]/div[89]/div[2]";
            //    IWebElement inputLanguage = wait.Until(driver => driver.FindElement(By.XPath(xStrInputLanguage)));
            //    actions.MoveToElement(inputLanguage).Click().Perform();


            //    string xStrOutputArrow = "/html/body/c-wiz/div/div[2]/c-wiz/div[2]/c-wiz/div[1]/div[1]/c-wiz/div[1]/c-wiz/div[5]/button/div[3]";
            //    IWebElement outputArrow = wait.Until(driver => driver.FindElement(By.XPath(xStrOutputArrow)));
            //    actions.MoveToElement(outputArrow).Click().Perform();
            //    string xStrOutputLanguage = "/html/body/c-wiz/div/div[2]/c-wiz/div[2]/c-wiz/div[1]/div[1]/c-wiz/div[2]/c-wiz/div[2]/div/div[3]/div/div[2]/div[6]/div[2]";
            //    IWebElement outputLanguage = wait.Until(driver => driver.FindElement(By.XPath(xStrOutputLanguage)));
            //    actions.MoveToElement(outputLanguage).Click().Perform();
            //}
            //catch
            //{
            //    errorStack += "Find input text area OR Pass input text";
            //}

            //Thread.Sleep(largeDelay);
            // Switch
            try
            {
                string xStrSwitchBtn = "/html/body/c-wiz/div/div[2]/c-wiz/div[2]/c-wiz/div[1]/div[1]/c-wiz/div[1]/c-wiz/div[3]/div/span/button";
                IWebElement switchBtn = wait.Until(driver => driver.FindElement(By.XPath(xStrSwitchBtn)));
                for (int i = 0; i < switchTimes; i++)
                {
                    Thread.Sleep(smallDelay);
                    actions.MoveToElement(switchBtn).Click().Perform();
                }
            }
            catch
            {
                errorStack += "switch";
            }

            Thread.Sleep(largeDelay);
            // Getting output
            res = inputTextArea.GetAttribute("value");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error message: {ex.Message}, errorStack: {errorStack}");
            res = $"{ex.Message}";
        }
        finally
        {
            driver.Close();
            driver.Quit();
        }


        return res;
    }

    public static void Test()
    {
        string inText = "Становился шарообразном предупреждал маркатрона монолитным\r\nОбыкновенный комплексный полагаться воспользоваться республика \r\nШуруповёрт шарообразном предисловие инструмента заползающая\r\nУправление религиозный предлагать генетический бесполезный \r\nфантастический подниматься принципиальный достоинство любопытный\r\nотделаешься благодарен монолитным шарообразном предупреждал\r\nВнимательный выдающийся железнодорожный квадратный изображать ";
        Console.WriteLine($"---Translator Input: {inText}---");
        DateTime start = DateTime.Now;
        string outText = Translator.GetTranslate(inText, 6);
        DateTime finish = DateTime.Now;
        Console.WriteLine($"---Translator Output: {outText}---");
        Console.WriteLine($"---Operation time: {(start - finish).TotalSeconds}---");
    }
}
