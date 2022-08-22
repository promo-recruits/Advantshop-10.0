using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Leads
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class LeadsTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\Leads\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\Leads\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\Leads\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\Leads\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\Leads\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\Leads\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\Leads\\CMS.LandingSubBlock.csv"
            );

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            ReInit();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void AddLead()
        {
            GoToFunnelTab(1, "Лиды", By.TagName("ui-grid-custom"));
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.ClassName("ui-grid-empty-text")).Text.Trim(), "default leads");

            ReInitClient();
            GoToClient("lp/addleadfunnel");
            FillFunnelForm();
            Driver.WaitForElem(AdvBy.DataE2E("FormSuccessText"));

            ReInit();
            GoToAdmin("funnels/site/1");
            VerifyAreEqual("1", Driver.FindElement(By.ClassName("lead-events__item__count_label")).Text, "leads count");
            GoToFunnelTab(1, "Лиды", By.TagName("ui-grid-custom"));
            VerifyAreEqual("FirstName", Driver.GetGridCellText(0, "FullName", "FunnelLeads"), "lead name");
            Driver.GetGridCell(0, "Id", "FunnelLeads").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));
            VerifyAreEqual("FirstName", GetInputValue("Lead.FirstName"), "lead name");
            VerifyAreEqual("Воронка продаж \"AddLeadFunnel\"", GetSelectedOptionText("Lead_OrderSourceId"),
                "lead sourse");
        }

        [Test]
        public void AddLeadWithProduct()
        {
            GoToFunnelTab(2, "Лиды", By.TagName("ui-grid-custom"));
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.ClassName("ui-grid-empty-text")).Text.Trim(), "default leads");

            ReInitClient();
            GoToClient("lp/addleadwithproductfunnel");
            FillFunnelForm();
            Driver.WaitForElem(AdvBy.DataE2E("FormSuccessText"));

            ReInit();
            GoToAdmin("funnels/site/2");
            VerifyAreEqual("1", Driver.FindElement(By.ClassName("lead-events__item__count_label")).Text, "leads count");
            GoToFunnelTab(2, "Лиды", By.TagName("ui-grid-custom"));
            VerifyAreEqual("FirstName", Driver.GetGridCellText(0, "FullName", "FunnelLeads"), "lead name");
            Driver.GetGridCell(0, "Id", "FunnelLeads").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));
            VerifyAreEqual("FirstName", GetInputValue("Lead.FirstName"), "lead name");
            VerifyAreEqual("Воронка продаж \"AddLeadWithProductFunnel\"", GetSelectedOptionText("Lead_OrderSourceId"),
                "lead sourse");
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector("[grid-unique-id=\"gridLeadItems\"] [data-e2e=\"gridRow\"]")).Count,
                "products in lead");
            VerifyAreEqual("Рубашка", Driver.GetGridCell(0, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "product name");
        }
    }
}