using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AutomationFW.Common.Helpers.Interfaces
{
    public interface IWaiter
    {
        WebDriverWait Wait { get; }
        WebDriverWait WaitFor(int seconds);
        IWebElement WaitForElement(By locator, int seconds = 20);
        IWebElement WaitForElement(IWebElement webElement, int seconds = 20);
        void WaitForOverlayToDisappear(int seconds = 30);
        void WaitForPageLoad(int seconds = 60);
    }
}