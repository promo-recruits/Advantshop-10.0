using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Booking.Tests.Affiliate.AffiliateSmsTemplates
{
    [TestFixture]
    public class BookingAffiliateGridTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Booking);
            InitializeService.LoadData(
                "data\\Admin\\SmsTemplates\\Booking.Affiliate.csv",
                "data\\Admin\\SmsTemplates\\Booking.AffiliateSmsTemplate.csv"
            );
            InitializeService.BookingActive();
            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("bookingaffiliate/settings#?settingsTab=smsnotification");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        [Order(0)]
        public void GridFilter() // фильтр грида
        {
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "(1)grid count before");
            Functions.GridFilterSet(Driver, BaseUrl, "StatusName");
            Driver.SetGridFilterSelectValue("StatusName", "Завершена");

            VerifyAreEqual("Найдено записей: 32",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "(1)grid count");
            Functions.GridFilterClose(Driver, BaseUrl, "StatusName");

            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "(1)grid count after");

            Refresh();

            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "(2)grid count before");
            Functions.GridFilterSet(Driver, BaseUrl, "Enabled");
            Driver.SetGridFilterSelectValue("Enabled", "Неактивные");

            VerifyAreEqual("Найдено записей: 17",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "(2)grid count");
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");

            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "(2)grid count after");
        }

        [Test]
        [Order(1)]
        public void GridInplace() // инплейс
        {
            VerifyAreEqual("Text1", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"), "+");
            VerifyAreEqual("true",
                Driver.FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).GetAttribute("value"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            VerifyAreEqual("false",
                Driver.FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).GetAttribute("value"));

            Refresh();

            VerifyAreEqual("false",
                Driver.FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).GetAttribute("value"), "*");
        }

        [Test]
        [Order(2)]
        public void GridSelectAndDelete() // удаление шаблона + массовое + полное
        {
            VerifyAreEqual("Text2", Driver.GetGridCellText(1, "Text", "BookingSmsTemplates"), "1");

            Driver.GetGridCellElement(1, "selectionRowHeaderCol", "BookingSmsTemplates",
                By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(Driver.GetGridCellElement(1, "selectionRowHeaderCol", "BookingSmsTemplates",
                By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"gridBookingSmsTemplates[1][\'_serviceColumn\']\"] ui-grid-custom-delete"))
                .Click();
            Driver.SwalCancel();
            VerifyAreEqual("Text2", Driver.GetGridCellText(1, "Text", "BookingSmsTemplates"));

            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"gridBookingSmsTemplates[1][\'_serviceColumn\']\"] ui-grid-custom-delete"))
                .Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("Text2", Driver.GetGridCellText(1, "Text", "BookingSmsTemplates"));

            Driver.GetGridCellElement(1, "selectionRowHeaderCol", "BookingSmsTemplates",
                By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCellElement(3, "selectionRowHeaderCol", "BookingSmsTemplates",
                By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCellElement(4, "selectionRowHeaderCol", "BookingSmsTemplates",
                By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCellElement(5, "selectionRowHeaderCol", "BookingSmsTemplates",
                By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("Text3", Driver.GetGridCellText(1, "Text", "BookingSmsTemplates"));
            VerifyAreNotEqual("Text5", Driver.GetGridCellText(3, "Text", "BookingSmsTemplates"));
            VerifyAreNotEqual("Text6", Driver.GetGridCellText(4, "Text", "BookingSmsTemplates"));
            VerifyAreNotEqual("Text7", Driver.GetGridCellText(5, "Text", "BookingSmsTemplates"));

            //Driver.GetGridCell(-1, "selectionRowHeaderCol", "BookingSmsTemplates").FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector(".ui-grid-custom-selection-count")).Text);
        }
    }
}