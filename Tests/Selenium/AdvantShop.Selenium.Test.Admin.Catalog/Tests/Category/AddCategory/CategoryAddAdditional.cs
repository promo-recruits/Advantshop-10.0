using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.AddCategory
{
    [TestFixture]
    public class CategoryAddAdditional : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\AddCategory\\Catalog.Brand.csv",
                "Data\\Admin\\Catalog\\AddCategory\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\AddCategory\\Catalog.Offer.csv",
                "Data\\Admin\\Catalog\\AddCategory\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\AddCategory\\Catalog.ProductCategories.csv"
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
        public void DisplayStyleListNewCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_List");
            Driver.DropFocus("h1");
            (new SelectElement(Driver.FindElement(By.Id("DisplayStyle")))).SelectByText("Список");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_List_Child");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Изменить')]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_List')]")).Click();
            Driver.WaitForElemEnabled(By.XPath("//button[contains(text(), 'Изменить')]"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/new_category_list");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-categories-item-thin")).Count > 0);
        }

        [Test]
        public void DisplayStyleTileNewCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Tile");
            Driver.DropFocus("h1");

            (new SelectElement(Driver.FindElement(By.Id("DisplayStyle")))).SelectByText("Плитка");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Tile_Child");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Изменить')]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_Tile')]")).Click();
            Driver.WaitForElemEnabled(By.XPath("//button[contains(text(), 'Изменить')]"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/new_category_tile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-categories.product-categories-slim")).Count > 0);
        }

        [Test]
        public void DisplayStyleNoPreCategoryNewCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Refresh();
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_No_PreCategory");
            Driver.DropFocus("h1");
            (new SelectElement(Driver.FindElement(By.Id("DisplayStyle")))).SelectByText("Не показывать");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Refresh();
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_No_PreCategory_Child");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Изменить')]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_No_PreCategory')]")).Click();
            Driver.WaitForElemEnabled(By.XPath("//button[contains(text(), 'Изменить')]"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/new_category_no_precategory");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".product-categories.product-categories-slim")).Count);
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".product-categories-thin")).Count);
        }

        [Test]
        public void BrandInCategoryNewCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Show_Brand");
            Driver.DropFocus("h1");
            Driver.ScrollTo(By.Id("DisplayStyle"));
            Driver.FindElements(By.CssSelector(".adv-radio-label input"))[0].Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Show_Brand_Child");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Изменить')]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_Show_Brand')]")).Click();
            Driver.WaitForElemEnabled(By.XPath("//button[contains(text(), 'Изменить')]"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();
            Driver.ScrollTo(By.Id("DisplayStyle"));
            Driver.FindElements(By.CssSelector(".adv-radio-label input"))[0].Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog?categoryid=1");
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Driver.FindElement(
                    By.CssSelector("[data-e2e-grid-cell=\"grid[1][\'Enabled\']\"] [data-e2e=\"switchOnOffLabel\"]"))
                .Click();
            Driver.FindElement(
                    By.CssSelector("[data-e2e-grid-cell=\"grid[2][\'Enabled\']\"] [data-e2e=\"switchOnOffLabel\"]"))
                .Click();

            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"3\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_Show_Brand')]")).Click();
            Driver.WaitForElemEnabled(By.XPath("//button[contains(text(), 'Добавить товары')]"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Добавить товары')]")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("catalog");
            VerifyIsTrue(Driver.PageSource.Contains("Производители"));
            VerifyIsTrue(Driver.PageSource.Contains("BrandName1"));
        }

        [Test]
        public void BrandInCategoryAddCategoryNo()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Hide_Brand");
            Driver.DropFocus("h1");
            Driver.ScrollTo(By.Id("DisplayStyle"));
            Driver.FindElements(By.CssSelector(".adv-radio-label input"))[1].Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Hide_Brand_Child");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Изменить')]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_Hide_Brand')]")).Click();
            Driver.WaitForElemEnabled(By.XPath("//button[contains(text(), 'Изменить')]"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog?categoryid=1");
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"3\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_Hide_Brand')]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//button[contains(text(), 'Добавить товары')]")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("catalog");
            //VerifyIsTrue(driver.PageSource.Contains("Производители"));
            VerifyIsFalse(Driver.PageSource.Contains("BrandName1"));
        }

        [Test]
        public void HiddenCategoryInmenuNewCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Hidden_InMenu");
            Driver.DropFocus("h1");

            Driver.ScrollTo(By.Id("DisplayBrandsInMenu"));

            Driver.FindElements(By.CssSelector(".adv-radio-label input"))[4].Click();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("catalog");
            VerifyIsFalse(Driver.PageSource.Contains("New_Category_Hidden_InMenu"));
        }

        [Test]
        public void HiddenCategoryInmenuNoNewCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Hidden_InMenu_No");
            Driver.DropFocus("h1");

            Driver.ScrollTo(By.Id("DisplayBrandsInMenu"));

            Driver.FindElements(By.CssSelector(".adv-radio-label input"))[5].Click();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("catalog");
            VerifyIsTrue(Driver.PageSource.Contains("New_Category_Hidden_InMenu_No"));
        }

        [Test]
        public void ATwoLevelInCategoryNewCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Two_Levels_InMenu");
            Driver.DropFocus("h1");
            Driver.ScrollTo(By.Id("DisplayStyle"));
            Driver.FindElements(By.CssSelector(".adv-radio-label input"))[2].Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();


            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Two_Levels_InMenu_Child_1");
            Driver.DropFocus("h1");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Изменить')]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_Two_Levels_InMenu')]")).Click();
            Driver.WaitForElemEnabled(By.XPath("//button[contains(text(), 'Изменить')]"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Two_Levels_InMenu_Child_2");
            Driver.DropFocus("h1");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Изменить')]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_Two_Levels_InMenu')]")).Click();
            Driver.WaitForElemEnabled(By.XPath("//button[contains(text(), 'Изменить')]"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("catalog");

            VerifyIsTrue(Driver.PageSource.Contains("New_Category_Two_Levels_InMenu_Child_1"));
            VerifyIsTrue(Driver.PageSource.Contains("New_Category_Two_Levels_InMenu_Child_2"));
        }

        [Test]
        public void ATwoLevelInCategoryNewCategoryNo()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Two_Levels_InMenu_No");
            Driver.DropFocus("h1");
            Driver.ScrollTo(By.Id("DisplayStyle"));
            Driver.FindElements(By.CssSelector(".adv-radio-label input"))[3].Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog");
            Driver.FindElement(By.LinkText("Добавить категорию")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Two_Levels_InMenu_No_Child_1");
            Driver.DropFocus("h1");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Изменить')]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_Two_Levels_InMenu_No')]")).Click();
            Driver.WaitForElemEnabled(By.XPath("//button[contains(text(), 'Изменить')]"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Two_Levels_InMenu_No_Child_2");
            Driver.DropFocus("h1");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Изменить')]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'New_Category_Two_Levels_InMenu_No')]")).Click();
            Driver.WaitForElemEnabled(By.XPath("//button[contains(text(), 'Изменить')]"));
            Driver.FindElement(By.XPath("//button[contains(text(), 'Изменить')]")).Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("catalog");

            VerifyIsTrue(Driver.PageSource.Contains("New_Category_Two_Levels_InMenu_Child_1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".menu-dropdown-sub-columns-item"))[1]
                .FindElements(By.CssSelector(".menu-dropdown-sub-block.menu-dropdown-sub-block-cats-only")).Count > 0);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".menu-dropdown-sub-columns-item"))[0]
                .FindElements(By.CssSelector(".menu-dropdown-sub-block.menu-dropdown-sub-block-cats-only")).Count == 0);
        }
    }
}