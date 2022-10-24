using AutomationFW.Common.DataModels;
using AutomationFW.Common.Helpers.Interfaces;

namespace AutomationFW.Common.Helpers
{
    public abstract class ScenarioDataContextBase : IScenarioDataContext
    {
        public string GetAccountFieldValue()
        {
            var properties = GetType().GetProperties(
                System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.DeclaredOnly);

            foreach (var property in properties)
            {
                if (property.GetValue(this) == null)
                {
                    continue;
                }

                if (property.PropertyType.IsSubclassOf(typeof(TestDataModelBase)))
                {
                    string result = property.PropertyType.GetMethod(nameof(this.GetAccountFieldValue))
                        .Invoke(property.GetValue(this), null)?.ToString();

                    if (result != null)
                    {
                        return result;
                    }
                }
                else if (property.Name == "Account")
                {
                    return property.GetValue(this).ToString();
                }
            }

            return null;
        }

        public string GetAllTestDataAsString(bool maskAcctNum = false)
        {
            var properties = GetType().GetProperties(
                System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.DeclaredOnly);

            string result = "";

            foreach (var property in properties)
            {
                if (property.GetValue(this) == null)
                {
                    continue;
                }

                if (property.PropertyType.IsSubclassOf(typeof(TestDataModelBase)))
                {
                    result += $"{property.Name} {{ ";

                    result += property.PropertyType.GetMethod(nameof(this.GetAllTestDataAsString))
                        .Invoke(property.GetValue(this), new object[] { maskAcctNum });

                    result += " };  ";
                }
                else
                {
                    result += $"{property.Name}: ";

                    if (property.Name == "Account" && maskAcctNum)
                    {
                        result += $"{maskAccountNumber(property.GetValue(this).ToString())};   ";
                    }
                    else
                    {
                        result += $"{property.GetValue(this).ToString()};   ";
                    }
                }
            }

            return result;
        }

        private string maskAccountNumber(string accountNumber, int lengthToKeep = 6)
        {
            if (accountNumber.Length <= lengthToKeep)
            {
                return accountNumber;
            }

            string maskedAccountNumber = accountNumber.Substring(0, lengthToKeep);

            for (int i = lengthToKeep; i < accountNumber.Length; i++)
            {
                maskedAccountNumber += "*";
            }

            return maskedAccountNumber;
        }
    }
}