using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexExportOptionsTest : ExportServices
    {
        string adminPath = "exportfeeds/indexyandex";
        string exportPath = "export/";
        Dictionary<string, string> csvData;

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.ExportFeed | ClearType.Catalog | ClearType.Currencies);
            InitializeService.LoadData(
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeed.csv",
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeedSelectedCategories.csv",
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeedSelectedProducts.csv",
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeedSettings.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.Brand.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.Tag.csv",
                "Data\\Admin\\Export\\Catalog\\YandexOptions\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile("Data\\Admin\\Export\\TestSettings\\YandexExportOptionsData.csv");
            InitializeService.YandexChannelActive();
            InitializeService.SetShopUrl();
            Init();

            //Functions.SingInYandex(Driver);
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
        public void ShopSettingsYandexExport()
        {
            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName1"]);

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            //common settings: наценка и utm-метки
            Driver.ScrollToTop();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["ssPriceMargin"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.AdditionalUrlTags"), csvData["ssAdditionalUrlTags"]);

            Driver.SendKeysInput(By.Name("ShopName"), csvData["ssShopName"]);
            Driver.SendKeysInput(By.Name("CompanyName"), csvData["ssCompanyName"]);
            Driver.SendKeysInput(By.Name("SalesNotes"), csvData["ssSalesNotes"]);
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            string xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue((xmlResult.IndexOf(ConvertCsvString("<yml_catalog date=\""
                                                             + (DateTime.Now).ToString("yyyy-MM-dd HH:"))) != -1),
                "string: '<yml_catalog date=\"yyyy-MM-dd hh:'   not finded");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["estrShopSet1"])) != -1,
                "Shop name not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["estrShopSet2"])) != -1,
                "Company name not expected");
            string firstProduct = GetYProductFromXml(xmlResult, csvData["firstProductStart"]);
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrShopSet11"])) != -1,
                "markup of first product not expected");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrShopSet12"])) != -1,
                "utm-mark of first product not expected");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrShopSet3"])) != -1,
                "sales_notes of first product not expected");
            string secondProduct = GetYProductFromXml(xmlResult, csvData["secondProductStart"]);
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrShopSet41"])) != -1,
                "markup of second product not expected");
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrShopSet42"])) != -1,
                "utm-mark of second product not expected");
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrShopSet3"])) != -1,
                "sales_notes of second product not expected");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void DeliveryAndCurrencySettingsYandexExport()
        {
            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName2"]);

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            //настройки доставки
            Driver.ScrollToTop();
            Driver.CheckBoxCheck("Store");
            Driver.CheckBoxCheck("Pickup");
            Driver.CheckBoxCheck("Delivery");

            SelectOption("ddlDeliveryCost", "LocalDeliveryCost");
            Driver.SendKeysInput(By.Name("LocalDeliveryOption.Days"), csvData["cdsLocalDeliveryDays"]);
            Driver.SendKeysInput(By.Name("LocalDeliveryOption.OrderBefore"), csvData["cdsLocalDeliveryOrderBefore"]);

            //настройки валют
            Driver.CheckBoxCheck("ExportProductDiscount");
            Driver.CheckBoxCheck("ExportPurchasePrice");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            string xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(xmlResult.IndexOf("<currencies>") == -1, "(1) no common currency tag");
            string firstProduct = GetYProductFromXml(xmlResult, csvData["firstProductStart"]);
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelDelivery1"])) != -1,
                "(1) teg 'Есть возможность доставки' of first product not expected");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelStore1"])) != -1,
                "(1) teg 'Есть точка продаж' of first product not expected");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPickup1"])) != -1,
                "(1) teg 'Есть возможность самовывоза' of first product not expected");

            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPrice1a"])) != -1,
                "(1) teg 'цена' of first product not expected");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelOldPrice1a"])) != -1,
                "(1) teg 'прежняя цена' of first product not expected");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPurchase1a"])) != -1,
                "(1) teg 'закупочная цена' of first product not expected");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelCurrency1"])) != -1,
                "(1) teg 'валюта' of first product not expected");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelDeliveryOptions1"])) != -1,
                "(1) teg 'опции доставки' of first product not expected");

            string secondProduct = GetYProductFromXml(xmlResult, csvData["secondProductStart"]);
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelDelivery1"])) != -1,
                "(1) teg 'Есть возможность доставки' of second product not expected");
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelStore1"])) != -1,
                "(1) teg 'Есть точка продаж' of second product not expected");
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPickup1"])) != -1,
                "(1) teg 'Есть возможность самовывоза' of second product not expected");

            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPrice1b"])) != -1,
                "(1) teg 'цена' of second product not expected");
            VerifyIsFalse(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelNoneOldPrice"])) != -1,
                "(1) teg 'прежняя цена' обнаружен у товара без скидки");
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPurchase1b"])) != -1,
                "(1) teg 'закупочная цена' of second product not expected");
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelCommonDeliveryOptions1"])) != -1,
                "(1) teg 'опции доставки' of second product (без заданной цены доставки) not expected");

            ReturnToExport();

            /*конец первой выгрузки теста*/
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxUncheck("Store");
            Driver.CheckBoxUncheck("Pickup");

            SelectOption("ddlDeliveryCost", "GlobalDeliveryCost");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddGlobalDeliveryOption\"]")).Click();
            Thread.Sleep(1000);
            //проверка на обязательность полей Стоимость и срок доставки
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Days\"]"), "1");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Cost\"]"), csvData["cdsGlobalDeliveryCost1"]);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Days\"]"), csvData["cdsGlobalDeliveryDays1"]);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.OrderBefore\"]"),
                csvData["cdsGlobalDeliveryOrderBefore1"]);
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddGlobalDeliveryOption\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Days\"]"), csvData["cdsGlobalDeliveryDays2"]);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Cost\"]"), csvData["cdsGlobalDeliveryCost2"]);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.OrderBefore\"]"),
                csvData["cdsGlobalDeliveryOrderBefore2"]);
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(1000);
            //настройка валют
            SelectOption("ddlCurrency", csvData["cdsCurSecond"]);
            Driver.CheckBoxUncheck("ExportProductDiscount");
            Driver.CheckBoxUncheck("ExportPurchasePrice");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(xmlResult.IndexOf("<currencies>") == -1, "(2) no common currency tag");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["estrCurDelCommonDeliveryOptions2"])) != -1,
                "teg 'опции доставки' not expected");
            VerifyIsFalse(xmlResult.IndexOf(ConvertCsvString(csvData["estrCurDelCommonDeliveryOptions1"])) != -1,
                "teg 'индивидуальные опции доставки'  обнаружен у товара при включенных глобальных доставках");

            firstProduct = GetYProductFromXml(xmlResult, csvData["firstProductStart"]);
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelStore2"])) != -1,
                "(2) teg 'Есть точка продаж' of first product not expected");
            VerifyIsTrue((firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPickup2"])) != -1)
                         || (firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPickup1"])) == -1),
                "(2) teg 'Есть возможность самовывоза' of first product not expected");

            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPrice2a"])) != -1,
                "(2) teg 'цена' of first product not expected");
            VerifyIsFalse(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelNoneOldPrice"])) != -1,
                "(2) teg 'прежняя цена'  обнаружен у товара при отключенном отображении скидки");
            VerifyIsFalse(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelNonePurchase"])) != -1,
                "(2) teg 'закупочная цена'  обнаружен у товара при отключенном отображении закупочной цены");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelCurrency2"])) != -1,
                "(2) teg 'валюта' of first product not expected");
            VerifyIsFalse(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelNoneDeliveryOptions"])) != -1,
                "(2) teg 'опции доставки' обнаружен внутри of first product");

            secondProduct = GetYProductFromXml(xmlResult, csvData["secondProductStart"]);
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelStore2"])) != -1,
                "(2) teg 'Есть точка продаж' of first product not expected");
            VerifyIsTrue((secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPickup2"])) != -1
                          || secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPickup1"])) == -1),
                "(2) teg 'Есть возможность самовывоза' of first product not expected");

            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelPrice2b"])) != -1,
                "(2) teg 'цена' of second product not expected");
            VerifyIsFalse(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelNoneOldPrice"])) != -1,
                "(2) teg 'прежняя цена' обнаружен у товара при отключенном отображении скидки");
            VerifyIsFalse(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelNonePurchase"])) != -1,
                "(2) teg 'закупочная цена' обнаружен у товара при отключенном отображении закупочной цены");
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelCurrency2"])) != -1,
                "(2) teg 'валюта' of second product not expected");
            VerifyIsFalse(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelNoneDeliveryOptions"])) != -1,
                "(2) teg 'опции доставки' обнаружен внутри of first product,");

            ReturnToExport();

            /*конец второй выгрузки теста*/

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxUncheck("Delivery");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            xmlResult = GetXmlFromYZip(exportName);

            VerifyIsFalse(xmlResult.IndexOf(ConvertCsvString(csvData["estrCurDelNoneDeliveryOptions"])) != -1,
                "teg 'опции доставки' отображается при выключенных возможностях доставки, хотя не должен");
            firstProduct = GetYProductFromXml(xmlResult, csvData["firstProductStart"]);
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["estrCurDelDelivery2"])) != -1,
                "teg 'Есть возможность доставки' of first product not expected");

            secondProduct = xmlResult.Substring(xmlResult.IndexOf(csvData["secondProductStart"].Replace("\\\"", "\"")));
            secondProduct = secondProduct.Substring(0, secondProduct.IndexOf("</offer>"));
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["estrCurDelDelivery2"])) != -1,
                "teg 'Есть возможность доставки' of first product not expected");

            ReturnToExport();

            /*конец третьей выгрузки теста*/
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void DeliveryOptionsVariationsYandexExport()
        {
            //этот тест прогоняется по возможным комбинациям для проверки отсутствия лишних tegs при настройке опций доставки
            int productsWithShipPriceCount = Convert.ToInt32(csvData["dovDelVarProductWithShipPriceCount"]);
            int productWithNotAvailable = Convert.ToInt32(csvData["dovDelVarProductWithNotAvailable"]);
            int productCount = Convert.ToInt32(csvData["dovDelVarProductCount"]);

            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName3"]);

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            //настройки доставки
            //1) выгрузка всех полей с пустыми настройками - проверка, что options отображаются как надо яндексу
            Driver.CheckBoxCheck("Delivery");
            SelectOption("ddlDeliveryCost", "None");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            string xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarDelivery1"]) == productCount,
                "(1) count of tegs delivery=true not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarNoneDelivery"]) == 0,
                "(1) В файле выгрузки найдены tegs delivery-options при настройке 'не выводить возм-ти доставки'");

            ReturnToExport();

            /*конец первой выгрузки теста*/

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlDeliveryCost", "LocalDeliveryCost");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarNoneDelivery"]) == productCount,
                "(2) count of tegs delivery-options not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarLocalDelOpt2a"]) == productsWithShipPriceCount,
                "(2) count of продуктов с индивидуальной ненулевой стоимостью доставки not expected");
            VerifyIsTrue(
                CountOfStrInXml(xmlResult, csvData["estrDelVarLocalDelOpt2b"]) ==
                (productCount - productsWithShipPriceCount),
                "(2) count of продуктов с нулевой стоимостью доставки not expected");
            //deliveryoptions count = product-count
            //tegs с ненулевой ценой за доставку - 1 один
            //tegs с нулевыми днями = все остальные, поскольку поля не заполнены

            ReturnToExport();

            /*конец второй выгрузки теста*/

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlDeliveryCost", "GlobalDeliveryCost");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarNoneDelivery"]) == 0,
                "(3) count of tegs delivery-options ненулевое при отсутствующих опциях");
            //т.к. ни одна опция не задана, tegа delivery-options нет совсем

            ReturnToExport();

            /*конец третей выгрузки теста*/

            //2) 4-5 тесты: все поля сразу заполнены, проверка на отсутствие избыточных данных при заполненности обоих настроек (local и global)
            //!начиная с версии 7.0.4 согласно требованию https://yandex.ru/support/partnermarket/elements/delivery-options.html#general
            //яндекс-маркета teg delivery-option обязателен в tegе shop.
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlDeliveryCost", "LocalDeliveryCost");
            Driver.SendKeysInput(By.Name("LocalDeliveryOption.Days"), csvData["cdsLocalDeliveryDays"]);
            Driver.SendKeysInput(By.Name("LocalDeliveryOption.OrderBefore"), csvData["cdsLocalDeliveryOrderBefore"]);
            SelectOption("ddlDeliveryCost", "GlobalDeliveryCost");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddGlobalDeliveryOption\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Cost\"]"), csvData["dovGlobalDeliveryCost1"]);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Days\"]"), csvData["dovGlobalDeliveryDays1"]);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.OrderBefore\"]"),
                csvData["dovGlobalDeliveryOrderBefore1"]);
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddGlobalDeliveryOption\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Cost\"]"), csvData["dovGlobalDeliveryCost2"]);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Days\"]"), csvData["dovGlobalDeliveryDays2"]);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.OrderBefore\"]"),
                csvData["dovGlobalDeliveryOrderBefore2"]);
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(1000);
            SelectOption("ddlDeliveryCost", "LocalDeliveryCost");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            xmlResult = GetXmlFromYZip(exportName);

            //для 7.0.3 или 7.0.4 в я-требованиях добавили пункт, что глобальные настройки необходимы всегда, поэтому count of товаров+1.
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarNoneDelivery"]) == productCount + 1,
                "(4) count of tegs delivery-options в выгрузке не равно заданному");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarLocalDelOpt4a"]) == productsWithShipPriceCount,
                "(4) count of tegs с ненулевой стоимостью delivery-options в выгрузке не равно заданному");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["estrDelVarLocalDelOpt4NotAv"])) != -1,
                "(4) в файле не обнаружен специальный teg delivery-options для товара под заказ");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarLocalDelOpt4NotAv"]) == productWithNotAvailable,
                "(4) count of tegs delivery-options для товаров под заказ в выгрузке не равно заданному");
            VerifyIsTrue(
                CountOfStrInXml(xmlResult, csvData["estrDelVarLocalDelOpt4b"]) ==
                (productCount - productsWithShipPriceCount - productWithNotAvailable),
                "(4) count of tegs delivery-options в выгрузке не равно заданному");
            //и что count of tegs строго равно кол-ву продуктов
            //здесь дополнительно проверить, что если товар под заказ, то ему индивидуальная настройка дней обнулилась

            ReturnToExport();

            /*конец четвертой выгрузки теста*/

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlDeliveryCost", "GlobalDeliveryCost");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarNoneDelivery"]) == 1,
                "(5) count of tegs delivery-options в выгрузке не равно единице");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarGlobalDelOpt"]) == 1,
                "(5) count of tegs delivery-options с заданными параметрами в выгрузке не равно 1");
            //deliveryoptions count = 1
            //tegs с ненулевой ценой за доставку - 1
            //tegs с нулевыми днями = 1

            ReturnToExport();

            /*конец пятой выгрузки теста*/

            //3) 6 тест: все поля сразу заполнены, возможность доставки = false; проверить, что нет ни одного tegа delivery-options
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxUncheck("Delivery");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarNoneDelivery"]) == 0,
                "(1) tegs " + csvData["estrDelVarNoneDelivery"] + "не 0");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["estrDelVarDelivery2"]) == productCount,
                "(1) count of tegs delivery=false not expected с count ofм продуктов в выгрузке");


            ReturnToExport();

            /*конец шестой выгрузки теста*/

            //end of test - remove exportfeed
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void ProductSettingsYandexExport()
        {
            //настройки товаров
            GoToAdmin("product/edit/" + csvData["productBarcodeID"]);
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.Id("BarCode"), csvData["productBarcodeValue"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(1000);

            GoToAdmin("catalog?showMethod=AllProducts");
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["productBarcodeID"]);
            Thread.Sleep(1000);

            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]~.adv-checkbox-emul")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"4\"]")).Click();

            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName3"]);

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            //выбираются товары 1-15,18-20, => каtegория 1, 2, 3, 5
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector(".jstree-closed[id=\"1\"] i.jstree-icon.jstree-ocl")).Click();
            Thread.Sleep(1000);
            //driver.FindElement(By.CssSelector("[id=\"5_anchor\"]")).Click();
            //для версии 7.0.4 и выше (4 выделена, её откликиваем)
            Driver.FindElement(By.CssSelector("[id=\"4_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            //1) 1 выгрузка теста: все select по умолчанию, все checkbox checked,
            Driver.CheckBoxCheck("ExportNotAvailable");
            Driver.CheckBoxCheck("TypeExportYandex");
            SelectOption("ddlOfferIdType", "id");
            Driver.CheckBoxCheck("ColorSizeToName");
            SelectOption("ddlProductDescriptionType", "short");
            Driver.CheckBoxCheck("RemoveHtml");
            Driver.CheckBoxCheck("ExportBarCode");
            Driver.CheckBoxCheck("ExportAllPhotos");
            Driver.CheckBoxCheck("ExportProductProperties");
            Driver.CheckBoxCheck("ExportRelatedProducts");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.CheckBoxCheck("AllowPreOrderProducts");
            //с 7.0.4 - габариты
            Driver.CheckBoxCheck("ExportDimensions");
            Driver.CheckBoxCheck("NeedZip");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            string xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer id=\"") == Convert.ToInt32(csvData["ps1ProdCount"]),
                "(1) count of продуктов в выгрузке not expected");
            //добавлять к названию цвет и размер ++ выгружать свойства
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["ps1ProdWithModificationGrID"])
                         == (Convert.ToInt32(csvData["ps1ProdCount"]) -
                             Convert.ToInt32(csvData["ps1ProdWithoutModifCount"])),
                "(1) count of продуктов без атрибута group_id (модификаций - цвета и размера) not expected");

            string productWithModification = GetYProductFromXml(xmlResult, csvData["ps1ProdColSizeStart1"]);
            VerifyIsTrue(productWithModification.IndexOf(ConvertCsvString(csvData["ps1ProdColSize1a"])) != -1,
                "(1) свойства Цвет и размер для товара с неск.модификациями не совпадают с эталоном/не найдены");
            VerifyIsTrue(productWithModification.IndexOf(ConvertCsvString(csvData["ps1ProdColSize1b"])) != -1,
                "(1) атрибуты Цвет и размер в имени для товара с неск.модификациями не совпадают с эталоном");
            //проверка наличия свойства
            VerifyIsTrue(productWithModification.IndexOf(ConvertCsvString(csvData["ps1ProdProperty1"])) != -1,
                "(1) свойства для товара с неск.модификациями не совпадают с эталоном/не найдены");
            productWithModification = GetYProductFromXml(xmlResult, csvData["ps1ProdColSizeStart2"]);
            VerifyIsTrue(productWithModification.IndexOf(ConvertCsvString(csvData["ps1ProdColSize2a"])) != -1,
                "(1) свойства Цвет и размер для товара с одной модификацией не совпадают с эталоном/не найдены");
            VerifyIsTrue(productWithModification.IndexOf(ConvertCsvString(csvData["ps1ProdColSize2b"])) != -1,
                "(1) атрибуты Цвет и размер в имени для товара с одной модификацией не совпадают с эталоном");
            //проверка наличия свойства
            VerifyIsTrue(productWithModification.IndexOf(ConvertCsvString(csvData["ps1ProdProperty2"])) != -1,
                "(1) свойства для товара с одной модификацией не совпадают с эталоном/не найдены");
            string productColorOnly = GetYProductFromXml(xmlResult, csvData["ps1ProdColOnlyStart"]);
            VerifyIsTrue(productColorOnly.IndexOf(ConvertCsvString(csvData["ps1ProdColOnlya1"])) != -1,
                "(1) свойство Цвет для товара только с цветом не совпадают с эталоном/не найдены");
            VerifyIsTrue(productColorOnly.IndexOf(ConvertCsvString(csvData["ps1ProdColOnlya2"])) == -1,
                "(1) свойства Размер для товара только с цветом найдены");
            VerifyIsTrue(productColorOnly.IndexOf(ConvertCsvString(csvData["ps1ProdColOnlyb"])) != -1,
                "(1) атрибуты Цвет и размер в имени файла для товара только с цветом не совпадают с эталоном");
            string productSizeOnly = GetYProductFromXml(xmlResult, csvData["ps1ProdSizeOnlyStart"]);
            VerifyIsTrue(productSizeOnly.IndexOf(ConvertCsvString(csvData["ps1ProdSizeOnlya1"])) != -1,
                "(1) свойство Размер для товара только с размером не совпадают с эталоном/не найдены");
            VerifyIsTrue(productSizeOnly.IndexOf(ConvertCsvString(csvData["ps1ProdSizeOnlya2"])) == -1,
                "(1) свойство Цвет для товара только с размером найдены");
            VerifyIsTrue(productSizeOnly.IndexOf(ConvertCsvString(csvData["ps1ProdSizeOnlyb"])) != -1,
                "(1) атрибуты Цвет и размер в имени файла для товара только с размером не совпадают с эталоном");

            string productWithoutColSize = GetYProductFromXml(xmlResult, csvData["ps1ProdWithoutColSizeStart"]);
            VerifyIsFalse(productWithoutColSize.IndexOf(ConvertCsvString(csvData["ps1ProdWithoutColSizea1"])) != -1,
                "(1) найдено свойство Цвет для товара без цвета");
            VerifyIsFalse(productWithoutColSize.IndexOf(ConvertCsvString(csvData["ps1ProdWithoutColSizea2"])) != -1,
                "(1) найдено свойство Размер для товара без размера");
            VerifyIsFalse(productWithoutColSize.IndexOf(ConvertCsvString(csvData["ps1ProdWithoutColSizeb"])) != -1,
                "(1) название товара без цвета и размера not expected");
            VerifyIsFalse(productWithoutColSize.IndexOf(ConvertCsvString(csvData["ps1ProdWithoutProperties"])) != -1,
                "(1) найдено свойство property для товара без свойств");

            //упрощенный тип
            VerifyIsTrue(
                GetYProductFromXml(xmlResult, csvData["ps1ProdSimpleTypeStart1"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdSimpleType1"])) != -1,
                "(1) упрощенный тип для товара с производителем и страной not expected с эталоном");
            VerifyIsTrue(
                GetYProductFromXml(xmlResult, csvData["ps1ProdSimpleTypeStart2"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdSimpleType2"])) != -1,
                "(1) упрощенный тип для товара с производителем без страны not expected с эталоном");
            VerifyIsTrue(
                GetYProductFromXml(xmlResult, csvData["ps1ProdSimpleTypeStart3"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdSimpleType3"])) != -1,
                "(1) упрощенный тип для товара без производителя not expected с эталоном");

            //недоступные продукты
            //VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["ps1ProdNotActiveSubstr"]) == Convert.ToInt32(csvData["ps1ProdNotActiveCount"]), 
            //  "(1) count of недоступных продуктов (с атрибутом available=false) not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["ps1ProdNotActive1"])) != -1,
                "(1) первый из недоступных товаров not expected с эталоном (id, available или group_id)");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["ps1ProdNotActive2"])) != -1,
                "(1) второй из недоступных товаров not expected с эталоном (id, available или group_id)");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["ps1ProdNotActive1Full"])) != -1,
                "(1) первый из недоступных товаров not expected с эталоном (полное сравнение)");

            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["ps1ProdNotActive2Full"])) != -1,
                "(1) второй из недоступных товаров not expected с эталоном (полное сравнение");

            //краткое описание
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps1ProdBreafDescrStart"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdBreafDescr"])) != -1,
                "(1) краткое описание для товара с описанием без html not expected");
            VerifyIsFalse(GetYProductFromXml(xmlResult, csvData["ps1ProdWithourDescrStart"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdWithoutDescr"])) != -1,
                "(1) для товара без описания был найден teg description");

            //удалять html из описания
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps1ProdHtmlDescrStart"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdHtmlDescr"])) != -1,
                "(1) краткое описание для товара с html not expected");

            //штрих-код
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["ps1ProdBarcodeTag"])
                         == Convert.ToInt32(csvData["ps1ProdBarcodeCount"]),
                "(1) count of штрихкодов в файле выгрузки not expected");
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps1ProdBarcodeStart"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdBarcode"])) != -1,
                "(1) штрихкод для товара с html not expected");

            //несколько фотографий
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps1ProdAllPhotoStart1"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdAllPhoto1"])) != -1,
                "(1) фотографии для товара с 1 фото not expected");
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps1ProdAllPhotoStart2"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdAllPhoto2"])) != -1,
                "(1) фотографии для товара с несколькими фото not expected");

            //товары с рекомендуемыми продуктами
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["ps1ProdWithRelatedTag"])
                         == Convert.ToInt32(csvData["ps1ProdWithRelatedCount"]),
                "(1) count of товаров с tegом rec (рекомендуемые) not expected");
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps1ProdWithRelatedStart1"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdWithRelated1"])) != -1,
                "(1) рекомендованные для товара 1 (несколько рекомендаций) not expected");
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps1ProdWithRelatedStart2"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdWithRelated2"])) != -1,
                "(1) рекомендованные для товара 2 (одна рекомендация) not expected");
            VerifyIsTrue(
                GetYProductFromXml(xmlResult, csvData["ps1ProdWithRelNotExportProductStart"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdWithRelNotExportProduct"])) == -1,
                "(1) обнаружен teg для рекомедованных у товара, где рекомендованный не уччаствует в выгрузке");

            //выгружать только главную модификацию
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["ps1ProdWithModifications"]) == 1,
                "(1) при заданной настройке Выгружать только главную модификацию найдено несколько товаров с одной group_id");

            //выгружать товары под заказ
            VerifyIsTrue(
                CountOfStrInXml(xmlResult, csvData["ps1ProdWithRelatedTag"]) ==
                Convert.ToInt32(csvData["ps1ProdWithRelatedCount"]),
                "(1) count of товаров под заказ not expected");
            VerifyIsFalse(
                string.IsNullOrEmpty(GetYProductFromXml(xmlResult, csvData["ps1ProdWithRelNotExportProductStart"])),
                "(1) товар под заказ not expected");

            //с 7.0.4 - габариты
            VerifyIsTrue(
                CountOfStrInXml(xmlResult, csvData["ps1ProdDimensionsTag"]) ==
                Convert.ToInt32(csvData["ps1ProdDimensionsCount"]),
                "(1) count of tegs размеров (измерений) not expected");
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps1ProdDimensions1Start"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdDimensions1"])) != -1,
                "(1) не найден заданный teg с размерами (измерениями) для of first product из указанных");
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps1ProdDimensions2Start"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdDimensions2"])) != -1,
                "(1) не найден заданный teg с размерами (измерениями) для of second product из указанных");
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps1ProdDimensions3Start"])
                    .IndexOf(ConvertCsvString(csvData["ps1ProdDimensions3"])) == -1,
                "(1) найден teg с размерами (измерениями) для товара без размеров ");


            ReturnToExport();

            /*конец первой выгрузки теста*/

            //2) вторая выгрузка: все галки в ложь, все селекты - во второй вариант
            //dsuh
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Thread.Sleep(1000);
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxUncheck("TypeExportYandex");
            SelectOption("ddlOfferIdType", "id"); //кроме этого, тут первый вариант
            Driver.CheckBoxUncheck("ColorSizeToName");
            SelectOption("ddlProductDescriptionType", "full");
            Driver.CheckBoxUncheck("RemoveHtml");
            Driver.CheckBoxUncheck("ExportProductProperties");
            Driver.CheckBoxUncheck("ExportBarCode");
            Driver.CheckBoxUncheck("ExportAllPhotos");
            Driver.CheckBoxUncheck("ExportRelatedProducts");
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.CheckBoxUncheck("ExportDimensions");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer id=\"") == Convert.ToInt32(csvData["ps2ProdCount"]),
                "(2) count of продуктов в выгрузке not expected");
            //отсутствие неактивных товаров в выгрузке
            VerifyIsFalse(xmlResult.IndexOf(ConvertCsvString(csvData["ps2ProdNotActive1"])) != -1,
                "(2) первый из недоступных товаров обнаружен в выгрузке при выключенной настройке вывода недоступных");
            VerifyIsFalse(xmlResult.IndexOf(ConvertCsvString(csvData["ps2ProdNotActive2"])) != -1,
                "(2) второй из недоступных товаров обнаружен в выгрузке при выключенной настройке вывода недоступных");

            //нет ни одного продукта с предзаказом
            VerifyIsFalse(xmlResult.IndexOf(ConvertCsvString(csvData["ps2ProdPreOrderTag"])) != -1,
                "(2) в выгрузке при выключенной настройке вывода товаров под заказ найден товар с предзаказом");

            //модификации товаров
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["ps2ProdWithModificationGrID"])
                         == Convert.ToInt32(csvData["ps2ProdWithModifCount"]),
                "(2) count of товаров с заданной group_id not expected. ");
            foreach (var modification in csvData.Where(i => i.Key.IndexOf("ps2ProdWithModifStart") != -1).ToList())
            {
                VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(modification.Value)) != -1,
                    "(2) модификация " + modification.Key + " товара не найдена в файле выгрузки");
            }

            //не добавлять цвет и размер к названию
            string productColorSize = GetYProductFromXml(xmlResult, csvData["ps2ProdColSizeStart1"]);
            VerifyIsTrue(productColorSize.IndexOf(ConvertCsvString(csvData["ps2ProdColSize1a"])) != -1,
                "(2) свойства Цвет и размер для товара 1 с цветом и размером не совпадают с эталоном/не найдены");
            VerifyIsTrue(productColorSize.IndexOf(ConvertCsvString(csvData["ps2ProdColSize1"])) != -1,
                "(2) Название товара для продукта с цветом и размером не совпадают с эталоном");
            productColorSize = GetYProductFromXml(xmlResult, csvData["ps2ProdColSizeStart2"]);
            VerifyIsTrue(productColorSize.IndexOf(ConvertCsvString(csvData["ps2ProdColSize2a"])) != -1,
                "(2) свойства Цвет и размер для товара 2 с цветом и размером не совпадают с эталоном/не найдены");
            VerifyIsTrue(productColorSize.IndexOf(ConvertCsvString(csvData["ps2ProdColSize2"])) != -1,
                "(2) Название товара для продукта с цветом и размером не совпадают с эталоном");

            productColorOnly = GetYProductFromXml(xmlResult, csvData["ps2ProdColOnlyStart"]);
            VerifyIsTrue(productColorOnly.IndexOf(ConvertCsvString(csvData["ps2ProdColOnlya1"])) != -1,
                "(2) свойство Цвет для товара только с цветом не совпадают с эталоном/не найдены");
            VerifyIsTrue(productColorOnly.IndexOf(ConvertCsvString(csvData["ps2ProdColOnlya2"])) == -1,
                "(2) свойство Размер для товара только с цветом найдены");
            VerifyIsTrue(productColorOnly.IndexOf(ConvertCsvString(csvData["ps2ProdColOnly"])) != -1,
                "(2) атрибуты Цвет и размер в имени файла для товара только с цветом не совпадают с эталоном");

            productSizeOnly = GetYProductFromXml(xmlResult, csvData["ps2ProdSizeOnlyStart"]);
            VerifyIsTrue(productSizeOnly.IndexOf(ConvertCsvString(csvData["ps2ProdSizeOnlya1"])) != -1,
                "(2) свойство Размер для товара только с размером не совпадают с эталоном/не найдены");
            VerifyIsTrue(productSizeOnly.IndexOf(ConvertCsvString(csvData["ps2ProdSizeOnlya2"])) == -1,
                "(2) свойство Цвет для товара только с размером найдены");
            VerifyIsTrue(productSizeOnly.IndexOf(ConvertCsvString(csvData["ps2ProdSizeOnly"])) != -1,
                "(2) атрибуты Цвет и размер в имени файла для товара только с размером не совпадают с эталоном");

            productWithoutColSize = GetYProductFromXml(xmlResult, csvData["ps2ProdWithoutColSizeStart"]);
            VerifyIsFalse(productWithoutColSize.IndexOf(ConvertCsvString(csvData["ps2ProdWithoutColSizea1"])) != -1,
                "(2) найдено свойство Цвет для товара без цвета");
            VerifyIsFalse(productWithoutColSize.IndexOf(ConvertCsvString(csvData["ps2ProdWithoutColSizea2"])) != -1,
                "(2) найдено свойство Размер для товара без размера");
            VerifyIsFalse(productWithoutColSize.IndexOf(ConvertCsvString(csvData["ps2ProdWithoutColSize"])) != -1,
                "(2) название товара без цвета и размера not expected");

            //отстутствия свойств property  в выгрузке
            VerifyIsFalse(xmlResult.IndexOf(ConvertCsvString(csvData["ps2ProdProperty"])) != -1,
                "(2) найдено свойство property в выгрузке при заданном Не выводить свойства");

            //неупрощенный формат записи
            //по факту это "not-simple"
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps2ProdNotSimpleTypeStart1"])
                    .IndexOf(ConvertCsvString(csvData["ps2ProdNotSimpleType1"])) != -1,
                "(2) неупрощенный тип для товара с производителем и страной not expected с эталоном");
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps2ProdNotSimpleTypeStart2"])
                    .IndexOf(ConvertCsvString(csvData["ps2ProdNotSimpleType2"])) != -1,
                "(2) неупрощенный тип для товара с производителем без страны not expected с эталоном");
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps2ProdNotSimpleTypeStart3"])
                    .IndexOf(ConvertCsvString(csvData["ps2ProdNotSimpleType3"])) != -1,
                "(2) неупрощенный тип для товара без производителя not expected с эталоном");

            //полное описание
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps2ProdFullDescrStart"])
                    .IndexOf(ConvertCsvString(csvData["ps2ProdFullDescr"])) != -1,
                "(2) полное описание для товара с описанием без html not expected");
            VerifyIsFalse(GetYProductFromXml(xmlResult, csvData["ps2ProdWithourDescrStart"])
                    .IndexOf(ConvertCsvString(csvData["ps2ProdWithoutDescr"])) != -1,
                "(2) для товара без описания был найден teg description");

            //удалять html из описания
            VerifyIsTrue(GetYProductFromXml(xmlResult, csvData["ps2ProdHtmlDescrStart"])
                    .IndexOf(ConvertCsvString(csvData["ps2ProdHtmlDescr"])) != -1,
                "(2) полное описание для товара с html not expected");

            //с 7.0.4 - габариты
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["ps1ProdDimensionsTag"]) == 0,
                "(2) count of tegs размеров (измерений) не равно нулю!");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["ps1ProdDimensionsTag2"]) == 0,
                "(2) count of tegs размеров (измерений) не равно нулю!");


            ReturnToExport();

            /*конец второй выгрузки теста*/


            //3) третья выгрузка: проверка непроверенного
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            //исключение товаров
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.GetGridCellInputForm(2, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("ui-modal-cross .close")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();


            Thread.Sleep(1000);
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxCheck("TypeExportYandex");
            SelectOption("ddlOfferIdType", "artno");
            Driver.CheckBoxCheck("ColorSizeToName");
            SelectOption("ddlProductDescriptionType", "none");
            Driver.CheckBoxCheck("RemoveHtml");
            Driver.CheckBoxUncheck("ExportProductProperties");
            Driver.CheckBoxUncheck("ExportBarCode");
            Driver.CheckBoxUncheck("ExportAllPhotos");
            Driver.CheckBoxCheck("ExportRelatedProducts");
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            Driver.CheckBoxCheck("AllowPreOrderProducts");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            xmlResult = GetXmlFromYZip(exportName);

            VerifyAreEqual(Convert.ToInt32(csvData["ps3ProdCount"]), CountOfStrInXml(xmlResult, "<offer id=\""),
                "(3) count of продуктов в выгрузке not expected");
            //используется артикул модификации
            string product = xmlResult.Substring(0,
                xmlResult.IndexOf(csvData["ps3ProdArticulMStart"].ToLower().Replace("\\\"", "\"")));

            product = product.Substring(product.LastIndexOf("</offer>"));
            VerifyIsTrue(product.IndexOf(csvData["ps3ProdArticulMValue"].Replace("\\\"", "\"")) != -1,
                "(3) для товара без модификаций артикул модификации not expected");

            ////проверка цвета и размера для товара с модификациями
            //foreach (var modification in csvData.Where(i => i.Key.IndexOf("ps2ProdWithModifStart") != -1).ToList())
            //{
            //    VerifyIsTrue(xmlResult.IndexOf(modification.Value) != -1, "(3) модификация " + modification.Key + " товара не найдена в файле выгрузки");
            //}
            //следующие строчки были закомментированы для версий 7.0.4 и выше, т.к.добавилось упорядочивание оферов по id.
            //а теперь оно, видимо, снова исчезло.
            var arrayStrOfModifGroup =
                xmlResult.Substring(
                    xmlResult.IndexOf(csvData["ps3ColSizeWithModifStart"].ToLower().Replace("\\\"", "\"")));
            arrayStrOfModifGroup = arrayStrOfModifGroup.Substring(0,
                arrayStrOfModifGroup.IndexOf(csvData["ps3ColSizeWithModifNext"].ToLower().Replace("\\\"", "\"")));
            /*начало для 7.0.4 и выше*/
            //var arrayStrOfModifGroup = GetYProductFromXml(xmlResult, csvData["ps3ColSizeWithModifStart"])+"</offer>";
            //var preLastProd = xmlResult.IndexOf(ConvertCsvString(csvData["ps3ColSizeWithModifPreLast"])) 
            //    + GetYProductFromXml(xmlResult, csvData["ps3ColSizeWithModifPreLast"]).Length + 8;
            //arrayStrOfModifGroup += xmlResult.Substring(preLastProd, xmlResult.LastIndexOf("</offer>") - preLastProd) + "</offer>";
            /*конец для 7.0.4 и выше*/
            int j = 1;

            foreach (var modification in csvData.Where(i => i.Key.IndexOf("ps3ColSizeWithModifName") != -1).ToList())
            {
                if (j > 1)
                {
                    VerifyIsFalse(
                        arrayStrOfModifGroup.IndexOf(ConvertCsvString(csvData["ps3ColSizeWithModStart" + j])) != -1,
                        "(3) для модификаций товара " + j +
                        " вместо артикула модификации обнаружен идентификатор модификации");
                }
                else
                {
                    VerifyIsTrue(
                        arrayStrOfModifGroup.IndexOf(ConvertCsvString(csvData["ps3ColSizeWithModStart" + j])) != -1,
                        "(3) для главной модификаций товара индентификатор модификации not expected");
                }

                product = arrayStrOfModifGroup.Substring(0, arrayStrOfModifGroup.IndexOf("</offer>"));
                arrayStrOfModifGroup = arrayStrOfModifGroup.Remove(0, arrayStrOfModifGroup.IndexOf("</offer>") + 8);
                VerifyIsTrue(product.IndexOf(ConvertCsvString(csvData["ps3ColSizeWithModifName" + j])) != -1,
                    "(3) - имя модификации " + j + " не совпало с эталонным");
                VerifyIsTrue(product.IndexOf(ConvertCsvString(csvData["ps3ColSizeWithModifParam" + (j++)])) != -1,
                    "(3) - свойства  модификации " + j + " не совпали с эталонными");
            }


            //отсутствие описания
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["ps3NoneDescription"]) == 0,
                "(3) при заданном 'не выгружать описание' обнаружен teg description");
            //рекомендованные только для участвующих в выгрузке
            VerifyAreEqual(CountOfStrInXml(xmlResult, csvData["ps3ProdWithRelatedTag"]),
                Convert.ToInt32(csvData["ps3ProdWithRelatedCount"]),
                "(3) количество tegs с рекомендованными товарами в выгрузке больше эталона");

            product = GetYProductFromXml(xmlResult, csvData["ps3ProdWithRelatedStart1"]);
            VerifyIsTrue(CountOfStrInXml(product, csvData["ps3ProdWithRelated1"]) != -1,
                "(3) для товара с рекомендованными не найден teg рекомендованных с заданным содержанием");
            product = GetYProductFromXml(xmlResult, csvData["ps3ProdWithRelatedExStart1"]);
            VerifyAreEqual(CountOfStrInXml(product, csvData["ps3ProdWithRelatedEx1"]), 0,
                "(3) для товара с рекомендованным, исключенным из выгрузки, обнаружен teg рекомендаций.");

            ReturnToExport();

            /*конец третьей выгрузки теста*/

            //end of test - remove exportfeed
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void ExportSettingsYGYandexExport()
        {
            //параметры экспорта для google и yandex market (ссылка в каточке товара)
            //индивидуальные sales notes, я-маркет typePrefix, я-маркет model, я-маркет название тоара, 
            //я -маркет срок доставки, я-маркет ставка для карточки, обозначения размерных сеток в unit, 
            //товар для взрослых, гарантия производителя
            //товар с сеттингами - 16, товар для сравнения без сеттингов - 17
            //настройки: добавить заметки (без и с sales-notes), выгружать ст-ть доставки (индив, общие), тип описания (упрощенный, не упр)
            //бид - добавляется к tegу оффер, 
            //+ проверить галку "добавлять цвет и размер" на счет name и model
            //в этом тесте руками добавить для 18-того размер8, в конце теста удалить его

            //1круг без селзнотес, индив ст-ть доставки, упрощ тип,
            //2круг с селзнотес, общ ст-ть доставки, неупрощ тип,
            GoToAdmin("product/edit/" + csvData["ps5ProductID"]);
            Driver.ScrollTo(By.Id("CurrencyId"));
            Thread.Sleep(1000);
            Driver.GetGridCellInputForm(0, "SizeId", "Offers").FindElement(AdvBy.DataE2E("select")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[ng-model=\"row.entity['SizeId']\"]"))
                .FindElements(By.CssSelector("[data-e2e=\"itemDropDownSelect\"]"))[1].Click();
            Driver.SendKeysInput(By.Name("ShippingPrice"), "100");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            Driver.FindElement(By.PartialLinkText("Параметры экспорта в Яндекс.Маркет")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.data.YandexDeliveryDays\"]"), "10-15");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(1000);

            GoToAdmin(adminPath);
            AddExportFeed("ExportSettingsYG");

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector(".jstree-closed[id=\"1\"] i.jstree-icon.jstree-ocl")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[id=\"5_anchor\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            SelectOption("ddlDeliveryCost", "LocalDeliveryCost");
            Driver.SendKeysInput(By.Name("LocalDeliveryOption.Days"), "1-5");
            Driver.FindElement(By.Name("LocalDeliveryOption.OrderBefore")).Clear();
            SelectOption("ddlDeliveryCost", "GlobalDeliveryCost");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddGlobalDeliveryOption\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Cost\"]"), "100");
            Driver.SendKeysInput(By.CssSelector("[data-ng-model=\"ctrl.Days\"]"), "1-10");
            Driver.FindElement(By.CssSelector("[data-ng-model=\"ctrl.OrderBefore\"]")).Clear();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(1000);
            Driver.CheckBoxCheck("TypeExportYandex");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            string xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["ps5ProductStart18"])) != -1,
                "(1) Bid was not added");
            VerifyIsTrue(
                CountOfStrInXml(xmlResult,
                    "<delivery-options><option cost=\"100\" days=\"1-10\" /></delivery-options>") == 1,
                "(1) quantity of delivery-options tegs not expected");
            string firstProduct = GetYProductFromXml(xmlResult, csvData["ps5ProductStart18"]);
            string secondProduct = GetYProductFromXml(xmlResult, csvData["ps5ProductStart20"]);

            VerifyAreEqual(GetTegContent(firstProduct, "name"), csvData["ps5Name18"].ToLower(),
                "(1)YA-name not expected");
            VerifyAreEqual(GetTegContent(secondProduct, "name"), csvData["ps5Name20"].ToLower(),
                "(1)product name without YA-name not expected");
            VerifyAreEqual(GetTegContent(firstProduct, "sales_notes"), csvData["ps5SalesNotes18"].ToLower(),
                "(1)individual sales notes");
            VerifyIsTrue(secondProduct.IndexOf("<sales_notes") == -1,
                "(1)not expected sales notes");
            VerifyIsTrue(firstProduct.IndexOf("<manufacturer_warranty>true</manufacturer_warranty>") != -1,
                "(1)expected manufacturer_warranty");
            VerifyIsTrue(secondProduct.IndexOf("<manufacturer_warranty") == -1,
                "(1)not expected manufacturer_warranty");
            VerifyIsTrue(firstProduct.IndexOf("<adult>true</adult>") != -1,
                "(1)expected adult");
            VerifyIsTrue(secondProduct.IndexOf("<manufacturer_warranty") == -1,
                "(1)not expected adult");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["ps5UnitSize18"])) != -1,
                "(1)expected UnitSize");
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["ps5UnitSize20"])) != -1,
                "(1)expected Size");

            ReturnToExport();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.SendKeysInput(By.Name("SalesNotes"), csvData["ps5SalesNotes19"]);
            SelectOption("ddlDeliveryCost", "LocalDeliveryCost");
            Driver.CheckBoxUncheck("TypeExportYandex");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["ps5ProductStart18_2"])) != -1,
                "(2) Bid was not added");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<delivery-options>") == 4,
                "(2) quantity of delivery-options tegs not expected");
            firstProduct = GetYProductFromXml(xmlResult, csvData["ps5ProductStart18_2"]);
            secondProduct = GetYProductFromXml(xmlResult, csvData["ps5ProductStart19"]);

            VerifyAreEqual(GetTegContent(firstProduct, "model"), csvData["ps5Model18"].ToLower(),
                "(2)YA-name not expected");
            VerifyAreEqual(GetTegContent(secondProduct, "model"), csvData["ps5Model19"].ToLower(),
                "(2)product name without YA-name not expected");
            VerifyAreEqual(GetTegContent(firstProduct, "sales_notes"), csvData["ps5SalesNotes18"].ToLower(),
                "(2)individual sales notes");
            VerifyAreEqual(GetTegContent(secondProduct, "sales_notes"), csvData["ps5SalesNotes19"].ToLower(),
                "(2)common sales notes");
            VerifyIsTrue(firstProduct.IndexOf(csvData["ps5TypePrefix18"].ToLower()) != -1,
                "(2)expected typePrefix");
            VerifyIsTrue(secondProduct.IndexOf("<typePrefix") == -1,
                "(2)not expected typePrefix");
            VerifyIsTrue(firstProduct.IndexOf(ConvertCsvString(csvData["ps5DeliveryOptions18"])) != -1,
                "(2)individual deliveryoptions ");
            VerifyIsTrue(secondProduct.IndexOf(ConvertCsvString(csvData["ps5DeliveryOptions19"])) != -1,
                "(2)common deliveryoptions ");
            VerifyIsTrue(firstProduct.IndexOf("<manufacturer_warranty>true</manufacturer_warranty>") != -1,
                "(2)expected manufacturer_warranty");
            VerifyIsTrue(secondProduct.IndexOf("<manufacturer_warranty") == -1,
                "(2)not expected manufacturer_warranty");
            VerifyIsTrue(firstProduct.IndexOf("<adult>true</adult>") != -1,
                "(2)expected adult");
            VerifyIsTrue(secondProduct.IndexOf("<manufacturer_warranty") == -1,
                "(2)not expected adult");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();

            GoToAdmin("product/edit/" + csvData["ps5ProductID"]);
            Driver.ScrollTo(By.Id("CurrencyId"));
            Thread.Sleep(1000);
            Driver.GetGridCellInputForm(0, "SizeId", "Offers").FindElement(AdvBy.DataE2E("select")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[ng-model=\"row.entity['SizeId']\"]"))
                .FindElements(By.CssSelector("[data-e2e=\"itemDropDownSelect\"]"))[0].Click();
            Driver.FindElement(By.Name("ShippingPrice")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.FindElement(By.PartialLinkText("Параметры экспорта в Яндекс.Маркет")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.data.YandexDeliveryDays\"]")).Clear();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(1000);
        }
    }
}