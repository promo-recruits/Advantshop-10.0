using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Selenium.Core.Domain;

public class CsvRecord
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public static class Functions
{
    public const string YandexEmail = "testmailimap@yandex.ru";
    public const string YandexPass = "ewqEWQ321#@!";

    public const string SymbolsString = "`~!@#№$;%^:&?*()-_=+[{]};:'\"\\|/.,,<.>/?";

    public const string SymbolsLong = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy" +
                                      " nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, " +
                                      "quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex e1234567890";
    /*
    public static void LogAdmin(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl);
        Cookie adminCookies = new Cookie("customer", "cfc2c33b-1e84-415e-8482-e98156341603", null, "/", null);
        Cookie authCoockies = new Cookie("Advantshop.AUTH", "B94F08F29EB95B90DE6A4F9DA269E31264F63297890284F1AC08F743A63EE43870AE1B6511FE12EBA260EDC51C96891E0DDDBD38F2ACE1A5E508DE3804ADD3F3649EDD0EEE6CDF544E23B7DF8BEB2B05937D45F26B6690BF91A8AE7FEA026F5DD3EC1CDD4082035D618D9EAEDC0576FE9A9B661DE5E996967930DB62D026E8A7620BD8BA", null, "/", null);
        Cookie adminNotifyCoockies = new Cookie("dontDisturbByNotify", "true", null, "/", null);

        driver.Manage().Cookies.AddCookie(adminCookies);
        driver.Manage().Cookies.AddCookie(authCoockies);
        driver.Manage().Cookies.AddCookie(adminNotifyCoockies);
    }*/

    public static void LogCustomer(IWebDriver driver, string baseUrl, string customerId, string auth)
    {
        driver.Navigate().GoToUrl(baseUrl);
        var adminCookies = new Cookie("customer", customerId, null, "/", null);
        var authCookies = new Cookie("Advantshop.AUTH", auth, null, "/", null);
        var taskCookies = new Cookie("tasks_viewmode", "grid", null, "/", null);
        var leadCookies = new Cookie("leads_viewmode", "grid", null, "/", null);
        var adminNotifyCookies = new Cookie("dontDisturbByNotify", "true", null, "/", null);

        driver.Manage().Cookies.AddCookie(adminCookies);
        driver.Manage().Cookies.AddCookie(authCookies);
        driver.Manage().Cookies.AddCookie(taskCookies);
        driver.Manage().Cookies.AddCookie(leadCookies);
        driver.Manage().Cookies.AddCookie(adminNotifyCookies);
    }

    #region Grid

    public static void ChangeEnabledGridColumn(IWebDriver driver, List<string> gridColumnHeaders)
    {
        driver.FindElement(By.ClassName("ui-grid-menu-button")).Click();

        var jse = driver as IJavaScriptExecutor;
        foreach (string gridColumnHeader in gridColumnHeaders)
        {
            By by = By.XPath("//button[contains(@class, 'ui-grid-menu-item')][contains(text(), '" + gridColumnHeader + "')]");
            var element = driver.FindElement(by);
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(by).Click();
        }
        driver.FindElement(By.ClassName("ui-grid-menu-button")).Click();
    }

    #endregion

    #region Filter

    //для любого фильтра
    public static void GridFilterSet(IWebDriver driver, string baseUrl, string name)
    {
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Click();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownItem\"][data-e2e-filter-dropdown-name=\"" +
                                          name + "\"]")).Click();
        Assert.IsTrue(driver
            .FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"" + name +
                                        "\"]")).Displayed);
    }

    public static void GridFilterClose(IWebDriver driver, string baseUrl, string name)
    {
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"" +
                                          name + "\"] [data-e2e=\"gridFilterItemClose\"]")).Click();
        //Thread.Sleep(1000);

        Assert.IsFalse(driver
            .FindElements(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"" +
                                         name + "\"]")).Count > 0);
    }

    //для фильтра где табы
    public static void GridFilterTabSet(IWebDriver driver, string baseUrl, string name, string gridId)
    {
        driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridId + "\"]"))
            .FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Click();
        driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridId + "\"]")).FindElement(
            By.CssSelector("[data-e2e=\"gridFilterDropdownItem\"][data-e2e-filter-dropdown-name=\"" + name +
                           "\"]"))
            .Click();
        Assert.IsTrue(driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridId + "\"]"))
            .FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"" + name +
                                        "\"]")).Displayed);
    }

    public static void GridFilterTabClose(IWebDriver driver, string baseUrl, string name, string gridId)
    {
        driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridId + "\"]")).FindElement(
        By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"" + name +
                       "\"] [data-e2e=\"gridFilterItemClose\"]")).Click();
        //WaitForAjaxFunction(driver);

        Assert.IsFalse(driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridId + "\"]"))
            .FindElements(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"" +
                                         name + "\"]")).Count > 0);
    }

    public static void FilterPageFromTo(IWebDriver driver, string baseUrl, string tag)
    {
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Click();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Clear();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).SendKeys("1");
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Click();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Clear();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).SendKeys("100");
        driver.FindElement(By.CssSelector(tag)).Click();
        //WaitForAjaxFunction(driver);
    }

    public static void GridFilterSelectDropFocus(IWebDriver driver, string baseUrl, string filterName,
        string filterItem, string gridId = null)
    {
        var gridSelector = string.IsNullOrEmpty(gridId) ? "" : "[grid-unique-id=\"grid" + gridId + "\"] ";
        driver.FindElement(By.CssSelector(gridSelector + "[data-e2e=\"gridFilterDropdownButton\"]")).Click();
        driver.FindElement(By.CssSelector(gridSelector +
                                          "[data-e2e=\"gridFilterDropdownItem\"][data-e2e-filter-dropdown-name=\"" +
                                          filterName + "\"]")).Click();
        Assert.IsTrue(driver.FindElement(By.CssSelector(gridSelector +
                                                        "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"" +
                                                        filterName + "\"]")).Displayed);
        driver.FindElement(By.CssSelector(gridSelector + "[data-e2e=\"gridFilterItemSelect\"]")).Click();
        //click
        driver.FindElement(By.CssSelector(gridSelector + "[data-e2e=\"" + filterItem + "\"]")).Click();
        //WaitForAjaxFunction(driver);
    }

    public static void GridDropdownDelete(IWebDriver driver, string baseUrl, string gridId = "", int dropdIndex = 0)
    {
        if (gridId != "")
        {
            driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridId + "\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridId + "\"]"))
                .FindElement(By.CssSelector(
                "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"0\"]"))
                .Click();
        }

        else
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            driver.FindElement(By.CssSelector(
            "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"" + dropdIndex +
            "\"]")).Click();
        }

        //Thread.Sleep(500);
        driver.FindElement(By.ClassName("swal2-confirm")).Click();
        Actions action = new Actions(driver);
        if (driver.FindElements(AdvBy.DataE2E("gridFilterSearch")).Count > 0)
        {
            action.MoveToElement(driver.FindElements(By.CssSelector(".tab-pane.active")).Count > 0
                ? driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(AdvBy.DataE2E("gridFilterSearch"))
                : driver.FindElement(AdvBy.DataE2E("gridFilterSearch")));
        }
        else
        {
            action.MoveToElement(driver.FindElement(By.CssSelector("h2.sticky-page-name-text")));
        }

        action.Perform();
        (driver as IJavaScriptExecutor)?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
        //WaitForAjaxFunction(driver);
    }

    #endregion Filter

    #region ProductLists

    public static void AddProduct_ProductListsFilter(IWebDriver driver, string baseUrl, string filterName)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/mainpageproductsstore");
        driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"1\"]")).Click();
        //Thread.Sleep(500);
        driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
        //Thread.Sleep(500);
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Click();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownItem\"][data-e2e-filter-dropdown-name=\"" +
                                          filterName + "\"]")).Click();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).Click();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).Clear();
    }

    public static void AddProduct_ProductListsFilterFromTo(IWebDriver driver, string baseUrl, string filterName)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/mainpageproductsstore");
        driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"1\"]")).Click();
        //Thread.Sleep(500);
        driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
        //Thread.Sleep(500);
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Click();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownItem\"][data-e2e-filter-dropdown-name=\"" +
                                          filterName + "\"]")).Click();
    }

    public static void AddProduct_ProductListsFilterSelect(IWebDriver driver, string baseUrl, string filter,
        string select)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/mainpageproductsstore?listId=1");
        //Thread.Sleep(500);
        driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
        //Thread.Sleep(500);
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Click();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownItem\"][data-e2e-filter-dropdown-name=\"" +
                                          filter + "\"]")).Click();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
        //scroll in select
        var element = driver.FindElement(By.CssSelector("[data-e2e=\"" + select + "\"]"));
        var jse = driver as IJavaScriptExecutor;
        jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
        //Thread.Sleep(500);
        driver.FindElement(By.CssSelector("[data-e2e=\"" + select + "\"]")).Click();
        driver.FindElement(By.CssSelector(".modal-header-title")).Click();
        //WaitForAjaxFunction(driver);
    }

    public static void AddProductToListByFilter(IWebDriver driver, string linkText, string filter, string item,
        int tabIndex = 0, string gridCell = "")
    {
        driver.FindElement(By.LinkText(linkText)).Click();
        driver.FindElements(By.CssSelector(".header-subtext .btn.btn-sm.btn-action"))[tabIndex].Click();
        //WaitForAjaxFunction(driver);
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Click();
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownItem\"][data-e2e-filter-dropdown-name=\"" +
                                          filter + "\"]")).Click();
        //Thread.Sleep(100);
        driver.FindElement(By.CssSelector("[data-e2e-grid-filter-block-name=\"" + filter + "\"] input"))
            .SendKeys(item);
        driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Click();

        Assert.AreEqual(item,
        driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[0][\'" + gridCell +
                                          "\']\"]")).Text);
        driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[0][\'Name\']\"]")).Click();
        driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
    }

    #endregion ProductLists

    #region CatalogRewiew

    public static void AdminSettingsReviewsImgUploadingOn(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        //Thread.Sleep(1000);

        if (!driver.FindElement(By.CssSelector("[data-e2e=\"allowReviewsImageUploading\"]"))
                .FindElement(By.Id("AllowReviewsImageUploading")).Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"allowReviewsImageUploading\"]"))
                .FindElement(By.TagName("span")).Click();
            //Thread.Sleep(2000);
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]"))
            .FindElement(By.Id("ModerateReviews")).Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (!driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.Id("AllowReviews"))
                .Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
        }
    }

    public static void AdminSettingsReviewsImgUploadingOff(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        if (driver.FindElement(By.CssSelector("[data-e2e=\"allowReviewsImageUploading\"]"))
            .FindElement(By.Id("AllowReviewsImageUploading")).Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"allowReviewsImageUploading\"]"))
                .FindElement(By.TagName("span")).Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]"))
            .FindElement(By.Id("ModerateReviews")).Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (!driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.Id("AllowReviews"))
                .Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }
    }

    public static void AdminSettingsReviewsOn(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        if (!driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.Id("AllowReviews"))
                .Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]"))
            .FindElement(By.Id("ModerateReviews")).Selected)
        {
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
        }
    }

    public static void AdminSettingsReviewsOff(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        if (driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.Id("AllowReviews"))
            .Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
        }
    }

    public static void AdminSettingsReviewsShowImgOn(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        if (!driver.FindElement(By.CssSelector("[data-e2e=\"displayReviewsImage\"]"))
                .FindElement(By.Id("DisplayReviewsImage")).Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"displayReviewsImage\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]"))
            .FindElement(By.Id("ModerateReviews")).Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (!driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.Id("AllowReviews"))
                .Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
        }
    }

    public static void AdminSettingsReviewsModerateOn(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        if (!driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]"))
                .FindElement(By.Id("ModerateReviews")).Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            //Thread.Sleep(1000);

            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            //Thread.Sleep(1000);

            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            //Thread.Sleep(2000);

            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (!driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.Id("AllowReviews"))
                .Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            //Thread.Sleep(1000);

            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            //Thread.Sleep(1000);

            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
        }
    }

    public static void AdminSettingsReviewsModerateOff(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        if (driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]"))
            .FindElement(By.Id("ModerateReviews")).Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (!driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.Id("AllowReviews"))
                .Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
        }
    }

    public static void AdminSettingsReviewsShowImgOff(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        if (driver.FindElement(By.CssSelector("[data-e2e=\"displayReviewsImage\"]"))
            .FindElement(By.Id("DisplayReviewsImage")).Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"displayReviewsImage\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            //WaitForAjaxFunction(driver);
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]"))
            .FindElement(By.Id("ModerateReviews")).Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"moderateReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        }

        if (!driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.Id("AllowReviews"))
                .Selected)
        {
            var element = driver.FindElement(By.CssSelector("[data-e2e=\"EnableZoom\"]"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            driver.FindElement(By.CssSelector("[data-e2e=\"allowReviews\"]")).FindElement(By.TagName("span"))
                .Click();
            jse?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
        }
    }

    #endregion CatalogRewiew

    #region Mobile

    public static void AdminMobileOn(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settings/mobileversion");
        if (!driver.FindElement(By.CssSelector("[data-e2e=\"mobileEnabled\"]")).FindElement(By.Id("Enabled"))
                .Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"mobileEnabled\"]")).FindElement(By.TagName("span"))
                .Click();
            //Thread.Sleep(2000);
            driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
            //Thread.Sleep(2000);
        }
    }

    public static void AdminMobileOff(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settings/mobileversion");
        //Thread.Sleep(1000);

        driver.FindElement(By.CssSelector("[data-e2e=\"mobileEnabled\"]")).FindElement(By.TagName("span")).Click();
        //Thread.Sleep(2000);
        driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
        //Thread.Sleep(2000);
    }

    public static void AdminMobileCheckoutOn(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settings/mobileversion");
        if (driver.FindElement(By.CssSelector("[data-e2e=\"mobileFullCheckout\"]"))
            .FindElement(By.Name("IsFullCheckout")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"mobileFullCheckout\"]")).FindElement(By.TagName("span"))
                .Click();
            //Thread.Sleep(2000);
            driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
            //Thread.Sleep(2000);
        }
    }

    public static void AdminMobileCheckoutOff(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settings/mobileversion");
        if (!driver.FindElement(By.CssSelector("[data-e2e=\"mobileFullCheckout\"]"))
                .FindElement(By.Name("IsFullCheckout")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"mobileFullCheckout\"]")).FindElement(By.TagName("span"))
                .Click();
            //Thread.Sleep(2000);
            driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
            //Thread.Sleep(2000);
        }
    }

    #endregion Mobile

    #region ExportProducts

    public static void ExportProductsNoInCategoryOn(IWebDriver driver, string baseUrl)
    {
        if (!driver.FindElement(By.Name("CsvExportNoInCategory")).Selected)
        {
            var element = driver.FindElement(By.Name("CsvPropertySeparator"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            //Thread.Sleep(1000);

            driver.FindElement(By.CssSelector("[data-e2e=\"CsvExportNoInCategory\"] span")).Click();
        }
    }

    public static void ExportProductsNoInCategoryOff(IWebDriver driver, string baseUrl)
    {
        if (driver.FindElement(By.Name("CsvExportNoInCategory")).Selected)
        {
            var element = driver.FindElement(By.Name("CsvPropertySeparator"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            //Thread.Sleep(1000);

            driver.FindElement(By.CssSelector("[data-e2e=\"CsvExportNoInCategory\"] span")).Click();
        }
    }

    public static void ExportProductsCategorySortOn(IWebDriver driver, string baseUrl)
    {
        if (!driver.FindElement(By.Name("CsvCategorySort")).Selected)
        {
            var element = driver.FindElement(By.Name("CsvExportNoInCategory"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            //Thread.Sleep(1000);

            driver.FindElement(By.CssSelector("[data-e2e=\"CsvCategorySort\"] span")).Click();
        }
    }

    public static void ExportProductsCategorySortOff(IWebDriver driver, string baseUrl)
    {
        if (driver.FindElement(By.Name("CsvCategorySort")).Selected)
        {
            var element = driver.FindElement(By.Name("CsvExportNoInCategory"));
            var jse = driver as IJavaScriptExecutor;
            jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            //Thread.Sleep(1000);

            driver.FindElement(By.CssSelector("[data-e2e=\"CsvCategorySort\"] span")).Click();
        }
    }

    #endregion ExportProducts

    #region OrderCart

    public static void NewOrderClient_450(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/products/test-product5");
        //Thread.Sleep(2000);

        var element = driver.FindElement(By.CssSelector(".link-dotted-invert.cs-l-d-1"));
        var jse = driver as IJavaScriptExecutor;
        jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
        //Thread.Sleep(1000);

        driver.FindElement(By.CssSelector(".link-dotted-invert.cs-l-d-1")).Click();
        //Thread.Sleep(2000);
        driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
        //Thread.Sleep(2000);
    }

    public static void NewOrderClient_1350(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/products/test-product15");
        //Thread.Sleep(2000);
        var element = driver.FindElement(By.CssSelector(".link-dotted-invert.cs-l-d-1"));
        var jse = driver as IJavaScriptExecutor;
        jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
        //Thread.Sleep(1000);

        driver.FindElement(By.CssSelector(".link-dotted-invert.cs-l-d-1")).Click();
        //Thread.Sleep(2000);
        driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
        //Thread.Sleep(2000);
    }

    public static void NewOrderClient_9000(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/products/test-product100");
        //Thread.Sleep(2000);
        var element = driver.FindElement(By.CssSelector(".link-dotted-invert.cs-l-d-1"));
        var jse = driver as IJavaScriptExecutor;
        jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
        //Thread.Sleep(1000);

        driver.FindElement(By.CssSelector(".link-dotted-invert.cs-l-d-1")).Click();
        //Thread.Sleep(2000);
        driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
        //Thread.Sleep(2000);
    }

    public static void NewFullOrderClient_9000(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/products/test-product100");
        var element = driver.FindElement(By.CssSelector(".link-dotted-invert.cs-l-d-1"));
        var jse = driver as IJavaScriptExecutor;
        jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
        //Thread.Sleep(500);
        driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
        //Thread.Sleep(1000);

        driver.Navigate().GoToUrl(baseUrl + "/checkout");

        element = driver.FindElement(By.CssSelector(".checkout-result"));
        jse = driver as IJavaScriptExecutor;
        jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
        //Thread.Sleep(500);
        driver.FindElement(AdvBy.DataE2E("btnCheckout"));
        driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
        //Thread.Sleep(1000);
    }

    public static void ProductToCart(IWebDriver driver, string baseUrl, string id)
    {
        driver.Navigate().GoToUrl(baseUrl + id);
        //Thread.Sleep(2000);
        var element = driver.FindElement(By.CssSelector(".link-dotted-invert.cs-l-d-1"));
        var jse = driver as IJavaScriptExecutor;
        jse?.ExecuteScript("arguments[0].scrollIntoView(true)", element);
        driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
        //Thread.Sleep(1000);
    }

    public static void CleanCart(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/cart");
        if (driver.FindElements(By.CssSelector(".cart-full-product")).Count > 0)
        {
            driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            //Thread.Sleep(2000);
        }
    }

    public static void AdminSettingsProductCart(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=product");
        //Thread.Sleep(2000);

        if (!driver.FindElement(By.Id("DisplayWeight")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"DisplayWeight\"]")).Click();
            //Thread.Sleep(500);
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            //Thread.Sleep(2000);
        }

        if (!driver.FindElement(By.Id("DisplayDimensions")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"DisplayDimensions\"]")).Click();
            //Thread.Sleep(500);
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            //Thread.Sleep(2000);
        }

        if (!driver.FindElement(By.Id("ShowStockAvailability")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"ShowStockAvailability\"]")).Click();
            //Thread.Sleep(500);
            driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            //Thread.Sleep(2000);
        }
    }

    #endregion orderCart

    #region DataTimePicker

    public static void DataTimePickerFilter(IWebDriver driver, string baseUrl, string monthFrom = "",
        string yearFrom = "", string dataFrom = "", string hourFrom = "", string minFrom = "", string monthTo = "",
        string yearTo = "", string dataTo = "", string hourTo = "", string minTo = "", string dropFocusElem = "h1",
        string fieldFrom = "", string fieldTo = "")
    {
        string yearSelector;
        if (fieldTo != "")
        {
            driver.FindElement(By.CssSelector("" + fieldTo + "")).Click();
            driver.FindElement(By.CssSelector("" + fieldTo + "")).Clear();
        }

        if (monthTo != "")
        {
            if (driver.FindElements(By.CssSelector(".flatpickr-calendar.hasTime.animate.open")).Count == 0)
            {
                driver.FindElement(By.CssSelector("[for=\"gridFilterDateTo\"]~div .input-group-addon")).Click();
            }

            //Thread.Sleep(2000);

            var curMonth =
                (new SelectElement(driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                    .FindElement(By.CssSelector(".flatpickr-monthDropdown-months")))).SelectedOption.Text;
            if (!curMonth.Contains(monthTo))
            {
                while (!curMonth.Contains(monthTo))
                {
                    driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                        .FindElement(By.CssSelector(".flatpickr-next-month")).Click();
                    //Thread.Sleep(2000);
                    curMonth = (new SelectElement(driver
                        .FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                        .FindElement(By.CssSelector(".flatpickr-monthDropdown-months")))).SelectedOption.Text;
                }
            }

            var curYear = driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                .FindElement(By.CssSelector(".numInput.cur-year")).GetAttribute("value");

            if (yearTo == "" || Convert.ToInt32(curYear) > Convert.ToInt32(yearTo))
                yearSelector = ".arrowDown";
            else yearSelector = ".arrowUp";

            if (!curYear.Contains(yearTo))
            {
                while (!curYear.Contains(yearTo))
                {
                    driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                        .FindElement(By.CssSelector(".flatpickr-month")).FindElement(By.CssSelector(yearSelector))
                        .Click();
                    //Thread.Sleep(2000);
                    curYear = driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                        .FindElement(By.CssSelector(".numInput.cur-year")).GetAttribute("value");
                }
            }

            if (monthFrom == monthTo)
            {
                driver.FindElements(By.CssSelector("[aria-label=\"" + dataTo + "\"]"))[0].Click();
            }
            else
            {
                driver.FindElement(By.CssSelector("[aria-label=\"" + dataTo + "\"]")).Click();
            }

            if (hourTo != "")
            {
                var curTimeHour = driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                    .FindElement(By.CssSelector(".numInput.flatpickr-hour")).GetAttribute("value");
                if (!curTimeHour.Contains(hourTo))
                {
                    while (!curTimeHour.Contains(hourTo))
                    {
                        driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                            .FindElement(By.CssSelector(".flatpickr-time.time24hr"))
                            .FindElement(By.CssSelector(".arrowUp")).Click();
                        //Thread.Sleep(2000);
                        curTimeHour = driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                            .FindElement(By.CssSelector(".numInput.flatpickr-hour")).GetAttribute("value");
                    }
                }
            }

            if (minTo != "")
            {
                var curTimeMin = driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                    .FindElement(By.CssSelector(".numInput.flatpickr-minute")).GetAttribute("value");
                if (!curTimeMin.Contains(minTo))
                {
                    while (!curTimeMin.Contains(minTo))
                    {
                        driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                            .FindElement(By.CssSelector(".flatpickr-time.time24hr"))
                            .FindElements(By.CssSelector(".arrowUp"))[1].Click();
                        //Thread.Sleep(2000);
                        curTimeMin = driver.FindElement(By.CssSelector(".flatpickr-calendar.hasTime.animate.open"))
                            .FindElement(By.CssSelector(".numInput.flatpickr-minute")).GetAttribute("value");
                    }
                }
            }
        }

        if (fieldFrom != "")
        {
            driver.FindElement(By.CssSelector("" + fieldFrom + "")).Click();
            driver.FindElement(By.CssSelector("" + fieldFrom + "")).Clear();

            if (monthFrom == "")
            {
                driver.FindElement(By.CssSelector(dropFocusElem)).Click();
            }
        }

        if (monthFrom != "")
        {
            if (driver.FindElements(By.CssSelector(".flatpickr-calendar.hasTime.animate.open")).Count == 0)
            {
                driver.FindElement(By.CssSelector("[for=\"gridFilterDateFrom\"]~div .input-group-addon")).Click();
            }

            //Thread.Sleep(2000);
            var curMonth =
                (new SelectElement(driver.FindElement(By.CssSelector(".flatpickr-calendar.open"))
                    .FindElement(By.CssSelector(".flatpickr-monthDropdown-months")))).SelectedOption.Text;
            if (!curMonth.Contains(monthFrom))
            {
                while (!curMonth.Contains(monthFrom))
                {
                    driver.FindElement(By.CssSelector(".flatpickr-calendar.open"))
                        .FindElement(By.CssSelector(".flatpickr-next-month")).Click();
                    //Thread.Sleep(2000);
                    curMonth = (new SelectElement(driver.FindElement(By.CssSelector(".flatpickr-calendar.open"))
                        .FindElement(By.CssSelector(".flatpickr-monthDropdown-months")))).SelectedOption.Text;
                }
            }

            var curYear = driver.FindElement(By.CssSelector(".flatpickr-calendar.open"))
                .FindElement(By.CssSelector(".numInput.cur-year")).GetAttribute("value");
            if (yearFrom == "" || Convert.ToInt32(curYear) > Convert.ToInt32(yearFrom))
                yearSelector = ".arrowDown";
            else yearSelector = ".arrowUp";
            if (!curYear.Contains(yearFrom))
            {
                while (!curYear.Contains(yearFrom))
                {
                    driver.FindElement(By.CssSelector(".flatpickr-calendar.open"))
                        .FindElement(By.CssSelector(".flatpickr-month")).FindElement(By.CssSelector(yearSelector))
                        .Click();
                    //Thread.Sleep(2000);
                    curYear = driver.FindElement(By.CssSelector(".flatpickr-calendar.open"))
                        .FindElement(By.CssSelector(".numInput.cur-year")).GetAttribute("value");
                }
            }

            driver.FindElement(By.CssSelector("[aria-label=\"" + dataFrom + "\"]")).Click();
            if (minFrom != "")
            {
                var curTimeMin = driver.FindElements(By.CssSelector(".numInput.flatpickr-minute"))[0]
                    .GetAttribute("value");
                if (!curTimeMin.Contains(minFrom))
                {
                    while (!curTimeMin.Contains(minFrom))
                    {
                        driver.FindElement(By.CssSelector(".flatpickr-calendar.open"))
                            .FindElement(By.CssSelector(".flatpickr-time.time24hr"))
                            .FindElements(By.CssSelector(".arrowUp"))[1].Click();
                        //Thread.Sleep(2000);
                        curTimeMin = driver.FindElements(By.CssSelector(".numInput.flatpickr-minute"))[0]
                            .GetAttribute("value");
                    }
                }
            }

            if (hourFrom != "")
            {
                var curTimeHour = driver.FindElement(By.CssSelector(".flatpickr-calendar.open"))
                    .FindElement(By.CssSelector(".numInput.flatpickr-hour")).GetAttribute("value");
                if (!curTimeHour.Contains(hourFrom))
                {
                    while (!curTimeHour.Contains(hourFrom))
                    {
                        driver.FindElement(By.CssSelector(".flatpickr-calendar.open"))
                            .FindElement(By.CssSelector(".flatpickr-time.time24hr"))
                            .FindElement(By.CssSelector(".arrowUp")).Click();
                        //Thread.Sleep(2000);
                        curTimeHour = driver.FindElement(By.CssSelector(".flatpickr-calendar.open"))
                            .FindElement(By.CssSelector(".numInput.flatpickr-hour")).GetAttribute("value");
                    }
                }
            }
        }

        driver.FindElement(By.CssSelector(dropFocusElem)).Click();
        //Thread.Sleep(1000);
    }

    public static void DataTimePicker(IWebDriver driver, string baseUrl, string month = "", string year = "",
        string data = "", string dropFocusElem = "h1", string field = "", int flatpickrIndex = 0)
    {
        if (field != "")
        {
            driver.FindElement(By.CssSelector("" + field + "")).SendKeys(Keys.Control + "a" + Keys.Delete);
        }

        var flatpickrCalendar =
            driver.FindElements(By.CssSelector(".flatpickr-calendar.animate.open"))[flatpickrIndex];

        if (month != "")
        {
            var curMonth =
                (new SelectElement(
                flatpickrCalendar.FindElement(By.CssSelector(".flatpickr-monthDropdown-months"))))
                .SelectedOption.Text;
            if (!curMonth.Contains(month))
            {
                while (!curMonth.Contains(month))
                {
                    flatpickrCalendar.FindElement(By.CssSelector(".flatpickr-next-month")).Click();
                    //Thread.Sleep(2000);
                    curMonth =
                        (new SelectElement(
                        flatpickrCalendar.FindElement(By.CssSelector(".flatpickr-monthDropdown-months"))))
                        .SelectedOption.Text;
                }
            }

            var curYear = flatpickrCalendar.FindElement(By.CssSelector(".numInput.cur-year")).GetAttribute("value");

            string yearSelector;
            if (year == "" || Convert.ToInt32(curYear) > Convert.ToInt32(year))
                yearSelector = ".arrowDown";
            else yearSelector = ".arrowUp";

            if (!curYear.Contains(year))
            {
                while (!curYear.Contains(year))
                {
                    flatpickrCalendar.FindElement(By.CssSelector(".flatpickr-month"))
                        .FindElement(By.CssSelector(yearSelector)).Click();
                    //Thread.Sleep(2000);
                    curYear = flatpickrCalendar.FindElement(By.CssSelector(".numInput.cur-year"))
                        .GetAttribute("value");
                }
            }

            driver.FindElements(By.CssSelector("[aria-label=\"" + data + "\"]"))[0].Click();
        }

        driver.FindElement(By.CssSelector(dropFocusElem)).Click();
        //Thread.Sleep(1000);
    }

    public static void DataTimePickerDay(IWebDriver driver, string baseUrl, string dropFocusElem = "h1",
        string dayCssSelector = ".flatpickr-day.today", int flatpickrIndex = 0)
    {
        driver.FindElement(By.CssSelector(dropFocusElem)).Click();
        //Thread.Sleep(1000);

        if (driver.FindElements(By.CssSelector(".flatpickr-calendar.animate.open")).Count == 0)
        {
            driver.FindElements(By.ClassName("input-group-addon"))[flatpickrIndex].Click();
        }

        driver.FindElements(By.CssSelector(".flatpickr-calendar.animate.open"))[0]
            .FindElement(By.CssSelector(dayCssSelector)).Click();

        driver.FindElement(By.CssSelector(dropFocusElem)).Click();
        //Thread.Sleep(1000);
    }

    #endregion DataTimePicker

    #region Indicators

    public static void IndicatorsNoBeforeMainPageAdmin(IWebDriver driver, string baseUrl)
    {
        driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();
        //Thread.Sleep(2000);
        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ProductsCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ProductsCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersTodayCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersTodayCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersYesterdayCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersYesterdayCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersMonthCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersMonthCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersAllTimeCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersAllTimeCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsTodayCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsTodayCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsYesterdayCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsYesterdayCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsMonthCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsMonthCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsTodayCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsTodayCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsYesterdayCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsYesterdayCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsMonthCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsMonthCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsTodayCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsTodayCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsYesterdayCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsYesterdayCount\"]")).Click();
        }

        if (driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsMonthCount\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsMonthCount\"]")).Click();
        }

        driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
        //Thread.Sleep(2000);
    }

    #endregion Indicators

    #region Funnels

    public static void SetFunnelName(IWebDriver driver, string funnelName)
    {
        driver.FindElement(By.CssSelector(".funnel-modal-content-new-item .form-control")).SendKeys(funnelName);
        driver.FindElement(By.CssSelector("[data-e2e=\"LandingSiteNext\"]")).Click();
        //Thread.Sleep(2000);
    }

    #endregion

    #region Mail

    public static void MailSmtp(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingsmail#?notifyTab=emailsettings");

        if (!driver.FindElement(By.CssSelector("[data-e2e=\"UseSmtpInput\"]")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"UseSmtpMail\"]")).Click();
        }

        driver.FindElement(By.Name("SMTP")).Click();
        driver.FindElement(By.Name("SMTP")).Clear();
        driver.FindElement(By.Name("SMTP")).SendKeys("smtp.yandex.ru");

        driver.FindElement(By.Name("Port")).Click();
        driver.FindElement(By.Name("Port")).Clear();
        driver.FindElement(By.Name("Port")).SendKeys("25");

        driver.FindElement(By.Name("Login")).Click();
        driver.FindElement(By.Name("Login")).Clear();
        driver.FindElement(By.Name("Login")).SendKeys(YandexEmail);

        driver.FindElement(By.Name("Password")).Click();
        driver.FindElement(By.Name("Password")).Clear();
        driver.FindElement(By.Name("Password")).SendKeys(YandexPass);

        if (!driver.FindElement(By.Name("SSL")).Selected)
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"SSL\"]")).Click();
        }

        driver.FindElement(By.Name("From")).Click();
        driver.FindElement(By.Name("From")).Clear();
        driver.FindElement(By.Name("From")).SendKeys(YandexEmail);

        driver.FindElement(By.Name("SenderName")).Click();
        driver.FindElement(By.Name("SenderName")).Clear();
        driver.FindElement(By.Name("SenderName")).SendKeys("Test Sender Name");

        driver.FindElement(By.Name("ImapHost")).Click();
        driver.FindElement(By.Name("ImapHost")).Clear();
        driver.FindElement(By.Name("ImapHost")).SendKeys("imap.yandex.ru");

        driver.FindElement(By.Name("ImapPort")).Click();
        driver.FindElement(By.Name("ImapPort")).Clear();
        driver.FindElement(By.Name("ImapPort")).SendKeys("993");

        (driver as IJavaScriptExecutor)?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
        try
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveMailSettings\"]")).Click();
            //Thread.Sleep(2000);
        }
        catch
        {
            // ignored
        }
    }

    public static void SingInYandex(IWebDriver driver)
    {
        driver.Navigate().GoToUrl("https://passport.yandex.ru/auth");
        if (driver.Manage().Cookies.GetCookieNamed("yandex_login") == null)
        {
            driver.Manage().Cookies.AddCookie(new Cookie("yandex_login", "testmailimap", ".yandex.ru", "/", null));
            driver.Manage().Cookies.AddCookie(new Cookie("yandexuid", "175826191634113453", ".yandex.ru", "/", null));
            driver.Manage().Cookies.AddCookie(new Cookie("yuidss", "175826191634113453", ".yandex.ru", "/", null));
            driver.Manage().Cookies.AddCookie(new Cookie("Session_id", "3:1634114726.5.0.1634114726330:QhDvWw:22.1|472614925.0.2|3:242063.274119.-aElYUoIxsHdRTl1HVVVgyttz9c", ".yandex.ru", "/", null));
        }
    }

    public static void CloseYandexMailPopups(IWebDriver driver)
    {
        if (driver.FindElements(By.ClassName("mail-Wizard-Close")).Count > 0)
        {
            driver.FindElement(By.ClassName("mail-Wizard-Close")).Click();
        }

        if (driver.FindElements(By.ClassName("b-popup__close")).Count > 0)
        {
            driver.FindElement(By.ClassName("b-popup__close")).Click();
        }
    }

    #endregion

    #region NewDashboard

    public static void TabSubPageClick(IWebDriver driver, string tabName)
    {
        driver.FindElement(By.ClassName("nav-tabs")).FindElement(By.PartialLinkText(tabName)).Click();
        //Thread.Sleep(2000);
    }

    #endregion

    #region Settings

    public static void EnableCapcha(IWebDriver driver, string baseUrl, bool inCheckout = false, bool inReg = false,
        bool inPreorder = false, bool inGiftCert = false, bool inFeedback = false, bool inReview = false,
        bool inBuyOneClick = false, bool otherPages = false)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingssystem");

        if (inCheckout)
        {
            CheckSelected("EnableCaptchaInCheckout", driver);
        }
        else
        {
            CheckNotSelected("EnableCaptchaInCheckout", driver);
        }

        if (inReg)
        {
            CheckSelected("EnableCaptchaInRegistration", driver);
        }
        else
        {
            CheckNotSelected("EnableCaptchaInRegistration", driver);
        }

        if (inPreorder)
        {
            CheckSelected("EnableCaptchaInPreOrder", driver);
        }
        else
        {
            CheckNotSelected("EnableCaptchaInPreOrder", driver);
        }

        if (inGiftCert)
        {
            CheckSelected("EnableCaptchaInGiftCerticate", driver);
        }
        else
        {
            CheckNotSelected("EnableCaptchaInGiftCerticate", driver);
        }

        if (inFeedback)
        {
            CheckSelected("EnableCaptchaInFeedback", driver);
        }
        else
        {
            CheckNotSelected("EnableCaptchaInFeedback", driver);
        }

        if (inReview)
        {
            CheckSelected("EnableCaptchaInProductReview", driver);
        }
        else
        {
            CheckNotSelected("EnableCaptchaInProductReview", driver);
        }

        if (inBuyOneClick)
        {
            CheckSelected("EnableCaptchaInBuyInOneClick", driver);
        }
        else
        {
            CheckNotSelected("EnableCaptchaInBuyInOneClick", driver);
        }

        if (otherPages)
        {
            CheckSelected("EnableCaptcha", driver);
        }
        else
        {
            CheckNotSelected("EnableCaptcha", driver);
        }

        (driver as IJavaScriptExecutor)?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
        //Thread.Sleep(1000);

        if (driver.FindElements(By.CssSelector("[data-e2e=\"BtnSaveSettings\"][disabled]")).Count == 0)
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
        }
    }

    #endregion

    public static void SetAdminStartPage(IWebDriver driver, string baseUrl, string startPage)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingssystem");
        (new SelectElement(driver.FindElement(By.Id("AdminStartPage")))).SelectByValue(startPage);
        driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
        //Thread.Sleep(1000);
    }

    public static List<string> GetTemplates()
    {
        var environmentVariable = GetEnvironmentVariable("TEMPLATES");
        if (!environmentVariable.IsNullOrEmpty() && environmentVariable.IndexOf("-l", StringComparison.Ordinal) != -1)
        {
            environmentVariable = environmentVariable.Substring(3);
            return environmentVariable.Split(' ').ToList();
        }
        else
        {
            return LoadCsvFile("Data\\Client\\TestSettings\\TemplateNamesData.csv", true).ToList();
        }
    }

    public static List<string> GetModules()
    {
        var environmentVariable = GetEnvironmentVariable("MODULES");
        return !environmentVariable.IsNullOrEmpty() ? environmentVariable.Split(',').ToList() : null;
    }

    public static string GetCustomSiteUrl()
    {
        var environmentVariable = GetEnvironmentVariable("TEMPLATE_SITE");
        return !environmentVariable.IsNullOrEmpty() ? environmentVariable : null;
    }

    public static string GetEnvironmentVariable(string key)
    {
        //with "EnvironmentVariableTarget.Machine" for local machine, without for teamcity
        //return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);
        return Environment.GetEnvironmentVariable(key);
    }

    public static void TemplateSettingsSelect(IWebDriver driver, string baseUrl, string select, string settingsName)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingstemplate#?settingsTemplateTab=common");
        //Thread.Sleep(2000);
        new SelectElement(driver.FindElement(By.Id(settingsName))).SelectByText(select);
        driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
        //Thread.Sleep(1000);
    }

    public static void SelectItem(IWebDriver driver, string selectId, int optionIndex)
    {
        (new SelectElement(driver.FindElement(By.Id(selectId)))).SelectByIndex(optionIndex);
        //Thread.Sleep(1000);
    }

    public static void SelectItem(IWebDriver driver, string selectId, string optionSelector,
        bool selectByText = true)
    {
        if (selectByText)
        {
            (new SelectElement(driver.FindElement(By.Id(selectId)))).SelectByText(optionSelector);
        }
        else
        {
            (new SelectElement(driver.FindElement(By.Id(selectId)))).SelectByValue(optionSelector);
        }

        //Thread.Sleep(1000);
    }

    public static void CloseTab(IWebDriver driver, string baseUrl)
    {
        driver.Close();
        //Thread.Sleep(2000);
        ReadOnlyCollection<string> windowHandles = driver.WindowHandles;
        var previousTab = windowHandles[^1];
        driver.SwitchTo().Window(previousTab);
        //Thread.Sleep(2000);
    }

    public static void OpenNewTab(IWebDriver driver, string baseUrl)
    {
        ReadOnlyCollection<string> windowHandles = driver.WindowHandles;
        var nextTab = windowHandles[^1];
        driver.SwitchTo().Window(nextTab);
        //Thread.Sleep(2000);
    }

    public static void CheckSelected(string id, IWebDriver driver)
    {
        if (!driver.FindElement(By.Id(id)).Selected)
        {
            driver.FindElement(AdvBy.DataE2E(id)).Click();
            //Thread.Sleep(2000);
        }
    }

    public static void CheckNotSelected(string id, IWebDriver driver)
    {
        if (driver.FindElement(By.Id(id)).Selected)
        {
            driver.FindElement(AdvBy.DataE2E(id)).Click();
            //Thread.Sleep(2000);
        }
    }

    public static void KanbanOn(IWebDriver driver, string baseUrl, string url = "")
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/" + url);
        if (!driver.FindElement(By.Name("UseKanban")).Selected)
        {
            driver.FindElement(AdvBy.DataE2E("UseKanban")).Click();
            //Thread.Sleep(2000);
        }
    }

    public static void KanbanOff(IWebDriver driver, string baseUrl, string url = "")
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/" + url);
        if (driver.FindElement(By.Name("UseKanban")).Selected)
        {
            driver.FindElement(AdvBy.DataE2E("UseGrid")).Click();
            //Thread.Sleep(2000);
        }
    }

    public static void CloseModalTask(IWebDriver driver)
    {
        driver.FindElement(By.CssSelector(".modal-header")).FindElement(By.CssSelector(".close")).Click();
        //Thread.Sleep(2000);
        driver.FindElement(By.CssSelector(".swal2-confirm")).Click();
        //Thread.Sleep(1000);
    }

    public static void DragDropElement(IWebDriver driver, IWebElement dragElement, IWebElement dropElement)
    {
        var builder = new Actions(driver);
        builder.ClickAndHold(dragElement);
        builder.MoveToElement(dropElement, 5, 5);
        builder.Perform();
        //Thread.Sleep(250);
        builder.Release(dropElement);
        builder.Perform();
    }

    public static void DelElement(IWebDriver driver)
    {
        int count = driver.FindElements(By.CssSelector(".pull-right a")).Count;
        for (int i = 0; i < count; i++)
        {
            driver.FindElement(By.CssSelector(".pull-right a")).Click();
            //Thread.Sleep(2000);
            driver.FindElement(By.ClassName("swal2-confirm")).Click();
            //Thread.Sleep(2000);
        }
    }

    public static void PriceRegulation(IWebDriver driver, string baseUrl, string @select, string selectOption)
    {
        driver.Navigate().GoToUrl(baseUrl + "/adminv3/settingscatalog#?catalogTab=priceregulation");
        //Thread.Sleep(2000);
        new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationSelect\"]")))
            .SelectByValue(@select);
        new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationSelectOption\"]")))
            .SelectByValue(selectOption);
    }

    public static void SetCity(IWebDriver driver, string city, bool inMobile = false)
    {
        if (inMobile)
        {
            driver.FindElement(By.ClassName("mobile-header__menu-triger")).Click();
            //Thread.Sleep(500);
            driver.FindElement(By.CssSelector(".menu__item--root .menu__city")).Click();
        }
        else
        {
            driver.FindElement(By.CssSelector("a[data-zone-dialog-trigger]")).Click();
        }

        driver.FindElement(By.Name("zoneCity")).SendKeys(city + Keys.Enter);
        if (inMobile)
        {
            driver.FindElement(By.ClassName("autocompleter-list-item")).Click();
            driver.FindElement(By.ClassName("mobile-header__menu-triger")).Click();
            //Thread.Sleep(500);
        }
    }


    #region CSVload

    public static Dictionary<string, string> LoadCsvFile(string filePath, string encoding = null,
        string separators = null, List<string> neededItems = null)
    {
        var projectPath =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (projectPath == null)
            return null;

        projectPath = new DirectoryInfo(projectPath).Parent?.Parent?.Parent?.FullName;

        if (projectPath == null)
            return null;

        filePath = projectPath + "\\" + filePath;

        Dictionary<string, string> searchDict = new();
        using var reader = new CsvReader(
        new StreamReader(filePath,
        Encoding.GetEncoding(encoding ?? EncodingsEnum.Utf8.StrName())),
        new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = separators ?? SeparatorsEnum.SemicolonSeparated.StrName()
        });

        var records = reader.GetRecords<CsvRecord>();
        if (neededItems == null)
        {
            foreach (var el in records)
            {
                searchDict[el.Name] = el.Value;
            }
        }
        else
        {
            foreach (var el in records)
            {
                if (neededItems.Contains(el.Name))
                {
                    searchDict[el.Name] = el.Value;
                }
            }
        }

        return searchDict;
    }

    public static IEnumerable<string> LoadCsvFile(string filePath, bool returnStr, string encoding = null,
        string separators = null)
    {
        var projectPath =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (projectPath == null)
            return null;

        projectPath = new DirectoryInfo(projectPath).Parent?.Parent?.Parent?.FullName;

        if (projectPath == null)
            return null;

        filePath = projectPath + "\\" + filePath;

        using var reader = new CsvReader(
        new StreamReader(filePath,
        Encoding.GetEncoding(encoding ?? EncodingsEnum.Utf8.StrName())),
        new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = separators ?? SeparatorsEnum.SemicolonSeparated.StrName()
        });

        return reader.GetRecords<CsvRecord>().Select(el => el.Name).ToList();
    }

    #endregion

    [Obsolete("Now AdvWebDriver wait http after FindElement.")]
    public static void WaitForAjaxFunction(IWebDriver driver)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        wait.Until(d =>
            (bool) ((d as IJavaScriptExecutor)?.ExecuteScript("return window.ajaxIsComplete();") ?? false));
    }
}