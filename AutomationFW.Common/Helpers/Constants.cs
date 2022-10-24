namespace AutomationFW.Common.Helpers
{
    public static class Constants
    {

    }

    public static class DefaultFilepaths
    {
        public const string ENVCONFIG_JSON_FILENAME = "EnvConfig.json";
        //public static readonly string ENVCONFIG_JSON_FILEPATH =
        //    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../Data/");
        public static readonly string ENVCONFIG_JSON_FILEPATH =
            Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Data\\");

        public const string ACCESS_JSON_FILENAME = "Access.json";
        public static readonly string ACCESS_JSON_FILEPATH =
            Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Data\\");

        public const string DATA_JSON_FILENAME = "Data.json";
        public static readonly string DATA_JSON_FILEPATH =
            Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Data\\");

        public const string VALIDITY_LOGGING_FILENAME = "TestDataRetrievalLogging.csv";
        public static readonly string VALIDITY_LOGGING_FILEPATH =
            Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Logs\\");

        public static readonly string SCREENSHOT_FILEPATH =
            Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Screenshots\\");

        public static readonly string TEST_RESULTS_FILEPATH =
            Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Results\\");
    }

    public static class TestingEnvironments
    {
        public const string DEV = "dev";
        public const string TEST = "test";
        public const string PREPROD = "preprod";
    }

    // supported literal scenario tags
    // not including parameterized ones
    public static class StandardScenarioTags
    {
        public const string SMOKE = "smoke";
        public const string REGRESSION = "regression";
        public const string WIP = "wip";
    }

    // data source types
    public static class DataSourceTypes
    {
        public const string QUERIES = "queries";
        public const string JSON = "json";
    }

    // log types
    public static class LogTypes
    {
        public const string SETUP = "Setup";
        public const string SELENIUM = "Selenium";
        public const string DATASOURCE = "DataSource";
        public const string DATAACCESS = "DataAccess";
        public const string GENERAL = "Logger";
    }

    // browser types
    public static class BrowserTypes
    {
        public const string CHROME = "chrome";
        public const string CHROME_HEADLESS = "chrome_headless";
        public const string IE = "ie";
        public const string FIREFOX = "firefox";
        public const string EDGE = "edge";
    }

    //TODO: relocate these to correct classes
    // test data types
    // If any steps are parameterized by TestDataType, ensure the wording used 
    // in the Gherkin matches up exactly with one of the TestDataTypes below
    // Add a new TestDataType below as needed
    public static class TestDataTypes
    {
        public const string ANY_ACCOUNT_NUMBER = "any account number";
    }
}
