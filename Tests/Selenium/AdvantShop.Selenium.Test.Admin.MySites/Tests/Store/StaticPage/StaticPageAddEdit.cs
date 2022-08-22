using System.Net;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.StaticPage
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class StaticPageStoreTest : MySitesFunctions
    {
        private string spPageUrl = "staticpages#?staticPageTab=pages";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Store\\StaticPage\\CMS.StaticPage.csv"
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

        //! все в мобилке и десктопе
        //go to changed url - in staticPageTest-> validation

        //Родительская категория - проверить, как изменение родительской категории для страницы влияет на отображение в клиентке
        //Page11, 16 in root, Page12-13-14 in 11, Page15 in 14  
        [Test]
        public void RenderPageParent()
        {
            //в клиентке в корне
            //в клиентке будучи вложенным в страницу
            //в клиентке будучи вложенным в 2 страницы
            //проверить отображение иерархии в хлебных крошках, в меню справа

            GoToAdmin("staticpages/edit/11");
            Driver.WaitForElem(By.ClassName("category-content"));
            VerifyAreEqual("Корень", Driver.FindElement(AdvBy.DataE2E("StaticPageParent")).Text, "page11 parent root");
            GoToAdmin("staticpages/edit/12");
            Driver.WaitForElem(By.ClassName("category-content"));
            VerifyAreEqual("Page11", Driver.FindElement(AdvBy.DataE2E("StaticPageParent")).Text,
                "page12 parent page11");
            GoToAdmin("staticpages/edit/14");
            Driver.WaitForElem(By.ClassName("category-content"));
            VerifyAreEqual("Page11", Driver.FindElement(AdvBy.DataE2E("StaticPageParent")).Text,
                "page14 parent page11");
            GoToAdmin("staticpages/edit/15");
            Driver.WaitForElem(By.ClassName("category-content"));
            VerifyAreEqual("Page14", Driver.FindElement(AdvBy.DataE2E("StaticPageParent")).Text,
                "page15 parent page14");

            GoToClient("pages/page16");
            string breads = GetBreadsText();
            VerifyAreEqual("ГлавнаяPage16", breads, "main parent for p16");
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("news-categories")).Count, "categories count at p16");

            GoToClient("pages/page11");
            breads = GetBreadsText();
            VerifyAreEqual("ГлавнаяPage11", breads, "main parent for p11");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("news-item-r-title")).Count, "title at p11");
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("news-categories")).Count,
                "categories count at p11 - 3 child and this");

            Driver.FindElement(By.LinkText("Page12")).Click();
            Thread.Sleep(500);
            breads = GetBreadsText();
            VerifyAreEqual("ГлавнаяPage11Page12", breads, "parent p11 for p12");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("news-item-r-title")).Count, "title at p12");
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("news-categories")).Count,
                "categories count at p11 - p12 - 0 child, 1 parent, 2 neighbor and this");

            Driver.FindElement(By.LinkText("Page14")).Click();
            Thread.Sleep(500);
            breads = GetBreadsText();
            VerifyAreEqual("ГлавнаяPage11Page14", breads, "parent p11 for p14");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("news-item-r-title")).Count, "title at p13");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("news-categories")).Count,
                "categories count at p14 - 1 child and this");

            GoToClient("pages/page15");
            breads = GetBreadsText();
            VerifyAreEqual("ГлавнаяPage11Page14Page15", breads, "parents for p15");
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("news-item-r-title")).Count, "title at p13");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("news-categories")).Count,
                "categories count at p15 - 1st parent, 2th parent, this");
        }

        //отображение стат.страницы в меню.: active root (19), not active root
        [Test]
        public void RenderPageInMenu()
        {
            GoToAdmin("menus");
            AddMenuItem("btnAddMainMenuItem", "root item", "StatPage", "pages/page19");

            GoToClient();
            Driver.FindElements(By.ClassName("menu-general-root-link"))[0].Click();
            Driver.WaitForElem(By.TagName("h1"));
            VerifyAreEqual("Page19", Driver.FindElement(By.TagName("h1")).Text.Trim(), "h1 page at first menu item");
            VerifyAreEqual("ГлавнаяPage19", GetBreadsText(), "page without parent");

            GoToAdmin("staticpages/edit/19");
            Driver.WaitForElem(By.ClassName("category-content"));
            Driver.CheckBoxUncheck("Enabled");
            SaveStaticPageSettings();

            GoToAdmin("menus"); //да, дублирую пункт меню. Хотела убедиться, что пункт с неактивной страницей добавится. По факту ничего не проверяется.
            AddMenuItem("btnAddMainMenuItem", "root item not enabled", "StatPage", "pages/page19");

            GoToClient();
            Driver.FindElements(By.ClassName("menu-general-root-link"))[0].Click();
            Thread.Sleep(500);
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.NotFound, "disable page");
            Driver.FindElements(By.ClassName("menu-general-root-link"))[1].Click();
            Thread.Sleep(500);
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.NotFound, "disable page duplicate");
        }

        //Изменение содержимого стат.страницы 
        [Test]
        public void RenderPageContent()
        {
            int spId = 47;
            string spUrl = "pages/page" + spId;
            By spSelector = By.ClassName("staticpage-content");

            GoToAdmin("staticpages/edit/" + spId);
            Driver.WaitForElem(By.ClassName("category-content"));

            string sbHtml = "Short text string";
            string sbText = sbHtml;
            SetStaticPageHtml(spId, sbHtml);
            CheckPageAllMode(spId, sbText, sbHtml, 1);

            sbHtml =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit " +
                "in voluptate velit esse cillum dolore eu fugiat nulla pariatur.";
            sbHtml = sbHtml + sbHtml + sbHtml;
            sbText = sbHtml;
            SetStaticPageHtml(spId, sbHtml);
            CheckPageAllMode(spId, sbText, sbHtml, 2);

            sbHtml =
                "<div><b>Lorem ipsum &amp; &quot;dolor&quot; sit amet,</b> consectetur adipiscing elit, <br><br><span>sed do eiusmod tempor incididunt ut <div>labore et <u>dolore magna</u> aliqua</div>.</span></div>";
            sbText =
                "Lorem ipsum & \"dolor\" sit amet, consectetur adipiscing elit,\r\n\r\nsed do eiusmod tempor incididunt ut\r\nlabore et dolore magna aliqua\r\n.";
            SetStaticPageHtml(spId, sbHtml);
            CheckPageAllMode(spId, sbText, sbHtml.Replace("&quot;", "\""), 3);

            sbHtml =
                "<div><b style=\"font-size: 28px; color: blue;\">Lorem ipsum &amp; dolor sit amet,</b> consectetur adipiscing elit, <br><br><span class=\"flex between-xs\">sed do eiusmod tempor incididunt ut <div style=\"display: grid\">labore et <u>dolore magna</u> aliqua</div>.</span></div>";
            sbText =
                "Lorem ipsum & dolor sit amet, consectetur adipiscing elit,\r\n\r\nsed do eiusmod tempor incididunt ut\r\nlabore et\r\ndolore magna\r\naliqua\r\n.";
            SetStaticPageHtml(spId, sbHtml);
            CheckPageAllMode(spId, sbText, sbHtml, 4);

            sbHtml =
                "<style>.my-sb-styles{background-color: pink; font-size: 20px;}.my-sb-styles b{text-decoration: underline;}.sb-display-grid{display: grid;}</style><div class=\"my-sb-styles\"><b>Lorem ipsum &amp; dolor sit amet,</b> consectetur adipiscing elit, <br><br><span>sed do eiusmod tempor incididunt ut <div class=\"sb-display-grid\">labore et <u>dolore magna</u> aliqua</div>.</span></div>";
            sbText =
                "Lorem ipsum & dolor sit amet, consectetur adipiscing elit,\r\n\r\nsed do eiusmod tempor incididunt ut\r\nlabore et\r\ndolore magna\r\naliqua\r\n.";
            SetStaticPageHtml(spId, sbHtml);
            CheckPageAllMode(spId, sbText, sbHtml, 5);

            sbHtml = "<img alt=\"owl\" src=\"http://bipbap.ru/wp-content/uploads/2017/04/72fqw2qq3kxh.jpg\">";
            sbText = "";
            SetStaticPageHtml(spId, sbHtml);
            //CheckPageAllMode(spId, sbText, sbHtml.Replace(">", "") + " class=\"js-qazy-loaded\" data-loaded=\"true\">", 6);

            GoToClient(spUrl);
            VerifyAreEqual(sbText, GetPageText(spSelector), $"staticPage text at desktop(6)");
            VerifyAreEqual(sbHtml, GetPageInnerHtml(spSelector), $"staticPage html at desktop(6)");
            GoToMobile(spUrl);
            VerifyAreEqual(sbText, GetPageText(spSelector), $"staticPage text at mobile(6)");
            VerifyAreEqual(sbHtml, GetPageInnerHtml(spSelector), $"staticPage html at mobile(6)");
            ReInitClient();
            GoToClient(spUrl);
            VerifyAreEqual(sbHtml.Replace(">", "") +
                           " class=\"js-qazy-loaded\" data-qazy-image=\"http://bipbap.ru/wp-content/uploads/2017/04/72fqw2qq3kxh.jpg\" " +
                           "data-loaded=\"true\">", GetPageInnerHtml(spSelector), $"staticPage at client(6)");
            GoToMobile(spUrl);
            VerifyAreEqual(sbHtml, GetPageInnerHtml(spSelector), $"staticPage at client mobile(6)");
            ReInit();

            sbHtml = "<a href=\"https://www.advantshop.net/\" target=\"_blank\">Create store with AdvantShop!</a>";
            sbText = "Create store with AdvantShop!";
            SetStaticPageHtml(spId, sbHtml);
            CheckPageAllMode(spId, sbText, sbHtml, 7);

            EnableInplaceOff();
            GoToClient(spUrl);
            Driver.FindElement(By.LinkText("Create store with AdvantShop!")).Click();
            Thread.Sleep(500);
            Driver.SwitchTo().Window(Driver.WindowHandles[1]);
            Thread.Sleep(500);
            VerifyAreEqual("https://www.advantshop.net/", Driver.Url, "link url");
            Thread.Sleep(500);
            Driver.SwitchTo().Window(Driver.WindowHandles[0]);
            EnableInplaceOn();

            sbHtml =
                "<div class=\"my-sb-div-for-script\">My not edited text.</div><script>document.querySelector(\".my-sb-div-for-script\").innerText = \"My new text in my-sb-div-for-script block\";</script>";
            sbText = "My new text in my-sb-div-for-script block";
            SetStaticPageHtml(spId, sbHtml);

            GoToClient(spUrl);
            VerifyAreEqual("My not edited text.", GetPageText(spSelector), $"staticPage text at desktop({8})");
            VerifyAreEqual(sbHtml, GetPageInnerHtml(spSelector), $"staticPage html at desktop({8})");
            GoToMobile(spUrl);
            VerifyAreEqual("My not edited text.", GetPageText(spSelector), $"staticPage text at mobile({8})");
            VerifyAreEqual(sbHtml.Replace("<script>", "<script type=\"inplace\">"), GetPageInnerHtml(spSelector),
                $"staticPage html at mobile({8})");
            ReInitClient();
            GoToClient(spUrl);
            VerifyAreEqual(sbHtml.Replace("My not edited text.", sbText),
                GetPageInnerHtml(spSelector), $"staticPage at client desktop({8})");
            GoToMobile(spUrl);
            VerifyAreEqual(sbHtml.Replace("My not edited text.", sbText),
                GetPageInnerHtml(spSelector), $"staticPage at client mobile desktop({8})");

            ReInit();

            EnableInplaceOff();

            GoToClient(spUrl);
            VerifyAreEqual(sbText, GetPageText(By.CssSelector(".staticpage-content .my-sb-div-for-script")),
                $"staticPage text at desktop(123)");
            GoToMobile(spUrl);
            VerifyAreEqual(sbText, GetPageText(By.CssSelector(".staticpage-content .my-sb-div-for-script")),
                $"staticPage text at mobile(123)");
            ReInitClient();
            GoToClient(spUrl);
            VerifyAreEqual(sbText, GetPageText(By.CssSelector(".staticpage-content .my-sb-div-for-script")),
                $"staticPage text at client desktop(123)");
            GoToMobile(spUrl);
            VerifyAreEqual(sbText, GetPageText(By.CssSelector(".staticpage-content .my-sb-div-for-script")),
                $"staticPage text at client mobile(123)");

            ReInit();

            EnableInplaceOn();
        }

        //активность в клиентке
        [Test]
        public void RenderActivePage()
        {
            GoToAdmin("staticpages/edit/51");
            Driver.WaitForElem(By.ClassName("category-content"));
            VerifyIsTrue(Driver.FindElement(By.Id("Enabled")).Selected, "default enabled");

            //включить блок, 
            //перейти на главную, найти блок.
            //перейти в мобилку, найти блок.
            GoToClient("pages/page51");
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "enable page");
            VerifyIsNull(CheckConsoleLog(), "enable page log");
            GoToMobile("pages/page51");
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "mobile enable page");
            VerifyIsNull(CheckConsoleLog(), "mobile enable page log");
            ReInit();

            //выключить блок, 
            //перейти на главную, не найти блок. 
            //перейти в мобилку, не найти блок. 
            GoToAdmin("staticpages/edit/51");
            Driver.WaitForElem(By.ClassName("category-content"));
            Driver.CheckBoxUncheck("Enabled");
            SaveStaticPageSettings();

            GoToClient("pages/page51");
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.NotFound, "disable page");
            GoToMobile("pages/page51");
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.NotFound, "mobile disable page");
            ReInit();

            GoToAdmin("staticpages/edit/51");
            Driver.WaitForElem(By.ClassName("category-content"));
            Driver.CheckBoxCheck("Enabled");
            SaveStaticPageSettings();
        }

        //мета в клиентке
        [Test]
        public void RenderMeta()
        {
            GoToAdmin(spPageUrl);

            //https://task.advant.me/adminv3/tasks#?modal=23419

            //проверить отображение дефолтных мета, 
            GoToAdmin("staticpages/edit/63");
            Driver.WaitForElem(By.ClassName("category-content"));
            VerifyIsTrue(Driver.FindElement(By.Id("DefaultMeta")).Selected, "default meta");

            GoToClient("pages/page63");
            string defaultMeta = "Мой магазин - Page63";
            VerifyAreEqual(defaultMeta, Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"),
                "default title");
            VerifyAreEqual("Page63", Driver.FindElement(By.TagName("h1")).Text.Trim(), "default h1");
            VerifyAreEqual(defaultMeta,
                Driver.FindElement(By.CssSelector("meta[name=\"Description\"]")).GetAttribute("content"),
                "default description");
            VerifyAreEqual(defaultMeta,
                Driver.FindElement(By.CssSelector("meta[name=\"Keywords\"]")).GetAttribute("content"),
                "default keywords");
            GoToMobile("pages/page63");
            VerifyAreEqual(defaultMeta, Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"),
                "mobile default title");
            VerifyAreEqual("Page63", Driver.FindElement(By.TagName("h1")).Text.Trim(), "mobile default h1");
            VerifyAreEqual(defaultMeta,
                Driver.FindElement(By.CssSelector("meta[name=\"Description\"]")).GetAttribute("content"),
                "mobile default description");
            VerifyAreEqual(defaultMeta,
                Driver.FindElement(By.CssSelector("meta[name=\"Keywords\"]")).GetAttribute("content"),
                "mobile default keywords");
            ReInit();

            //задать свои мета текстом - некоторые

            GoToAdmin("staticpages/edit/63");
            Driver.CheckBoxUncheck("DefaultMeta");
            string customMeta = "Custom meta for static page";
            Driver.SendKeysInput(By.Name("SeoTitle"), customMeta);
            Driver.SendKeysInput(By.Name("SeoH1"), customMeta);
            SaveStaticPageSettings();

            GoToClient("pages/page63");
            VerifyAreEqual(customMeta, Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"),
                "custom title");
            VerifyAreEqual(customMeta, Driver.FindElement(By.TagName("h1")).Text.Trim(), "custom h1");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector("meta[name=\"Description\"]")).Count,
                "empty description");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector("meta[name=\"Keywords\"]")).Count, "empty keywords");
            GoToMobile("pages/page63");
            VerifyAreEqual(customMeta, Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"),
                "mobile custom title");
            VerifyAreEqual(customMeta, Driver.FindElement(By.TagName("h1")).Text.Trim(), "mobile custom h1");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector("meta[name=\"Description\"]")).Count,
                "mobile empty description");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector("meta[name=\"Keywords\"]")).Count,
                "mobile empty keywords");
            ReInit();

            //задать свои мета текстом - все
            GoToAdmin("staticpages/edit/63");
            Driver.SendKeysInput(By.Name("SeoDescription"), customMeta);
            Driver.SendKeysInput(By.Name("SeoKeywords"), customMeta);
            SaveStaticPageSettings();

            GoToClient("pages/page63");
            VerifyAreEqual(customMeta, Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"),
                "custom title");
            VerifyAreEqual(customMeta, Driver.FindElement(By.TagName("h1")).Text.Trim(), "custom h1");
            VerifyAreEqual(customMeta,
                Driver.FindElement(By.CssSelector("meta[name=\"Description\"]")).GetAttribute("content"),
                "custom description");
            VerifyAreEqual(customMeta,
                Driver.FindElement(By.CssSelector("meta[name=\"Keywords\"]")).GetAttribute("content"),
                "custom keywords");
            GoToMobile("pages/page63");
            VerifyAreEqual(customMeta, Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"),
                "mobile custom title");
            VerifyAreEqual(customMeta, Driver.FindElement(By.TagName("h1")).Text.Trim(), "mobile custom h1");
            VerifyAreEqual(customMeta,
                Driver.FindElement(By.CssSelector("meta[name=\"Description\"]")).GetAttribute("content"),
                "mobile custom description");
            VerifyAreEqual(customMeta,
                Driver.FindElement(By.CssSelector("meta[name=\"Keywords\"]")).GetAttribute("content"),
                "mobile custom keywords");
            ReInit();

            //задать свои мета с переменными
            GoToAdmin("staticpages/edit/63");
            Driver.SendKeysInput(By.Name("SeoTitle"), "#STORE_NAME# - Название магазина");
            Driver.SendKeysInput(By.Name("SeoH1"), "#PAGE_NAME# - Название статической страницы");
            Driver.SendKeysInput(By.Name("SeoDescription"), "#STORE_NAME##PAGE_NAME#");
            Driver.SendKeysInput(By.Name("SeoKeywords"), "#STORE_NAME#,,, #PAGE_NAME#");
            SaveStaticPageSettings();

            GoToClient("pages/page63");
            VerifyAreEqual("Мой магазин - Название магазина",
                Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"), "custom with variables title");
            VerifyAreEqual("Page63 - Название статической страницы", Driver.FindElement(By.TagName("h1")).Text.Trim(),
                "custom with variables h1");
            VerifyAreEqual("Мой магазинPage63",
                Driver.FindElement(By.CssSelector("meta[name=\"Description\"]")).GetAttribute("content"),
                "custom with variables description");
            VerifyAreEqual("Мой магазин,,, Page63",
                Driver.FindElement(By.CssSelector("meta[name=\"Keywords\"]")).GetAttribute("content"),
                "custom with variables keywords");
            GoToMobile("pages/page63");
            VerifyAreEqual("Мой магазин - Название магазина",
                Driver.FindElement(By.TagName("title")).GetAttribute("innerHTML"),
                "mobile custom with variables title");
            VerifyAreEqual("Page63 - Название статической страницы", Driver.FindElement(By.TagName("h1")).Text.Trim(),
                "mobile custom with variables h1");
            VerifyAreEqual("Мой магазинPage63",
                Driver.FindElement(By.CssSelector("meta[name=\"Description\"]")).GetAttribute("content"),
                "mobile custom with variables description");
            VerifyAreEqual("Мой магазин,,, Page63",
                Driver.FindElement(By.CssSelector("meta[name=\"Keywords\"]")).GetAttribute("content"),
                "mobile custom with variables keywords");
            ReInit();
        }

        protected void SetStaticPageHtml(int sPageId, string htmlString)
        {
            GoToAdmin("staticpages/edit/" + sPageId);
            Driver.SetCkHtml(htmlString);
            Thread.Sleep(500);
            SaveStaticPageSettings();
        }

        protected string GetPageText(By cssSelector)
        {
            return Driver.FindElement(cssSelector).Text;
        }

        protected string GetPageInnerHtml(By cssSelector)
        {
            return Driver.FindElement(cssSelector).GetAttribute("innerHTML").Trim();
        }

        protected void CheckPageAllMode(int spId, string text, string html, int checkId)
        {
            By spUrl = By.ClassName("staticpage-content");
            string url = "pages/page" + spId;

            GoToClient(url);
            VerifyAreEqual(text, GetPageText(spUrl), $"staticPage text at desktop({checkId})");
            VerifyAreEqual(html, GetPageInnerHtml(spUrl), $"staticPage html at desktop({checkId})");
            GoToMobile(url);
            VerifyAreEqual(text, GetPageText(spUrl), $"staticPage text at mobile({checkId})");
            VerifyAreEqual(html, GetPageInnerHtml(spUrl), $"staticPage html at mobile({checkId})");
            ReInitClient();
            GoToClient(url);
            VerifyAreEqual(html, GetPageInnerHtml(spUrl), $"staticPage at client desktop({checkId})");
            GoToMobile(url);
            VerifyAreEqual(html, GetPageInnerHtml(spUrl), $"staticPage at client mobile({checkId})");

            ReInit();
        }

        protected string GetBreadsText()
        {
            return System.Text.RegularExpressions.Regex.Replace(GetPageText(By.ClassName("breads")), "[^\\w\\d]", "");
        }
    }
}