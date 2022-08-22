using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex.Gift
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexGiftExportProductsTest : ExportServices
    {
        string adminPath = "exportfeeds/indexyandex";
        string exportPath = "export/";
        Dictionary<string, string> csvData;
        int uibtabIndex = 6;

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
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.Tag.csv",
                "Data\\Admin\\Export\\Catalog\\Gift\\Products\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile(
                "Data\\Admin\\Export\\TestSettings\\YandexPromos\\GiftExportProductData.csv");
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
        public void YandexGiftPNotAvailable()
        {
            //1 - не выгружать неактивные.
            //а-товар неакт, подарок норм

            //б-товар акт, подарок неакт
            //в-товар неакт, подарок неакт
            //-------------------
            //2 - выгружать неактивные
            //а-товар неакт, подарок норм

            //б-товар акт, подарок неакт
            //в-товар неакт, подарок неакт
            //1 - не выгружать без цены.
            //а-товар без цены, подарок норм

            //б-товар акт, подарок без цены
            //в-товар без цены, подарок без цены
            //-------------------
            //2 - выгружать без цены
            //а-товар без цены, подарок норм

            //б-товар акт, подарок без цены
            //в-товар без цены, подарок без цены

            GoToAdmin(adminPath);
            AddExportFeed("GiftNotAvailable");

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            //товары из первой к, продукты из второй, чтоб отображаться в gifts
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();

            string[] arrayOfProductIndex1 = csvData["gp1Products"].Split(',');
            string[] arrayOfGiftIndex1 = csvData["gp1Gifts"].Split(',');
            string[] arrayOfProductIndex2 = csvData["gp2Products"].Split(',');
            string[] arrayOfGiftIndex2 = csvData["gp2Gifts"].Split(',');

            //ПЕРВОЕ - НЕ ВЫГРУЖАТЬ НЕАКТИВНОЕ
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            //неактивный товар и активный подарок - жду, что tegа гифт и акций, и товара нет
            AddGiftWithPurchase("GiftDisabled1-1", new List<string> {arrayOfProductIndex1[1]}, arrayOfGiftIndex1[0]);
            //Без цены товар и с ценой подарок - жду, что tegа гифт и акций, и товара нет
            AddGiftWithPurchase("GiftDisabled1-2", new List<string> {arrayOfProductIndex2[1]}, arrayOfGiftIndex2[0]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex1[1] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex1[1] + "\">") == -1,
                "(1-1) Неактивный товар найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex2[1] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex2[1] + "\">") == -1,
                "(1-1) Товар без цены найден в выгрузке");

            VerifyIsTrue(xmlResult.IndexOf("<gifts>") == -1,
                "(1-1)В выгрузке найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<gift ") == -1,
                "(1-1)В выгрузке найдена подстрока gift");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") == -1,
                "(1-1)В выгрузке найдена подстрока promos");

            ReturnToExport();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.GetGridCell(1, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            //активный товар и неактивный подарок - жду, что teg гифт и акции будут, и товар есть
            AddGiftWithPurchase("GiftDisabled2-1", new List<string> {arrayOfProductIndex1[0]}, arrayOfGiftIndex1[1]);
            //Товар с ценой и без цены подарок - жду, что teg гифт и акции будут, и товар есть
            AddGiftWithPurchase("GiftDisabled2-2", new List<string> {arrayOfProductIndex2[0]}, arrayOfGiftIndex2[1]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1, "(1-2)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex1[0] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex1[0] + "\">") != -1,
                "(1-2) Активный товар не найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex2[0] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex2[0] + "\">") != -1,
                "(1-2) Товар с ценой не найден в выгрузке");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 2,
                "(1-2)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp1GiftContentNotActive"])) != -1,
                "(1-2)В выгрузке не найдено эталонное содержимое tegа gifts(1,2)");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") != -1,
                "(1-2)В promo не найдена подстрока promos");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 2,
                "(1-2)количество tegs promo not expected");

            string promo = GetPromoFromXml(xmlResult, 1);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(1-2)В promo не найдена подстрока с типом купона (1)");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp1Purchase"]),
                "(1-2)Cодержимое tegа purchase not expected(1)");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp1PromoGiftNotActive"]),
                "(1-2)Cодержимое tegа promo-gifts not expected(1)");

            promo = GetPromoFromXml(xmlResult, 2);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(1-2)В promo не найдена подстрока с типом купона (2)");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp2Purchase"]),
                "(1-2)Cодержимое tegа purchase not expected(2)");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp2PromoGiftNotActive"]),
                "(1-2)Cодержимое tegа promo-gifts not expected(2)");

            ReturnToExport();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.GetGridCell(1, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            //неактивный товар и неактивный подарок - жду, что tegа гифт и акций, и товара нет
            AddGiftWithPurchase("GiftDisabled3-1", new List<string> {arrayOfProductIndex1[1]}, arrayOfGiftIndex1[1]);
            //Без цены товар и без цены подарок - жду, что tegа гифт и акций, и товара нет
            AddGiftWithPurchase("GiftDisabled3-2", new List<string> {arrayOfProductIndex2[1]}, arrayOfGiftIndex2[1]);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex1[1] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex1[1] + "\">") == -1,
                "(1-3) Недоступный товар найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex2[1] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex2[1] + "\">") == -1,
                "(1-3) Товар без цены найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") == -1,
                "(1-3)В выгрузке найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<gift ") == -1,
                "(1-3)В выгрузке найдена подстрока gift");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") == -1,
                "(1-3)В выгрузке найдена подстрока promos");

            ReturnToExport();

            //ВТОРОЕ - ВЫГРУЖАТЬ НЕАКТИВНЫЕ
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxCheck("ExportNotAvailable");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.GetGridCell(1, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            //неактивный товар и активный подарок - жду, что teg гифт и акции, и товар есть
            AddGiftWithPurchase("GiftDisabled4", new List<string> {arrayOfProductIndex1[1]}, arrayOfGiftIndex1[0]);
            //Без цены товар и с ценой подарок - жду, что teg гифт и акции, и товар ест
            AddGiftWithPurchase("GiftDisabled4", new List<string> {arrayOfProductIndex2[1]}, arrayOfGiftIndex2[0]);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1, "(2-1)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex1[1] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex1[1] + "\">") != -1,
                "(2-1) Неактивный товар не найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex2[1] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex2[1] + "\">") != -1,
                "(2-1) Товар без цены не найден в выгрузке");

            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 1,
                "(2-1)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp1GiftContent"])) != -1,
                "(2-1)В выгрузке не найдено эталонное содержимое tegа gifts");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") != -1,
                "(2-1)В promo не найдена подстрока promos");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 2,
                "(2-1)количество tegs promo not expected");

            promo = GetPromoFromXml(xmlResult, 1);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(2-1)В promo не найдена подстрока с типом купона (1)");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp1PurchaseNotActive"]),
                "(2-1)Cодержимое tegа purchase not expected (1)");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp1PromoGift"]),
                "(2-1)Cодержимое tegа promo-gifts not expected(1)");
            promo = GetPromoFromXml(xmlResult, 2);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(2-1)В promo не найдена подстрока с типом купона (2)");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp2PurchaseNotActive"]),
                "(2-1)Cодержимое tegа purchase not expected(2)");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp1PromoGift"]),
                "(2-1)Cодержимое tegа promo-gifts not expected(2)");

            ReturnToExport();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.GetGridCell(1, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            //активный товар и неактивный подарок - жду, что teg гифт и акции будут, и товар есть
            AddGiftWithPurchase("GiftDisabled5-1", new List<string> {arrayOfProductIndex1[0]}, arrayOfGiftIndex1[1]);
            //Товар с ценой и без цены подарок - жду, что teg гифт и акции будут, и товар есть
            AddGiftWithPurchase("GiftDisabled5-2", new List<string> {arrayOfProductIndex2[0]}, arrayOfGiftIndex2[1]);

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1, "(2-2)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex1[0] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex1[0] + "\">") != -1,
                "(2-2) Активный товар не найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex2[0] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex2[0] + "\">") != -1,
                "(2-2) Товар с ценой не найден в выгрузке");

            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 2,
                "(2-2)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp1GiftContentNotActive"])) != -1,
                "(2-2)В выгрузке не найдено эталонное содержимое tegа gifts(1,2)");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") != -1,
                "(2-2)В promo не найдена подстрока promos");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 2,
                "(2-2)количество tegs promo not expected");

            promo = GetPromoFromXml(xmlResult, 1);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(2-2)В promo не найдена подстрока с типом купона (1)");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp1Purchase"]),
                "(2-2)Cодержимое tegа purchase not expected(1)");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp1PromoGiftNotActive"]),
                "(2-2)Cодержимое tegа promo-gifts not expected(1)");
            promo = GetPromoFromXml(xmlResult, 2);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(2-2)В promo не найдена подстрока с типом купона (2)");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp2Purchase"]),
                "(2-2)Cодержимое tegа purchase not expected(2)");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp2PromoGiftNotActive"]),
                "(2-2)Cодержимое tegа promo-gifts not expected(2)");


            ReturnToExport();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.GetGridCell(1, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            //неактивный товар и неактивный подарок - жду, что tegа гифт и акций, и товара нет
            AddGiftWithPurchase("GiftDisabled6-1", new List<string> {arrayOfProductIndex1[1]}, arrayOfGiftIndex1[1]);
            //Товар без цены и без цены подарок - жду, что tegа гифт и акций, и товара нет
            AddGiftWithPurchase("GiftDisabled6-2", new List<string> {arrayOfProductIndex2[1]}, arrayOfGiftIndex2[1]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1,
                "(2-3)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex1[1] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex1[1] + "\">") != -1,
                "(2-3) Нективный товар не найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex2[1] +
                                           "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex2[1] + "\">") != -1,
                "(2-3) Товар без цены не найден в выгрузке");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 2,
                "(2-3)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp1GiftContentNotActive"])) != -1,
                "(2-3)В выгрузке не найдено эталонное содержимое tegа gifts(1,2)");

            VerifyIsTrue(xmlResult.IndexOf("<promos>") != -1,
                "(2-3)В promo не найдена подстрока promos");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 2,
                "(2-3)количество tegs promo not expected");

            promo = GetPromoFromXml(xmlResult, 1);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(2-3)В promo не найдена подстрока с типом купона (1)");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp1PurchaseNotActive"]),
                "(2-3)Cодержимое tegа purchase not expected(1)");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp1PromoGiftNotActive"]),
                "(2-3)Cодержимое tegа promo-gifts not expected(1)");
            promo = GetPromoFromXml(xmlResult, 2);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(2-3)В promo не найдена подстрока с типом купона (2)");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp2PurchaseNotActive"]),
                "(2-3)Cодержимое tegа purchase not expected(2)");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp2PromoGiftNotActive"]),
                "(2-3)Cодержимое tegа promo-gifts not expected(2)");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexGiftPPreOrder()
        {
            //1 - не выгружать под заказ.
            //а-товар не в наличии, подарок норм

            //б-товар акт, подарок не в наличии
            //в-товар не в наличии, подарок не в наличии
            //-------------------
            //2 - выгружать под заказ
            //а-товар не в наличии, подарок норм

            //б-товар акт, подарок не в наличии
            //в-товар не в наличии, подарок не в наличии

            GoToAdmin(adminPath);
            AddExportFeed("GiftPPreOrder");


            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            //товары из первой к, продукты из второй, чтоб отображаться в gifts
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();

            string[] arrayOfProductIndex = csvData["gp3Products"].Split(',');
            string[] arrayOfGiftIndex = csvData["gp3Gifts"].Split(',');

            //ПЕРВОЕ - НЕ ВЫГРУЖАТЬ под заказ
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxUncheck("AllowPreOrderProducts");

            Driver.ScrollToTop();

            //не в наличии товар и в наличии подарок - жду, что tegа гифт и акций, и товара нет
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            AddGiftWithPurchase("GiftDisabled1", new List<string> {arrayOfProductIndex[1]}, arrayOfGiftIndex[0]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex[1] +
                                           "\" available=\"false\" group_id=\""
                                           + arrayOfProductIndex[1] + "\">") == -1,
                "(1-1) Товар не в наличии найден в выгрузке");

            VerifyIsTrue(xmlResult.IndexOf("<gifts>") == -1,
                "(1-1)В выгрузке найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<gift ") == -1,
                "(1-1)В выгрузке найдена подстрока gift");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") == -1,
                "(1-1)В выгрузке найдена подстрока promos");

            ReturnToExport();
            //Товар в наличии и не в наличии подарок - жду, что teg гифт и акции будут, и товар есть
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            AddGiftWithPurchase("GiftDisabled2", new List<string> {arrayOfProductIndex[0]}, arrayOfGiftIndex[1]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1,
                "(1-2)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex[0] + "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex[0] + "\">") != -1,
                "(1-2) Товар не в наличии не найден в выгрузке");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 1,
                "(1-2)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp3GiftContentNotActive"])) != -1,
                "(1-2)В выгрузке не найдено эталонное содержимое tegа gifts");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") != -1,
                "(1-2)В promo не найдена подстрока promos");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(1-2)количество tegs promo not expected");

            string promo = GetPromoFromXml(xmlResult, 1);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(1-2)В promo не найдена подстрока с типом купона ");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp3Purchase"]),
                "(1-2)Cодержимое tegа purchase not expected");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp3PromoGiftNotActive"]),
                "(1-2)Cодержимое tegа promo-gifts not expected");

            ReturnToExport();
            //не в наличии товар и не в наличии подарок - жду, что tegа гифт и акций, и товара нет
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            AddGiftWithPurchase("GiftDisabled3", new List<string> {arrayOfProductIndex[1]}, arrayOfGiftIndex[1]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex[1] +
                                           "\" available=\"false\" group_id=\""
                                           + arrayOfProductIndex[1] + "\">") == -1,
                "(1-3) Товар не в наличии найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") == -1,
                "(1-3)В выгрузке найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<gift ") == -1,
                "(1-3)В выгрузке найдена подстрока gift");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") == -1,
                "(1-3)В выгрузке найдена подстрока promos");

            ReturnToExport();

            //ВТОРОЕ - ВЫГРУЖАТЬ под заказ
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxCheck("AllowPreOrderProducts");
            Driver.ScrollToTop();

            //не в наличии товар и в наличии подарок - жду, что tegа гифт и акций, и товара нет
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            AddGiftWithPurchase("GiftDisabled4", new List<string> {arrayOfProductIndex[1]}, arrayOfGiftIndex[0]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1,
                "(2-1)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex[1] +
                                           "\" available=\"false\" group_id=\""
                                           + arrayOfProductIndex[1] + "\">") != -1,
                "(2-1) Товар не в наличии не найден в выгрузке");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 1,
                "(2-1)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp3GiftContent"])) != -1,
                "(2-1)В выгрузке не найдено эталонное содержимое tegа gifts");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") != -1,
                "(2-1)В promo не найдена подстрока promos");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(2-1)количество tegs promo not expected");

            promo = GetPromoFromXml(xmlResult, 1);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(2-1)В promo не найдена подстрока с типом купона ");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp3PurchaseNotActive"]),
                "(2-1)Cодержимое tegа purchase not expected");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp3PromoGift"]),
                "(2-1)Cодержимое tegа promo-gifts not expected");

            ReturnToExport();
            //Товар в наличии и не в наличии подарок - жду, что teg гифт и акции будут, и товар есть
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            AddGiftWithPurchase("GiftDisabled5", new List<string> {arrayOfProductIndex[0]}, arrayOfGiftIndex[1]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1,
                "(2-2)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex[0] + "\" available=\"true\" group_id=\""
                                           + arrayOfProductIndex[0] + "\">") != -1,
                "(2-2) Товар не в наличии не найден в выгрузке");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 1,
                "(2-2)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp3GiftContentNotActive"])) != -1,
                "(2-2)В выгрузке не найдено эталонное содержимое tegа gifts");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") != -1,
                "(2-2)В promo не найдена подстрока promos");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(2-2)количество tegs promo not expected");

            promo = GetPromoFromXml(xmlResult, 1);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(2-2)В promo не найдена подстрока с типом купона ");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp3Purchase"]),
                "(2-2)Cодержимое tegа purchase not expected");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp3PromoGiftNotActive"]),
                "(2-2)Cодержимое tegа promo-gifts not expected");

            ReturnToExport();
            //Товар не в наличии и не в наличии подарок - жду, что tegа гифт и акций, и товара нет
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();
            AddGiftWithPurchase("GiftDisabled6", new List<string> {arrayOfProductIndex[1]}, arrayOfGiftIndex[1]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1,
                "(2-3)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfProductIndex[1] +
                                           "\" available=\"false\" group_id=\""
                                           + arrayOfProductIndex[1] + "\">") != -1,
                "(2-3) Товар не в наличии не найден в выгрузке");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 1,
                "(2-3)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp3GiftContentNotActive"])) != -1,
                "(2-3)В выгрузке не найдено эталонное содержимое tegа gifts");
            VerifyIsTrue(xmlResult.IndexOf("<promos>") != -1,
                "(2-3)В promo не найдена подстрока promos");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == 1,
                "(2-3)количество tegs promo not expected");

            promo = GetPromoFromXml(xmlResult, 1);
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["gpPromoType"])) != -1,
                "(2-3)В promo не найдена подстрока с типом купона ");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp3PurchaseNotActive"]),
                "(2-3)Cодержимое tegа purchase not expected");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp3PromoGiftNotActive"]),
                "(2-3)Cодержимое tegа promo-gifts not expected");

            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexGiftPModification()
        {
            //товар с модификациями - 9; подарок с модификациями - 16 (хз зачем)
            //++другой идентификатор.
            //нужно: 1 = не выгружать модификации, 2 - выгружать модификации.
            //1а - 9 товар и подарок 11, 9 товар и подарок 16-1, 9 товар и подарок 16-2; выгружать кат.1, 2
            //ожидаемо - подарок 16-2 в гифтах, остальное в выгрузке. 3 промо-акции, в каждой  по 1 оферу и 1 подарку.
            //2а - 9 товар и подарок 11, 9 товар и подарок 16-1, 9 товар и подарок 16-2; выгружать кат.1,2
            //ожидаемо - все в выгрузке, нет гифтов. 3 промо-акции, в каждой  по 3 оферу и 1 подарку.
            //2б - 9 товар и подарок 11, 9 товар и подарок 16-1, 9 товар и подарок 16-2; выгружать кат.1
            //ожидаемо - все в выгрузке, 3 гифта. 3 промо-акции, в каждой  по 3 оферу и 1 подарку.

            //потом продублировать, но выбрать артикул модификации

            GoToAdmin(adminPath);
            AddExportFeed("GiftModification");

            string Product = csvData["gp4Product"];
            string[] arrayOfGiftArtNo = csvData["gp4GiftsArtNo"].Split(',');
            string[] arrayOfGiftID = csvData["gp4GiftsID"].Split(',');

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            //товары из первой к, продукты из второй, чтоб отображаться в gifts
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            AddGiftWithPurchase("GiftModification1", new List<string> {Product}, arrayOfGiftArtNo[0]);
            AddGiftWithPurchase("GiftModification2", new List<string> {Product}, arrayOfGiftArtNo[1]);
            AddGiftWithPurchase("GiftModification3", new List<string> {Product}, arrayOfGiftArtNo[1]);
            Driver.GetGridCell(2, "_serviceColumn", "YandexPromoGift")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ClearGift\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddGift\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"),
                "TestProduct" + arrayOfGiftArtNo[1]);
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[0]" +
                                              "[\'treeBaseRowHeaderCol\']\"]"))
                .FindElement(By.ClassName("ui-grid-icon-plus-squared")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[1]" +
                                              "[\'selectionRowHeaderCol\']\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            //ожидаемо - подарок 16-2 в гифтах, остальное в выгрузке. 3 промо-акции, в каждой  по 1 оферу и 1 подарку.
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1,
                "(1-1)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfGiftID[0] + "\" available=\"true\" ") != -1,
                "(1-1) Товар " + arrayOfGiftID[0] + " не найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfGiftID[1] + "\" available=\"true\" ") != -1,
                "(1-1) Товар " + arrayOfGiftID[1] + " не найден в выгрузке");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 1,
                "(1-1)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp4GiftContent1"])) != -1,
                "(1-1)В выгрузке не найдено эталонное содержимое tegа gifts");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["gp4Purchase1"]) == 3,
                "(1-1)В промоакциях не найдено три заданных tegа purchase");

            string[] arrayPromoGift1 = csvData["gp4PromoGift1"].Split(',');
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 1), "promo-gifts") == ConvertCsvString(arrayPromoGift1[0]),
                "(1-1)P1 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 2), "promo-gifts") == ConvertCsvString(arrayPromoGift1[1]),
                "(1-1)P2 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 3), "promo-gifts") == ConvertCsvString(arrayPromoGift1[2]),
                "(1-1)P3 - Cодержимое tegа promo-gifts not expected");
            ReturnToExport();

            //2а
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            //ожидаемо - все в выгрузке, нет гифтов. 3 промо-акции, в каждой  по 3 оферу и 1 подарку.
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") == -1, "(1-2)В выгрузке найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<gift ") == -1, "(1-2)В выгрузке найдена подстрока gift");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfGiftID[0] + "\" available=\"true\" ") != -1,
                "(1-2) Товар " + arrayOfGiftID[0] + " не найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfGiftID[1] + "\" available=\"true\" ") != -1,
                "(1-2) Товар " + arrayOfGiftID[1] + " не найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfGiftID[2] + "\" available=\"true\" ") != -1,
                "(1-2) Товар " + arrayOfGiftID[2] + " не найден в выгрузке");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["gp4Purchase2"]) == 3,
                "(1-2)В промоакциях не найдено три заданных tegа purchase");

            string[] arrayPromoGift2 = csvData["gp4PromoGift2"].Split(',');
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 1), "promo-gifts") == ConvertCsvString(arrayPromoGift2[0]),
                "(1-2)P1 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 2), "promo-gifts") == ConvertCsvString(arrayPromoGift2[1]),
                "(1-2)P2 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 3), "promo-gifts") == ConvertCsvString(arrayPromoGift2[2]),
                "(1-2)P3 - Cодержимое tegа promo-gifts not expected");
            ReturnToExport();

            //2b
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            //только товары из первой 
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            //ожидаемо - все в выгрузке, 3 гифта. 3 промо-акции, в каждой  по 3 оферу и 1 подарку.
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1,
                "(1-3)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 3,
                "(1-3)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp4GiftContent3"])) != -1,
                "(1-3)В выгрузке не найдено эталонное содержимое tegа gifts");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["gp4Purchase2"]) == 3,
                "(1-3)В промоакциях не найдено три заданных tegа purchase");
            string[] arrayPromoGift3 = csvData["gp4PromoGift3"].Split(',');
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 1), "promo-gifts") == ConvertCsvString(arrayPromoGift3[0]),
                "(1-3)P1 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 2), "promo-gifts") == ConvertCsvString(arrayPromoGift3[1]),
                "(1-3)P2 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 3), "promo-gifts") == ConvertCsvString(arrayPromoGift3[2]),
                "(1-3)P3 - Cодержимое tegа promo-gifts not expected");
            ReturnToExport();


            //ЗАМЕНА ID НА ARTNO
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            SelectOption("ddlOfferIdType", "artno");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            //только товары из первой и второй 
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            //ожидаемо - подарок 162 в гифтах, остальное в выгрузке. 3 промо-акции, в каждой  по 1 оферу и 1 подарку.
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1, "(1-1)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfGiftArtNo[0] + "\" available=\"true\" ") != -1,
                "(2-1) Товар " + arrayOfGiftArtNo[0] + " не найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfGiftArtNo[1] + "\" available=\"true\" ") != -1,
                "(2-1) Товар " + arrayOfGiftArtNo[1] + " не найден в выгрузке");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 1,
                "(2-1)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp4GiftContent4"])) != -1,
                "(2-1)В выгрузке не найдено эталонное содержимое tegа gifts");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["gp4Purchase4"]) == 3,
                "(2-1)В промоакциях не найдено три заданных tegа purchase");

            string[] arrayPromoGift4 = csvData["gp4PromoGift4"].Split(',');
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 1), "promo-gifts") == ConvertCsvString(arrayPromoGift4[0]),
                "(2-1)P1 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 2), "promo-gifts") == ConvertCsvString(arrayPromoGift4[1]),
                "(2-1)P2 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 3), "promo-gifts") == ConvertCsvString(arrayPromoGift4[2]),
                "(2-1)P3 - Cодержимое tegа promo-gifts not expected");
            ReturnToExport();

            //2а
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.CheckBoxUncheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            //ожидаемо - все в выгрузке, нет гифтов. 3 промо-акции, в каждой  по 3 оферу и 1 подарку.
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") == -1, "(2-2)В выгрузке найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<gift ") == -1, "(2-2)В выгрузке найдена подстрока gift");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfGiftArtNo[0] + "\" available=\"true\" ") != -1,
                "(2-2) Товар " + arrayOfGiftArtNo[0] + " не найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfGiftArtNo[1] + "\" available=\"true\" ") != -1,
                "(2-2) Товар " + arrayOfGiftArtNo[1] + " не найден в выгрузке");
            VerifyIsTrue(xmlResult.IndexOf("<offer id=\"" + arrayOfGiftArtNo[2] + "\" available=\"true\" ") != -1,
                "(2-2) Товар " + arrayOfGiftArtNo[2] + " не найден в выгрузке");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["gp4Purchase5"]) == 3,
                "(2-2)В промоакциях не найдено три заданных tegа purchase");

            string[] arrayPromoGift5 = csvData["gp4PromoGift5"].Split(',');
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 1), "promo-gifts") == ConvertCsvString(arrayPromoGift5[0]),
                "(2-2)P1 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 2), "promo-gifts") == ConvertCsvString(arrayPromoGift5[1]),
                "(2-2)P2 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 3), "promo-gifts") == ConvertCsvString(arrayPromoGift5[2]),
                "(2-2)P3 - Cодержимое tegа promo-gifts not expected");
            ReturnToExport();

            //2b
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            //только товары из первой 
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            //ожидаемо - все в выгрузке, 3 гифта. 3 промо-акции, в каждой  по 3 оферу и 1 подарку.
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1,
                "(2-3)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<gift ") == 3,
                "(2-3)количество подарков в файле выгрузки not expected");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp4GiftContent6"])) != -1,
                "(2-3)В выгрузке не найдено эталонное содержимое tegа gifts");
            VerifyIsTrue(CountOfStrInXml(xmlResult, csvData["gp4Purchase5"]) == 3,
                "(2-3)В промоакциях не найдено три заданных tegа purchase");
            string[] arrayPromoGift6 = csvData["gp4PromoGift6"].Split(',');
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 1), "promo-gifts") == ConvertCsvString(arrayPromoGift6[0]),
                "(2-3)P1 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 2), "promo-gifts") == ConvertCsvString(arrayPromoGift6[1]),
                "(2-3)P2 - Cодержимое tegа promo-gifts not expected");
            VerifyIsTrue(
                GetTegContent(GetPromoFromXml(xmlResult, 3), "promo-gifts") == ConvertCsvString(arrayPromoGift6[2]),
                "(2-3)P3 - Cодержимое tegа promo-gifts not expected");
            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexGiftPExcluded()
        {
            GoToAdmin(adminPath);
            AddExportFeed("GiftExcluded");
            //проверка: если товар в каtegории, у него в promo-gifts отображается offer-id, иначе gift-id
            //товар 1, подарок 10. Сперва просто выгрузка, потом выгрузка с исключенным 10-тым.
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            string Product = csvData["gp5Product"];
            string Gift = csvData["gp5Gift"];

            AddGiftWithPurchase("GiftExcluded", new List<string> {Product}, Gift);

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();
            Driver.ClearToastMessages();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            //ожидаемо - есть офер с заданным id, нет gifts, у акции offer-id.
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") == -1,
                "(1)В выгрузке найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf("<gift ") == -1,
                "(1)В выгрузке найдена подстрока gift");
            VerifyIsTrue(
                xmlResult.IndexOf("<offer id=\"" + Gift + "\" available=\"true\" group_id=\"" + Gift + "\">") != -1,
                "(1) Товар не найден в выгрузке");

            string promo = GetPromosFromXml(xmlResult);
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp5Purchase"]),
                "(1)Cодержимое tegа purchase not expected");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp5PromoGift1"]),
                "(1)Cодержимое tegа promo-gifts not expected");
            ReturnToExport();

            //исключить 10 товар
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            //исключение товаров
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), "TestProduct" + Gift);
            Thread.Sleep(1000);
            Driver.GetGridCellInputForm(0, "ExcludeFromExport", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.FindElement(By.CssSelector("ui-modal-cross .close")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            VerifyIsTrue(xmlResult.IndexOf("<gifts>") != -1, "(1)В выгрузке не найдена подстрока gifts");
            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["gp5GiftContent"])) != -1,
                "(2)В выгрузке не найдено эталонное содержимое tegа gifts");
            promo = GetPromosFromXml(xmlResult);
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["gp5Purchase"]),
                "(2)Cодержимое tegа purchase not expected");
            VerifyIsTrue(GetTegContent(promo, "promo-gifts") == ConvertCsvString(csvData["gp5PromoGift2"]),
                "(2)Cодержимое tegа promo-gifts not expected");
            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }
    }
}