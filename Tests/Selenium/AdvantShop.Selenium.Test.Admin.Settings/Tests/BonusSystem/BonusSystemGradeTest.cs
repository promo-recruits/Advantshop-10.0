using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem
{
    [TestFixture]
    public class BonusSystemGradeTest : BaseSeleniumTest
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
        public void CheckGradeGrid()
        {
            GoToAdmin("grades");
            VerifyIsFalse(Is404Page(""), " 404 error");
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Бонусная программа"), "h1 page edit");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".balance-block h2")).Text.Contains("Грейды"),
                "h1 grade edit");

            VerifyAreEqual("Гостевой", Driver.GetGridCell(0, "Name").Text, "grade name 1");
            VerifyAreEqual("Бронзовый", Driver.GetGridCell(1, "Name").Text, "grade name 2");
            VerifyAreEqual("Серебряный", Driver.GetGridCell(2, "Name").Text, "grade name 3");
            VerifyAreEqual("Золотой", Driver.GetGridCell(3, "Name").Text, "grade name 4");
            VerifyAreEqual("Платиновый", Driver.GetGridCell(4, "Name").Text, "grade name 4");

            VerifyAreEqual("3", Driver.GetGridCell(0, "BonusPercent").Text, "grade BonusPercent 1");
            VerifyAreEqual("5", Driver.GetGridCell(1, "BonusPercent").Text, "grade BonusPercent 2");
            VerifyAreEqual("7", Driver.GetGridCell(2, "BonusPercent").Text, "grade BonusPercent 3");
            VerifyAreEqual("10", Driver.GetGridCell(3, "BonusPercent").Text, "grade BonusPercent 4");
            VerifyAreEqual("30", Driver.GetGridCell(4, "BonusPercent").Text, "grade BonusPercent 4");


            VerifyAreEqual("0", Driver.GetGridCell(0, "PurchaseBarrier").Text, "grade PurchaseBarrier 1");
            VerifyAreEqual("5000", Driver.GetGridCell(1, "PurchaseBarrier").Text, "grade PurchaseBarrier 2");
            VerifyAreEqual("15000", Driver.GetGridCell(2, "PurchaseBarrier").Text, "grade PurchaseBarrier 3");
            VerifyAreEqual("25000", Driver.GetGridCell(3, "PurchaseBarrier").Text, "grade PurchaseBarrier 4");
            VerifyAreEqual("50000", Driver.GetGridCell(4, "PurchaseBarrier").Text, "grade PurchaseBarrier 4");

            VerifyAreEqual("0", Driver.GetGridCell(0, "SortOrder").Text, "grade SortOrder 1");
            VerifyAreEqual("1", Driver.GetGridCell(1, "SortOrder").Text, "grade SortOrder 2");
            VerifyAreEqual("2", Driver.GetGridCell(2, "SortOrder").Text, "grade SortOrder 3");
            VerifyAreEqual("3", Driver.GetGridCell(3, "SortOrder").Text, "grade SortOrder 4");
            VerifyAreEqual("4", Driver.GetGridCell(4, "SortOrder").Text, "grade SortOrder 4");
        }

        [Test]
        public void GradeGridSortTest()
        {
            GoToAdmin("grades");
            Driver.GetGridCell(-1, "Name").Click();
            VerifyAreEqual("Бронзовый", Driver.GetGridCell(0, "Name").Text, "grade sort by name 1");
            VerifyAreEqual("Гостевой", Driver.GetGridCell(1, "Name").Text, "grade sort by name 2");

            Driver.GetGridCell(-1, "Name").Click();
            VerifyAreEqual("Серебряный", Driver.GetGridCell(0, "Name").Text, "grade 2 sort by name 1");
            VerifyAreEqual("Платиновый", Driver.GetGridCell(1, "Name").Text, "grade 2 sort by name 2");

            Driver.GetGridCell(-1, "BonusPercent").Click();
            VerifyAreEqual("Гостевой", Driver.GetGridCell(0, "Name").Text, "grade sort by BonusPercent 1");
            VerifyAreEqual("Бронзовый", Driver.GetGridCell(1, "Name").Text, "grade sort by BonusPercent 2");

            Driver.GetGridCell(-1, "BonusPercent").Click();
            VerifyAreEqual("Платиновый", Driver.GetGridCell(0, "Name").Text, "grade 2 sort by BonusPercent 1");
            VerifyAreEqual("Золотой", Driver.GetGridCell(1, "Name").Text, "grade 2 sort by BonusPercent 2");

            Driver.GetGridCell(-1, "PurchaseBarrier").Click();
            VerifyAreEqual("Гостевой", Driver.GetGridCell(0, "Name").Text, "grade sort by PurchaseBarrier 1");
            VerifyAreEqual("Бронзовый", Driver.GetGridCell(1, "Name").Text, "grade sort by PurchaseBarrier 2");

            Driver.GetGridCell(-1, "PurchaseBarrier").Click();
            VerifyAreEqual("Платиновый", Driver.GetGridCell(0, "Name").Text, "grade 2 sort by PurchaseBarrier 1");
            VerifyAreEqual("Золотой", Driver.GetGridCell(1, "Name").Text, "grade 2 sort by PurchaseBarrier 2");

            Driver.GetGridCell(-1, "SortOrder").Click();
            VerifyAreEqual("Гостевой", Driver.GetGridCell(0, "Name").Text, "grade sort by SortOrder 1");
            VerifyAreEqual("Бронзовый", Driver.GetGridCell(1, "Name").Text, "grade sort by SortOrder 2");

            Driver.GetGridCell(-1, "SortOrder").Click();
            VerifyAreEqual("Платиновый", Driver.GetGridCell(0, "Name").Text, "grade 2 sort by SortOrder 1");
            VerifyAreEqual("Золотой", Driver.GetGridCell(1, "Name").Text, "grade 2 sort by SortOrder 2");

            Driver.GetGridCell(-1, "SortOrder").Click();
            VerifyAreEqual("Гостевой", Driver.GetGridCell(0, "Name").Text, "grade sort by SortOrder 1");
            VerifyAreEqual("Бронзовый", Driver.GetGridCell(1, "Name").Text, "grade sort by SortOrder 2");
        }

        [Test]
        public void GradeGridSearch()
        {
            GoToAdmin("grades");
            Driver.GridFilterSendKeys("Бронзовый");
            VerifyAreEqual("Бронзовый", Driver.GetGridCell(0, "Name").Text, "search valid element");

            //search not exist product
            Driver.GridFilterSendKeys("Небронзовый");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search noexist element");

            //search too much symbols
            Driver.GridFilterSendKeys(
                    "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.DropFocus("h1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search long name element");

            //search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid simvol");
            //drop search
            /*  Driver.GridFilterSendKeys("");
                VerifyAreEqual("Гостевой", Driver.GetGridCell(0, "Name").Text, "clear search");*/
        }

        [Test]
        public void GradeGridFilter()
        {
            GoToAdmin("grades");
            Filter("Name", "Золотой");
            VerifyAreEqual("Золотой", Driver.GetGridCell(0, "Name").Text, "fillter name element");
            FilterClose("Name");

            Filter("BonusPercent", "7");
            VerifyAreEqual("Серебряный", Driver.GetGridCell(0, "Name").Text, "fillter BonusPercent element");
            FilterClose("BonusPercent");

            Filter("PurchaseBarrier", "5000");
            VerifyAreEqual("Бронзовый", Driver.GetGridCell(0, "Name").Text, "fillter PurchaseBarrier element");
            FilterClose("PurchaseBarrier");
        }


        [Test]
        public void GradeGridGoToRules()
        {
            GoToAdmin("grades");
            VerifyIsTrue(
                Driver.PageSource.Contains(
                    "Для автоматической смены грейда необходимо добавить правило \"Смена грейда\""),
                "page source rule");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"GoRules\"] a")).Click();
            Thread.Sleep(100);
            Functions.OpenNewTab(Driver, BaseUrl);
            // Driver.XPathContainsText("a", "Правила");
            VerifyIsTrue(Driver.Url.Contains("rules"), "url rules");
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Бонусная программа"), "h1 page edit");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".balance-block h2")).Text.Contains("Правила"),
                "h1 rules edit");
        }


        [Test]
        public void GradeGridzSelectAndDelTest()
        {
            GoToAdmin("settingsbonus");
            (new SelectElement(Driver.FindElement(By.Id("BonusGradeId")))).SelectByText("Платиновый");
            Driver.FindElement(By.Id("CardNumTo")).SendKeys("1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("grades");
            Driver.GridReturnDefaultView10(BaseUrl);
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Гостевой", Driver.GetGridCell(0, "Name").Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Серебряный", Driver.GetGridCell(1, "Name").Text, "1 grid delete"); //!!!!

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //доделать удаление
            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "grid");
            VerifyAreEqual("Платиновый", Driver.GetGridCell(0, "Name").Text, "selected 3 grid delete"); //!!!!!!!!

            //check select all on page
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected  page 1 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "grid");

            VerifyAreEqual("Платиновый", Driver.GetGridCell(0, "Name").Text, " no delete vip grade");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "no notes after del");
        }

        [Test]
        public void GradezAddNew()
        {
            GoToAdmin("grades");

            Driver.FindElement(By.CssSelector("[data-e2e=\"GradeAdd\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новый грейд", Driver.FindElement(By.TagName("h2")).Text, " grade title h2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeName\"]")).SendKeys("NewTestGrade");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeBonusPercent\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeBonusPercent\"]")).SendKeys("5");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gradePurchaseBarrier\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradePurchaseBarrier\"]")).SendKeys("5");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeSortOrder\"]")).SendKeys("10");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeButtonSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Driver.GridFilterSendKeys("NewTestGrade");
            VerifyAreEqual("NewTestGrade", Driver.GetGridCell(0, "Name").Text, "searchnew element");
            VerifyAreEqual("5", Driver.GetGridCell(0, "BonusPercent").Text, "grade BonusPercent 5");
            VerifyAreEqual("5", Driver.GetGridCell(0, "PurchaseBarrier").Text, "grade PurchaseBarrier 5");
            VerifyAreEqual("10", Driver.GetGridCell(0, "SortOrder").Text, "grade SortOrder 10");
        }

        [Test]
        public void GradezGoToEdit()
        {
            GoToAdmin("grades");
            //go to edit
            Driver.GetGridCell(1, "Name").Click();
            Driver.WaitForModal();
            VerifyAreEqual("Грейд NewTestGrade", Driver.FindElement(By.TagName("h2")).Text, " grade title h2");
            VerifyAreEqual("NewTestGrade",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gradeName\"]")).GetAttribute("value"),
                "pop up window name grade");
            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gradeBonusPercent\"]")).GetAttribute("value"),
                "pop up window gradeBonusPercent grade");
            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gradePurchaseBarrier\"]")).GetAttribute("value"),
                "pop up window gradePurchaseBarrier grade");
            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gradeSortOrder\"]")).GetAttribute("value"),
                "pop up window gradeSortOrder grade");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeName\"]")).SendKeys("TestGrade");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeBonusPercent\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeBonusPercent\"]")).SendKeys("90");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gradePurchaseBarrier\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradePurchaseBarrier\"]")).SendKeys("100");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeSortOrder\"]")).SendKeys("-1");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gradeButtonSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("TestGrade", Driver.GetGridCell(0, "Name").Text, "grade name 1");
            VerifyAreEqual("90", Driver.GetGridCell(0, "BonusPercent").Text, "grade BonusPercent 1");
            VerifyAreEqual("100", Driver.GetGridCell(0, "PurchaseBarrier").Text, "grade PurchaseBarrier 1");
            VerifyAreEqual("-1", Driver.GetGridCell(0, "SortOrder").Text, "grade SortOrder 1");
        }

        public void Filter(string name, string date)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownItem\"][data-e2e-filter-dropdown-name=\"" +
                                              name + "\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"" +
                                                  name + "\"]")).Displayed, "display filter by " + name);
            Driver.SetGridFilterValue(name, date);
        }

        public void FilterClose(string name)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"" +
                                              name + "\"] [data-e2e=\"gridFilterItemClose\"]")).Click();
            VerifyIsFalse(Driver
                .FindElements(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"" +
                                             name + "\"]")).Count > 0);
            VerifyAreEqual("Гостевой", Driver.GetGridCell(0, "Name").Text, " close fillter");
        }
    }
}