using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Countdown.Progressbar
{
    [TestFixture]
    public class LandingsProgressbarMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\CMS.LandingSiteSettings.csv"
            );

            Init();
        }

        private readonly string blockName = "progressbar";
        private readonly string blockType = "Counters";
        private readonly int numberBlock = 1;
        private readonly string blockNameClient = "block-progressbar";
        private string blockNameClientfull = "block-progressbar";
        private readonly string blockTitle = "TitleBlock";
        private readonly string blockSubTitle = "SubTitleBlock";
        private readonly string blockContent = "ContentProgressbar";

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

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 0,
                "blockTitle 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "blockSubTitle 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "blockContent 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-countdown__item")).Count == 0, "item countdown");
            VerifyAreEqual("Шаг 1 из 3. Оформление заказа",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarTextBlock\"]")).Text, "title text");

            VerifyAreEqual("rgba(23, 121, 250, 1)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]"))
                    .GetCssValue("background-color"), "background color passed");
            VerifyAreEqual("rgb(23, 121, 250)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"] path"))
                    .GetAttribute("fill"), "background color passed lable");
            VerifyAreEqual("rgb(23, 121, 250)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"] path"))[1]
                    .GetAttribute("fill"), "background color passed filllable");

            VerifyAreEqual("rgba(242, 242, 242, 0.9)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarAll\"]"))
                    .GetCssValue("background-color"), "background color all");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-countdown__item-part")).Count == 0,
                "0 countdown elem");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ContentProgressbar\"]")).Count == 1,
                "ContentProgressbar");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ProgressbarAll\"]")).Count == 1,
                "ProgressbarAll");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ProgressbarPassed\"]")).Count == 1,
                "ProgressbarPassed");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 50%"), "width passed");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabProgressbarData");
            VerifyAreEqual("50",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PercentPassed\"]")).GetAttribute("value"),
                "Percent Passed");
            VerifyAreEqual("rgb(23, 121, 250)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BackgroundColor\"] input")).GetAttribute("value"),
                "Percent Passed");
            BlockSettingsClose();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 0,
                "blockTitle client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "blockSubTitle client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "blockContent client");

            VerifyAreEqual("Шаг 1 из 3. Оформление заказа",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarTextBlock\"]")).Text,
                "title desktop text");
            VerifyAreEqual("rgba(23, 121, 250, 1)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]"))
                    .GetCssValue("background-color"), "background color passed client");
            VerifyAreEqual("rgba(242, 242, 242, 0.9)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarAll\"]"))
                    .GetCssValue("background-color"), "background color all client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 50%"), "width passed client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 0,
                "blockTitle mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "blockSubTitle mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "blockContent mobile");

            VerifyAreEqual("Шаг 1 из 3. Оформление заказа",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarTextBlock\"]")).Text,
                "title text mobile");
            VerifyAreEqual("rgba(23, 121, 250, 1)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]"))
                    .GetCssValue("background-color"), "background color passed mobile");
            VerifyAreEqual("rgba(242, 242, 242, 0.9)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarAll\"]"))
                    .GetCssValue("background-color"), "background color all mobile");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 50%"), "width passed mobile");

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
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1, "count after del");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 0, "after del");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 0, "after delete block after refresh");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "count after del after refresh");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 0,
                "count after del refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 0, "after delete block client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 0,
                "count after del client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 0, "after delete block mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 0,
                "count after del mobile");

            VerifyFinally(TestName);
        }
    }
}