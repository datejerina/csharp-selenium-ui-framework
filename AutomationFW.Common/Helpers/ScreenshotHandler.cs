using NUnit.Framework;
using OpenQA.Selenium;
using Serilog;
using System;
using TechTalk.SpecFlow;

namespace AutomationFW.Common.Helpers
{
    public static class ScreenshotHandler
    {
        public static int ScreenshotCounter = 1;

        /* Capture a screenshot of the current browser window.
         * Sceenshot will be saved to the screenshots folder within this project as a .png file.
         * Specifying a custom name is optional, as it will default to the current scenario name.
         * Custom name should have no extension.
         * Beware that using the same static filename or an existing file will overwrite it in subsequent test runs.
         */

        public static string CaptureScreenshot(
            IWebDriver driver,
            ScenarioContext currentScenario,
            string customScreenshotFilename)
        {
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            var fileName = customScreenshotFilename ?? TestContext.CurrentContext.Test.Name;

            // replacing characters disallowed in filename
            fileName = fileName.Replace('<', '{');
            fileName = fileName.Replace('>', '}');
            fileName = fileName.Replace("\"", "'");
            fileName = fileName.Replace("/", ",");

            // If filename is too long, cut it down.  Max is 260, but need room for # and file extension
            if (fileName.Length + FilepathsManager.ScreenshotFilepath.Length > 252)
            {
                fileName = fileName.Substring(0, 252 - FilepathsManager.ScreenshotFilepath.Length);
            }

            fileName += $"({ScreenshotCounter}).png";
            var fullFilepath = FilepathsManager.ScreenshotFilepath + "/" + fileName;
            screenshot.SaveAsFile(fullFilepath, ScreenshotImageFormat.Png);
            var stepName = "[Could not retrieve step name.]";

            try
            {
                stepName = currentScenario.StepContext.StepInfo.Text;
            }
            catch (Exception)
            {
            }

            Log.Information($"Screenshot taken on step '{stepName}'", LogTypes.SETUP);
            ScreenshotCounter++;
            return fullFilepath;
        }
    }
}