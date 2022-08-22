using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsTemplate
{
    [TestFixture]
    public class SettingsTemplateProductCartTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Property.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductExt.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Tag.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductPropertyValue.csv"
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
        public void DisplayWeightOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.CheckSelected("DisplayWeight", "SettingsTemplateSave");

            GoToClient("products/apple_iphone_4s_64gb_chernyi");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-weight")).Displayed, "display weight");
        }

        [Test]
        public void DisplayWeightOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.CheckNotSelected("DisplayWeight", "SettingsTemplateSave");

            GoToClient("products/apple_iphone_4s_64gb_chernyi");

            VerifyIsFalse(Driver.FindElements(By.CssSelector(".details-row.details-weight")).Count > 0,
                "display weight");
        }

        [Test]
        public void DisplayDimensionsOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.CheckSelected("DisplayDimensions", "SettingsTemplateSave");

            GoToClient("products/smartfon-apple-iphone-7-32gb-chernyi");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-dimensions")).Displayed,
                "display dimensions");
        }

        [Test]
        public void DisplayDimensionsOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.CheckNotSelected("DisplayDimensions", "SettingsTemplateSave");

            GoToClient("products/smartfon-apple-iphone-7-32gb-chernyi");

            VerifyIsFalse(Driver.FindElements(By.CssSelector(".details-row.details-dimensions")).Count > 0,
                "display dimensions");
        }

        [Test]
        public void ShowStockAvailabilityOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.CheckSelected("ShowStockAvailability", "SettingsTemplateSave");

            GoToClient("products/ipd35");
            Thread.Sleep(2000);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".availability.available")).Text.Contains("Есть в наличии (8 шт.)"),
                "show stock availability");
        }

        [Test]
        public void ShowStockAvailabilityOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.CheckNotSelected("ShowStockAvailability", "SettingsTemplateSave");

            GoToClient("products/ipd35");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".availability.available")).Text.Contains("Есть в наличии (8 шт.)"),
                "show stock availability");
        }
    }

    [TestFixture]
    public class SettingsTemplateProductCartPhotoTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Property.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductExt.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Tag.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductPropertyValue.csv"
            );
            Init();
            uploadImg();
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
        public void EnableZoomOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.CheckSelected("EnableZoom", "SettingsTemplateSave");

            GoToClient("products/ipd35");

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElement(By.CssSelector(".gallery-block")).FindElement(By.TagName("img")));
            a.Perform();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".zoomer-window")).Displayed, "enable zoom");
        }

        [Test]
        public void EnableZoomOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.CheckNotSelected("EnableZoom", "SettingsTemplateSave");

            GoToClient("products/ipd35");

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElement(By.CssSelector(".gallery-block")).FindElement(By.TagName("img")));
            a.Perform();

            VerifyIsFalse(Driver.FindElements(By.CssSelector(".zoomer-window")).Count > 0, "enable zoom");
        }

        public void uploadImg()
        {
            GoToAdmin("product/edit/35");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Фотографии')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"PhotoImg\"]"));
            Thread.Sleep(2000);
            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PhotoImg\"]")));
            a.Perform();
            Driver.FindElement(By.CssSelector(".product-block-state.clearfix"))
                .FindElement(By.CssSelector("[data-e2e=\"PhotoItemDelete\"]")).Click();
            Driver.WaitForElem(By.ClassName("swal2-container"));
            Driver.SwalConfirm();

            AttachFile(By.CssSelector("input[type=\"file\"]"), GetPicturePath("big.png"));
            Thread.Sleep(2000);
        }
    }

    [TestFixture]
    public class SettingsTemplateProductCartReviewsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Property.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductExt.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Tag.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductPropertyValue.csv"
            );
            Init();
            Functions.AdminSettingsReviewsImgUploadingOn(Driver, BaseUrl);
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"displayReviewsImage\"]"))
                .FindElement(By.Id("DisplayReviewsImage")).Selected)
            {
                var element = Driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
                IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
                jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector("[data-e2e=\"displayReviewsImage\"]")).FindElement(By.TagName("span"))
                    .Click();
                Thread.Sleep(1000);
                //element = driver.FindElement(By.Id("header-top"));
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
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
        public void ReviewImageWidthHeight()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");
            Driver.ScrollTo(By.Id("AllowReviews"));

            Driver.FindElement(By.Id("ReviewImageWidth")).Click();
            Driver.FindElement(By.Id("ReviewImageWidth")).Clear();
            Driver.FindElement(By.Id("ReviewImageWidth")).SendKeys("300");

            Driver.FindElement(By.Id("ReviewImageHeight")).Click();
            Driver.FindElement(By.Id("ReviewImageHeight")).Clear();
            Driver.FindElement(By.Id("ReviewImageHeight")).SendKeys("200");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            VerifyAreEqual("300", Driver.FindElement(By.Id("ReviewImageWidth")).GetAttribute("value"),
                "review image width admin value");
            VerifyAreEqual("200", Driver.FindElement(By.Id("ReviewImageHeight")).GetAttribute("value"),
                "review image height admin value");

            //upload img
            GoToClient("products/dress1?tab=tabReviews");
            Driver.ScrollTo(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before"));

            AttachFile(By.XPath("(//input[@type='file'])[1]"), GetPicturePath("big.png"));


            Driver.FindElement(By.Name("reviewsFormName")).Click();
            Driver.FindElement(By.Name("reviewsFormName")).Clear();
            Driver.FindElement(By.Name("reviewsFormName")).SendKeys("Review Name");

            Driver.FindElement(By.Name("reviewsFormEmail")).Click();
            Driver.FindElement(By.Name("reviewsFormEmail")).Clear();
            Driver.FindElement(By.Name("reviewsFormEmail")).SendKeys("review@mail.test");

            Driver.FindElement(By.Name("reviewFormText")).Click();
            Driver.FindElement(By.Name("reviewFormText")).Clear();
            Driver.FindElement(By.Name("reviewFormText")).SendKeys("Review Text");

            Driver.FindElement(By.Name("reviewSubmit")).Click();
            Thread.Sleep(2000);

            //check client
            GoToClient("products/dress1?tab=tabReviews");
            Driver.ScrollTo(By.Id("tabReviews"));

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".reviews")).FindElement(By.CssSelector(".review-item__photo-item"))
                    .GetAttribute("style").Contains("flex-basis: 300px"), "uploaded img width");
        }
    }

    [TestFixture]
    public class SettingsTemplateProductCartShippingsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Property.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductExt.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Tag.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductPropertyValue.csv"
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
        public void ShippingsInProductCartShowNever()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.ScrollTo(By.Id("ReviewImageWidth"));
            (new SelectElement(Driver.FindElement(By.Id("ShowShippingsMethodsInDetails")))).SelectByText("Никогда");

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Enabled)
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");
            IWebElement selectShippingsInDetails = Driver.FindElement(By.Id("ShowShippingsMethodsInDetails"));
            SelectElement select = new SelectElement(selectShippingsInDetails);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Никогда"), "show shippings in product cart admin");

            //check client
            GoToClient("products/ipd35");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Доставка в") &&
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Самовывоз") &&
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Курьером"),
                "show shippings in product cart client");
        }

        [Test]
        public void ShippingsInProductCartShowAlways()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.ScrollTo(By.Id("ReviewImageWidth"));
            (new SelectElement(Driver.FindElement(By.Id("ShowShippingsMethodsInDetails")))).SelectByText("Всегда");

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Enabled)
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");
            IWebElement selectShippingsInDetails = Driver.FindElement(By.Id("ShowShippingsMethodsInDetails"));
            SelectElement select = new SelectElement(selectShippingsInDetails);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Всегда"), "show shippings in product cart admin");

            //check client
            GoToClient("products/ipd35");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Доставка в") &&
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Самовывоз") &&
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Курьером"),
                "show shippings in product cart client");
        }

        [Test]
        public void ShippingsInProductCartShowByClick()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.ScrollTo(By.Id("ReviewImageWidth"));
            (new SelectElement(Driver.FindElement(By.Id("ShowShippingsMethodsInDetails")))).SelectByText("По клику");

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Enabled)
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");
            IWebElement selectShippingsInDetails = Driver.FindElement(By.Id("ShowShippingsMethodsInDetails"));
            SelectElement select = new SelectElement(selectShippingsInDetails);
            VerifyIsTrue(select.SelectedOption.Text.Contains("По клику"), "show shippings in product cart admin");

            //check client
            GoToClient("products/ipd35");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Доставка в") &&
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Самовывоз") &&
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Курьером"),
                "show shippings in product cart client by click");
            Driver.FindElement(By.LinkText("Рассчитать стоимость доставки")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Доставка в") &&
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Самовывоз") &&
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding")).Text.Contains("Курьером"),
                "show shippings in product cart client");
        }

        [Test]
        public void ShippingzInProductCartCount()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");
            Driver.ScrollTo(By.Id("ReviewImageWidth"));
            (new SelectElement(Driver.FindElement(By.Id("ShowShippingsMethodsInDetails")))).SelectByText("Всегда");
            Driver.FindElement(By.Id("ShippingsMethodsInDetailsCount")).Click();
            Driver.FindElement(By.Id("ShippingsMethodsInDetailsCount")).Clear();
            Driver.FindElement(By.Id("ShippingsMethodsInDetailsCount")).SendKeys("1");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admins
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            VerifyAreEqual("1", Driver.FindElement(By.Id("ShippingsMethodsInDetailsCount")).GetAttribute("value"),
                "shippings count in product cart admin value");

            //check client
            GoToClient("products/ipd35");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".cs-bg-4.block-exuding"))
                    .FindElements(By.CssSelector(".shipping-variants-row")).Count == 1,
                "shippings count in product cart client");
        }
    }

    [TestFixture]
    public class SettingsTemplateProductCartMarketingTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Property.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductExt.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.RelatedCategories.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.Tag.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Product\\Catalog.ProductPropertyValue.csv"
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
        public void RelatedProductName()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.ScrollTo(By.Id("ReviewImageHeight"));
            Driver.FindElement(By.Id("RelatedProductName")).Click();
            Driver.FindElement(By.Id("RelatedProductName")).Clear();
            Driver.FindElement(By.Id("RelatedProductName")).SendKeys("RelatedProductName Test");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            VerifyAreEqual("RelatedProductName Test",
                Driver.FindElement(By.Id("RelatedProductName")).GetAttribute("value"),
                "related product name admin value");

            //check client
            GoToClient("products/ipd165");

            VerifyIsTrue(Driver.PageSource.Contains("RelatedProductName Test"), "related product name client");
        }

        [Test]
        public void AlternativeProductName()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.ScrollTo(By.Id("ReviewImageHeight"));
            Driver.FindElement(By.Id("AlternativeProductName")).Click();
            Driver.FindElement(By.Id("AlternativeProductName")).Clear();
            Driver.FindElement(By.Id("AlternativeProductName")).SendKeys("AlternativeProductName Test");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            VerifyAreEqual("AlternativeProductName Test",
                Driver.FindElement(By.Id("AlternativeProductName")).GetAttribute("value"),
                "alternative product name admin value");

            //check client
            GoToClient("products/ipd165");

            VerifyIsTrue(Driver.PageSource.Contains("AlternativeProductName Test"), "alternative product name client");
        }

        [Test]
        public void RelatedProductSourceTypeFromProduct()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            string relatedProductName = Driver.FindElement(By.Id("RelatedProductName")).GetAttribute("value");
            string alternativeProductName = Driver.FindElement(By.Id("AlternativeProductName")).GetAttribute("value");

            Driver.ScrollTo(By.Id("ReviewImageHeight"));
            (new SelectElement(Driver.FindElement(By.Id("RelatedProductSourceType")))).SelectByText(
                "Из списка назначенных товаров");

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Enabled)
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");
            IWebElement selectRelatedProductSourceType = Driver.FindElement(By.Id("RelatedProductSourceType"));
            SelectElement select = new SelectElement(selectRelatedProductSourceType);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Из списка назначенных товаров"),
                "related products source type admin");

            //check client
            GoToClient("products/apple_iphone_4s_16gb_belyi");

            VerifyIsFalse(
                Driver.PageSource.Contains(relatedProductName) && Driver.PageSource.Contains(alternativeProductName),
                "related products source type from product not show");

            GoToClient("products/ipd165");

            VerifyIsTrue(
                Driver.PageSource.Contains(relatedProductName) && Driver.PageSource.Contains(alternativeProductName),
                "related products source type from product show");
        }

        [Test]
        public void RelatedProductSourceTypeFromCategory()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            string relatedProductName = Driver.FindElement(By.Id("RelatedProductName")).GetAttribute("value");
            string alternativeProductName = Driver.FindElement(By.Id("AlternativeProductName")).GetAttribute("value");

            Driver.ScrollTo(By.Id("ReviewImageHeight"));
            (new SelectElement(Driver.FindElement(By.Id("RelatedProductSourceType")))).SelectByText(
                "Из назначенной категории");

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Enabled)
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");
            IWebElement selectRelatedProductSourceType = Driver.FindElement(By.Id("RelatedProductSourceType"));
            SelectElement select = new SelectElement(selectRelatedProductSourceType);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Из назначенной категории"),
                "related products source type admin");

            //check client
            GoToClient("products/apple_iphone_4s_16gb_belyi");

            VerifyIsTrue(
                Driver.PageSource.Contains(relatedProductName) && Driver.PageSource.Contains(alternativeProductName),
                "related products source type from category show");

            GoToClient("products/ipd237"); //category without related

            VerifyIsFalse(
                Driver.PageSource.Contains(relatedProductName) && Driver.PageSource.Contains(alternativeProductName),
                "related products source type from category not show");

            GoToClient("products/ipd165"); //product with related (relatedProducts always shown)

            VerifyIsTrue(
                Driver.PageSource.Contains(relatedProductName) && Driver.PageSource.Contains(alternativeProductName),
                "related products source type from product show");
        }

        [Test]
        public void RelatedProductsMaxCount()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            VerifyAreEqual("10", Driver.FindElement(By.Id("RelatedProductsMaxCount")).GetAttribute("value"),
                "related products max count admin value");
            IWebElement selectRelatedProductSourceType = Driver.FindElement(By.Id("RelatedProductSourceType"));
            SelectElement select = new SelectElement(selectRelatedProductSourceType);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Из назначенной категории"),
                "related products source type admin");

            //check client
            GoToClient("products/apple_iphone_4s_16gb_belyi");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view.products-view-tile"))[0]
                    .FindElements(By.CssSelector(".products-view-item.text-static.cs-br-1.js-products-view-item"))
                    .Count == 8, "related products max count default");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view.products-view-tile"))[1]
                    .FindElements(By.CssSelector(".products-view-item.text-static.cs-br-1.js-products-view-item"))
                    .Count == 9, "alternative products max count default");

            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            Driver.ScrollTo(By.Id("ReviewImageHeight"));
            Driver.FindElement(By.Id("RelatedProductsMaxCount")).Click();
            Driver.FindElement(By.Id("RelatedProductsMaxCount")).Clear();
            Driver.FindElement(By.Id("RelatedProductsMaxCount")).SendKeys("1");

            (new SelectElement(Driver.FindElement(By.Id("RelatedProductSourceType")))).SelectByText(
                "Из назначенной категории");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=product");

            VerifyAreEqual("1", Driver.FindElement(By.Id("RelatedProductsMaxCount")).GetAttribute("value"),
                "related products max count admin value");
            selectRelatedProductSourceType = Driver.FindElement(By.Id("RelatedProductSourceType"));
            select = new SelectElement(selectRelatedProductSourceType);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Из назначенной категории"),
                "related products source type admin");

            //check client
            GoToClient("products/apple_iphone_4s_16gb_belyi");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view.products-view-tile"))[0]
                    .FindElements(By.CssSelector(".products-view-item.text-static.cs-br-1.js-products-view-item"))
                    .Count == 1, "related products max count client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view.products-view-tile"))[1]
                    .FindElements(By.CssSelector(".products-view-item.text-static.cs-br-1.js-products-view-item"))
                    .Count == 1, "alternative products max count client");
        }
    }
}