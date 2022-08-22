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
    public class ProductFunnelsCreateTest : MySitesFunctions
    {
        string landingId;
        string landingName;
        string landingTab = "Товарные воронки";

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

            //какую еще проверку текущей воронки добавить?
            //повторить для мобилки

            //прокликать все кнопки, заполнить все формы
            //для мобилки проверять меню в шапке!
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
        public void ArticleWithCrossSellsCreate()
        {
            landingId = "ArticleWithCrossSells";
            landingName = "Воронка \"Продающая статья с допродажами\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts("TestProduct1", "TestProduct2", "TestProduct3");
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "upsell");
                CheckFunnelPageInplaceEnabled(2, waitElem, landingId, "downsell");
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
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console log");

                GoToFunnelPageFromLp(2, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console log");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void CompanySiteCourseCreate()
        {
            landingId = "CompanySiteCourse";
            landingName = "Воронка \"Онлайн-школа\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "thanks page");
                CheckFunnelPageInplaceEnabled(2, By.ClassName("columns-menu__item"), landingId, "lk page");
                CheckFunnelPageInplaceEnabled(3, waitElem, landingId, "example page");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(8, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons primary count");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--secondary")).Count,
                    "buttons senondary count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel thanks page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel thanks page console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons primary count");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--secondary")).Count,
                    "buttons senondary count");

                GoToFunnelPageFromLp(2, By.ClassName("columns-menu__item"));
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel lk page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel lk page console log");
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons primary count");

                GoToFunnelPageFromLp(3, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel example page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel example page console log");
                VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons primary count");


                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void LandingFunnelOrderFreeWithDeliveryCreate()
        {
            landingId = "LandingFunnelOrderFreeWithDelivery";
            landingName = "Воронка \"Бесплатный товар + доставка\"";
            By waitElem = By.CssSelector(".lp-header-logo img");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts("TestProduct1", "TestProduct2", "TestProduct3");
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "upsell");
                CheckFunnelPageInplaceEnabled(2, waitElem, landingId, "downsell");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console log");

                GoToFunnelPageFromLp(2, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console log");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void LandingFunnelOrderWithDetailsCreate()
        {
            landingId = "LandingFunnelOrderWithDetails";
            landingName = "Воронка \"Один товар с допродажами. Детально\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts("TestProduct1", "TestProduct2", "TestProduct3");
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "upsell");
                CheckFunnelPageInplaceEnabled(2, waitElem, landingId, "downsell");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console log");

                GoToFunnelPageFromLp(2, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console log");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void LandingFunnelOrderWithFormCreate()
        {
            landingId = "LandingFunnelOrderWithForm";
            landingName = "Воронка \"Один товар с допродажами\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts("TestProduct1", "TestProduct2", "TestProduct3");
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "upsell");
                CheckFunnelPageInplaceEnabled(2, waitElem, landingId, "downsell");
                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status inplace");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log inplace");
            }

            //inplaceDisabled + buttons count
            {
                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
                Driver.WaitForElem(waitElem);

                GoToFunnelPageFromLp(0, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");
                VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "buttons count");

                GoToFunnelPageFromLp(1, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console log");

                GoToFunnelPageFromLp(2, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console log");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void MultyProductsCreate()
        {
            landingId = "MultyProducts";
            landingName = "Воронка \"Мультитоварная\"";
            By waitElem = By.CssSelector(".lp-h1");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts(new List<string>
                {"TestProduct1", "TestProduct2", "TestProduct3", "TestProduct4", "TestProduct5", "TestProduct9"});
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
                VerifyAreEqual(8, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons count");
                VerifyAreEqual(2, Driver.FindElements(By.ClassName("block-type-productsView")).Count,
                    "productsByCategories count");
                VerifyAreEqual("СВЕТОДИОДНЫЕ ЛЮСТРЫ", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(),
                    "main page header");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void MultyProductsCreate2()
        {
            landingId = "MultyProducts2";
            landingName = "Воронка \"Мультитоварная\"";
            By waitElem = By.CssSelector(".lp-h1");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            Thread.Sleep(500);
            Driver.FindElements(By.ClassName("adv-radio-label"))[1].Click();

            SetFunnelCategories(new List<string> { "TestCategory3", "TestCategory4" });
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
                VerifyAreEqual(11, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons count");
                VerifyAreEqual(2, Driver.FindElements(By.ClassName("block-type-productsByCategories")).Count,
                    "productsByCategories count");
                VerifyAreEqual("СВЕТОДИОДНЫЕ ЛЮСТРЫ", Driver.FindElement(By.ClassName("lp-h1")).Text.Trim(),
                    "main page header");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void MultyProductsWithCategoriesCreate()
        {
            landingId = "MultyProductsWithCategories";
            landingName = "Воронка \"Мультитоварная с категориями\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelCategories(new List<string> { "TestCategory3", "TestCategory4" });
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
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons count");
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("block-type-productsByCategories")).Count,
                    "productsByCategories count");
                VerifyAreEqual(2, Driver.FindElements(By.ClassName("category-name-button")).Count,
                    "category name links count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void MultyProductsShortCreate()
        {
            landingId = "MultyProductsShort";
            landingName = "Воронка \"Мультитоварная упрощенная\"";
            By waitElem = By.CssSelector(".lp-h1");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(3, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts(new List<string>
            {
                "TestProduct1", "TestProduct2", "TestProduct3", "TestProduct9", "TestProduct10", "TestProduct11",
                "TestProduct12", "TestProduct13"
            });
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
                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary")).Count,
                    "primary buttons count");
                VerifyAreEqual(3, Driver.FindElements(By.ClassName("lp-block-products-view")).Count,
                    "products container blocks count");
                VerifyAreEqual(2, Driver.FindElements(By.ClassName("lp-block-products-view--one-big-picture")).Count,
                    "big photo blocks count");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        [Test]
        public void VideoWithCrossSellsCreate()
        {
            landingId = "VideoWithCrossSells";
            landingName = "Воронка \"Видеопредложение с допродажами\"";
            By waitElem = By.CssSelector(".lp-h2");

            GoToCreateFunnel(landingTab, landingName);
            VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".screenshot-item")).Count, "funnel pages count");
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts("TestProduct1", "TestProduct2", "TestProduct3");
            SetFunnelName(landingId);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            //inplaceEnabled
            {
                CheckFunnelPageInplaceEnabled(0, waitElem, landingId, "main page");
                CheckFunnelPageInplaceEnabled(1, waitElem, landingId, "upsell");
                CheckFunnelPageInplaceEnabled(2, waitElem, landingId, "downsell");
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
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console log");

                GoToFunnelPageFromLp(2, waitElem);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console log");

                Driver.FindElement(By.CssSelector(".lp-admin-panel-link.icon-lp-eye")).Click();
            }
        }

        #endregion
    }


    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class ProductFunnelsCheckFunctionalityTest : MySitesFunctions
    {
        string landingId;
        string orderNum;

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
                "data\\Admin\\Funnel\\FromTemplate\\ProductFunnels\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\FromTemplate\\ProductFunnels\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\FromTemplate\\ProductFunnels\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\FromTemplate\\ProductFunnels\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\FromTemplate\\ProductFunnels\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\FromTemplate\\ProductFunnels\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\FromTemplate\\ProductFunnels\\CMS.LandingSubBlock.csv"
            );

            Init(false);

            GoToAdmin("leads?salesFunnelId=1");
            Driver.FindElement(AdvBy.DataE2E("UseGrid")).Click();
            Thread.Sleep(1000);

            //какую еще проверку текущей воронки добавить?
            //повторить для мобилки

            //прокликать все кнопки, заполнить все формы
            //для мобилки проверять меню в шапке!
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
        public void ArticleWithCrossSellsCheck()
        {
            landingId = "ArticleWithCrossSells";
            By waitElem = By.CssSelector(".lp-h2");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts("TestProduct1", "TestProduct2", "TestProduct3");
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first button, first downsell WITHOUT scroll to top button - short page
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/upsell?code=") != -1,
                    "funnel upsell page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console client");
                Driver.FindElement(By.ClassName("lp-btn--secondary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/downsell?code=") != -1,
                    "funnel downsell page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console client");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3" });
            }

            //check all pages of funnel mobile client - second button, second downsell WITHOUT menu
            {
                ReInitClient();

                GoToMobile("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client mobile");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/upsell?code=") != -1,
                    "funnel upsell page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console client mobile");
                Driver.FindElements(By.ClassName("lp-btn--secondary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/downsell?code=") != -1,
                    "funnel downsell page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console client mobile");
                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client mobile");

                orderNum = GetOrderNum(true);
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3" });
            }

            //check all pages of funnel client - third button, first uppsell
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                FillCheckoutForm();

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client(3)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client(3)");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client(3)");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct2" });
            }

            //check all pages of funnel client - first button, second uppsell
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                FillCheckoutForm();

                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client(4)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client(4)");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client(4)");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct2" });
            }
        }

        [Test]
        public void CompanySiteCourseCheckNOTCOMPLETE()
        {
            landingId = "CompanySiteCourse";
            By waitElem = By.CssSelector(".lp-h2");

            VerifyIsTrue(false, "test not work");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //check first page, all buttons + scroll
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());

                Driver.ScrollTo(By.ClassName("lp-footer-social"));
                Driver.FindElement(By.ClassName("scroll-to-top-active")).Click();
                VerifyAreEqual(0, ScrollPosition(), "position after scroll button click");

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");

                Driver.ScrollTo(AdvBy.DataE2E("formSubscribe_container"));
                FillFunnelForm(true, false, true, false, false);
                VerifyAreEqual("Спасибо за заявку! С Вами свяжется наш менеджер!",
                    Driver.FindElement(By.ClassName("lp-form__content--success")).Text.Trim(),
                    "form consultation success");
                Driver.ScrollToTop();

                string currentUrl = Driver.Url;
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[0].Click();
                VerifyAreNotEqual(0, ScrollPosition(), "position after btn click(1)");
                VerifyAreEqual(currentUrl, Driver.Url, "none url tail after btn click(1)");

                Driver.ScrollToTop();
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[1].Click();
                VerifyAreNotEqual(0, ScrollPosition(), "position after btn click(2)");
                VerifyAreEqual(currentUrl, Driver.Url, "none url tail after btn click(2)");

                string loginUrl = BaseUrl + "lp/" + landingId + "/kabinet-kursa";
                VerifyAreEqual(loginUrl.ToLower(),
                    Driver.FindElement(By.ClassName("lp-btn--secondary")).GetAttribute("href"), "lk link (1)");
                VerifyAreEqual("_blank", Driver.FindElement(By.ClassName("lp-btn--secondary")).GetAttribute("target"),
                    "lk link target (1)");

                //TODO
                VerifyIsTrue(false, "from here not ready");

                Driver.FindElement(By.ClassName("lp-btn--secondary")).Click();
                Thread.Sleep(1000);
                Functions.OpenNewTab(Driver, BaseUrl);
                VerifyIsTrue(Driver.Url.IndexOf("/kabinet-kursa") != -1, "lk page");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel lk page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel lk page console client");

                VerifyAreEqual(24, Driver.FindElements(By.ClassName("columns-menu__list-link")).Count, "links count");
                Driver.FindElement(By.ClassName("columns-menu__list-link")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/primer-vnutrennei-stranitsy") != -1, "expample page");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel expample page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel expample page console client");

                VerifyAreEqual(Driver.Url,
                    Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).GetAttribute("href"),
                    "next lesson link");
                Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")).Click();
                Thread.Sleep(1000);
                VerifyIsTrue(Driver.Url.IndexOf("/primer-vnutrennei-stranitsy") != -1, "expample page(2)");

                //три кнопки с ценой
                GoToClient("lp/" + landingId.ToLower());

                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".adv-modal.ng-hide")).Count,
                    "hidden modal count(1)");
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[2].Click();
                Thread.Sleep(500);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".adv-modal.ng-hide")).Count,
                    "hidden modal count(2)");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client(1)");

                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".adv-modal.ng-hide")).Count,
                    "hidden modal count(1)");
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[3].Click();
                Thread.Sleep(500);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".adv-modal.ng-hide")).Count,
                    "hidden modal count(2)");
                Driver.FindElement(By.ClassName("adv-modal-close")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client(2)");

                VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".adv-modal.ng-hide")).Count,
                    "hidden modal count(1)");
                Driver.FindElements(By.CssSelector(".lp-btn.lp-btn--primary"))[4].Click();
                Thread.Sleep(500);
                VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".adv-modal.ng-hide")).Count,
                    "hidden modal count(2)");
                FillFunnelFormModal();

                VerifyIsTrue(Driver.Url.IndexOf("/stranitsa-blagodarnosti?lid") != -1, "success page");
                VerifyAreEqual(loginUrl, Driver.FindElement(By.ClassName("lp-btn--secondary")).GetAttribute("href"),
                    "lk link (2)");
                VerifyAreEqual("_blank", Driver.FindElement(By.ClassName("lp-btn--secondary")).GetAttribute("target"),
                    "lk link target (2)");

                Driver.FindElement(By.ClassName("lp-btn--secondary")).Click();
                Thread.Sleep(1000);
                Functions.OpenNewTab(Driver, BaseUrl);
                VerifyIsTrue(Driver.Url.IndexOf("/kabinet-kursa") != -1, "lk page");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel lk page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel lk page console client");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-menu-header-container--open")).Count,
                    "show burger menu (1)");
                Driver.FindElements(By.ClassName("lp-menu-header__link"))[1].Click();
                Driver.FindElement(By.ClassName("lp-menu-header__submenu-link")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/primer-vnutrennei-stranitsy") != -1, "expample page(1.1)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel expample page status client(1.1)");
                VerifyIsNull(CheckConsoleLog(true), "funnel expample page console client(1.1)");
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                Driver.FindElements(By.ClassName("lp-menu-header__link"))[0].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/kabinet-kursa") != -1, "lk page(2.1)");

                ReInit();
                CheckLeadInAdmin(landingId, 2);
            }

            //for mobile
            {
            }
        }

        [Test]
        public void LandingFunnelOrderFreeWithDeliveryCheck()
        {
            landingId = "LandingFunnelOrderFreeWithDelivery";
            By waitElem = By.CssSelector(".lp-header-logo img");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts("TestProduct1", "TestProduct2", "TestProduct3");
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first button, first downsell + scroll to top button
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());

                Driver.ScrollTo(By.ClassName("lp-block-footer"));
                Driver.FindElement(By.ClassName("scroll-to-top-active")).Click();
                VerifyAreEqual(0, ScrollPosition(), "position after scroll button click");

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/upsell?code=") != -1,
                    "funnel upsell page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console client");
                Driver.FindElement(By.ClassName("lp-btn--secondary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/downsell?code=") != -1,
                    "funnel downsell page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console client");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3" });
            }

            //check all pages of funnel mobile client - second button, second downsell + menu
            {
                ReInitClient();

                GoToMobile("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-menu-header-container--open")).Count,
                    "shown burger menu");
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel after burgerMenu mobile");

                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client mobile");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/upsell?code=") != -1,
                    "funnel upsell page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console client mobile");
                Driver.FindElements(By.ClassName("lp-btn--secondary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/downsell?code=") != -1,
                    "funnel downsell page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console client mobile");
                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client mobile");

                orderNum = GetOrderNum(true);
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3" });
            }

            //check all pages of funnel client - third button, first uppsell
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                FillCheckoutForm();

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client(3)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client(3)");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client(3)");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct2" });
            }

            //check all pages of funnel client - first button, second uppsell
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                FillCheckoutForm();

                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client(4)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client(4)");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client(4)");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct2" });
            }
        }

        [Test]
        public void LandingFunnelOrderWithDetailsCheck()
        {
            landingId = "LandingFunnelOrderWithDetails";
            By waitElem = By.CssSelector(".lp-h2");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts("TestProduct1", "TestProduct2", "TestProduct3");
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first button, first downsell + scroll to top button
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());

                Driver.ScrollTo(By.ClassName("lp-block-columns-one-icons"));
                Driver.FindElement(By.ClassName("scroll-to-top-active")).Click();
                VerifyAreEqual(0, ScrollPosition(), "position after scroll button click");

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                Driver.FindElement(AdvBy.DataE2E("textCenterBtn1")).FindElement(By.ClassName("lp-btn--primary"))
                    .Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/upsell?code=") != -1,
                    "funnel upsell page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console client");
                Driver.FindElement(By.ClassName("lp-btn--secondary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/downsell?code=") != -1,
                    "funnel downsell page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console client");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3" });
            }

            //check all pages of funnel mobile client - second button, second downsell + menu
            {
                ReInitClient();

                GoToMobile("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-menu-header-container--open")).Count,
                    "shown burger menu");
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel after burgerMenu mobile");

                Driver.FindElement(AdvBy.DataE2E("CoversBtn")).FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client mobile");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/upsell?code=") != -1,
                    "funnel upsell page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console client mobile");
                Driver.FindElements(By.ClassName("lp-btn--secondary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/downsell?code=") != -1,
                    "funnel downsell page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console client mobile");
                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client mobile");

                orderNum = GetOrderNum(true);
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3" });
            }

            //check all pages of funnel client - third button, first uppsell
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElements(AdvBy.DataE2E("textCenterBtn1"))[1].FindElement(By.ClassName("lp-btn--primary"))
                    .Click();
                Thread.Sleep(1000);

                FillCheckoutForm();

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client(3)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client(3)");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client(3)");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct2" });
            }

            //check all pages of funnel client - first button, second uppsell
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElement(AdvBy.DataE2E("textCenterBtn1")).FindElement(By.ClassName("lp-btn--primary"))
                    .Click();
                Thread.Sleep(1000);

                FillCheckoutForm();

                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client(4)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client(4)");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client(4)");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct2" });
            }
        }

        [Test]
        public void LandingFunnelOrderWithFormCheck()
        {
            landingId = "LandingFunnelOrderWithForm";
            By waitElem = By.CssSelector(".lp-h2");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts("TestProduct1", "TestProduct2", "TestProduct3");
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first button, first downsell + scroll to top button
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());

                Driver.ScrollTo(By.ClassName("lp-block-footer"));
                Driver.FindElement(By.ClassName("scroll-to-top-active")).Click();
                VerifyAreEqual(0, ScrollPosition(), "position after scroll button click");

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/upsell?code=") != -1,
                    "funnel upsell page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console client");
                Driver.FindElement(By.ClassName("lp-btn--secondary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/downsell?code=") != -1,
                    "funnel downsell page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console client");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3" });
            }

            //check all pages of funnel mobile client - second button, second downsell + menu
            {
                ReInitClient();

                GoToMobile("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-menu-header-container--open")).Count,
                    "shown burger menu");
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel after burgerMenu mobile");

                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client mobile");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/upsell?code=") != -1,
                    "funnel upsell page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console client mobile");
                Driver.FindElements(By.ClassName("lp-btn--secondary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/downsell?code=") != -1,
                    "funnel downsell page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console client mobile");
                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client mobile");

                orderNum = GetOrderNum(true);
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3" });
            }

            //check all pages of funnel client - first button, first uppsell
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                FillCheckoutForm();

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client(3)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client(3)");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client(3)");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct2" });
            }

            //check all pages of funnel client - first button, second uppsell
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                FillCheckoutForm();

                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client(4)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client(4)");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client(4)");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct2" });
            }
        }

        [Test]
        public void MultyProductsCheck()
        {
            landingId = "MultyProducts";
            By waitElem = By.CssSelector(".lp-h1");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts(new List<string>
                {"TestProduct1", "TestProduct2", "TestProduct3", "TestProduct4", "TestProduct5", "TestProduct9"});
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - scroll to top, 2 scroll buttons, quick view, one product from quickview, show cart spinbox more, second product
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                CheckScrollToTop(By.ClassName("lp-block-contacts-center-simple"));

                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")));
                CheckScrollingFromTop(Driver.FindElement(By.ClassName("scroll-to-block-trigger")));

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[0]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .quickview-arrows-next")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after quickview");

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[0]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Driver.FindElement(By.CssSelector(".lp-product-info__payment-item  .lp-btn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after add to cart");

                Driver.FindElement(By.ClassName("lp-cart-trigger")).Click();
                Thread.Sleep(1000);
                string cartResult = Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim();
                VerifyAreEqual("18 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price before spinbox more");
                Driver.FindElement(By.ClassName("spinbox-more")).Click();
                Driver.FindElement(By.ClassName("spinbox-more")).Click();
                VerifyAreEqual("54 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price after spinbox more");
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[2]
                    .FindElement(By.ClassName("lp-btn--primary")).Click();
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
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3", "TestProduct2", });
            }

            //mobile client - menu links, 2 scroll buttons, quick view, one product from quickview, show cart spinbox more, second product
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                CheckBurgerMenuLink(0);
                CheckBurgerMenuLink(1);
                CheckBurgerMenuLink(2);
                VerifyIsNull(CheckConsoleLog(true), "funnel after burgerMenu mobile");

                Driver.ScrollToTop();
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")));
                CheckScrollingFromTop(Driver.FindElement(By.ClassName("scroll-to-block-trigger")));

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[1]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .quickview-arrows-next")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after quickview");

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[1]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Driver.FindElement(By.CssSelector(".lp-product-info__payment-item  .lp-btn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".lp-btn--secondary")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after add to cart");

                Driver.FindElement(By.ClassName("lp-cart-trigger")).Click();
                Thread.Sleep(1000);
                string cartResult = Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim();
                VerifyAreEqual("32 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price before spinbox more");
                Driver.FindElement(By.ClassName("spinbox-more")).Click();
                Driver.FindElement(By.ClassName("spinbox-more")).Click();
                VerifyAreEqual("96 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price after spinbox more");
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[4]
                    .FindElement(By.ClassName("lp-btn--primary")).Click();
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
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct2", "TestProduct5", "TestProduct9" });
            }
        }

        [Test]
        public void MultyProductsCheck2()
        {
            landingId = "MultyProducts2";
            By waitElem = By.CssSelector(".lp-h1");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            Thread.Sleep(500);
            Driver.FindElements(By.ClassName("adv-radio-label"))[1].Click();

            SetFunnelCategories(new List<string> { "TestCategory3", "TestCategory4" });
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());
            
            //check all pages of funnel client - scroll to top, 2 scroll buttons, quick view, one product from quickview, show cart spinbox more, second product
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                CheckScrollToTop(By.ClassName("lp-block-contacts-center-simple"));

                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")));
                CheckScrollingFromTop(Driver.FindElement(By.ClassName("scroll-to-block-trigger")));

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[0]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .quickview-arrows-next")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after quickview");

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[0]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Driver.FindElement(By.CssSelector(".lp-product-info__payment-item  .lp-btn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after add to cart");

                Driver.FindElement(By.ClassName("lp-cart-trigger")).Click();
                Thread.Sleep(1000);
                string cartResult = Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim();
                VerifyAreEqual("220 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price before spinbox more");
                Driver.FindElement(By.ClassName("spinbox-more")).Click();
                VerifyAreEqual("440 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price after spinbox more");
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[2]
                    .FindElement(By.ClassName("lp-btn--primary")).Click();
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
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct11", "TestProduct13" });
            }

            //mobile client - menu links, 2 scroll buttons, quick view, one product from quickview, show cart spinbox more, second product
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                CheckBurgerMenuLink(0);
                CheckBurgerMenuLink(1);
                CheckBurgerMenuLink(2);
                VerifyIsNull(CheckConsoleLog(true), "funnel after burgerMenu mobile");

                Driver.ScrollToTop();
                CheckScrollingFromTop(Driver.FindElement(By.CssSelector(".lp-btn.lp-btn--primary")));
                CheckScrollingFromTop(Driver.FindElement(By.ClassName("scroll-to-block-trigger")));

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[1]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .quickview-arrows-next")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after quickview");

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[1]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Driver.FindElement(By.CssSelector(".lp-product-info__payment-item  .lp-btn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".lp-btn--secondary")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after add to cart");

                Driver.FindElement(By.ClassName("lp-cart-trigger")).Click();
                Thread.Sleep(1000);
                string cartResult = Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim();
                VerifyAreEqual("240 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price before spinbox more");
                Driver.FindElement(By.ClassName("spinbox-more")).Click();
                VerifyAreEqual("480 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price after spinbox more");
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[4]
                    .FindElement(By.ClassName("lp-btn--primary")).Click();
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
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct12", "TestProduct15" });
            }
        }

        [Test]
        public void MultyProductsWithCategoriesCheck()
        {
            landingId = "MultyProductsWithCategories";
            By waitElem = By.CssSelector(".lp-h2");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelCategories(new List<string> { "TestCategory3", "TestCategory4" });
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - scroll to top, category tab click, quick view, one product from quickview, show cart spinbox more, oth category - second product
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                CheckScrollToTop(By.ClassName("lp-block-contacts-center-simple"));

                VerifyAreEqual(5,
                    Driver.FindElements(By.ClassName("lp-products-view-by-categories-item-wrapper")).Count,
                    "1st category products count");
                Driver.FindElements(By.ClassName("category-name-button"))[1].Click();
                Thread.Sleep(500);
                VerifyAreEqual(3,
                    Driver.FindElements(By.ClassName("lp-products-view-by-categories-item-wrapper")).Count,
                    "1st category products count");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log change active category");

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[0]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .quickview-arrows-next")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after quickview");

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

            //mobile client - category links, quick view, one product from quickview, show cart spinbox more, second product
            {
                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                VerifyAreEqual(5,
                    Driver.FindElements(By.ClassName("lp-products-view-by-categories-item-wrapper")).Count,
                    "1st category products count");
                Driver.FindElements(By.ClassName("category-name-button"))[1].Click();
                Thread.Sleep(500);
                VerifyAreEqual(3,
                    Driver.FindElements(By.ClassName("lp-products-view-by-categories-item-wrapper")).Count,
                    "1st category products count");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log change active category");

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[1]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .quickview-arrows-next")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after quickview");

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[1]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Driver.FindElement(By.CssSelector(".lp-product-info__payment-item  .lp-btn")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".lp-btn--secondary")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after add to cart");

                Driver.FindElement(By.ClassName("lp-cart-trigger")).Click();
                Thread.Sleep(1000);
                string cartResult = Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim();
                VerifyAreEqual("340 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price before spinbox more");
                Driver.FindElement(By.ClassName("spinbox-more")).Click();
                VerifyAreEqual("680 руб.", Driver.FindElements(By.ClassName("lp-cart-result__value"))[1].Text.Trim(),
                    "cart price after spinbox more");
                Driver.FindElement(By.CssSelector(".lp-cart-modal .adv-modal-close")).Click();
                Thread.Sleep(500);

                Driver.FindElements(By.ClassName("category-name-button"))[0].Click();
                Thread.Sleep(500);
                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[4]
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
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct17", "TestProduct15" });
            }
        }

        [Test]
        public void MultyProductsShortCheck()
        {
            landingId = "MultyProductsShort";
            By waitElem = By.CssSelector(".lp-h1");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts(new List<string>
            {
                "TestProduct1", "TestProduct2", "TestProduct3", "TestProduct9", "TestProduct10", "TestProduct11",
                "TestProduct12", "TestProduct13"
            });
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - scroll to top, quick view, one product from quickview, show cart spinbox more, oth category - second product
            {
                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console log");

                CheckScrollToTop(By.ClassName("lp-footer-social"));

                Driver.FindElements(By.CssSelector(".lp-block-products-view--one-big-picture"))[0]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Driver.FindElement(By.CssSelector(".lp-product-info__payment-item  .lp-btn")).Click();
                Thread.Sleep(500);
                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success/") != -1 && Driver.Url.IndexOf("?mode=lp") != -1,
                    "funnel success client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1" });


                ReInitClient();
                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[1]
                    .FindElement(By.ClassName("lp-products-view-item-link-wrap")).Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .quickview-arrows-next")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after quickview");

                Thread.Sleep(500);
                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[2]
                    .FindElement(By.ClassName("lp-products-view-item-link-wrap")).Click();
                Driver.FindElements(By.CssSelector(".lp-product-info__payment-item  .lp-btn"))[1].Click();
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
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct3", "TestProduct2" });
            }

            //mobile client - two product from quickview by button + menu
            {
                ReInitClient();

                GoToMobile("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");

                Driver.FindElement(AdvBy.DataE2E("BurgerMenuBtn")).Click();
                Thread.Sleep(500);
                VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-menu-header-container--open")).Count,
                    "shown burger menu");
                Driver.FindElement(AdvBy.DataE2E("BurgerMenuClose")).Click();
                Thread.Sleep(500);
                VerifyIsNull(CheckConsoleLog(true), "funnel after burgerMenu mobile");

                Driver.ScrollTo(By.ClassName("lp-block-products-view--one-big-picture"), 1);

                Driver.FindElements(By.CssSelector(".lp-block-products-view--one-big-picture"))[1]
                    .FindElement(By.ClassName("lp-products-view-item-photo")).Click();
                Driver.FindElement(By.CssSelector(".lp-product-info__payment-item  .lp-btn")).Click();
                Thread.Sleep(500);
                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success/") != -1 && Driver.Url.IndexOf("?mode=lp") != -1,
                    "funnel success client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client");

                orderNum = GetOrderNum(true);
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct13" });

                ReInitClient();
                GoToMobile("lp/" + landingId.ToLower());

                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[2]
                    .FindElement(By.ClassName("lp-products-view-item-link-wrap")).Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .quickview-arrows-next")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".adv-modal:not(.ng-hide) .adv-modal-close")).Click();
                VerifyIsNull(CheckConsoleLog(true), "funnel console after quickview");

                Thread.Sleep(500);
                Driver.FindElements(By.CssSelector(".lp-products-view-item"))[3]
                    .FindElement(By.ClassName("lp-products-view-item-link-wrap")).Click();
                Driver.FindElements(By.CssSelector(".lp-product-info__payment-item  .lp-btn"))[1].Click();
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
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct9" });
            }
        }

        [Test]
        public void VideoWithCrossSellsCheck()
        {
            landingId = "VideoWithCrossSells";
            By waitElem = By.CssSelector(".lp-h2");

            GoToAdmin("dashboard/createFunnel?id=" + landingId);
            Driver.FindElement(AdvBy.DataE2E("funnelCreateBtn")).Click();
            SetFunnelProducts("TestProduct1", "TestProduct2", "TestProduct3");
            SetFunnelName(landingId);

            GoToClient("lp/" + landingId.ToLower());

            //check all pages of funnel client - first button, first downsell WITHOUT scroll to top button - short page
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/upsell?code=") != -1,
                    "funnel upsell page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console client");
                Driver.FindElement(By.ClassName("lp-btn--secondary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/downsell?code=") != -1,
                    "funnel downsell page client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console client");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3" });
            }

            //check all pages of funnel mobile client - second button, second downsell WITHOUT menu
            {
                ReInitClient();

                GoToMobile("lp/" + landingId.ToLower());
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel main page status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel main page console client mobile");
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("/checkout/lp?") != -1, "funnel checkout page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel checkout status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel checkout console client mobile");
                FillCheckoutForm();

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/upsell?code=") != -1,
                    "funnel upsell page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel upsell status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel upsell console client mobile");
                Driver.FindElements(By.ClassName("lp-btn--secondary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf(landingId.ToLower() + "/downsell?code=") != -1,
                    "funnel downsell page client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel downsell status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel downsell console client mobile");
                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client mobile");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client mobile");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client mobile");

                orderNum = GetOrderNum(true);
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct3" });
            }

            //check all pages of funnel client - third button, first uppsell
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                FillCheckoutForm();

                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client(3)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client(3)");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client(3)");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct2" });
            }

            //check all pages of funnel client - first button, second uppsell
            {
                ReInitClient();

                GoToClient("lp/" + landingId.ToLower());
                Driver.FindElement(By.ClassName("lp-btn--primary")).Click();
                Thread.Sleep(1000);

                FillCheckoutForm();

                Driver.FindElements(By.ClassName("lp-btn--primary"))[1].Click();
                Thread.Sleep(1000);

                VerifyIsTrue(Driver.Url.IndexOf("checkout/success?code") != -1 && Driver.Url.IndexOf("&mode=lp") != -1,
                    "funnel success client(4)");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "funnel success status client(4)");
                VerifyIsNull(CheckConsoleLog(true), "funnel success console client(4)");

                orderNum = GetOrderNum();
                CheckOrderInAdmin(orderNum, new List<string> { "TestProduct1", "TestProduct2" });
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
    }
}