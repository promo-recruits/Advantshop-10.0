using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.EditCategory
{
    [TestFixture]
    public class CategoryEditSort : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\EditCategory\\Catalog.Category.csv",
                "Data\\Admin\\EditCategory\\Catalog.Brand.csv",
                "Data\\Admin\\EditCategory\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\EditCategory\\Catalog.Property.csv",
                "Data\\Admin\\EditCategory\\Catalog.PropertyValue.csv",
                "Data\\Admin\\EditCategory\\Catalog.Product.csv",
                "Data\\Admin\\EditCategory\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\EditCategory\\Catalog.Offer.csv",
                "Data\\Admin\\EditCategory\\Catalog.ProductCategories.csv"
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
        public void SortCategory()
        {
            GoToClient("categories/newcategory");
            VerifyAreEqual("TestProduct1",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct20",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[19]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }

        [Test]
        public void SortNoSortingCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");

            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("Без сортировки");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/newcategory");
            VerifyAreEqual("TestProduct1",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct20",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[19]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }

        [Test]
        public void SortByNewCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");

            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("Новинки");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/newcategory");
            VerifyAreEqual("TestProduct1",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct24",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct2",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[23]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }

        /*
        [Test]
        public void SortByNameCategory()
        {
           GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
                        
            driver.FindElement(By.Id("UrlPath")).Clear();
            driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");

            (new SelectElement(driver.FindElement(By.Id("Sorting")))).SelectByText("Названию, по возрастанию");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/newcategory");
            VerifyAreEqual("TestProduct1", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0].FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct10", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[1].FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct9", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[23].FindElement(By.CssSelector(".products-view-name-link")).Text);
        }

        [Test]
        public void SortByNameDesCategory()
        {
           GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));            
            
            driver.FindElement(By.Id("UrlPath")).Clear();
            driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");

            (new SelectElement(driver.FindElement(By.Id("Sorting")))).SelectByText("Названию, по убыванию");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/newcategory");
            VerifyAreEqual("TestProduct9", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0].FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct8", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[1].FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct1", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[23].FindElement(By.CssSelector(".products-view-name-link")).Text);
        }
        */
        [Test]
        public void SortByPriceCategory()
        {
            //Functions.RecalculateProducts(driver, baseURL);
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");

            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("Сначала дешевле");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/newcategory");
            VerifyAreEqual("TestProduct1",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct2",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct24",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[23]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }

        [Test]
        public void SortByPriceDesCategory()
        {
            // Functions.RecalculateProducts(driver, baseURL);
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");

            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("Сначала дороже");
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/newcategory");
            VerifyAreEqual("TestProduct24",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct23",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct1",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[23]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }

        [Test]
        public void SortByRaitingCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");

            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("Высокий рейтинг");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/newcategory");
            VerifyAreEqual("TestProduct1",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct20",
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[19]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }
    }
}