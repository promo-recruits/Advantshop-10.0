using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Countdown.Progressbar
{
    [TestFixture]
    public class LandingsProgressbarSettings : LandingsFunctions
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
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Countdown\\ProgressBar\\CMS.LandingSubBlock.csv"
            );

            Init();
        }

        private string blockName = "progressbar";
        private string blockType = "Counters";
        private readonly int numberBlock = 1;
        private string blockNameClient = "block-progressbar";
        private string blockNameClientfull = "block-progressbar";
        private readonly string blockTitle = "TitleBlock";
        private readonly string blockSubTitle = "SubTitleBlock";
        private string blockContent = "ContentProgressbar";

        [Test]
        public void EditColorLine()
        {
            TestName = "EditColorLine";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabProgressbarData");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".color-picker-open")).Count == 0, "count picker");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".color-picker-closed")).Count == 1, "no picker");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BackgroundColor\"] input")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".color-picker-open")).Count == 1, "count picker 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".color-picker-closed")).Count == 0, "no picker 1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BackgroundColor\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BackgroundColor\"] input"))
                .SendKeys("rgba(231, 99, 99, 0.7)");

            BlockSettingsSave();

            VerifyAreEqual("rgba(231, 99, 99, 0.7)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]"))
                    .GetCssValue("background-color"), "background color passed");
            VerifyAreEqual("rgba(231, 99, 99, 0.7)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"] path"))
                    .GetAttribute("fill"), "background color passed lable");
            VerifyAreEqual("rgba(231, 99, 99, 0.7)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"] path"))[1]
                    .GetAttribute("fill"), "background color passed filllable ");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("rgba(231, 99, 99, 0.7)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]"))
                    .GetCssValue("background-color"), "background color passed client");
            VerifyAreEqual("rgba(231, 99, 99, 0.7)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"] path"))
                    .GetAttribute("fill"), "background color passed lable client");
            VerifyAreEqual("rgba(231, 99, 99, 0.7)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"] path"))[1]
                    .GetAttribute("fill"), "background color passed filllable client");

            GoToMobile("lp/test1");
            VerifyAreEqual("rgba(231, 99, 99, 0.7)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]"))
                    .GetCssValue("background-color"), "background color passed mobile");
            VerifyAreEqual("rgba(231, 99, 99, 0.7)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"] path"))
                    .GetAttribute("fill"), "background color passed lable mobile");
            VerifyAreEqual("rgba(231, 99, 99, 0.7)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"] path"))[1]
                    .GetAttribute("fill"), "background color passed filllable ");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabProgressbarData");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BackgroundColor\"] input")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BackgroundColor\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BackgroundColor\"] input"))
                .SendKeys("rgba(168, 168, 168, 0.5)");

            BlockSettingsSave();

            VerifyAreEqual("rgba(168, 168, 168, 0.5)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]"))
                    .GetCssValue("background-color"), "background color passed 2");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("rgba(168, 168, 168, 0.5)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]"))
                    .GetCssValue("background-color"), "background color passed client 2");

            GoToMobile("lp/test1");
            VerifyAreEqual("rgba(168, 168, 168, 0.5)",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]"))
                    .GetCssValue("background-color"), "background color passed mobile 2");

            VerifyFinally(TestName);
        }

        [Test]
        public void EditWidth()
        {
            TestName = "EditWidth";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabProgressbarData");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BackgroundColor\"] input")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"PercentPassed\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PercentPassed\"]")).SendKeys("80");

            BlockSettingsSave();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 80%"), "width passed");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 50%"), "old width passed");
            VerifyAreEqual("left: 80%;",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"]")).GetAttribute("style"),
                "width passed lable");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"]")).GetAttribute("style")
                    .Contains("left: 50%;"), "old width passed lable");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 80%"), "width passed client");
            VerifyAreEqual("left: 80%;",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"]")).GetAttribute("style"),
                "width passed lable client");

            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 80%"), "width passed mobile");
            VerifyAreEqual("left: 80%;",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"]")).GetAttribute("style"),
                "width passed lable mobile");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabProgressbarData");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BackgroundColor\"] input")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"PercentPassed\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PercentPassed\"]")).SendKeys("42");

            BlockSettingsSave();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 42%"), "width passed 2");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 80%"), "old width passed 2");
            VerifyAreEqual("left: 42%;",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"]")).GetAttribute("style"),
                "width passed labl 2e");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 42%"), "width passed client 2");
            VerifyAreEqual("left: 42%;",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"]")).GetAttribute("style"),
                "width passed lable client 2");

            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ProgressbarPassed\"]")).GetAttribute("style")
                    .Contains("width: 42%"), "width passed mobile 2");
            VerifyAreEqual("left: 42%;",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentProgressbar\"]")).GetAttribute("style"),
                "width passed lable mobile 2");

            VerifyFinally(TestName);
        }

        [Test]
        public void InplaceHeaders()
        {
            TestName = "InplaceHeaders";
            VerifyBegin(TestName);
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            ShowTitle();
            ShowSubTitle();
            BlockSettingsSave();

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized"))
                .SendKeys("New SubTitle");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ContentProgressbar\"]")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized"))
                .SendKeys("New Title");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ProgressbarTextBlock\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ProgressbarTextBlock\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ProgressbarTextBlock\"] .inplace-initialized"))
                .SendKeys("New Text Progressbar Steps");

            Thread.Sleep(1000);

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Text,
                "subtitle ");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Text,
                "title ");
            VerifyAreEqual("New Text Progressbar Steps",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ProgressbarTextBlock\"] .inplace-initialized")).Text,
                "steps ");

            Refresh();
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Text,
                "subtitle after refresh");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Text,
                "title after refresh");
            VerifyAreEqual("New Text Progressbar Steps",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ProgressbarTextBlock\"] .inplace-initialized")).Text,
                "steps after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text, "subtitle client");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text, "title client");
            VerifyAreEqual("New Text Progressbar Steps",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ProgressbarTextBlock\"]")).Text, "steps client");

            GoToMobile("lp/test1");
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text, "subtitle mobile");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text, "title mobile");
            VerifyAreEqual("New Text Progressbar Steps",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ProgressbarTextBlock\"]")).Text, "steps mobile");

            VerifyFinally(TestName);
        }
    }
}