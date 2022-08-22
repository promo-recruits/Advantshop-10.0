using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Google
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class GoogleExportOptionsTest : ExportServices
    {
        string adminPath = "exportfeeds/indexgoogle";
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
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.Brand.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.Tag.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleOptions\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile("Data\\Admin\\Export\\TestSettings\\GoogleExportOptionsData.csv");
            InitializeService.GoogleChannelActive();
            InitializeService.SetShopUrl();
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

        [Test]
        public void DisplayGoogleExport()
        {
            GoToAdmin(adminPath);
            AddExportFeed("DisplayG");
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"6_anchor\"]")).Click();
            Driver.ClearToastMessages();

            string exportName = SetCommonGExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromGFile(Driver, exportPath, exportName);
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["geo1XmlHeader"])) != -1,
                "xmlHeader");
            VerifyIsTrue(xmlResult.IndexOf("<channel>") != -1,
                "shop chanel");
            VerifyIsTrue(xmlResult.IndexOf("<channel><title>") != -1,
                "shop title");
            VerifyIsTrue(xmlResult.IndexOf("<link>") != -1,
                "shop link");
            VerifyIsTrue(xmlResult.IndexOf("<description>") != -1,
                "shop description");

            string product = GetGProductFromXml(xmlResult, 1);
            VerifyIsTrue(product.IndexOf("<g:id>") != -1,
                "product id");
            VerifyIsTrue(product.IndexOf("<title>") != -1,
                "product title");
            VerifyIsTrue(product.IndexOf("<g:description>") != -1,
                "product description");
            VerifyIsTrue(product.IndexOf("<g:google_product_category>") != -1,
                "product google_product_category");
            VerifyIsTrue(product.IndexOf("<g:product_type>") != -1,
                "product product_type");
            VerifyIsTrue(product.IndexOf("<g:adult>") != -1,
                "product adult");
            VerifyIsTrue(product.IndexOf("<link>") != -1,
                "product link");
            VerifyIsTrue(product.IndexOf("<g:image_link>") != -1,
                "product image_link");
            VerifyIsTrue(product.IndexOf("<g:condition>") != -1,
                "product condition");
            VerifyIsTrue(product.IndexOf("<g:availability>") != -1,
                "product availability");
            VerifyIsTrue(product.IndexOf("<g:price>") != -1,
                "product price");
            VerifyIsTrue(product.IndexOf("<g:gtin>") != -1,
                "product gtin");
            VerifyIsTrue(product.IndexOf("<g:mpn>") != -1,
                "product mpn");
            VerifyIsTrue(product.IndexOf("<g:item_group_id>") != -1,
                "product item_group_id");
            VerifyIsTrue(product.IndexOf("<g:color>") != -1,
                "product color");
            VerifyIsTrue(product.IndexOf("<g:size>") != -1,
                "product size");
            VerifyIsTrue(product.IndexOf("<g:expiration_date>") != -1,
                "product expiration_date");

            GoToAdmin(adminPath);
            RemoveGoogleExport("DisplayG");
        }

        [Test]
        public void ShopSettingsGoogleExport()
        {
            GoToAdmin(adminPath);
            AddExportFeed("ShopSettingsG");

            string exportName = SetCommonGExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["geo2PriceMargin"]);
            //проверить наценку на товаре со скидкой
            Driver.SendKeysInput(By.Name("ExportFeedSettings.AdditionalUrlTags"), csvData["geo2AdditionalUrlTags"]);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromGFile(Driver, exportPath, exportName);

            string product = GetGProductFromXml(xmlResult, csvData["geo2ProductId1"]);
            VerifyIsTrue(GetTegContent(product, "g:price") == csvData["geo2Price1"],
                "(1)price");
            VerifyIsTrue(GetTegContent(product, "g:sale_price") == csvData["geo2SalePrice1"],
                "(1)sale price");
            VerifyIsTrue(GetTegContent(product, "link") == csvData["geo2Link1"],
                "(1)link");

            product = GetGProductFromXml(xmlResult, csvData["geo2ProductId2"]);
            VerifyIsTrue(GetTegContent(product, "g:price") == csvData["geo2Price2"],
                "(2)price");
            VerifyIsTrue(GetTegContent(product, "link") == csvData["geo2Link2"],
                "(2)link");

            GoToAdmin(adminPath);
            RemoveGoogleExport("ShopSettingsG");
        }

        [Test]
        public void AdditionalSettingsGoogleExport()
        {
            //1 тест - все галки в тру, из селектов 1 вариант, кат по умолчанию не заполнена, описание и титл дефолтные
            //2 тест - все галки в фолс, селекторы 2 вариант, кат по умолчанию заполнена
            //выбираем все каtegории

            //1т - 9 только 1; 2т - 9.1, 9.2, 9.3
            //1т - 1, 5 товары в руб, 2т - 1, 5 товары долл 
            //1т - заголовок1, 2т - заголовок2
            //1т - описание1, 2т - описание описание с хтмл?
            //1т - с кат по дефолту, 2т - без кат по дефолту
            //1т - с 6,7,8 товарами; 2т - без 6,7,8 товаров
            //1т - 9, 10 без хтмл, 11 - что в описании?; 2т - 9, 10 с хтмл, 11 - что в описании?
            //тут же проверять-сравнивать описание
            //1т - 1,9 товары; 2т - 101,109,91

            //отдельный тест - с неакт, без предзаказ; без неакт, с предзаказ
            //отдельный тест - не выгр хтмл, полн описание; выгр хтмл, краткое описание
            //отдельный тест - только гл модификация, артно; не только главн, айди

            GoToAdmin("settingscatalog#?catalogTab=currency");
            Thread.Sleep(1000);
            Driver.GetGridCellInputForm(2, "RoundNumbers", "Currencies").FindElement(AdvBy.DataE2E("select")).Click();
            Driver.FindElements(By.CssSelector("ul.ui-select-choices .ui-select-choices-group " +
                                               "[data-e2e=\"itemDropDownSelect\"]"))[1].Click();
            Thread.Sleep(1000);
            GoToAdmin(adminPath);
            AddExportFeed("AdditionalSettingsG");
            Refresh();
            string exportName = SetCommonGExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            //1 тест
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.CheckBoxCheck("RemoveHtml");
            Driver.CheckBoxCheck("ExportNotAvailable");
            Driver.CheckBoxCheck("AllowPreOrderProducts");
            SelectOption("ddlCurrency", "RUB");
            SelectOption("ddlProductDescriptionType", "short");
            SelectOption("ddlOfferIdType", "id");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromGFile(Driver, exportPath, exportName);
            //shop title + shop description

            VerifyIsTrue(xmlResult.IndexOf(ConvertGCsvString(csvData["geo3ShopNameDefault"])) != -1,
                "(1)shop name");
            VerifyIsTrue(xmlResult.IndexOf(ConvertGCsvString(csvData["geo3ShopDescriptionDefault"])) != -1,
                "(1)shop description");
            //modifications
            VerifyIsTrue(CountOfStrInXml(xmlResult, ConvertGCsvString(csvData["geo3Modifications"])) == 1,
                "(1)modifications");
            //currency
            VerifyIsTrue(GetGProductFromXml(xmlResult, 1).IndexOf(csvData["geo3PriceCur11"]) != -1,
                "(1-1)currency");
            VerifyIsTrue(GetGProductFromXml(xmlResult, 5).IndexOf(csvData["geo3PriceCur12"]) != -1,
                "(1-2)currency");
            //defaultCategory
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<g:google_product_category>")
                         == Convert.ToInt32(csvData["geo3GoogleCatCount1"]),
                "(1)defaultCategory");

            //not available and preorder
            string[] notAvailableAndPreorderId = csvData["geo3NotAvailableAndPreorder"].Split(',');
            foreach (var productId in notAvailableAndPreorderId)
            {
                VerifyIsTrue(xmlResult.IndexOf("<item>" + productId) != -1,
                    "(1) not available/preorder product " + productId);
            }

            //html
            string[] productDescriptions = csvData["geo3PDescription1"].Split(',');
            VerifyIsTrue(GetGProductFromXml(xmlResult, "9").IndexOf(ConvertGCsvString(productDescriptions[0])) != -1,
                "(1) description in product 9");
            VerifyIsTrue(GetGProductFromXml(xmlResult, "10").IndexOf(ConvertGCsvString(productDescriptions[1])) != -1,
                "(1) description in product 10");
            VerifyIsTrue(GetGProductFromXml(xmlResult, "11").IndexOf("<g:description>") == -1,
                "(1) description in product 11");
            //id-artno
            string[] idArtnoProducts = csvData["geo3IdArtno1"].Split(',');
            foreach (var productId in idArtnoProducts)
            {
                VerifyIsTrue(xmlResult.IndexOf("<item>" + productId) != -1,
                    "(1) id-artno product " + productId);
            }

            GoToAdmin(adminPath);
            Driver.FindElements(By.CssSelector(".fa-pencil-alt.export-feed-block-settings")).Last().Click();
            Thread.Sleep(1000);
            Refresh();
            //2 тест
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            exportName = SetCommonGExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            Driver.SendKeysInput(By.Name("DatafeedTitle"), csvData["geo3ShopName"]);
            Driver.SendKeysInput(By.Name("DatafeedDescription"), csvData["geo3ShopDescription"]);
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            Driver.CheckBoxUncheck("RemoveHtml");
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            SelectOption("ddlCurrency", "USD");
            SelectOption("ddlProductDescriptionType", "full");
            SelectOption("ddlOfferIdType", "artno");
            Driver.SendKeysInput(By.Name("GoogleProductCategory"), csvData["geo3GoogleCat"]);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromGFile(Driver, exportPath, exportName);
            //shop title + shop description
            VerifyIsTrue(xmlResult.IndexOf(ConvertGCsvString(csvData["geo3ShopName1"])) != -1,
                "(2)shop name");
            VerifyIsTrue(xmlResult.IndexOf(ConvertGCsvString(csvData["geo3ShopDescription1"])) != -1,
                "(2)shop description");
            //modifications
            VerifyIsTrue(CountOfStrInXml(xmlResult, ConvertGCsvString(csvData["geo3Modifications"]))
                         == Convert.ToInt32(csvData["geo3ModificationsCount"]),
                "(2)modifications");

            //currency
            VerifyIsTrue(GetGProductFromXml(xmlResult, 1).IndexOf(csvData["geo3PriceCur21"]) != -1,
                "(2-1)currency");
            VerifyIsTrue(GetGProductFromXml(xmlResult, 5).IndexOf(csvData["geo3PriceCur22"]) != -1,
                "(2-2)currency");

            //defaultCategory
            int google_p_cat_count = 0;
            int startIndex = 0;
            string google_p_cat = "<g:google_product_category><![cdata[" + csvData["geo3GoogleCat"].ToLower()
                                                                         + "]]></g:google_product_category>";
            while (xmlResult.IndexOf(google_p_cat, startIndex) != -1)
            {
                ++google_p_cat_count;
                startIndex = xmlResult.IndexOf(google_p_cat, startIndex) + 1;
            }

            VerifyIsTrue(google_p_cat_count == Convert.ToInt32(csvData["geo3GoogleCatCount22"]),
                "(2-1)defaultCategory");
            //not available and preorder
            foreach (var productId in notAvailableAndPreorderId)
            {
                VerifyIsTrue(xmlResult.IndexOf("<item>" + productId) == -1,
                    "(2) not available/preorder product " + productId);
            }

            //html
            productDescriptions = csvData["geo3PDescription2"].Split(',');
            VerifyIsTrue(GetGProductFromXml(xmlResult, "9").IndexOf(ConvertGCsvString(productDescriptions[0])) != -1,
                "(2) description in product 9");
            VerifyIsTrue(GetGProductFromXml(xmlResult, "10").IndexOf(ConvertGCsvString(productDescriptions[1])) != -1,
                "(2) description in product 10");
            VerifyIsTrue(GetGProductFromXml(xmlResult, "11").IndexOf("<g:description>") == -1,
                "(2) description in product 11");
            //id-artno
            idArtnoProducts = csvData["geo3IdArtno2"].Split(',');
            foreach (var productId in idArtnoProducts)
            {
                VerifyIsTrue(xmlResult.IndexOf("<item>" + productId) != -1,
                    "(2) id-artno product " + productId);
            }

            GoToAdmin(adminPath);
            Driver.FindElements(By.CssSelector(".fa-pencil-alt.export-feed-block-settings")).Last().Click();
            Thread.Sleep(1000);
            Refresh();
            //3 тест
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            exportName = SetCommonGExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            SelectOption("ddlCurrency", "RUB");
            Driver.CheckBoxCheck("ExportNotAvailable");
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.CheckBoxCheck("RemoveHtml");
            SelectOption("ddlProductDescriptionType", "full");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            SelectOption("ddlOfferIdType", "artno");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromGFile(Driver, exportPath, exportName);
            foreach (string productId in csvData["geo3NotActive"].Split(','))
            {
                VerifyIsTrue(xmlResult.IndexOf(ConvertGCsvString(productId)) != -1,
                    "(3) not active " + productId);
            }

            VerifyIsTrue(xmlResult.IndexOf(ConvertGCsvString(csvData["geo3PreOrder"])) != -1,
                "(3) preorder");
            productDescriptions = csvData["geo3PDescription3"].Split(',');
            VerifyIsTrue(GetGProductFromXml(xmlResult, "9").IndexOf(ConvertGCsvString(productDescriptions[0])) != -1,
                "(3) description in product 9");
            VerifyIsTrue(GetGProductFromXml(xmlResult, "10").IndexOf(ConvertGCsvString(productDescriptions[1])) != -1,
                "(3) description in product 10");
            VerifyIsTrue(GetGProductFromXml(xmlResult, "11").IndexOf("<g:description>") == -1,
                "(3) description in product 11");

            VerifyIsTrue(xmlResult.IndexOf("<item>" + csvData["geo3IdArtno31"]) != -1,
                "(3) id-artno product " + csvData["geo3IdArtno31"]);
            VerifyIsTrue(xmlResult.IndexOf("<item>" + csvData["geo3IdArtno32"]) == -1,
                "(3) id-artno product " + csvData["geo3IdArtno31"]);

            GoToAdmin(adminPath);
            Driver.FindElements(By.CssSelector(".fa-pencil-alt.export-feed-block-settings")).Last().Click();
            Thread.Sleep(1000);
            Refresh();
            //4 тест
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            exportName = SetCommonGExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxCheck("AllowPreOrderProducts");
            Driver.CheckBoxUncheck("RemoveHtml");
            SelectOption("ddlProductDescriptionType", "short");
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            SelectOption("ddlOfferIdType", "id");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromGFile(Driver, exportPath, exportName);
            foreach (string productId in csvData["geo3NotActive"].Split(','))
            {
                VerifyIsTrue(xmlResult.IndexOf(ConvertGCsvString(productId)) == -1,
                    "(4) not active " + productId);
            }

            VerifyIsTrue(xmlResult.IndexOf(ConvertGCsvString(csvData["geo3PreOrder"])) != -1,
                "(4) preorder");
            productDescriptions = csvData["geo3PDescription4"].Split(',');
            VerifyIsTrue(GetGProductFromXml(xmlResult, "9").IndexOf(ConvertGCsvString(productDescriptions[0])) != -1,
                "(4) description in product 9");
            VerifyIsTrue(GetGProductFromXml(xmlResult, "10").IndexOf(ConvertGCsvString(productDescriptions[1])) != -1,
                "(4) description in product 10");
            VerifyIsTrue(GetGProductFromXml(xmlResult, "11").IndexOf("<g:description>") == -1,
                "(4) description in product 11");
            idArtnoProducts = csvData["geo3IdArtno4"].Split(',');
            foreach (var productId in idArtnoProducts)
            {
                VerifyIsTrue(xmlResult.IndexOf("<item>" + productId) != -1,
                    "(4) id-artno product " + productId);
            }

            GoToAdmin(adminPath);
            RemoveGoogleExport("AdditionalSettingsG");
        }

        [Test]
        public void ExportSettingsYGGoogleExport()
        {
            //gtin, adult, google category
            GoToAdmin(adminPath);
            AddExportFeed("ExportSettingsYG");
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"6_anchor\"]")).Click();
            Driver.ClearToastMessages();

            string exportName = SetCommonGExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.SendKeysInput(By.Name("GoogleProductCategory"), csvData["geo4GPCategoryDefault"]);
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromGFile(Driver, exportPath, exportName);
            string product = GetGProductFromXml(xmlResult, csvData["geo4ProductId1"]);
            VerifyIsTrue(product.IndexOf("<g:adult>") == -1,
                "adult for product 1 found");
            VerifyIsTrue(product.IndexOf("<g:gtin>") == -1,
                "gtin for product 1 found");
            VerifyIsTrue(product.IndexOf(ConvertGCsvString(csvData["geo4GCat1"])) != -1,
                "google_product_category for product 1 not found");

            product = GetGProductFromXml(xmlResult, csvData["geo4ProductId2"]);
            VerifyIsTrue(product.IndexOf("<g:adult>") == -1,
                "adult for product 2 found");
            VerifyIsTrue(product.IndexOf("<g:gtin>") == -1,
                "gtin for product 2 found");
            VerifyIsTrue(product.IndexOf(ConvertGCsvString(csvData["geo4GCat1"])) != -1,
                "google_product_category for product 2 not found");

            product = GetGProductFromXml(xmlResult, csvData["geo4ProductId3"]);

            VerifyIsTrue(product.IndexOf("<g:adult>true</g:adult>") != -1,
                "adult for product 3 not found");
            VerifyIsTrue(product.IndexOf(ConvertGCsvString(csvData["geo4ProductGtin"])) != -1,
                "gtin for product 3 not found");
            VerifyIsTrue(product.IndexOf(ConvertGCsvString(csvData["geo4GCat2"])) != -1,
                "google_product_category for product 3 not found");

            GoToAdmin(adminPath);
            RemoveGoogleExport("ExportSettingsYG");
        }
    }
}