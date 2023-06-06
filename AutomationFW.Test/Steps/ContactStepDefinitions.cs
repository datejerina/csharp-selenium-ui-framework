using OpenQA.Selenium;
using AutomationFW.Common.Helpers;
using System;
using TechTalk.SpecFlow;
using AutomationFW.Test.PageModels;
using AutomationFW.Test.FrameworkConfiguration;
using NUnit.Framework;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace AutomationFW.Test.Steps
{
    [Binding]
    public class ContactStepDefinitions
    {
        private IWebDriver _driver;
        private ContactPage _contactPage;
        private FWScenarioDataContext _scenarioDataContext;
        private FWTestRunContext _testRunContext;
        private Waiter _waiter;
        private String actualmessage;

        public ContactStepDefinitions(FWScenarioDataContext sceanrioDatacontext, FWTestRunContext testRunContext)
        {
            _scenarioDataContext = sceanrioDatacontext;
            _testRunContext = testRunContext;
            _driver = _testRunContext.Driver;
            _waiter = new Waiter(_driver);
            _contactPage = new ContactPage(_driver);
        }

        [Given(@"El usuario ingresa a la pagina Store Product")]
        public void GivenElUsuarioIngresaALaPaginaStoreProduct()
        {
            _driver.Navigate().GoToUrl(_testRunContext.Browser.BaseUrl);
        }

        [When(@"El usuario selecciona la opcion de menu Contact")]
        public void WhenElUsuarioSeleccionaLaOpcionDeMenuContact()
        {
            _contactPage.SelectContact();
        }

        [When(@"El usuario ingresa el email ""([^""]*)"" en Contact Email")]
        public void WhenElUsuarioIngresaElEmailEnContactEmail(string email)
        {
            _contactPage.EnterEmailContact(email);
        }

        [When(@"El usuario ingresa el nombre ""([^""]*)"" en Contact Name")]
        public void WhenElUsuarioIngresaElNombreEnContactName(string name)
        {
            _contactPage.EnterNameContact(name);
        }

        [When(@"El usuario ingresa el mensaje ""([^""]*)"" en Message")]
        public void WhenElUsuarioIngresaElMensajeEnMessage(string mensaje)
        {
            _contactPage.EnterMessageContact(mensaje);
        }

        [When(@"El usuario presiona el boton Send Message")]
        public void WhenElUsuarioPresionaElBoton()
        {
            _contactPage.SendMessage();
            if (_contactPage.IsAlertPresent())
            {
                actualmessage = _contactPage.GetAlertText();
                _contactPage.AcceptAlert();
            }
            else
                Assert.Fail("Alert message is not present");
        }

        [Then(@"El usuario recibe el mensaje ""([^""]*)""")]
        public void ThenElUsuarioRecibeLaConfirmacion(string expectedMessage)
        {
            Assert.That(actualmessage, Is.EqualTo(expectedMessage));

        }

    }
}
