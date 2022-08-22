using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Video.VideoBlock
{
    [TestFixture]
    public class LandingsVideoMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Video\\Video\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Video\\Video\\CMS.LandingSiteSettings.csv"
            );

            Init();
        }

        private readonly string blockName = "video";
        private readonly string blockType = "Video";
        private readonly int numberBlock = 1;
        private readonly string blockNameClient = "lp-block-video";
        private readonly string blockTitle = "TitleVideo";
        private readonly string blockSubTitle = "SubTitleVideo";
        private readonly string blockContent = "ContentVideo";

        [Test]
        public void AddBlock()
        {
            TestName = "AddBlock";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "no blocks on page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 0,
                "no TitleVideo");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "no SubTitleVideo");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 0,
                "no ContentVideo");

            AddBlockByBtnBig(blockType, blockName);

            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 2,
                "new block on page");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor")).Count == 2, "block settings on page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AddBlockBtnSml\"]")).Count == 1,
                "add block btn up");
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "display add block");
            VerifyIsTrue(Driver.PageSource.Contains(blockName + ", ID блока: 1"), "name and id correct");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "TitleVideo 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "SubTitleVideo 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "ContentVideo 1");

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
        public void DeleteBlock()
        {
            TestName = "DeleteBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
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

            VerifyFinally(TestName);
        }
    }
}