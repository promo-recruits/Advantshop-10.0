using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditMainAddBrandTest : BaseSeleniumTest
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
        public void ProductEditAddFirstBrand()
        {
            GoToAdmin("product/edit/1");
            VerifyAreEqual("Не выбран", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count > 0);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBrand\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Производители", Driver.FindElement(By.TagName("h2")).Text);
            Driver.GetGridCell(0, "BrandName", "Brands").FindElement(By.XPath("//a[contains(text(), 'Выбрать')]"))
                .Click();
            Refresh();

            //check admin
            VerifyAreEqual("BrandName1", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count == 1);

            //check client
            GoToClient("products/test-product1");
            VerifyIsTrue(Driver.PageSource.Contains("BrandName1"));
        }

        [Test]
        public void ProductEditAddBrandviaSearch()
        {
            GoToAdmin("product/edit/4");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBrand\"]")).Click();
            Driver.WaitForModal();
            Driver.GridFilterSendKeys("BrandName10");
            VerifyAreEqual("BrandName10", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            Driver.GetGridCell(0, "BrandName", "Brands").FindElement(By.XPath("//a[contains(text(), 'Выбрать')]"))
                .Click();
            Refresh();

            //check admin
            VerifyAreEqual("BrandName10", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);

            //check client
            GoToClient("products/test-product4");
            VerifyIsTrue(Driver.PageSource.Contains("BrandName10"));
        }

        [Test]
        public void ProductEditAddBrandUsingPage()
        {
            GoToAdmin("product/edit/5");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBrand\"]")).Click();
            Driver.WaitForModal();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.GetGridCell(0, "BrandName", "Brands").FindElement(By.XPath("//a[contains(text(), 'Выбрать')]"))
                .Click();
            Refresh();

            //check admin
            VerifyAreEqual("BrandName22", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);

            //check client
            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.PageSource.Contains("BrandName22"));
        }

        [Test]
        public void ProductEditAddDisabledBrand()
        {
            GoToAdmin("product/edit/6");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBrand\"]")).Click();
            Driver.WaitForModal();
            Driver.GridFilterSendKeys("BrandName3");
            VerifyAreEqual("BrandName3", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            Driver.GetGridCell(0, "BrandName", "Brands").FindElement(By.XPath("//a[contains(text(), 'Выбрать')]"))
                .Click();
            Refresh();

            //check admin
            VerifyAreEqual("BrandName3", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);

            //check client
            GoToClient("products/test-product6");
            VerifyIsFalse(Driver.PageSource.Contains("BrandName3"));
        }

        [Test]
        public void ProductEditAddBrandElse()
        {
            GoToAdmin("product/edit/7");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBrand\"]")).Click();
            Driver.WaitForModal();
            Driver.GetGridCell(0, "BrandName", "Brands").FindElement(By.XPath("//a[contains(text(), 'Выбрать')]"))
                .Click();
            Refresh();

            //check admin
            VerifyAreEqual("BrandName1", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count == 1);

            //choose another brand
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBrand\"]")).Click();
            Driver.WaitForModal();
            Driver.GridFilterSendKeys("BrandName60");
            VerifyAreEqual("BrandName60", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            Driver.GetGridCell(0, "BrandName", "Brands").FindElement(By.XPath("//a[contains(text(), 'Выбрать')]"))
                .Click();

            //check admin
            GoToAdmin("product/edit/7");
            VerifyAreEqual("BrandName60", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count == 1);

            //check client
            GoToClient("products/test-product7");
            VerifyIsTrue(Driver.PageSource.Contains("BrandName60"));
        }

        [Test]
        public void ProductEditAddBrandDelete()
        {
            GoToAdmin("product/edit/53");

            //check admin
            VerifyAreEqual("BrandName12", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count == 1);

            //check delete brand
            Driver.FindElement(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Click();
            Thread.Sleep(1000);

            //check admin
            GoToAdmin("product/edit/53");
            VerifyAreEqual("Не выбран", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count > 0);

            //check brand grid
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName12");
            VerifyAreEqual("BrandName12", Driver.GetGridCell(0, "BrandName").Text);

            //check client
            GoToClient("products/test-product53");
            VerifyIsFalse(Driver.PageSource.Contains("BrandName12"));
        }

        [Test]
        public void ProductzEditAddBrandDeleteFromGrid()
        {
            GoToAdmin("product/edit/50");

            VerifyAreEqual("BrandName10", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count == 1);

            //delete brand from grid
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName10");
            VerifyAreEqual("BrandName10", Driver.GetGridCell(0, "BrandName").Text);
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();

            //check admin
            GoToAdmin("product/edit/50");
            VerifyAreEqual("Не выбран", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count > 0);

            //check client
            GoToClient("products/test-product50");
            VerifyIsFalse(Driver.PageSource.Contains("BrandName10"));
        }

        [Test]
        public void ProductEditAddBrandToDisabled()
        {
            GoToAdmin("product/edit/51");

            VerifyAreEqual("BrandName11", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count == 1);

            //brand do disabled
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName11");
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);

            //check admin
            GoToAdmin("product/edit/51");
            VerifyAreEqual("BrandName11", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count == 1);

            //check client
            GoToClient("products/test-product51");
            VerifyIsFalse(Driver.PageSource.Contains("BrandName11"));
        }

        [Test]
        public void ProductEditAddBrandToEnabled()
        {
            GoToAdmin("product/edit/52");

            VerifyAreEqual("BrandName2", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count == 1);

            //brand do disabled
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName2");
            VerifyAreEqual("BrandName2", Driver.GetGridCell(0, "BrandName").Text);
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);

            //check admin
            GoToAdmin("product/edit/52");
            VerifyAreEqual("BrandName2", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductBrandName\"]")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"DeleteBrand\"]")).Count == 1);

            //check client
            GoToClient("products/test-product52");
            VerifyIsTrue(Driver.PageSource.Contains("BrandName2"));
        }
    }
}