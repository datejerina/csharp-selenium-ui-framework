using System;

namespace AutomationFW.Common.DataModels
{
    public class TestData
    {
        public string Account { get; set; }
        public DateTime? OrignalLastPayDate { get; set; }
        public DateTime? ChargeOffDate { get; set; }
        public decimal? DebtorId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        // Only used for json test data if data is changed/consumed by the act of testing it.
        // For example: Starting with an account with no active disputes, then creating a new dispute for it
        //      would make that account unfit for use in that test again.
        public bool? HasBeenUsedUp { get; set; }

        public string GetAllNonNullData(bool truncateAcctNum = false)
        {
            var properties = typeof(TestData).GetProperties();
            string result = "";
            foreach (var property in properties)
            {
                if (property.Name != "HasBeenUsed" && property.GetValue(this) != null)
                {
                    result += $"{property.Name}: ";
                    if (property.Name == "Account" && truncateAcctNum)
                        result += $"{GetFirstSixDigitsOfAccount(property.GetValue(this).ToString())};   ";
                    else
                        result += $"{property.GetValue(this).ToString()};   ";
                }
            }
            return result;
        }

        public string GetFirstSixDigitsOfAccount(string accountNumber)
        {
            if (accountNumber.Length <= 6)
                return accountNumber;
            return accountNumber.Substring(0, 6) + "*";
        }

        // Merges the fields of two test data objects.
        // Only null fields of this TestData object instance will be altered.
        // No non-null data will be overwritten
        public void MergeInOtherTestData(TestData otherTestData)
        {
            var properties = typeof(TestData).GetProperties();
            foreach (var property in properties)
            {
                if (property.GetValue(otherTestData) != null && property.GetValue(this) == null && property.Name != "HasBeenUsed")
                {
                    property.SetValue(this, property.GetValue(otherTestData));
                }
            }
        }
    }
}
