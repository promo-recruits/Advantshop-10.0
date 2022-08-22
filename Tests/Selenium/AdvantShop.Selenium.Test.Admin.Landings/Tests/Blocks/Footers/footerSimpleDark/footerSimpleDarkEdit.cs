using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Footers.footerSimpleDark
{
    [TestFixture]
    public class footerSimpleDarkEdit : LandingsFunctions
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
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Footers\\footerSimpleDark\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "footerSimpleDark";
        private string blockType = "Footers";
        private int numberBlock = 1;
        private readonly string text1 = "footerDarkText1";
        private readonly string text2 = "footerDarkText2";

        [Test]
        public void Inplace()
        {
            TestName = "Inplace";
            VerifyBegin(TestName);

            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2, "all subblocks displayed");
            VerifyAreEqual("text", Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"),
                "text subblock");
            VerifyAreEqual("text2",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"), "text2 subblock");

            VerifyAreEqual("© 2018 Разработано в AdvantShop",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + text1 + "\"]")).Text, "block text initial");
            VerifyAreEqual("г. Москва, ул. Облукова, д.5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + text2 + "\"]")).Text, "block text2 initial");

            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + text1 + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + text1 + "\"] .inplace-initialized")).SendKeys("New Title");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + text2 + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + text2 + "\"] .inplace-initialized"))
                .SendKeys("New Subtitle");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("blocks-constructor-container")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + text1 + "\"]")).Text,
                "block text1");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + text2 + "\"]")).Text,
                "block text2");

            Refresh();

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + text1 + "\"]")).Text,
                "block text1 after refresh");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + text2 + "\"]")).Text,
                "block text2 after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + text1 + "\"]")).Text,
                "block text1 in client");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + text2 + "\"]")).Text,
                "block text2 in client");

            GoToMobile("lp/test1");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + text1 + "\"]")).Text,
                "block text1 in mobile");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + text2 + "\"]")).Text,
                "block text2 in mobile");

            VerifyFinally(TestName);
        }
    }
}