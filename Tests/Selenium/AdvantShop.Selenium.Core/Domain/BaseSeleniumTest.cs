using System.Collections.ObjectModel;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using AdvantShop.Selenium.Core.Domain.WebDriver;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;

namespace AdvantShop.Selenium.Core.Domain;

public class BaseSeleniumTest
{
    private const string AdminCustomerId = "cfc2c33b-1e84-415e-8482-e98156341603";

    private const string AdminAuthToken =
        "E17F09E760D38AB10A7FDBABCD770C1049B17D570C4F54F14BCDA5B28EFC79191D3FA6B3D94D6526CD290C962BACAA444A7A7AA2D64111D461BF520922BF43D55CA10CB5AB0002D405AB5388D8CB3B88BB83B56556361D65C50A17AD64DCCB1BC5F4AB6073F6203FB18DA1E9F97B9DCBAC811B089704E18CAA05FC742ABD3E674B385135";

    private string _baseScreenshotsPath;
    private string _logFilePath = "";
    private string _verificationErrors = "";

    protected IWebDriver Driver;
    protected string BaseUrl = "";
    protected string TestName = "";
        
    private static readonly HttpClient HttpClient = new ();


    protected void Debug(string message)
    {
        if (string.IsNullOrWhiteSpace(_logFilePath)) return;

        var dirParent = Directory.GetParent(_logFilePath);
        if (dirParent is { Exists: false })
            Directory.CreateDirectory(dirParent.FullName);

        using TextWriter tw = new StreamWriter(_logFilePath, true);
        tw.WriteLine($"{DateTime.Now:yyyy.MM.dd HH:mm:ss}: {message}");
    }

    protected void IsElementNotPresent(By by, string attr, int index = 0)
    {
        try
        {
            var a = Driver.FindElements(by)[index].GetAttribute(attr);
            Assert.IsTrue(a.Equals(null));
        }
        catch (NullReferenceException)
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    protected void EnableInplaceOn()
    {
        GoToAdmin("settingstemplate");
        if (!Driver.FindElement(By.Id("EnableInplace")).Selected)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"EnableInplace\"]")).Click();

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
        }
    }

    protected void EnableInplaceOff()
    {
        GoToAdmin("settingstemplate");
        if (Driver.FindElement(By.Id("EnableInplace")).Selected)
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"EnableInplace\"]")).Click();
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
        }
    }

    protected void ChangeSidebarState(bool rollUp = true)
    {
        if (!Driver.Url.Contains("adminv3")) GoToAdmin();
        var classList = Driver.FindElement(By.CssSelector(".sidebar")).GetAttribute("class");
        if (classList.Contains("sidebar--default") && rollUp ||
            classList.Contains("sidebar--compact") && rollUp == false)
            Driver.FindElement(By.CssSelector(".top-panel__item .burger")).Click();
    }

    protected void ReindexSearch()
    {
        GoToAdmin("settingscatalog#?catalogTab=search");
        Driver.FindElement(By.LinkText("Обновить индекс поиска")).Click();
    }

    protected void ReCalc()
    {
        GoToAdmin("catalog");
        Driver.FindElement(By.TagName("recalc-trigger")).Click();
    }

    protected void UnHide(IWebElement element)
    {
        const string script =
            @"arguments[0].parentNode.style.visibility = 'visible';arguments[0].parentNode.height='auto'; arguments[0].parentNode.width='auto'; ";
        (Driver as IJavaScriptExecutor)?.ExecuteScript(script, element);
    }

    protected void AttachFile(By locator, string file)
    {
        var input = Driver.FindElement(locator);
        UnHide(input);
        input.SendKeys(file);
    }

    [OneTimeTearDown]
    protected void TeardownTest()
    {
        try
        {
            Driver?.Quit();
        }
        catch (Exception)
        {
            Debug("OneTimeTearDown");
        }
    }

    #region Init

    protected void Init(bool isStoreActive = true)
    {
        //Был баг: когда после init сразу переходишь в клиентку на главную, скрипты ложатся. 
        //Связано с тем, что для применения "логина" необходимо открыть другую страницу, а gotoclient() 
        //не осуществляет полноценный переход. 
        //Решение: ClearCache после логина, проблема уходит, но на -2+4 строчки кода больше.
        Debug("begin test");
        InitializeService.BaseInitial(isStoreActive, HttpClient);
        try
        {
            InitializeService.InitBrowser(out Driver, out BaseUrl, out _baseScreenshotsPath, out _logFilePath); //admin
        }
        catch (Exception ex)
        {
            Driver?.Quit();
            Assert.Fail($"can't init browser: {ex.Message}");
        }
        Functions.LogCustomer(Driver, BaseUrl, AdminCustomerId, AdminAuthToken);
        InitializeService.ClearCache(HttpClient); 
    }

    protected void ReInit(bool useAdvDriver = true, bool useTrial = false)
    {
        Driver.Quit();
        try
        {
            if (!useTrial)
            {
                InitializeService.InitBrowser(out Driver, out BaseUrl, out _baseScreenshotsPath, out _logFilePath,
                useAdvDriver); //admin
                Functions.LogCustomer(Driver, BaseUrl, AdminCustomerId, AdminAuthToken);
            }
            else
            {
                InitializeService.InitTrialBrowser(Functions.GetCustomSiteUrl(), out Driver, out BaseUrl,
                out _baseScreenshotsPath, out _logFilePath); //admin
                Functions.LogCustomer(Driver, BaseUrl, Functions.GetEnvironmentVariable("CUSTOMER_ID"),
                Functions.GetEnvironmentVariable("AUTH"));
            }
            InitializeService.ClearCache(HttpClient);
        }
        catch
        {
            Driver?.Quit();
            Assert.Fail("can't init browser");
        }
    }

    protected void ReInitClient(bool useAdvDriver = true, bool useTrial = false)
    {
        Driver.Quit();
        try
        {
            if (!useTrial)
                InitializeService.InitBrowser(out Driver, out BaseUrl, out _baseScreenshotsPath, out _logFilePath,
                useAdvDriver);
            else
                InitializeService.InitTrialBrowser(Functions.GetCustomSiteUrl(), out Driver, out BaseUrl,
                out _baseScreenshotsPath, out _logFilePath); //admin
        }
        catch
        {
            Driver?.Quit();
            Assert.Fail("can't init browser");
        }
        InitializeService.ClearCache(HttpClient);
    }

    protected void InitTrial()
    {
        //Email: "testmailimap@yandex.ru"
        //Pass: "ewqEWQ321#@!"
        //Auth: "490E53CD1440EAF4376D9D60C170F1F38ED2095D580A0F25A97640E50D519D151201031857988DCE85E82396B67C8AE2E717761407754E41BA809E74A35D2CF180D1632E865913940B29A6C4FBA2043C55BF30C297CA6A73D0328E2591A0416DA86964902F36708E36C6F25FED11828D7D440BF86AB15D878EDF0ED77807950D787AD572C20272D79072284667ECC1ADCE8BFE342D1E2A734DB889E31A8F361BE2D96DDE"
        //Customer_id: "42b70289-465f-4955-b72c-046c1c815522"

        Debug("begin test");
        InitializeService.BaseInitialTrial(HttpClient);
        try
        {
            InitializeService.InitTrialBrowser(Functions.GetCustomSiteUrl(), out Driver, out BaseUrl,
            out _baseScreenshotsPath, out _logFilePath); //admin
        }
        catch
        {
            Driver?.Quit();
            Assert.Fail("can't init browser");
        }
        Functions.LogCustomer(Driver, BaseUrl, Functions.GetEnvironmentVariable("CUSTOMER_ID"),
        Functions.GetEnvironmentVariable("AUTH"));
        InitializeService.ClearCache(HttpClient);
    }

    #endregion

    #region GoTo and Refresh

    private void GoTo(string url = "", bool noRefresh = false)
    {
        var needRefresh = false;
        const string pattern = "\\?|#\\?";
        if (!noRefresh && Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase))
        {
            var currUrl = Regex.Split(Driver.Url, pattern).FirstOrDefault() ?? string.Empty;
            var newUrl = Regex.Split(url, pattern).FirstOrDefault() ?? string.Empty;
            if (string.Equals(currUrl, newUrl))
                needRefresh = true;
        }

        Driver.Navigate().GoToUrl(url);

        if (needRefresh)
            Refresh();
    }

    protected void Refresh()
    {
        Driver.Navigate().Refresh();
    }

    protected void GoBack()
    {
        Driver.Navigate().Back();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="repetitionCounter">Параметр нужен для ситуации, когда в Url прописываются атрибуты.
    /// </param>
    protected void GoBack(int repetitionCounter)
    {
        for (int i = 0; i < repetitionCounter; i++)
            GoBack();
    }

    protected void GoToAdmin(string url = "", bool noRefresh = false)
    {
        GoTo(BaseUrl.Trim('/') + "/adminv3/" + url.Trim('/'), noRefresh: noRefresh);
    }

    protected void GoToMobile(string url = "", bool fromDesktop = false, bool noRefresh = false)
    {
        Driver.Manage().Window.Size = new Size(414, 700);
        if (fromDesktop)
        {
            Driver.FindElements(By.CssSelector(".device-panel__mobile-direction .device-panel__btn"))[0].Click();
        }
        GoTo(BaseUrl.Trim('/') + "/" + url.Trim('/') + "/?deviceMode=mobile", noRefresh: noRefresh);
    }

    protected void GoToClient(string url = "", bool noRefresh = false)
    {
        GoTo(BaseUrl.Trim('/') + "/" + url.Trim('/'), noRefresh: noRefresh);
    }

    protected void GoToModule(string moduleName, bool noRefresh = false)
    {
        GoTo(BaseUrl.Trim('/') + "/adminv3/modules/details/" + moduleName, noRefresh: noRefresh);
    }

    #endregion

    #region Verify

    protected void VerifyAreEqual(int what, int where, string description = "")
    {
        try
        {
            Assert.AreEqual(what, where);
        }
        catch (AssertionException e)
        {
            VerifyAddErrors(description, RemoveDuplicatesInErrorMessage(e.Message));
        }
    }

    protected void VerifyAreEqual(string what = "", string where = "", string description = "")
    {
        try
        {
            Assert.AreEqual(what, where);
        }
        catch (AssertionException e)
        {
            VerifyAddErrors(description, RemoveDuplicatesInErrorMessage(e.Message));
        }
    }

    protected void VerifyAreNotEqual(int what, int where, string description = "")
    {
        try
        {
            Assert.AreNotEqual(what, where);
        }
        catch (AssertionException e)
        {
            VerifyAddErrors(description, RemoveDuplicatesInErrorMessage(e.Message));
        }
    }

    protected void VerifyAreNotEqual(string what = "", string where = "", string description = "")
    {
        try
        {
            Assert.AreNotEqual(what, where);
        }
        catch (AssertionException e)
        {
            VerifyAddErrors(description, RemoveDuplicatesInErrorMessage(e.Message));
        }
    }

    protected void VerifyIsTrue(bool what = false, string description = "", bool showeMessage = true)
    {
        try
        {
            Assert.IsTrue(what);
        }
        catch (AssertionException e)
        {
            VerifyAddErrors(description, RemoveDuplicatesInErrorMessage(e.Message));
        }
    }

    protected void VerifyIsFalse(bool what = false, string description = "")
    {
        try
        {
            Assert.IsFalse(what);
        }
        catch (AssertionException e)
        {
            VerifyAddErrors(description, RemoveDuplicatesInErrorMessage(e.Message));
        }
    }

    protected void VerifyIsNull(object what, string description = "")
    {
        try
        {
            Assert.IsNull(what);
        }
        catch (AssertionException e)
        {
            VerifyAddErrors(description, RemoveDuplicatesInErrorMessage(e.Message));
        }
    }

    protected void VerifyIsNotNull(object what, string description = "")
    {
        try
        {
            Assert.IsNotNull(what);
        }
        catch (AssertionException e)
        {
            VerifyAddErrors(description, RemoveDuplicatesInErrorMessage(e.Message));
        }
    }

    protected void VerifyAddErrors(string description = "", string eMessage = "")
    {
        _verificationErrors += Environment.NewLine + description + eMessage;
        Console.Error.WriteLine(Environment.NewLine + description + eMessage);
        Debug(description + eMessage);
        (Driver as AdvWebDriver)?.TakeScreenshot(TestName, description);
    }

    protected void VerifyAddMessage(string description = "")
    {
        Console.Error.WriteLine(description);
        Debug(description);
    }

    protected void VerifyBegin(string testName = "")
    {
        _verificationErrors = testName;
        Console.Error.WriteLine(testName);
        Debug(testName);
    }

    protected void VerifyFinally(string testName = "")
    {
        Assert.AreEqual(testName, _verificationErrors);
        Console.Error.WriteLine("-pass");
        Debug(testName + " finished");
    }

    protected string RemoveDuplicatesInErrorMessage(string errorMessage)
    {
        return errorMessage.Substring(errorMessage.LastIndexOf("\r\n\r\n  ", StringComparison.Ordinal) + 1)
            .Replace("\n\r\n  ", " ")
            .Replace("\r\n\r\n", "\r\n");
    }

    #endregion

    #region PageStatus

    protected bool Is404Page(string url)
    {
        return GetPageStatus(url, false) == HttpStatusCode.NotFound;
    }

    protected HttpStatusCode GetPageStatus(string url, bool isAdmin = false)
    {
        var baseurl = url.Contains("http://") || url.Contains("https://")
            ? url
            : BaseUrl.Trim('/') + "/" + url;
        try
        {
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(baseurl));

            if (isAdmin)
                message.Headers.Add("Cookie", "Advantshop.AUTH=E17F09E760D38AB10A7FDBABCD770C1049B17D570C4F54F14BCDA5B28EFC79191D3FA6B3D94D6526CD290C962BACAA444A7A7AA2D64111D461BF520922BF43D55CA10CB5AB0002D405AB5388D8CB3B88BB83B56556361D65C50A17AD64DCCB1BC5F4AB6073F6203FB18DA1E9F97B9DCBAC811B089704E18CAA05FC742ABD3E674B385135");

            var result = HttpClient.Send(message);
            return result.StatusCode;
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode ?? HttpStatusCode.OK;
        }
    }

    #endregion

    #region ConsoleLog

    protected bool MessageIsInDefaultList(string consoleMessage)
    {
        // ReSharper disable once CollectionNeverUpdated.Local, RedundantEmptyObjectOrCollectionInitializer
        var defaultErrorsFull = new List<string>()
        {
            //"http://widgets.twimg.com/j/2/widget.js - Failed to load resource: net::ERR_NAME_NOT_RESOLVED",
            //baseURL + "/images/ajax-loader.gif - Failed to load resource: the server responded with a status of 404 (Not Found)",
            //baseURL + "/scripts/_common/share42/share42.js?2 60:3 Uncaught ReferenceError: jQuery is not defined"
        };
        var defaultErrorsPartial = new List<string>
        {
            "https://livechatv2.chat2desk.com/messages"
        };

        return defaultErrorsFull.Contains(consoleMessage) ||
               defaultErrorsPartial.Any(error => consoleMessage.IndexOf(error, StringComparison.Ordinal) != -1);
    }

    protected bool GetConsoleLog(string message)
    {
        //NOTE: метод работает только на Selenium до 3.11.0.
        var error = false;

        var entries = Driver.Manage().Logs.GetLog(LogType.Browser);
        if (entries.Any())
            foreach (var enter in entries)
                if (!_verificationErrors.Contains(enter.Message) && !MessageIsInDefaultList(enter.Message))
                {
                    _verificationErrors += Environment.NewLine + message + " " + enter.Message;
                    Console.Error.WriteLine(message + " " + enter.Message);
                    Debug(message + " " + enter.Message);
                    error = true;
                }

        return error;
    }

    /// <summary>
    ///     return console log list or null. Does not work for Firefox https://github.com/SeleniumHQ/selenium/issues/1161
    /// </summary>
    /// <param name="ignoreDefaultErrors">the default erros list includes logo and favicon errors</param>
    /// <returns></returns>
    protected ReadOnlyCollection<LogEntry> CheckConsoleLog(bool ignoreDefaultErrors = false)
    {
        var entries = Driver.Manage().Logs.GetLog(LogType.Browser);
        if (entries.Any())
        {
            if (ignoreDefaultErrors)
            {
                if (entries.Any(enter => !MessageIsInDefaultList(enter.Message.ToString()))) return entries;
            }
            else
            {
                return entries;
            }
        }

        return null;
    }

    /// <summary>
    ///     return true if expected substring in console. Does not work for Firefox
    ///     https://github.com/SeleniumHQ/selenium/issues/1161
    /// </summary>
    /// <param name="expectedSubstr"></param>
    /// <returns></returns>
    protected bool CheckConsoleLog(string expectedSubstr)
    {
        var entries = Driver.Manage().Logs.GetLog(LogType.Browser);
        return entries.Any() && entries.Any(enter => enter.Message.IndexOf(expectedSubstr, StringComparison.Ordinal) != -1);
    }

    #endregion

    #region GetPath

    protected string GetPicturePath(string filename)
    {
        var dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (dllPath == null)
            return null;

        return new DirectoryInfo(dllPath).Parent?.Parent?.Parent?.FullName + "\\data\\pictures\\" + filename;
    }

    protected string GetDownloadPath()
    {
        return InitializeService.GetBaseDownloadPath().Replace("\\", "/");
    }

    protected string GetSitePath()
    {
        return InitializeService.GetSitePath().Replace("\\", "/");
    }

    #endregion

    #region Modules

    protected void InstallModule(string baseUrl, string moduleName, bool activate = false)
    {
        GoTo(baseUrl + "/adminv3/modules/market");
        Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys(moduleName);
        Driver.WaitForElem(By.CssSelector("[data-module-string-id=\"" + moduleName.ToLower() + "\"]"));
        Driver.FindElement(By.CssSelector("[data-module-string-id=\"" + moduleName.ToLower() + "\"] .btn-success"))
            .Click();
        Driver.WaitForElem(By.ClassName("module-details-title"));
        if (activate) ActivateModule(moduleName);
    }

    protected void ActivateModule(string moduleName, string state = "on")
    {
        if (Driver.FindElements(By.Id("Module_Enabled")).Count == 1)
        {
            Driver.FindElement(By.ClassName("form-label-block")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
        }
        else
        {
            //old modules
            Driver.SwitchTo().Frame("moduleIFrame");
            Driver.FindElement(By.CssSelector(".admin-module-checkbox__item--" + state)).Click();
            Driver.SwitchTo().DefaultContent();
        }
    }

    #endregion
}