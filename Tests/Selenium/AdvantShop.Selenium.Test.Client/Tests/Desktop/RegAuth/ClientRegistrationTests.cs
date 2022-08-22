using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Client.Tests.Desktop.RegAuth
{
    [TestFixture]
    public class ClientRegistrationTests : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS | ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Client\\SmokeTests\\CMS.StaticBlock.csv",
                "data\\Client\\SmokeTests\\CMS.StaticPage.csv",
                "data\\Client\\SmokeTests\\CMS.Menu.csv",
                "data\\Client\\SmokeTests\\Catalog.Color.csv",
                "data\\Client\\SmokeTests\\Catalog.Size.csv",
                "data\\Client\\SmokeTests\\Catalog.Photo.csv",
                "data\\Client\\SmokeTests\\Catalog.Brand.csv",
                "data\\Client\\SmokeTests\\Settings.NewsCategory.csv",
                "data\\Client\\SmokeTests\\Settings.News.csv",
                "data\\Client\\SmokeTests\\Catalog.Product.csv",
                "data\\Client\\SmokeTests\\Catalog.Offer.csv",
                "data\\Client\\SmokeTests\\Catalog.Category.csv",
                "data\\Client\\SmokeTests\\Catalog.ProductCategories.csv",
                "data\\Client\\SmokeTests\\Catalog.Tag.csv",
                "data\\Client\\SmokeTests\\Catalog.Property.csv",
                "data\\Client\\SmokeTests\\Catalog.PropertyValue.csv",
                "data\\Client\\SmokeTests\\Catalog.ProductPropertyValue.csv",
                "data\\Client\\SmokeTests\\Catalog.PropertyGroup.csv",
                "data\\Client\\SmokeTests\\Customers.Customer.csv",
                "data\\Client\\SmokeTests\\Customers.CustomerGroup.csv"
            );

            Init();
            EnableInplaceOff();

            Functions.EnableCapcha(Driver, BaseUrl);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            ReInitClient();
            GoToClient();
            Refresh();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void RegSuccess()
        {
            var curDate = DateTime.Now.Date.ToString("dd.MM.yyyy");

            Driver.FindElement(By.LinkText("Регистрация")).Click();
            Driver.WaitForElem(By.Id("FirstName"));

            VerifyAreEqual("Регистрация", Driver.FindElement(By.TagName("h1")).Text, "registration h1");

            Driver.FindElement(By.Id("FirstName")).Click();
            Driver.FindElement(By.Id("FirstName")).Clear();
            Driver.FindElement(By.Id("FirstName")).SendKeys("FirstName");

            Driver.FindElement(By.Id("LastName")).Click();
            Driver.FindElement(By.Id("LastName")).Clear();
            Driver.FindElement(By.Id("LastName")).SendKeys("LastName");

            Driver.FindElement(By.Id("Email")).Click();
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("Email@Email.Email");

            Driver.FindElement(By.Id("Phone")).Click();
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89279272727");

            Driver.FindElement(By.Id("Password")).Click();
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("123123");

            Driver.FindElement(By.Id("PasswordConfirm")).Click();
            Driver.FindElement(By.Id("PasswordConfirm")).Clear();
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123123");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("Личный кабинет", Driver.FindElement(By.TagName("h1")).Text, "registration success h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("header.site-head")).Text.Contains("Личный кабинет"),
                "customer registered");

            //check admin
            ReInit();
            GoToAdmin("customers");

            VerifyAreEqual("LastName FirstName",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text, "new customers Name");
            VerifyAreEqual("79279272727", Driver.GetGridCell(0, "Phone", "Customers").Text, "new customers Phone");
            VerifyAreEqual("Email@Email.Email", Driver.GetGridCell(0, "Email", "Customers").Text,
                "new customers Email");
            VerifyAreEqual("0", Driver.GetGridCell(0, "OrdersCount", "Customers").Text, "new customers OrdersCount");
            VerifyAreEqual("", Driver.GetGridCell(0, "LastOrderNumber", "Customers").Text,
                "new customers LastOrderNumber");
            VerifyAreEqual("0", Driver.GetGridCell(0, "OrdersSum", "Customers").Text, "new customers OrdersSum");
            VerifyIsTrue(Driver.GetGridCell(0, "RegistrationDateTimeFormatted", "Customers").Text.Contains(curDate),
                "new customers reg date");
        }

        [Test]
        public void RegNewsSubscribe()
        {
            Driver.FindElement(By.LinkText("Регистрация")).Click();
            Driver.WaitForElem(By.Id("FirstName"));

            Driver.FindElement(By.Id("FirstName")).Click();
            Driver.FindElement(By.Id("FirstName")).Clear();
            Driver.FindElement(By.Id("FirstName")).SendKeys("News");

            Driver.FindElement(By.Id("LastName")).Click();
            Driver.FindElement(By.Id("LastName")).Clear();
            Driver.FindElement(By.Id("LastName")).SendKeys("Subscribe");

            Driver.FindElement(By.Id("Email")).Click();
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("News@Email.Email");

            Driver.FindElement(By.Id("Phone")).Click();
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89279272331");

            Driver.FindElement(By.Id("Password")).Click();
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("123123");

            Driver.FindElement(By.Id("PasswordConfirm")).Click();
            Driver.FindElement(By.Id("PasswordConfirm")).Clear();
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123123");

            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsSubscription\"] .custom-input-checkbox")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("Личный кабинет", Driver.FindElement(By.TagName("h1")).Text, "registration success h1");

            //check admin
            ReInit();
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("News Subscribe");

            Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"TabClient\"]"));

            Driver.FindElement(By.LinkText("Редактировать")).Click();
            Thread.Sleep(2000);
            Driver.WaitForElem(By.Id("Customer_LastName"));
            VerifyIsTrue(Driver.FindElement(By.Id("Customer_SubscribedForNews")).Selected,
                "pop up news subscribtion selected");
        }

        [Test]
        public void RegInvalidEmail()
        {
            Driver.FindElement(By.LinkText("Регистрация")).Click();
            Driver.WaitForElem(By.Id("FirstName"));

            VerifyAreEqual("rgb(226, 227, 228)", Driver.FindElement(By.Id("Email")).GetCssValue("border-color"),
                "pre check email field border");

            Driver.FindElement(By.Id("FirstName")).Click();
            Driver.FindElement(By.Id("FirstName")).Clear();
            Driver.FindElement(By.Id("FirstName")).SendKeys("No Reg");

            Driver.FindElement(By.Id("LastName")).Click();
            Driver.FindElement(By.Id("LastName")).Clear();
            Driver.FindElement(By.Id("LastName")).SendKeys("No Reg Last Name");

            Driver.FindElement(By.Id("Email")).Click();
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("Email.ru");

            Driver.FindElement(By.Id("Phone")).Click();
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("8927927337");

            Driver.FindElement(By.Id("Password")).Click();
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("123123");

            Driver.FindElement(By.Id("PasswordConfirm")).Click();
            Driver.FindElement(By.Id("PasswordConfirm")).Clear();
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123123");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreNotEqual("Личный кабинет", Driver.FindElement(By.TagName("h1")).Text,
                "registration not success h1");
            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("Email")).GetCssValue("border-color"),
                "invalid email field border");

            //check admin
            ReInit();
            GoToAdmin("customers");

            VerifyIsFalse(Driver.PageSource.Contains("No Reg"), "no users registred");
        }

        [Test]
        public void RegEmptyFields()
        {
            Driver.FindElement(By.LinkText("Регистрация")).Click();
            Driver.WaitForElem(By.Id("FirstName"));

            //pre check 
            VerifyAreEqual("rgb(226, 227, 228)", Driver.FindElement(By.Id("FirstName")).GetCssValue("border-color"),
                "pre check first name field border");
            VerifyAreEqual("rgb(226, 227, 228)", Driver.FindElement(By.Id("LastName")).GetCssValue("border-color"),
                "pre check last name field border");
            VerifyAreEqual("rgb(226, 227, 228)", Driver.FindElement(By.Id("Email")).GetCssValue("border-color"),
                "pre check email field border");
            VerifyAreEqual("rgb(226, 227, 228)", Driver.FindElement(By.Id("Phone")).GetCssValue("border-color"),
                "pre check phone field border");
            VerifyAreEqual("rgb(226, 227, 228)", Driver.FindElement(By.Id("Password")).GetCssValue("border-color"),
                "pre check password field border");
            VerifyAreEqual("rgb(226, 227, 228)",
                Driver.FindElement(By.Id("PasswordConfirm")).GetCssValue("border-color"),
                "pre check password confirm field border");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreNotEqual("Личный кабинет", Driver.FindElement(By.TagName("h1")).Text,
                "registration not success h1");

            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("FirstName")).GetCssValue("border-color"),
                "empty first name field border");
            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("LastName")).GetCssValue("border-color"),
                "empty last name field border");
            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("Email")).GetCssValue("border-color"),
                "empty email field border");
            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("Phone")).GetCssValue("border-color"),
                "empty phone field border");
            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("Password")).GetCssValue("border-color"),
                "empty password field border");
            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("PasswordConfirm")).GetCssValue("border-color"),
                "empty password confirm field border");
        }

        [Test]
        public void RegExistedEmail()
        {
            Driver.FindElement(By.LinkText("Регистрация")).Click();
            Driver.WaitForElem(By.Id("FirstName"));

            Driver.FindElement(By.Id("FirstName")).Click();
            Driver.FindElement(By.Id("FirstName")).Clear();
            Driver.FindElement(By.Id("FirstName")).SendKeys("Same");

            Driver.FindElement(By.Id("LastName")).Click();
            Driver.FindElement(By.Id("LastName")).Clear();
            Driver.FindElement(By.Id("LastName")).SendKeys("Email");

            Driver.FindElement(By.Id("Email")).Click();
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("test@test.test");

            Driver.FindElement(By.Id("Phone")).Click();
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89234227337");

            Driver.FindElement(By.Id("Password")).Click();
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("123123");

            Driver.FindElement(By.Id("PasswordConfirm")).Click();
            Driver.FindElement(By.Id("PasswordConfirm")).Clear();
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123123");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreNotEqual("Личный кабинет", Driver.FindElement(By.TagName("h1")).Text,
                "registration not success h1");
            VerifyIsTrue(Driver.PageSource.Contains("Пользователь с таким email уже существует"),
                "the same email error");

            //check forgot password
            Driver.FindElement(By.LinkText("восстановлением пароля")).Click();
            VerifyIsTrue(Driver.Url.Contains("forgotpassword"), "forgot password url");
            VerifyAreEqual("Восстановление пароля", Driver.FindElement(By.TagName("h1")).Text,
                "forgot password from registraion error");
            VerifyIsTrue(Driver.FindElement(By.Id("email")).Enabled, "forgot password field");

            //check admin
            ReInit();
            GoToAdmin("customers");

            VerifyIsFalse(Driver.PageSource.Contains("Same"), "no users registred");
        }

        [Test]
        public void RegExistedManagerEmail()
        {
            Driver.FindElement(By.LinkText("Регистрация")).Click();
            Driver.WaitForElem(By.Id("FirstName"));

            Driver.FindElement(By.Id("FirstName")).Click();
            Driver.FindElement(By.Id("FirstName")).Clear();
            Driver.FindElement(By.Id("FirstName")).SendKeys("Manager");

            Driver.FindElement(By.Id("LastName")).Click();
            Driver.FindElement(By.Id("LastName")).Clear();
            Driver.FindElement(By.Id("LastName")).SendKeys("Email");

            Driver.FindElement(By.Id("Email")).Click();
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("elena@advantshop.net");

            Driver.FindElement(By.Id("Phone")).Click();
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89233227337");

            Driver.FindElement(By.Id("Password")).Click();
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("123123");

            Driver.FindElement(By.Id("PasswordConfirm")).Click();
            Driver.FindElement(By.Id("PasswordConfirm")).Clear();
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123123");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreNotEqual("Личный кабинет", Driver.FindElement(By.TagName("h1")).Text,
                "registration not success h1");
            VerifyIsTrue(Driver.PageSource.Contains("Пользователь с таким email уже существует"),
                "the same email error");

            //check forgot password
            Driver.FindElement(By.LinkText("восстановлением пароля")).Click();
            VerifyIsTrue(Driver.Url.Contains("forgotpassword"), "forgot password url");
            VerifyAreEqual("Восстановление пароля", Driver.FindElement(By.TagName("h1")).Text,
                "forgot password from registraion error");
            VerifyIsTrue(Driver.FindElement(By.Id("email")).Enabled, "forgot password field");

            //check admin
            ReInit();
            GoToAdmin("customers");

            VerifyIsFalse(Driver.PageSource.Contains("Manager Email"), "no users registred");
        }


        [Test]
        public void RegDiffPasswords()
        {
            Driver.FindElement(By.LinkText("Регистрация")).Click();
            Driver.WaitForElem(By.Id("FirstName"));

            Driver.FindElement(By.Id("FirstName")).Click();
            Driver.FindElement(By.Id("FirstName")).Clear();
            Driver.FindElement(By.Id("FirstName")).SendKeys("passwords");

            Driver.FindElement(By.Id("LastName")).Click();
            Driver.FindElement(By.Id("LastName")).Clear();
            Driver.FindElement(By.Id("LastName")).SendKeys("different");

            Driver.FindElement(By.Id("Email")).Click();
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("test123@test.test");

            Driver.FindElement(By.Id("Phone")).Click();
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89233324337");

            Driver.FindElement(By.Id("Password")).Click();
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("1233123");

            Driver.FindElement(By.Id("PasswordConfirm")).Click();
            Driver.FindElement(By.Id("PasswordConfirm")).Clear();
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("3111111");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreNotEqual("Личный кабинет", Driver.FindElement(By.TagName("h1")).Text,
                "registration not success h1");
            VerifyIsTrue(Driver.PageSource.Contains("Введенные пароли не совпадают"), "diff passwords error");
            //check admin
            ReInit();
            GoToAdmin("customers");

            VerifyIsFalse(Driver.PageSource.Contains("different"), "no users registred");
        }

        [Test]
        public void RegPasswordsIndalidSymbolsCount()
        {
            Driver.FindElement(By.LinkText("Регистрация")).Click();
            Driver.WaitForElem(By.Id("FirstName"));

            Driver.FindElement(By.Id("FirstName")).Click();
            Driver.FindElement(By.Id("FirstName")).Clear();
            Driver.FindElement(By.Id("FirstName")).SendKeys("passwords");

            Driver.FindElement(By.Id("LastName")).Click();
            Driver.FindElement(By.Id("LastName")).Clear();
            Driver.FindElement(By.Id("LastName")).SendKeys("symbols");

            Driver.FindElement(By.Id("Email")).Click();
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("test123@test123.test");

            Driver.FindElement(By.Id("Phone")).Click();
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89231743372");

            Driver.FindElement(By.Id("Password")).Click();
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("123");

            Driver.FindElement(By.Id("PasswordConfirm")).Click();
            Driver.FindElement(By.Id("PasswordConfirm")).Clear();
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreNotEqual("Личный кабинет", Driver.FindElement(By.TagName("h1")).Text,
                "registration not success h1");
            VerifyIsTrue(Driver.PageSource.Contains("Длина пароля должна быть не менее 6 символов"),
                "invalid passwords symbols count error");

            //check admin
            ReInit();
            GoToAdmin("customers");

            VerifyIsFalse(Driver.PageSource.Contains("symbols"), "no users registred");
        }
    }
}