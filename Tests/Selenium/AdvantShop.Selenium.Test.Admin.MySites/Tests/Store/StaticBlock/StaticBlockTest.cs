using System;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.StaticBlock
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class StaticBlockTest : MySitesFunctions
    {
        private string sbPageUrl = "staticpages#?staticPageTab=blocks";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Store\\StaticBlock\\CMS.StaticBlock.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin(sbPageUrl);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void AddBlock()
        {
            Driver.GetByE2E("btnAdd").Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "My_static_block_key");
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "My static block name");
            Driver.CheckBoxCheck("[data-e2e=\"StaticBlockEnabled\"]", "CssSelector");
            Driver.SetCkText("My static block content", "editor1");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            string newDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("My_static_block_key");
            VerifyAreEqual("My_static_block_key", Driver.GetGridCell(0, "Key", "Blocks").Text, "added block key");
            VerifyAreEqual("My static block name", Driver.GetGridCell(0, "InnerName", "Blocks").Text,
                "added block name");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected,
                "added block enabled");
            VerifyAreEqual(newDateTime, Driver.GetGridCell(0, "AddedFormatted", "Blocks").Text,
                "added block dateAdded");
            VerifyAreEqual(newDateTime, Driver.GetGridCell(0, "ModifiedFormatted", "Blocks").Text,
                "added block dateModified");

            Driver.GetGridCell(0, "Key", "Blocks").FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("My_static_block_key", Driver.GetValue(AdvBy.DataE2E("StaticBlockKey")),
                "added block key in modal");
            VerifyAreEqual("My static block name", Driver.GetValue(AdvBy.DataE2E("StaticBlockName")),
                "added block name in modal");
            VerifyIsTrue(Driver.GetByE2E("StaticBlockEnabled").FindElement(By.TagName("input")).Selected,
                "added block enabled in modal");
            Driver.AssertCkText("My static block content", "editor2");
        }

        [Test]
        public void DeleteBlock()
        {
            VerifyAreEqual(1, Driver.GetGridCell(0, "_serviceColumn", "Blocks").FindElements(By.TagName("a")).Count,
                "count of links in serviceColumn");
            VerifyAreEqual(1,
                Driver.GetGridCell(0, "_serviceColumn", "Blocks").FindElements(By.CssSelector("a.fa-pencil-alt")).Count,
                "count of editLinks in serviceColumn");
            VerifyAreEqual(0,
                Driver.GetGridCell(0, "_serviceColumn", "Blocks").FindElements(By.CssSelector("a.fa-times")).Count,
                "count of deleteLinks in serviceColumn");
            VerifyAreEqual(10, Driver.FindElements(By.ClassName("fa-pencil-alt")).Count, "count of editLinks at page");
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("fa-times")).Count, "count of deleteLinks at page");

            VerifyAreEqual(0, Driver.FindElements(By.PartialLinkText("Удалить")).Count,
                "count of deleteLinks at page (by text)");
            SelectGridRow(0, "Blocks");
            Driver.GetByE2E("gridSelectionDropdownButton").Click();
            VerifyAreEqual(0, Driver.FindElements(By.PartialLinkText("Удалить")).Count,
                "count of deleteLinks at page (by text)");
        }

        [Test]
        public void EditBlock()
        {
            Driver.GridFilterSendKeys("staticblockkey100");
            VerifyAreEqual("staticblockkey100", Driver.GetGridCell(0, "Key", "Blocks").Text, "default key");
            VerifyAreEqual("inner name 100", Driver.GetGridCell(0, "InnerName", "Blocks").Text, "default name");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected,
                "default enabled");
            VerifyAreEqual("21.12.2013 11:11", Driver.GetGridCell(0, "AddedFormatted", "Blocks").Text,
                "default dateAdded");
            VerifyAreEqual("21.11.2016 11:09", Driver.GetGridCell(0, "ModifiedFormatted", "Blocks").Text,
                "default dateModified");

            Driver.GetGridCell(0, "_serviceColumn", "Blocks").Click();
            Thread.Sleep(500);
            VerifyAreEqual("staticblockkey100", Driver.GetValue(AdvBy.DataE2E("StaticBlockKey")), "default key(1)");
            VerifyAreEqual("inner name 100", Driver.GetValue(AdvBy.DataE2E("StaticBlockName")), "default name(1)");
            VerifyIsTrue(GetCheckboxState(Driver.GetByE2E("StaticBlockEnabled").FindElement(By.TagName("input"))),
                "default enabled(1)");
            Driver.AssertCkText("test static block content 100", "editor1");

            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "new_static_block_key");
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "New static block name");
            Driver.CheckBoxUncheck("[data-e2e=\"StaticBlockEnabled\"]", "CssSelector");
            Driver.SetCkText("New static block text content", "editor1");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            string newDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("staticblockkey100");
            VerifyAreEqual("Найдено записей: 0", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "old key is not found");

            Driver.GridFilterSendKeys("new_static_block_key");
            VerifyAreEqual("Найдено записей: 1", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "new key is found");
            VerifyAreEqual("new_static_block_key", Driver.GetGridCell(0, "Key", "Blocks").Text, "new key");
            VerifyAreEqual("New static block name", Driver.GetGridCell(0, "InnerName", "Blocks").Text, "new name");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected,
                "new enabled");
            VerifyAreEqual("21.12.2013 11:11", Driver.GetGridCell(0, "AddedFormatted", "Blocks").Text,
                "new dateAdded"); //is not changed
            VerifyAreEqual(newDateTime, Driver.GetGridCell(0, "ModifiedFormatted", "Blocks").Text, "new dateModified");

            Driver.GetGridCell(0, "Key", "Blocks").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("new_static_block_key", Driver.GetValue(AdvBy.DataE2E("StaticBlockKey")), "new key(1)");
            VerifyAreEqual("New static block name", Driver.GetValue(AdvBy.DataE2E("StaticBlockName")), "new name(1)");
            VerifyIsFalse(GetCheckboxState(Driver.GetByE2E("StaticBlockEnabled").FindElement(By.TagName("input"))),
                "new enabled(1)");
            Driver.AssertCkText("New static block text content", "editor2");

            //редактирование без сохранения изменений
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "new_static_block_key_edited_123");
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "New static block name edited 123");
            Driver.CheckBoxCheck("[data-e2e=\"StaticBlockEnabled\"]", "CssSelector");
            Driver.SetCkText("New static block text content edited 123", "editor2");
            Driver.FindElement(By.ClassName("close")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("new_static_block_key_edited_123");
            VerifyAreEqual("Найдено записей: 0", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "new key is not found (without saving)");

            Driver.GridFilterSendKeys("new_static_block_key");
            VerifyAreEqual("Найдено записей: 1", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "old key is found (without saving)");
            VerifyAreEqual("new_static_block_key", Driver.GetGridCell(0, "Key", "Blocks").Text,
                "new key (without saving)");
            VerifyAreEqual("New static block name", Driver.GetGridCell(0, "InnerName", "Blocks").Text,
                "new name (without saving)");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected,
                "new enabled (without saving)");
            VerifyAreEqual("21.12.2013 11:11", Driver.GetGridCell(0, "AddedFormatted", "Blocks").Text,
                "new dateAdded (without saving)"); //is not changed
            VerifyAreEqual(newDateTime, Driver.GetGridCell(0, "ModifiedFormatted", "Blocks").Text,
                "new dateModified (without saving)");
        }

        [Test]
        public void EditBlockInplace()
        {
            //ЗАМЕНИТЬ ДАТЫ НА ЗАДАННЫЕ В ГРИДЕ
            Driver.GridFilterSendKeys("staticblockkey2");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected,
                "block 2 not enabled default");
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected,
                "block 20 not enabled default");
            string dateAdded1 = Driver.GetGridCell(0, "AddedFormatted", "Blocks").Text;
            string dateAdded2 = Driver.GetGridCell(1, "AddedFormatted", "Blocks").Text;
            string dateFormatted1 = Driver.GetGridCell(0, "ModifiedFormatted", "Blocks").Text;
            string dateFormatted2 = Driver.GetGridCell(1, "ModifiedFormatted", "Blocks").Text;

            string dateFormattedNew = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Driver.GetGridCell(1, "Enabled", "Blocks").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected,
                "block 2 enable changed (1)");
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected,
                "block 20 enable changed (1)");

            Refresh();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected,
                "block 2 enable changed (2)");
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected,
                "block 20 enable changed (2)");
            VerifyAreEqual(dateAdded1, Driver.GetGridCell(0, "AddedFormatted", "Blocks").Text,
                "dateAdded1 after changed");
            VerifyAreEqual(dateAdded2, Driver.GetGridCell(1, "AddedFormatted", "Blocks").Text,
                "dateAdded1 after changed");
            VerifyAreNotEqual(dateFormatted1, Driver.GetGridCell(0, "ModifiedFormatted", "Blocks").Text,
                "dateFormatted1 after changed (1)"); //?
            VerifyAreEqual(dateFormattedNew, Driver.GetGridCell(0, "ModifiedFormatted", "Blocks").Text,
                "dateFormatted1 after changed (2)");
            VerifyAreNotEqual(dateFormatted2, Driver.GetGridCell(1, "ModifiedFormatted", "Blocks").Text,
                "dateFormatted2 after changed (1)"); //?
            VerifyAreEqual(dateFormattedNew, Driver.GetGridCell(1, "ModifiedFormatted", "Blocks").Text,
                "dateFormatted2 after changed (2)");
        }

        [Test]
        public void InstructionPage()
        {
            VerifyAreEqual("Инструкция. Статические блоки",
                Driver.FindElement(By.CssSelector(".link-academy span")).Text, "instruction link text");
            VerifyAreEqual("https://www.advantshop.net/help/pages/static-block-common",
                Driver.FindElement(By.CssSelector(".link-academy")).GetAttribute("href"), "instruction link");
            Driver.FindElement(By.CssSelector(".link-academy span")).Click();
            Thread.Sleep(1000);
            Driver.SwitchTo().Window(Driver.WindowHandles[1]);
            VerifyAreEqual("https://www.advantshop.net/help/pages/static-block-common", Driver.Url, "instruction link");
            ReInit();
        }

        [Test]
        public void StaticBlockPage()
        {
            VerifyIsNull(CheckConsoleLog(true), "log at staticBlockPage(1)");

            GoToStoreSettings("Страницы и блоки", "Блоки");
            VerifyAreEqual(BaseUrl + "adminv3/" + sbPageUrl, Driver.Url, "static block page url");
            VerifyIsNull(CheckConsoleLog(true), "log at staticBlockPage(2)");
        }

        [Test]
        public void ValidationNOTCOMPLETE()
        {
            //??? валидация символов и длины
            //symbols.length ~ 45, longString.length ~ 334
            string symbols = "ёЁ1!2\"3№4;5%6:7?8*9(0)-_=+`~@#$^&[{]}\\|'/,<.>";
            string longString =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit " +
                "in voluptate velit esse cillum dolore eu fugiat nulla pariatur.";

            VerifyIsTrue(false, "добить проверку разрешенных символов и длины имени и ключа");

            //новый блок, проверить количество обязательных
            //пустой ключ и заполненное имя
            //заполненный ключ и пустое имя
            //заполненный ключ и имя -- ок

            Driver.GetByE2E("btnAdd").Click();
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count, "empty staticBlock form(1.1)");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Не заполнены поля"),
                "not valid form message(1.1)");
            VerifyAreEqual(2, Driver.FindElements(By.TagName("validation-list-item")).Count,
                "count of not valid fields(1.1)");
            VerifyAreEqual("Ключ доступа", Driver.FindElements(By.TagName("validation-list-item"))[0].Text,
                "not valid key(1.1)");
            VerifyAreEqual("Название", Driver.FindElements(By.TagName("validation-list-item"))[1].Text,
                "not valid name(1.1)");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "validation-key-1");
            Driver.ClearInput(AdvBy.DataE2E("StaticBlockName"));
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count, "empty staticBlock form(1.2)");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Не заполнены поля"),
                "not valid form message(1.2)");
            VerifyAreEqual(1, Driver.FindElements(By.TagName("validation-list-item")).Count,
                "count of not valid fields(1.2)");
            VerifyAreEqual("Название", Driver.FindElements(By.TagName("validation-list-item"))[0].Text,
                "not valid name(1.2)");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();

            Driver.ClearInput(AdvBy.DataE2E("StaticBlockKey"));
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "validation name 1");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count, "empty staticBlock form(1.3)");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Не заполнены поля"),
                "not valid form message(1.3)");
            VerifyAreEqual(1, Driver.FindElements(By.TagName("validation-list-item")).Count,
                "count of not valid fields(1.3)");
            VerifyAreEqual("Ключ доступа", Driver.FindElements(By.TagName("validation-list-item"))[0].Text,
                "not valid key(1.3)");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "validation-key-1");
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "validation name 1");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".toast-success .toast-close-button")).Click();

            //редактирование
            //очистить ключ и имя
            //очистить ключ, непустое имя
            //непустой ключ, очистить имя
            //закрыть без сохранения, ключ и имя -- ок
            Driver.GridFilterSendKeys("validation-key-1");
            Driver.GetGridCell(0, "Key", "Blocks").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);

            Driver.ClearInput(AdvBy.DataE2E("StaticBlockKey"));
            Driver.ClearInput(AdvBy.DataE2E("StaticBlockName"));

            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count, "empty staticBlock form(2.1)");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Не заполнены поля"),
                "not valid form message(2.1)");
            VerifyAreEqual(2, Driver.FindElements(By.TagName("validation-list-item")).Count,
                "count of not valid fields(2.1)");
            VerifyAreEqual("Ключ доступа", Driver.FindElements(By.TagName("validation-list-item"))[0].Text,
                "not valid key(2.1)");
            VerifyAreEqual("Название", Driver.FindElements(By.TagName("validation-list-item"))[1].Text,
                "not valid name(2.1)");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "validation-key-1");
            Driver.ClearInput(AdvBy.DataE2E("StaticBlockName"));
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count, "empty staticBlock form(2.2)");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Не заполнены поля"),
                "not valid form message(2.2)");
            VerifyAreEqual(1, Driver.FindElements(By.TagName("validation-list-item")).Count,
                "count of not valid fields(2.2)");
            VerifyAreEqual("Название", Driver.FindElements(By.TagName("validation-list-item"))[0].Text,
                "not valid name(2.2)");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();

            Driver.ClearInput(AdvBy.DataE2E("StaticBlockKey"));
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "validation name 1");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count, "empty staticBlock form(2.3)");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Не заполнены поля"),
                "not valid form message(2.3)");
            VerifyAreEqual(1, Driver.FindElements(By.TagName("validation-list-item")).Count,
                "count of not valid fields(2.3)");
            VerifyAreEqual("Ключ доступа", Driver.FindElements(By.TagName("validation-list-item"))[0].Text,
                "not valid key(2.3)");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "validation-key-1");
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "validation name 1");
            Driver.FindElement(By.ClassName("btn-cancel")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("validation-key-1", Driver.GetGridCell(0, "Key", "Blocks").Text, "grid cell key (2.4)");

            //новый блок, ключ и имя как есть в системе
            //ключ как в системе, имя как нет в системе
            //ключ как нет в системе, имя как в системе - ок
            Driver.GridFilterSendKeys("");

            Driver.GetByE2E("btnAdd").Click();
            Thread.Sleep(500); //bannerDetails Баннер, единый для всех товаров
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "bannerDetails");
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "Баннер, единый для всех товаров");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count,
                "dublicated key of staticBlock(3.1)");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Дублируется поле"),
                "not valid form message(3.1)");
            VerifyAreEqual(1, Driver.FindElements(By.TagName("validation-list-item")).Count,
                "count of not valid fields(3.1)");
            VerifyAreEqual("Ключ доступа", Driver.FindElements(By.TagName("validation-list-item"))[0].Text,
                "not valid key(3.1)");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "Новый баннер для товаров");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count,
                "dublicated key of staticBlock(3.2)");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Дублируется поле"),
                "not valid form message(3.2)");
            VerifyAreEqual(1, Driver.FindElements(By.TagName("validation-list-item")).Count,
                "count of not valid fields(3.2)");
            VerifyAreEqual("Ключ доступа", Driver.FindElements(By.TagName("validation-list-item"))[0].Text,
                "not valid key(3.2)");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "newBannerDetails");
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "Баннер, единый для всех товаров");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".toast-success .toast-close-button")).Click();

            //редактирование
            //ключ и имя как есть в системе
            //ключ как в системе, имя как нет в системе
            //ключ как нет в системе, имя как в системе - ок
            Driver.GridFilterSendKeys("newBannerDetails");
            Driver.GetGridCell(0, "Key", "Blocks").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "bannerDetails");
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "Баннер, единый для всех товаров");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count,
                "dublicated key of staticBlock(4.1)");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Дублируется поле"),
                "not valid form message(4.1)");
            VerifyAreEqual(1, Driver.FindElements(By.TagName("validation-list-item")).Count,
                "count of not valid fields(4.1)");
            VerifyAreEqual("Ключ доступа", Driver.FindElements(By.TagName("validation-list-item"))[0].Text,
                "not valid key(4.1)");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "Новый баннер для товаров");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("toast-error")).Count,
                "dublicated key of staticBlock(4.2)");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-error")).Text.Contains("Дублируется поле"),
                "not valid form message(4.2)");
            VerifyAreEqual(1, Driver.FindElements(By.TagName("validation-list-item")).Count,
                "count of not valid fields(4.2)");
            VerifyAreEqual("Ключ доступа", Driver.FindElements(By.TagName("validation-list-item"))[0].Text,
                "not valid key(4.2)");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockKey"), "newBannerDetails");
            Driver.SendKeysInput(AdvBy.DataE2E("StaticBlockName"), "Баннер, единый для всех товаров");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".toast-success .toast-close-button")).Click();
        }
    }
}