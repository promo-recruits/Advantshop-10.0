using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class DesignTest : MySitesFunctions
    {
        string settingName = "Дизайн";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();

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
        public void ReviewDesignPageNOTCOMPLETE()
        {
            GoToStoreSettings(settingName);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".design-first")).Text.IndexOf("Домен магазина") != -1,
                "design page: domain");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".design-first")).Text.IndexOf(BaseUrl) != -1,
                "design page: domain path");
            Driver.FindElement(By.PartialLinkText(BaseUrl)).Click();
            Driver.SwitchTo().Window(Driver.WindowHandles[1]);
            VerifyAreEqual(BaseUrl.TrimEnd('/'), Driver.Url.TrimEnd('/'), "shop domain");
            Driver.Close();
            Driver.SwitchTo().Window(Driver.WindowHandles[0]);
            Thread.Sleep(1000);
            GoToStoreSettings(settingName);

            //страницы доменов            
            VerifyAreEqual("Привязать домен", Driver.FindElement(By.CssSelector(".design-first .btn-success")).Text,
                "domain btn1");
            Driver.FindElement(By.CssSelector(".design-first .btn-success")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(false, "check domain page is empty!");

            GoToStoreSettings(settingName);
            VerifyAreEqual("Управление доменами",
                Driver.FindElement(By.CssSelector(".design-first .btn-success ~ a")).Text, "domain btn2");
            Driver.FindElement(By.CssSelector(".design-first .btn-success ~ a")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(false, "check domain page is empty!");

            GoToStoreSettings(settingName);
            Driver.FindElement(By.CssSelector(".design-last .h4 a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual((BaseUrl + "/adminv3/dashboard/createsite?mode=store").Replace("//", "/"),
                Driver.Url.Replace("//", "/"), "templateshop url1");
            GoToStoreSettings(settingName);
            Driver.FindElement(By.CssSelector(".page-name-block ~ .btn-success")).Click();
            VerifyAreEqual((BaseUrl + "/adminv3/dashboard/createsite?mode=store").Replace("//", "/"),
                Driver.Url.Replace("//", "/"), "templateshop url1");
        }

        [Test]
        public void SetTemplate()
        {
            GoToStoreSettings(settingName);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".design-first")).Text.IndexOf("Стандартный") != -1,
                "design page: template name");
            VerifyAreEqual("Установленные шаблоны",
                Driver.FindElement(By.CssSelector(".sticky-page-name h2")).Text, "installed templates h2 default");
            VerifyAreEqual("Нет установленных шаблонов. Для установки перейдите в Магазин шаблонов",
                Driver.FindElement(By.CssSelector(".design-last .h4")).Text, "installed templates h4 default");

            Driver.FindElement(By.CssSelector(".page-name-block ~ .btn-success")).Click();

            //Установить шаблон
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"template.Metro\"] .btn-default"));
            Thread.Sleep(3000);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"template.Metro\"]"));
            Driver.GetByE2E("template.Metro").FindElement(By.LinkText("Подробнее")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector(".create__sites-left-block .btn-success")).Click();
            Thread.Sleep(5000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".design-first")).Text.IndexOf("Metro") != -1,
                "design page: template name");
            VerifyIsFalse(CheckConsoleLog("https://f003.backblazeb2.com/file/scrshots/"), "site scrshots error");

            GoToClient();
            VerifyIsTrue(Driver.PageSource.ToLower()
                             .Contains(BaseUrl + "templates/metro/design/colors/_none/styles/styles.css") &&
                         Driver.PageSource.ToLower()
                             .Contains(BaseUrl + "templates/metro/design/themes/_none/styles/styles.css") &&
                         Driver.PageSource.ToLower()
                             .Contains(BaseUrl + "templates/metro/design/backgrounds/_none/styles/styles.css"),
                "mainpage: fly color styles");
            VerifyIsTrue(
                Driver.PageSource.ToLower()
                    .Contains("http://cs71.advantshop.net/templates/metro/pictures/carousel/artboard.jpg"),
                "mainpage: fly carousel banners");

            GoToMobile();
            VerifyIsTrue(Driver.PageSource.ToLower().Contains("<meta name=\"theme-color\" content=\"#0095c0\">"),
                "mainpage mobile: template styles");
            VerifyIsTrue(
                Driver.PageSource.ToLower()
                    .Contains("http://cs71.advantshop.net/templates/metro/pictures/carousel/artboard.jpg"),
                "mainpage mobile: fly carousel banners");

            ReInit();

            //установить второй шаблон
            GoToStoreSettings(settingName);
            Driver.FindElement(By.CssSelector(".page-name-block ~ .btn-success")).Click();

            Driver.WaitForElem(By.CssSelector("[data-e2e=\"template.Fly\"] .btn-default"));
            Thread.Sleep(3000);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"template.Fly\"]"));
            Driver.GetByE2E("template.Fly").FindElement(By.LinkText("Подробнее")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector(".create__sites-left-block .btn-success")).Click();
            Thread.Sleep(5000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".design-first")).Text.IndexOf("Fly") != -1,
                "design page: template name");
            VerifyIsFalse(CheckConsoleLog("https://f003.backblazeb2.com/file/scrshots/"), "site scrshots error");

            GoToClient();
            VerifyIsTrue(Driver.PageSource.ToLower()
                             .Contains(BaseUrl + "templates/fly/design/colors/_none/styles/styles.css") &&
                         Driver.PageSource.ToLower()
                             .Contains(BaseUrl + "templates/fly/design/themes/_none/styles/styles.css") &&
                         Driver.PageSource.ToLower()
                             .Contains(BaseUrl + "templates/fly/design/backgrounds/_none/styles/styles.css"),
                "mainpage: fly color styles");
            VerifyIsTrue(Driver.PageSource.ToLower()
                             .Contains("http://cs71.advantshop.net/templates/fly/pictures/carousel/slide(1).jpg") &&
                         Driver.PageSource.ToLower()
                             .Contains("http://cs71.advantshop.net/templates/fly/pictures/carousel/slide(2).jpg") &&
                         Driver.PageSource.ToLower()
                             .Contains("http://cs71.advantshop.net/templates/fly/pictures/carousel/slide(3).jpg"),
                "mainpage: fly carousel banners");

            GoToMobile();
            VerifyIsTrue(Driver.PageSource.ToLower().Contains("<meta name=\"theme-color\" content=\"#5bbad2\">"),
                "mainpage mobile: template styles");
            VerifyIsTrue(Driver.PageSource.ToLower()
                             .Contains("http://cs71.advantshop.net/templates/fly/pictures/carousel/slide(1).jpg") &&
                         Driver.PageSource.ToLower()
                             .Contains("http://cs71.advantshop.net/templates/fly/pictures/carousel/slide(2).jpg") &&
                         Driver.PageSource.ToLower()
                             .Contains("http://cs71.advantshop.net/templates/fly/pictures/carousel/slide(3).jpg"),
                "mainpage mobile: fly carousel banners");

            ReInit();
            //online demo шаблона
            GoToStoreSettings(settingName);
            Driver.FindElement(By.LinkText("Online demo")).Click();
            Driver.SwitchTo().Window(Driver.WindowHandles[1]);
            VerifyAreEqual("https://www.advantshop.net/themestore/metro", Driver.Url.TrimEnd('/'), "shop domain");
            Driver.Close();
            Driver.SwitchTo().Window(Driver.WindowHandles[0]);

            //поменять шаблон
            GoToStoreSettings(settingName);
            Driver.FindElement(By.CssSelector("#templateId[value=\"Metro\"] + .btn-info")).Click();
            Thread.Sleep(3000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".design-first")).Text.IndexOf("Metro") != -1,
                "design page: template name");
            GoToClient();
            VerifyIsTrue(Driver.PageSource.ToLower()
                             .Contains(BaseUrl + "templates/metro/design/colors/_none/styles/styles.css") &&
                         Driver.PageSource.ToLower()
                             .Contains(BaseUrl + "templates/metro/design/themes/_none/styles/styles.css") &&
                         Driver.PageSource.ToLower()
                             .Contains(BaseUrl + "templates/metro/design/backgrounds/_none/styles/styles.css"),
                "mainpage: fly color styles");
            VerifyIsTrue(
                Driver.PageSource.ToLower()
                    .Contains("http://cs71.advantshop.net/templates/metro/pictures/carousel/artboard.jpg"),
                "mainpage: fly carousel banners");

            GoToMobile();
            VerifyIsTrue(Driver.PageSource.ToLower().Contains("<meta name=\"theme-color\" content=\"#0095c0\">"),
                "mainpage mobile: template styles");
            VerifyIsTrue(
                Driver.PageSource.ToLower()
                    .Contains("http://cs71.advantshop.net/templates/metro/pictures/carousel/artboard.jpg"),
                "mainpage mobile: fly carousel banners");

            ReInit();

            //удалить шаблон
            GoToStoreSettings(settingName);
            VerifyAreEqual(
                Driver.FindElements(By.TagName("tr"))[1].FindElement(By.CssSelector(".tpl-table__td.bold")).Text,
                "Стандартный", "first standart template");
            VerifyAreEqual(
                Driver.FindElements(By.TagName("tr"))[2].FindElement(By.CssSelector(".tpl-table__td.bold")).Text, "Fly",
                "second template");
            VerifyIsTrue(Driver.FindElements(By.TagName("tr"))[1].Text.IndexOf("Бесплатно") != -1,
                "first standart template price");
            VerifyAreEqual(0,
                Driver.FindElements(By.TagName("tr"))[1].FindElements(By.CssSelector("button[type=\"button\"]")).Count,
                "standart remove button");

            Driver.FindElements(By.TagName("tr"))[2].FindElement(By.CssSelector("button[type=\"button\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual(2, Driver.FindElements(By.TagName("tr")).Count, "remove template");

            Driver.FindElements(By.TagName("tr"))[1].FindElement(By.CssSelector("button[type=\"button\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual(0, Driver.FindElements(By.TagName("tr")).Count, "remove template");
            VerifyAreEqual("Нет установленных шаблонов. Для установки перейдите в Магазин шаблонов",
                Driver.FindElement(By.CssSelector(".design-last .h4")).Text, "installed templates h4 default");
        }

        [Test]
        public void ReviewDomainsPageNOTCOMPLETE()
        {
            GoToStoreSettings(settingName);

            VerifyIsTrue(false, "test not work");
        }
    }
}