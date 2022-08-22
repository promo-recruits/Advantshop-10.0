using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Images.columnsWithPicture
{
    [TestFixture]
    public class columnsWithPictureMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingSiteSettings.csv"
            );
            Init();
        }

        private readonly string blockName = "columnsWithPicture";
        private readonly string blockType = "Images";
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
            VerifyIsTrue(Driver.PageSource.Contains("columnsWithPicture, ID блока: 1"), "name and id correct");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 11, "subblocks displayed");
            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("subtitle",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "subtitle subblock");
            VerifyAreEqual("header1",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[2].GetAttribute("data-name"),
                "header1 subblock");
            VerifyAreEqual("text1",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[3].GetAttribute("data-name"), "text1 subblock");
            VerifyAreEqual("header2",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[4].GetAttribute("data-name"),
                "header2 subblock");
            VerifyAreEqual("text2",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[5].GetAttribute("data-name"), "text2 subblock");
            VerifyAreEqual("picture",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[6].GetAttribute("data-name"),
                "picture subblock");
            VerifyAreEqual("header3",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[7].GetAttribute("data-name"),
                "header3 subblock");
            VerifyAreEqual("text3",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[8].GetAttribute("data-name"), "text3 subblock");
            VerifyAreEqual("header4",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[9].GetAttribute("data-name"),
                "header4 subblock");
            VerifyAreEqual("text4",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[10].GetAttribute("data-name"),
                "text4 subblock");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-columns-with-picture")).Displayed,
                "displayed block in desktop");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"TitleBlock\"]")).Displayed,
                "displayed title in client");
            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"TitleBlock\"]")).Text, "title in client");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"SubTitleBlock\"]")).Displayed,
                "displayed subtitle in client");
            VerifyAreEqual("Реализуй все с Advantshop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"SubTitleBlock\"]")).Text, "subtitle in client");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"columnsWithPicturePicture\"]")).Displayed,
                "displayed picture in client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"columnsWithPicturePicture\"] img"))
                    .GetAttribute("src").Contains("/areas/landing/images/columns/columns_with_picture.jpg"),
                "picture in client");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[0].Displayed,
                "displayed header1 in client");
            VerifyAreEqual("Трансформер дизайна",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[0].Text,
                "header1 in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[0].Displayed,
                "displayed text1 in client");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (расширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[0].Text,
                "text1 in client");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[1].Displayed,
                "displayed header2 in client");
            VerifyAreEqual("CRM для интернет-магазина",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[1].Text,
                "header2 in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[1].Displayed,
                "displayed text2 in client");
            VerifyAreEqual("Имманентный подход подразумевает отношение к тексту как к автономной реальности",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[1].Text,
                "text2 in client");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[2].Displayed,
                "displayed header3 in client");
            VerifyAreEqual("Регистрация SSL",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[2].Text,
                "header3 in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[2].Displayed,
                "displayed text3 in client");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (рас-ширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[2].Text,
                "text3 in client");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[3].Displayed,
                "displayed header4 in client");
            VerifyAreEqual("Регистрация SSL",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[3].Text,
                "header4 in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[3].Displayed,
                "displayed text4 in client");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (рас-ширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[3].Text,
                "text4 in client");

            //mobile version
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-columns-with-picture")).Displayed,
                "displayed block in mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"TitleBlock\"]")).Displayed,
                "displayed title in mobile");
            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"TitleBlock\"]")).Text, "title in mobile");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"SubTitleBlock\"]")).Displayed,
                "displayed subtitle in mobile");
            VerifyAreEqual("Реализуй все с Advantshop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"SubTitleBlock\"]")).Text, "subtitle in mobile");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"columnsWithPicturePicture\"]")).Displayed,
                "displayed picture in mobile");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"columnsWithPicturePicture\"] img"))
                    .GetAttribute("src").Contains("/areas/landing/images/columns/columns_with_picture.jpg"),
                "picture in mobile");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[0].Displayed,
                "displayed header1 in mobile");
            VerifyAreEqual("Трансформер дизайна",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[0].Text,
                "header1 in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[0].Displayed,
                "displayed text1 in mobile");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (расширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[0].Text,
                "text1 in mobile");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[1].Displayed,
                "displayed header2 in mobile");
            VerifyAreEqual("CRM для интернет-магазина",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[1].Text,
                "header2 in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[1].Displayed,
                "displayed text2 in mobile");
            VerifyAreEqual("Имманентный подход подразумевает отношение к тексту как к автономной реальности",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[1].Text,
                "text2 in mobile");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[2].Displayed,
                "displayed header3 in mobile");
            VerifyAreEqual("Регистрация SSL",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[2].Text,
                "header3 in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[2].Displayed,
                "displayed text3 in mobile");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (рас-ширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[2].Text,
                "text3 in mobile");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[3].Displayed,
                "displayed header4 in mobile");
            VerifyAreEqual("Регистрация SSL",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureHeader\"]"))[3].Text,
                "header4 in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[3].Displayed,
                "displayed text4 in mobile");
            VerifyAreEqual(
                "Существуют две основные трактовки понятия «текст»: «имманентная» (рас-ширенная, философски нагруженная)",
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"columnsWithPictureText\"]"))[3].Text,
                "text4 in mobile");

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