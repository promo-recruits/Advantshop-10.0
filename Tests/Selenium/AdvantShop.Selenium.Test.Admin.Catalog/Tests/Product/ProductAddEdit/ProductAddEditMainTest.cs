using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditMainTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.PropertyGroup.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.Color.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.Tag.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.TagMap.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.Property.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.PropertyValue.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Main\\Catalog.ProductPropertyValue.csv"
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
        public void ProductEditName()
        {
            TestName = "ProductEditName";
            //pre check client
            GoToClient("products/test-product1");

            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".breads")).Text.Contains("TestProduct1"));

            //edit name
            GoToAdmin("product/edit/1");

            VerifyAreEqual("TestProduct1", Driver.FindElement(By.Name("Name")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Товар \"TestProduct1\""));

            Driver.FindElement(By.Name("Name")).Click();
            Driver.FindElement(By.Name("Name")).Clear();
            Driver.FindElement(By.Name("Name")).SendKeys("Edited name 1");
            Driver.XPathContainsText("h2", "Основное");
            Driver.ScrollToTop();

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/1");

            VerifyAreEqual("Edited name 1", Driver.FindElement(By.Name("Name")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Edited name 1"));

            //check client
            GoToClient("products/test-product1");

            VerifyAreEqual("Edited name 1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".breads")).Text.Contains("Edited name 1"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".breads")).Text.Contains("TestProduct1"));
        }

        [Test]
        public void ProductEditArtNo()
        {
            //pre check client
            GoToClient("products/test-product4");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-sku")).Text.Contains("4"));

            //edit name
            GoToAdmin("product/edit/4");

            VerifyAreEqual("4", Driver.FindElement(By.Name("ArtNo")).GetAttribute("value"));

            Driver.FindElement(By.Name("ArtNo")).Click();
            Driver.FindElement(By.Name("ArtNo")).Clear();
            Driver.FindElement(By.Name("ArtNo")).SendKeys("11111");
            Driver.XPathContainsText("h2", "Основное");
            Driver.ScrollToTop();

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/4");
            Driver.ScrollTo(By.XPath("//h2[contains(text(), 'Цена и наличие')]"));
            Driver.GetGridCell(0, "_serviceColumn", "Offers").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();

            //check admin
            GoToAdmin("product/edit/4");

            VerifyAreEqual("11111", Driver.FindElement(By.Name("ArtNo")).GetAttribute("value"));

            //check client
            GoToClient("products/test-product4");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-sku")).Text.Contains("11111"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".details-row.details-sku")).Text.Contains("4"));
        }

        [Test]
        public void ProductEditBarcode()
        {
            GoToAdmin("product/edit/5");

            VerifyAreEqual("BarCodeTest5", Driver.FindElement(By.Name("BarCode")).GetAttribute("value"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ProductCategory\"]"));
            Driver.FindElement(By.Name("BarCode")).Click();
            Driver.FindElement(By.Name("BarCode")).Clear();
            Driver.FindElement(By.Name("BarCode")).SendKeys("55555");
            Driver.XPathContainsText("label", "Штрих код");
            Driver.ScrollToTop();

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/5");

            VerifyAreEqual("55555", Driver.FindElement(By.Name("BarCode")).GetAttribute("value"));
        }

        [Test]
        public void ProductEditBarcodeAdd()
        {
            GoToAdmin("product/edit/101");

            VerifyAreEqual("", Driver.FindElement(By.Name("BarCode")).GetAttribute("value"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ProductCategory\"]"));
            Driver.FindElement(By.Name("BarCode")).Click();
            Driver.FindElement(By.Name("BarCode")).Clear();
            Driver.FindElement(By.Name("BarCode")).SendKeys("123 test");
            Driver.XPathContainsText("label", "Штрих код");
            Driver.ScrollToTop();

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/101");

            VerifyAreEqual("123 test", Driver.FindElement(By.Name("BarCode")).GetAttribute("value"));
        }

        [Test]
        public void ProductEditDoEnabled()
        {
            //pre check client
            VerifyIsTrue(Is404Page("products/test-product2"));

            //edit enabled
            GoToAdmin("product/edit/2");
            VerifyIsFalse(Driver.FindElement(By.Id("Enabled")).Selected);

            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxEnabledClick\"]")).Click();
            Thread.Sleep(1000);
            Driver.ScrollToTop();

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/2");

            VerifyIsTrue(Driver.FindElement(By.Id("Enabled")).Selected);

            //check client
            VerifyIsFalse(Is404Page("products/test-product2"));
        }

        [Test]
        public void ProductEditDoDisabled()
        {
            //pre check client
            VerifyIsFalse(Is404Page("products/test-product6"));

            //edit enabled
            GoToAdmin("product/edit/6");
            VerifyIsTrue(Driver.FindElement(By.Id("Enabled")).Selected);

            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxEnabledClick\"]")).Click();
            Thread.Sleep(1000);
            Driver.ScrollToTop();

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/6");

            VerifyIsFalse(Driver.FindElement(By.Id("Enabled")).Selected);

            //check client
            VerifyIsTrue(Is404Page("products/test-product6"));
        }
    }
}