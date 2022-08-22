using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Countdown.countdownRow
{
    [TestFixture]
    public class LandingsCountdownRowMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Countdown\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Countdown\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Countdown\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Countdown\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Countdown\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Countdown\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Countdown\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Countdown\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Countdown\\CMS.LandingSiteSettings.csv"
            );

            Init();
        }

        private readonly string blockName = "countdownRow";
        private readonly string blockType = "Counters";
        private readonly int numberBlock = 1;
        private readonly string blockNameClient = "countdown-row";
        private string blockNameClientfull = "countdown-row";
        private readonly string blockTitle = "TitleCountdownrow";
        private readonly string blockSubTitle = "SubTitleCountdownrow";
        private readonly string blockContent = "ContentCountdownrow";


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
                "blockTitle 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "blockSubTitle 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "blockContent 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-countdown__item")).Count == 4, "item countdown");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.Id("block_1")).GetCssValue("background-color"), "background color");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.CssSelector("#block_1 .lp-countdown__item-part")).GetCssValue("color"),
                " color light");
            VerifyAreEqual("До конца акции осталось",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Text,
                "title desktop text");

            for (var i = 0; i < Driver.FindElements(By.CssSelector(".lp-countdown__item-part")).Count; i++)
                VerifyAreEqual("0", Driver.FindElements(By.CssSelector(".lp-countdown__item-part"))[i].Text,
                    "countdown elem: " + i);

            BlockSettingsBtn(numberBlock);
            TabSelect("tabCountdown");
            var selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"TypeCountdown\"]"));
            var select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("До указанной даты"), "Type Countdown ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"DateCountdown\"]")).GetAttribute("value")
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "Date Countdown");
            BlockSettingsClose();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "blockTitle client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "blockSubTitle client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "blockContent client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-countdown__item")).Count == 4,
                "item countdown client");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).GetCssValue("background-color"),
                "background color  client");
            VerifyAreEqual("До конца акции осталось",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Text,
                "title desktop text client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "blockTitle mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "blockSubTitle mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "blockContent mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-countdown__item")).Count == 4,
                "item countdown mobile");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).GetCssValue("background-color"),
                "background color  mobile");
            VerifyAreEqual("До конца акции осталось",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Text,
                "title desktop text mobile");

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