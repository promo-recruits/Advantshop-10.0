using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Characteristics.CharacteristicsBlock
{
    [TestFixture]
    public class LandingsCharacteristicsMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\CMS.LandingSiteSettings.csv"
            );

            Init();
        }

        private readonly string blockName = "characteristics";
        private readonly string blockType = "Characteristics";
        private readonly int numberBlock = 1;
        private readonly string blockNameClient = "lp-block-characteristics";
        private string blockNameClientfull = "lp-block-characteristics";

        [Test]
        public void AddBlock()
        {
            TestName = "AddBlock";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "no blocks on page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristics\"]")).Count == 0,
                "no characteristics category");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CharacteristicsName\"]")).Count == 0,
                "no characteristics name");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CharacteristicsValue\"]")).Count == 0,
                "no characteristics value");

            AddBlockByBtnBig(blockType, blockName);

            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 2,
                "new block on page");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor")).Count == 2, "block settings on page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AddBlockBtnSml\"]")).Count == 1,
                "add block btn up");
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "display add block");
            VerifyIsTrue(Driver.PageSource.Contains(blockName + ", ID блока: 1"), "name and id correct");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"TitleCharacteristics\"]")).Count == 1,
                "characteristics 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristics\"]")).Count > 0,
                "characteristics category 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CharacteristicsName\"]")).Count > 0,
                "characteristics name 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CharacteristicsValue\"]")).Count > 0,
                "characteristics value 1");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void MoveBlock()
        {
            TestName = "MoveBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            AddBlockByBtnBig("Services", "servicesAccordion");
            AddBlockByBtnBig("Services", "servicesAccordion");

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
        public void yDeleteBlock()
        {
            TestName = "DeleteBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            Driver.ScrollTo(By.Id("block_1"));
            var countBlock = Driver.FindElements(By.TagName("blocks-constructor-container")).Count;
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "block displayed");
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 1, "block is only one");

            DelBlockBtnCancel(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 1, "before delete block");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == countBlock,
                "count before del");
            DelBlockBtn(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 0, "after delete block");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == countBlock - 1,
                "count after del");

            DelBlocksAll();
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "count before del all");

            VerifyFinally(TestName);
        }
    }
}