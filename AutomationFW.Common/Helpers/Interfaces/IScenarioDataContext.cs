namespace AutomationFW.Common.Helpers.Interfaces
{
    public interface IScenarioDataContext
    {
        string GetAllTestDataAsString(bool maskAcctNum = false);
    }
}
