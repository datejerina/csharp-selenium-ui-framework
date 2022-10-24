using AutomationFW.Common.Helpers.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;

namespace AutomationFW.Common.Helpers
{
    public class Waiter : IWaiter
    {
        private IWebDriver _driver;
        public WebDriverWait Wait { get; }

        public Waiter(IWebDriver driver, int timeout = 25)
        {
            _driver = driver;
            Wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeout));
        }

        // waiting for blockUI overlay to disappear ("Please Wait...." Popup)
        public void WaitForOverlayToDisappear(int seconds = 30)
        {
            WaitFor(seconds).Until(d => d.FindElements(By.ClassName("blockUI")).Count == 0);
        }

        // Waits for the specified element to appear, and then returns it.
        public IWebElement WaitForElement(By locator, int seconds = 20)
        {
            return WaitFor(seconds).Until(d => d.FindElement(locator));
        }

        // Waits for the specified element to appear, and then returns it.
        // Overload that takes the element reference instead of a By locator
        public IWebElement WaitForElement(IWebElement webElement, int seconds = 20)
        {
            return WaitFor(seconds).Until(d => webElement);
        }

        // Used to specify the timeout duration for this waiter.
        // Can be chained with .Until to form a custom wait condition.
        public WebDriverWait WaitFor(int seconds)
        {
            Wait.Timeout = TimeSpan.FromSeconds(seconds);
            return Wait;
        }

        // Waits for the page and contained javascript to be marked as finished loading
        public void WaitForPageLoad(int seconds = 60)
        {
            WaitFor(seconds).Until(d =>
            {
                try
                {
                    var status = ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState");
                    return status.Equals("complete") || status.Equals("interactive");
                }
                catch (Exception e)
                {
                    Log.Error(
                        $"Exception was thrown while waiting for page load. Message {e.Message}",
                        LogTypes.SELENIUM);

                    return false;
                }
            });
        }
    }
}