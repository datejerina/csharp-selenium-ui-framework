using AutomationFW.Common.DataAccess;
using AutomationFW.Common.DataModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using Serilog;
using System.Reflection;

namespace AutomationFW.Common.Helpers
{
    public abstract class TestRunContextBase
    {
        public IWebDriver Driver { get; set; }
        public Browser Browser { get; set; }
        public string BrowserType { get; set; }
        public string Environment { get; set; }
        public JObject AccessJson { get; set; }
        public JObject DataJson { get; set; }
        public JObject EnvConfigJson { get; set; }
        public EnvConfig EnvConfig { get; set; }
        public OracleConnectionHandler OracleConnectionHandler { get; set; }
        public SqlConnectionHandler SqlConnectionHandler { get; set; }

        public TestRunContextBase(
            string defaultEnvironment,
            string defaultBrowser,
            CustomFilepaths customFilepaths)
        {
            var testSuiteName = Path.GetFileName(Assembly.GetExecutingAssembly().Location).ToString();
            Log.Information($"Test launch '{testSuiteName}' is being setup...", LogTypes.SETUP);

            //Store custom Filepaths and verify all exist
            FilepathsManager.CustomFilepaths = customFilepaths;
            FilepathsManager.ValidateFilepaths();

            //load command line inputs
            BrowserType = TestContext.Parameters.Get("Browser", defaultBrowser);
            Environment = TestContext.Parameters.Get("Environment", defaultEnvironment);
            Log.Information($"The environment is set to '{Environment}'", LogTypes.SETUP);

            JsonIoHandler.env = Environment;
            Logger.env = Environment;
            //load in all jsons
            AccessJson = JsonIoHandler.GetAccessJObject();
            EnvConfigJson = JsonIoHandler.GetEnvConfigJObject();
            DataJson = JsonIoHandler.GetDataJObject();

            // grab any needed values from EnvConfigJson
            EnvConfig = JsonConvert.DeserializeObject<EnvConfig>(EnvConfigJson?.ToString() ?? "") ?? new EnvConfig();
            Logger.config = EnvConfig.ValidityLoggingConfig;

            //start up selenium driver
            Browser = new Browser(EnvConfigJson.GetValue("baseUrl").ToString());
            Driver = Browser.GetDriverForBrowserAndInitialize(BrowserType);

            OracleConnectionHandler =
                new OracleConnectionHandler(AccessJson?.GetValue("OracleConnectionString") as JObject);

            SqlConnectionHandler = new SqlConnectionHandler(AccessJson?.GetValue("SqlConnectionString") as JObject);
        }

        public string PrintScreenAndReturnModel()
        {
            try
            {
                string screenshot = ((ITakesScreenshot)Driver).GetScreenshot().AsBase64EncodedString;
                return screenshot;
                //return MediaEntityBuilder.CreateScreenCaptureFromBase64String(screenshot, name).Build();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}