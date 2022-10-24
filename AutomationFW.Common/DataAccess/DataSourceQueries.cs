using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutomationFW.Common.DataModels;
using AutomationFW.Common.Helpers;
using AutomationFW.Common.DataAccess.Interfaces;

namespace AutomationFW.Common.DataAccess
{
    public class DataSourceQueries<T> : IDataSource<T> where T : TestDataModelBase, new()
    {
        public string DataSourceName { get; set; } = DataSourceTypes.QUERIES;
        private Dictionary<string, Query<T>> _testDataQueries { get; set; } = new Dictionary<string, Query<T>>();
        private int _queryResultIndex = 0;
        private Dictionary<string, HashSet<DataRow>> _usedResults = new Dictionary<string, HashSet<DataRow>>();

        public DataSourceQueries(Dictionary<string, Query<T>> testDataQueries)
        {
            _testDataQueries = testDataQueries;
        }

        public List<T> GetMultipleNewTestData(string testDataType, int amount, bool isTestDataUsedUp = false)
        {
            Query<T> query;
            List<T> resultList = new List<T>();
            DataTable queryResultsTable = null;

            if (!_testDataQueries.TryGetValue(testDataType, out query))
            {
                Assert.Inconclusive($"No predefined query for test data type of: {testDataType}");
            }

            for (int i = 1; i <= amount; i++)
            {
                if (!CanGetNextQueryResult(queryResultsTable))
                {
                    queryResultsTable = PerformQueryForTestDataType(testDataType, query);
                }

                var result = GetNextResultFromQuery(query, queryResultsTable, testDataType);
                Logger.LogTestDataValidityInCsvFile(result, testDataType, DataSourceName);
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
            Query<T> query;
            List<T> resultList = new List<T>();
            DataTable queryResultsTable = null;
            Validator<T> validator = new Validator<T>(testDataType);
            string invalidReasons;
            int attemptNumber;

            if (!_testDataQueries.TryGetValue(testDataType, out query))
            {
                Assert.Inconclusive($"No predefined query for test data type of: {testDataType}");
            }

            for (int i = 1; i <= amount; i++)
            {
                T result;
                attemptNumber = 0;

                do
                {
                    attemptNumber++;

                    if (!CanGetNextQueryResult(queryResultsTable))
                    {
                        queryResultsTable = PerformQueryForTestDataType(testDataType, query, queryResultsTable);
                    }

                    result = GetNextResultFromQuery(query, queryResultsTable, testDataType);
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

        public DataTable PerformQueryForTestDataType(
            string testDataType,
            Query<T> query,
            DataTable resultsTable = null)
        {
            var queryResults = query.DbConnectionHandler.PerformQuery(
                AppendUsedResultsToSqlCommand(query, testDataType));

            if (queryResults == null || queryResults.Rows.Count == 0)
            {
                Assert.Inconclusive($"Couldn't find a valid account in the database for data type '{testDataType}'" +
                    $".  It returned an empty set for the following query:\n{query.Command}");
            }

            _queryResultIndex = 0;
            return queryResults;
        }

        public string AppendUsedResultsToSqlCommand(Query<T> query, string testDataType)
        {
            string commandText = query.Command;
            if (!_usedResults.TryGetValue(testDataType, out HashSet<DataRow> usedResultSet))
                usedResultSet = new HashSet<DataRow>();

            foreach (DataRow row in usedResultSet)
            {
                commandText += " and not ( ";

                for (int columnIndex = 0; columnIndex < row.Table.Columns.Count; columnIndex++)
                {
                    string fullFieldName = query.FieldMappingList.Select(x => x.TableFieldName).FirstOrDefault(
                        x => x.ToLower().EndsWith("." + row.Table.Columns[columnIndex].ToString().ToLower()))
                        ?? row.Table.Columns[columnIndex].ToString();

                    if (row.Table.Columns[columnIndex].DataType.Equals(typeof(DateTime))
                        && row[columnIndex].ToString() != "")
                    {
                        var dateString = ((DateTime)row[columnIndex]).ToString("yyyy-MM-dd HH:mm:ss");
                        commandText += $"{fullFieldName} = to_date('{dateString}', 'YYYY-MM-DD HH24:MI:SS')";
                    }
                    else
                    {
                        commandText += $"{fullFieldName} = '{row[columnIndex]}'";
                    }

                    if (columnIndex + 1 < row.Table.Columns.Count)
                    {
                        commandText += " and ";
                    }
                }

                commandText += " )";
            }

            return commandText;
        }

        public T GetNextResultFromQuery(Query<T> query, DataTable queryResultsTable, string testDataType)
        {
            if (_queryResultIndex >= queryResultsTable.Rows.Count)
                return null;
            var resultRow = queryResultsTable.Rows[_queryResultIndex];
            if (_usedResults.TryGetValue(testDataType, out HashSet<DataRow> usedResultSet))
                usedResultSet.Add(resultRow);
            else
            {
                usedResultSet = new HashSet<DataRow>() { resultRow };
                _usedResults.Add(testDataType, usedResultSet);
            }
            var result = new T();
            foreach (var fieldNameMapping in query.FieldMappingList)
            {
                string fieldNameFixed = fieldNameMapping.TableFieldName;
                if (fieldNameFixed.Contains("."))
                    fieldNameFixed = fieldNameFixed.Substring(fieldNameFixed.IndexOf(".") + 1);
                string fieldValue = null;
                try
                {
                    fieldValue = queryResultsTable.Rows[_queryResultIndex][fieldNameFixed].ToString();
                }
                catch (ArgumentException e)
                {
                    Assert.Inconclusive($"Could not find any field by the name '{fieldNameFixed}' in the DataTable " +
                         $"of results from executing the following query:\n{query.Command}\nEnsure the " +
                         $"TableFieldName specified for the Field Mappings for this query in " +
                         $"DataAccess/TestDataQueries.cs matches a field retrieved by the query. " +
                         $"Exception:\n{e.Message}");
                }

                fieldNameMapping.MapFieldDataToTestData(
                    fieldValue,
                    queryResultsTable.Columns[fieldNameFixed].DataType,
                    ref result);
            }

            _queryResultIndex++;
            return result;
        }

        public bool CanGetNextQueryResult(DataTable queryResultsTable)
        {
            return queryResultsTable != null && _queryResultIndex < queryResultsTable.Rows.Count;
        }
    }
}