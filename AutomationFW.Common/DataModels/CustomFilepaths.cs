namespace AutomationFW.Common.DataModels
{
    public class CustomFilepaths
    {
        public virtual string AccessJsonFilename { get; set; }
        public virtual string AccessJsonFilepath { get; set; }
        public virtual string DataJsonFilename { get; set; }
        public virtual string DataJsonFilepath { get; set; }
        public virtual string EnvConfigJsonFilename { get; set; }
        public virtual string EnvConfigJsonFilepath { get; set; }
        public virtual string ScreenshotFilepath { get; set; }
        public virtual string TestResultsCsvFilepath { get; set; }
        public virtual string ValidityLoggingCsvFilename { get; set; }
        public virtual string ValidityLoggingCsvFilepath { get; set; }
    }
}
