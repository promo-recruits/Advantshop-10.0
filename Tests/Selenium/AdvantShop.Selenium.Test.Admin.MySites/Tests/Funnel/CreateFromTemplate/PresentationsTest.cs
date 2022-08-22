using System.Net;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.FromTemplate
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class PresentationsCreateTest : MySitesFunctions
    {
        string landingId;
        string landingName;
        string landingTab = "Презентационные воронки";
        string defaultDomain = "http://mydomain123.ru/";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();

            Init(false);

            GoToAdmin("leads?salesFunnelId=1");
            Driver.FindElement(AdvBy.DataE2E("UseGrid")).Click();
            Thread.Sleep(1000);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin("dashboard/createsite");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        #region Presentations

        [Test]
        public void EventActionCreate()
        {
            landingId = "EventAction";
            landingName = "Воронка \"Мероприятие\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "thanks page");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons count");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--secondary")).Count,
                    "secondary buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void WebinarCreate()
        {
            landingId = "Webinar";
            landingName = "Воронка \"Вебинар\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "content page");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons count");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--secondary")).Count,
                    "secondary buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel content page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel content page console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons content page count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        #endregion
    }

    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class PresentationsCheckFunctionalityTest : MySitesFunctions
    {
        string landingId;

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\FromTemplate\\Presentations\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Presentations\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Presentations\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Presentations\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Presentations\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Presentations\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\FromTemplate\\Presentations\\CMS.LandingSubBlock.csv"
                );

            Init(false);

            GoToAdmin("leads?salesFunnelId=1");
            Driver.FindElement(AdvBy.DataE2E("UseGrid")).Click();
            Thread.Sleep(1000);
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

        #region Presentations

        [Test]
        public void EventActionCheck()
        {
            landingId = "EventAction";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //client - scroll buttons, fill first form
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                //кнопка программы - скролл
                //кнопка регистрация - скролл
                //стрелка вниз - скролл
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")), "btn1");
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--secondary")), "btn secondary");
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".scroll-to-block-trigger")), "btn scroll");

                //три ссылки в шапке - скролл
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-menu-header__link"))[0], "menu link1");
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-menu-header__link"))[1], "menu link2");
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-menu-header__link"))[2], "menu link3");

                //включить видео
                Driver.ScrollTo(By.ClassName("iframe-responsive__container"));
                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("iframe-responsive__container")).Click();
                Thread.Sleep(3000);
                VerifyIsNull(CheckConsoleLog(true), "console client after video play");

                //клик по картинке - чек быстрого просмотра
                Driver.FindElement(By.CssSelector(".lp-block-gallery img")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("close-button")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "console client after image click");

                //клик по скроллу карусели отзывов
                Driver.ScrollTo(By.ClassName("lp-block-reviews"));
                Driver.FindElement(By.CssSelector(".lp-block-reviews .slick-next")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "console client after review click");

                //первая форма
                FillFunnelForm();
                Driver.WaitForElem(waitElem);

                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?lid=") != -1, "funnel thanks page client");
                VerifyAreEqual("СПАСИБО!",
                    Driver.FindElement(By.CssSelector(".lp-h2")).Text.Trim(), "thanks page header");

                ReInit();
                CheckLeadInAdmin(landingId, 1);
            }

            //mobile - scroll buttons, menu, fill second form
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                //скроллы по 3 кнопкам, 
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")), "btn1 mobile");
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--secondary")),
                    "btn secondary mobile");
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".scroll-to-block-trigger")),
                    "btn scroll mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                ////шапка - три ссылки
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-menu-header-container--open")).Count,
                    "shown burger menu");
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                CheckBurgerMenuLink(0);
                CheckBurgerMenuLink(1);
                CheckBurgerMenuLink(2);
                VerifyIsNull(CheckConsoleLog(true), "funnel after burgerMenu mobile(4)");


                //включить видео
                Driver.ScrollTo(By.ClassName("iframe-responsive__container"));
                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("iframe-responsive__container")).Click();
                Thread.Sleep(3000);
                VerifyIsNull(CheckConsoleLog(true), "console client after video play");

                //клик по картинке - чек быстрого просмотра
                Driver.FindElement(By.CssSelector(".lp-block-gallery img")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("close-button")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "console client after image click");

                //клик по скроллу карусели отзывов
                Driver.ScrollTo(By.ClassName("lp-block-reviews"));
                Driver.FindElements(By.CssSelector(".slick-dots li"))[2].Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "console client after review click");

                //вторая форма
                Driver.ScrollTo(By.ClassName("lp-form"), 1);
                Driver.FindElements(By.ClassName("lp-form"))[1].FindElement(By.CssSelector("input[type=\"text\"]"))
                    .SendKeys("FirstName");
                Driver.FindElements(By.ClassName("lp-form"))[1].FindElement(By.CssSelector("input[type=\"email\"]"))
                    .SendKeys("test@email.ru");
                Driver.FindElements(By.ClassName("lp-form"))[1].FindElement(By.CssSelector("input[type=\"tel\"]"))
                    .SendKeys("79012345678");
                Driver.FindElements(By.ClassName("lp-form"))[1].FindElement(By.CssSelector(".lp-form__agreement label"))
                    .Click();
                Driver.FindElements(By.ClassName("lp-form"))[1].FindElement(By.CssSelector(".lp-btn")).Click();
                Thread.Sleep(1000);
                Driver.WaitForElem(waitElem);

                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?lid=") != -1, "funnel thanks page client");
                VerifyAreEqual("СПАСИБО!",
                    Driver.FindElement(By.CssSelector(".lp-h2")).Text.Trim(), "thanks page header");

                ReInit();
                CheckLeadInAdmin(landingId, 2);
            }
        }

        [Test]
        public void WebinarCheck()
        {
            landingId = "Webinar";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //client - scroll buttons, fill form
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                //кнопка принять участие - скролл
                //Кнопка "конечно хочу" - скролл
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")), "btn1");
                CheckScrollingFromMiddle(By.CssSelector(".lp-btn.lp-btn--secondary"), "btn secondary");

                //4 ссылки в шапке - скролл
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-menu-header__link"))[0], "menu link1");
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-menu-header__link"))[1], "menu link2");
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-menu-header__link"))[2], "menu link3");
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-menu-header__link"))[3], "menu link4");

                //одна форма
                Driver.ScrollTo(By.ClassName("lp-block-form-with-picture"));
                FillFunnelForm();
                Driver.WaitForElem(waitElem);

                VerifyIsTrue(Driver.Url.IndexOf("/spasibo?lid=") != -1, "funnel thanks page client");
                VerifyAreEqual("Спасибо, ждем вас на вебинаре",
                    Driver.FindElement(By.CssSelector(".lp-h2")).Text.Trim(), "thanks page header");

                ReInit();
                CheckLeadInAdmin(landingId, 1);
            }

            //mobile - scroll buttons, menu, fill form
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                //кнопка принять участие - скролл
                //Кнопка "конечно хочу" - скролл 
                CheckScrollingFromMiddle(By.CssSelector(".lp-btn.lp-btn--primary"), "btn1 mobile");
                CheckScrollingFromMiddle(By.CssSelector(".lp-btn.lp-btn--secondary"), "btn secondary mobile");

                //шапка - 4 ссылки
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-menu-header-container--open")).Count,
                    "shown burger menu");
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                CheckBurgerMenuLink(0);
                CheckBurgerMenuLink(1);
                CheckBurgerMenuLink(2);
                CheckBurgerMenuLink(3);
                VerifyIsNull(CheckConsoleLog(true), "funnel after burgerMenu mobile(4)");

                //одна форма
                Driver.ScrollTo(By.ClassName("lp-block-form-with-picture"));
                FillFunnelForm();
                Driver.WaitForElem(waitElem);

                VerifyIsTrue(Driver.Url.IndexOf("/spasibo?lid=") != -1, "funnel thanks page client mobile");
                VerifyAreEqual("Спасибо, ждем вас на вебинаре",
                    Driver.FindElement(By.CssSelector(".lp-h2")).Text.Trim(), "thanks page header mobile");

                ReInit();
                CheckLeadInAdmin(landingId, 2);
            }
        }

        #endregion

        private void CheckScrollingFromTop(IWebElement scroller, string btnNum = "btn1")
        {
            VerifyAreEqual(0, ScrollPosition(), "position before scroll by " + btnNum);
            scroller.Click();
            Thread.Sleep(500);
            VerifyAreNotEqual(0, ScrollPosition(), "position after click by " + btnNum);
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status " + btnNum);
            VerifyIsNull(CheckConsoleLog(true), "funnel main page console  " + btnNum);
            Driver.ScrollToTop();
            Thread.Sleep(500);
        }

        private void CheckScrollingFromMiddle(By scroller, string btnNum)
        {
            Driver.ScrollTo(scroller);
            int curPosition = ScrollPosition();
            Driver.FindElement(scroller).Click();
            Thread.Sleep(500);
            VerifyAreNotEqual(curPosition, ScrollPosition(), "position after click by " + btnNum);
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status " + btnNum);
            VerifyIsNull(CheckConsoleLog(true), "funnel main page console  " + btnNum);
            Driver.ScrollToTop();
            Thread.Sleep(500);
        }
    }
}