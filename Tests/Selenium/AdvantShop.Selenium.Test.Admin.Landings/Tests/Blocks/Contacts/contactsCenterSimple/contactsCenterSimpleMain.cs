using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Contacts.contactsCenterSimple
{
    [TestFixture]
    public class contactsCenterSimpleMain : LandingsFunctions
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
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.LandingSiteSettings.csv"
            );
            Init();
        }

        private readonly string blockName = "contactsCenterSimple";
        private readonly string blockType = "Contacts";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "contactsSimpleTitle";
        private readonly string blockText = "contactsSimpleText";

        [Test]
        public void AddBlock()
        {
            TestName = "AddBlock";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "no blocks on page");
            AddBlockByBtnBig(blockType, blockName);

            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 2,
                "new block on page");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor")).Count == 2, "block settings on page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AddBlockBtnSml\"]")).Count == 1,
                "add block btn up");
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "display add block");
            VerifyIsTrue(Driver.PageSource.Contains("contactsCenterSimple, ID блока: 1"), "name and id correct");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2, "subblock displayed");
            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("text", Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "text subblock");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-contacts-center-simple")).Displayed,
                "displayed block in desktop");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Displayed,
                "displayed title in client");
            VerifyAreEqual("Контакты",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Text,
                "title in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]")).Count == 1,
                "all texts displayed in client");
            VerifyAreEqual("+7 (800) 333-68-03\r\nг. Москва, ул. Облукова, 5\r\ninfo@advantshop.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]")).Text, "Text in client");

            //mobile version
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-contacts-center-simple")).Displayed,
                "displayed block in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Displayed,
                "displayed title in mobile");
            VerifyAreEqual("Контакты",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Text,
                "title in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockText + "\"]")).Count == 1,
                "all texts displayed in mobile");
            VerifyAreEqual("+7 (800) 333-68-03\r\nг. Москва, ул. Облукова, 5\r\ninfo@advantshop.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockText + "\"]")).Text, "Text in mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void MoveBlock()
        {
            TestName = "MoveBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            AddBlockByBtnBig("Text", "textHeader");
            Thread.Sleep(2000);
            AddBlockByBtnBig("Text", "textHeader");
            Thread.Sleep(2000);

            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[0].GetAttribute("id"),
                "initial position of block 1");
            VerifyAreEqual("block_2",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "initial position of block 2");
            VerifyAreEqual("block_3",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[2].GetAttribute("id"),
                "initial position of block 3");

            MoveDownBlockByBtn(numberBlock);
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Down block 1st");

            MoveDownBlockByBtn(numberBlock);
            VerifyAreEqual("block_2",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[0].GetAttribute("id"),
                "Move Down block 2nd /1");
            VerifyAreEqual("block_3",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Down block 2nd /2");
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[2].GetAttribute("id"),
                "Move Down block 2nd /3");

            MoveUpBlockByBtn(numberBlock);
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Up block 1st");

            MoveUpBlockByBtn(numberBlock);
            VerifyAreEqual("block_1",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[0].GetAttribute("id"),
                "Move Up block 2nd /1");
            VerifyAreEqual("block_2",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[1].GetAttribute("id"),
                "Move Up block 2nd /2");
            VerifyAreEqual("block_3",
                Driver.FindElements(By.CssSelector("blocks-constructor-container .lp-block"))[2].GetAttribute("id"),
                "Move Up block 2nd /3");

            VerifyFinally(TestName);
        }

        [Test]
        public void zDeleteBlock()
        {
            TestName = "DeleteBlock";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            Driver.ScrollTo(By.Id("block_1"));

            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "block displayed");
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 1, "block is only one");

            DelBlockBtnCancel(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 1, "before delete block");

            DelBlockBtn(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 0, "after delete block");

            DelBlocksAll();
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "no blocks on page anymore");

            VerifyFinally(TestName);
        }
    }
}