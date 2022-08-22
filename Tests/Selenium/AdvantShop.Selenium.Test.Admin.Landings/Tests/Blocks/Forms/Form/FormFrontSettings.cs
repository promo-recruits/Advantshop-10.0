using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Forms.Form
{
    [TestFixture]
    internal class FormFrontSettings : FunctionsForms
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Forms\\Form\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSubBlock.csv"
            );

            Init();
        }

        private string blockName = "form";
        private string blockType = "Forms";
        private readonly int numberBlock = 1;
        private readonly string blockNameClient = "lp-block-form";
        private readonly string blockTitle = "FormTitle";
        private readonly string blockSubTitle = "FormSubTitle";
        private readonly string blockCapture = "FormField";
        private readonly string lpPath = "lp/test1";
        private readonly string blockId = "#block_1";
        private readonly string btnSubmit = "FormBtn";

        //form-front-setting
        ////color-scheme, top-padding, bottom-padding, hide in mobile, hide in desctope, add title, add subtitle, show all pages
        ////background-image, refresh-background-image, convert-to-html, copy-block, refreshblock

        [Test]
        public void AddTitle()
        {
            TestName = "AddTitle";
            VerifyBegin(TestName);

            #region showTitle

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            ShowTitle();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Count == 1,
                "TitleFormBlock 1");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Count == 1,
                "TitleFormBlock 1");

            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Count == 1,
                "TitleFormBlock 1");

            #endregion

            #region hideTitle

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            HiddenTitle();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Count == 0,
                "TitleFormBlock 1");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Count == 0,
                "TitleFormBlock 1");

            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Count == 0,
                "TitleFormBlock 1");

            #endregion

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            ShowTitle();
            BlockSettingsSave();

            VerifyFinally(TestName);
        }

        [Test]
        public void AddSubtitle()
        {
            TestName = "AddSubtitle";
            VerifyBegin(TestName);

            #region showSubTitle

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            ShowSubTitle();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Count == 1,
                "TitleFormBlock 1");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Count == 1,
                "TitleFormBlock 1");

            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Count == 1,
                "TitleFormBlock 1");

            #endregion

            #region hideSubTitle

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            HiddenSubTitle();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Count == 0,
                "TitleFormBlock 1");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Count == 0,
                "TitleFormBlock 1");

            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Count == 0,
                "TitleFormBlock 1");

            #endregion

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            ShowSubTitle();
            BlockSettingsSave();

            VerifyFinally(TestName);
        }

        [Test]
        public void BackgroundImageUpdate()
        {
            TestName = "BackgroundImageUpdate";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            VerifyIsTrue(GetCssValue(blockId, "background-image").Contains("none"),
                "default background image");

            //from computer
            BlockSettingsBtn(numberBlock);
            VerifyIsTrue(GetElAttribute("[data-e2e=\"img_picture\"]", "src").Contains("nophoto_cover.png"),
                "no photo in background 1");
            OpenPopupUpdateImage();
            UpdateLoadImageDesktop("images2.jpg");
            Thread.Sleep(2000);
            BlockSettingsSave();
            VerifyIsTrue(GetElAttribute("block_1", "style", "Id").Contains("background-image: url(\"pictures/landing/"),
                "upload photo via PC");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(GetElAttribute("#block_1 div", "style").Contains("background-image:"),
                "1");

            ReInit();
            GoToClient("lp/test1?inplace=true");
            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            DelImageSave();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            VerifyIsTrue(GetElAttribute("[data-e2e=\"img_picture\"]", "src").Contains("nophoto_cover.png"),
                "no photo in background 2");
            BlockSettingsSave();
            Thread.Sleep(2000);

            //from URL
            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            UpdateImageByUrl(
                "https://s2.best-wallpaper.net/wallpaper/1280x800/1902/Warm-light-circles-bright_1280x800.jpg");
            BlockSettingsSave();
            VerifyIsTrue(GetElAttribute("block_1", "style", "Id").Contains("background-image: url"),
                "upload photo via URL");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(GetElAttribute("#block_1 div", "style").Contains("background-image:"),
                "2");

            ReInit();
            GoToClient("lp/test1?inplace=true");
            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            DelImageSave();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            VerifyIsTrue(GetElAttribute("[data-e2e=\"img_picture\"]", "src").Contains("nophoto_cover.png"),
                "no photo in background 3");
            BlockSettingsSave();
            Thread.Sleep(2000);

            //from gallery
            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            UpdateLoadImageGallery();
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyIsTrue(GetElAttribute("block_1", "style", "Id").Contains("background-image:"),
                "upload photo via Gallery");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(GetElAttribute("#block_1 div", "style").Contains("background-image:"),
                "3");

            //lazyload
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 div[data-qazy-background]")).Count == 1,
                "is on"); //is on

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");

            VerifyIsFalse(Driver.FindElements(By.CssSelector("#block_1 div[data-qazy-background]")).Count == 1,
                "is off"); //is off

            //remove img
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            Driver.FindElement(By.CssSelector("[data-e2e=\"deletePicture\"]")).Click();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("none",
                Driver.FindElement(By.CssSelector(blockId)).GetCssValue("background-image"),
                "back image delete"); //is off

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangeBackgroundBehaviour()
        {
            TestName = "ChangeBackgroundBehaviour";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            VerifyIsFalse(GetElAttribute("block_1", "style", "Id").Contains("fixed"),
                "background without fixed");
            VerifyIsFalse(GetElAttribute("block_1", "style", "Id").Contains("parallax"),
                "background without parallax");
            VerifyIsFalse(GetElAttribute("block_1", "style", "Id").Contains("box-shadow:"),
                "background without shadow");

            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            UpdateLoadImageDesktop("images.jpg");
            BlockSettingsSave();

            BlockSettingsBtn(numberBlock);
            BehaviorBackgroundImage("Фиксированный фон");
            BlockSettingsSave();
            VerifyIsTrue(GetElAttribute("block_1", "style", "Id").Contains("fixed"),
                "background with fixed");

            BlockSettingsBtn(numberBlock);
            BehaviorBackgroundImage("Параллакс");
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyIsTrue(GetElAttribute("block_1", "class", "Id").Contains("parallax"),
                "background with parallax");

            BlockSettingsBtn(numberBlock);
            BehaviorBackgroundImage("Не выбрано");
            BlockSettingsSave();
            VerifyIsFalse(GetElAttribute("block_1", "style", "Id").Contains("fixed"),
                "background without fixed again");
            VerifyIsFalse(GetElAttribute("block_1", "style", "Id").Contains("parallax"),
                "background without parallax again");

            BlockSettingsBtn(numberBlock);
            moveSlider(5);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyIsTrue(GetElAttribute("block_1", "style", "Id").Contains("box-shadow:"),
                "background with shadow");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            Driver.FindElement(By.CssSelector("[data-e2e=\"deletePicture\"]")).Click();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("none",
                Driver.FindElement(By.CssSelector(blockId)).GetCssValue("background-image"),
                "back image delete"); //is off

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangeColorScheme()
        {
            TestName = "ChangeColorScheme";
            VerifyBegin(TestName);
            ReInit();
            GoToClient(lpPath);

            #region lightScheme

            BlockSettingsBtn(numberBlock);
            Driver.FindElement(By.CssSelector("#tabFormFront span")).Click();
            VerifyAreEqual("Светлая",
                GetElAttribute("[data-e2e=\"ColorScheme\"] option[selected=\"selected\"]", "label"),
                "1-setting color scheme");
            BlockSettingsCancel();
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue(blockId, "background-color"),
                "1-background-color");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized div", "color"),
                "1-TitleFormBlock color");
            VerifyAreEqual("300",
                GetCssValue("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized div", "font-weight"),
                "1-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(96, 96, 96, 1)",
                GetCssValue("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized div", "color"),
                "1-SubTitleFormBlock color");
            VerifyAreEqual("300",
                GetCssValue("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized div", "font-weight"),
                "1-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "1-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "1-TitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "1-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "1-SubTitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "1-FormField agreement color");
            //VerifyAreEqual("300",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "1-FormField agreement font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "1-FormField button color");
            VerifyAreEqual("rgba(23, 121, 250, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "1-FormField button background-color");
            VerifyAreEqual("rgb(23, 121, 250)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "1-FormField button border-color");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "1-FormField button border-radius");
            VerifyAreEqual("1px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "1-FormField button border-width");
            VerifyAreEqual("400", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "1-FormField button font-weight");

            ReInitClient();
            GoToClient(lpPath);
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue(blockId + " ." + blockNameClient, "background-color"),
                "1d-background-color");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "color"),
                "1d-TitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "font-weight"),
                "1d-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(96, 96, 96, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "color"),
                "1d-SubTitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "font-weight"),
                "1d-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "1d-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "1d-TitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "1d-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "1d-SubTitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "1d-FormField agreement color");
            //VerifyAreEqual("300",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "1d-FormField agreement font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "1d-FormField button color");
            VerifyAreEqual("rgba(23, 121, 250, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "1d-FormField button background-color");
            VerifyAreEqual("rgb(23, 121, 250)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "1d-FormField button border-color");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "1d-FormField button border-radius");
            VerifyAreEqual("1px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "1d-FormField button border-width");
            VerifyAreEqual("400", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "1d-FormField button font-weight");

            GoToMobile(lpPath);
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue(blockId + " ." + blockNameClient, "background-color"),
                "1m-background-color");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "color"),
                "1m-TitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "font-weight"),
                "1m-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(96, 96, 96, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "color"),
                "1m-SubTitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "font-weight"),
                "1m-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "1m-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "1m-TitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "1m-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "1m-SubTitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "1m-FormField agreement color");
            //VerifyAreEqual("300",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "1m-FormField agreement font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "1m-FormField button color");
            VerifyAreEqual("rgba(23, 121, 250, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "1m-FormField button background-color");
            VerifyAreEqual("rgb(23, 121, 250)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "1m-FormField button border-color");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "1m-FormField button border-radius");
            VerifyAreEqual("1px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "1m-FormField button border-width");
            VerifyAreEqual("400", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "1m-FormField button font-weight");

            #endregion

            ReInit();
            GoToClient(lpPath);
            BlockSettingsBtn(numberBlock);
            Driver.FindElement(By.CssSelector("#tabFormFront span")).Click();
            SetColorScheme("Умеренная");
            BlockSettingsCancel();
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue(blockId, "background-color"),
                "background-color");

            #region MildScheme

            BlockSettingsBtn(numberBlock);
            Driver.FindElement(By.CssSelector("#tabFormFront span")).Click();
            SetColorScheme("Умеренная");
            BlockSettingsSave();
            VerifyAreEqual("rgba(248, 248, 248, 1)", GetCssValue(blockId, "background-color"),
                "2-background-color");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized div", "color"),
                "2-TitleFormBlock color");
            VerifyAreEqual("300",
                GetCssValue("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized div", "font-weight"),
                "2-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(96, 96, 96, 1)",
                GetCssValue("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized div", "color"),
                "2-SubTitleFormBlock color");
            VerifyAreEqual("300",
                GetCssValue("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized div", "font-weight"),
                "2-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "2-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "2-TitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "2-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "2-SubTitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "2-FormField agreement color");
            //VerifyAreEqual("300",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "2-FormField agreement font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "2-FormField button color");
            VerifyAreEqual("rgba(23, 121, 250, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "2-FormField button background-color");
            VerifyAreEqual("rgb(23, 121, 250)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "2-FormField button border-color");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "2-FormField button border-radius");
            VerifyAreEqual("1px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "2-FormField button border-width");
            VerifyAreEqual("400", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "2-FormField button font-weight");

            ReInitClient();
            GoToClient(lpPath);
            VerifyAreEqual("rgba(248, 248, 248, 1)", GetCssValue(blockId + " ." + blockNameClient, "background-color"),
                "2d-background-color");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "color"),
                "2d-TitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "font-weight"),
                "2d-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(96, 96, 96, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "color"),
                "2d-SubTitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "font-weight"),
                "2d-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "2d-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "2d-TitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "2d-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "2d-SubTitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "2d-FormField agreement color");
            //VerifyAreEqual("300",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "2d-FormField agreement font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "2d-FormField button color");
            VerifyAreEqual("rgba(23, 121, 250, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "2d-FormField button background-color");
            VerifyAreEqual("rgb(23, 121, 250)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "2d-FormField button border-color");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "2d-FormField button border-radius");
            VerifyAreEqual("1px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "2d-FormField button border-width");
            VerifyAreEqual("400", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "2d-FormField button font-weight");

            GoToMobile(lpPath);
            VerifyAreEqual("rgba(248, 248, 248, 1)", GetCssValue(blockId + " ." + blockNameClient, "background-color"),
                "2m-background-color");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "color"),
                "2m-TitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "font-weight"),
                "2m-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(96, 96, 96, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "color"),
                "2m-SubTitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "font-weight"),
                "2m-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "2m-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "2m-TitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "2m-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "2m-SubTitleForm font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "2m-FormField agreement color");
            //VerifyAreEqual("300",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "2m-FormField agreement font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "2m-FormField button color");
            VerifyAreEqual("rgba(23, 121, 250, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "2m-FormField button background-color");
            VerifyAreEqual("rgb(23, 121, 250)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "2m-FormField button border-color");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "2m-FormField button border-radius");
            VerifyAreEqual("1px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "2m-FormField button border-width");
            VerifyAreEqual("400", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "2m-FormField button font-weight");

            #endregion

            #region DarkScheme

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            Driver.FindElement(By.CssSelector("#tabFormFront span")).Click();
            SetColorScheme("Темная");
            BlockSettingsSave();
            VerifyAreEqual("rgba(0, 0, 0, 1)", GetCssValue(blockId, "background-color"),
                "3-background-color");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized div", "color"),
                "3-TitleFormBlock color");
            VerifyAreEqual("300",
                GetCssValue("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized div", "font-weight"),
                "3-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(232, 232, 232, 1)",
                GetCssValue("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized div", "color"),
                "3-SubTitleFormBlock color");
            VerifyAreEqual("300",
                GetCssValue("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized div", "font-weight"),
                "3-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "3-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "3-TitleForm font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "3-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "3-SubTitleForm font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "3-FormField agreement color");
            //VerifyAreEqual("300",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "3-FormField agreement font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "3-FormField button color");
            VerifyAreEqual("rgba(23, 121, 250, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "3-FormField button background-color");
            VerifyAreEqual("rgb(23, 121, 250)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "3-FormField button border-color");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "3-FormField button border-radius");
            VerifyAreEqual("1px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "3-FormField button border-width");
            VerifyAreEqual("400", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "3-FormField button font-weight");

            ReInitClient();
            GoToClient(lpPath);
            VerifyAreEqual("rgba(0, 0, 0, 1)", GetCssValue(blockId + " ." + blockNameClient, "background-color"),
                "3d-background-color");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "color"),
                "3d-TitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "font-weight"),
                "3d-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(232, 232, 232, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "color"),
                "3d-SubTitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "font-weight"),
                "3d-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "3d-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "3d-TitleForm font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "3d-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "3d-SubTitleForm font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "3d-FormField agreement color");
            //VerifyAreEqual("300",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "3d-FormField agreement font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "3d-FormField button color");
            VerifyAreEqual("rgba(23, 121, 250, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "3d-FormField button background-color");
            VerifyAreEqual("rgb(23, 121, 250)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "3d-FormField button border-color");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "3d-FormField button border-radius");
            VerifyAreEqual("1px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "3d-FormField button border-width");
            VerifyAreEqual("400", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "3d-FormField button font-weight");

            GoToMobile(lpPath);
            VerifyAreEqual("rgba(0, 0, 0, 1)", GetCssValue(blockId + " ." + blockNameClient, "background-color"),
                "3m-background-color");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "color"),
                "3m-TitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "font-weight"),
                "3m-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(232, 232, 232, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "color"),
                "3m-SubTitleFormBlock color");
            VerifyAreEqual("300", GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "font-weight"),
                "3m-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "3m-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "3m-TitleForm font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "3m-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "3m-SubTitleForm font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "3m-FormField agreement color");
            //VerifyAreEqual("300",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "3m-FormField agreement font-weight");
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "3m-FormField button color");
            VerifyAreEqual("rgba(23, 121, 250, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "3m-FormField button background-color");
            VerifyAreEqual("rgb(23, 121, 250)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "3m-FormField button border-color");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "3m-FormField button border-radius");
            VerifyAreEqual("1px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "3m-FormField button border-width");
            VerifyAreEqual("400", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "3m-FormField button font-weight");

            #endregion

            #region UserScheme

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            Driver.FindElement(By.CssSelector("#tabFormFront span")).Click();
            SetColorScheme("Пользовательская");
            EditUserScheme();
            Thread.Sleep(2000);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ButtonBackgroundColor\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"buttonBorderRadius\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"buttonBorderRadius\"]")).SendKeys("0");
            Driver.FindElement(By.CssSelector("[data-e2e=\"buttonBorderWidth\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"buttonBorderWidth\"]")).SendKeys("4");
            SelectOption("700", "[data-e2e=\"ButtonTextBold\"]");
            SetColor("ButtonBorderColor", "rgb(255, 21, 0)");
            SetColor("ButtonTextColor", "rgb(0, 0, 0)");
            SetColor("ButtonBackgroundColor", "rgb(255, 255, 255)");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"TitleColor\"]"));
            SelectOption("400",
                "[data-e2e=\"TextBold\"]"); //нет смысла, все равно нет текстов. Но пригодится в нек.формах
            SetColor("TextColor", "rgb(3, 3, 3)");
            SelectOption("600", "[data-e2e=\"SubTitleBold\"]");
            SetColor("SubTitleColor", "rgb(255, 139, 139)");
            SelectOption("700", "[data-e2e=\"TitleBold\"]");
            SetColor("TitleColor", "rgb(255, 21, 0)");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"isShowCustomColors\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"isShowCustomColors\"]")).Click();
            Thread.Sleep(500);
            SetColor("customColor", "rgb(89, 33, 199)");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveCustomColorScheme\"]")).Click();

            VerifyAreEqual("rgba(89, 33, 199, 1)", GetCssValue(blockId, "background-color"),
                "4-background-color");
            VerifyAreEqual("rgba(255, 21, 0, 1)",
                GetCssValue("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized div", "color"),
                "4-TitleFormBlock color");
            VerifyAreEqual("700",
                GetCssValue("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized div", "font-weight"),
                "4-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(255, 139, 139, 1)",
                GetCssValue("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized div", "color"),
                "4-SubTitleFormBlock color");
            VerifyAreEqual("600",
                GetCssValue("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized div", "font-weight"),
                "4-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(3, 3, 3, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "4-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "4-TitleForm font-weight");
            VerifyAreEqual("rgba(3, 3, 3, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "4-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "4-SubTitleForm font-weight");
            VerifyAreEqual("rgba(3, 3, 3, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "4-FormField agreement color");
            //VerifyAreEqual("400",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "4-FormField agreement font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "4-FormField button color");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "4-FormField button background-color");
            VerifyAreEqual("rgb(255, 21, 0)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "4-FormField button border-color");
            VerifyAreEqual("0px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "4-FormField button border-radius");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "4-FormField button border-width");
            VerifyAreEqual("700", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "4-FormField button font-weight");

            ReInitClient();
            GoToClient(lpPath);
            VerifyAreEqual("rgba(89, 33, 199, 1)", GetCssValue(blockId + " ." + blockNameClient, "background-color"),
                "4d-background-color");
            VerifyAreEqual("rgba(255, 21, 0, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "color"),
                "4d-TitleFormBlock color");
            VerifyAreEqual("700", GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "font-weight"),
                "4d-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(255, 139, 139, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "color"),
                "4d-SubTitleFormBlock color");
            VerifyAreEqual("600", GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "font-weight"),
                "4d-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(3, 3, 3, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "4d-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "4d-TitleForm font-weight");
            VerifyAreEqual("rgba(3, 3, 3, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "4d-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "4d-SubTitleForm font-weight");
            VerifyAreEqual("rgba(3, 3, 3, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "4d-FormField agreement color");
            //VerifyAreEqual("400",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "4d-FormField agreement font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "4d-FormField button color");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "4d-FormField button background-color");
            VerifyAreEqual("rgb(255, 21, 0)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "4d-FormField button border-color");
            VerifyAreEqual("0px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "4d-FormField button border-radius");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "4d-FormField button border-width");
            VerifyAreEqual("700", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "4d-FormField button font-weight");

            GoToMobile(lpPath);
            VerifyAreEqual("rgba(89, 33, 199, 1)", GetCssValue(blockId + " ." + blockNameClient, "background-color"),
                "4m-background-color");
            VerifyAreEqual("rgba(255, 21, 0, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "color"),
                "4m-TitleFormBlock color");
            VerifyAreEqual("700", GetCssValue(blockId + " [data-e2e=\"" + blockTitle + "Block\"]", "font-weight"),
                "4m-TitleFormBlock font-weight");
            VerifyAreEqual("rgba(255, 139, 139, 1)",
                GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "color"),
                "4m-SubTitleFormBlock color");
            VerifyAreEqual("600", GetCssValue(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]", "font-weight"),
                "4m-SubTitleFormBlock font-weight");
            VerifyAreEqual("rgba(3, 3, 3, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "color"),
                "4m-TitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__title", "font-weight"),
                "4m-TitleForm font-weight");
            VerifyAreEqual("rgba(3, 3, 3, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "color"),
                "4m-SubTitleForm color");
            VerifyAreEqual("300", GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle ", "font-weight"),
                "4m-SubTitleForm font-weight");
            VerifyAreEqual("rgba(3, 3, 3, 1)",
                GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "color"),
                "4m-FormField agreement color");
            //VerifyAreEqual("400",GetCssValue("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement span", "font-weight"),
            //    "4m-FormField agreement font-weight");
            VerifyAreEqual("rgba(0, 0, 0, 1)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "color"),
                "4m-FormField button color");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "background-color"),
                "4m-FormField button background-color");
            VerifyAreEqual("rgb(255, 21, 0)", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-color"),
                "4m-FormField button border-color");
            VerifyAreEqual("0px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-radius"),
                "4m-FormField button border-radius");
            VerifyAreEqual("4px", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "border-width"),
                "4m-FormField button border-width");
            VerifyAreEqual("700", GetCssValue("[data-e2e=\"" + btnSubmit + "\"]", "font-weight"),
                "4m-FormField button font-weight");

            #endregion

            ReInit();
            GoToClient(lpPath);
            Thread.Sleep(1000);
            BlockSettingsBtn(numberBlock);
            Driver.FindElement(By.CssSelector("#tabFormFront span")).Click();
            SetColorScheme("Светлая");
            BlockSettingsSave();

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangePadding()
        {
            TestName = "ChangePadding";
            VerifyBegin(TestName);

            #region PaddingTop

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            Driver.FindElement(By.CssSelector("#tabFormFront span")).Click();
            VerifyAreEqual("85px",
                Driver.FindElement(
                    By.CssSelector("[data-e2e=\"padding_top\"] .ngrs-value-runner .ngrs-value-max .ng-binding")).Text,
                "setting padding-top");
            BlockSettingsCancel();
            VerifyIsTrue(GetElAttribute(blockId, "class").Contains("block-padding-top--85"),
                "1-padding-top-class");
            VerifyAreEqual("85px", GetCssValue(blockId, "padding-top"),
                "1-padding-top");

            BlockSettingsBtn(numberBlock);
            moveSliderPaddingTop(0);
            BlockSettingsSave();
            VerifyIsTrue(GetElAttribute(blockId, "class").Contains("block-padding-top--0"),
                "2.1-padding-top-class");
            VerifyAreEqual("0px", GetCssValue(blockId, "padding-top"),
                "2.1-padding-top");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(GetElAttribute(blockId + " .lp-block-form", "class").Contains("block-padding-top--0"),
                "2.2-padding-top-class");
            VerifyAreEqual("0px", GetCssValue(blockId + " .lp-block-form", "padding-top"),
                "2.2-padding-top");

            GoToMobile(lpPath);
            VerifyIsTrue(GetElAttribute(blockId + " .lp-block-form", "class").Contains("block-padding-top--0"),
                "2.3-padding-top-class");
            VerifyAreEqual("0px", GetCssValue(blockId + " .lp-block-form", "padding-top"),
                "2.3-padding-top");

            ReInit();
            GoToClient(lpPath);
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingTop(140);
            BlockSettingsSave();
            VerifyIsTrue(GetElAttribute(blockId, "class").Contains("block-padding-top--140"),
                "3.1-padding-top-class");
            VerifyAreEqual("140px", GetCssValue(blockId, "padding-top"),
                "3.1-padding-top");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(GetElAttribute(blockId + " .lp-block-form", "class").Contains("block-padding-top--140"),
                "3.2-padding-top-class");
            VerifyAreEqual("140px", GetCssValue(blockId + " .lp-block-form", "padding-top"),
                "3.2-padding-top");

            GoToMobile(lpPath);
            VerifyIsTrue(GetElAttribute(blockId + " .lp-block-form", "class").Contains("block-padding-top--140"),
                "3.3-padding-top-class");
            VerifyAreEqual("70px", GetCssValue(blockId + " .lp-block-form", "padding-top"),
                "3.3-padding-top");

            #endregion

            #region PaddingBottom

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            Driver.FindElement(By.CssSelector("#tabFormFront span")).Click();
            VerifyAreEqual("85px",
                Driver.FindElement(
                        By.CssSelector("[data-e2e=\"padding_bottom\"] .ngrs-value-runner .ngrs-value-max .ng-binding"))
                    .Text,
                "setting padding-bottom");
            BlockSettingsCancel();
            VerifyIsTrue(GetElAttribute(blockId, "class").IndexOf("block-padding-bottom--85") != -1,
                "1-padding-bottom-class");
            VerifyAreEqual("85px", GetCssValue(blockId, "padding-bottom"),
                "padding-bottom");

            BlockSettingsBtn(numberBlock);
            moveSliderPaddingBottom(0);
            BlockSettingsSave();
            VerifyIsTrue(GetElAttribute(blockId, "class").IndexOf("block-padding-bottom--0") != -1,
                "2.1-padding-bottom-class");
            VerifyAreEqual("0px", GetCssValue(blockId, "padding-bottom"),
                "2.1-padding-bottom");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(GetElAttribute(blockId + " .lp-block-form", "class").IndexOf("block-padding-bottom--0") != -1,
                "2.2-padding-bottom-class");
            VerifyAreEqual("0px", GetCssValue(blockId + " .lp-block-form", "padding-bottom"),
                "2.2-padding-bottom");

            GoToMobile(lpPath);
            VerifyIsTrue(GetElAttribute(blockId + " .lp-block-form", "class").IndexOf("block-padding-bottom--0") != -1,
                "2.3-padding-bottom-class");
            VerifyAreEqual("0px", GetCssValue(blockId + " .lp-block-form", "padding-bottom"),
                "2.3-padding-bottom");

            ReInit();
            GoToClient(lpPath);
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingBottom(140);
            BlockSettingsSave();
            VerifyIsTrue(GetElAttribute(blockId, "class").IndexOf("block-padding-bottom--140") != -1,
                "3.1-padding-bottom-class");
            VerifyAreEqual("140px", GetCssValue(blockId, "padding-bottom"),
                "3.1-padding-bottom");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(
                GetElAttribute(blockId + " .lp-block-form", "class").IndexOf("block-padding-bottom--140") != -1,
                "3.2-padding-bottom-class");
            VerifyAreEqual("140px", GetCssValue(blockId + " .lp-block-form", "padding-bottom"),
                "3.2-padding-bottom");

            GoToMobile(lpPath);
            VerifyIsTrue(
                GetElAttribute(blockId + " .lp-block-form", "class").IndexOf("block-padding-bottom--140") != -1,
                "3.3-padding-bottom-class");
            VerifyAreEqual("70px", GetCssValue(blockId + " .lp-block-form", "padding-bottom"),
                "3.3-padding-bottom");

            #endregion

            VerifyFinally(TestName);
        }

        [Test]
        public void CopyFormBlock()
        {
            TestName = "CopyBlock";
            VerifyBegin(TestName);
            ReInit();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(blockId)).Count == 1,
                "block-id in editor count before");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 1,
                "block in editor count before");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 1,
                "block in desctop count before");
            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 1,
                "block in mobile count before");

            ReInit();
            GoToClient(lpPath);
            Thread.Sleep(1000);
            BlockSettingsBtn();
            SetColorScheme("Темная");
            BlockSettingsSave();

            BlockSettingsBtn();
            CopyBlock();
            Thread.Sleep(2000);

            var blockCopyId = "#block_3";

            #region inplaceEditor

            VerifyIsTrue(Driver.FindElements(By.CssSelector(blockCopyId)).Count == 1,
                "block-copy in editor count after");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 2,
                "block in editor count after");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockTitle +
                                                  "Block\"] .inplace-initialized div")).Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockTitle +
                                                  "Block\"] .inplace-initialized div")).Text,
                "1-blockTitle");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockSubTitle +
                                                  "Block\"] .inplace-initialized div")).Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockSubTitle +
                                                  "Block\"] .inplace-initialized div")).Text,
                "1-blockSubTitle");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__title"))
                    .Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__title"))
                    .Text,
                "1-FormTitle");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle"))
                    .Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__subtitle")).Text,
                "1-FormSubTitle");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture +
                                                   "\"] .lp-form__field input")).Count ==
                Driver.FindElements(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture +
                                                   "\"] .lp-form__field input")).Count,
                "1-FormField field count");
            VerifyAreEqual(
                GetElAttribute(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                GetElAttribute(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input",
                    "placeholder"),
                "1-FormField field name");
            VerifyAreEqual(
                GetElAttribute(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                GetElAttribute(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input",
                    "placeholder", "CssSelector", 1),
                "1-FormField field phone");
            VerifyAreEqual(
                GetElAttribute(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                GetElAttribute(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input",
                    "placeholder", "CssSelector", 2),
                "1-FormField field email");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span")).Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span")).Text,
                "1-FormField agreement text");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("background-color"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("background-color"),
                "1-background-color");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("padding-top"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("padding-top"),
                "1-padding-top");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("padding-bottom"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("padding-bottom"),
                "1-padding-bottom");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("background-image"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("background-image"),
                "1-background-image");

            #endregion

            #region desctop

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(blockCopyId)).Count == 1,
                "1d-block-copy in editor count after");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 2,
                "1d-block in editor count after");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockTitle + "Block\"]")).Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockTitle + "Block\"]")).Text,
                "1d-blockTitle");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]")).Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockSubTitle + "Block\"]")).Text,
                "1d-blockSubTitle");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__title"))
                    .Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__title"))
                    .Text,
                "1d-FormTitle");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle"))
                    .Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__subtitle")).Text,
                "1d-FormSubTitle");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture +
                                                   "\"] .lp-form__field input")).Count ==
                Driver.FindElements(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture +
                                                   "\"] .lp-form__field input")).Count,
                "1d-FormField field count");
            VerifyAreEqual(
                GetElAttribute(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                GetElAttribute(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input",
                    "placeholder"),
                "1d-FormField field name");
            VerifyAreEqual(
                GetElAttribute(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                GetElAttribute(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input",
                    "placeholder", "CssSelector", 1),
                "1d-FormField field phone");
            VerifyAreEqual(
                GetElAttribute(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                GetElAttribute(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input",
                    "placeholder", "CssSelector", 2),
                "1d-FormField field email");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span")).Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span")).Text,
                "1d-FormField agreement text");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("background-color"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("background-color"),
                "1d-background-color");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("padding-top"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("padding-top"),
                "1d-padding-top");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("padding-bottom"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("padding-bottom"),
                "1d-padding-bottom");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("background-image"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("background-image"),
                "1d-background-image");

            #endregion

            #region mobile

            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(blockCopyId)).Count == 1,
                "1m-block-copy in editor count after");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 2,
                "1m-block in editor count after");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockTitle + "Block\"]")).Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockTitle + "Block\"]")).Text,
                "1m-blockTitle");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockSubTitle + "Block\"]")).Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockSubTitle + "Block\"]")).Text,
                "1m-blockSubTitle");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__title"))
                    .Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__title"))
                    .Text,
                "1m-FormTitle");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__subtitle"))
                    .Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__subtitle")).Text,
                "1m-FormSubTitle");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture +
                                                   "\"] .lp-form__field input")).Count ==
                Driver.FindElements(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture +
                                                   "\"] .lp-form__field input")).Count,
                "1m-FormField field count");
            VerifyAreEqual(
                GetElAttribute(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                GetElAttribute(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input",
                    "placeholder"),
                "1m-FormField field name");
            VerifyAreEqual(
                GetElAttribute(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                GetElAttribute(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input",
                    "placeholder", "CssSelector", 1),
                "1m-FormField field phone");
            VerifyAreEqual(
                GetElAttribute(blockId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                GetElAttribute(blockCopyId + " [data-e2e=\"" + blockCapture + "\"] .lp-form__field input",
                    "placeholder", "CssSelector", 2),
                "1m-FormField field email");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector(blockId + " [data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span")).Text,
                Driver.FindElement(By.CssSelector(blockCopyId + " [data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span")).Text,
                "1m-FormField agreement text");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("background-color"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("background-color"),
                "1m-background-color");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("padding-top"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("padding-top"),
                "1m-padding-top");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("padding-bottom"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("padding-bottom"),
                "1m-padding-bottom");
            VerifyAreEqual(Driver.FindElement(By.CssSelector(blockId)).GetCssValue("background-image"),
                Driver.FindElement(By.CssSelector(blockCopyId)).GetCssValue("background-image"),
                "1m-background-image");

            #endregion

            ReInit();
            GoToClient(lpPath);
            DelBlockBtn(3);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 1,
                "1-block in editor count after");


            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 1,
                "1d-block in editor count after");
            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("." + blockNameClient)).Count == 1,
                "1m-block in editor count after");

            VerifyFinally(TestName);
        }

        [Test]
        public void HideInDesctop()
        {
            TestName = "HideInDesctop";
            VerifyBegin(TestName);

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "displayed block in desctop initial");
            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "displayed block in mobile initial");

            ReInit();
            GoToClient(lpPath);
            BlockSettingsBtn();
            HiddenInDesktop();
            BlockSettingsSave();

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsFalse(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "hidden block in desctop initial");
            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "hidden block in mobile initial");

            ReInit();
            GoToClient(lpPath);
            BlockSettingsBtn();
            ShowInDesktop();
            BlockSettingsSave();

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "2-displayed block in desctop initial");
            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "2-displayed block in mobile initial");

            VerifyFinally(TestName);
        }

        [Test]
        public void HideInMobile()
        {
            TestName = "HideInMobile";
            VerifyBegin(TestName);

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "displayed block in desctop initial");
            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "displayed block in mobile initial");

            ReInit();
            GoToClient(lpPath);
            BlockSettingsBtn();
            HiddenInMobile();
            BlockSettingsSave();

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "hidden block in desctop initial");
            GoToMobile(lpPath);
            VerifyIsFalse(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "hidden block in mobile initial");

            ReInit();
            GoToClient(lpPath);
            BlockSettingsBtn();
            ShowInMobile();
            BlockSettingsSave();

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "2-displayed block in desctop initial");
            GoToMobile(lpPath);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + " ." + blockNameClient)).Displayed,
                "2-displayed block in mobile initial");

            VerifyFinally(TestName);
        }

        [Test]
        public void ShowOnPages()
        {
            TestName = "ShowAllPages";
            VerifyBegin(TestName);

            ReInitClient();
            GoToClient(lpPath);

            VerifyIsTrue(Driver.PageSource.Contains("id=\"block_" + numberBlock + "\""),
                "page1 default");
            GoToClient(lpPath + "/page1");
            VerifyIsFalse(Driver.PageSource.Contains("id=\"block_" + numberBlock + "\""),
                "page2 default");

            #region AllPages

            ReInit();
            GoToClient(lpPath);
            BlockSettingsBtn();
            ShowOnAllPage();
            BlockSettingsSave();

            VerifyIsTrue(Driver.PageSource.Contains("id=\"block_" + numberBlock + "\""),
                "page1 all pages (1)");
            GoToClient(lpPath + "/page1");
            VerifyIsTrue(Driver.PageSource.Contains("id=\"block_" + numberBlock + "\""),
                "page2 all pages (1)");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.PageSource.Contains("id=\"block_" + numberBlock + "\""),
                "page1 all pages (2)");
            GoToClient(lpPath + "/page1");
            VerifyIsTrue(Driver.PageSource.Contains("id=\"block_" + numberBlock + "\""),
                "page2 all pages (2)");

            #endregion

            #region OnePage

            ReInit();
            GoToClient(lpPath);
            BlockSettingsBtn();
            ShowOnThisPage();
            BlockSettingsSave();

            VerifyIsTrue(Driver.PageSource.Contains("id=\"block_" + numberBlock + "\""),
                "page1 one page (1)");
            GoToClient(lpPath + "/page1");
            VerifyIsFalse(Driver.PageSource.Contains("id=\"block_" + numberBlock + "\""),
                "page2 one page (1)");

            ReInitClient();
            GoToClient(lpPath);
            VerifyIsTrue(Driver.PageSource.Contains("id=\"block_" + numberBlock + "\""),
                "page1 one page (2)");
            GoToClient(lpPath + "/page1");
            VerifyIsFalse(Driver.PageSource.Contains("id=\"block_" + numberBlock + "\""),
                "page2 one page (2)");

            #endregion

            VerifyFinally(TestName);
        }

        [Test]
        public void UpdateBlock()
        {
            TestName = "UpdateBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient(lpPath);

            BlockSettingsBtn(numberBlock);
            SetColorScheme("Темная");
            moveSliderPaddingTop(30);
            moveSliderPaddingBottom(30);

            BlockSettingsSave();
            Refresh();

            BlockSettingsBtn(numberBlock);
            UpdateBlockCancel();
            BlockSettingsSave();

            VerifyAreEqual("rgba(0, 0, 0, 1)", GetCssValue(blockId, "background-color"),
                "1-background-color");
            VerifyAreEqual("30px", GetCssValue(blockId, "padding-top"),
                "1-padding-top");
            VerifyAreEqual("30px", GetCssValue(blockId, "padding-bottom"),
                "1-padding-bottom");

            BlockSettingsBtn(numberBlock);
            UpdateBlockSave();

            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue(blockId, "background-color"),
                "2-background-color");
            VerifyAreEqual("85px", GetCssValue(blockId, "padding-top"),
                "2-padding-top");
            VerifyAreEqual("85px", GetCssValue(blockId, "padding-bottom"),
                "2-padding-bottom");

            ReInitClient();
            GoToClient(lpPath);
            VerifyAreEqual("rgba(255, 255, 255, 1)", GetCssValue(blockId + " ." + blockNameClient, "background-color"),
                "2d-background-color");
            VerifyAreEqual("85px", GetCssValue(blockId + " ." + blockNameClient, "padding-top"),
                "2d-padding-top");
            VerifyAreEqual("85px", GetCssValue(blockId + " ." + blockNameClient, "padding-bottom"),
                "2d-padding-bottom");

            VerifyFinally(TestName);
        }

        [Test]
        public void zConvertHTML()
        {
            TestName = "zConvertHTML";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            var convertedBlock = "block_4";
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId + "." + blockNameClient)).Displayed,
                "block before all");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-string-id=\"block_1\"] .hidden-xs")).Text
                    .Contains("form, ID блока"),
                "form type in constructor before all");
            BlockSettingsBtn(numberBlock);
            ConvertToHtmlCancel();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(blockId)).Displayed, "block before convert");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-string-id=\"block_1\"] .hidden-xs")).Text
                    .Contains("form, ID блока"),
                "form type in constructor before convert");

            BlockSettingsBtn(numberBlock);
            ConvertToHtmlSave();
            VerifyIsFalse(Driver.PageSource.Contains(blockId), "block after convert 1");
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 0, "block after convert 2");
            VerifyIsTrue(Driver.FindElement(By.Id(convertedBlock)).Displayed, "block after convert html");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-string-id=\"" + convertedBlock + "\"] .hidden-xs")).Text
                    .Contains("html, ID блока"),
                "form type in constructor after convert");

            VerifyFinally(TestName);
        }
    }
}