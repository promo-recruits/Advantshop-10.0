using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Images.gallery
{
    [TestFixture]
    public class galleryMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Images\\gallery\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\CMS.LandingSiteSettings.csv"
            );
            Init();
        }

        private readonly string blockName = "gallery";
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
            VerifyIsTrue(Driver.PageSource.Contains("gallery, ID блока: 1"), "name and id correct");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 0, "subblocks not displayed");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"galleryPicture\"]")).Count == 6,
                "all block`s pictures displayed");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-gallery")).Displayed,
                "displayed block in desktop");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"galleryTitle\"]")).Count == 0,
                "no title in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"gallerySubtitle\"]")).Count == 0,
                "no subtitle in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[0].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/01-img_preview.jpg"), "picture1 in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[1].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/02-img_preview.jpg"), "picture2 in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[2].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/03-img_preview.jpg"), "picture3 in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[3].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/04-img_preview.jpg"), "picture4 in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[4].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/05-img_preview.jpg"), "picture5 in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[5].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/06-img_preview.jpg"), "picture6 in client");

            //mobile version
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-block-gallery")).Displayed,
                "displayed block in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"galleryTitle\"]")).Count == 0,
                "no title in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"gallerySubtitle\"]")).Count == 0,
                "no subtitle in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[0].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/01-img_preview.jpg"), "picture1 in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[1].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/02-img_preview.jpg"), "picture2 in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[2].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/03-img_preview.jpg"), "picture3 in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[3].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/04-img_preview.jpg"), "picture4 in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[4].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/05-img_preview.jpg"), "picture5 in mobile");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"galleryPicture\"] img"))[5].GetAttribute("src")
                    .Contains("areas/landing/images/gallery/06-img_preview.jpg"), "picture6 in mobile");

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