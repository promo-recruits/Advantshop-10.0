namespace AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;

public static class ScrollHelper
{
    public static void ScrollTo(this IWebDriver driver, By by)
    {
        var element = driver.FindElement(by);
        if (driver is not IJavaScriptExecutor jse)
            return;

        jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);
    }

    public static void ScrollTo(this IWebDriver driver, By by, int index)
    {
        var element = driver.FindElements(by)[index];
        if (driver is not IJavaScriptExecutor jse)
            return;

        jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);
    }

    public static void ScrollToTop(this IWebDriver driver)
    {
        (driver as IJavaScriptExecutor)?.ExecuteScript("window.scrollTo(0, -document.body.scrollHeight)");
    }

    public static void ScrollToUISelect(this IWebDriver driver, string selectOption)
    {
        driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
        var element = driver.FindElement(By.CssSelector("[data-e2e=\"" + selectOption + "\"]"));
        if (driver is not IJavaScriptExecutor jse)
            return;

        jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);
    }
}