using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex.FlashDiscount
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexDiscountAddRemoveTest : ExportServices
    {
        string adminPath = "exportfeeds/indexyandex";
        string exportPath = "export/";
        Dictionary<string, string> csvData;
        int uibtabIndex = 5;

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
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.Brand.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.Tag.csv",
                "Data\\Admin\\Export\\Catalog\\Discount\\AddRemove\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile(
                "Data\\Admin\\Export\\TestSettings\\YandexPromos\\DiscountAddRemoveData.csv");
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
        public void YandexDiscountAdd()
        {
            //добавление акции, добавление более одной акции
            GoToAdmin(adminPath);

            AddExportFeed("DiscountAdd");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();

            AddDiscount(csvData["da1Name"], null, DateTime.Now.ToShortDateString(),
                DateTime.Now.AddDays(3).ToShortDateString(), csvData["da1Description"], csvData["da1Url"]);
            VerifyAreEqual(Driver.GetGridCell(0, "Name", "YandexPromoFlash").Text, csvData["da1Name"],
                "(1)В таблице скидок не найдено название акции");
            VerifyAreEqual(Driver.GetGridCell(0, "Description", "YandexPromoFlash").Text, csvData["da1Description"],
                "(1)В таблице скидок не найдено описание акции");
            VerifyAreEqual(Driver.GetGridCell(0, "StartDate", "YandexPromoFlash").Text,
                DateTime.Now.ToShortDateString() + " 00:00",
                "(1)В таблице промокодов не найдена заданная дата начала акции");
            VerifyAreEqual(Driver.GetGridCell(0, "ExpirationDate", "YandexPromoFlash").Text,
                DateTime.Now.AddDays(3).ToShortDateString() + " 00:00",
                "(1)В таблице промокодов не найдена заданная дата окончания акции");
            VerifyAreEqual(Driver.GetGridCell(0, "PromoUrl", "YandexPromoFlash").Text, csvData["da1Url"],
                "(1)В таблице скидок не найдена ссылка на акцию");

            AddDiscount(csvData["da1Name2"], null, DateTime.Now.ToShortDateString(),
                DateTime.Now.AddDays(3).ToShortDateString());
            VerifyAreEqual(Driver.GetGridCell(1, "Name", "YandexPromoFlash").Text, csvData["da1Name2"],
                "(2)В таблице скидок не найдено название акции");
            VerifyAreEqual(Driver.GetGridCell(1, "Description", "YandexPromoFlash").Text, "",
                "(2)В таблице скидок не найдено описание акции");
            VerifyAreEqual(Driver.GetGridCell(1, "StartDate", "YandexPromoFlash").Text,
                DateTime.Now.ToShortDateString() + " 00:00",
                "(2)В таблице промокодов не найдена заданная дата начала акции");
            VerifyAreEqual(Driver.GetGridCell(1, "ExpirationDate", "YandexPromoFlash").Text,
                DateTime.Now.AddDays(3).ToShortDateString() + " 00:00",
                "(2)В таблице промокодов не найдена заданная дата окончания акции");
            VerifyAreEqual(Driver.GetGridCell(1, "PromoUrl", "YandexPromoFlash").Text, "",
                "(2)В таблице скидок не найдена ссылка на акцию");
        }

        [Test]
        public void YandexDiscountEdit()
        {
            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed("DiscountEdit");

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            DateTime startDate = DateTime.Now;
            AddDiscount(csvData["da2Name1"], new List<string> {csvData["da2Product"]}, startDate.ToShortDateString(),
                startDate.AddDays(5).ToShortDateString(), csvData["da2Description1"], csvData["da2Url1"]);

            VerifyAreEqual(Driver.GetGridCell(0, "Name", "YandexPromoFlash").Text, csvData["da2Name1"],
                "(1)В таблице скидок не найдено название акции");
            VerifyAreEqual(Driver.GetGridCell(0, "Description", "YandexPromoFlash").Text, csvData["da2Description1"],
                "(1)В таблице скидок не найдено описание акции");
            VerifyAreEqual(Driver.GetGridCell(0, "StartDate", "YandexPromoFlash").Text,
                startDate.ToShortDateString() + " 00:00",
                "(1)В таблице скидок не найдена заданная дата начала акции");
            VerifyAreEqual(Driver.GetGridCell(0, "ExpirationDate", "YandexPromoFlash").Text,
                startDate.AddDays(5).ToShortDateString() + " 00:00",
                "(1)В таблице скидок не найдена заданная дата окончания акции");
            VerifyAreEqual(Driver.GetGridCell(0, "PromoUrl", "YandexPromoFlash").Text, csvData["da2Url1"],
                "(1)В таблице скидок не найдена ссылка на акцию");

            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoFlash")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();

            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]"), csvData["da2Name2"]);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Description\"]"), csvData["da2Description2"]);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.PromoUrl\"]"), csvData["da2Url2"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ClearProducts\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"),
                "TestProduct" + csvData["da2Product2"]);
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[0]" +
                                              "[\'selectionRowHeaderCol\']\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();

            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.AddDays(7).ToShortDateString() + Keys.Enter);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"),
                startDate.AddDays(1).ToShortDateString() + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(Driver.GetGridCell(0, "Name", "YandexPromoFlash").Text, csvData["da2Name2"],
                "(2)В таблице скидок не найдено название акции");
            VerifyAreEqual(Driver.GetGridCell(0, "Description", "YandexPromoFlash").Text, csvData["da2Description2"],
                "(2)В таблице скидок не найдено описание акции");
            VerifyAreEqual(Driver.GetGridCell(0, "StartDate", "YandexPromoFlash").Text,
                startDate.AddDays(1).ToShortDateString() + " 00:00",
                "(2)В таблице промокодов не найдена заданная дата начала акции");
            VerifyAreEqual(Driver.GetGridCell(0, "ExpirationDate", "YandexPromoFlash").Text,
                startDate.AddDays(7).ToShortDateString() + " 00:00",
                "(2)В таблице промокодов не найдена заданная дата окончания акции");
            VerifyAreEqual(Driver.GetGridCell(0, "PromoUrl", "YandexPromoFlash").Text, csvData["da2Url2"],
                "(2)В таблице скидок не найдена ссылка на акцию");
        }

        [Test]
        public void YandexDiscountRemove()
        {
            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed("DiscountRemove");

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            AddDiscount("Discount1");
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoFlash").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();

            VerifyIsFalse(IsElementExists("[data-e2e=\"gridRow\"][data-e2e-row-index=\"0\"]", "CssSelector"),
                "Промоакция не была удалена");
            Thread.Sleep(1000);

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexDiscountValidate()
        {
            //валидация полей: 
            //+без изменений не сохранятеся, +с пустым названием не сохраняется, +с пустым описанием сохр, +с пустой ссылкой сохр, 
            //+описание не более 500 символов
            //+с пустой датой начала не сохр, +с пустой датой оконч не сохр; 
            //в поле дат (1, 2) +не сохранятся тексты (очистка), +не сохранятся символ, 
            //в поле дат: если ввести 1-2 цифры - +подставится текущая дата. если ввести цифры.цифры - +подставится дата+текущ.год
            //+в поле дат можно выбрать день из списка; 
            //дата в окончании не меньше даты в начале - сообщение; дата в окончании строго больше даты в начале хотя бы на минуту.
            //с невыбранным продуктам не сохр; с выбранным, потом сброшенным. продуктом не сохр; с выбранными 2 и более прод сохр; 
            //с выбранными 2 и более из разн.кат сохраняются. С выбранным неактивным сохраняется. С выбранным недоступным сохраняется. 
            //*это же при редактировании
            //***добавить акцию для товара, удалить товар, проверить акцию (должен сброситься товар)

            GoToAdmin(adminPath);
            DateTime startDate = DateTime.Now;

            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed("DiscountValidate");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-controller=\"'ModalAddEditYandexPromoFlashCtrl'\"] [data-e2e=\"AddYandexPromo\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).GetAttribute("disabled") == "true",
                "Кнопка 'сохранить акцию (ok)' доступна без изменения формы");
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]"), csvData["da4Name1"]);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"),
                startDate.ToShortDateString() + Keys.Enter);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.AddDays(3).ToShortDateString() + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyIsTrue(IsElementExists("error", "ClassName"),
                "Не обнаружено сообщение о запрете акции без продуктов при добавлении акции");
            Driver.FindElement(By.ClassName("close")).Click();
            Thread.Sleep(1000);
            string[] arrayOfProductIndex = csvData["da4ProductsID"].Split(',');
            AddDiscount(csvData["da4Name1"], new List<string> {arrayOfProductIndex[0]}, startDate.ToShortDateString(),
                startDate.AddDays(5).ToShortDateString(), csvData["text252"] + csvData["text252"], csvData["da4Url1"]);

            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoFlash")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).GetAttribute("disabled") == "true",
                "(2)Кнопка 'сохранить акцию (ok)' доступна без изменения формы");

            Driver.ClearInput(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]"), csvData["da4Name1"]);

            Driver.ClearInput(By.CssSelector("[ng-model=\"ctrl.promo.PromoUrl\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoFlash")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();

            VerifyAreEqual(
                Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Description\"]")).GetAttribute("value"),
                (csvData["text252"] + csvData["text252"]).Substring(0, 500),
                "Максимально возможное описание не равно эталонным 500 символов!");
            Driver.ClearInput(By.CssSelector("[ng-model=\"ctrl.promo.Description\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoFlash")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();
            //пустота дат
            Driver.ClearInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"),
                startDate.ToShortDateString() + Keys.Enter);

            Driver.ClearInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.AddDays(5).ToShortDateString() + Keys.Enter);
            //текст, символ в поля дат.
            //старт дата

            Functions.DataTimePickerDay(Driver, BaseUrl, ".modal-body .text-required",
                ".flatpickr-day.today~.flatpickr-day");
            string startDate1 = Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"))
                .GetAttribute("value");
            VerifyIsTrue((startDate1 == startDate.AddDays(1).ToShortDateString() + " 12:00"
                          || startDate1 == startDate.AddDays(1).ToShortDateString() + " 00:00"),
                "(2)Выбор даты по календарю не прошел валидацию");
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"), "first" + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"), "." + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"), "14" + Keys.Enter);
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Click();
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]")).GetAttribute("value"),
                "14.01." + DateTime.Now.Year + " 00:00",
                "(1)Валидация стартовой даты - '14'");
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"),
                startDate.ToShortDateString() + Keys.Enter);

            //окончат. дата
            Driver.FindElement(By.CssSelector(".text-required")).Click();
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Click();
            Functions.DataTimePickerDay(Driver, BaseUrl, ".modal-body .text-required",
                ".flatpickr-day.selected~.flatpickr-day", 1);
            string expirationDate = Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"))
                .GetAttribute("value");
            VerifyIsTrue((expirationDate == startDate.AddDays(6).ToShortDateString() + " 12:00"
                          || expirationDate == startDate.AddDays(6).ToShortDateString() + " 00:00"),
                "(2)Выбор даты по календарю не прошел валидацию");
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"), "second" + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"), "." + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"), "14" + Keys.Enter);
            Driver.FindElement(By.CssSelector(".text-required")).Click();
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Click();
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]")).GetAttribute("value"),
                "14.01." + DateTime.Now.Year + " 00:00",
                "(2)Валидация стартовой даты - '14'");
            //более 7 дней
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.AddDays(8).ToShortDateString() + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyIsTrue(IsElementExists("error", "ClassName"),
                "Не обнаружено сообщение о запрете акции длительностью более 7 дней");
            //7 дней
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.AddDays(7).ToShortDateString() + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoFlash")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            //0 дней
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.ToShortDateString() + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyIsTrue(IsElementExists("error", "ClassName"),
                "Не обнаружено сообщение о запрете акции длительностью 0 дней 0 минут");
            //0 дней + 1 мин
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.ToShortDateString() + " 00:01" + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoFlash")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();

            //валидация продуктов
            //с невыбранным продуктам не сохр; с выбранным, потом сброшенным. продуктом не сохр; с выбранными 2 и более прод сохр; 
            //с выбранными 2 и более из разн.кат сохраняются. С выбранным неактивным сохраняется. С выбранным недоступным сохраняется. 
            Driver.FindElement(By.CssSelector("[data-e2e=\"ClearProducts\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyIsTrue(IsElementExists("error", "ClassName"),
                "Не обнаружено сообщение о запрете акции без продуктов при редактировании");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            AddProductToPromo(arrayOfProductIndex[0]);
            AddProductToPromo(arrayOfProductIndex[1]);
            Driver.FindElement(By.ClassName("btn-save")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.XPath("//ui-modal-trigger/preceding-sibling::span")).Text
                    .IndexOf("2 товар(ов)") != -1,
                "(1)Неверное отображение количества добавленных товаров");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ClearProducts\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            AddProductToPromo(arrayOfProductIndex[0]);
            AddProductToPromo(arrayOfProductIndex[2]);
            Driver.FindElement(By.ClassName("btn-save")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.XPath("//ui-modal-trigger/preceding-sibling::span")).Text
                    .IndexOf("2 товар(ов)") != -1,
                "(2)Неверное отображение количества добавленных товаров");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ClearProducts\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            string[] arrayOfDisabledProductIndex = csvData["da4ProductsDisabledID"].Split(',');

            AddProductToPromo(arrayOfDisabledProductIndex[0]);
            AddProductToPromo(arrayOfDisabledProductIndex[1]);
            AddProductToPromo(arrayOfDisabledProductIndex[2]);

            Driver.FindElement(By.ClassName("btn-save")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.XPath("//ui-modal-trigger/preceding-sibling::span")).Text
                    .IndexOf("3 товар(ов)") != -1,
                "(3)Неверное отображение количества добавленных товаров (для недоступных товаров)");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Thread.Sleep(1000);
        }

        [Test]
        public void YandexDiscountDisplay()
        {
            //акция отображается в файле выгрузки:
            //1) +промо отображается. +count of промо верное. 
            //+у промо есть идентификатор. +Длина идентификатора не более 20.
            //+описание промо отображается, +ссылка на промо отображается
            //+пустое описание не отображается, +пустая ссылка не отображается.
            //+у промо тип Флеш-дискоунт
            //+у промо есть старт-дата, у промо есть енд-дата; стартдата, энддата в правильном формате
            //++у промо есть teg пурчайз, внутри пурчайз есть продукты. КОличество продуктов соответствует ожидаемому.
            //у продукта есть teg дискоутн-прайс; в tegе есть валюта; разные типы валют отображаются;
            //в tegе есть цена; цена соовтетствует ожидаемой.
            //у товаров, для которых есть промоакции, не отображается teg oldprice; у участв товаров в tegе price  цена без скидки11

            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed("DiscountDisplay");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Categories\"]")).Click();
            //в первой каtegории 4 товара с разными скидками и 1 товар без скидки
            Driver.FindElement(By.CssSelector("[id=\"1_anchor\"]")).Click();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            DateTime dateNow = DateTime.Now;
            string[] arrayOfProductIndex = csvData["da5ProductsID"].Split(',');
            AddDiscount(csvData["da5Name1"], arrayOfProductIndex.ToList(),
                dateNow.ToShortDateString() + " 12:00",
                dateNow.AddDays(5).ToShortDateString() + " 12:00",
                csvData["da5Desc1"], csvData["da5Url1"]);
            Thread.Sleep(1000);
            AddDiscount(csvData["da5Name2"], new List<string> {arrayOfProductIndex[0]},
                dateNow.ToShortDateString() + " 12:00",
                dateNow.AddDays(6).ToShortDateString() + " 12:00");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            string promos = GetPromosFromXml(xmlResult);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["da5ProductCount"]),
                "количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(promos, "<promo ") == Convert.ToInt32(csvData["da5PromoCount"]),
                "количество tegs promo not expected");
            //первая промо - с описанием, урл, 4 товара
            string promo = GetPromoFromXml(promos, 1);
            VerifyIsTrue(promo.ToLower().Substring(11, promo.IndexOf("\" type=") - 12).Length <= 20,
                "(1)В promo длина id промоакции больше 20 символов");
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["daPromoType"])) != -1,
                "(1)В promo не найдена подстрока с типом купона ");
            VerifyIsTrue(GetTegContent(promo, "description") == csvData["da5Desc1"].ToLower(),
                "(1)В promo description не соответствует эталонным");
            VerifyIsTrue(GetTegContent(promo, "url") == csvData["da5Url1"].ToLower(),
                "(1)В promo url не соответствует эталонным");
            VerifyIsTrue(GetTegContent(promo, "start-date") == DateToYmlFormat(dateNow),
                "(1)Дата начала акции not expected с сегодняшней датой");
            VerifyIsTrue(GetTegContent(promo, "end-date") == DateToYmlFormat(dateNow.AddDays(5)),
                "(1)Дата окончания акции not expected с заданной");
            VerifyIsTrue(CountOfStrInXml(promo, "<product offer-id=") == Convert.ToInt32(csvData["da5ProductCount1"]),
                "(1) Количество продуктов для промоакции не совпало с ожидаемым");
            VerifyIsTrue(GetTegContent(promo, "purchase") == ConvertCsvString(csvData["da5PurchaseContent1"]),
                "(1) Содержимое tegа purchase не совпало с ожидаемым");

            //вторая промо - без описания, url, 1 товар
            promo = GetPromoFromXml(promos, 2);
            VerifyIsTrue(promo.ToLower().Substring(11, promo.IndexOf("\" type=") - 12).Length <= 20,
                "(2)В promo длина id промоакции больше 20 символов");
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["daPromoType"])) != -1,
                "(2)В promo не найдена подстрока с типом купона ");
            VerifyIsTrue(promo.IndexOf("description") == -1,
                "(2)В promo 2 найден teg описания, кот.должен быть пустым");
            VerifyIsTrue(promo.IndexOf("url") == -1,
                "(2)В promo 2 найден teg ссылки, кот.должен быть пустым");
            VerifyIsTrue(GetTegContent(promo, "start-date") == DateToYmlFormat(dateNow),
                "(2)Дата начала акции not expected с сегодняшней датой");
            VerifyIsTrue(GetTegContent(promo, "end-date") == DateToYmlFormat(dateNow.AddDays(6)),
                "(2)Дата окончания акции not expected с заданной");
            VerifyIsTrue(CountOfStrInXml(promo, "<product offer-id=") == Convert.ToInt32(csvData["da5ProductCount2"]),
                "(2) Количество продуктов для промоакции не совпало с ожидаемым");
            VerifyIsTrue(GetTegContent(promo, "purchase") == ConvertCsvString(csvData["da5PurchaseContent2"]),
                "(2) Содержимое tegа purchase не совпало с ожидаемым");
            ReturnToExport();

            //ПОВТОРИТЬ, НО ЗАДАТЬ ВАЛЮТУ ДОЛЛАРЫ и другой артикул
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.ScrollTo(By.Id("ddlCurrency"));
            SelectOption("ddlCurrency", "USD");
            SelectOption("ddlOfferIdType", "artno");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            xmlResult = GetXmlFromYZip(exportName);
            promos = GetPromosFromXml(xmlResult);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["da5ProductCount"]),
                "количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(promos, "<promo ") == Convert.ToInt32(csvData["da5PromoCount"]),
                "количество tegs promo not expected");
            //первая промо - с описанием, урл, 4 товара
            promo = GetPromoFromXml(promos, 1);
            VerifyIsTrue(promo.ToLower().Substring(11, promo.IndexOf("\" type=") - 12).Length <= 20,
                "(usd1)В promo длина id промоакции больше 20 символов");
            VerifyIsTrue(CountOfStrInXml(promo, "<product offer-id=") == Convert.ToInt32(csvData["da5ProductCount1"]),
                "(usd1) Количество продуктов для промоакции не совпало с ожидаемым");
            VerifyIsTrue(GetTegContent(promo, "purchase") == ConvertCsvString(csvData["da5PurchaseContent3"]),
                "(usd1) Содержимое tegа purchase не совпало с ожидаемым");


            //вторая промо - без описания, url, 1 товар
            promo = GetPromoFromXml(promos, 2);
            VerifyIsTrue(promo.ToLower().Substring(11, promo.IndexOf("\" type=") - 12).Length <= 20,
                "(usd2)В promo длина id промоакции больше 20 символов");
            VerifyIsTrue(CountOfStrInXml(promo, "<product offer-id=") == Convert.ToInt32(csvData["da5ProductCount2"]),
                "(usd2) Количество продуктов для промоакции не совпало с ожидаемым");
            VerifyIsTrue(GetTegContent(promo, "purchase") == ConvertCsvString(csvData["da5PurchaseContent4"]),
                "(usd2) Содержимое tegа purchase не совпало с ожидаемым");
            ReturnToExport();

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }
    }
}