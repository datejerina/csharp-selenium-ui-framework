using AutomationFW.Common.DataModels;
using AutomationFW.Common.Helpers.Interfaces;
using System.Collections.Generic;

namespace AutomationFW.Common.Helpers
{
    public class Validator<T> : IValidator<T> where T : TestDataModelBase
    {
        public string TestDataType { get; set; }

        public Validator(string testDataType)
        {
            TestDataType = testDataType;
        }

        // Performs any validation checks given to it and ensures their expectedResult values are returned.
        // If not, adds a message to invalidReasons as to why, and returns such string upon completion.  
        public string GetAnyInvalidReasons(T testData, List<ValidationCheck<T>> validationChecks)
        {
            string invalidReasons = "";

            foreach (var validationCheck in validationChecks)
            {
                if (validationCheck.ExpectedResult == null)
                {
                    invalidReasons += $"Cannot run validation '{validationCheck.CriterionName}' for testDataType " +
                        $"'{TestDataType}' since ExpectedResult is null.  Ensure a value is set for ExpectedResult " +
                        $"in Helpers/ValidatorOptions.GetValidationChecksForCurrentTestDataType();  ";
                }
                else
                {
                    var validationResult = validationCheck.ValidationMethod.Invoke(TestDataType, testData);

                    if (validationResult != validationCheck.ExpectedResult)
                    {
                        invalidReasons += $"expected '{validationCheck.CriterionName}' to be " +
                            $"{validationCheck.ExpectedResult.ToString()} but instead was " +
                            $"{validationResult.ToString()};   ";
                    }
                }
            }

            return invalidReasons;
        }

    }
}
