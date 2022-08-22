using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Client.Tests.Desktop.RegAuth
{
    [TestFixture]
    public class ClientAuthorizationTests : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS | ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Client\\SmokeTests\\CMS.StaticBlock.csv",
                "data\\Client\\SmokeTests\\CMS.StaticPage.csv",
                "data\\Client\\SmokeTests\\Catalog.Photo.csv",
                "data\\Client\\SmokeTests\\Catalog.Category.csv",
                "data\\Client\\SmokeTests\\Catalog.Brand.csv",
                "data\\Client\\SmokeTests\\Catalog.Product.csv",
                "data\\Client\\SmokeTests\\Catalog.ProductCategories.csv",
                "data\\Client\\SmokeTests\\Catalog.Tag.csv",
                "data\\Client\\SmokeTests\\Catalog.Property.csv",
                "data\\Client\\SmokeTests\\Catalog.PropertyValue.csv",
                "data\\Client\\SmokeTests\\Catalog.ProductPropertyValue.csv",
                "data\\Client\\SmokeTests\\Catalog.PropertyGroup.csv",
                "data\\Client\\SmokeTests\\Catalog.Color.csv",
                "data\\Client\\SmokeTests\\Catalog.Size.csv",
                "data\\Client\\SmokeTests\\Catalog.Offer.csv",
                "data\\Client\\SmokeTests\\CMS.Menu.csv",
                "data\\Client\\SmokeTests\\Settings.NewsCategory.csv",
                "data\\Client\\SmokeTests\\Settings.News.csv",
                "data\\Client\\SmokeTests\\Customers.Customer.csv",
                "data\\Client\\SmokeTests\\Customers.Contact.csv",
                "data\\Client\\SmokeTests\\Customers.CustomerGroup.csv"
            );

            Init();
            EnableInplaceOff();
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
        public void AuthSuccess()
        {
            Driver.FindElement(By.LinkText("Войти")).Click();
            Driver.WaitForElem(By.Id("email"));

            VerifyAreEqual("Авторизация", Driver.FindElement(By.TagName("h1")).Text, "authorization h1");

            Driver.FindElement(By.Id("email")).Click();
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test@test.test");

            Driver.FindElement(By.Id("password")).Click();
            Driver.FindElement(By.Id("password")).Clear();
            Driver.FindElement(By.Id("password")).SendKeys("123123");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector("header.site-head")).Text.Contains("Личный кабинет"),
                "customer logged in");
        }

        [Test]
        public void AuthToRegister()
        {
            Driver.FindElement(By.LinkText("Войти")).Click();
            Driver.WaitForElem(By.Id("email"));

            VerifyAreEqual("Регистрация", Driver.FindElement(By.CssSelector("a.btn.btn-confirm")).Text,
                "refister button");

            Driver.FindElement(By.CssSelector(".btn.btn-confirm")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("Регистрация", Driver.FindElement(By.TagName("h1")).Text,
                "registration from authorization h1");
            VerifyIsTrue(Driver.Url.Contains("registration"), "registration from authorization url");
        }

        [Test]
        public void AuthWrongData()
        {
            Driver.FindElement(By.LinkText("Войти")).Click();
            Driver.WaitForElem(By.Id("email"));

            VerifyAreEqual("Авторизация", Driver.FindElement(By.TagName("h1")).Text, "authorization h1");

            Driver.FindElement(By.Id("email")).Click();
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("te@test.test");

            Driver.FindElement(By.Id("password")).Click();
            Driver.FindElement(By.Id("password")).Clear();
            Driver.FindElement(By.Id("password")).SendKeys("123123");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Неверный логин или пароль"), "invalid data error");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("header.site-head")).Text.Contains("Личный кабинет"),
                "customer not logged in");
        }

        [Test]
        public void AuthInvalidEmail()
        {
            Driver.FindElement(By.LinkText("Войти")).Click();
            Driver.WaitForElem(By.Id("email"));

            //pre check 
            VerifyAreEqual("rgb(226, 227, 228)", Driver.FindElement(By.Id("email")).GetCssValue("border-color"),
                "pre check email field border");

            Driver.FindElement(By.Id("email")).Click();
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("testtest.test");

            Driver.FindElement(By.Id("password")).Click();
            Driver.FindElement(By.Id("password")).Clear();
            Driver.FindElement(By.Id("password")).SendKeys("123123");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsFalse(Driver.FindElement(By.CssSelector("header.site-head")).Text.Contains("Личный кабинет"),
                "customer not logged in");
            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("email")).GetCssValue("border-color"),
                "invalid email field border");
        }

        [Test]
        public void AuthEmptyFields()
        {
            Driver.FindElement(By.LinkText("Войти")).Click();
            Driver.WaitForElem(By.Id("email"));

            //pre check 
            VerifyAreEqual("rgb(226, 227, 228)", Driver.FindElement(By.Id("email")).GetCssValue("border-color"),
                "pre check email field border");
            VerifyAreEqual("rgb(226, 227, 228)", Driver.FindElement(By.Id("password")).GetCssValue("border-color"),
                "pre check password field border");

            Driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsFalse(Driver.FindElement(By.CssSelector("header.site-head")).Text.Contains("Личный кабинет"),
                "customer not logged in");
            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("email")).GetCssValue("border-color"),
                "invalid email field border");
            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("password")).GetCssValue("border-color"),
                "invalid password field border");
        }

        [Test]
        public void AuthForgotPassword()
        {
            Driver.FindElement(By.LinkText("Войти")).Click();
            Driver.WaitForElem(By.Id("email"));

            Driver.FindElement(By.LinkText("Забыли пароль?")).Click();
            Thread.Sleep(2000);

            //check forgot password
            VerifyIsTrue(Driver.Url.Contains("forgotpassword"), "forgot password url");
            VerifyAreEqual("Восстановление пароля", Driver.FindElement(By.TagName("h1")).Text,
                "forgot password from authorization error");
            VerifyIsTrue(Driver.FindElement(By.Id("email")).Enabled, "forgot password field");

            Driver.FindElement(By.Id("email")).Click();
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test@test.test");
            Thread.Sleep(2000);

            Driver.FindElement(By.Name("authForm")).FindElement(By.CssSelector(".btn.btn-action")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.PageSource.Contains(
                    "На указанный при регистрации E-mail было выслано сообщение. Проверьте почту и следуйте полученной инструкции."),
                "forgot password letter sent");
        }

        [Test]
        public void AuthForgotPasswordWrongEmail()
        {
            Driver.FindElement(By.LinkText("Войти")).Click();
            Driver.WaitForElem(By.Id("email"));
            Driver.FindElement(By.LinkText("Забыли пароль?")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test8@test.test");
            Thread.Sleep(500);
            Driver.FindElement(By.Name("authForm")).FindElement(By.CssSelector(".btn.btn-action")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.PageSource.Contains("Пользователь с указанным e-mail не найден."),
                "forgot password wrong email");
        }

        [Test]
        public void AuthForgotPasswordInvalidEmail()
        {
            Driver.FindElement(By.LinkText("Войти")).Click();
            Driver.WaitForElem(By.Id("email"));

            Driver.FindElement(By.LinkText("Забыли пароль?")).Click();
            Thread.Sleep(2000);

            Driver.FindElement(By.Id("email")).Click();
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("teste@sttest");

            Driver.FindElement(By.Name("authForm")).FindElement(By.CssSelector(".btn.btn-action")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("email")).GetCssValue("border-color"),
                "email field border");
        }

        [Test]
        public void AuthForgotPasswordEmptyField()
        {
            Driver.FindElement(By.LinkText("Войти")).Click();
            Driver.WaitForElem(By.Id("email"));

            Driver.FindElement(By.LinkText("Забыли пароль?")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("rgb(226, 227, 228)", Driver.FindElement(By.Id("email")).GetCssValue("border-color"),
                "pre check email field border");

            Driver.FindElement(By.Name("authForm")).FindElement(By.CssSelector(".btn.btn-action")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("rgb(241, 89, 89)", Driver.FindElement(By.Id("email")).GetCssValue("border-color"),
                "email field border");
        }
    }
}