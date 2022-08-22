using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Forms.Form
{
    [TestFixture]
    internal class FormMain : FunctionsForms
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
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.CustomerField.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSiteSettings.csv"
            );

            Init();
        }

        private readonly string blockName = "form";
        private readonly string blockType = "Forms";
        private readonly int numberBlock = 1;
        private readonly string blockNameClient = "lp-block-form";
        private readonly string blockTitle = "FormTitle";
        private readonly string blockSubTitle = "FormSubTitle";
        private readonly string blockCapture = "FormField";
        private readonly string lpPath = "lp/test1";

        [Test]
        public void AddBlock()
        {
            //по дефолту добавилась форма с заголовком, подзаголовком, блоком-формой, тремя полями, кнопкой и чекбоксом.
            //потом сравнить текст можна
            TestName = "AddBlock";
            VerifyBegin(TestName);
            GoToClient(lpPath);
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 1,
                "no blocks on page");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor")).Count == 1,
                "no blocks-constructor on page");
            VerifyIsTrue(Driver.FindElements(By.ClassName(blockNameClient)).Count == 0,
                "no blocks blocknameclient");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AddBlockBtnSml\"]")).Count == 0,
                "add block btn small");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AddBlockBtnBig\"]")).Count == 1,
                "add block btn big");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockName + "_container\"]")).Count == 0,
                "find form-block");

            AddBlockByBtnBig(blockType, blockName);

            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor-container")).Count == 2,
                "not added block");
            VerifyIsTrue(Driver.FindElements(By.TagName("blocks-constructor")).Count == 2,
                "not added block-constructor");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AddBlockBtnSml\"]")).Count == 1,
                "not added add block btn small");
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed,
                "display added block");
            VerifyIsTrue(Driver.PageSource.Contains(blockName + ", ID блока: " + numberBlock),
                "name and id correct");
            VerifyIsTrue(Driver.FindElements(By.ClassName(blockNameClient)).Count == 1,
                "block blocknameclient");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockName + "_container\"]")).Count == 1,
                "FormContainer 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"]")).Count == 1,
                "FormField 1");
            //содержимое формы
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input"))
                    .Count == 3,
                "FormField field count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement")).Count ==
                1,
                "FormField agreement count");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] " +
                                                            ".lp-form__submit-block button.lp-btn--primary")).Count ==
                         1,
                "FormField button count");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("subblock-inplace")).Count == 2,
                "subblocks displayed");
            VerifyAreEqual("title", GetElAttribute("subblock-inplace", "data-name"),
                "title subblock");
            VerifyAreEqual("subtitle", GetElAttribute("subblock-inplace", "data-name", "CssSelector", 1),
                "subtitle subblock");

            ReInitClient();
            //display in desctop
            GoToClient(lpPath);
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[id=\"block_" + numberBlock + "\"]")).Count == 1,
                "(d)block id=" + numberBlock);
            VerifyIsTrue(Driver.FindElements(By.ClassName(blockNameClient)).Count == 1,
                "(d)block blocknameclient");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockName + "_container\"]")).Count == 1,
                "(d)FormContainer");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"]")).Count == 1,
                "(d)FormField");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input"))
                    .Count == 3,
                "(d)FormField field count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement")).Count ==
                1,
                "(d)FormField agreement count");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] " +
                                                            ".lp-form__submit-block button.lp-btn--primary")).Count ==
                         1,
                "(d)FormField button count");


            //display in mobile
            GoToMobile(lpPath);
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[id=\"block_" + numberBlock + "\"]")).Count == 1,
                "(m)block id=" + numberBlock);
            VerifyIsTrue(Driver.FindElements(By.ClassName(blockNameClient)).Count == 1,
                "(m)block blocknameclient");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockName + "_container\"]")).Count == 1,
                "(m)FormContainer");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"]")).Count == 1,
                "(m)FormField");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input"))
                    .Count == 3,
                "(m)FormField field count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement")).Count ==
                1,
                "(m)FormField agreement count");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] " +
                                                            ".lp-form__submit-block button.lp-btn--primary")).Count ==
                         1,
                "(m)FormField button count");

            VerifyFinally(TestName);
        }

        [Test]
        public void DefaultSettingsBlock()
        {
            TestName = "DefaultSettingsBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient(lpPath);
            Thread.Sleep(1000);
            ////УДАЛИТЬ
            //AddBlockByBtnBig(blockType, blockName);

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Count == 1,
                "TitleFormBlock 1");
            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized div"))
                    .Text,
                "TitleFormBlock text 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Count == 1,
                "SubTitleFormBlock 1");
            VerifyAreEqual("Более 5000 функций для удобных и эффективных продаж",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized div"))
                    .Text,
                "SubTitleFormBlock text 1");

            VerifyAreEqual("Имя",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                "FormField field name");
            VerifyAreEqual("Телефон",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                "FormField field phone");
            VerifyAreEqual("Email",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                "FormField field email");
            VerifyAreEqual("Я согласен на обработку персональных данных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span:not(.lp-checkbox-decor)")).Text,
                "FormField agreement text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] " +
                                                           "[data-e2e=\"FormBtn\"] span.ladda-label")).Text
                    .Contains("Отправить"),
                "FormField button text");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock)).GetCssValue("background-color"),
                "background-color");
            VerifyIsTrue(GetElAttribute("#block_" + numberBlock, "class").Contains("block-padding-top--85"),
                "padding-top-class");
            VerifyIsTrue(GetElAttribute("#block_" + numberBlock, "class").Contains("block-padding-bottom--85"),
                "padding-bottom-class");
            VerifyAreEqual("85px",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock)).GetCssValue("padding-top"),
                "padding-top");
            VerifyAreEqual("85px",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock)).GetCssValue("padding-bottom"),
                "padding-bottom");
            VerifyAreEqual("none",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock)).GetCssValue("background-image"),
                "background-image");

            BlockSettingsBtn(numberBlock);

            Driver.FindElement(By.CssSelector("#tabFormFront span")).Click();
            VerifyAreEqual("Светлая",
                GetElAttribute("[data-e2e=\"ColorScheme\"] option[selected=\"selected\"]", "label"),
                "setting color scheme");
            VerifyAreEqual("85px", Driver.FindElement(By.CssSelector("[data-e2e=\"padding_top\"] " +
                                                                     ".ngrs-value-runner .ngrs-value-max .ng-binding"))
                    .Text,
                "setting padding-top");
            VerifyAreEqual("85px", Driver.FindElement(By.CssSelector("[data-e2e=\"padding_bottom\"] " +
                                                                     ".ngrs-value-runner .ngrs-value-max .ng-binding"))
                    .Text,
                "setting padding-bottom");
            VerifyIsTrue(GetElAttribute("[data-e2e=\"mobile_hidden\"] input", "class").Contains("ng-empty"),
                "mobile-hidden");
            VerifyIsTrue(GetElAttribute("[data-e2e=\"desktop_hidden\"] input", "class").Contains("ng-empty"),
                "desctop-hidden");
            VerifyIsTrue(GetElAttribute("[data-e2e=\"show_title\"] input", "class").Contains("ng-not-empty"),
                "show_title");
            VerifyIsTrue(GetElAttribute("[data-e2e=\"show_subtitle\"] input", "class").Contains("ng-not-empty"),
                "show_subtitle");
            VerifyIsTrue(GetElAttribute("[data-e2e=\"ShowOnAllPage\"] input", "class").Contains("ng-empty"),
                "ShowOnAllPage");
            VerifyAreEqual("./areas/landing/frontend/images/nophoto_cover.png",
                GetElAttribute("[data-e2e=\"img_picture\"]", "ng-src"),
                "img_picture");

            Driver.FindElement(By.CssSelector("#tabForm span")).Click();

            Driver.FindElement(By.CssSelector("#tabHeaderForm span")).Click();
            VerifyAreEqual("", GetElAttribute("input[data-e2e=\"SettingsFormTitle\"]"),
                "FormTitle");
            VerifyAreEqual("", GetElAttribute("input[data-e2e=\"SettingsFormSubTitle\"]"),
                "FormSubTitle");
            VerifyAreEqual("Отправить", GetElAttribute("input[data-e2e=\"FormButtonText\"]"),
                "FormButtonText");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormGrid\"]"))
                    .FindElements(By.CssSelector("[as-sortable-item]")).Count == 3,
                "FormGrid rows-count");
            VerifyAreEqual("Имя", Driver.GetGridCell(0, "Title").FindElement(By.CssSelector("input")).GetAttribute("value"),
                "grid[0] - input Title");
            VerifyAreEqual("Имя",
                Driver.GetGridCell(0, "TitleCrm").FindElement(By.CssSelector("select option[selected=\"selected\"]"))
                    .GetAttribute("label"),
                "grid[0] - input TitleCrm");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Required").FindElement(By.CssSelector("input")).GetAttribute("class")
                    .Contains("ng-not-empty"),
                "grid[0] - input Required");
            VerifyAreEqual("Телефон",
                Driver.GetGridCell(1, "Title").FindElement(By.CssSelector("input")).GetAttribute("value"),
                "grid[1] - input Title");
            VerifyAreEqual("Телефон",
                Driver.GetGridCell(1, "TitleCrm").FindElement(By.CssSelector("select option[selected=\"selected\"]"))
                    .GetAttribute("label"),
                "grid[1] - input TitleCrm");
            VerifyIsTrue(
                Driver.GetGridCell(1, "Required").FindElement(By.CssSelector("input")).GetAttribute("class")
                    .Contains("ng-not-empty"),
                "grid[1] - input Required");
            VerifyAreEqual("Email", Driver.GetGridCell(2, "Title").FindElement(By.CssSelector("input")).GetAttribute("value"),
                "grid[2] - input Title");
            VerifyAreEqual("Email",
                Driver.GetGridCell(2, "TitleCrm").FindElement(By.CssSelector("select option[selected=\"selected\"]"))
                    .GetAttribute("label"),
                "grid[2] - input TitleCrm");
            VerifyIsTrue(
                Driver.GetGridCell(2, "Required").FindElement(By.CssSelector("input")).GetAttribute("class")
                    .Contains("ng-not-empty"),
                "grid[2] - input Required");
            VerifyAreEqual("0", GetElAttribute("[data-e2e=\"TextPosition\"] option[selected=\"selected\"]"),
                "InputTextPosition");
            VerifyIsTrue(GetElAttribute("[data-e2e=\"ShowAgreement\"] input", "class").Contains("ng-not-empty"),
                "show_subtitle");
            VerifyAreEqual("Я согласен на обработку персональных данных",
                GetElAttribute("[data-e2e=\"AgreementText\"]"),
                "AgreementText");

            Driver.FindElement(By.CssSelector("#tabHeaderActions span")).Click();
            VerifyAreEqual("Показать сообщение",
                GetElAttribute("[data-e2e=\"PosFormAction\"] option[selected=\"selected\"]", "label"),
                "PosFormAction");
            //VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
            //    driver.FindElement(By.CssSelector("#cke_editor3 iframe body")).Text,
            //    "PostMessageText");
            VerifyAreEqual("Лиды",
                GetElAttribute("[data-e2e=\"SelectSalesFunnels\"] option[selected=\"selected\"]", "label"),
                "SelectSalesFunnels");

            Driver.FindElement(By.CssSelector("#tabHeaderGoals span")).Click();
            VerifyAreEqual("",
                GetElAttribute("[data-e2e=\"YaMetrikaEvent\"]"),
                "YaMetrikaEvent");
            VerifyAreEqual("",
                GetElAttribute("[data-e2e=\"gaEventCategory\"]"),
                "gaEventCategory");
            VerifyAreEqual("",
                GetElAttribute("[data-e2e=\"gaEventAction\"]"),
                "gaEventAction");

            Driver.FindElement(By.CssSelector("#tabFormSetting_0 span")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"OfferItems\"]"))
                    .FindElements(By.ClassName("blocks-constructor-row")).Count == 0,
                "OfferItems");
            VerifyIsTrue(
                GetElAttribute("[data-e2e=\"SelectUsell\"] option[selected=\"selected\"]", "label")
                    .Contains("Не выбран"),
                "SelectedUpsell");
            BlockSettingsClose();

            ReInitClient();
            //display in desctop
            GoToClient(lpPath);
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[id=\"block_" + numberBlock + "\"]")).Count == 1,
                "(d)block id=" + numberBlock);
            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Text,
                "(d)TitleFormBlock text 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Count == 1,
                "(d)SubTitleFormBlock 1");
            VerifyAreEqual("Более 5000 функций для удобных и эффективных продаж",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Text,
                "(d)SubTitleFormBlock text 1");

            VerifyAreEqual("Имя",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                "(d)FormField field name");
            VerifyAreEqual("Телефон",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                "(d)FormField field phone");
            VerifyAreEqual("Email",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                "(d)FormField field email");
            VerifyAreEqual("Я согласен на обработку персональных данных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span:not(.lp-checkbox-decor)")).Text,
                "(d)FormField agreement text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] " +
                                                           ".lp-form__submit-block button.lp-btn--primary span.ladda-label"))
                    .Text.Contains("Отправить"),
                "(d)FormField button text");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock + " .lp-block-form"))
                    .GetCssValue("background-color"),
                "(d)background-color");
            VerifyIsTrue(
                GetElAttribute("#block_" + numberBlock + " .lp-block-form", "class").Contains("block-padding-top--85"),
                "(d)padding-top-class");
            VerifyIsTrue(
                GetElAttribute("#block_" + numberBlock + " .lp-block-form", "class")
                    .Contains("block-padding-bottom--85"),
                "(d)padding-bottom-class");
            VerifyAreEqual("85px",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock + " .lp-block-form"))
                    .GetCssValue("padding-top"),
                "(d)padding-top");
            VerifyAreEqual("85px",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock + " .lp-block-form"))
                    .GetCssValue("padding-bottom"),
                "(d)padding-bottom");
            VerifyAreEqual("none",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock + " .lp-block-form"))
                    .GetCssValue("background-image"),
                "(d)background-image");

            //display in mobile
            GoToMobile(lpPath);
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[id=\"block_" + numberBlock + "\"]")).Count == 1,
                "(m)block id=" + numberBlock);
            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Text,
                "(m)TitleFormBlock text 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Count == 1,
                "(m)SubTitleFormBlock 1");
            VerifyAreEqual("Более 5000 функций для удобных и эффективных продаж",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Text,
                "(m)SubTitleFormBlock text 1");

            VerifyAreEqual("Имя",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                "(m)FormField field name");
            VerifyAreEqual("Телефон",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                "(m)FormField field phone");
            VerifyAreEqual("Email",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                "(m)FormField field email");
            VerifyAreEqual("Я согласен на обработку персональных данных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span:not(.lp-checkbox-decor)")).Text,
                "(m)FormField agreement text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] " +
                                                           ".lp-form__submit-block button.lp-btn--primary span.ladda-label"))
                    .Text.Contains("Отправить"),
                "(m)FormField button text");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock + " .lp-block-form"))
                    .GetCssValue("background-color"),
                "(m)background-color");
            VerifyIsTrue(
                GetElAttribute("#block_" + numberBlock + " .lp-block-form", "class").Contains("block-padding-top--85"),
                "(m)padding-top-class");
            VerifyIsTrue(
                GetElAttribute("#block_" + numberBlock + " .lp-block-form", "class")
                    .Contains("block-padding-bottom--85"),
                "(m)padding-bottom-class");
            VerifyAreEqual("42.5px",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock + " .lp-block-form"))
                    .GetCssValue("padding-top"),
                "(m)padding-top");
            VerifyAreEqual("42.5px",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock + " .lp-block-form"))
                    .GetCssValue("padding-bottom"),
                "(m)padding-bottom");
            VerifyAreEqual("none",
                Driver.FindElement(By.CssSelector("#block_" + numberBlock + " .lp-block-form"))
                    .GetCssValue("background-image"),
                "(m)background-image");

            VerifyFinally(TestName);
        }

        [Test]
        public void EditBlock()
        {
            //изменения текста в заголовке и подзаголовке
            TestName = "EditBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient(lpPath);
            Thread.Sleep(1000);
            ////УДАЛИТЬ
            //AddBlockByBtnBig(blockType, blockName);
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized")).Clear();
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"] .inplace-initialized"))
                .SendKeys("New Title");
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized")).Clear();
            Thread.Sleep(500);
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"] .inplace-initialized"))
                .SendKeys("New Subtitle");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("blocks-constructor-container")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("New Title",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"] [data-inplace-rich]")).Text,
                "block title");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"] [data-inplace-rich]"))
                    .Text,
                "block subtitle");

            ReInitClient();
            //display in desctop
            GoToClient(lpPath);
            Thread.Sleep(1000);
            VerifyAreEqual("New Title",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Text,
                "(d)block title");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Text,
                "(d)block subtitle");

            GoToMobile(lpPath);
            Thread.Sleep(1000);
            VerifyAreEqual("New Title",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "Block\"]")).Text,
                "(m)block title");
            VerifyAreEqual("New Subtitle",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "Block\"]")).Text,
                "(m)block subtitle");

            VerifyFinally(TestName);
        }

        [Test]
        public void FunctionalityBlock()
        {
            //при заполнении формы в crm добавляется лид
            TestName = "FunctionalityBlock";
            VerifyBegin(TestName);
            ReInit();
            GoToClient(lpPath);
            Thread.Sleep(1000);
            ////УДАЛИТЬ
            //AddBlockByBtnBig(blockType, blockName);
            FillLpFormField("MyName");
            FillLpFormField("89009009090", 1);
            FillLpFormField("mymail@mail.ru", 2);
            Driver.CheckBoxCheck("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement");
            SendForm();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"]")).Count == 0,
                "form count after form send");
            VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                Driver.FindElement(By.CssSelector(".lp-form__content--success span")).Text,
                "form-success-message");

            ReInitClient();
            GoToClient(lpPath);
            Thread.Sleep(1000);
            FillLpFormField("MyDesctopName");
            FillLpFormField("89009009091", 1);
            FillLpFormField("mydesctopmail@mail.ru", 2);
            Driver.CheckBoxCheck("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement");
            SendForm();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"]")).Count == 0,
                "form count after form send");
            VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                Driver.FindElement(By.CssSelector(".lp-form__content--success span")).Text,
                "form-success-message");

            ReInitClient();
            GoToMobile(lpPath);
            Thread.Sleep(1000);
            FillLpFormField("MyMobileName");
            FillLpFormField("89009009092", 1);
            FillLpFormField("mymobilemail@mail.ru", 2);
            Driver.CheckBoxCheck("[data-e2e=\"" + blockCapture + "\"] .lp-form__agreement");
            SendForm();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"]")).Count == 0,
                "form count after form send");
            VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                Driver.FindElement(By.CssSelector(".lp-form__content--success span")).Text,
                "form-success-message");

            ReInit();
            GoToAdmin("leads?salesFunnelId=1");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"UseGrid\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("li[data-e2e=\"Новый\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("MyName");
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("MyName",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "leadFirstName");
            VerifyAreEqual("89009009090",
                GetElAttribute("Lead_Customer_Phone", "value", "Id"),
                "Lead_Customer_Phone");
            VerifyAreEqual("mymail@mail.ru",
                GetElAttribute("Lead_Customer_EMail", "value", "Id"),
                "Lead_Customer_EMail");

            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).Text
                    .Contains("Лид из лендинга \"" + lpPath.Substring(lpPath.IndexOf("/") + 1)),
                "Lead_Description landing name");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(lpPath + "/"),
                "Lead_Description landing paht");
            Driver.FindElement(By.CssSelector(".sticky-page-name .link-danger")).Click();
            Driver.SwalConfirm();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("MyDesctopName");
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("MyDesctopName",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "(d)leadFirstName");
            VerifyAreEqual("89009009091",
                GetElAttribute("Lead_Customer_Phone", "value", "Id"),
                "(d)Lead_Customer_Phone");
            VerifyAreEqual("mydesctopmail@mail.ru",
                GetElAttribute("Lead_Customer_EMail", "value", "Id"),
                "(d)Lead_Customer_EMail");

            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).Text
                    .Contains("Лид из лендинга \"" + lpPath.Substring(lpPath.IndexOf("/") + 1)),
                "(d)Lead_Description landing name");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(lpPath + "/"),
                "(d)Lead_Description landing paht");
            Driver.FindElement(By.CssSelector(".sticky-page-name .link-danger")).Click();
            Driver.SwalConfirm();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("MyMobileName");
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("MyMobileName",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "(m)leadFirstName");
            VerifyAreEqual("89009009092",
                GetElAttribute("Lead_Customer_Phone", "value", "Id"),
                "(m)Lead_Customer_Phone");
            VerifyAreEqual("mymobilemail@mail.ru",
                GetElAttribute("Lead_Customer_EMail", "value", "Id"),
                "(m)Lead_Customer_EMail");

            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).Text
                    .Contains("Лид из лендинга \"" + lpPath.Substring(lpPath.IndexOf("/") + 1)),
                "(m)Lead_Description landing name");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(lpPath + "/"),
                "(m)Lead_Description landing paht");
            Driver.FindElement(By.CssSelector(".sticky-page-name .link-danger")).Click();
            Driver.SwalConfirm();

            //ТУДУ: ДОПИСАТЬ ДЛЯ ВСЕХ РАЗМЕРОВ 
            VerifyFinally(TestName);
        }

        [Test]
        public void MoveBlock()
        {
            TestName = "MoveBlock";
            VerifyBegin(TestName);
            ReInit();
            GoToClient(lpPath);
            ////УДАЛИТЬ
            //AddBlockByBtnBig(blockType, blockName);

            AddBlockByBtnBig("Text", "textHeader");
            Thread.Sleep(1000);
            AddBlockByBtnBig("Text", "textHeader");
            Thread.Sleep(1000);

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
        public void RemoveBlock()
        {
            TestName = "RemoveBlock";
            VerifyBegin(TestName);
            GoToClient("lp/test1");
            Driver.ScrollTo(By.Id("block_1"));
            ////УДАЛИТЬ
            //AddBlockByBtnBig(blockType, blockName);

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