using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Booking.Tests.Affiliate.AffiliateSmsTemplates
{
    [TestFixture]
    public class GridSmsTemplatesPaginationTest : BaseSeleniumTest
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
        public void PageGridSmsTemplates() //паджинация грида 
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            VerifyAreEqual("Text1", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text10", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Text11", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text20", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Text21", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text30", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Text31", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text40", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Text41", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text50", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Text51", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text60", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));
        }

        [Test]
        public void NextPrevPagination()
        {
            VerifyAreEqual("Text1", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text10", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Text11", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text20", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Text21", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text30", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Text11", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text20", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Text1", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text10", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));
        }

        [Test]
        public void PaginationToEndToBegin()
        {
            VerifyAreEqual("Text1", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text10", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            //to end
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Text111", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text120", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Text1", Driver.GetGridCellText(0, "Text", "BookingSmsTemplates"));
            VerifyAreEqual("Text10", Driver.GetGridCellText(9, "Text", "BookingSmsTemplates"));
        }
    }
}