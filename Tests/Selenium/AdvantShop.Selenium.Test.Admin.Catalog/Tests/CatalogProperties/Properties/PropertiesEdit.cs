using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesEdit : BaseSeleniumTest
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
        public void EditapenWindow()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'_serviceColumn\']\"] a")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            VerifyAreEqual("Property1", Driver.FindElement(By.CssSelector(".col-xs-9 input")).GetAttribute("value"));
            VerifyAreEqual("DescriptionProperty1",
                Driver.FindElement(By.CssSelector(".col-xs-9 textarea")).GetAttribute("value"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".adv-checkbox-label input"))[0].Selected);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".adv-checkbox-label input"))[1].Selected);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".adv-checkbox-label input"))[2].Selected);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".adv-checkbox-label input"))[3].Selected);
            VerifyAreEqual("1", Driver.FindElements(By.CssSelector(".col-xs-9 input"))[7].GetAttribute("value"));
        }

        [Test]
        public void EditCheckBoxProperties()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'_serviceColumn\']\"] a")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".adv-checkbox-label")).Click();
            Driver.FindElements(By.CssSelector(".adv-checkbox-label"))[1].Click();
            var element = Driver.FindElements(By.CssSelector(".col-xs-9 input"))[7];
            IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            Thread.Sleep(1000);

            Driver.FindElements(By.CssSelector(".adv-checkbox-label"))[2].Click();
            Driver.FindElements(By.CssSelector(".adv-checkbox-label"))[3].Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"grid[0][\'UseInFilter\']\"] [data-e2e=\"switchOnOffLabel\"] input"))
                .Selected);
            VerifyIsFalse(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"grid[0][\'UseInDetails\']\"] [data-e2e=\"switchOnOffLabel\"] input"))
                .Selected);
            VerifyIsFalse(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"grid[0][\'UseInBrief\']\"] [data-e2e=\"switchOnOffLabel\"] input"))
                .Selected);
        }

        [Test]
        public void EditProperty()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'_serviceColumn\']\"] a")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).Clear();
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).SendKeys("New_name");
            Driver.SetCkText("New_description", "editor1");
            (new SelectElement(Driver.FindElements(By.CssSelector(".col-xs-9 select"))[1])).SelectByText("Group2");
            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-9 select")))).SelectByText(
                "Раскрывающийся список (селект)");
            Driver.FindElements(By.CssSelector(".col-xs-9 input"))[7].Clear();
            Driver.FindElements(By.CssSelector(".col-xs-9 input"))[7].SendKeys("10");

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Property2", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Group1", Driver.GetGridCell(0, "GroupName").Text);
            VerifyAreEqual("New_name", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("Group2", Driver.GetGridCell(9, "GroupName").Text);
            Driver.GridFilterSendKeys("New_name");
            VerifyAreEqual("New_name", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Group2", Driver.GetGridCell(0, "GroupName").Text);
            GoToAdmin("settingscatalog?groupId=2");
            VerifyAreEqual("New_name", Driver.GetGridCell(0, "Name").Text);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'_serviceColumn\']\"] a")).Click();
            Driver.WaitForModal();
            Driver.AssertCkText("New_description", "editor1");
        }
    }
}