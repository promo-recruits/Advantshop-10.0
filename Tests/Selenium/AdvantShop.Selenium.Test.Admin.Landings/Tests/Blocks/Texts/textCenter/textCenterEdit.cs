using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Texts.textCenter
{
    [TestFixture]
    public class textCenterEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Texts\\textCenter\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Texts\\textCenter\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "textCenter";
        private string blockType = "Text";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "textCenterTitle";
        private readonly string blockSubTitle = "textCenterSubtitle";
        private readonly string blockText = "textCenterText";

        [Test]
        public void Inplace()
        {
            TestName = "Inplace";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 3,
                "not all subblocks displayed");

            BlockSettingsBtn(numberBlock);
            ShowTitle();
            ShowSubTitle();
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 3, "all subblocks displayed");
            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("subtitle",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "subtitle subblock");
            VerifyAreEqual("text", Driver.FindElements(By.CssSelector("subblock-inplace"))[2].GetAttribute("data-name"),
                "text subblock");

            VerifyAreEqual("Вдохновляйся идеями",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title initial");
            VerifyAreEqual("Реализуй все с Advantshop",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle initial");
            VerifyAreEqual(
                "Простой инструмент для онлайн-продаж. Зарабатывайте больше, используя различные каналы продаж, и выстраивайте правильные отношения с клиентом.",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))
                    .Text, "block text initial");

            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized"))
                .SendKeys("New Title");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized"))
                .SendKeys("New Subtitle");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized"))
                .SendKeys("New Text");
            Thread.Sleep(500);

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle");
            VerifyAreEqual("New Text",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))
                    .Text, "block text");
            Driver.FindElement(By.CssSelector("blocks-constructor-container")).Click();
            Thread.Sleep(1000);

            Refresh();

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title after refresh");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle after refresh");
            VerifyAreEqual("New Text",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))
                    .Text, "block text after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title in client");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle in client");
            VerifyAreEqual("New Text",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))
                    .Text, "block text in client");


            VerifyFinally(TestName);
        }
    }
}