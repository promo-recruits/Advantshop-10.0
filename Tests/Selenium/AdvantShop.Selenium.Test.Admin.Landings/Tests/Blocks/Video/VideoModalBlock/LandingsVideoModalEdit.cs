using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Video.VideoModalBlock
{
    [TestFixture]
    public class LandingsVideoModalEdit : LandingsFunctions
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
                "data\\Admin\\LandingTest\\Video\\VideoModal\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Video\\VideoModal\\CMS.LandingSubBlock.csv"
            );

            Init();
        }

        private string blockName = "modalVideo";
        private string blockType = "Video";
        private readonly int numberBlock = 1;
        private string blockNameClient = "lp-block-modal-video";
        private string blockNameClientfull = "lp-block-modal-video";
        private readonly string blockTitle = "TitleModalVideo";
        private readonly string blockSubTitle = "SubTitleModalVideo";

        [Test]
        public void EditLocateVideoBtnSettings()
        {
            TestName = "EditLocateVideoBtnSettings";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);

            TabSelect("tabCoverVideo");

            var selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"SelectLocateVideo\"]"));
            var select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("По центру"), "initial locate video");

            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"SelectLocateVideo\"]")))
                .SelectByText("Слева");
            Thread.Sleep(500);
            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentModalVideo\"] subblock-inplace"))
                    .GetAttribute("data-settings").Contains("start-xs"), "left locate btn video");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentModalVideo\"] subblock-inplace"))
                    .GetAttribute("data-settings").Contains("end-xs"), "no right locate btn video");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentModalVideo\"] subblock-inplace"))
                    .GetAttribute("data-settings").Contains("center-xs"), "no center locate btn video");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentModalVideo\"] subblock-inplace"))
                    .GetAttribute("data-settings").Contains("start-xs"), "left locate btn video after refresh");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabCoverVideo");

            selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"SelectLocateVideo\"]"));
            select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Слева"), "changed locate video");

            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"SelectLocateVideo\"]")))
                .SelectByText("Справа");
            Thread.Sleep(500);
            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentModalVideo\"] subblock-inplace"))
                    .GetAttribute("data-settings").Contains("end-xs"), "right locate btn video");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabCoverVideo");

            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"SelectLocateVideo\"]")))
                .SelectByText("По центру");
            Thread.Sleep(500);
            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentModalVideo\"] subblock-inplace"))
                    .GetAttribute("data-settings").Contains("center-xs"), "center locate btn video");

            VerifyFinally(TestName);
        }

        [Test]
        public void EditLinkVideoSettings()
        {
            TestName = "EditLinkVideoSettings";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentModalVideo\"] subblock-inplace"))
                    .GetAttribute("data-settings").Contains("https://www.youtube.com/watch?v=XEfDYMngJeE"),
                "initial link video");
            BlockSettingsBtn(numberBlock);

            TabSelect("tabCoverVideo");
            VerifyAreEqual("https://www.youtube.com/watch?v=XEfDYMngJeE",
                Driver.FindElement(By.CssSelector("[data-e2e=\"UrlVideo\"]")).GetAttribute("value"),
                "initial link video setting");

            Driver.FindElement(By.CssSelector("[data-e2e=\"UrlVideo\"]")).Clear();
            Thread.Sleep(500);
            BlockSettingsSave();
            // add check for empty link
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);
            TabSelect("tabCoverVideo");
            Driver.FindElement(By.CssSelector("[data-e2e=\"UrlVideo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"UrlVideo\"]")).SendKeys("https://youtu.be/8d8Kyx-h-P8");
            BlockSettingsSave();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"ContentModalVideo\"] subblock-inplace"))
                    .GetAttribute("data-settings").Contains("https://youtu.be/8d8Kyx-h-P8"), "link video");
            VerifyAreEqual("СОЗДАВАЙ С ADVANTSHOP",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"TitleModalVideo\"]")).Text,
                "title desktop text");

            Driver.FindElement(By.CssSelector("[data-e2e=\"VideoPlayBtn\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-iframe-video-inner")).Displayed, "displayed modal");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-iframe-video-inner")).Count == 1, "count modal");
            VerifyAreEqual("https://youtu.be/8d8Kyx-h-P8",
                Driver.FindElement(By.CssSelector(".modal-iframe-video-inner iframe-responsive")).GetAttribute("src"),
                "url-video setting");

            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-iframe-video-inner")).Count == 0,
                "no displayed modal");


            VerifyFinally(TestName);
        }

        [Test]
        public void InplaceHeaders()
        {
            TestName = "InplaceHeaders";
            VerifyBegin(TestName);

            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            ShowTitle();
            ShowSubTitle();
            BlockSettingsSave();

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized"))
                .SendKeys("New Title");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized"))
                .SendKeys("New SubTitle");

            Thread.Sleep(1000);
            MoveUpBlockByBtn(1);
            Thread.Sleep(1000);

            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Text,
                "subtitle ");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Text,
                "title ");

            Refresh();
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Text,
                "subtitle after refresh");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Text,
                "title after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text, "subtitle client");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text, "title client");

            GoToMobile("lp/test1");
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text, "subtitle mobile");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text, "title mobile");

            VerifyFinally(TestName);
        }
    }
}