namespace AutomationFW.Common.DataModels
{
    // All values listed in this class are defaults that are only used if none are given in the actual json.
    public class EnvConfig
    {
        public ValidityLoggingConfig ValidityLoggingConfig = new ValidityLoggingConfig();
    }

    public class ValidityLoggingConfig
    {
        public bool useValidityLogging = false;
        public bool onlyShowFirstSixOfAccount = false;
    }
}
