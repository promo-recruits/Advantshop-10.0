using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditReviewAndCopyTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.CMS);
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
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Customers.CustomerGroup.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Customers.Customer.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Customers.Departments.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Customers.Managers.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Customers.ManagerTask.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\CMS.Review.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\CMS.StaticPage.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\CMS.Menu.csv"
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
        public void ProductEditReviews()
        {
            GoToAdmin("product/edit/4");

            VerifyAreEqual("3 смотреть", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductReviewsCount\"]")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Review\"]")).Click();
            Thread.Sleep(1000);

            //focus to second browser tab
            Functions.OpenNewTab(Driver, BaseUrl);

            //check admin
            VerifyAreEqual("Давно искала такое платье! Доставили очень оперативно, спасибо!",
                Driver.GetGridCell(1, "Text").Text);

            Functions.CloseTab(Driver, BaseUrl);

            //check client
            GoToClient("products/test-product4?tab=tabReviews");
            VerifyAreEqual("Отзывы (3)", Driver.FindElement(By.Id("tabReviews")).Text);
            VerifyIsTrue(Driver.PageSource.Contains("Давно искала такое платье! Доставили очень оперативно, спасибо!"));
            VerifyIsTrue(Driver.PageSource.Contains("3 отзыва"));
        }


        [Test]
        public void ProductEditNoReviews()
        {
            GoToAdmin("product/edit/10");

            VerifyAreEqual("0", Driver.FindElement(By.CssSelector("[data-e2e=\"ProductReviewsCount\"]")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Review\"]")).Count == 0);

            //check client
            GoToClient("products/test-product10");
            VerifyAreEqual("Отзывы", Driver.FindElement(By.Id("tabReviews")).Text);
        }


        [Test]
        public void ProductEditDoCopy()
        {
            GoToAdmin("product/edit/11");

            Driver.ScrollTo(By.Id("ArtNo"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCopy\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Добавление копии товара",
                Driver.FindElement(By.CssSelector(".modal-dialog")).FindElement(By.TagName("h2")).Text);
            VerifyAreEqual("TestProduct11 - копия",
                Driver.FindElement(By.CssSelector(".modal-dialog"))
                    .FindElement(By.CssSelector("[data-e2e=\"CopyName\"]")).GetAttribute("value"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"CopyProductAdd\"]")).Click();
            Thread.Sleep(1000);

            //check admin
            VerifyAreEqual("Товар \"TestProduct11 - копия\"", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("TestProduct11 - копия", Driver.FindElement(By.Id("Name")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.Id("Enabled")).Selected);
            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyIsFalse(Driver.FindElement(By.Id("Recomended")).Selected);
            VerifyIsFalse(Driver.FindElement(By.Id("Sales")).Selected);
            VerifyIsFalse(Driver.FindElement(By.Id("BestSeller")).Selected);
            VerifyIsFalse(Driver.FindElement(By.Id("New")).Selected);
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            IWebElement selectCurrency = Driver.FindElement(By.Id("CurrencyId"));
            SelectElement select1 = new SelectElement(selectCurrency);
            VerifyIsTrue(select1.AllSelectedOptions[0].Text.Contains("Рубли"));
            VerifyAreEqual("0", Driver.FindElement(By.Id("DiscountPercent")).GetAttribute("value"));
            VerifyIsTrue(Driver.GetGridCell(0, "SizeId", "Offers").Text.Contains("SizeName11"));
            VerifyIsTrue(Driver.GetGridCell(0, "ColorId", "Offers").Text.Contains("Color11"));
            VerifyAreEqual("11", Driver.GetGridCell(0, "BasePrice", "Offers").Text);
            VerifyAreEqual("11", Driver.GetGridCell(0, "SupplyPrice", "Offers").Text);
            VerifyAreEqual("11", Driver.GetGridCell(0, "Amount", "Offers").Text);
            VerifyAreEqual("unit", Driver.FindElement(By.Id("Unit")).GetAttribute("value"));
            VerifyAreEqual("", Driver.FindElement(By.Id("MinAmount")).GetAttribute("value"));
            VerifyAreEqual("", Driver.FindElement(By.Id("MaxAmount")).GetAttribute("value"));
            VerifyAreEqual("1", Driver.FindElement(By.Id("Multiplicity")).GetAttribute("value"));
            Driver.FindElement(By.XPath("//div[contains(text(), 'Доставка')]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("11", Driver.FindElement(By.Id("Length")).GetAttribute("value"));
            VerifyAreEqual("11", Driver.FindElement(By.Id("Width")).GetAttribute("value"));
            VerifyAreEqual("11", Driver.FindElement(By.Id("Height")).GetAttribute("value"));
            VerifyAreEqual("11", Driver.FindElement(By.Id("Weight")).GetAttribute("value"));
            VerifyAreEqual("11", Driver.FindElement(By.Id("ShippingPrice")).GetAttribute("value"));
            Driver.FindElement(By.XPath("//div[contains(text(), 'Описание')]")).Click();
            Thread.Sleep(1000);
            Driver.AssertCkText("briefDesc11", "BriefDescription");
            Driver.AssertCkText("Desc11", "Description");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Свойства товара')]")).Click();
            Thread.Sleep(500);//костыль, с товаром не придумала, как иначе
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".properties-item-value")).Text.Contains("PropertyValue12"));
            VerifyAreEqual("Property2", Driver.FindElement(By.CssSelector(".properties-item-name")).Text);
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("testproduct11-kopiya", Driver.FindElement(By.Id("UrlPath")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.Id("DefaultMeta")).Selected);

            //check client
            VerifyIsFalse(Is404Page("products/testproduct11-kopiya"));
        }

        [Test]
        public void ProductEditDelete()
        {
            VerifyIsFalse(Is404Page("products/test-product12"));

            GoToAdmin("product/edit/12");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Удалить')]")).Click();
            Driver.SwalConfirm();

            //check admin
            GoToAdmin("catalog?categoryid=1");
            Driver.GridFilterSendKeys("TestProduct12");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            GoToAdmin("catalog?showMethod=AllProducts");
            Driver.GridFilterSendKeys("TestProduct12");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check client
            VerifyIsTrue(Is404Page("products/test-product12"));
        }
    }
}