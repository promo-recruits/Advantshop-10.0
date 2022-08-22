using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Tag
{
    [TestFixture]
    public class TagSort : BaseSeleniumTest
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
        public void SortByName()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            Driver.GetGridCell(-1, "Name", "Tags").Click();
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag12", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.GetGridCell(-1, "Name", "Tags").Click();
            VerifyAreEqual("New_Tag99", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag90", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.GetGridCell(-1, "Name", "Tags").Click();
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
        }

        [Test]
        public void SortByUrl()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            Driver.GetGridCell(-1, "UrlPath", "Tags").Click();
            VerifyAreEqual("teg1", Driver.GetGridCell(0, "UrlPath", "Tags").Text);
            VerifyAreEqual("teg12", Driver.GetGridCell(9, "UrlPath", "Tags").Text);
            Driver.GetGridCell(-1, "UrlPath", "Tags").Click();
            VerifyAreEqual("teg99", Driver.GetGridCell(0, "UrlPath", "Tags").Text);
            VerifyAreEqual("teg90", Driver.GetGridCell(9, "UrlPath", "Tags").Text);
            Driver.GetGridCell(-1, "UrlPath", "Tags").Click();
            VerifyAreEqual("teg1", Driver.GetGridCell(0, "UrlPath", "Tags").Text);
            VerifyAreEqual("teg10", Driver.GetGridCell(9, "UrlPath", "Tags").Text);
        }

        [Test]
        public void SortByActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GetGridCell(-1, "Enabled", "Tags").Click();
            VerifyAreEqual("New_Tag102", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag6", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.GetGridCell(-1, "Enabled", "Tags").Click();
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
            Driver.GetGridCell(-1, "Enabled", "Tags").Click();
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
        }
    }
}