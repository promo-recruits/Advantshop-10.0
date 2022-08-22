using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Team.teamThreeColumns
{
    [TestFixture]
    public class teamThreeColumnsMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSiteSettings.csv"
            );
            Init();
        }

        private readonly string blockName = "teamThreeColumns";
        private readonly string blockType = "Team";
        private readonly int numberBlock = 1;

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
            VerifyIsTrue(Driver.PageSource.Contains("teamThreeColumns, ID блока: 1"), "name and id correct");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2, "subblock displayed");
            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("subtitle",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "subtitle subblock");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-team-three-columns")).Displayed,
                "displayed block in desktop");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"teamThreeColumnsTitle\"]")).Displayed,
                "displayed title in client");
            VerifyAreEqual("Наша команда",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"teamThreeColumnsTitle\"]")).Text,
                "title in client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"teamThreeColumnsSubtitle\"]")).Displayed,
                "displayed subtitle in client");
            VerifyAreEqual("Advantshop предлагает широкий кадровый резерв специалистов",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"teamThreeColumnsSubtitle\"]")).Text,
                "subtitle in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsPic\"] img")).Count == 3,
                "all images displayed in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsPic\"] img"))[0].GetAttribute("src")
                    .Contains("areas/landing/images/team/teamThreeColumns1.jpg"), "1st image in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsPic\"] img"))[1].GetAttribute("src")
                    .Contains("areas/landing/images/team/teamThreeColumns2.jpg"), "2nd image in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsPic\"] img"))[2].GetAttribute("src")
                    .Contains("areas/landing/images/team/teamThreeColumns3.jpg"), "3d image in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsHeader\"]")).Count == 3,
                "all headers displayed in client");
            VerifyAreEqual("Дмитрий Чкалов",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsHeader\"]"))[0].Text,
                "1st header in client");
            VerifyAreEqual("Елена Волкова",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsHeader\"]"))[1].Text,
                "2nd header in client");
            VerifyAreEqual("Ярослав Громов",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsHeader\"]"))[2].Text,
                "3d header in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsText\"]")).Count == 3,
                "all texts displayed in client");
            VerifyAreEqual("Руководитель отдела маркетинга",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsText\"]"))[0].Text,
                "1st text in client");
            VerifyAreEqual("Руководитель Frontend отдела",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsText\"]"))[1].Text,
                "2nd text in client");
            VerifyAreEqual("Mobile-разработчик",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsText\"]"))[2].Text,
                "3d text in client");

            //mobile version
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-team-three-columns")).Displayed,
                "displayed block in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"teamThreeColumnsTitle\"]")).Displayed,
                "displayed title in mobile");
            VerifyAreEqual("Наша команда",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"teamThreeColumnsTitle\"]")).Text,
                "title in mobile");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"teamThreeColumnsSubtitle\"]")).Displayed,
                "displayed subtitle in mobile");
            VerifyAreEqual("Advantshop предлагает широкий кадровый резерв специалистов",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"teamThreeColumnsSubtitle\"]")).Text,
                "subtitle in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsPic\"] img")).Count == 3,
                "all image displayed in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsPic\"] img"))[0].GetAttribute("src")
                    .Contains("areas/landing/images/team/teamThreeColumns1.jpg"), "1st image in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsPic\"] img"))[1].GetAttribute("src")
                    .Contains("areas/landing/images/team/teamThreeColumns2.jpg"), "2nd image in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsPic\"] img"))[2].GetAttribute("src")
                    .Contains("areas/landing/images/team/teamThreeColumns3.jpg"), "3d image in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsHeader\"]")).Count == 3,
                "all headers displayed in mobile");
            VerifyAreEqual("Дмитрий Чкалов",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsHeader\"]"))[0].Text,
                "1st header in mobile");
            VerifyAreEqual("Елена Волкова",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsHeader\"]"))[1].Text,
                "2nd header in mobile");
            VerifyAreEqual("Ярослав Громов",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsHeader\"]"))[2].Text,
                "3d header in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsText\"]")).Count == 3,
                "all texts displayed in mobile");
            VerifyAreEqual("Руководитель отдела маркетинга",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsText\"]"))[0].Text,
                "1st text in mobile");
            VerifyAreEqual("Руководитель Frontend отдела",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsText\"]"))[1].Text,
                "2nd text in mobile");
            VerifyAreEqual("Mobile-разработчик",
                Driver.FindElements(By.CssSelector("[data-e2e=\"teamThreeColumnsText\"]"))[2].Text,
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