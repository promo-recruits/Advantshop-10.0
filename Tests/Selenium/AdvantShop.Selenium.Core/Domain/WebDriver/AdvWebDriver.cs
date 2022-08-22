using System.Collections.ObjectModel;

namespace AdvantShop.Selenium.Core.Domain.WebDriver;

/// <summary>
/// Provides a mechanism of compilation waiting functions AngularJS
/// </summary>
public class AdvWebDriver : IWebDriver, IWrapsDriver, IJavaScriptExecutor, ITakesScreenshot
{
    private const string AngularDeferBootstrap = "NG_DEFER_BOOTSTRAP!";

    private readonly IWebDriver _driver;
    private readonly IJavaScriptExecutor _jsExecutor;
    private readonly string _rootElement;
    private readonly string _screenshotPath;
    private readonly string _baseUrl;

    /// <summary>
    /// Creates a new instance of <see cref="AdvWebDriver"/> by wrapping a <see cref="IWebDriver"/> instance.
    /// </summary>
    /// <param name="driver">The configured webdriver instance.</param>
    /// <param name="rootElement">The CSS selector for an element on which to find Angular.</param>
    /// <param name="screenshotPath">Directory for screenshots.</param>
    /// <param name="baseUrl">Base url of website</param>
    /// <exception cref="NotSupportedException">Thrown when the WebDriver instance does not implement the IJavaScriptExecutor interface.</exception>
    public AdvWebDriver(IWebDriver driver, string rootElement, string screenshotPath, string baseUrl = null)
    {
        if (driver is not IJavaScriptExecutor executor)
        {
            throw new NotSupportedException(
            "The WebDriver instance must implement the IJavaScriptExecutor interface.");
        }

        _driver = driver;
        _jsExecutor = executor;
        _rootElement = rootElement;
        _screenshotPath = screenshotPath;
        _baseUrl = baseUrl;
    }

    private void WaitForAngular()
    {
        try
        {
            var wait = new WebDriverWait(new SystemClock(), _driver, TimeSpan.FromSeconds(10),
            TimeSpan.FromMilliseconds(200));

            _ = wait.Until(d =>
                (bool) ((d as IJavaScriptExecutor)?.ExecuteScript(ClientSideScripts.WaitForAngular, _rootElement) ??
                        false));
        }
        catch (Exception)
        {
            //ignore
        }
    }

    #region IWebDriver implementation

    /// <summary>
    /// Gets or sets the URL the browser is currently displaying.
    /// </summary>
    public string Url
    {
        get
        {
            WaitForAngular();
            return _driver.Url;
        }
        set
        {
            _driver.Url = "about:blank";

            var hcDriver = _driver as IHasCapabilities;
            string browserName = null;
            if (hcDriver != null && hcDriver.Capabilities.HasCapability("browserName"))
            {
                browserName = hcDriver.Capabilities.GetCapability("browserName").ToString();
            }

            if (browserName?.ToLower() is "internet explorer" or "microsoftedge" or "phantomjs" or "firefox" or "safari")
            {
                ExecuteScript("window.name += '" + AngularDeferBootstrap + "';");
                _driver.Url = value;
            }
            else
            {
                ExecuteScript("window.name += '" + AngularDeferBootstrap + "'; window.location.href = '" +
                              value + "';");
            }
        }
    }

    /// <summary>
    /// Gets the title of the current browser window.
    /// </summary>
    public string Title
    {
        get
        {
            WaitForAngular();
            return _driver.Title;
        }
    }

    /// <summary>
    /// Gets the source of the page last loaded by the browser.
    /// </summary>
    public string PageSource
    {
        get
        {
            WaitForAngular();
            return _driver.PageSource;
        }
    }

    /// <summary>
    /// Gets the current window handle, which is an opaque handle to this 
    /// window that uniquely identifies it within this driver instance.
    /// </summary>
    public string CurrentWindowHandle => _driver.CurrentWindowHandle;

    /// <summary>
    /// Gets the window handles of open browser windows.
    /// </summary>
    public ReadOnlyCollection<string> WindowHandles => _driver.WindowHandles;

    /// <summary>
    /// Close the current window, quitting the browser if it is the last window currently open.
    /// </summary>
    public void Close()
    {
        _driver.Close();
    }

    /// <summary>
    /// Quits this driver, closing every associated window.
    /// </summary>
    public void Quit()
    {
        _driver.Quit();
    }

    /// <summary>
    /// Instructs the driver to change its settings.
    /// </summary>
    /// <returns>
    /// An <see cref="IOptions"/> object allowing the user to change the settings of the driver.
    /// </returns>
    public IOptions Manage()
    {
        return _driver.Manage();
    }

    public INavigation Navigate()
    {
        WaitForAngular();
        return _driver.Navigate();
    }

    /// <summary>
    /// Instructs the driver to send future commands to a different frame or window.
    /// </summary>
    /// <returns>
    /// An <see cref="ITargetLocator"/> object which can be used to select a frame or window.
    /// </returns>
    public ITargetLocator SwitchTo()
    {
        return _driver.SwitchTo();
    }

    #region ISearchContext

    /// <summary>
    /// Finds the first <see cref="IWebElement"/> using the given mechanism. 
    /// </summary>
    /// <param name="by">The locating mechanism to use.</param>
    /// <returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
    public IWebElement FindElement(By by)
    {
        WaitForAngular();
        try
        {
            return _driver.FindElement(by);
        }
        catch (NoSuchElementException)
        {
            TakeScreenshot(description: "no such element:" + by.Criteria);
            throw;
        }
        catch (Exception)
        {
            TakeScreenshot("unhandled exception");
            throw;
        }
    }

    /// <summary>
    /// Finds all <see cref="IWebElement"/>s within the current context 
    /// using the given mechanism.
    /// </summary>
    /// <param name="by">The locating mechanism to use.</param>
    /// <returns>
    /// A <see cref="ReadOnlyCollection{T}"/> of all <see cref="IWebElement"/>s 
    /// matching the current criteria, or an empty list if nothing matches.
    /// </returns>
    public ReadOnlyCollection<IWebElement> FindElements(By @by)
    {
        WaitForAngular();
        return new ReadOnlyCollection<IWebElement>(_driver.FindElements(by));
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, 
    /// releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _driver.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion

    #endregion

    #region IWrapsDriver implementation

    public IWebDriver WrappedDriver => _driver;

    #endregion

    #region IJavaScriptExecutor implementation

    /// <summary>
    /// Executes JavaScript in the context of the currently selected frame or window.
    /// </summary>
    /// <param name="script">The JavaScript code to execute.</param>
    /// <param name="args">The arguments to the script.</param>
    /// <returns>The value returned by the script.</returns>
    /// <remarks>
    ///   <para>
    ///     The <see cref="M:OpenQA.Selenium.IJavaScriptExecutor.ExecuteScript(System.String,System.Object[])" /> method executes JavaScript in the context of
    ///     the currently selected frame or window. This means that "document" will refer
    ///     to the current document. If the script has a return value, then the following
    ///     steps will be taken:
    ///   </para>
    ///   <para>
    ///     <list type="bullet">
    ///       <item>
    ///         <description>For an HTML element, this method returns a <see cref="T:OpenQA.Selenium.IWebElement" /></description>
    ///       </item>
    ///       <item>
    ///         <description>For a number, a <see cref="T:System.Int64" /> is returned</description>
    ///       </item>
    ///       <item>
    ///         <description>For a boolean, a <see cref="T:System.Boolean" /> is returned</description>
    ///       </item>
    ///       <item>
    ///         <description>For all other cases a <see cref="T:System.String" /> is returned.</description>
    ///       </item>
    ///       <item>
    ///         <description>
    ///           For an array, we check the first element, and attempt to return a
    ///           <see cref="T:System.Collections.Generic.List`1" /> of that type, following the rules above. 
    ///           Nested lists are not supported.
    ///         </description>
    ///       </item>
    ///       <item>
    ///         <description>If the value is null or there is no return value, <see langword="null" /> is returned.</description>
    ///       </item>
    ///     </list>
    ///   </para>
    ///   <para>
    ///     Arguments must be a number (which will be converted to a <see cref="T:System.Int64" />),
    ///     a <see cref="T:System.Boolean" />, a <see cref="T:System.String" /> or a <see cref="T:OpenQA.Selenium.IWebElement" />.
    ///     An exception will be thrown if the arguments do not meet these criteria.
    ///     The arguments will be made available to the JavaScript via the "arguments" magic
    ///     variable, as if the function were called via "Function.apply"
    ///   </para>
    /// </remarks>
    public object ExecuteScript(string script, params object[] args)
    {
        return _jsExecutor.ExecuteScript(script, args);
    }

    public object ExecuteScript(PinnedScript script, params object[] args)
    {
        return _jsExecutor.ExecuteScript(script, args);
    }

    /// <summary>
    /// Executes JavaScript asynchronously in the context of the currently selected frame or window.
    /// </summary>
    /// <param name="script">The JavaScript code to execute.</param>
    /// <param name="args">The arguments to the script.</param>
    /// <returns>The value returned by the script.</returns>
    public object ExecuteAsyncScript(string script, params object[] args)
    {
        return _jsExecutor.ExecuteAsyncScript(script, args);
    }

    #endregion

    public Screenshot GetScreenshot()
    {
        return (_driver as ITakesScreenshot)?.GetScreenshot();
    }

    /// <summary>
    /// Take screenshot, needed for detecting errors
    /// </summary>
    /// <param name="testName">name of current test</param>
    /// <param name="description">description of screenshot</param>
    public void TakeScreenshot(string testName = "", string description = "")
    {
        if (DontTakeScreenshot)
        {
            DontTakeScreenshot = false;
            return;
        }

        var screenshotDriver = _driver as ITakesScreenshot;
        var screenshot = screenshotDriver?.GetScreenshot();

        if (screenshot is null)
            return;

        var dirPath = Directory.GetDirectories(_screenshotPath).LastOrDefault();
        if (dirPath is null)
        {
            dirPath = _screenshotPath + "temp";
            Directory.CreateDirectory(dirPath);
        }

        dirPath += "\\";

        var cleanDesc = CleanString(description);

        var cleanUrl = _baseUrl is not null
            ? CleanString(_driver.Url.Replace(_baseUrl, ""))
            : string.Empty;

        var cleanTestName = CleanString(testName);
            
        var url = dirPath + cleanTestName + "_" + cleanDesc + "_" + cleanUrl;
            
        var filePath = url.Length > 100 ? url[..99] : url;

        var file = filePath + ".png";

        var i = 1;

        while (File.Exists(file)) file = filePath + "_" + i++ + ".png";

        screenshot.SaveAsFile(file, ScreenshotImageFormat.Png);
    }

    private bool DontTakeScreenshot { get; set; }

    public void DontTakeScreenshotOneTime() => DontTakeScreenshot = true;

    private static string CleanString(string value)
    {
        return value.Replace("/", "-")
            .Replace("?", "-")
            .Replace(":", "$")
            .Replace("{", "")
            .Replace("\"", "'")
            .Replace("\\", "")
            .Replace("*", "")
            .Trim('-');
    }
}