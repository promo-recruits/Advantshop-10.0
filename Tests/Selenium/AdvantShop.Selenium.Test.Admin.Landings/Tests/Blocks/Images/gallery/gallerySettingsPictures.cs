using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Images.gallery
{
    [TestFixture]
    public class gallerySettingsPictures : LandingsFunctions
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
                "data\\Admin\\LandingTest\\Images\\gallery\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Images\\gallery\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "gallery";
        private string blockType = "Images";
        private readonly int numberBlock = 1;
        private readonly string blockPicture = "galleryPicture";

        [Test]
        public void ChangePicturesAligment()
        {
            TestName = "ChangePicturesAligment";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row")).GetAttribute("class").Contains("center-xs"),
                "center aligment 1");

            //слева
            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            ImagesAligment("По левому краю");
            BlockSettingsSave();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row")).GetAttribute("class").Contains("start-xs"),
                "left aligment");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row")).GetAttribute("class").Contains("start-xs"),
                "left aligment in client");

            //справа
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            ImagesAligment("По правому краю");
            BlockSettingsSave();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row")).GetAttribute("class").Contains("end-xs"),
                "right aligment");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row")).GetAttribute("class").Contains("end-xs"),
                "right aligment in client");

            //по центру
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            ImagesAligment("По центру");
            BlockSettingsSave();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row")).GetAttribute("class").Contains("center-xs"),
                "center aligment 2");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row")).GetAttribute("class").Contains("center-xs"),
                "center aligment 2 in client");

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangePicturesBlock()
        {
            TestName = "ChangePicturesBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            ImageBlockHeight("300");
            ImageBlockCount("4");
            BlockSettingsSave();
            Thread.Sleep(1000);

            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            VerifyAreEqual("300",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ImageBlockHeight\"] input")).GetAttribute("value"),
                "image block height setting");
            VerifyAreEqual("4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ImageBlockCount\"] input")).GetAttribute("value"),
                "image block count");
            BlockSettingsClose();

            VerifyAreEqual("316px",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]")).GetCssValue("height"),
                "image block height"); // + bottom-padding 16px

            Refresh();

            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");
            VerifyAreEqual("300",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ImageBlockHeight\"] input")).GetAttribute("value"),
                "image block height setting after refresh");
            VerifyAreEqual("4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ImageBlockCount\"] input")).GetAttribute("value"),
                "image block count after refresh");
            BlockSettingsClose();

            VerifyAreEqual("316px",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockPicture + "\"]")).GetCssValue("height"),
                "image block height after refresh"); // + bottom-padding 16px

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangePicture()
        {
            TestName = "ChangePicture";
            VerifyBegin(TestName);

            //ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabPictures");


            VerifyFinally(TestName);
        }

        //РЕДАКТИРОВАНИЕ: через комп
        //через ссылку
        //из галереи
        //lazy-load
        //УДАЛЕНИЕ одного
        //удаление всех
        //ДОБАВЛЕНИЕ нового
        //drag`n`drop
    }
}