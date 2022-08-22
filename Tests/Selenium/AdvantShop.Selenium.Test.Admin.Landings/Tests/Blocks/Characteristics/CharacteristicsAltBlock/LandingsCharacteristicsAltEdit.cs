using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Characteristics.CharacteristicsAltBlock
{
    [TestFixture]
    public class LandingsCharacteristicsAltEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Characteristics\\CharacteristicsAtl\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "characteristicsAlt";
        private string blockType = "Characteristics";
        private readonly int numberBlock = 1;

        [Test]
        public void InplaceTable()
        {
            TestName = "InplaceTable";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);
            ShowTitle();
            ShowSubTitle();
            BlockSettingsSave();

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))
                .SendKeys("New Category Characteristics");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"TitleCharacteristicsAlt\"] .inplace-initialized")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"TitleCharacteristicsAlt\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"TitleCharacteristicsAlt\"] .inplace-initialized"))
                .SendKeys("New Title");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))
                .SendKeys("New Name Characteristics");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"SubTitleCharacteristicsAlt\"] .inplace-initialized")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"SubTitleCharacteristicsAlt\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"SubTitleCharacteristicsAlt\"] .inplace-initialized"))
                .SendKeys("New SubTitle");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))
                .SendKeys("New Value Characteristics");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Click();

            VerifyAreEqual("New Name Characteristics",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text, "name characteristics");
            VerifyAreEqual("New Value Characteristics",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics");
            VerifyAreEqual("New Category Characteristics",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics");
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"SubTitleCharacteristicsAlt\"] .inplace-initialized")).Text,
                "subtitle characteristics");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"TitleCharacteristicsAlt\"] .inplace-initialized")).Text,
                "title characteristics");

            Refresh();

            VerifyAreEqual("New Name Characteristics",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text,
                "name characteristics after refresh");
            VerifyAreEqual("New Value Characteristics",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics after refresh");
            VerifyAreEqual("New Category Characteristics",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics after refresh");
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"SubTitleCharacteristicsAlt\"] .inplace-initialized")).Text,
                "subtitle characteristics after refresh");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"TitleCharacteristicsAlt\"] .inplace-initialized")).Text,
                "title characteristics after refresh");
            VerifyFinally(TestName);
        }

        [Test]
        public void EditCharacteristicSettings()
        {
            TestName = "EditCharacteristicSettings";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);

            TabSelect("tabCharacteristicsItems");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"editMode\"]")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['header']\"]"));
            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).SendKeys("New name category By Setting");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"editModeApply\"]")).Click();

            Driver.GetGridCell(0, "name").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(0, "name").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(0, "name").FindElement(By.TagName("textarea")).SendKeys("New name сharacteristic By Setting");

            Driver.GetGridCell(0, "value").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "value").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "value").FindElement(By.TagName("input")).SendKeys("value By Setting");

            BlockSettingsSave();

            VerifyAreEqual("New name сharacteristic By Setting",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text, "name characteristics");
            VerifyAreEqual("value By Setting",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics");
            VerifyAreEqual("New name category By Setting",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics");

            Refresh();

            VerifyAreEqual("New name сharacteristic By Setting",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text,
                "name characteristics after refresh");
            VerifyAreEqual("value By Setting",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics after refresh");
            VerifyAreEqual("New name category By Setting",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics after refresh");

            VerifyFinally(TestName);
        }

        [Test]
        public void EditCharacteristicsSettingsAdd()
        {
            TestName = "EditCharacteristicsSettingsAdd";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");
            Thread.Sleep(2000);

            BlockSettingsBtn(numberBlock);
            HiddenTitle();
            HiddenSubTitle();
            TabSelect("tabCharacteristicsItems");
            while (Driver.FindElements(By.CssSelector("[data-e2e=\"ItemDel\"]")).Count > 0)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ItemDel\"]")).Click();
                Thread.Sleep(500);
                Driver.SwalConfirm();
                Thread.Sleep(500);
            }

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['header']\"]")).Count == 0,
                "no Category Characteristics settings");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['name']\"]")).Count == 0,
                "no name Characteristics settings");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['value']\"]")).Count == 0,
                "no value Characteristics settings");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]")).Text.Contains("Нет элементов"),
                "no elem Category Characteristics settings");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-content")).Text.Contains("Добавьте группу"),
                "no elem name Characteristics settings");
            BlockSettingsSave();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Count == 0,
                "no Category Characteristics");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Count == 0,
                "no Characteristic Name");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Count == 0,
                "no Characteristic Value");


            BlockSettingsBtn(numberBlock);
            TabSelect("tabCharacteristicsItems");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['header']\"]")).Count == 0,
                "no Category Characteristics settings after refresh");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['name']\"]")).Count == 0,
                "no name Characteristics settings after refresh");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['value']\"]")).Count == 0,
                "no value Characteristics settings after refresh");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]")).Text.Contains("Нет элементов"),
                "no elem Category Characteristics settings after refresh");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-content")).Text.Contains("Добавьте группу"),
                "no elem name Characteristics settings after refresh");

            AddHeaderCharacteristics("New Name 1");
            AddNewCharacteristics("name1", "value1");
            AddNewCharacteristics("name2", "value2");
            AddNewCharacteristics("name3", "value3");
            AddNewCharacteristics("name4", "value4");
            AddNewCharacteristics("name5", "value5");

            AddHeaderCharacteristics("New Name 2");
            AddNewCharacteristics("name2.1", "value2.1");
            AddNewCharacteristics("name2.2", "value2.2");
            AddNewCharacteristics("name2.3", "value2.3");
            AddNewCharacteristics("name2.4", "value2.4");
            AddNewCharacteristics("name2.5", "value2.5");

            AddHeaderCharacteristics("New Name 3");
            AddHeaderCharacteristics("New Name 4");
            AddHeaderCharacteristics("New Name 5");

            BlockSettingsSave();

            VerifyAreEqual("New Name 1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics new 1");
            VerifyAreEqual("name1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text,
                "name characteristics new 1");
            VerifyAreEqual("value1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics new 1");


            VerifyAreEqual("New Name 3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[2].Text,
                "Category characteristics new 2");
            VerifyAreEqual("name3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[2].Text,
                "name characteristics new 2");
            VerifyAreEqual("value3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[2].Text,
                "value characteristics new 2");

            VerifyAreEqual("name2.1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[5].Text,
                "name characteristics new 5");
            VerifyAreEqual("value2.1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[5].Text,
                "value characteristics new 5");

            VerifyAreEqual("name2.5",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[9].Text,
                "name characteristics new 10");
            VerifyAreEqual("value2.5",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[9].Text,
                "value characteristics new 10");

            Refresh();
            VerifyAreEqual("New Name 1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics new 1 after refresh");
            VerifyAreEqual("name1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text,
                "name characteristics new 1 after refresh");
            VerifyAreEqual("value1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics new 1 after refresh");

            VerifyAreEqual("New Name 3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[2].Text,
                "Category characteristics new 2 after refresh");
            VerifyAreEqual("name3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[2].Text,
                "name characteristics new 2 after refresh");
            VerifyAreEqual("value3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[2].Text,
                "value characteristics new 2 after refresh");

            VerifyAreEqual("name2.1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[5].Text,
                "name characteristics new 5 after refresh");
            VerifyAreEqual("value2.1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[5].Text,
                "value characteristics new 5 after refresh");

            VerifyAreEqual("name2.5",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[9].Text,
                "name characteristics new 10 after refresh");
            VerifyAreEqual("value2.5",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[9].Text,
                "value characteristics new 10 after refresh");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabCharacteristicsItems");

            VerifyAreEqual("New Name 1",
                Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace header");
            VerifyAreEqual("name1", Driver.GetGridCell(0, "name").FindElement(By.TagName("textarea")).GetAttribute("value"),
                "settings inplace name");
            VerifyAreEqual("value1", Driver.GetGridCell(0, "value").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace value");
            VerifyAreEqual("New Name 5",
                Driver.GetGridCell(4, "header").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace header 5");
            VerifyAreEqual("name5", Driver.GetGridCell(4, "name").FindElement(By.TagName("textarea")).GetAttribute("value"),
                "settings inplace name 5");
            VerifyAreEqual("value5", Driver.GetGridCell(4, "value").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace value 5");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"] [data-e2e-row-index=\"4\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"editMode\"]")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e-grid-cell=\"grid[4]['header']\"]"));
            Driver.GetGridCell(4, "header").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(4, "header").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(4, "header").FindElement(By.TagName("input")).SendKeys("New name category Cancel");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"] [data-e2e-row-index=\"4\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"editModeCancel\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("New Name 5",
                Driver.GetGridCell(4, "header").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace header cancel");

            //Drag and drop

            Driver.GetGridCell(0, "header").Click();
            DragDrop(0, 3, "gridSubHeader");
            Driver.GetGridCell(1, "header").Click();
            DragDrop(2, 1, "gridSubHeader");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridSubHeader\"] [data-e2e=\"AddNewElem\"]"));
            DragDrop(4, 0, "gridSubHeader");

            DragDrop(2, 0, "gridHeader");
            DragDrop(2, 3, "gridHeader");
            BlockSettingsSave();

            VerifyAreEqual("New Name 3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics drag 1");
            VerifyAreEqual("New Name 1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[1].Text,
                "Category characteristics drag 2");
            VerifyAreEqual("New Name 4",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[2].Text,
                "Category characteristics drag 3");
            VerifyAreEqual("New Name 2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[3].Text,
                "Category characteristics drag 4");
            VerifyAreEqual("New Name 5",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[4].Text,
                "Category characteristics drag 5");

            VerifyAreEqual("name2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text,
                "name characteristics drag 1");
            VerifyAreEqual("value2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics drag 1");
            VerifyAreEqual("name1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[3].Text,
                "name characteristics drag 4");
            VerifyAreEqual("value1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[3].Text,
                "value characteristics drag 4");

            VerifyAreEqual("name2.5",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[5].Text,
                "name characteristics drag 1");
            VerifyAreEqual("value2.5",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[5].Text,
                "value characteristics drag 1");
            VerifyAreEqual("name2.1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[6].Text,
                "name characteristics drag 2");
            VerifyAreEqual("value2.1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[6].Text,
                "value characteristics drag 2");
            VerifyAreEqual("name2.3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[7].Text,
                "name characteristics drag 3");
            VerifyAreEqual("value2.3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[7].Text,
                "value characteristics drag 3");
            VerifyAreEqual("name2.2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[8].Text,
                "name characteristics drag 4");
            VerifyAreEqual("value2.2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[8].Text,
                "value characteristics drag 4");
            VerifyAreEqual("name2.4",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[9].Text,
                "name characteristics drag 5");
            VerifyAreEqual("value2.4",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[9].Text,
                "value characteristics drag 5");

            VerifyFinally(TestName);
        }

        [Test]
        public void EditCharacteristicsSettingAddDel()
        {
            TestName = "EditCharacteristicsSettingAddDel";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabCharacteristicsItems");
            while (Driver.FindElements(By.CssSelector("[data-e2e=\"ItemDel\"]")).Count > 0)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ItemDel\"]")).Click();
                Thread.Sleep(500);
                Driver.SwalConfirm();
                Thread.Sleep(500);
            }

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['header']\"]")).Count == 0,
                "no header Characteristics settings");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['name']\"]")).Count == 0,
                "no elem Characteristics settings");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"AddNewElem\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"editMode\"]")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['header']\"]"));
            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).SendKeys("name Header no save");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"editModeCancel\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ItemDel\"]")).Click();
            Thread.Sleep(500);
            Driver.SwalConfirm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['header']\"]")).Count == 0,
                "no Category Characteristics settings");
            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Count == 0,
                "Category characteristics count");
            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Count == 0,
                "Category characteristics count after refresh");

            BlockSettingsBtn(numberBlock);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['header']\"]")).Count == 0,
                "no Category Characteristics settings");
            AddHeaderCharacteristics("");
            AddNewCharacteristics("", "");
            AddNewCharacteristics("", "");
            AddHeaderCharacteristics("");
            AddHeaderCharacteristics("");
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Count == 3,
                "count Category Characteristics settings");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Count == 2,
                "count Name Characteristics settings");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Count == 2,
                "count Value Characteristics settings");

            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics 1");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[1].Text,
                "Category characteristics 2");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[2].Text,
                "Category characteristics 3");

            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text,
                "name characteristics 1");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics 1");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[1].Text,
                "name characteristics 2");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[1].Text,
                "value characteristics 2");
            Refresh();
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics 1 after refresh");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text,
                "name characteristics 1 after refresh");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics 1 after refresh");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))
                .SendKeys("New Category Inplace");
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).SendKeys("New Name Inplace");
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).SendKeys("New Value Inplace");
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Click();

            VerifyAreEqual("New Category Inplace",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics 11");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[1].Text,
                "Category characteristics 12");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[2].Text,
                "Category characteristics 13");

            VerifyAreEqual("New Name Inplace",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text,
                "name characteristics 1");
            VerifyAreEqual("New Value Inplace",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics 1");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[1].Text,
                "name characteristics 2");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[1].Text,
                "value characteristics 2");

            BlockSettingsBtn(numberBlock);

            VerifyAreEqual("New Category Inplace",
                Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace header");
            VerifyAreEqual("New Name Inplace",
                Driver.GetGridCell(0, "name").FindElement(By.TagName("textarea")).GetAttribute("value"),
                "settings inplace name");
            VerifyAreEqual("New Value Inplace",
                Driver.GetGridCell(0, "value").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace value");

            BlockSettingsCancel();
            Refresh();
            VerifyAreEqual("New Category Inplace",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]")).Text,
                "Category characteristics 1 inplace");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[1].Text,
                "Category characteristics 2 inplace");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CategoryCharacteristicsAlt\"]"))[2].Text,
                "Category characteristics 3 inplace");

            VerifyAreEqual("New Name Inplace",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]")).Text,
                "name characteristics 1 inplace ");
            VerifyAreEqual("New Value Inplace",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]")).Text,
                "value characteristics 1 inplace ");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltName\"]"))[1].Text,
                "name characteristics 2 inplace ");
            VerifyAreEqual("Нажмите сюда, чтобы добавить описание",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"CharacteristicsAltValue\"]"))[1].Text,
                "value characteristics 2 inplace ");

            VerifyFinally(TestName);
        }


        public void AddHeaderCharacteristics(string nameHeader)
        {
            var count = Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]"))
                .FindElements(By.CssSelector("[data-e2e=\"ItemDel\"]")).Count;
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"AddNewElem\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]"))
                .FindElements(By.CssSelector("[data-e2e=\"editMode\"]"))[count].Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e-grid-cell=\"grid[" + count + "]['header']\"]"));
            Driver.GetGridCell(count, "header").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "header").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "header").FindElement(By.TagName("input")).SendKeys(nameHeader);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeader\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"editModeApply\"]")).Click();
            Driver.GetGridCell(count, "header").Click();
        }

        public void AddNewCharacteristics(string name, string value)
        {
            var count = Driver.FindElement(By.CssSelector("[data-e2e=\"gridSubHeader\"]"))
                .FindElements(By.CssSelector(".lp-table__body [data-e2e=\"ItemDel\"]")).Count;
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSubHeader\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"AddNewElem\"]")).Click();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(count, "name").FindElement(By.TagName("textarea")).SendKeys(name);

            Driver.GetGridCell(count, "value").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "value").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "value").FindElement(By.TagName("input")).SendKeys(value);
        }
    }
}