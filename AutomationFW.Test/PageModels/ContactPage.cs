using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFW.Test.PageModels
{
    public class ContactPage : PageBase
    {
        private IWebElement emailField => _driver.FindElement(By.Id("recipient-email"));
        private IWebElement nameField => _driver.FindElement(By.Id("recipient-name"));
        private IWebElement messageField => _driver.FindElement(By.Id("message-text"));
        private IWebElement submitButton => _driver.FindElement(By.XPath("//*[@id='exampleModal']/div/div/div[3]/button[2]"));
        private IWebElement menuOption => _driver.FindElement(By.XPath("//*[@id ='navbarExample']/ul/li[2]/a"));

        public ContactPage(IWebDriver driver) : base(driver)
        {
            /*emailField = _driver.FindElement(By.Id("recipient-email"));
            nameField = _driver.FindElement(By.Id("recipient-name"));
            messageField = _driver.FindElement(By.Id("message-text"));
            submitButton = _driver.FindElement(By.XPath("//*[@id='exampleModal']/div/div/div[3]/button[2]"));
            menuOption = _driver.FindElement(By.XPath("//*[@id ='navbarExample']/ul/li[2]/a"));
            */             
        }
        public void SelectContact()
        {
            menuOption.Click();
        }
        public void EnterEmailContact(string email)
        {
            emailField.SendKeys(email);
        }
        public void EnterNameContact(string name)
        {
            nameField.SendKeys(name);
        }
        public void EnterMessageContact(string message)
        {
            messageField.SendKeys(message);
        }
        public void FillContactForm(string email, string name, string message)
        {
            emailField.SendKeys(email);
            nameField.SendKeys(name);
            messageField.SendKeys(message);
        }

        public void SendMessage()
        {
            submitButton.Click();
        }
    }
}
