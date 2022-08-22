using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditPhoto360DelTest : BaseSeleniumTest
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
        public void ProductEditPhoto360Delete()
        {
            GoToClient("products/test-product8");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-modal-open=\"modalProductRotate\"]")).Count > 0);

            GoToAdmin("product/edit/8");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"Photo360Img\"]")).Count > 0);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360Input\"]")).Selected);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"imgByHref\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360CheckBox\"]")).Click();
            Thread.Sleep(1000);
            AttachFile(By.XPath("(//input[@type='file'])[3]"),
                GetPicturePath("pics3d\\2.jpg")); //selenium can't upload multiple files
            Thread.Sleep(1000);

            //check after uploading file
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360Input\"]")).Selected);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Photo360Img\"]")).Count == 1);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".aside-image img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsFalse(Driver.PageSource.Contains("Главное фото"));

            GoToAdmin("product/edit/8");

            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"imgByHref\"]"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"imgByHref\"]"));
            Thread.Sleep(1000);

            //check after refreshing page
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360Input\"]")).Selected);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Photo360Img\"]")).Count == 1);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".aside-image img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsFalse(Driver.PageSource.Contains("Главное фото"));

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360Img\"]")));
            a.Perform();
            Driver.FindElement(By.CssSelector(".product-block-state.clearfix"))
                .FindElement(By.CssSelector("[data-e2e=\"Photo360ImgDelete\"]")).Click();
            Driver.WaitForElem(By.ClassName("swal2-container"));
            Driver.SwalConfirm();

            //check admin
            GoToAdmin("product/edit/8");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"Photo360Input\"]"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"Photo360Input\"]")).Selected);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"Photo360Img\"]")).Count > 0);

            //check client
            GoToClient("products/test-product8");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-modal-open=\"modalProductRotate\"]")).Count > 0);
        }
    }
}