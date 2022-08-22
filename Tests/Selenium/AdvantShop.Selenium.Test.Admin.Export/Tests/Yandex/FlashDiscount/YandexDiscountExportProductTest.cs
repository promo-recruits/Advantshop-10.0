using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex.FlashDiscount
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexDiscountExportProductTest : ExportServices
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
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.Brand.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.Tag.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\Products\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile(
                "Data\\Admin\\Export\\TestSettings\\YandexPromos\\DiscountExportProductData.csv");
            InitializeService.YandexChannelActive();
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
        public void YandexDiscountPDisabled()
        {
            //1-товар без цены и скидки (10) - не отоб.
            //2-товар без скидки с ценой (5) - не отоб.
            //3-товар без цены со скидкой в валюте (11)
            //4-товар без цены со скидкой в процентах (7)
            //5-товар с скидкой и нулевым количеством (8)
            //6-товар с ценой и скидкой в рублях (1)
            //сперва с выключенными, потом с включенными галками

            GoToAdmin(adminPath);

            AddExportFeed("DiscountPDisabled");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"5\"]")).Click();
            string[] arrayOfProductIndex = csvData["dp1Products"].Split(',');
            AddDiscount("PDisabled1", csvData["dp1Products"].Split(',').ToList());
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["dp1ProductCount1"]),
                "(1)количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(GetPromosFromXml(xmlResult), "<promo ") == 1,
                "(1)количество tegs promo not expected");
            string promoPurchase = GetTegContent(GetPromosFromXml(xmlResult), "purchase");
            VerifyIsTrue(
                CountOfStrInXml(promoPurchase, "<product offer-id") ==
                Convert.ToInt32(csvData["dp1PromoProductCount1"]),
                "(1)количество предложений в промоакции");
            string[] promoPurchaseExpected1 = csvData["dp1PromosArray1"].Split(',');

            for (int i = 0; i < promoPurchaseExpected1.Count(); i++)
            {
                VerifyIsTrue(promoPurchase.IndexOf(ConvertCsvString(promoPurchaseExpected1[i])) != -1,
                    "(1)На итерации " + (i + 1) + " в purchase не нашелся ожидаемый товар");
            }

            promoPurchaseExpected1 = null;
            ReturnToExport();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxCheck("ExportNotAvailable");
            Driver.CheckBoxCheck("AllowPreOrderProducts");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["dp1ProductCount2"]),
                "(2)количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(GetPromosFromXml(xmlResult), "<promo ") == 1,
                "(2)количество tegs promo not expected");
            promoPurchase = GetTegContent(GetPromosFromXml(xmlResult), "purchase");
            VerifyIsTrue(
                CountOfStrInXml(promoPurchase, "<product offer-id") ==
                Convert.ToInt32(csvData["dp1PromoProductCount2"]),
                "(2)количество предложений в промоакции");
            string[] promoPurchaseExpected2 = csvData["dp1PromosArray2"].Split(',');

            for (int i = 0; i < promoPurchaseExpected2.Count(); i++)
            {
                VerifyIsTrue(promoPurchase.IndexOf(ConvertCsvString(promoPurchaseExpected2[i])) != -1,
                    "(2)На итерации " + (i + 1) + " в purchase не нашелся ожидаемый товар");
            }

            promoPurchaseExpected2 = null;

            ReturnToExport();
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexDiscountPExcluded()
        {
            //товар в акции из каtegории, не уч в выгрузке(16)
            //товар в акции , исключенный из выгрузки.(2)
            GoToAdmin(adminPath);

            AddExportFeed("DiscountPExcluded");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"5\"]")).Click();
            string[] arrayOfProductIndex = csvData["dp1Products"].Split(',');
            AddDiscount("PDisabled1", csvData["dp2Products"].Split(',').ToList());
            //на первой итерации нет ни одного включенного продукта->нет промоакции
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), "TestProduct2",
                byToDropFocus: By.ClassName("modal-header-title"));
            Driver.GetGridCellInputForm(0, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.ClassName("close")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["dp2ProductCount1"]),
                "(1)количество товаров в выгрузке not expected");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") == -1,
                "(1)В выгрузке без акций обнаружен teg promos ");
            VerifyIsTrue(xmlResult.IndexOf("<promo ") == -1,
                "(1)В выгрузке без акций обнаружен teg promo");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<product offer-id") == 0,
                "(1)количество предложений в промоакции не совпало с ожидаемым");
            ReturnToExport();
            //на второй итерации экспорт всех каtegорий - должно быть 2 промоакции
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AllProducts\"]")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["dp2ProductCount2"]),
                "(2)количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(GetPromosFromXml(xmlResult), "<promo ") == 1,
                "(2)количество tegs promo not expected");
            string promoPurchase = GetTegContent(GetPromosFromXml(xmlResult), "purchase");
            VerifyIsTrue(
                CountOfStrInXml(promoPurchase, "<product offer-id") == Convert.ToInt32(csvData["dp2PromoProductCount"]),
                "(1)количество предложений в промоакции");
            VerifyIsTrue(promoPurchase.IndexOf(ConvertCsvString(csvData["dp2Promos1"])) != -1,
                "(2)В purchase не нашелся ожидаемый 1 товар");
            VerifyIsTrue(promoPurchase.IndexOf(ConvertCsvString(csvData["dp2Promos2"])) != -1,
                "(2)В purchase не нашелся ожидаемый 2 товар");

            ReturnToExport();
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexDiscountPModifications()
        {
            //9 товар с модификациями: нужно проверить, что при скидке у модификации скидка у всего;
            //1 - без акции выгрузить с артикулом. ПРоверить, что цена у всех модификаций снизилась.
            //2 - без акции выгрузить с идентификатором. ПРоверить, что цена у всех модификаций снизилась.
            //3 - с акцией с артикулом
            //4 - с акцией с модификатором

            GoToAdmin(adminPath);
            AddExportFeed("DiscountPModifications");
            string exportName = SetCommonYExportSettings(TestName, exportPath);
            Driver.ScrollToTop();
            int productCount = Convert.ToInt32(csvData["dp3ProductCount"]);
            int purchaseCount = Convert.ToInt32(csvData["dp3PurchaseCount"]);
            string productPrice = csvData["dp3ProductPrice"];
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.ClearToastMessages();
            //первый
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlOfferIdType", "id");
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == productCount,
                "(1)количество товаров в выгрузке not expected");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") == -1,
                "(1)в выгрузке без промоакций обнаружен teg promos");
            VerifyIsTrue(xmlResult.IndexOf("<promo ") == -1,
                "(1)в выгрузке без промоакций обнаружен teg promo");
            VerifyIsTrue(xmlResult.IndexOf("<product offer-id") == -1,
                "(1)количество предложений в промоакции not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, productPrice) == productCount,
                "(1)количество эталонных tegs с ценой не соответствует ожидаемому");
            ReturnToExport();
            //второй
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlOfferIdType", "artno");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == productCount,
                "(2)количество товаров в выгрузке not expected");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") == -1,
                "(2)в выгрузке без промоакций обнаружен teg promos");
            VerifyIsTrue(xmlResult.IndexOf("<promo ") == -1,
                "(2)в выгрузке без промоакций обнаружен teg promo");
            VerifyIsTrue(xmlResult.IndexOf("<product offer-id") == -1,
                "(2)количество предложений в промоакции not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, productPrice) == productCount,
                "(2)количество эталонных tegs с ценой не соответствует ожидаемому");
            ReturnToExport();
            //третий - артикул

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"5\"]")).Click();
            AddDiscount("DiscountPModifications", new List<string> {csvData["dp3Product"]});

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlOfferIdType", "id");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == productCount,
                "(3)количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(GetPromosFromXml(xmlResult), "<promo ") == 1,
                "(3)количество tegs promo not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["dp3ProductOldPrice"]) == productCount,
                "(3)к-во tegs с ценой в офере не соответствует ожидаемому");
            string promoPurchase = GetTegContent(GetPromoFromXml(xmlResult, 1), "purchase");
            VerifyIsTrue(CountOfStrInXml(promoPurchase, "<product offer-id") == purchaseCount,
                "(3) количество предложений в промоакции not expected");

            string[] arrayOfModif1 = csvData["dp3ProductsArray1"].Split(',');
            string[] arrayOfMPurchase1 = csvData["dp3PromosArray1"].Split(',');
            for (int i = 0; i < productCount; i++)
            {
                VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(arrayOfModif1[i])) != -1,
                    "(3) Для оффера " + (i + 1) + " идентификатор not expected");
                VerifyIsTrue(promoPurchase.IndexOf(ConvertCsvString(arrayOfMPurchase1[i])) != -1,
                    "(3) Эталонный оффер для модификации " + (i + 1) + " не найден в promo");
            }

            ReturnToExport();
            //четвертый - идентификатор

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            SelectOption("ddlOfferIdType", "artno");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == productCount,
                "(4)количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(GetPromosFromXml(xmlResult), "<promo ") == 1,
                "(4)количество tegs promo not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["dp3ProductOldPrice"]) == productCount,
                "(4)к-во tegs с ценой в офере не соответствует ожидаемому");
            promoPurchase = GetTegContent(GetPromoFromXml(xmlResult, 1), "purchase");
            VerifyIsTrue(CountOfStrInXml(promoPurchase, "<product offer-id") == purchaseCount,
                "(4) количество предложений в промоакции not expected");

            arrayOfModif1 = csvData["dp3ProductsArray2"].Split(',');
            arrayOfMPurchase1 = csvData["dp3PromosArray2"].Split(',');
            for (int i = 0; i < productCount; i++)
            {
                VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(arrayOfModif1[i])) != -1,
                    "(4) Для оффера " + (i + 1) + " идентификатор not expected");
                VerifyIsTrue(promoPurchase.IndexOf(ConvertCsvString(arrayOfMPurchase1[i])) != -1,
                    "(4) Эталонный оффер для модификации " + (i + 1) + " не найден в promo");
            }

            ReturnToExport();
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexDiscountPOldPrice()
        {
            //1 товар со скидкой добавить в акцию, один не добавлять. в рублях. каtegория 3. 
            //1 = 14, 2 = 15
            //первый тест - +олдпрайс, у of first product нет олд, цена отоб старая, у второго - цена отоб новая, есть олд
            //второй тест - -олдпрайс, у of first product нет олд, цена отоб старая, у второго - цена отоб новая, нет олд
            GoToAdmin(adminPath);
            AddExportFeed("DiscountPOldPrice");
            string exportName = SetCommonYExportSettings(TestName, exportPath);
            Driver.ScrollTo(By.Id("SalesNotes")); //чтоб шапка не перекрыла инпут
            Driver.CheckBoxCheck("ExportProductDiscount");
            string productId1 = csvData["dp4Product1"];
            string productId2 = csvData["dp4Product2"];
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"5\"]")).Click();
            AddDiscount("DiscountOldPrice", new List<string> {productId1});
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["dp4ProductCount"]),
                "(1)количество товаров в выгрузке not expected");
            string product = GetYProductFromXml(xmlResult, "<offer id=\"" + productId1);
            VerifyIsTrue(product.IndexOf(ConvertCsvString(csvData["dp4Product1Price"])) != -1,
                "(1) Для оффера с Id=" + productId1 + " цена not expected с эталоном ");
            VerifyIsTrue(product.IndexOf("<oldprice") == -1,
                "(1) Для оффера с Id=" + productId1 + " обнаружен teg олдпрайс");
            product = GetYProductFromXml(xmlResult, "<offer id=\"" + productId2);
            VerifyIsTrue(product.IndexOf(ConvertCsvString(csvData["dp4Product2Price1"])) != -1,
                "(1) Для оффера с Id=" + productId2 + " цена not expected с эталоном ");
            VerifyIsTrue(product.IndexOf(ConvertCsvString(csvData["dp4Product2OldPrice"])) != -1,
                "(1) Для оффера с Id=" + productId2 + " oldprice not expected с эталоном ");
            ReturnToExport();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxUncheck("ExportProductDiscount");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            product = GetYProductFromXml(xmlResult, "<offer id=\"" + productId1);
            VerifyIsTrue(product.IndexOf(ConvertCsvString(csvData["dp4Product1Price"])) != -1,
                "(2) Для оффера с Id=" + productId1 + " цена not expected с эталоном ");
            VerifyIsTrue(product.IndexOf("<oldprice") == -1,
                "(2) Для оффера с Id=" + productId1 + " обнаружен teg олдпрайс");
            product = GetYProductFromXml(xmlResult, "<offer id=\"" + productId2);
            VerifyIsTrue(product.IndexOf(ConvertCsvString(csvData["dp4Product2Price2"])) != -1,
                "(2) Для оффера с Id=" + productId2 + " цена not expected с эталоном ");
            VerifyIsTrue(product.IndexOf("<oldprice") == -1,
                "(2) Для оффера с Id=" + productId2 + " обнаружен teg олдпрайс");
            VerifyIsTrue(product.IndexOf(ConvertCsvString(csvData["dp4Product2Price2"])) != -1,
                "(2) Для оффера с Id=" + productId2 + " цена not expected с эталоном ");

            ReturnToExport();
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }
    }
}