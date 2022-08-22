using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.Card
{
    [TestFixture]
    public class BonusSystemCardToAll : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\CardsBonusToAll\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\CardsBonusToAll\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\CardsBonusToAll\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\CardsBonusToAll\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\CardsBonusToAll\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\CardsBonusToAll\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\CardsBonusToAll\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\CardsBonusToAll\\Bonus.Card.csv"
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
        public void AddToAllAdditionalBonuses()
        {
            GoToAdmin("cards");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"indexBonusesAddAll\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.LinkText("Начислить дополнительные")).Click();

            Driver.WaitForElem(By.CssSelector(".modal-content"));
            VerifyAreEqual("Начислить дополнительные бонусы", Driver.FindElement(By.TagName("h2")).Text,
                "pop up header");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Name\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Name\"]")).SendKeys("AdditionalBonuses");

            Driver.FindElement(By.CssSelector("[data-e2e=\"addAdditionBonusStartDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"addAdditionBonusStartDate\"]")).SendKeys("28.03.2014");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Name\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"addAdditionBonusEndDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"addAdditionBonusEndDate\"]")).SendKeys("28.03.2030");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Name\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).SendKeys("1000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).SendKeys("Reason");

            Driver.FindElement(By.CssSelector("[data-e2e=\"bonusAdd\"]")).Click();
            Thread.Sleep(500);

            GoToAdmin("cards/edit/2C8FB106-8F07-499B-B06F-51B43076F3C1");
            VerifyAreEqual("1 основных и 1 000 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"] div")).Text,
                "card 1 additional bonuses added");

            GoToAdmin("cards/edit/2C8FB106-8F07-499B-B06F-51B43076F3C2");
            VerifyAreEqual("2 основных и 1 000 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"] div")).Text,
                "card 2 additional bonuses added");

            GoToAdmin("cards/edit/2C8FB106-8F07-499B-B06F-51B43076F3C3");
            VerifyAreEqual("3 основных и 1 000 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"] div")).Text,
                "card 3 additional bonuses added");

            GoToAdmin("cards/edit/2C8FB106-8F07-499B-B06F-51B43076F420");
            VerifyAreEqual("120 основных и 1 000 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"] div")).Text,
                "last card additional bonuses added");
        }

        [Test]
        public void AddToAllMainBonuses()
        {
            GoToAdmin("cards");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"indexBonusesAddAll\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.LinkText("Начислить основные")).Click();

            Driver.WaitForElem(By.CssSelector(".modal-content"));
            VerifyAreEqual("Начислить основные бонусы", Driver.FindElement(By.TagName("h2")).Text, "pop up header");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Amount\"]")).SendKeys("1000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Reason\"]")).SendKeys("Reason");

            Driver.FindElement(By.CssSelector("[data-e2e=\"bonusAdd\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("cards/edit/2C8FB106-8F07-499B-B06F-51B43076F3C1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text
                    .Contains("1 001 основных и 0 дополнительных"), "card 1 additional bonuses added");

            GoToAdmin("cards/edit/2C8FB106-8F07-499B-B06F-51B43076F3C2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text
                    .Contains("1 002 основных и 0 дополнительных"), "card 2 additional bonuses added");

            GoToAdmin("cards/edit/2C8FB106-8F07-499B-B06F-51B43076F3C3");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text
                    .Contains("1 003 основных и 0 дополнительных"), "card 3 additional bonuses added");

            GoToAdmin("cards/edit/2C8FB106-8F07-499B-B06F-51B43076F420");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text
                    .Contains("1 120 основных и 0 дополнительных"), "last card additional bonuses added");
        }
    }
}