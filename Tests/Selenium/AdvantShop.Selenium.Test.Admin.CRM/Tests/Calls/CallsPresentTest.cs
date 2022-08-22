using NUnit.Framework;
using OpenQA.Selenium;
using AdvantShop.SeleniumTest.Core;

namespace AdvantShop.Web.Site.Selenium.Test.Admin.CRM.Calls
{
    [TestFixture]
    public class CRMCallsPresentTest : BaseSeleniumTest
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
            driver.FindElement(By.CssSelector(".flatpickr-custom-wrap input")).Click();
            driver.FindElement(By.CssSelector(".flatpickr-custom-wrap input")).Clear();
            driver.FindElement(By.CssSelector(".flatpickr-custom-wrap input")).SendKeys("01.01.2015");
            DropFocus("h1");
        }

        [Test]
        public void CallsPresent10()
        {
            testname = "CRMCallsPresent10";
            VerifyBegin(testname);

            ScrollToElements(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            PageSelectItems("10");
            VerifyAreEqual("16", GetGridCell(0, "SrcNum").Text, "present line 1");
            VerifyAreEqual("7", GetGridCell(9, "SrcNum").Text, "present line 10");

            VerifyFinally(testname);
        }

        [Test]
        public void CallsPresent20()
        {
            testname = "CRMCallsresent20";
            VerifyBegin(testname);

            ScrollToElements(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            PageSelectItems("20");
            VerifyAreEqual("16", GetGridCell(0, "SrcNum").Text, "present line 1");
            VerifyAreEqual("109", GetGridCell(19, "SrcNum").Text, "present line 20");

            VerifyFinally(testname);
        }

        [Test]
        public void CallsPresent50()
        {
            testname = "CRMCallsPresent50";
            VerifyBegin(testname);

            ScrollToElements(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            PageSelectItems("50");
            VerifyAreEqual("16", GetGridCell(0, "SrcNum").Text, "present line 1");
            VerifyAreEqual("79", GetGridCell(49, "SrcNum").Text, "present line 50");

            VerifyFinally(testname);
        }

        [Test]
        public void CallsPresent100()
        {
            testname = "CRMCallsPresent100";
            VerifyBegin(testname);

            ScrollToElements(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            PageSelectItems("100");
            VerifyAreEqual("16", GetGridCell(0, "SrcNum").Text, "present line 1");
            VerifyAreEqual("29", GetGridCell(99, "SrcNum").Text, "present line 100");

            VerifyFinally(testname);
        }
    }
}