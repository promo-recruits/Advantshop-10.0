using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns.Button.Product
{
    [TestFixture]
    public class LandingsServicesProductAdd : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing | ClearType.CRM);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Color.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Size.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerField.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSubBlock.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.DealStatus.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.SalesFunnel.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.SalesFunnel_DealStatus.csv"
            );

            Init();
        }

        [Test]
        public void ServicesFormAddOne()
        {
            TestName = "ServicesFormAddOne";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            //check grid
            VerifyAreEqual("1", Driver.GetGridCell(0, "ArtNo", "OffersSelectvizr").Text, "ArtNo product at grid");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid");
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "OffersSelectvizr").Text, "color product at grid");
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "OffersSelectvizr").Text, "size product at grid");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "PriceFormatted", "OffersSelectvizr").Text,
                "Price product at grid");

            VerifyAreEqual("rgba(0, 0, 0, 1)", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("color"),
                "ArtNo product at color font color");
            VerifyAreEqual("700", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("font-weight"),
                "ArtNo product at color font-weight");
            VerifyIsTrue(
                Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                    .FindElements(By.CssSelector(".ui-grid-icon-plus-squared")).Count == 0, "no btn tree");

            Driver.GridFilterSendKeys("TestProduct504");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no match search");

            Driver.GridFilterSendKeys("TestProduct2");
            VerifyAreEqual("TestProduct2", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            VerifyAreEqual("Color2", Driver.GetGridCell(0, "ColorName", "OffersSelectvizr").Text, "color product at grid 2");
            VerifyAreEqual("SizeName2", Driver.GetGridCell(0, "SizeName", "OffersSelectvizr").Text, "size product at grid 2");
            VerifyAreEqual("101 руб.", Driver.GetGridCell(0, "PriceFormatted", "OffersSelectvizr").Text,
                "Price product at grid 2");
            VerifyAreEqual("rgba(181, 180, 180, 1)", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("color"),
                "ArtNo product at color font color 2");
            VerifyAreEqual("700", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("font-weight"),
                "ArtNo product at color font-weight 2");
            VerifyIsTrue(
                Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                    .FindElements(By.CssSelector(".ui-grid-icon-plus-squared")).Count == 0, " btn tree 2");

            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            VerifyAreEqual("Color10", Driver.GetGridCell(0, "ColorName", "OffersSelectvizr").Text, "color product at grid 2");
            VerifyAreEqual("SizeName10", Driver.GetGridCell(0, "SizeName", "OffersSelectvizr").Text, "size product at grid 2");
            VerifyAreEqual("109 руб.", Driver.GetGridCell(0, "PriceFormatted", "OffersSelectvizr").Text,
                "Price product at grid 2");
            VerifyAreEqual("rgba(0, 0, 0, 1)", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("color"),
                "ArtNo product at color font color 3");
            VerifyAreEqual("700", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("font-weight"),
                "ArtNo product at color font-weight 3");
            VerifyIsTrue(
                Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                    .FindElements(By.CssSelector(".ui-grid-icon-plus-squared")).Count == 1, " btn tree 3");

            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            VerifyAreEqual("103", Driver.GetGridCell(1, "ArtNo", "OffersSelectvizr").Text, "ArtNo product at grid 4");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text, "Name product at grid 4");
            VerifyAreEqual("Color2", Driver.GetGridCell(1, "ColorName", "OffersSelectvizr").Text, "color product at grid 4");
            VerifyAreEqual("SizeName2", Driver.GetGridCell(1, "SizeName", "OffersSelectvizr").Text, "size product at grid 4");
            VerifyAreEqual("111 руб.", Driver.GetGridCell(1, "PriceFormatted", "OffersSelectvizr").Text,
                "Price product at grid 4");
            VerifyAreEqual("rgba(0, 0, 0, 1)", Driver.GetGridCell(1, "Name", "OffersSelectvizr").GetCssValue("color"),
                "ArtNo product at color font color 4");
            VerifyAreEqual("400", Driver.GetGridCell(1, "Name", "OffersSelectvizr").GetCssValue("font-weight"),
                "ArtNo product at color font-weight 4");
            VerifyIsTrue(
                Driver.GetGridCell(1, "treeBaseRowHeaderCol", "OffersSelectvizr")
                    .FindElements(By.CssSelector(".ui-grid-icon-plus-squared")).Count == 0, " btn tree 4");

            VerifyAreEqual("106", Driver.GetGridCell(4, "ArtNo", "OffersSelectvizr").Text, "ArtNo product at grid 5");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(4, "Name", "OffersSelectvizr").Text, "Name product at grid 5");
            VerifyAreEqual("Color5", Driver.GetGridCell(4, "ColorName", "OffersSelectvizr").Text, "color product at grid 5");
            VerifyAreEqual("SizeName5", Driver.GetGridCell(4, "SizeName", "OffersSelectvizr").Text, "size product at grid 5");
            VerifyAreEqual("200 руб.", Driver.GetGridCell(4, "PriceFormatted", "OffersSelectvizr").Text,
                "Price product at grid 5");
            VerifyAreEqual("rgba(0, 0, 0, 1)", Driver.GetGridCell(4, "Name", "OffersSelectvizr").GetCssValue("color"),
                "product color font color 5");
            VerifyAreEqual("400", Driver.GetGridCell(4, "Name", "OffersSelectvizr").GetCssValue("font-weight"),
                "product color font-weight 5");
            VerifyIsTrue(
                Driver.GetGridCell(4, "treeBaseRowHeaderCol", "OffersSelectvizr")
                    .FindElements(By.CssSelector(".ui-grid-icon-plus-squared")).Count == 0, " btn tree 5");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-minus-squared")).Click();

            Driver.GridFilterSendKeys("10");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count grid 5");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[7]['Name']\"]")).Count ==
                0, " no 8 row");
            Driver.GetGridCell(-1, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[7]['Name']\"]")).Count ==
                1, " display 8 row");

            Driver.GridFilterSendKeys("TestProduct1");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).GetAttribute("href")
                    .Contains("product/edit/1"), "href product");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Click();
            Thread.Sleep(2000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual("Товар \"TestProduct1\"", Driver.FindElement(By.TagName("h1")).Text, "name product on page");
            VerifyAreEqual(BaseUrl + "/adminv3/product/edit/1", Driver.Url, "url product on page");
            Functions.CloseTab(Driver, BaseUrl);

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name product");
            checkField(2, "test@name.product");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name product", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("100", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name product",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual("test@name.product", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("100 руб."), " Cost product at order");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddOneColor()
        {
            TestName = "ServicesFormAddOneColor";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_104\"]")).Displayed, "display product");
            Thread.Sleep(1000);
            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name Color");
            checkField(2, "test@name.Color");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name Color", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("112 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("112", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name Color",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual("test@name.Color", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct10\r\nАртикул: 104\r\nЦвет: Color3\r\nРазмер: SizeName3",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order");
            VerifyAreEqual("112", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("112 руб."), " Cost product at order");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddOneDisabled()
        {
            TestName = "ServicesFormAddOneDisabled";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct2");
            VerifyAreEqual("TestProduct2", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product");
            Thread.Sleep(1000);
            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name Disabled");
            checkField(2, "test@name.Disabled");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name Disabled", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("101 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("101", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name Disabled",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual("test@name.Disabled", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct2\r\nАртикул: 2\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order");
            VerifyAreEqual("101", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("101 руб."), " Cost product at order");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSome()
        {
            TestName = "ServicesFormAddSome";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct1");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct2");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 4");
            Thread.Sleep(1000);
            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name many");
            checkField(2, "test@name.many");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name many", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("4", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("421 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("421", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name many", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("test@name.many", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("100 руб."), " Cost product at order");

            VerifyAreEqual("TestProduct2\r\nАртикул: 2\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(1, "Name", "LeadItems").Text, "Name product at order 2");
            VerifyAreEqual("101", Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyIsTrue(Driver.GetGridCell(1, "Cost", "LeadItems").Text.Contains("101 руб."), " Cost product at order 2");

            VerifyAreEqual("TestProduct10\r\nАртикул: 10\r\nЦвет: Color10\r\nРазмер: SizeName10",
                Driver.GetGridCell(2, "Name", "LeadItems").Text, "Name product at order 3");
            VerifyAreEqual("109", Driver.GetGridCell(2, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyIsTrue(Driver.GetGridCell(2, "Cost", "LeadItems").Text.Contains("109 руб."), " Cost product at order 3");

            VerifyAreEqual("TestProduct10\r\nАртикул: 103\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(3, "Name", "LeadItems").Text, "Name product at order 4");
            VerifyAreEqual("111", Driver.GetGridCell(3, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 4");
            VerifyAreEqual("1", Driver.GetGridCell(3, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 4");
            VerifyIsTrue(Driver.GetGridCell(3, "Cost", "LeadItems").Text.Contains("111 руб."), " Cost product at order 4");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomeCategory()
        {
            TestName = "ServicesFormAddSomeCategory";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product 1 at grid");
            VerifyAreEqual("Найдено записей: 102",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count grid 2");

            Driver.GridFilterSendKeys("10");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count search");
            Driver.GetGridFilter().SendKeys(Keys.Backspace);
            Driver.GetGridFilter().SendKeys(Keys.Backspace);
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 1 at grid after del");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_2\"]")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 2 at grid 2");
            VerifyAreEqual("TestProduct22", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product 3 at grid 2");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_4\"]")).GetAttribute("class")
                    .Contains("jstree-leaf"), " btn tree 4");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_1\"]")).GetAttribute("class")
                    .Contains("jstree-leaf"), " btn tree 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-tree-id=\"categoryItemId_5\"]")).Count == 0,
                " no btn tree 5");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_4\"] i")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-tree-id=\"categoryItemId_5\"]")).Count == 1,
                " yes btn tree 5");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_5\"]")).GetAttribute("class")
                    .Contains("jstree-leaf"), " btn tree 5");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_5\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product 2 at grid");
            VerifyAreEqual("TestProduct82", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text, "Name product 3 at grid");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count grid 5");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count select 1");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_2\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count select 1 after ");
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 2 at grid 2");
            VerifyAreEqual("TestProduct22", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product 3 at grid 2");
            VerifyAreEqual("Найдено записей: 20",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count grid 2");
            Driver.GetGridCell(2, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count select 2");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_1\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product 2 at grid 1");
            VerifyAreEqual("TestProduct2", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text, "Name product 3 at grid 1");
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count select 3");

            Driver.GridFilterSendKeys("10");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count search in category");
            Driver.GetGridFilter().SendKeys(Keys.Backspace);
            Driver.GetGridFilter().SendKeys(Keys.Backspace);
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_4\"] .jstree-anchor")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 2 at grid 4");
            VerifyAreEqual("TestProduct62", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product 3 at grid 4");
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            VerifyAreEqual("4", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count select 4");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_0\"] .jstree-anchor")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 1 at grid all");
            VerifyAreEqual("Найдено записей: 102",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count grid all");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_4\"] i")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-tree-id=\"categoryItemId_5\"]")).Count == 0,
                " hide btn tree 5");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 1 at grid hide");

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_81\"]")).Displayed,
                "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_23\"]")).Displayed,
                "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_62\"]")).Displayed,
                "display product 4");

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name category");
            checkField(2, "test@name.category");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name category", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("4", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("564 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("564", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name category",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual("test@name.category", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct81\r\nАртикул: 81", Driver.GetGridCell(0, "Name", "LeadItems").Text,
                "Name product at order");
            VerifyAreEqual("180", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("180 руб."), " Cost product at order");

            VerifyAreEqual("TestProduct62\r\nАртикул: 62", Driver.GetGridCell(3, "Name", "LeadItems").Text,
                "Name product at order 5");
            VerifyAreEqual("161", Driver.GetGridCell(3, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 5");
            VerifyAreEqual("1", Driver.GetGridCell(3, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 5");
            VerifyIsTrue(Driver.GetGridCell(3, "Cost", "LeadItems").Text.Contains("161 руб."), " Cost product at order 5");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomeDel()
        {
            TestName = "ServicesFormAddSomeDel";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct1");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GridFilterSendKeys("TestProduct2");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product 4");
            Thread.Sleep(1000);

            //del elem
            Driver.FindElement(By.CssSelector("[data-e2e=\"DelProduct\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"DelProduct\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed,
                "display product 2.2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 4.2");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_10\"]")).Count == 0,
                "no display product 3");

            //add elem
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct15");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 5");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product 6");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_15\"]")).Displayed,
                "display product 7");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product 11");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_10\"]")).Count == 0,
                "no display product 31");

            BlockSettingsSave();
            Thread.Sleep(1000);

            //check client
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name del");
            checkField(2, "test@name.del");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name del", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("3", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("326 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("326", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name del", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("test@name.del", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");

            VerifyAreEqual("TestProduct10\r\nАртикул: 103\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order 0");
            VerifyAreEqual("111", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 0");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 0");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("111 руб."), " Cost product at order 0");

            VerifyAreEqual("TestProduct2\r\nАртикул: 2\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(1, "Name", "LeadItems").Text, "Name product at order 1");
            VerifyAreEqual("101", Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 1");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 1");
            VerifyIsTrue(Driver.GetGridCell(1, "Cost", "LeadItems").Text.Contains("101 руб."), " Cost product at order 1");

            VerifyAreEqual("TestProduct15\r\nАртикул: 15\r\nЦвет: Color5\r\nРазмер: SizeName5",
                Driver.GetGridCell(2, "Name", "LeadItems").Text, "Name product at order 2");
            VerifyAreEqual("114", Driver.GetGridCell(2, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyIsTrue(Driver.GetGridCell(2, "Cost", "LeadItems").Text.Contains("114 руб."), " Cost product at order 2");

            VerifyFinally(TestName);
        }


        [Test]
        public void ServicesFormAddSomeDelAll()
        {
            TestName = "ServicesFormAddSomeDelAll";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(2000);

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GridFilterSendKeys("TestProduct2");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product 2");
            Thread.Sleep(1000);

            BlockSettingsSave();
            Thread.Sleep(1000);
            Refresh();
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            TabSelect("tabFormSetting_0");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed,
                "display product 1 after refresh");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed,
                "display product 2 after refresh");
            //del all elem
            DelAllProduct();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_2\"]")).Count == 0,
                "no display product 2");

            BlockSettingsSave();
            Thread.Sleep(1000);

            //check client
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name delAll");
            checkField(2, "test@name.delAll");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name delAll", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("0", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name delAll",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual("test@name.delAll", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                " no displayed product grid");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[grid-unique-id=\"gridLeadItems\"] [data-e2e=\"gridCell\"]"))
                    .Count == 0, " no displayed product grid cell");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomePagination()
        {
            TestName = "ServicesFormAddSomePagination";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            //view
            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 1");
            VerifyAreEqual("TestProduct15", Driver.GetGridCell(9, "Name", "OffersSelectvizr").Text, "Name product at grid 10");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[10]['Name']\"]"))
                    .Count == 0, " no 11 row");

            Driver.GridPaginationSelectItems("100");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[50]['Name']\"]"))
                    .Count == 1, " 51 row");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 1");
            VerifyAreEqual("TestProduct97", Driver.GetGridCell(99, "Name", "OffersSelectvizr").Text,
                "Name product at grid 10");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[100]['Name']\"]"))
                    .Count == 0, " no 101 row");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 1");
            VerifyAreEqual("TestProduct15", Driver.GetGridCell(9, "Name", "OffersSelectvizr").Text, "Name product at grid 10");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[10]['Name']\"]"))
                    .Count == 0, " no 11 row 1");

            //pagination
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct16", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 1");
            VerifyAreEqual("TestProduct17", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text, "Name product at grid 2");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product at grid 1 page");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product at grid 2 page");

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct98", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product at grid 1 page last");
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product at grid 2 page last");

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product at grid 1 page first");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product at grid 2 page first");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct25", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product at grid 1 page");
            VerifyAreEqual("TestProduct26", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product at grid 2 page");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_25\"]")).Displayed,
                "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_26\"]")).Displayed,
                "display product 2");
            Thread.Sleep(1000);
            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name Pagination");
            checkField(2, "test@name.Pagination");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name Pagination", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("2", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("249 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("249", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name Pagination",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual("test@name.Pagination",
                Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");

            //check product grid

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct25\r\nАртикул: 25\r\nЦвет: Color5\r\nРазмер: SizeName5",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order");
            VerifyAreEqual("124", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("124 руб."), " Cost product at order");

            VerifyAreEqual("TestProduct26\r\nАртикул: 26\r\nЦвет: Color6\r\nРазмер: SizeName6",
                Driver.GetGridCell(1, "Name", "LeadItems").Text, "Name product at order 2");
            VerifyAreEqual("125", Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyIsTrue(Driver.GetGridCell(1, "Cost", "LeadItems").Text.Contains("125 руб."), " Cost product at order 2");

            //mobile
            ReInitClient();
            GoToMobile("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name mobile");
            checkField(2, "test@name.mobile");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree mobile");

            ReInit();
            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName mobile");
            VerifyAreEqual("test name mobile", Driver.GetGridCell(0, "FullName").Text, "FullName mobile");
            VerifyAreEqual("2", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount mobile");
            VerifyAreEqual("249 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum mobile");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date mobile");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("249", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead mobile");
            VerifyAreEqual("test name mobile",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead mobile");
            VerifyAreEqual("test@name.mobile", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead mobile");

            //check product grid mobile

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid mobile");
            VerifyAreEqual("TestProduct25\r\nАртикул: 25\r\nЦвет: Color5\r\nРазмер: SizeName5",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order mobile");
            VerifyAreEqual("124", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order mobile");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order mobile");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("124 руб."),
                " Cost product at order mobile");

            VerifyAreEqual("TestProduct26\r\nАртикул: 26\r\nЦвет: Color6\r\nРазмер: SizeName6",
                Driver.GetGridCell(1, "Name", "LeadItems").Text, "Name product at order 2 mobile");
            VerifyAreEqual("125", Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2 mobile");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2 mobile");
            VerifyIsTrue(Driver.GetGridCell(1, "Cost", "LeadItems").Text.Contains("125 руб."),
                " Cost product at order 2 mobile");


            VerifyFinally(TestName);
        }
    }
}