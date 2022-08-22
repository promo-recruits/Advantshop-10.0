using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Booking.Tests.Affiliate.AffiliateSmsTemplates
{
    [TestFixture]
    public class BookingAffiliateSmsTemplatesTest : BaseSeleniumTest
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
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void SendSmsBeforeStartBooiking() //шаблон смс до бронирования
        {
            GoToAdmin("bookingaffiliate/settings#?settingsTab=smsnotification");

            Driver.FindElement(By.CssSelector("[data-e2e=\"noSendSmsBeforeStartBooiking\"]")).Click();

            Driver.FindElement(By.Id("Affiliate_ForHowManyMinutesToSendSms")).Click();
            Driver.FindElement(By.Id("Affiliate_ForHowManyMinutesToSendSms")).Clear();
            Driver.FindElement(By.Id("Affiliate_ForHowManyMinutesToSendSms")).SendKeys("60");

            Driver.FindElement(By.Id("Affiliate_SmsTemplateBeforeStartBooiking")).Click();
            Driver.FindElement(By.Id("Affiliate_SmsTemplateBeforeStartBooiking")).Clear();
            Driver.FindElement(By.Id("Affiliate_SmsTemplateBeforeStartBooiking")).SendKeys("Sms template");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();

            GoToAdmin("bookingaffiliate/settings#?settingsTab=smsnotification");

            VerifyAreEqual("60",
                Driver.FindElement(By.Id("Affiliate_ForHowManyMinutesToSendSms")).GetAttribute("value"),
                "save success");
        }

        [Test]
        public void SmsTemplatesAddEdit() //добавление смс шаблона при смене статуса брони + редактирование + поиск
        {
            GoToAdmin("bookingaffiliate/settings#?settingsTab=smsnotification");

            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "grid count all before");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddSmsTemplate\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));

            Driver.FindElement(By.CssSelector(".ui-select-match"))
                .FindElement(By.CssSelector(".btn.btn-default.form-control.ui-select-toggle")).Click();
            Driver.XPathContainsText("div", "Подтверждена");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BookingSmsTemplateText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BookingSmsTemplateText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BookingSmsTemplateText\"]")).SendKeys("OldSmsTemplateText");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary.ladda-button")).Click();

            VerifyAreEqual("Найдено записей: 121",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "grid count all after");

            Driver.GridFilterSendKeys("OldSmsTemplateText", "h1");

            VerifyAreEqual("Подтверждена",
                Driver.GetGridCellElement(0, "StatusName", "BookingSmsTemplates",
                    By.CssSelector(".ui-grid-cell-contents")).Text, "template status before");
            VerifyAreEqual("OldSmsTemplateText", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"),
                "template text before");
            VerifyIsTrue(
                Driver.GetGridCellElement(0, "Enabled", "BookingSmsTemplates",
                    By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected, "template is switchOn before");

            Driver.ClearToastMessages();
            Driver.GetGridCellElement(0, "_serviceColumn", "BookingSmsTemplates", By.TagName("a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));

            Driver.FindElement(By.CssSelector(".ui-select-match"))
                .FindElement(By.CssSelector(".btn.btn-default.form-control.ui-select-toggle")).Click();
            Driver.XPathContainsText("div", "Отменена");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BookingSmsTemplateText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BookingSmsTemplateText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BookingSmsTemplateText\"]")).SendKeys("NewSmsTemplateText");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BookingSmsTemplateEnabled\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary.ladda-button")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();

            Refresh();
            Driver.GridFilterSendKeys("NewSmsTemplateText", "h1");

            Driver.WaitForElem(By.CssSelector("[data-e2e-grid-cell=\"gridBookingSmsTemplates[0]['StatusName']\"]"));
            VerifyAreEqual("Отменена",
                Driver.GetGridCellElement(0, "StatusName", "BookingSmsTemplates",
                    By.CssSelector(".ui-grid-cell-contents")).Text, "template status after");
            VerifyAreEqual("NewSmsTemplateText", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"),
                "template text after");
            VerifyIsFalse(
                Driver.GetGridCellElement(0, "Enabled", "BookingSmsTemplates",
                    By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected, "template is switchOff after");

            Driver.GridFilterSendKeys("OldSmsTemplateText", "h1");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "template is not exist");
        }
    }
}

//изменение интервала бронирования (простой вариант)
/*GoToAdmin("bookingaffiliate/settings#?settingsTab=timeofbooking");
new SelectElement(driver.FindElement(By.Id("Affiliate_BookingIntervalMinutes"))).SelectByText("1 ч");
Thread.Sleep(2000);
Driver.WaitForElem(By.CssSelector(".swal2-popup.swal2-modal"));
Driver.SwalConfirm();
Thread.Sleep(2000);

VerifyAreEqual("00:00-01:00", driver.FindElement(By.CssSelector(".ui-selectable .adv-checkbox-label")).Text, "booking interval");*/