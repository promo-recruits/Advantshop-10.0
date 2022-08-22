using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Headers.HeaderCommunicationSimple
{
    [TestFixture]
    public class headerCommunicationSimpleSettingsLogo : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\Settings.Settings.csv"
            );
            Init();
        }

        private readonly string blockName = "headerCommunicationSimple";
        private readonly string blockType = "Headers";
        private readonly int numberBlock = 1;

        public void ChangeLogoAdmin()
        {
            GoToAdmin("settings/common");
            Thread.Sleep(2000);
            AttachFile(By.XPath("(//input[@type='file'])[2]"), GetPicturePath("img_logo.jpg"));
            AttachFile(By.XPath("(//input[@type='file'])[1]"), GetPicturePath("img_logo.jpg"));
            Thread.Sleep(1000);
        }

        public string LogoPath()
        {
            GoToAdmin("settings/common");
            var logo = Driver.FindElement(By.CssSelector("picture-uploader img")).GetAttribute("src");
            var words = logo.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            var count = words.Length;
            logo = words[count - 1];
            return logo;
        }

        [Test]
        public void LogoCheck()
        {
            TestName = "LogoCheck";
            VerifyBegin(TestName);

            ChangeLogoAdmin();
            var logo = LogoPath();

            GoToClient("lp/test1");

            AddBlockByBtnBig(blockType, blockName);
            VerifyAreEqual("Мой магазин", Driver.FindElement(By.CssSelector(".lp-header img")).GetAttribute("alt"),
                "logo alt");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderAddational");
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LogoGeneratorPreview\"]")).GetAttribute("src")
                    .Contains(logo), "src logo vs admin");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header a")).GetAttribute("href").Contains("/lp/test1/"),
                "href logo in client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header img")).GetAttribute("src").Contains(logo),
                "src logo vs admin in client");
            VerifyAreEqual("Мой магазин", Driver.FindElement(By.CssSelector(".lp-header img")).GetAttribute("alt"),
                "logo alt in client");

            VerifyFinally(TestName);
        }


        [Test]
        public void LogoUpdate()
        {
            TestName = "LogoUpdate";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            //AddBlockByBtnBig(blockType, blockName); //****
            //Thread.Sleep(2000); //****

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".lp-header img")).GetAttribute("src").Contains("logo_generated"),
                "initial logo");
            VerifyAreEqual("true",
                Driver.FindElement(By.CssSelector(".logo-generator-preview picture-loader-trigger"))
                    .GetAttribute("lazy-load-enabled"), "initial lazy-load");

            //delete
            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderAddational");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnLoadLogo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"deletePicture\"]")).Click();
            VerifyAreEqual("Изображение успешно удалено",
                Driver.FindElement(By.CssSelector(".ng-binding.toast-title")).Text, "green toast");
            Driver.FindElement(By.CssSelector(".adv-modal-inner.picture-upload-modal .adv-modal-close")).Click();
            Thread.Sleep(1000);
            BlockSettingsSave();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-header img")).GetAttribute("src")
                    .Contains("/areas/landing/frontend/images/nophoto.jpg"), "logo after delete");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-header img")).Count == 0,
                "logo after delete in client");

            //create
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderAddational");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnGenerateLogo\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Генерация логотипа", Driver.FindElements(By.CssSelector(".modal-header"))[1].Text,
                "modal generation is visible");
            Driver.FindElement(By.CssSelector(".input.input-small")).Clear();
            Driver.FindElement(By.CssSelector(".input.input-small")).SendKeys("Alt text");
            Driver.FindElement(By.CssSelector(".input-small.spinbox-input")).Clear();
            Driver.FindElement(By.CssSelector(".input-small.spinbox-input")).SendKeys("32");
            Driver.FindElement(By.CssSelector(".color-picker-input-wrapper")).Click();
            Driver.FindElement(By.CssSelector(".color-picker-wrapper input")).Clear();
            Driver.FindElement(By.CssSelector(".color-picker-wrapper input")).SendKeys("d18ef0");
            Driver.FindElement(By.CssSelector(".logo-generator-modal-btn-save")).Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-header img")).GetAttribute("src").Contains("logo_generated"),
                "generated logo");

            //upload
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderAddational");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnLoadLogo\"]")).Click();
            VerifyAreEqual("Обновить изображение", Driver.FindElements(By.CssSelector(".modal-header"))[1].Text,
                "modal update is visible");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"LoadPicture\"]")).Count == 0, "via PC");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"uploadByUrl\"]")).Count == 1, "via URL");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"GalleryCloud\"]")).Count == 1, "via Gallery");
            //logo lazy-load
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"]")).Click();
            Driver.FindElement(By.CssSelector(".adv-modal-inner.picture-upload-modal .adv-modal-close")).Click();
            Thread.Sleep(1000);
            BlockSettingsSave();

            Refresh();
            Thread.Sleep(1000);
            VerifyAreEqual("false",
                Driver.FindElement(By.CssSelector(".logo-generator-preview picture-loader-trigger"))
                    .GetAttribute("lazy-load-enabled"), "lazy-load is off");

            VerifyFinally(TestName);
        }

        [Test]
        public void LogoVisible()
        {
            TestName = "LogoVisible";
            VerifyBegin(TestName);

            //hide
            ReInit();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo")).Displayed, "initial logo displayed");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderAddational");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowLogo\"]")).Click();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-header-logo")).Count == 0, "logo not displayed");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-header-logo")).Count == 0,
                "logo not displayed in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-header-logo")).Count == 0,
                "logo not displayed in mobile");

            //show
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderAddational");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowLogo\"]")).Click();
            BlockSettingsSave();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".lp-header-logo")).Count == 0, "logo displayed");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".lp-header-logo")).Count == 0,
                "logo displayed in client");

            GoToMobile("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".lp-header-logo")).Count == 0,
                "logo displayed in mobile");

            VerifyFinally(TestName);
        }
    }
}