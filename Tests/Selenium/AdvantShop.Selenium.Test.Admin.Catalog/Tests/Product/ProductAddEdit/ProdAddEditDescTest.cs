using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditDescriptionTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Tag.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.PropertyGroup.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Property.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Color.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Photo.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductPropertyValue.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductCategories.csv"
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
        public void ProductEditChangeDescription()
        {
            GoToAdmin("product/edit/1");

            Driver.FindElement(By.XPath("//div[contains(text(), 'Описание')]")).Click();
            Thread.Sleep(1000);

            Driver.SetCkText("Brief_Description_here", "BriefDescription");
            Driver.SetCkText("Description_here", "Description");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("products/test-product1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-tab-content=\"tabDescription\"]")).Text
                .Contains("Description_here"));
            GoToClient("categories/test-category1");
            VerifyIsTrue(Driver.PageSource.Contains("Brief_Description_here"));
        }

        [Test]
        public void ProductAddDescription()
        {
            GoToAdmin("catalog");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProduct\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"inputProductName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"inputProductName\"]")).SendKeys("new_product");
            Driver.FindElement(By.CssSelector(".modal-dialog .edit")).Click();
            Driver.FindElement(By.CssSelector(".modal-dialog [data-tree-id=\"categoryItemId_2\"]")).Click();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save"))
                .Click(); // вторая кнопка во втором модальном окне
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save"))
                .Click(); // первая кнопка в первом модальном окне
            Thread.Sleep(1000);

            Driver.FindElement(By.XPath("//div[contains(text(), 'Описание')]")).Click();
            Thread.Sleep(1000);

            Driver.SetCkText("NEW_Brief_Description_here", "BriefDescription");
            Driver.SetCkText("NEW_Description_here", "Description");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("products/new_product");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-tab-content=\"tabDescription\"]")).Text
                .Contains("NEW_Description_here"));
            GoToClient("categories/test-category2");
            VerifyIsTrue(Driver.PageSource.Contains("NEW_Brief_Description_here"));
        }
    }
}