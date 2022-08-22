using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrdersExport
{
    [TestFixture]
    public class OrdersExportTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Orders);
            InitializeService.LoadData(
                "Data\\Admin\\Orders\\OrdersExport\\Catalog.Product.csv",
                "Data\\Admin\\Orders\\OrdersExport\\Catalog.Offer.csv",
                "Data\\Admin\\Orders\\OrdersExport\\Catalog.Category.csv",
                "Data\\Admin\\Orders\\OrdersExport\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Orders\\OrdersExport\\Customers.CustomerGroup.csv",
                "data\\Admin\\Orders\\OrdersExport\\Customers.Customer.csv",
                "data\\Admin\\Orders\\OrdersExport\\Customers.Contact.csv",
                "data\\Admin\\Orders\\OrdersExport\\Customers.Departments.csv",
                "data\\Admin\\Orders\\OrdersExport\\Customers.Managers.csv",
                "data\\Admin\\Orders\\OrdersExport\\[Order].OrderStatus.csv",
                "Data\\Admin\\Orders\\OrdersExport\\[Order].OrderSource.csv",
                "data\\Admin\\Orders\\OrdersExport\\[Order].[Order].csv",
                "data\\Admin\\Orders\\OrdersExport\\[Order].OrderCustomer.csv",
                "data\\Admin\\Orders\\OrdersExport\\[Order].OrderContact.csv",
                "data\\Admin\\Orders\\OrdersExport\\[Order].OrderCurrency.csv",
                "data\\Admin\\Orders\\OrdersExport\\[Order].OrderItems.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingscheckout#?checkoutTab=exportOrders");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void ExportAll()
        {
            if (Driver.FindElement(By.Id("UseStatus")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseStatus\"]")).Click();
            }

            VerifyIsFalse(Driver.FindElement(By.Id("ddlOrderStatuses")).Enabled, "use status field disabled");

            if (Driver.FindElement(By.Id("UseDate")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseDate\"]")).Click();
            }

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Enabled,
                "use date from field disabled");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Enabled,
                "use date to field disabled");

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("30"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("30"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                    "1;Новый;04.08.2016 00:00:00;LastName1 FirstName1;;;[1 - TestProduct1 - 1 руб. - 1шт.];0;0;0;0;0;1;\" руб.\";0;0;1;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment1;Нет;;\r\n" +
                    "2;Новый;05.08.2016 00:00:00;LastName2 FirstName2;;;[2 - TestProduct2 - 4 руб. - 2шт.];0;0;0;0;0;2;\" руб.\";0;0;2;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment2;Нет;;\r\n" +
                    "3;Новый;06.08.2016 00:00:00;LastName3 FirstName3;;;[3 - TestProduct3 - 9 руб. - 3шт.];0;0;0;0;0;3;\" руб.\";0;0;3;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment3;Нет;;\r\n" +
                    "4;Новый;07.08.2016 00:00:00;LastName4 FirstName4;;;[4 - TestProduct4 - 16 руб. - 4шт.];0;0;0;0;0;4;\" руб.\";0;0;4;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment4;Нет;;\r\n" +
                    "5;Новый;08.08.2016 00:00:00;LastName5 FirstName5;;;[5 - TestProduct5 - 25 руб. - 5шт.];0;0;0;0;0;5;\" руб.\";0;0;5;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment5;Нет;;\r\n" +
                    "6;В обработке;09.08.2016 00:00:00;LastName1 FirstName1;;;[6 - TestProduct6 - 36 руб. - 6шт.];0;0;0;0;0;6;\" руб.\";0;0;6;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment6;Нет;;\r\n" +
                    "7;В обработке;10.08.2016 00:00:00;LastName2 FirstName2;;;[7 - TestProduct7 - 49 руб. - 7шт.];0;0;0;0;0;7;\" руб.\";0;0;7;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment7;Нет;;\r\n" +
                    "8;В обработке;11.08.2016 00:00:00;LastName3 FirstName3;;;[8 - TestProduct8 - 64 руб. - 8шт.];0;0;0;0;0;8;\" руб.\";0;0;8;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment8;Нет;;\r\n" +
                    "9;В обработке;12.08.2016 00:00:00;LastName4 FirstName4;;;[9 - TestProduct9 - 81 руб. - 9шт.];0;0;0;0;0;9;\" руб.\";0;0;9;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment9;Нет;;\r\n" +
                    "10;В обработке;13.08.2016 00:00:00;LastName5 FirstName5;;;[10 - TestProduct10 - 100 руб. - 10шт.];0;0;0;0;0;10;\" руб.\";0;0;10;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment10;Нет;;\r\n" +
                    "11;Отправлен;14.08.2016 00:00:00;LastName1 FirstName1;;;[11 - TestProduct11 - 121 руб. - 11шт.];0;0;0;0;0;11;\" руб.\";0;0;11;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment11;Нет;;\r\n" +
                    "12;Отправлен;15.08.2016 00:00:00;LastName2 FirstName2;;;[12 - TestProduct12 - 144 руб. - 12шт.];0;0;0;0;0;12;\" руб.\";0;0;12;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment12;Нет;;\r\n" +
                    "13;Отправлен;16.08.2016 00:00:00;LastName3 FirstName3;;;[13 - TestProduct13 - 169 руб. - 13шт.];0;0;0;0;0;13;\" руб.\";0;0;13;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment13;Нет;;\r\n" +
                    "14;Отправлен;17.08.2016 00:00:00;LastName4 FirstName4;;;[14 - TestProduct14 - 196 руб. - 14шт.];0;0;0;0;0;14;\" руб.\";0;0;14;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment14;Нет;;\r\n" +
                    "15;Отправлен;18.08.2016 00:00:00;LastName5 FirstName5;;;[15 - TestProduct15 - 225 руб. - 15шт.];0;0;0;0;0;15;\" руб.\";0;0;15;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment15;Да;;\r\n" +
                    "16;Доставлен;19.08.2016 00:00:00;LastName1 FirstName1;;;[16 - TestProduct16 - 256 руб. - 16шт.];0;0;0;0;0;16;\" руб.\";0;0;16;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment16;Да;;\r\n" +
                    "17;Доставлен;20.08.2016 00:00:00;LastName2 FirstName2;;;[17 - TestProduct17 - 289 руб. - 17шт.];0;0;0;0;0;17;\" руб.\";0;0;17;Банковский перевод для юр. лиц;Курьером;Россия, Самарская область, Самара;;;comment17;Да;;\r\n" +
                    "18;Доставлен;21.08.2016 00:00:00;LastName3 FirstName3;;;[18 - TestProduct18 - 324 руб. - 18шт.];0;0;0;0;0;18;\" руб.\";0;0;18;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment18;Да;;\r\n" +
                    "19;Доставлен;22.08.2016 00:00:00;LastName4 FirstName4;;;[19 - TestProduct19 - 361 руб. - 19шт.];0;0;0;0;0;19;\" руб.\";0;0;19;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment19;Да;;\r\n" +
                    "20;Доставлен;23.08.2016 00:00:00;LastName5 FirstName5;;;[20 - TestProduct20 - 400 руб. - 20шт.];0;0;0;0;0;20;\" руб.\";0;0;20;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment20;Да;;\r\n" +
                    "21;Отменён;24.08.2016 00:00:00;LastName5 FirstName5;;;[21 - TestProduct21 - 441 руб. - 21шт.];0;0;0;0;0;21;\" руб.\";0;0;21;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment21;Да;;\r\n" +
                    "22;Отменён;25.08.2016 00:00:00;LastName5 FirstName5;;;[22 - TestProduct22 - 484 руб. - 22шт.];0;0;0;0;0;22;\" руб.\";0;0;22;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment22;Да;;\r\n" +
                    "23;Отменён;26.08.2016 00:00:00;LastName5 FirstName5;;;[23 - TestProduct23 - 529 руб. - 23шт.];0;0;0;0;0;23;\" руб.\";0;0;23;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment23;Да;;\r\n" +
                    "24;Отменён;27.08.2016 00:00:00;LastName5 FirstName5;;;[24 - TestProduct24 - 576 руб. - 24шт.];0;0;0;0;0;24;\" руб.\";0;0;24;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment24;Да;;\r\n" +
                    "25;Отменён;28.08.2016 00:00:00;LastName5 FirstName5;;;[25 - TestProduct25 - 625 руб. - 25шт.];0;0;0;0;0;25;\" руб.\";0;0;25;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment25;Да;FirstName1 LastName1;\r\n" +
                    "26;Отменен навсегда;29.08.2016 00:00:00;LastName5 FirstName5;;;[26 - TestProduct26 - 676 руб. - 26шт.];0;0;0;0;0;26;\" руб.\";0;0;26;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment26;Да;;\r\n" +
                    "27;Отменен навсегда;30.08.2016 00:00:00;LastName5 FirstName5;;;[27 - TestProduct27 - 729 руб. - 27шт.];0;0;0;0;0;27;\" руб.\";0;0;27;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment27;Да;;\r\n" +
                    "28;Отменен навсегда;31.08.2016 10:00:00;LastName5 FirstName5;;;[28 - TestProduct28 - 784 руб. - 28шт.];0;0;0;0;0;28;\" руб.\";0;0;28;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment28;Да;;\r\n" +
                    "29;Отменен навсегда;31.08.2016 15:00:00;LastName5 FirstName5;;;[29 - TestProduct29 - 841 руб. - 29шт.];0;0;0;0;0;29;\" руб.\";0;0;29;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment29;Да;;\r\n" +
                    "30;Отменен навсегда;31.08.2016 23:00:00;LastName5 FirstName5;;;[30 - TestProduct30 - 900 руб. - 30шт.];0;0;0;0;0;30;\" руб.\";0;0;30;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment30;Да;;"),
                "export file content 100");
            string s =
                "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                "1;Новый;04.08.2016 00:00:00;LastName1 FirstName1;;;[1 - TestProduct1 - 1 руб. - 1шт.];0;0;0;0;0;1;\" руб.\";0;0;1;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment1;Нет;;\r\n" +
                "2;Новый;05.08.2016 00:00:00;LastName2 FirstName2;;;[2 - TestProduct2 - 4 руб. - 2шт.];0;0;0;0;0;2;\" руб.\";0;0;2;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment2;Нет;;\r\n" +
                "3;Новый;06.08.2016 00:00:00;LastName3 FirstName3;;;[3 - TestProduct3 - 9 руб. - 3шт.];0;0;0;0;0;3;\" руб.\";0;0;3;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment3;Нет;;\r\n" +
                "4;Новый;07.08.2016 00:00:00;LastName4 FirstName4;;;[4 - TestProduct4 - 16 руб. - 4шт.];0;0;0;0;0;4;\" руб.\";0;0;4;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment4;Нет;;\r\n" +
                "5;Новый;08.08.2016 00:00:00;LastName5 FirstName5;;;[5 - TestProduct5 - 25 руб. - 5шт.];0;0;0;0;0;5;\" руб.\";0;0;5;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment5;Нет;;\r\n" +
                "6;В обработке;09.08.2016 00:00:00;LastName1 FirstName1;;;[6 - TestProduct6 - 36 руб. - 6шт.];0;0;0;0;0;6;\" руб.\";0;0;6;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment6;Нет;;\r\n" +
                "7;В обработке;10.08.2016 00:00:00;LastName2 FirstName2;;;[7 - TestProduct7 - 49 руб. - 7шт.];0;0;0;0;0;7;\" руб.\";0;0;7;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment7;Нет;;\r\n" +
                "8;В обработке;11.08.2016 00:00:00;LastName3 FirstName3;;;[8 - TestProduct8 - 64 руб. - 8шт.];0;0;0;0;0;8;\" руб.\";0;0;8;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment8;Нет;;\r\n" +
                "9;В обработке;12.08.2016 00:00:00;LastName4 FirstName4;;;[9 - TestProduct9 - 81 руб. - 9шт.];0;0;0;0;0;9;\" руб.\";0;0;9;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment9;Нет;;\r\n" +
                "10;В обработке;13.08.2016 00:00:00;LastName5 FirstName5;;;[10 - TestProduct10 - 100 руб. - 10шт.];0;0;0;0;0;10;\" руб.\";0;0;10;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment10;Нет;;\r\n" +
                "11;Отправлен;14.08.2016 00:00:00;LastName1 FirstName1;;;[11 - TestProduct11 - 121 руб. - 11шт.];0;0;0;0;0;11;\" руб.\";0;0;11;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment11;Нет;;\r\n" +
                "12;Отправлен;15.08.2016 00:00:00;LastName2 FirstName2;;;[12 - TestProduct12 - 144 руб. - 12шт.];0;0;0;0;0;12;\" руб.\";0;0;12;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment12;Нет;;\r\n" +
                "13;Отправлен;16.08.2016 00:00:00;LastName3 FirstName3;;;[13 - TestProduct13 - 169 руб. - 13шт.];0;0;0;0;0;13;\" руб.\";0;0;13;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment13;Нет;;\r\n" +
                "14;Отправлен;17.08.2016 00:00:00;LastName4 FirstName4;;;[14 - TestProduct14 - 196 руб. - 14шт.];0;0;0;0;0;14;\" руб.\";0;0;14;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment14;Нет;;\r\n" +
                "15;Отправлен;18.08.2016 00:00:00;LastName5 FirstName5;;;[15 - TestProduct15 - 225 руб. - 15шт.];0;0;0;0;0;15;\" руб.\";0;0;15;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment15;Да;;\r\n" +
                "16;Доставлен;19.08.2016 00:00:00;LastName1 FirstName1;;;[16 - TestProduct16 - 256 руб. - 16шт.];0;0;0;0;0;16;\" руб.\";0;0;16;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment16;Да;;\r\n" +
                "17;Доставлен;20.08.2016 00:00:00;LastName2 FirstName2;;;[17 - TestProduct17 - 289 руб. - 17шт.];0;0;0;0;0;17;\" руб.\";0;0;17;Банковский перевод для юр. лиц;Курьером;Россия, Самарская область, Самара;;;comment17;Да;;\r\n" +
                "18;Доставлен;21.08.2016 00:00:00;LastName3 FirstName3;;;[18 - TestProduct18 - 324 руб. - 18шт.];0;0;0;0;0;18;\" руб.\";0;0;18;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment18;Да;;\r\n" +
                "19;Доставлен;22.08.2016 00:00:00;LastName4 FirstName4;;;[19 - TestProduct19 - 361 руб. - 19шт.];0;0;0;0;0;19;\" руб.\";0;0;19;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment19;Да;;\r\n" +
                "20;Доставлен;23.08.2016 00:00:00;LastName5 FirstName5;;;[20 - TestProduct20 - 400 руб. - 20шт.];0;0;0;0;0;20;\" руб.\";0;0;20;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment20;Да;;\r\n" +
                "21;Отменён;24.08.2016 00:00:00;LastName5 FirstName5;;;[21 - TestProduct21 - 441 руб. - 21шт.];0;0;0;0;0;21;\" руб.\";0;0;21;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment21;Да;;\r\n" +
                "22;Отменён;25.08.2016 00:00:00;LastName5 FirstName5;;;[22 - TestProduct22 - 484 руб. - 22шт.];0;0;0;0;0;22;\" руб.\";0;0;22;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment22;Да;;\r\n" +
                "23;Отменён;26.08.2016 00:00:00;LastName5 FirstName5;;;[23 - TestProduct23 - 529 руб. - 23шт.];0;0;0;0;0;23;\" руб.\";0;0;23;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment23;Да;;\r\n" +
                "24;Отменён;27.08.2016 00:00:00;LastName5 FirstName5;;;[24 - TestProduct24 - 576 руб. - 24шт.];0;0;0;0;0;24;\" руб.\";0;0;24;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment24;Да;;\r\n" +
                "25;Отменён;28.08.2016 00:00:00;LastName5 FirstName5;;;[25 - TestProduct25 - 625 руб. - 25шт.];0;0;0;0;0;25;\" руб.\";0;0;25;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment25;Да;FirstName1 LastName1;\r\n" +
                "26;Отменен навсегда;29.08.2016 00:00:00;LastName5 FirstName5;;;[26 - TestProduct26 - 676 руб. - 26шт.];0;0;0;0;0;26;\" руб.\";0;0;26;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment26;Да;;\r\n" +
                "27;Отменен навсегда;30.08.2016 00:00:00;LastName5 FirstName5;;;[27 - TestProduct27 - 729 руб. - 27шт.];0;0;0;0;0;27;\" руб.\";0;0;27;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment27;Да;;\r\n" +
                "28;Отменен навсегда;31.08.2016 10:00:00;LastName5 FirstName5;;;[28 - TestProduct28 - 784 руб. - 28шт.];0;0;0;0;0;28;\" руб.\";0;0;28;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment28;Да;;\r\n" +
                "29;Отменен навсегда;31.08.2016 15:00:00;LastName5 FirstName5;;;[29 - TestProduct29 - 841 руб. - 29шт.];0;0;0;0;0;29;\" руб.\";0;0;29;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment29;Да;;\r\n" +
                "30;Отменен навсегда;31.08.2016 23:00:00;LastName5 FirstName5;;;[30 - TestProduct30 - 900 руб. - 30шт.];0;0;0;0;0;30;\" руб.\";0;0;30;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment30;Да;;";
        }

        [Test]
        public void ExportUseStatus()
        {
            if (!Driver.FindElement(By.Id("UseStatus")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseStatus\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.Id("ddlOrderStatuses")).Enabled, "use status field enabled");

            (new SelectElement(Driver.FindElement(By.Id("ddlOrderStatuses")))).SelectByText("Доставлен");

            if (Driver.FindElement(By.Id("UseDate")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseDate\"]")).Click();
            }

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Enabled,
                "use date from field disabled");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Enabled,
                "use date to field disabled");

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("5"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("5"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                    "16;Доставлен;19.08.2016 00:00:00;LastName1 FirstName1;;;[16 - TestProduct16 - 256 руб. - 16шт.];0;0;0;0;0;16;\" руб.\";0;0;16;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment16;Да;;\r\n" +
                    "17;Доставлен;20.08.2016 00:00:00;LastName2 FirstName2;;;[17 - TestProduct17 - 289 руб. - 17шт.];0;0;0;0;0;17;\" руб.\";0;0;17;Банковский перевод для юр. лиц;Курьером;Россия, Самарская область, Самара;;;comment17;Да;;\r\n" +
                    "18;Доставлен;21.08.2016 00:00:00;LastName3 FirstName3;;;[18 - TestProduct18 - 324 руб. - 18шт.];0;0;0;0;0;18;\" руб.\";0;0;18;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment18;Да;;\r\n" +
                    "19;Доставлен;22.08.2016 00:00:00;LastName4 FirstName4;;;[19 - TestProduct19 - 361 руб. - 19шт.];0;0;0;0;0;19;\" руб.\";0;0;19;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment19;Да;;\r\n" +
                    "20;Доставлен;23.08.2016 00:00:00;LastName5 FirstName5;;;[20 - TestProduct20 - 400 руб. - 20шт.];0;0;0;0;0;20;\" руб.\";0;0;20;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment20;Да;"),
                "export file content");
        }


        [Test]
        public void ExportUseDate()
        {
            if (Driver.FindElement(By.Id("UseStatus")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseStatus\"]")).Click();
            }

            VerifyIsFalse(Driver.FindElement(By.Id("ddlOrderStatuses")).Enabled, "use status field disabled");

            if (!Driver.FindElement(By.Id("UseDate")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseDate\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Enabled,
                "use date from field enabled");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Enabled,
                "use date to field enabled");

            /*check set date by calender*/
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Август", yearFrom: "2016",
                dataFrom: "Август 20, 2016", hourFrom: "00", minFrom: "00", monthTo: "Август", yearTo: "2016",
                dataTo: "Август 27, 2016", hourTo: "00", minTo: "30", dropFocusElem: "[data-e2e=\"OrderExportTitle\"]",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]", fieldTo: "[data-e2e=\"datetimeFilterTo\"]");
            VerifyAreEqual("20.08.2016 00:00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).GetAttribute("value"),
                "date filed from by calender");
            VerifyAreEqual("27.08.2016 00:30",
                Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).GetAttribute("value"),
                "date filed to by calender");

            /*check set date by print*/
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).SendKeys("20.08.2016 00:00");
            Driver.DropFocusCss("[data-e2e=\"OrderExportTitle\"]");

            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).SendKeys("27.08.2016 00:30");
            Driver.DropFocusCss("[data-e2e=\"OrderExportTitle\"]");

            VerifyAreEqual("20.08.2016 00:00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).GetAttribute("value"),
                "date filed from by print");
            VerifyAreEqual("27.08.2016 00:30",
                Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).GetAttribute("value"),
                "date filed to by print");

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("8"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("8"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                    "17;Доставлен;20.08.2016 00:00:00;LastName2 FirstName2;;;[17 - TestProduct17 - 289 руб. - 17шт.];0;0;0;0;0;17;\" руб.\";0;0;17;Банковский перевод для юр. лиц;Курьером;Россия, Самарская область, Самара;;;comment17;Да;;\r\n" +
                    "18;Доставлен;21.08.2016 00:00:00;LastName3 FirstName3;;;[18 - TestProduct18 - 324 руб. - 18шт.];0;0;0;0;0;18;\" руб.\";0;0;18;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment18;Да;;\r\n" +
                    "19;Доставлен;22.08.2016 00:00:00;LastName4 FirstName4;;;[19 - TestProduct19 - 361 руб. - 19шт.];0;0;0;0;0;19;\" руб.\";0;0;19;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment19;Да;;\r\n" +
                    "20;Доставлен;23.08.2016 00:00:00;LastName5 FirstName5;;;[20 - TestProduct20 - 400 руб. - 20шт.];0;0;0;0;0;20;\" руб.\";0;0;20;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment20;Да;;\r\n" +
                    "21;Отменён;24.08.2016 00:00:00;LastName5 FirstName5;;;[21 - TestProduct21 - 441 руб. - 21шт.];0;0;0;0;0;21;\" руб.\";0;0;21;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment21;Да;;\r\n" +
                    "22;Отменён;25.08.2016 00:00:00;LastName5 FirstName5;;;[22 - TestProduct22 - 484 руб. - 22шт.];0;0;0;0;0;22;\" руб.\";0;0;22;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment22;Да;;\r\n" +
                    "23;Отменён;26.08.2016 00:00:00;LastName5 FirstName5;;;[23 - TestProduct23 - 529 руб. - 23шт.];0;0;0;0;0;23;\" руб.\";0;0;23;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment23;Да;;\r\n" +
                    "24;Отменён;27.08.2016 00:00:00;LastName5 FirstName5;;;[24 - TestProduct24 - 576 руб. - 24шт.];0;0;0;0;0;24;\" руб.\";0;0;24;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment24;Да;"),
                "export file content");
        }

        [Test]
        public void ExportUseStatusAndDate()
        {
            if (!Driver.FindElement(By.Id("UseStatus")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseStatus\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.Id("ddlOrderStatuses")).Enabled, "use status field enabled");

            (new SelectElement(Driver.FindElement(By.Id("ddlOrderStatuses")))).SelectByText("Отменён");

            if (!Driver.FindElement(By.Id("UseDate")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseDate\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Enabled,
                "use date from field enabled");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Enabled,
                "use date to field enabled");

            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).SendKeys("20.08.2016 00:00");
            Driver.DropFocusCss("[data-e2e=\"OrderExportTitle\"]");

            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).SendKeys("27.08.2016 00:00");
            Driver.DropFocusCss("[data-e2e=\"OrderExportTitle\"]");

            VerifyAreEqual("20.08.2016 00:00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).GetAttribute("value"),
                "date filed from by print");
            VerifyAreEqual("27.08.2016 00:00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).GetAttribute("value"),
                "date filed to by print");

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("4"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("4"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                    "21;Отменён;24.08.2016 00:00:00;LastName5 FirstName5;;;[21 - TestProduct21 - 441 руб. - 21шт.];0;0;0;0;0;21;\" руб.\";0;0;21;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment21;Да;;\r\n" +
                    "22;Отменён;25.08.2016 00:00:00;LastName5 FirstName5;;;[22 - TestProduct22 - 484 руб. - 22шт.];0;0;0;0;0;22;\" руб.\";0;0;22;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment22;Да;;\r\n" +
                    "23;Отменён;26.08.2016 00:00:00;LastName5 FirstName5;;;[23 - TestProduct23 - 529 руб. - 23шт.];0;0;0;0;0;23;\" руб.\";0;0;23;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment23;Да;;\r\n" +
                    "24;Отменён;27.08.2016 00:00:00;LastName5 FirstName5;;;[24 - TestProduct24 - 576 руб. - 24шт.];0;0;0;0;0;24;\" руб.\";0;0;24;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment24;Да;"),
                "export file content");
        }

        [Test]
        public void ExportUseStatusAndDateNotPass()
        {
            if (!Driver.FindElement(By.Id("UseStatus")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseStatus\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.Id("ddlOrderStatuses")).Enabled, "use status field enabled");

            (new SelectElement(Driver.FindElement(By.Id("ddlOrderStatuses")))).SelectByText("Отменен навсегда");

            if (!Driver.FindElement(By.Id("UseDate")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseDate\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Enabled,
                "use date from field enabled");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Enabled,
                "use date to field enabled");

            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).SendKeys("20.08.2016 00:00");
            Driver.DropFocusCss("[data-e2e=\"OrderExportTitle\"]");

            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).SendKeys("27.08.2016 00:00");
            Driver.DropFocusCss("[data-e2e=\"OrderExportTitle\"]");

            VerifyAreEqual("20.08.2016 00:00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).GetAttribute("value"),
                "date filed from by print");
            VerifyAreEqual("27.08.2016 00:00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).GetAttribute("value"),
                "date filed to by print");

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("0"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("0"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(
                Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term"),
                "export file content");
        }

        [Test]
        public void ExportUseStatusAndDateDisabledAfterPrint()
        {
            if (!Driver.FindElement(By.Id("UseStatus")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseStatus\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.Id("ddlOrderStatuses")).Enabled, "use status field enabled");

            (new SelectElement(Driver.FindElement(By.Id("ddlOrderStatuses")))).SelectByText("Отменен навсегда");

            if (!Driver.FindElement(By.Id("UseDate")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseDate\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Enabled,
                "use date from field enabled");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Enabled,
                "use date to field enabled");

            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).SendKeys("20.08.2016 00:00");
            Driver.DropFocusCss("[data-e2e=\"OrderExportTitle\"]");

            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).SendKeys("27.08.2016 00:00");
            Driver.DropFocusCss("[data-e2e=\"OrderExportTitle\"]");

            VerifyAreEqual("20.08.2016 00:00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).GetAttribute("value"),
                "date filed from by print");
            VerifyAreEqual("27.08.2016 00:00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).GetAttribute("value"),
                "date filed to by print");

            IWebElement selectElem = Driver.FindElement(By.Id("ddlOrderStatuses"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Отменен навсегда"), "order status selected");

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseStatus\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseDate\"]")).Click();

            VerifyIsFalse(Driver.FindElement(By.Id("ddlOrderStatuses")).Enabled,
                "use status field disabled with value");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Enabled,
                "use date from field disabled with value");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Enabled,
                "use date to field disabled with value");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("30"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("30"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                    "1;Новый;04.08.2016 00:00:00;LastName1 FirstName1;;;[1 - TestProduct1 - 1 руб. - 1шт.];0;0;0;0;0;1;\" руб.\";0;0;1;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment1;Нет;;\r\n" +
                    "2;Новый;05.08.2016 00:00:00;LastName2 FirstName2;;;[2 - TestProduct2 - 4 руб. - 2шт.];0;0;0;0;0;2;\" руб.\";0;0;2;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment2;Нет;;\r\n" +
                    "3;Новый;06.08.2016 00:00:00;LastName3 FirstName3;;;[3 - TestProduct3 - 9 руб. - 3шт.];0;0;0;0;0;3;\" руб.\";0;0;3;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment3;Нет;;\r\n" +
                    "4;Новый;07.08.2016 00:00:00;LastName4 FirstName4;;;[4 - TestProduct4 - 16 руб. - 4шт.];0;0;0;0;0;4;\" руб.\";0;0;4;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment4;Нет;;\r\n" +
                    "5;Новый;08.08.2016 00:00:00;LastName5 FirstName5;;;[5 - TestProduct5 - 25 руб. - 5шт.];0;0;0;0;0;5;\" руб.\";0;0;5;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment5;Нет;;\r\n" +
                    "6;В обработке;09.08.2016 00:00:00;LastName1 FirstName1;;;[6 - TestProduct6 - 36 руб. - 6шт.];0;0;0;0;0;6;\" руб.\";0;0;6;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment6;Нет;;\r\n" +
                    "7;В обработке;10.08.2016 00:00:00;LastName2 FirstName2;;;[7 - TestProduct7 - 49 руб. - 7шт.];0;0;0;0;0;7;\" руб.\";0;0;7;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment7;Нет;;\r\n" +
                    "8;В обработке;11.08.2016 00:00:00;LastName3 FirstName3;;;[8 - TestProduct8 - 64 руб. - 8шт.];0;0;0;0;0;8;\" руб.\";0;0;8;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment8;Нет;;\r\n" +
                    "9;В обработке;12.08.2016 00:00:00;LastName4 FirstName4;;;[9 - TestProduct9 - 81 руб. - 9шт.];0;0;0;0;0;9;\" руб.\";0;0;9;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment9;Нет;;\r\n" +
                    "10;В обработке;13.08.2016 00:00:00;LastName5 FirstName5;;;[10 - TestProduct10 - 100 руб. - 10шт.];0;0;0;0;0;10;\" руб.\";0;0;10;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment10;Нет;;\r\n" +
                    "11;Отправлен;14.08.2016 00:00:00;LastName1 FirstName1;;;[11 - TestProduct11 - 121 руб. - 11шт.];0;0;0;0;0;11;\" руб.\";0;0;11;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment11;Нет;;\r\n" +
                    "12;Отправлен;15.08.2016 00:00:00;LastName2 FirstName2;;;[12 - TestProduct12 - 144 руб. - 12шт.];0;0;0;0;0;12;\" руб.\";0;0;12;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment12;Нет;;\r\n" +
                    "13;Отправлен;16.08.2016 00:00:00;LastName3 FirstName3;;;[13 - TestProduct13 - 169 руб. - 13шт.];0;0;0;0;0;13;\" руб.\";0;0;13;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment13;Нет;;\r\n" +
                    "14;Отправлен;17.08.2016 00:00:00;LastName4 FirstName4;;;[14 - TestProduct14 - 196 руб. - 14шт.];0;0;0;0;0;14;\" руб.\";0;0;14;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment14;Нет;;\r\n" +
                    "15;Отправлен;18.08.2016 00:00:00;LastName5 FirstName5;;;[15 - TestProduct15 - 225 руб. - 15шт.];0;0;0;0;0;15;\" руб.\";0;0;15;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment15;Да;;\r\n" +
                    "16;Доставлен;19.08.2016 00:00:00;LastName1 FirstName1;;;[16 - TestProduct16 - 256 руб. - 16шт.];0;0;0;0;0;16;\" руб.\";0;0;16;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment16;Да;;\r\n" +
                    "17;Доставлен;20.08.2016 00:00:00;LastName2 FirstName2;;;[17 - TestProduct17 - 289 руб. - 17шт.];0;0;0;0;0;17;\" руб.\";0;0;17;Банковский перевод для юр. лиц;Курьером;Россия, Самарская область, Самара;;;comment17;Да;;\r\n" +
                    "18;Доставлен;21.08.2016 00:00:00;LastName3 FirstName3;;;[18 - TestProduct18 - 324 руб. - 18шт.];0;0;0;0;0;18;\" руб.\";0;0;18;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment18;Да;;\r\n" +
                    "19;Доставлен;22.08.2016 00:00:00;LastName4 FirstName4;;;[19 - TestProduct19 - 361 руб. - 19шт.];0;0;0;0;0;19;\" руб.\";0;0;19;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment19;Да;;\r\n" +
                    "20;Доставлен;23.08.2016 00:00:00;LastName5 FirstName5;;;[20 - TestProduct20 - 400 руб. - 20шт.];0;0;0;0;0;20;\" руб.\";0;0;20;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment20;Да;;\r\n" +
                    "21;Отменён;24.08.2016 00:00:00;LastName5 FirstName5;;;[21 - TestProduct21 - 441 руб. - 21шт.];0;0;0;0;0;21;\" руб.\";0;0;21;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment21;Да;;\r\n" +
                    "22;Отменён;25.08.2016 00:00:00;LastName5 FirstName5;;;[22 - TestProduct22 - 484 руб. - 22шт.];0;0;0;0;0;22;\" руб.\";0;0;22;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment22;Да;;\r\n" +
                    "23;Отменён;26.08.2016 00:00:00;LastName5 FirstName5;;;[23 - TestProduct23 - 529 руб. - 23шт.];0;0;0;0;0;23;\" руб.\";0;0;23;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment23;Да;;\r\n" +
                    "24;Отменён;27.08.2016 00:00:00;LastName5 FirstName5;;;[24 - TestProduct24 - 576 руб. - 24шт.];0;0;0;0;0;24;\" руб.\";0;0;24;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment24;Да;;\r\n" +
                    "25;Отменён;28.08.2016 00:00:00;LastName5 FirstName5;;;[25 - TestProduct25 - 625 руб. - 25шт.];0;0;0;0;0;25;\" руб.\";0;0;25;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment25;Да;FirstName1 LastName1;\r\n" +
                    "26;Отменен навсегда;29.08.2016 00:00:00;LastName5 FirstName5;;;[26 - TestProduct26 - 676 руб. - 26шт.];0;0;0;0;0;26;\" руб.\";0;0;26;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment26;Да;;\r\n" +
                    "27;Отменен навсегда;30.08.2016 00:00:00;LastName5 FirstName5;;;[27 - TestProduct27 - 729 руб. - 27шт.];0;0;0;0;0;27;\" руб.\";0;0;27;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment27;Да;;\r\n" +
                    "28;Отменен навсегда;31.08.2016 10:00:00;LastName5 FirstName5;;;[28 - TestProduct28 - 784 руб. - 28шт.];0;0;0;0;0;28;\" руб.\";0;0;28;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment28;Да;;\r\n" +
                    "29;Отменен навсегда;31.08.2016 15:00:00;LastName5 FirstName5;;;[29 - TestProduct29 - 841 руб. - 29шт.];0;0;0;0;0;29;\" руб.\";0;0;29;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment29;Да;;\r\n" +
                    "30;Отменен навсегда;31.08.2016 23:00:00;LastName5 FirstName5;;;[30 - TestProduct30 - 900 руб. - 30шт.];0;0;0;0;0;30;\" руб.\";0;0;30;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment30;Да;;"),
                "export file content 100");
        }


        [Test]
        public void ExportUseStatusPaid()
        {
            if (!Driver.FindElement(By.Id("UsePaid")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUsePaid\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.Id("ddlPaid")).Enabled, "use paid field enabled");

            (new SelectElement(Driver.FindElement(By.Id("ddlPaid")))).SelectByText("Оплачен");

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("16"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("16"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                    "15;Отправлен;18.08.2016 00:00:00;LastName5 FirstName5;;;[15 - TestProduct15 - 225 руб. - 15шт.];0;0;0;0;0;15;\" руб.\";0;0;15;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment15;Да;;\r\n" +
                    "16;Доставлен;19.08.2016 00:00:00;LastName1 FirstName1;;;[16 - TestProduct16 - 256 руб. - 16шт.];0;0;0;0;0;16;\" руб.\";0;0;16;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment16;Да;;\r\n" +
                    "17;Доставлен;20.08.2016 00:00:00;LastName2 FirstName2;;;[17 - TestProduct17 - 289 руб. - 17шт.];0;0;0;0;0;17;\" руб.\";0;0;17;Банковский перевод для юр. лиц;Курьером;Россия, Самарская область, Самара;;;comment17;Да;;\r\n" +
                    "18;Доставлен;21.08.2016 00:00:00;LastName3 FirstName3;;;[18 - TestProduct18 - 324 руб. - 18шт.];0;0;0;0;0;18;\" руб.\";0;0;18;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment18;Да;;\r\n" +
                    "19;Доставлен;22.08.2016 00:00:00;LastName4 FirstName4;;;[19 - TestProduct19 - 361 руб. - 19шт.];0;0;0;0;0;19;\" руб.\";0;0;19;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment19;Да;;\r\n" +
                    "20;Доставлен;23.08.2016 00:00:00;LastName5 FirstName5;;;[20 - TestProduct20 - 400 руб. - 20шт.];0;0;0;0;0;20;\" руб.\";0;0;20;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment20;Да;;\r\n" +
                    "21;Отменён;24.08.2016 00:00:00;LastName5 FirstName5;;;[21 - TestProduct21 - 441 руб. - 21шт.];0;0;0;0;0;21;\" руб.\";0;0;21;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment21;Да;;\r\n" +
                    "22;Отменён;25.08.2016 00:00:00;LastName5 FirstName5;;;[22 - TestProduct22 - 484 руб. - 22шт.];0;0;0;0;0;22;\" руб.\";0;0;22;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment22;Да;;\r\n" +
                    "23;Отменён;26.08.2016 00:00:00;LastName5 FirstName5;;;[23 - TestProduct23 - 529 руб. - 23шт.];0;0;0;0;0;23;\" руб.\";0;0;23;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment23;Да;;\r\n" +
                    "24;Отменён;27.08.2016 00:00:00;LastName5 FirstName5;;;[24 - TestProduct24 - 576 руб. - 24шт.];0;0;0;0;0;24;\" руб.\";0;0;24;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment24;Да;;\r\n" +
                    "25;Отменён;28.08.2016 00:00:00;LastName5 FirstName5;;;[25 - TestProduct25 - 625 руб. - 25шт.];0;0;0;0;0;25;\" руб.\";0;0;25;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment25;Да;FirstName1 LastName1;\r\n" +
                    "26;Отменен навсегда;29.08.2016 00:00:00;LastName5 FirstName5;;;[26 - TestProduct26 - 676 руб. - 26шт.];0;0;0;0;0;26;\" руб.\";0;0;26;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment26;Да;;\r\n" +
                    "27;Отменен навсегда;30.08.2016 00:00:00;LastName5 FirstName5;;;[27 - TestProduct27 - 729 руб. - 27шт.];0;0;0;0;0;27;\" руб.\";0;0;27;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment27;Да;;\r\n" +
                    "28;Отменен навсегда;31.08.2016 10:00:00;LastName5 FirstName5;;;[28 - TestProduct28 - 784 руб. - 28шт.];0;0;0;0;0;28;\" руб.\";0;0;28;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment28;Да;;\r\n" +
                    "29;Отменен навсегда;31.08.2016 15:00:00;LastName5 FirstName5;;;[29 - TestProduct29 - 841 руб. - 29шт.];0;0;0;0;0;29;\" руб.\";0;0;29;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment29;Да;;\r\n" +
                    "30;Отменен навсегда;31.08.2016 23:00:00;LastName5 FirstName5;;;[30 - TestProduct30 - 900 руб. - 30шт.];0;0;0;0;0;30;\" руб.\";0;0;30;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment30;Да;;"),
                "export file content 100");
        }


        [Test]
        public void ExportUseStatusNotPaid()
        {
            if (!Driver.FindElement(By.Id("UsePaid")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUsePaid\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.Id("ddlPaid")).Enabled, "use paid field enabled");

            (new SelectElement(Driver.FindElement(By.Id("ddlPaid")))).SelectByText("Не оплачен");

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("14"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("14"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                    "1;Новый;04.08.2016 00:00:00;LastName1 FirstName1;;;[1 - TestProduct1 - 1 руб. - 1шт.];0;0;0;0;0;1;\" руб.\";0;0;1;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment1;Нет;;\r\n" +
                    "2;Новый;05.08.2016 00:00:00;LastName2 FirstName2;;;[2 - TestProduct2 - 4 руб. - 2шт.];0;0;0;0;0;2;\" руб.\";0;0;2;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment2;Нет;;\r\n" +
                    "3;Новый;06.08.2016 00:00:00;LastName3 FirstName3;;;[3 - TestProduct3 - 9 руб. - 3шт.];0;0;0;0;0;3;\" руб.\";0;0;3;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment3;Нет;;\r\n" +
                    "4;Новый;07.08.2016 00:00:00;LastName4 FirstName4;;;[4 - TestProduct4 - 16 руб. - 4шт.];0;0;0;0;0;4;\" руб.\";0;0;4;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment4;Нет;;\r\n" +
                    "5;Новый;08.08.2016 00:00:00;LastName5 FirstName5;;;[5 - TestProduct5 - 25 руб. - 5шт.];0;0;0;0;0;5;\" руб.\";0;0;5;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment5;Нет;;\r\n" +
                    "6;В обработке;09.08.2016 00:00:00;LastName1 FirstName1;;;[6 - TestProduct6 - 36 руб. - 6шт.];0;0;0;0;0;6;\" руб.\";0;0;6;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment6;Нет;;\r\n" +
                    "7;В обработке;10.08.2016 00:00:00;LastName2 FirstName2;;;[7 - TestProduct7 - 49 руб. - 7шт.];0;0;0;0;0;7;\" руб.\";0;0;7;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment7;Нет;;\r\n" +
                    "8;В обработке;11.08.2016 00:00:00;LastName3 FirstName3;;;[8 - TestProduct8 - 64 руб. - 8шт.];0;0;0;0;0;8;\" руб.\";0;0;8;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment8;Нет;;\r\n" +
                    "9;В обработке;12.08.2016 00:00:00;LastName4 FirstName4;;;[9 - TestProduct9 - 81 руб. - 9шт.];0;0;0;0;0;9;\" руб.\";0;0;9;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment9;Нет;;\r\n" +
                    "10;В обработке;13.08.2016 00:00:00;LastName5 FirstName5;;;[10 - TestProduct10 - 100 руб. - 10шт.];0;0;0;0;0;10;\" руб.\";0;0;10;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment10;Нет;;\r\n" +
                    "11;Отправлен;14.08.2016 00:00:00;LastName1 FirstName1;;;[11 - TestProduct11 - 121 руб. - 11шт.];0;0;0;0;0;11;\" руб.\";0;0;11;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment11;Нет;;\r\n" +
                    "12;Отправлен;15.08.2016 00:00:00;LastName2 FirstName2;;;[12 - TestProduct12 - 144 руб. - 12шт.];0;0;0;0;0;12;\" руб.\";0;0;12;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment12;Нет;;\r\n" +
                    "13;Отправлен;16.08.2016 00:00:00;LastName3 FirstName3;;;[13 - TestProduct13 - 169 руб. - 13шт.];0;0;0;0;0;13;\" руб.\";0;0;13;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment13;Нет;;\r\n" +
                    "14;Отправлен;17.08.2016 00:00:00;LastName4 FirstName4;;;[14 - TestProduct14 - 196 руб. - 14шт.];0;0;0;0;0;14;\" руб.\";0;0;14;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment14;Нет;"),
                "export file content");
        }

        [Test]
        public void ExportUseShipping()
        {
            if (!Driver.FindElement(By.Id("UseShipping")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseShipping\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.Id("ddlShippings")).Enabled, "use shipping field enabled");

            (new SelectElement(Driver.FindElement(By.Id("ddlShippings")))).SelectByText("Курьером");

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("6"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("6"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                    "16;Доставлен;19.08.2016 00:00:00;LastName1 FirstName1;;;[16 - TestProduct16 - 256 руб. - 16шт.];0;0;0;0;0;16;\" руб.\";0;0;16;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment16;Да;;\r\n" +
                    "17;Доставлен;20.08.2016 00:00:00;LastName2 FirstName2;;;[17 - TestProduct17 - 289 руб. - 17шт.];0;0;0;0;0;17;\" руб.\";0;0;17;Банковский перевод для юр. лиц;Курьером;Россия, Самарская область, Самара;;;comment17;Да;;\r\n" +
                    "18;Доставлен;21.08.2016 00:00:00;LastName3 FirstName3;;;[18 - TestProduct18 - 324 руб. - 18шт.];0;0;0;0;0;18;\" руб.\";0;0;18;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment18;Да;;\r\n" +
                    "19;Доставлен;22.08.2016 00:00:00;LastName4 FirstName4;;;[19 - TestProduct19 - 361 руб. - 19шт.];0;0;0;0;0;19;\" руб.\";0;0;19;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment19;Да;;\r\n" +
                    "20;Доставлен;23.08.2016 00:00:00;LastName5 FirstName5;;;[20 - TestProduct20 - 400 руб. - 20шт.];0;0;0;0;0;20;\" руб.\";0;0;20;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment20;Да;;\r\n" +
                    "21;Отменён;24.08.2016 00:00:00;LastName5 FirstName5;;;[21 - TestProduct21 - 441 руб. - 21шт.];0;0;0;0;0;21;\" руб.\";0;0;21;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment21;Да;"),
                "export file content");
        }

        [Test]
        public void ExportUseCity()
        {
            if (!Driver.FindElement(By.Id("UseCity")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseCity\"]")).Click();
            }

            Driver.FindElement(By.Name("City")).SendKeys("Самара");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".form-group .dropdown-menu a")).Click();

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("4"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("4"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                    "2;Новый;05.08.2016 00:00:00;LastName2 FirstName2;;;[2 - TestProduct2 - 4 руб. - 2шт.];0;0;0;0;0;2;\" руб.\";0;0;2;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment2;Нет;;\r\n" +
                    "7;В обработке;10.08.2016 00:00:00;LastName2 FirstName2;;;[7 - TestProduct7 - 49 руб. - 7шт.];0;0;0;0;0;7;\" руб.\";0;0;7;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment7;Нет;;\r\n" +
                    "12;Отправлен;15.08.2016 00:00:00;LastName2 FirstName2;;;[12 - TestProduct12 - 144 руб. - 12шт.];0;0;0;0;0;12;\" руб.\";0;0;12;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment12;Нет;;\r\n" +
                    "17;Доставлен;20.08.2016 00:00:00;LastName2 FirstName2;;;[17 - TestProduct17 - 289 руб. - 17шт.];0;0;0;0;0;17;\" руб.\";0;0;17;Банковский перевод для юр. лиц;Курьером;Россия, Самарская область, Самара;;;comment17;Да;"),
                "export file content");
        }

        [Test]
        public void ExportUseOrderSum()
        {
            if (!Driver.FindElement(By.Id("UseSum")).Selected)
            {
                VerifyIsFalse(Driver.FindElement(By.Id("OrderSumFrom")).Enabled, "use sum from field disabled");
                VerifyIsFalse(Driver.FindElement(By.Id("OrderSumTo")).Enabled, "use sum to field disabled");

                Driver.FindElement(By.CssSelector("[data-e2e=\"exportOrdersUseSum\"]")).Click();
            }

            VerifyIsTrue(Driver.FindElement(By.Id("OrderSumFrom")).Enabled, "use sum from field enabled");
            VerifyIsTrue(Driver.FindElement(By.Id("OrderSumTo")).Enabled, "use sum to field enabled");

            Driver.FindElement(By.Id("OrderSumFrom")).Click();
            Driver.FindElement(By.Id("OrderSumFrom")).Clear();
            Driver.FindElement(By.Id("OrderSumFrom")).SendKeys("10");

            Driver.FindElement(By.Id("OrderSumTo")).Click();
            Driver.FindElement(By.Id("OrderSumTo")).Clear();
            Driver.FindElement(By.Id("OrderSumTo")).SendKeys("20");

            (new SelectElement(Driver.FindElement(By.Id("ddlEncodings")))).SelectByText("UTF-8");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".progress-bar"));
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountValue\"]")).Text.Contains("11"),
                "export count value");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"exportCountTotal\"]")).Text.Contains("11"),
                "export count total");

            string linkExport = Driver.FindElement(By.LinkText("Скачать файл")).GetAttribute("href");

            Driver.Navigate().GoToUrl(linkExport + "&openinbrowser=true");
            Driver.WaitForElem(By.TagName("pre"));

            VerifyIsTrue(Driver.Url.Contains("csv"), "export file format");
            VerifyIsTrue(Driver.PageSource.Contains(
                    "Номер заказа;Статус;Дата заказа;ФИО;Email;Телефон;Товары;Оплачено бонусами;Скидка;Стоимость доставки;Наценка оплаты;Купон;Итоговая стоимость;Валюта;Налог;Стоимость товаров;Выгода;Метод оплаты;Метод доставки;Адрес доставки;Комментарий пользователя;Комментарий администратора;Комментарий к статусу;Оплачен;Менеджер;Код купона;Google client id;Yandex client id;Referral;Страница входа;UTM Source;UTM Medium;UTM Campaign;UTM Content;UTM Term\r\n" +
                    "10;В обработке;13.08.2016 00:00:00;LastName5 FirstName5;;;[10 - TestProduct10 - 100 руб. - 10шт.];0;0;0;0;0;10;\" руб.\";0;0;10;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment10;Нет;;\r\n" +
                    "11;Отправлен;14.08.2016 00:00:00;LastName1 FirstName1;;;[11 - TestProduct11 - 121 руб. - 11шт.];0;0;0;0;0;11;\" руб.\";0;0;11;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment11;Нет;;\r\n" +
                    "12;Отправлен;15.08.2016 00:00:00;LastName2 FirstName2;;;[12 - TestProduct12 - 144 руб. - 12шт.];0;0;0;0;0;12;\" руб.\";0;0;12;Банковский перевод для юр. лиц;Самовывоз;Россия, Самарская область, Самара;;;comment12;Нет;;\r\n" +
                    "13;Отправлен;16.08.2016 00:00:00;LastName3 FirstName3;;;[13 - TestProduct13 - 169 руб. - 13шт.];0;0;0;0;0;13;\" руб.\";0;0;13;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment13;Нет;;\r\n" +
                    "14;Отправлен;17.08.2016 00:00:00;LastName4 FirstName4;;;[14 - TestProduct14 - 196 руб. - 14шт.];0;0;0;0;0;14;\" руб.\";0;0;14;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment14;Нет;;\r\n" +
                    "15;Отправлен;18.08.2016 00:00:00;LastName5 FirstName5;;;[15 - TestProduct15 - 225 руб. - 15шт.];0;0;0;0;0;15;\" руб.\";0;0;15;Банковский перевод для юр. лиц;Самовывоз;Россия, Московская область, Москва;;;comment15;Да;;\r\n" +
                    "16;Доставлен;19.08.2016 00:00:00;LastName1 FirstName1;;;[16 - TestProduct16 - 256 руб. - 16шт.];0;0;0;0;0;16;\" руб.\";0;0;16;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment16;Да;;\r\n" +
                    "17;Доставлен;20.08.2016 00:00:00;LastName2 FirstName2;;;[17 - TestProduct17 - 289 руб. - 17шт.];0;0;0;0;0;17;\" руб.\";0;0;17;Банковский перевод для юр. лиц;Курьером;Россия, Самарская область, Самара;;;comment17;Да;;\r\n" +
                    "18;Доставлен;21.08.2016 00:00:00;LastName3 FirstName3;;;[18 - TestProduct18 - 324 руб. - 18шт.];0;0;0;0;0;18;\" руб.\";0;0;18;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment18;Да;;\r\n" +
                    "19;Доставлен;22.08.2016 00:00:00;LastName4 FirstName4;;;[19 - TestProduct19 - 361 руб. - 19шт.];0;0;0;0;0;19;\" руб.\";0;0;19;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment19;Да;;\r\n" +
                    "20;Доставлен;23.08.2016 00:00:00;LastName5 FirstName5;;;[20 - TestProduct20 - 400 руб. - 20шт.];0;0;0;0;0;20;\" руб.\";0;0;20;Банковский перевод для юр. лиц;Курьером;Россия, Московская область, Москва;;;comment20;Да;"),
                "export file content");
        }
    }
}