using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem
{
    [TestFixture]
    public class BonusSystemPayInOrder : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\PayByBonusInOrder\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\PayByBonusInOrder\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\PayByBonusInOrder\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\PayByBonusInOrder\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\PayByBonusInOrder\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\PayByBonusInOrder\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\PayByBonusInOrder\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\PayByBonusInOrder\\Bonus.Card.csv",
                "Data\\Admin\\Settings\\BonusSystem\\PayByBonusInOrder\\Bonus.AdditionBonus.csv"
            );
            InitializeService.BonusSystemActive();
            Init();

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void PayByMainBonusInOrder()
        {
            GoToAdmin("orders/add");
            Driver.FindElement(By.LinkText("Выбрать покупателя")).Click();
            Driver.WaitForModal();

            Driver.GridFilterSendKeys("FirstName1 LastName1");
            Driver.XPathContainsText("h2", "Выбор покупателя");

            Driver.XPathContainsText("a", "Выбрать");
            Driver.WaitForToastSuccess();

            Driver.ScrollTo(By.Id("Order_OrderCustomer_Phone"));
            Driver.XPathContainsText("a", "Добавить товар");
            Driver.WaitForModal();

            Driver.GridFilterSendKeys("TestProduct4");
            Driver.XPathContainsText("h2", "Выбор товара");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();

            Driver.XPathContainsText("button", "Выбрать");
            Driver.WaitForToastSuccess();

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("530801", Driver.FindElement(By.CssSelector("[data-e2e=\"numberBonusCart\"]")).Text,
                "card num");
            VerifyAreEqual("100 (Гостевой 3 %)", Driver.FindElement(By.CssSelector("[data-e2e=\"percentBonus\"]")).Text,
                "card grade and percent");
            Driver.ScrollTo(By.CssSelector("[grid-unique-id=\"gridOrderItems\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"BonusPay\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BonusesAvailable\"]")).Text
                    .Contains("доступно: 100 бонусов"), "bonuses available");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BonusesUseAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BonusesUseAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BonusesUseAdd\"]")).SendKeys("60");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BonusesUse\"]")).Click();
            Driver.SwalConfirm();

            Refresh();

            //check order
            VerifyAreEqual("40 (Гостевой 3 %)", Driver.FindElement(By.CssSelector("[data-e2e=\"percentBonus\"]")).Text,
                "card num after bonus payment");
            VerifyAreEqual("- 60", Driver.FindElement(By.CssSelector("[data-e2e=\"BonusCost\"]")).Text,
                "bonus cost after bonus payment");
            VerifyAreEqual("340 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSum\"]")).Text,
                "order num after bonus payment");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"CountBonus\"]")).Text.Contains("10.20"),
                "bonus purchase amount");
            VerifyAreEqual("400", Driver.GetGridCell(0, "PriceString", "OrderItems").Text, "order item price");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "OrderItems").Text.Contains("400"), "order item cost");

            //check card purchase table
            GoToAdmin("cards/edit/2c8fb106-8f07-499b-b06f-51b43076f3c1");

            VerifyAreEqual("40 бонусов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).FindElement(By.TagName("span")).Text,
                "bonus count in card");
            VerifyAreEqual("40 основных и 0 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).FindElement(By.TagName("div")).Text,
                "all bonuses count in card");

            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseId\"]")).Text, "purchase PurchaseId");
            VerifyAreEqual("400",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseFullAmount\"]")).Text, "purchase full amount");
            VerifyAreEqual("400",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseDiscount\"]")).Text, "purchase PurchaseDiscount");
            VerifyAreEqual("340",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseCash\"]")).Text, "purchase PurchaseCash");
            VerifyAreEqual("60",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseMainBonuses\"]")).Text,
                "purchase PurchaseMainBonuses");
            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseAdditionBonuses\"]")).Text,
                "purchase PurchaseAdditionBonuses");
            VerifyAreEqual("10,20",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseAddNewBonuses\"]")).Text,
                "purchase PurchaseAddNewBonuses");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseStatus\"]")).Text.Contains("Ожидание платежа"),
                "purchase PurchaseStatus");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LinkToAllPurchases\"]")).FindElement(By.TagName("a"))
                    .GetAttribute("href").Contains("cards/allpurchase?CardId=2c8fb106-8f07-499b-b06f-51b43076f3c1"),
                "link to all purchases");

            //check card transaction table
            VerifyAreEqual("-60",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesUsed\"]")).Text,
                "Transaction MainBonuses Used");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesAdded\"]")).Text,
                "Transaction MainBonuses Added");
            VerifyAreEqual("40",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesSaldo\"]")).Text,
                "Transaction MainBonuses Saldo");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesUsed\"]")).Text,
                "Transaction AdditionBonuses Used");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesAdded\"]")).Text,
                "Transaction AdditionBonuses Added");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesSaldo\"]")).Text,
                "Transaction AdditionBonuses Saldo");
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseId\"]")).Text, "Transaction PurchaseId");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LinkToAllTransactions\"]")).FindElement(By.TagName("a"))
                    .GetAttribute("href").Contains("cards/alltransaction?CardId=2c8fb106-8f07-499b-b06f-51b43076f3c1"),
                "link to all Transactions");

            //check card all purchases table
            GoToAdmin("cards/allpurchase?CardId=2c8fb106-8f07-499b-b06f-51b43076f3c1");

            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseId\"]")).Text, "all purchase PurchaseId");
            VerifyAreEqual("400,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseFullAmount\"]")).Text, "all purchase full amount");
            VerifyAreEqual("400,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseDiscount\"]")).Text,
                "all purchase PurchaseDiscount");
            VerifyAreEqual("340,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseCash\"]")).Text, "all purchase PurchaseCash");
            VerifyAreEqual("60,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseMainBonuses\"]")).Text,
                "all purchase PurchaseMainBonuses");
            VerifyAreEqual("0,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseAdditionBonuses\"]")).Text,
                "all purchase PurchaseAdditionBonuses");
            VerifyAreEqual("10,20",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseAddNewBonuses\"]")).Text,
                "all purchase PurchaseAddNewBonuses");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseStatus\"]")).Text.Contains("Ожидание платежа"),
                "all purchase PurchaseStatus");

            //check card all transactions table
            GoToAdmin("cards/alltransaction?CardId=2c8fb106-8f07-499b-b06f-51b43076f3c1");

            VerifyAreEqual("-60,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesUsed\"]")).Text,
                "all Transaction MainBonuses Used");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesAdded\"]")).Text,
                "all Transaction MainBonuses Added");
            VerifyAreEqual("40,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesSaldo\"]")).Text,
                "all Transaction MainBonuses Saldo");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesUsed\"]")).Text,
                "all Transaction AdditionBonuses Used");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesAdded\"]")).Text,
                "all Transaction AdditionBonuses Added");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesSaldo\"]")).Text,
                "all Transaction AdditionBonuses Saldo");
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseId\"]")).Text, "all Transaction PurchaseId");
        }

        [Test]
        public void PayByAdditionBonusInOrder()
        {
            GoToAdmin("orders/add");
            Driver.FindElement(By.LinkText("Выбрать покупателя")).Click();
            Driver.WaitForModal();

            Driver.GridFilterSendKeys("FirstName8 LastName8");
            Driver.XPathContainsText("h2", "Выбор покупателя");

            Driver.XPathContainsText("a", "Выбрать");
            Driver.WaitForToastSuccess();

            Driver.ScrollTo(By.Id("Order_OrderCustomer_Phone"));
            Driver.XPathContainsText("a", "Добавить товар");
            Driver.WaitForModal();

            Driver.GridFilterSendKeys("TestProduct6");
            Driver.XPathContainsText("h2", "Выбор товара");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();

            Driver.XPathContainsText("button", "Выбрать");
            Driver.WaitForToastSuccess();

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("530808", Driver.FindElement(By.CssSelector("[data-e2e=\"numberBonusCart\"]")).Text,
                "card num");
            VerifyAreEqual("500 (Бронзовый 5 %)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"percentBonus\"]")).Text, "card grade and percent");
            Driver.ScrollTo(By.CssSelector("[grid-unique-id=\"gridOrderItems\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"BonusPay\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BonusesAvailable\"]")).Text
                    .Contains("доступно: 500 бонусов"), "bonuses available");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BonusesUseAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BonusesUseAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BonusesUseAdd\"]")).SendKeys("100");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BonusesUse\"]")).Click();
            Driver.SwalConfirm();

            Refresh();

            //check order
            VerifyAreEqual("400 (Бронзовый 5 %)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"percentBonus\"]")).Text, "card num after bonus payment");
            VerifyAreEqual("- 100", Driver.FindElement(By.CssSelector("[data-e2e=\"BonusCost\"]")).Text,
                "bonus cost after bonus payment");
            VerifyAreEqual("500 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSum\"]")).Text,
                "order num after bonus payment");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"CountBonus\"]")).Text.Contains("25.00"),
                "bonus purchase amount");
            VerifyAreEqual("600", Driver.GetGridCell(0, "PriceString", "OrderItems").Text, "order item price");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "OrderItems").Text.Contains("600"), "order item cost");

            //check addition bonuses table
            GoToAdmin("cards/edit/2c8fb106-8f07-499b-b06f-51b43076f3c8");

            VerifyAreEqual("AdditionBonus Name 8",
                Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionBonusId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"AdditionBonusName\"]")).Text, "AdditionBonus Name");
            VerifyAreEqual("400",
                Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionBonusId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"AdditionBonusAmount\"]")).Text, "AdditionBonus Amount");
            VerifyAreEqual("Description8",
                Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionBonusId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"AdditionBonusDescription\"]")).Text,
                "AdditionBonus Description");
            VerifyAreEqual("05 05 2016",
                Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionBonusId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"AdditionBonusStartDate\"]")).Text,
                "AdditionBonus StartDate");
            VerifyAreEqual("05 05 2050",
                Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionBonusId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"AdditionBonusEndDate\"]")).Text, "AdditionBonus EndDate");

            //check card purchase table
            VerifyAreEqual("400 бонусов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).FindElement(By.TagName("span")).Text,
                "bonus count in card");
            VerifyAreEqual("0 основных и 400 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).FindElement(By.TagName("div")).Text,
                "all bonuses count in card");

            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseId\"]")).Text, "purchase PurchaseId");
            VerifyAreEqual("600",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseFullAmount\"]")).Text, "purchase full amount");
            VerifyAreEqual("600",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseDiscount\"]")).Text, "purchase PurchaseDiscount");
            VerifyAreEqual("500",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseCash\"]")).Text, "purchase PurchaseCash");
            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseMainBonuses\"]")).Text,
                "purchase PurchaseMainBonuses");
            VerifyAreEqual("100",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseAdditionBonuses\"]")).Text,
                "purchase PurchaseAdditionBonuses");
            VerifyAreEqual("25",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseAddNewBonuses\"]")).Text,
                "purchase PurchaseAddNewBonuses");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseStatus\"]")).Text.Contains("Ожидание платежа"),
                "purchase PurchaseStatus");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LinkToAllPurchases\"]")).FindElement(By.TagName("a"))
                    .GetAttribute("href").Contains("cards/allpurchase?CardId=2c8fb106-8f07-499b-b06f-51b43076f3c8"),
                "link to all purchases");

            //check card transaction table
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesUsed\"]")).Text,
                "Transaction MainBonuses Used");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesAdded\"]")).Text,
                "Transaction MainBonuses Added");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesSaldo\"]")).Text,
                "Transaction MainBonuses Saldo");
            VerifyAreEqual("-100",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesUsed\"]")).Text,
                "Transaction AdditionBonuses Used");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesAdded\"]")).Text,
                "Transaction AdditionBonuses Added");
            VerifyAreEqual("400",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesSaldo\"]")).Text,
                "Transaction AdditionBonuses Saldo");
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseId\"]")).Text, "Transaction PurchaseId");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LinkToAllTransactions\"]")).FindElement(By.TagName("a"))
                    .GetAttribute("href").Contains("cards/alltransaction?CardId=2c8fb106-8f07-499b-b06f-51b43076f3c8"),
                "link to all Transactions");

            //check card all purchases table
            GoToAdmin("cards/allpurchase?CardId=2c8fb106-8f07-499b-b06f-51b43076f3c8");

            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseId\"]")).Text, "all purchase PurchaseId");
            VerifyAreEqual("600,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseFullAmount\"]")).Text, "all purchase full amount");
            VerifyAreEqual("600,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseDiscount\"]")).Text,
                "all purchase PurchaseDiscount");
            VerifyAreEqual("500,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseCash\"]")).Text, "all purchase PurchaseCash");
            VerifyAreEqual("0,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseMainBonuses\"]")).Text,
                "all purchase PurchaseMainBonuses");
            VerifyAreEqual("100,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseAdditionBonuses\"]")).Text,
                "all purchase PurchaseAdditionBonuses");
            VerifyAreEqual("25,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseAddNewBonuses\"]")).Text,
                "all purchase PurchaseAddNewBonuses");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"PurchaseId-2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseStatus\"]")).Text.Contains("Ожидание платежа"),
                "all purchase PurchaseStatus");

            //check card all transactions table
            GoToAdmin("cards/alltransaction?CardId=2c8fb106-8f07-499b-b06f-51b43076f3c8");

            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesUsed\"]")).Text,
                "all Transaction MainBonuses Used");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesAdded\"]")).Text,
                "all Transaction MainBonuses Added");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionMainBonusesSaldo\"]")).Text,
                "all Transaction MainBonuses Saldo");
            VerifyAreEqual("-100,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesUsed\"]")).Text,
                "all Transaction AdditionBonuses Used");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesAdded\"]")).Text,
                "all Transaction AdditionBonuses Added");
            VerifyAreEqual("400,00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"TransactionAdditionBonusesSaldo\"]")).Text,
                "all Transaction AdditionBonuses Saldo");
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TransactionId-1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"PurchaseId\"]")).Text, "all Transaction PurchaseId");
        }
    }
}