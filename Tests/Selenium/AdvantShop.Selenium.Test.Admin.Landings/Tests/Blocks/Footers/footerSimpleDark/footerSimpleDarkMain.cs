using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Footers.footerSimpleDark
{
    [TestFixture]
    public class footerSimpleDarkMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\CMS.LandingSiteSettings.csv"
            );
            Init();
        }

        private readonly string blockName = "footerSimpleDark";
        private readonly string blockType = "Footers";
        private readonly int numberBlock = 1;
        private readonly string text1 = "footerDarkText1";
        private readonly string text2 = "footerDarkText2";

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
            VerifyIsTrue(Driver.PageSource.Contains("footerSimpleDark, ID блока: 1"), "name and id correct");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2, "subblock displayed");
            VerifyAreEqual("text", Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"),
                "text subblock");
            VerifyAreEqual("text2",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"), "text2 subblock");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-footer")).Displayed,
                "displayed block in desktop");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + text1 + "\"]")).Displayed,
                "displayed text1 in client");
            VerifyAreEqual("© 2018 Разработано в AdvantShop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + text1 + "\"]")).Text, "text1 in client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + text2 + "\"]")).Displayed,
                "displayed text2 in client");
            VerifyAreEqual("г. Москва, ул. Облукова, д.5",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + text2 + "\"]")).Text, "text2 in client");

            //mobile version
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-footer")).Displayed,
                "displayed block in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + text1 + "\"]")).Displayed,
                "displayed text1 in mobile");
            VerifyAreEqual("© 2018 Разработано в AdvantShop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + text1 + "\"]")).Text, "text1 in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + text2 + "\"]")).Displayed,
                "displayed text2 in mobile");
            VerifyAreEqual("г. Москва, ул. Облукова, д.5",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + text2 + "\"]")).Text, "text2 in mobile");

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