using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex.Promocode
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexPromocodeAddRemoveTest : ExportServices
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
                "Data\\Admin\\Export\\TestSettings\\YandexPromos\\PromocodeAddRemoveData.csv");
            InitializeService.YandexChannelActive();
            couponVal = csvData["paCouponValueCur"];
            couponCur = csvData["paCouponCur1"];
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
        public void YandexPromocodeAdd()
        {
            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed(csvData["testName1"]);
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pa1Name"]);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddDescription\"]"), csvData["pa1Description"]);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddUrl\"]"), csvData["pa1Url"]);
            AddCoupon(csvData["pa1CouponName"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pa1CouponName"], "CssSelector", "Text");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual(Driver.GetGridCell(0, "Name", "YandexPromoCodes").Text, csvData["pa1Name"],
                "В таблице промокодов не найдено название промокода");
            VerifyAreEqual(Driver.GetGridCell(0, "Description", "YandexPromoCodes").Text, csvData["pa1Description"],
                "В таблице промокодов не найдено описание промокода");
            VerifyAreEqual(Driver.GetGridCell(0, "PromoUrl", "YandexPromoCodes").Text, csvData["pa1Url"],
                "В таблице промокодов не найдена ссылка на промокод");
        }

        [Test]
        public void YandexPromocodeEdit()
        {
            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed(csvData["testName2"]);

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pa2Name1"]);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddDescription\"]"), csvData["pa2Description1"]);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddUrl\"]"), csvData["pa2Url"]);
            AddCoupon(csvData["pa2CouponName1"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pa2CouponName1"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoCodes")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();
            AddCoupon(csvData["pa2CouponName2"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"PromoAddName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PromoAddDescription\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PromoAddUrl\"]")).Clear();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pa2Name2"]);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddDescription\"]"), csvData["pa2Description2"]);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddUrl\"]"), csvData["pa2Url2"]);
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pa2CouponName2"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(1000);

            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoCodes")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();

            VerifyAreEqual(Driver.FindElement(By.CssSelector("[data-e2e=\"PromoAddName\"]")).GetAttribute("value"),
                csvData["pa2Name2"],
                "Редактирование названия не было осуществлено");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector("[data-e2e=\"PromoAddDescription\"]")).GetAttribute("value"),
                csvData["pa2Description2"],
                "Редактирование описания не было осуществлено");
            VerifyAreEqual(Driver.FindElement(By.CssSelector("[data-e2e=\"PromoAddUrl\"]")).GetAttribute("value"),
                csvData["pa2Url2"],
                "Редактирование ссылки не было осуществлено");
            VerifyIsTrue(
                SelectOptionSelected("[data-e2e=\"ExportAddType\"]", csvData["pa2CouponName2"], "CssSelector", "Label"),
                "Измененное название купона not expected");
            Driver.FindElement(By.ClassName("close")).Click();
        }

        [Test]
        public void YandexPromocodeRemove()
        {
            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed(csvData["testName3"]);

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Thread.Sleep(1000);

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pa3Name"]);
            AddCoupon(csvData["pa3CouponName"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pa3CouponName"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoCodes").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();

            VerifyIsFalse(IsElementExists("[data-e2e=\"gridRow\"][data-e2e-row-index=\"0\"]", "CssSelector"),
                "Промоакция не была удалена");
            Thread.Sleep(1000);

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexPromocodeValidate()
        {
            GoToAdmin("coupons");

            if (IsElementExists("[data-e2e=\"gridRow\"]", "CssSelector"))
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxSelectAll\"]~.adv-checkbox-emul"))
                    .Click();
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownItem\"]" +
                                                  "[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
                Driver.SwalConfirm();
            }

            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed(csvData["testName4"]);
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();

            //до 7.0.4 фикса
            //var toasts = driver.FindElements(By.ClassName("toast-close-button"));
            //toasts[1].Click();
            //toasts[0].Click();
            //VerifyIsTrue(driver.FindElement(By.ClassName("error")).Text.ToLower().IndexOf("все купоны отключены") != -1, "Не отображается надпись в модальном окне об отключенных купонах");
            //после 704 фикса
            Thread.Sleep(500);
            Driver.ClearToastMessages();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).GetAttribute("disabled") == "true",
                "Кнопка 'сохранить промокод (ok)' доступна без изменения формы");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddType\"]")).GetAttribute("disabled") == "true",
                "Доступен выбор купонов при отключенных купонах");
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pa4Name1"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();
            AddCoupon(csvData["pa4CouponNameNone"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponCode\"]"), csvData["pa4CouponNameMore20"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();
            Thread.Sleep(500);
            Driver.ClearToastMessages();

            Thread.Sleep(1000);
            //до 704 фикса
            //VerifyIsTrue(driver.FindElements(By.ClassName("error"))[1].Text.ToLower().IndexOf("купоны были отфильтрованы") != -1, "Не отображается надпись в модальном окне об отфильтрованных купонах");
            //после 704 фикса
            VerifyIsTrue(
                Driver.FindElements(By.ClassName("error"))[0].Text.ToLower().IndexOf("купоны были отфильтрованы") != -1,
                "Не отображается надпись в модальном окне об отфильтрованных купонах");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddType\"]")).GetAttribute("disabled") == "true",
                "Доступен выбор купонов при единственном сверхдлинном купоне");
            AddCoupon(csvData["pa4CouponNameNotActive"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponEnabled\"] .adv-checkbox-emul")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();
            Driver.ClearToastMessages();

            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddType\"]")).FindElement(By.CssSelector(
                    "[label=\""
                    + csvData["pa4CouponNameNotActive"] + "\"]")).GetAttribute("disabled") == "true",
                "Неактивный купон доступен для выбора");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddType\"]")).GetAttribute("disabled") == "true",
                "Доступен выбор купонов при всех недоступных купонах");
            AddCoupon(csvData["pa4CouponNameNormal"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();
            Thread.Sleep(1000);
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pa4CouponNameNormal"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();

            Thread.Sleep(1000);

            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoCodes")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PromoAddName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pa4Name1"]);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddDescription\"]"),
                csvData["text252"] + csvData["text252"]);
            var text500 = (csvData["text252"] + csvData["text252"]).Substring(0, 500);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"PromoAddDescription\"]")).GetAttribute("value").Length ==
                500,
                "Длина описания не соответствует ожидаемым 500 символам");
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector("[data-e2e=\"PromoAddDescription\"]")).GetAttribute("value"), text500,
                "Описание not expected");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();

            Thread.Sleep(500);

            Driver.ClearToastMessages();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pa4Name2"]);
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pa4CouponNameNormal"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();
            //добавление купона с ключем, который уже занят
            AddCoupon(csvData["pa4CouponNameNormal"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponCode\"]"), csvData["pa4CouponName20"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();

            Thread.Sleep(1000);
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pa4CouponName20"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void YandexPromocodeDisplay()
        {
            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed(csvData["testName5"]);
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            //выбираются category 2, 3
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            int j = 1;
            foreach (var promoAct in csvData.Where(p => p.Key.IndexOf("pa5Name") != -1).ToList())
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
                Thread.Sleep(1000);
                Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pa5Name" + j]);
                Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddDescription\"]"), csvData["pa5Desc" + j]);
                Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddUrl\"]"), csvData["pa5Url" + j]);
                Driver.FindElement(By.CssSelector("[data-e2e=\"AddCoupon\"]")).Click();
                Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponCode\"]"), csvData["pa5CouponName" + j]);
                Driver.FindElement(By.CssSelector("[data-e2e=\"couponValue\"]")).Clear();

                if (j % 4 == 1 || j % 4 == 2)
                {
                    SelectOption("[data-e2e=\"couponType\"]", "1", "CssSelector");
                    Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponValue\"]"), csvData["paCouponValueCur"]);
                }
                else
                {
                    SelectOption("[data-e2e=\"couponType\"]", "2", "CssSelector");
                    Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponValue\"]"), csvData["paCouponValuePers"]);
                    Driver.CheckBoxUncheck("[data-e2e=\"couponUseExpirationDate\"]", "CssSelector");
                    Driver.FindElement(By.ClassName("input-group-addon")).Click();
                    Driver.FindElement(By.CssSelector(".flatpickr-day.today~.flatpickr-day")).Click();
                }

                if (j % 2 == 0)
                {
                    SelectOption("[data-e2e=\"couponCurrency\"]", csvData["paCouponCur1"], "CssSelector");
                }
                else
                {
                    SelectOption("[data-e2e=\"couponCurrency\"]", csvData["paCouponCur2"], "CssSelector");
                }

                switch (j)
                {
                    case 2:
                        Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
                        Thread.Sleep(1000);
                        Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
                        Driver.FindElement(By.ClassName("btn-save")).Click();
                        break;
                    case 3:
                        Driver.FindElement(By.CssSelector("[data-e2e=\"couponProducts\"]")).Click();
                        Thread.Sleep(1000);
                        Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
                        Driver.CheckBoxCheck("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]", "CssSelector");
                        Driver.FindElement(By.ClassName("btn-save")).Click();
                        break;
                    case 4:
                        Driver.FindElement(By.CssSelector("[data-e2e=\"couponProducts\"]")).Click();
                        Thread.Sleep(1000);
                        Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();
                        Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                            .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]~.adv-checkbox-emul"))
                            .Click();
                        Driver.GetGridCell(1, "selectionRowHeaderCol", "ProductsSelectvizr")
                            .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]~.adv-checkbox-emul"))
                            .Click();
                        Driver.FindElement(By.ClassName("btn-save")).Click();
                        break;
                }

                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pa5CouponName" + j++], "CssSelector", "Text");
                Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
                Thread.Sleep(500);
                Driver.ClearToastMessages();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            string promos = GetPromosFromXml(xmlResult);
            string promo;
            j = 1;
            VerifyIsTrue(CountOfStrInXml(promos, "<promo ") == Convert.ToInt32(csvData["pa5PromosCount"]),
                "количество tegs promo not expected");
            while (promos.IndexOf("<promo") != -1)
            {
                promo = promos.Substring(promos.IndexOf("<promo "), promos.IndexOf("</promo>"));
                promos = promos.Remove(0, promo.Length + 8);
                VerifyIsTrue(promo.ToLower().Substring(11, promo.IndexOf("\" type=") - 12).Length <= 20,
                    "В promo " + j + " длина id промоакции больше 20 символов");
                VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["paCouponType"])) != -1,
                    "В promo " + j + " не найдена подстрока с типом купона ");
                VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["pa5PromoDescUrlPromocode" + j])) != -1,
                    "В promo " + j + " блок tegs {description, url, promo-code} не соответствует эталонным");
                VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["pa5PromoDiscount" + j])) != -1,
                    "В promo " + j + " teg валют не соответствует эталонному");
                VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["pa5Purchase"])) != -1,
                    "Внутри промоакции " + j + " не обнаружен teg <purchase>");
                VerifyIsTrue(GetTegContent(promo, "purchase").Length != 0,
                    "teg Purchase для промоакции " + j + "пуст");
                if (csvData["pa5PromoExpirationDate" + j] == "false")
                {
                    VerifyIsTrue(promo.IndexOf("start-date") == -1 && promo.IndexOf("end-date") == -1,
                        "В promo " + j + " купон бессрочный, однако обнаружены tegs дат начала/конца");
                }
                else
                {
                    VerifyIsTrue(Convert.ToDateTime(GetTegContent(promo, "start-date")).ToShortDateString()
                                 == DateTime.Now.ToShortDateString(),
                        "Дата начала акции not expected с сегодняшней датой");
                    VerifyIsTrue(Convert.ToDateTime(GetTegContent(promo, "end-date")).ToShortDateString()
                                 == DateTime.Now.AddDays(Convert.ToInt32(csvData["pa5PromoExpirationDateCount3"]))
                                     .ToShortDateString(),
                        "Дата окончания акции not expected с заданной");
                }

                if (j == 2)
                {
                    VerifyIsTrue(GetTegContent(promo, "description").Length <= 500,
                        "Длина описания продукта 3 превышает 500 символов");
                }

                ++j;
            }

            ReturnToExport();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            j = 0;
            foreach (var promoAct in csvData.Where(p => p.Key.IndexOf("pa5Name") != -1).ToList())
            {
                VerifyAreEqual(Driver.GetGridCell(j, "Name", "YandexPromoCodes").Text, csvData["pa5Name" + ++j],
                    "В таблице промокодов после выгрузки и обновления страницы не найдено название промокода" + (j));
            }

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexPromocodeEmptyPromos()
        {
            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed(csvData["testName6"]);
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            //выбираются category 2, 3
            Driver.FindElement(By.CssSelector("[id=\"2_anchor\"]")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pa6Name"]);
            AddCoupon(csvData["pa6CouponName"], couponVal, couponCur);
            Driver.FindElement(By.CssSelector("[data-e2e=\"couponCategories\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[id=\"3_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pa6CouponName"], "CssSelector", "Text");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(xmlResult.IndexOf(ConvertCsvString(csvData["pa6EmptyPromos"])) == -1,
                "В файле выгрузки с недействующими promos был обнаружен teg promos");
        }

        [Test]
        public void YandexPromocodeComplexValidate()
        {
            //1 купон станет неактивным и не экспортнется; 
            //2 купон будет переименован в < 20 симв и экспортнется;
            //3 купон переименуется в 20 симв. и экспортнется;
            //+++попытка переименовать 3 купон во второй купон, которая заканчивается ошибкой.
            //4 купон переименуется в 20+ символов и не экспортнется;
            //5 купон будет удален и не экспортнется
            GoToAdmin(adminPath);
            AddExportFeed(csvData["testName7"]);

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();

            int j = 1;
            foreach (var promoItem in csvData.Where(i => i.Key.IndexOf("pa7Name") != -1))
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AddYandexPromo\"]")).Click();
                Thread.Sleep(1000);
                Driver.SendKeysInput(By.CssSelector("[data-e2e=\"PromoAddName\"]"), csvData["pa7Name" + j]);
                AddCoupon(csvData["pa7CouponName" + j], couponVal, couponCur);
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                SelectOption("[data-e2e=\"ExportAddType\"]", csvData["pa7CouponName" + j++], "CssSelector", "Text");
                Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
                Thread.Sleep(500);
                Driver.ClearToastMessages();
            }

            Thread.Sleep(1000);
            GoToAdmin("coupons");
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pa7CouponName1"]);
            Thread.Sleep(1000);

            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pa7CouponName2"]);
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.ClassName("ui-grid-custom-service-icon")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponCode\"]"), csvData["pa7CouponName2_edited"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pa7CouponName3"]);
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.ClassName("ui-grid-custom-service-icon")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponCode\"]"), csvData["pa7CouponName2_edited"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();

            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponCode\"]"), csvData["pa7CouponName3_edited"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pa7CouponName4"]);
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.ClassName("ui-grid-custom-service-icon")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"couponCode\"]"), csvData["pa7CouponName4_edited"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(500);
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), csvData["pa7CouponName5"]);
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.ClassName("fa-times")).Click();
            Driver.SwalConfirm();

            GoToAdmin(adminPath);
            Driver.FindElement(By.LinkText(csvData["testName7"])).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"4\"]")).Click();

            //проверить тут, что купоны изменены, они в промо-акциях, но они недоступны
            j = 1;
            foreach (var promoItem in Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")))
            {
                promoItem.FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
                var option = Driver.FindElement(By.CssSelector("select[data-e2e=\"ExportAddType\"]"))
                    .FindElement(By.CssSelector("[selected=\"selected\"]"));

                switch (j)
                {
                    case 1:
                        VerifyIsTrue(option.Text == csvData["pa7CouponName1"],
                            "Для промокода 1 выбран не соответствующий эталону купон.");
                        VerifyIsTrue(Convert.ToBoolean(option.GetAttribute("disabled")),
                            "Для промокода 1 выбран не соответствующий эталону купон.");
                        break;
                    case 2:
                        VerifyIsTrue(Driver.FindElement(By.ClassName("error")) != null,
                            "Для промокода 2 не выведено сообщение об удаленном купоне.");
                        VerifyIsTrue(option.Text == csvData["pa7CouponName2_edited"],
                            "Для промокода 2 выбран не соответствующий эталону купон.");
                        VerifyIsTrue(option.GetAttribute("disabled") == null,
                            "Для промокода 2 выбран не соответствующий эталону купон.");
                        //VerifyIsTrue(option.Text == "",
                        //    "Для промокода 2 выбран не соответствующий эталону купон.");
                        //VerifyIsTrue(option.GetAttribute("disabled") == null,
                        //    "Для промокода 2 выбран не соответствующий эталону купон.");
                        break;
                    case 3:
                        VerifyIsTrue(Driver.FindElement(By.ClassName("error")) != null,
                            "Для промокода 3 не выведено сообщение об удаленном купоне.");
                        VerifyIsTrue(option.Text == csvData["pa7CouponName3_edited"],
                            "Для промокода 3  выбран не соответствующий эталону купон.");
                        VerifyIsTrue(option.GetAttribute("disabled") == null,
                            "Для промокода 3 выбран не соответствующий эталону купон.");
                        break;
                    case 4:
                        //для версии до 7.0.4
                        //VerifyIsTrue(driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddType\"]")).GetAttribute("disabled") == "true", "Для промокода 4 выбран не соответствующий эталону купон.");
                        VerifyIsTrue(Driver.FindElement(By.ClassName("error")) != null,
                            "Для промокода 4 не выведено сообщение об удаленном купоне.");
                        //VerifyIsTrue(option.GetAttribute("value") == "?",
                        //    "Для промокода 4 выбран не соответствующий эталону купон.");
                        VerifyIsTrue(option.Text == csvData["pa7CouponName4_edited"],
                            "Для промокода 4  выбран не соответствующий эталону купон.");
                        VerifyIsTrue(option.GetAttribute("disabled") != null,
                            "Для промокода 4 выбран не соответствующий эталону купон.");

                        break;
                    case 5:
                        //для версии до 7.0.4
                        //VerifyIsTrue(driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddType\"]")).GetAttribute("disabled") == "true", "Для промокода 5 выбран не соответствующий эталону купон.");
                        VerifyIsTrue(Driver.FindElement(By.ClassName("error")) != null,
                            "Для промокода 5 не выведено сообщение об удаленном купоне.");
                        VerifyIsTrue(option.GetAttribute("value") == "?",
                            "Для промокода 5 выбран не соответствующий эталону купон.");
                        break;
                }

                ++j;
                Driver.FindElement(By.ClassName("close")).Click();
                Thread.Sleep(1000);
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);

            VerifyIsTrue(CountOfStrInXml(xmlResult, "<promo ") == Convert.ToInt32(csvData["pa7PromosCount"]),
                "В файле выгрузки с недействующими promos был обнаружен teg promos");
            Thread.Sleep(1000);
            VerifyIsFalse(
                xmlResult.IndexOf("<promo-code>" + ConvertCsvString(csvData["pa7CouponName1"]) + "</promo-code>") != -1,
                "Купон 1 найден в выгрузке");
            VerifyIsTrue(
                xmlResult.IndexOf("<promo-code>" +
                                  ConvertCsvString(csvData["pa7CouponName2_edited"] + "</promo-code>")) != -1,
                "Купон 2 в выгрузке не найден");
            VerifyIsTrue(
                xmlResult.IndexOf("<promo-code>" +
                                  ConvertCsvString(csvData["pa7CouponName2_edited"] + "</promo-code>")) != -1,
                "Купон 3 в выгрузке не найден");
            VerifyIsFalse(
                xmlResult.IndexOf("<promo-code>" +
                                  ConvertCsvString(csvData["pa7CouponName4_edited"] + "</promo-code>")) != -1,
                "Купон 4 найден в выгрузке");
            VerifyIsFalse(
                xmlResult.IndexOf("<promo-code>" + ConvertCsvString(csvData["pa7CouponName5"] + "</promo-code>")) != -1,
                "Купон 5 найден в выгрузке");
        }
    }
}