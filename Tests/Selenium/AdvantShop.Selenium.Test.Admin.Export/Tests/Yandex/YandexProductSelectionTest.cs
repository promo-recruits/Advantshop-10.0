using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexProductSelectionTest : ExportServices
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
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.Brand.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\YandexDefault\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile("Data\\Admin\\Export\\TestSettings\\YandexProductSelectionData.csv");
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
        public void AllCategoriesYandexExport()
        {
            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName1"]);

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AllProducts\"]")).Click();

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            Driver.CheckBoxCheck("ExportProductProperties");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Console.Error.WriteLine("begin GetXmlFromYZip");
            string xmlResult = GetXmlFromYZip(exportName).ToLower();
            Console.Error.WriteLine("end GetXmlFromYZip");
            VerifyIsTrue((xmlResult.IndexOf(ConvertCsvString(csvData["estrAllCategories1"])) != -1),
                "categories not expected");
            Console.Error.WriteLine("end estrAllCategories1 - categories");
            VerifyIsTrue((xmlResult.IndexOf(ConvertCsvString(csvData["estrAllCategories2"])) != -1),
                "offers not expected");
            Console.Error.WriteLine("end estrAllCategories1 - offers");

            GoToAdmin(adminPath);
            Driver.FindElement(By.LinkText(csvData["testName1"])).Click();
            Driver.WaitForElem(By.ClassName("link-danger"));
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void SelectedCategoriesYandexExport()
        {
            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName2"]);


            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            //выбор экспортируемых каtegорий
            Driver.FindElement(By.CssSelector(".jstree-closed[id=\"1\"] i.jstree-icon.jstree-ocl")).Click();
            Driver.FindElement(By.CssSelector("[id=\"4_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            //704 и выше
            Driver.CheckBoxCheck("ExportProductProperties");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName).ToLower();

            VerifyIsTrue((xmlResult.IndexOf(ConvertCsvString(csvData["estrSelectedCat1"])) != -1),
                "categories not expected");
            VerifyIsTrue((xmlResult.IndexOf(ConvertCsvString(csvData["estrSelectedCat2"])) != -1),
                "offers not expected");

            GoToAdmin(adminPath);
            Driver.FindElement(By.LinkText(csvData["testName2"])).Click();
            Driver.WaitForElem(By.ClassName("link-danger"));
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void ExcludeProductYandexExport()
        {
            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName3"]);

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            //выбор экспортируемых каtegорий
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            //исключение товаров
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".jstree-closed[id=\"1\"] i.jstree-icon.jstree-ocl")).Click();
            Driver.FindElement(By.CssSelector("[id=\"4_anchor\"]")).Click();
            Driver.GetGridCellInputForm(0, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"5_anchor\"]")).Click();
            Driver.GetGridCellInputForm(1, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.FindElement(By.CssSelector("ui-modal-cross .close")).Click();

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            //704 и выше
            Driver.CheckBoxCheck("ExportProductProperties");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName).ToLower();

            VerifyIsTrue((xmlResult.IndexOf(ConvertCsvString(csvData["estrExcludeProd1"])) != -1),
                "categories not expected");
            VerifyIsTrue((xmlResult.IndexOf(ConvertCsvString(csvData["estrExcludeProd2"])) != -1),
                "offers not expected");

            GoToAdmin(adminPath);
            Driver.FindElement(By.LinkText(csvData["testName3"])).Click();
            Driver.WaitForElem(By.ClassName("link-danger"));
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }
    }
}