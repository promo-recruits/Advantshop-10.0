using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Colors
{
    [TestFixture]
    public class ColorMainPage : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Color\\Catalog.Color.csv"
            );


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

        [Order(0)]
        [Test]
        public void OpenColorWindows()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            VerifyAreEqual("Цвета", Driver.FindElement(By.CssSelector("[data-e2e=\"ColorSettingTitle\"]")).Text);
            VerifyAreEqual("color: rgb(0, 0, 0);",
                Driver.GetGridCell(0, "ColorIcon", "Colors").FindElement(By.TagName("i")).GetAttribute("style"));
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Colors").Text);
        }

        [Order(1)]
        [Test]
        public void SearchColor()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridFilterSendKeys("Color11");
            VerifyAreEqual("Color11", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color110", Driver.GetGridCell(1, "ColorName", "Colors").Text);
            VerifyAreEqual("Color111", Driver.GetGridCell(2, "ColorName", "Colors").Text);
            VerifyAreEqual(3,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
        }

        [Order(1)]
        [Test]
        public void SearchNotExistColor()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridFilterSendKeys("Color1111");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Order(1)]
        [Test]
        public void SearchLongNameColor()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridFilterSendKeys(
                    "qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Order(1)]
        [Test]
        public void SearchInvalidSimbolColor()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridFilterSendKeys("123!@##$%5423");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Order(10)]
        [Test]
        public void InplaceEdit()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.SendKeysGridCell("3", 0, "SortOrder", "Colors");
            Refresh();
            VerifyAreEqual("Color2", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color1", Driver.GetGridCell(1, "ColorName", "Colors").Text);
            VerifyAreEqual("Color3", Driver.GetGridCell(2, "ColorName", "Colors").Text);
        }

        [Order(1)]
        [Test]
        public void SortByName()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GetGridCell(-1, "ColorName", "Colors").Click();
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color107", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            VerifyAreEqual(10,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);

            Driver.GetGridCell(-1, "ColorName", "Colors").Click();
            VerifyAreEqual("Color99", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color90", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            VerifyAreEqual(10,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);

            Driver.GetGridCell(-1, "ColorName", "Colors").Click();
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color10", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            VerifyAreEqual(10,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
        }

        [Order(1)]
        [Test]
        public void SortBySortOrder()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");

            Driver.GetGridCell(-1, "SortOrder", "Colors").Click();
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color10", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            VerifyAreEqual(10,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);

            Driver.GetGridCell(-1, "SortOrder", "Colors").Click();
            VerifyAreEqual("Color111", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color102", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            VerifyAreEqual(10,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            Driver.GetGridCell(-1, "SortOrder", "Colors").Click();
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color10", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            VerifyAreEqual(10,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
        }
    }
}