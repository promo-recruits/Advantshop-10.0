using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Colors
{
    [TestFixture]
    public class ColorEdit : BaseSeleniumTest
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
        public void EditByNameOpenWindows()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridColors[0][\'ColorName\']\"] a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            VerifyAreEqual("Color1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).GetAttribute("value"));
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).GetAttribute("value"));
            VerifyAreEqual("#000000",
                Driver.FindElement(By.CssSelector("[data-e2e=\"codColor\"] input")).GetAttribute("value"));
        }

        [Test]
        public void EditByPencilOpenWindows()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridColors[0][\'_serviceColumn\']\"] a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            VerifyAreEqual("Color1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).GetAttribute("value"));
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).GetAttribute("value"));
            VerifyAreEqual("#000000",
                Driver.FindElement(By.CssSelector("[data-e2e=\"codColor\"] input")).GetAttribute("value"));
        }

        [Test]
        public void EditCodeColor()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridColors[0][\'ColorName\']\"] a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"codColor\"] input"))
                .SendKeys(Keys.Control + "a" + Keys.Delete);
            Driver.FindElement(By.CssSelector("[data-e2e=\"codColor\"] input")).SendKeys("#ffffff");
            Driver.FindElement(By.CssSelector("[data-e2e=\"saveColor\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("color: rgb(255, 255, 255);",
                Driver.GetGridCell(0, "ColorIcon", "Colors").FindElement(By.TagName("i")).GetAttribute("style"));
        }

        [Test]
        public void EditIconColor()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridColors[0][\'ColorName\']\"] a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            AttachFile(By.CssSelector("input[data-e2e=\"iconColor\"]"), GetPicturePath("color.png"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"saveColor\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.ScrollToTop();
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            String str = Driver.GetGridCell(0, "ColorIcon", "Colors").FindElement(By.TagName("img"))
                .GetAttribute("src");
            VerifyIsTrue(str.Contains("pictures/color/details"));
        }

        [Test]
        public void EditIconDelColor()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridColors[0][\'ColorName\']\"] a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            String str = Driver.FindElement(By.CssSelector(".col-xs-9 img")).GetAttribute("src");
            VerifyIsTrue(str.Contains("pictures/color/details"));
            Driver.FindElement(By.LinkText("Удалить")).Click();

            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".col-xs-9 img")).Count);
            Driver.FindElement(By.CssSelector("[data-e2e=\"saveColor\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("color: rgb(0, 0, 0);",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridColors[0][\'ColorIcon\']\"] i"))
                    .GetAttribute("style"));
        }

        [Test]
        public void EditNameColor()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridColors[0][\'ColorName\']\"] a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).SendKeys("Edited_name");
            Driver.FindElement(By.CssSelector("[data-e2e=\"saveColor\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Edited_name", Driver.GetGridCell(0, "ColorName", "Colors").Text);
        }

        [Test]
        public void EditSortColor()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridColors[0][\'ColorName\']\"] a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).SendKeys("3");
            Driver.FindElement(By.CssSelector("[data-e2e=\"saveColor\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Color2", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Edited_name", Driver.GetGridCell(2, "ColorName", "Colors").Text);
        }

        [Test]
        public void EditColorCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridColors[0][\'ColorName\']\"] a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameColor\"]")).SendKeys("New_name_color_cancel");
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortColor\"]")).SendKeys("-5");
            Driver.FindElement(By.CssSelector("[data-e2e=\"cancelColor\"]")).Click();

            VerifyAreNotEqual("New_name_color_cancel", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color2", Driver.GetGridCell(1, "ColorName", "Colors").Text);
        }
    }
}