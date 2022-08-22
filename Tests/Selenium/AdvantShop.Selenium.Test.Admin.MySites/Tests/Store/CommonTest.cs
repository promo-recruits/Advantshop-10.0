using System;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class SettingsCommonTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();
            InitializeService.SetCustomLogoAndFavicon();

            Init();
        }
        //дописать строки кода для проверки происходящего в мобилке - 
        //для альт-тега, логотипа, storename, когда поправят настройки современной мобилки

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
        public void ChangeStoreName()
        {
            GoToAdmin("settings/common");
            VerifyAreEqual("Мой магазин", Driver.FindElement(By.Name("StoreName")).GetAttribute("value"),
                "default alt admin");
            GoToClient();
            VerifyAreEqual("Мой магазин", Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"),
                "default title");
            VerifyAreEqual("Мой магазин",
                Driver.FindElement(By.CssSelector("meta[name='Description']")).GetAttribute("content"),
                "default description");
            VerifyAreEqual("Мой магазин",
                Driver.FindElement(By.CssSelector("meta[name='Keywords']")).GetAttribute("content"),
                "default keywords");
            GoToClient("categories/kategoriya-1");
            VerifyAreEqual("Мой магазин - Категория 1",
                Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"), "default title in category");
            VerifyAreEqual("Мой магазин - Категория 1",
                Driver.FindElement(By.CssSelector("meta[name='Description']")).GetAttribute("content"),
                "default description in category");
            VerifyAreEqual("Мой магазин - Категория 1",
                Driver.FindElement(By.CssSelector("meta[name='Keywords']")).GetAttribute("content"),
                "default keywords in category");

            GoToAdmin("settings/common");
            Driver.FindElement(By.Name("StoreName")).Clear();
            Driver.GetByE2E("btnSave").FindElement(By.ClassName("btn-success")).Click();
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-message")).Text.IndexOf("Не заполнены поля") != -1,
                "error message");
            Driver.FindElement(By.CssSelector(".toast-error .toast-close-button")).Click();
            Driver.SendKeysInput(By.Name("StoreName"), "My-custom-name", false);
            SaveCommonSettings();
            VerifyAreEqual("My-custom-name", Driver.FindElement(By.Name("StoreName")).GetAttribute("value"),
                "custom alt admin");

            GoToClient();
            VerifyAreEqual("My-custom-name", Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"),
                "custom title");
            VerifyAreEqual("My-custom-name",
                Driver.FindElement(By.CssSelector("meta[name='Description']")).GetAttribute("content"),
                "custom description");
            VerifyAreEqual("My-custom-name",
                Driver.FindElement(By.CssSelector("meta[name='Keywords']")).GetAttribute("content"), "custom keywords");
            GoToClient("categories/kategoriya-1");
            VerifyAreEqual("My-custom-name - Категория 1",
                Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"), "custom title in category");
            VerifyAreEqual("My-custom-name - Категория 1",
                Driver.FindElement(By.CssSelector("meta[name='Description']")).GetAttribute("content"),
                "custom description in category");
            VerifyAreEqual("My-custom-name - Категория 1",
                Driver.FindElement(By.CssSelector("meta[name='Keywords']")).GetAttribute("content"),
                "custom keywords in category");

            GoToAdmin("settings/common");
            Driver.SendKeysInput(By.Name("StoreName"), "My-custom %name#!.. +-*/ Hello_world$,   ~favorite'\"", true);
            SaveCommonSettings();
            VerifyAreEqual("My-custom %name#!.. +-*/ Hello_world$,   ~favorite'\"",
                Driver.FindElement(By.Name("StoreName")).GetAttribute("value"), "custom alt admin");

            GoToClient();
            VerifyAreEqual("My-custom %name#!.. +-*/ Hello_world$,   ~favorite'\"",
                Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"), "custom title");
            VerifyAreEqual("My-custom %name#!.. +-*/ Hello_world$,   ~favorite'\"",
                Driver.FindElement(By.CssSelector("meta[name='Description']")).GetAttribute("content"),
                "custom description");
            VerifyAreEqual("My-custom %name#!.. +-*/ Hello_world$,   ~favorite'\"",
                Driver.FindElement(By.CssSelector("meta[name='Keywords']")).GetAttribute("content"), "custom keywords");
            GoToClient("categories/kategoriya-1");
            VerifyAreEqual("My-custom %name#!.. +-*/ Hello_world$,   ~favorite'\" - Категория 1",
                Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"), "custom title in category");
            VerifyAreEqual("My-custom %name#!.. +-*/ Hello_world$,   ~favorite'\" - Категория 1",
                Driver.FindElement(By.CssSelector("meta[name='Description']")).GetAttribute("content"),
                "custom description in category");
            VerifyAreEqual("My-custom %name#!.. +-*/ Hello_world$,   ~favorite'\" - Категория 1",
                Driver.FindElement(By.CssSelector("meta[name='Keywords']")).GetAttribute("content"),
                "custom keywords in category");
        }

        [Test]
        public void ChangeStoreUrl()
        {
            string url = "http://mydomain123.ru";
            Functions.SingInYandex(Driver);
            Functions.MailSmtp(Driver, BaseUrl);
            SetCheckoutMailDisable();

            GoToAdmin("settings/common");
            VerifyAreEqual(url, Driver.GetValue(By.Name("StoreUrl")), "default url admin");

            GoToAdmin("design");
            VerifyAreEqual(url, Driver.GetByE2E("storeUrlLink").Text, "default url in design");
            GoToAdmin("settingsapi");
            VerifyIsTrue(Driver.GetValue(By.Name("LeadAddUrl")).IndexOf(url) != -1, "default url in settingsapi");
            GoToAdmin("settingssystem#?systemTab=sitemap");
            VerifyIsTrue(Driver.PageSource.IndexOf(url) != -1, "default url in sitemap");
            CheckStoreUrlInGiftCertificate(url);

            GoToAdmin("settings/common");
            string newUrl = "https://dev.custom-domain.com";
            Driver.FindElement(By.Name("StoreUrl")).Clear();
            Driver.GetByE2E("btnSave").FindElement(By.ClassName("btn-success")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-message")).Text.IndexOf("Не заполнены поля") != -1,
                "error message");
            Driver.FindElement(By.CssSelector(".toast-error .toast-close-button")).Click();
            Driver.FindElement(By.Name("StoreUrl")).SendKeys(newUrl);
            SaveCommonSettings();
            Refresh();
            VerifyAreEqual(newUrl, Driver.GetValue(By.Name("StoreUrl")), "custom alt admin");

            GoToAdmin("design");
            VerifyAreEqual(newUrl, Driver.GetByE2E("storeUrlLink").Text, "custom url in design");
            GoToAdmin("settingsapi");
            VerifyIsFalse(Driver.GetValue(By.Name("LeadAddUrl")).IndexOf(url) != -1, "old url in settingsapi");
            VerifyIsTrue(Driver.GetValue(By.Name("LeadAddUrl")).IndexOf(newUrl) != -1, "custom url in settingsapi");
            GoToAdmin("settingssystem#?systemTab=sitemap");
            VerifyIsFalse(Driver.PageSource.IndexOf(url) != -1, "old url in sitemap");
            VerifyIsTrue(Driver.PageSource.IndexOf(newUrl) != -1, "custom url in sitemap");
            CheckStoreUrlInGiftCertificate(newUrl);
        }

        [Test]
        public void ChangeLogo()
        {
            By logoSelector = By.CssSelector("img#logo");
            string logoContainerClass = ".setting-logo-wrap";
            By logoImgSelector = By.CssSelector(logoContainerClass + " img");

            GoToClient();
            VerifyAreEqual(BaseUrl + "pictures/my_logo.png", Driver.FindElement(logoSelector).GetAttribute("src"),
                "default href");

            GoToAdmin("settings/common");
            VerifyAreEqual(BaseUrl + "pictures/my_logo.png", Driver.FindElement(logoImgSelector).GetAttribute("src"),
                "default href admin");

            SetImgByHref(logoContainerClass, "https://www.advantshop.net/content/favicon.svg", false);
            VerifyIsTrue(Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf(".png") != -1,
                "invalid format1 in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(logoSelector).GetAttribute("src").IndexOf(".png") != -1, "invalid format1");

            GoToAdmin("settings/common");
            //удалить
            Driver.FindElement(By.CssSelector(logoContainerClass + " [data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf("/images/nophoto_small.jpg") != -1,
                "removed logo in admin");
            GoToClient();
            VerifyAreEqual(BaseUrl + "images/nophoto-logo.png", Driver.FindElement(logoSelector).GetAttribute("src"),
                "removed logo");

            GoToAdmin("settings/common");
            SetImgByHref(logoContainerClass, "https://ru.wikipedia.org/static/favicon/wikipedia.ico", false);
            VerifyIsTrue(
                Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf("/images/nophoto_small.jpg") != -1,
                "invalid format2 admin");
            GoToClient();
            VerifyAreEqual(BaseUrl + "images/nophoto-logo.png", Driver.FindElement(logoSelector).GetAttribute("src"),
                "invalid format2");

            GoToAdmin("settings/common");
            SetImgByHref(logoContainerClass, "https://www.advantshop.net/content/opinion/images/antufev.jpg");
            VerifyIsTrue(Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf(".jpg") != -1,
                "valid format jpg in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(logoSelector).GetAttribute("src").IndexOf(".jpg") != -1,
                "valid format jpg");

            GoToAdmin("settings/common");
            SetImgByHref(logoContainerClass, "https://www.advantshop.net/xsmall.png");
            VerifyIsTrue(Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf(".png") != -1,
                "valid format png in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(logoSelector).GetAttribute("src").IndexOf(".png") != -1,
                "valid format png");

            GoToAdmin("settings/common");
            SetImgByHref(logoContainerClass,
                "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Rotating_earth_%28large%29.gif/274px-Rotating_earth_%28large%29.gif");
            VerifyIsTrue(Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "valid format gif in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(logoSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "valid format gif");


            GoToAdmin("settings/common");
            VerifyIsTrue(Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "upload: default href admin");
            SetImg("icon.svg", "Logo", false);
            VerifyIsTrue(Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "upload: invalid format1 in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(logoSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "upload: invalid format1");

            GoToAdmin("settings/common");
            //удалить
            Driver.FindElement(By.CssSelector(logoContainerClass + " [data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf("/images/nophoto_small.jpg") != -1,
                "upload: removed logo in admin");
            GoToClient();
            VerifyAreEqual(BaseUrl + "images/nophoto-logo.png", Driver.FindElement(logoSelector).GetAttribute("src"),
                "removed logo");

            GoToAdmin("settings/common");
            SetImg("icon.ico", "Logo", false);
            VerifyIsTrue(
                Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf("/images/nophoto_small.jpg") != -1,
                "upload: invalid format2 admin");
            GoToClient();
            VerifyAreEqual(BaseUrl + "images/nophoto-logo.png", Driver.FindElement(logoSelector).GetAttribute("src"),
                "upload: invalid format2");

            GoToAdmin("settings/common");
            SetImg("icon.jpg", "Logo");
            VerifyIsTrue(Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf(".jpg") != -1,
                "upload: valid format jpg in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(logoSelector).GetAttribute("src").IndexOf(".jpg") != -1,
                "upload: valid format jpg");

            GoToAdmin("settings/common");
            SetImg("icon.png", "Logo");
            VerifyIsTrue(Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf(".png") != -1,
                "upload: valid format png in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(logoSelector).GetAttribute("src").IndexOf(".png") != -1,
                "upload: valid format png");

            GoToAdmin("settings/common");
            SetImg("icon.gif", "Logo");
            VerifyIsTrue(Driver.FindElement(logoImgSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "upload: valid format gif in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(logoSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "upload: valid format gif");
        }

        [Test]
        public void ChangeAltTag()
        {
            GoToAdmin("settings/common");
            VerifyAreEqual("Мой магазин", Driver.GetValue(By.Name("LogoImgAlt")), "default alt admin");
            GoToClient();
            VerifyAreEqual("Мой магазин", Driver.FindElement(By.CssSelector("img#logo")).GetAttribute("alt"),
                "default alt");

            GoToAdmin("settings/common");
            Driver.FindElement(By.Name("LogoImgAlt")).Clear();
            SaveCommonSettings();
            VerifyIsTrue(string.IsNullOrEmpty(Driver.GetValue(By.Name("LogoImgAlt"))), "empty alt admin");
            GoToClient();
            VerifyIsTrue(string.IsNullOrEmpty(Driver.FindElement(By.CssSelector("img#logo")).GetAttribute("alt")),
                "empty alt");

            GoToAdmin("settings/common");
            Driver.SendKeysInput(By.Name("LogoImgAlt"), "My-custom %tag#!.. +-*/ Hello_world$, ~favorite'\"");
            SaveCommonSettings();
            VerifyAreEqual("My-custom %tag#!.. +-*/ Hello_world$, ~favorite'\"", Driver.GetValue(By.Name("LogoImgAlt")),
                "custom alt admin");
            GoToClient();
            VerifyAreEqual("My-custom %tag#!.. +-*/ Hello_world$, ~favorite'",
                Driver.FindElement(By.CssSelector("img#logo")).GetAttribute("alt"), "custom alt");
        }

        [Test]
        public void ChangeFavicon()
        {
            By faviconSelector = By.CssSelector("link[rel=\"shortcut icon\"]");
            string faviconContainerClass = ".setting-favicon-wrap";
            By faviconImgSelector = By.CssSelector(faviconContainerClass + " img");

            GoToClient();
            VerifyAreEqual("image/png", Driver.FindElement(faviconSelector).GetAttribute("type"), "default type");
            VerifyAreEqual(BaseUrl + "pictures/my_favicon.png",
                Driver.FindElement(faviconSelector).GetAttribute("href"), "default href");

            GoToAdmin("settings/common");
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".ico") != -1,
                "default href admin");

            SetImgByHref(faviconContainerClass, "https://www.advantshop.net/content/favicon.svg", false);
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf("/my_favicon.png") != -1,
                "invalid format1 in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf("/my_favicon.png") != -1,
                "invalid format1");

            GoToAdmin("settings/common");
            //удалить
            Driver.FindElement(By.CssSelector(faviconContainerClass + " [data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf("/images/nophoto_small.jpg") != -1,
                "removed favicon in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf("/my_favicon.png") != -1,
                "removed favicon");

            GoToAdmin("settings/common");
            SetImgByHref(faviconContainerClass, "https://www.advantshop.net/content/opinion/images/antufev.jpg", false);
            VerifyIsTrue(
                Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf("/images/nophoto_small.jpg") != -1,
                "removed in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf("/my_favicon.png") != -1,
                "invalid format2");

            GoToAdmin("settings/common");
            SetImgByHref(faviconContainerClass, "https://www.advantshop.net/xsmall.png");
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".png") != -1,
                "valid format png in admin");
            GoToClient();
            VerifyAreEqual("image/png", Driver.FindElement(faviconSelector).GetAttribute("type"), "valid type png");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".png") != -1,
                "valid format png");

            GoToAdmin("settings/common");
            SetImgByHref(faviconContainerClass, "https://ru.wikipedia.org/static/favicon/wikipedia.ico");
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".ico") != -1,
                "valid format ico in admin");
            GoToClient();
            VerifyAreEqual("image/x-icon", Driver.FindElement(faviconSelector).GetAttribute("type"), "valid type ico");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".ico") != -1,
                "valid format ico");

            GoToAdmin("settings/common");
            SetImgByHref(faviconContainerClass,
                "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Rotating_earth_%28large%29.gif/274px-Rotating_earth_%28large%29.gif");
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "valid format gif in admin");
            GoToClient();
            VerifyAreEqual("image/gif", Driver.FindElement(faviconSelector).GetAttribute("type"), "valid type gif");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".gif") != -1,
                "valid format gif");


            GoToAdmin("settings/common");
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "upload: default href admin");
            SetImg("icon.svg", "Favicon", false);
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "upload: invalid format1 in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".gif") != -1,
                "upload: invalid format1");

            GoToAdmin("settings/common");
            //удалить
            Driver.FindElement(By.CssSelector(faviconContainerClass + " [data-e2e=\"imgDel\"]")).Click();
            Driver.SwalConfirm();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf("/images/nophoto_small.jpg") != -1,
                "upload: removed favicon in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf("/my_favicon.png") != -1,
                "upload: removed favicon");

            GoToAdmin("settings/common");
            SetImg("icon.jpg", "Favicon", false);
            VerifyIsTrue(
                Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf("/images/nophoto_small.jpg") != -1,
                "upload: removed in admin");
            GoToClient();
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf("/my_favicon.png") != -1,
                "upload: invalid format2");

            GoToAdmin("settings/common");
            SetImg("icon.png", "Favicon");
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".png") != -1,
                "upload: valid format png in admin");
            GoToClient();
            VerifyAreEqual("image/png", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type png");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".png") != -1,
                "upload: valid format png");

            GoToAdmin("settings/common");
            SetImg("icon.ico", "Favicon");
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".ico") != -1,
                "upload: valid format ico in admin");
            GoToClient();
            VerifyAreEqual("image/x-icon", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type ico");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".ico") != -1,
                "upload: valid format ico");

            GoToAdmin("settings/common");
            SetImg("icon.gif", "Favicon");
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "upload: valid format gif in admin");
            GoToClient();
            VerifyAreEqual("image/gif", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type gif");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".gif") != -1,
                "upload: valid format gif");
        }

        [Test]
        public void ChangeLocation()
        {
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Id("CountryId"));
            VerifyAreEqual("Россия", GetSelectedOptionText("CountryId"), "default country");
            VerifyAreEqual("Москва", GetSelectedOptionText("RegionSelect"), "default region");
            VerifyAreEqual("Москва", GetInputValue("City"), "default city");

            SelectItem("RegionSelect", "Самарская область");
            VerifyIsTrue(GetSelect(By.Id("RegionSelect")).WrappedElement.Text.IndexOf("Республика Татарстан") != -1,
                "region of Rus when country is Rus");
            VerifyIsFalse(GetSelect(By.Id("RegionSelect")).WrappedElement.Text.IndexOf("Запорожская область") != -1,
                "region of Ukr when country is Rus");
            SaveCommonSettings();
            Driver.ScrollTo(By.Id("CountryId"));
            VerifyAreEqual("Россия", GetSelectedOptionText("CountryId"), "Russia country");
            VerifyAreEqual("Самарская область", GetSelectedOptionText("RegionSelect"), "Samara region");
            VerifyAreEqual("Москва", GetInputValue("City"), "Moscow city");

            SelectItem("CountryId", "Индия");
            VerifyIsTrue(Driver.FindElements(By.Name("RegionSelect")).Count == 0, "Country without region");
            SaveCommonSettings();
            Driver.ScrollTo(By.Id("CountryId"));
            VerifyAreEqual("Индия", GetSelectedOptionText("CountryId"), "India country");
            VerifyIsTrue(Driver.FindElements(By.Name("RegionSelect")).Count == 0, "Country without region2");
            VerifyAreEqual("Москва", GetInputValue("City"), "Moscow city2");

            Driver.FindElement(By.Name("CountryId")).SendKeys("Каза" + Keys.Enter);
            VerifyAreEqual("Казахстан", GetSelectedOptionText("CountryId"), "Qazaqstan country");
            VerifyIsTrue(Driver.FindElements(By.Name("RegionSelect")).Count == 1, "Country with region");
            //не срабатывает, т.к. нет нормального клика (не по чему)
            //VerifyAreEqual("Акмолинская область", GetSelectedOptionText("RegionSelect"), "Akmolinsk' region(default)");
            Driver.FindElement(By.Name("RegionSelect")).SendKeys("Нур" + Keys.Enter);
            VerifyAreEqual("Нур-Султан", GetSelectedOptionText("RegionSelect"), "Nur-sultan region");
            VerifyIsTrue(GetSelect(By.Id("RegionSelect")).WrappedElement.Text.IndexOf("Карагандинская область") != -1,
                "region of Qaz when country is Qaz");
            VerifyIsFalse(GetSelect(By.Id("RegionSelect")).WrappedElement.Text.IndexOf("Москва") != -1,
                "region of Rus when country is Qaz");
            SaveCommonSettings();
            Driver.ScrollTo(By.Id("CountryId"));
            VerifyAreEqual("Казахстан", GetSelectedOptionText("CountryId"), "Qazaqstan country2");
            VerifyAreEqual("Нур-Султан", GetSelectedOptionText("RegionSelect"), "Nur-sultan region2");
            VerifyAreEqual("Москва", GetInputValue("City"), "Moscow city3");

            //потом уже с городами
            //перейти в десктоп и мобилку, убедиться, что москва.
            //обновить - москва не пропала
            //reinitclient - москва
            GoToClient();

            VerifyAreEqual("Москва", GetCurrentCity(), "default city for admin - desktop");
            Refresh();
            VerifyAreEqual("Москва", GetCurrentCity(), "default city for admin - desktop2");
            VerifyAreEqual("Москва", GetIpzoneCookie(4), "default city for admin - cookie");
            GoToMobile();
            VerifyAreEqual("Москва", GetCurrentCity(true), "default city for admin - mobile");
            Refresh();
            VerifyAreEqual("Москва", GetCurrentCity(true), "default city for admin - mobile2");

            ReInitClient();
            GoToClient();
            VerifyAreEqual("Москва", GetCurrentCity(), "default city for client - desktop");
            GoToMobile();
            VerifyAreEqual("Москва", GetCurrentCity(true), "default city for client - mobile2");

            //изменить в админке город на Ростов
            //проверить в десктопе и мобилке - ростов
            //reinitClient - ростов
            //reinit, в клиенке изменить город на Питер, 
            //проверить в десктопе и мобилке - питер
            //reinitclient - ростов
            //проверить десктоп и мобилку
            ReInit();
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Id("City"));
            Driver.SendKeysInput(By.Name("City"), "Ростов-на-Дону");
            SaveCommonSettings();
            VerifyAreEqual("Ростов-на-Дону", GetInputValue("City"), "Rostov city");

            GoToClient();
            VerifyAreEqual("Москва", GetCurrentCity(), "new city for admin - desktop, cookie not refreshed");
            VerifyAreEqual("Москва", GetIpzoneCookie(4), "default city for admin - cookie, cookie not refreshed");
            GoToMobile();
            VerifyAreEqual("Москва", GetCurrentCity(true), "new city for admin - mobile, cookie not refreshed");

            ReInit();
            GoToClient("?112");
            Refresh();
            VerifyAreEqual("Ростов-на-Дону", GetCurrentCity(), "new city for admin - desktop, cookie refreshed");
            VerifyAreEqual("Ростов-на-Дону", GetIpzoneCookie(4), "default city for admin - cookie, cookie refreshed");
            GoToMobile();
            VerifyAreEqual("Ростов-на-Дону", GetCurrentCity(true), "new city for admin - mobile, cookie refreshed");

            ReInitClient();
            GoToClient();
            VerifyAreEqual("Ростов-на-Дону", GetCurrentCity(), "new city for client - desktop");
            GoToMobile();
            VerifyAreEqual("Ростов-на-Дону", GetCurrentCity(true), "new city for client - mobile");

            ReInit();
            GoToClient("?112");
            VerifyAreEqual("Ростов-на-Дону", GetIpzoneCookie(4), "custom city - cookie before");
            Functions.SetCity(Driver, "Санкт-Петербург");
            VerifyAreEqual("Санкт-Петербург", GetIpzoneCookie(4), "custom city - new cookie before refresh");
            Refresh();
            VerifyAreEqual("Санкт-Петербург", GetIpzoneCookie(4), "custom city - new cookie after refresh");
            VerifyAreEqual("Санкт-Петербург", GetCurrentCity(), "custom city for admin - desktop");
            Driver.FindElement(By.LinkText("Выйти")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Санкт-Петербург", GetIpzoneCookie(4), "custom city - new cookie after logout");
            VerifyAreEqual("Санкт-Петербург", GetCurrentCity(), "custom city for admin - desktop after logout");
            GoToMobile();
            VerifyAreEqual("Санкт-Петербург", GetCurrentCity(true), "custom city for admin - mobile");

            ReInitClient();
            GoToClient();
            VerifyAreEqual("Ростов-на-Дону", GetCurrentCity(), "new city for client - desktop2");
            GoToMobile();
            VerifyAreEqual("Ростов-на-Дону", GetCurrentCity(true), "new city for client - mobile2");

            //изменить город на Нурсултан 
            //в клиентке - Питер - нет, так не работает...
            //в reinitclient - нурсултан
            //проверить десктоп и мобилку
            ReInit();
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Id("City"));
            Driver.SendKeysInput(By.Name("City"), "Нур-Султан");
            SaveCommonSettings();

            ReInit();
            GoToClient("?112");
            Refresh();
            VerifyAreEqual("Нур-Султан", GetCurrentCity(), "custom city for admin - desktop2");
            GoToMobile();
            VerifyAreEqual("Нур-Султан", GetCurrentCity(true), "custom city for admin - mobile2");

            ReInitClient();
            GoToClient();
            VerifyAreEqual("Нур-Султан", GetCurrentCity(), "new city for client - desktop3");
            GoToMobile();
            VerifyAreEqual("Нур-Султан", GetCurrentCity(true), "new city for client - mobile3");
            ReInit();
        }

        [Test]
        public void ChangePhones()
        {
            string[] cities = {"Москва", "Махачкала", "Днепропетровск"};
            string phone = "+7 (495) 000-00-00";
            string phoneHtml = "+7 (495) 000-00-00";
            string mobilePhone = "+74950000000";

            //стандартный номер в десктопе
            //+в мобилке
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("Phone"));
            VerifyAreEqual(phone, Driver.GetValue(By.Name("Phone")), "default phone admin");
            VerifyAreEqual(mobilePhone, Driver.GetValue(By.Name("MobilePhone")), "default mobile phone admin");
            CheckSamePhones(cities, phone, phoneHtml, mobilePhone, "default");

            //изменить стандартный номер
            //+ в мобилке
            phoneHtml = phone = "8-901-781-45-67";
            mobilePhone = "89017814567";
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("Phone"));
            Driver.SendKeysInput(By.Name("Phone"), phoneHtml);
            Thread.Sleep(500);
            VerifyAreEqual("+79017814567", Driver.GetValue(By.Name("MobilePhone")), "autocomplete mobile phone admin");
            Driver.ScrollToTop();
            SaveCommonSettings();
            Driver.ScrollTo(By.Name("Phone"));
            VerifyAreEqual("+79017814567", Driver.GetValue(By.Name("MobilePhone")), "autocomplete2 mobile phone admin");
            Driver.SendKeysInput(By.Name("MobilePhone"), phoneHtml);
            Thread.Sleep(500);
            Driver.ScrollToTop();
            SaveCommonSettings();
            VerifyAreEqual(phoneHtml, Driver.GetValue(By.Name("Phone")), "changed phone admin");
            VerifyAreEqual(mobilePhone, Driver.GetValue(By.Name("MobilePhone")), "changed mobile phone admin");
            CheckSamePhones(cities, phone, phoneHtml, mobilePhone, "changed");

            //два стандартных номера
            //+ в мобилке (невалидный)
            phone = "+7 (5747) 77-57-72\r\n8-901-781-45-67";
            phoneHtml = "+7 (5747) 77-57-72<br>8-901-781-45-67";
            mobilePhone = "+75747775772";
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("Phone"));
            Driver.SendKeysInput(By.Name("Phone"), phoneHtml);
            Thread.Sleep(500);
            VerifyAreEqual("+", Driver.GetValue(By.Name("MobilePhone")), "autocomplete mobile 2 phones admin");
            SaveCommonSettings();
            Driver.ScrollTo(By.Name("Phone"));
            VerifyAreEqual(String.Empty, Driver.GetValue(By.Name("MobilePhone")),
                "autocomplete2 mobile 2 phones admin");

            Thread.Sleep(500);
            Driver.SendKeysInput(By.Name("MobilePhone"), phoneHtml);
            SaveCommonSettings();
            VerifyAreEqual(phoneHtml, Driver.GetValue(By.Name("Phone")), "2 phones admin");
            VerifyAreEqual(String.Empty, Driver.GetValue(By.Name("MobilePhone")), "2 phones mobile");
            CheckSamePhones(cities, phone, phoneHtml, String.Empty, "two phones");

            //номер с версткой
            //+ в мобилке (валидный)
            phone = "8-901-781-45-67";
            phoneHtml = "<b><span style=\"color:green;font-size:40px;\">8-901-781-45-67</span></b>";
            mobilePhone = "+79017814567";
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("Phone"));
            Driver.SendKeysInput(By.Name("Phone"), phoneHtml);
            Thread.Sleep(500);
            VerifyAreEqual(mobilePhone, Driver.GetValue(By.Name("MobilePhone")),
                "autocomplete phone's html admin mobile");
            SaveCommonSettings();
            Driver.ScrollTo(By.Name("Phone"));
            VerifyAreEqual(mobilePhone, Driver.GetValue(By.Name("MobilePhone")),
                "autocomplete2 phone's html admin mobile");
            VerifyAreEqual(phoneHtml, Driver.GetValue(By.Name("Phone")), "phone's html admin");
            VerifyAreEqual(mobilePhone, Driver.GetValue(By.Name("MobilePhone")), "phone's html mobile");
            CheckSamePhones(cities, phone, phoneHtml, mobilePhone, "phone's html");

            //номер для другого города
            //+ в мобилке
            string phone1 = "+75747771235";
            string phoneD2 = "89017819876";
            string phoneM2 = "89015514724";
            string phoneD3 = "+380997447135";
            string phoneM3 = "+380215498721";

            GoToAdmin("settings/common");
            Driver.GetByE2E("ChooseOtherCityPhonesLink").Click();
            Thread.Sleep(2000);

            Driver.SwitchTo().Window(Driver.WindowHandles[1]);
            Thread.Sleep(1000);
            Driver.GetGridIdFilter("gridCity", cities[1]);
            Driver.SendKeysGridCell(phone1, 0, "PhoneNumber", "City");
            Driver.SendKeysGridCell(phone1, 0, "MobilePhoneNumber", "City");
            //задан только один номер
            Driver.GetGridIdFilter("gridCity", "Санкт-Петербург");
            Driver.SendKeysGridCell(phone1, 0, "PhoneNumber", "City");
            Driver.GetGridIdFilter("gridCity", "Петрозаводск");
            Driver.SendKeysGridCell(phone1, 0, "MobilePhoneNumber", "City");

            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridIdFilter("gridCountry", "Украина");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(500);
            Driver.GetByE2E("GoToCity").Click();
            Thread.Sleep(500);

            Driver.GetGridIdFilter("gridCity", cities[2]);
            Driver.SendKeysGridCell(phoneD2, 0, "PhoneNumber", "City");
            Driver.SendKeysGridCell(phoneM2, 0, "MobilePhoneNumber", "City");

            Driver.GetGridIdFilter("gridCity", "Ужгород");
            Driver.SendKeysGridCell(phoneD3, 0, "PhoneNumber", "City");
            Driver.SendKeysGridCell(phoneM3, 0, "MobilePhoneNumber", "City");

            CheckDifferentPhones(new string[7, 3]
            {
                {cities[0], phone, mobilePhone},
                {"Вологда", phone, mobilePhone},
                {cities[1], phone1, phone1},
                {"Санкт-Петербург", phone1, mobilePhone},
                {"Петрозаводск", phone, phone1},
                {cities[2], phoneD2, phoneM2},
                {"Ужгород", phoneD3, phoneM3}
            }, "different cities");
        }

        [Test]
        public void ChangePhoneMobileValidation()
        {
            string[] cities = {"Москва"};
            string phoneDefault = "+7 (495) 000-00-00";
            string mobilePhoneDefault = "+74950000000";

            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("Phone"));
            VerifyAreEqual(mobilePhoneDefault, Driver.GetValue(By.Name("MobilePhone")), "default mobile phone admin");

            //пустая строка
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.FindElement(By.Name("MobilePhone")).Clear();
            SaveCommonSettings();
            VerifyAreEqual(String.Empty, Driver.GetValue(By.Name("MobilePhone")), "empty mobile phone admin");
            CheckPhoneMobile(cities, String.Empty, "empty");

            //корректный номер с +7
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "+79012345678");
            SaveCommonSettings();
            VerifyAreEqual("+79012345678", Driver.GetValue(By.Name("MobilePhone")), "+7- mobile phone admin");
            CheckPhoneMobile(cities, "+79012345678", "+7-");

            //корректный номер с 7
            //это не очень хорошо, потому что в андроиде работает только с +7
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "79012345678");
            SaveCommonSettings();
            VerifyAreEqual("79012345678", Driver.GetValue(By.Name("MobilePhone")), "7- mobile phone admin");
            CheckPhoneMobile(cities, "79012345678", "7-");

            //корректный номер с 8
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "89012345678");
            SaveCommonSettings();
            VerifyAreEqual("89012345678", Driver.GetValue(By.Name("MobilePhone")), "8- mobile phone admin");
            CheckPhoneMobile(cities, "89012345678", "8-");

            //корректный украинский номер с +380
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "+380754312687");
            SaveCommonSettings();
            VerifyAreEqual("+380754312687", Driver.GetValue(By.Name("MobilePhone")), "+380- mobile phone admin");
            CheckPhoneMobile(cities, "+380754312687", "+380-");

            //корректный украинский номер с 380
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "380754312687");
            SaveCommonSettings();
            VerifyAreEqual("380754312687", Driver.GetValue(By.Name("MobilePhone")), "380- mobile phone admin");
            CheckPhoneMobile(cities, "380754312687", "380-");

            //корректный беларусский номер с +375
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "+375987654321");
            SaveCommonSettings();
            VerifyAreEqual("+375987654321", Driver.GetValue(By.Name("MobilePhone")), "+375- mobile phone admin");
            CheckPhoneMobile(cities, "+375987654321", "+375-");

            //корректный беларусский номер с 375
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "375987654321");
            SaveCommonSettings();
            VerifyAreEqual("375987654321", Driver.GetValue(By.Name("MobilePhone")), "375- mobile phone admin");
            CheckPhoneMobile(cities, "375987654321", "375-");

            //корректный киргизский номер с +996
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "+996987654321");
            SaveCommonSettings();
            VerifyAreEqual("+996987654321", Driver.GetValue(By.Name("MobilePhone")), "+996- mobile phone admin");
            CheckPhoneMobile(cities, "+996987654321", "+996-");

            //корректный киргизский номер с 996
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "996987654321");
            SaveCommonSettings();
            VerifyAreEqual("996987654321", Driver.GetValue(By.Name("MobilePhone")), "996- mobile phone admin");
            CheckPhoneMobile(cities, "996987654321", "996-");

            //номер из 12 символов, начинающийся с рандомных цифр
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "+357987654321");
            SaveCommonSettings();
            VerifyAreEqual("+357987654321", Driver.GetValue(By.Name("MobilePhone")), "+rnd- mobile phone admin");
            CheckPhoneMobile(cities, "+357987654321", "+rnd-");

            //номер из 6 символов, начинающийся с рандомных цифр
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "999990");
            SaveCommonSettings();
            VerifyAreEqual("999990", Driver.GetValue(By.Name("MobilePhone")), "6rnd- mobile phone admin");
            CheckPhoneMobile(cities, "999990", "6rnd-");

            //некорректная строка без цифр до 19 симв
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "`~q!@#$%^&*(-_+\'/.");
            SaveCommonSettings();
            VerifyAreEqual(String.Empty, Driver.GetValue(By.Name("MobilePhone")), "`~q!@#$ <= 19 mobile phone admin");
            CheckPhoneMobile(cities, String.Empty, "`~q!@#$ <= 19");

            //некорректная строка с цифрами до 19 симв
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "8512@#$%^&*(-_+\'/.");
            SaveCommonSettings();
            VerifyAreEqual("8512", Driver.GetValue(By.Name("MobilePhone")), "8512#$ <= 19 mobile phone admin");
            CheckPhoneMobile(cities, "8512", "8512#$ <= 19");

            //некорректная строка с цифрами больше 19 симв
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "4512@#$%^&*(-_+\'/.!m");
            SaveCommonSettings();
            VerifyAreEqual("4512", Driver.GetValue(By.Name("MobilePhone")), "4512#$ > 19 mobile phone admin");
            CheckPhoneMobile(cities, "4512", "4512#$ > 19");

            //некорректная строка только с цифрами больше 19 симв
            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("MobilePhone"), "+712345678901234567890");
            SaveCommonSettings();
            VerifyAreEqual(String.Empty, Driver.GetValue(By.Name("MobilePhone")), "+7123... > 19 mobile phone admin");
            CheckPhoneMobile(cities, String.Empty, "+7123... > 19");

            GoToAdmin("settings/common");
            Driver.ScrollTo(By.Name("MobilePhone"));
            Driver.SendKeysInput(By.Name("Phone"), phoneDefault);
            Thread.Sleep(1000);
            VerifyAreEqual(mobilePhoneDefault, Driver.GetValue(By.Name("MobilePhone")), "mobile phone from main phone");
            SaveCommonSettings();
        }

        public void SetCheckoutMailDisable()
        {
            GoToAdmin("settingsmail#?notifyTab=formats");
            Driver.GridFilterSendKeys("Оформление заказа");
            Driver.GetGridCell(0, "Enable").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Driver.WaitForToastSuccess();
        }
    }
}