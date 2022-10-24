using OpenQA.Selenium;

namespace AutomationFW.Common.Helpers.Interfaces
{
    public interface IBrowser
    {
        string BaseUrl { get; set; }

        IWebDriver GetDriverForBrowserAndInitialize(string browserType);
        void GoToDriverStartPage();
        void GoToHomePage();
        IWebDriver SetupDriverEventListener(IWebDriver driver);
    }
}