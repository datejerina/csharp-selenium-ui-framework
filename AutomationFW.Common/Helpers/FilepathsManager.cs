using AutomationFW.Common.DataModels;
using Serilog;
using System;
using System.IO;

namespace AutomationFW.Common.Helpers
{
    public static class FilepathsManager
    {
        internal static CustomFilepaths CustomFilepaths { get; set; }

        public static string AccessJsonFilename
        {
            get
            {
                return CustomFilepaths?.AccessJsonFilename == null ?
                    DefaultFilepaths.ACCESS_JSON_FILENAME : CustomFilepaths.AccessJsonFilename;
            }
        }

        public static string AccessJsonFilepath
        {
            get
            {
                return CustomFilepaths?.AccessJsonFilepath == null ?
                    DefaultFilepaths.ACCESS_JSON_FILEPATH : CustomFilepaths.AccessJsonFilepath;
            }
        }

        public static string DataJsonFilename
        {
            get
            {
                return CustomFilepaths?.DataJsonFilename == null ?
                    DefaultFilepaths.DATA_JSON_FILENAME : CustomFilepaths.DataJsonFilename;
            }
        }

        public static string DataJsonFilepath
        {
            get
            {
                return CustomFilepaths?.DataJsonFilepath == null ?
                    DefaultFilepaths.DATA_JSON_FILEPATH : CustomFilepaths.DataJsonFilepath;
            }
        }

        public static string EnvConfigJsonFilename
        {
            get
            {
                return CustomFilepaths?.EnvConfigJsonFilename == null ?
                    DefaultFilepaths.ENVCONFIG_JSON_FILENAME : CustomFilepaths.EnvConfigJsonFilename;
            }
        }

        public static string EnvConfigJsonFilepath
        {
            get
            {
                return CustomFilepaths?.EnvConfigJsonFilepath == null ?
                    DefaultFilepaths.ENVCONFIG_JSON_FILEPATH : CustomFilepaths.EnvConfigJsonFilepath;
            }
        }

        public static string ValidityLoggingCsvFilename
        {
            get
            {
                return CustomFilepaths?.ValidityLoggingCsvFilename == null ?
                    DefaultFilepaths.VALIDITY_LOGGING_FILENAME : CustomFilepaths.ValidityLoggingCsvFilename;
            }
        }

        public static string ValidityLoggingCsvFilepath
        {
            get
            {
                return CustomFilepaths?.ValidityLoggingCsvFilepath == null ?
                    DefaultFilepaths.VALIDITY_LOGGING_FILEPATH : CustomFilepaths.ValidityLoggingCsvFilepath;
            }
        }

        public static string ScreenshotFilepath
        {
            get
            {
                return CustomFilepaths?.ScreenshotFilepath == null ?
                    DefaultFilepaths.SCREENSHOT_FILEPATH : CustomFilepaths.ScreenshotFilepath;
            }
        }

        public static string TestResultsCsvFilepath
        {
            get
            {
                return CustomFilepaths?.TestResultsCsvFilepath == null ?
                    DefaultFilepaths.TEST_RESULTS_FILEPATH : CustomFilepaths.TestResultsCsvFilepath;
            }
        }

        public static void ValidateFilepaths()
        {
            if (!Directory.Exists(AccessJsonFilepath))
            {
                Log.Information($"The filepath for Access Json could not be found: {AccessJsonFilepath}",
                    LogTypes.SETUP);
            }
            else if (!File.Exists(AccessJsonFilepath + AccessJsonFilename))
            {
                Log.Error($"The filename for Access Json could not be found in the given directory." +
                    $"{Environment.NewLine}FilePath: {AccessJsonFilepath}" +
                    $"{Environment.NewLine}FileName: {AccessJsonFilename}",
                    LogTypes.SETUP);
            }

            if (!Directory.Exists(DataJsonFilepath))
            {
                Log.Information($"The filepath for Data Json could not be found: {DataJsonFilepath}",
                    LogTypes.SETUP);
            }
            else if (!File.Exists(DataJsonFilepath + DataJsonFilename))
            {
                Log.Error($"The filename for Data Json could not be found in the given directory." +
                    $"{Environment.NewLine}FilePath: {DataJsonFilepath}" +
                    $"{Environment.NewLine}FileName: {DataJsonFilename}",
                    LogTypes.SETUP);
            }

            if (!Directory.Exists(EnvConfigJsonFilepath))
            {
                Log.Information($"The filepath for EnvConfig Json could not be found: {EnvConfigJsonFilepath}",
                    LogTypes.SETUP);
            }
            else if (!File.Exists(EnvConfigJsonFilepath + EnvConfigJsonFilename))
            {
                Log.Error($"The filename for EnvConfig Json could not be found in the given directory." +
                     $"{Environment.NewLine}FilePath: {EnvConfigJsonFilepath}" +
                     $"{Environment.NewLine}FileName: {EnvConfigJsonFilename}",
                     LogTypes.SETUP);
            }

            if (!Directory.Exists(ValidityLoggingCsvFilepath))
            {
                Log.Information($"The filepath for validity logging csv could not be found: {ValidityLoggingCsvFilepath}",
                    LogTypes.SETUP);
            }
            else if (!File.Exists(ValidityLoggingCsvFilepath + ValidityLoggingCsvFilename))
            {
                Log.Error($"The filename for validity logging csv could not be found in the given directory." +
                    $"{Environment.NewLine}FilePath: {ValidityLoggingCsvFilepath}" +
                    $"{Environment.NewLine}FileName: {ValidityLoggingCsvFilename}",
                    LogTypes.SETUP);
            }

            if (!Directory.Exists(ScreenshotFilepath))
            {
                Log.Information($"The filepath for saving screenshots could not be found: {ScreenshotFilepath}",
                    LogTypes.SETUP);
            }

            if (!Directory.Exists(TestResultsCsvFilepath))
            {
                Log.Error($"The filepath for saving test results csv could not be found: {TestResultsCsvFilepath}",
                    LogTypes.SETUP);
            }
        }
    }
}