using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using AutomationFW.Common.Helpers;
using AutomationFW.Common.DataModels;
using AutomationFW.Common.DataAccess.Interfaces;

namespace AutomationFW.Common.DataAccess
{
    public class DataSourceJson<T> : IDataSource<T> where T : TestDataModelBase, new()
    {
        public string DataSourceName { get; set; } = DataSourceTypes.JSON;
        protected List<T> _entriesOfOneType;
        protected string _currentTestDataType = "";
        protected HashSet<T> _usedEntriesSet = new HashSet<T>();
        protected JObject _allTestData;

        public DataSourceJson(JObject allTestData)
        {
            _allTestData = allTestData;
        }

        public List<T> GetMultipleNewTestData(string testDataType, int amount, bool isTestDataUsedUp = false)
        {
            var resultList = new List<T>();

            for (int i = 1; i <= amount; i++)
            {
                var result = GetSingleEntryFromJson(testDataType);

                if (result == null)
                {
                    Assert.Inconclusive(
                        $"Could not find enough unused test data of type '{testDataType}' in Data.json.");
                }

                Logger.LogTestDataValidityInCsvFile(result, testDataType, DataSourceName);

                if (isTestDataUsedUp)
                {
                    MarkEntryAsUsedUp(result);
                    JsonIoHandler.WriteDataJson(_allTestData);
                }

                resultList.Add(result);
            }

            return resultList;
        }

        public List<T> GetMultipleNewValidatedTestData(
            string testDataType,
            List<ValidationCheck<T>> validations,
            Action<T> navigationAction,
            int amount,
            int maxResultsToCheck = -1,
            bool isTestDataUsedUp = false)
        {
            Validator<T> validator = new Validator<T>(testDataType);
            string invalidReasons;
            T result;
            int attemptNumber;
            var resultList = new List<T>();

            for (int i = 1; i <= amount; i++)
            {
                attemptNumber = 0;

                do
                {
                    attemptNumber++;
                    result = GetSingleEntryFromJson(testDataType);
                    if (result == null)
                    {
                        Assert.Inconclusive($"Could not find any valid test data of type '{testDataType}' from json" +
                            $" out of the {attemptNumber - 1} unused entries available.");
                    }

                    navigationAction.Invoke(result);
                    invalidReasons = validator.GetAnyInvalidReasons(result, validations);
                    var isValid = invalidReasons == "";

                    Logger.LogTestDataValidityInCsvFile(
                        result,
                        testDataType,
                        DataSourceName,
                        attemptNumber,
                        isValid,
                        invalidReasons);
                }
                while (invalidReasons != "" && (attemptNumber < maxResultsToCheck || maxResultsToCheck == -1));

                if (attemptNumber >= maxResultsToCheck && maxResultsToCheck != -1)
                {
                    Assert.Inconclusive(
                        $"Could not find valid test data of type '{testDataType}' after {attemptNumber} attempts.");
                }

                if (isTestDataUsedUp)
                {
                    MarkEntryAsUsedUp(result);
                    JsonIoHandler.WriteDataJson(_allTestData);
                }

                resultList.Add(result);
            }
            return resultList;
        }

        public T GetNewTestData(string testDataType, bool isTestDataUsedUp = false)
        {
            return GetMultipleNewTestData(testDataType, 1, isTestDataUsedUp).FirstOrDefault();
        }

        public T GetNewValidatedTestData(
            string testDataType,
            List<ValidationCheck<T>> validations,
            Action<T> navigationAction,
            int maxResultsToCheck = -1,
            bool isTestDataUsedUp = false)
        {
            return GetMultipleNewValidatedTestData(
                testDataType,
                validations,
                navigationAction,
                1,
                maxResultsToCheck,
                isTestDataUsedUp).FirstOrDefault();
        }

        // If the json format ever changes, this method will need to be updated to work with the new format
        protected virtual T GetSingleEntryFromJson(string testDataType)
        {
            if (testDataType != _currentTestDataType)
            {
                if (_allTestData[testDataType] == null)
                {
                    Log.Information($"The specified test data type of '{testDataType}' could not be found in the json", LogTypes.DATASOURCE);

                    Assert.Inconclusive(
                        $"The specified test data type of '{testDataType}' could not be found in the json");
                }

                _entriesOfOneType = _allTestData[testDataType].ToObject<List<T>>();
                _currentTestDataType = testDataType;
            }

            var result = _entriesOfOneType
                .Where(x => x.HasBeenUsedUp != true)
                .Except(_usedEntriesSet).FirstOrDefault();

            _usedEntriesSet.Add(result);
            return result;
        }

        // If the json format ever changes, this method will need to be updated to work with the new format
        protected virtual void MarkEntryAsUsedUp(TestDataModelBase testData)
        {
            _entriesOfOneType.First(a => a.GetAllTestDataAsString() == testData.GetAllTestDataAsString())
                .HasBeenUsedUp = true;

            _allTestData[_currentTestDataType].Replace(JToken.FromObject(
                _entriesOfOneType.ToArray(),
                new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}