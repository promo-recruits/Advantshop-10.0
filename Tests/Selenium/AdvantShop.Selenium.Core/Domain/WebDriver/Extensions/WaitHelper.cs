namespace AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;

public static class WaitHelper
{
    private static WebDriverWait CreateWebDriverWait(IWebDriver driver, TimeSpan? span = null)
    {
        span ??= TimeSpan.FromSeconds(30);

        var wait = new WebDriverWait(driver, span.Value);
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait;
    }

    public static void WaitForElem(this IWebDriver driver, By by)
    {
        WaitForElem(driver, by, null);
    }

    public static void WaitForElem(this IWebDriver driver, By by, TimeSpan? span)
    {
        var wait = CreateWebDriverWait(driver, span);
        _ = wait.Until(d => d.FindElement(by));
    }

    [Obsolete("Now AdvWebDriver wait http after FindElement.")]
    public static void WaitForAjax(this IWebDriver driver)
    {
        var wait = CreateWebDriverWait(driver);
        _ = wait.Until(d =>
            (bool)((d as IJavaScriptExecutor)?.ExecuteScript("return window.ajaxIsComplete();") ?? false));
    }

    public static void WaitForElemEnabled(this IWebDriver driver, By by)
    {
        while (!driver.FindElement(by).Enabled)
            Thread.Sleep(100);
    }

    public static void WaitForElemDisplayedAndClick(this IWebDriver driver, By by, int index = 0)
    {
        var isDisplayed = false;
        var counter = 0;

        while (!isDisplayed && counter < 10)
            try
            {
                Thread.Sleep(100);
                counter++;
                driver.FindElements(by)[index].Click();
                isDisplayed = true;
            }
            catch (Exception)
            {
                // ignored
            }

        if (counter >= 10) throw new Exception("Elem not displayed more 10 second");
    }

    public static void WaitForToastSuccess(this IWebDriver driver)
    {
        var wait = CreateWebDriverWait(driver, TimeSpan.FromSeconds(10));
        _ = wait.Until(d => d.FindElement(By.ClassName("toast-success")));
    }

    public static void WaitForToastError(this IWebDriver driver)
    {
        var wait = CreateWebDriverWait(driver, TimeSpan.FromSeconds(10));
        _ = wait.Until(d => d.FindElement(By.ClassName("toast-error")));
    }

    public static void WaitForModal(this IWebDriver driver)
    {
        var wait = CreateWebDriverWait(driver, TimeSpan.FromSeconds(10));
        _ = wait.Until(d => d.FindElement(By.ClassName("modal-dialog")));
    }

    public static void WaitForModalClose(this IWebDriver driver)
    {
        var wait = CreateWebDriverWait(driver, TimeSpan.FromSeconds(10));
        _ = wait.Until(d => {
            (driver as AdvWebDriver)?.DontTakeScreenshotOneTime();
            return d.FindElement(By.ClassName("modal-dialog"));
        });
    }

    public static void WaitForElems(this IWebDriver driver, By by, int index = 0)
    {
        //Thread.Sleep(200);
        var wait = CreateWebDriverWait(driver, TimeSpan.FromSeconds(10));
        _ = wait.Until(d => {
            var elems = d.FindElements(by);
            return elems is { Count: > 0 } && (index == 0 || elems[index] != null);
        });
    }
}