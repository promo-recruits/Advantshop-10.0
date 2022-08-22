using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsSettingsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Reviews\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\Reviews\\Customers.CustomerGroup.csv",
                "data\\Admin\\Catalog\\Reviews\\Customers.Customer.csv",
                "data\\Admin\\Catalog\\Reviews\\CMS.Review.csv"
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
        public void AddCheckedWithImg()
        {
            Functions.AdminSettingsReviewsShowImgOn(Driver, BaseUrl);

            GoToAdmin("reviews");

            Driver.FindElement(By.CssSelector("[data-e2e=\"reviewAdd\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Отзыв", Driver.FindElement(By.TagName("h2")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).SendKeys("50");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).SendKeys("CustomerName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).SendKeys("ReviewEmail@gmail.com");
            Driver.SetCkText("Хороший товар и не дорогой, спасибо", "ReviewText");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).SendKeys("18.11.2016 11:03:23");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewCheckedSelected\"]")).Selected);
            //driver.FindElement(By.CssSelector("input[type=\"file\"]")).Clear();
            AttachFile(By.CssSelector("input[type=\"file\"]"), GetPicturePath("icon.jpg"));

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("reviews");

            Driver.GridFilterSendKeys("Хороший товар и не дорогой, спасибо");
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("ReviewEmail@gmail.com"));
            VerifyAreEqual("Хороший товар и не дорогой, спасибо", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(0, "ProductName").Text);
            VerifyAreEqual("18.11.2016 11:03", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);

            //check client
            GoToClient("products/test-product50?tab=tabReviews");
            VerifyIsTrue(Driver.PageSource.Contains("CustomerName"));
            VerifyIsTrue(Driver.FindElement(By.Id("tabReviews")).Text.Contains("Отзывы (1)"));
            VerifyIsTrue(Driver.PageSource.Contains("Хороший товар и не дорогой, спасибо"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".review-item"))
                .FindElements(By.CssSelector(".review-item__photo-link")).Count == 1);
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".review-item")).FindElement(By.TagName("img"))
                .GetAttribute("src").Contains("nophoto"));
        }


        [Test]
        public void AddNotCheckedWithoutImg()
        {
            Functions.AdminSettingsReviewsShowImgOn(Driver, BaseUrl);

            GoToAdmin("reviews");

            Driver.FindElement(By.CssSelector("[data-e2e=\"reviewAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).SendKeys("11");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).SendKeys("Customer");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).SendKeys("ReviewEmail1@gmail.com");
            Driver.SetCkText("Все понравилось", "ReviewText");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).SendKeys("18.11.2016 11:03:23");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewCheckedClick\"]")).Click();
            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("reviews");

            Driver.GridFilterSendKeys("Все понравилось");
            VerifyIsTrue(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("Customer"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("ReviewEmail1@gmail.com"));
            VerifyAreEqual("Все понравилось", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(0, "ProductName").Text);
            VerifyAreEqual("18.11.2016 11:03", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);

            //check client
            GoToClient("products/test-product11?tab=tabReviews");
            VerifyIsTrue(Driver.PageSource.Contains("Customer"));
            VerifyIsTrue(Driver.FindElement(By.Id("tabReviews")).Text.Contains("Отзывы (1)"));
            VerifyIsTrue(Driver.PageSource.Contains("Все понравилось"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".review-item")).FindElements(By.TagName("img")).Count == 0);
            VerifyIsTrue(Driver.PageSource.Contains("18 ноября 2016"));
        }


        [Test]
        public void AddCheckedWithImgShowOff()
        {
            Functions.AdminSettingsReviewsShowImgOff(Driver, BaseUrl);

            GoToAdmin("reviews");

            Driver.FindElement(By.CssSelector("[data-e2e=\"reviewAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).SendKeys("12");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).SendKeys("CustomerName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).SendKeys("ReviewEmail@gmail.com");
            Driver.SetCkText("ура", "ReviewText");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).SendKeys("18.11.2016 11:03:23");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewCheckedSelected\"]")).Selected);
            // driver.FindElement(By.CssSelector("input[type=\"file\"]")).Clear();
            AttachFile(By.CssSelector("input[type=\"file\"]"), GetPicturePath("icon.jpg"));

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("reviews");

            Driver.GridFilterSendKeys("ура");
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("ReviewEmail@gmail.com"));
            VerifyAreEqual("ура", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("TestProduct12", Driver.GetGridCell(0, "ProductName").Text);
            VerifyAreEqual("18.11.2016 11:03", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);

            //check client
            GoToClient("products/test-product12?tab=tabReviews");
            VerifyIsTrue(Driver.PageSource.Contains("CustomerName"));
            VerifyIsTrue(Driver.FindElement(By.Id("tabReviews")).Text.Contains("Отзывы (1)"));
            VerifyIsTrue(Driver.PageSource.Contains("ура"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".reviews"))
                .FindElements(By.CssSelector(".review-item-image")).Count > 0);
        }

        [Test]
        public void AddNotCheckedModerateOn()
        {
            Functions.AdminSettingsReviewsModerateOn(Driver, BaseUrl);

            GoToAdmin("reviews");

            Driver.FindElement(By.CssSelector("[data-e2e=\"reviewAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).SendKeys("13");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).SendKeys("CustomerName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).SendKeys("ReviewEmail@gmail.com");
            Driver.SetCkText("тестовый отзыв", "ReviewText");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).SendKeys("18.11.2016 11:03:23");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewCheckedClick\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("reviews");

            Driver.GridFilterSendKeys("тестовый отзыв");
            VerifyIsTrue(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("ReviewEmail@gmail.com"));
            VerifyAreEqual("тестовый отзыв", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("TestProduct13", Driver.GetGridCell(0, "ProductName").Text);
            VerifyAreEqual("18.11.2016 11:03", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);

            //check client
            GoToClient("products/test-product13?tab=tabReviews");
            VerifyIsFalse(Driver.PageSource.Contains("CustomerName"));
            VerifyIsFalse(Driver.FindElement(By.Id("tabReviews")).Text.Contains("Отзывы (1)"));
            VerifyIsFalse(Driver.PageSource.Contains("тестовый отзыв"));
        }

        [Test]
        public void AddCheckedModerateOn()
        {
            Functions.AdminSettingsReviewsModerateOn(Driver, BaseUrl);

            GoToAdmin("reviews");

            Driver.FindElement(By.CssSelector("[data-e2e=\"reviewAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).SendKeys("14");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).SendKeys("CustomerName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).SendKeys("ReviewEmail@gmail.com");
            Driver.SetCkText("закажем еще", "ReviewText");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).SendKeys("18.11.2016 11:03:23");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("reviews");

            Driver.GridFilterSendKeys("закажем еще");
            VerifyIsTrue(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("ReviewEmail@gmail.com"));
            VerifyAreEqual("закажем еще", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("TestProduct14", Driver.GetGridCell(0, "ProductName").Text);
            VerifyAreEqual("18.11.2016 11:03", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);

            //check client
            GoToClient("products/test-product14?tab=tabReviews");
            VerifyIsTrue(Driver.PageSource.Contains("CustomerName"));
            VerifyIsTrue(Driver.FindElement(By.Id("tabReviews")).Text.Contains("Отзывы (1)"));
            VerifyIsTrue(Driver.PageSource.Contains("закажем еще"));
        }

        [Test]
        public void AddNotCheckedModerateOff()
        {
            Functions.AdminSettingsReviewsModerateOff(Driver, BaseUrl);

            GoToAdmin("reviews");

            Driver.FindElement(By.CssSelector("[data-e2e=\"reviewAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).SendKeys("15");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).SendKeys("CustomerName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).SendKeys("ReviewEmail@gmail.com");
            Driver.SetCkText("супер отзыв", "ReviewText");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).SendKeys("18.11.2016 11:03:23");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewCheckedClick\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("reviews");

            Driver.GridFilterSendKeys("супер отзыв");
            VerifyIsTrue(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("ReviewEmail@gmail.com"));
            VerifyAreEqual("супер отзыв", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("TestProduct15", Driver.GetGridCell(0, "ProductName").Text);
            VerifyAreEqual("18.11.2016 11:03", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);

            //check client
            GoToClient("products/test-product15?tab=tabReviews");
            VerifyIsTrue(Driver.PageSource.Contains("CustomerName"));
            VerifyIsTrue(Driver.FindElement(By.Id("tabReviews")).Text.Contains("Отзывы (1)"));
            VerifyIsTrue(Driver.PageSource.Contains("супер отзыв"));
        }

        [Test]
        public void AddClientImgUpLoadimgOn()
        {
            Functions.AdminSettingsReviewsImgUploadingOn(Driver, BaseUrl);

            GoToClient("products/test-product1?tab=tabReviews");

            Driver.ScrollTo(By.CssSelector(".review-form-block"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".review-form-block")).Text.Contains("Выберите файл"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".review-form-block")).Text.Contains("Изображение"));
        }

        [Test]
        public void AddClientImgUpLoadimgOff()
        {
            Functions.AdminSettingsReviewsImgUploadingOff(Driver, BaseUrl);

            GoToClient("products/test-product1?tab=tabReviews");

            Driver.ScrollTo(By.CssSelector(".review-form-block"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".review-form-block")).Text.Contains("Выберите файл"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".review-form-block")).Text.Contains("Изображение"));
        }

        [Test]
        public void AllowReviewsOn()
        {
            Functions.AdminSettingsReviewsOn(Driver, BaseUrl);

            GoToClient("products/test-product1?tab=tabReviews");

            VerifyIsTrue(Driver.PageSource.Contains("Отзывы"));
        }

        [Test]
        public void AllowReviewsOff()
        {
            Functions.AdminSettingsReviewsOff(Driver, BaseUrl);

            GoToClient("products/test-product1?tab=tabReviews");

            VerifyIsFalse(Driver.PageSource.Contains("Отзывы"));
        }
    }
}