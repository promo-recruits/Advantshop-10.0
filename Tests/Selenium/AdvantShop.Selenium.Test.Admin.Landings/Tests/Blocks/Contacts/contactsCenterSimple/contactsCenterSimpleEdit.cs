using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Contacts.contactsCenterSimple
{
    [TestFixture]
    public class contactsCenterSimpleEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "contactsCenterSimple";
        private string blockType = "Contacts";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "contactsSimpleTitle";
        private readonly string blockSubTitle = "contactsSimpleSubtitle";
        private readonly string blockText = "contactsSimpleText";

        [Test]
        public void Inplace()
        {
            TestName = "Inplace";
            VerifyBegin(TestName);

            GoToClient("lp/test1");

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

            VerifyAreEqual("Контакты", Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text,
                "block title initial");
            VerifyAreEqual("Как с нами связаться",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "block subtitle initial");
            VerifyAreEqual("+7 (800) 333-68-03\r\nг. Москва, ул. Облукова, 5\r\ninfo@advantshop.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]")).Text, "block text initial");

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
            Driver.FindElement(By.CssSelector("blocks-constructor-container")).Click();
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

            GoToMobile("lp/test1");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title in mobile");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle in mobile");
            VerifyAreEqual("New Text",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))
                    .Text, "block text in mobile");

            VerifyFinally(TestName);
        }
    }
}