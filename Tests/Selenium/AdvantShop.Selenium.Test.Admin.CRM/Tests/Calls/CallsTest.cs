using System;
using NUnit.Framework;
using OpenQA.Selenium;
using AdvantShop.SeleniumTest.Core;
using System.Threading;

namespace AdvantShop.Web.Site.Selenium.Test.Admin.CRM.Calls
{
    [TestFixture]
    public class CRMCallsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM);
            InitializeService.LoadData(
               "data\\Admin\\CRM\\Calls\\Customers.Call.csv"
           );

            Init();
            GoToAdmin("analytics#?analyticsReportTab=telephony&telephonyTab=сallLog");
       /*     driver.FindElement(By.CssSelector(".flatpickr-custom-wrap input")).Click();
            driver.FindElement(By.CssSelector(".flatpickr-custom-wrap input")).Clear();
            driver.FindElement(By.CssSelector(".flatpickr-custom-wrap input")).SendKeys("01.01.2015");
            DropFocus("h1");*/
        }

        [Test]
        public void Calls()
        {
            testname = "CRMCalls";
            VerifyBegin(testname);

            VerifyIsTrue(driver.FindElement(By.TagName("h1")).Text.Contains("Журнал вызовов"), "call h1 grid");

            VerifyAreEqual("Исходящий", GetGridCell(0, "Type").FindElement(By.TagName("span")).GetAttribute("title"), "call Type");
            VerifyAreEqual("01.01.2017 00:00", GetGridCell(0, "CallDateFormatted").Text, "call CallDate");
            VerifyAreEqual("16", GetGridCell(0, "SrcNum").Text, "call from");
            VerifyAreEqual("95", GetGridCell(0, "DstNum").Text, "call to");
            VerifyAreEqual("26", GetGridCell(0, "Extension").Text, "call Extension");
            VerifyAreEqual("16 с", GetGridCell(0, "DurationFormatted").Text, "call Duration");

            VerifyAreEqual("Найдено записей: 110", driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            VerifyFinally(testname);
        }

        [Test]
        public void CallsSelectDelete()
        {
            testname = "CRMCallsSelectDelete";
            VerifyBegin(testname);
            
            //check delete cancel 
            GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            driver.FindElement(By.ClassName("swal2-cancel")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("16", GetGridCell(0, "SrcNum").Text, "1 grid canсel delete");

            //check delete
            GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            driver.FindElement(By.ClassName("swal2-confirm")).Click();
            Thread.Sleep(2000);
            VerifyAreNotEqual("16", GetGridCell(0, "SrcNum").Text, "1 grid delete");

            //check select 
            GetGridCell(0, "selectionRowHeaderCol").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            GetGridCell(1, "selectionRowHeaderCol").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            GetGridCell(2, "selectionRowHeaderCol").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(GetGridCell(0, "selectionRowHeaderCol").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(GetGridCell(1, "selectionRowHeaderCol").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(GetGridCell(2, "selectionRowHeaderCol").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3", driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text, "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(driver, baseURL);
            VerifyAreEqual("15", GetGridCell(0, "SrcNum").Text, "selected 2 grid delete");
            VerifyAreEqual("10", GetGridCell(1, "SrcNum").Text, "selected 3 grid delete");
            VerifyAreEqual("11", GetGridCell(2, "SrcNum").Text, "selected 4 grid delete");

            //check select all on page
            driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(GetGridCell(0, "selectionRowHeaderCol").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected all on page 1 grid");
            VerifyIsTrue(GetGridCell(9, "selectionRowHeaderCol").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(driver, baseURL);
            VerifyAreEqual("5", GetGridCell(0, "SrcNum").Text, "selected all on page 2 grid delete");
            VerifyAreEqual("106", GetGridCell(9, "SrcNum").Text, "selected all on page 10 grid delete");

            //check select all
            driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("96", driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text, "count all selected after deleting");

            //check deselect all 
            driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(!GetGridCell(0, "selectionRowHeaderCol").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(!GetGridCell(9, "selectionRowHeaderCol").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(driver, baseURL);

            GoToAdmin("analytics#?analyticsReportTab=telephony&telephonyTab=сallLog");
            VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0", driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all after deleting");

            VerifyFinally(testname);
        }
    }
}