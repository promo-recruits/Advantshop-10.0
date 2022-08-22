using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Service
{
    [TestFixture]
    public class SuppotCenter : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
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
        public void OpenSuppotCenterReadyAnswer()
        {
            GoToAdmin();

            Driver.FindElement(By.XPath("//a[contains(text(), 'Поддержка')]")).Click();
            Driver.WaitForElem(By.ClassName("js-iframe-wrap"));
            Thread.Sleep(100);
            VerifyIsTrue(Driver.Url.Contains("service/supportcenter"), "url supportcenter");
            VerifyIsTrue(Driver.FindElements(By.TagName("iframe")).Count > 0, "iframe on page");
            VerifyIsTrue(
                Driver.FindElement(By.TagName("iframe")).GetAttribute("src").ToString()
                    .Contains("www.advantshop.net/help"), "src page");

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);

            Driver.FindElement(By.XPath("//span[contains(text(), 'Товары')]")).Click();
            Thread.Sleep(100);
            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("help-search__help"));

            VerifyIsTrue(Driver.Url.Contains("advantshop.net/help/pages/add-product"), "url link");
            VerifyAreEqual("Добавление/удаление товара", Driver.FindElement(By.TagName("h1")).Text,
                "open page by link, h1");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void OpenSuppotCenterSearch()
        {
            GoToAdmin();

            // driver.FindElement(By.XPath("//span[contains(text(), 'Помощь')]")).Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Поддержка')]")).Click();
            Driver.WaitForElem(By.ClassName("js-iframe-wrap"));
            Thread.Sleep(100);

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);

            Driver.FindElement(By.CssSelector(".input.input_md.input_block")).SendKeys("Хостинг");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector(".btn.btn_md.btn_primary.btn_block")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);

            VerifyIsTrue(Driver.Url.Contains("advantshop.net/help/search"), "url link");
            VerifyAreEqual("Как вам помочь ?", Driver.FindElement(By.CssSelector(".help-search__help-title")).Text,
                "help, h");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".help-details-right__title")).Text.Contains("Найдено"),
                "rezult search, h3");
            VerifyAreEqual("хостинг", Driver.FindElement(By.Name("q")).GetAttribute("value"),
                "placeholder search value" + Driver.FindElement(By.Name("q")).GetAttribute("value").ToString());

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void OpenSuppotCenterCallBackMessange()
        {
            GoToAdmin();

            Driver.FindElement(By.XPath("//a[contains(text(), 'Поддержка')]")).Click();
            Driver.WaitForElem(By.ClassName("js-iframe-wrap"));
            Thread.Sleep(100);

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);

            VerifyAreEqual("Центр поддержки",
                Driver.FindElement(By.CssSelector(".help-search__internal-support-toptext-title")).Text,
                "open page, h2");
            VerifyAreEqual("Не нашли ответ на вопрос?",
                Driver.FindElement(By.CssSelector(".help-search__box-title")).Text, "callback title, h2");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".help-search__box-item")).Count > 0, "callback icon");
            VerifyAreEqual("Написать сообщение",
                Driver.FindElement(By.CssSelector(".help-search__box-item.help-search__mail span")).Text,
                "callback name messange");
            VerifyAreEqual("Позвонить нам",
                Driver.FindElement(By.CssSelector(".help-search__box-item.help-search__call-us span")).Text,
                "callback name call");

            //messange
            Driver.ScrollTo(By.CssSelector(".help-search__box-item"));
            Driver.FindElement(By.CssSelector(".help-search__box-item.help-search__mail")).Click();
            Thread.Sleep(100);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual("Написать сообщение", Driver.FindElement(By.TagName("h4")).Text, "open send messange, h1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".btn.btn_lg.btn_danger")).Count == 1, "btn send messange");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void OpenSuppotCenterCallBackPhone()
        {
            GoToAdmin();

            // driver.FindElement(By.XPath("//span[contains(text(), 'Помощь')]")).Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Поддержка')]")).Click();
            Driver.WaitForElem(By.ClassName("js-iframe-wrap"));
            Thread.Sleep(100);

            var iframe = Driver.FindElement(By.TagName("iframe"));
            Driver.SwitchTo().Frame(iframe);


            //call
            Driver.ScrollTo(By.CssSelector(".help-search__box-title"));
            Driver.FindElement(By.CssSelector(".help-search__box-item.help-search__call-us")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".call-us-info")).Count == 1, " open modal windows");
            VerifyAreEqual("Позвоните нам", Driver.FindElement(By.CssSelector(".call-us-info__title")).Text,
                " modal windows call, title");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".call-us-info__number-client-text"))[1].Text
                    .Contains("Ваш номер клиента: "), " modal windows call, account");
            VerifyAreEqual("Написать сообщение", Driver.FindElement(By.CssSelector(".call-us-info__write-us a")).Text,
                " modal windows call, send messange");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".call-us-info__write-us a")).GetAttribute("href").ToString()
                    .Contains("/support"), "href send messange");

            Driver.FindElement(By.CssSelector(".call-us-button-wrap .btn_primary")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal modal-dialog")).Count == 0, " close modal windows");
            VerifyIsTrue(Driver.Url.Contains("service/supportcenter"), "url supportcenter");
            VerifyAreEqual("Центр поддержки",
                Driver.FindElements(By.CssSelector(".help-search__internal-support-toptext-title"))[0].Text,
                "open page, h2");
        }
    }
}