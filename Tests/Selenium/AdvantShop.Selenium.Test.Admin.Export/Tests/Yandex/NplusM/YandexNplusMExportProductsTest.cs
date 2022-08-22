using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex.NplusM
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexNplusMExportProductsTest : ExportServices
    {
        string adminPath = "exportfeeds/indexyandex";
        string exportPath = "export/";
        Dictionary<string, string> csvData;
        int uibtabIndex = 7;

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
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.Tag.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\Products\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile(
                "Data\\Admin\\Export\\TestSettings\\YandexPromos\\NplusMExportProductData.csv");
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
        public void YandexNplusMPSelected()
        {
            //выгрузка избранных товаров, избранных каtegорий. Отсутствие промо, если товар и каtegория не участвуют
            //+ исключенные
            GoToAdmin(adminPath);
            AddExportFeed("NplusMPSelected");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector(".jstree-closed[id=\"3\"] i.jstree-icon.jstree-ocl")).Click();
            Driver.FindElement(By.CssSelector("[id=\"4_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.GetGridCellInputForm(9, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.FindElement(By.CssSelector("ui-modal-cross .close")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Thread.Sleep(1000);
            AddNplusM("NplusMPSelected1", csvData["npm1ProductID1"].Split(',').ToList());
            AddNplusM("NplusMPSelected2", csvData["npm1ProductID2"].Split(',').ToList());
            AddNplusM("NplusMPSelected3", null, csvData["npm1CategoryID1"].Split(',').ToList());
            AddNplusM("NplusMPSelected4", null, csvData["npm1CategoryID2"].Split(',').ToList());
            AddNplusM("NplusMPSelected5", csvData["npm1ProductID1"].Split(',').ToList(),
                csvData["npm1CategoryID1"].Split(',').ToList());
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            string promos = GetPromosFromXml(xmlResult);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["npm1ProductCount"]),
                "количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(promos, "<promo ") == Convert.ToInt32(csvData["npm1PromoCount"]),
                "количество tegs promo not expected");
            VerifyAreEqual(GetTegContent(GetPromoFromXml(promos, 1), "purchase"),
                ConvertCsvString(csvData["npm1PurchaseContent1"]),
                "(1)Содержимое tegа purchase not expected");
            VerifyAreEqual(GetTegContent(GetPromoFromXml(promos, 2), "purchase"),
                ConvertCsvString(csvData["npm1PurchaseContent2"]),
                "(2)Содержимое tegа purchase not expected");
            VerifyAreEqual(GetTegContent(GetPromoFromXml(promos, 3), "purchase"),
                ConvertCsvString(csvData["npm1PurchaseContent3"]),
                "(3)Содержимое tegа purchase not expected");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexNplusMPDisabled()
        {
            //выгрузка избранных товаров, избранных каtegорий. Отсутствие промо, если товар и каtegория не участвуют
            //+ исключенные
            GoToAdmin(adminPath);
            AddExportFeed("NplusMPDisabled");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Thread.Sleep(1000);
            AddNplusM("NplusMPDisabled", csvData["npm2ProductID"].Split(',').ToList());
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["npm2ProductCount1"]),
                "(1)количество товаров в выгрузке not expected");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") == -1,
                "(1)В выгрузке найдена подстрока promos");
            ReturnToExport();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Thread.Sleep(1000);
            Driver.CheckBoxCheck("ExportNotAvailable");
            Driver.CheckBoxCheck("AllowPreOrderProducts");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["npm2ProductCount2"]),
                "(2)количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(2)количество tegs promo not expected");
            VerifyAreEqual(GetTegContent(GetPromoFromXml(GetPromosFromXml(xmlResult), 1), "purchase"),
                ConvertCsvString(csvData["npm2PurchaseContent"]),
                "(1)Содержимое tegа purchase not expected");
            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexNplusMPModification()
        {
            //без модификаций, с модификациями, продублировать с другими артикулами
            GoToAdmin(adminPath);
            AddExportFeed("NplusMPModification");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Thread.Sleep(1000);
            AddNplusM("NplusMPModification", csvData["npm3ProductID"].Split(',').ToList());
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["npm3ProductCount1"]),
                "(1)количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(1)количество tegs promo not expected");
            VerifyAreEqual(GetTegContent(GetPromosFromXml(xmlResult), "purchase"),
                ConvertCsvString(csvData["npm3PurchaseContent1"]),
                "(1)Содержимое tegа purchase not expected");
            ReturnToExport();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Thread.Sleep(1000);
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["npm3ProductCount2"]),
                "(2)количество товаров в выгрузке not expected");
            VerifyAreEqual(GetTegContent(GetPromosFromXml(xmlResult), "purchase"),
                ConvertCsvString(csvData["npm3PurchaseContent2"]),
                "(2)Содержимое tegа purchase not expected");
            ReturnToExport();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Thread.Sleep(1000);
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            SelectOption("ddlOfferIdType", "artno");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["npm3ProductCount1"]),
                "(3)количество товаров в выгрузке not expected");
            VerifyAreEqual(GetTegContent(GetPromosFromXml(xmlResult), "purchase"),
                ConvertCsvString(csvData["npm3PurchaseContent3"]),
                "(3)Содержимое tegа purchase not expected");
            ReturnToExport();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Thread.Sleep(1000);
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["npm3ProductCount2"]),
                "(4)количество товаров в выгрузке not expected");
            VerifyAreEqual(GetTegContent(GetPromosFromXml(xmlResult), "purchase"),
                ConvertCsvString(csvData["npm3PurchaseContent4"]),
                "(4)Содержимое tegа purchase not expected");
            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }
    }
}