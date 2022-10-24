using AutomationFW.Common.DataModels;
using System.Collections.Generic;

namespace AutomationFW.Common.Helpers.Interfaces
{
    public interface IValidator<T> where T : TestDataModelBase
    {
        string TestDataType { get; set; }
        string GetAnyInvalidReasons(T testData, List<ValidationCheck<T>> validationChecks);
    }
}
