using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns
{
    [TestFixture]
    public class LandingsServicesColumnsMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSiteSettings.csv"
            );

            Init();
        }

        private readonly string blockName = "servicesThreeColumns";
        private readonly string blockType = "Services";
        private readonly int numberBlock = 1;
        private readonly string blockNameClient = "lp-block-services-three-columns";
        private readonly string blockTitle = "TitleBlock";
        private readonly string blockSubTitle = "SubTitleBlock";
        private readonly string blockContent = "ServicesContent";

        [Test]
        public void AddBlock()
        {
            TestName = "AddBlock";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "no blocks on page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 0,
                "no Title");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "no SubTitle");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 0,
                "no Content");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesLoadPic\"]")).Count == 0,
                "no ServicesLoadPic");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Count == 0,
                "no ServicesItemHeader");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Count == 0,
                "no ServicesItemText");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 0,
                "no ServicesPrice");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]")).Count == 0,
                "no ServicesItem");

            AddBlockByBtnBig(blockType, blockName);

            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 2,
                "new block on page");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor")).Count == 2, "block settings on page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AddBlockBtnSml\"]")).Count == 1,
                "add block btn up");
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "display add block");
            VerifyIsTrue(Driver.PageSource.Contains(blockName + ", ID блока: 1"), "name and id correct");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "Title 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "SubTitle 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockContent + "\"]")).Count == 1,
                "Content 1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesLoadPic\"]")).Count == 3,
                "ServicesLoadPic");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Count == 3,
                "ServicesItemHeader");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Count == 3,
                "ServicesItemText");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 3,
                "ServicesPrice");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]")).Count == 3, "ServicesItem");

            VerifyAreEqual("Более 5000 функций для удобных и эффективных продаж",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "sub title desktop text");
            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"" + blockTitle + "\"]")).Text,
                "title desktop text");

            VerifyAreEqual("Трансформер дизайна",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text,
                "ServicesItemHeader text");
            VerifyAreEqual("Оформите интрернет магазин. Выберите более 100 тем, цветовых схем и фонов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Text, "ServicesItemText text");
            VerifyAreEqual("7 000 руб", Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Text,
                "ServicesPrice text");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void DeleteBlock()
        {
            TestName = "DeleteBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            var countBlock = Driver.FindElements(By.TagName("blocks-constructor-container")).Count;
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "block displayed");
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 1, "block is only one");

            DelBlockBtnCancel(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 1, "before delete block");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == countBlock,
                "count before del");
            DelBlockBtn(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 0, "after delete block");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == countBlock - 1,
                "count after del");

            VerifyFinally(TestName);
        }
    }
}