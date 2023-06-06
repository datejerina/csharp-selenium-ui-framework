using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.MarkupUtils;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using System.Reflection;
using TechTalk.SpecFlow;

namespace AutomationFW.Common.Helpers
{
    public static class ReportHelper
    {
        public static readonly string defaultEnvironment = TestingEnvironments.PREPROD;
        public static ExtentTest featureName;
        public static ExtentTest scenario;
        public static ExtentReports extent;
        public static string ReportName = "Framework-Reports " + DateTime.Now.ToString("dd-MM-yyyy hh.mm.ss") + ".html";
        public static string ReportPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Reports\\");

        public static void ReportConfigure()
        {
            //ExtentReport
            //Initialize Extent report before test starts
            var htmlReporter = new ExtentHtmlReporter(ReportPath + ReportName);
            htmlReporter.Config.DocumentTitle = "Automation Testing Report - Testing";
            htmlReporter.Config.ReportName = "Functional Testing";
            htmlReporter.Config.Theme = Theme.Standard;
            //Attach report to reporter
            extent = new ExtentReports();
            extent.AddSystemInfo("Application Under Test", "demoblaze - PRODUCT STORE");
            extent.AddSystemInfo("Environment", defaultEnvironment);
            extent.AddSystemInfo("Machine", Environment.MachineName);
            extent.AttachReporter(htmlReporter);
            extent.AddSystemInfo("OS", Environment.OSVersion.VersionString);
        }

        public static void ConfigureFeatureNode(string scenarioTitle)
        {
            scenario = featureName.CreateNode<Scenario>(scenarioTitle);
        }

        public static void ConfigureFeatureTestName(string featureTitle)
        {
            featureName = extent.CreateTest<Feature>(featureTitle);
        }

        public static void ReportOrchestration(ScenarioContext currentScenario, Exception currentScenarioTestError, string scenarioStepContextType, string scenarioStepContextInfo, string mediaEntity)
        {
            //var mediaEntity = .PrintScreenAndReturnModel(currentScenario.ScenarioInfo.Title.Trim());
            PropertyInfo pInfo = typeof(ScenarioContext).GetProperty("ScenarioExecutionStatus", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo getter = pInfo.GetGetMethod(nonPublic: true);
            object TestResult = getter.Invoke(currentScenario, null);

            if (currentScenarioTestError == null)
            {
                if (scenarioStepContextType == "Given")
                    scenario.CreateNode<Given>(scenarioStepContextInfo);
                else if (scenarioStepContextType == "When")
                    scenario.CreateNode<When>(scenarioStepContextInfo);
                else if (scenarioStepContextType == "Then")
                    scenario.CreateNode<Then>(scenarioStepContextInfo);
                else if (scenarioStepContextType == "And")
                    scenario.CreateNode<And>(scenarioStepContextInfo);
            }
            else if (currentScenarioTestError != null)
            {
                // If we want to insert an HTML Tag, use the following line to insert
                // image, log or any other additional information
                // string mediaHTMLFormatter = $"<a href='data:image/jpeg;base64,{mediaEntity}' target='_blank')' >";
                if (scenarioStepContextType == "Given")
                {
                    scenario.CreateNode<Given>(scenarioStepContextInfo)
                            .Fail(MarkupHelper.CreateLabel("Test case FAILED due to below issues: " + currentScenarioTestError.Message, ExtentColor.Red).GetMarkup().ToString(), MediaEntityBuilder.CreateScreenCaptureFromBase64String(mediaEntity).Build());
                }
                else if (scenarioStepContextType == "When")
                {
                    scenario.CreateNode<When>(scenarioStepContextInfo)
                            .Fail(MarkupHelper.CreateLabel("Test case FAILED due to below issues: " + currentScenarioTestError.Message, ExtentColor.Red).GetMarkup().ToString(), MediaEntityBuilder.CreateScreenCaptureFromBase64String(mediaEntity).Build());
                }
                else if (scenarioStepContextType == "Then")
                {
                    scenario.CreateNode<Then>(scenarioStepContextInfo)
                            .Fail(MarkupHelper.CreateLabel("Test case FAILED due to below issues: " + currentScenarioTestError.Message, ExtentColor.Red).GetMarkup().ToString(), MediaEntityBuilder.CreateScreenCaptureFromBase64String(mediaEntity).Build());
                }
                else if (scenarioStepContextType == "And")
                {
                    scenario.CreateNode<And>(scenarioStepContextInfo)
                            .Fail(MarkupHelper.CreateLabel("Test case FAILED due to below issues: " + currentScenarioTestError.Message, ExtentColor.Red).GetMarkup().ToString(), MediaEntityBuilder.CreateScreenCaptureFromBase64String(mediaEntity).Build());
                }
            }

            //Pending Status
            if (TestResult.ToString() == "StepDefinitionPending")
            {
                if (scenarioStepContextType == "Given")
                    scenario.CreateNode<Given>(scenarioStepContextInfo).Warning("Step Definition Pending");
                else if (scenarioStepContextType == "When")
                    scenario.CreateNode<When>(scenarioStepContextInfo).Warning("Step Definition Pending");
                else if (scenarioStepContextType == "Then")
                    scenario.CreateNode<Then>(scenarioStepContextInfo).Warning("Step Definition Pending");
                else if (scenarioStepContextType == "And")
                    scenario.CreateNode<And>(scenarioStepContextInfo).Warning("Step Definition Pending");
            }
        }

        public static void CloseAndFlushReport()
        {
            extent.Flush();
            Extensions.RenameReportFile(ReportPath, "index.html", ReportName);
            Extensions.ReplaceTextHTMLReportFile(ReportPath + ReportName, "index.html", ReportName);
        }

        //public static string ErrorScreenshot(this IWebDriver driver)
        //{
        //    ITakesScreenshot screenshotDriver = driver as ITakesScreenshot;
        //    Screenshot screenshot = screenshotDriver.GetScreenshot();
        //    string base64 = screenshot.AsBase64EncodedString.Replace("\"\"", "");

        //    string ErrDirectory = DefaultFilepaths.SCREENSHOT_FILEPATH;
        //    Directory.CreateDirectory(ErrDirectory);
        //    ErrDirectory = ErrDirectory + "Err_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
        //    screenshot.SaveAsFile(ErrDirectory, ScreenshotImageFormat.Jpeg);

        //    return base64;
        //}

        //public static string PrintScreen(this IWebDriver driver, ScreenshotImageFormat ImageFormat)
        //{
        //    try
        //    {
        //        return ((ITakesScreenshot)driver).GetScreenshot().AsBase64EncodedString;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw;
        //    }
        //}
    }
}