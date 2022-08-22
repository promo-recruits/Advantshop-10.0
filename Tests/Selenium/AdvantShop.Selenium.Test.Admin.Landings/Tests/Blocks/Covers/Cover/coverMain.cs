using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Covers.Cover
{
    [TestFixture]
    public class coverMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSiteSettings.csv"
            );
            Init();
        }

        private readonly string blockName = "cover";
        private readonly string blockType = "Covers";
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
            VerifyIsTrue(Driver.PageSource.Contains("cover, ID блока: 1"), "name and id correct");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2,
                "text subblocks displayed");
            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("text", Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "text subblock");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-cover-btn-wrap .lp-btn")).Count == 1,
                "button displayed");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).Displayed,
                "displayed block in desktop");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-cover-header")).Displayed,
                "displayed title in client");
            VerifyAreEqual("Создавай с AdvantShop",
                Driver.FindElement(By.CssSelector("#block_1 .lp-cover-header")).Text, "title in client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-cover-text")).Displayed,
                "displayed text in client");
            VerifyAreEqual(
                "Простой инструмент для онлайн-продаж. Зарабатывайте больше, используя различные каналы продаж, и выстраивайте правильные отношения с клиентом.",
                Driver.FindElement(By.CssSelector("#block_1 .lp-cover-text")).Text, "text in client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-btn.lp-btn--primary")).Displayed,
                "displayed button in client");
            VerifyAreEqual("Начать работу", Driver.FindElement(By.CssSelector("#block_1 .lp-btn.lp-btn--primary")).Text,
                "button in client");

            //mobile version
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).Displayed,
                "displayed block in mobile");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-cover-header")).Displayed,
                "displayed title in mobile");
            VerifyAreEqual("Создавай с AdvantShop",
                Driver.FindElement(By.CssSelector("#block_1 .lp-cover-header")).Text, "title in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-cover-text")).Displayed,
                "displayed text in mobile");
            VerifyAreEqual(
                "Простой инструмент для онлайн-продаж. Зарабатывайте больше, используя различные каналы продаж, и выстраивайте правильные отношения с клиентом.",
                Driver.FindElement(By.CssSelector("#block_1 .lp-cover-text")).Text, "text in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-btn.lp-btn--primary")).Displayed,
                "displayed button in mobile");
            VerifyAreEqual("Начать работу", Driver.FindElement(By.CssSelector("#block_1 .lp-btn.lp-btn--primary")).Text,
                "button in mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void MoveBlock()
        {
            TestName = "MoveBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            AddBlockByBtnBig("Text", "textHeader");
            Thread.Sleep(2000);
            AddBlockByBtnBig("Text", "textHeader");
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