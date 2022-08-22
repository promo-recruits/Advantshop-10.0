using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductEditCategoryBlockTest : BaseSeleniumTest
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
        [Order(1)]
        public void ProductAdd()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProduct\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Добавление товара", Driver.FindElement(By.TagName("h2")).Text);
            Driver.FindElement(By.XPath("//a[contains(text(), 'Выбрать')]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Родительская категория", Driver.FindElement(By.TagName("h2")).Text);
            Driver.FindElements(By.CssSelector(".jstree-anchor"))[1].Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();
            Driver.FindElement(By.CssSelector(".modal-body")).FindElements(By.TagName("input"))[1].Click();
            Driver.FindElement(By.CssSelector(".modal-body")).FindElements(By.TagName("input"))[1]
                .SendKeys("ProductNew");

            Driver.FindElement(By.XPath("//span[contains(text(), 'Добавить')]")).Click();
            Thread.Sleep(1000);

            GoToAdmin("catalog?categoryid=1");
            Driver.GridFilterSendKeys("ProductNew");
            VerifyAreEqual("ProductNew", Driver.GetGridCell(0, "Name").Text);
        }

        [Test]
        [Order(1)]
        public void ProductAddCheckLook()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProduct\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Выбрать')]")).Click();
            Thread.Sleep(1000);

            //  driver.FindElements(By.CssSelector(".jstree-anchor"))[1].Click();
            Driver.FindElements(By.CssSelector(".jstree-anchor"))[1]
                .FindElement(By.CssSelector(".jstree-advantshop-name")).Click();
            //driver.FindElements(By.Id("1"))[1].FindElement(By.CssSelector(".jstree-advantshop-name")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();
            Driver.FindElement(By.CssSelector(".modal-body")).FindElements(By.TagName("input"))[1].Click();
            Driver.FindElement(By.CssSelector(".modal-body")).FindElements(By.TagName("input"))[1]
                .SendKeys("ProductNewCheckLookButton");

            Driver.FindElement(By.XPath("//span[contains(text(), 'Добавить')]")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.XPath("//a[contains(text(), 'Просмотр')]")).Click();
            Thread.Sleep(1000);

            //focus to another browser tab
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.PageSource.Contains("ProductNewCheckLookButton"));
            VerifyIsTrue(Driver.Url.Contains("products/productnewchecklookbutton"));

            //close tab
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        [Order(0)]
        public void ProductEditToDisabledAddCategory()
        {
            GoToAdmin("product/edit/1");

            //check change name
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("ChangedNameTestProduct1");
            Driver.FindElement(By.XPath("//h2[contains(text(), 'Основное')]")).Click();

            //check change activity
            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxEnabledClick\"]")).Click();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/1");

            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));

            //check add category
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory2')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Каталог → TestCategory2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"2\"]")).GetAttribute("label"));

            GoToAdmin("product/edit/1");

            //check product card
            VerifyAreEqual("ChangedNameTestProduct1", Driver.FindElement(By.Id("Name")).GetAttribute("value"));
            VerifyIsFalse(Driver.FindElement(By.Id("Enabled")).Selected);
            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[0].Text);
            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"2\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[1].Text);

            //check admin grid
            GoToAdmin("catalog?categoryId=1");
            //Driver.GridFilterSendKeys("ChangedNameTestProduct1");
            VerifyAreEqual("ChangedNameTestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);

            //check client
            VerifyIsTrue(Is404Page("products/test-product1"));
        }

        [Test]
        [Order(1)]
        public void ProductEditToEnabledSetMainCategory()
        {
            GoToAdmin("product/edit/2");
            VerifyAreEqual("Товар \"TestProduct2\"", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("TestCategory1",
                Driver.FindElement(By.CssSelector(".breadcrumb")).FindElements(By.CssSelector(".link-invert"))[1].Text);

            //check change activity
            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxEnabledClick\"]")).Click();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/2");

            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));

            //check add category
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            VerifyAreEqual("Категория", Driver.FindElement(By.TagName("h2")).Text);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();
            Driver.WaitForToastSuccess();

            //check set another main category
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElements(By.TagName("option"))[1]
                .Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategorySetMain\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/2");

            //check product card
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductEnabled\"]"))
                .FindElement(By.TagName("input")).Selected);
            VerifyAreEqual("Каталог → TestCategory3 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory3 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"3\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));
            VerifyAreEqual("TestCategory3",
                Driver.FindElement(By.CssSelector(".breadcrumb")).FindElements(By.CssSelector(".link-invert"))[1].Text);

            //check admin grid
            GoToAdmin("catalog?categoryId=1");
            Driver.GridFilterSendKeys("TestProduct2");
            Driver.GetGridCell(-1, "SortOrder").Click();
            VerifyAreEqual("TestProduct2", Driver.GetGridCell(0, "Name").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);

            GoToAdmin("catalog?categoryId=3");
            Driver.GridFilterSendKeys("TestProduct2");
            VerifyAreEqual("TestProduct2", Driver.GetGridCell(0, "Name").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);

            //check client
            GoToClient("products/test-product2");
            VerifyIsTrue(Driver.PageSource.Contains("TestCategory3"));
            GoToClient("categories/test-category3");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-product-id=\"2\"]")).Count > 0);
            GoToClient("categories/test-category1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-product-id=\"2\"]")).Count > 0);
        }

        [Test]
        [Order(1)]
        public void ProductEnabledAddInDisabledCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProduct\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".modal-body")).FindElements(By.TagName("input"))[1].Click();
            Driver.FindElement(By.CssSelector(".modal-body")).FindElements(By.TagName("input"))[1]
                .SendKeys("ProductActiveCategoryDisabled");

            Driver.FindElement(By.XPath("//span[contains(text(), 'Добавить')]")).Click();
            Thread.Sleep(1000);

            //check admin grid
            GoToAdmin("catalog?categoryid=5");
            Driver.GridFilterSendKeys("ProductActiveCategoryDisabled");
            VerifyAreEqual("ProductActiveCategoryDisabled", Driver.GetGridCell(0, "Name").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);

            //check client
            VerifyIsTrue(Is404Page("products/productactivecategorydisabled"));
        }

        [Test]
        [Order(1)]
        public void ProductEditSetMainCategoryToEnabled()
        {
            GoToAdmin("product/edit/81");
            VerifyIsTrue(Is404Page("products/test-product81"));
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"5\"]")).GetAttribute("label"));

            //check add category
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));

            //check set another main category
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElements(By.TagName("option"))[1]
                .Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategorySetMain\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/81");

            //check product card
            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[0].Text);
            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"5\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[1].Text);

            //check client
            VerifyIsFalse(Is404Page("products/test-product81"));
            GoToClient("products/test-product81");
            VerifyIsTrue(Driver.PageSource.Contains("TestCategory1"));
            GoToClient("categories/test-category1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-product-id=\"81\"]")).Count > 0);
        }

        [Test]
        [Order(1)]
        public void ProductEditSetMainCategoryToDisabled()
        {
            GoToAdmin("product/edit/21");
            VerifyIsFalse(Is404Page("products/test-product21"));
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"2\"]")).GetAttribute("label"));

            //check add category
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"5\"]")).GetAttribute("label"));

            //check set another main category
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElements(By.TagName("option"))[1]
                .Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategorySetMain\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/21");

            //check product card
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[0].Text);
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"5\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"2\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[1].Text);

            //check client
            VerifyIsTrue(Is404Page("products/test-product21"));
        }

        [Test]
        [Order(1)]
        public void ProductEditDeleteCategory()
        {
            GoToAdmin("product/edit/31");
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"2\"]")).GetAttribute("label"));

            //check add category
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));

            GoToAdmin("product/edit/31");

            //check product card
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"2\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[1].Text);

            //check delete category
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElements(By.TagName("option"))[1]
                .Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryDelete\"]")).Click();
            Driver.SwalConfirm();

            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"2\"]")).GetAttribute("label"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                .FindElements(By.TagName("option")).Count == 1);

            //check client
            GoToClient("products/test-product31");
            VerifyIsTrue(Driver.PageSource.Contains("TestCategory2"));
            GoToClient("categories/test-category2");
            //  VerifyIsTrue(driver.PageSource.Contains("TestProduct31"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-product-id=\"31\"]")).Count > 0);
            GoToClient("categories/test-category1");
            //   VerifyIsFalse(driver.PageSource.Contains("TestProduct31"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-product-id=\"31\"]")).Count > 0);
        }

        [Test]
        [Order(1)]
        public void ProductEditDeleteMainEnabledCategory()
        {
            GoToAdmin("product/edit/41");
            VerifyAreEqual("Каталог → TestCategory3 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory3 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"3\"]")).GetAttribute("label"));

            //check add category
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));

            GoToAdmin("product/edit/41");

            //check product card
            VerifyAreEqual("Каталог → TestCategory3 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory3 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"3\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[1].Text);

            //check delete category
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                .Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryDelete\"]")).Click();
            Driver.SwalConfirm();

            GoToAdmin("product/edit/41");

            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                .FindElements(By.TagName("option")).Count == 1);

            //check client
            GoToClient("products/test-product41");
            VerifyIsTrue(Driver.PageSource.Contains("TestCategory1"));
            GoToClient("categories/test-category1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-product-id=\"41\"]")).Count > 0);
            GoToClient("categories/test-category3");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-product-id=\"41\"]")).Count > 0);
        }

        [Test]
        [Order(1)]
        public void ProductEditDeleteMainEnabledCategoryToDisabled()
        {
            GoToAdmin("product/edit/42");
            VerifyIsFalse(Is404Page("products/test-product42"));
            VerifyAreEqual("Каталог → TestCategory3 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory3 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"3\"]")).GetAttribute("label"));

            //check add category
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"5\"]")).GetAttribute("label"));

            GoToAdmin("product/edit/42");

            //check product card
            VerifyAreEqual("Каталог → TestCategory3 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory3 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"3\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"5\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[1].Text);

            //check delete category
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                .Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryDelete\"]")).Click();
            Driver.SwalConfirm();

            GoToAdmin("product/edit/42");

            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"5\"]")).GetAttribute("label"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                .FindElements(By.TagName("option")).Count == 1);

            //check client
            VerifyIsTrue(Is404Page("products/test-product42"));
        }

        [Test]
        [Order(1)]
        public void ProductEditDeleteMainDisabledCategoryToEnabled()
        {
            VerifyIsTrue(Is404Page("products/test-product82"));
            GoToAdmin("product/edit/82");
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"5\"]")).GetAttribute("label"));

            //check add category
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));

            GoToAdmin("product/edit/82");

            //check product card
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory4 → TestCategory5 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"5\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[1].Text);

            //check delete category
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                .Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryDelete\"]")).Click();
            Driver.SwalConfirm();

            GoToAdmin("product/edit/82");
            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory1 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                .FindElements(By.TagName("option")).Count == 1);

            //check client
            VerifyIsFalse(Is404Page("products/test-product82"));
            GoToClient("products/test-product82");
            VerifyIsTrue(Driver.PageSource.Contains("TestCategory1"));
            GoToClient("categories/test-category1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-product-id=\"82\"]")).Count > 0);
            GoToClient("categories/test-category5");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-product-id=\"82\"]")).Count > 0);
        }


        [Test]
        [Order(1)]
        public void ProductEditDeleteAllCategories()
        {
            GoToAdmin("product/edit/22");
            VerifyIsFalse(Is404Page("products/test-product22"));
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"2\"]")).GetAttribute("label"));

            //check add category
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));

            GoToAdmin("product/edit/22");

            //check product card
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                    .Text);
            VerifyAreEqual("Каталог → TestCategory2 (Основная)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"2\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElement(By.CssSelector("[value=\"1\"]")).GetAttribute("label"));
            VerifyAreEqual("Каталог → TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                    .FindElements(By.TagName("option"))[1].Text);

            //check delete all categories
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                .Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryDelete\"]")).Click();
            Driver.SwalConfirm();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]")).FindElement(By.TagName("option"))
                .Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CategoryDelete\"]")).Click();
            Driver.SwalConfirm();

            GoToAdmin("product/edit/22");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductCategory\"]"))
                .FindElements(By.TagName("option")).Count > 0);

            //check admin grid
            GoToAdmin("catalog?categoryId=2");
            Driver.GridFilterSendKeys("TestProduct22");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            GoToAdmin("catalog?showMethod=OnlyWithoutCategories");
            Driver.GridFilterSendKeys("TestProduct22");
            VerifyAreEqual("TestProduct22", Driver.GetGridCell(0, "Name").Text);

            //check client
            VerifyIsTrue(Is404Page("products/test-product22"));
        }
    }
}