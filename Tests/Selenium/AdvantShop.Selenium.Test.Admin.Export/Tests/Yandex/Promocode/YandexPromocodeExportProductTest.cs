using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex.Promocode
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexPromocodeExportProductTest : ExportServices
    {
        string adminPath = "exportfeeds/indexyandex";
        string exportPath = "export/";
        string couponVal, couponCur;
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
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.Brand.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.Tag.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Products\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile(
                "Data\\Admin\\Export\\TestSettings\\YandexPromos\\PromocodeExportProductData.csv");
            InitializeService.YandexChannelActive();
            couponVal = csvData["pepCouponValueCur1"];
            couponCur = csvData["pepCouponCur"];

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
        public void YandexPromocodePCat()
        {
            //для всех каtegорий: промо для 1 кат и 1 товара внутри нее (3 кат и 11 товар)
            //для всех каtegорий: промо для 1 кат и 1 вне ее (2кат и 1товар)
            //для 3-й и 4й каteg: промо для 3 каteg и 2 товара 
            //для 3-й и 4й каteg: промо для 5 каteg и 11 товара
            GoToAdmin(adminPath);
            AddExportFeed("PromocodePCat");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), "Promocode1");
            AddCoupon("CouponPCat1", couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponProducts\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", "CouponPCat1", "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), "Promocode2");
            AddCoupon("CouponPCat2", couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponProducts\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", "CouponPCat2", "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pep1CategoryCount1"]),
                "(1)Количество выгруженных каtegорий not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pep1PromosCount1"]),
                "(1)Количество выгруженных promo-акций not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 1), "purchase") == ConvertCsvString(csvData["pep1Promos11"]),
                "(1)Для промоакции 1 содержимое tegа purchase not expected с эталоном");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 2), "purchase") == ConvertCsvString(csvData["pep1Promos12"]),
                "(1)Для промоакции 2 содержимое tegа purchase not expected с эталоном");


            ReturnToExport();

            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoCodes").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoCodes").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector(".jstree-closed[id=\"1\"] i.jstree-icon.jstree-ocl")).Click();
            Driver.FindElement(By.CssSelector("[id=\"4_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), "Promocode3");
            AddCoupon("CouponPCat3", couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponProducts\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", "CouponPCat3", "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), "Promocode4");
            AddCoupon("CouponPCat4", couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"1\"] .jstree-icon")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"5_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponProducts\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", "CouponPCat4", "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pep1CategoryCount2"]),
                "(2)Количество выгруженных каtegорий not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pep1PromosCount2"]),
                "(2)Количество выгруженных promo-акций not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 1), "purchase") == ConvertCsvString(csvData["pep1Promos21"]),
                "(2)Для промоакции 1 содержимое tegа purchase not expected с эталоном");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 2), "purchase") == ConvertCsvString(csvData["pep1Promos22"]),
                "(2)Для промоакции 2 содержимое tegа purchase not expected с эталоном");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexPromocodePAll()
        {
            //все кат, промокод для всх прод в 1 каtegории(4 кат)
            //все кат, промокод для неск прод в разных кат(3, 10, 12 прод)
            //все кат, промокод для неактивных продуктов(6-7 прод)
            //все кат, промокод для норм продукта из неативн каtegории(27)
            GoToAdmin(adminPath);
            AddExportFeed("PromocodePAll");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();

            //все продукты 4-й каtegории
            AddPromocode(
                "Promocode1", "CouponPAll1", couponVal, couponCur,
                null, new List<string> {csvData["pep2CategoryId"]}, true);

            //неск прод в разных кат(3, 10, 12 прод)
            AddPromocode(
                "Promocode2", "CouponPAll2", couponVal, couponCur,
                null, new List<string> {csvData["pep2Product21"], csvData["pep2Product22"], csvData["pep2Product23"]});

            //2 неактивных продукта(6,7 прод)
            AddPromocode(
                "Promocode3", "CouponPAll3", couponVal, couponCur,
                null, new List<string> {csvData["pep2Product31"], csvData["pep2Product32"],});

            //Норм продукт из неактивной каtegории(27 прод)
            AddPromocode(
                "Promocode4", "CouponPAll4", couponVal, couponCur,
                null, new List<string> {csvData["pep2Product4"]});
            Thread.Sleep(500);
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pep2CategoryCount"]),
                "(2)Количество выгруженных каtegорий not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pep2PromosCount"]),
                "(2)Количество выгруженных promo-акций not expected");
            string promos = GetPromosFromXml(xmlResult);
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(promos, 1), "purchase") == ConvertCsvString(csvData["pep2Promos1"]),
                "Для промоакции 1 содержимое tegа purchase not expected с эталоном");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(promos, 2), "purchase") == ConvertCsvString(csvData["pep2Promos2"]),
                "Для промоакции 2 содержимое tegа purchase not expected с эталоном");
            VerifyIsFalse(promos.IndexOf(ConvertCsvString(csvData["pep2Promos31"])) != -1
                          || promos.IndexOf(ConvertCsvString(csvData["pep2Promos32"])) != -1,
                "В выгрузке обнаружен промокод для неактивных товаров");
            VerifyIsFalse(promos.IndexOf(ConvertCsvString(csvData["pep2Promos4"])) != -1,
                "В выгрузке обнаружен промокод для неактивной каtegории");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexPromocodePSelected()
        {
            //для 3, 6 кат:
            //промо для 26 товар
            //промо для 11, 12, 16
            //промо для 5
            GoToAdmin(adminPath);
            AddExportFeed("PromocodePSelected");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"6_anchor\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Thread.Sleep(1000);
            AddPromocode(
                "Promocode1", "CouponPSelected1", couponVal, couponCur,
                null, new List<string> {csvData["pep3Product1"]});
            AddPromocode(
                "Promocode2", "CouponPSelected2", couponVal, couponCur,
                null, new List<string> {csvData["pep3Product21"], csvData["pep3Product22"], csvData["pep3Product23"]});
            AddPromocode(
                "Promocode3", "CouponPSelected3", couponVal, couponCur,
                null, new List<string> {csvData["pep3Product3"]});
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pep3CategoryCount"]),
                "Количество выгруженных каtegорий not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pep3PromosCount"]),
                "Количество выгруженных promo-акций not expected");
            string promos = GetPromosFromXml(xmlResult);
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(promos, 1), "purchase") == ConvertCsvString(csvData["pep3Promos1"]),
                "Для промоакции 1 содержимое tegа purchase not expected с эталоном");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(promos, 2), "purchase") == ConvertCsvString(csvData["pep3Promos2"]),
                "Для промоакции 1 содержимое tegа purchase not expected с эталоном");
            VerifyIsFalse(promos.IndexOf(ConvertCsvString(csvData["pep3Promos3"])) != -1,
                "В выгрузке обнаружен промокод для неактивной каtegории");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexPromocodePExcluded()
        {
            //для 3, 6 кат; исключены = 15 26:
            //промо для всех товаров из 3 каtegории
            //промо для 26
            //промо для удаленного товара - экспортнутся все каtegории выгрузки
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProduct\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.ClassName("edit")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"inputProductName\"]"), "NewTestProduct");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(1000);
            Driver.ScrollTo(By.Id("CurrencyId"));
            Thread.Sleep(1000);
            Driver.SendKeysGridCell("1", 0, "BasePrice", "Offers");
            Driver.SendKeysGridCell("1001", 0, "ArtNo", "Offers");
            Driver.FindElement(By.Id("TaxId")).Click();

            GoToAdmin(adminPath);
            AddExportFeed("PromocodePExcluded");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            SelectOption("ddlOfferIdType", "artno");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"6_anchor\"]")).Click();
            Driver.ClearToastMessages();

            //исключение товаров
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pep4Excluded1"]);
            Thread.Sleep(1000);
            Driver.GetGridCellInputForm(0, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pep4Excluded2"]);
            Thread.Sleep(1000);
            Driver.GetGridCellInputForm(0, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.FindElement(By.CssSelector("ui-modal-cross .close")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Thread.Sleep(1000);
            AddPromocode("Promocode1", "CouponPExcluded1", couponVal, couponCur, null,
                new List<string> {csvData["pep4CategoryId"]}, true);
            AddPromocode("Promocode2", "CouponPExcluded2", couponVal, couponCur, null,
                new List<string> {csvData["pep4Product1"]});
            AddPromocode("Promocode3", "CouponPExcluded3", couponVal, couponCur, null,
                new List<string> {"NewTestProduct"});

            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pep4CategoryCount"]),
                "Количество выгруженных каtegорий not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pep4PromosCount"]),
                "Количество выгруженных promo-акций not expected");
            string promos = GetPromosFromXml(xmlResult);
            //Убрала, т.к. индекс нового товара изменился в тестовой базе
            //VerifyIsTrue(GetTegContent(GetPromoFromXml(promos, 1), "purchase") == ConvertCsvString(csvData["pep4Promos11"]),
            //    "Для промоакции 1 содержимое tegа purchase not expected с эталоном");
            VerifyIsTrue(
                CountOfStrInXml(GetTegContent(GetPromoFromXml(promos, 1), "purchase"), "product offer-id") == 5,
                "Для промоакции 1 count of товаров с предложениями not expected с эталоном");
            VerifyIsFalse(promos.IndexOf(ConvertCsvString(csvData["pep4Promos2"])) != -1,
                "В выгрузке обнаружен промокод для товара из неактивной каtegории");
            //Убрала, т.к. индекс нового товара изменился в тестовой базе
            //VerifyIsTrue(GetTegContent(GetPromoFromXml(promos, 2), "purchase") == ConvertCsvString(csvData["pep4Promos31"]),
            //    "Для промоакции 3 содержимое tegа purchase not expected с эталоном");
            VerifyIsTrue(
                CountOfStrInXml(GetTegContent(GetPromoFromXml(promos, 2), "purchase"), "product offer-id") == 1,
                "Для промоакции 2 count of товаров с предложениями not expected с эталоном");
            GoToAdmin("catalog?categoryId=3");
            Driver.FindElement(By.LinkText("NewTestProduct")).Click();
            Driver.WaitForElem(By.ClassName("link-danger"));
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();

            GoToAdmin(adminPath);
            Thread.Sleep(1000);
            Driver.FindElement(By.LinkText("PromocodePExcluded")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pep4PromosCount2"]),
                "(2)Количество выгруженных promo-акций not expected");
            promos = GetPromosFromXml(xmlResult);
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(promos, 1), "purchase") == ConvertCsvString(csvData["pep4Promos12"]),
                "(2)Для промоакции 1 содержимое tegа purchase not expected с эталоном");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(promos, 2), "purchase") == ConvertCsvString(csvData["pep4Promos32"]),
                "(2)Для промоакции 3 содержимое tegа purchase not expected с эталоном");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexPromocodePModifications()
        {
            //для 9-го продукта
            //первые настройки - идентификатор + только главная
            //вторые настройки - артикул + только главная
            //третьи настройки - идентификатор + все модификации
            //четвертые настройки - артикул + все модификации
            GoToAdmin(adminPath);
            AddExportFeed("PromocodePModifications");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.GetGridCell(-1, "selectionRowHeaderCol", "ProductsSelectvizr").FindElement(By.CssSelector(
                "[data-e2e=\"gridHeaderCheckboxSelectAll\"]" +
                "~.adv-checkbox-emul")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pep5Product"]);
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr").FindElement(By.CssSelector(
                "[data-e2e=\"gridCheckboxWrapSelect\"] " +
                ".adv-checkbox-emul")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.FindElement(By.ClassName("close")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Thread.Sleep(1000);
            AddPromocode("Promocode", "CouponPModifications", couponVal, couponCur, null,
                new List<string> {csvData["pep5Product"]});
            Thread.Sleep(1000);
            //(1) первые настройки - идентификатор + только главная
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlOfferIdType", "id");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == 1,
                "(1) Количество выгруженных товаров not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(1) Количество выгруженных promo-акций not expected");
            string promos = GetPromosFromXml(xmlResult);
            VerifyIsTrue(
                GetTegContent(promos, "purchase").IndexOf("<product offer-id=\"" + csvData["pep5ProductID1"]) != -1,
                "(1) Заданный идентификатор главной модификации не найден в promo");

            ReturnToExport();

            //(2) вторые настройки - артикул + только главная
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlOfferIdType", "artno");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == 1,
                "(2) Количество выгруженных товаров not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(2) Количество выгруженных promo-акций not expected");
            promos = GetPromosFromXml(xmlResult);
            VerifyIsTrue(
                GetTegContent(promos, "purchase").IndexOf("<product offer-id=\"" + csvData["pep5ProductID2"]) != -1,
                "(2) Заданный идентификатор главной модификации не найден в promo");

            ReturnToExport();

            //(3) третьи настройки - идентификатор + все модификации
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlOfferIdType", "id");
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            int countOfModifications = Convert.ToInt32(csvData["pep5ModifCount"]);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == countOfModifications,
                "(3) Количество выгруженных товаров not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(3) Количество выгруженных promo-акций not expected");
            promos = GetPromosFromXml(xmlResult);
            string[] arrayOfModifId = csvData["pep5ProductID3"].Split(',');
            string offer = "";
            for (int i = 0; i < countOfModifications; i++)
            {
                offer = xmlResult.Substring(xmlResult.IndexOf("<offer "), xmlResult.IndexOf("</offer>"));
                xmlResult = xmlResult.Substring(offer.Length + 7);
                VerifyIsTrue(offer.IndexOf("<offer id=\"" + arrayOfModifId[i] + "\"") != -1,
                    "(3) Для оффера " + (i + 1) + " идентификатор not expected");
                VerifyIsTrue(
                    GetTegContent(promos, "purchase").IndexOf("<product offer-id=\"" + arrayOfModifId[i]) != -1,
                    "(3) Ожидаемый оффер для модификации " + (i + 1) + " не найден в promo");
            }

            ReturnToExport();

            //(4) четвертые настройки - артикул + все модификации
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlOfferIdType", "artno");
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == countOfModifications,
                "(4) Количество выгруженных товаров not expected: ожидалось " + countOfModifications + ", " +
                "получено " + CountOfStrInXml(xmlResult, "<promo "));
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(4) Количество выгруженных promo-акций not expected: ожидалось 1, " +
                "получено " + CountOfStrInXml(xmlResult, "<promo "));
            promos = GetPromosFromXml(xmlResult);
            offer = "";

            for (int i = 0; i < countOfModifications; i++)
            {
                offer = xmlResult.Substring(xmlResult.IndexOf("<offer "), xmlResult.IndexOf("</offer>"));
                xmlResult = xmlResult.Substring(offer.Length + 7);
                int indexID = offer.IndexOf("<offer id=\"") + 11;
                arrayOfModifId[i] = offer.Substring(indexID, offer.IndexOf("\" ") - indexID);
                VerifyIsTrue(
                    GetTegContent(promos, "purchase").IndexOf("<product offer-id=\"" + arrayOfModifId[i]) != -1,
                    "(4) Оффер для модификации " + (i + 1) + " не найден в promo" + "(" + promos + ")");
            }

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexPromocodePDisabled()
        {
            //для 6, 7, 8 продуктов
            //первые настройки - не выгружать под заказ, не выгружать недоступные (неактивный, нет цены).
            //вторые настройки - выгружать под заказ, выгружать недоступные.
            GoToAdmin(adminPath);
            AddExportFeed("PromocodePDisabled");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.GetGridCell(-1, "selectionRowHeaderCol", "ProductsSelectvizr").FindElement(By.CssSelector(
                "[data-e2e=\"gridHeaderCheckboxSelectAll\"]" +
                "~.adv-checkbox-emul")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pep6Product1"]);
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr").FindElement(By.CssSelector(
                "[data-e2e=\"gridCheckboxWrapSelect\"] " +
                ".adv-checkbox-emul")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pep6Product2"]);
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr").FindElement(By.CssSelector(
                "[data-e2e=\"gridCheckboxWrapSelect\"] " +
                ".adv-checkbox-emul")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pep6Product3"]);
            Thread.Sleep(1000);
            //хмл-валидатор не пропускает пустую выгрузку без товаров, нужно добавить хотя бы 1
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr").FindElement(By.CssSelector(
                "[data-e2e=\"gridCheckboxWrapSelect\"] " +
                ".adv-checkbox-emul")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pep6Product4"]);
            Thread.Sleep(1000);

            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr").FindElement(By.CssSelector(
                "[data-e2e=\"gridCheckboxWrapSelect\"] " +
                ".adv-checkbox-emul")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.FindElement(By.ClassName("close")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Thread.Sleep(1000);
            AddPromocode("Promocode", "CouponPDisabled", couponVal, couponCur, null, new List<string>
            {
                csvData["pep6Product1"], csvData["pep6Product2"], csvData["pep6Product3"]
            });
            Thread.Sleep(1000);
            //(1) первые настройки - не выгружать под заказ, не выгружать недоступные (неактивный, нет цены).
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == 0 + 1,
                "(1) Количество выгруженных товаров not expected: ожидалось 1, получено "
                + CountOfStrInXml(xmlResult, "<offer "));
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 0,
                "(1) Количество выгруженных promo-акций not expected: ожидалось 0, получено "
                + CountOfStrInXml(xmlResult, "<promo "));

            ReturnToExport();

            //(2) вторые настройки - выгружать под заказ, выгружать недоступные.
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxCheck("ExportNotAvailable");
            Driver.CheckBoxCheck("AllowPreOrderProducts");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == 3 + 1,
                "(2) Количество выгруженных товаров not expected: ожидалось 3+1, получено "
                + CountOfStrInXml(xmlResult, "<offer "));
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(2) Количество выгруженных promo-акций not expected: ожидалось 1, получено "
                + CountOfStrInXml(xmlResult, "<promo "));
            string promos = GetPromosFromXml(xmlResult);
            VerifyAreEqual(GetTegContent(promos, "purchase"), ConvertCsvString(csvData["pep6Promos"]),
                "(2) Содержимое tegа promo purchase not expected: ожидалось " +
                ConvertCsvString(csvData["pep6Promos"]) + ", " +
                "получено " + GetTegContent(promos, "purchase"));

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }
    }
}