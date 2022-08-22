using System;
using System.Collections.Generic;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.StaticPage
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class StaticPageTest : MySitesFunctions
    {
        private string spPageUrl = "staticpages#?staticPageTab=pages";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Store\\StaticPage\\CMS.StaticPage.csv"
            );

            Init();

            GoToAdmin("settings/common");
            Driver.SendKeysInput(By.Name("StoreUrl"), BaseUrl);
            Thread.Sleep(500);
            Driver.FindElement(AdvBy.DataE2E("btnSave")).FindElement(By.ClassName("btn-success")).Click();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin(spPageUrl);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void AddPage()
        {
            Driver.GetByE2E("btnAdd").Click();
            Driver.WaitForElem(By.ClassName("category-content")); //ждем страницу добавления блока
            Driver.SendKeysInput(By.Id("PageName"), "My static page name");
            Driver.CheckBoxCheck("Enabled");
            Driver.FindElement(AdvBy.DataE2E("StaticPageParentLink")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(AdvBy.DataE2E("gridFilterSearch"), "Page17");
            Driver.GetGridCell(0, "_serviceColumn", "StaticPageModal").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(By.Id("SortOrder"), "170");
            Driver.CheckBoxCheck("IndexAtSiteMap");
            Driver.SetCkText("My static page content", "PageText");
            Driver.SendKeysInput(By.Id("URL"), "my-static-page-url");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            string newDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Thread.Sleep(1000);

            GoToAdmin(spPageUrl);
            Driver.GridFilterSendKeys("My static page name");
            VerifyAreEqual("My static page name", Driver.GetGridCell(0, "PageName", "Pages").Text, "added page key");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Pages").FindElement(By.TagName("input")).Selected,
                "added page enabled");
            VerifyAreEqual("170", Driver.GetGridCell(0, "SortOrder", "Pages").Text, "added page sort");
            VerifyAreEqual(newDateTime, Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text,
                "added page dateModified");

            Driver.GetGridCell(0, "PageName", "Pages").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.ClassName("category-content"));
            VerifyAreEqual("My static page name", Driver.GetValue(By.Id("PageName")), "added page key in editor");
            VerifyIsTrue(Driver.FindElement(By.Id("Enabled")).Selected, "added page enabled");
            VerifyAreEqual("Page17", Driver.FindElement(AdvBy.DataE2E("StaticPageParent")).Text,
                "added page parent in editor");
            VerifyAreEqual("170", Driver.GetValue(By.Id("SortOrder")), "added page sort in editor");
            VerifyIsTrue(Driver.FindElement(By.Id("IndexAtSiteMap")).Selected, "added page siteMap in editor");
            Driver.AssertCkText("My static page content", "PageText");
            VerifyAreEqual("my-static-page-url", Driver.GetValue(By.Id("URL")), "added page url in editor");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnViewStaticPage")).Click();
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual("My static page name", Driver.FindElement(By.TagName("h1")).Text.Trim(),
                "header at static page");
            VerifyAreEqual(BaseUrl + "pages/my-static-page-url", Driver.Url, "url at static page");
            VerifyIsTrue(Driver.FindElement(By.ClassName("breads")).Text.Contains("Page17"),
                "parent in breads at static page");
            VerifyAreEqual("My static page content", Driver.FindElement(By.ClassName("staticpage-content")).Text.Trim(),
                "contant at static page");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void DeletePage()
        {
            int countPages = Convert.ToInt32(Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text
                .Replace("Найдено записей: ", ""));
            Driver.GridFilterSendKeys("Page101");
            VerifyAreEqual("Page101", Driver.GetGridCellText(0, "PageName", "Pages"), "page name");
            VerifyAreEqual(2, Driver.GetGridCell(0, "_serviceColumn", "Pages").FindElements(By.TagName("a")).Count,
                "count of links in serviceColumn");
            VerifyAreEqual(1,
                Driver.GetGridCell(0, "_serviceColumn", "Pages").FindElements(By.CssSelector("a.fa-pencil-alt")).Count,
                "count of editLinks in serviceColumn");
            VerifyAreEqual(1,
                Driver.GetGridCell(0, "_serviceColumn", "Pages").FindElements(By.CssSelector("a.fa-times")).Count,
                "count of deleteLinks in serviceColumn");

            Driver.GetGridCell(0, "_serviceColumn", "Pages").FindElement(By.CssSelector("a.fa-times")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Page101", Driver.GetGridCell(0, "PageName", "Pages").Text, "page not removed");
            Driver.GetGridCell(0, "_serviceColumn", "Pages").FindElement(By.CssSelector("a.fa-times")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "page removed");


            Driver.GridFilterSendKeys("Page101");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "page removed2");
            Driver.GridFilterSendKeys("Page100");
            Driver.GetGridCell(0, "PageName", "Pages").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.ClassName("category-content"));
            VerifyAreEqual("Удалить", Driver.FindElement(By.ClassName("link-danger")).Text,
                "delete btn at staticpage edit");
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalCancel();
            Driver.FindElement(By.ClassName("link-danger")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual(BaseUrl + "adminv3/staticpages", Driver.Url, "redirect after");
            VerifyAreEqual("Найдено записей: " + (countPages - 2),
                Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text, "staticpages count after");
        }

        [Test]
        public void EditPage()
        {
            //check default
            Driver.GridFilterSendKeys("Page41");
            VerifyAreEqual("Page41", Driver.GetGridCellText(0, "PageName", "Pages"), "default name");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Pages").FindElement(By.TagName("input")).Selected,
                "default enabled");
            VerifyAreEqual("41", Driver.GetGridCellText(0, "SortOrder", "Pages"), "default sort order");
            VerifyAreEqual("06.05.2012 14:22", Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text,
                "default dateModified");
            Driver.GetGridCell(0, "PageName", "Pages").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.ClassName("category-content"));
            VerifyAreEqual("Page41", Driver.GetValue(By.Id("PageName")), "default key in editor");
            VerifyIsTrue(Driver.FindElement(By.Id("Enabled")).Selected, "default enabled in editor");
            VerifyAreEqual("Корень", Driver.FindElement(AdvBy.DataE2E("StaticPageParent")).Text,
                "default parent in editor");
            VerifyAreEqual("41", Driver.GetValue(By.Id("SortOrder")), "default sort in editor");
            VerifyIsTrue(Driver.FindElement(By.Id("IndexAtSiteMap")).Selected, "default siteMap in editor");
            Driver.AssertCkText("text8", "PageText");
            VerifyAreEqual("page41", Driver.GetValue(By.Id("URL")), "added page url in editor");
            Driver.ScrollToTop();
            //посмотреть в клиентке
            Driver.FindElement(AdvBy.DataE2E("btnViewStaticPage")).Click();
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual("Page41", Driver.FindElement(By.TagName("h1")).Text.Trim(), "header at static page");
            VerifyAreEqual(BaseUrl + "pages/page41", Driver.Url, "url at static page");
            VerifyIsTrue(Driver.FindElement(By.ClassName("breads")).Text.Contains("Главная"),
                "parent in breads at static page");
            VerifyAreEqual("text8", Driver.FindElement(By.ClassName("staticpage-content")).Text.Trim(),
                "contant at static page");
            Functions.CloseTab(Driver, BaseUrl);
            //+check sitemap
            GoToAdmin("settingssystem#?systemTab=sitemap");
            Driver.FindElement(By.LinkText("Обновить карты")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.PartialLinkText("/sitemap.html")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual(1, Driver.FindElements(By.LinkText("Page41")).Count, "default page at sitemap");
            Functions.CloseTab(Driver, BaseUrl);

            //edit without saving
            GoToAdmin("staticpages/edit/41");
            Driver.SendKeysInput(By.Id("PageName"), "Edited static page");
            Driver.CheckBoxUncheck("Enabled");
            Driver.FindElement(AdvBy.DataE2E("StaticPageParentLink")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(AdvBy.DataE2E("gridFilterSearch"), "Page27");
            Driver.GetGridCell(0, "_serviceColumn", "StaticPageModal").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(By.Id("SortOrder"), "-15");
            Driver.CheckBoxUncheck("IndexAtSiteMap");
            Driver.SetCkText("Edited static page content", "PageText");
            Driver.SendKeysInput(By.Id("URL"), "edited-page-url");
            Driver.ScrollToTop();
            Thread.Sleep(1000);
            Refresh();
            VerifyAreEqual("Page41", Driver.GetValue(By.Id("PageName")), "edited without saving key in editor");
            VerifyIsTrue(Driver.FindElement(By.Id("Enabled")).Selected, "edited without saving enabled in editor");
            VerifyAreEqual("Корень", Driver.FindElement(AdvBy.DataE2E("StaticPageParent")).Text,
                "edited without saving parent in editor");
            VerifyAreEqual("41", Driver.GetValue(By.Id("SortOrder")), "edited without saving sort in editor");
            VerifyIsTrue(Driver.FindElement(By.Id("IndexAtSiteMap")).Selected,
                "edited without saving siteMap in editor");
            Driver.AssertCkText("text8", "PageText");
            VerifyAreEqual("page41", Driver.GetValue(By.Id("URL")), "edited without saving page url in editor");
            Driver.ScrollToTop();

            Driver.SendKeysInput(By.Id("PageName"), "Edited static page");
            Driver.CheckBoxUncheck("Enabled");
            Driver.FindElement(AdvBy.DataE2E("StaticPageParentLink")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(AdvBy.DataE2E("gridFilterSearch"), "Page27");
            Driver.GetGridCell(0, "_serviceColumn", "StaticPageModal").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(By.Id("SortOrder"), "-15");
            Driver.CheckBoxUncheck("IndexAtSiteMap");
            Driver.SetCkText("Edited static page content", "PageText");
            Driver.SendKeysInput(By.Id("URL"), "edited-page-url");
            Thread.Sleep(1000);

            GoToAdmin(spPageUrl);
            Driver.GridFilterSendKeys("Edited static page");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "page renamed");
            Driver.GridFilterSendKeys("Page41");
            VerifyAreEqual("Page41", Driver.GetGridCellText(0, "PageName", "Pages"), "edited without saving name");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Pages").FindElement(By.TagName("input")).Selected,
                "edited without saving enabled");
            VerifyAreEqual("41", Driver.GetGridCellText(0, "SortOrder", "Pages"), "edited without saving sort order");
            VerifyAreEqual("06.05.2012 14:22", Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text,
                "edited without saving dateModified");
            Driver.GetGridCell(0, "PageName", "Pages").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.ClassName("category-content"));


            //edit and check new data (set disable, set enable)
            GoToAdmin("staticpages/edit/41");
            Driver.SendKeysInput(By.Id("PageName"), "Edited static page");
            Driver.CheckBoxUncheck("Enabled");
            Driver.FindElement(AdvBy.DataE2E("StaticPageParentLink")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(AdvBy.DataE2E("gridFilterSearch"), "Page27");
            Driver.GetGridCell(0, "_serviceColumn", "StaticPageModal").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(By.Id("SortOrder"), "-15");
            Driver.CheckBoxUncheck("IndexAtSiteMap");
            Driver.SetCkText("Edited static page content", "PageText");
            Driver.SendKeysInput(By.Id("URL"), "edited-page-url");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            string newDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Thread.Sleep(1000);

            GoToAdmin(spPageUrl);
            Driver.GridFilterSendKeys("Page41");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "page renamed");
            GoToAdmin(spPageUrl);
            VerifyAreEqual("Edited static page", Driver.GetGridCellText(0, "PageName", "Pages"), "edited name");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Pages").FindElement(By.TagName("input")).Selected,
                "edited enabled");
            VerifyAreEqual("-15", Driver.GetGridCellText(0, "SortOrder", "Pages"), "edited sort order");
            VerifyAreEqual(newDateTime, Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text,
                "edited dateModified");
            Driver.GetGridCell(0, "PageName", "Pages").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.ClassName("category-content"));

            VerifyAreEqual("Edited static page", Driver.GetValue(By.Id("PageName")), "edited key in editor");
            VerifyIsFalse(Driver.FindElement(By.Id("Enabled")).Selected, "edited enabled in editor");
            VerifyAreEqual("Page27", Driver.FindElement(AdvBy.DataE2E("StaticPageParent")).Text,
                "edited parent in editor");
            VerifyAreEqual("-15", Driver.GetValue(By.Id("SortOrder")), "edited sort in editor");
            VerifyIsFalse(Driver.FindElement(By.Id("IndexAtSiteMap")).Selected, "edited siteMap in editor");
            Driver.AssertCkText("Edited static page content", "PageText");
            VerifyAreEqual("edited-page-url", Driver.GetValue(By.Id("URL")), "edited page url in editor");
            Driver.ScrollToTop();
            //посмотреть в клиентке
            Driver.FindElement(AdvBy.DataE2E("btnViewStaticPage")).Click();
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Is404Page(Driver.Url), "edited disable page");
            Functions.CloseTab(Driver, BaseUrl);
            //sitemap
            GoToAdmin("settingssystem#?systemTab=sitemap");
            Driver.FindElement(By.LinkText("Обновить карты")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.PartialLinkText("/sitemap.html")).Click();
            Functions.OpenNewTab(Driver, BaseUrl); 
            VerifyAreEqual(0, Driver.FindElements(By.LinkText("Page41")).Count, "old name edited hidden page at sitemap");
            VerifyAreEqual(0, Driver.FindElements(By.LinkText("Edited static page")).Count, "edited hidden page at sitemap");
            Functions.CloseTab(Driver, BaseUrl);

            GoToAdmin("staticpages/edit/41");
            Driver.CheckBoxCheck("Enabled");
            Driver.CheckBoxCheck("IndexAtSiteMap");
            SaveStaticPageSettings();
            //посмотреть в клиентке
            Driver.FindElement(AdvBy.DataE2E("btnViewStaticPage")).Click();
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual("Edited static page", Driver.FindElement(By.TagName("h1")).Text.Trim(),
                "header at edited static page");
            VerifyAreEqual(BaseUrl + "pages/edited-page-url", Driver.Url, "url at edited static page");
            VerifyIsTrue(Driver.FindElement(By.ClassName("breads")).Text.Contains("Главная"),
                "parent in breads at edited static page");
            VerifyIsTrue(Driver.FindElement(By.ClassName("breads")).Text.Contains("Page27"),
                "parent_2 in breads at edited static page");
            VerifyAreEqual("Edited static page content",
                Driver.FindElement(By.ClassName("staticpage-content")).Text.Trim(), "contant at edited static page");
            Functions.CloseTab(Driver, BaseUrl);
            //sitemap
            GoToAdmin("settingssystem#?systemTab=sitemap");
            Driver.FindElement(By.LinkText("Обновить карты")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.PartialLinkText("/sitemap.html")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual(0, Driver.FindElements(By.LinkText("Page41")).Count, "old name edited enabled page at sitemap");
            VerifyAreEqual(1, Driver.FindElements(By.LinkText("Edited static page")).Count, "edited enabled page at sitemap");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void EditPageInplace()
        {
            //inplace изменить активность, проверить, что раз изменяется дата модификации.
            Driver.GridFilterSendKeys("page6");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Pages").FindElement(By.TagName("input")).Selected,
                "page 6 not enabled default");
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled", "Pages").FindElement(By.TagName("input")).Selected,
                "page 60 enabled default");
            string dateFormatted1 = Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text;
            string dateFormatted2 = Driver.GetGridCell(1, "ModifyDateFormatted", "Pages").Text;

            string dateFormattedNew = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Driver.GetGridCell(0, "Enabled", "Pages").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Driver.GetGridCell(1, "Enabled", "Pages").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Pages").FindElement(By.TagName("input")).Selected,
                "page 6 enable changed (1)");
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled", "Pages").FindElement(By.TagName("input")).Selected,
                "page 60 enable changed (1)");

            Refresh();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Pages").FindElement(By.TagName("input")).Selected,
                "page 6 enable changed (2)");
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled", "Pages").FindElement(By.TagName("input")).Selected,
                "page 60 enable changed (2)");
            VerifyAreNotEqual(dateFormatted1, Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text,
                "dateFormatted1 after changed (1)");
            VerifyAreEqual(dateFormattedNew, Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text,
                "dateFormatted1 after changed (2)");
            VerifyAreNotEqual(dateFormatted2, Driver.GetGridCell(1, "ModifyDateFormatted", "Pages").Text,
                "dateFormatted2 after changed (1)");
            VerifyAreEqual(dateFormattedNew, Driver.GetGridCell(1, "ModifyDateFormatted", "Pages").Text,
                "dateFormatted2 after changed (2)");


            //inplace нельзя оставить пустую сортировку; изменить значение сортировки, проверить дату модификации. 
            Driver.GridFilterSendKeys("page61");
            VerifyAreEqual("61", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 default sort");
            string dateFormatted3 = Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text;

            Driver.SendKeysGridCell("", 0, "SortOrder", "Pages");
            Driver.GetGridFilter().Click();
            VerifyAreEqual("61", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 sort not empty");
            Refresh();
            VerifyAreEqual(dateFormatted3, Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text,
                "dateFormatted3 after empty changed");

            Driver.SendKeysGridCell("999", 0, "SortOrder", "Pages");
            dateFormattedNew = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Driver.GetGridFilter().Click();
            VerifyAreEqual("999", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 sort edited");
            Refresh();
            VerifyAreEqual("999", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 sort edited(1)");
            VerifyAreNotEqual(dateFormatted3, Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text,
                "dateFormatted3 after changed");

            //+ невалидные значения сортировки

            Driver.SendKeysGridCell(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", 0,
                "SortOrder", "Pages");
            Driver.GetGridFilter().Click();
            VerifyAreEqual("", Driver.GetGridCellText(0, "SortOrder", "Pages"),
                "page 61 sort edited_long+e"); //отображается длинная строка, но value у input empty
            Refresh();
            VerifyAreEqual("999", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 sort edited_long+e(1)");

            Driver.SendKeysGridCell("12345678910", 0, "SortOrder", "Pages");
            Driver.GetGridFilter().Click();
            VerifyAreEqual("12345678910", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 sort edited(2)");
            Refresh();
            VerifyAreEqual("999", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 sort edited(3)");

            Driver.SendKeysGridCell("Lorem ipsum dolor sit amet", 0, "SortOrder", "Pages");
            Driver.GetGridFilter().Click();
            VerifyAreEqual("", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 sort edited(4)");
            Refresh();
            VerifyAreEqual("999", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 sort edited(5)");

            Driver.SendKeysGridCell(Functions.SymbolsString, 0, "SortOrder", "Pages");
            Driver.GetGridFilter().Click();
            VerifyAreEqual("", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 sort edited(6)");
            Refresh();
            VerifyAreEqual("999", Driver.GetGridCellText(0, "SortOrder", "Pages"), "page 61 sort edited(7)");
        }

        [Test]
        public void MetaPage()
        {
            //https://task.advant.me/adminv3/tasks#?modal=23419

            //проверить отображение дефолтных мета, 
            Driver.GridFilterSendKeys("Page63");
            Driver.GetGridCell(0, "PageName", "Pages").FindElement(By.TagName("a")).Click();
            Driver.CheckBoxUncheck("DefaultMeta");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            CheckToastErrors(new List<string> {"Заголовок", "H1"}, "empty spage meta");
            //задать свои мета текстом - некоторые
            string customMeta = "Custom meta for static page";
            Driver.SendKeysInput(By.Name("SeoTitle"), customMeta);
            Driver.SendKeysInput(By.Name("SeoH1"), customMeta);
            SaveStaticPageSettings();

            VerifyAreEqual(customMeta, GetInputValue("SeoTitle"), "custom title");
            VerifyAreEqual(customMeta, GetInputValue("SeoH1"), "custom h1");
            VerifyIsTrue(String.IsNullOrEmpty(GetInputValue("SeoDescription")), "empty description");
            VerifyIsTrue(String.IsNullOrEmpty(GetInputValue("SeoKeywords")), "empty description");

            //задать свои мета текстом - все
            Driver.SendKeysInput(By.Name("SeoDescription"), customMeta);
            Driver.SendKeysInput(By.Name("SeoKeywords"), customMeta);
            SaveStaticPageSettings();

            Refresh();
            VerifyAreEqual(customMeta, GetInputValue("SeoTitle"), "custom title");
            VerifyAreEqual(customMeta, GetInputValue("SeoH1"), "custom h1");
            VerifyAreEqual(customMeta, GetInputValue("SeoDescription"), "custom title");
            VerifyAreEqual(customMeta, GetInputValue("SeoKeywords"), "custom h1");

            //задать свои мета с переменными
            Driver.SendKeysInput(By.Name("SeoTitle"), "#STORE_NAME# - Название магазина");
            Driver.SendKeysInput(By.Name("SeoH1"), "#PAGE_NAME# - Название статической страницы");
            Driver.SendKeysInput(By.Name("SeoDescription"), "#STORE_NAME##PAGE_NAME#");
            Driver.SendKeysInput(By.Name("SeoKeywords"), "#STORE_NAME#,,, #PAGE_NAME#");
            Driver.ScrollToTop();
            Refresh();
            //обновить без сохранения
            VerifyAreEqual(customMeta, GetInputValue("SeoTitle"), "refresh without saving title");
            VerifyAreEqual(customMeta, GetInputValue("SeoH1"), "refresh without saving h1");
            VerifyAreEqual(customMeta, GetInputValue("SeoDescription"), "refresh without saving title");
            VerifyAreEqual(customMeta, GetInputValue("SeoKeywords"), "refresh without saving h1");
            //сохранить мета с переменными
            Driver.SendKeysInput(By.Name("SeoTitle"), "#STORE_NAME# - Название магазина");
            Driver.SendKeysInput(By.Name("SeoH1"), "#PAGE_NAME# - Название статической страницы");
            Driver.SendKeysInput(By.Name("SeoDescription"), "#STORE_NAME##PAGE_NAME#");
            Driver.SendKeysInput(By.Name("SeoKeywords"), "#STORE_NAME#,,, #PAGE_NAME#");
            SaveStaticPageSettings();

            VerifyAreEqual("#STORE_NAME# - Название магазина", GetInputValue("SeoTitle"),
                "refresh without saving title");
            VerifyAreEqual("#PAGE_NAME# - Название статической страницы", GetInputValue("SeoH1"),
                "refresh without saving h1");
            VerifyAreEqual("#STORE_NAME##PAGE_NAME#", GetInputValue("SeoDescription"), "refresh without saving title");
            VerifyAreEqual("#STORE_NAME#,,, #PAGE_NAME#", GetInputValue("SeoKeywords"), "refresh without saving h1");

            Driver.CheckBoxCheck("DefaultMeta");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Refresh();
            VerifyIsTrue(Driver.FindElement(By.Id("DefaultMeta")).Selected, "saved default meta");

            Driver.CheckBoxUncheck("DefaultMeta");
            VerifyAreEqual("#STORE_NAME# - Название магазина", GetInputValue("SeoTitle"), "memorized title");
            VerifyAreEqual("#PAGE_NAME# - Название статической страницы", GetInputValue("SeoH1"), "memorized h1");
            VerifyAreEqual("#STORE_NAME##PAGE_NAME#", GetInputValue("SeoDescription"), "memorized title");
            VerifyAreEqual("#STORE_NAME#,,, #PAGE_NAME#", GetInputValue("SeoKeywords"), "memorized h1");
            Refresh();
            VerifyIsTrue(Driver.FindElement(By.Id("DefaultMeta")).Selected, "refresh without saving2");
        }

        [Test]
        public void InstructionPage()
        {
            VerifyAreEqual("Инструкция. Работа со статическими страницами",
                Driver.FindElement(By.CssSelector(".link-academy span")).Text, "instruction link text");
            VerifyAreEqual("https://www.advantshop.net/help/pages/static-stranicy",
                Driver.FindElement(By.CssSelector(".link-academy")).GetAttribute("href"), "instruction link");
            Driver.FindElement(By.CssSelector(".link-academy span")).Click();
            Thread.Sleep(1000);
            Driver.SwitchTo().Window(Driver.WindowHandles[1]);
            VerifyAreEqual("https://www.advantshop.net/help/pages/static-stranicy", Driver.Url, "instruction link");
            ReInit();
        }

        [Test]
        public void StaticPagePage()
        {
            VerifyIsNull(CheckConsoleLog(true), "log at staticPage Page(1)");

            GoToStoreSettings("Страницы и блоки", "Страницы");
            VerifyAreEqual(BaseUrl + "adminv3/" + spPageUrl, Driver.Url, "static page page url");
            VerifyIsNull(CheckConsoleLog(true), "log at staticPage Page(2)");
        }

        [Test]
        public void ValidationNOTCOMPLETE()
        {
            //??? валидация символов и длины
            //symbols.length ~ 45, longString.length ~ 334
            string symbols = "ёЁ1!2\"3№4;5%6:7?8*9(0)-_=+`~@#$^&[{]}\\|'/,<.>";
            string longString =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit " +
                "in voluptate velit esse cillum dolore eu fugiat nulla pariatur.";

            VerifyIsTrue(false, "добить проверку разрешенных символов и длины имени и ключа");

            //новая страница, проверить количество обязательных
            //пустое имя и заполненный урл
            //заполненное имя и пустой урл
            //заполненное имя и урл -- ок
            //если удалить родителя - останется корень, а не пустота

            Driver.GetByE2E("btnAdd").Click();
            Driver.WaitForElem(By.ClassName("category-content"));
            VerifyIsFalse(Driver.FindElement(AdvBy.DataE2E("btnSave")).Enabled, "save button is disabled");
            Driver.SendKeysInput(By.Id("SortOrder"), "10");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            CheckToastErrors(new List<string> {"Название страницы", "Синоним для URL запроса"}, "new spage empty");

            //+автозаполнение урл
            Driver.SendKeysInput(By.Name("PageName"), "validation name 1");
            Thread.Sleep(500);
            VerifyAreEqual("validation-name-1", GetInputValue("UrlPath"), "autogenerated url1");
            Driver.ClearInput(By.Name("UrlPath"));
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            CheckToastErrors(new List<string> {"Синоним для URL запроса"}, "new spage empty url");

            Driver.ClearInput(By.Name("PageName"));
            Driver.SendKeysInput(By.Name("UrlPath"), "validation-url-1");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            CheckToastErrors(new List<string> {"Название страницы"}, "new spage empty page name");

            Driver.SendKeysInput(By.Name("PageName"), "validation name 1");
            Thread.Sleep(500);
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".toast-success .toast-close-button")).Click();

            //редактирование
            //очистить ключ и имя
            //очистить ключ, непустое имя
            //непустой ключ, очистить имя
            //закрыть без сохранения, ключ и имя -- ок
            GoToAdmin(spPageUrl);
            Driver.GridFilterSendKeys("validation name 1");
            Driver.GetGridCell(0, "PageName", "Pages").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);

            Driver.ClearInput(By.Name("PageName"));
            Driver.ClearInput(By.Name("UrlPath"));
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            CheckToastErrors(new List<string> {"Название страницы", "Синоним для URL запроса"}, "edited spage empty");

            Driver.SendKeysInput(By.Name("PageName"), "validation name 1");
            Thread.Sleep(500);
            VerifyAreEqual("validation-name-1", GetInputValue("UrlPath"), "autogenerated url2");
            Driver.ClearInput(By.Name("UrlPath"));
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Thread.Sleep(500);
            CheckToastErrors(new List<string> {"Синоним для URL запроса"}, "edited spage empty url");

            Driver.ClearInput(By.Name("PageName"));
            Driver.SendKeysInput(By.Name("UrlPath"), "validation-url-1");
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Thread.Sleep(500);
            CheckToastErrors(new List<string> {"Название страницы"}, "edited spage empty page name");

            Driver.SendKeysInput(By.Name("PageName"), "validation name 1");
            Thread.Sleep(500);
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Thread.Sleep(500);


            //новый блок, ключ и имя как есть в системе - имя сохранилось, урл обновился.
            GoToAdmin(spPageUrl);
            Driver.GetByE2E("btnAdd").Click();
            Driver.WaitForElem(By.ClassName("category-content"));
            Driver.SendKeysInput(By.Name("PageName"), "validation name 1");
            Thread.Sleep(500);
            Driver.SendKeysInput(By.Name("UrlPath"), "validation-url-1");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".toast-success .toast-close-button")).Click();
            VerifyAreEqual("validation name 1", Driver.GetValue(By.Id("PageName")), "added page key in editor");
            VerifyAreEqual("validation-url-1-1", Driver.GetValue(By.Id("URL")), "added page url in editor");

            GoToAdmin(spPageUrl);
            Driver.GridFilterSendKeys("validation name 1");
            VerifyAreEqual("Найдено записей: 2", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "added page with same name");


            //редактирование
            //ключ и имя как есть в системе - имя сохранилось, урл обновился.
            Driver.GetGridCell(1, "PageName", "Pages").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);
            Driver.SendKeysInput(By.Name("UrlPath"), "validation-url-1-1");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".toast-success .toast-close-button")).Click();
            VerifyAreEqual("validation-url-1-1-1", Driver.GetValue(By.Id("URL")), "added page url in editor");


            //валидация url
            //цифры, /, русские слова через пробел, знаки препинания, длинная строка
            //проверять преобразование и отображение в мобилке
            GoToAdmin("staticpages/edit/44");
            Driver.WaitForElem(By.ClassName("category-content"));
            Driver.SendKeysInput(By.Name("UrlPath"), "0");
            SaveStaticPageSettings();
            VerifyAreEqual("0", Driver.GetValue(By.Id("URL")), "added page url in editor");
            GoToClient("pages/0");
            VerifyAreEqual("Page44", Driver.FindElement(By.TagName("h1")).Text.Trim(), "0 in url");
            GoToMobile("pages/0");
            VerifyAreEqual("Page44", Driver.FindElement(By.TagName("h1")).Text.Trim(), "mobile 0 in url");
            ReInit();

            GoToAdmin("staticpages/edit/44");
            Driver.WaitForElem(By.ClassName("category-content"));
            Driver.SendKeysInput(By.Name("UrlPath"), "//");
            SaveStaticPageSettings();
            VerifyAreEqual("staticpage", Driver.GetValue(By.Id("URL")), "slash page url in editor");
            GoToClient("pages/staticpage");
            VerifyAreEqual("Page44", Driver.FindElement(By.TagName("h1")).Text.Trim(), "slash in url");
            //check how will replaced 0 at second page - https://task.advant.me/adminv3/tasks#?modal=23461
            GoToAdmin("staticpages/edit/45");
            Driver.WaitForElem(By.ClassName("category-content"));
            Driver.SendKeysInput(By.Name("UrlPath"), "//");
            SaveStaticPageSettings();
            VerifyAreEqual("staticpage-1", Driver.GetValue(By.Id("URL")), "slash 2-th page url in editor");
            GoToClient("pages/staticpage");
            VerifyAreEqual("Page44", Driver.FindElement(By.TagName("h1")).Text.Trim(), "slash in  1-th url");
            GoToClient("pages/staticpage-1");
            VerifyAreEqual("Page45", Driver.FindElement(By.TagName("h1")).Text.Trim(), "slash in  2-th url");
            GoToMobile("pages/staticpage-1");
            VerifyAreEqual("Page45", Driver.FindElement(By.TagName("h1")).Text.Trim(), "mobile slash in  2-th url");
            ReInit();
            GoToAdmin("staticpages/edit/46");
            Driver.WaitForElem(By.ClassName("category-content"));
            Driver.SendKeysInput(By.Name("UrlPath"), "//");
            SaveStaticPageSettings();
            VerifyAreEqual("staticpage-1-1", Driver.GetValue(By.Id("URL")), "slash 3-th page url in editor");
            GoToClient("pages/staticpage-1-1");
            VerifyAreEqual("Page46", Driver.FindElement(By.TagName("h1")).Text.Trim(), "slash in  3-th url");
            GoToMobile("pages/staticpage-1-1");
            VerifyAreEqual("Page45", Driver.FindElement(By.TagName("h1")).Text.Trim(), "mobile slash in  3-th url");
            ReInit();

            GoToAdmin("staticpages/edit/44");
            Driver.WaitForElem(By.ClassName("category-content"));
            Driver.SendKeysInput(By.Name("UrlPath"), "название на русском");
            SaveStaticPageSettings();
            VerifyAreEqual("nazvanie-na-russkom", Driver.GetValue(By.Id("URL")), "russian page url in editor");
            GoToClient("pages/nazvanie-na-russkom");
            VerifyAreEqual("Page44", Driver.FindElement(By.TagName("h1")).Text.Trim(), "russian in url");
            GoToMobile("pages/nazvanie-na-russkom");
            VerifyAreEqual("Page44", Driver.FindElement(By.TagName("h1")).Text.Trim(), "mobile russian in url");
            ReInit();

            GoToAdmin("staticpages/edit/44");
            Driver.WaitForElem(By.ClassName("category-content"));
            Driver.SendKeysInput(By.Name("UrlPath"),
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex e");
            SaveStaticPageSettings();
            VerifyAreEqual("nazvanie-na-russkom", Driver.GetValue(By.Id("URL")), "250+symb page url in editor");
            Driver.FindElement(AdvBy.DataE2E("btnViewStaticPage")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Page44", Driver.FindElement(By.TagName("h1")).Text.Trim(), "250 symb in url");
            VerifyIsTrue(Driver.Url.IndexOf("pages/lorem-ipsum-dolor-sit-amet-consectetuer") != -1,
                "250 symb in url-5");
            string url = Driver.Url;
            GoToMobile(url);
            VerifyAreEqual("Page44", Driver.FindElement(By.TagName("h1")).Text.Trim(), "mobile 250 symb in url");
            VerifyIsTrue(Driver.Url.IndexOf("pages/lorem-ipsum-dolor-sit-amet-consectetuer") != -1,
                "mobile 250 symb in url-2");
            ReInit();
        }
    }
}