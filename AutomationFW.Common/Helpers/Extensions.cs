using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AutomationFW.Common.Helpers
{
    public static class Extensions
    {
        public static bool ContainsText(this IList<IWebElement> table, string text)
        {
            foreach (var row in table)
            {
                if (row.Text == text)
                {
                    return true;
                }
            }
            return false;
        }

        public static IWebElement FindElementOrNull(this IWebDriver driver, By identifier)
        {
            try
            {
                return driver.FindElement(identifier);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        public static string GetLink(this IWebElement element)
        {
            if (element == null)
            {
                Debug.WriteLine("Get link failed. IWebElement is null.");
                return null;
            }
            return element.GetAttribute("href");
        }

        public static IWebElement SetAttribute(this IWebElement element, string name, string value)
        {
            IWebElement targetElement = element;
            IWrapsDriver wrapsDriver = targetElement as IWrapsDriver;
            if (wrapsDriver == null)
            {
                var wrapsElement = element as IWrapsElement;
                if (wrapsElement == null)
                {
                    throw new InvalidOperationException("webElement does not implement either IWrapsDriver or IWrapsElement");
                }

                targetElement = wrapsElement.WrappedElement;
                wrapsDriver = targetElement as IWrapsDriver;
                if (wrapsDriver == null)
                {
                    throw new InvalidOperationException("webElement wraps another IWebElement, but the wrapped element does not implement IWrapsDriver");
                }
            }
            var driver = wrapsDriver.WrappedDriver;
            var jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", element, name, value);

            return element;
        }

        public static void ScrollToElement(this IWebDriver driver, IWebElement element)
        {
            var jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript("arguments[0].scrollIntoView(true);", element); ;
        }

        public static bool FindElementOrFalse(this IWebDriver driver, By element)
        {
            try
            {
                driver.FindElement(element);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static void RenameReportFile(string filePath, string oldFileName, string newFileName)
        {
            File.Move(filePath + oldFileName, filePath + newFileName);
        }

        public static void ReplaceTextHTMLReportFile(string filePath, string findText, string replaceText)
        {
            try
            {
                StreamReader objReader = new StreamReader(filePath);
                string content = objReader.ReadToEnd();
                objReader.Close();

                content = Regex.Replace(content, findText, replaceText);

                StreamWriter writerObj = new StreamWriter(filePath);
                writerObj.Write(content);
                writerObj.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred. Messgae: " + e.Message);
            }
        }
    }
}
