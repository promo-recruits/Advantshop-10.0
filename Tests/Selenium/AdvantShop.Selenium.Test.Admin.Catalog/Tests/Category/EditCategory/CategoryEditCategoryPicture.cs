using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.EditCategory
{
    [TestFixture]
    public class CategoryEditPicture : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\EditCategory\\EditCategoryColor\\Catalog.Category.csv"
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
        public void ChangeImgCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));

            var element = Driver.FindElements(By.TagName("iframe"))[1];
            IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("new");
            Driver.ScrollTo(By.TagName("figure"));
            AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath("small.jpg"));
            AttachFile(By.XPath("(//input[@type='file'])[4]"), GetPicturePath("icon.jpg"));
            AttachFile(By.XPath("(//input[@type='file'])[6]"), GetPicturePath("big.png"));
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            //Проверка
            GoToAdmin("catalog");
            string str = Driver.FindElement(By.CssSelector("[data-e2e-block-category-drag-id=\"1\"] img"))
                .GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            //Проверка в клиентке
            GoToClient("catalog");
            str = Driver.FindElement(By.CssSelector(".product-categories-item-photo-link-slim img"))
                .GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            str = Driver.FindElement(By.CssSelector(".menu-dropdown-icon img")).GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            Driver.FindElement(By.CssSelector(".product-categories-header-slim-title")).Click();

            str = Driver.FindElement(By.CssSelector(".category-picture img")).GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
        }

        [Test]
        public void HrefChangeImgCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));

            var element = Driver.FindElements(By.TagName("iframe"))[1];
            IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("new");
            Driver.ScrollTo(By.TagName("figure"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"BigImg\"] [data-e2e=\"imgByHref\"]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Загрузка изображения по ссылке", Driver.FindElement(By.TagName("h2")).Text);
            Driver.FindElement(By.CssSelector("input")).Click();
            Driver.FindElement(By.CssSelector("input")).Clear();
            Driver.FindElement(By.CssSelector("input")).SendKeys(
                "https://st.depositphotos.com/2000885/1902/i/450/depositphotos_19021343-stock-photo-red-heart.jpg");

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            Driver.ScrollTo(By.TagName("figure"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"IconImg\"] [data-e2e=\"imgByHref\"]")).Click();
            Driver.FindElement(By.CssSelector("input")).Clear();
            Driver.FindElement(By.CssSelector("input")).SendKeys(
                "https://upload.wikimedia.org/wikipedia/commons/thumb/4/47/PNG_transparency_demonstration_1.png/274px-PNG_transparency_demonstration_1.png");

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            Driver.ScrollTo(By.TagName("figure"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"SmallImg\"] [data-e2e=\"imgByHref\"]")).Click();

            Driver.FindElement(By.CssSelector("input")).Click();
            Driver.FindElement(By.CssSelector("input")).Clear();
            Driver.FindElement(By.CssSelector("input"))
                .SendKeys("https://texnomaniya.ru/pictures/pages/img_20926_page_26667_r.jpg");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            //Проверка
            GoToAdmin("catalog");
            string str = Driver.FindElement(By.CssSelector("[data-e2e-block-category-drag-id=\"1\"] img"))
                .GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            //Проверка в клиентке
            GoToClient("catalog");
            str = Driver.FindElement(By.CssSelector(".product-categories-item-photo-link-slim img"))
                .GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            str = Driver.FindElement(By.CssSelector(".menu-dropdown-icon img")).GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            Driver.FindElement(By.CssSelector(".product-categories-header-slim-title")).Click();

            str = Driver.FindElement(By.CssSelector(".category-picture img")).GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
        }

        [Test]
        public void DelImgByHrefCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.ScrollTo(By.TagName("figure"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"BigImg\"] [data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();
            Driver.FindElement(By.CssSelector("[data-e2e=\"IconImg\"] [data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SmallImg\"] [data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("new");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //Проверка
            GoToAdmin("catalog");
            string str = Driver.FindElement(By.CssSelector("[data-e2e-block-category-drag-id=\"1\"] img"))
                .GetAttribute("src");
            VerifyIsTrue(str.Contains("nophoto"));

            GoToClient("catalog");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-categories-item-inner-slim img"))
                .GetAttribute("src").Contains("nophoto"));
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".menu-dropdown-icon-img")).Count);

            GoToClient("categories/new");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".category-picture img")).Count);
        }
    }
}