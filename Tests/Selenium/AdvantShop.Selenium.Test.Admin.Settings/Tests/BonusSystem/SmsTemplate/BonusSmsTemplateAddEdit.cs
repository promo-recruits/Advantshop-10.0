using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.SmsTemplate
{
    [TestFixture]
    public class BonusSystemSmsTemplateAddEdit : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplateAddEdit\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplateAddEdit\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplateAddEdit\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplateAddEdit\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplateAddEdit\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplateAddEdit\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplateAddEdit\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplateAddEdit\\Bonus.Card.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplateAddEdit\\Bonus.SmsTemplate.csv"
            );
            InitializeService.BonusSystemActive();
            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void SmsTemplateAdd()
        {
            GoToAdmin("smstemplates");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Add\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Добавить шаблон"), "pop up header");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"smsType\"]")))).SelectByText(
                "При смене грейда");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SmsBody\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SmsBody\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SmsBody\"]")).SendKeys("New Sms Template Added");

            Driver.WaitForElemEnabled(By.XPath("//span[contains(text(), 'Добавить')]"));
            Driver.XPathContainsText("span", "Добавить");

            VerifyAreEqual("При смене грейда", Driver.GetGridCell(1, "SmsType").FindElement(By.TagName("a")).Text,
                "sms type added");
            VerifyAreEqual("New Sms Template Added", Driver.GetGridCell(1, "SmsBody").Text, "sms body added");
        }


        [Test]
        public void SmsTemplateEdit()
        {
            GoToAdmin("smstemplates");

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"SmsBody\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SmsBody\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SmsBody\"]")).SendKeys("Edited text");

            Driver.WaitForElemEnabled(By.XPath("//span[contains(text(), 'Сохранить')]"));
            Driver.XPathContainsText("span", "Сохранить");

            VerifyAreEqual("При пополнении бонусов", Driver.GetGridCell(0, "SmsType").FindElement(By.TagName("a")).Text,
                "sms type not edited");
            VerifyAreEqual("Edited text", Driver.GetGridCell(0, "SmsBody").Text, "sms body edited");
        }
    }
}