using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Colors
{
    [TestFixture]
    public class ColorPaginationAndView : BaseSeleniumTest
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

        [Test]
        public void Present10Page()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");

            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color10", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Color11", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color20", Driver.GetGridCell(9, "ColorName", "Colors").Text);

            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Color21", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color30", Driver.GetGridCell(9, "ColorName", "Colors").Text);
        }

        [Test]
        public void Present10PageToNext()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");

            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color10", Driver.GetGridCell(9, "ColorName", "Colors").Text);

            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Color11", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color20", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Color21", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color30", Driver.GetGridCell(9, "ColorName", "Colors").Text);
        }

        [Test]
        public void Present10PageToPrevious()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");

            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color10", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Color11", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color20", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color10", Driver.GetGridCell(9, "ColorName", "Colors").Text);
        }

        [Test]
        public void Present10PageToEnd()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");

            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color10", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            //to end
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Color111", Driver.GetGridCell(0, "ColorName", "Colors").Text);
        }

        [Test]
        public void Present10PageToBegin()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");

            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color10", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Color11", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color20", Driver.GetGridCell(9, "ColorName", "Colors").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Color21", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color30", Driver.GetGridCell(9, "ColorName", "Colors").Text);

            //to begin
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color10", Driver.GetGridCell(9, "ColorName", "Colors").Text);
        }
    }
}