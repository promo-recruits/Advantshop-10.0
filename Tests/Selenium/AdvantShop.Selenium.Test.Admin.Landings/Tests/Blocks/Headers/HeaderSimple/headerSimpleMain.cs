using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Headers.HeaderSimple
{
    [TestFixture]
    public class headerSimpleMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimple\\CMS.LandingSiteSettings.csv"
            );
            Init();
        }

        private readonly string blockName = "headerSimple";
        private readonly string blockType = "Headers";
        private readonly int numberBlock = 1;

        [Test]
        public void AddBlock()
        {
            TestName = "AddBlock";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "no blocks on page");
            AddBlockByBtnBig(blockType, blockName);

            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 2,
                "new block on page");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor")).Count == 2, "block settings on page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AddBlockBtnSml\"]")).Count == 1,
                "add block btn up");
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "display add block");
            VerifyIsTrue(Driver.PageSource.Contains("headerSimple, ID блока: 1"), "name and id correct");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo")).Displayed, "logo displayed");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimplePhone\"]")).Displayed,
                "phone displayed");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in desktop");

            //mobile version
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Count == 1,
                "burger menu in mobile");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".lp-menu-header--transform div")).GetAttribute("class")
                    .Contains("lp-menu-header-container--open"), "burger menu initial");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-menu-header--transform div")).GetAttribute("class")
                    .Contains("lp-menu-header-container--open"), "open burger menu");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuClose\"]")).Click();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".lp-menu-header--transform div")).GetAttribute("class")
                    .Contains("lp-menu-header-container--open"), "close burger menu");

            VerifyFinally(TestName);
        }

        [Test]
        public void MoveBlock()
        {
            TestName = "MoveBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            AddBlockByBtnBig("Characteristics", "characteristics");
            Thread.Sleep(2000);
            AddBlockByBtnBig("Characteristics", "characteristics");
            Thread.Sleep(2000);

            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[0].GetAttribute("id"),
                "initial position of block 1");
            VerifyAreEqual("block_2",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "initial position of block 2");
            VerifyAreEqual("block_3",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[2].GetAttribute("id"),
                "initial position of block 3");

            MoveDownBlockByBtn(numberBlock);
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Down block 1st");

            MoveDownBlockByBtn(numberBlock);
            VerifyAreEqual("block_2",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[0].GetAttribute("id"),
                "Move Down block 2nd /1");
            VerifyAreEqual("block_3",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Down block 2nd /2");
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[2].GetAttribute("id"),
                "Move Down block 2nd /3");

            MoveUpBlockByBtn(numberBlock);
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Up block 1st");

            MoveUpBlockByBtn(numberBlock);
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[0].GetAttribute("id"),
                "Move Up block 2nd /1");
            VerifyAreEqual("block_2",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Up block 2nd /2");
            VerifyAreEqual("block_3",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[2].GetAttribute("id"),
                "Move Up block 2nd /3");

            VerifyFinally(TestName);
        }

        [Test]
        public void zDeleteBlock()
        {
            TestName = "DeleteBlock";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            Driver.ScrollTo(By.Id("block_1"));

            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "block displayed");
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 1, "block is only one");

            DelBlockBtnCancel(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 1, "before delete block");

            DelBlockBtn(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 0, "after delete block");

            DelBlocksAll();
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "no blocks on page anymore");

            VerifyFinally(TestName);
        }
    }
}