using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Images.imageFullWidth
{
    [TestFixture]
    public class imageFullWidthEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Images\\imageFullWidth\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "imageFullWidth";
        private string blockType = "Images";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "TitleBlock";
        private readonly string blockSubTitle = "SubTitleBlock";
        private readonly string blockPicture = "PictureBlock";

        [Test]
        public void InplacePicture()
        {
            TestName = "InplacePicture";
            VerifyBegin(TestName);
            var pathInit = "";
            var path = "";

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img[data-qazy]")).Count == 1,
                "lazy-load initial");

            ReInit();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1"))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains("/areas/landing/images/image/image_full_width.jpg"), "block picture initial");
            pathInit = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img"))
                .GetAttribute("src");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".picture-upload-modal")).Displayed, "modal is dysplayed");
            VerifyAreEqual("Обновить изображение",
                Driver.FindElement(By.CssSelector(".picture-upload-modal .modal-header")).Text, "modal is correct");

            //from PC
            UpdateLoadImageDesktop("light.jpg");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via PC");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img"))
                .GetAttribute("src"); //pathPC
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathPC");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via PC after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via PC in client");

            //from URL
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();

            UpdateImageByUrl(
                "https://s2.best-wallpaper.net/wallpaper/1280x800/1902/Warm-light-circles-bright_1280x800.jpg");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via URL");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src"),
                "pathPC vs pathURL");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img"))
                .GetAttribute("src"); //pathURL
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathURL");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via URL after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via URL in client");

            //from Gallery
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();

            UpdateLoadImageGallery();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src"),
                "pathURL vs pathG");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img"))
                .GetAttribute("src"); //pathG
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathG");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery in client");

            //lazy-load
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]"));
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(1000);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img[data-qazy]")).Count == 1,
                "lazy-load off");

            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]"));
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(1000);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img[data-qazy]")).Count == 1,
                "lazy-load on");


            VerifyFinally(TestName);
        }

        [Test]
        public void InplaceText()
        {
            TestName = "InplaceText";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            ShowTitle();
            ShowSubTitle();
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 3, "all subblocks displayed");
            VerifyAreEqual("title",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[0].GetAttribute("data-name"), "title subblock");
            VerifyAreEqual("subtitle",
                Driver.FindElements(By.CssSelector("subblock-inplace"))[1].GetAttribute("data-name"),
                "subtitle subblock");

            VerifyAreEqual("Вдохновляйся идеями",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title initial");
            VerifyAreEqual("Реализуй все с Advantshop",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle initial");

            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized"))
                .SendKeys("New Title");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_1"))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized"))
                .SendKeys("New Subtitle");
            Thread.Sleep(500);

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]")).Click();
            Thread.Sleep(1000);

            Refresh();

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title after refresh");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle after refresh");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]"))
                    .Text, "block title in client");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]"))
                    .Text, "block subtitle in client");

            VerifyFinally(TestName);
        }
    }
}