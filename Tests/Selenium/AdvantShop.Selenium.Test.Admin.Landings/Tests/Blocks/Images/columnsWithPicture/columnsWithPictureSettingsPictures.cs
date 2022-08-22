using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Images.columnsWithPicture
{
    [TestFixture]
    public class columnsWithPictureSettingsPictures : LandingsFunctions
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
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Images\\columnsWithPicture\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "columnsWithPicture";
        private string blockType = "Images";
        private readonly int numberBlock = 1;
        private readonly string blockPicture = "columnsWithPicturePicture";

        [Test]
        public void ChangePicture()
        {
            TestName = "ChangePicture";
            VerifyBegin(TestName);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img[data-qazy]")).Count == 1,
                "lazy-load initial");

            var pathInit = "";
            var path = "";

            ReInit();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1"))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img")).GetAttribute("src")
                    .Contains("/areas/landing/images/columns/columns_with_picture.jpg"), "block picture initial");
            pathInit = Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img"))
                .GetAttribute("src");

            //from computer
            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            Driver.FindElement(By.CssSelector("[data-e2e=\"UpdatelImg\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".adv-modal-inner.picture-upload-modal")).Displayed,
                "modal is dysplayed");
            VerifyAreEqual("Обновить изображение",
                Driver.FindElement(By.CssSelector(".adv-modal-inner.picture-upload-modal .modal-header")).Text,
                "modal is correct");
            UpdateLoadImageDesktop("dark.jpg");
            BlockSettingsSave();

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

            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            Driver.FindElement(By.CssSelector("[data-e2e=\"UpdatelImg\"]")).Click();
            Thread.Sleep(500);
            UpdateImageByUrl(
                "https://s2.best-wallpaper.net/wallpaper/1280x800/1902/Warm-light-circles-bright_1280x800.jpg");
            BlockSettingsSave();

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

            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            Driver.FindElement(By.CssSelector("[data-e2e=\"UpdatelImg\"]")).Click();
            Thread.Sleep(500);
            UpdateLoadImageGallery(3);
            BlockSettingsSave();

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

            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            Driver.FindElement(By.CssSelector("[data-e2e=\"UpdatelImg\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            Thread.Sleep(1000);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img[data-qazy]")).Count == 1,
                "lazy-load off");

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            Driver.FindElement(By.CssSelector("[data-e2e=\"UpdatelImg\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            Thread.Sleep(1000);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockPicture + "\"] img[data-qazy]")).Count == 1,
                "lazy-load on");

            VerifyFinally(TestName);
        }
    }
}