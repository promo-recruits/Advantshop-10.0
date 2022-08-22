using System;
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
    public class CollectingLeadsCreateTest : MySitesFunctions
    {
        string landingId;
        string landingName;
        string landingTab = "Сбор лидов";
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
                "Data\\Admin\\Catalog\\Catalog.ProductPropertyValue.csv"
            );

            Init(false);

            GoToAdmin("leads?salesFunnelId=1");
            Driver.FindElement(AdvBy.DataE2E("UseGrid")).Click();
            Thread.Sleep(1000);

            GoToAdmin("dashboard/createsite");
            CreateEmptyFunnel("emptyfunnel");
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

        #region ProductSinglePages

        [Test]
        public void ArticleLeadCreate()
        {
            landingId = "ArticleLead";
            landingName = "Воронка \"Статья\"";
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
                VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CollectContactsForAccessFunnelCreate()
        {
            landingId = "CollectContactsForAccess";
            landingName = "Воронка \"Доступ к категории\"";
            By waitElem = By.CssSelector(".lp-h1");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            Thread.Sleep(500);
            SelectItem("ModalAddLandingSiteSelectPostAction", "Воронка");
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
            Thread.Sleep(500);
            SelectItem("ModalAddLandingSiteSelectPostFunnel", "emptyfunnel");
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
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

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CollectContactsForAccessShopCategoryCreate()
        {
            landingId = "CollectContactsForAccess";
            landingName = "Воронка \"Доступ к категории\"";
            By waitElem = By.CssSelector(".lp-h1");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            Thread.Sleep(500);
            SelectItem("ModalAddLandingSiteSelectPostAction", "Категория Интернет-магазина");
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.Id("1_anchor")).Click();
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
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

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CollectContactsForAccessURLCreate()
        {
            landingId = "CollectContactsForAccess";
            landingName = "Воронка \"Доступ к категории\"";
            By waitElem = By.CssSelector(".lp-h1");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            Thread.Sleep(500);
            SelectItem("ModalAddLandingSiteSelectPostAction", "Свой URL");
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
            Thread.Sleep(500);
            //driver.FindElement(By.Id("1_anchor")).Click();
            Driver.FindElement(By.ClassName("form-control")).SendKeys("https://www.advantshop.net/");
            Driver.FindElement(AdvBy.DataE2E("LandingSiteNext")).Click();
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

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void ConsultingNewCreate()
        {
            landingId = "ConsultingNew";
            landingName = "Воронка \"Консалтинг\"";
            By waitElem = By.CssSelector(".lp-h2");
            By waitElem2 = By.CssSelector(".lp-form__title");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, $"funnel content page status inplace");
                VerifyIsNull(CheckConsoleLog(true), $"funnel content page console log inplace");
                Driver.FindElement(By.ClassName("video__addition-header")).Click();
                CheckFunnelBlocksConsole(landingId, 1); //settings blocks console

                CheckFunnelPageInplaceEnabled(2, waitElem2, landingId, "anketa page");
                CheckFunnelPageInplaceEnabled(3, waitElem, landingId, "thanks page");
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

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel content page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel content page console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons content page count");

                VerifyAreEqual(String.Empty,
                    Driver.FindElement(By.ClassName("lp-block-booking-three-columns")).GetAttribute("innerHTML").Trim(),
                    "booking not shown");

                GoToFunnelPageFromLp(2, waitElem2);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel anketa status");
                VerifyIsNull(CheckConsoleLog(true), "funnel anketa console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons anketa count");

                GoToFunnelPageFromLp(3, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons thanks page count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }

            //activate booking
            {
                GoToFunnelPageFromLp(1, waitElem);
                //check message aboun disable booking
                VerifyAreEqual("Бронирование не включено",
                    Driver.FindElement(By.CssSelector(".lp-block-booking-three-columns .lp-h2")).Text.Trim(),
                    "booking not active message");
                VerifyAreEqual("Перейти к настройкам бронирования",
                    Driver.FindElement(By.CssSelector(".lp-btn--primary")).Text.Trim(), "booking not active btn");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Functions.OpenNewTab(Driver, BaseUrl);
                Driver.WaitForElem(By.Id("BookingActive"));
                Driver.SwitchOn("BookingActive", "BtnSaveBooking");
                Functions.CloseTab(Driver, BaseUrl);

                Refresh();
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK,
                    "funnel content page status booking active");
                VerifyIsNull(CheckConsoleLog(true), "funnel content page console log  booking active");
                VerifyAreEqual("Бесплатная консультация",
                    Driver.FindElement(By.CssSelector(".lp-block-booking-three-columns .lp-h2")).Text.Trim(),
                    "booking active message");
                VerifyAreEqual("Показать услуги",
                    Driver.FindElement(By.CssSelector(".lp-block-booking-resources__content a")).Text.Trim(),
                    "booking active link");
                VerifyAreEqual("Выбрать время бесплатной консультации",
                    Driver.FindElement(By.CssSelector(".lp-btn--primary")).Text.Trim(), "booking active btn");
            }

            ReInit();
            GoToAdmin("settingsbooking");
            Driver.WaitForElem(By.Id("BookingActive"));
            Driver.SwitchOff("BookingActive", "BtnSaveBooking");
        }

        [Test]
        public void ContactForContentCreate()
        {
            landingId = "ContactForContent";
            landingName = "Воронка \"Захват контакта за контент\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, By.ClassName("lp-block-text-thanks__header"), landingId,
                    "thanks page");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(By.ClassName("lp-block-text-thanks__header"));

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                GoToFunnelPageFromLp(1, By.ClassName("lp-block-text-thanks__header"));
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void ContactForEBookCreate()
        {
            landingId = "ContactForEBook";
            landingName = "Воронка \"Захват контакта за эл. книгу\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "success page");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CouponWithDiscountCreate()
        {
            landingId = "CouponWithDiscount";
            landingName = "Воронка \"Получи купон на скидку\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, By.ClassName("lp-block-text-coupon__block"), landingId, "thanks page");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(By.ClassName("lp-block-text-coupon__block"));

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                GoToFunnelPageFromLp(1, By.ClassName("lp-block-text-coupon__block"));
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void LotteryCreate()
        {
            landingId = "Lottery";
            landingName = "Воронка \"Розыгрыш\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts(new List<string> {"TestProduct3"});

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
                VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons count");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--secondary")).Count,
                    "secondary buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void VideoLeadMagnetNewCreate()
        {
            landingId = "LandingFunnelOrderWithForm";
            landingName = "Воронка \"Лид-магнит \"Видео\"\"";
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
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        #endregion
    }

    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CollectingLeadsCheckFunctionalityTest : MySitesFunctions
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
                "data\\Admin\\Funnel\\FromTemplate\\CollectingLeads\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CollectingLeads\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CollectingLeads\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CollectingLeads\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CollectingLeads\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CollectingLeads\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\FromTemplate\\CollectingLeads\\CMS.LandingSubBlock.csv"
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

        #region ProductSinglePages

        [Test]
        public void ArticleLeadCheckFunctionality()
        {
            landingId = "ArticleLead";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first btn and form + scroll to top button
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());

                Driver.ScrollTo(By.ClassName("lp-footer-social"));
                Driver.FindElement(By.ClassName("scroll-to-top-active")).Click();
                VerifyAreEqual(0, ScrollPosition(), "position after scroll button click");

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                Driver.ScrollTo(By.ClassName("lp-block-form-with-text-aside"));
                FillFunnelForm(phone: false, textarea: true);
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(), "form success message");

                Driver.ScrollTo(By.ClassName("lp-btn--primary"));

                VerifyAreEqual("Выбрать тандыр",
                    Driver.FindElement(By.ClassName("lp-btn--primary")).Text.Trim(), "first button");
                VerifyAreEqual(defaultDomain, Driver.FindElement(By.ClassName("lp-btn--primary")).GetAttribute("href"),
                    "first btn link");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                VerifyAreEqual(defaultDomain, Driver.Url, "first btn link page url");
                Driver.Navigate().Back();

                ReInit();

                CheckLeadInAdmin(landingId, 1);
            }

            //check all pages of funnel mobile client - second btn and form WITHOUT menu
            {
                ReInitClient();

                GoToMobile("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                Driver.ScrollTo(By.ClassName("lp-block-form-with-text-aside"));
                FillFunnelForm(phone: false, textarea: true);
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form success message  mobile");


                Driver.ScrollTo(By.ClassName("lp-btn--primary"), 1);

                VerifyAreEqual("Выбрать тандыр",
                    Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Text.Trim(), "second button mobile");
                VerifyAreEqual(defaultDomain,
                    Driver.FindElements(By.ClassName("lp-btn--primary"))[1].GetAttribute("href"),
                    "second btn link mobile");
                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                VerifyAreEqual(defaultDomain, Driver.Url, "second btn link page url mobile");
                Driver.Navigate().Back();

                ReInit();

                CheckLeadInAdmin(landingId, 2);
            }
        }

        [Test]
        public void CollectContactsForAccessFunnelCheckFunctionality()
        {
            landingId = "CollectContactsForAccess";
            By waitElem = By.CssSelector(".lp-h1");

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first button WITHOUT scroll to top button
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                VerifyAreEqual("FURNITURE 24/7", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(), "page header");
                VerifyAreEqual("Бесплатная доставка офисной мебели на заказы от 1500 рублей",
                    Driver.FindElement(By.ClassName("cover-form-center__subtitle")).Text.Trim(), "page subtitle");
                FillFunnelForm(name: false, phone: false, agreement: false);

                VerifyIsTrue(Driver.Url.IndexOf(BaseUrl + "lp/emptyfunnel?lid=") != -1,
                    "lead in url, success page client");

                CheckLeadInAdmin(landingId, 1);
            }

            //check all pages of funnel mobile client - second button, second downsell WITHOUT menu
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                VerifyAreEqual("FURNITURE 24/7", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(), "page header");
                VerifyAreEqual("Бесплатная доставка офисной мебели на заказы от 1500 рублей",
                    Driver.FindElement(By.ClassName("cover-form-center__subtitle")).Text.Trim(), "page subtitle");
                FillFunnelForm(name: false, phone: false, agreement: false);

                VerifyIsTrue(Driver.Url.IndexOf(BaseUrl + "lp/emptyfunnel?lid=") != -1,
                    "lead in url, success page client mobile");

                CheckLeadInAdmin(landingId, 2);
            }
        }

        [Test]
        public void CollectContactsForAccessShopCategoryCheckFunctionality()
        {
            landingId = "CollectContactsForAccess";
            By waitElem = By.CssSelector(".lp-h1");

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first button WITHOUT scroll to top button
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower() + "-1");

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                VerifyAreEqual("FURNITURE 24/7", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(), "page header");
                VerifyAreEqual("Бесплатная доставка офисной мебели на заказы от 1500 рублей",
                    Driver.FindElement(By.ClassName("cover-form-center__subtitle")).Text.Trim(), "page subtitle");
                FillFunnelForm(name: false, phone: false, agreement: false);

                VerifyIsTrue(Driver.Url.IndexOf(BaseUrl + "adminv2/login") != -1,
                    "lead in url, success page client without active shop");

                CheckLeadInAdmin(landingId, 3);

                GoToAdmin("dashboard/createTemplate?id=_default");
                Driver.FindElement(By.CssSelector(".create__sites-left-block .btn-success")).Click();
                Driver.WaitForElem(By.ClassName("design-first"));

                ReInitClient();
                GoToClient("lp/" + landingId.ToLower() + "-1");
                FillFunnelForm(name: false, phone: false, agreement: false);

                VerifyIsTrue(Driver.Url.IndexOf(BaseUrl + "categories/test-category1?lid=") != -1,
                    "lead in url, success page client");

                CheckLeadInAdmin(landingId, 4);
            }

            //check all pages of funnel mobile client - second button, second downsell WITHOUT menu
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower() + "-1");

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                VerifyAreEqual("FURNITURE 24/7", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(), "page header");
                VerifyAreEqual("Бесплатная доставка офисной мебели на заказы от 1500 рублей",
                    Driver.FindElement(By.ClassName("cover-form-center__subtitle")).Text.Trim(), "page subtitle");
                FillFunnelForm(name: false, phone: false, agreement: false);

                VerifyIsTrue(Driver.Url.IndexOf(BaseUrl + "categories/test-category1?lid=") != -1,
                    "lead in url, success page client");

                CheckLeadInAdmin(landingId, 5);

                GoToAdmin("dashboard");
                Driver.WaitForElem(By.LinkText("Удалить"));
                Driver.FindElement(By.LinkText("Удалить")).Click();
                Driver.SwalConfirm();
            }
        }

        [Test]
        public void CollectContactsForAccessURLCheckFunctionality()
        {
            landingId = "CollectContactsForAccess";
            By waitElem = By.CssSelector(".lp-h1");

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first button WITHOUT scroll to top button
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower() + "-2");

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                VerifyAreEqual("FURNITURE 24/7", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(), "page header");
                VerifyAreEqual("Бесплатная доставка офисной мебели на заказы от 1500 рублей",
                    Driver.FindElement(By.ClassName("cover-form-center__subtitle")).Text.Trim(), "page subtitle");
                FillFunnelForm(name: false, phone: false, agreement: false);

                VerifyIsTrue(Driver.Url.IndexOf("https://www.advantshop.net/?lid=") != -1,
                    "lead in url, success page client");

                CheckLeadInAdmin(landingId, 6);
            }

            //check all pages of funnel mobile client - second button, second downsell WITHOUT menu
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower() + "-2");

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                VerifyAreEqual("FURNITURE 24/7", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(), "page header");
                VerifyAreEqual("Бесплатная доставка офисной мебели на заказы от 1500 рублей",
                    Driver.FindElement(By.ClassName("cover-form-center__subtitle")).Text.Trim(), "page subtitle");
                FillFunnelForm(name: false, phone: false, agreement: false);

                VerifyIsTrue(Driver.Url.IndexOf("https://www.advantshop.net/?lid=") != -1,
                    "lead in url, success page client mobile");

                CheckLeadInAdmin(landingId, 7);
            }
        }

        [Test]
        public void ConsultingNewCheckFunctionality()
        {
            landingId = "ConsultingNew";
            By waitElem = By.CssSelector(".lp-h2");
            By waitElem2 = By.CssSelector(".lp-form__title");

            GoToClient("lp/" + landingId.ToLower());

            //activate booking
            {
                GoToFunnelPageFromLp(1, waitElem);
                //check message aboun disable booking
                VerifyAreEqual("Бронирование не включено",
                    Driver.FindElement(By.CssSelector(".lp-block-booking-three-columns .lp-h2")).Text.Trim(),
                    "booking not active message");
                VerifyAreEqual("Перейти к настройкам бронирования",
                    Driver.FindElement(By.CssSelector(".lp-btn--primary")).Text.Trim(), "booking not active btn");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Functions.OpenNewTab(Driver, BaseUrl);
                Driver.WaitForElem(By.Id("BookingActive"));
                Driver.SwitchOn("BookingActive", "BtnSaveBooking");
                Functions.CloseTab(Driver, BaseUrl);

                Refresh();
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK,
                    "funnel content page status booking active");
                VerifyIsNull(CheckConsoleLog(true), "funnel content page console log  booking active");
                VerifyAreEqual("Бесплатная консультация",
                    Driver.FindElement(By.CssSelector(".lp-block-booking-three-columns .lp-h2")).Text.Trim(),
                    "booking active message");
                VerifyAreEqual("Показать услуги",
                    Driver.FindElement(By.CssSelector(".lp-block-booking-resources__content a")).Text.Trim(),
                    "booking active link");
                VerifyAreEqual("Выбрать время бесплатной консультации",
                    Driver.FindElement(By.CssSelector(".lp-btn--primary")).Text.Trim(), "booking active btn");
            }

            //check all pages of funnel client - first button, without questionary, without booking WITHOUT scroll to top button
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(1000);
                FillFunnelFormModal(false, true, false);

                VerifyIsTrue(Driver.Url.IndexOf("/poleznyi-kontent?lid=") != -1, "funnel kontent page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel kontent status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client");

                CheckLeadInAdmin(landingId, 1);
            }

            //check all pages of funnel client - first button, with questionary, with booking
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(1000);
                FillFunnelFormModal(false, true, false);

                VerifyIsTrue(Driver.Url.IndexOf("/poleznyi-kontent?lid=") != -1, "funnel kontent page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel kontent status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client");

                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("iframe-responsive__container")).Click();
                Thread.Sleep(3000);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client after video play");

                //show services, check console
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (1)");
                Driver.FindElement(AdvBy.DataE2E("BookingResourcesColLink")).FindElement(By.TagName("a")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (2)");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (3)");
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client after modal");

                //select consultation day, close form
                Driver.FindElement(AdvBy.DataE2E("BookingResourcesBtn")).Click();
                Driver.WaitForElem(By.ClassName("lp-modal-booking"));
                VerifyAreEqual("Онлайн Запись", Driver.FindElement(By.ClassName("lp-form__title")).Text.Trim(),
                    "modal header");
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.ClassName("lp-form__subtitle")).Text.Trim(), "modal sub header");
                Driver.FindElements(By.ClassName("adv-modal-close"))[1].Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client after booking modal");

                Driver.FindElement(AdvBy.DataE2E("BookingResourcesBtn")).Click();
                Driver.WaitForElem(By.ClassName("lp-modal-booking"));
                VerifyAreEqual("Онлайн Запись", Driver.FindElement(By.ClassName("lp-form__title")).Text.Trim(),
                    "modal header");
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.ClassName("lp-form__subtitle")).Text.Trim(), "modal sub header");
                string curTime = Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Text.Trim();
                Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Click();
                Thread.Sleep(500);
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("lp-form__subtitle")).Text.IndexOf("Время: " + curTime) != -1,
                    "selected time (1)");
                Driver.FindElement(By.CssSelector(".lp-form__subtitle a")).Click();
                Thread.Sleep(500);
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.ClassName("lp-form__subtitle")).Text.Trim(), "modal sub header (2)");
                curTime = Driver.FindElements(By.ClassName("lp-modal-booking-time-btn"))[1].Text.Trim();
                Driver.FindElements(By.ClassName("lp-modal-booking-time-btn"))[1].Click();
                Thread.Sleep(500);
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("lp-form__subtitle")).Text.IndexOf("Время: " + curTime) != -1,
                    "selected time (1)");
                Driver.FindElements(By.ClassName("adv-modal-close"))[1].Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client after booking modal");

                Driver.FindElement(AdvBy.DataE2E("BookingResourcesBtn")).Click();
                Driver.WaitForElem(By.ClassName("lp-modal-booking"));
                Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal();

                Driver.WaitForElem(waitElem2);
                VerifyIsTrue(Driver.Url.IndexOf("/anketa?lid=") != -1, "funnel anketa page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel kontent status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client");
                FillQuestionary("Questionary Name");

                Driver.WaitForElem(waitElem);
                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?lid=") != -1, "funnel thanks page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console client");
                VerifyAreEqual("Спасибо! Ваша заявка на рассмотрении.",
                    Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "thanks page header");

                CheckLeadInAdmin(landingId, 2);
                CheckLeadQuestionaryDescription("Questionary Name");
                CheckLeadBooking("Questionary Name", 1);
                //repeat for mobile
            }

            //check all pages of funnel mobile - first button, with questionary, with booking
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(1000);
                FillFunnelFormModal(false, true, false);

                VerifyIsTrue(Driver.Url.IndexOf("/poleznyi-kontent?lid=") != -1, "funnel kontent page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel kontent status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client mobile");

                Thread.Sleep(500);
                Driver.FindElement(By.ClassName("iframe-responsive__container")).Click();
                Thread.Sleep(3000);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client mobile after video play");

                //show services, check console
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (1)");
                Driver.FindElement(AdvBy.DataE2E("BookingResourcesColLink")).FindElement(By.TagName("a")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (2)");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (3)");
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client mobile after modal");

                //select consultation day, close form
                Driver.FindElement(AdvBy.DataE2E("BookingResourcesBtn")).Click();
                Driver.WaitForElem(By.ClassName("lp-modal-booking"));
                VerifyAreEqual("Онлайн Запись", Driver.FindElement(By.ClassName("lp-form__title")).Text.Trim(),
                    "modal header");
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.ClassName("lp-form__subtitle")).Text.Trim(), "modal sub header");
                Driver.FindElements(By.ClassName("adv-modal-close"))[1].Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client mobile after booking modal");

                Driver.FindElement(AdvBy.DataE2E("BookingResourcesBtn")).Click();
                Driver.WaitForElem(By.ClassName("lp-modal-booking"));
                VerifyAreEqual("Онлайн Запись", Driver.FindElement(By.ClassName("lp-form__title")).Text.Trim(),
                    "modal header");
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.ClassName("lp-form__subtitle")).Text.Trim(), "modal sub header");
                string curTime = Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Text.Trim();
                Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Click();
                Thread.Sleep(500);
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("lp-form__subtitle")).Text.IndexOf("Время: " + curTime) != -1,
                    "selected time (1)");
                Driver.FindElement(By.CssSelector(".lp-form__subtitle a")).Click();
                Thread.Sleep(500);
                VerifyAreEqual("Запишитесь на удобное время",
                    Driver.FindElement(By.ClassName("lp-form__subtitle")).Text.Trim(), "modal sub header (2)");
                curTime = Driver.FindElements(By.ClassName("lp-modal-booking-time-btn"))[1].Text.Trim();
                Driver.FindElements(By.ClassName("lp-modal-booking-time-btn"))[1].Click();
                Thread.Sleep(500);
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("lp-form__subtitle")).Text.IndexOf("Время: " + curTime) != -1,
                    "selected time (1)");
                Driver.FindElements(By.ClassName("adv-modal-close"))[1].Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client mobile after booking modal");

                Driver.FindElement(AdvBy.DataE2E("BookingResourcesBtn")).Click();
                Driver.WaitForElem(By.ClassName("lp-modal-booking"));
                Driver.FindElement(By.ClassName("lp-modal-booking-time-btn")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal();

                Driver.WaitForElem(waitElem2);
                VerifyIsTrue(Driver.Url.IndexOf("/anketa?lid=") != -1, "funnel anketa page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel kontent status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel kontent console client mobile");
                FillQuestionary("Mobile Questionary Name");

                Driver.WaitForElem(waitElem);
                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?lid=") != -1,
                    "funnel thanks page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console client mobile");
                VerifyAreEqual("Спасибо! Ваша заявка на рассмотрении.",
                    Driver.FindElement(By.ClassName("lp-h2")).Text.Trim(), "thanks page header");

                CheckLeadInAdmin(landingId, 3);
                CheckLeadQuestionaryDescription("Mobile Questionary Name");
                CheckLeadBooking("Mobile Questionary Name", 2);
                //repeat for mobile
            }

            ReInit();
            GoToAdmin("settingsbooking");
            Driver.WaitForElem(By.Id("BookingActive"));
            Driver.SwitchOff("BookingActive", "BtnSaveBooking");
        }

        [Test]
        public void ContactForContentCheckFunctionality()
        {
            landingId = "ContactForContent";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - button, link WITHOUT scroll to top button
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (1)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (2)");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (3)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal(false, true, false);

                Driver.WaitForElem(By.ClassName("lp-block-text-thanks"));

                VerifyIsTrue(Driver.Url.IndexOf("/spasibo?lid=") != -1, "funnel thanks page client");
                VerifyAreEqual("Проверьте почту, мы отправили Вам обещанный документ.",
                    Driver.FindElement(By.CssSelector(".lp-block-text-thanks__content p")).Text.Trim(),
                    "success message");
                VerifyAreEqual("Интернет-магазин для фотостудий",
                    Driver.FindElement(By.ClassName("lp-btn--primary")).Text.Trim(), "success button");
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("lp-btn--primary")).GetAttribute("href")
                        .IndexOf(defaultDomain + "?lid=") != -1, "success btn link");

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                VerifyIsTrue(Driver.Url.IndexOf(defaultDomain + "?lid=") != -1, "success btn link page url");

                ReInit();

                CheckLeadInAdmin(landingId, 1);
            }

            //check all pages of funnel mobile client - button, link WITHOUT menu
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count mobile (1)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count mobile (2)");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count mobile (3)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal(false, true, false);

                Driver.WaitForElem(By.ClassName("lp-block-text-thanks"));

                VerifyIsTrue(Driver.Url.IndexOf("/spasibo?lid=") != -1, "funnel thanks page client mobile");
                VerifyAreEqual("Проверьте почту, мы отправили Вам обещанный документ.",
                    Driver.FindElement(By.CssSelector(".lp-block-text-thanks__content p")).Text.Trim(),
                    "success message mobile");
                VerifyAreEqual("Интернет-магазин для фотостудий",
                    Driver.FindElement(By.ClassName("lp-btn--primary")).Text.Trim(), "success button mobile");
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("lp-btn--primary")).GetAttribute("href")
                        .IndexOf(defaultDomain + "?lid=") != -1, "success btn link mobile");

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                VerifyIsTrue(Driver.Url.IndexOf(defaultDomain + "?lid=") != -1, "success btn link page url mobile");

                ReInit();

                CheckLeadInAdmin(landingId, 2);
            }
        }

        [Test]
        public void ContactForEBookCheckFunctionality()
        {
            landingId = "ContactForEBook";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - button, link WITHOUT scroll to top button
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (1)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (2)");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (3)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal(name: false, phone: false);

                Driver.WaitForElem(By.ClassName("lp-block-contacts-map__header"));

                VerifyIsTrue(Driver.Url.IndexOf("/spasibo?lid=") != -1, "funnel thanks page client");
                VerifyAreEqual("На Ваш email отправлено письмо со ссылкой для скачивания бесплатной книги",
                    Driver.FindElement(By.CssSelector(".lp-h2")).Text.Trim(), "success message");

                ReInit();

                CheckLeadInAdmin(landingId, 1);
            }

            //check all pages of funnel mobile client - button, link WITHOUT menu
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count mobile (1)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count mobile (2)");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count mobile (3)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal(name: false, phone: false);

                Driver.WaitForElem(By.ClassName("lp-block-contacts-map__header"));

                VerifyIsTrue(Driver.Url.IndexOf("/spasibo?lid=") != -1, "funnel thanks page client mobile");
                VerifyAreEqual("На Ваш email отправлено письмо со ссылкой для скачивания бесплатной книги",
                    Driver.FindElement(By.CssSelector(".lp-h2")).Text.Trim(), "success message mobile");

                ReInit();

                CheckLeadInAdmin(landingId, 2);
            }
        }

        [Test]
        public void CouponWithDiscountCheckFunctionality()
        {
            landingId = "CouponWithDiscount";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - button, link WITHOUT scroll to top button
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (1)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (2)");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count (3)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal(false, true, false);

                Driver.WaitForElem(By.ClassName("lp-block-text-coupon__block"));

                VerifyIsTrue(Driver.Url.IndexOf("/spasibo?lid=") != -1, "funnel thanks page client");
                VerifyIsTrue(
                    Driver.FindElement(By.CssSelector(".lp-block-text-coupon__block")).Text.IndexOf("КУПОН НА 20%") !=
                    -1, "coupon messabe");
                VerifyAreEqual(defaultDomain,
                    Driver.FindElement(By.CssSelector(".lp-block-text-coupon__block a")).GetAttribute("href"),
                    "shop link");
                VerifyAreEqual("20-HERBAL-OFF",
                    Driver.FindElement(By.CssSelector(".lp-block-text-coupon__coupon")).Text.Trim(), "coupon code");
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("lp-btn--primary")).GetAttribute("href")
                        .IndexOf(defaultDomain + "?lid=") != -1, "success btn link");

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                VerifyIsTrue(Driver.Url.IndexOf(defaultDomain + "?lid=") != -1, "success btn link page url");

                ReInit();

                CheckLeadInAdmin(landingId, 1);
            }

            //check all pages of funnel mobile client - button, link WITHOUT menu
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count mobile (1)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count mobile (2)");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ng-hide.adv-modal")).Count,
                    "hidden modal count mobile (3)");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(500);
                FillFunnelFormModal(false, true, false);

                Driver.WaitForElem(By.ClassName("lp-block-text-coupon__block"));

                VerifyIsTrue(Driver.Url.IndexOf("/spasibo?lid=") != -1, "funnel thanks page client");
                VerifyIsTrue(
                    Driver.FindElement(By.CssSelector(".lp-block-text-coupon__block")).Text.IndexOf("КУПОН НА 20%") !=
                    -1, "coupon messabe");
                VerifyAreEqual(defaultDomain,
                    Driver.FindElement(By.CssSelector(".lp-block-text-coupon__block a")).GetAttribute("href"),
                    "shop link");
                VerifyAreEqual("20-HERBAL-OFF",
                    Driver.FindElement(By.CssSelector(".lp-block-text-coupon__coupon")).Text.Trim(), "coupon code");
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("lp-btn--primary")).GetAttribute("href")
                        .IndexOf(defaultDomain + "?lid=") != -1, "success btn link");

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                VerifyIsTrue(Driver.Url.IndexOf(defaultDomain + "?lid=") != -1, "success btn link page url");

                ReInit();

                CheckLeadInAdmin(landingId, 2);
            }
        }

        [Test]
        public void LotteryCheckFunctionality()
        {
            landingId = "Lottery";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first button, first downsell WITHOUT scroll to top button
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                VerifyAreEqual(0, ScrollPosition(), "position before scroll to conditions");
                Driver.FindElement(By.ClassName("lp-btn--secondary")).Click();
                Thread.Sleep(500);
                VerifyAreNotEqual(0, ScrollPosition(), "position after btn click(1)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                Driver.ScrollToTop();

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);
                FillFunnelFormModal();

                Driver.WaitForElem(By.ClassName("lp-block-text-single"));

                VerifyIsTrue(Driver.Url.IndexOf("/spasibo?lid=") != -1, "funnel thanks page client");
                VerifyAreEqual("Ваша заявка принята",
                    Driver.FindElement(By.CssSelector(".lp-h2")).Text.Trim(), "success message");
                VerifyAreEqual("Перейти в Интернет-магазин",
                    Driver.FindElement(By.ClassName("lp-btn--primary")).Text.Trim(), "success button");
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("lp-btn--primary")).GetAttribute("href")
                        .IndexOf(defaultDomain + "?lid=") != -1, "success btn link");

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                VerifyIsTrue(Driver.Url.IndexOf(defaultDomain + "?lid=") != -1, "success btn link page url");

                CheckLeadInAdmin(landingId, 1);
            }

            //check all pages of funnel mobile client - second button, second downsell WITHOUT menu
            {
                ReInitClient();

                GoToMobile("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");
                VerifyAreEqual(0, ScrollPosition(), "position before scroll to conditions mobile");
                Driver.FindElement(By.ClassName("lp-btn--secondary")).Click();
                Thread.Sleep(500);
                VerifyAreNotEqual(0, ScrollPosition(), "position after btn click(1) mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");
                Driver.ScrollToTop();

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);
                FillFunnelFormModal();

                Driver.WaitForElem(By.ClassName("lp-block-text-single"));

                VerifyIsTrue(Driver.Url.IndexOf("/spasibo?lid=") != -1, "funnel thanks page client mobile");
                VerifyAreEqual("Ваша заявка принята",
                    Driver.FindElement(By.CssSelector(".lp-h2")).Text.Trim(), "success message mobile");
                VerifyAreEqual("Перейти в Интернет-магазин",
                    Driver.FindElement(By.ClassName("lp-btn--primary")).Text.Trim(), "success button mobile");
                VerifyIsTrue(
                    Driver.FindElement(By.ClassName("lp-btn--primary")).GetAttribute("href")
                        .IndexOf(defaultDomain + "?lid=") != -1, "success btn link mobile");

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                VerifyIsTrue(Driver.Url.IndexOf(defaultDomain + "?lid=") != -1, "success btn link page url mobile");

                CheckLeadInAdmin(landingId, 2);
            }
        }

        [Test]
        public void VideoLeadMagnetNewCheckFunctionality()
        {
            landingId = "LandingFunnelOrderWithForm";
            By waitElem = By.CssSelector(".lp-h2");

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - button, link WITHOUT scroll to top button
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                Driver.FindElement(By.ClassName("video-view")).Click();
                Thread.Sleep(2000);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("adv-modal-active")).Count, "modal window");
                VerifyIsFalse(VideoModalIsEmpty(), "video html at modal window");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                FillFunnelForm(false, true, false);

                Driver.WaitForElem(waitElem);

                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?lid=") != -1, "funnel thanks page client");
                Driver.FindElement(By.ClassName("video-view")).Click();
                Thread.Sleep(2000);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("adv-modal-active")).Count,
                    "modal window thanks page");
                VerifyIsFalse(VideoModalIsEmpty(), "video html at modal window thanks page");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console client");

                ReInit();

                CheckLeadInAdmin(landingId, 1);
            }

            //check all pages of funnel mobile client - button, link WITHOUT menu
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-menu-header-container--open")).Count,
                    "shown burger menu");
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel after burgerMenu mobile");

                Driver.FindElement(By.ClassName("video-view")).Click();
                Thread.Sleep(2000);
                //VerifyAreEqual(1, driver.FindElements(By.ClassName("adv-modal-active")).Count, "modal window");
                //VerifyIsFalse(VideoModalIsEmpty(), "video html at modal window");
                //driver.FindElement(By.ClassName("adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                FillFunnelForm(false, true, false);

                Driver.WaitForElem(waitElem);

                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?lid=") != -1,
                    "funnel thanks page client mobile");
                Driver.FindElement(By.ClassName("video-view")).Click();
                Thread.Sleep(2000);
                //VerifyAreEqual(1, driver.FindElements(By.ClassName("adv-modal-active")).Count, "modal window thanks page mobile");
                //VerifyIsFalse(VideoModalIsEmpty(), "video html at modal window thanks page mobile");
                //driver.FindElement(By.ClassName("adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console client mobile");

                ReInit();

                CheckLeadInAdmin(landingId, 2);
            }
        }

        #endregion

        protected bool VideoModalIsEmpty()
        {
            return string.IsNullOrEmpty(Driver.FindElement(By.ClassName("adv-modal-active"))
                .FindElement(By.ClassName("iframe-responsive__container")).GetAttribute("innerHTML"));
        }

        protected void FillQuestionary(string name)
        {
            Driver.FindElements(By.CssSelector(".lp-form__field input"))[0].SendKeys(name);
            Driver.FindElements(By.CssSelector(".lp-form__field input"))[1].SendKeys("79012345678");
            Driver.FindElements(By.CssSelector(".lp-form__field input"))[2].SendKeys("Question 1 text");
            Driver.FindElements(By.CssSelector(".lp-form__field textarea"))[0].SendKeys("Question 2 text");
            Driver.FindElements(By.CssSelector(".lp-form__field textarea"))[1].SendKeys("Question 3 text");

            Driver.FindElement(By.CssSelector(".lp-form__agreement label")).Click();
            Driver.FindElement(AdvBy.DataE2E("FormBtn")).Click();
            Thread.Sleep(1000);
        }

        protected void CheckLeadQuestionaryDescription(string name)
        {
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(By.TagName("simple-edit"));
            VerifyIsTrue(Driver.FindElement(By.Name("Lead.Description")).Text.IndexOf("Имя: " + name) != -1,
                "questionary name");
            VerifyIsTrue(Driver.FindElement(By.Name("Lead.Description")).Text.IndexOf("Телефон: 79012345678") != -1,
                "questionary phone");
            VerifyIsTrue(Driver.FindElement(By.Name("Lead.Description")).Text.IndexOf("Адрес: Question 1 text") != -1,
                "questionary adress");
            VerifyIsTrue(
                Driver.FindElement(By.Name("Lead.Description")).Text.IndexOf("Комментарий: Question 2 text") != -1,
                "questionary comment");
            VerifyIsTrue(
                Driver.FindElement(By.Name("Lead.Description")).Text.IndexOf("Многострочный текст: Question 3 text") !=
                -1, "questionary textarea");
            Driver.FindElement(By.ClassName("lead-info-close")).Click();
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