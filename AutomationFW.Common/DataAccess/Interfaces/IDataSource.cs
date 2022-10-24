using AutomationFW.Common.DataModels;
using System;
using System.Collections.Generic;

namespace AutomationFW.Common.DataAccess.Interfaces
{
    public interface IDataSource<T> where T : TestDataModelBase, new()
    {
        string DataSourceName { get; set; }

        T GetNewTestData(string testDataType, bool isTestDataUsedUp = false);

        T GetNewValidatedTestData
        (
            string testDataType,
            List<ValidationCheck<T>> validations,
            Action<T> navigationAction,
            int maxResultsToCheck = -1,
            bool isTestDataUsedUp = false
        );

        List<T> GetMultipleNewTestData(string testDataType, int amount, bool isTestDataUsedUp = false);

        List<T> GetMultipleNewValidatedTestData
        (
            string testDataType,
            List<ValidationCheck<T>> validations,
            Action<T> navigationAction,
            int amount,
            int maxResultsToCheck = -1,
            bool isTestDataUsedUp = false
        );

    }
}
