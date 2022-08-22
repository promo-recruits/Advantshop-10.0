using AdvantShop.Selenium.Core.Infrastructure;

namespace AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;

public static class UiElementsHelper
{
    public static IWebElement GetByE2E(this IWebDriver driver, string dataE2E)
    {
        return driver.FindElement(AdvBy.DataE2E(dataE2E));
    }

    public static void XPathContainsText(this IWebDriver driver, string tag = "", string text = "")
    {
        driver.FindElement(By.XPath("//" + tag + "[contains(text(), '" + text + "')]")).Click();
    }

    public static void ElementSendKeys(this IWebDriver driver, By by, string text, string tagToDropFocus = null)
    {
        driver.FindElement(by).Click();
        driver.FindElement(by).Clear();
        driver.FindElement(by).SendKeys(text);
        if (tagToDropFocus is not null)
            driver.DropFocus(tagToDropFocus);
    }

    #region Input

    public static string GetValue(this IWebDriver driver, By by)
    {
        return driver.FindElement(by).GetAttribute("value");
    }

    public static void ClearInput(this IWebDriver driver, By by)
    {
        driver.FindElement(by).SendKeys(Keys.Control + "a" + Keys.Delete);
    }

    public static void ClearInput(this IWebDriver driver, IWebElement element)
    {
        element.SendKeys(Keys.Control + "a" + Keys.Delete);
    }

    public static void SendKeysInput(this IWebDriver driver, By by, string text, bool clearInput = true,
        By byToDropFocus = null)
    {
        driver.FindElement(by).Click();
        if (clearInput)
        {
            driver.ClearInput(by);
        }

        driver.FindElement(by).SendKeys(text);

        if (byToDropFocus is not null)
            driver.DropFocusBy(byToDropFocus);
    }

    #endregion

    #region Button

    public static IWebElement GetButton(this IWebDriver driver, EButtonType type, string name = null)
    {
        return driver.FindElement(By.CssSelector(string.Format("[data-e2e=\"btn{0}{1}\"]",
        type != EButtonType.Simple ? type.ToString() : string.Empty, name)));
    }

    #endregion

    #region Switch

    public static void SwitchOn(this IWebDriver driver, string dataId, string idSave = "")
    {
        if (driver.FindElement(By.CssSelector("[data-id=\"" + dataId + "\"] input")).GetAttribute("value") !=
            "false")
            return;

        driver.FindElement(By.CssSelector("[data-id=\"" + dataId + "\"] label")).Click();
        if (!string.IsNullOrEmpty(idSave))
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"" + idSave + "\"]")).Click();
        }
    }

    public static void SwitchOff(this IWebDriver driver, string dataId, string idSave = "")
    {
        if (driver.FindElement(By.CssSelector("[data-id=\"" + dataId + "\"] input")).GetAttribute("value") !=
            "true")
            return;

        driver.FindElement(By.CssSelector("[data-id=\"" + dataId + "\"] label")).Click();
        driver.ScrollToTop();
        if (!string.IsNullOrEmpty(idSave))
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"" + idSave + "\"]")).Click();
        }
    }

    #endregion

    #region Grid

    public static IWebElement GetGridCell(this IWebDriver driver, int row, string column, string gridUniqueId = "")
    {
        var cellBy = By.CssSelector($"[data-e2e-grid-cell=\"{"grid" + gridUniqueId}[{row}][\'{column}\']\"]");
        driver.WaitForElem(cellBy);
        return driver.FindElement(cellBy);
    }

    public static IWebElement GetGridCellElement(this IWebDriver driver, int row, string column,
        string gridUniqueId = "", By by = null)
    {
        var cellBy = By.CssSelector($"[data-e2e-grid-cell=\"{"grid" + gridUniqueId}[{row}][\'{column}\']\"]");
        driver.WaitForElem(cellBy);
        driver.MouseFocus(cellBy);
        return driver.FindElement(cellBy).FindElement(by);
    }

    [Obsolete("Use GetGridCellElement instead.")]
    public static IWebElement GetGridCellInputForm(this IWebDriver driver, int row, string column,
        string gridUniqueId = "", By by = null)
    {
        var cellBy = By.CssSelector($"[data-e2e-grid-cell=\"{"grid" + gridUniqueId}[{row}][\'{column}\']\"]");
        driver.WaitForElem(cellBy);
        driver.MouseFocus(cellBy);
        return driver.FindElement(cellBy).FindElement(By.Name("inputForm"));
    }

    public static string GetGridCellText(this IWebDriver driver, int row, string column, string gridUniqueId = "")
    {
        var grid = driver.GetGridCell(row, column, gridUniqueId);

        if (!string.IsNullOrEmpty(grid.Text) ||
            string.IsNullOrEmpty(grid.FindElement(By.ClassName("ui-grid-cell-contents")).GetAttribute("innerHTML")))
            return grid.Text;

        return grid.FindElements(By.TagName("input")).Count > 0
            ? grid.FindElement(By.TagName("input")).GetAttribute("value")
            : grid.FindElement(By.ClassName("ui-grid-custom-edit__text--default")).Text;
    }

    public static IWebElement GetGridFilter(this IWebDriver driver)
    {
        return driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"));
    }

        public static void GridFilterSendKeys(this IWebDriver driver, string text)
        {
            driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Click();
            driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
            driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys(text);
        }

    public static void GridFilterSendKeys(this IWebDriver driver, string text, By byToDropFocus)
    {
        GridFilterSendKeys(driver, text);

        byToDropFocus ??= By.ClassName("ui-grid-custom-filter-total");

        driver.DropFocusBy(byToDropFocus);
    }

    public static void GridFilterSendKeys(this IWebDriver driver, string text, string tagToDropFocus)
    {
        GridFilterSendKeys(driver, text);

        if (tagToDropFocus is not null)
            driver.DropFocus(tagToDropFocus);
    }

    public static void GridFilterModalSendKeys(this IWebDriver driver, string text)
    {
        driver.WaitForElem(AdvBy.DataE2E("gridFilterSearch"));
        GridFilterSendKeys(driver, text, byToDropFocus: By.ClassName("modal-header-title"));
    }

    public static void GetGridFilterTab(this IWebDriver driver, int tabIndex = 0, string text = "")
    {
        driver.FindElements(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"))[tabIndex].Click();
        driver.FindElements(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"))[tabIndex].Clear();
        driver.FindElements(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"))[tabIndex].SendKeys(text);

        driver.MouseFocus(By.ClassName("ui-grid-custom-filter-total"));
    }

    public static void GetGridIdFilter(this IWebDriver driver, string gridName = "", string text = "")
    {
        driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridName + "\"]"))
            .FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Click();
        driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridName + "\"]"))
            .FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).Clear();
        driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridName + "\"]"))
            .FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys(text);
    }

    public static void ClearGridCell(this IWebDriver driver, int row, string column, string gridUniqueId = "",
        string selector = ".ui-grid-custom-edit-field.form-control")
    {
        driver.ClearInput(driver.GetGridCell(row, column, gridUniqueId).FindElement(By.CssSelector(selector)));
    }

    public static void SelectGridRow(this IWebDriver driver, int row, string gridUniqueId = "")
    {
        driver.GetGridCell(row, "selectionRowHeaderCol", gridUniqueId)
            .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
    }

    public static void SetGridFilter(this IWebDriver driver, string baseUrl, string filterName, string filterValue,
        bool isSelect = false)
    {
        driver.ScrollToTop();
        Functions.GridFilterSet(driver, baseUrl, filterName);
        if (isSelect)
            driver.SetGridFilterSelectValue(filterName, filterValue);
        else
            driver.SetGridFilterValue(filterName, filterValue);
    }

    public static void SetGridFilterValue(this IWebDriver driver, string filterName, string filterValue)
    {
        var filterInput =
            driver.FindElement(By.CssSelector("[data-e2e-grid-filter-block-name=\"" + filterName + "\"] input"));
        filterInput.Click();
        filterInput.Clear();
        filterInput.SendKeys(filterValue);
        driver.DropFocus("h5");
    }

    public static void SetGridFilterSelectValue(this IWebDriver driver, string filterName, string filterValue)
    {
        var filterInput =
            driver.FindElement(By.CssSelector("[data-e2e-grid-filter-block-name=\"" + filterName + "\"]"));
        filterInput.FindElement(By.ClassName("ui-select-match")).Click();
        filterInput.FindElement(By.TagName("input")).Click();
        filterInput.FindElement(By.TagName("input")).Clear();
        filterInput.FindElement(By.TagName("input")).SendKeys(filterValue + Keys.Enter);
        driver.DropFocus("h5");
    }

    public static void SetGridFilterRange(this IWebDriver driver, string filterName, string filterValueFrom, string filterValueTo, string cssString = "h5")
    {
        var filterInput =
            driver.FindElements(By.CssSelector("[data-e2e-grid-filter-block-name=\"" + filterName + "\"] input"));
        filterInput[0].Click();
        filterInput[0].Clear();
        filterInput[0].SendKeys(filterValueFrom);
        driver.DropFocusCss(cssString);

        filterInput[1].Click();
        filterInput[1].Clear();
        filterInput[1].SendKeys(filterValueTo);
        driver.DropFocusCss(cssString);

        Thread.Sleep(100);
    }

    public static void SetGridAction(this IWebDriver driver, string action)
    {
        driver.GetByE2E("gridSelectionDropdownButton").Click();
        driver.FindElement(By.TagName("ui-grid-custom-selection"))
            .FindElement(By.XPath("//span[text() = '" + action + "']")).Click();
    }

    /// <summary>
    /// Фокус на ячейке, чтоб появилось текстовое поле, необязательная очистка поля, 
    /// ввод текста и последующий фокус на ToastMessage, чтоб изменения применились.
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="text"></param>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="gridUniqueId"></param>
    /// <param name="selector"></param>
    /// <param name="clearCell"></param>
    public static void SendKeysGridCell(this IWebDriver driver, string text, int row, string column,
        string gridUniqueId = "",
        string selector = ".ui-grid-custom-edit-field.form-control", 
        bool clearCell = true)
    {
        driver.MouseFocus(By.CssSelector(
        $"[data-e2e-grid-cell=\"{"grid" + gridUniqueId}[{row}][\'{column}\']\"]"));

        if (clearCell)
            driver.ClearGridCell(row, column, gridUniqueId, selector);

        driver.GetGridCell(row, column, gridUniqueId).FindElement(By.CssSelector(selector))
            .SendKeys(text + Keys.Enter);

        driver.ClearToastMessages();
    }

    public static void GridReturnDefaultView10(this IWebDriver driver, string baseUrl)
    {
        var selectElem = driver.FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
        var select = new SelectElement(selectElem);

        if (!select.SelectedOption.Text.Equals("10"))
        {
            driver.GridPaginationSelectItems("10");
            driver.ScrollToTop();
        }
    }

    public static void GridPaginationSelectItems(this IWebDriver driver, string num = "", string gridName = "")
    {
        driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));

        if (gridName != "")
        {
            new SelectElement(driver.FindElement(By.CssSelector("[grid-unique-id=\"" + gridName + "\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"))).SelectByValue(num);
        }
        else
        {
            new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")))
                .SelectByValue(num);
        }
    }

    public static bool CheckExpectedValuesInGridPaginationSelect(this IWebDriver driver)
    {
        SelectElement select = new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")));
        for (int i = 0; i < 4; i++)
            if ((select.Options[i].Text == i + "0") && (select.Options[i].GetAttribute("value") == i + "0"))
                return false;
        return true;
    }


    #endregion

    #region Ckeditor

    public static void SetCkHtml(this IWebDriver driver, string text)
    {
        driver.FindElement(By.CssSelector("a.cke_button__source")).Click();
        var codeMirror = driver.FindElement(By.CssSelector(".CodeMirror"));
        if (driver is not IJavaScriptExecutor js)
            return;

        js.ExecuteScript("arguments[0].CodeMirror.setValue('" + text + "');", codeMirror);
    }

    public static void SetCkText(this IWebDriver driver, string text, string textareaId)
    {
        IWebElement iframe = null;
        try
        {
            driver.WaitForElem(By.CssSelector("#cke_" + textareaId + " iframe"));
            iframe = driver.FindElement(By.CssSelector("#cke_" + textareaId + " iframe"));
        }
        catch (Exception)
        {
            // TakeScreenshot();
            //throw;
        }

        driver.SwitchTo().Frame(iframe);
        var body = driver.FindElement(By.TagName("body"));
        body.Clear();
        body.Click();
        body.SendKeys(text);
        driver.SwitchTo().DefaultContent();
    }

    public static void SetCkText(this IWebDriver driver, string text, By parentItem)
    {
        IWebElement iframe = null;
        try
        {
            driver.WaitForElem(By.CssSelector(".cke iframe"));
            iframe = driver.FindElement(parentItem).FindElement(By.CssSelector(".cke iframe"));
        }
        catch (Exception)
        {
            // TakeScreenshot();
            //throw;
        }

        driver.SwitchTo().Frame(iframe);
        var body = driver.FindElement(By.TagName("body"));
        body.Clear();
        body.SendKeys(text);
        driver.SwitchTo().DefaultContent();
    }

    public static void AssertCkText(this IWebDriver driver, string text, string textareaId)
    {
        driver.WaitForElem(By.CssSelector("#cke_" + textareaId + " iframe"));
        var iframe = driver.FindElement(By.CssSelector("#cke_" + textareaId + " iframe"));
        driver.SwitchTo().Frame(iframe);
        Assert.AreEqual(text, driver.FindElement(By.TagName("body")).Text);
        driver.SwitchTo().DefaultContent();
    }

    public static void AssertCkText(this IWebDriver driver, string text, By parentItem, int index)
    {
        driver.WaitForElem(By.CssSelector(".cke iframe"));
        var iframe = driver.FindElements(parentItem)[index].FindElement(By.CssSelector(".cke iframe"));
        driver.SwitchTo().Frame(iframe);
        Assert.AreEqual(text, driver.FindElement(By.TagName("body")).Text);
        driver.SwitchTo().DefaultContent();
    }

    #endregion

    #region CheckBox

    public static void CheckSelected(this IWebDriver driver, string id, string idSave)
    {
        if (driver.FindElement(By.Id(id)).Selected) return;

        driver.FindElement(By.CssSelector("[data-e2e=\"" + id + "\"]")).Click();
        driver.ScrollToTop();
        driver.FindElement(By.CssSelector("[data-e2e=\"" + idSave + "\"]")).Click();
    }

    public static void CheckNotSelected(this IWebDriver driver, string id, string idSave)
    {
        if (!driver.FindElement(By.Id(id)).Selected) return;

        driver.FindElement(By.CssSelector("[data-e2e=\"" + id + "\"]")).Click();
        driver.ScrollToTop();
        driver.FindElement(By.CssSelector("[data-e2e=\"" + idSave + "\"]")).Click();
    }


    public static void CheckBoxCheck(this IWebDriver driver, string selector, string selectorType = "Id")
    {
        switch (selectorType)
        {
            case "Id":
                if (driver.FindElement(By.Id(selector)).GetAttribute("class")
                        .IndexOf("ng-not-empty", StringComparison.Ordinal) == -1)
                    driver.FindElement(By.CssSelector("[id=\"" + selector + "\"]~.adv-checkbox-emul")).Click();
                break;
            case "CssSelector":
                if (driver.FindElement(By.CssSelector(selector + " input[type=\"checkbox\"]")).GetAttribute("class")
                        .IndexOf("ng-not-empty", StringComparison.Ordinal) == -1)
                    driver.FindElement(By.CssSelector(selector + " input[type=\"checkbox\"]~.adv-checkbox-emul"))
                        .Click();
                break;
        }
    }

    public static void CheckBoxUncheck(this IWebDriver driver, string selector, string selectorType = "Id")
    {
        switch (selectorType)
        {
            case "Id":
                if (driver.FindElement(By.Id(selector)).GetAttribute("class")
                        .IndexOf("ng-not-empty", StringComparison.Ordinal) != -1)
                    driver.FindElement(By.CssSelector("[id=\"" + selector + "\"]~.adv-checkbox-emul")).Click();
                break;
            case "CssSelector":
                if (driver.FindElement(By.CssSelector(selector + " input[type=\"checkbox\"]")).GetAttribute("class")
                        .IndexOf("ng-not-empty", StringComparison.Ordinal) != -1)
                    driver.FindElement(By.CssSelector(selector + " input[type=\"checkbox\"]~.adv-checkbox-emul"))
                        .Click();
                break;
        }
    }

    public static bool CheckBoxChecked(this IWebDriver driver, By by)
    {
        return driver.FindElement(by).GetAttribute("class").Contains("ng-not-empty");
    }

    #endregion

    #region Kanban

    public static IWebElement GetKanbanCard(this IWebDriver driver, int column, int row, string elem = "Number")
    {
        return driver.FindElement(By.CssSelector($"[data-columnindex=\"{column}\"]"))
            .FindElement(By.CssSelector($"[data-e2e-cell=\"{row}\"]"))
            .FindElement(By.CssSelector($"[data-e2e=\"{elem}\"]"));
    }

    #endregion

    #region Toaster

    public static void ClearToastMessages(this IWebDriver driver)
    {
        var closeButtons = driver.FindElements(By.ClassName("toast-close-button"));
        if (closeButtons.Count == 0) return;

        foreach (var btn in closeButtons)
        {
            try
            {
                btn.Click();
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }

    #endregion

    #region Swal

    public static void SwalConfirm(this IWebDriver driver)
    {
        driver.WaitForElem(By.ClassName("swal2-container"), TimeSpan.FromSeconds(45));
        driver.FindElement(By.ClassName("swal2-confirm")).Click();
    }

    public static void SwalCancel(this IWebDriver driver)
    {
        driver.WaitForElem(By.ClassName("swal2-container"), TimeSpan.FromSeconds(45));
        driver.FindElement(By.ClassName("swal2-cancel")).Click();
    }

    #endregion
}