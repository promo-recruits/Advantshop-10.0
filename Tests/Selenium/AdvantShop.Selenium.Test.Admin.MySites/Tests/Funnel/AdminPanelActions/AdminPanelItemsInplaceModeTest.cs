using System;
using System.Drawing;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.AdminPanelActions
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class AdminPanelItemsInplaceModeTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSubBlock.csv"
            );

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin("funnels/site/3");
            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CheckInplaceModeDisable()
        {
            APItemInplaceModeClick(true);

            //ii.проверить, что в клиентском режиме ненужные блоки и классы не отображаются:
            //1.иконка неперечеркнутого глаза, в url? inplace = false
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".lp-admin-panel-state-edit:checked")).Count,
                "inplace not checked");
            VerifyIsTrue(Driver.Url.Contains("?inplace=false"), "inplace in url");
            //2.у тела нет атрибута blocks-constructor - main
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector("body[blocks-constructor-main]")).Count,
                "body inplace attribute");
            //3.blocks - constructor - container нет
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".lp-main blocks-constructor-container")).Count,
                "inplace containers");
            //4.lp - block - constructor - hint(содержит тип и id) нет
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".lp-block-constructor-hint")).Count, "inplace hints");
            //5.кнопок добавления блоков нет
            VerifyAreEqual(0, Driver.FindElements(AdvBy.DataE2E("AddBlockBtnSml")).Count, "add block - small btns");
            VerifyAreEqual(0, Driver.FindElements(AdvBy.DataE2E("AddBlockBtnBig")).Count, "add block - big btn");
            //6.при наведении не отображаются кнопки настроек, перемещение вверх - вниз, удаление;
            Driver.MouseFocus(By.ClassName("lp-block-header"));
            VerifyAreEqual(0, Driver.FindElements(AdvBy.DataE2E("BlockSettingsBtn")).Count,
                "at block hover settings btns count");
            VerifyAreEqual(0, Driver.FindElements(AdvBy.DataE2E("MoveUpBlockBtn")).Count,
                "at block hover settings btns count");
            VerifyAreEqual(0, Driver.FindElements(AdvBy.DataE2E("BlockSeMoveDownBlockBtnttingsBtn")).Count,
                "at block hover settings btns count");
            VerifyAreEqual(0, Driver.FindElements(AdvBy.DataE2E("DelBlockBtn")).Count,
                "at block hover settings btns count");
            //7.разделителей между блоками нет
            //8.инплейс - редактирования логотипа нет
            Driver.MouseFocus(By.ClassName("lp-header-logo"));
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".lp-header-logo .inplace-buttons-item")).Count,
                "add/edit logo disabled");
            //9.инплейс - редактирования картинок нет.
            Driver.MouseFocus(By.ClassName("lp-block-text-image-reverse"));
            VerifyAreEqual(0,
                Driver.FindElements(By.CssSelector(".lp-block-text-image-reverse .inplace-buttons-item")).Count,
                "edit image disabled");

            //10.инплейс - редактирования иконок нет.
            Driver.MouseFocus(By.ClassName("lp-block-columns-icon-title"));
            VerifyAreEqual(0,
                Driver.FindElements(By.CssSelector(".lp-block-columns-icon-title .inplace-buttons-item")).Count,
                "edit icon disabled");

            //11.инплейс - редактирования текстовых блоков нет.
            Driver.FindElement(By.CssSelector(".lp-block-text-image-reverse .lp-h3--color")).Click();
            VerifyAreEqual(0,
                Driver.FindElements(By.CssSelector(".lp-block-text-image-reverse .lp-h3--color .cke_focus")).Count,
                "edit text after");

            //13.Ссылки на блоки - пустой текст
            VerifyIsTrue(String.IsNullOrEmpty(Driver.FindElement(By.ClassName("block-type-reference")).Text.Trim()),
                "block reference");

            APItemInplaceModeClick(false);
        }

        [Test]
        public void CheckInplaceModeEnable()
        {
            //i.проверить отображение всех блоков, характерных для инплейс режима: 
            //1.Иконка перечеркнутого глаза, в url есть хвост? inplace = true
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-admin-panel-state-edit:checked")).Count,
                "inplace checked");
            VerifyIsTrue(Driver.Url.Contains("?inplace=true"), "inplace in url");

            //2.У тела есть атрибут blocks-constructor - main
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector("body[blocks-constructor-main]")).Count,
                "body inplace attribute");

            //3.Есть обертывающие классы с тегами blocks - constructor - container
            VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".lp-main blocks-constructor-container")).Count,
                "inplace containers");

            //4.отображается блок lp-block - constructor - hint с типом и id блока, 
            VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".lp-block-constructor-hint")).Count, "inplace hints");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-block-constructor-hint")).Displayed,
                "1st hint displayed");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-block-constructor-hint")).Text
                    .Contains("headerCommunication, ID блока:"),
                "1st hint text");

            //5.отображается кнопка «Добавить блок(+)», 
            VerifyAreEqual(4, Driver.FindElements(AdvBy.DataE2E("AddBlockBtnSml")).Count, "add block - small btns");
            VerifyIsTrue(Driver.FindElement(AdvBy.DataE2E("AddBlockBtnSml")).Displayed &&
                         Driver.FindElement(AdvBy.DataE2E("AddBlockBtnSml")).Enabled,
                "add block - first small btn enabled");

            //8.отображается кнопка «Добавить блок» текстом внизу в качестве последнего блока. 
            VerifyAreEqual(1, Driver.FindElements(AdvBy.DataE2E("AddBlockBtnBig")).Count, "add block - big btn");
            VerifyIsTrue(Driver.FindElement(AdvBy.DataE2E("AddBlockBtnBig")).Displayed &&
                         Driver.FindElement(AdvBy.DataE2E("AddBlockBtnBig")).Enabled, "add block - big btn enabled");

            //6.при наведении отображаются кнопки настроек, перемещение вверх - вниз, удаление;
            Driver.MouseFocus(By.ClassName("lp-block-header"));
            VerifyIsTrue(Driver.FindElements(AdvBy.DataE2E("BlockSettingsBtn"))[0].Displayed &&
                         Driver.FindElements(AdvBy.DataE2E("BlockSettingsBtn"))[0].Enabled,
                "at block hover settings btn enabled");
            VerifyIsTrue(Driver.FindElements(AdvBy.DataE2E("MoveUpBlockBtn"))[0].Displayed &&
                         Driver.FindElements(AdvBy.DataE2E("MoveUpBlockBtn"))[0].Enabled,
                "at block hover up btn enabled");
            VerifyIsTrue(Driver.FindElements(AdvBy.DataE2E("MoveDownBlockBtn"))[0].Displayed &&
                         Driver.FindElements(AdvBy.DataE2E("MoveDownBlockBtn"))[0].Enabled,
                "at block hover down btn enabled");
            VerifyIsTrue(Driver.FindElements(AdvBy.DataE2E("DelBlockBtn"))[0].Displayed &&
                         Driver.FindElements(AdvBy.DataE2E("DelBlockBtn"))[0].Enabled,
                "at block hover remove btn enabled");
            //7.отображается разделитель между блоками - не нужно и сложно

            //9.При наведении на лого отображается возможность редактировать лого.
            Driver.MouseFocus(By.ClassName("lp-header-logo"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo .icon-plus-circled-before")).Displayed &&
                         Driver.FindElement(By.CssSelector(".lp-header-logo .icon-plus-circled-before")).Enabled,
                "add logo enabled");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo .icon-pencil-before")).Displayed &&
                         Driver.FindElement(By.CssSelector(".lp-header-logo .icon-pencil-before")).Enabled,
                "edit logo enabled");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo .icon-trash-before")).Displayed &&
                         Driver.FindElement(By.CssSelector(".lp-header-logo .icon-trash-before")).Enabled,
                "remove logo enabled");

            //11.При наведении на картинку отображается возможность редактировать картинку.
            Driver.MouseFocus(By.CssSelector(".lp-block-text-image-reverse picture-loader-trigger"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-block-text-image-reverse .subblock-inplace-image-trigger"))
                    .Displayed &&
                Driver.FindElement(By.CssSelector(".lp-block-text-image-reverse .subblock-inplace-image-trigger"))
                    .Enabled, "edit image enabled");

            //12.При наведении на иконку отображается возможность редактировать иконку.
            Driver.MouseFocus(By.CssSelector(".lp-block-columns-icon-title picture-loader-trigger"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-block-columns-icon-title .subblock-inplace-image-trigger"))
                    .Displayed &&
                Driver.FindElement(By.CssSelector(".lp-block-columns-icon-title .subblock-inplace-image-trigger"))
                    .Enabled, "edit icon enabled");

            //10.При наведении на текст отображается возможность редактировать текст.
            VerifyAreEqual(0,
                Driver.FindElements(By.CssSelector(".lp-block-text-image-reverse .lp-h3--color .cke_focus")).Count,
                "edit text before");
            Driver.FindElement(By.CssSelector(".lp-block-text-image-reverse .lp-h3--color")).Click();
            Thread.Sleep(100);
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".lp-block-text-image-reverse .lp-h3--color .cke_focus")).Count,
                "edit text after");

            //13.Ссылки на блоки - отображается текст
            VerifyAreEqual("Выберите в настройках блок, который будет отображаться",
                Driver.FindElement(By.CssSelector(".block-type-reference .lp-block")).Text.Trim(), "block reference");
        }

        [Test]
        public void CheckInplaceModeEnableMobile()
        {
            Driver.Manage().Window.Size = new Size(414, 700);
            //iii.Проверить, что если inplace включен, и переход в мобилку, то ЧАСТЬ инплейс-элементов исчезает, а часть продолжает работать
            //1.Иконка перечеркнутого глаза, в url есть хвост? inplace = true
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-admin-panel-state-edit:checked")).Count,
                "inplace checked");
            VerifyIsTrue(Driver.Url.Contains("?inplace=true"), "inplace in url");

            //2.У тела есть атрибут blocks-constructor - main
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector("body[blocks-constructor-main]")).Count,
                "body inplace attribute");

            //3.Есть обертывающие классы с тегами blocks - constructor - container
            VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".lp-main blocks-constructor-container")).Count,
                "inplace containers");

            //4.отображается блок lp-block - constructor - hint с типом и id блока, 
            VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".lp-block-constructor-hint")).Count, "inplace hints");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".lp-block-constructor-hint")).Displayed,
                "1st hint not displayed");

            //5.отображается кнопка «Добавить блок(+)», 
            VerifyAreEqual(4, Driver.FindElements(AdvBy.DataE2E("AddBlockBtnSml")).Count, "add block - small btns");
            VerifyIsFalse(Driver.FindElement(AdvBy.DataE2E("AddBlockBtnSml")).Displayed,
                "add block - first small btn not displayed");

            //8.отображается кнопка «Добавить блок» текстом внизу в качестве последнего блока. 
            VerifyAreEqual(1, Driver.FindElements(AdvBy.DataE2E("AddBlockBtnBig")).Count, "add block - big btn");
            VerifyIsFalse(Driver.FindElement(AdvBy.DataE2E("AddBlockBtnBig")).Displayed,
                "add block - big btn not displayed");

            //6.при наведении отображаются кнопки настроек, перемещение вверх - вниз, удаление;
            Driver.FindElement(By.ClassName("lp-block-header")).Click();
            VerifyIsFalse(Driver.FindElements(AdvBy.DataE2E("BlockSettingsBtn"))[0].Displayed,
                "at block hover settings btn not displayed");
            VerifyIsFalse(Driver.FindElements(AdvBy.DataE2E("MoveUpBlockBtn"))[0].Displayed,
                "at block hover up btn not displayed");
            VerifyIsFalse(Driver.FindElements(AdvBy.DataE2E("MoveDownBlockBtn"))[0].Displayed,
                "at block hover down btn not displayed");
            VerifyIsFalse(Driver.FindElements(AdvBy.DataE2E("DelBlockBtn"))[0].Displayed,
                "at block hover remove btn not displayed");
            //7.отображается разделитель между блоками - не нужно и сложно

            //9.При наведении на лого отображается возможность редактировать лого.
            Driver.FindElement(By.ClassName("lp-header-logo")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo .icon-plus-circled-before")).Displayed &&
                         Driver.FindElement(By.CssSelector(".lp-header-logo .icon-plus-circled-before")).Enabled,
                "add logo enabled");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo .icon-pencil-before")).Displayed &&
                         Driver.FindElement(By.CssSelector(".lp-header-logo .icon-pencil-before")).Enabled,
                "edit logo enabled");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo .icon-trash-before")).Displayed &&
                         Driver.FindElement(By.CssSelector(".lp-header-logo .icon-trash-before")).Enabled,
                "remove logo enabled");

            //11.При наведении на картинку отображается возможность редактировать картинку.
            Driver.FindElement(By.CssSelector(".lp-block-text-image-reverse picture-loader-trigger")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lp-block-text-image-reverse .subblock-inplace-image-trigger"))
                    .Displayed &&
                Driver.FindElement(By.CssSelector(".lp-block-text-image-reverse .subblock-inplace-image-trigger"))
                    .Enabled, "edit image enabled");

            //12.При наведении на иконку отображается возможность редактировать иконку.
            Driver.FindElement(By.CssSelector(".lp-block-columns-icon-title picture-loader-trigger")).Click();
            if (Driver.FindElements(By.ClassName("adv-modal-close")).Count > 0)
            {
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
            }
            else
            {
                VerifyIsTrue(
                    Driver.FindElement(By.CssSelector(".lp-block-columns-icon-title .subblock-inplace-image-trigger"))
                        .Displayed &&
                    Driver.FindElement(By.CssSelector(".lp-block-columns-icon-title .subblock-inplace-image-trigger"))
                        .Enabled, "edit icon enabled");
            }

            //10.При наведении на текст отображается возможность редактировать текст.
            Driver.FindElement(By.CssSelector(".lp-block-text-image-reverse .lp-h3--color")).Click();
            VerifyAreEqual(0,
                Driver.FindElements(By.CssSelector(".lp-block-text-image-reverse .lp-h3--color .cke_focus")).Count,
                "edit text after");

            //13.Ссылки на блоки - отображается текст
            VerifyAreEqual("Выберите в настройках блок, который будет отображаться",
                Driver.FindElement(By.ClassName("block-type-reference")).Text.Trim(), "block reference");
            ReInit();
        }
    }
}