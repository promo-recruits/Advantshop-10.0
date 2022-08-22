using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Google
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class GoogleProductSelectionTest : ExportServices
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
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.Brand.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\GoogleDefault\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile("Data\\Admin\\Export\\TestSettings\\GoogleProductSelectionData.csv");
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
        public void AllProductGoogleExport()
        {
            //проверка кол-ва продуктов, проверка айдишников выгруженных продуктов,
            //выгрузка проводится с выключенным предзаказом, выключ недост товарами, только гл модификацией, id модификации
            GoToAdmin(adminPath);
            AddExportFeed("AllProductGExport");
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AllProducts\"]")).Click();

            string exportName = SetCommonGExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromGFile(Driver, exportPath, exportName);
            int productCount = Convert.ToInt32(csvData["gps1ProductCount"]);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<item>") == productCount,
                "excluded product google export - countError");

            string[] expectedProducts = csvData["gps1ProductArray"].Split(',');
            for (int productId = 1; productId <= productCount; productId++)
            {
                VerifyIsTrue(
                    GetGProductFromXml(xmlResult, productId)
                        .IndexOf(ConvertGCsvString(expectedProducts[productId - 1])) != -1,
                    productId + " product does not match the expected ");
            }

            GoToAdmin(adminPath);
            RemoveGoogleExport("AllProductGExport");
        }

        [Test]
        public void SelectedCategoriesGoogleExport()
        {
            //проверка кол-ва продуктов, проверка айдишников выгруженных продуктов,
            //выгрузка проводится с выключенным предзаказом, 
            //выключ недост товарами, только гл модификацией, id модификации

            GoToAdmin(adminPath);
            AddExportFeed("SelectedCategoriesGExport");
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.ClearToastMessages();
            string exportName = SetCommonGExportSettings("SelectedCategoriesGExport", exportPath);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromGFile(Driver, exportPath, exportName);
            int productCount = Convert.ToInt32(csvData["gps2ProductCount"]);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<item>") == productCount,
                "selected categories google export - countError");

            string[] expectedProducts = csvData["gps2ProductArray"].Split(',');
            for (int productId = 1; productId <= productCount; productId++)
            {
                VerifyIsTrue(GetGProductFromXml(xmlResult, productId)
                        .IndexOf(ConvertGCsvString(expectedProducts[productId - 1])) != -1,
                    productId + " product does not match the expected ");
            }

            GoToAdmin(adminPath);
            RemoveGoogleExport("SelectedCategoriesGExport");
        }

        [Test]
        public void ExcludedProductsGoogleExport()
        {
            //проверка кол-ва продуктов, проверка айдишников выгруженных продуктов,
            //выгрузка проводится с выключенным предзаказом, 
            //выключ недост товарами, только гл модификацией, id модификации

            GoToAdmin(adminPath);
            AddExportFeed("ExcludedProductsGExport");
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            string[] excludedProducts = csvData["gps3ProductExcludedIdArray"].Split(',');
            foreach (string productName in excludedProducts)
            {
                Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), "TestProduct" + productName);
                Thread.Sleep(500);
                Driver.GetGridCellInputForm(0, "ExcludeFromExport", "ProductsSelectvizr")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
                Driver.ClearToastMessages();
                Thread.Sleep(500);
            }

            Driver.FindElement(By.CssSelector("ui-modal-cross .close")).Click();

            string exportName = SetCommonGExportSettings("ExcludedProductsGExport", exportPath);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromGFile(Driver, exportPath, exportName);
            int productCount = Convert.ToInt32(csvData["gps3ProductCount"]);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<item>") == productCount,
                "excluded product google export - countError");

            string[] expectedProducts = csvData["gps3ProductArray"].Split(',');
            for (int productId = 1; productId <= productCount; productId++)
            {
                VerifyIsTrue(GetGProductFromXml(xmlResult, productId)
                        .IndexOf(ConvertGCsvString(expectedProducts[productId - 1])) != -1,
                    productId + " product does not match the expected ");
            }

            GoToAdmin(adminPath);
            RemoveGoogleExport("ExcludedProductsGExport");
        }
    }
}