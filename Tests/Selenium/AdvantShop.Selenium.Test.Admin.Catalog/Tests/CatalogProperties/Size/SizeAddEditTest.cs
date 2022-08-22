using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Size
{
    [TestFixture]
    public class SizeAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.ProductCategories.csv"
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
        public void SizeAdd()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeSettingAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeName\"]")).SendKeys("NewSizeName");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeSort\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeSort\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeSort\"]")).SendKeys("10");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            Driver.GridFilterSendKeys("NewSizeName");
            VerifyAreEqual("NewSizeName", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("10", Driver.GetGridCell(0, "SortOrder", "Sizes").Text);
        }

        [Test]
        public void SizeAddCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeSettingAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeName\"]")).SendKeys("NewSizeNameCancel");
            Driver.FindElement(By.XPath("//button[contains(text(), 'Отмена')]")).Click();
            Thread.Sleep(1000);
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            Driver.GridFilterSendKeys("NewSizeNameCancel");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void SizeEdit()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");

            Driver.GridFilterSendKeys("NewSizeName");

            Driver.GetGridCell(0, "_serviceColumn", "Sizes")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Размер", Driver.FindElement(By.TagName("h2")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeName\"]")).SendKeys("Changed");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeSort\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeSort\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SizeSort\"]")).SendKeys("50");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            Driver.GridFilterSendKeys("Changed");
            VerifyAreEqual("Changed", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("50", Driver.GetGridCell(0, "SortOrder", "Sizes").Text);

            Driver.GridFilterSendKeys("NewSizeName");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }
    }
}