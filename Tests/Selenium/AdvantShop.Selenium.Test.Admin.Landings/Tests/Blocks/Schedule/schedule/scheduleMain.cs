using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Schedule.schedule
{
    [TestFixture]
    public class scheduleMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Sсhedule\\sсhedule\\CMS.LandingSiteSettings.csv"
            );
            Init();
        }

        private readonly string blockName = "schedule";
        private readonly string blockType = "Schedule";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "TitleBlock";
        private readonly string blockSubTitle = "SubTitleBlock";
        private readonly string blockImage = "schedulePic";
        private readonly string blockColTitle = "scheduleName";
        private readonly string blockColSubtitle = "schedulePosition";
        private readonly string blockColText = "scheduleText";
        private readonly string blockColTime = "scheduleTime";

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
            VerifyIsTrue(Driver.PageSource.Contains("schedule, ID блока: 1"), "name and id correct");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2, "subblock displayed");
            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("subtitle",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "subtitle subblock");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-block-schedule__item")).Count == 1, "block_item");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-schedule")).Displayed,
                "displayed block in desktop");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Displayed,
                "displayed title in client");
            VerifyAreEqual("Расписание",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Text,
                "title in client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockSubTitle + "\"]")).Displayed,
                "displayed subtitle in client");
            VerifyAreEqual("Advantshop предлагает широкий выбор специлистов",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "subtitle in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).Count == 1,
                "all images displayed in client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/schedule/schedule1.jpg"), "Image in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]")).Count == 1,
                "all headers displayed in client");
            VerifyAreEqual("Дмитрий Чкалов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]")).Text, "Header in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]")).Count == 1,
                "all position displayed in client");
            VerifyAreEqual("Руководитель отдела маркетинга",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]")).Text,
                "Position in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]")).Count == 1,
                "all texts displayed in client");
            VerifyAreEqual("В какой момент дизайн-команде нужно начинать делать свою дизайн-систему",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]")).Text, "Text in client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]")).Count == 1,
                "all time displayed in client");
            VerifyAreEqual("09:30 - 10:15",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]")).Text, "Time in client");

            //mobile version
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-schedule")).Displayed,
                "displayed block in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Displayed,
                "displayed title in mobile");
            VerifyAreEqual("Расписание",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Text,
                "title in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockSubTitle + "\"]")).Displayed,
                "displayed subtitle in mobile");
            VerifyAreEqual("Advantshop предлагает широкий выбор специлистов",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "subtitle in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).Count == 1,
                "all images displayed in mobile");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockImage + "\"] img")).GetAttribute("src")
                    .Contains("areas/landing/frontend/images/schedule/schedule1.jpg"), "Image in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]")).Count == 1,
                "all headers displayed in mobile");
            VerifyAreEqual("Дмитрий Чкалов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColTitle + "\"]")).Text, "Header in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]")).Count == 1,
                "all position displayed in mobile");
            VerifyAreEqual("Руководитель отдела маркетинга",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColSubtitle + "\"]")).Text,
                "Position in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColText + "\"]")).Count == 1,
                "all texts displayed in mobile");
            VerifyAreEqual("В какой момент дизайн-команде нужно начинать делать свою дизайн-систему",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColText + "\"]")).Text, "Text in mobile");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]")).Count == 1,
                "all time displayed in mobile");
            VerifyAreEqual("09:30 - 10:15",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColTime + "\"]")).Text, "Time in mobile");

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