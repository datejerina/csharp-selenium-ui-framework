using AutomationFW.Common.DataAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutomationFW.Common.DataModels
{
    public class Query<T> where T : TestDataModelBase
    {
        public string Command { get; set; }
        public List<FieldMapping<T>> FieldMappingList { get; set; } = new List<FieldMapping<T>>();
        public DbConnectionHandlerBase DbConnectionHandler { get; set; }

        // constructor to aid with creation of query objects
        public Query(string command, List<FieldMapping<T>> fieldMappingList, DbConnectionHandlerBase dbConnection)
        {
            Command = command;
            FieldMappingList = fieldMappingList;
            DbConnectionHandler = dbConnection;
        }
    }

    public class FieldMapping<T> where T : TestDataModelBase
    {
        public string TableFieldName { get; set; }
        public string TestDataPropertyName { get; set; }

        public FieldMapping(string tableFieldName, string testDataPropertyName)
        {
            TableFieldName = tableFieldName;
            TestDataPropertyName = testDataPropertyName;
        }

        public void MapFieldDataToTestData(string fieldValue, Type fieldType, ref T testData)
        {
            var testDataProperty = typeof(T).GetProperty(TestDataPropertyName);

            if (testDataProperty == null)
            {
                Assert.Inconclusive($"Could not find property '{TestDataPropertyName}' in TestData.cs while" +
                    $" attempting to parse query results.  Ensure the property specified by name in " +
                    $"TestDataQueries.cs matches a property of TestData.cs");
            }

            try
            {
                testDataProperty.SetValue(testData, Convert.ChangeType(fieldValue, fieldType));
            }
            catch (FormatException)
            {
                testDataProperty.SetValue(testData, null);
            }
            catch (ArgumentException e)
            {
                Assert.Inconclusive($"Exception thrown while attempting to store query result field " +
                    $"'{TableFieldName}' with a value of '{fieldValue}' into TestData property " +
                    $"'{TestDataPropertyName}'.  Ensure the type returned by the DB query matches the property's " +
                    $"type in TestData (or underlying type if nullable).  Original exception: \n{e}");
            }
        }
    }
}
