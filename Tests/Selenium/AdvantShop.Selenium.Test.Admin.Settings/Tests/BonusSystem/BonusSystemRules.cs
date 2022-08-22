using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem
{
    [TestFixture]
    public class BonusSystemRules : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.ProductCategories.csv"
            );
            InitializeService.BonusSystemActive();
            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CheckRulesGrid()
        {
            GoToAdmin("rules");
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Бонусная программа"), "h1 page edit");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".balance-block h2")).Text.Contains("Правила"),
                "h1 rules edit");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "no notes");
        }

        [Test]
        public void CheckzAddRuleCancelBonus()
        {
            GoToAdmin("rules");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Add\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"RuleSelect\"]")))).SelectByText(
                "Аннулирование баллов");
            Driver.FindElement(By.CssSelector("[data-e2e=\"addTemplate\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveRules"));

            VerifyAreEqual("Правило - \"Аннулирование баллов\"",
                Driver.FindElement(By.CssSelector(".balance-block h2")).Text, " grade title h1");
            VerifyIsTrue(Driver.Url.Contains("rules/edit/CancellationsBonus"), "url");
            Driver.FindElement(By.Id("Name")).SendKeys("1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Enabled\"]")).Click();
            Driver.FindElement(By.Id("AgeCard")).Clear();
            Driver.FindElement(By.Id("AgeCard")).SendKeys("12");
            Driver.FindElement(By.CssSelector("[data-e2e=\"NotSendSms\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.Id("SmsDayBefore")).Count == 0, "invisible field count day");
            Driver.FindElement(By.CssSelector("[data-e2e=\"NotSendSms\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.Id("SmsDayBefore")).Count == 1, "visible field count day");
            Driver.FindElement(By.Id("SmsDayBefore")).Clear();
            Driver.FindElement(By.Id("SmsDayBefore")).SendKeys("1");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveRules\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("rules");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "yes notes");

            VerifyAreEqual("Аннулирование баллов", Driver.GetGridCell(0, "RuleTypeStr").Text, "rule added RuleTypeStr");
            VerifyAreEqual("Аннулирование баллов1", Driver.GetGridCell(0, "Name").Text, "rule added Name");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.TagName("input")).Selected, " select enabled");

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveRules"));
            VerifyAreEqual("Аннулирование баллов1", Driver.FindElement(By.Id("Name")).GetAttribute("value"), "name");
            VerifyAreEqual("12", Driver.FindElement(By.Id("AgeCard")).GetAttribute("value"), "AgeCard");
            VerifyAreEqual("1", Driver.FindElement(By.Id("SmsDayBefore")).GetAttribute("value"), "SmsDayBefore");
            VerifyIsTrue(Driver.FindElement(By.Id("Enabled")).Selected, " select enabled");
            VerifyIsFalse(Driver.FindElement(By.Id("NotSendSms")).Selected, " select NotSendSms");
        }

        [Test]
        public void CheckzAddRuleNewCart()
        {
            GoToAdmin("rules");
            DelItem();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Add\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"RuleSelect\"]")))).SelectByText(
                "Начисление баллов при получении карты");
            Driver.FindElement(By.CssSelector("[data-e2e=\"addTemplate\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveRules"));

            VerifyAreEqual("Правило - \"Начисление баллов при получении карты\"",
                Driver.FindElement(By.CssSelector(".balance-block h2")).Text, " grade title h1");
            VerifyIsTrue(Driver.Url.Contains("rules/edit/NewCard"), "url");
            Driver.FindElement(By.Id("Name")).SendKeys("2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Enabled\"]")).Click();
            Driver.FindElement(By.Id("GiftBonus")).Clear();
            Driver.FindElement(By.Id("GiftBonus")).SendKeys("200");
            Driver.FindElement(By.Id("BonusAvailableDays")).Clear();
            Driver.FindElement(By.Id("BonusAvailableDays")).SendKeys("20");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveRules\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("rules");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "yes notes");

            VerifyAreEqual("Начисление баллов при получении карты", Driver.GetGridCell(0, "RuleTypeStr").Text,
                "rule added RuleTypeStr");
            VerifyAreEqual("Начисление баллов при получении карты2", Driver.GetGridCell(0, "Name").Text,
                "rule added Name");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.TagName("input")).Selected, " select enabled");
        }

        [Test]
        public void CheckzAddRulesChangeGrade()
        {
            GoToAdmin("rules");
            DelItem();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Add\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"RuleSelect\"]")))).SelectByText(
                "Смена грейда");
            Driver.FindElement(By.CssSelector("[data-e2e=\"addTemplate\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveRules"));

            VerifyAreEqual("Правило - \"Смена грейда\"", Driver.FindElement(By.CssSelector(".balance-block h2")).Text,
                " grade title h1");
            VerifyIsTrue(Driver.Url.Contains("rules/edit/ChangeGrade"), "url");
            Driver.FindElement(By.Id("Name")).SendKeys("3");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Enabled\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.Id("Period")).Count == 0, "invisible field count day");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AllPeriod\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.Id("Period")).Count == 1, "visible field count day");
            Driver.FindElement(By.Id("Period")).Clear();
            Driver.FindElement(By.Id("Period")).SendKeys("30");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveRules\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("rules");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "yes notes");

            VerifyAreEqual("Смена грейда", Driver.GetGridCell(0, "RuleTypeStr").Text, "rule added RuleTypeStr");
            VerifyAreEqual("Смена грейда3", Driver.GetGridCell(0, "Name").Text, "rule added Name");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.TagName("input")).Selected, " select enabled");
        }

        [Test]
        public void CheckzAddRulesCleanExpired()
        {
            GoToAdmin("rules");
            DelItem();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Add\"]")).Click();
            Driver.WaitForModal();
            // (new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"RuleSelect\"]")))).SelectByText("Правило: списание истекших бонусов");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"RuleSelect\"]")))).SelectByText(
                "Cписание истекших бонусов");
            Driver.FindElement(By.CssSelector("[data-e2e=\"addTemplate\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveRules"));

            VerifyAreEqual("Правило - \"Cписание истекших бонусов\"",
                Driver.FindElement(By.CssSelector(".balance-block h2")).Text, " grade title h1");
            VerifyIsTrue(Driver.Url.Contains("rules/edit/CleanExpiredBonus"), "url");
            Driver.FindElement(By.Id("Name")).SendKeys("4");
            VerifyIsTrue(Driver.FindElements(By.Id("DayBefore")).Count == 0, "invisible field count day");
            Driver.FindElement(By.CssSelector("[data-e2e=\"NeedSms\"]")).Click();
            Thread.Sleep(100);
            VerifyIsTrue(Driver.FindElements(By.Id("DayBefore")).Count == 1, "visible field count day");
            Driver.FindElement(By.Id("DayBefore")).Clear();
            Driver.FindElement(By.Id("DayBefore")).SendKeys("40");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveRules\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("rules");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "yes notes");

            VerifyAreEqual("Cписание истекших бонусов", Driver.GetGridCell(0, "RuleTypeStr").Text,
                "rule added RuleTypeStr");
            VerifyAreEqual("Cписание истекших бонусов4", Driver.GetGridCell(0, "Name").Text, "rule added Name");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled").FindElement(By.TagName("input")).Selected,
                " select enabled");

            Driver.GetGridCell(0, "RuleTypeStr").Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveRules"));
            VerifyAreEqual("Cписание истекших бонусов4", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "name");
            VerifyAreEqual("40", Driver.FindElement(By.Id("DayBefore")).GetAttribute("value"), "DayBefore");
            VerifyIsFalse(Driver.FindElement(By.Id("Enabled")).Selected, " select enabled");
            VerifyIsTrue(Driver.FindElement(By.Id("NeedSms")).Selected, " select NeedSms");

            Driver.FindElement(By.CssSelector("[data-e2e=\"DelRules\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "no notes");
        }

        [Test]
        public void CheckzAddRulesTwoRules()
        {
            GoToAdmin("rules");
            DelItem();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Add\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"RuleSelect\"]")))).SelectByText(
                "Аннулирование баллов");
            Driver.FindElement(By.CssSelector("[data-e2e=\"addTemplate\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveRules"));
            VerifyAreEqual("Правило - \"Аннулирование баллов\"",
                Driver.FindElement(By.CssSelector(".balance-block h2")).Text, " grade title h1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ReturnRules\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("Add"));
            VerifyIsTrue(Driver.FindElements(By.TagName("ui-grid-custom-delete")).Count == 1, "count row");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Add\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"RuleSelect\"]")))).SelectByText(
                "Аннулирование баллов");
            Driver.FindElement(By.CssSelector("[data-e2e=\"addTemplate\"]")).Click();
            Driver.WaitForToastError();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog.modal-md")).Count == 1, " window not clode");
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();
            VerifyIsTrue(Driver.FindElements(By.TagName("ui-grid-custom-delete")).Count == 1, "count row after add");
        }

        protected void DelItem()
        {
            if (Driver.FindElements(By.TagName("ui-grid-custom-delete")).Count > 0)
            {
                Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
                Driver.SwalConfirm();
                VerifyAreEqual("Ни одной записи не найдено",
                    Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text, "no notes");
            }
        }
    }
}