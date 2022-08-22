using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.CheckoutFields
{
    [TestFixture]
    public class SettingsCheckoutCustomer : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductCategories.csv"
            );
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
        public void CustomerRequiredAllField()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Driver.FindElement(By.Id("CustomerFirstNameField")).Clear();
            Driver.FindElement(By.Id("CustomerFirstNameField")).SendKeys("Ваше имя");
            Driver.FindElement(By.Id("CustomerPhoneField")).Clear();
            Driver.FindElement(By.Id("CustomerPhoneField")).SendKeys("Ваш телефон");
            Driver.FindElement(By.Id("BirthDayFieldName")).Clear();
            Driver.FindElement(By.Id("BirthDayFieldName")).SendKeys("Ваш ДР");

            Functions.CheckSelected("IsShowLastName", Driver);
            Functions.CheckSelected("IsShowPatronymic", Driver);
            Functions.CheckSelected("IsShowPhone", Driver);
            Functions.CheckSelected("IsShowBirthDay", Driver);

            Functions.CheckSelected("IsRequiredLastName", Driver);
            Functions.CheckSelected("IsRequiredPatronymic", Driver);
            Functions.CheckSelected("IsRequiredPhone", Driver);
            Functions.CheckSelected("IsRequiredBirthDay", Driver);
            Functions.CheckNotSelected("IsRequiredZip", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_LastName")).GetAttribute("data-ng-required") == "true",
                "lastname required");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_Patronymic")).GetAttribute("data-ng-required") == "true",
                "patronymic required");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_Phone")).GetAttribute("data-ng-required") == "true",
                "phone required");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_BirthDay")).GetAttribute("data-ng-required") == "true",
                "BirthDay required");

            VerifyIsTrue(Driver.FindElement(By.Name("checkoutNewCustomerForm")).Text.Contains("E-mail"), "email label");
            VerifyIsTrue(Driver.FindElement(By.Name("checkoutNewCustomerForm")).Text.Contains("Ваше имя"),
                "name label");
            VerifyIsTrue(Driver.FindElement(By.Name("checkoutNewCustomerForm")).Text.Contains("Фамилия"),
                "lastname label");
            VerifyIsTrue(Driver.FindElement(By.Name("checkoutNewCustomerForm")).Text.Contains("Отчество"),
                "patronymic label");
            VerifyIsTrue(Driver.FindElement(By.Name("checkoutNewCustomerForm")).Text.Contains("Ваш телефон"),
                "phone label");
            VerifyIsTrue(Driver.FindElement(By.Name("checkoutNewCustomerForm")).Text.Contains("Ваш ДР"),
                "BirthDay label");

            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_FirstName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_LastName")).Displayed, "lastname field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_Patronymic")).Displayed, "patronymic field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_Phone")).Displayed, "phone field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_BirthDay")).Displayed, "BirthDay field");

            Driver.FindElement(By.Id("Data_User_FirstName")).Clear();
            Driver.FindElement(By.Id("Data_User_LastName")).Clear();
            Driver.FindElement(By.Id("Data_User_Patronymic")).Clear();
            Driver.FindElement(By.Id("Data_User_Phone")).Clear();
            //driver.FindElement(By.Id("Data_User_BirthDay")).Clear();

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("checkout"), "error checkout 1");

            Driver.FindElement(By.Id("Data_User_FirstName")).SendKeys("Name");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("checkout"), "error checkout 2");

            Driver.FindElement(By.Id("Data_User_LastName")).SendKeys("LastName");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("checkout"), "error checkout 3");

            Driver.FindElement(By.Id("Data_User_Patronymic")).SendKeys("Patronymic");
            Thread.Sleep(2000);
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("checkout"), "error checkout 4");
            Thread.Sleep(1000);
            Driver.FindElement(By.Id("Data_User_Phone")).Click();
            Driver.FindElement(By.Id("Data_User_Phone")).SendKeys(Keys.Control + "a" + Keys.Delete);
            Driver.FindElement(By.Id("Data_User_Phone")).SendKeys("79999998877");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("checkout"), "error checkout 5");

            Functions.DataTimePicker(Driver, BaseUrl, month: "Январь", year: "1990", data: "Январь 17, 1990",
                field: "#Data_User_BirthDay");
            Thread.Sleep(1000);
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void CustomerRequiredNolField()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowLastName", Driver);
            Functions.CheckSelected("IsShowPatronymic", Driver);
            Functions.CheckSelected("IsShowPhone", Driver);
            Functions.CheckSelected("IsShowBirthDay", Driver);

            Functions.CheckNotSelected("IsRequiredLastName", Driver);
            Functions.CheckNotSelected("IsRequiredPatronymic", Driver);
            Functions.CheckNotSelected("IsRequiredPhone", Driver);
            Functions.CheckNotSelected("IsRequiredZip", Driver);
            Functions.CheckNotSelected("IsRequiredBirthDay", Driver);

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            //VerifyIsTrue(driver.FindElement(By.Id("Data_User_FirstName")).GetAttribute("data-ng-required") == "true", "name required");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_LastName")).GetAttribute("data-ng-required") == "false",
                "lastname required");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_Patronymic")).GetAttribute("data-ng-required") == "false",
                "patronymic required");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_Phone")).GetAttribute("data-ng-required") == "false",
                "phone required");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_BirthDay")).GetAttribute("data-ng-required") == "false",
                "BirthDay required");

            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_FirstName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_LastName")).Displayed, "lastname field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_Patronymic")).Displayed, "patronymic field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_Phone")).Displayed, "phone field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_BirthDay")).Displayed, "BirthDay field");

            Driver.FindElement(By.Id("Data_User_FirstName")).Clear();
            Driver.FindElement(By.Id("Data_User_LastName")).Clear();
            Driver.FindElement(By.Id("Data_User_Patronymic")).Clear();
            Driver.FindElement(By.Id("Data_User_Phone")).Clear();
            //driver.FindElement(By.Id("Data_User_BirthDay")).Clear();

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("checkout"), "error checkout");

            Driver.FindElement(By.Id("Data_User_FirstName")).SendKeys("Name");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void CustomerVisibleLastName()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowLastName", Driver);
            Functions.CheckNotSelected("IsShowPatronymic", Driver);
            Functions.CheckNotSelected("IsShowPhone", Driver);
            Functions.CheckNotSelected("IsShowBirthDay", Driver);
            Functions.CheckNotSelected("IsRequiredZip", Driver);

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));

            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_FirstName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_LastName")).Displayed, "lastname field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_Patronymic")).Count == 0, "patronymic field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_Phone")).Count == 0, "phone field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_BirthDay")).Count == 0, "BirthDay field");
        }

        [Test]
        public void CustomerVisiblePatronymic()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowLastName", Driver);
            Functions.CheckSelected("IsShowPatronymic", Driver);
            Functions.CheckNotSelected("IsShowPhone", Driver);
            Functions.CheckNotSelected("IsShowBirthDay", Driver);
            Functions.CheckNotSelected("IsRequiredZip", Driver);

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));

            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_FirstName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_LastName")).Count == 0, "lastname field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_Patronymic")).Displayed, "patronymic field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_Phone")).Count == 0, "phone field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_BirthDay")).Count == 0, "BirthDay field");
        }

        [Test]
        public void CustomerVisiblePhone()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowLastName", Driver);
            Functions.CheckNotSelected("IsShowPatronymic", Driver);
            Functions.CheckSelected("IsShowPhone", Driver);
            Functions.CheckNotSelected("IsShowBirthDay", Driver);
            Functions.CheckNotSelected("IsRequiredZip", Driver);

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));

            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_FirstName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_LastName")).Count == 0, "lastname field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_Patronymic")).Count == 0, "patronymic field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_Phone")).Displayed, "phone field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_BirthDay")).Count == 0, "BirthDay field");
        }

        [Test]
        public void CustomerVisibleBirthDay()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowLastName", Driver);
            Functions.CheckNotSelected("IsShowPatronymic", Driver);
            Functions.CheckNotSelected("IsShowPhone", Driver);
            Functions.CheckSelected("IsShowBirthDay", Driver);
            Functions.CheckNotSelected("IsRequiredZip", Driver);

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(2000);

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));

            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_FirstName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_LastName")).Count == 0, "lastname field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_Patronymic")).Count == 0, "patronymic field");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_Phone")).Count == 0, "phone field");
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_BirthDay")).Displayed, "BirthDay field");

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".flatpickr-calendar")).Displayed, "BirthDay no calendar");
            Driver.FindElement(By.Id("Data_User_BirthDay")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".flatpickr-calendar")).Displayed, "BirthDay calendar");
        }
    }
}