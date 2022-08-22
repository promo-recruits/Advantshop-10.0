using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesAdd : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Properties\\Catalog.Category.csv",
                "Data\\Admin\\Properties\\Catalog.Brand.csv",
                "Data\\Admin\\Properties\\Catalog.Property.csv",
                "Data\\Admin\\Properties\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Properties\\Catalog.Product.csv",
                "Data\\Admin\\Properties\\Catalog.Offer.csv",
                "Data\\Admin\\Properties\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Properties\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\Properties\\Catalog.PropertyGroup.csv"
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
        public void _OpenWindowsAdd()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertySettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
        }

        [Test]
        public void PropertieAdd()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            //add_1
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertySettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyName\"]")).SendKeys("New_name_Check_1");
            Driver.SetCkText("New_descriptions_1", "editor1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInFilter\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInDetails\"]")).Click();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"PropertyuseInDetails\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInBrief\"]")).Click();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyType\"]")))).SelectByText(
                "Раскрывающийся список (селект)");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyGroup\"]"))))
                .SelectByText("Group1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertysortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertysortOrder\"]")).SendKeys("-3");

            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyBtnSave\"]")).Click();
            Driver.WaitForToastSuccess();
            Refresh();
            VerifyAreEqual("New_name_Check_1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Group1", Driver.GetGridCell(0, "GroupName").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "UseInFilter")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(0, "UseInDetails")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(0, "UseInBrief")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'_serviceColumn\']\"] a")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("New_name_Check_1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyName\"]")).GetAttribute("value"));
            VerifyAreEqual("New_name_Check_1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyNameClient\"]")).GetAttribute("value"));
            Driver.AssertCkText("New_descriptions_1", "editor1");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyUnits\"]")).GetAttribute("value"));
            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyType\"]"));
            SelectElement select1 = new SelectElement(selectElem1);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Раскрывающийся список (селект)"));
            IWebElement selectElem2 = Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyGroup\"]"));
            SelectElement select2 = new SelectElement(selectElem2);
            VerifyIsTrue(select2.SelectedOption.Text.Contains("Group1"));

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInFilter\"] input")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInDetails\"] input")).Selected);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInBrief\"] input")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyBtnCancel\"]")).Click();

            //add_2
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertySettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyName\"]")).SendKeys("New_name_Check_2");
            Driver.SetCkText("New_descriptions_2", "editor2");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"PropertyuseInDetails\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyUnits\"]")).SendKeys("Unit_2");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyType\"]")))).SelectByText(
                "Мультивыбор (чекбоксы)");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyGroup\"]"))))
                .SelectByText("Group2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertysortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertysortOrder\"]")).SendKeys("-2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyBtnSave\"]")).Click();
            Driver.WaitForToastSuccess();
            Refresh();

            VerifyAreEqual("New_name_Check_2", Driver.GetGridCell(1, "Name").Text);
            VerifyAreEqual("Group2", Driver.GetGridCell(1, "GroupName").Text);
            VerifyIsTrue(Driver.GetGridCell(1, "UseInFilter")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "UseInDetails")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(1, "UseInBrief")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[1][\'_serviceColumn\']\"] a")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("New_name_Check_2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyName\"]")).GetAttribute("value"));
            VerifyAreEqual("New_name_Check_2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyNameClient\"]")).GetAttribute("value"));
            Driver.AssertCkText("New_descriptions_2", "editor1");
            VerifyAreEqual("Unit_2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyUnits\"]")).GetAttribute("value"));
            IWebElement selectElem3 = Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyType\"]"));
            SelectElement select3 = new SelectElement(selectElem3);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Мультивыбор (чекбоксы)"));
            IWebElement selectElem4 = Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyGroup\"]"));
            SelectElement select4 = new SelectElement(selectElem4);
            VerifyIsTrue(select4.SelectedOption.Text.Contains("Group2"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInFilter\"] input")).Selected);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInDetails\"] input")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInBrief\"] input")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyBtnCancel\"]")).Click();

            //add_3
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertySettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyName\"]")).SendKeys("New_name_Check_3");
            Driver.SetCkText("New_descriptions_3", "editor2");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"PropertyuseInDetails\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyUnits\"]")).SendKeys("Unit_3");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyType\"]"))))
                .SelectByText("Диапазон");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyGroup\"]"))))
                .SelectByText("Group3");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInDetails\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInBrief\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertysortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertysortOrder\"]")).SendKeys("-1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyBtnSave\"]")).Click();
            Driver.WaitForToastSuccess();
            Refresh();
            VerifyAreEqual("New_name_Check_3", Driver.GetGridCell(2, "Name").Text);
            VerifyAreEqual("Group3", Driver.GetGridCell(2, "GroupName").Text);
            VerifyIsTrue(Driver.GetGridCell(2, "UseInFilter")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(2, "UseInDetails")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "UseInBrief")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[2][\'_serviceColumn\']\"] a")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("New_name_Check_3",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyName\"]")).GetAttribute("value"));
            VerifyAreEqual("New_name_Check_3",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyNameClient\"]")).GetAttribute("value"));
            Driver.AssertCkText("New_descriptions_3", "editor1");
            VerifyAreEqual("Unit_3",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyUnits\"]")).GetAttribute("value"));
            IWebElement selectElem5 = Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyType\"]"));
            SelectElement select5 = new SelectElement(selectElem5);
            VerifyIsTrue(select5.SelectedOption.Text.Contains("Диапазон"));
            IWebElement selectElem6 = Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyGroup\"]"));
            SelectElement select6 = new SelectElement(selectElem6);
            VerifyIsTrue(select6.SelectedOption.Text.Contains("Group3"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInFilter\"] input")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInDetails\"] input")).Selected);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyuseInBrief\"] input")).Selected);
        }

        [Test]
        public void PropertiesAddCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertySettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyName\"]")).SendKeys("New_New_name");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyBtnCancel\"]")).Click();
            VerifyIsFalse(Driver.PageSource.Contains("New_New_name"));
        }
    }
}