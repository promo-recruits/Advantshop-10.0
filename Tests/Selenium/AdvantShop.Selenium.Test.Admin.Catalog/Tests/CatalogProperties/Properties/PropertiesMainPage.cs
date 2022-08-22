using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesMainPage : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Properties\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Properties\\Catalog.Category.csv",
                "Data\\Admin\\Properties\\Catalog.Brand.csv",
                "Data\\Admin\\Properties\\Catalog.Property.csv",
                "Data\\Admin\\Properties\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Properties\\Catalog.Product.csv",
                "Data\\Admin\\Properties\\Catalog.Offer.csv",
                "Data\\Admin\\Properties\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\Properties\\Catalog.ProductCategories.csv"
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
        public void OpenProperties()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            VerifyIsTrue(Driver.PageSource.Contains("Group1"));
        }

        [Test]
        public void OpenPropertiesWithoutGroup()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Свойства без группы')]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Property100", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property101", Driver.GetGridCell(1, "Name").Text);
        }

        [Test]
        public void OpenAllProperties()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.FindElements(By.CssSelector(".as-sortable-item a"))[0].Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void OpenPropertiesInGroup()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Group1')]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Group1", Driver.FindElement(By.CssSelector("[data-e2e=\"PropertySettingTitle\"]")).Text);
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property9", Driver.GetGridCell(8, "Name").Text);
            Driver.FindElement(By.XPath("//a[contains(text(), 'Group2')]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Group2", Driver.FindElement(By.CssSelector("[data-e2e=\"PropertySettingTitle\"]")).Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property19", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void SearchPropertiesAll()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridFilterSendKeys("Property1");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property18", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual(10, Driver.FindElements(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Count);
        }

        [Test]
        public void SearchPropertiesInGroup()
        {
            GoToAdmin("settingscatalog?groupId=1");
            Driver.GridFilterSendKeys("Property1");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Count == 1);
        }
    }
}