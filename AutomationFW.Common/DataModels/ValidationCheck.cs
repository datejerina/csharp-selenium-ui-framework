using System;

namespace AutomationFW.Common.DataModels
{
    public class ValidationCheck<T> where T : TestDataModelBase
    {
        public string CriterionName { get; set; }
        // A method used to perform this single check as part of validation.
        public Func<string, T, bool> ValidationMethod { get; set; }

        /* A bool specifying whether we expect the validation method to return true or false in this instance 
         * of validation.
         * Validation succeeds if ValidationMethod returns the same value as expected result.
         * For example, if we have some criterion named 'should any error message label appear'
         * and ExpectedResult = false, then we are running validation to ensure that no error message label appears.
         */
        public bool? ExpectedResult { get; set; }

        // constructors to aid creation of ValidationCheck objects

        // creating a full ValidationCheck from scratch for single use
        public ValidationCheck(string criterionName, Func<string, T, bool> validationMethod, bool expectedResult)
        {
            CriterionName = criterionName;
            ValidationMethod = validationMethod;
            ExpectedResult = expectedResult;
        }

        // creating a generic template of a validation check for use in making specific validation checks
        public ValidationCheck(string criterionName, Func<string, T, bool> validationMethod)
        {
            CriterionName = criterionName;
            ValidationMethod = validationMethod;
            ExpectedResult = null;
        }

        // creating a specific validation check based off of a generic one
        public ValidationCheck(ValidationCheck<T> existingVC, bool expectedResult)
        {
            CriterionName = existingVC.CriterionName;
            ValidationMethod = existingVC.ValidationMethod;
            ExpectedResult = expectedResult;
        }
    }
}
