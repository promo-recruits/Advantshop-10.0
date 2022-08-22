using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Covers.CoverAlt
{
    [TestFixture]
    public class coverAltEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "coverAlt";
        private string blockType = "Covers";
        private int numberBlock = 1;
        private readonly string blockTitle = "CoverAltTitle";
        private readonly string blockText = "CoverAltText";

        [Test]
        public void Inplace()
        {
            TestName = "Inplace";
            VerifyBegin(TestName);

            GoToClient("lp/test1");

            VerifyAreEqual("Создавай с AdvantShop",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "cover title initial");
            VerifyAreEqual(
                "Простой инструмент для онлайн-продаж. Зарабатывайте больше, используя различные каналы продаж, и выстраивайте правильные отношения с клиентом.",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))
                    .Text, "cover text initial");

            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized"))
                .SendKeys("New Title");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"] .inplace-initialized"))
                .SendKeys("New Text");
            Thread.Sleep(500);
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "cover title");
            VerifyAreEqual("New Text",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))
                    .Text, "cover text");
            Driver.FindElement(By.CssSelector("#block_1")).Click();
            Thread.Sleep(1000);

            Refresh();

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "cover title after refresh");
            VerifyAreEqual("New Text",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))
                    .Text, "cover text after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "cover title in client");
            VerifyAreEqual("New Text",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]"))
                    .Text, "cover text in client");

            VerifyFinally(TestName);
        }
    }
}