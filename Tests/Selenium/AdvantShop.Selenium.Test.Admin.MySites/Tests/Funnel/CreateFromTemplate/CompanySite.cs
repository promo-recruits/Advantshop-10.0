using System.Collections.Generic;
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
    public class CompanySiteCreateTest : MySitesFunctions
    {
        string landingId;
        string landingName;
        string landingTab = "Лендинги";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Catalog\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Catalog\\Catalog.Color.csv",
                "Data\\Admin\\Catalog\\Catalog.Size.csv",
                "Data\\Admin\\Catalog\\Catalog.Photo.csv",
                "Data\\Admin\\Catalog\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\Catalog.Brand.csv",
                "Data\\Admin\\Catalog\\Catalog.Property.csv",
                "Data\\Admin\\Catalog\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Catalog\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Catalog\\Catalog.Offer.csv",
                "Data\\Admin\\Catalog\\Catalog.ProductExt.csv",
                "Data\\Admin\\Catalog\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Catalog\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Catalog\\Catalog.Tag.csv"

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
            GoToAdmin("dashboard/createsite");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        #region CompanySite

        [Test]
        public void CompanySiteBePartnerCreate()
        {
            landingId = "CompanySiteBePartner";
            landingName = "Стань партнером";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status inplace");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log inplace");
                CheckFunnelBlocksConsole(landingId, 0);
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");
                VerifyAreEqual("Стань партнером", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(),
                    "main page header");
                VerifyAreEqual("Стать партнёром",
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[0].Text.Trim(), "text at button1");
                VerifyAreEqual("Стать партнёром",
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[1].Text.Trim(), "text at button2");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CompanySiteExpertCreate()
        {
            landingId = "CompanySiteExpert";
            landingName = "Личная страница эксперта";
            By waitElem = By.CssSelector(".lp-h1");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status inplace");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log inplace");
                CheckFunnelBlocksConsole(landingId, 0);
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                //expected 8, but now 9 https://task.advant.me/adminv3/tasks#?modal=22836
                VerifyAreEqual(8, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");
                VerifyAreEqual("Анна\r\nСоколова",
                    Driver.FindElement(By.ClassName("lp-block-team-details__header")).Text.Trim(), "main page header");

                VerifyAreEqual("Записаться на бесплатную консультацию",
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[0].Text.Trim(), "text at button1");
                VerifyAreEqual("Перейти в магазин",
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[1].Text.Trim(), "text at button2");
                VerifyAreEqual("Читать статью",
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[2].Text.Trim(), "text at button3");
                VerifyAreEqual("Читать статью",
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[3].Text.Trim(), "text at button4");
                VerifyAreEqual("Читать статью",
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[4].Text.Trim(), "text at button5");
                VerifyAreEqual("Отправить",
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[5].Text.Trim(), "text at button6");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CompanySiteUnderConstructionCreate()
        {
            landingId = "CompanySiteUnderConstruction";
            landingName = "Сайт в разработке";
            By waitElem = By.CssSelector(".lp-h1");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status inplace");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log inplace");
                CheckFunnelBlocksConsole(landingId, 0);
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");
                VerifyAreEqual("Сайт в разработке", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(),
                    "main page header");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CompanySiteWithBookingCreate()
        {
            landingId = "CompanySiteWithBooking";
            landingName = "Сайт компании с записью к специалистам";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();

            Thread.Sleep(500);
            Driver.SendKeysInput(AdvBy.DataE2E("LandingSiteName"), landingId);
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
            Driver.WaitForElem(By.CssSelector("h2.swal2-title"));
            VerifyIsTrue(
                Driver.FindElement(By.Id("swal2-content")).Text
                    .IndexOf("Нельзя создать воронку. Не активировано приложение 'Бронирование'.") != -1,
                "booking not active mess");
            Driver.FindElement(By.CssSelector("#swal2-content a")).Click();
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual(BaseUrl + "adminv3/settingsbooking", Driver.Url, "booking settings page");
            Driver.SwitchOn("BookingActive", "BtnSaveBooking");
            Functions.CloseTab(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector(".swal2-actions .btn-success")).Click();
            Thread.Sleep(1000);

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
                VerifyAreEqual(9, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel content page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel content page console log");
                VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons content page count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CompanySiteWithCatalogCreate()
        {
            landingId = "CompanySiteWithCatalog";
            landingName = "Сайт компании с каталогом товаров";
            By waitElem = By.CssSelector(".lp-h1");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(6, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();

            SetFunnelCategories(new List<string> {"TestCategory3", "TestCategory4"});
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "about page");
                CheckFunnelPageInplaceEnabled(2, waitElem, landingId, "menu page");
                CheckFunnelPageInplaceEnabled(3, waitElem, landingId, "banquet page");
                CheckFunnelPageInplaceEnabled(4, waitElem, landingId, "contacts page");
                CheckFunnelPageInplaceEnabled(5, waitElem, landingId, "photos page");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons count");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--secondary")).Count,
                    "secondary buttons count");
                VerifyAreEqual("Тимьян", Driver.FindElement(waitElem).Text.Trim(), "main page header");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel about page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel about page console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons about page count");
                VerifyAreEqual("О ресторане", Driver.FindElement(waitElem).Text.Trim(), "about page header");

                GoToFunnelPageFromLp(2, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel menu page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel menu page console log");
                VerifyAreEqual(7, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons menu page count");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--secondary")).Count,
                    "secondary buttons count");
                VerifyAreEqual("Меню и Доставка", Driver.FindElement(waitElem).Text.Trim(), "menu page header");

                GoToFunnelPageFromLp(3, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel banquet page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel banquet page console log");
                VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons banquet page count");
                VerifyAreEqual("Банкеты", Driver.FindElement(waitElem).Text.Trim(), "banquet page header");

                GoToFunnelPageFromLp(4, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel contacts page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel contacts page console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons contacts page count");
                VerifyAreEqual("Контакты", Driver.FindElement(waitElem).Text.Trim(), "contacts page header");

                GoToFunnelPageFromLp(5, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel photos page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel photos page console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons photos page count");
                VerifyAreEqual("Фотогалерея", Driver.FindElement(waitElem).Text.Trim(), "photos page header");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CompanySiteWithLeadsCreate()
        {
            landingId = "CompanySiteWithLeads";
            landingName = "Сайт компании, сбор заявок";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status inplace");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log inplace");
                CheckFunnelBlocksConsole(landingId, 0);
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");
                VerifyAreEqual("Название компании", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(),
                    "main page header");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CompanySiteWithPricesCreate()
        {
            landingId = "CompanySiteWithPrices";
            landingName = "Сайт компании с ценами";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status inplace");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log inplace");
                CheckFunnelBlocksConsole(landingId, 0);
            }
            GoToAdmin("dashboard/createsite");
            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            GoToAdmin("dashboard/createsite");
            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName("dsf");

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");
                VerifyAreEqual("Универсальный сайт", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(),
                    "main page header");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void InstagramFunnelCreate()
        {
            landingId = "InstagramFunnel";
            landingName = "Микролендинг \"Instagram\"";
            By waitElem = By.CssSelector(".lp-block-team-three-columns-rounded");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status inplace");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log inplace");
                CheckFunnelBlocksConsole(landingId, 0);
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");
                VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".lp-block-contacts-buttons-social__btn")).Count,
                    "buttons count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        #endregion
    }

    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CompanySiteCheckFunctionalityTest : MySitesFunctions
    {
        string landingId;
        string defaultDomain = "http://mydomain123.ru/";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Catalog\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Catalog\\Catalog.Color.csv",
                "Data\\Admin\\Catalog\\Catalog.Size.csv",
                "Data\\Admin\\Catalog\\Catalog.Photo.csv",
                "Data\\Admin\\Catalog\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\Catalog.Brand.csv",
                "Data\\Admin\\Catalog\\Catalog.Property.csv",
                "Data\\Admin\\Catalog\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Catalog\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Catalog\\Catalog.Offer.csv",
                "Data\\Admin\\Catalog\\Catalog.ProductExt.csv",
                "Data\\Admin\\Catalog\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Catalog\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Catalog\\Catalog.Tag.csv",
                "Data\\Admin\\Catalog\\Catalog.ProductPropertyValue.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CompanySite\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CompanySite\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CompanySite\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CompanySite\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CompanySite\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CompanySite\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CompanySite\\CMS.LandingSubBlock.csv"
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

        #region CompanySite

        [Test]
        public void CompanySiteBePartnerCheck()
        {
            landingId = "CompanySiteBePartner";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //client: scroll to top, fill first modal form, second modal form
            {
                ReInitClient();
                GoToClient("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                CheckScrollToTop(By.ClassName("text-reviews"));

                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[0].Click();
                FillFunnelFormModal();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message modal");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form modal");

                ReInit();
                CheckLeadInAdmin(landingId, 1);


                ReInitClient();
                GoToClient("lp/" + landingId);

                Driver.ScrollTo(By.ClassName("text-reviews"));
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[1].Click();
                FillFunnelFormModal();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message modal");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form modal");

                ReInit();
                CheckLeadInAdmin(landingId, 2);
            }

            //mobile: burger, firt menu link, second modal form
            {
                ReInitClient();
                GoToMobile("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log mobile");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "client log mobile afret burger show-hide");
                CheckBurgerMenuLink(0);

                Driver.ScrollTo(By.ClassName("text-reviews"));
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[1].Click();
                FillFunnelFormModal();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message modal mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form modal mobile");

                ReInit();
                CheckLeadInAdmin(landingId, 3);
            }
        }

        [Test]
        public void CompanySiteExpertCheck()
        {
            landingId = "CompanySiteExpert";
            By waitElem = By.CssSelector(".lp-h1");

            GoToClient("lp/" + landingId.ToLower());

            //client: scroll to top, click review slide, link to store btn2-5, fill form, modal form btn1
            {
                ReInitClient();
                GoToClient("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                CheckScrollToTop(By.ClassName("lp-block-contacts-form"));

                ClickReviewDot();

                Driver.ScrollToTop();

                VerifyAreEqual(defaultDomain,
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[1].GetAttribute("href"),
                    "btn2 link");
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);
                Functions.OpenNewTab(Driver, BaseUrl);
                VerifyAreEqual(defaultDomain, Driver.Url, "btn2 url");
                Functions.CloseTab(Driver, BaseUrl);

                VerifyAreEqual(defaultDomain,
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[2].GetAttribute("href"),
                    "btn3 link");
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[2].Click();
                Thread.Sleep(1000);
                Functions.OpenNewTab(Driver, BaseUrl);
                VerifyAreEqual(defaultDomain, Driver.Url, "btn3 url");
                Functions.CloseTab(Driver, BaseUrl);

                VerifyAreEqual(defaultDomain,
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[3].GetAttribute("href"),
                    "btn4 link");
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[3].Click();
                Thread.Sleep(1000);
                Functions.OpenNewTab(Driver, BaseUrl);
                VerifyAreEqual(defaultDomain, Driver.Url, "btn4 url");
                Functions.CloseTab(Driver, BaseUrl);

                VerifyAreEqual(defaultDomain,
                    Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[4].GetAttribute("href"),
                    "btn5 link");
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[4].Click();
                Thread.Sleep(1000);
                Functions.OpenNewTab(Driver, BaseUrl);
                VerifyAreEqual(defaultDomain, Driver.Url, "btn5 url");
                Functions.CloseTab(Driver, BaseUrl);

                FillFunnelForm();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form");

                ReInit();
                CheckLeadInAdmin(landingId, 1);


                ReInitClient();
                GoToClient("lp/" + landingId);

                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message modal");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form modal");

                ReInit();
                CheckLeadInAdmin(landingId, 2);
            }

            //mobile: click store slide, modal form btn1
            {
                ReInitClient();
                GoToMobile("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log mobile");

                ClickReviewDot();

                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message modal mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form modal mobile");

                ReInit();
                CheckLeadInAdmin(landingId, 3);
            }
        }

        [Test]
        public void CompanySiteUnderConstructionCheck()
        {
            landingId = "CompanySiteUnderConstruction";
            By waitElem = By.CssSelector(".lp-h1");

            GoToClient("lp/" + landingId.ToLower());

            //client: fill form
            {
                ReInitClient();
                GoToClient("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal(textarea: true);

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message modal");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form modal");

                ReInit();
                CheckLeadInAdmin(landingId, 1);
            }

            //mobile: burger menu, fill form
            {
                ReInitClient();
                GoToMobile("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log mobile");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "client log mobile afret burger show-hide");

                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal(textarea: true);

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message modal mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form modal mobile");

                ReInit();
                CheckLeadInAdmin(landingId, 2);
            }
        }

        [Test]
        public void CompanySiteWithBookingCheck()
        {
            landingId = "CompanySiteWithBooking";
            By waitElem = By.CssSelector(".lp-h2");

            GoToAdmin("settingsbooking");
            Driver.SwitchOn("BookingActive", "BtnSaveBooking");

            GoToClient("lp/" + landingId.ToLower());

            //client: 3 ссылки скролла, 2 кнопки скролла, скролл снизу вверх, "показать услуги" у всех, Записаться у всех, 
            {
                ReInitClient();
                GoToClient("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                CheckScrollToTop(By.ClassName("lp-block-contacts-map-background"));

                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-menu-header__link"))[0], "menu link1");
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-menu-header__link"))[1], "menu link2");
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-menu-header__link"))[2], "menu link3");

                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[1], "scroll btn1");
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[2], "scroll btn2");

                Driver.ScrollTo(By.ClassName("lp-block-booking-three-columns"));
                Driver.FindElements(By.CssSelector("modal-booking-services a"))[0].Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".adv-modal:not(.ng-hide)")).Count,
                    "show modal link1");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();

                Driver.FindElements(By.CssSelector("modal-booking-services a"))[1].Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".adv-modal:not(.ng-hide)")).Count,
                    "show modal link2");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();

                Driver.FindElements(By.CssSelector("modal-booking-services a"))[2].Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".adv-modal:not(.ng-hide)")).Count,
                    "show modal link3");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console after service preview");

                Driver.FindElement(AdvBy.DataE2E("BookingResourcesBtn")).Click();
                Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
                VerifyAreEqual("Онлайн Запись",
                    Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .lp-form__title")).Text.Trim(),
                    "modal header1");
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .lp-form__subtitle")).Text.Trim(),
                    "modal sub header1");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(AdvBy.DataE2E("BookingResourcesBtn"))[1].Click();
                Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
                VerifyAreEqual("Онлайн Запись",
                    Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .lp-form__title")).Text.Trim(),
                    "modal header2");
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .lp-form__subtitle")).Text.Trim(),
                    "modal sub header2");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(AdvBy.DataE2E("BookingResourcesBtn"))[2].Click();
                Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
                VerifyAreEqual("Онлайн Запись",
                    Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .lp-form__title")).Text.Trim(),
                    "modal header3");
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .lp-form__subtitle")).Text.Trim(),
                    "modal sub header3");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client after booking modal");


                Refresh();
                Driver.ScrollTo(By.ClassName("lp-block-booking-three-columns"));
                Driver.FindElements(AdvBy.DataE2E("BookingResourcesBtn"))[1].Click();
                Driver.WaitForElem(By.ClassName("lp-modal-booking"));
                string curTime = Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Text.Trim();
                Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Click();
                Thread.Sleep(500);
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("modal-booking-form__time")).Text.IndexOf("Время: " + curTime) !=
                    -1, "selected time (1)");
                Driver.FindElement(By.CssSelector(".modal-booking-form__change-time")).Click();
                Thread.Sleep(500);
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.ClassName("modal-booking-form__subtitle")).Text.Trim(),
                    "modal sub header (2)");
                curTime = Driver.FindElements(By.ClassName("lp-modal-booking-time-btn"))[2].Text.Trim();
                Driver.FindElements(By.ClassName("lp-modal-booking-time-btn"))[2].Click();
                Thread.Sleep(500);
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("modal-booking-form__time")).Text.IndexOf("Время: " + curTime) !=
                    -1, "selected time (1)");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client after booking modal");

                Refresh();
                Driver.FindElement(AdvBy.DataE2E("BookingResourcesBtn")).Click();
                Driver.WaitForElem(By.ClassName("lp-modal-booking"));
                Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal();
                Driver.WaitForElem(waitElem);
                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?bid=") != -1,
                    "funnel thanks page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console client mobile");
                VerifyAreEqual("Ваша заявка принята", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(),
                    "thanks page header");

                CheckLeadInAdmin(landingId, 0);
                CheckLeadBooking("FirstName", 1);
            }

            //mobile: меню - 3 ссылки скролла, 2 кнопки скролла; "показать услуги" у всех, Записаться у всех, 
            {
                ReInitClient();
                GoToMobile("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log mobile");

                //скроллы по 2 кнопкам, 
                CheckScrollingFromTop(Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[2], "btn1 mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                ////шапка - три ссылки
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-menu-header-container--open")).Count,
                    "shown burger menu mobile");
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                CheckBurgerMenuLink(0);
                CheckBurgerMenuLink(1);
                CheckBurgerMenuLink(2);
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreNotEqual(0, ScrollPosition(), "position after click by burger menu link mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel after burgerMenu mobile(4)");

                Driver.ScrollTo(By.ClassName("lp-block-booking-three-columns"));
                Driver.FindElements(By.CssSelector("modal-booking-services a"))[0].Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".adv-modal:not(.ng-hide)")).Count,
                    "show modal link1 mobile");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();

                Driver.FindElements(By.CssSelector("modal-booking-services a"))[1].Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".adv-modal:not(.ng-hide)")).Count,
                    "show modal link2 mobile");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();

                Driver.FindElements(By.CssSelector("modal-booking-services a"))[2].Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".adv-modal:not(.ng-hide)")).Count,
                    "show modal link3 mobile");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console after service preview mobile");


                Driver.FindElement(AdvBy.DataE2E("BookingResourcesBtn")).Click();
                Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(AdvBy.DataE2E("BookingResourcesBtn"))[1].Click();
                Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(AdvBy.DataE2E("BookingResourcesBtn"))[2].Click();
                Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client after booking modal mobile");


                Refresh();
                Driver.ScrollTo(By.ClassName("lp-block-booking-three-columns"));
                Driver.FindElements(AdvBy.DataE2E("BookingResourcesBtn"))[1].Click();
                Driver.WaitForElem(By.ClassName("lp-modal-booking"));
                string curTime = Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Text.Trim();
                Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Click();
                Thread.Sleep(500);
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("modal-booking-form__time")).Text.IndexOf("Время: " + curTime) !=
                    -1, "selected time (1) mobile");
                Driver.FindElement(By.CssSelector(".modal-booking-form__change-time")).Click();
                Thread.Sleep(500);
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.ClassName("modal-booking-form__subtitle")).Text.Trim(),
                    "modal sub header (2) mobile");
                curTime = Driver.FindElements(By.ClassName("lp-modal-booking-time-btn"))[2].Text.Trim();
                Driver.FindElements(By.ClassName("lp-modal-booking-time-btn"))[2].Click();
                Thread.Sleep(500);
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("modal-booking-form__time")).Text.IndexOf("Время: " + curTime) !=
                    -1, "selected time (1) mobile");
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client after booking modal mobile");

                Refresh();
                Driver.FindElement(AdvBy.DataE2E("BookingResourcesBtn")).Click();
                Driver.WaitForElem(By.ClassName("lp-modal-booking"));
                Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal();
                Driver.WaitForElem(waitElem);
                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?bid=") != -1,
                    "funnel thanks page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console client mobile");
                VerifyAreEqual("Ваша заявка принята", Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(),
                    "thanks page header mobile");

                CheckLeadInAdmin(landingId, 0);
                CheckLeadBooking("FirstName", 2);
            }
        }

        [Test]
        public void CompanySiteWithCatalogCheckNOTCOMPLETE()
        {
            landingId = "CompanySiteWithCatalog";
            By waitElem = By.CssSelector(".lp-h1");
            string orderNum = "";

            VerifyIsTrue(false, "test not work");

            GoToClient("lp/" + landingId.ToLower());

            //mainpage menu
            {
                ReInitClient();
                GoToClient("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                //btn to menu, btn to banket, scroll next, check gallery
                CheckScrollToTop(By.ClassName("block-type-contacts"));
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/meniu-i-dostavka").ToLower(),
                    Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).GetAttribute("href"),
                    "mainpage btn 1");
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/meniu-i-dostavka").ToLower(), Driver.Url,
                    "url mainpage btn1");
                GoToClient("lp/" + landingId);
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/bankety").ToLower(),
                    Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--secondary")).GetAttribute("href"),
                    "mainpage btn 2");
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--secondary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/bankety").ToLower(), Driver.Url, "url mainpage btn2");
                GoToClient("lp/" + landingId);
                CheckScrollingFromTop(Driver.FindElement(By.ClassName("scroll-to-block-trigger")));
                Driver.FindElements(By.ClassName("lp-block-gallery"))[0]
                    .FindElements(By.ClassName("picture-loader-trigger-image"))[0].Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("next-button")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("previous-button")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("close-button")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel mainpage after gallery");


                Driver.FindElements(By.ClassName("lp-menu-header__link"))[0].Click();
                Thread.Sleep(1000);
                //видео, кнопка отзыва
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/o-restorane").ToLower(), Driver.Url,
                    "link to about-page");
                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("iframe-responsive__container")).Click();
                Thread.Sleep(3000);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client after video play");
                Driver.ScrollTo(By.ClassName("lp-block-reviews"));
                Driver.FindElement(By.CssSelector(".slick-dots li:not(.slick-active)")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "console client after review click");

                //click to logo
                Driver.FindElement(By.CssSelector(".lp-header-logo img")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual((BaseUrl + "lp/" + landingId).ToLower(), Driver.Url, "link at logo");

                Driver.FindElements(By.ClassName("lp-menu-header__link"))[2].Click();
                Thread.Sleep(1000);
                //scroll to top, scroll button, carousel dot, carousel next, fill form
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/bankety").ToLower(), Driver.Url,
                    "link to banquet-page");
                CheckScrollToTop(By.ClassName("block-type-form"));
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")), "btn1 bankety");
                Driver.ScrollTo(By.ClassName("image-carousel"));
                Driver.FindElement(By.ClassName("slick-next")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".slick-dots li:not(.slick-active)")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "console client after carousel clicks");
                FillFunnelForm();
                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(), "success callback");
                CheckLeadInAdmin(landingId, 1);

                ReInitClient();
                GoToClient("lp/" + landingId + "/meniu-i-dostavka");

                Driver.FindElements(By.ClassName("lp-menu-header__link"))[3].Click();
                Thread.Sleep(2000);
                //photos: click to first and second gallery
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/fotogalereya").ToLower(), Driver.Url,
                    "link to photo-page");
                Driver.FindElements(By.ClassName("lp-block-gallery"))[0]
                    .FindElements(By.ClassName("picture-loader-trigger-image"))[0].Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.Id("next-button")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("close-button")).Click();
                Thread.Sleep(500);
                Driver.FindElements(By.ClassName("lp-block-gallery"))[1]
                    .FindElements(By.ClassName("picture-loader-trigger-image"))[3].Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.Id("previous-button")).Click();
                Driver.FindElement(By.Id("previous-button")).Click();
                Driver.FindElement(By.Id("previous-button")).Click();
                Driver.FindElement(By.Id("previous-button")).Click();
                Driver.FindElement(By.Id("previous-button")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("close-button")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel photos page console log after photos");

                Driver.ScrollToTop();
                Driver.FindElements(By.ClassName("lp-menu-header__link"))[4].Click();
                Thread.Sleep(1000);
                //contacts: check header, console
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/kontakty").ToLower(), Driver.Url,
                    "link to contacts-page");


                Driver.FindElements(By.ClassName("lp-menu-header__link"))[1].Click();
                Thread.Sleep(1000);
                //btn 1, switch categories, add products like in multiproduct
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/meniu-i-dostavka").ToLower(), Driver.Url,
                    "link to menu-page");
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel menu btn1");

                Driver.FindElements(By.ClassName("category-name-button"))[1].Click();
                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[0]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Driver.FindElement(By.CssSelector(".lp-product-info__payment-item  .lp-btn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after add to cart");

                Driver.FindElement(By.ClassName("lp-cart-trigger")).Click();
                Thread.Sleep(1000);
                string cartResult = Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim();
                VerifyAreEqual("320 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price before spinbox more");
                Driver.FindElement(By.ClassName("spinbox-more")).Click();
                VerifyAreEqual("640 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price after spinbox more");
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(By.ClassName("category-name-button"))[0].Click();
                Thread.Sleep(500);
                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[2]
                    .FindElement(By.ClassName("lp-products-view-item-link-wrap")).Click();
                Driver.FindElements(By.CssSelector(".lp-product-info__payment-item  .lp-btn"))[1].Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".lp-cart-btn-confirm .lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success/") != -1 && Driver.Url.IndexOf("?mode=lp") != -1,
                    "funnel success client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct16", "TestProduct13" });
            }

            //mobile:
            {
                ReInitClient();
                GoToMobile("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                //btn to menu, btn to banket, scroll next, check gallery
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/meniu-i-dostavka").ToLower(),
                    Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).GetAttribute("href"),
                    "mainpage btn 1");
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/meniu-i-dostavka").ToLower(), Driver.Url,
                    "url mainpage btn1");
                GoToMobile("lp/" + landingId);
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/bankety").ToLower(),
                    Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--secondary")).GetAttribute("href"),
                    "mainpage btn 2");
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--secondary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/bankety").ToLower(), Driver.Url, "url mainpage btn2");
                GoToMobile("lp/" + landingId);
                CheckScrollingFromTop(Driver.FindElement(By.ClassName("scroll-to-block-trigger")));

                Driver.FindElements(By.ClassName("lp-block-gallery"))[0]
                    .FindElements(By.ClassName("picture-loader-trigger-image"))[0].Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("next-button")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("previous-button")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("close-button")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel mainpage after gallery");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Driver.FindElements(By.ClassName("lp-menu-header__link"))[0].Click();
                Thread.Sleep(1000);
                //видео, кнопка отзыва
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/o-restorane").ToLower(), Driver.Url,
                    "link to about-page");
                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("iframe-responsive__container")).Click();
                Thread.Sleep(3000);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client after video play");
                Driver.ScrollTo(By.ClassName("lp-block-reviews"));
                Driver.FindElement(By.CssSelector(".slick-dots li:not(.slick-active)")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "console client after review click");

                //click to logo
                Driver.FindElement(By.CssSelector(".lp-header-logo img")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual((BaseUrl + "lp/" + landingId).ToLower(), Driver.Url, "link at logo");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Driver.FindElements(By.ClassName("lp-menu-header__link"))[2].Click();
                Thread.Sleep(1000);
                //scroll to top, scroll button, carousel dot, carousel next, fill form
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/bankety").ToLower(), Driver.Url,
                    "link to banquet-page");
                CheckScrollToTop(By.ClassName("block-type-form"));
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")), "btn1 bankety");
                Driver.ScrollTo(By.ClassName("image-carousel"));
                Driver.FindElement(By.CssSelector(".slick-dots li:not(.slick-active)")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "console client after carousel clicks");
                FillFunnelForm();
                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(), "success callback");
                CheckLeadInAdmin(landingId, 2);

                ReInitClient();
                GoToMobile("lp/" + landingId + "/meniu-i-dostavka");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Driver.FindElements(By.ClassName("lp-menu-header__link"))[3].Click();
                Thread.Sleep(2000);
                //photos: click to first and second gallery
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/fotogalereya").ToLower(), Driver.Url,
                    "link to photo-page");
                Driver.FindElements(By.ClassName("lp-block-gallery"))[0]
                    .FindElements(By.ClassName("picture-loader-trigger-image"))[0].Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.Id("next-button")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("close-button")).Click();
                Thread.Sleep(500);
                Driver.FindElements(By.ClassName("lp-block-gallery"))[1]
                    .FindElements(By.ClassName("picture-loader-trigger-image"))[3].Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.Id("previous-button")).Click();
                Driver.FindElement(By.Id("previous-button")).Click();
                Driver.FindElement(By.Id("previous-button")).Click();
                Driver.FindElement(By.Id("previous-button")).Click();
                Driver.FindElement(By.Id("previous-button")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.Id("close-button")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel photos page console log after photos");

                Driver.ScrollToTop();
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Driver.FindElements(By.ClassName("lp-menu-header__link"))[4].Click();
                Thread.Sleep(1000);
                //contacts: check header, console
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/kontakty").ToLower(), Driver.Url,
                    "link to contacts-page");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Driver.FindElements(By.ClassName("lp-menu-header__link"))[1].Click();
                Thread.Sleep(1000);
                //btn 1, switch categories, add products like in multiproduct
                VerifyAreEqual((BaseUrl + "lp/" + landingId + "/meniu-i-dostavka").ToLower(), Driver.Url,
                    "link to menu-page");
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel menu btn1");

                Driver.FindElements(By.ClassName("category-name-button"))[1].Click();
                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[0]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Driver.FindElement(By.CssSelector(".lp-product-info__payment-item  .lp-btn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after add to cart");

                Driver.FindElement(By.ClassName("lp-cart-trigger")).Click();
                Thread.Sleep(1000);
                string cartResult = Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim();
                VerifyAreEqual("320 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price before spinbox more");
                Driver.FindElement(By.ClassName("spinbox-more")).Click();
                VerifyAreEqual("640 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price after spinbox more");
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(By.ClassName("category-name-button"))[0].Click();
                Thread.Sleep(500);
                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[2]
                    .FindElement(By.ClassName("lp-products-view-item-link-wrap")).Click();
                Driver.FindElements(By.CssSelector(".lp-product-info__payment-item  .lp-btn"))[1].Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".lp-cart-btn-confirm .lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success/") != -1 && Driver.Url.IndexOf("?mode=lp") != -1,
                    "funnel success client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client");

                orderNum = GetOrderNum(true);
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct16", "TestProduct13" });
            }
        }

        [Test]
        public void CompanySiteWithLeadsCheck()
        {
            landingId = "CompanySiteWithLeads";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //client: scroll to top, scroll btn 1, scroll btn2, fill form
            {
                ReInitClient();
                GoToClient("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                CheckScrollToTop(By.ClassName("lp-block-contacts-map-background"));

                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")), "btn1");

                Driver.FindElement(By.ClassName("scroll-to-block-trigger")).Click();
                VerifyAreNotEqual(0, ScrollPosition(), "position after click by scroll-trigger");

                Driver.ScrollTo(By.ClassName("lp-block-form-with-text-aside"));
                FillFunnelForm();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form");

                ReInit();
                CheckLeadInAdmin(landingId, 1);
            }

            //mobile: without menu, scroll btn 1, scroll btn2, fill form
            {
                ReInitClient();
                GoToMobile("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log mobile");

                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")), "btn1 mobile");

                Driver.FindElement(By.ClassName("scroll-to-block-trigger")).Click();
                VerifyAreNotEqual(0, ScrollPosition(), "position after click by scroll-trigger mobile");

                Driver.ScrollTo(By.ClassName("lp-block-form-with-text-aside"));
                FillFunnelForm();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form");

                ReInit();
                CheckLeadInAdmin(landingId, 2);
            }
        }

        [Test]
        public void CompanySiteWithPricesCheck()
        {
            landingId = "CompanySiteWithPrices";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //client: scroll to top, 3 ссылки в шапке, клик по ссылке вниз, клик по отзыву, обычная форма, форма из шапки
            {
                ReInitClient();
                GoToClient("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                CheckScrollToTop(By.ClassName("lp-block-contacts-form"));

                CheckScrollingFromTop(Driver.FindElements(By.ClassName("lp-menu-header__item"))[0], "link1");
                CheckScrollingFromTop(Driver.FindElements(By.ClassName("lp-menu-header__item"))[1], "link2");
                CheckScrollingFromTop(Driver.FindElements(By.ClassName("lp-menu-header__item"))[2], "link3");

                Driver.FindElement(By.ClassName("scroll-to-block-trigger")).Click();
                VerifyAreNotEqual(0, ScrollPosition(), "position after click by scroll-trigger");

                ClickReviewDot();

                Driver.ScrollTo(By.ClassName("lp-block-contacts-form"));
                FillFunnelForm();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form");

                ReInit();
                CheckLeadInAdmin(landingId, 1);


                ReInitClient();
                GoToClient("lp/" + landingId);
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[1].Click();
                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[1].Click();
                Thread.Sleep(500);
                FillFunnelFormModal();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message modal");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form modal");

                ReInit();
                CheckLeadInAdmin(landingId, 2);
            }

            //mobile: 3 ссылки в бургер меню, клик по ссылке вниз, клик по отзыву, форма из шапки
            {
                ReInitClient();
                GoToMobile("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log mobile");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "client log mobile afret burger show-hide");
                CheckBurgerMenuLink(0);
                CheckBurgerMenuLink(1);
                CheckBurgerMenuLink(2);

                Driver.FindElement(By.ClassName("scroll-to-block-trigger")).Click();
                VerifyAreNotEqual(0, ScrollPosition(), "position after click by scroll-trigger");

                ClickReviewDot();

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(500);
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal();

                Driver.WaitForElem(By.ClassName("lp-form__content--success"));
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form sended successfull message modal");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log after form modal");

                ReInit();
                CheckLeadInAdmin(landingId, 3);
            }
        }

        [Test]
        public void InstagramFunnelCheck()
        {
            landingId = "InstagramFunnel";
            By waitElem = By.CssSelector(".lp-block-team-three-columns-rounded");

            GoToClient("lp/" + landingId.ToLower());

            //client: header, btn link, compare social buttons href, without scroll
            {
                ReInitClient();
                GoToClient("lp/" + landingId);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                VerifyAreEqual("BodyFit",
                    Driver.FindElement(By.ClassName("lp-block-team-three-columns-rounded__header")).Text.Trim(),
                    "header client");
                VerifyAreEqual("Интернет-магазин",
                    Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Text.Trim(), "btn1 client");
                VerifyAreEqual(defaultDomain,
                    Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).GetAttribute("href"),
                    "btn1 client href");

                VerifyAreEqual("WhatsApp",
                    Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[0].Text,
                    "text at social btn1");
                VerifyAreEqual(
                    "https://wa.me/+7999999999?text=%D0%97%D0%B4%D1%80%D0%B0%D0%B2%D1%81%D1%82%D0%B2%D1%83%D0%B9%D1%82%D0%B5!",
                    Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[0].GetAttribute("href"),
                    "href at social btn1");
                VerifyAreEqual("ВКонтакте",
                    Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[1].Text,
                    "text at social btn2");
                VerifyAreEqual("https://vk.com/advantshop",
                    Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[1].GetAttribute("href"),
                    "href at social btn2");
                VerifyAreEqual("Facebook",
                    Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[2].Text,
                    "text at social btn3");
                VerifyAreEqual("https://m.me/%3CMassageId%3E",
                    Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[2].GetAttribute("href"),
                    "href at social btn3");
                VerifyAreEqual("Позвонить",
                    Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[3].Text,
                    "text at social btn4");
                VerifyAreEqual("tel:+79999999999",
                    Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[3].GetAttribute("href"),
                    "href at social btn4");

                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(defaultDomain, Driver.Url, "url afret click by btn1");

                GoToClient("lp/" + landingId);
                Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[0].Click();
                Functions.OpenNewTab(Driver, BaseUrl);
                VerifyIsTrue(Driver.Url.IndexOf("https://api.whatsapp.com/send/?phone") != -1,
                    "after click by social btn1");
                Functions.CloseTab(Driver, BaseUrl);

                Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[1].Click();
                Functions.OpenNewTab(Driver, BaseUrl);
                VerifyIsTrue(Driver.Url.IndexOf("https://vk.com/advantshop") != -1, "after click by social btn2");
                Functions.CloseTab(Driver, BaseUrl);

                Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[2].Click();
                Functions.OpenNewTab(Driver, BaseUrl);
                VerifyIsTrue(Driver.Url.IndexOf("https://www.messenger.com/") != -1, "after click by social btn3");
                Functions.CloseTab(Driver, BaseUrl);

                Driver.FindElements(By.ClassName("lp-block-contacts-buttons-social__btn"))[3].Click();
                VerifyIsNull(CheckConsoleLog(true), "after socila btn phone click");
            }

            //mobile: header, btn link, compare social buttons href, without menu
            {
                ReInitClient();
                GoToMobile("lp/" + landingId);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log mobile");

                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(defaultDomain, Driver.Url, "url afret click by btn1");
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

        private void ClickReviewDot()
        {
            Driver.ScrollTo(By.ClassName("lp-block-reviews"));
            Driver.FindElement(By.CssSelector(".slick-dots li:not(.slick-active)")).Click();
            Thread.Sleep(500);
            VerifyIsNull(CheckConsoleLog(true), "console client after review click");
        }

        protected void CheckLeadBooking(string leadName, int leadsCount)
        {
            GoToAdmin("booking");
            SelectItem("BookingJournalChangeViewMode", "Таблица");
            Thread.Sleep(2000);
            Driver.GridFilterSendKeys(leadName);
            VerifyAreEqual("Найдено записей: " + leadsCount,
                Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text, "lead from funnel");
        }
    }
}