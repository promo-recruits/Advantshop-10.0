using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditPhotoDelTest : BaseSeleniumTest
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
        public void ProductEditPhotoDelete()
        {
            //pre check
            GoToClient("products/test-product1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));

            GoToAdmin("product/edit/1");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElement(By.Id("leftAsideProduct")).FindElement(By.TagName("img"))
                .GetAttribute("src").Contains("nophoto"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".italic.hover-padding-left")).Count > 0);

            GoToAdmin("product/edit/1");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"PhotoImg\"]"));

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoImg\"]")));
            a.Perform();
            Driver.FindElement(By.CssSelector(".product-block-state.clearfix"))
                .FindElement(By.CssSelector("[data-e2e=\"PhotoItemDelete\"]")).Click();
            Driver.WaitForElem(By.ClassName("swal2-container"));
            Driver.SwalConfirm();

            //check admin
            GoToAdmin("product/edit/1");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.Id("leftAsideProduct")).FindElement(By.TagName("img"))
                .GetAttribute("src").Contains("nophoto"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".italic.hover-padding-left")).Count > 0);

            //check client
            GoToClient("products/test-product1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));
        }
    }
}