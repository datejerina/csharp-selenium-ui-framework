using AutomationFW.Common.DataModels;
using AutomationFW.Common.Helpers;

namespace AutomationFW.Test.FrameworkConfiguration
{
    public class FWTestRunContext : TestRunContextBase
    {
        public FWTestRunContext
        (
            string defaultEnvironment,
            string defaultBrowser,
            CustomFilepaths customFilepaths = null
        ) : base(defaultEnvironment, defaultBrowser, customFilepaths)
        {
            Browser.GoToHomePage();
        }
    }
}