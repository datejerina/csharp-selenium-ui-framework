using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace AutomationFW.Common.Helpers
{
    public static class JsonIoHandler
    {
        public static string env;

        public static JObject GetAccessJObject()
        {
            try
            {
                var accessJsonFile = JObject.Parse(File.ReadAllText(
                    FilepathsManager.AccessJsonFilepath + FilepathsManager.AccessJsonFilename));

                Log.Information("Access file parsed successfully", LogTypes.SETUP);
                return accessJsonFile[env] as JObject;
            }
            catch (Exception e)
            {
                Log.Fatal($"Exception was thrown parsing access file. Message: {e.Message}", LogTypes.SETUP);
                return null;
            }
        }

        public static JObject GetDataJObject()
        {
            try
            {
                var dataJsonFile = JObject.Parse(File.ReadAllText(
                    FilepathsManager.DataJsonFilepath + FilepathsManager.DataJsonFilename));

                Log.Information("Data file parsed successfully", LogTypes.SETUP);
                return dataJsonFile[$"{env}TestData"] as JObject;
            }
            catch (Exception e)
            {
                Log.Fatal($"Exception was thrown parsing data file. Message: {e.Message}", LogTypes.SETUP);
                return null;
            }
        }

        public static JObject GetEnvConfigJObject()
        {
            try
            {
                var envConfigJsonFile = JObject.Parse(File.ReadAllText(
                    FilepathsManager.EnvConfigJsonFilepath + FilepathsManager.EnvConfigJsonFilename));

                Log.Information("Config file parsed successfully", LogTypes.SETUP);
                return envConfigJsonFile[$"{env}Config"] as JObject;
            }
            catch (Exception e)
            {
                Log.Fatal($"Exception was thrown parsing config file. Message: {e.Message}", LogTypes.SETUP);
                return null;
            }
        }

        public static void WriteDataJson(JObject envData)
        {
            JObject dataJObject;
            try
            {
                dataJObject = JObject.Parse(File.ReadAllText(
                    FilepathsManager.DataJsonFilepath + FilepathsManager.DataJsonFilename)) as JObject;
            }
            catch (Exception e)
            {
                Log.Fatal($"Exception was thrown parsing data file prior to writing changes. Message: {e.Message}",
                    LogTypes.SETUP);

                return;
            }

            dataJObject[$"{env}TestData"] = envData;

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };
                string serializedJson = JsonConvert.SerializeObject(dataJObject, settings);
                File.WriteAllText(FilepathsManager.DataJsonFilepath + FilepathsManager.DataJsonFilename, serializedJson);
            }
            catch (Exception e)
            {
                Log.Fatal($"Exception was thrown writing updates to data file. Message: {e.Message}", LogTypes.SETUP);
                return;
            }
        }
    }
}