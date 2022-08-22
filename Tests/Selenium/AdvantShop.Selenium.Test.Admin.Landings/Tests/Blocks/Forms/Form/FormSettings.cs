using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Forms.Form
{
    [TestFixture]
    internal class FormSettings : FunctionsForms
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing | ClearType.CRM);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Forms\\Form\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.CustomerField.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSubBlock.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CMS.LandingSite_Product.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CRM.DealStatus.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CRM.SalesFunnel.csv",
                "data\\Admin\\LandingTest\\Forms\\Form\\CRM.SalesFunnel_DealStatus.csv"
            );

            Init();
        }

        private string blockName = "form";
        private string blockType = "Forms";
        private readonly int numberBlock = 1;
        private string blockNameClient = "lp-block-form";
        private readonly string blockTitle = "FormTitle";
        private readonly string blockSubTitle = "FormSubTitle";
        private readonly string blockCapture = "FormField";

        [Test]
        public void FormAction_AddToCartAndGoToCheckout()
        {
            //TODO: Добавить апселлы!
            //список 1, без товара, без апсел
            //список 2, 1 товар,    без апсел, своя цена
            //список 2, 1 товар,   с апселл , другая цена
            //список 2, 3 товара,  с апселл , своя цена

            TestName = "FormAction_AddToCartAndGoToCheckout";
            VerifyBegin(TestName);

            ReInit();

            #region Settings

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxUncheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabHeaderActions");
            SelectOption("number:5", "[data-e2e=\"PosFormAction\"]");
            SelectOption("number:1", "[data-e2e=\"SelectSalesFunnels\"]");
            TabFormSettingsClick("tabFormSetting_0");
            //SelectOption("number:1", "[data-e2e=\"SelectUsell\"]");
            DelAllProduct();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name1");
            FillLpFormField("89000000001", 1);
            FillLpFormField("email11@test.ru", 2);
            SendForm();
            //Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains("/checkout/lp?lpid="),
                "1-page-checkout url not expected");
            VerifyIsTrue(RowItemsCount(".checkout-cart-content", ".checkout-cart-item-row") == 0,
                "1-checkout items count");
            VerifyIsTrue(Driver.FindElement(By.ClassName("checkout-cart-result-price")).Text.Contains("0 руб."),
                "1-checkout result price");
            SetCheckoutData("Name1", "89000000001", "email11@test.ru");
            VerifyIsTrue(Driver.Url.Contains("/checkout/success/") && Driver.Url.Contains("?mode=lp"),
                "1-page-success url not expected");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderActions");
            SelectOption("number:2", "[data-e2e=\"SelectSalesFunnels\"]");
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct1"});
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name2");
            FillLpFormField("89000000002", 1);
            FillLpFormField("email12@test.ru", 2);
            SendForm();
            //Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/checkout/lp?lpid="),
                "2-page-checkout url not expected");
            VerifyIsTrue(RowItemsCount(".checkout-cart-items", ".checkout-cart-item-row") == 1,
                "2-checkout items count");
            VerifyIsTrue(Driver.FindElement(By.ClassName("checkout-cart-result-price")).Text.Contains("100 руб."),
                "2-checkout result price");
            SetCheckoutData("Name2", "89000000002", "email12@test.ru");
            VerifyIsTrue(Driver.Url.Contains("/checkout/success/") && Driver.Url.Contains("?mode=lp"),
                "2-page-success url not expected");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabFormSetting_0");
            SetInputText("NewPriceProduct", "999");
            SetInputText("NewAmountProduct", "2");
            //добавить апселл
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name3");
            FillLpFormField("89000000003", 1);
            FillLpFormField("email13@test.ru", 2);
            SendForm();
            //Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/checkout/lp?lpid="),
                "3-page-checkout url not expected");
            VerifyIsTrue(RowItemsCount(".checkout-cart-items", ".checkout-cart-item-row") == 1,
                "3-checkout items count");
            VerifyIsTrue(Driver.FindElement(By.ClassName("checkout-cart-result-price")).Text.Contains("1 998 руб."),
                "3-checkout result price");
            SetCheckoutData("Name3", "89000000003", "email13@test.ru");
            //попадает в апселл!
            //VerifyIsTrue(driver.Url.Contains("/checkout/success/"),
            //    "2-page-success url not expected");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct1", "TestProduct10", "TestProduct100"});
            //добавить апселл
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name4");
            FillLpFormField("89000000004", 1);
            FillLpFormField("email14@test.ru", 2);
            SendForm();
            //Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/checkout/lp?lpid="),
                "4-page-checkout url not expected");
            VerifyIsTrue(RowItemsCount(".checkout-cart-items", ".checkout-cart-item-row") == 3,
                "4-checkout items count");
            VerifyIsTrue(Driver.FindElement(By.ClassName("checkout-cart-result-price")).Text.Contains("408 руб."),
                "4-checkout result price");
            SetCheckoutData("Name4", "89000000004", "email14@test.ru");
            //попадает в апселл!
            //VerifyIsTrue(driver.Url.Contains("/checkout/success/"),
            //    "2-page-success url not expected");

            #endregion

            #region Leads

            ReInit();
            GoToAdmin("leads?salesFunnelId=1");
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".ui-grid-render-container-body [data-e2e=\"gridRow\"]")).Count == 1,
                "1-salesFunnel has not 1 leads");
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name1",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "1-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 0,
                "1-Lead product count not expected");
            GoToCustomerPage();
            VerifyIsTrue(RowItemsCount("[ng-if=\"tabOrdersShown\"]", "[data-e2e=\"gridRow\"]") == 1,
                "1-customer order count");
            VerifyIsTrue(Driver.GetGridCell(0, "SumFormatted", "CustomerOrders").Text.Contains("0 руб."),
                "1-order sum");
            CloseCustomerPage();
            DelLead();

            GoToAdmin("leads?salesFunnelId=2");
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".ui-grid-render-container-body [data-e2e=\"gridRow\"]")).Count == 3,
                "2-salesFunnel has not 3 leads");

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name4",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "4-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 3,
                "4-Lead product count not expected");
            VerifyAreEqual("TestProduct1",
                Driver.GetGridCell(0, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "4-product1 name");
            VerifyAreEqual("TestProduct10",
                Driver.GetGridCell(1, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "4-product2 name");
            VerifyAreEqual("TestProduct100",
                Driver.GetGridCell(2, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "4-product3 name");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text == "408 руб.",
                "4-lead discount summary not expected");
            GoToCustomerPage();
            VerifyIsTrue(RowItemsCount("[ng-if=\"tabOrdersShown\"]", "[data-e2e=\"gridRow\"]") == 1,
                "4-customer order count");
            VerifyIsTrue(Driver.GetGridCell(0, "SumFormatted", "CustomerOrders").Text.Contains("408 руб."),
                "4-order sum");
            CloseCustomerPage();
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name3",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "3-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 1,
                "3-Lead product count not expected");
            VerifyAreEqual("TestProduct1",
                Driver.GetGridCell(0, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "3-product1 name");
            VerifyAreEqual("999",
                Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "3-product1 price");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text == "1 998 руб.",
                "3-lead discount summary not expected");
            GoToCustomerPage();
            VerifyIsTrue(RowItemsCount("[ng-if=\"tabOrdersShown\"]", "[data-e2e=\"gridRow\"]") == 1,
                "3-customer order count");
            VerifyIsTrue(Driver.GetGridCell(0, "SumFormatted", "CustomerOrders").Text.Contains("1 998 руб."),
                "3-order sum");
            CloseCustomerPage();
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name2",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "2-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 1,
                "2-Lead product count not expected");
            VerifyAreEqual("TestProduct1",
                Driver.GetGridCell(0, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "2-product1 name");
            VerifyAreEqual("100",
                Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "2-product1 price");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text == "100 руб.",
                "2-lead discount summary not expected");
            GoToCustomerPage();
            VerifyIsTrue(RowItemsCount("[ng-if=\"tabOrdersShown\"]", "[data-e2e=\"gridRow\"]") == 1,
                "2-customer order count");
            VerifyIsTrue(Driver.GetGridCell(0, "SumFormatted", "CustomerOrders").Text.Contains("100 руб."),
                "2-order sum");
            CloseCustomerPage();
            DelLead();

            #endregion

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxCheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            VerifyFinally(TestName);
        }

        [Test]
        public void FormAction_AddToCartAndGoToCheckout_WithCrossSell()
        {
            //1 товар  с зад.апп , не выбран апп. - страница спасибо за заказ; в заказе 1 товар.
            //1 товар  с зад.апп ,    выбран апп. - проверить, что в заказ добавился товар.
            //2 товара с разн.апп, не выбран апп.
            //2 товара с разн.апп,    выбран апп.
            //1 товар - 7, 2 товар - 9
            //+ в первом случае добавить товар из апселла в корзину
            //смотрю по отображаемым страницам и кол-ву итоговому товаров в корзине

            TestName = "FormAction_AddToCartAndGoToCheckout_WithCrossSell";
            VerifyBegin(TestName);

            ReInit();

            #region Settings

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxUncheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabHeaderActions");
            SelectOption("number:5", "[data-e2e=\"PosFormAction\"]");
            SelectOption("number:1", "[data-e2e=\"SelectSalesFunnels\"]");
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct7"});
            SelectOption("string:", "[data-e2e=\"SelectUsell\"]");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name1");
            FillLpFormField("89000000001", 1);
            FillLpFormField("email21@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/checkout/lp?lpid="),
                "1-page-checkout url not expected");
            SetCheckoutData("Name1", "89000000001", "email21@test.ru");
            VerifyIsTrue(Driver.Url.Contains("/lp/crosssell1") && Driver.Url.Contains("mode=lp"),
                "1-page-uppsell url not expected");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonForm\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains("/checkout/success") && Driver.Url.Contains("mode=lp"),
                "1-page-success url not expected");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabFormSetting_0");
            SelectOption("number:2", "[data-e2e=\"SelectUsell\"]");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name2");
            FillLpFormField("89000000002", 1);
            FillLpFormField("email22@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/checkout/lp?lpid="),
                "2-page-checkout url not expected");
            SetCheckoutData("Name2", "89000000002", "email22@test.ru");
            VerifyIsTrue(Driver.Url.Contains("/lp/test1/page1") && Driver.Url.Contains("mode=lp"),
                "2-page-success url not expected");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct9", "TestProduct7"});
            SelectOption("string:", "[data-e2e=\"SelectUsell\"]");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name3");
            FillLpFormField("89000000003", 1);
            FillLpFormField("email23@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/checkout/lp?lpid="),
                "3-page-checkout url not expected");
            SetCheckoutData("Name3", "89000000003", "email23@test.ru");
            //попадает в апселл товара, который был добавлен в воронку первым
            VerifyIsTrue(Driver.Url.Contains("/lp/crosssell2") && Driver.Url.Contains("mode=lp"),
                "3-page-uppsell url not expected");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ButtonForm\"]")).Count == 0,
                "3-not expected upsell-page");
            Thread.Sleep(1000);

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabFormSetting_0");
            SelectOption("number:2", "[data-e2e=\"SelectUsell\"]");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name4");
            FillLpFormField("89000000004", 1);
            FillLpFormField("email24@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/checkout/lp?lpid="),
                "4-page-checkout url not expected");
            SetCheckoutData("Name4", "89000000004", "email24@test.ru");
            VerifyIsTrue(Driver.Url.Contains("/lp/test1/page1") && Driver.Url.Contains("mode=lp"),
                "4-page-success url not expected");

            #endregion

            #region Leads

            ReInit();
            GoToAdmin("leads?salesFunnelId=1");
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".ui-grid-render-container-body [data-e2e=\"gridRow\"]")).Count == 4,
                "4-salesFunnel has not 4 leads");
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name4",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "4-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 2,
                "4-Lead product count not expected");
            GoToCustomerPage();
            CloseCustomerPage();
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name3",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "3-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 2,
                "3-Lead product count not expected");
            GoToCustomerPage();
            CloseCustomerPage();
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name2",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "2-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 1,
                "2-Lead product count not expected");
            GoToCustomerPage();
            CloseCustomerPage();
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name1",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "1-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 1,
                "1-Lead product count not expected"); //товар из допродаж попадает не в лид, а в заказ

            GoToCustomerPage();
            VerifyIsTrue(RowItemsCount("[ng-if=\"tabOrdersShown\"]", "[data-e2e=\"gridRow\"]") == 1,
                "1-customer order count");
            VerifyIsTrue(Driver.GetGridCell(0, "SumFormatted", "CustomerOrders").Text.Contains("213 руб."),
                "1-lead discount summary not expected");

            Driver.GetGridCell(0, "Number", "CustomerOrders").FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            Driver.SwitchTo().Window(Driver.WindowHandles[2]);
            VerifyIsTrue(RowItemsCount(".order-items-content", "[data-e2e=\"gridRow\"]") == 2,
                "1-Order product count (landing with upsell) not expected");
            Driver.FindElement(By.PartialLinkText("Удалить заказ")).Click();
            Driver.FindElement(By.CssSelector(".swal2-confirm.btn-success")).Click();
            Thread.Sleep(500);
            Driver.Close();
            Driver.SwitchTo().Window(Driver.WindowHandles[1]);
            Thread.Sleep(500);

            CloseCustomerPage(false);

            DelLead();

            #endregion

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxCheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            VerifyFinally(TestName);
        }

        [Test]
        public void FormAction_ExcludedProduct()
        {
            //тест на недоступные для покупки товары ( неактивные(2), не в наличии(3), товары без цены(4))
            TestName = "FormAction_ExcludedProduct";
            VerifyBegin(TestName);

            ReInit();

            #region Settings

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxUncheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabHeaderActions");
            SelectOption("number:5", "[data-e2e=\"PosFormAction\"]");
            SelectOption("number:1", "[data-e2e=\"SelectSalesFunnels\"]");
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct2"});
            BlockSettingsSave();

            Thread.Sleep(1000);
            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name1");
            FillLpFormField("89000000001", 1);
            FillLpFormField("email31@test.ru", 2);
            SendForm();
            //Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.ClassName("checkout-cart-result-price")).Text.Contains("101 руб."),
                "1-checkout result price");
            SetCheckoutData("Name1", "89000000001", "email31@test.ru");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct3"});
            BlockSettingsSave();

            Thread.Sleep(1000);
            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name2");
            FillLpFormField("89000000002", 1);
            FillLpFormField("email32@test.ru", 2);
            SendForm();
            //Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.ClassName("checkout-cart-result-price")).Text.Contains("102 руб."),
                "2-checkout result price");
            SetCheckoutData("Name2", "89000000002", "email32@test.ru");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct4"});
            BlockSettingsSave();

            Thread.Sleep(1000);
            ReInitClient();
            GoToClient("lp/test1");
            FillLpFormField("Name3");
            FillLpFormField("89000000003", 1);
            FillLpFormField("email33@test.ru", 2);
            SendForm();
            //Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.ClassName("checkout-cart-result-price")).Text.Contains("0 руб."),
                "2-checkout result price");
            SetCheckoutData("Name3", "89000000003", "email33@test.ru");

            #endregion

            #region Leads

            ReInit();
            GoToAdmin("leads?salesFunnelId=1");
            Thread.Sleep(1000);

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name3",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "3-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 1,
                "3-Lead product count not expected");
            VerifyAreEqual("0 руб.",
                Driver.GetGridCell(0, "Cost", "LeadItems").Text,
                "3-product3 price");

            GoToCustomerPage();
            VerifyIsTrue(RowItemsCount("[ng-if=\"tabOrdersShown\"]", "[data-e2e=\"gridRow\"]") == 1,
                "3-customer order count");
            VerifyIsTrue(Driver.GetGridCell(0, "SumFormatted", "CustomerOrders").Text.Contains("0 руб."),
                "3-order sum");
            CloseCustomerPage();
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name2",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "2-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 1,
                "2-Lead product count not expected");
            VerifyAreEqual("102 руб.",
                Driver.GetGridCell(0, "Cost", "LeadItems").Text,
                "2-product2 price");

            GoToCustomerPage();
            VerifyIsTrue(RowItemsCount("[ng-if=\"tabOrdersShown\"]", "[data-e2e=\"gridRow\"]") == 1,
                "2-customer order count");
            VerifyIsTrue(Driver.GetGridCell(0, "SumFormatted", "CustomerOrders").Text.Contains("102 руб."),
                "2-order sum");
            CloseCustomerPage();
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name1",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "1-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 1,
                "1-Lead product count not expected");
            VerifyAreEqual("101 руб.",
                Driver.GetGridCell(0, "Cost", "LeadItems").Text,
                "1-product1 price");

            GoToCustomerPage();
            VerifyIsTrue(RowItemsCount("[ng-if=\"tabOrdersShown\"]", "[data-e2e=\"gridRow\"]") == 1,
                "1-customer order count");
            VerifyIsTrue(Driver.GetGridCell(0, "SumFormatted", "CustomerOrders").Text.Contains("101 руб."),
                "1-order sum");
            CloseCustomerPage();
            DelLead();

            #endregion

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxCheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            VerifyFinally(TestName);
        }


        [Test]
        public void FormAction_GoToUrl()
        {
            //на страницу воронки,    передавать по лиду, список 1, без товара
            //на страницу воронки, не передавать по лиду, список 2, с 1 товаром
            //на свою страницу   ,    передавать по лиду, (2)     , с 3 товарами
            //на свою страницу   , не передавать по лиду, (2)     , без товара
            TestName = "FormAction_GoToUrl";
            VerifyBegin(TestName);

            ReInit();

            #region Settings

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxUncheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabHeaderActions");
            SelectOption("number:2", "[data-e2e=\"PosFormAction\"]");
            SelectOption("string:2", "[data-e2e=\"PostMessageRedirect\"]");
            SelectOption("number:1", "[data-e2e=\"SelectSalesFunnels\"]");
            Driver.CheckBoxUncheck("[data-e2e=\"DontSendLead\"]");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name1");
            FillLpFormField("89000000001", 1);
            FillLpFormField("email41@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/lp/test1/page1?lid="),
                "1-page1 url not contain lid data");

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderActions");
            Driver.CheckBoxCheck("[data-e2e=\"DontSendLead\"]");
            SelectOption("number:2", "[data-e2e=\"SelectSalesFunnels\"]");
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct1"});
            SetInputText("NewPriceProduct", "999");
            SetInputText("NewAmountProduct", "2");
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name2");
            FillLpFormField("89000000002", 1);
            FillLpFormField("email42@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/lp/test1/page1"),
                "2.1-page1 url");
            VerifyIsFalse(Driver.Url.Contains("?lid="),
                "2.2-page1 url contain lid data");

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderActions");
            SelectOption("object:null", "[data-e2e=\"PostMessageRedirect\"]");
            SetInputText("UrlRedirect", "products/test-product1");
            Driver.CheckBoxUncheck("[data-e2e=\"DontSendLead\"]");
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct1", "TestProduct10", "TestProduct100"});
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name3");
            FillLpFormField("89000000003", 1);
            FillLpFormField("email43@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/products/test-product1?lid="),
                "3-products/test-product1 url not contain lid data");

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderActions");
            Driver.CheckBoxCheck("[data-e2e=\"DontSendLead\"]");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name4");
            FillLpFormField("89000000004", 1);
            FillLpFormField("email44@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/products/test-product1"),
                "4.1-products/test-product1-page1 url");
            VerifyIsFalse(Driver.Url.Contains("?lid="),
                "4.2-products/test-product1-page1 url contain lid data");

            #endregion

            #region Leads

            GoToAdmin("leads?salesFunnelId=1");
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".ui-grid-render-container-body [data-e2e=\"gridRow\"]")).Count == 1,
                "1-salesFunnel has not 1 leads");
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name1",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "1-LeadFirstName");
            DelLead();

            GoToAdmin("leads?salesFunnelId=2");
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".ui-grid-render-container-body [data-e2e=\"gridRow\"]")).Count == 3,
                "2-salesFunnel has not 3 leads");

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name4",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "4-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 0,
                "4-Lead product count not expected");
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name3",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "3-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 3,
                "3-Lead product count not expected");
            VerifyAreEqual("TestProduct1",
                Driver.GetGridCell(0, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "3-product1 name");
            VerifyAreEqual("100",
                Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "3-product1 price");
            VerifyAreEqual("TestProduct10",
                Driver.GetGridCell(1, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "3-product2 name");
            VerifyAreEqual("109",
                Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "3-product2 price");
            VerifyAreEqual("TestProduct100",
                Driver.GetGridCell(2, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "3-product3 name");
            VerifyAreEqual("199",
                Driver.GetGridCell(2, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "3-product3 price");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text == "408 руб.",
                "3-lead discount summary not expected");
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name2",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "2-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 1,
                "2-Lead product count not expected");
            VerifyAreEqual("TestProduct1",
                Driver.GetGridCell(0, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "2-product1 name");
            VerifyAreEqual("999",
                Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "2-product3 price");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text == "1 998 руб.",
                "2-lead discount summary not expected");
            DelLead();

            #endregion

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxCheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            VerifyFinally(TestName);
        }

        [Test]
        public void FormAction_GoToUrlAndSendLetter()
        {
            //на страницу воронки,    передавать по лиду, список 1, заголовок 1, текст 1, без товара
            //на страницу воронки, не передавать по лиду, список 2, заголовок 1, текст 1, с 1 товаром
            //на свою страницу   ,    передавать по лиду, (2)     , заголовок 2, текст 2, с 3 товарами
            //на свою страницу   , не передавать по лиду, (2)     , заголовок 2, текст 2, без товара
            TestName = "FormAction_GoToUrlAndSendLetter";
            VerifyBegin(TestName);

            ReInit();

            GoToAdmin("settingsmail");
            Driver.FindElement(By.CssSelector("li[index=\"'formats'\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("Оформление заказа");
            Thread.Sleep(1000);
            if (Driver.FindElements(By.CssSelector(".ng-empty[data-e2e=\"switchOnOffInput\"]")).Count > 0)
                Driver.GetGridCell(0, "TypeName").FindElement(By.CssSelector("label[data-e2e=\"switchOnOffInput\"]")).Click();
            Thread.Sleep(1000);

            #region Settings

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxUncheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabHeaderActions");
            SelectOption("number:4", "[data-e2e=\"PosFormAction\"]");
            SelectOption("string:2", "[data-e2e=\"PostMessageRedirect\"]");
            SelectOption("number:1", "[data-e2e=\"SelectSalesFunnels\"]");
            Driver.CheckBoxUncheck("[data-e2e=\"DontSendLead\"]");
            SetInputText("EmailSubject", "MyEmailSubject1");
            SetIframeText("MyEmailText");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name1");
            FillLpFormField("89000000001", 1);
            FillLpFormField("email51@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/lp/test1/page1?lid="),
                "1-page1 url not contain lid data");

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderActions");
            Driver.CheckBoxCheck("[data-e2e=\"DontSendLead\"]");
            SelectOption("number:2", "[data-e2e=\"SelectSalesFunnels\"]");
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct1"});
            SetInputText("NewPriceProduct", "999");
            SetInputText("NewAmountProduct", "2");
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name2");
            FillLpFormField("89000000002", 1);
            FillLpFormField("email52@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/lp/test1/page1"),
                "2.1-page1 url");
            VerifyIsFalse(Driver.Url.Contains("?lid="),
                "2.2-page1 url contain lid data");

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderActions");
            SelectOption("object:null", "[data-e2e=\"PostMessageRedirect\"]");
            SetInputText("UrlRedirect", "products/test-product1");
            Driver.CheckBoxUncheck("[data-e2e=\"DontSendLead\"]");
            SetInputText("EmailSubject", "MyEmailSubject2");
            SetIframeText("Hello, #NAME#. Your phone is #PHONE#!");
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct1", "TestProduct10", "TestProduct100"});
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name3");
            FillLpFormField("89000000003", 1);
            FillLpFormField("email53@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/products/test-product1?lid="),
                "3-products/test-product1 url not contain lid data");

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderActions");
            Driver.CheckBoxCheck("[data-e2e=\"DontSendLead\"]");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name4");
            FillLpFormField("89000000004", 1);
            FillLpFormField("email54@test.ru", 2);
            SendForm();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/products/test-product1"),
                "4.1-products/test-product1-page1 url");
            VerifyIsFalse(Driver.Url.Contains("?lid="),
                "4.2-products/test-product1-page1 url contain lid data");

            #endregion

            #region Leads

            GoToAdmin("leads?salesFunnelId=1");
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".ui-grid-render-container-body [data-e2e=\"gridRow\"]")).Count == 1,
                "1-salesFunnel has not 1 leads");
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name1",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "1-LeadFirstName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]"))
                    .FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 1,
                "1-emails sent count != 1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"] [data-e2e=\"EventBlock-email\"]"))
                .FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("MyEmailSubject1",
                Driver.FindElement(By.CssSelector(".modal-body h2")).Text,
                "1 email-header text not expected");
            var textFrame = Driver.FindElement(By.CssSelector("iframe#emailBody"));
            Driver.SwitchTo().Frame(textFrame);
            VerifyAreEqual("MyEmailText",
                Driver.FindElement(By.TagName("body")).Text,
                "1 email-header text not expected");
            Driver.SwitchTo().DefaultContent();
            Driver.FindElement(By.ClassName("btn-cancel")).Click();
            DelLead();

            GoToAdmin("leads?salesFunnelId=2");
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".ui-grid-render-container-body [data-e2e=\"gridRow\"]")).Count == 3,
                "2-salesFunnel has not 3 leads");

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name4",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "4-LeadFirstName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]"))
                    .FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 1,
                "4-emails sent count != 1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"] [data-e2e=\"EventBlock-email\"]"))
                .FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("MyEmailSubject2",
                Driver.FindElement(By.CssSelector(".modal-body h2")).Text,
                "4-email-header text not expected");
            textFrame = Driver.FindElement(By.CssSelector("iframe#emailBody"));
            Driver.SwitchTo().Frame(textFrame);
            VerifyAreEqual("Hello, Name4. Your phone is 89000000004!",
                Driver.FindElement(By.TagName("body")).Text,
                "4-email-header text not expected");
            Driver.SwitchTo().DefaultContent();
            Driver.FindElement(By.ClassName("btn-cancel")).Click();
            VerifyIsTrue(LeadProductCount() == 0,
                "4-Lead product count not expected");
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name3",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "3-LeadFirstName");
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]"))
                    .FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 1,
                "3-emails sent count != 1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"] [data-e2e=\"EventBlock-email\"]"))
                .FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("MyEmailSubject2",
                Driver.FindElement(By.CssSelector(".modal-body h2")).Text,
                "3-email-header text not expected");
            textFrame = Driver.FindElement(By.CssSelector("iframe#emailBody"));
            Driver.SwitchTo().Frame(textFrame);
            VerifyAreEqual("Hello, Name3. Your phone is 89000000003!",
                Driver.FindElement(By.TagName("body")).Text,
                "3-email-header text not expected");
            Driver.SwitchTo().DefaultContent();
            Driver.FindElement(By.ClassName("btn-cancel")).Click();
            VerifyIsTrue(LeadProductCount() == 3,
                "3-Lead product count not expected");
            VerifyAreEqual("TestProduct1",
                Driver.GetGridCell(0, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "3-product1 name");
            VerifyAreEqual("100",
                Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "3-product1 price");
            VerifyAreEqual("TestProduct10",
                Driver.GetGridCell(1, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "3-product2 name");
            VerifyAreEqual("109",
                Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "3-product2 price");
            VerifyAreEqual("TestProduct100",
                Driver.GetGridCell(2, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "3-product3 name");
            VerifyAreEqual("199",
                Driver.GetGridCell(2, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "3-product3 price");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text == "408 руб.",
                "3-lead discount summary not expected");
            DelLead();

            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name2",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "2-LeadFirstName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]"))
                    .FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 1,
                "2-emails sent count != 1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"] [data-e2e=\"EventBlock-email\"]"))
                .FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("MyEmailSubject1",
                Driver.FindElement(By.CssSelector(".modal-body h2")).Text,
                "2-email-header text not expected");
            textFrame = Driver.FindElement(By.CssSelector("iframe#emailBody"));
            Driver.SwitchTo().Frame(textFrame);
            VerifyAreEqual("MyEmailText",
                Driver.FindElement(By.TagName("body")).Text,
                "2-email-header text not expected");
            Driver.SwitchTo().DefaultContent();
            Driver.FindElement(By.ClassName("btn-cancel")).Click();
            VerifyIsTrue(LeadProductCount() == 1,
                "2-Lead product count not expected");
            VerifyAreEqual("TestProduct1",
                Driver.GetGridCell(0, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "2-product1 name");
            VerifyAreEqual("999",
                Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "2-product3 price");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text == "1 998 руб.",
                "2-lead discount summary not expected");
            DelLead();

            #endregion

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxCheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            //GoToAdmin("settingsmail");
            //Thread.Sleep(1000);
            //driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
            //driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("Оформление заказа");
            //Thread.Sleep(1000);
            //driver.FindElement(By.CssSelector("li[index=\"'formats'\"]")).Click();
            //Driver.GetGridCell(0, "TypeName").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            //Thread.Sleep(1000);

            VerifyFinally(TestName);
        }

        [Test]
        public void FormAction_ShowMessage()
        {
            //   показать 1 сообщение, проверить 1 список, без товара.
            //   показать 2 сообщение, проверить 2 список, с 1 товаром.
            //   сообщение не важно(2, список не важен(2), с 3 товарами.
            TestName = "FormAction_ShowMessage";
            VerifyBegin(TestName);

            ReInit();

            #region Settings

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxUncheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabHeaderActions");
            SelectOption("number:1", "[data-e2e=\"PosFormAction\"]");
            SelectOption("number:1", "[data-e2e=\"SelectSalesFunnels\"]");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name1");
            FillLpFormField("89000000001", 1);
            FillLpFormField("email61@test.ru", 2);
            SendForm();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"]")).Count == 0,
                "1-form count after form send");
            VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                Driver.FindElement(By.CssSelector(".lp-form__content--success span")).Text,
                "1-form-success-message");

            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderActions");
            SetIframeText("Hello, it's test!");
            SelectOption("number:2", "[data-e2e=\"SelectSalesFunnels\"]");
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct1"});
            SetInputText("NewPriceProduct", "999");
            SetInputText("NewAmountProduct", "2");
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name2");
            FillLpFormField("89000000002", 1);
            FillLpFormField("email62@test.ru", 2);
            SendForm();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"]")).Count == 0,
                "2-form count after form send");
            VerifyAreEqual("Hello, it's test!",
                Driver.FindElement(By.CssSelector(".lp-form__content--success span")).Text,
                "2-form-success-message");

            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabFormSetting_0");
            SelectMultiProduct(new List<string> {"TestProduct1", "TestProduct10", "TestProduct100"});
            BlockSettingsSave();

            Thread.Sleep(1000);
            FillLpFormField("Name3");
            FillLpFormField("89000000003", 1);
            FillLpFormField("email63@test.ru", 2);
            SendForm();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockCapture + "\"]")).Count == 0,
                "3-form count after form send");
            VerifyAreEqual("Hello, it's test!",
                Driver.FindElement(By.CssSelector(".lp-form__content--success span")).Text,
                "3-form-success-message");

            #endregion

            #region Leads

            GoToAdmin("leads?salesFunnelId=1");
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".ui-grid-render-container-body [data-e2e=\"gridRow\"]")).Count == 1,
                "1-salesFunnel has not 1 leads");
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name1",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "1-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 0,
                "2.1-Lead product count not expected");
            DelLead();

            GoToAdmin("leads?salesFunnelId=2");
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".ui-grid-render-container-body [data-e2e=\"gridRow\"]")).Count == 2,
                "2-salesFunnel has not 2 leads");
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name3",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "3-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 3,
                "3-Lead product count not expected");
            VerifyAreEqual("TestProduct1",
                Driver.GetGridCell(0, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "3-product1 name");
            VerifyAreEqual("100",
                Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "3-product1 price");
            VerifyAreEqual("TestProduct10",
                Driver.GetGridCell(1, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "3-product2 name");
            VerifyAreEqual("109",
                Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "3-product2 price");
            VerifyAreEqual("TestProduct100",
                Driver.GetGridCell(2, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "3-product3 name");
            VerifyAreEqual("199",
                Driver.GetGridCell(2, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "3-product3 price");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text == "408 руб.",
                "3-lead discount summary not expected");
            DelLead();

            Thread.Sleep(1000);
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(500);
            VerifyAreEqual("Name2",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "2-LeadFirstName");
            VerifyIsTrue(LeadProductCount() == 1,
                "2-Lead product count not expected");
            VerifyAreEqual("TestProduct1",
                Driver.GetGridCell(0, "Name", "LeadItems").FindElement(By.TagName("a")).Text,
                "2-product1 name");
            VerifyAreEqual("999",
                Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.TagName("span")).Text,
                "2-product3 price");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text == "1 998 руб.",
                "2-lead discount summary not expected");
            DelLead();

            #endregion

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxCheck("[data-e2e=\"ShowAgreement\"]");
            TabFormSettingsClick("tabFormSetting_0");
            DelAllProduct();
            BlockSettingsSave();

            VerifyFinally(TestName);
        }

        [Test]
        public void FormFields()
        {
            TestName = "FormFields";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            #region text fields

            VerifyAreEqual("Оставьте заявку",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text,
                "1-TitleFormBlock text");
            VerifyAreEqual("Поспешите оставить заявку",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "1-SubTitleFormBlock text");
            VerifyAreEqual("Я согласен на обработку персональных данных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span:not(.lp-checkbox-decor)")).Text,
                "1-FormField agreement text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] " +
                                                           "[data-e2e=\"FormBtn\"] span.ladda-label")).Text
                    .Contains("Отправить"),
                "1-FormField button text");
            VerifyAreEqual("Имя",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                "1-FormField field name");
            VerifyAreEqual("Телефон",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                "1-FormField field phone");
            VerifyAreEqual("Email",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                "1-FormField field email");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__field-label:not(.hidden-xs)")).Count == 0,
                "1-Field-label count");

            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            SetInputText("SettingsFormTitle", "MyFormTitle");
            SetInputText("SettingsFormSubTitle", "MyFormSubTitle");
            SetInputText("FormButtonText", "ByFormButtonText");
            SelectOption("1", "[data-e2e=\"TextPosition\"]");
            SetInputText("AgreementText", "MyFormAgreementText");
            BlockSettingsCancel();
            Refresh();

            VerifyAreEqual("Оставьте заявку",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text,
                "1cansel-TitleFormBlock text");
            VerifyAreEqual("Поспешите оставить заявку",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "1cansel-SubTitleFormBlock text");
            VerifyAreEqual("Я согласен на обработку персональных данных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span:not(.lp-checkbox-decor)")).Text,
                "1cansel-FormField agreement text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] " +
                                                           "[data-e2e=\"FormBtn\"] span.ladda-label")).Text
                    .Contains("Отправить"),
                "1cansel-FormField button text");
            VerifyAreEqual("Имя",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                "1cansel-FormField field name");
            VerifyAreEqual("Телефон",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                "1cansel-FormField field phone");
            VerifyAreEqual("Email",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                "1cansel-FormField field email");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__field-label:not(.hidden-xs)")).Count == 0,
                "1cansel-Field-label count");

            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            SetInputText("SettingsFormTitle", "MyFormTitle");
            SetInputText("SettingsFormSubTitle", "MyFormSubTitle");
            SetInputText("FormButtonText", "ByFormButtonText");
            SelectOption("1", "[data-e2e=\"TextPosition\"]");
            SetInputText("AgreementText", "MyFormAgreementText");
            BlockSettingsSave();
            Thread.Sleep(1000);

            VerifyAreEqual("MyFormTitle",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text,
                "2-TitleFormBlock text");
            VerifyAreEqual("MyFormSubTitle",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "2-SubTitleFormBlock text");
            VerifyAreEqual("MyFormAgreementText",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span:not(.lp-checkbox-decor)")).Text,
                "2-FormField agreement text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] " +
                                                           "[data-e2e=\"FormBtn\"] span.ladda-label")).Text
                    .Contains("ByFormButtonText"),
                "2-FormField button text");
            VerifyAreEqual("",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                "2-FormField field name");
            VerifyAreEqual("",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                "2-FormField field phone");
            VerifyAreEqual("",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                "2-FormField field email");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__field-label:not(.hidden-xs)")).Count == 3,
                "2-Field-label count");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("MyFormTitle",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Text,
                "2d-TitleFormBlock text");
            VerifyAreEqual("MyFormSubTitle",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Text,
                "2d-SubTitleFormBlock text");
            VerifyAreEqual("MyFormAgreementText",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span:not(.lp-checkbox-decor)")).Text,
                "2d-FormField agreement text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture + "\"] " +
                                                           "[data-e2e=\"FormBtn\"] span.ladda-label")).Text
                    .Contains("ByFormButtonText"),
                "2d-FormField button text");
            VerifyAreEqual("",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                "2d-FormField field name");
            VerifyAreEqual("",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                "2d-FormField field phone");
            VerifyAreEqual("",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                "2d-FormField field email");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__field-label:not(.hidden-xs)")).Count == 3,
                "2d-Field-label count");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            SetInputText("SettingsFormTitle", "");
            SetInputText("SettingsFormSubTitle", "");
            SetInputText("FormButtonText", "");
            SelectOption("0", "[data-e2e=\"TextPosition\"]");
            SetInputText("AgreementText", "");
            BlockSettingsSave();
            Refresh();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 0,
                "3-TitleFormBlock text");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "3-SubTitleFormBlock text");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span:not(.lp-checkbox-decor)")).Text,
                "3-FormField agreement text");
            VerifyIsTrue(string.IsNullOrWhiteSpace(Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"] " +
                    "span.ladda-label")).Text),
                "3-FormField button text");
            VerifyAreEqual("Имя",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                "3-FormField field name");
            VerifyAreEqual("Телефон",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                "3-FormField field phone");
            VerifyAreEqual("Email",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                "3-FormField field email");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__field-label:not(.hidden-xs)")).Count == 0,
                "3-Field-label count");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 0,
                "3d-TitleFormBlock text");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 0,
                "3d-SubTitleFormBlock text");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockCapture +
                                                  "\"] .lp-form__agreement span:not(.lp-checkbox-decor)")).Text,
                "3d-FormField agreement text");
            VerifyIsTrue(string.IsNullOrWhiteSpace(Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"] " +
                    "span.ladda-label")).Text),
                "3d-FormField button text");
            VerifyAreEqual("Имя",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                "3d-FormField field name");
            VerifyAreEqual("Телефон",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                "3d-FormField field phone");
            VerifyAreEqual("Email",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                "3d-FormField field email");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__field-label:not(.hidden-xs)")).Count == 0,
                "3d-Field-label count");

            #endregion

            #region showAgreement

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.CheckBoxUncheck("[data-e2e=\"ShowAgreement\"]");
            BlockSettingsSave();
            Refresh();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__agreement")).Count == 0,
                "4-AgreementBlock");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__agreement")).Count == 0,
                "4d-AgreementBlock");

            #endregion

            #region formFields

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            FormDelFieldSave(3);
            FormEditFormName(0, "UserName");
            FormEditFormName(1, "UserCountry");
            FormEditFormFieldCrm(1, "Страна");
            FormEditFormNoRequired(1);
            FormAddField("UserPhone", "Телефон", true);
            FormAddField("UserLastName", "Фамилия", false);

            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__field input")).Count == 4,
                "5-Field-label count");
            VerifyAreEqual("UserName",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder"),
                "5-FormField field userName");
            VerifyAreEqual("UserCountry",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 1),
                "5-FormField field UserCountry");
            VerifyAreEqual("UserPhone",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 2),
                "5-FormField field UserPhone");
            VerifyAreEqual("UserLastName",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "placeholder",
                    "CssSelector", 3),
                "5-FormField field UserLastName");
            VerifyAreEqual("true",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "required"),
                "5-FormField field userName required");
            VerifyAreEqual(null,
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "required", "CssSelector",
                    1),
                "5-FormField field UserCountry required");
            VerifyAreEqual("true",
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "required", "CssSelector",
                    2),
                "5-FormField field UserPhone required");
            VerifyAreEqual(null,
                GetElAttribute("[data-e2e=\"" + blockCapture + "\"] .lp-form__field input", "required", "CssSelector",
                    3),
                "5-FormField field UserLastName required");


            SendForm();
            FillLpFormField("Ivan");
            FillLpFormField("Russia", 1);
            SendForm();
            FillLpFormField("+89049382928", 2);
            FillLpFormField("Ivanov", 3);
            SendForm();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 1,
                "5 - form sent");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            SetInputText("FormButtonText", "Отправить");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");

            FillLpFormField("+89049382928", 2);
            SendForm();
            FillLpFormField("Petr");
            SendForm();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 1,
                "5d - form sent");

            ReInit();
            GoToAdmin("leads?salesFunnelId=1");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"UseGrid\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("li[data-e2e=\"New\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("Ivan");
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Ivan",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "5.1-leadFirstName");
            VerifyAreEqual("Ivanov",
                GetElAttribute("Lead_Customer_LastName", "value", "Id"),
                "5.1-Lead_Customer_LastName");
            VerifyAreEqual("+89049382928",
                GetElAttribute("Lead_Customer_Phone", "value", "Id"),
                "5.1-Lead_Customer_Phone");
            VerifyIsTrue(GetElAttribute("Lead_Description", "value", "Id").Contains("Имя: Ivan"),
                "5.1-Comment not contains Name");
            VerifyIsTrue(GetElAttribute("Lead_Description", "value", "Id").Contains("Страна: Russia"),
                "5.1-Comment not contains Country");
            VerifyIsTrue(GetElAttribute("Lead_Description", "value", "Id").Contains("Телефон: +89049382928"),
                "5.1-Comment not contains Phone");
            VerifyIsTrue(GetElAttribute("Lead_Description", "value", "Id").Contains("Фамилия: Ivanov"),
                "5.1-Comment not contains LastName");
            VerifyIsTrue(GetElAttribute("Lead_Description", "value", "Id").Contains("Кнопка: "),
                "5.1-Comment not contains Button");
            DelLead();

            Driver.FindElement(By.CssSelector("li[data-e2e=\"New\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("Petr");
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Petr",
                GetElAttribute("Lead_Customer_FirstName", "value", "Id"),
                "5.2-leadFirstName");
            VerifyAreEqual("",
                GetElAttribute("Lead_Customer_LastName", "value", "Id"),
                "5.2-Lead_Customer_LastName");
            VerifyAreEqual("+89049382928",
                GetElAttribute("Lead_Customer_Phone", "value", "Id"),
                "5.2-Lead_Customer_Phone");
            VerifyIsTrue(GetElAttribute("Lead_Description", "value", "Id").Contains("Имя: Petr"),
                "5.2-Comment not contains Name");
            VerifyIsTrue(GetElAttribute("Lead_Description", "value", "Id").Contains("Телефон: +89049382928"),
                "5.2-Comment not contains Phone");
            VerifyIsTrue(GetElAttribute("Lead_Description", "value", "Id").Contains("Кнопка: Отправить"),
                "5.2-Comment not contains Button");
            DelLead();


            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ItemDel\"]")).Click();
            Thread.Sleep(500);
            Driver.SwalConfirm();
            Thread.Sleep(500);
            BlockSettingsSave();
            FormAddField("Имя", "Имя", true);
            FormAddField("Телефон", "Телефон", true);
            FormAddField("Email", "Email", true);
            Driver.CheckBoxCheck("[data-e2e=\"ShowAgreement\"]");
            BlockSettingsSave();

            #endregion

            VerifyFinally(TestName);
        }

        [Test]
        public void FormFieldsType()
        {
            TestName = "FormFieldsType";
            VerifyBegin(TestName);
            ReInit();
            GoToClient("lp/test1");

            Thread.Sleep(1000);
            BlockSettingsBtn();
            TabFormSettingsClick("tabHeaderForm");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ItemDel\"]")).Click();
            Thread.Sleep(500);
            Driver.SwalConfirm();
            Thread.Sleep(500);

            FormAddField("Select", "Customer Field 1", true);
            FormAddField("Text", "Customer Field 2", true);
            FormAddField("Number", "Customer Field 3", true);
            FormAddField("Textarea", "Customer Field 4", true);
            FormAddField("Data", "Customer Field 5", true);
            BlockSettingsSave();
            Thread.Sleep(1000);

            SelectOption("Customer Field 1 Value 2",
                "[data-e2e=\"FormField\"] [data-ng-model=\"lpForm.form.fields[0].value\"]");
            FillLpFormField("CustomerText1", 1);
            FillLpFormField("-123Text,13", 2);
            VerifyAreEqual("-123e13",
                GetElAttribute("[data-ng-model=\"lpForm.form.fields[2].value\"]"),
                "1-number input");
            FillLpFormField("Long multiline \r\n text", 3);
            FillLpFormField("11-02-2018", 4);
            SendForm();
            Thread.Sleep(500);
            Driver.CheckBoxCheck(".lp-form__agreement");
            SendForm();

            ReInitClient();
            GoToClient("lp/test1");
            SelectOption("Customer Field 1 Value 5",
                "[data-e2e=\"FormField\"] [data-ng-model=\"lpForm.form.fields[0].value\"]");
            FillLpFormField("CustomerText2 - `~!@#$%^&*()_+-=\\\"|?/><", 1);
            FillLpFormField("33.22", 2);
            FillLpFormField("Long multiline", 3);
            Driver.CheckBoxCheck(".lp-form__agreement");
            Driver.FindElement(
                By.CssSelector("[data-e2e=\"FormField\"] [data-ng-model=\"lpForm.form.fields[4].value\"]")).Click();
            Driver.FindElement(By.ClassName("flatpickr-calendar"))
                .FindElements(By.CssSelector(".flatpickr-day:not(.prevMonthDay)"))[5].Click();
            SendForm();


            ReInit();
            GoToAdmin("leads?salesFunnelId=1");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"UseGrid\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("li[data-e2e=\"New\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("CustomerText1");
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Customer Field 1 Value 2",
                GetElAttribute("#customerfields_0__value option[selected=\"selected\"]", "text"),
                "1-customerfields1");
            VerifyAreEqual("CustomerText1",
                GetElAttribute("customerfields_1__value", "value", "Id"),
                "1-customerfields2");
            VerifyAreEqual("-1230000000000000",
                GetElAttribute("customerfields_2__value", "value", "Id"),
                "1-customerfields3");
            VerifyAreEqual("Long multiline \r\n text",
                Driver.FindElement(By.Id("customerfields_3__value")).Text,
                "1-customerfields4");
            VerifyAreEqual("11.02.2018",
                GetElAttribute("customerfields_4_value", "value", "Id"),
                "1-customerfields5");
            DelLead();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("CustomerText2");
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "FullName").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Customer Field 1 Value 5",
                GetElAttribute("#customerfields_0__value option[selected=\"selected\"]", "text"),
                "2-customerfields1");
            VerifyAreEqual("CustomerText2 - `~!@#$%^&amp;amp;*()_+-=\\&amp;quot;|?/&amp;gt;&amp;lt;",
                GetElAttribute("customerfields_1__value", "value", "Id"), //?????????????????
                "2-customerfields2");
            VerifyAreEqual("33,22",
                GetElAttribute("customerfields_2__value", "value", "Id"),
                "2-customerfields3");
            VerifyAreEqual("Long multiline",
                Driver.FindElement(By.Id("customerfields_3__value")).Text,
                "2-customerfields4");
            VerifyAreEqual(DateTime.Now.ToString("06.MM.yyyy"),
                GetElAttribute("customerfields_4_value", "value", "Id"),
                "2-customerfields5");
            DelLead();


            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabFormSettingsClick("tabHeaderForm");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ItemDel\"]")).Click();
            Thread.Sleep(500);
            Driver.SwalConfirm();
            Thread.Sleep(500);
            BlockSettingsSave();
            FormAddField("Имя", "Имя", true);
            FormAddField("Телефон", "Телефон", true);
            FormAddField("Email", "Email", true);
            BlockSettingsSave();

            VerifyFinally(TestName);
        }
    }
}