using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Tag
{
    [TestFixture]
    public class TagPaginationAndView : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Catalog\\Tag\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.Tag.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.TagMap.csv"
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
        public void Present10Page()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("New_Tag11", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag20", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("New_Tag21", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag30", Driver.GetGridCell(9, "Name", "Tags").Text);
        }

        [Test]
        public void Present10PageToNext()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("New_Tag11", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag20", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("New_Tag21", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag30", Driver.GetGridCell(9, "Name", "Tags").Text);
        }

        [Test]
        public void Present10PageToPrevious()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("New_Tag11", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag20", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
        }

        [Test]
        public void Present10PageToEnd()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
            //to end
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("New_Tag101", Driver.GetGridCell(0, "Name", "Tags").Text);
        }

        [Test]
        public void Present10PageToBegin()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("New_Tag11", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag20", Driver.GetGridCell(9, "Name", "Tags").Text);
            Thread.Sleep(1000);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("New_Tag21", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag30", Driver.GetGridCell(9, "Name", "Tags").Text);

            //to begin
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
        }
    }
}