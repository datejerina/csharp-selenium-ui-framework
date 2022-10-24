using AutomationFW.Common.Helpers.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.Events;
using Serilog;

namespace AutomationFW.Common.Helpers
{
    public class Browser : IBrowser
    {
        private IWebDriver _driver;
        private string _driverStartUrl;
        public string BaseUrl { get; set; }

        public Browser(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public IWebDriver GetDriverForBrowserAndInitialize(string browserType)
        {
            switch (browserType.ToLower())
            {
                case BrowserTypes.CHROME_HEADLESS:
                    // Headless Chrome:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArguments(new List<string>() {
                                                "--silent-launch",
                                                "--no-startup-window",
                                                "no-sandbox",
                                                "headless",});
                    // Headless tests run much faster than Selenium tests, while running in a real browser.
                    // The difference is that, in contrast to normal Selenium tests, Headless tests do not need to wait or interact with the browser UI.
                    // This type of test provides quick success/failure data, it is perfect for quick tests; sanity checks, pull requests, smoke tests and so on.
                    _driver = new ChromeDriver(chromeOptions);
                    Log.Information("The browser being used is Chrome", LogTypes.SETUP);
                    break;

                case BrowserTypes.CHROME:
                    _driver = new ChromeDriver();
                    Log.Information("The browser being used is Chrome", LogTypes.SETUP);
                    break;

                case BrowserTypes.FIREFOX:
                    _driver = new FirefoxDriver();
                    Log.Information("The browser being used is Firefox", LogTypes.SETUP);
                    break;

                case BrowserTypes.EDGE:
                    var edgeOptions = new EdgeOptions();
                    edgeOptions.UseSpecCompliantProtocol = true;
                    _driver = new EdgeDriver(edgeOptions);
                    Log.Information("The browser being used is Microsoft Edge", LogTypes.SETUP);
                    break;

                case BrowserTypes.IE:
                default:
                    InternetExplorerOptions ieOptions = new InternetExplorerOptions();
                    ieOptions.IgnoreZoomLevel = true;
                    _driver = new InternetExplorerDriver(ieOptions);
                    Log.Information("The browser being used is Internet Explorer", LogTypes.SETUP);
                    break;
            }

            _driver = SetupDriverEventListener(_driver);
            _driver.Manage().Window.Maximize();
            _driverStartUrl = _driver.Url;
            return _driver;
        }

        public IWebDriver SetupDriverEventListener(IWebDriver driver)
        {
            EventFiringWebDriver firingDriver = new EventFiringWebDriver(driver);
            firingDriver.ElementClicking += new EventHandler<WebElementEventArgs>(firingDriver_ElementClicking);
            firingDriver.Navigating += new EventHandler<WebDriverNavigationEventArgs>(firingDriver_Navigating);

            firingDriver.ElementValueChanging += new EventHandler<WebElementValueEventArgs>(
                firingDriver_ElementValueChanging);

            firingDriver.FindingElement += new EventHandler<FindElementEventArgs>(firingDriver_FindingElement);

            firingDriver.ExceptionThrown += new EventHandler<WebDriverExceptionEventArgs>(
                firingDriver_ExceptionThrown);

            return firingDriver;
        }

        public void GoToDriverStartPage()
        {
            _driver.Navigate().GoToUrl(_driverStartUrl);
            new Waiter(_driver).WaitForPageLoad();
        }

        public void GoToHomePage()
        {
            try
            {
                _driver.Navigate().GoToUrl(BaseUrl);
            }
            catch (Exception e)
            {
                Log.Error($"Exception was thrown navigating to the home page. Message: {e.Message}", LogTypes.SETUP);
            }
        }

        private void firingDriver_ElementClicking(object sender, WebElementEventArgs e)
        {
            if (e.Element.Text == "")
            {
                Log.Information($"Clicked on element with value {e.Element.GetAttribute("value")}", LogTypes.SELENIUM);
            }
        }

        private void firingDriver_Navigating(object sender, WebDriverNavigationEventArgs e)
        {
            if (!e.Url.Contains("localhost"))
            {
                Log.Information($"Navigated to url: {e.Url}", LogTypes.SELENIUM);
            }
        }

        private void firingDriver_ElementValueChanging(object sender, WebElementValueEventArgs e)
        {
            Log.Information($"Value '{e.Value}' inserted into element '{e.Element.GetAttribute("id")}'", LogTypes.SELENIUM);
        }

        private void firingDriver_FindingElement(object sender, FindElementEventArgs e)
        {
            if (!e.FindMethod.ToString().Contains("blockUI"))
            {
                Log.Information($"Selenium is finding element {e.FindMethod}", LogTypes.SELENIUM);
            }
        }

        private void firingDriver_ExceptionThrown(object sender, WebDriverExceptionEventArgs e)
        {
            Log.Information($"Selenium threw an exception. Message: {e.ThrownException.Message}", LogTypes.SELENIUM);
        }
    }
}