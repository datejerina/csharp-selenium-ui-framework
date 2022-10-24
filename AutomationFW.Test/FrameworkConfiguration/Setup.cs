using AutomationFW.Common.Helpers;
using BoDi;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Serilog;
using System.Reflection;
using TechTalk.SpecFlow;

namespace AutomationFW.Test.FrameworkConfiguration
{
    [Binding]
    public class Setup
    {
        public static string defaultEnvironment = TestingEnvironments.PREPROD;
        public static string defaultBrowser = BrowserTypes.CHROME;

        // Enables or disables ExtentReports report generation: true or false
        public static bool defaultReport = false;

        public static readonly TimeSpan returnToHomepageTimer = new TimeSpan(0, 4, 0);

        private FWScenarioDataContext _scenarioDataContext;
        private static FWTestRunContext _testRunContext;
        private readonly IObjectContainer _objectContainer;
        private string? _screenshotFilepath;
        private static DateTime _timeoutTimer = DateTime.Now;

        public Setup(IObjectContainer container)
        {
            _objectContainer = container;
        }

        //Order -30000 means this will run before other BeforeTestRun hook (default is 10000)
        [BeforeTestRun(Order = -30000)]
        public static void BeforeTestRunInitialization()
        {
            Logger.Configure();

            // The values that exist in the appSettings.json file are captured
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
                .AddJsonFile("appSettings.json");
            // A build of the attributes is made and sent as a configuration.
            IConfigurationRoot configuration = builder.Build();
            // Overwrite default configuration from appSetting.json file
            SetConfiguration(configuration);

            try
            {
                _testRunContext = new FWTestRunContext(
                    defaultEnvironment,
                    defaultBrowser);
            }
            catch (Exception e)
            {
                Log.Fatal($"Exception thrown while initializing test run context: {e.Message}", LogTypes.SETUP);
                throw;
            }
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            // The report and its settings are initialized
            if (defaultReport) ReportHelper.ReportConfigure();
            var testLaunchName = Path.GetFileName(Assembly.GetExecutingAssembly().Location).ToString();
            Log.Information($"Test launch '{testLaunchName}' has started", LogTypes.SETUP);
            // Uncomment the next line once you have the class ValidatorOptions added
            //ValidatorOptions.InitializeValidationChecksForValidator(_testRunContext.Driver);
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext currentScenario, FeatureContext currentFeature)
        {
            //Create a fresh ScenarioDataContext for the new scenairo.
            _scenarioDataContext = new FWScenarioDataContext();
            ScreenshotHandler.ScreenshotCounter = 1;

            _objectContainer.RegisterInstanceAs(_testRunContext);
            _objectContainer.RegisterInstanceAs(_scenarioDataContext);

            AvoidTimeout();

            //update scenario title if it is a scenario outline
            string scenarioTitle = Logger.AdjustScenarioTitleForScenarioOutlines(currentScenario.ScenarioInfo.Title);
            var categories = currentScenario.ScenarioInfo.Tags.Concat(currentFeature.FeatureInfo.Tags).ToArray();
            PreventRegressionTestsInProd(scenarioTitle, categories);
            PreventTestsInGivenEnvironmentByTag(scenarioTitle, categories);
            EnsureSmokeOrRegressionTagExists(scenarioTitle, categories);
            //skipWipTests(scenarioTitle, categories);

            Log.Information($"Scenario '{scenarioTitle}' setup is complete", LogTypes.SETUP);
            Log.Information($"Scenario '{scenarioTitle}' has begun", LogTypes.SETUP);

            //Create dynamic scenario name - ExtentReport
            // If we need the test labels name, replace currentScenario.ScenarioInfo.Title > TestContext.CurrentContext.Test.Arguments[0].ToString()
            if (defaultReport) ReportHelper.ConfigureFeatureNode(TestContext.CurrentContext.Test.Arguments.Length > 0
                ? TestContext.CurrentContext.Test.Arguments[0].ToString() : currentScenario.ScenarioInfo.Title);
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext currentFeature)
        {
            //Create dynamic feature name
            if (defaultReport) ReportHelper.ConfigureFeatureTestName(currentFeature.FeatureInfo.Title);
        }

        [AfterStep]
        public void AfterStep(ScenarioContext currentScenario)
        {
            if (defaultReport)
            {
                // Base64 screenshots for the report
                var mediaEntity = _testRunContext.PrintScreenAndReturnModel();
                // The parameterization for the generation of steps in the report is sent.
                ReportHelper.ReportOrchestration(currentScenario,
                                                 currentScenario.TestError,
                                                 ScenarioStepContext.Current.StepInfo.StepDefinitionType.ToString(),
                                                 ScenarioStepContext.Current.StepInfo.Text,
                                                 mediaEntity);
            }
            CaptureScreenshotIfStepFailed(currentScenario);
        }

        [AfterScenario]
        public void AfterScenario(ScenarioContext currentScenario, FeatureContext currentFeature)
        {
            // Update scenario title if it is a scenario outline.
            string scenarioTitle = Logger.AdjustScenarioTitleForScenarioOutlines(currentScenario.ScenarioInfo.Title);

            Log.Information(
                $"Scenario '{scenarioTitle}' has finished with status '{currentScenario.ScenarioExecutionStatus}'",
                LogTypes.SETUP);

            // record result in test result csv file
            Logger.LogTestResultInCsvFile(
                currentFeature.FeatureInfo.Title,
                scenarioTitle,
                TestContext.CurrentContext.Result.Outcome.Status.ToString(),
                _scenarioDataContext,
                currentScenario?.TestError?.Message);

            CloseSurplusBrowserWindows();
            Log.Information($"Scenario '{scenarioTitle}' tear down is complete", LogTypes.SETUP);
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            // Flush report once test completes
            if (defaultReport) ReportHelper.CloseAndFlushReport();
            var testLaunchName = Path.GetFileName(Assembly.GetExecutingAssembly().Location).ToString();
            Log.Information($"Test launch '{testLaunchName}' has completed successfully.", LogTypes.SETUP);
            Log.Information($"Test launch '{testLaunchName}' tear down started", LogTypes.SETUP);

            DisposeDbConnections();
            CloseSeleniumDriver();

            Log.Information($"Test launch '{testLaunchName}' tear down finished." + Environment.NewLine, LogTypes.SETUP);
            Log.CloseAndFlush();
        }

        private static void SetConfiguration(IConfigurationRoot configuration)
        {
            string browserSetting = configuration.GetSection("appSettings")["BrowserType"];
            string environmentSetting = configuration.GetSection("appSettings")["Environment"];
            string reporterSetting = configuration.GetSection("appSettings")["Reporter"];

            if (!string.IsNullOrEmpty(browserSetting))
            {
                defaultBrowser = browserSetting.ToLower();
                Log.Information($"Load browser configuration value: '{defaultBrowser}' from appSettings.json");
            }
            if (!string.IsNullOrEmpty(reporterSetting))
            {
                defaultReport = Convert.ToBoolean(reporterSetting);
                Log.Information($"Load Report configuration value: '{defaultReport}' from appSettings.json");
            }
            if (!string.IsNullOrEmpty(environmentSetting))
            {
                defaultEnvironment = environmentSetting.ToLower();
                Log.Information($"Load Environment configuration value: '{defaultEnvironment}' from appSettings.json");
            }
        }

        private static void AvoidTimeout()
        {
            // Returns to home page every 4 minutes to avoid timeouts.
            if (_timeoutTimer.Add(returnToHomepageTimer).CompareTo(DateTime.Now) < 0)
            {
                _testRunContext.Browser.GoToHomePage();
                _timeoutTimer = DateTime.Now;
            }
            _testRunContext.Browser.GoToDriverStartPage();
        }

        private static void PreventRegressionTestsInProd(string scenarioTitle, string[] categories)
        {
            // Extra safety check for running in prod environment, skips regression tests
            if (_testRunContext.Environment.ToLower() == "prod"
                && categories.Contains(StandardScenarioTags.REGRESSION))
            {
                Log.Information(
                    "Regression Test Scenario: '" + scenarioTitle + "' cannot be run in prod environment.",
                    LogTypes.SETUP);

                Assert.Ignore("Regression Test Scenario: " + scenarioTitle + " cannot be run in prod environment.");
            }
        }

        private static void PreventTestsInGivenEnvironmentByTag(string scenarioTitle, string[] categories)
        {
            if (categories.Contains($"no_{_testRunContext.Environment.ToLower()}"))
            {
                Log.Information(
                    $"Test Scenario: '{scenarioTitle}' cannot be run in " +
                        $"'{_testRunContext.Environment.ToLower()}' environment due to tagging.",
                    LogTypes.SETUP);

                Assert.Ignore($"Cannot run this test on {_testRunContext.Environment.ToLower()} " +
                    $"because it contains the 'no_{_testRunContext.Environment.ToLower()}' tag.");
            }
        }

        private static void EnsureSmokeOrRegressionTagExists(string scenarioTitle, string[] categories)
        {
            // Extra check to ensure either the @smoke or @regression tag is used
            if (!categories.Contains(StandardScenarioTags.SMOKE)
                && !categories.Contains(StandardScenarioTags.REGRESSION))
            {
                Log.Information($"Test Scenario: '{scenarioTitle}' is missing the @smoke or @regression tag.  " +
                    $"Skipping test.", LogTypes.SETUP);

                Assert.Ignore($"Ignoring this scenario because it contains neither a " +
                    $"@{StandardScenarioTags.SMOKE} nor @{StandardScenarioTags.REGRESSION} tag.  " +
                    $"Please add the correct tag to this scenario's feature file.");
            }

            if (categories.Contains(StandardScenarioTags.SMOKE)
                && categories.Contains(StandardScenarioTags.REGRESSION))
            {
                Log.Information($"Test Scenario: '{scenarioTitle}' contains both @{StandardScenarioTags.SMOKE} " +
                    $"and @{StandardScenarioTags.REGRESSION} tags.  Skipping test.", LogTypes.SETUP);

                Assert.Ignore($"Ignoring this scenario because it contains both @{StandardScenarioTags.SMOKE} " +
                    $"and @{StandardScenarioTags.REGRESSION} tag.  Only one of these tags must be used.");
            }
        }

        //private void skipWipTests(string scenarioTitle, string[] categories)
        //{
        //    //TODO: Modify to apply check for preprod/prod only
        //    if (categories.Contains("wip") || categories.Contains("not_scheduled"))
        //    {
        //        Log.Information(
        //            $"Test Scenario: '{scenarioTitle}' has the @wip tag and will be skipped.",
        //            LogTypes.SETUP);

        //        Assert.Ignore($"Cannot run this test on {_testRunContext.Environment.ToLower()} " +
        //                      $"because it contains the 'wip' tag.");
        //    }
        //}

        private void CaptureScreenshotIfStepFailed(ScenarioContext currentScenario)
        {
            if (currentScenario.TestError != null)
            {
                _screenshotFilepath = ScreenshotHandler.CaptureScreenshot(_testRunContext.Driver, currentScenario, currentScenario.ScenarioInfo.Title.ToString() + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            }
        }

        private static void DisposeDbConnections()
        {
            if (_testRunContext?.OracleConnectionHandler?.Connection != null)
            {
                _testRunContext.OracleConnectionHandler.Connection.Dispose();
            }

            if (_testRunContext?.SqlConnectionHandler?.Connection != null)
            {
                _testRunContext.SqlConnectionHandler.Connection.Dispose();
            }
        }

        private static void CloseSeleniumDriver()
        {
            if (_testRunContext?.Driver != null)
            {
                _testRunContext.Driver.Quit();
                Log.Information("Selenium driver has been closed successfully", LogTypes.SETUP);
                _testRunContext.Driver.Dispose();
                Log.Information("Selenium driver resources have been disposed successfully", LogTypes.SETUP);
            }
        }

        private static void CloseSurplusBrowserWindows()
        {
            if (_testRunContext?.Driver?.WindowHandles.Count > 1)
            {
                var mainWindow = _testRunContext.Driver.WindowHandles.First();

                foreach (var window in _testRunContext.Driver.WindowHandles.Where(x => x != mainWindow))
                {
                    _testRunContext.Driver.SwitchTo().Window(window);
                    _testRunContext.Driver.Close();
                }

                _testRunContext.Driver.SwitchTo().Window(mainWindow);
            }
        }
    }
}