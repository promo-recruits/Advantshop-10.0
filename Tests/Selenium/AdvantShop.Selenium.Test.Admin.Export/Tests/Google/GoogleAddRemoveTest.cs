using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Export.Tests.Google
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class GoogleAddRemoveTest : ExportServices
    {
        string adminPath = "exportfeeds/indexgoogle";
        string exportPath = "export/";
        Dictionary<string, string> csvData;

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.ExportFeed | ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeed.csv",
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeedSelectedCategories.csv",
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeedSelectedProducts.csv",
                "Data\\Admin\\Export\\ExportFeed\\Settings.ExportFeedSettings.csv"
            );

            csvData = Functions.LoadCsvFile("Data\\Admin\\Export\\TestSettings\\GoogleAddRemoveData.csv");
            InitializeService.GoogleChannelActive();
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
        public void AddGoogleExport()
        {
            GoToAdmin(adminPath);

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

            //Проверка работы второй кнопки "Добавить"
            AddExportButtons[1].Click();
            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed,
                "Big upload add button not found");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]")).Clear();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed,
                "Unload name field is not required ");

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddName\"]"), csvData["gar1Name"]);

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddDesc\"]"), csvData["gar1Description"]);

            Driver.FindElement(By.CssSelector(".btn-save")).Click();

            GoToAdmin(adminPath);
            //Выгрузка добавлена
            VerifyIsTrue(Driver.FindElement(By.LinkText(csvData["gar1Name"])).Displayed, "Upload not added");
        }

        [Test]
        public void EditGoogleExport()
        {
            GoToAdmin(adminPath);

            //редактирование выгрузки - 1
            AddExportFeed(csvData["gar3Name1"], csvData["gar3Description1"]);

            GoToAdmin(adminPath);

            Driver.FindElement(By.LinkText(csvData["gar3Name1"])).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.SendKeysInput(By.Id("Name"), csvData["gar3Name2"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();

            GoToAdmin(adminPath);
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElement(By.LinkText(csvData["gar3Name2"])).Displayed,
                "edit by link click");

            //редактирование выгрузки - 2
            Driver.FindElements(By.CssSelector(".fa-pencil-alt.export-feed-block-settings")).Last().Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(By.Id("Description"), csvData["gar3Description2"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Refresh();
            VerifyAreEqual(Driver.FindElement(By.Id("Description")).GetAttribute("value"), csvData["gar3Description2"],
                "данные при редактировании 2 не сохранились");
            Thread.Sleep(1000);
        }

        [Test]
        public void RemoveGoogleExport()
        {
            //удаление выгрузки - 1
            GoToAdmin(adminPath);
            AddExportFeed("GoogleRemove1");
            GoToAdmin(adminPath);

            Driver.FindElements(By.CssSelector(".fa-times.export-feed-block-settings")).Last().Click();
            Driver.SwalConfirm();

            VerifyIsFalse(IsElementExists("GoogleRemove1", "LinkText"),
                "(1)remove by times button click");

            //удаление выгрузки - 2
            GoToAdmin(adminPath);
            AddExportFeed("GoogleRemove2");
            GoToAdmin(adminPath);

            Driver.FindElement(By.LinkText("GoogleRemove2")).Click();
            Driver.WaitForElem(By.ClassName("link-danger"));
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
            GoToAdmin(adminPath);

            VerifyIsFalse(IsElementExists("GoogleRemove2", "LinkText"),
                "(2)remove by link-danger click");
        }

        [Test]
        public void DefaultSettingsGoogleExport()
        {
            GoToAdmin(adminPath);
            AddExportFeed("GoogleDefaultSettings");

            //Выгрузка всех товаров по умолчанию
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"1\"]")).Click();
            VerifyAreEqual("true", Driver.FindElement(By.CssSelector("[name=\"exportCatalogType\"]" +
                                                                     "[value=\"AllProducts\"]"))
                    .GetAttribute("checked"),
                "default export not for all cagetories");

            //Заполненность обязательных полей по умолчанию
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();

            VerifyIsFalse(string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("Name")).GetAttribute("value")),
                "default export name");
            VerifyIsTrue(string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("Description")).GetAttribute("value")),
                "default Description");
            VerifyIsFalse(CheckboxChecked("ExportFeedSettings.Active"),
                "выгрузка по расписанию");
            VerifyIsTrue(Driver.FindElement(By.Name("ExportFeedSettings.Interval")).GetAttribute("disabled") == "true",
                "время интервала запуска ");
            VerifyIsTrue(Driver.FindElement(By.Name("ddlIntervalType")).GetAttribute("disabled") == "true",
                "тип автовыгрузки ");
            VerifyIsTrue(
                Driver.FindElement(By.Name("ExportFeedSettings.JobStartHour")).GetAttribute("disabled") == "true",
                "время запуска часы ");
            VerifyIsTrue(
                Driver.FindElement(By.Name("ExportFeedSettings.JobStartMinute")).GetAttribute("disabled") == "true",
                "время запуска минуты ");
            VerifyIsFalse(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("ExportFeedSettings.Interval"))
                    .GetAttribute("value")),
                "default export interval");
            VerifyIsTrue(SelectOptionSelected("ddlIntervalType", "Hours"),
                "default selected intervaltype");
            VerifyIsFalse(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("ExportFeedSettings.FileName"))
                    .GetAttribute("value")),
                "default export filename");
            VerifyIsTrue(SelectOptionSelected("ddlFileExtention", "xml"),
                "default selected intervaltype");
            VerifyAreEqual(Driver.FindElement(By.Name("ExportFeedSettings.PriceMarginInNumbers")).GetAttribute("value"), "0",
                "default price margin");
            VerifyIsTrue(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("ExportFeedSettings.AdditionalUrlTags"))
                    .GetAttribute("value")),
                "default additional url tags");

            VerifyIsFalse(CheckboxChecked("OnlyMainOfferToExport"),
                "only main offer to export");
            VerifyIsTrue(SelectOptionSelected("ddlCurrency", "RUB"),
                "currency");
            VerifyIsTrue(CheckboxChecked("RemoveHtml"),
                "remove html");
            VerifyIsFalse(string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("DatafeedTitle")).GetAttribute("value")),
                "shop title");
            VerifyIsFalse(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("DatafeedDescription")).GetAttribute("value")),
                "shop description");
            VerifyIsTrue(
                string.IsNullOrWhiteSpace(Driver.FindElement(By.Name("GoogleProductCategory")).GetAttribute("value")),
                "shop default category");
            VerifyIsFalse(CheckboxChecked("ExportNotAvailable"),
                "not available");
            VerifyIsFalse(CheckboxChecked("AllowPreOrderProducts"),
                "pre order");
            VerifyIsTrue(SelectOptionSelected("ddlProductDescriptionType", "short"),
                "product description type");
        }

        [Test]
        public void ValidateGoogleExport()
        {
            GoToAdmin(adminPath);
            string text252 = csvData["text252"];

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddExportFeed\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]")).Clear();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed,
                "Unload name field is not required ");
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddName\"]"), text252, false);
            VerifyAreEqual(text252.Substring(0, 250),
                Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddName\"]")).GetAttribute("value"),
                "exportfeed name is more 250 symbols");
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddName\"]"), "GoogleValidationName");

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"ExportAddDesc\"]"), text252);
            VerifyAreEqual(text252.Substring(0, 250),
                Driver.FindElement(By.CssSelector("[data-e2e=\"ExportAddDesc\"]")).GetAttribute("value"),
                "exportfeed description is more 250 symbols");
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Thread.Sleep(1000);
            Refresh();
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();


            //Длина названия не более 250 символов
            VerifyAreEqual(Driver.FindElement(By.Name("Name")).GetAttribute("value"), "GoogleValidationName",
                "имя выгрузки не  совпало с заданным при создании");
            Driver.FindElement(By.Name("Name")).Clear();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.ClearToastMessages();
            Driver.Navigate().Refresh();
            VerifyAreEqual(Driver.FindElement(By.Name("Name")).GetAttribute("value"), "GoogleValidationName",
                "с пустым именем выгрузки не сохранилось");
            Driver.FindElement(By.Name("Name")).Clear();
            Thread.Sleep(500);
            Driver.SendKeysInput(By.Name("Name"), text252, false);
            VerifyAreEqual(text252.Substring(0, 250), Driver.FindElement(By.Name("Name")).GetAttribute("value"),
                "имя не больше 250 символов");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.Navigate().Refresh();
            Thread.Sleep(1000);
            VerifyAreEqual(Driver.FindElement(By.Name("Name")).GetAttribute("value"), text252.Substring(0, 250),
                "(2)имя не больше 250 символов");
            Driver.SendKeysInput(By.Name("Name"), "GoogleValidationName");
            Driver.Navigate().Refresh();
            Thread.Sleep(1000);
            VerifyAreEqual(text252.Substring(0, 250), Driver.FindElement(By.Name("Name")).GetAttribute("value"),
                "без клика по кнопке сохраения данные ен сохранятся");
            Driver.SendKeysInput(By.Name("Name"), "GoogleValidationName");

            Driver.SendKeysInput(By.CssSelector("textarea[name=\"Description\"]"), text252);
            VerifyAreEqual(Driver.FindElement(By.CssSelector("textarea[name=\"Description\"]")).GetAttribute("value"),
                text252.Substring(0, 250),
                "description length");
            Driver.FindElement(By.CssSelector("textarea[name=\"Description\"]")).Clear();
            Thread.Sleep(1000);


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
            //driver.SendKeysInput(By.Name("ExportFeedSettings.Interval"), csvData["validationVal13"]);
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

            Driver.SendKeysInput(By.Name("ExportFeedSettings.FileName"), text252, false);
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.FileName", text252.Substring(0, 100)),
                "ExportFeedSettings.FileName lengtn");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            //пока в некоторых случаях слетает.Сережа чинит.
            //driver.SendKeysInput(By.Name("ExportFeedSettings.FileName"), csvData["validationFileName"]);
            //driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            //VerifyIsTrue(CompareTextValue(driver.FindElement(By.Name("ExportFeedSettings.FileName")).GetAttribute("value"), csvData["validationFileNameEtalon"]), "");
            //добавить полную выгрузку с неадекватным именем и проверить её на работоспособность

            GoToAdmin(adminPath);
            Driver.FindElement(By.LinkText("GoogleValidationName")).Click();
            Thread.Sleep(1000);

            /*выгрузка в разных форматах*/
            string exportName = SetCommonGExportSettings(TestName, exportPath);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            
            VerifyIsFalse(string.IsNullOrWhiteSpace(GetXmlFromGFile(Driver, exportPath, exportName)),
                "ошибка в выгрузке в формате xml");

            GoToAdmin(adminPath);

            /*наценка - проверка на обязательность, */
            Driver.FindElement(By.LinkText("GoogleValidationName")).Click();
            Refresh(); //убрать потом, если починят
            Driver.FindElement(By.CssSelector(".uib-tab[index=\"3\"]")).Click();
            Driver.FindElement(By.Name("ExportFeedSettings.PriceMarginInNumbers")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal1"]);
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.PriceMarginInNumbers"),
                csvData["validationVal1"]);
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal2"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal14"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal3"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Refresh();
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.PriceMarginInNumbers",
                csvData["validationVal3"]));
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal5"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Refresh();
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.PriceMarginInNumbers",
                csvData["validationVal5"]));
            Driver.SendKeysInput(By.Name("ExportFeedSettings.PriceMarginInNumbers"), csvData["validationVal13"]);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Refresh();
            VerifyIsTrue(CompareTextValue("ExportFeedSettings.PriceMarginInNumbers",
                csvData["validationVal13"]));


            //настройки магазина
            VerifyAreEqual(Driver.FindElement(By.Name("DatafeedTitle")).GetAttribute("value"),
                csvData["DatafeedTitle1"],
                "с пустым именем выгрузки не сохранилось");
            VerifyAreEqual(Driver.FindElement(By.Name("DatafeedDescription")).GetAttribute("value"),
                csvData["DatafeedDescription1"],
                "с пустым именем выгрузки не сохранилось");

            Driver.FindElement(By.Name("DatafeedTitle")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("DatafeedTitle"), csvData["DatafeedTitle2"]);
            Driver.FindElement(By.Name("DatafeedDescription")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ButtonSave\"]")).Click();
            Driver.ClearToastMessages();
            Driver.SendKeysInput(By.Name("DatafeedDescription"), csvData["DatafeedDescription2"]);
        }

        [Test]
        public void RequiredSettingsGoogleExport()
        {
            //выгрузка с дефолтными настройками отрабатывает
            GoToAdmin(adminPath);
            AddExportFeed("RequiredSetting");

            string exportName = SetCommonGExportSettings(TestName, exportPath);
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            VerifyIsFalse(string.IsNullOrWhiteSpace(GetXmlFromGFile(Driver, exportPath, exportName)),
                "ошибка в выгрузке с дефолтными настройками");
        }
    }
}