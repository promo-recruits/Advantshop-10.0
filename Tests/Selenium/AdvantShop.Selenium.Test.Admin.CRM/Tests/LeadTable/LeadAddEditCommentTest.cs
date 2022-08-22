using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadAddEditCommentTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.TaskGroup.csv",
                //  "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadCurrency.csv",
                // "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadItem.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CMS.ChangeHistory.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CMS.AdminComment.csv"
            );

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
        public void CommentAdd()
        {
            GoToAdmin("leads#?leadIdInfo=11");

            Driver.ScrollTo(By.TagName("lead-events"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeComment\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEvent\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEvent\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEvent\"]")).SendKeys("new comment test");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventAdd\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text.Contains("new comment test"),
                "added comment text");

            GoToAdmin("leads#?leadIdInfo=11");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeComment\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text.Contains("new comment test"),
                "added comment text after refresh");
        }

        [Test]
        public void CommentEdit()
        {
            GoToAdmin("leads#?leadIdInfo=6");

            Driver.ScrollTo(By.TagName("lead-events"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeComment\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text.Contains("TestComment6"),
                "pre check lead comment");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadCommentEdit\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadCommentText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadCommentText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadCommentText\"]")).SendKeys("comment edited");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadCommentEditSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text.Contains("comment edited"),
                "comment edited");
            VerifyIsFalse(Driver.PageSource.Contains("TestComment6"), "comment edited - no prev comment text");

            GoToAdmin("leads#?leadIdInfo=6");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeComment\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text.Contains("comment edited"),
                "comment edited after refresh");
            VerifyIsFalse(Driver.PageSource.Contains("TestComment6"),
                "comment edited - no prev comment text after refresh");
        }

        [Test]
        public void CommentDelete()
        {
            GoToAdmin("leads#?leadIdInfo=7");

            Driver.ScrollTo(By.TagName("lead-events"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeComment\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text.Contains("TestComment7"),
                "pre check lead comment");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadCommentDelete\"]")).Click();
            Driver.SwalConfirm();
            Driver.WaitForToastSuccess();
            Thread.Sleep(1000); //потому что пропадает удаленный комментарий с задержкой

            VerifyIsFalse(Driver.PageSource.Contains("TestComment7"), "comment deleted");
            VerifyIsTrue(Driver.PageSource.Contains("Ничего не найдено"), "comment deleted text");

            GoToAdmin("leads#?leadIdInfo=7");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeComment\"]")).Click();
            VerifyIsFalse(Driver.PageSource.Contains("TestComment7"), "comment deleted after refreshing");
            VerifyIsTrue(Driver.PageSource.Contains("Ничего не найдено"), "comment deleted text after refreshing");
        }
    }
}