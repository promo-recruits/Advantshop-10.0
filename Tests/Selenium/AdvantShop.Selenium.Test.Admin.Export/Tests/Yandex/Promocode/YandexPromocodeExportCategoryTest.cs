using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex.Promocode
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexPromocodeExportCategoryTest : ExportServices
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
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.Brand.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.Tag.csv",
                "Data\\Admin\\Export\\Catalog\\Promocode\\Category\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile(
                "Data\\Admin\\Export\\TestSettings\\YandexPromos\\PromocodeExportCategoryData.csv");
            InitializeService.YandexChannelActive();
            couponVal = csvData["pecCouponValueCur1"];
            couponCur = csvData["pecCouponCur"];

            Init();
            //Functions.SingInYandex(Driver);
            //Для всех тестов условие: --- в промо-акции только каtegории, без оферов
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
        public void YandexPromocodeECatAll()
        {
            //купон для всех каtegорий при экспорте всх каtegорий; купон для всех каtegорий при экспорте 2 каtegорий;

            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName1"]);
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pec1Name"]);
            AddCoupon(csvData["pec1CouponName"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pec1CouponName"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();

            Driver.ClearToastMessages();
            //Driver.ClearToastMessages();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pec1CategoryCount1"]),
                "(1)Количество выгруженных каtegорий not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pec1PromosCount1"]),
                "(1)Количество tegs promo не совпало с ожидаемым");
            string promos = GetPromosFromXml(xmlResult);
            string promo = GetPromoFromXml(promos, 1);
            VerifyIsTrue(
                CountOfStrInXml(promo, "<product category-id=") == Convert.ToInt32(csvData["pec1PromosCategoryCount1"]),
                "(1)Количество каtegорий в tegе предложений not expected");
            VerifyIsTrue(promo.IndexOf("<product offer-id=") == -1,
                "(1)В промо-акции найден teg-предложение продукта, ожидались только каtegории");
            VerifyIsTrue(GetTegContent(promo, "purchase") == ConvertCsvString(csvData["pec1Promos1"]),
                "(1)Содержимое tegа purchase not expected с эталоном");

            ReturnToExport();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"4_anchor\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pec1CategoryCount2"]),
                "(2)Количество выгруженных каtegорий not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pec1PromosCount2"]),
                "(2)Количество tegs promo не совпало с ожидаемым");
            promos = GetPromosFromXml(xmlResult);
            promo = GetPromoFromXml(promos, 1);
            VerifyIsTrue(
                CountOfStrInXml(promo, "<product category-id=") == Convert.ToInt32(csvData["pec1PromosCategoryCount2"]),
                "(2)Количество каtegорий в tegе предложений not expected");
            VerifyIsTrue(promo.IndexOf("<product offer-id=") == -1,
                "(2)В промо-акции найден teg-предложение продукта, ожидались только каtegории");
            VerifyIsTrue(GetTegContent(promo, "purchase") == ConvertCsvString(csvData["pec1Promos2"]),
                "(2)Содержимое tegа purchase not expected с эталоном");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexPromocodeECatSelected()
        {
            //купон для неск.кат-й при экспорте всех кат-й; купон для неск.кат, в выгрузке все уч.; купон для неск.кат, в выгрузке чыасть уч., часть неуч.;

            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName2"]);
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"4_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"5_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"7_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.GetGridCellInputForm(0, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            //Driver.GetGridCell(1, "ExcludeFromExport", "ProductsSelectvizr").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.FindElement(By.CssSelector("ui-modal-cross .close")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            //promo for 1-5 category
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pec2Name1"]);
            AddCoupon(csvData["pec2CouponName1"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"4_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"5_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pec2CouponName1"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();

            Driver.ClearToastMessages();
            //promo for 6-7 category
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pec2Name2"]);
            AddCoupon(csvData["pec2CouponName2"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"1\"] i")).Click();
            Driver.FindElement(By.CssSelector("[id=\"6_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"7_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pec2CouponName2"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();

            Driver.ClearToastMessages();

            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pec2CategoryCount1"]),
                "(1)Количество выгруженных каtegорий not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pec2PromosCount1"]),
                "(1)Количество tegs promo не совпало с ожидаемым");
            string promos = GetPromosFromXml(xmlResult);
            string promo = GetPromoFromXml(promos, 1);
            VerifyIsTrue(
                CountOfStrInXml(promo, "<product category-id=") ==
                Convert.ToInt32(csvData["pec2PromosCategoryCount11"]),
                "(1)Количество каtegорий оффера 1 в tegе предложений not expected");
            VerifyIsTrue(promo.IndexOf("<product offer-id=") == -1,
                "(1)В промо-акции 1 найден teg-предложение продукта, ожидались только каtegории");
            VerifyIsTrue(GetTegContent(promo, "purchase") == ConvertCsvString(csvData["pec2Promos11"]),
                "(1)Содержимое tegа purchase оффера 1 not expected с эталоном");
            promo = GetPromoFromXml(promos, 2);
            VerifyIsTrue(
                CountOfStrInXml(promo, "<product category-id=") ==
                Convert.ToInt32(csvData["pec2PromosCategoryCount12"]),
                "(1)Количество каtegорий оффера 2 в tegе предложений not expected");
            VerifyIsTrue(promo.IndexOf("<product offer-id=") == -1,
                "(1)В промо-акции 2 найден teg-предложение продукта, ожидались только каtegории");
            VerifyIsTrue(GetTegContent(promo, "purchase") == ConvertCsvString(csvData["pec2Promos12"]),
                "(1)Содержимое tegа purchase оффера 2 not expected с эталоном");

            ReturnToExport();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pec2CategoryCount2"]),
                "(2)Количество выгруженных каtegорий not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pec2PromosCount2"]),
                "(2)Количество tegs promo не совпало с ожидаемым");
            promos = GetPromosFromXml(xmlResult);
            promo = GetPromoFromXml(promos, 1);
            VerifyIsTrue(
                CountOfStrInXml(promo, "<product category-id=") ==
                Convert.ToInt32(csvData["pec2PromosCategoryCount21"]),
                "(2)Количество каtegорий оффера 1 в tegе предложений not expected");
            VerifyIsTrue(promo.IndexOf("<product offer-id=") == -1,
                "(2)В промо-акции 1 найден teg-предложение продукта, ожидались только каtegории");
            VerifyIsTrue(GetTegContent(promo, "purchase") == ConvertCsvString(csvData["pec2Promos21"]),
                "(2)Содержимое tegа purchase оффера 1 not expected с эталоном");
            promo = GetPromoFromXml(promos, 2);
            VerifyIsTrue(
                CountOfStrInXml(promo, "<product category-id=") ==
                Convert.ToInt32(csvData["pec2PromosCategoryCount22"]),
                "(2)Количество каtegорий оффера 2 в tegе предложений not expected");
            VerifyIsTrue(promo.IndexOf("<product offer-id=") == -1,
                "(2)В промо-акции 2 найден teg-предложение продукта, ожидались только каtegории");
            VerifyIsTrue(GetTegContent(promo, "purchase") == ConvertCsvString(csvData["pec2Promos22"]),
                "(2)Содержимое tegа purchase оффера 2 not expected с эталоном");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexPromocodeECatNotInvolve()
        {
            //купон для кат-й, которые не уч.в выгрузке

            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName3"]);
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pec3Name"]);
            AddCoupon(csvData["pec3CouponName"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"5_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.ClearToastMessages();
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pec3CouponName"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();

            Driver.ClearToastMessages();

            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pec3CategoryCount"]),
                "(1)Количество выгруженных каtegорий not expected");
            VerifyIsTrue(xmlResult.IndexOf("<promos") == -1,
                "(1)В файле выгрузки обнаружен teg promos, когда не ожидается ни одной выгрузки");
            VerifyIsTrue(xmlResult.IndexOf("<promo ") == -1,
                "(1)Количество tegs promo не совпало с ожидаемым");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexPromocodeECatEmpty()
        {
            //купон для каtegории, ВСЕ товары которой неактивны (5); купон для каtegории, ВСЕ товары которой исключены;
            //купон для пустой каtegории; купон для неактивной каtegории

            GoToAdmin("catalog/?categoryid=2");
            //костыль - чтоб в третьей админке при экране 1280 выводился чекбокс активности
            if (Driver.FindElements(By.ClassName("sidebar--compact")).Count == 0)
            {
                Driver.FindElement(By.CssSelector(".top-panel__item button.burger")).Click();
                Thread.Sleep(100);
            }

            //конец костыля
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);
            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName3"]);
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"4_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[id=\"5_anchor\"]")).Click();
            Driver.ClearToastMessages();
            //добавлена, чтоб не я-валидация при отстутствии продуктов не падала
            Driver.FindElement(By.CssSelector(".jstree-closed[id=\"1\"] i.jstree-icon.jstree-ocl")).Click();
            Driver.FindElement(By.CssSelector("[id=\"6_anchor\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[id=\"5_anchor\"]")).Click();
            Driver.GetGridCellInputForm(0, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.FindElement(By.CssSelector("ui-modal-cross .close")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();

            int countOfIteration = Convert.ToInt32(csvData["pec4PromocodeCount"]);
            for (int j = 1; j <= countOfIteration; j++)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
                Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pec4Name"] + j);
                AddCoupon(csvData["pec4CouponName"] + j, couponVal, couponCur);
                Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector("[id=\"" + csvData["pec4CategoryId" + j] + "_anchor\"]")).Click();
                Driver.FindElement(By.ClassName("btn-save")).Click();

                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pec4CouponName"] + j, "CssSelector", "Text");
                Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
                Driver.ClearToastMessages();
            }

            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            //+2 каtegория - та, которая добавлена для я-валидации
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<category ") == Convert.ToInt32(csvData["pec4CategoryCount"]) + 2,
                "Количество выгруженных каtegорий not expected");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pec4PromoCount"]),
                "Количество выгруженных promo-акций not expected");
            VerifyIsTrue(xmlResult.IndexOf("<product offer-id ") == -1,
                "Количество выгруженных промо- товаров not expected");
            string promos = GetPromosFromXml(xmlResult);
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(promos, 1), "purchase") == ConvertCsvString(csvData["pec4Promo1"]),
                "Для промо-акции 1 каtegории не совпали с эталоном");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(promos, 2), "purchase") == ConvertCsvString(csvData["pec4Promo2"]),
                "Для промо-акции 2 каtegории не совпали с эталоном");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(promos, 3), "purchase") == ConvertCsvString(csvData["pec4Promo3"]),
                "Для промо-акции 3 каtegории не совпали с эталоном");
            VerifyIsFalse(promos.IndexOf(ConvertCsvString(csvData["pec4Promo4"])) != -1,
                "В промоакциях обнаружена акция для неактивной каtegории!");

            GoToAdmin("catalog/?categoryid=2");
            //костыль - чтоб в третьей админке при экране 1280 выводился чекбокс активности
            if (Driver.FindElements(By.ClassName("sidebar--compact")).Count == 0)
            {
                Driver.FindElement(By.CssSelector(".top-panel__item button.burger")).Click();
                Thread.Sleep(100);
            }

            //конец костыля
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
        }
    }
}