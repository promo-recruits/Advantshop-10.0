using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Colors
{
    [TestFixture]
    public class ColorAdd : BaseSeleniumTest
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

            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GetGridCell(-1, "SortOrder", "Colors").Click();
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
        public void AddNameColor()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"ColorSettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);

            Driver.FindElement(By.CssSelector("[data-e2e=\"codColor\"] input"))
                .SendKeys(Keys.Control + "a" + Keys.Delete);
            Driver.FindElement(By.CssSelector("[data-e2e=\"codColor\"] input")).SendKeys("#180000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).SendKeys("New_name_color");
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).SendKeys("-2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"codColor\"] input")).Click();
            Driver.DropFocus("h2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"saveColor\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("New_name_color", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("color: rgb(24, 0, 0);",
                Driver.GetGridCell(0, "ColorIcon", "Colors").FindElement(By.TagName("i")).GetAttribute("style"));
        }

        [Test]
        public void AddIconColor()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"ColorSettingAdd\"]")).Click();
            Driver.WaitForModal();

            AttachFile(By.CssSelector("input[data-e2e=\"iconColor\"]"), GetPicturePath("color.png"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).SendKeys("New_name_color_icon");
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).SendKeys("-1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"saveColor\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("New_name_color_icon", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            string str = Driver.GetGridCell(0, "ColorIcon", "Colors").FindElement(By.TagName("img"))
                .GetAttribute("src");
            VerifyIsTrue(str.Contains("pictures/color/details"));
        }

        [Test]
        public void AddNewColorCancel()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"ColorSettingAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).SendKeys("New_name_color_cancel");
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).SendKeys("-5");
            Driver.FindElement(By.CssSelector("[data-e2e=\"cancelColor\"]")).Click();
            VerifyAreNotEqual("New_name_color_cancel", Driver.GetGridCell(0, "ColorName", "Colors").Text);
        }
        //Mozila browser automatically fiils field cod color
        /*
        [Test]
        public void AddOnlyNameColor()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            driver.FindElement(By.CssSelector(".btn.btn-sm.btn-success.btn--margin-left")).Click();
            VerifyIsTrue(driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).SendKeys("New_onlyname_color");
            driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).Clear();
            driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).SendKeys("-6");
          
            Driver.FindElement(By.CssSelector("[data-e2e=\"saveColor\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyIsTrue(driver.FindElement(By.CssSelector(".toast-container")).Displayed);
            VerifyIsTrue(driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            driver.FindElement(By.ClassName("close")).Click();
            Driver.GetGridCell(-1, "SortOrder").Click();
            VerifyAreEqual("New_name_color", Driver.GetGridCell(0, "ColorName").Text);
        }
        */
    }
}