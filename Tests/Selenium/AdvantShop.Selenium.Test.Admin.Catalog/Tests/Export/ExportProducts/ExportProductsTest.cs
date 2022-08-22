using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Export.ExportProducts
{
    [TestFixture]
    public class ExportProductsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Export\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Catalog\\Export\\Catalog.Color.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Photo.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Property.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.PropertyValue.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\Export\\Catalog.ProductPropertyValue.csv"
            );

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
        [Order(0)]
        public void ExportProductsChoiceCateroiesFields()
        {
            GoToAdmin("exportfeeds/exportfeed/2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Выбор товаров')]")).Click();

            choiceCategories();
            selectCategory(0, "TestCategory1", 1, "TestCategory2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Поля выгрузки')]")).Click();
            Driver.WaitForElem(By.Name("0ddl"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportSetNone\"]")).Click();
            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Артикул");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Наименование");
            (new SelectElement(Driver.FindElement(By.Name("2ddl")))).SelectByText("Урл синоним");
            (new SelectElement(Driver.FindElement(By.Name("3ddl")))).SelectByText("Категории");
            (new SelectElement(Driver.FindElement(By.Name("4ddl")))).SelectByText("Активность");
            (new SelectElement(Driver.FindElement(By.Name("5ddl")))).SelectByText(
                "Артикул:Размер:Цвет:Цена:ЗакупочнаяЦена:Наличие");
            (new SelectElement(Driver.FindElement(By.Name("8ddl")))).SelectByText("Производитель");
            (new SelectElement(Driver.FindElement(By.Name("9ddl")))).SelectByText("Изображения");
            (new SelectElement(Driver.FindElement(By.Name("10ddl")))).SelectByText("Скидка (%, процент)");
            (new SelectElement(Driver.FindElement(By.Name("11ddl")))).SelectByText("Свойства");
            Driver.ScrollToTop();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            if (!Driver.FindElement(By.Id("ExportFeedSettings_Active")).Selected)

            {
                VerifyIsFalse(Driver.FindElement(By.Id("ExportFeedSettings_Interval")).Enabled);
                VerifyIsFalse(Driver.FindElement(By.Id("ddlIntervalType")).Enabled);

                Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
                VerifyIsTrue(Driver.FindElement(By.Id("ExportFeedSettings_Interval")).Enabled);
                VerifyIsTrue(Driver.FindElement(By.Id("ddlIntervalType")).Enabled);
            }

            (new SelectElement(Driver.FindElement(By.Name("ddlFileExtention")))).SelectByText("csv");
            (new SelectElement(Driver.FindElement(By.Id("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Id("CsvEnconing")))).SelectByText("UTF-8");
            Driver.FindElement(By.Id("CsvColumSeparator")).Click();
            Driver.FindElement(By.Id("CsvColumSeparator")).Clear();
            Driver.FindElement(By.Id("CsvColumSeparator")).SendKeys(";");
            Driver.FindElement(By.Id("CsvPropertySeparator")).Click();
            Driver.FindElement(By.Id("CsvPropertySeparator")).Clear();
            Driver.FindElement(By.Id("CsvPropertySeparator")).SendKeys(":");
            var priceMargin = Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).GetAttribute("value");
            if (!priceMargin.Equals("0"))
            {
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Click();
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Clear();
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).SendKeys("0");
            }

            Driver.DropFocus("h1");
            Functions.ExportProductsNoInCategoryOff(Driver, BaseUrl);
            Functions.ExportProductsCategorySortOff(Driver, BaseUrl);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountTotal\"]")).Text.Contains("10"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountValue\"]")).Text.Contains("10"));

            GoToClient("catalog.csv?OpenInBrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"));
            VerifyIsTrue(Driver.PageSource.Contains(
                "sku;name;paramsynonym;category;enabled;sku:size:color:price:purchaseprice:amount;producer;photos;discount;properties\r\n1;TestProduct1;test-product1;[TestCategory1];+;1:SizeName1:Color1:1:1:1;BrandName1;281.jpg;1,00;Property1:PropertyValue1\r\n2;TestProduct2;test-product2;[TestCategory1];-;2:SizeName2:Color2:2:2:2;BrandName2;284.jpg;2,00;Property2:PropertyValue2\r\n3;TestProduct3;test-product3;[TestCategory1];-;3:SizeName3:Color3:3:3:3;BrandName3;342.jpg;3,00;Property3:PropertyValue3\r\n4;TestProduct4;test-product4;[TestCategory1];+;4:SizeName4:Color4:4:4:4;BrandName4;348.jpg;4,00;Property4:PropertyValue4\r\n5;TestProduct5;test-product5;[TestCategory1];+;5:SizeName5:Color5:5:5:5;BrandName5;349.jpg;5,00;Property5:PropertyValue5\r\n6;TestProduct6;test-product6;[TestCategory1];+;6:SizeName6:Color6:6:6:6;BrandName6;350.jpg;6,00;Property1:PropertyValue6\r\n7;TestProduct7;test-product7;[TestCategory1];+;7:SizeName7:Color7:7:7:7;BrandName7;351.jpg;7,00;Property2:PropertyValue7\r\n8;TestProduct8;test-product8;[TestCategory1];+;8:SizeName8:Color8:8:8:8;BrandName8;353.jpg;8,00;Property3:PropertyValue8\r\n9;TestProduct9;test-product9;[TestCategory1];+;9:SizeName9:Color9:9:9:9;BrandName9;358.jpg;9,00;Property4:PropertyValue9\r\n10;TestProduct10;test-product10;[TestCategory1];+;10:SizeName10:Color10:10:10:10;BrandName10;360.jpg;10,00;\"Property1:PropertyValue1;Property5:PropertyValue10\""));
        }

        [Test]
        [Order(2)]
        public void ExportProductsAllFieldsAll()
        {
            //check modules
            GoToAdmin("modules");

            bool modules = false;
            if ((!Driver.PageSource.Contains("У вас еще нет установленных модулей")) &&
                (Driver.PageSource.Contains("Комплекты товаров")) && 
                (!Driver.Url.Contains("modules/market")))
            {
                modules = true;
                addProductsSet();
            }

            GoToAdmin("exportfeeds/indexcsv#?exportTab=exportProducts");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddExportFeed\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));
            VerifyAreEqual("Новая выгрузка", Driver.FindElement(By.TagName("h2")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]")).SendKeys("Новая выгрузка");
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("Export"));

            Driver.FindElement(By.XPath("//a[contains(text(), 'Выбор товаров')]")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"AllProducts\"]"));

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"AllProducts\"] input")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AllProducts\"] span")).Click();
            }

            Driver.FindElement(By.XPath("//a[contains(text(), 'Поля выгрузки')]")).Click();
            Driver.WaitForElem(By.Name("0ddl"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportSetNone\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportSetDefault\"]")).Click();

            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            if (Driver.FindElement(By.Id("ExportFeedSettings_Active")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
            }

            (new SelectElement(Driver.FindElement(By.Id("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Id("CsvEnconing")))).SelectByText("UTF-8");
            (new SelectElement(Driver.FindElement(By.Name("ddlFileExtention")))).SelectByText("txt");
            Driver.FindElement(By.Id("ExportFeedSettings_FileName")).Click();
            Driver.FindElement(By.Id("ExportFeedSettings_FileName")).Clear();
            Driver.FindElement(By.Id("ExportFeedSettings_FileName")).SendKeys("ChangeName");
            Driver.FindElement(By.Id("CsvColumSeparator")).Click();
            Driver.FindElement(By.Id("CsvColumSeparator")).Clear();
            Driver.FindElement(By.Id("CsvColumSeparator")).SendKeys(";");
            Driver.FindElement(By.Id("CsvPropertySeparator")).Click();
            Driver.FindElement(By.Id("CsvPropertySeparator")).Clear();
            Driver.FindElement(By.Id("CsvPropertySeparator")).SendKeys(":");
            var priceMargin = Driver.FindElement(By.Id("ExportFeedSettings_PriceMarginInPercents")).GetAttribute("value") +
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMarginInNumbers")).GetAttribute("value");
            if (!priceMargin.Equals("0"))
            {
                Driver.SendKeysInput(By.Id("ExportFeedSettings_PriceMarginInPercents"), "0");
                Driver.SendKeysInput(By.Id("ExportFeedSettings_PriceMarginInNumbers"), "0");
            }

            Driver.DropFocus("h1");
            Functions.ExportProductsNoInCategoryOff(Driver, BaseUrl);
            Functions.ExportProductsCategorySortOff(Driver, BaseUrl);

            Driver.ScrollTo(By.Id("CsvPropertySeparator"));
            if (Driver.FindElement(By.Id("AllOffersToMultiOfferColumn")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AllOffersToMultiOfferColumn\"]"))
                    .FindElement(By.TagName("span")).Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountValue\"]")).Text.Contains("20"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountTotal\"]")).Text.Contains("20"));
           
            GoToClient("ChangeName.txt?OpenInBrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("txt"));
            VerifyIsTrue(Driver.Url.Contains("ChangeName"));

            //all
            if (modules == true)
            {
                VerifyIsTrue(Driver.PageSource.Contains(
                    "sku;name;paramsynonym;category;enabled;currency;price;purchaseprice;amount;sku:size:color:price:purchaseprice:amount;unit;discount;discountamount;shippingprice;weight;size;briefdescription;description;title;metakeywords;metadescription;h1;photos;videos;markers;properties;producer;preorder;salesnote;related sku;alternative sku;custom options;gtin;googleproductcategory;yandexproductcategory;yandextypeprefix;yandexname;yandexmodel;yandexsizeunit;adult;manufacturer_warranty;tags;gifts;minamount;maxamount;multiplicity;cbid;barcode;tax;externalcategoryid;avitoproductproperties;productsets;productsetsdiscount\r\n1;TestProduct1;test-product1;[TestCategory1];+;RUB;;;;1:SizeName1:Color1:1:1:1;unit;1,00;;1,00;1,00;1x1x1;briefDesc1;Desc1;;;;;281.jpg;;r;Property1:PropertyValue1;BrandName1;-;sales note 1;;;;gtin1;Google Product Category1;yandex market category1;yandex type prefix 1;;yandex model 1;AU;-;-;;;1,00;101,00;1,;1;1;Без НДС;;;0,00\r\n2;TestProduct2;test-product2;[TestCategory1];-;RUB;;;;2:SizeName2:Color2:2:2:2;unit;2,00;;2,00;2,00;2x2x2;briefDesc2;Desc2;;;;;284.jpg;;r;Property2:PropertyValue2;BrandName2;-;sales note 2;;;;gtin2;Google Product Category2;yandex market category2;yandex type prefix 2;;yandex model 2;AU;-;-;;;2,00;102,00;1,;2;2;Без НДС;;\"1;10\";50,00\r\n3;TestProduct3;test-product3;[TestCategory1];-;RUB;;;;3:SizeName3:Color3:3:3:3;unit;3,00;;3,00;3,00;3x3x3;briefDesc3;Desc3;;;;;342.jpg;;r;Property3:PropertyValue3;BrandName3;-;sales note 3;;;;gtin3;Google Product Category3;yandex market category3;yandex type prefix 3;;yandex model 3;AU;-;-;;;3,00;103,00;1,;3;3;Без НДС;;;0,00\r\n4;TestProduct4;test-product4;[TestCategory1];+;RUB;;;;4:SizeName4:Color4:4:4:4;unit;4,00;;4,00;0,00;4x4x4;briefDesc4;Desc4;;;;;348.jpg;;n;Property4:PropertyValue4;BrandName4;-;sales note 4;;;;gtin4;Google Product Category4;yandex market category4;yandex type prefix 4;;yandex model 4;AU;-;-;;;4,00;104,00;1,;4;4;Без НДС;;;0,00\r\n5;TestProduct5;test-product5;[TestCategory1];+;RUB;;;;5:SizeName5:Color5:5:5:5;unit;5,00;;5,00;5,00;0x0x0;briefDesc5;Desc5;;;;;349.jpg;;n;Property5:PropertyValue5;BrandName5;-;sales note 5;;;;gtin5;Google Product Category5;yandex market category5;yandex type prefix 5;;yandex model 5;AU;-;-;;;5,00;105,00;1,;5;5;Без НДС;;;0,00\r\n6;TestProduct6;test-product6;[TestCategory1];+;RUB;;;;6:SizeName6:Color6:6:6:6;unit;6,00;;6,00;6,00;6x6x6;briefDesc6;Desc6;;;;;350.jpg;;n;Property1:PropertyValue6;BrandName6;-;sales note 6;;;;gtin6;Google Product Category6;yandex market category6;yandex type prefix 6;;yandex model 6;AU;-;-;;;6,00;106,00;1,;6;6;Без НДС;;;0,00\r\n7;TestProduct7;test-product7;[TestCategory1];+;RUB;;;;7:SizeName7:Color7:7:7:7;unit;7,00;;0,00;7,00;7x7x7;briefDesc7;Desc7;;;;;351.jpg;;b;Property2:PropertyValue7;BrandName7;-;sales note 7;;;;gtin7;Google Product Category7;yandex market category7;yandex type prefix 7;;yandex model 7;AU;-;-;;;7,00;107,00;1,;7;7;Без НДС;;;0,00\r\n8;TestProduct8;test-product8;[TestCategory1];+;RUB;;;;8:SizeName8:Color8:8:8:8;unit;8,00;;8,00;8,00;8x8x8;briefDesc8;Desc8;;;;;353.jpg;;b;Property3:PropertyValue8;BrandName8;-;sales note 8;;;;gtin8;Google Product Category8;yandex market category8;yandex type prefix 8;;yandex model 8;AU;-;-;;;8,00;108,00;1,;8;8;Без НДС;;;0,00\r\n9;TestProduct9;test-product9;[TestCategory1];+;RUB;;;;9:SizeName9:Color9:9:9:9;unit;9,00;;9,00;9,00;9x9x9;briefDesc9;Desc9;;;;;358.jpg;;b;Property4:PropertyValue9;BrandName9;-;sales note 9;;;;gtin9;Google Product Category9;yandex market category9;yandex type prefix 9;;yandex model 9;AU;-;-;;;9,00;109,00;1,;9;9;Без НДС;;;0,00\r\n10;TestProduct10;test-product10;[TestCategory1];+;RUB;;;;10:SizeName10:Color10:10:10:10;unit;10,00;;10,00;10,00;10x10x10;briefDesc10;Desc10;;;;;360.jpg;;s;\"Property1:PropertyValue1;Property5:PropertyValue10\";BrandName10;-;sales note 10;;;;gtin10;Google Product Category10;yandex market category10;yandex type prefix 10;;yandex model 10;AU;-;-;;;10,00;110,00;1,;10;10;Без НДС;;;0,00\r\n11;TestProduct11;test-product11;[TestCategory2];+;RUB;;;;11:SizeName1:Color1:11:11:11;unit;;1,00;11,00;11,00;11x11x11;briefDesc11;Desc11;;;;;;;s;Property1:PropertyValue11;BrandName1;-;sales note 11;;;;gtin11;Google Product Category11;yandex market category11;yandex type prefix 11;;yandex model 11;AU;-;-;;;11,00;111,00;1,;11;11;Без НДС;;;0,00\r\n12;TestProduct12;test-product12;[TestCategory2];+;RUB;;;;12:SizeName2:Color2:12:12:12;unit;;2,00;12,00;12,00;12x12x12;briefDesc12;Desc12;;;;;;;s;Property2:PropertyValue12;BrandName2;-;sales note 12;;;;gtin12;Google Product Category12;yandex market category12;yandex type prefix 12;;yandex model 12;AU;-;-;;;12,00;112,00;1,;12;12;Без НДС;;;0,00\r\n13;TestProduct13;test-product13;[TestCategory2];+;RUB;;;;13:SizeName3:Color3:13:13:13;;;3,00;13,00;13,00;13x13x13;briefDesc13;Desc13;;;;;;;;Property3:PropertyValue13;BrandName3;+;sales note 13;;;;gtin13;Google Product Category13;yandex market category13;yandex type prefix 13;;yandex model 13;AU;-;-;;;13,00;113,00;1,;13;13;Без НДС;;;0,00\r\n14;TestProduct14;test-product14;[TestCategory2];+;RUB;;;;14:SizeName4:Color4:14:14:14;unit;;4,00;14,00;14,00;14x14x14;briefDesc14;Desc14;;;;;;;;Property4:PropertyValue14;BrandName4;+;sales note 14;;;;gtin14;Google Product Category14;yandex market category14;yandex type prefix 14;;yandex model 14;AU;-;-;;;14,00;114,00;1,;14;14;Без НДС;;;0,00\r\n15;TestProduct15;test-product15;[TestCategory2];+;RUB;;;;15:SizeName5:Color5:15:15:15;unit;;5,00;15,00;15,00;15x15x15;briefDesc15;Desc15;;;;;;;;Property5:PropertyValue15;BrandName5;+;sales note 15;;;;gtin15;Google Product Category15;yandex market category15;yandex type prefix 15;;yandex model 15;AU;-;-;;;15,00;115,00;1,;15;15;Без НДС;;;0,00\r\n16;TestProduct16;test-product16;[TestCategory2];+;RUB;;;;16:SizeName6:Color6:16:16:16;unit;;6,00;16,00;16,00;16x16x16;briefDesc16;Desc16;;;;;;;;Property1:PropertyValue16;BrandName6;-;sales note 16;;;;gtin16;Google Product Category16;yandex market category16;yandex type prefix 16;;yandex model 16;AU;-;-;;;16,00;116,00;1,;16;16;Без НДС;;;0,00\r\n17;TestProduct17;test-product17;[TestCategory2];+;RUB;;;;17:SizeName7:Color7:17:17:0;unit;;7,00;17,00;17,00;17x17x17;briefDesc17;Desc17;;;;;;;;Property2:PropertyValue17;BrandName7;-;sales note 17;;;;gtin17;Google Product Category17;yandex market category17;yandex type prefix 17;;yandex model 17;AU;-;-;;;17,00;117,00;1,;17;17;Без НДС;;;0,00\r\n18;TestProduct18;test-product18;[TestCategory2];+;RUB;18,00;18,00;18;;unit;;8,00;18,00;18,00;18x18x18;briefDesc18;Desc18;;;;;;;;Property3:PropertyValue18;BrandName8;-;sales note 18;;;;gtin18;Google Product Category18;yandex market category18;yandex type prefix 18;;yandex model 18;AU;-;-;;;18,00;118,00;1,;18;18;Без НДС;;;0,00\r\n19;TestProduct19;test-product19;[TestCategory2];+;RUB;;;;19:null:Color9:19:19:19;unit;;9,00;19,00;19,00;19x19x19;briefDesc19;Desc19;;;;;;;;Property4:PropertyValue19;BrandName9;-;sales note 19;;;;gtin19;Google Product Category19;yandex market category19;yandex type prefix 19;;yandex model 19;AU;-;-;;;19,00;119,00;1,;19;19;Без НДС;;;0,00\r\n20;TestProduct20;test-product20;[TestCategory2];+;RUB;;;;20:SizeName10:null:20:20:20;unit;;10,00;20,00;20,00;20x20x20;briefDesc20;Desc20;;;;;;;;Property5:PropertyValue20;BrandName10;-;sales note 20;;;;gtin20;Google Product Category20;yandex market category20;yandex type prefix 20;;yandex model 20;Height;-;-;;;20,00;120,00;1,;20;20;Без НДС;;;0,00"));
            }

            else
            {
                VerifyIsTrue(Driver.PageSource.Contains(
                    "sku;name;paramsynonym;category;enabled;currency;price;purchaseprice;amount;sku:size:color:price:purchaseprice:amount;unit;discount;discountamount;shippingprice;weight;size;briefdescription;description;title;metakeywords;metadescription;h1;photos;videos;markers;properties;producer;preorder;salesnote;related sku;alternative sku;custom options;gtin;googleproductcategory;yandextypeprefix;yandexname;yandexmodel;yandexsizeunit;adult;manufacturer_warranty;tags;gifts;minamount;maxamount;multiplicity;bid;barcode;tax;externalcategoryid;avitoproductproperties\r\n1"));
            }
        }

        [Test]
        [Order(0)]
        public void ExportProductsSeparaters()
        {
            GoToAdmin("exportfeeds/exportfeed/2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Выбор товаров')]")).Click();

            choiceCategories();
            selectCategory(0, "TestCategory1", 1, "TestCategory2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Поля выгрузки')]")).Click();
            Driver.WaitForElem(By.Name("0ddl"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportSetNone\"]")).Click();
            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Артикул");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Наименование");
            (new SelectElement(Driver.FindElement(By.Name("2ddl")))).SelectByText("Урл синоним");
            (new SelectElement(Driver.FindElement(By.Name("3ddl")))).SelectByText("Категории");
            (new SelectElement(Driver.FindElement(By.Name("4ddl")))).SelectByText("Свойства");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            if (Driver.FindElement(By.Id("ExportFeedSettings_Active")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
            }

            (new SelectElement(Driver.FindElement(By.Name("ddlFileExtention")))).SelectByText("csv");
            (new SelectElement(Driver.FindElement(By.Id("CsvSeparator")))).SelectByText("Запятая");
            (new SelectElement(Driver.FindElement(By.Id("CsvEnconing")))).SelectByText("UTF-8");
            Driver.FindElement(By.Id("CsvColumSeparator")).Click();
            Driver.FindElement(By.Id("CsvColumSeparator")).Clear();

            Driver.FindElement(By.Id("CsvColumSeparator")).SendKeys(".");
            Driver.FindElement(By.Id("CsvPropertySeparator")).Click();
            Driver.FindElement(By.Id("CsvPropertySeparator")).Clear();

            Driver.FindElement(By.Id("CsvPropertySeparator")).SendKeys("!");
            var priceMargin = Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).GetAttribute("value");
            if (!priceMargin.Equals("0"))
            {
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Click();
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Clear();

                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).SendKeys("0");
            }

            Driver.DropFocus("h1");
            Functions.ExportProductsNoInCategoryOff(Driver, BaseUrl);
            Functions.ExportProductsCategorySortOff(Driver, BaseUrl);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountValue\"]")).Text.Contains("10"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountTotal\"]")).Text.Contains("10"));
            GoToClient("catalog.csv?OpenInBrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.PageSource.Contains(
                "sku,name,paramsynonym,category,properties\r\n1,TestProduct1,test-product1,[TestCategory1],Property1!PropertyValue1\r\n2,TestProduct2,test-product2,[TestCategory1],Property2!PropertyValue2\r\n3,TestProduct3,test-product3,[TestCategory1],Property3!PropertyValue3\r\n4,TestProduct4,test-product4,[TestCategory1],Property4!PropertyValue4\r\n5,TestProduct5,test-product5,[TestCategory1],Property5!PropertyValue5\r\n6,TestProduct6,test-product6,[TestCategory1],Property1!PropertyValue6\r\n7,TestProduct7,test-product7,[TestCategory1],Property2!PropertyValue7\r\n8,TestProduct8,test-product8,[TestCategory1],Property3!PropertyValue8\r\n9,TestProduct9,test-product9,[TestCategory1],Property4!PropertyValue9\r\n10,TestProduct10,test-product10,[TestCategory1],Property1!PropertyValue1.Property5!PropertyValue10"));
        }

        [Test]
        [Order(0)]
        public void ExportProductsNoInCategory()
        {
            GoToAdmin("exportfeeds/exportfeed/2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Выбор товаров')]")).Click();

            choiceCategories();
            selectCategory(0, "TestCategory1", 1, "TestCategory2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Поля выгрузки')]")).Click();
            Driver.WaitForElem(By.Name("0ddl"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportSetNone\"]")).Click();
            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Артикул");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Наименование");
            (new SelectElement(Driver.FindElement(By.Name("2ddl")))).SelectByText("Урл синоним");
            (new SelectElement(Driver.FindElement(By.Name("3ddl")))).SelectByText("Категории");
            (new SelectElement(Driver.FindElement(By.Name("4ddl")))).SelectByText("Свойства");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            if (Driver.FindElement(By.Id("ExportFeedSettings_Active")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
            }

            (new SelectElement(Driver.FindElement(By.Name("ddlFileExtention")))).SelectByText("csv");
            (new SelectElement(Driver.FindElement(By.Id("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Id("CsvEnconing")))).SelectByText("UTF-8");
            Driver.FindElement(By.Id("CsvColumSeparator")).Clear();

            Driver.FindElement(By.Id("CsvColumSeparator")).SendKeys(";");
            Driver.FindElement(By.Id("CsvPropertySeparator")).Clear();

            Driver.FindElement(By.Id("CsvPropertySeparator")).SendKeys(":");
            var priceMargin = Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).GetAttribute("value");
            if (!priceMargin.Equals("0"))
            {
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Click();
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Clear();

                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).SendKeys("0");
            }

            Driver.DropFocus("h1");
            Functions.ExportProductsNoInCategoryOn(Driver, BaseUrl);
            Functions.ExportProductsCategorySortOff(Driver, BaseUrl);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountValue\"]")).Text.Contains("15"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountTotal\"]")).Text.Contains("15"));
            GoToClient("catalog.csv?OpenInBrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.PageSource.Contains(
                "sku;name;paramsynonym;category;properties\r\n1;TestProduct1;test-product1;[TestCategory1];Property1:PropertyValue1\r\n2;TestProduct2;test-product2;[TestCategory1];Property2:PropertyValue2\r\n3;TestProduct3;test-product3;[TestCategory1];Property3:PropertyValue3\r\n4;TestProduct4;test-product4;[TestCategory1];Property4:PropertyValue4\r\n5;TestProduct5;test-product5;[TestCategory1];Property5:PropertyValue5\r\n6;TestProduct6;test-product6;[TestCategory1];Property1:PropertyValue6\r\n7;TestProduct7;test-product7;[TestCategory1];Property2:PropertyValue7\r\n8;TestProduct8;test-product8;[TestCategory1];Property3:PropertyValue8\r\n9;TestProduct9;test-product9;[TestCategory1];Property4:PropertyValue9\r\n10;TestProduct10;test-product10;[TestCategory1];\"Property1:PropertyValue1;Property5:PropertyValue10\"\r\n21;TestProduct21;test-product21;;Property1:PropertyValue1\r\n22;TestProduct22;test-product22;;Property2:PropertyValue2\r\n23;TestProduct23;test-product23;;Property3:PropertyValue3\r\n24;TestProduct24;test-product24;;Property4:PropertyValue4\r\n25;TestProduct25;test-product25;;Property5:PropertyValue5"));
        }

        [Test]
        [Order(0)]
        public void ExportProductsCategorySort()
        {
            GoToAdmin("exportfeeds/exportfeed/2#?exportfeedtab=1");

            choiceCategories();
            selectCategory(0, "TestCategory1", 1, "TestCategory2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Поля выгрузки')]")).Click();
            Driver.WaitForElem(By.Name("0ddl"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportSetNone\"]")).Click();
            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Артикул");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Наименование");
            (new SelectElement(Driver.FindElement(By.Name("2ddl")))).SelectByText("Категории");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            if (Driver.FindElement(By.Id("ExportFeedSettings_Active")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
            }

            (new SelectElement(Driver.FindElement(By.Name("ddlFileExtention")))).SelectByText("csv");
            (new SelectElement(Driver.FindElement(By.Id("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Id("CsvEnconing")))).SelectByText("UTF-8");
            Driver.FindElement(By.Id("CsvColumSeparator")).Click();
            Driver.FindElement(By.Id("CsvColumSeparator")).Clear();

            Driver.FindElement(By.Id("CsvColumSeparator")).SendKeys(";");
            Driver.FindElement(By.Id("CsvPropertySeparator")).Click();
            Driver.FindElement(By.Id("CsvPropertySeparator")).Clear();

            Driver.FindElement(By.Id("CsvPropertySeparator")).SendKeys(":");
            Driver.DropFocus("h1");
            var priceMargin = Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).GetAttribute("value");
            if (!priceMargin.Equals("0"))
            {
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Click();
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Clear();

                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).SendKeys("0");
            }

            Driver.DropFocus("h1");
            Functions.ExportProductsNoInCategoryOff(Driver, BaseUrl);
            Functions.ExportProductsCategorySortOn(Driver, BaseUrl);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountValue\"]")).Text.Contains("10"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountTotal\"]")).Text.Contains("10"));
            GoToClient("catalog.csv?OpenInBrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.PageSource.Contains(
                "sku;name;category;sorting\r\n1;TestProduct1;[TestCategory1];1\r\n2;TestProduct2;[TestCategory1];2\r\n3;TestProduct3;[TestCategory1];3\r\n4;TestProduct4;[TestCategory1];4\r\n5;TestProduct5;[TestCategory1];5\r\n6;TestProduct6;[TestCategory1];6\r\n7;TestProduct7;[TestCategory1];7\r\n8;TestProduct8;[TestCategory1];8\r\n9;TestProduct9;[TestCategory1];9\r\n10;TestProduct10;[TestCategory1];10"));
        }


        [Test]
        [Order(0)]
        public void ExportProductsPriceMargin()
        {
            GoToAdmin("exportfeeds/exportfeed/2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Выбор товаров')]")).Click();

            choiceCategories();
            selectCategory(0, "TestCategory1", 1, "TestCategory2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Поля выгрузки')]")).Click();
            Driver.WaitForElem(By.Name("0ddl"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportSetNone\"]")).Click();
            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Артикул");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Наименование");
            (new SelectElement(Driver.FindElement(By.Name("2ddl")))).SelectByText("Категории");
            (new SelectElement(Driver.FindElement(By.Name("3ddl")))).SelectByText(
                "Артикул:Размер:Цвет:Цена:ЗакупочнаяЦена:Наличие");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            if (Driver.FindElement(By.Id("ExportFeedSettings_Active")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
            }

            (new SelectElement(Driver.FindElement(By.Name("ddlFileExtention")))).SelectByText("csv");
            (new SelectElement(Driver.FindElement(By.Id("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Id("CsvEnconing")))).SelectByText("UTF-8");
            Driver.FindElement(By.Id("CsvColumSeparator")).Clear();

            Driver.FindElement(By.Id("CsvColumSeparator")).SendKeys(";");
            Driver.FindElement(By.Id("CsvPropertySeparator")).Clear();

            Driver.FindElement(By.Id("CsvPropertySeparator")).SendKeys(":");
            Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Clear();

            Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).SendKeys("50");
            Driver.DropFocus("h1");
            Functions.ExportProductsNoInCategoryOff(Driver, BaseUrl);
            Functions.ExportProductsCategorySortOff(Driver, BaseUrl);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountTotal\"]")).Text.Contains("10"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountValue\"]")).Text.Contains("10"));
            GoToClient("catalog.csv?OpenInBrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"));
            VerifyIsTrue(Driver.PageSource.Contains(
                "sku;name;category;sku:size:color:price:purchaseprice:amount\r\n1;TestProduct1;[TestCategory1];1:SizeName1:Color1:1,5:1:1\r\n2;TestProduct2;[TestCategory1];2:SizeName2:Color2:3:2:2\r\n3;TestProduct3;[TestCategory1];3:SizeName3:Color3:4,5:3:3\r\n4;TestProduct4;[TestCategory1];4:SizeName4:Color4:6:4:4\r\n5;TestProduct5;[TestCategory1];5:SizeName5:Color5:7,5:5:5\r\n6;TestProduct6;[TestCategory1];6:SizeName6:Color6:9:6:6\r\n7;TestProduct7;[TestCategory1];7:SizeName7:Color7:10,5:7:7\r\n8;TestProduct8;[TestCategory1];8:SizeName8:Color8:12:8:8\r\n9;TestProduct9;[TestCategory1];9:SizeName9:Color9:13,5:9:9\r\n10;TestProduct10;[TestCategory1];10:SizeName10:Color10:15:10:10"));
        }

        [Test]
        [Order(0)]
        public void ExportProductsMultiOfferAllToColumn()
        {
            GoToAdmin("exportfeeds/exportfeed/2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Выбор товаров')]")).Click();

            choiceCategories();
            selectCategory(1, "TestCategory2", 0, "TestCategory1");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Поля выгрузки')]")).Click();
            Driver.WaitForElem(By.Name("0ddl"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportSetNone\"]")).Click();
            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Артикул");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Наименование");
            (new SelectElement(Driver.FindElement(By.Name("2ddl")))).SelectByText("Цена");
            (new SelectElement(Driver.FindElement(By.Name("3ddl")))).SelectByText("Закупочная цена");
            (new SelectElement(Driver.FindElement(By.Name("4ddl")))).SelectByText("Количество");
            (new SelectElement(Driver.FindElement(By.Name("5ddl")))).SelectByText(
                "Артикул:Размер:Цвет:Цена:ЗакупочнаяЦена:Наличие");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            if (Driver.FindElement(By.Id("ExportFeedSettings_Active")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
            }

            (new SelectElement(Driver.FindElement(By.Name("ddlFileExtention")))).SelectByText("csv");
            (new SelectElement(Driver.FindElement(By.Id("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Id("CsvEnconing")))).SelectByText("UTF-8");
            Driver.FindElement(By.Id("CsvColumSeparator")).Clear();

            Driver.FindElement(By.Id("CsvColumSeparator")).SendKeys(";");
            Driver.FindElement(By.Id("CsvPropertySeparator")).Clear();

            Driver.FindElement(By.Id("CsvPropertySeparator")).SendKeys(":");

            var priceMargin = Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).GetAttribute("value");
            if (!priceMargin.Equals("0"))
            {
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Click();
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Clear();

                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).SendKeys("0");
            }

            Driver.DropFocus("h1");
            Functions.ExportProductsNoInCategoryOff(Driver, BaseUrl);
            Functions.ExportProductsCategorySortOff(Driver, BaseUrl);

            Driver.ScrollTo(By.Id("CsvPropertySeparator"));
            if (!Driver.FindElement(By.Id("AllOffersToMultiOfferColumn")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AllOffersToMultiOfferColumn\"]"))
                    .FindElement(By.TagName("span")).Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountValue\"]")).Text.Contains("10"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountTotal\"]")).Text.Contains("10"));
            GoToClient("catalog.csv?OpenInBrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"));
            VerifyIsTrue(Driver.PageSource.Contains(
                "sku;name;price;purchaseprice;amount;sku:size:color:price:purchaseprice:amount\r\n11;TestProduct11;;;;11:SizeName1:Color1:11:11:11\r\n12;TestProduct12;;;;12:SizeName2:Color2:12:12:12\r\n13;TestProduct13;;;;13:SizeName3:Color3:13:13:13\r\n14;TestProduct14;;;;14:SizeName4:Color4:14:14:14\r\n15;TestProduct15;;;;15:SizeName5:Color5:15:15:15\r\n16;TestProduct16;;;;16:SizeName6:Color6:16:16:16\r\n17;TestProduct17;;;;17:SizeName7:Color7:17:17:0\r\n18;TestProduct18;;;;18:null:null:18:18:18\r\n19;TestProduct19;;;;19:null:Color9:19:19:19\r\n20;TestProduct20;;;;20:SizeName10:null:20:20:20"));
        }

        [Test]
        [Order(0)]
        public void ExportProductsMultiOfferAllToColumnNo()
        {
            GoToAdmin("exportfeeds/exportfeed/2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Выбор товаров')]")).Click();

            choiceCategories();
            selectCategory(1, "TestCategory2", 0, "TestCategory1");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Поля выгрузки')]")).Click();
            Driver.WaitForElem(By.Name("0ddl"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportSetNone\"]")).Click();
            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Артикул");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Наименование");
            (new SelectElement(Driver.FindElement(By.Name("2ddl")))).SelectByText("Цена");
            (new SelectElement(Driver.FindElement(By.Name("3ddl")))).SelectByText("Закупочная цена");
            (new SelectElement(Driver.FindElement(By.Name("4ddl")))).SelectByText("Количество");
            (new SelectElement(Driver.FindElement(By.Name("5ddl")))).SelectByText(
                "Артикул:Размер:Цвет:Цена:ЗакупочнаяЦена:Наличие");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            if (Driver.FindElement(By.Id("ExportFeedSettings_Active")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
            }

            (new SelectElement(Driver.FindElement(By.Name("ddlFileExtention")))).SelectByText("csv");
            (new SelectElement(Driver.FindElement(By.Id("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Id("CsvEnconing")))).SelectByText("UTF-8");
            Driver.FindElement(By.Id("CsvColumSeparator")).Clear();

            Driver.FindElement(By.Id("CsvColumSeparator")).SendKeys(";");
            Driver.FindElement(By.Id("CsvPropertySeparator")).Clear();

            Driver.FindElement(By.Id("CsvPropertySeparator")).SendKeys(":");

            var priceMargin = Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).GetAttribute("value");
            if (!priceMargin.Equals("0"))
            {
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Click();
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Clear();

                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).SendKeys("0");
            }

            Driver.DropFocus("h1");
            Functions.ExportProductsNoInCategoryOff(Driver, BaseUrl);
            Functions.ExportProductsCategorySortOff(Driver, BaseUrl);

            Driver.ScrollTo(By.Id("CsvPropertySeparator"));
            if (Driver.FindElement(By.Id("AllOffersToMultiOfferColumn")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AllOffersToMultiOfferColumn\"]"))
                    .FindElement(By.TagName("span")).Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountValue\"]")).Text.Contains("10"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountTotal\"]")).Text.Contains("10"));
            GoToClient("catalog.csv?OpenInBrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"));
            VerifyIsTrue(Driver.PageSource.Contains(
                "sku;name;price;purchaseprice;amount;sku:size:color:price:purchaseprice:amount\r\n11;TestProduct11;;;;11:SizeName1:Color1:11:11:11\r\n12;TestProduct12;;;;12:SizeName2:Color2:12:12:12\r\n13;TestProduct13;;;;13:SizeName3:Color3:13:13:13\r\n14;TestProduct14;;;;14:SizeName4:Color4:14:14:14\r\n15;TestProduct15;;;;15:SizeName5:Color5:15:15:15\r\n16;TestProduct16;;;;16:SizeName6:Color6:16:16:16\r\n17;TestProduct17;;;;17:SizeName7:Color7:17:17:0\r\n18;TestProduct18;18,00;18,00;18;\r\n19;TestProduct19;;;;19:null:Color9:19:19:19\r\n20;TestProduct20;;;;20:SizeName10:null:20:20:20"));
        }

        [Test]
        [Order(0)]
        public void ExportProductzExcluded()
        {
            GoToAdmin("exportfeeds/exportfeed/2");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Выбор товаров')]")).Click();

            choiceCategories();
            selectCategory(0, "TestCategory1", 1, "TestCategory2");

            //set excluded products
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector("[grid-unique-id=\"gridProductsSelectvizr\"]"));
            Driver.XPathContainsText("span", "TestCategory1");
            Driver.GetGridCell(0, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.GetGridCell(1, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.GetGridCell(2, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();

            Driver.XPathContainsText("span", "TestCategory2");
            Driver.GetGridCell(3, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.FindElement(By.CssSelector(".modal-header .close")).Click();

            //check excluded products
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector("[grid-unique-id=\"gridProductsSelectvizr\"]"));
            Driver.XPathContainsText("span", "TestCategory1");
            VerifyIsTrue(Driver.GetGridCell(0, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(3, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(4, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);

            Driver.XPathContainsText("span", "TestCategory2");
            VerifyIsFalse(Driver.GetGridCell(0, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(1, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(2, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(3, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);

            Driver.FindElement(By.CssSelector(".modal-header .close")).Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Поля выгрузки')]")).Click();
            Driver.WaitForElem(By.Name("0ddl"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportSetNone\"]")).Click();
            (new SelectElement(Driver.FindElement(By.Name("0ddl")))).SelectByText("Артикул");
            (new SelectElement(Driver.FindElement(By.Name("1ddl")))).SelectByText("Наименование");
            (new SelectElement(Driver.FindElement(By.Name("3ddl")))).SelectByText("Категории");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Параметры выгрузки')]")).Click();
            Driver.WaitForElem(By.Id("Name"));

            if (Driver.FindElement(By.Id("ExportFeedSettings_Active")).Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
            }

            (new SelectElement(Driver.FindElement(By.Name("ddlFileExtention")))).SelectByText("csv");
            (new SelectElement(Driver.FindElement(By.Id("CsvSeparator")))).SelectByText("Точка с запятой");
            (new SelectElement(Driver.FindElement(By.Id("CsvEnconing")))).SelectByText("UTF-8");
            Driver.FindElement(By.Id("CsvColumSeparator")).Clear();

            Driver.FindElement(By.Id("CsvColumSeparator")).SendKeys(";");
            Driver.FindElement(By.Id("CsvPropertySeparator")).Clear();

            Driver.FindElement(By.Id("CsvPropertySeparator")).SendKeys(":");
            var priceMargin = Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).GetAttribute("value");
            if (!priceMargin.Equals("0"))
            {
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Click();
                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).Clear();

                Driver.FindElement(By.Id("ExportFeedSettings_PriceMargin")).SendKeys("0");
            }

            Driver.DropFocus("h1");
            Functions.ExportProductsNoInCategoryOff(Driver, BaseUrl);
            Functions.ExportProductsCategorySortOff(Driver, BaseUrl);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.ClassName("setting-label-wrap"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountTotal\"]")).Text.Contains("7"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportCountValue\"]")).Text.Contains("7"));
            GoToClient("catalog.csv?OpenInBrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.PageSource.Contains(
                "sku;name;category\r\n4;TestProduct4;[TestCategory1]\r\n5;TestProduct5;[TestCategory1]\r\n6;TestProduct6;[TestCategory1]\r\n7;TestProduct7;[TestCategory1]\r\n8;TestProduct8;[TestCategory1]\r\n9;TestProduct9;[TestCategory1]\r\n10;TestProduct10;[TestCategory1]"));
        }

        public void selectCategory(int idSelect, string categorySelect, int idNotSelect, string categoryNotSelect)
        {
            if (!Driver.FindElements(By.ClassName("jstree-anchor"))[idSelect].GetAttribute("aria-selected").Equals("true"))
            {
                Driver.FindElement(By.XPath("//span[contains(text(), '" + categorySelect + "')]")).Click();
            }

            if (Driver.FindElements(By.ClassName("jstree-anchor"))[idNotSelect].GetAttribute("aria-selected").Equals("true"))
            {
                Driver.FindElement(By.XPath("//span[contains(text(), '" + categoryNotSelect + "')]")).Click();
            }
        }

        public void choiceCategories()
        {
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"] input")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"] span")).Click();
            }
        }

        public void addProductsSet()
        {
            GoToAdmin("product/edit/2");
            Driver.XPathContainsText("div", "Комплекты товаров");
            Driver.FindElement(By.Name("ProductSetDiscount")).Click();
            Driver.FindElement(By.Name("ProductSetDiscount")).Clear();
            Driver.FindElement(By.Name("ProductSetDiscount")).SendKeys("50");
            Driver.XPathContainsText("h2", "Комплекты товаров");

            Driver.FindElement(By.CssSelector("[uid=\"tabProductSets\"]")).FindElement(By.TagName("ui-modal-trigger"))
                .Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[uid=\"tabProductSets\"]")).FindElement(By.TagName("ui-modal-trigger"))
                .Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.XPathContainsText("button", "Выбрать");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();
        }
    }
}