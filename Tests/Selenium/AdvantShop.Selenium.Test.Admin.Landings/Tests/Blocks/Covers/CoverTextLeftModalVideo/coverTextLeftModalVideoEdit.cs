using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Covers.CoverTextLeftModalVideo
{
    [TestFixture]
    public class coverTextLeftModalVideoEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeftModalVideo\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "coverTextLeftModalVideo";
        private string blockType = "Covers";
        private int numberBlock = 1;
        private readonly string blockTitle = "CoverTLMVTitle";
        private readonly string blockText = "CoverTLMVText";

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