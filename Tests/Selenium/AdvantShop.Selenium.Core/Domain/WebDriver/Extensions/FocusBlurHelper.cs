namespace AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;

public static class FocusBlurHelper
{
    public static void MouseFocus(this IWebDriver driver, By by)
    {
        var action = new Actions(driver);
        var elem = driver.FindElement(by);
        action.MoveToElement(elem);
        action.Perform();
    }

    public static void MouseFocus(this IWebDriver driver, By by, int index)
    {
        var action = new Actions(driver);
        var elem = driver.FindElements(by)[index];
        action.MoveToElement(elem);
        action.Perform();
    }

    public static void MouseFocus(this IWebDriver driver, IWebElement elem)
    {
        var action = new Actions(driver);
        action.MoveToElement(elem);
        action.Perform();
    }

    public static void DropFocus(this IWebDriver driver, string tag)
    {
        driver.FindElement(By.TagName(tag)).Click();
    }

    public static void DropFocusCss(this IWebDriver driver, string selector)
    {
        driver.FindElement(By.CssSelector(selector)).Click();
    }

    public static void DropFocusBy(this IWebDriver driver, By by)
    {
        driver.FindElement(by).Click();
    }

    public static void Blur(this IWebDriver driver)
    {
        (driver as IJavaScriptExecutor)?.ExecuteScript(
        "!!document.activeElement ? document.activeElement.blur() : 0");
    }
}
