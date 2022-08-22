using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.CreateTest
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CreateStoreTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            //ReInit();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CreateEmpty()
        {
            GoToAdmin();
            Driver.WaitForElem(By.ClassName("create__site-page-title"));
            VerifyAreEqual(BaseUrl + "adminv3/dashboard/createsite", Driver.Url, "startup page");
            VerifyAreEqual("Интернет-магазин",
                Driver.FindElement(By.CssSelector(".create__site-page .uib-tab.active a")).Text, "active tab");
            
            SetTemplate("Стандартный");

            VerifyAreEqual("Шаблон Стандартный", Driver.FindElement(By.ClassName("page-name-block-text")).Text,
                "template page header");
            VerifyIsNotNull(Driver.FindElement(By.CssSelector(".create__sites-left-block > div:first-child")).Text,
                "template description");
            VerifyAreEqual(0, Driver.FindElements(By.LinkText("Онлайн демо")).Count, "standart template online demo");
            Driver.FindElement(By.CssSelector(".create__sites-left-block .btn-success")).Click();
            Driver.WaitForElem(By.ClassName("funnel-page__name"));

            VerifyAreEqual(BaseUrl + "adminv3/design", Driver.Url, "created store");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".design-first")).Text.IndexOf("Стандартный") != -1,
                "design page: template name");
            //если получится - то проверить скриншот 
            VerifyIsFalse(CheckConsoleLog("https://f003.backblazeb2.com/file/scrshots/"), "site scrshots error");

            GoToClient();
            VerifyIsTrue(Driver.PageSource.ToLower().Contains(BaseUrl + "design/colors/_none/styles/styles.css") &&
                         Driver.PageSource.ToLower().Contains(BaseUrl + "design/themes/_none/styles/styles.css") &&
                         Driver.PageSource.ToLower().Contains(BaseUrl + "design/backgrounds/_none/styles/styles.css"),
                "mainpage: standart styles");
            VerifyIsTrue(
                Driver.PageSource.ToLower().Contains("https://img.advstatic.ru/templates/modern/carousel/slide1.jpg") &&
                Driver.PageSource.ToLower().Contains("https://img.advstatic.ru/templates/modern/carousel/slide2.jpg") &&
                Driver.PageSource.ToLower().Contains("https://img.advstatic.ru/templates/modern/carousel/slide3.jpg"),
                "mainpage: standart carousel banners");
            VerifyIsFalse(Driver.PageSource.ToLower().Contains(BaseUrl + "templates/"),
                "mainpage: other templates styles");

            GoToMobile();
            VerifyIsTrue(Driver.PageSource.ToLower().Contains("<meta name=\"theme-color\" content=\"#0662c1\">"),
                "mainpage mobile: expected template styles");
            VerifyIsTrue(
                Driver.PageSource.ToLower().Contains("https://img.advstatic.ru/templates/modern/carousel/slide1_mobile.jpg") &&
                Driver.PageSource.ToLower().Contains("https://img.advstatic.ru/templates/modern/carousel/slide2_mobile.jpg") &&
                Driver.PageSource.ToLower().Contains("https://img.advstatic.ru/templates/modern/carousel/slide3_mobile.jpg"),
                "mainpage mobile: standart carousel banners");
            VerifyIsFalse(Driver.PageSource.ToLower().Contains(BaseUrl + "templates/"),
                "mainpage mobile: other templates styles");

            ReInit();
            VerifyIsTrue(DeleteSite(), "site delete");
        }

        [Test]
        public void CreateFromTemplate()
        {
            //проверить слайды карусели??
            GoToAdmin();
            Driver.WaitForElem(By.ClassName("create__site-page-title"));
            VerifyAreEqual(BaseUrl + "adminv3/dashboard/createsite", Driver.Url, "startup page");

            VerifyAreEqual("Интернет-магазин",
                Driver.FindElement(By.CssSelector(".create__site-page .uib-tab.active a")).Text, "active tab");
            
            SetTemplate("Fly");

            VerifyAreEqual("Шаблон Fly", Driver.FindElement(By.ClassName("page-name-block-text")).Text,
                "template page header");
            VerifyIsNotNull(Driver.FindElement(By.CssSelector(".create__sites-left-block > div:first-child")).Text,
                "template description");
            VerifyAreEqual(1, Driver.FindElements(By.LinkText("Online demo")).Count, "fly template online demo");
            VerifyAreEqual("https://fly.advant.design/",
                Driver.FindElement(By.LinkText("Online demo")).GetAttribute("href"), "online demo link");
            Driver.FindElement(By.CssSelector(".create__sites-left-block .btn-success")).Click();
            Driver.WaitForElem(By.ClassName("funnel-page__name"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".design-first")).Text.IndexOf("Fly") != -1,
                "design page: template name");
            //если получится - то проверить скриншот 
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
                             .Contains("http://cs71.advantshop.net/templates/Fly/pictures/carousel/slide(1).jpg") &&
                         Driver.PageSource.ToLower()
                             .Contains("http://cs71.advantshop.net/templates/Fly/pictures/carousel/slide(2).jpg") &&
                         Driver.PageSource.ToLower()
                             .Contains("http://cs71.advantshop.net/templates/Fly/pictures/carousel/slide(3).jpg"),
                "mainpage: fly carousel banners");
            VerifyIsFalse(Driver.PageSource.ToLower().Contains(BaseUrl + "design/"),
                "mainpage: not expected template scheme");

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
            VerifyIsTrue(DeleteSite(), "site delete");
        }
    }


    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CreateFunnelTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            ReInit();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CreateEmpty()
        {
            string funnelName = "My funnel";
            string funnelUrl = "my-funnel";
            GoToAdmin();
            Driver.WaitForElem(By.ClassName("create__site-page-title"));
            VerifyAreEqual(BaseUrl + "adminv3/dashboard/createsite", Driver.Url, "startup page");
            Driver.FindElement(By.LinkText("Презентационные воронки")).Click();
            Driver.WaitForElem(By.CssSelector(".last-item-empty-template"));

            VerifyAreEqual("Презентационные воронки",
                Driver.FindElement(By.CssSelector(".create__site-page .uib-tab.active a")).Text, "active tab");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("last-item-empty-template")).Count,
                "empty template block");
            VerifyAreEqual("Пустой шаблон", Driver.FindElement(By.ClassName("last-item-empty-template")).Text,
                "empty template name");

            Driver.FindElement(By.ClassName("last-item-empty-template-funnel")).Click();
            Driver.WaitForModal();
            Functions.SetFunnelName(Driver, funnelName);

            VerifyAreEqual("Воронка: \r\n" + funnelName, Driver.FindElement(By.TagName("h1")).Text, "funnel header");
            VerifyAreEqual("Опубликован", Driver.GetByE2E("funnelEnablerBtn").Text, "funnel enabler");
            VerifyAreEqual(BaseUrl + "lp/" + funnelUrl, Driver.GetByE2E("funnelDomains").Text, "funnel domain");
            VerifyAreEqual(1, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count, "funnel pages count");
            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual(BaseUrl + "lp/" + funnelUrl + "?inplace=true", Driver.Url, "funnel url");

            ReInitClient();
            GoToClient("lp/" + funnelUrl);
            VerifyIsFalse(Is404Page(Driver.Url), "funnel in client desktop");

            GoToMobile("lp/" + funnelUrl);
            VerifyIsFalse(Is404Page(Driver.Url), "funnel in client mobile");

            ReInit();
            VerifyIsTrue(DeleteFunnel(funnelName, funnelUrl), "funnel deleted");
        }

        [Test]
        public void CreateFromTemplate()
        {
            string funnelName = "My Webinar Funnel";
            string funnelUrl = "my-webinar-funnel";
            GoToAdmin();
            Driver.WaitForElem(By.ClassName("create__site-page-title"));
            VerifyAreEqual(BaseUrl + "adminv3/dashboard/createsite", Driver.Url, "startup page");
            Driver.FindElement(By.LinkText("Презентационные воронки")).Click();
            Driver.WaitForElem(By.CssSelector(".last-item-empty-template"));

            VerifyAreEqual("Презентационные воронки",
                Driver.FindElement(By.CssSelector(".create__site-page .uib-tab.active a")).Text, "active tab");
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("last-item")).Count, "funnel templates count");
            {
                IWebElement curFunnelBlock = Driver.FindElements(By.ClassName("last-item"))[1];
                VerifyAreEqual("Воронка \"Вебинар\"", curFunnelBlock.FindElement(By.ClassName("last-item-title")).Text,
                    "webinar template name");
                VerifyAreEqual("http://demo-funnel.on-advantshop.net/lp/webinar",
                    curFunnelBlock.FindElement(AdvBy.DataE2E("funnelDemoLink")).GetAttribute("href"),
                    "webinar demo link");
                VerifyAreEqual(curFunnelBlock.FindElement(By.ClassName("link-img-abs")).GetAttribute("href"),
                    curFunnelBlock.FindElement(AdvBy.DataE2E("funnelCreateLink")).GetAttribute("href"),
                    "webinar create link");
                curFunnelBlock.FindElement(AdvBy.DataE2E("funnelCreateLink")).Click();
                Thread.Sleep(3000);
            }

            VerifyIsTrue(Driver.PageSource.IndexOf("Воронка \"Вебинар\"") != -1, "webinar funnel header");
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("screenshot-item")).Count, "webinar funnel pages count");
            VerifyAreEqual("http://demo-funnel.on-advantshop.net/lp/webinar",
                Driver.FindElement(AdvBy.DataE2E("funnelDemoLink")).GetAttribute("href"), "webinar demo link");
            VerifyIsNull(CheckConsoleLog(), "webinar page console log");
            Driver.GetByE2E("funnelCreateBtn").Click();
            Driver.WaitForModal();
            Functions.SetFunnelName(Driver, funnelName);


            VerifyAreEqual("Воронка: \r\n" + funnelName, Driver.FindElement(By.TagName("h1")).Text, "funnel header");
            VerifyAreEqual("Опубликован", Driver.GetByE2E("funnelEnablerBtn").Text, "funnel enabler");
            VerifyAreEqual(BaseUrl + "lp/" + funnelUrl, Driver.GetByE2E("funnelDomains").Text, "funnel domain");
            VerifyAreEqual(2, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count, "funnel pages count");
            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual(BaseUrl + "lp/" + funnelUrl + "?inplace=true", Driver.Url, "funnel url");

            ReInitClient();
            GoToClient("lp/" + funnelUrl);
            VerifyIsFalse(Is404Page(Driver.Url), "funnel main in client desktop");
            GoToClient("lp/" + funnelUrl + "/spasibo");
            VerifyIsFalse(Is404Page(Driver.Url), "funnel spasibo-page in client desktop");

            GoToMobile("lp/" + funnelUrl);
            VerifyIsFalse(Is404Page(Driver.Url), "funnel main in client mobile");
            GoToMobile("lp/" + funnelUrl + "/spasibo");
            VerifyIsFalse(Is404Page(Driver.Url), "funnel spasibo-page in client mobile");

            ReInit();
            VerifyIsTrue(DeleteFunnel(funnelName, funnelUrl), "funnel deleted");
        }
    }


    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class MainSiteTest : BaseSeleniumTest
    {
        string funnelDomain = "mydomain.ru";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();

            Init(false);

            GoToAdmin("dashboard/createTemplate?id=_default");
            Driver.WaitForElem(By.ClassName("page-name-block-text"));
            Driver.FindElement(By.CssSelector(".create__sites-left-block .btn-success")).Click();
            Driver.WaitForElem(By.ClassName("funnel-page__name"));


            GoToAdmin("/dashboard/createsite#?tabs=CollectingLeads");
            Driver.FindElement(By.ClassName("last-item-empty-template-funnel")).Click();
            Driver.WaitForModal();
            Functions.SetFunnelName(Driver, "EmptyFunnel");

            Driver.WaitForElem(By.ClassName("funnel-page__name"));
            Driver.FindElement(By.XPath("//span[contains(@class, 'lead-events__item__label')]" +
                "[contains(text(), 'Настройки')]")).Click();
            Driver.WaitForElem(By.Id("SiteName"));
            Driver.FindElement(By.LinkText("Домены")).Click();
            Driver.WaitForElem(By.Id("SiteUrl"));

            Driver.SendKeysInput(By.CssSelector(".funnel-domains input"), funnelDomain);
            Driver.FindElement(By.CssSelector(".funnel-domains .btn-success")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));

            GoToAdmin();
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

        [Test, Description("Установить магазин основным сайтом")]
        public void SetMainStore()
        {
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("dashboard-site")).Count, "sites count");
            Driver.FindElement(By.ClassName("select-main-site__block")).Click();
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("dashoboard-dropdown__item")).Count,
                "main site items count");
            VerifyIsTrue(Driver.FindElements(By.ClassName("dashoboard-dropdown__item"))[0].Text.Contains("Интернет-магазин:") &&
                 Driver.FindElements(By.ClassName("dashoboard-dropdown__item"))[0].Text.Contains("Мой магазин"),
                "store name in dropdown");


            Driver.FindElements(By.ClassName("dashoboard-dropdown__item"))[0].Click();
            //Driver.WaitForElem(By.ClassName("dashboard-btn-main__site--active"));
            //VerifyAreEqual("Основной сайт",
            //    Driver.FindElement(By.ClassName("dashboard-btn-main__site--active")).Text.Trim(), "main site btn text");
            VerifyIsTrue(Driver.FindElement(By.ClassName("select-main-site__block")).Text.Contains("Мой магазин"),
                "Main site label text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".dashboard-balance__block .link-alternative"))
                .GetAttribute("href").Equals(BaseUrl), "links to main site after");

            Refresh();
            Driver.WaitForElem(By.ClassName("dashboard-btn-main__site--active"));
            VerifyAreEqual("Основной сайт",
                Driver.FindElement(By.ClassName("dashboard-btn-main__site--active")).Text.Trim(), "main site btn text after refresh");
            VerifyIsTrue(Driver.FindElement(By.ClassName("select-main-site__block")).Text.Contains("Мой магазин"),
                "Main site label text after refresh");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".dashboard-balance__block .link-alternative"))
                .GetAttribute("href").Equals(BaseUrl), "links to main site after refresh");
        }

        [Test, Description("Установить воронку основным сайтом, должен быть привязан домен")]
        public void SetMainFunnel()
        {
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("dashboard-site")).Count, "sites count");
            Driver.FindElement(By.ClassName("select-main-site__block")).Click();
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("dashoboard-dropdown__item")).Count,
                "main site items count");
            VerifyIsTrue(Driver.FindElements(By.ClassName("dashoboard-dropdown__item"))[1].Text.Contains("Воронка продаж:") &&
                 Driver.FindElements(By.ClassName("dashoboard-dropdown__item"))[1].Text.Contains("EmptyFunnel"),
                "store name in dropdown");

            Driver.FindElements(By.ClassName("dashoboard-dropdown__item"))[1].Click();
            //Driver.WaitForElem(By.ClassName("dashboard-btn-main__site--active"));
            //VerifyAreEqual("Основной сайт",
            //    Driver.FindElement(By.ClassName("dashboard-btn-main__site--active")).Text.Trim(), "main site btn text");
            VerifyIsTrue(Driver.FindElement(By.ClassName("select-main-site__block")).Text.Contains("EmptyFunnel"),
                "Main site label text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".dashboard-balance__block .link-alternative"))
                .GetAttribute("href").Equals("http://" + funnelDomain + "/"), "links to main site after");

            Refresh();
            Driver.WaitForElem(By.ClassName("dashboard-btn-main__site--active"));
            VerifyAreEqual("Основной сайт",
                Driver.FindElement(By.ClassName("dashboard-btn-main__site--active")).Text.Trim(), "main site btn text after refresh");
            VerifyIsTrue(Driver.FindElement(By.ClassName("select-main-site__block")).Text.Contains("EmptyFunnel"),
                "Main site label text afret refresh");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".dashboard-balance__block .link-alternative"))
                .GetAttribute("href").Equals("http://" + funnelDomain + "/"), "links to main site after refresh");
        }

        [Test, Description("Чек, что нельзя установить воронку без домена")]
        public void SetMainFunnelWithoutDomain()
        {
            string funnelName = "EmptyFunnelWithoutDomain";

            GoToAdmin("/dashboard/createsite#?tabs=CollectingLeads");
            Driver.FindElement(By.ClassName("last-item-empty-template-funnel")).Click();
            Driver.WaitForModal();
            Functions.SetFunnelName(Driver, funnelName);

            GoToAdmin();
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("dashboard-site")).Count, "sites count");
            Driver.FindElement(By.ClassName("select-main-site__block")).Click();
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("dashoboard-dropdown__item")).Count,
                "main site items count");

            Driver.FindElements(By.ClassName("link-alternative--delete"))[2].Click();
            Driver.SwalConfirm();
        }

        [Test, Description("Сменить сайт на добавленный, удалить основной сайт")]
        public void ChangeMainStore()
        {
            string funnelName = "SecondEmptyFunnel";
            string funnelDomain = "second.emptyfunnels.ru";
            GoToAdmin("/dashboard/createsite#?tabs=CollectingLeads");
            Driver.FindElement(By.ClassName("last-item-empty-template-funnel")).Click();
            Driver.WaitForModal();
            Functions.SetFunnelName(Driver, funnelName);

            Driver.WaitForElem(By.ClassName("funnel-page__name"));
            Driver.FindElement(By.XPath("//span[contains(@class, 'lead-events__item__label')]" +
                "[contains(text(), 'Настройки')]")).Click();
            Driver.WaitForElem(By.Id("SiteName"));
            Driver.FindElement(By.LinkText("Домены")).Click();
            Driver.WaitForElem(By.Id("SiteUrl"));

            Driver.SendKeysInput(By.CssSelector(".funnel-domains input"), funnelDomain);
            Driver.FindElement(By.CssSelector(".funnel-domains .btn-success")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));

            GoToAdmin();
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("dashboard-site")).Count, "sites count");
            Driver.FindElement(By.ClassName("select-main-site__block")).Click();
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("dashoboard-dropdown__item")).Count,
                "main site items count");
            VerifyIsTrue(Driver.FindElements(By.ClassName("dashoboard-dropdown__item"))[2].Text.Contains(funnelName),
                "added funnel name in dropdown");

            Driver.FindElements(By.ClassName("dashoboard-dropdown__item"))[2].Click();
            //Driver.WaitForElem(By.ClassName("dashboard-btn-main__site--active"));
            //VerifyIsTrue(Driver.FindElements(By.ClassName("dashboard-btn-main__site--active")).Count == 1, "main-labels count");
            //VerifyAreEqual("Основной сайт",
            //    Driver.FindElement(By.ClassName("dashboard-btn-main__site--active")).Text.Trim(), "main site btn text");
            VerifyIsTrue(Driver.FindElement(By.ClassName("select-main-site__block")).Text.Contains(funnelName),
                "Main site label text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".dashboard-balance__block .link-alternative"))
                .GetAttribute("href").Equals("http://" + funnelDomain + "/"), "links to main site after");

            Refresh();
            Driver.WaitForElem(By.ClassName("dashboard-btn-main__site--active"));
            VerifyIsTrue(Driver.FindElements(By.ClassName("dashboard-btn-main__site--active")).Count == 1, "main-labels count");
            VerifyAreEqual("Основной сайт",
                Driver.FindElement(By.ClassName("dashboard-btn-main__site--active")).Text.Trim(), "main site btn text after refresh");
            VerifyIsTrue(Driver.FindElement(By.ClassName("select-main-site__block")).Text.Contains(funnelName),
                "Main site label text afret refresh");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".dashboard-balance__block .link-alternative"))
                .GetAttribute("href").Equals("http://" + funnelDomain + "/"), "links to main site after refresh");

            Driver.FindElements(By.ClassName("link-alternative--delete"))[2].Click();
            Driver.SwalConfirm();

            VerifyIsTrue(Driver.FindElements(By.ClassName("dashboard-btn-main__site--active")).Count == 0, "after removed main site");
        }
    }


    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class ProjectTest : BaseSeleniumTest
    {
        private string defaultProjectName = "Мой проект";

        [OneTimeSetUp]
        public void OneTimeSetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();

            Init(false);
            GoToAdmin("dashboard");
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            Driver.WaitForElem(By.ClassName("title-page__dashboard"));
        }

        [TearDown]
        public void TearDownTest()
        {
            Driver.WaitForElem(By.ClassName("title-page__dashboard"));
            SetProjectName(defaultProjectName);
            VerifyFinally(TestName);
        }

        [Test]
        public void RenameProjectEmpty()
        {
            SetProjectName("");
            Driver.WaitForElem(By.ClassName("toast-error"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".toast-error [validation-output]")).Text
                    .Contains("Не заполнены поля"), "empty field message");
            Driver.FindElement(By.ClassName("close")).Click();
            Thread.Sleep(100);
        }

        [Test]
        public void RenameProjectCancel()
        {
            Driver.FindElement(By.ClassName("top-menu-shopname-edit")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(AdvBy.DataE2E("inputAdminShopName"), "New project name");
            Driver.FindElement(By.ClassName("btn-cancel")).Click();
            Thread.Sleep(100);
            VerifyAreEqual(defaultProjectName, Driver.FindElement(By.ClassName("title-page__dashboard")).Text.Trim(),
                "new name canceled");
        }

        [Test]
        public void RenameProjectClose()
        {
            Driver.FindElement(By.ClassName("top-menu-shopname-edit")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(AdvBy.DataE2E("inputAdminShopName"), "New project name");
            Driver.FindElement(By.ClassName("close")).Click();
            Thread.Sleep(100);
            VerifyAreEqual(defaultProjectName, Driver.FindElement(By.ClassName("title-page__dashboard")).Text.Trim(),
                "new name closed");
        }

        [Test]
        public void RenameProjectConfirm()
        {
            SetProjectName("New project name");
            Driver.WaitForElem(By.ClassName("toast-success"));
            VerifyAreEqual("New project name", Driver.FindElement(By.ClassName("title-page__dashboard")).Text.Trim(),
                "new name confirmed");
        }

        [Test]
        public void RenameProjectLongNameNOTCOMPLETE()
        {
            VerifyAddErrors("Test not worked");
        }

        [Test]
        public void RenameProjectLongNameMoreNOTCOMPLETE()
        {
            VerifyAddErrors("Test not worked");
        }

        [Test]
        public void RenameProjectHtmlNOTCOMPLETE()
        {
            VerifyAddErrors("Test not worked");
        }

        [Test]
        public void RenameProjectSymbolsNOTCOMPLETE()
        {
            VerifyAddErrors("Test not worked");
        }


        public void SetProjectName(string projectName)
        {
            Driver.FindElement(By.ClassName("top-menu-shopname-edit")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(AdvBy.DataE2E("inputAdminShopName"), projectName);
            Driver.FindElement(By.ClassName("btn-save")).Click();
        }
    }
}