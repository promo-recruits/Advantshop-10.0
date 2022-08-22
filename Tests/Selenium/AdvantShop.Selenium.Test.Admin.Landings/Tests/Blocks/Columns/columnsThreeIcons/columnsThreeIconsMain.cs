using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Columns.columnsThreeIcons
{
    [TestFixture]
    public class columnsThreeIconsMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Columns\\columnsThreeIcons\\CMS.LandingSiteSettings.csv"
            );
            Init();
        }

        private readonly string blockName = "columnsThreeIcons";
        private readonly string blockType = "Columns";
        private readonly int numberBlock = 1;
        private string blockTitle = "columnsThreeIconsIcon";
        private string blockSubTitle = "columnsThreeIconsHeader";
        private string blockText = "columnsThreeIconsText";

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
            VerifyIsTrue(Driver.PageSource.Contains("columnsThreeIcons, ID блока: 1"), "name and id correct");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 1, "subblock displayed");
            VerifyAreEqual("title", Driver.FindElement(By.CssSelector("subblock-inplace")).GetAttribute("data-name"),
                "title subblock");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-columns-three-icons")).Displayed,
                "displayed block in desktop");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"columnsThreeIconsTitle\"]")).Displayed,
                "displayed title in client");
            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"columnsThreeIconsTitle\"]")).Text,
                "title in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsIcon\"] img")).Count == 3,
                "all icons displayed in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsIcon\"] img"))[0].GetAttribute("src")
                    .Contains("areas/landing/images/columns/good-review.png"), "1st icon in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsIcon\"] img"))[1].GetAttribute("src")
                    .Contains("areas/landing/images/columns/love.png"), "2nd icon in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsIcon\"] img"))[2].GetAttribute("src")
                    .Contains("areas/landing/images/columns/megaphone.png"), "3d icon in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsHeader\"]")).Count == 3,
                "all headers displayed in client");
            VerifyAreEqual("Трансформер дизайна",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsHeader\"]"))[0].Text,
                "1st header in client");
            VerifyAreEqual("CRM для интернет-магазина",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsHeader\"]"))[1].Text,
                "2nd header in client");
            VerifyAreEqual("Регистрация SSL",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsHeader\"]"))[2].Text,
                "3d header in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsText\"]")).Count == 3,
                "all texts displayed in client");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (расширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsText\"]"))[0].Text,
                "1st text in client");
            VerifyAreEqual("Имманентный подход подразумевает отношение к тексту как к автономной реальности",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsText\"]"))[1].Text,
                "2nd text in client");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (рас-ширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsText\"]"))[2].Text,
                "3d text in client");

            //mobile version
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-columns-three-icons")).Displayed,
                "displayed block in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"columnsThreeIconsTitle\"]")).Displayed,
                "displayed title in mobile");
            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"columnsThreeIconsTitle\"]")).Text,
                "title in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsIcon\"] img")).Count == 3,
                "all icons displayed in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsIcon\"] img"))[0].GetAttribute("src")
                    .Contains("areas/landing/images/columns/good-review.png"), "1st icon in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsIcon\"] img"))[1].GetAttribute("src")
                    .Contains("areas/landing/images/columns/love.png"), "2nd icon in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsIcon\"] img"))[2].GetAttribute("src")
                    .Contains("areas/landing/images/columns/megaphone.png"), "3d icon in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsHeader\"]")).Count == 3,
                "all headers displayed in mobile");
            VerifyAreEqual("Трансформер дизайна",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsHeader\"]"))[0].Text,
                "1st header in mobile");
            VerifyAreEqual("CRM для интернет-магазина",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsHeader\"]"))[1].Text,
                "2nd header in mobile");
            VerifyAreEqual("Регистрация SSL",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsHeader\"]"))[2].Text,
                "3d header in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsText\"]")).Count == 3,
                "all texts displayed in mobile");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (расширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsText\"]"))[0].Text,
                "1st text in mobile");
            VerifyAreEqual("Имманентный подход подразумевает отношение к тексту как к автономной реальности",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsText\"]"))[1].Text,
                "2nd text in mobile");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (рас-ширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"columnsThreeIconsText\"]"))[2].Text,
                "3d text in mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void MoveBlock()
        {
            TestName = "MoveBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            AddBlockByBtnBig("Headers", "headerCommunicationSimple");
            AddBlockByBtnBig("Headers", "headerCommunicationSimple");

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