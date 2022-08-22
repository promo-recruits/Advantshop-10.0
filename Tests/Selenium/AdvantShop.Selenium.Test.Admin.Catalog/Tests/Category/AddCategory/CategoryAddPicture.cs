using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.AddCategory
{
    [TestFixture]
    public class CategoryAddPicture : BaseSeleniumTest
    {
        bool del = true;

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\AddCategory\\Catalog.Brand.csv",
                "Data\\Admin\\Catalog\\AddCategory\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\AddCategory\\Catalog.Offer.csv"
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

        //Pictures
        [Test]
        [Order(0)]
        public void AddImgByHrefCategory()
        {
            GoToAdmin("catalog");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category");
            Driver.DropFocus("h1");
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("new1");
            Driver.ScrollTo(By.TagName("figure"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"BigImg\"] [data-e2e=\"imgByHref\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("input")).Click();
            Driver.FindElement(By.CssSelector("input")).Clear();
            Driver.FindElement(By.CssSelector("input")).SendKeys(
                "https://st.depositphotos.com/2000885/1902/i/450/depositphotos_19021343-stock-photo-red-heart.jpg");
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefBtnSave\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.ScrollTo(By.TagName("figure"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"IconImg\"] [data-e2e=\"imgByHref\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("input")).Clear();
            Driver.FindElement(By.CssSelector("input")).SendKeys(
                "https://upload.wikimedia.org/wikipedia/commons/thumb/4/47/PNG_transparency_demonstration_1.png/274px-PNG_transparency_demonstration_1.png");
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefBtnSave\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.ScrollTo(By.TagName("figure"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"SmallImg\"] [data-e2e=\"imgByHref\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("input")).Click();
            Driver.FindElement(By.CssSelector("input")).Clear();
            Driver.FindElement(By.CssSelector("input"))
                .SendKeys("https://www.1zoom.ru/big2/30/104540-spider280578.jpg");
            Driver.FindElement(By.CssSelector("[data-e2e=\"imgByHrefBtnSave\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            del = false;
            //Проверка
            GoToAdmin("catalog");
            string str = Driver.FindElement(By.CssSelector("[data-e2e-block=\"CategoryDrag\"] img"))
                .GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            //Проверка в клиентке
            GoToClient("categories/new1");
            str = Driver.FindElement(By.CssSelector(".category-picture")).FindElement(By.TagName("img"))
                .GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            GoToClient("catalog");
            str = Driver.FindElement(By.CssSelector(".menu-dropdown-icon-img")).GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            str = Driver.FindElement(By.CssSelector(".product-categories-item-photo-link-slim"))
                .FindElement(By.TagName("img")).GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
        }

        [Test]
        [Order(1)]
        public void AddImgCategory()
        {
            GoToAdmin("catalog");
            if (del == false)
                del_category();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category");
            Driver.DropFocus("h1");
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("new");
            Driver.ScrollTo(By.TagName("figure"));

            AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath("small.jpg"));
            AttachFile(By.XPath("(//input[@type='file'])[4]"), GetPicturePath("icon.jpg"));
            AttachFile(By.XPath("(//input[@type='file'])[6]"), GetPicturePath("big.png"));

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            del = false;
            //Проверка
            GoToClient("categories/new");
            string str = Driver.FindElement(By.CssSelector(".category-picture img")).GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            GoToClient("catalog");
            str = Driver.FindElement(By.CssSelector(".menu-dropdown-icon img")).GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            str = Driver.FindElement(By.CssSelector(".product-categories-item-photo-link-slim img"))
                .GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
            GoToAdmin("catalog");
            str = Driver.FindElement(By.CssSelector("[data-e2e-block=\"CategoryDrag\"] img")).GetAttribute("src");
            VerifyIsFalse(str.Contains("nophoto"));
        }

        [Test]
        [Order(2)]
        public void DelImgCategory()
        {
            GoToAdmin("catalog");
            if (del == false)
                del_category();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.ScrollTo(By.TagName("figure"));

            AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath("small.jpg"));
            AttachFile(By.XPath("(//input[@type='file'])[4]"), GetPicturePath("icon.jpg"));
            AttachFile(By.XPath("(//input[@type='file'])[6]"), GetPicturePath("big.png"));

            Driver.FindElement(By.Id("Name")).SendKeys("New_Category");
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("new2");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector("[data-e2e=\"EditCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));
            Driver.ScrollTo(By.TagName("figure"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"BigImg\"] [data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();
            Driver.FindElement(By.CssSelector("[data-e2e=\"IconImg\"] [data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SmallImg\"] [data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            del = false;
            //Проверка
            GoToAdmin("catalog");
            string str = Driver.FindElement(By.CssSelector("[data-e2e-block=\"CategoryDrag\"] img"))
                .GetAttribute("src");
            VerifyIsTrue(str.Contains("nophoto"));

            GoToClient("categories/new2");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".category-picture img")).Count);
            GoToClient("catalog");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".menu-dropdown-icon-img")).Count);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-categories-item-inner-slim img"))
                .GetAttribute("src").Contains("nophoto"));
        }

        public void del_category()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemDelete\"]")).Click();
            Driver.SwalConfirm();
        }
    }
}