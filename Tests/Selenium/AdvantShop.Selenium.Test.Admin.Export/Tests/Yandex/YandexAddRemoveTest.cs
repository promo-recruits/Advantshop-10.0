using AdvantShop.Helpers;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Yandex
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class YandexAddRemoveTest : ExportServices
    {
        string testType = "Yandex";
        string adminPath = "exportfeeds/indexyandex";
        string exportPath = "export/";
        string text252;

        Dictionary<string, string> csvData;

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.ExportFeed | ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeed.csv",
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeedSelectedCategories.csv",
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeedExcludedProducts.csv",
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeedSettings.csv"
            );

            csvData = Functions.LoadCsvFile("Data\\Admin\\Export\\TestSettings\\YandexAddRemoveData.csv");
            InitializeService.YandexChannelActive();
            text252 = csvData["text252"];
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
        public void AddYandexExport()
        {
            /*добавление новой выгрузки*/
            GoToAdmin(adminPath);

            //Driver.GetButton(eButtonType.Add).Click();
            var AddExportButtons = Driver.FindElements(By.CssSelector("[data-e2e=\"AddExportFeed\"]"));
            //Проверка работы первой кнопки "Добавить"
            AddExportButtons[0].Click();
            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed,
                "Upload add button not found");
            //С пустым именем не сохраняется
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed,
                "Unload name field is not required ");

            Driver.FindElement(By.CssSelector(".close")).Click();
            Thread.Sleep(1000);
            //Проверка работы второй кнопки "Добавить"
            AddExportButtons[1].Click();
            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed,
                "Big upload add button not found");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]")).Clear();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed,
                "Unload name field is not required ");

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddName\"]"), testType + csvData["add1"]);

            //Длина описания не более 250 символов
            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddDesc\"]")).Clear();

            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Thread.Sleep(1000);
            GoToAdmin(adminPath);
            //Выгрузка добавлена
            VerifyIsTrue(Driver.FindElement(By.LinkText(testType + csvData["add1"])).Displayed,
                "Upload not added");
        }

        [Test]
        public void RequiredSettingsYandexExport()
        {
            /*проверяет, что выгрузка с параметрами по умолчанию работает корректно*/
            /*товаров никаких нет почему-то...*/
            GoToAdmin(adminPath);
            AddExportFeed(testType + csvData["required1"]);

            //Выгрузка всех товаров по умолчанию
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            VerifyAreEqual("true",
                Driver.FindElement(By.CssSelector("[name=\"exportCatalogType\"][value=\"AllProducts\"]"))
                    .GetAttribute("checked"),
                "default export not for all cagetories");

            //Заполненность обязательных полей по умолчанию
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();

            VerifyIsFalse(string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("Name")).GetAttribute("value")),
                "default export name");
            VerifyIsFalse(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("ExportFeedSettings.Interval"))
                    .GetAttribute("value")),
                "default export interval");
            VerifyIsFalse(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("ExportFeedSettings.FileName"))
                    .GetAttribute("value")),
                "default export filename");
            Driver.FindElement(By.Name("ExportFeedSettings.FileName")).Clear();

            string localExportName = GetExportName(TestName);
            string localExportPath = exportPath + localExportName;

            Driver.SendKeysInput(By.Name("ExportFeedSettings.FileName"), localExportPath);
            SelectOption("ddlFileExtention", csvData["defaultExportType"]);

            VerifyIsFalse(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("ExportFeedSettings.PriceMarginInNumbers"))
                    .GetAttribute("value")),
                "default export pricemargin");
            VerifyIsFalse(string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("ShopName")).GetAttribute("value")),
                "default export columseparator");
            VerifyIsFalse(string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("CompanyName")).GetAttribute("value")),
                "default export propertyseparator");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            localExportName += "." + csvData["defaultExportType"];
            localExportPath += "." + csvData["defaultExportType"];

            VerifyIsFalse(IsElementExists("Скачать zip архив", "LinkText"),
                "not expected archive link availability");
            //Открытие файла выгрузки
            VerifyIsTrue(IsElementExists(localExportPath, "LinkText"),
                "opening export file");

            var href = Driver.FindElement(By.LinkText(localExportPath)).GetAttribute("href");

            GoToClient(localExportPath);
            //тут лучше сравнить не пути, а содержимое файлов
            VerifyAreEqual(href.Substring(0, href.IndexOf('?')).ToLower(),
                Driver.Url.ToLower());

            GoToAdmin(adminPath);
            Driver.FindElement(By.LinkText(testType + csvData["required1"])).Click();
            Thread.Sleep(1000);
            Refresh();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.FindElement(By.ClassName("adv-checkbox-emul")).Click();
            Driver.FindElement(By.CssSelector("[name=\"NeedZip\"]~.adv-checkbox-emul")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();

            //Скачивание файла выгрузкиp
            //можно скачивать без архива, более красивенько (см. Google), но пока пусть будет так. А то я если потеряю эту ерунду, в осадок выпаду.
            VerifyIsTrue(IsElementExists(".btn-success[download]", "CssSelector"),
                "downloading export file");
            VerifyIsTrue(IsElementExists("Скачать zip архив", "LinkText"),
                "downloading export zip-archive");

            Driver.FindElement(By.CssSelector("[ng-if=\"exportFeeds.IsZip\"] a")).Click();
            Thread.Sleep(500);

            string exportFullFilePath = GetDownloadPath() + localExportName;
            VerifyIsTrue(File.Exists(exportFullFilePath + ".zip"),
                "archive was not downloaded");
            FileHelpers.UnZipFile(exportFullFilePath + ".zip");
            VerifyIsTrue(File.Exists(exportFullFilePath),
                "upload file not packed into an archive");
            File.Delete(exportFullFilePath + ".zip");

            File.Delete(exportFullFilePath);
        }

        [Test]
        public void DefaultSettingsYandexExport()
        {
            /*проверяет, что все поля и чекбоксы заполнены по умолчанию в соответствии с нижеприведенным шаблоном*/
            GoToAdmin(adminPath);


            Driver.FindElement(By.CssSelector("[data-e2e=\"AddExportFeed\"]")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]"))
                    .GetAttribute("value")),
                "default name not empty");
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddName\"]"), testType + csvData["default1"]);
            VerifyIsTrue(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddDesc\"]"))
                    .GetAttribute("value")),
                "default description not empty");
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Thread.Sleep(1000);

            /*common settings*/
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[name=\"exportCatalogType\"][value=\"AllProducts\"]"))
                    .GetAttribute("class").IndexOf("ng-not-modified") != -1,
                "exportCatalogType = allProduct");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[name=\"exportCatalogType\"][value=\"Categories\"]"))
                    .GetAttribute("class").IndexOf("ng-not-modified") == -1,
                "exportCatalogType = Categories");
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            VerifyIsTrue(CompareTextValue("Name", testType + csvData["default1"]),
                "defaultName");
            VerifyIsFalse(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("ExportFeedSettings.FileName"))
                    .GetAttribute("value")),
                "exportFileName");
            VerifyIsFalse(CheckboxChecked("ExportFeedSettings.Active"),
                "exportInTime");
            //VerifyAreEqual(driver.FindElement(By.Name("ExportFeedSettings.Interval")).GetAttribute("disabled"), "disabled", "ExportFeedInterval");
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.Interval")).GetAttribute("disabled"), "true",
                "ExportFeedInterval");
            VerifyAreEqual(Driver.FindElement(By.Name("ddlIntervalType")).GetAttribute("disabled"), "true",
                "ddlIntervalType");
            VerifyAreEqual(Driver.FindElement(By.Name("ddlIntervalType")).GetAttribute("disabled"), "true",
                "ExportFeedInterval");
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.JobStartHour")).GetAttribute("disabled"),
                "true",
                "ExportFeedInterval");
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.JobStartMinute")).GetAttribute("disabled"),
                "true",
                "ExportFeedInterval");

            Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.Interval", "1"),
                "ExportFeedInterval");
            VerifyIsTrue(SelectOptionSelected("ddlIntervalType", "Days"),
                "ddlIntervalType");
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.JobStartHour", "1"),
                "exportFileName");
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.JobStartMinute", "0"),
                "exportFileName");
            VerifyIsTrue(SelectOptionSelected("ddlFileExtention", csvData["defaultExportType"]),
                "exportFileType");
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.PriceMarginInNumbers", "0"),
                "priceMargin");
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.AdditionalUrlTags"),
                "additionalUrlTags");

            /*yandex settings*/
            VerifyIsFalse(CompareTextValue("ShopName"),
                "ShopName");
            VerifyIsFalse(CompareTextValue("CompanyName"),
                "CompanyName");
            VerifyIsTrue(CompareTextValue("SalesNotes"),
                "SalesNotes");

            VerifyIsFalse(CheckboxChecked("Store"),
                "Store");
            VerifyIsFalse(CheckboxChecked("Pickup"),
                "Pickup");
            VerifyIsTrue(CheckboxChecked("Delivery"),
                "Delivery");
            VerifyIsTrue(SelectOptionSelected("ddlDeliveryCost", "None"),
                "dllDeliveryCost");
            VerifyAreEqual(Driver.FindElement(By.Name("LocalDeliveryOption.Days")).GetAttribute("disabled"), "true",
                "ExportFeedInterval");
            VerifyAreEqual(Driver.FindElement(By.Name("LocalDeliveryOption.OrderBefore")).GetAttribute("disabled"),
                "true",
                "ExportFeedInterval");
            SelectOption("ddlDeliveryCost",
                "LocalDeliveryCost");
            VerifyIsTrue(CompareTextValue("LocalDeliveryOption.Days"),
                "LocalDeliveryOption.Days");
            VerifyIsTrue(CompareTextValue("LocalDeliveryOption.OrderBefore"),
                "LocalDeliveryOption.OrderBefore");

            VerifyIsTrue(SelectOptionSelected("ddlCurrency", csvData["defaultCurrency"]),
                "defaultCUrrency");
            VerifyIsFalse(CheckboxChecked("ExportProductDiscount"),
                "exportProductDiscount");
            VerifyIsFalse(CheckboxChecked("ExportPurchasePrice"),
                "ExportPurchasePrice");

            VerifyIsFalse(CheckboxChecked("ExportNotAvailable"),
                "ExportNotAvailable");
            VerifyIsTrue(CheckboxChecked("TypeExportYandex"),
                "TypeExportYandex - упрощенный тип");
            VerifyIsTrue(SelectOptionSelected("ddlOfferIdType", "id"),
                "dllOfferIdType");
            VerifyIsTrue(CheckboxChecked("ColorSizeToName"),
                "ColorSizeToName");
            VerifyIsTrue(SelectOptionSelected("ddlProductDescriptionType", "short"),
                "ddlProducctDescriptionType");
            VerifyIsTrue(CheckboxChecked("RemoveHtml"),
                "RemoveHtml");
            //VerifyIsTrue(CheckboxChecked("ExportProductProperties"), 
            //  "ExportProductProperties");
            //704 и выше
            VerifyIsFalse(CheckboxChecked("ExportProductProperties"),
                "ExportProductProperties");
            VerifyIsFalse(CheckboxChecked("ExportBarCode"),
                "ExportBarCode");
            VerifyIsTrue(CheckboxChecked("ExportAllPhotos"),
                "ExportAllPhotos");
            VerifyIsFalse(CheckboxChecked("ExportRelatedProducts"),
                "ExportRelatedProducts");
            VerifyIsFalse(CheckboxChecked("OnlyMainOfferToExport"),
                "OnlyMainOfferToExport");
            VerifyIsTrue(CheckboxChecked("AllowPreOrderProducts"),
                "AllowPreOrderProducts");
            //версия 7.0.4 и выше
            VerifyIsFalse(CheckboxChecked("ExportDimensions"),
                "ExportDimensions");
            VerifyIsFalse(CheckboxChecked("NeedZip"),
                "NeedZip");
        }

        [Test]
        public void ValidationCommonYandexTest()
        {
            /*проверка всех полей на коррекность, обязательность и на ограничения вводимых данных*/
            GoToAdmin(adminPath);

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddExportFeed\"]")).Click();
            Driver.WaitForModal();

            string text250 = text252.Substring(0, 250);
            //Длина названия не более 250 символов
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddName\"]"), text252);
            var textareaValue = Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]"))
                .GetAttribute("value");
            VerifyAreEqual(text250, textareaValue,
                "exportfeed name is more 250 symbols");
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddName\"]"), csvData["validation1"]);

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddDesc\"]"), text252);
            textareaValue = Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddDesc\"]")).GetAttribute("value");
            VerifyAreEqual(text250, textareaValue,
                "exportfeed description is more 250 symbols");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddDesc\"]")).Clear();
            Driver.FindElement(By.ClassName("btn-save")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.FindElement(By.Name("Name")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("Name"), text252);
            VerifyAreEqual(Driver.FindElement(By.Name("Name")).GetAttribute("value"), text250,
                "name length");
            Driver.SendKeysInput(By.CssSelector("textarea[name=\"Description\"]"), text252);
            VerifyAreEqual(Driver.FindElement(By.CssSelector("textarea[name=\"Description\"]")).GetAttribute("value"),
                text250,
                "description length");
            Driver.FindElement(By.CssSelector("textarea[name=\"Description\"]")).Clear();

            Driver.FindElement(By.CssSelector("[data-e2e=\"exportCommonSettingsActive\"]")).Click();
            SelectOption("ddlIntervalType", "Hours");

            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.JobStartHour")).GetAttribute("disabled"),
                "true",
                "ExportFeedInterval active hours JobStartHour");
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.JobStartMinute")).GetAttribute("disabled"),
                "true",
                "ExportFeedInterval active hours JobStartMinute");

            //you must check and uncheck
            //intervalZ hours
            /*проверка поля "интервал" на пустоту поля, отрицательное, дробное и нечисловое значение.
            В случае нуля программа автоматически использует значение интервала 1.
            В ограничей в положительную сторону нет.*/
            Driver.FindElement(By.Name("ExportFeedSettings.Interval")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.Name("ExportFeedSettings.Interval"), csvData["validationVal12"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            //если нулевой интервал, то выгрузка будет каждый час
            //driver.FindElement(By.Name("ExportFeedSettings.Interval"), csvData["validationVal13"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.Interval"), csvData["validationVal3"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.Interval"), csvData["validationVal1"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.Interval"), csvData["validationVal14"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.Interval"), csvData["validationVal7"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Refresh();
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.Interval")).GetAttribute("value"),
                csvData["validationVal7"]);

            //intervalZ days
            SelectOption("ddlIntervalType", "Days");
            /*проверка поля "часы" на пустоту, нечисловое, дробное, отрицательное, положительное значение и положительное больше 23
            + перезагрузка страницы без сохранения*/
            Driver.FindElement(By.Name("ExportFeedSettings.JobStartHour")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartHour"), csvData["validationVal1"]);
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.JobStartHour"),
                "ExportFeedSettings.JobStartHour");
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartHour"), csvData["validationVal2"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartHour"), csvData["validationVal3"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Refresh();
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.JobStartHour")).GetAttribute("value"),
                csvData["validationVal4"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartHour"), csvData["validationVal5"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartHour"), csvData["validationVal14"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartHour"), csvData["validationVal6"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartHour"), csvData["validationVal7"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            //Driver.ClearToastMessages();
            Refresh();
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.JobStartHour")).GetAttribute("value"),
                csvData["validationVal7"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartHour"), csvData["validationVal8"]);
            Refresh();
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.JobStartHour")).GetAttribute("value"),
                csvData["validationVal7"]);
            /*проверка поля "минуты" на пустоту, нечисловое, дробное, отрицательное, положительное значение и положительное больше 59
             + перезагрузка страницы без сохранения*/
            Driver.FindElement(By.Name("ExportFeedSettings.JobStartMinute")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartMinute"), csvData["validationVal1"]);
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.JobStartMinute"),
                "ExportFeedSettings.JobStartMinute");
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartMinute"), csvData["validationVal2"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartMinute"), csvData["validationVal3"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Refresh();
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.JobStartMinute")).GetAttribute("value"),
                csvData["validationVal4"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartMinute"), csvData["validationVal5"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartMinute"), csvData["validationVal14"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartMinute"), csvData["validationVal9"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartMinute"), csvData["validationVal10"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            //Driver.ClearToastMessages();
            Refresh();
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.JobStartMinute")).GetAttribute("value"),
                csvData["validationVal10"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.JobStartMinute"), csvData["validationVal11"]);
            Refresh();
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.JobStartMinute")).GetAttribute("value"),
                csvData["validationVal10"]);

            /*имя файла - проверка на обязательность, длину не более 100 симв и способность удалять ненужные символы */
            Driver.FindElement(By.Name("ExportFeedSettings.FileName")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.Name("ExportFeedSettings.FileName"), text252);
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.FileName", text252.Substring(0, 100)),
                "ExportFeedSettings.FileName lengtn");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            //пока в некоторых случаях слетает.Сережа чинит.
            //driver.SendKeysInput(By.Name("ExportFeedSettings.FileName"), csvData["validationFileName"]);
            //driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            //VerifyIsTrue(CompareTextValue(driver.FindElement(By.Name("ExportFeedSettings.FileName")).GetAttribute("value"), csvData["validationFileNameEtalon"]), "");
            //добавить полную выгрузку с неадекватным именем и проверить её на работоспособность

            GoToAdmin(adminPath);
            Thread.Sleep(1000);
            Driver.FindElement(By.LinkText(text250)).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();


            /*выгрузка в разных форматах*/
            string localExportName = GetExportName(TestName);
            string localExportPath = exportPath + localExportName;
            Driver.SendKeysInput(By.Name("ExportFeedSettings.FileName"), localExportPath);
            SelectOption("ddlFileExtention", csvData["defaultExportType"]);
            Driver.FindElement(By.CssSelector("[name=\"NeedZip\"]~.adv-checkbox-emul")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            localExportName += "." + csvData["defaultExportType"];
            localExportPath += "." + csvData["defaultExportType"];

            Driver.FindElement(By.CssSelector("[ng-if=\"exportFeeds.IsZip\"] a")).Click();
            Thread.Sleep(500);
            string exportFullFilePath = GetDownloadPath() + localExportName;
            FileHelpers.UnZipFile(exportFullFilePath + ".zip");
            VerifyIsTrue(File.Exists(exportFullFilePath),
                "Upload file not downloaded");
            File.Delete(exportFullFilePath + ".zip");
            File.Delete(exportFullFilePath);

            GoToClient(localExportPath);
            VerifyIsTrue(IsElementExists("[id=\"webkit-xml-viewer-source-xml\"]", "CssSelector"),
                "is not export file");

            GoToAdmin(adminPath);
            Driver.FindElement(By.LinkText(text250)).Click();
            Refresh(); //убрать потом, если починят
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            localExportName = GetExportName(TestName);
            localExportPath = exportPath + localExportName;
            Driver.SendKeysInput(By.Name("ExportFeedSettings.FileName"), localExportPath);
            SelectOption("ddlFileExtention", csvData["exportType2"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            localExportName += "." + csvData["exportType2"];
            localExportPath += "." + csvData["exportType2"];

            Driver.FindElement(By.CssSelector("[ng-if=\"exportFeeds.IsZip\"] a")).Click();
            Thread.Sleep(500);
            exportFullFilePath = GetDownloadPath() + localExportName;
            FileHelpers.UnZipFile(exportFullFilePath + ".zip");
            VerifyIsTrue(File.Exists(exportFullFilePath),
                "Upload file not downloaded");
            File.Delete(exportFullFilePath + ".zip");
            File.Delete(exportFullFilePath);
            GoToClient(localExportPath);
            VerifyIsTrue(IsElementExists("[id=\"webkit-xml-viewer-source-xml\"]", "CssSelector"),
                "is not export file");

            GoToAdmin(adminPath);

            /*наценка - проверка на обязательность, */
            Driver.FindElement(By.LinkText(text250)).Click();
            Refresh(); //убрать потом, если починят
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.FindElement(By.Name("ExportFeedSettings.PriceMarginInNumbers")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal1"]);
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.PriceMarginInNumbers"),
                "ExportFeedSettings.PriceMarginInNumbers " + csvData["validationVal1"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal2"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal14"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal3"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Refresh();
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.PriceMarginInNumbers", csvData["validationVal3"]),
                "ExportFeedSettings.PriceMarginInNumbers " + csvData["validationVal3"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal5"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Refresh();
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.PriceMarginInNumbers", csvData["validationVal5"]),
                "ExportFeedSettings.PriceMarginInNumbers " + csvData["validationVal5"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal13"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Refresh();
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.PriceMarginInNumbers", csvData["validationVal13"]),
                "ExportFeedSettings.PriceMarginInNumbers " + csvData["validationVal13"]);


            //настройки магазина
            Driver.FindElement(By.Name("ShopName")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ShopName"), csvData["ShopName"]);
            Driver.FindElement(By.Name("CompanyName")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("CompanyName"), csvData["CompanyName"]);
            Driver.SendKeysInput(By.Name("SalesNotes"), text252);
            CompareTextValue("SalesNotes", text252.Substring(0, 50));

            //все следующие настройки не включают в себя ограничений текстовых полей, а также в большинстве своем чекбоксы
            //тут я проверяю валидацию всех полей на адекватный тип: числа, их границы и 
            //выход за границы, длины, белеберду в exportName и прочее. Прям каждое поле.
        }

        [Test]
        public void EditYandexExport()
        {
            GoToAdmin(adminPath);

            //редактирование выгрузки - 1
            AddExportFeed(testType + csvData["edit1"], testType + csvData["edit1d"]);

            GoToAdmin(adminPath);

            Driver.FindElement(By.LinkText(testType + csvData["edit1"])).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"exportFeeds.CommonSettings.Name\"]"),
                testType + csvData["edit2"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();

            GoToAdmin(adminPath);
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElement(By.LinkText(testType + csvData["edit2"])).Displayed,
                "edit by link click");

            //редактирование выгрузки - 2
            Driver.FindElements(By.CssSelector(".fa-pencil-alt.export-feed-block-settings")).Last().Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"exportFeeds.CommonSettings.Name\"]"),
                testType + csvData["edit3"]);

            GoToAdmin(adminPath);
            Thread.Sleep(1000);
            Driver.FindElements(By.CssSelector(".fa-pencil-alt.export-feed-block-settings")).Last().Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.SendKeysInput(By.CssSelector("[ng-model=\"exportFeeds.CommonSettings.Name\"]"),
                testType + csvData["edit3"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();

            GoToAdmin(adminPath);

            VerifyIsTrue(Driver.FindElement(By.LinkText(testType + csvData["edit3"])).Displayed,
                "edit by pencil button click");
        }

        [Test]
        public void RemoveYandexExport()
        {
            GoToAdmin(adminPath);

            //удаление выгрузки - 1
            AddExportFeed(testType + csvData["remove1"]);

            GoToAdmin(adminPath);

            Driver.FindElements(By.CssSelector(".fa-times.export-feed-block-settings")).Last().Click();
            Driver.SwalConfirm();
            Thread.Sleep(1000);

            VerifyIsFalse(IsElementExists(testType + csvData["remove1"], "LinkText"),
                "remove by times button click");

            //удаление выгрузки - 2
            GoToAdmin(adminPath);
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddExportFeed\"]")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddName\"]"), testType + csvData["remove2"]);
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Driver.WaitForElem(By.ClassName("link-danger"));
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalCancel();

            GoToAdmin(adminPath);

            Driver.FindElement(By.LinkText(testType + csvData["remove2"])).Click();
            Driver.WaitForElem(By.ClassName("link-danger"));
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();

            GoToAdmin(adminPath);

            VerifyIsFalse(IsElementExists(testType + csvData["remove2"],
                "LinkText"));
        }
    }
}