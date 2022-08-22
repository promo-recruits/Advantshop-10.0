using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex.NplusM
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexNplusMAddRemoveTest : ExportServices
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
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.Currency.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.Color.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.Size.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.Photo.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.Category.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.Property.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.Product.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.Offer.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.ProductExt.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.Tag.csv",
                "Data\\Admin\\Export\\Catalog\\NplusM\\AddRemove\\Catalog.ProductPropertyValue.csv"
            );
            csvData = Functions.LoadCsvFile("Data\\Admin\\Export\\TestSettings\\YandexPromos\\NplusMAddRemoveData.csv");
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
        public void YandexNplusMAdd()
        {
            //добавление акции, добавление более одной акции
            GoToAdmin(adminPath);

            AddExportFeed("NplusMAdd");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();

            AddNplusM(
                "NplusMAdd1", new List<string> {"1"}, new List<string> {"1"},
                "2", "1", DateTime.Now.ToShortDateString(), DateTime.Now.AddDays(3).ToShortDateString(),
                csvData["npm1Description"], csvData["npm1Url"]);
            VerifyAreEqual(Driver.GetGridCell(0, "Name", "YandexPromoNPlusM").Text, "NplusMAdd1",
                "(1)В таблице не найдено название акции");
            VerifyAreEqual(Driver.GetGridCell(0, "Description", "YandexPromoNPlusM").Text, csvData["npm1Description"],
                "(1)В таблице не найдено описание акции");
            VerifyAreEqual(Driver.GetGridCell(0, "StartDate", "YandexPromoNPlusM").Text,
                DateTime.Now.ToShortDateString() + " 00:00",
                "(1)В таблице не найдена заданная дата начала акции");
            VerifyAreEqual(Driver.GetGridCell(0, "ExpirationDate", "YandexPromoNPlusM").Text,
                DateTime.Now.AddDays(3).ToShortDateString() + " 00:00",
                "(1)В таблице не найдена заданная дата окончания акции");
            VerifyAreEqual(Driver.GetGridCell(0, "PromoUrl", "YandexPromoNPlusM").Text, csvData["npm1Url"],
                "(1)В таблице не найдена ссылка на акцию");

            AddNplusM(
                "NplusMAdd1", new List<string> {"2"}, null,
                "1", "2", DateTime.Now.ToShortDateString(), DateTime.Now.AddDays(3).ToShortDateString());
            VerifyAreEqual(Driver.GetGridCell(1, "Name", "YandexPromoNPlusM").Text, "NplusMAdd1",
                "(2)В таблице не найдено название акции");
            VerifyAreEqual(Driver.GetGridCell(1, "Description", "YandexPromoNPlusM").Text, "",
                "(2)В таблице не найдено описание акции");
            VerifyAreEqual(Driver.GetGridCell(1, "StartDate", "YandexPromoNPlusM").Text,
                DateTime.Now.ToShortDateString() + " 00:00",
                "(2)В таблице не найдена заданная дата начала акции");
            VerifyAreEqual(Driver.GetGridCell(1, "ExpirationDate", "YandexPromoNPlusM").Text,
                DateTime.Now.AddDays(3).ToShortDateString() + " 00:00",
                "(2)В таблице не найдена заданная дата окончания акции");
            VerifyAreEqual(Driver.GetGridCell(1, "PromoUrl", "YandexPromoNPlusM").Text, "",
                "(2)В таблице не найдена ссылка на акцию");
        }

        [Test]
        public void YandexNplusMEdit()
        {
            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed("NplusMEdit");

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            DateTime startDate = DateTime.Now;
            AddNplusM(csvData["npm2Name1"], new List<string> {"1"}, new List<string> {"1"}, "2",
                "1", DateTime.Now.ToShortDateString(), DateTime.Now.AddDays(5).ToShortDateString(),
                csvData["npm2Description1"], csvData["npm2Url1"]);

            VerifyAreEqual(Driver.GetGridCell(0, "Name", "YandexPromoNPlusM").Text, csvData["npm2Name1"],
                "(1)В таблице не найдено название акции");
            VerifyAreEqual(Driver.GetGridCell(0, "Description", "YandexPromoNPlusM").Text, csvData["npm2Description1"],
                "(1)В таблице не найдено описание акции");
            VerifyAreEqual(Driver.GetGridCell(0, "StartDate", "YandexPromoNPlusM").Text,
                startDate.ToShortDateString() + " 00:00",
                "(1)В таблице не найдена заданная дата начала акции");
            VerifyAreEqual(Driver.GetGridCell(0, "ExpirationDate", "YandexPromoNPlusM").Text,
                startDate.AddDays(5).ToShortDateString() + " 00:00",
                "(1)В таблице не найдена заданная дата окончания акции");
            VerifyAreEqual(Driver.GetGridCell(0, "PromoUrl", "YandexPromoNPlusM").Text, csvData["npm2Url1"],
                "(1)В таблице не найдена ссылка на акцию");

            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();

            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]"), csvData["npm2Name2"]);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Description\"]"), csvData["npm2Description2"]);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.PromoUrl\"]"), csvData["npm2Url2"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ClearProducts\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), "TestProduct2");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[0]" +
                                              "[\'selectionRowHeaderCol\']\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"), "TestProduct3");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[0]" +
                                              "[\'selectionRowHeaderCol\']\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();

            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategories\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("a[id=\"1_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("a[id=\"2_anchor\"]")).Click();

            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.AddDays(7).ToShortDateString() + Keys.Enter);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"),
                startDate.AddDays(1).ToShortDateString() + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(Driver.GetGridCell(0, "Name", "YandexPromoNPlusM").Text, csvData["npm2Name2"],
                "(2)В таблице не найдено название акции");
            VerifyAreEqual(Driver.GetGridCell(0, "Description", "YandexPromoNPlusM").Text, csvData["npm2Description2"],
                "(2)В таблице не найдено описание акции");
            VerifyAreEqual(Driver.GetGridCell(0, "StartDate", "YandexPromoNPlusM").Text,
                startDate.AddDays(1).ToShortDateString() + " 00:00",
                "(2)В таблице не найдена заданная дата начала акции");
            VerifyAreEqual(Driver.GetGridCell(0, "ExpirationDate", "YandexPromoNPlusM").Text,
                startDate.AddDays(7).ToShortDateString() + " 00:00",
                "(2)В таблице не найдена заданная дата окончания акции");
            VerifyAreEqual(Driver.GetGridCell(0, "PromoUrl", "YandexPromoNPlusM").Text, csvData["npm2Url2"],
                "(2)В таблице не найдена ссылка на акцию");
        }

        [Test]
        public void YandexNplusMRemove()
        {
            GoToAdmin(adminPath);
            //создание выгрузки и установка имени выгрузки, экспорта в зип-архив
            AddExportFeed("NplusMRemove");

            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            AddNplusM("NplusMRemove", null, new List<string> {"1"});
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();

            VerifyIsFalse(IsElementExists("[data-e2e=\"gridRow\"][data-e2e-row-index=\"0\"]", "CssSelector"),
                "Промоакция не была удалена");
            Thread.Sleep(1000);

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void YandexNplusMValidate()
        {
            //валидация полей: 
            //+без изменений не сохранятеся, +с пустым названием не сохраняется, +с пустым описанием сохр, +с пустой ссылкой сохр, 
            //+описание не более 500 символов
            //+с пустой датой начала и с пустой датой оконч сохр; 
            //+с пустой датой начала и с заполн датой оконч не сохр; 
            //+с заполн датой начала и с пустой датой оконч не сохр; 
            //+с заполн датой начала и с заполн датой оконч сохр; 
            //+в поле дат (1, 2) не сохранятся тексты (очистка), +не сохранятся символ, 
            //+в поле дат: если ввести 1-2 цифры - подставится текущая дата. если ввести цифры.цифры - подставится дата+текущ.год
            //+в поле дат можно выбрать день из списка; 
            //+дата в окончании не меньше даты в начале - сообщение; дата в окончании строго больше даты в начале хотя бы на минуту.
            //+валидация товаров за полн цену, +валидация бесплатных товаров
            //валидация продуктов
            //валидация каtegорий
            //есть кнопка Изменить и Сбросить товары
            //есть кнопка Изменить и Сбросить каtegории
            //если не выбран кат и прод - не сохраняется.
            //если выбран кат и прод - сохраняется.
            //с невыбранным продуктам не сохр; с выбранным, потом сброшенным. продуктом не сохр; с выбранными 2 и более прод сохр; 
            //С выбранным неактивным сохраняется. С выбранным недоступным сохраняется. 
            //с невыбранным кат не сохр; с выбранным, потом сброшенным. кат не сохр; 
            //с выбранными 2 и более кат сохр

            //*это же при редактировании
            //***добавить акцию для товара, удалить товар, проверить акцию (должен сброситься товар)
            //***добавить акцию для подарка, удалить подарка, проверить акцию (должен сброситься подарка)
            GoToAdmin(adminPath);
            DateTime startDate = DateTime.Now;
            string[] arrayOfProductIndex = csvData["npm4ProductsID"].Split(',');
            string text252 = csvData["text252"];

            AddExportFeed("NplusMValidate");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-controller=\"'ModalAddEditYandexPromoNPlusMCtrl'\"] " +
                                              "[data-e2e=\"AddYandexPromo\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).GetAttribute("disabled") == "true",
                "Кнопка 'сохранить акцию (ok)' доступна без изменения формы");
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]"), csvData["npm4Name1"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector("label.error")).Count,
                "Не обнаружено(ы) сообщения о запрете акции без продуктов или каtegорий при добавлении акции");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            AddProductToPromo(arrayOfProductIndex[0]);
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM").FindElement(By.CssSelector("a.fa-times"))
                .Click();
            Driver.SwalConfirm();

            AddNplusM(
                csvData["npm4Name1"], new List<string> {arrayOfProductIndex[0]}, null,
                "2", "1", startDate.ToShortDateString(), startDate.AddDays(5).ToShortDateString(),
                text252 + text252, csvData["npm4Url1"]);

            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).GetAttribute("disabled") == "true",
                "(2)Кнопка 'сохранить акцию (ok)' доступна без изменения формы");

            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]"), csvData["npm4Name1"]);

            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.PromoUrl\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();

            VerifyAreEqual(
                Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Description\"]")).GetAttribute("value"),
                (text252 + text252).Substring(0, 500),
                "Максимально возможное описание не равно эталонным 500 символов!");
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Description\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();
            //пустота дат
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector("label.error")).Count,
                "(1)Не обнаружено сообщение о необходимости обоих дат");
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"),
                startDate.ToShortDateString() + Keys.Enter);

            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector("label.error")).Count,
                "(2)Не обнаружено сообщение о необходимости обоих дат");
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.AddDays(5).ToShortDateString() + Keys.Enter);

            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();

            //текст, символ в поля дат.
            //старт дата
            Driver.FindElement(
                    By.XPath(
                        "//input[@ng-model='ctrl.promo.StartDate']/../following::span[@class='input-group-addon']"))
                .Click();
            Driver.FindElements(By.CssSelector(".flatpickr-calendar"))[0]
                .FindElement(By.CssSelector(".flatpickr-day.today~.flatpickr-day")).Click();
            string startDate1 = Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"))
                .GetAttribute("value");
            VerifyIsTrue((startDate1 == startDate.AddDays(1).ToShortDateString() + " 12:00"
                          || startDate1 == startDate.AddDays(1).ToShortDateString() + " 00:00"),
                "(1)Выбор даты по календарю не прошел валидацию");
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"), "first" + Keys.Enter);
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]")).Click();
            VerifyIsTrue(
                string.IsNullOrWhiteSpace(
                    Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]")).Text),
                "(1)При вводе слова поле даты не очищается");
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]"), "." + Keys.Enter);
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]")).Click();
            VerifyIsTrue(
                string.IsNullOrWhiteSpace(
                    Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.StartDate\"]")).Text),
                "(1)При вводе символа поле даты не очищается");
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
            Driver.FindElement(By.XPath(
                    "//input[@ng-model='ctrl.promo.ExpirationDate']/../following::span[@class='input-group-addon']"))
                .Click();
            Driver.FindElements(By.CssSelector(".flatpickr-calendar"))[1]
                .FindElement(By.CssSelector(".flatpickr-day.today~.flatpickr-day")).Click();
            string expirationDate = Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"))
                .GetAttribute("value");
            VerifyIsTrue((expirationDate == startDate.AddDays(1).ToShortDateString() + " 12:00" || expirationDate
                    == startDate.AddDays(1).ToShortDateString() + " 00:00"),
                "(2)Выбор даты по календарю не прошел валидацию");
            Thread.Sleep(1000);
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"), "second" + Keys.Enter);
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]")).Click();
            VerifyIsTrue(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"))
                    .Text),
                "(2)При вводе слова поле даты не очищается");
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"), "." + Keys.Enter);
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]")).Click();
            VerifyIsTrue(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"))
                    .Text),
                "(2)При вводе символа поле даты не очищается");
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"), "14" + Keys.Enter);
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Click();
            VerifyAreEqual(
                Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]")).GetAttribute("value"),
                "14.01." + DateTime.Now.Year + " 00:00",
                "(2)Валидация стартовой даты - '14'");

            //-1 день
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.AddDays(-1).ToShortDateString() + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector("label.error")).Count,
                "Не обнаружено сообщение о недопустимой длине акции (-1 день)");

            //более 7 дней - можно, нет ограничения по срокам
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.AddDays(8).ToShortDateString() + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();

            //0 дней
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.ToShortDateString() + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyIsTrue(IsElementExists("label.error", "CssSelector"),
                "Не обнаружено сообщение о запрете акции длительностью 0 дней 0 минут");

            //0 дней + 1 мин
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"ctrl.promo.ExpirationDate\"]"),
                startDate.ToShortDateString() + " 00:01" + Keys.Enter);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();

            //валидация продуктов

            Driver.FindElement(By.CssSelector("[data-e2e=\"ClearProducts\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyIsTrue(IsElementExists("label.error", "CssSelector"),
                "Не обнаружено сообщение о запрете акции без продуктов и каtegорий при редактировании");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            AddProductToPromo(arrayOfProductIndex[0]);
            Driver.FindElement(By.ClassName("btn-save")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.XPath("//ui-modal-trigger/preceding-sibling::span")).Text
                    .IndexOf("1 товар(ов)") != -1,
                "(1)Неверное отображение количества добавленных товаров");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            AddProductToPromo(arrayOfProductIndex[1]);
            AddProductToPromo(arrayOfProductIndex[2]);
            Driver.FindElement(By.ClassName("btn-save")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.XPath("//ui-modal-trigger/preceding-sibling::span")).Text
                    .IndexOf("3 товар(ов)") != -1,
                "(1.1)Неверное отображение количества добавленных товаров");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();

            string[] arrayOfDisabledProductIndex = csvData["npm4ProductsDisabledID"].Split(',');
            Driver.FindElement(By.CssSelector("[data-e2e=\"ClearProducts\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddProducts\"]")).Click();
            AddProductToPromo(arrayOfDisabledProductIndex[0]);
            AddProductToPromo(arrayOfDisabledProductIndex[1]);
            AddProductToPromo(arrayOfDisabledProductIndex[2]);

            Driver.FindElement(By.ClassName("btn-save")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.XPath("//ui-modal-trigger/preceding-sibling::span")).Text
                    .IndexOf("3 товар(ов)") != -1,
                "(1.2)Неверное отображение количества добавленных товаров (для недоступных товаров)");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();

            //валидация каtegорий
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategories\"]")).Click();
            Driver.FindElement(By.CssSelector("a[id=\"1_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ClearProducts\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ClearCategories\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyIsTrue(IsElementExists("label.error", "CssSelector"),
                "Не обнаружено сообщение о запрете акции без продуктов и каtegорий при редактировании");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategories\"]")).Click();
            Driver.FindElement(By.CssSelector("a[id=\"1_anchor\"]")).Click();
            Driver.FindElement(By.CssSelector("a[id=\"2_anchor\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("li.jstree-node.category-item-active.jstree-closed:not(.jstree-leaf) " +
                                              "i.jstree-icon.jstree-ocl")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("a[id=\"4_anchor\"]")).Click();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();

            //количество продуктов за полн цену
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] " +
                                                           "input.spinbox-input")).GetAttribute("value") == "2",
                "(1)Дефолтное count of товаров за полн цену != 2");
            Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"))
                .SendKeys(Keys.Backspace);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector("label.error")).Count,
                "(1)Не обнаружено сообщение необходимости ввода кол-ва товаров за полн цену");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"),
                "one");
            VerifyIsTrue(string.IsNullOrWhiteSpace(Driver.FindElement(By.CssSelector(
                    "[data-value=\"ctrl.promo.RequiredQuantity\"] " +
                    "input.spinbox-input")).GetAttribute("value")),
                "(1)В count of товаров можно ввести буквы");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"),
                ",!@\"'*()+-*/|");
            VerifyIsTrue(string.IsNullOrWhiteSpace(Driver.FindElement(By.CssSelector(
                    "[data-value=\"ctrl.promo.RequiredQuantity\"] " +
                    "input.spinbox-input")).GetAttribute("value")), "" +
                                                                    "(1)В count of товаров можно ввести символы");
            Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"))
                .SendKeys(Keys.Backspace);
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"),
                "1.2");
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Click();
            VerifyAreEqual(Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] " +
                                                             "input.spinbox-input")).GetAttribute("value"), "2",
                "(1)результат ввода 1,2 не совпал с эталоном");
            
            Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"))
                .SendKeys(Keys.Backspace);
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"),
                "6.8");
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Click();
            VerifyAreEqual(Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] " +
                                                             "input.spinbox-input")).GetAttribute("value"), "7",
                "(1)результат ввода 6.8 не совпал с эталоном");

            Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"))
                .SendKeys(Keys.Backspace);
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"),
                "0");
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Click();
            VerifyAreEqual(Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] " +
                                                             "input.spinbox-input")).GetAttribute("value"), "1",
                "(1)результат ввода 0 не совпал с эталоном");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"),
                "3");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual(Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] " +
                                                             "input.spinbox-input")).GetAttribute("value"), "3",
                "(1)результат ввода 6.8 не совпал с эталоном");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"),
                "25");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.RequiredQuantity\"] input.spinbox-input"),
                "24");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();

            //количество подарков
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] " +
                                                           "input.spinbox-input")).GetAttribute("value") == "1",
                "(2)Дефолтное count of товаров за полн цену != 1");
            Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] input.spinbox-input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector("label.error")).Count,
                "(2)Не обнаружено сообщение необходимости ввода кол-ва подарков");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] input.spinbox-input"), "one");
            VerifyIsTrue(string.IsNullOrWhiteSpace(Driver.FindElement(By.CssSelector(
                    "[data-value=\"ctrl.promo.FreeQuantity\"] " +
                    "input.spinbox-input")).GetAttribute("value")),
                "(2)В count of товаров можно ввести буквы");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] input.spinbox-input"),
                ",!@\"'*()+-*/|");
            VerifyIsTrue(string.IsNullOrWhiteSpace(Driver.FindElement(By.CssSelector(
                    "[data-value=\"ctrl.promo.FreeQuantity\"] " +
                    "input.spinbox-input")).GetAttribute("value")),
                "(2)В count of товаров можно ввести символы");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] input.spinbox-input"), "1.2");
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Click();
            VerifyAreEqual(Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] " +
                                                             "input.spinbox-input")).GetAttribute("value"), "2",
                "(2)результат ввода 1,2 не совпал с эталоном");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] input.spinbox-input"), "6.8");
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Click();
            VerifyAreEqual(Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] " +
                                                             "input.spinbox-input")).GetAttribute("value"), "7",
                "(2)результат ввода 6.8 не совпал с эталоном");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] input.spinbox-input"), "0");
            Driver.FindElement(By.CssSelector("[ng-model=\"ctrl.promo.Name\"]")).Click();
            VerifyAreEqual(Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] " +
                                                             "input.spinbox-input")).GetAttribute("value"), "1",
                "(2)результат ввода 0 не совпал с эталоном");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] input.spinbox-input"), "3");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.GetGridCell(0, "_serviceColumn", "YandexPromoNPlusM")
                .FindElement(By.CssSelector("[data-e2e=\"btnEdit\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual(Driver.FindElement(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] " +
                                                             "input.spinbox-input")).GetAttribute("value"), "3",
                "(2)результат ввода 6.8 не совпал с эталоном");
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] input.spinbox-input"), "25");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.CssSelector("[data-value=\"ctrl.promo.FreeQuantity\"] input.spinbox-input"), "24");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Thread.Sleep(1000);
        }

        [Test]
        public void YandexNplusMDisplay()
        {
            //акция отображается в файле выгрузки:
            //промо отображается. count of промо верное. 
            //у промо есть идентификатор. Длина идентификатора не более 20.
            //описание промо отображается, ссылка на промо отображается
            //пустое описание не отображается, пустая ссылка не отображается, пустая дата не отображается
            //у промо тип n+m; 
            //у промо еть teg required-quantity, его содержимое равно кол-ву указанных.
            //у промо еть teg free-quantity, его содержимое равно кол-ву указанных.
            //у промо есть старт-дата, у промо есть енд-дата; стартдата, энддата в правильном формате
            //у промо есть teg purchase, в нем есть продукты с id, в нем есть каtegории
            //в offer-id отображаются участвующие в выгрузке товары
            //в category-id отображаются учавствующие в выгрузке каtegории

            GoToAdmin(adminPath);
            AddExportFeed("NplusMDisplay");
            string exportName = SetCommonYExportSettings(TestName, exportPath, csvData["defaultExportType"]);
            Driver.CheckBoxUncheck("ExportNotAvailable");
            Driver.CheckBoxUncheck("AllowPreOrderProducts");
            Driver.CheckBoxCheck("OnlyMainOfferToExport");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            DateTime dateNow = DateTime.Now;
            AddNplusM("NplusMDisplay1", csvData["npm5ProductID"].Split(',').ToList(), null,
                csvData["npm5RQuantity"], csvData["npm5FQuantity"],
                dateNow.ToShortDateString() + " 12:00", dateNow.AddDays(5).ToShortDateString() + " 12:00",
                csvData["npm5Desc"], csvData["npm5Url"]);
            AddNplusM("NplusMDisplay2", null, csvData["npm5CategoryID"].Split(',').ToList());

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            string xmlResult = GetXmlFromYZip(exportName);
            string promos = GetPromosFromXml(xmlResult);
            VerifyIsTrue(CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["npm5ProductCount"]),
                "(1)количество товаров в выгрузке not expected");
            VerifyIsTrue(CountOfStrInXml(promos, "<promo ") == 2,
                "(1)количество tegs promo not expected");

            //первая промо - с описанием, урл, с офферами
            string promo = GetPromoFromXml(promos, 1);
            VerifyIsTrue(promo.ToLower().Substring(11, promo.IndexOf("\" type=") - 12).Length <= 20,
                "(1)В promo длина id промоакции больше 20 символов");
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["npmPromoType"])) != -1,
                "(1)В promo не найдена подстрока с типом купона ");
            VerifyIsTrue(GetTegContent(promo, "description") == csvData["npm5Desc"].ToLower(),
                "(1)В promo description не соответствует эталонным");
            VerifyIsTrue(GetTegContent(promo, "url") == csvData["npm5Url"].ToLower(),
                "(1)В promo url не соответствует эталонным");
            VerifyIsTrue(GetTegContent(promo, "start-date") == DateToYmlFormat(dateNow),
                "(1)Дата начала акции not expected с сегодняшней датой");
            VerifyIsTrue(GetTegContent(promo, "end-date") == DateToYmlFormat(dateNow.AddDays(5)),
                "(1)Дата окончания акции not expected с заданной");
            VerifyIsTrue(promo.IndexOf("<purchase>") != -1,
                "(1)В promo не найден teg <purchase> ");
            VerifyIsTrue(GetTegContent(promo, "purchase").IndexOf(csvData["npm5QuantityContent1"]) != -1,
                "(1)В promo quality not expected с эталонным");
            VerifyIsTrue(
                GetTegContent(promo, "purchase").IndexOf(ConvertCsvString(csvData["npm5ProductContent1"])) != -1,
                "(1)В promo products not expected с эталонным");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["npm5PurchaseContent1"]),
                "(1)Содержимое tegа purchase not expected");
            //вторая промо - без описания, url, 1 товар, с каtegориями
            promo = GetPromoFromXml(promos, 2);
            VerifyIsTrue(promo.ToLower().Substring(11, promo.IndexOf("\" type=") - 12).Length <= 20,
                "(1)В promo 2 длина id промоакции больше 20 символов");
            VerifyIsTrue(promo.IndexOf(ConvertCsvString(csvData["npmPromoType"])) != -1,
                "(1)В promo 2 не найдена подстрока с типом купона ");
            VerifyIsTrue(promo.IndexOf("description") == -1,
                "(1)В promo 2 найден teg описания, кот.должен быть пустым");
            VerifyIsTrue(promo.IndexOf("url") == -1,
                "(1)В promo 2 найден teg ссылки, кот.должен быть пустым");
            VerifyIsTrue(promo.IndexOf("start-date") == -1,
                "(1)В promo 2 найден teg даты начала, кот.должен быть пустым");
            VerifyIsTrue(promo.IndexOf("end-date") == -1,
                "(1)В promo 2 найден teg даты окончания, кот.должен быть пустым");
            VerifyIsTrue(promo.IndexOf("<purchase>") != -1,
                "(1)В promo 2 не найден teg <purchase> ");
            VerifyIsTrue(GetTegContent(promo, "purchase").IndexOf(csvData["npm5QuantityContent2"]) != -1,
                "(1)В promo 2 quality not expected с эталонным");
            VerifyIsTrue(
                GetTegContent(promo, "purchase").IndexOf(ConvertCsvString(csvData["npm5ProductContent2"])) != -1,
                "(1)В promo 2 products not expected с эталонным");
            VerifyAreEqual(GetTegContent(promo, "purchase"), ConvertCsvString(csvData["npm5PurchaseContent2"]),
                "(1) В промо2 Содержимое tegа purchase not expected");

            ReturnToExport();

            //driver.FindElement(By.CssSelector(".uib-tab[index=\"" + uibtabIndex + "\"]")).Click();
            //Driver.GetGridCell(2, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times")).Click();
            //Driver.SwalConfirm();
            //Driver.ClearToastMessages();
            //Driver.GetGridCell(1, "_serviceColumn", "YandexPromoGift").FindElement(By.CssSelector("a.fa-times")).Click();
            //Driver.SwalConfirm();
            //Driver.ClearToastMessages();

            //driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            //xmlResult = GetXmlFromYZip(exportName);
            //promos = Services.GetPromosFromXml(xmlResult);
            //VerifyIsTrue(Services.CountOfStrInXml(xmlResult, "<offer ") == Convert.ToInt32(csvData["gar5ProductCount"]), "(2)количество товаров в выгрузке not expected");
            //VerifyIsTrue(xmlResult.IndexOf("<gifts>") == -1, "(2)В выгрузке найдена подстрока gifts");
            //VerifyIsTrue(xmlResult.IndexOf("<gift ") == -1, "(2)В выгрузке найдена подстрока gift");
            //VerifyIsTrue(xmlResult.IndexOf("<promos>") != -1, "(2)В promo не найдена подстрока promos");
            //VerifyIsTrue(Services.CountOfStrInXml(promos, "<promo ") == Convert.ToInt32(csvData["gar5PromoCount2"]), "(2)количество tegs promo not expected");

            //ReturnToExport();
            //Thread.Sleep(1000);

            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
        }
    }
}