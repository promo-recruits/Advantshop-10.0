using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Taxes
{
    [TestFixture]
    public class SettingsTaxesAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Taxes | ClearType.Shipping);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\Taxes\\[Order].PaymentMethod.csv",
                "data\\Admin\\Settings\\Taxes\\[Order].ShippingMethod.csv",
                "data\\Admin\\Settings\\Taxes\\Settings.Settings.csv",
                "data\\Admin\\Settings\\Taxes\\Catalog.Tax.csv"
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
        [Order(0)]
        public void TaxesGrid()
        {
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            VerifyAreEqual("Налоги",
                Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Text,
                "h1 taxes page");

            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "Name");
            VerifyAreEqual("Без НДС", Driver.GetGridCell(0, "TaxTypeFormatted", "Taxes").Text, "Tax Type");
            VerifyAreEqual("0", Driver.GetGridCell(0, "Rate", "Taxes").Text, "Rate");

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "Enabled");
            VerifyIsTrue(Driver.GetGridCell(0, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected,
                "IsDefault");
            VerifyIsFalse(Driver.GetGridCell(1, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected,
                "IsDefault no");

            VerifyAreEqual("Найдено записей: 107",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(1)]
        public void TaxAddEnabled()
        {
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxAdd\"]")).Click();
            Thread.Sleep(2000);
            Driver.WaitForElem(By.Name("addEditTaxForm"));

            VerifyAreEqual("Новый налог",
                Driver.FindElement(By.Name("addEditTaxForm")).FindElement(By.TagName("h2")).Text, "pop up h2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).SendKeys("Tax Added Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxEnabled\"]")).FindElement(By.TagName("span")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).SendKeys("10");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"taxType\"]")))).SelectByText(
                "НДС по ставке 10%");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin grid
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.GetGridIdFilter("gridTaxes", "Tax Added Name");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();

            VerifyAreEqual("Tax Added Name", Driver.GetGridCell(0, "Name", "Taxes").Text, "tax Name added");
            VerifyAreEqual("НДС по ставке 10%", Driver.GetGridCell(0, "TaxTypeFormatted", "Taxes").Text,
                "Tax Type added");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Rate", "Taxes").Text, "Tax Rate added");

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "tax Enabled added");
            VerifyIsFalse(Driver.GetGridCell(0, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected,
                "tax IsDefault added");

            //check admin pop up
            Driver.GetGridCell(0, "_serviceColumn", "Taxes")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Name("addEditTaxForm"));

            VerifyAreEqual("Tax Added Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).GetAttribute("value"),
                "tax Name added pop up");
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).GetAttribute("value"),
                "tax Rate added pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "tax Rate added pop up");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxIsDefault\"]")).FindElement(By.TagName("input"))
                    .Selected, "tax IsDefault added pop up");

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"taxType\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("НДС по ставке 10%"), "tax Type pop up");

            //check admin tax added to product
            GoToAdmin("product/edit/38");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);

            IWebElement selectElemProd = Driver.FindElement(By.Id("TaxId"));
            SelectElement select2 = new SelectElement(selectElemProd);
            int lastElem = select2.Options.Count - 1;
            VerifyIsTrue(select2.Options[lastElem].Text.Contains("Tax Added Name"), "tax added to product");
        }

        [Test]
        [Order(2)]
        public void TaxAddDisabled()
        {
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxAdd\"]")).Click();
            Thread.Sleep(2000);
            Driver.WaitForElem(By.Name("addEditTaxForm"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).SendKeys("Tax Added Name Disabled");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).SendKeys("14");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"taxType\"]"))))
                .SelectByText("Не указано");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin grid
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            VerifyAreEqual("Найдено записей: 109",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all with added");

            Driver.GetGridIdFilter("gridTaxes", "Tax Added Name Disabled");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();

            VerifyAreEqual("Tax Added Name Disabled", Driver.GetGridCell(0, "Name", "Taxes").Text, "tax Name added");
            VerifyAreEqual("Не указано", Driver.GetGridCell(0, "TaxTypeFormatted", "Taxes").Text, "Tax Type added");
            VerifyAreEqual("14", Driver.GetGridCell(0, "Rate", "Taxes").Text, "Tax Rate added");

            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "tax Enabled added");
            VerifyIsFalse(Driver.GetGridCell(0, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected,
                "tax IsDefault added");

            //check admin pop up
            Driver.GetGridCell(0, "_serviceColumn", "Taxes")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Name("addEditTaxForm"));

            VerifyAreEqual("Tax Added Name Disabled",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).GetAttribute("value"),
                "tax Name added pop up");
            VerifyAreEqual("14", Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).GetAttribute("value"),
                "tax Rate added pop up");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "tax Rate added pop up");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxIsDefault\"]")).FindElement(By.TagName("input"))
                    .Selected, "tax IsDefault added pop up");

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"taxType\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Не указано"), "tax Type pop up");

            //check admin tax added in product hidden
            GoToAdmin("product/edit/34");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);

            IWebElement selectElemProd = Driver.FindElement(By.Id("TaxId"));
            SelectElement select2 = new SelectElement(selectElemProd);
            VerifyIsTrue(select2.Options.Count == 43, "disabled tax added to product hidden"); //enabled taxes only
        }

        [Test]
        [Order(3)]
        public void TaxEdit()
        {
            //pre check admin tax count all
            GoToAdmin("product/edit/35");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);

            IWebElement selectProdBegin = Driver.FindElement(By.Id("TaxId"));
            SelectElement select = new SelectElement(selectProdBegin);
            int allTaxOptionsBegin = select.Options.Count;

            //test
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.GetGridIdFilter("gridTaxes", "Tax 40");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();
            Driver.GetGridCell(0, "_serviceColumn", "Taxes")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Thread.Sleep(2000);
            Driver.WaitForElem(By.Name("addEditTaxForm"));

            //pre check admin pop up
            VerifyAreEqual("Tax 40", Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).GetAttribute("value"),
                "pre check tax Name added pop up");
            VerifyAreEqual("39", Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).GetAttribute("value"),
                "pre check tax Rate added pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "pre check tax Rate added pop up");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxIsDefault\"]")).FindElement(By.TagName("input"))
                    .Selected, "pre check tax IsDefault added pop up");

            IWebElement ElemPopUpBegin = Driver.FindElement(By.CssSelector("[data-e2e=\"taxType\"]"));
            SelectElement select1 = new SelectElement(ElemPopUpBegin);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Не указано"), "pre check tax Type pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).SendKeys("Edited Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxEnabled\"]")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxIsDefault\"]")).FindElement(By.TagName("span")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).SendKeys("111");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"taxType\"]")))).SelectByText(
                "НДС по ставке 18%");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin prev name
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.GetGridIdFilter("gridTaxes", "Tax 40");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "prev tax name");

            //check admin grid
            Driver.GetGridIdFilter("gridTaxes", "Edited Name");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();

            VerifyAreEqual("Edited Name", Driver.GetGridCell(0, "Name", "Taxes").Text, "tax Name edited");
            VerifyAreEqual("НДС по ставке 18%", Driver.GetGridCell(0, "TaxTypeFormatted", "Taxes").Text,
                "Tax Type edited");
            VerifyAreEqual("111", Driver.GetGridCell(0, "Rate", "Taxes").Text, "Tax Rate edited");

            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "tax Enabled edited");
            VerifyIsTrue(Driver.GetGridCell(0, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected,
                "tax IsDefault edited");

            //check admin pop up
            Driver.GetGridCell(0, "_serviceColumn", "Taxes")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Name("addEditTaxForm"));

            VerifyAreEqual("Edited Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).GetAttribute("value"),
                "tax Name edited pop up");
            VerifyAreEqual("111", Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).GetAttribute("value"),
                "tax Rate edited pop up");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "tax Rate edited pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxIsDefault\"]")).FindElement(By.TagName("input"))
                    .Selected, "tax IsDefault edited pop up");

            IWebElement ElemPopUpEnd = Driver.FindElement(By.CssSelector("[data-e2e=\"taxType\"]"));
            SelectElement select2 = new SelectElement(ElemPopUpEnd);
            VerifyIsTrue(select2.SelectedOption.Text.Contains("НДС по ставке 18%"), "tax Type pop up");

            //check admin tax added to product
            GoToAdmin("product/edit/35");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);

            IWebElement selectProdEnd = Driver.FindElement(By.Id("TaxId"));
            SelectElement select3 = new SelectElement(selectProdEnd);
            VerifyIsFalse(select3.SelectedOption.Text.Contains("Edited Name"), "new tax default edited");

            int allTaxOptionsEnd = select3.Options.Count;
            int allTaxOptionsEndCount = allTaxOptionsBegin - 1;
            VerifyIsTrue(allTaxOptionsEndCount == allTaxOptionsEnd, "tax default edited count all");
        }


        [Test]
        [Order(10)]
        public void AddDefault()
        {
            //pre check admin default tax and count all
            GoToAdmin("product/edit/37");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);

            IWebElement selectProdBegin = Driver.FindElement(By.Id("TaxId"));
            SelectElement select = new SelectElement(selectProdBegin);
            int allTaxOptionsBegin = select.Options.Count;

            VerifyIsTrue(select.SelectedOption.Text.Contains("Tax 1"), "pre check tax default");

            //test
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxAdd\"]")).Click();
            Thread.Sleep(2000);
            Driver.WaitForElem(By.Name("addEditTaxForm"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).SendKeys("Tax Added Default Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxEnabled\"]")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxIsDefault\"]")).FindElement(By.TagName("span")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).SendKeys("26");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"taxType\"]")))).SelectByText(
                "НДС по ставке 18%");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taxButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin prev default
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.GetGridIdFilter("gridTaxes", "Tax 1");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();

            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "tax Name prev default");
            VerifyIsFalse(Driver.GetGridCell(0, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected,
                "tax IsDefault prev");

            //check admin grid
            Driver.GetGridIdFilter("gridTaxes", "Tax Added Default Name");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();

            VerifyAreEqual("Tax Added Default Name", Driver.GetGridCell(0, "Name", "Taxes").Text, "tax Name added");
            VerifyAreEqual("НДС по ставке 18%", Driver.GetGridCell(0, "TaxTypeFormatted", "Taxes").Text,
                "Tax Type added");
            VerifyAreEqual("26", Driver.GetGridCell(0, "Rate", "Taxes").Text, "Tax Rate added");

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "tax Enabled added");
            VerifyIsTrue(Driver.GetGridCell(0, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected,
                "tax IsDefault added");

            //check admin pop up
            Driver.GetGridCell(0, "_serviceColumn", "Taxes")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Name("addEditTaxForm"));

            VerifyAreEqual("Tax Added Default Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxName\"]")).GetAttribute("value"),
                "tax Name added pop up");
            VerifyAreEqual("26", Driver.FindElement(By.CssSelector("[data-e2e=\"taxRate\"]")).GetAttribute("value"),
                "tax Rate added pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "tax Rate added pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"taxIsDefault\"]")).FindElement(By.TagName("input"))
                    .Selected, "tax IsDefault added pop up");

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"taxType\"]"));
            SelectElement select1 = new SelectElement(selectElem);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("НДС по ставке 18%"), "tax Type pop up");

            //check admin tax added to product
            GoToAdmin("product/edit/37");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);

            IWebElement selectProdEnd = Driver.FindElement(By.Id("TaxId"));
            SelectElement select2 = new SelectElement(selectProdEnd);
            VerifyIsTrue(select2.SelectedOption.Text.Contains("Tax Added Default Name"), "new tax default added");

            int allTaxOptionsEnd = select2.Options.Count;
            int allTaxOptionsEndCount = allTaxOptionsBegin + 1;
            VerifyIsTrue(allTaxOptionsEndCount == allTaxOptionsEnd, "tax default added count all");
        }
    }
}