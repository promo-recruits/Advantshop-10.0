using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.Product_ProductLists
{
    [TestFixture]
    public class Product_ProductListAddPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.ProductList.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.Product_ProductList.csv"
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
        public void PageAddProduct_ProductLists()
        {
            GoToAdmin("mainpageproductsstore?listId=2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory6')]")).Click();
            VerifyAreEqual("18/20", Driver.FindElement(By.Id("1_anchor")).FindElements(By.TagName("span"))[1].Text,
                "products in categories count");
            VerifyAreEqual("TestProduct151", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct160", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("TestProduct161", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct170", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10");


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("TestProduct171", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 3 line 1");
            VerifyAreEqual("TestProduct180", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 3 line 10");


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("TestProduct181", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 4 line 1");
            VerifyAreEqual("TestProduct190", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 4 line 10");


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct191", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 5 line 1");
            VerifyAreEqual("TestProduct200", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 5 line 10");


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct201", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 6 line 1");
            VerifyAreEqual("TestProduct210", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 6 line 10");

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("TestProduct151", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct160", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");
        }

        [Test]
        public void PageAddProduct_ProductListsToPrevious()
        {
            GoToAdmin("mainpageproductsstore?listId=2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory6')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct151", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct160", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct161", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct170", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10");


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct171", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 3 line 1");
            VerifyAreEqual("TestProduct180", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 3 line 10");


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct161", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct170", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10");


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct151", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct160", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("TestProduct241", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "last page line 1");
            VerifyAreEqual("TestProduct250", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "last page line 10");
        }
    }
}