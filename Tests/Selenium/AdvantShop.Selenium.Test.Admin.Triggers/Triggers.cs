using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AdvantShop.Selenium.Test.Admin.Triggers
{
    /// <summary>
    /// Этот класс сделан отдельно из-за того что в нем не нужно в методе OneTimeSetup осуществлять установку Триггеров, в отличие от остальных тестов
    /// </summary>
    [TestFixture]
    public class AddTrigger : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            InitializeService.RollBackDatabase();
            Init();
        }

        [Test]
        public void Add_trigger()
        {
            TestName = "AddTrigger";
            VerifyBegin(TestName);
            GoToAdmin();
            //T001->Т002
            Driver.WaitForElem(By.CssSelector(".sidebar__group-item ui-modal-trigger"));
            Driver.FindElement(By.CssSelector(".sidebar__group-item ui-modal-trigger")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Добавить канал продаж", Driver.FindElement(By.ClassName("modal-dialog")).FindElement(By.TagName("h1")).Text);
            Driver.FindElement(By.ClassName("close")).Click();
            Thread.Sleep(500);
            VerifyIsFalse(Driver.FindElements(By.ClassName("modal-dialog")).Count > 0, "q");
            Driver.FindElement(By.CssSelector(".sidebar__group-item ui-modal-trigger")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Триггерный маркетинг", Driver.FindElements(By.CssSelector(".modal-dialog .card-channel__title"))[0].Text);
            //проверку наличия картинки не получится сделать, т.к. находится в закрытом DOMе (shadow-root(close))
            Driver.FindElements(By.CssSelector(".card-channel"))[0].Click();
            Thread.Sleep(500);
            VerifyAreEqual("Канал продаж \"Триггерный маркетинг\"", Driver.FindElement(By.ClassName("modal-dialog")).FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("Триггерный маркетинг", Driver.FindElement(By.CssSelector(".sales-channel .card-channel__title")).Text);
            VerifyAreEqual("488px", Driver.FindElement(By.CssSelector("[alt=\"triggers 1\"]")).GetCssValue("width"));
            Driver.FindElement(By.CssSelector(".carousel-nav-next")).Click();
            VerifyAreEqual("488px", Driver.FindElement(By.CssSelector("[alt=\"triggers 2\"]")).GetCssValue("width"));
            Driver.FindElement(By.CssSelector(".carousel-nav-next")).Click();
            VerifyAreEqual("488px", Driver.FindElement(By.CssSelector("[alt=\"triggers 3\"]")).GetCssValue("width"));
            Driver.FindElement(By.CssSelector(".carousel-nav-next")).Click();
            VerifyAreEqual("488px", Driver.FindElement(By.CssSelector("[alt=\"triggers 4\"]")).GetCssValue("width"));
            Driver.FindElement(By.CssSelector(".carousel-nav-next")).Click();
            VerifyAreEqual("488px", Driver.FindElement(By.CssSelector("[alt=\"triggers 5\"]")).GetCssValue("width"));
            Driver.FindElements(By.CssSelector(".simple-modal__footer button"))[1].Click();
            Driver.FindElement(By.CssSelector(".simple-modal__footer button")).Click();
            Thread.Sleep(500);
            VerifyIsFalse(Driver.FindElements(By.ClassName("modal-dialog")).Count > 0, "w");
            VerifyIsTrue(Driver.FindElements(By.ClassName("sidebar__group-item"))[2].FindElements(By.LinkText("Триггеры")).Count < 1, "Триггеры добавились в каналы продаж, хотя не должны");
            Driver.FindElement(By.CssSelector(".sidebar__group-item ui-modal-trigger")).Click();
            Driver.WaitForModal();
            Actions action = new Actions(Driver);
            action.MoveToElement(Driver.FindElement(By.ClassName("close")), 200, 200).Click();
            action.Perform();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElements(By.ClassName("modal-dialog")).Count > 0, "e");
            //T003
            Driver.FindElement(By.CssSelector(".sidebar__group-item ui-modal-trigger")).Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".card-channel"))[0].Click();
            Driver.FindElement(By.CssSelector(".simple-modal__footer .btn-success")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("TriggerAddModal"));
            Driver.FindElement(AdvBy.DataE2E("TriggerAddModal")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            Driver.WaitForElem(By.ClassName("relative"));
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("sidebar__group-item"))[2].FindElements(By.LinkText("Триггеры")).Count, "Триггеры добавились в каналы продаж, все ок");
            VerifyIsTrue(Driver.FindElements(By.TagName("h1"))[0].Text.Contains("Триггерный маркетинг"), "r");
            VerifyIsTrue(Driver.FindElements(By.TagName("h1"))[1].Text.Contains("Новый заказ"), "t");
            VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".ibox-content.border_none")).Count);
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".ibox-content.border_none .adv-checkbox-label")).Count);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ibox .clearfix"))[0].Text.Contains("Выберите условия срабатывания действий"), "a");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ibox .clearfix"))[1].Text.Contains("Выберите действие"), "b");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ibox .uib-tab"))[0].Text.Contains("Отправить Email"), "c");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ibox .uib-tab"))[1].Text.Contains("Отправить SMS"), "d");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ibox .uib-tab"))[2].Text.Contains("Изменить данные"), "e");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ibox .uib-tab"))[3].Text.Contains("Отправить HTTP-запрос"), "f");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".form-group .col-xs-2"))[0].Text.Contains("Тема письма"), "n");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".form-group .col-xs-2"))[1].Text.Contains("Текст письма"), "j");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".form-group .col-xs-2"))[2].Text.Contains("Генерация купона"), "y");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".form-group .col-xs-2"))[3].Text.Contains("Время срабатывания"), "k");
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".form-group .textcomplete-wrapper")).Count);
            Driver.FindElements(By.CssSelector(".ibox .uib-tab"))[1].Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".col-xs-12.m-b-md")).Text.Contains("Для отправки СМС необходимо настроить"), "p");
            Driver.FindElements(By.CssSelector(".ibox .uib-tab"))[2].Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".form-group .col-xs-2"))[0].Text.Contains("Поле"), "z");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".form-group .col-xs-2"))[1].Text.Contains("Новое значение"), "x");
            VerifyAreEqual(1, Driver.FindElements(AdvBy.DataE2E("EditField")).Count);
            Driver.FindElements(By.CssSelector(".ibox .uib-tab"))[3].Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".form-group .m-b-xs"))[0].Text.Contains("Метод"), "h");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".form-group .m-b-xs"))[1].Text.Contains("URL"), "u");
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".action-item select")).Count);
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".action-item input")).Count);
            VerifyFinally(TestName);
        }
    }

    [TestFixture]
    public class TriggersTest: BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            Init();
            GoToAdmin();
            Driver.WaitForElem(By.CssSelector(".sidebar__group-item ui-modal-trigger"));
            Driver.FindElement(By.CssSelector(".sidebar__group-item ui-modal-trigger")).Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".card-channel"))[0].Click();
            Driver.FindElement(By.CssSelector(".simple-modal__footer .btn-success")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("TriggerAddModal"));
            Thread.Sleep(100);
            Driver.FindElement(AdvBy.DataE2E("TriggerAddModal")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            GoToAdmin("triggers");
            Driver.WaitForElem(By.CssSelector("[index=\"'triggers'\"]"));
        }

        /*[TearDown]
        public void EndTest()
        {
            driver.FindElement(By.CssSelector(".sidebar__group-item ui-modal-trigger")).Click();
            Driver.WaitForModal();
            Driver.ScrollTo(By.CssSelector(".modal-dialog button"), 10);
            driver.FindElements(By.CssSelector(".modal-dialog button"))[11].Click();
            driver.FindElement(By.CssSelector(".simple-modal__footer .btn-success")).Click();
            Driver.WaitForElem(By.ClassName("swal2-header"));
            driver.FindElement(By.CssSelector(".swal2-actions .btn-success")).Click();
        }*/

        [Test]
        [Order(1)]
        public void PageTriggers()
        {
            TestName = "PageTriggers";
            VerifyBegin(TestName);
            //T004
            GoToAdmin("triggers");
            VerifyAreEqual("Название", Driver.GetGridCell(-1, "Name", "Triggers").Text);
            VerifyAreEqual("Описание", Driver.GetGridCell(-1, "Description", "Triggers").Text);
            VerifyAreEqual("Категория", Driver.GetGridCell(-1, "CategoryName", "Triggers").Text);
            VerifyAreEqual("Актив.", Driver.GetGridCell(-1, "Enabled", "Triggers").Text);
            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "Name", "Triggers").Text);
            VerifyAreEqual("Триггер на новый заказ", Driver.GetGridCell(0, "Description", "Triggers").Text);
            VerifyAreEqual("Общая", Driver.GetGridCell(0, "CategoryName", "Triggers").Text);
            VerifyAreEqual("true", Driver.GetGridCell(0, "Enabled", "Triggers").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"));
            VerifyAreEqual("Найдено записей: 1", Driver.FindElements(By.CssSelector(".input-group .ui-grid-custom-filter-total"))[1].Text);
            //T005->T006
            Driver.FindElement(By.CssSelector("[index=\"'categories'\"]")).Click();
            VerifyAreEqual("Название", Driver.GetGridCell(-1, "Name").Text);
            VerifyAreEqual("Порядок", Driver.GetGridCell(-1, "SortOrder").Text);
            VerifyAreEqual("Найдено записей: 0", Driver.FindElements(By.CssSelector(".input-group .ui-grid-custom-filter-total"))[0].Text);
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text);
            VerifyAreEqual("Найдено категорий: 0", Driver.FindElement(By.CssSelector("ui-grid-custom-footer")).Text);
            Driver.FindElement(AdvBy.DataE2E("AddCategory")).Click();
            Driver.FindElement(AdvBy.DataE2E("categoryName")).SendKeys("Новая категория");
            Driver.FindElement(AdvBy.DataE2E("categoryButtonSave")).Click();
            VerifyAreEqual("Найдено записей: 1", Driver.FindElements(By.CssSelector(".input-group .ui-grid-custom-filter-total"))[0].Text);
            VerifyAreEqual("Новая категория", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Найдено категорий: 1", Driver.FindElement(By.CssSelector(".ui-grid-custom-footer")).Text);
            //T007
            Driver.FindElement(AdvBy.DataE2E("AddTrigger")).Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[value=\"number:3\"]")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[1].FindElement(By.CssSelector("[value=\"string:1\"]")).Click();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            Driver.WaitForElem(By.CssSelector(".sticky-hidden a"));
            Driver.FindElement(By.CssSelector(".sticky-hidden a")).Click();
            Driver.FindElement(By.CssSelector("[index=\"'triggers'\"]")).Click();
            VerifyAreEqual("Новый лид", Driver.GetGridCell(1, "Name", "Triggers").Text);
            VerifyAreEqual("Триггер на новый лид", Driver.GetGridCell(1, "Description", "Triggers").Text);
            VerifyAreEqual("Новая категория", Driver.GetGridCell(1, "CategoryName", "Triggers").Text);
            VerifyAreEqual("true", Driver.GetGridCell(1, "Enabled", "Triggers").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"));
            VerifyAreEqual("Найдено записей: 2", Driver.FindElements(By.CssSelector(".input-group .ui-grid-custom-filter-total"))[1].Text);
            //T008
            Driver.FindElements(By.CssSelector("ui-grid-custom-delete"))[1].Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Найдено записей: 1", Driver.FindElements(By.CssSelector(".input-group .ui-grid-custom-filter-total"))[1].Text);
            //T009
            Driver.GetGridCell(0, "_serviceColumn", "Triggers").FindElements(By.TagName("a"))[0].Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Триггерный маркетинг"));
            VerifyIsTrue(Driver.FindElements(By.TagName("h1"))[1].Text.Contains("Новый заказ"));
            Driver.FindElement(By.CssSelector(".sticky-hidden a")).Click();
            //T010
            Driver.FindElement(By.CssSelector("[index=\"'categories'\"]")).Click();
            Driver.GetGridCell(0, "_serviceColumn").FindElements(By.TagName("a"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".btn-cancel")).Click();
            //T011
            Driver.GetGridCell(0, "_serviceColumn").FindElements(By.TagName("a"))[1].Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text);
            VerifyFinally(TestName);
        }

        [Test]
        [Order(2)]
        public void SortingTriggers()
        {
            TestName = "SortingTriggers";
            VerifyBegin(TestName);
            GoToAdmin("triggers");
            Driver.FindElement(By.CssSelector("[index=\"'categories'\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("AddCategory")).Click();
            Driver.WaitForModal();
            Driver.FindElement(AdvBy.DataE2E("categoryName")).SendKeys("Новая категория");
            Driver.FindElement(AdvBy.DataE2E("categoryButtonSave")).Click();
            Driver.FindElement(AdvBy.DataE2E("AddCategory")).Click();
            Driver.FindElement(AdvBy.DataE2E("categoryName")).SendKeys("Высшая категория");
            Driver.FindElement(AdvBy.DataE2E("categorySortOrder")).Clear();
            Driver.FindElement(AdvBy.DataE2E("categorySortOrder")).SendKeys("1");
            Driver.FindElement(AdvBy.DataE2E("categoryButtonSave")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[index=\"'triggers'\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("AddTrigger")).Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[value=\"number:2\"]")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[value=\"string:2\"]")).Click();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".sticky-hidden a")).Click();
            Driver.FindElements(AdvBy.DataE2E("switchOnOffLabel"))[1].Click();
            //T013
            Driver.GetGridCell(-1, "Name", "Triggers").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "Name", "Triggers").Text);
            VerifyAreEqual("Смена статуса заказа на Новый", Driver.GetGridCell(1, "Name", "Triggers").Text);
            Driver.GetGridCell(-1, "Name", "Triggers").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Смена статуса заказа на Новый", Driver.GetGridCell(0, "Name", "Triggers").Text);
            VerifyAreEqual("Новый заказ", Driver.GetGridCell(1, "Name", "Triggers").Text);
            Driver.GetGridCell(-1, "Name", "Triggers").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "Name", "Triggers").Text);
            VerifyAreEqual("Смена статуса заказа на Новый", Driver.GetGridCell(1, "Name", "Triggers").Text);
            //T014 - смысла сортировки нет, т.к. там все с одной буквы
            //T015
            Driver.GetGridCell(-1, "CategoryName", "Triggers").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Новая категория", Driver.GetGridCell(0, "CategoryName", "Triggers").Text);
            VerifyAreEqual("Общая", Driver.GetGridCell(1, "CategoryName", "Triggers").Text);
            Driver.GetGridCell(-1, "CategoryName", "Triggers").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Общая", Driver.GetGridCell(0, "CategoryName", "Triggers").Text);
            VerifyAreEqual("Новая категория", Driver.GetGridCell(1, "CategoryName", "Triggers").Text);
            Driver.GetGridCell(-1, "CategoryName", "Triggers").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Общая", Driver.GetGridCell(0, "CategoryName", "Triggers").Text);
            VerifyAreEqual("Новая категория", Driver.GetGridCell(1, "CategoryName", "Triggers").Text);
            //T016
            Driver.GetGridCell(-1, "Enabled", "Triggers").Click();
            Thread.Sleep(500);
            VerifyAreEqual("true", Driver.GetGridCell(0, "Enabled", "Triggers").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"));
            VerifyAreEqual("false", Driver.GetGridCell(1, "Enabled", "Triggers").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"));
            Driver.GetGridCell(-1, "Enabled", "Triggers").Click();
            Thread.Sleep(500);
            VerifyAreEqual("false", Driver.GetGridCell(0, "Enabled", "Triggers").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"));
            VerifyAreEqual("true", Driver.GetGridCell(1, "Enabled", "Triggers").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"));
            Driver.GetGridCell(-1, "Enabled", "Triggers").Click();
            Thread.Sleep(500);
            VerifyAreEqual("true", Driver.GetGridCell(0, "Enabled", "Triggers").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"));
            VerifyAreEqual("false", Driver.GetGridCell(1, "Enabled", "Triggers").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"));
            //T017
            Driver.FindElement(By.CssSelector("[index=\"'categories'\"]")).Click();
            Driver.GetGridCell(-1, "Name").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Высшая категория", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Новая категория", Driver.GetGridCell(1, "Name").Text);
            Driver.GetGridCell(-1, "Name").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Новая категория", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Высшая категория", Driver.GetGridCell(1, "Name").Text);
            Driver.GetGridCell(-1, "Name").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Новая категория", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Высшая категория", Driver.GetGridCell(1, "Name").Text);
            //T018
            Driver.GetGridCell(-1, "SortOrder").Click();
            Thread.Sleep(500);
            VerifyAreEqual("0", Driver.GetGridCell(0, "SortOrder").Text);
            VerifyAreEqual("1", Driver.GetGridCell(1, "SortOrder").Text);
            Driver.GetGridCell(-1, "SortOrder").Click();
            Thread.Sleep(500);
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder").Text);
            VerifyAreEqual("0", Driver.GetGridCell(1, "SortOrder").Text);
            Driver.GetGridCell(-1, "SortOrder").Click();
            Thread.Sleep(500);
            VerifyAreEqual("0", Driver.GetGridCell(0, "SortOrder").Text);
            VerifyAreEqual("1", Driver.GetGridCell(1, "SortOrder").Text);
            VerifyFinally(TestName);
        }

        [Test]
        [Order(0)]
        public void EditingTriggers()
        {
            TestName = "EditingTriggers";
            VerifyBegin(TestName);
            //T019->T020 (в SetUp есть)
            Driver.GetGridCell(0, "Name", "Triggers").Click();
            Driver.WaitForElem(By.TagName("trigger-edit"));
            //T021
            Driver.FindElement(By.CssSelector("[data-e2e=\"TriggerWorksOnlyOnce\"]~span")).Click();
            Driver.FindElement(AdvBy.DataE2E("TriggerSave")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.CheckBoxChecked(AdvBy.DataE2E("TriggerWorksOnlyOnce")), "checkbox is checked");
            //T022
            Driver.FindElement(By.CssSelector(".clearfix button")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.ClassName("close")).Click();
            Thread.Sleep(500);
            VerifyIsFalse(Driver.FindElements(By.ClassName("modal-dialog")).Count > 0);
            Driver.FindElement(By.CssSelector(".clearfix button")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".modal-footer .btn-cancel")).Click();
            Thread.Sleep(500);
            VerifyIsFalse(Driver.FindElements(By.ClassName("modal-dialog")).Count > 0);
            //T023->T025
            //проверка создания условия "Фамилия"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Фамилия\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).SendKeys("Иванов");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Фамилия" ,Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Иванов", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //проверка возможности редактирования условия
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[1].FindElement(By.CssSelector("[value=\"number:1\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            //отредактировали, проверяем отображение "не равно"
            Driver.WaitForElem(AdvBy.DataE2E("RuleFilter"));
            VerifyAreEqual("не равно Иванов", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //проверка удаления условия
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Имя"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Имя\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).SendKeys("Иван");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Имя", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Иван", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Отчество"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Отчество\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).SendKeys("Иванович");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Отчество", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Иванович", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Телефон"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Телефон\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).SendKeys("+79854759234");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Телефон", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= +79854759234", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Email"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Email\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).SendKeys("mymail123@gmail.com");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Email", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= mymail123@gmail.com", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Страна"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Страна\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).SendKeys("Россия");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Страна", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Россия", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Регион"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Регион\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).SendKeys("Самарская область");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Регион", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Самарская область", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Город"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Город\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).SendKeys("Самара");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Город", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Самара", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Организация"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Организация\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).SendKeys("Старбакс");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Организация", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Старбакс", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Менеджер"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Менеджер\"]")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Администратор Магазина\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Менеджер", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Администратор Магазина", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "Группа покупателя"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Группа покупателя\"]")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Обычный покупатель\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Группа покупателя", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Обычный покупатель", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Дилер\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Дилер", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Постоянный клиент\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Постоянный клиент", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Представитель\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Представитель", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Оптовик\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Оптовик", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Сегмент покупателей"
            /*driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Сегмент покупателей\"]")).Click();
            driver.FindElement(AdvBy.DataE2e("BizRuleParamValue")).Click();
            driver.FindElement(AdvBy.DataE2e("BizRuleParamValue")).SendKeys("");
            driver.FindElement(AdvBy.DataE2e("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Сегмент покупателей", driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2e("RuleFilterName")).Text);
            VerifyAreEqual("= ", driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2e("RuleFilter")).Text);
            driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2e("RuleFilterDelete")).Click();
            VerifyIsFalse(driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2e("RuleFilterName")).Count > 0);*/
            //проверка создания и удаления условия "Сумма оплаченных заказов"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Сумма оплаченных заказов\"]")).Click();
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[0].Click();
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[0].SendKeys("100");
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[1].Click();
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[1].SendKeys("1000");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Сумма оплаченных заказов", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("от 100 до 1000", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Кол-во оформленных заказов"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Кол-во оформленных заказов\"]")).Click();
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[0].Click();
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[0].SendKeys("1");
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[1].Click();
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[1].SendKeys("10");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Кол-во оформленных заказов", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("от 1 до 10", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Кол-во оплаченных заказов"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Кол-во оплаченных заказов\"]")).Click();
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[0].Click();
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[0].SendKeys("1");
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[1].Click();
            Driver.FindElements(By.CssSelector("[type=\"number\"]"))[1].SendKeys("10");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Кол-во оплаченных заказов", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("от 1 до 10", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Товары купленные покупателем"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Товары купленные покупателем\"]")).Click();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-success")).Click();
            Driver.FindElements(AdvBy.DataE2E("gridCheckboxWrapSelect"))[0].Click();
            Driver.FindElements(AdvBy.DataE2E("gridCheckboxWrapSelect"))[3].Click();
            Driver.FindElements(By.CssSelector(".btn-save"))[0].Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Товары купленные покупателем", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text.Contains("Рубашка"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text.Contains("Шорты"));
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Товары купленные по категориям"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Товары купленные по категориям\"]")).Click();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-success")).Click();
            Driver.FindElement(By.Id("5701")).Click();
            Driver.FindElements(By.CssSelector(".btn-save"))[0].Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Товары купленные по категориям", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Категория 1", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "Наличие открытых лидов в списке"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Наличие открытых лидов в списке\"]")).Click();
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".modal-dialog select"))[3].FindElements(By.TagName("option")).Count);
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Лиды\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Наличие открытых лидов в списке", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Лиды Любой", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[3].FindElement(By.CssSelector("[label=\"Новый\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Лиды Новый", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[3].FindElement(By.CssSelector("[label=\"Созвон с клиентом\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Лиды Созвон с клиентом", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[3].FindElement(By.CssSelector("[label=\"Выставление КП\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Лиды Выставление КП", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[3].FindElement(By.CssSelector("[label=\"Ожидание решения клиента\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Лиды Ожидание решения клиента", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[3].FindElement(By.CssSelector("[label=\"Сделка заключена\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Лиды Сделка заключена", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[3].FindElement(By.CssSelector("[label=\"Сделка отклонена\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Лиды Сделка отклонена", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Менеджер"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Менеджер\"]")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Администратор Магазина\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Менеджер", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Администратор Магазина", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Сумма заказа"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Сумма заказа\"]")).Click();
            //указание точного значения
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValue")).SendKeys("1000");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Сумма заказа", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= 1000", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            //указание диапазона значений
            Driver.FindElement(AdvBy.DataE2E("BizRuleRange")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueFrom")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueFrom")).SendKeys("1");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueTo")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueTo")).SendKeys("1000");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("от 1 до 1000", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Заказ содержит товары"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Заказ содержит товары\"]")).Click();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-success")).Click();
            Driver.FindElements(AdvBy.DataE2E("gridCheckboxWrapSelect"))[2].Click();
            Driver.FindElements(By.CssSelector(".btn-save"))[0].Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Заказ содержит товары", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Платье", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "Заказ содержит товары из категорий"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Заказ содержит товары из категорий\"]")).Click();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-success")).Click();
            Driver.FindElement(By.Id("5701")).Click();
            Driver.FindElements(By.CssSelector(".btn-save"))[0].Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Заказ содержит товары из категорий", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Категория 1", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "Заказ содержит подарочный сертификат"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Заказ содержит подарочный сертификат\"]")).Click();
            //установка значения Да
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Да\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Заказ содержит подарочный сертификат", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Да", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Нет
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Нет\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Нет", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "Источник заказа"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Источник заказа\"]")).Click();
            //установка значения Корзина интернет магазина
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Корзина интернет магазина\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Источник заказа", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Корзина интернет магазина", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Оффлайн
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Оффлайн\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Оффлайн", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения В один клик
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"В один клик\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= В один клик", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Воронка продаж
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Воронка продаж\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Воронка продаж", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Мобильная версия
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Мобильная версия\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Мобильная версия", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения По телефону
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"По телефону\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= По телефону", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Онлайн консультант
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Онлайн консультант\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Онлайн консультант", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Социальные сети
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Социальные сети\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Социальные сети", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Нашли дешевле
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Нашли дешевле\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Нашли дешевле", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Брошенные корзины
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Брошенные корзины\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Брошенные корзины", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Обратный звонок
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Обратный звонок\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Обратный звонок", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Другое
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Другое\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Другое", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "Метод оплаты"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Метод оплаты\"]")).Click();
            //установка значения При получении (наличными или банковской картой)
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"При получении (наличными или банковской картой)\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Метод оплаты", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= При получении (наличными или банковской картой)", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Банковский перевод для юр. лиц
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Банковский перевод для юр. лиц\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Банковский перевод для юр. лиц", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "Метод доставки"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Метод доставки\"]")).Click();
            //установка значения Самовывоз
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Самовывоз\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Метод доставки", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Самовывоз", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Курьером
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Курьером\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Курьером", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "Оплачен"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Оплачен\"]")).Click();
            //установка значения Да
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Да\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Оплачен", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Да", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Нет
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Нет\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Нет", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "Статус заказа"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Статус заказа\"]")).Click();
            //установка значения Новый
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Новый\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Статус заказа", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Новый", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения В обработке
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"В обработке\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= В обработке", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Отправлен
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Отправлен\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Отправлен", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Доставлен
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Доставлен\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Доставлен", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Отменён
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Отменён\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Отменён", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Отменен навсегда
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Отменен навсегда\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Отменен навсегда", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "Создан из лида"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Создан из лида\"]")).Click();
            //установка значения Да
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Да\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Создан из лида", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Да", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Нет
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Нет\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Нет", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "Создан в части администрирования"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"Создан в части администрирования\"]")).Click();
            //установка значения Да
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Да\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("Создан в части администрирования", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("= Да", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            //установка значения Нет
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[2].FindElement(By.CssSelector("[label=\"Нет\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("= Нет", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания, редактирования и удаления условия "До определенной даты и времени"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"До определенной даты и времени\"]")).Click();
            //указание точного значения
            Driver.FindElement(By.CssSelector(".modal-dialog input")).Click();
            Driver.FindElement(By.CssSelector(".modal-dialog input")).SendKeys("222222222222");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("До определенной даты и времени", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterName")).Text);
            VerifyAreEqual("до 22.22.2222 22:22", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Click();
            //указание диапазона значений
            Driver.FindElement(By.CssSelector(".modal-dialog a")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog input"))[0].Click();
            Driver.FindElements(By.CssSelector(".modal-dialog input"))[0].SendKeys("111111111111");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("от 11.11.1111 11:11 до 22.22.2222 22:22", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //проверка создания и удаления условия "В определенное время"
            Driver.FindElements(By.CssSelector(".clearfix .pull-right"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElements(By.CssSelector(".modal-dialog select"))[0].FindElement(By.CssSelector("[label=\"В определенное время\"]")).Click();
            Driver.FindElements(By.CssSelector(".modal-dialog input"))[0].Click();
            Driver.FindElements(By.CssSelector(".modal-dialog input"))[0].SendKeys("1111");
            Driver.FindElements(By.CssSelector(".modal-dialog input"))[1].Click();
            Driver.FindElements(By.CssSelector(".modal-dialog input"))[1].SendKeys("2222");
            Driver.FindElement(AdvBy.DataE2E("BizRuleParamValueOk")).Click();
            VerifyAreEqual("от 11:11 до 22:22", Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilter")).Text);
            Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElement(AdvBy.DataE2E("RuleFilterDelete")).Click();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".ibox-content"))[2].FindElements(AdvBy.DataE2E("RuleFilterName")).Count > 0);
            //T026
            Driver.FindElements(By.CssSelector(".action-item input"))[0].Click();
            Driver.FindElements(By.CssSelector(".action-item input"))[0].SendKeys("Супер письмо");
            Driver.ScrollTo(By.CssSelector(".ibox button"), 1);
            Driver.SetCkText("Это письмо будет отправлено непонятно куда, непонятно как и непонятно зачем", By.CssSelector(".action-item"));
            Driver.FindElement(AdvBy.DataE2E("TriggerSave")).Click();
            VerifyAreEqual("Супер письмо", Driver.FindElements(By.CssSelector(".action-item input"))[0].GetAttribute("value"));
            Driver.AssertCkText("Это письмо будет отправлено непонятно куда, непонятно как и непонятно зачем", By.CssSelector(".ibox-content"), 3);
            //T027
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            Driver.FindElements(By.TagName("h1"))[1].Clear();
            Driver.FindElements(By.TagName("h1"))[1].SendKeys("Триггер 123");
            //T028
            Thread.Sleep(500);
            VerifyAreEqual("Триггер 123", Driver.FindElements(By.TagName("h1"))[1].Text);
            //возвращаем старое название триггера
            Driver.FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            Driver.FindElements(By.TagName("h1"))[1].Clear();
            Driver.FindElements(By.TagName("h1"))[1].SendKeys("Новый заказ");
            Driver.FindElement(AdvBy.DataE2E("TriggerSave")).Click();
            //T029
            Driver.FindElements(By.CssSelector(".ibox .btn-success"))[3].Click();
            Driver.FindElements(By.CssSelector(".ibox .btn-success"))[3].Click();
            VerifyAreEqual(6, Driver.FindElements(By.CssSelector(".ibox .btn-success")).Count);
            VerifyAreEqual(6, Driver.FindElements(By.CssSelector(".action-item input")).Count);
            Driver.ScrollTo(By.CssSelector(".clearfix"), 2);
            Driver.FindElements(By.CssSelector(".action-item input"))[2].Click();
            Driver.FindElements(By.CssSelector(".action-item input"))[2].SendKeys("Мега письмо");
            Driver.ScrollTo(By.CssSelector(".clearfix"), 3);
            Driver.FindElements(By.CssSelector(".action-item input"))[4].Click();
            Driver.FindElements(By.CssSelector(".action-item input"))[4].SendKeys("Турбо письмо");
            //T030
            Driver.ScrollTo(By.CssSelector(".clearfix"), 0);
            var elem1 = Driver.FindElements(By.CssSelector(".clearfix"))[1];
            Driver.MouseFocus(elem1);
            Driver.FindElements(By.CssSelector(".fa-arrow-down"))[0].Click();
            VerifyAreEqual("Мега письмо", Driver.FindElements(By.CssSelector(".action-item input"))[0].GetAttribute("value"));
            VerifyAreEqual("Супер письмо", Driver.FindElements(By.CssSelector(".action-item input"))[2].GetAttribute("value"));
            //T031
            Driver.ScrollTo(By.CssSelector(".action-item input"), 3);
            var elem2 = Driver.FindElements(By.CssSelector(".clearfix"))[3];
            Driver.MouseFocus(elem2);
            Driver.FindElements(By.CssSelector(".fa-arrow-up"))[1].Click();
            VerifyAreEqual("Турбо письмо", Driver.FindElements(By.CssSelector(".action-item input"))[2].GetAttribute("value"));
            VerifyAreEqual("Супер письмо", Driver.FindElements(By.CssSelector(".action-item input"))[4].GetAttribute("value"));
            //T032
            Driver.ScrollTo(By.CssSelector(".action-item input"), 3);
            Driver.FindElements(AdvBy.DataE2E("RemoveAction"))[1].Click();
            Driver.ScrollTo(By.CssSelector(".action-item input"), 1);
            Driver.FindElement(AdvBy.DataE2E("RemoveAction")).Click();
            VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".ibox .btn-success")).Count);
            //T033
            Driver.ScrollToTop();
            Driver.FindElements(By.CssSelector(".ibox li"))[3].Click();
            Driver.FindElement(By.CssSelector(".ibox select")).FindElement(By.CssSelector("[label=\"Город\"]")).Click();
            Driver.FindElements(By.CssSelector(".ibox input"))[3].Click();
            Driver.FindElements(By.CssSelector(".ibox input"))[3].SendKeys("Москва");
            Driver.FindElement(AdvBy.DataE2E("TriggerSave")).Click();
            VerifyAreEqual("true", Driver.FindElement(By.CssSelector(".ibox select")).FindElement(By.CssSelector("[label=\"Город\"]")).GetAttribute("selected"));
            VerifyAreEqual("Москва", Driver.FindElements(By.CssSelector(".ibox input"))[3].GetAttribute("value"));
            //T034
            Driver.FindElements(By.CssSelector(".ibox li"))[1].Click();
            Driver.ScrollTo(By.CssSelector(".ibox button"), 1);
            Driver.FindElements(By.CssSelector(".action-item input"))[0].Clear();
            Driver.FindElements(By.CssSelector(".action-item input"))[0].SendKeys("Письмецо");
            Driver.SetCkText("Очень длинное письмо", By.CssSelector(".action-item"));
            Driver.FindElement(AdvBy.DataE2E("TriggerSave")).Click();
            VerifyAreEqual("Письмецо", Driver.FindElements(By.CssSelector(".action-item input"))[0].GetAttribute("value"));
            Driver.AssertCkText("Очень длинное письмо", By.CssSelector(".ibox-content"), 3);
            Driver.FindElements(By.CssSelector(".ibox li"))[3].Click();
            Driver.FindElement(By.CssSelector(".ibox select")).FindElement(By.CssSelector("[label=\"Источник заказа\"]")).Click();
            Driver.FindElements(By.CssSelector(".ibox select"))[1].FindElement(By.CssSelector("[label=\"Мобильная версия\"]")).Click();
            Thread.Sleep(250);
            Driver.FindElement(AdvBy.DataE2E("TriggerSave")).Click();
            VerifyAreEqual("true", Driver.FindElements(By.CssSelector(".ibox select"))[0].FindElement(By.CssSelector("[label=\"Источник заказа\"]")).GetAttribute("selected"));
            VerifyAreEqual("true", Driver.FindElements(By.CssSelector(".ibox select"))[1].FindElement(By.CssSelector("[label=\"Мобильная версия\"]")).GetAttribute("selected"));
            //T035
            Driver.FindElement(By.CssSelector("[data-e2e=\"TriggerTimeDelay\"]~span")).Click();
            Driver.FindElement(By.CssSelector("[type=\"number\"]")).Clear();
            Driver.FindElement(By.CssSelector("[type=\"number\"]")).SendKeys("5");
            Driver.FindElements(By.CssSelector(".ibox select"))[2].FindElement(By.CssSelector("[label=\"В минутах\"]")).Click();
            Thread.Sleep(250);
            Driver.FindElement(AdvBy.DataE2E("TriggerSave")).Click();
            VerifyAreEqual("5", Driver.FindElement(By.CssSelector("[type=\"number\"]")).GetAttribute("value"));
            VerifyAreEqual("true", Driver.FindElements(By.CssSelector(".ibox select"))[2].FindElement(By.CssSelector("[label=\"В минутах\"]")).GetAttribute("selected"));
            //VerifyFinally(TestName);
        }
    }
}
 