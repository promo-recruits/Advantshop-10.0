using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadAddEditProductsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadItem.csv"
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
        public void LeadAddSumEditable()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("sum editable");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71231212923");

            VerifyAreEqual("Нет товаров", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadNoProducts\"]")).Text);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"LeadAddItemsTable\"]")).Count > 0);

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check admin
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetGridFilterTab(0, "sum editable");

            VerifyAreEqual("sum editable", Driver.GetGridCell(0, "FullName").Text);
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            IsElementNotPresent(By.CssSelector("[data-e2e=\"Lead_Sum\"]"), "readonly"); //можно редактировать вручную 
        }

        [Test]
        public void LeadAddSumNotEditable()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("sum not editable");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("72312129233");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check admin
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetGridFilterTab(0, "sum not editable");

            VerifyAreEqual("sum not editable", Driver.GetGridCell(0, "FullName").Text);
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Lead_Sum\"]")).GetAttribute("readonly")
                .Equals("true")); //нельзя редактировать вручную 
        }

        [Test]
        public void LeadAdd()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).SendKeys("WithProductLastName");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("WithProductFirstName");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).SendKeys("WithProductPatronymic");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71231212143");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).SendKeys("mailtest123@mail.ru");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "selected items count");

            Driver.XPathContainsText("button", "Выбрать");

            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"LeadNoProducts\"]")).Count > 0,
                "products added");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"LeadAddItemsTable\"]")).Count > 0,
                "products table presents");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"LeadItemArtNoName\"]"))[0].Text
                    .Contains("TestProduct1"), "product 1 added");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"LeadItemArtNoName\"]"))[1].Text
                    .Contains("TestProduct2"), "product 2 added");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"LeadItemPrice\"]"))[0].Text.Contains("1"),
                "product 1 price added");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"LeadItemPrice\"]"))[1].Text.Contains("2"),
                "product 2 price added");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check admin lead details
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetGridFilterTab(0, "WithProductLastName");

            VerifyAreEqual("WithProductLastName WithProductFirstName WithProductPatronymic",
                Driver.GetGridCell(0, "FullName").Text, "lead added full name");
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "lead details products added");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct1"), "product 1 grid");
            VerifyIsTrue(Driver.GetGridCell(1, "Name", "LeadItems").Text.Contains("TestProduct2"), "product 2 grid");

            VerifyAreEqual("1", Driver.GetGridCell(0, "Price", "LeadItems").Text, "product 1 Price grid");
            VerifyAreEqual("2", Driver.GetGridCell(1, "Price", "LeadItems").Text, "product 2 Price grid");

            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").Text, "product 1 Amount grid");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "LeadItems").Text, "product 2 Amount grid");

            // VerifyAreEqual("в наличии", Driver.GetGridCell(0, "Available", "LeadItems").Text, "product 1 Available grid");
            // VerifyAreEqual("в наличии", Driver.GetGridCell(1, "Available", "LeadItems").Text, "product 2 Available grid");

            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "Cost", "LeadItems").Text, "product 1 Cost grid");
            VerifyAreEqual("2 руб.", Driver.GetGridCell(1, "Cost", "LeadItems").Text, "product 2 Cost grid");
        }

        [Test]
        public void LeadAddDeleteItems()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).SendKeys("LastNameDelItem");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("FirstNameDelItem");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).SendKeys("PatronymicDelItem");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71237712143");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).SendKeys("mailtqweest123@mail.ru");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"LeadNoProducts\"]")).Count > 0,
                "products added");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"LeadAddItemsTable\"]")).Count > 0,
                "products table presents");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemRemove\"]")).Click();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadNoProducts\"]")).Text.Contains("Нет товаров"),
                "products deleted");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"LeadAddItemsTable\"]")).Count > 0,
                "no table - products deleted");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check admin
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetGridFilterTab(0, "LastNameDelItem");

            VerifyAreEqual("LastNameDelItem FirstNameDelItem PatronymicDelItem", Driver.GetGridCell(0, "FullName").Text,
                "lead added full name");
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "lead no products");
        }

        [Test]
        public void LeadEditDeleteProducts()
        {
            GoToAdmin("leads#?leadIdInfo=10");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct10"),
                "pre check product lead");
            var attr = Driver.FindElement(By.CssSelector("[data-e2e=\"Lead_Sum\"]")).GetAttribute("readonly");
            VerifyIsTrue(attr.Equals("true"), "pre check sum not editable");

            Driver.GetGridCell(0, "_serviceColumn", "LeadItems").FindElement(By.TagName("a")).Click();
            Driver.SwalConfirm();

            GoToAdmin("leads#?leadIdInfo=10");

            //check lead history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();

            VerifyIsTrue(Driver.PageSource.Contains("Удален товар TestProduct10 (10)"), "lead deleted product history");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".crm-old-status")).Text.Contains("100"),
                "lead deleted product sum old history");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".crm-new-status")).Text.Contains("0"),
                "lead deleted product sum new history");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "products deleted");
            IsElementNotPresent(By.CssSelector("[data-e2e=\"Lead_Sum\"]"), "readonly"); //можно редактировать вручную 
        }

        [Test]
        public void LeadEditDeleteNotAllProducts()
        {
            GoToAdmin("leads#?leadIdInfo=1");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct1"),
                "pre check product 1 lead");
            VerifyIsTrue(Driver.GetGridCell(1, "Name", "LeadItems").Text.Contains("TestProduct2"),
                "pre check product 2 lead");
            var attr = Driver.FindElement(By.CssSelector("[data-e2e=\"Lead_Sum\"]")).GetAttribute("readonly");
            VerifyIsTrue(attr.Equals("true"), "pre check sum not editable");

            Driver.GetGridCell(0, "_serviceColumn", "LeadItems").FindElement(By.TagName("a")).Click();
            Driver.SwalConfirm();

            GoToAdmin("leads#?leadIdInfo=1");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct2"),
                "product 2 not deleted");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "products not all deleted");
            attr = Driver.FindElement(By.CssSelector("[data-e2e=\"Lead_Sum\"]")).GetAttribute("readonly");
            VerifyIsTrue(attr.Equals("true"), "sum not editable");

            //check lead history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-history\"]"))[1]
                    .FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text
                    .Contains("Удален товар TestProduct1 (1)"), "lead deleted product history");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-history\"]"))[0]
                    .FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]"))
                    .FindElement(By.CssSelector(".crm-old-status")).Text.Contains("5"),
                "lead deleted product sum old history");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-history\"]"))[0]
                    .FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]"))
                    .FindElement(By.CssSelector(".crm-new-status")).Text.Contains("4"),
                "lead deleted product sum new history");
        }

        [Test]
        public void LeadEditProductsAdd()
        {
            GoToAdmin("leads#?leadIdInfo=70");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct70"),
                "pre check lead product grid name");
            VerifyAreEqual("70", Driver.GetGridCell(0, "Amount", "LeadItems").Text,
                "pre check lead product grid amount");
            VerifyAreEqual("4900", Driver.FindElement(By.CssSelector("[data-e2e=\"Lead_Sum\"]")).GetAttribute("value"),
                "pre check lead sum");

            Driver.FindElement(By.CssSelector("[ data-e2e=\"LeadProductAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.XPathContainsText("span", "TestCategory3");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.XPathContainsText("button", "Выбрать");

            GoToAdmin("leads#?leadIdInfo=70");

            VerifyIsTrue(Driver.GetGridCell(1, "Name", "LeadItems").Text.Contains("TestProduct41"),
                "lead added product grid name");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "LeadItems").Text, "lead added product grid amount");
            VerifyAreEqual("4941", Driver.FindElement(By.CssSelector("[data-e2e=\"Lead_Sum\"]")).GetAttribute("value"),
                "lead sum after adding product");

            var attr = Driver.FindElement(By.CssSelector("[data-e2e=\"Lead_Sum\"]")).GetAttribute("readonly");
            VerifyIsTrue(attr.Equals("true")); //нельзя редактировать вручную 

            //check lead history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-history\"]"))[1]
                    .FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text
                    .Contains("Добавлен товар TestProduct41 (41)"), "lead added product history");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-history\"]"))[0]
                    .FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]"))
                    .FindElement(By.CssSelector(".crm-old-status")).Text.Contains("4900"),
                "lead added product sum old history");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-history\"]"))[0]
                    .FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]"))
                    .FindElement(By.CssSelector(".crm-new-status")).Text.Contains("4941"),
                "lead added product sum new history");
        }

        [Test]
        public void LeadEditProductsCount()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("LeadEditProductsCount");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71223423423431212923");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetGridFilterTab(0, "LeadEditProductsCount");

            VerifyAreEqual("LeadEditProductsCount", Driver.GetGridCell(0, "FullName").Text, "lead added full name");

            //check admin lead details
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct1"),
                "pre check lead product grid name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").Text,
                "pre check lead product grid amount");
            //VerifyAreEqual("в наличии", Driver.GetGridCell(0, "Available", "LeadItems").Text, "pre check lead product Available grid");

            var attrAvailable = Driver.FindElement(By.CssSelector("[data-e2e=\"Lead_Sum\"]")).GetAttribute("readonly");
            VerifyIsTrue(attrAvailable.Equals("true"), "no print editing"); //нельзя редактировать вручную 

            //check products count 0
            Driver.SendKeysGridCell("0", 0, "Amount", "LeadItems");

            //VerifyAreEqual("в наличии", Driver.GetGridCell(0, "Available", "LeadItems").Text, "lead product available count 0 grid");
            VerifyAreEqual("0", Driver.GetGridCell(0, "Amount", "LeadItems").Text,
                "lead product available count 0 amount grid");
            IsElementNotPresent(By.CssSelector("[data-e2e=\"Lead_Sum\"]"), "readonly"); //можно редактировать вручную 
        }


        [Test]
        public void LeadEditProductsAddBySearch()
        {
            GoToAdmin("leads#?leadIdInfo=25");

            Driver.FindElement(By.CssSelector("[ data-e2e=\"LeadProductAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.GridFilterSendKeys("TestProduct55");

            Driver.XPathContainsText("h2", "Выбор товара");

            VerifyAreEqual("TestProduct55", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "modal add product by search");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.XPathContainsText("button", "Выбрать");

            GoToAdmin("leads#?leadIdInfo=25");

            VerifyIsTrue(Driver.GetGridCell(1, "Name", "LeadItems").Text.Contains("TestProduct55"),
                "lead product grid added by search");

            var attrAvailable = Driver.FindElement(By.CssSelector("[data-e2e=\"Lead_Sum\"]")).GetAttribute("readonly");
            VerifyIsTrue(attrAvailable.Equals("true")); //нельзя редактировать вручную 
        }

        [Test]
        public void LeadAddCustomerOrganization()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("customer name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71231212977");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadOrganization\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadOrganization\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadOrganization\"]")).SendKeys("Lead Organization");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory3");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check admin lead
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetGridFilterTab(0, "Lead Organization");
            VerifyAreEqual("customer name (Lead Organization)", Driver.GetGridCell(0, "FullName").Text,
                "search by organization in lead grid");

            Driver.GetGridFilterTab(0, "customer name");

            VerifyAreEqual("customer name (Lead Organization)", Driver.GetGridCell(0, "FullName").Text,
                "search by customer name lead grid");
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            VerifyAreEqual("customer name", Driver.FindElement(By.Id("Lead_FirstName")).GetAttribute("value"),
                "lead customer name");
            VerifyAreEqual("Lead Organization", Driver.FindElement(By.Id("Lead_Organization")).GetAttribute("value"),
                "lead customer Organization");
            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct41"), "lead product");

            //check admin customer
            GoToAdmin("customers");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCustomers\"]")).Text.Contains("customer name"),
                "no name in customer grid");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCustomers\"]")).Text
                    .Contains("Lead Organization"), "organization name in grid");

            Driver.GridFilterSendKeys("Lead Organization");
            VerifyAreEqual("Lead Organization", Driver.GetGridCell(0, "Name", "Customers").Text,
                "grid customer organization");

            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));

            VerifyAreEqual("Lead Organization",
                Driver.FindElement(By.Id("Customer_Organization")).GetAttribute("value"), "customer organization name");
        }
    }

    [TestFixture]
    public class CRMLeadAddEditDiscountTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadItem.csv"
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
        public void LeadEditDiscountAddPercent()
        {
            GoToAdmin("leads#?leadIdInfo=60");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAddLink\"]")).Text.Contains("Добавить скидку"),
                "no discount text add");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAddLink\"]")).Text.Contains("Скидка"),
                "no discount text saved");

            Driver.FindElement(By.LinkText("Добавить скидку")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountPercent\"]")).Click();

            Driver.FindElement(By.Name("orderFormDiscount")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("orderFormDiscount")).FindElement(By.TagName("input")).Clear();
            Driver.FindElement(By.Name("orderFormDiscount")).FindElement(By.TagName("input")).SendKeys("50");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountAdd\"]")).Click();

            GoToAdmin("leads?salesFunnelId=1#?leadIdInfo=60");
            VerifyAreEqual("- 1 800 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscount\"]")).Text,
                "lead discount added saved");
            VerifyAreEqual("(50%)", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountPercentAdded\"]")).Text,
                "lead discount percent added");
            VerifyAreEqual("1 800 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text,
                "lead summary");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAddLink\"]")).Text.Contains("Добавить скидку"),
                "lead discount added text add");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAddLink\"]")).Text.Contains("Скидка"),
                "lead discount added text save");
        }

        [Test]
        public void LeadAddDiscountAddNumber()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).SendKeys("LastNameAddDiscount");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("FirstNameAddDiscount");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).SendKeys("PatronymicAddDiscount");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71237712143");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).SendKeys("mailtqweesdiscountt123@mail.ru");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.FindElement(By.Id("4")).FindElement(By.CssSelector(".jstree-icon.jstree-ocl")).Click();
            Driver.XPathContainsText("span", "TestCategory5");

            Driver.GridFilterSendKeys("TestProduct100");
            Driver.XPathContainsText("h2", "Выбор товара");

            VerifyAreEqual("TestProduct100", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "modal add product");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check admin
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetGridFilterTab(0, "FirstNameAddDiscount");

            VerifyAreEqual("LastNameAddDiscount FirstNameAddDiscount PatronymicAddDiscount",
                Driver.GetGridCell(0, "FullName").Text);
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAddLink\"]")).Text.Contains("Добавить скидку"),
                "no discount text add");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAddLink\"]")).Text.Contains("Скидка"),
                "no discount text saved");

            Driver.FindElement(By.LinkText("Добавить скидку")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountNumber\"]")).Click();

            Driver.FindElement(By.Name("orderFormDiscount")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("orderFormDiscount")).FindElement(By.TagName("input")).Clear();
            Driver.FindElement(By.Name("orderFormDiscount")).FindElement(By.TagName("input")).SendKeys("80");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountAdd\"]")).Click();

            Refresh();

            VerifyAreEqual("- 80 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscount\"]")).Text,
                "lead discount added saved");
            VerifyAreEqual("20 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDiscountSummary\"]")).Text,
                "lead summary");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAddLink\"]")).Text.Contains("Добавить скидку"),
                "lead discount added text add");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountAddLink\"]")).Text.Contains("Скидка"),
                "lead discount added text save");
        }

        [Test]
        public void LeadAddExistedCustomer()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".modal-content")).Text.Contains("Выбран покупатель"),
                "no customer added");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).SendKeys("Last");

            Driver.XPathContainsText("span", "LastName FirstName Patronymic, mail@mail.com +7 495 800 20 01");

            Driver.XPathContainsText("h2", "Новый лид");

            VerifyAreEqual("LastName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).GetAttribute("value"),
                "existing customer LastName");
            VerifyAreEqual("FirstName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).GetAttribute("value"),
                "existing customer FirstName");
            VerifyAreEqual("Patronymic",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).GetAttribute("value"),
                "existing customer Patronymic");
            VerifyAreEqual("+7(495)800-20-01",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).GetAttribute("value"),
                "existing customer PhoneNum");
            VerifyAreEqual("mail@mail.com",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).GetAttribute("value"),
                "existing customer Email");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".modal-content"))
                    .FindElement(By.CssSelector("[data-e2e=\"LeadCustomer\"]")).Text
                    .Contains("FirstName LastName 74958002001 mail@mail.com"), "customer info added");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-content")).Text.Contains("Выбран покупатель"),
                "customer added");

            //add products
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            GoToAdmin("leads?salesFunnelId=-1");
            Driver.GetGridFilterTab(0, "LastName FirstName Patronymic");
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            VerifyAreEqual("LastName", Driver.FindElement(By.Id("Lead_LastName")).GetAttribute("value"),
                "lead customer first name");
            VerifyAreEqual("FirstName", Driver.FindElement(By.Id("Lead_FirstName")).GetAttribute("value"),
                "lead customer last name");
            VerifyAreEqual("Patronymic", Driver.FindElement(By.Id("Lead_Patronymic")).GetAttribute("value"),
                "lead customer patronymic");
            VerifyAreEqual("mail@mail.com", Driver.FindElement(By.Id("Lead_Email")).GetAttribute("value"),
                "lead customer email");
            VerifyAreEqual("+7(495)800-20-01", Driver.FindElement(By.Id("Lead_Phone")).GetAttribute("value"),
                "lead customer phone");

            Driver.FindElement(By.LinkText("Карточка клиента")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            VerifyIsTrue(Driver.WindowHandles.Count.Equals(2), "count tabs");

            //check customer cart lead grid
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Click();
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName", "CustomerLeads").Text,
                "customer cart grid lead deal status");
            VerifyAreEqual("LastName FirstName Patronymic (Organization Test)",
                Driver.GetGridCell(0, "FullName", "CustomerLeads").Text, "customer cart grid lead full name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Sum", "CustomerLeads").Text, "customer cart grid lead sum");

            Driver.FindElement(By.LinkText("Редактировать")).Click();
            Driver.WaitForElem(By.ClassName("order-header-item-name"));

            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "customer edit tab opened");

            //check customer edit
            VerifyAreEqual("LastName", Driver.FindElement(By.Id("Customer_LastName")).GetAttribute("value"),
                "customer first name edit");
            VerifyAreEqual("FirstName", Driver.FindElement(By.Id("Customer_FirstName")).GetAttribute("value"),
                "customer last name edit");
            VerifyAreEqual("Patronymic", Driver.FindElement(By.Id("Customer_Patronymic")).GetAttribute("value"),
                "customer patronymic edit");
            VerifyAreEqual("mail@mail.com", Driver.FindElement(By.Id("Customer_EMail")).GetAttribute("value"),
                "customer email edit");
            VerifyAreEqual("+7(495)800-20-01", Driver.FindElement(By.Id("Customer_Phone")).GetAttribute("value"),
                "customer phone edit");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadAddCustomerDelete()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).SendKeys("Last");

            Driver.XPathContainsText("span", "LastName FirstName Patronymic, mail@mail.com +7 495 800 20 01");

            Driver.XPathContainsText("h2", "Новый лид");

            VerifyAreEqual("LastName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).GetAttribute("value"),
                "existing customer LastName");
            VerifyAreEqual("FirstName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).GetAttribute("value"),
                "existing customer FirstName");
            VerifyAreEqual("Patronymic",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).GetAttribute("value"),
                "existing customer Patronymic");
            VerifyAreEqual("+7(495)800-20-01",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).GetAttribute("value"),
                "existing customer PhoneNum");
            VerifyAreEqual("mail@mail.com",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).GetAttribute("value"),
                "existing customer Email");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".modal-content"))
                    .FindElement(By.CssSelector("[data-e2e=\"LeadCustomer\"]")).Text
                    .Contains("FirstName LastName 74958002001 mail@mail.com"), "customer added info");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadCustomerDelete\"]")).Click();

            //check delete customer
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).GetAttribute("value"),
                "existing customer LastName deleted");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).GetAttribute("value"),
                "existing customer FirstName deleted");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).GetAttribute("value"),
                "existing customer Patronymic deleted");
            VerifyAreEqual("+_(___)___-__-__",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).GetAttribute("value"),
                "existing customer PhoneNum deleted");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).GetAttribute("value"),
                "existing customer Email deleted");
        }
    }

    [TestFixture]
    public class CRMLeadAddEditManagersTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadItem.csv",
                "data\\Admin\\CRM\\Lead\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadEvent.csv"
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
        public void LeadAddNoProduct()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Новый лид", Driver.FindElement(By.CssSelector(".modal-header-title")).Text,
                "h1 lead modal");
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).SendKeys("NoProductLastName");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("NoProductFirstName");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).SendKeys("NoProductPatronymic");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71231212123");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).SendKeys("mailtest@mail.ru");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            VerifyAreEqual("Нет товаров", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadNoProducts\"]")).Text,
                "no products at first");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"LeadAddItemsTable\"]")).Count > 0,
                "no products table at first");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).SendKeys("Description Test");

            VerifyAreEqual("0", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadSum\"]")).GetAttribute("value"),
                "lead sum no products");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadCurrency\"]")).Text.Contains("руб."),
                "currency");

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDealStatus\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.AllSelectedOptions[0].Text.Contains("Новый"), "selected new status at first");

            //check all lead statuses
            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDealStatus\"]"));
            select = new SelectElement(selectElem);
            IList<IWebElement> allOptionsLeadStatus = select.Options;
            VerifyIsTrue(allOptionsLeadStatus.Count == 6, "count all lead statuses");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDealStatus\"]")))).SelectByText(
                "Созвон с клиентом");

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"LeadManager\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.AllSelectedOptions[0].Text.Contains("Выберите менеджера"), "no manager at first");

            //check all managers
            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"LeadManager\"]"));
            select = new SelectElement(selectElem);
            IList<IWebElement> allOptionsLeadManager = select.Options;
            VerifyIsTrue(allOptionsLeadManager.Count == 3, "count all 2 lead managers + null manager");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadManager\"]")))).SelectByText(
                "test testov");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check admin grid
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetGridFilterTab(0, "NoProductLastName");

            VerifyAreEqual("121", Driver.GetGridCell(0, "Id").Text, "lead added number");
            VerifyAreEqual("Созвон с клиентом", Driver.GetGridCell(0, "DealStatusName").Text,
                "lead added deal status name");
            VerifyAreEqual("NoProductLastName NoProductFirstName NoProductPatronymic",
                Driver.GetGridCell(0, "FullName").Text, "lead added full name");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text, "lead added manager name");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "lead added products count");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "lead added sum");

            //check admin lead details
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.AllSelectedOptions[0].Text.Contains("Созвон с клиентом"), "lead added deal status");

            VerifyAreEqual("Description Test", Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value"),
                "lead added description");
            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead added title");
            VerifyAreEqual("0", Driver.FindElement(By.CssSelector("[data-e2e=\"Lead_Sum\"]")).GetAttribute("value"),
                "lead added sum");

            selectElem = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.AllSelectedOptions[0].Text.Contains("test testov"), "lead added manager");

            selectElem = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            select = new SelectElement(selectElem);

            VerifyIsTrue(select.AllSelectedOptions[0].Text.Contains("Другое"), "lead added order source");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTasks\"]")).Displayed,
                "lead added no tasks");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "lead added no products");
        }

        [Test]
        [Order(1)]
        public void LeadEditManager()
        {
            GoToAdmin("leads#?leadIdInfo=119");

            IWebElement selectElem1 = Driver.FindElement(By.Id("Lead_ManagerId"));
            SelectElement select1 = new SelectElement(selectElem1);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("test testov"), "pre check lead manager");

            (new SelectElement(Driver.FindElement(By.Id("Lead_ManagerId")))).SelectByText("Elena El");
            Driver.WaitForToastSuccess();

            GoToAdmin("leads#?leadIdInfo=119");

            IWebElement selectElem2 = Driver.FindElement(By.Id("Lead_ManagerId"));
            SelectElement select2 = new SelectElement(selectElem2);
            VerifyIsTrue(select2.SelectedOption.Text.Contains("Elena El"), "lead manager edited");
        }
    }
}