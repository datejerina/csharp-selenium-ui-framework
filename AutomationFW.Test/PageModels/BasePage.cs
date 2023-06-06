using AutomationFW.Common.Helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using Serilog;

namespace AutomationFW.Test.PageModels
{
    public abstract class PageBase
    {
        //public abstract string PageTitle { get; set; }
        protected IWebDriver _driver;
        protected Waiter _waiter;

        public PageBase(IWebDriver driver)
        {
            _driver = driver;
            _waiter = new Waiter(_driver);
        }

        public virtual void HandleSSLError()
        {
            var overrideLink = _driver.FindElement(By.Id("overridelink"));
            var exec = (IJavaScriptExecutor)_driver;
            exec.ExecuteScript("arguments[0].click()", overrideLink);
        }

        public virtual void HandlePopupIfAny()
        {
            //** Handling popup for IE Driver
            if (((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("interactive"))
            {
                _waiter.Wait.Until(d => _driver.WindowHandles.Count > 1);

                var windows = _driver.WindowHandles.ToList();
                var mainWindow = _driver.CurrentWindowHandle;

                if (windows.First() == mainWindow)
                {
                    _waiter.Wait.Until(d => _driver.SwitchTo().Window(windows.Last()));
                }
                else
                {
                    _waiter.Wait.Until(d => _driver.SwitchTo().Window(windows.First()));
                }

                _driver.Close();
                _driver.SwitchTo().Window(mainWindow);
            }
        }
        public virtual void SwitchToMainContentFrame()
        {
            _driver.SwitchTo().DefaultContent();
        }

        public virtual void SwitchToFirstWindow()
        {
            var windows = _driver.WindowHandles.ToList();
            _driver.SwitchTo().Window(windows.First());
        }


        public virtual void CloseAlert()
        {
            _driver.SwitchTo().Alert().Dismiss();
        }

        public virtual bool IsAlertPresent()
        {
            try
            {
                _driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException e)
            {
                Log.Error($"Exception thrown: {e.Message}", "Exception");
                return false;
            }
        }
        public virtual void AcceptAlert()
        {
            if (IsAlertPresent())
                _driver.SwitchTo().Alert().Accept();
        }
        public virtual string GetAlertText()
        {
            return _driver.SwitchTo().Alert().Text;
            
        }

        /*        public virtual void AssertCorrectTitle()
                {
                    Assert.That(_driver.Title, Is.EqualTo(PageTitle), "Incorrect page was loaded, Title was incorrect");
                }

                public virtual void AssertCorrectTitle(string title)
                {
                    Assert.That(_driver.Title, Is.EqualTo(title), "Incorrect page was loaded, Title was incorrect");
                }
        */

        protected void AssertAlertAppears(string alertName, string alertText)
        {
            try
            {
                _waiter.Wait.Until(_ => _driver.SwitchTo().Alert());
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail($"No {alertName} alert found.");
            }

            var alert = _driver.SwitchTo().Alert();
            Assert.That(alert?.Text, Does.Contain(alertText), $"{alertName} alert did not contain the required text.");
            alert?.Accept();
            _driver.SwitchTo().ActiveElement();
        }
    }
}
