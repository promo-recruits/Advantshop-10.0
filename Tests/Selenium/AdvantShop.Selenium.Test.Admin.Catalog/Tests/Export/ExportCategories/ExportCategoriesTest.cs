using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Export.ExportCategories
{
    [TestFixture]
    public class ExportCategoriesTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Export\\ExportCategories\\Catalog.Brand.csv",
                "Data\\Admin\\Catalog\\Export\\ExportCategories\\Catalog.Color.csv",
                "data\\Admin\\Catalog\\Export\\ExportCategories\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\Export\\ExportCategories\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Export\\ExportCategories\\Catalog.Photo.csv",
                "data\\Admin\\Catalog\\Export\\ExportCategories\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Export\\ExportCategories\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Export\\ExportCategories\\Catalog.ProductCategories.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("exportfeeds/indexcsv#?exportTab=exportCategories");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void ExportProductsChoiceCateroiesFields()
        {
            (new SelectElement(Driver.FindElement(By.Name("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Name("CsvEncoding")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportSetNone\"]")).Click();

            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Id");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Название");
            (new SelectElement(Driver.FindElement(By.Name("2ddl")))).SelectByText("Урл синоним");
            (new SelectElement(Driver.FindElement(By.Name("3ddl")))).SelectByText("Id род. категории");
            (new SelectElement(Driver.FindElement(By.Name("4ddl")))).SelectByText("Сортировка");
            (new SelectElement(Driver.FindElement(By.Name("5ddl")))).SelectByText("Активность");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCategoriesCount\"]")).Text
                .Contains("11 / 11"));
            GoToClient("content/price_temp/export_categories.csv?OpenInBrowser=true");

            VerifyIsTrue(Driver.Url.Contains("csv"));
            VerifyIsTrue(Driver.PageSource.Contains(
                "categoryid;name;slug;parentcategory;sortorder;enabled\r\n0;Каталог;catalog;0;0;+\r\n1;TestCategory1;test-category1;0;1;+\r\n2;TestCategory2;test-category2;0;2;+\r\n3;TestCategory3;test-category3;0;3;+\r\n4;TestCategory4;test-category4;0;4;+\r\n5;TestCategory5;test-category5;0;5;+\r\n6;TestCategory6;test-category6;1;6;+\r\n7;TestCategory7;test-category7;1;7;+\r\n8;TestCategory8;test-category8;2;8;+\r\n9;TestCategory9;test-category9;2;9;-\r\n10;TestCategory10;test-category10;5;10;+"));
        }

        [Test]
        public void ExportProductsNullCateroiesFields()
        {
            (new SelectElement(Driver.FindElement(By.Name("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Name("CsvEncoding")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportSetNone\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsFalse(Driver.Url.Contains("exportcategories/export"));
            VerifyIsTrue(Driver.Url.Contains("exportfeeds/indexcsv#?exportTab=exportCategories"));
        }

        [Test]
        public void ExportProductsAllFieldsAll()
        {
            (new SelectElement(Driver.FindElement(By.Name("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Name("CsvEncoding")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportSetDefault\"]")).Click();

            IWebElement selectElem = Driver.FindElement(By.Name("0ddl"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Id"));

            selectElem = Driver.FindElement(By.Name("23ddl"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Вложенность категорий"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCategoriesCount\"]")).Text
                .Contains("11 / 11"));
            GoToClient("content/price_temp/export_categories.csv?OpenInBrowser=true");

            VerifyIsTrue(Driver.Url.Contains("csv"));
            string exportTextEtalon =
                "categoryid;externalid;name;slug;parentcategory;sortorder;enabled;hidden;briefdescription;description;displaystyle;sorting;displaybrandsinmenu;displaysubcategoriesinmenu;tags;picture;minipicture;icon;title;metakeywords;metadescription;h1;propertygroups;categoryhierarchy\r\n0;0;Каталог;catalog;0;0;+;-;;;Tile;NoSorting;+;-;;;;;;;;;;[]\r\n1;;TestCategory1;test-category1;0;1;+;-;;;Tile;NoSorting;-;-;;;;;;;;;;[TestCategory1]\r\n2;;TestCategory2;test-category2;0;2;+;-;;;Tile;NoSorting;-;-;;;;;;;;;;[TestCategory2]\r\n3;;TestCategory3;test-category3;0;3;+;-;;;Tile;NoSorting;-;-;;;;;;;;;;[TestCategory3]\r\n4;;TestCategory4;test-category4;0;4;+;-;;;Tile;NoSorting;-;-;;;;;;;;;;[TestCategory4]\r\n5;;TestCategory5;test-category5;0;5;+;-;;;Tile;NoSorting;-;-;;;;;;;;;;[TestCategory5]\r\n6;;TestCategory6;test-category6;1;6;+;-;;;Tile;NoSorting;-;-;;;;;;;;;;[TestCategory1 >> TestCategory6]\r\n7;;TestCategory7;test-category7;1;7;+;-;;;Tile;NoSorting;-;-;;;;;;;;;;[TestCategory1 >> TestCategory7]\r\n8;;TestCategory8;test-category8;2;8;+;-;;;Tile;NoSorting;-;-;;;;;;;;;;[TestCategory2 >> TestCategory8]\r\n9;;TestCategory9;test-category9;2;9;-;-;;;Tile;NoSorting;-;-;;;;;;;;;;[TestCategory2 >> TestCategory9]\r\n10;;TestCategory10;test-category10;5;10;+;-;;;Tile;NoSorting;-;-;;;;;;;;;;[TestCategory5 >> TestCategory10]";
            string pageSourceTest = Driver.FindElement(By.TagName("pre")).Text;
            VerifyIsTrue(exportTextEtalon.Equals(pageSourceTest));
        }

        [Test]
        public void ExportProductsChoiceCateroiesFieldsTrimСomma()
        {
            (new SelectElement(Driver.FindElement(By.Name("CsvSeparator")))).SelectByText("Запятая");
            (new SelectElement(Driver.FindElement(By.Name("CsvEncoding")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportSetNone\"]")).Click();

            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Id");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Название");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));
            
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCategoriesCount\"]")).Text
                .Contains("11 / 11"));
            GoToClient("content/price_temp/export_categories.csv?OpenInBrowser=true");

            VerifyIsTrue(Driver.Url.Contains("csv"));
            VerifyIsTrue(Driver.PageSource.Contains(
                "categoryid,name\r\n0,Каталог\r\n1,TestCategory1\r\n2,TestCategory2\r\n3,TestCategory3\r\n4,TestCategory4\r\n5,TestCategory5\r\n6,TestCategory6\r\n7,TestCategory7\r\n8,TestCategory8\r\n9,TestCategory9\r\n10,TestCategory10"));
        }

        [Test]
        public void ExportProductsChoiceCateroiesFieldsTrimTabu()
        {
            GoToAdmin("exportfeeds/indexcsv#?exportTab=exportCategories");

            (new SelectElement(Driver.FindElement(By.Name("CsvSeparator")))).SelectByText("Символ табуляции");
            (new SelectElement(Driver.FindElement(By.Name("CsvEncoding")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportSetNone\"]")).Click();

            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Id");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Название");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCategoriesCount\"]")).Text
                .Contains("11 / 11"));

            GoToClient("content/price_temp/export_categories.csv?OpenInBrowser=true");

            VerifyIsTrue(Driver.Url.Contains("csv"));
            VerifyAreEqual(
                "categoryid name\r\n0 Каталог\r\n1 TestCategory1\r\n2 TestCategory2\r\n3 TestCategory3\r\n4 TestCategory4\r\n5 TestCategory5\r\n6 TestCategory6\r\n7 TestCategory7\r\n8 TestCategory8\r\n9 TestCategory9\r\n10 TestCategory10",
                Driver.FindElement(By.TagName("pre")).Text);
        }
    }
}