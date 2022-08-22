using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.Card
{
    [TestFixture]
    public class BonusSystemCardGrid : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Bonus.Card.csv"
            );
            InitializeService.BonusSystemActive();
            Init();

            GoToAdmin("cards");
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
        public void CardGrid()
        {
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Бонусная программа"), "h1 page edit");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".balance-block h2")).Text.Contains("Карты"),
                "h1 card edit");

            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "Card Number");
            VerifyAreEqual("LastName1 FirstName1", Driver.GetGridCell(0, "FIO").Text, "FIO");
            VerifyAreEqual("Гостевой", Driver.GetGridCell(0, "GradeName").Text, "GradeName");
            VerifyAreEqual("3", Driver.GetGridCell(0, "GradePersent").Text, "Grade Percent");
            VerifyAreEqual("20.04.2017 15:40", Driver.GetGridCell(0, "CreatedFormatted").Text, "card date");

            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all cards");
        }

        [Test]
        public void CardGoToEditByCardNum()
        {
            Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("numberCardBonus"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Бонусная программа"), "h1 page edit");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".balance-block h2")).Text.Contains("Карта LastName1 FirstName1"),
                "h1 card edit");
            VerifyIsTrue(Driver.Url.Contains("edit"), "url card edit");

            GoToAdmin("cards");
        }


        [Test]
        public void CardGoToEditByServiceCol()
        {
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("numberCardBonus"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Бонусная программа"), "h1 page edit");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".balance-block h2")).Text.Contains("Карта LastName1 FirstName1"),
                "h1 card edit");
            VerifyIsTrue(Driver.Url.Contains("edit"), "url lead edit");

            Driver.FindElement(By.LinkText("Все карты")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("indexBonusesAddAll"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Бонусная программа"), "h1 page edit");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".balance-block h2")).Text.Contains("Карты"),
                "h1 card edit");
        }

        [Test]
        public void CardzSelectDelete()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "1 grid delete");

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

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("530805", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "selected 2 grid delete");
            VerifyAreEqual("530806", Driver.GetGridCell(1, "CardNumber").FindElement(By.TagName("a")).Text,
                "selected 3 grid delete");
            VerifyAreEqual("530807", Driver.GetGridCell(2, "CardNumber").FindElement(By.TagName("a")).Text,
                "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("530815", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "selected all on page 2 grid delete");
            VerifyAreEqual("530824", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("106", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            GoToAdmin("cards");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");
        }
    }
}