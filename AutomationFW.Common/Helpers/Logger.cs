using AutomationFW.Common.DataModels;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Serilog.Formatting;
using System.Text.RegularExpressions;

namespace AutomationFW.Common.Helpers
{
    public static class Logger
    {
        private static string _testResultsFileName = "";
        public static ValidityLoggingConfig config;
        public static string env;

        public static void LogTestDataValidityInCsvFile
        (
            TestDataModelBase testData,
            string testDataType,
            string dataSourceType,
            int attemptNumber = 1,
            bool? isValid = null,
            string reason = ""
        )
        {
            //ensure toggle is being followed, and no logging will be performed if turned off
            if (!config.useValidityLogging)
            {
                return;
            }

            var fullFilePath = FilepathsManager.ValidityLoggingCsvFilepath
                + FilepathsManager.ValidityLoggingCsvFilename;

            if (!File.Exists(fullFilePath))
            {
                var tableHeader =
                    "Date & Time, " +
                    "Environment, " +
                    "Test Name, " +
                    "Test Data Type, " +
                    "Data Source, " +
                    "Attempt #, " +
                    "Retrieved Data Value, " +
                    "Usable?, " +
                    "Reason(s) why not usable\n";

                try
                {
                    File.WriteAllText(fullFilePath, tableHeader);
                }
                catch (Exception e)
                {
                    string msg = $"Exception caught while attempting to write header for new validity logging csv " +
                        $"file.  No csv data can be written.  \r\n   Exception text: {e}";

                    Log.Fatal(msg, LogTypes.GENERAL);
                    return;
                }
            }

            string isValidText;

            if (isValid == null)
            {
                isValidText = "not checked";
            }
            else
            {
                isValidText = isValid.Value ? "yes" : "no";
            }

            string record =
                $"{DateTime.Now.ToString()}," +
                $"{env}," +
                $"{TestContext.CurrentContext.Test.Name.Replace(",", ";")}," +
                $"{testDataType}," +
                $"{dataSourceType}," +
                $"{attemptNumber}," +
                $"{testData.GetAllTestDataAsString().Replace(",", ".")}," +
                $"{isValidText}," +
                $"{reason}\n";

            try
            {
                File.AppendAllText(fullFilePath, record);
            }
            catch (IOException e)
            {
                Log.Fatal($"Exception caught while attempting to write to validity logging csv file.  No data can be " +
                    $"written.  \r\n   Ensure the file is closed before attempting to run a test that writes to it." +
                    $"\r\n   Exception text: {e}",
                    LogTypes.GENERAL);

                return;
            }
        }

        public static void LogTestResultInCsvFile(
            string featureName,
            string scenarioName,
            string result,
            ScenarioDataContextBase testData,
            string failureReason)
        {
            if (_testResultsFileName == "")
            {
                _testResultsFileName = "test_results_" + DateTime.Now.ToString("yyyy-MM-dd_HH;mm;ss") + ".csv";
            }

            if (failureReason == null)
            {
                failureReason = "";
            }
            else
            {
                failureReason = failureReason.Replace("\r\n", ";");
                failureReason = failureReason.Replace(',', ' ');
            }

            scenarioName = scenarioName.Replace(',', ' ');

            var fullFilePath = FilepathsManager.TestResultsCsvFilepath + _testResultsFileName;
            var fullFilePathNewest = FilepathsManager.TestResultsCsvFilepath + "test_results_newest.csv";

            if (!File.Exists(fullFilePath))
            {
                var tableHeader =
                    "Feature," +
                    "Scenario," +
                    "Test Name," +
                    "Result," +
                    "Time completed," +
                    "Test data," +
                    "Failure reason\n";
                try
                {
                    File.WriteAllText(fullFilePath, tableHeader);
                    File.WriteAllText(fullFilePathNewest, tableHeader);
                }
                catch (Exception e)
                {
                    Log.Fatal($"Exception caught while attempting to write header for test results csv file.  " +
                        $"No csv data can be written.  \r\n   Exception text: {e}",
                        LogTypes.GENERAL);

                    return;
                }
            }

            string testNameCSV = TestContext.CurrentContext.Test.Arguments.Length > 0
                ? TestContext.CurrentContext.Test.Arguments[0].ToString() : TestContext.CurrentContext.Test.Name.Replace(",", ";");

            string record =
                $"{featureName}," +
                $"{scenarioName}," +
                $"{testNameCSV}," +
                $"{result}," +
                $"{DateTime.Now.ToString("G")}," +
                $"{testData.GetAllTestDataAsString().Replace(",", ".")}," +
                $"{failureReason}\n";

            try
            {
                File.AppendAllText(fullFilePath, record);
                File.AppendAllText(fullFilePathNewest, record);
            }
            catch (IOException e)
            {
                string msg = $"Exception caught while attempting to write to test results csv file.  No data can " +
                    $"be written.  \r\n   Ensure the file is closed before attempting to run a test that writes to " +
                    $"it.\r\n   Exception text: {e}";

                Log.Fatal(msg, LogTypes.GENERAL);
                return;
            }
        }


        private static LogEventLevel LogEventLevelConfig()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
                    .AddJsonFile("appSettings.json");
            IConfigurationRoot configuration = builder.Build();
            string logSetting = configuration.GetSection("appSettings")["LogLevel"];
            LogEventLevel eventLevel = new();
            switch (logSetting)
            {
                case string a when a.Contains("Verbose"):
                    eventLevel = LogEventLevel.Verbose;
                    break;
                case string a when a.Contains("Debug"):
                    eventLevel = LogEventLevel.Debug;
                    break;
                case string a when a.Contains("Information"):
                    eventLevel = LogEventLevel.Information;
                    break;
                case string a when a.Contains("Warning"):
                    eventLevel = LogEventLevel.Warning;
                    break;
                case string a when a.Contains("Error"):
                    eventLevel = LogEventLevel.Error;
                    break;
                case string a when a.Contains("Fatal"):
                    eventLevel = LogEventLevel.Fatal;
                    break;
                default:
                    return eventLevel = LogEventLevel.Error;
            }
            return eventLevel;
        }

        public static void Configure()
        {
            var eventLevel = LogEventLevelConfig();
            var pathLog = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Logs\\");
            ITextFormatter jsonFormatter = new Serilog.Formatting.Json.JsonFormatter(renderMessage: true);

            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(path: pathLog + "log_" + DateTime.Now.ToString("HH-mm-ss") + "_" + ".log",
                             formatter: jsonFormatter,
                             rollingInterval: RollingInterval.Day,
                             rollOnFileSizeLimit: true,
                             fileSizeLimitBytes: 10000000,
                             restrictedToMinimumLevel: eventLevel)
               .CreateLogger();
        }

        public static string AdjustScenarioTitleForScenarioOutlines(string scenarioName)
        {
            //This is a pattern we have decided on for naming tests with scenario outlilne tables in gherkin
            //(ie. I add <number1> to <number2>)
            Regex pattern = new Regex(@"\<.*?\>");
            var tableRow = TestContext.CurrentContext.Test.Arguments;

            for (int parameterIndex = 0; parameterIndex < tableRow.Length - 1; parameterIndex++)
            {
                scenarioName = pattern.Replace(scenarioName, tableRow[parameterIndex].ToString(), 1);
            }

            return scenarioName;
        }
    }
}
