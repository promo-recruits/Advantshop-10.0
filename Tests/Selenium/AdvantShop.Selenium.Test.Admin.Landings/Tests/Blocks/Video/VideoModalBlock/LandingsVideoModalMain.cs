using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Video.VideoModalBlock
{
    [TestFixture]
    public class LandingsVideoModalMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Video\\VideoModal\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\CMS.LandingSiteSettings.csv"
            );

            Init();
        }

        private readonly string blockName = "modalVideo";
        private readonly string blockType = "Video";
        private readonly int numberBlock = 1;
        private readonly string blockNameClient = "lp-block-modal-video";
        private string blockNameClientfull = "lp-block-modal-video";
        private readonly string blockTitle = "TitleModalVideo";
        private readonly string blockSubTitle = "SubTitleModalVideo";
        private readonly string blockContent = "ContentModalVideo";

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
            VerifyIsTrue(Driver.FindElements(By.Id("modalIframeVideo")).Count == 0, "no iframe");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "TitleVideo 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "SubTitleVideo 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "ContentVideo 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"VideoPlayBtn\"]")).Count == 1,
                "Video PlayBtn 1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"VideoPlayBtn\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.Id("modalIframeVideo")).Count == 1, "display iframe");
            VerifyIsTrue(
                Driver.FindElement(By.Id("modalIframeVideo")).FindElement(By.CssSelector(".adv-modal-close")).Displayed,
                "display iframe");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "TitleVideo client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "SubTitleVideo client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "ContentVideo client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"VideoPlayBtn\"]")).Count == 1,
                "Video PlayBtn client");
            VerifyAreEqual("https://www.youtube.com/watch?v=XEfDYMngJeE",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ContentModalVideo\"] modal-video"))
                    .GetAttribute("data-src"), "initial link video setting");

            Driver.FindElement(By.CssSelector("[data-e2e=\"VideoPlayBtn\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.Id("modalIframeVideo")).Displayed, "display iframe client");
            VerifyIsTrue(
                Driver.FindElement(By.Id("modalIframeVideo")).FindElement(By.CssSelector(".adv-modal-close")).Displayed,
                "display iframe client");
            VerifyAreEqual("https://www.youtube.com/watch?v=XEfDYMngJeE",
                Driver.FindElement(By.CssSelector(".modal-iframe-video-inner iframe-responsive")).GetAttribute("src"),
                "url-video setting");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "TitleVideo mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "SubTitleVideo mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "ContentVideo mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"VideoPlayBtn\"]")).Count == 1,
                "Video PlayBtn mobile");

            Driver.FindElement(By.CssSelector("[data-e2e=\"VideoPlayBtn\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.Id("modalIframeVideo")).Displayed, "display iframe mobile");
            VerifyIsTrue(
                Driver.FindElement(By.Id("modalIframeVideo")).FindElement(By.CssSelector(".adv-modal-close")).Displayed,
                "display iframe mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void DeleteBlock()
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