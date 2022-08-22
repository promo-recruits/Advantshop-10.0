using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.Card
{
    [TestFixture]
    public class BonusSystemCardAdd : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\CardsBonusToAll\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Bonus.Card.csv"
            );
            InitializeService.BonusSystemActive();
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
        public void AddNewCard()
        {
            GoToAdmin("cards");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCard\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".edit")).Click();
            Thread.Sleep(500);

            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"selectUserGrid[0]['_serviceColumn']\"]")).Click();
            Thread.Sleep(100);
            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-9 select")))).SelectByText("Серебряный");
            Driver.FindElement(By.CssSelector("[data-e2e=\"addCard\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveCard"));

            VerifyAreEqual("Testsurename TestName", Driver.FindElement(By.TagName("strong")).Text, " card title h1");
            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"gradeBonus\"] select"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Серебряный"), "select item Gost");

            VerifyAreEqual("0 бонусов\r\n0 основных и 0 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus")).Text, " in str card");
            VerifyAreEqual("Testsurename", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardSuname\"]")).Text,
                " bonusCardSuname in str card");
            VerifyAreEqual("TestName", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardName\"]")).Text,
                " bonusCardName in str card");
            VerifyAreEqual("999999", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardPhone\"]")).Text,
                " bonusCardPhone in str card");
            VerifyAreEqual("test@mail.ru", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardMail\"]")).Text,
                " bonusCardMail in str card");

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElements(By.TagName("td"))[0].Text, " no bonus1");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElements(By.TagName("td"))[1].Text, " no bonus2");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElements(By.TagName("td"))[1].Text, " no bonus3");

            //Check client cart
            Driver.FindElement(By.CssSelector(".edit.link-decoration-none")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            VerifyAreEqual("Testsurename TestName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientName\"]")).Text, " client cart title h1");
            VerifyAreEqual("Грейд: Серебряный 7%", Driver.FindElement(By.CssSelector("[data-e2e=\"GradeCart\"]")).Text,
                " client cart type card");
        }

        [Test]
        public void EditCardAddMainBonus()
        {
            GoToAdmin("cards/edit/2c8fb106-8f07-499b-b06f-51b43076f3cf");
            //fhdghfghfg
            Driver.FindElement(By.CssSelector("[data-e2e=\"MainBonus\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"MainBonusAdd\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).SendKeys("1000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).SendKeys("Reason");

            Driver.FindElement(By.CssSelector("[data-e2e=\"bonusAdd\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text
                    .Contains("1 000 основных и 0 дополнительных"), "card additional bonuses added");

            VerifyAreEqual("+ 1 000",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[2].Text,
                " count bonus  table transaction");
            VerifyAreEqual("1 000",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[3].Text,
                "  saldo table transaction");
            VerifyAreEqual("Reason",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[7].Text,
                " number sell table transaction");

            Driver.FindElement(By.CssSelector("[data-e2e=\"MainBonus\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"MainBonusSub\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).SendKeys("100");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).SendKeys("second reason");

            Driver.FindElement(By.CssSelector("[data-e2e=\"bonussubtract\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text
                    .Contains("900 основных и 0 дополнительных"), "card additional bonuses added");
            VerifyAreEqual("-100",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[1].Text,
                " count bonus  table transaction");
            VerifyAreEqual("900",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[3].Text,
                "  saldo table transaction");
            VerifyAreEqual("second reason",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[7].Text,
                " number sell table transaction");
            Driver.FindElement(By.CssSelector("[data-e2e=\"MainBonus\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"MainBonusSub\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).SendKeys("1000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).SendKeys("reason");

            Driver.FindElement(By.CssSelector("[data-e2e=\"bonussubtract\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog.modal-md")).Displayed, "display modal win");
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();
            Thread.Sleep(100);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text
                    .Contains("900 основных и 0 дополнительных"), "card additional bonuses added");
            VerifyAreEqual("-100",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[1].Text,
                " count bonus  table transaction");
            VerifyAreEqual("900",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[3].Text,
                "  saldo table transaction");
            VerifyAreEqual("second reason",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[7].Text,
                " number sell table transaction");
        }

        [Test]
        public void EditCardAddsAdditionalBonus()
        {
            GoToAdmin("cards/edit/2c8fb106-8f07-499b-b06f-51b43076f3cf");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionalBonus\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionalBonusAdd\"]")).Click();

            VerifyAreEqual("Начислить дополнительные бонусы", Driver.FindElement(By.TagName("h2")).Text,
                "pop up header");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Name\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Name\"]")).SendKeys("AdditionalBonuses");

            Driver.FindElement(By.CssSelector("[data-e2e=\"addAdditionBonusEndDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"addAdditionBonusEndDate\"]")).SendKeys("28.03.2030");

            Driver.FindElement(By.CssSelector("[data-e2e=\"addAdditionBonusStartDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"addAdditionBonusStartDate\"]")).SendKeys("28.03.2014");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Name\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).SendKeys("thid Reason");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).SendKeys("1000");


            Driver.FindElement(By.CssSelector("[data-e2e=\"bonusAdd\"]")).Click();
            Thread.Sleep(500);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text.Contains("1 000 дополнительных"),
                "card additional bonuses added");

            VerifyAreEqual("AdditionalBonuses",
                Driver.FindElement(By.Id("viewTableAdditionalBonus")).FindElements(By.TagName("td"))[0].Text,
                " name bonus  table additional");
            VerifyAreEqual("1 000",
                Driver.FindElement(By.Id("viewTableAdditionalBonus")).FindElements(By.TagName("td"))[1].Text,
                "  count table additional");
            VerifyAreEqual("thid Reason",
                Driver.FindElement(By.Id("viewTableAdditionalBonus")).FindElements(By.TagName("td"))[2].Text,
                " reason table additional");

            //через точки
            VerifyAreEqual("28 03 2014",
                Driver.FindElement(By.Id("viewTableAdditionalBonus")).FindElements(By.TagName("td"))[3].Text,
                " dateFrom table additional");
            VerifyAreEqual("28 03 2030",
                Driver.FindElement(By.Id("viewTableAdditionalBonus")).FindElements(By.TagName("td"))[4].Text,
                " dateTo table additional");

            VerifyAreEqual("+ 1 000",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[5].Text,
                " count bonus  table transaction");
            VerifyAreEqual("1 000",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[6].Text,
                "  saldo table transaction");
            VerifyAreEqual("thid Reason",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[7].Text,
                " number sell table transaction");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionalBonus\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionalBonusSub\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Списать дополнительные бонусы", Driver.FindElement(By.TagName("h2")).Text,
                "pop up header 1");

            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-9 select")))).SelectByText(
                "AdditionalBonuses (1000)");
            Driver.FindElement(By.CssSelector("[data-e2e=\"amount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"amount\"]")).SendKeys("100");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).SendKeys("five Reason");

            Driver.FindElement(By.CssSelector("[data-e2e=\"subctractBonus\"]")).Click();
            Thread.Sleep(500);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text.Contains("900 дополнительных"),
                "card additional bonuses added");

            VerifyAreEqual("-100",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[4].Text,
                " count bonus  table transaction");
            VerifyAreEqual("900",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[6].Text,
                "  saldo table transaction");
            VerifyAreEqual("five Reason",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[7].Text,
                " number sell table transaction");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionalBonus\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionalBonusSub\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Списать дополнительные бонусы", Driver.FindElement(By.TagName("h2")).Text,
                "pop up header 2");

            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-9 select")))).SelectByText(
                "AdditionalBonuses (900)");
            Driver.FindElement(By.CssSelector("[data-e2e=\"amount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"amount\"]")).SendKeys("1000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).SendKeys("six Reason");

            Driver.FindElement(By.CssSelector("[data-e2e=\"subctractBonus\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog.modal-md")).Displayed, "display modal win");
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();
            Thread.Sleep(100);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text.Contains("900 дополнительных"),
                "card additional bonuses added");

            VerifyAreEqual("-100",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[4].Text,
                " count bonus  table transaction");
            VerifyAreEqual("900",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[6].Text,
                "  saldo table transaction");
            VerifyAreEqual("five Reason",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[7].Text,
                " number sell table transaction");
        }

        [Test]
        public void EditCustomer()
        {
            GoToAdmin("cards");
            Driver.GetGridCell(0, "FIO").Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveCard"));

            Driver.FindElement(By.CssSelector(".edit.link-decoration-none")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"EditClientRight\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));

            Driver.FindElement(By.Id("Customer_LastName")).Click();
            Driver.FindElement(By.Id("Customer_LastName")).Clear();
            Driver.FindElement(By.Id("Customer_LastName")).SendKeys("changedSurname");

            Driver.FindElement(By.Id("Customer_FirstName")).Click();
            Driver.FindElement(By.Id("Customer_FirstName")).Clear();
            Driver.FindElement(By.Id("Customer_FirstName")).SendKeys("changedName");

            Driver.FindElement(By.Id("Customer_EMail")).Click();
            Driver.FindElement(By.Id("Customer_EMail")).Clear();
            Driver.FindElement(By.Id("Customer_EMail")).SendKeys("editedtest@mail.ru");

            Driver.FindElement(By.Id("Customer_Phone")).Click();
            Driver.ClearInput(By.Id("Customer_Phone"));
            Driver.FindElement(By.Id("Customer_Phone")).SendKeys("+79308888888");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();
            GoToAdmin("cards");
            Driver.GetGridCell(0, "FIO").Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveCard"));

            VerifyAreEqual("changedSurname", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardSuname\"]")).Text,
                " bonusCardSuname in str card");
            VerifyAreEqual("changedName", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardName\"]")).Text,
                " bonusCardName in str card");
            VerifyAreEqual("79308888888", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardPhone\"]")).Text,
                " bonusCardPhone in str card");
            VerifyAreEqual("editedtest@mail.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardMail\"]")).Text, " bonusCardMail in str card");
        }

        [Test]
        public void EditCustomerAddCard()
        {
            GoToAdmin("customers/view/cfc2c33b-1e84-415e-8482-e98156341601");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCard\"]")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("test testov", Driver.FindElement(By.CssSelector(".m-l-xs.link-invert")).Text,
                " name customer cart");
            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-9 select")))).SelectByText("Серебряный");
            Driver.FindElement(By.CssSelector("[data-e2e=\"addCard\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveCard"));

            VerifyIsTrue(Driver.Url.Contains("cards/edit/cfc2c33b-1e84-415e-8482-e98156341601"),
                "redirect to bonus card");
            VerifyAreEqual("0 бонусов\r\n0 основных и 0 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus")).Text, " in str card");
            VerifyAreEqual("testov", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardSuname\"]")).Text,
                " bonusCardSuname in str card");
            VerifyAreEqual("test", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardName\"]")).Text,
                " bonusCardName in str card");
            VerifyAreEqual("qwe@qwe.qwe", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardMail\"]")).Text,
                " bonusCardMail in str card");

            Driver.FindElement(By.CssSelector(".breadcrumb a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("AddCard"));
            VerifyAreEqual("testov test", Driver.GetGridCell(0, "FIO").Text, "grid cart fio");
            VerifyAreEqual("Серебряный", Driver.GetGridCell(0, "GradeName").Text, "grid cart GradeName");
            VerifyAreEqual("7", Driver.GetGridCell(0, "GradePersent").Text, "grid cart GradePersent");

            Driver.GetGridCell(0, "FIO").Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveCard"));

            //Check client cart
            Driver.FindElement(By.CssSelector(".edit.link-decoration-none")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            VerifyAreEqual("testov test", Driver.FindElement(By.CssSelector("[data-e2e=\"clientName\"]")).Text,
                " client cart title h1");
            VerifyAreEqual("Грейд: Серебряный 7%", Driver.FindElement(By.CssSelector("[data-e2e=\"GradeCart\"]")).Text,
                " client cart type card");

            //del cart
            Driver.FindElement(By.CssSelector("[data-e2e=\"DelClient\"]")).Click();
            Driver.SwalConfirm();
            Driver.GridFilterSendKeys("testov");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), " NO del cart");
        }

        [Test]
        public void EditEnabledCard()
        {
            GoToAdmin("cards");
            Driver.GridFilterSendKeys("LastName");
            Driver.GetGridCell(0, "FIO").Click();
            Driver.WaitForElem(AdvBy.DataE2E("SaveCard"));

            Driver.FindElements(By.CssSelector(".switcher-state-trigger"))[1].Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveCard\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector(".edit.link-decoration-none")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            VerifyIsTrue(Driver.PageSource.Contains("Карта заблокирована"), " enabled card in cart customer");

            ReInitClient();
            GoToClient("login");
            Refresh();
            Driver.FindElement(By.Id("email")).SendKeys("mail@mail.com");
            Driver.FindElement(By.Id("password")).SendKeys("123123");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Driver.WaitForElem(By.ClassName("slider-main-block"));

            GoToClient("products/test-product22");
            Driver.ScrollTo(By.CssSelector(".rating"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".bonus-string-sum")).Count == 0,
                "no bonus in product cart");
            Driver.FindElement(By.CssSelector(".details-payment-inline a")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyAreEqual("Бонусная карта заблокирована",
                Driver.FindElement(By.CssSelector(".custom-input-text")).Text, " enabled bonus cart");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
        }
    }
}