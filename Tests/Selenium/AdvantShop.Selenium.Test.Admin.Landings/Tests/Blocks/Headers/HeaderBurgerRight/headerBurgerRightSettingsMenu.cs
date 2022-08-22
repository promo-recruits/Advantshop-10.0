using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Headers.HeaderBurgerRight
{
    [TestFixture]
    public class headerBurgerRightSettingsMenu : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Headers\\headerBurgerRight\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "headerBurgerRight";
        private string blockType = "Headers";
        private readonly int numberBlock = 1;

        [Test]
        public void DesktopMenu()
        {
            TestName = "DesktopMenu";
            VerifyBegin(TestName);

            ReInitClient();
            GoToClient("lp/test1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-menu-header")).Displayed,
                "initial menu in client");
            VerifyAreEqual("Главная", Driver.FindElements(By.CssSelector("#block_1 .lp-link--text"))[0].Text,
                "initial 1st item client");
            VerifyAreEqual("О нас", Driver.FindElements(By.CssSelector("#block_1 .lp-link--text"))[1].Text,
                "initial 2nd item client");
            VerifyAreEqual("Контакты", Driver.FindElements(By.CssSelector("#block_1 .lp-link--text"))[2].Text,
                "initial 3d item client");

            GoToMobile("lp/test1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Главная", Driver.FindElements(By.CssSelector("#block_1 .lp-link--text"))[0].Text,
                "initial 1st item mobile");
            VerifyAreEqual("О нас", Driver.FindElements(By.CssSelector("#block_1 .lp-link--text"))[1].Text,
                "initial 2nd item mobile");
            VerifyAreEqual("Контакты", Driver.FindElements(By.CssSelector("#block_1 .lp-link--text"))[2].Text,
                "initial 3d item mobile");

            //DesktopMenu is off
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderMenu");
            FunctionsHeaders.HiddenMenuInDesktop(Driver);
            Thread.Sleep(1000);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-menu-header")).Displayed,
                "menu is off in client");

            GoToMobile("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-menu-header")).Displayed,
                "menu is on in mobile");

            //DesktopMenu is on
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderMenu");
            FunctionsHeaders.ShowMenuInDesktop(Driver);
            Thread.Sleep(1000);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-menu-header")).Displayed, "menu is on");

            VerifyFinally(TestName);
        }

        [Test]
        public void DistanceBetweenPoints()
        {
            TestName = "DistanceBetweenPoints";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-menu-header li")).GetAttribute("style")
                    .Contains("padding-left: 20px; padding-right: 20px;"), "initial distance");

            GoToMobile("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-menu-header-container--open li")).GetAttribute("style")
                    .Contains("padding-left: 20px; padding-right: 20px;"), "initial distance in burger-menu");

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderMenu");
            FunctionsHeaders.moveSliderDistanceBtwnPoints(Driver, 10);
            Thread.Sleep(1000);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-menu-header li")).GetAttribute("style")
                    .Contains("padding-left: 10px; padding-right: 10px;"), "distance after");

            GoToMobile("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-menu-header-container--open li")).GetAttribute("style")
                    .Contains("padding-left: 10px; padding-right: 10px;"), "distance in burger-menu after");

            VerifyFinally(TestName);
        }

        [Test]
        public void FontSizeMenu()
        {
            TestName = "FontSizeMenu";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-menu-header li")).GetAttribute("style")
                    .Contains("font-size: 16px;"), "initial font size");

            GoToMobile("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-menu-header-container--open li")).GetAttribute("style")
                    .Contains("font-size: 16px;"), "font size in burger-menu 1");

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderMenu");
            FunctionsHeaders.ShowMenuInDesktop(Driver);
            FunctionsHeaders.MenuFontSize(Driver, 24);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-menu-header li")).GetAttribute("style")
                    .Contains("font-size: 24px;"), "font size after");

            GoToMobile("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-menu-header-container--open li")).GetAttribute("style")
                    .Contains("font-size: 24px;"), "font size in burger-menu 2");

            VerifyFinally(TestName);
        }

        [Test]
        public void MenuItems()
        {
            TestName = "MenuItems";
            VerifyBegin(TestName);

            //Delete
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderMenu");
            FunctionsHeaders.ShowMenuInDesktop(Driver);
            FunctionsHeaders.DelAllMenuItem(Driver);
            VerifyAreEqual("Нет элементов", Driver.FindElement(By.CssSelector(".lp-table__cell")).Text,
                "no elements in grid");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .lp-link--text")).Count == 0,
                "no menu items in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .lp-link--text")).Count == 0,
                "no menu items in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 .lp-menu-header-container--open .lp-link--text")).Count ==
                0, "no menu items in mobile");

            //Add
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderMenu");
            FunctionsHeaders.AddMenuItem(Driver, "First Menu", "");
            FunctionsHeaders.AddSubMenuItem(Driver, 0, "1 Sub menu", "");
            FunctionsHeaders.AddSubMenuItem(Driver, 0, "2 Sub menu", "");
            FunctionsHeaders.AddMenuItem(Driver, "Second Menu", "lp/test1/page1");
            Thread.Sleep(500);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            Driver.MouseFocus(By.LinkText("First Menu"));
            VerifyAreEqual("First Menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-link--text.lp-menu-header__link"))[0].Text,
                "1st item client");
            VerifyAreEqual("Second Menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-link--text.lp-menu-header__link"))[1].Text,
                "2nd item client");
            VerifyAreEqual("1 Sub menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-link--text.lp-menu-header__submenu-link"))[0].Text,
                "1st subitem client");
            VerifyAreEqual("2 Sub menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-link--text.lp-menu-header__submenu-link"))[1].Text,
                "2nd subitem client");

            GoToMobile("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            Driver.MouseFocus(By.LinkText("First Menu"));
            VerifyAreEqual("First Menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-menu-header-container--open .lp-menu-header__link"))[0]
                    .Text, "1st item in burger-menu");
            VerifyAreEqual("Second Menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-menu-header-container--open .lp-menu-header__link"))[1]
                    .Text, "2nd item in burger-menu");
            VerifyAreEqual("1 Sub menu",
                Driver.FindElements(
                    By.CssSelector("#block_1 .lp-menu-header-container--open .lp-menu-header__submenu-link"))[0].Text,
                "1st subitem client");
            VerifyAreEqual("2 Sub menu",
                Driver.FindElements(
                    By.CssSelector("#block_1 .lp-menu-header-container--open .lp-menu-header__submenu-link"))[1].Text,
                "2nd subitem client");

            //Drag`n`Drop in Menu
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderMenu");
            DragDrop(0, 1, "MenuGrid");
            Thread.Sleep(500);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Second Menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-link--text.lp-menu-header__link"))[0].Text,
                "2nd item client after");
            VerifyAreEqual("First Menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-link--text.lp-menu-header__link"))[1].Text,
                "1st item client after");

            GoToMobile("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Second Menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-menu-header-container--open .lp-menu-header__link"))[0]
                    .Text, "2nd item in burger-menu after");
            VerifyAreEqual("First Menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-menu-header-container--open .lp-menu-header__link"))[1]
                    .Text, "1st item in burger-menu after");

            //Drag`n`Drop in SubMenu
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderMenu");
            Driver.FindElement(By.CssSelector("[data-e2e-row-index=\"1\"] [data-e2e=\"closedGroups\"]")).Click();
            DragDrop(0, 1, "SubMenuGrid");
            Thread.Sleep(500);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            Driver.MouseFocus(By.LinkText("First Menu"));
            VerifyAreEqual("2 Sub menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-link--text.lp-menu-header__submenu-link"))[0].Text,
                "2nd subitem client after");
            VerifyAreEqual("1 Sub menu",
                Driver.FindElements(By.CssSelector("#block_1 .lp-link--text.lp-menu-header__submenu-link"))[1].Text,
                "1st subitem client after");

            GoToMobile("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            Driver.MouseFocus(By.LinkText("First Menu"));
            VerifyAreEqual("2 Sub menu",
                Driver.FindElements(
                    By.CssSelector("#block_1 .lp-menu-header-container--open .lp-menu-header__submenu-link"))[0].Text,
                "2nd subitem client after");
            VerifyAreEqual("1 Sub menu",
                Driver.FindElements(
                    By.CssSelector("#block_1 .lp-menu-header-container--open .lp-menu-header__submenu-link"))[1].Text,
                "1st subitem client after");

            VerifyFinally(TestName);
        }

        [Test]
        public void MenuItemsNewTab()
        {
            TestName = "MenuItemsNewTab";
            VerifyBegin(TestName);

            ReInitClient();
            GoToClient("lp/test1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.LinkText("Second Menu")).GetAttribute("href").Contains("/lp/test1/page1"),
                "link is correct before");
            VerifyAreNotEqual("_blank", Driver.FindElement(By.LinkText("Second Menu")).GetAttribute("target"),
                "new tab is off");

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderMenu");
            Driver.GetGridCell(0, "target").FindElement(By.TagName("span")).Click();
            Thread.Sleep(500);
            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.LinkText("Second Menu")).GetAttribute("href").Contains("/lp/test1/page1"),
                "link is correct after");
            VerifyAreEqual("_blank", Driver.FindElement(By.LinkText("Second Menu")).GetAttribute("target"),
                "new tab is on");

            VerifyFinally(TestName);
        }
    }
}