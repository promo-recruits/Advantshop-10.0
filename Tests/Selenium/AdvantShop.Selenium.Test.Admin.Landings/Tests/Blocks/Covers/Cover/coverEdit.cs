using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Covers.Cover
{
    [TestFixture]
    public class coverEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "cover";
        private string blockType = "Covers";
        private int numberBlock = 1;
        private readonly string blockTitle = "TitleCover";
        private readonly string blockText = "TextCover";

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
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"TextCover\"]")).Text,
                "cover text initial");

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
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Click();
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