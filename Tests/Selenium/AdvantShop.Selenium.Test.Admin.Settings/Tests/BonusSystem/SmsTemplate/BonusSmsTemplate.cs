using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.SmsTemplate
{
    [TestFixture]
    public class BonusSystemSmsTemplate : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplate\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplate\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplate\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplate\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplate\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplate\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplate\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplate\\Bonus.Card.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsTemplate\\Bonus.SmsTemplate.csv"
            );
            InitializeService.BonusSystemActive();
            Init();

            GoToAdmin("smstemplates");
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
        public void SmsTemplateGrid()
        {
            VerifyAreEqual("При продаже", Driver.GetGridCell(0, "SmsType").FindElement(By.TagName("a")).Text,
                "sms type line 1");
            VerifyAreEqual("Sms body 1", Driver.GetGridCell(0, "SmsBody").Text, "sms body line 1");

            VerifyAreEqual("При пополнении бонусов", Driver.GetGridCell(1, "SmsType").FindElement(By.TagName("a")).Text,
                "sms type line 2");
            VerifyAreEqual("Sms body 2", Driver.GetGridCell(1, "SmsBody").Text, "sms body line 2");

            VerifyAreEqual("При списании бонусов", Driver.GetGridCell(2, "SmsType").FindElement(By.TagName("a")).Text,
                "sms type line 3");
            VerifyAreEqual("Sms body 3", Driver.GetGridCell(2, "SmsBody").Text, "sms body line 3");

            VerifyAreEqual("При смене грейда", Driver.GetGridCell(3, "SmsType").FindElement(By.TagName("a")).Text,
                "sms type line 4");
            VerifyAreEqual("Sms body 4", Driver.GetGridCell(3, "SmsBody").Text, "sms body line 4");

            VerifyAreEqual("Отмена продажи", Driver.GetGridCell(4, "SmsType").FindElement(By.TagName("a")).Text,
                "sms type line 5");
            VerifyAreEqual("Sms body 5", Driver.GetGridCell(4, "SmsBody").Text, "sms body line 5");

            VerifyAreEqual("Аннулирование баллов", Driver.GetGridCell(5, "SmsType").FindElement(By.TagName("a")).Text,
                "sms type line 6");
            VerifyAreEqual("Sms body 6", Driver.GetGridCell(5, "SmsBody").Text, "sms body line 6");
        }

        [Test]
        public void SmsTemplatezSelectDelete()
        {
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("При продаже", Driver.GetGridCell(0, "SmsType").FindElement(By.TagName("a")).Text,
                "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("При продаже", Driver.GetGridCell(0, "SmsType").FindElement(By.TagName("a")).Text,
                "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("При смене грейда", Driver.GetGridCell(0, "SmsType").FindElement(By.TagName("a")).Text,
                "selected 2 grid delete");
            VerifyAreEqual("Отмена продажи", Driver.GetGridCell(1, "SmsType").FindElement(By.TagName("a")).Text,
                "selected 3 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(2, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 3 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            GoToAdmin("smstemplates");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
        }
    }
}