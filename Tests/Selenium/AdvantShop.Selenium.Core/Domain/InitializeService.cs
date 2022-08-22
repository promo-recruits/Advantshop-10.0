using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Xml;
using AdvantShop.Selenium.Core.Domain.WebDriver;
using AdvantShop.Selenium.Core.SQL;
using CsvHelper;
using CsvHelper.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace AdvantShop.Selenium.Core.Domain;

[Flags]
public enum ClearType
{
    None = 0,
    Basic = 1,
    Catalog = 2,
    Customers = 4,
    Orders = 8,
    CRM = 16,
    CMS = 32,
    Payment = 64,
    Shipping = 128,
    ExportFeed = 256,
    Settings = 512,
    Total = 1024,
    SettingsSearch = 2048,
    Tasks = 4096,
    Bonuses = 8192,
    Taxes = 16384,
    SettingsProductsPerPage = 32768,
    Countries = 65536,
    Currencies = 131072,
    Redirect = 262144,
    MailFormat = 524288,
    MailTemplate = 1048576,
    SocialWidget = 2097152,
    Booking = 4194304,
    Landing = 8388608,
}

public static class InitializeService
{
    private static string _etalonConnectionString;
    private static string _testConnectionString;

    private static string _etalonDatabase;
    private static string _testDatabase;
    private static string _backupPath;
    private static string _databasePath;

    private static readonly string ProjectPath;

    private static string _baseScreenshotsPath;

    private static string _baseDownloadPath;
    private static string _sitePath;

    private static string _logFile;


    private static string _siteUrl;

    static InitializeService()
    {
        var dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (dllPath is null)
            throw new Exception("Unexpected path");

        var parent = new DirectoryInfo(dllPath).Parent?.Parent?.Parent;

        if (parent is null)
            throw new Exception("Unexpected path");

        ProjectPath = parent.FullName + "\\";

        ReadConfig();
    }


    public static void InitBrowser(out IWebDriver driver, out string baseUrl, out string baseScreenshotsPath,
        out string logFile, bool useAdvDriver = true)
    {
        baseUrl = _siteUrl;
        baseScreenshotsPath = _baseScreenshotsPath.Trim('\\') + "\\";
        logFile = _logFile;

        switch (Functions.GetEnvironmentVariable("DRIVER_TYPE"))
        {
            case "Firefox":
                FirefoxOptions firefoxOptions = new();

                firefoxOptions.SetLoggingPreference(LogType.Browser, LogLevel.Severe);
                firefoxOptions.AddArgument("no-sandbox");

                if (useAdvDriver)
                {
                    driver = new AdvWebDriver(new FirefoxDriver(firefoxOptions), "html", baseScreenshotsPath, baseUrl: baseUrl);
                }
                else
                {
                    driver = new FirefoxDriver(firefoxOptions);
                }

                driver.Manage().Window.Maximize();
                driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(10);
                break;

            default:
                ChromeOptions chromeOptions = new();
                chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Severe);
                chromeOptions.AddArgument("no-sandbox");

                if (useAdvDriver)
                {
                    driver = new AdvWebDriver(new ChromeDriver(chromeOptions), "html", baseScreenshotsPath, baseUrl: _siteUrl);
                }
                else
                {
                    driver = new ChromeDriver(chromeOptions);
                }

                driver.Manage().Window.Maximize();
                driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(10);
                break;
        }
    }

    public static void InitTrialBrowser(string trialUrl, out IWebDriver driver, out string baseUrl,
        out string baseScreenshotsPath, out string logFile, bool useAdvDriver = true)
    {
        baseUrl = trialUrl;
        baseScreenshotsPath = _baseScreenshotsPath.Trim('\\') + "\\";
        logFile = _logFile;

        ChromeOptions options = new();
        options.SetLoggingPreference(LogType.Browser, LogLevel.Severe);

        if (useAdvDriver)
        {
            driver = new AdvWebDriver(new ChromeDriver(options), "html", baseScreenshotsPath, baseUrl: baseUrl);
        }
        else
        {
            driver = new ChromeDriver(options);
        }

        driver.Manage().Window.Maximize();

        driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(10);
    }

    public static void BaseInitial(bool isStoreActive, HttpClient httpClient)
    {
        SetLicKey();
        SetDisplayCityBubble();
        EnableStore(isStoreActive);
        AppActive();
        Reindex();
        //ClearCache();
        ReindexLucene(httpClient);
    }

    public static void BaseInitialTrial(HttpClient httpClient)
    {
        //ClearCache();
        ReindexLucene(httpClient);
    }

    public static void ClearData(ClearType type)
    {
        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Catalog))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.product", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.category where categoryid <> 0",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.property", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.propertygroup",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.PropertyValue",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.ProductCategories",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.ProductPropertyValue",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.PropertyGroupCategory",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.RelatedCategories",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.RelatedProducts",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.RelatedPropertyValues",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.color", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.size", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.tag", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.brand", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.coupon", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.options", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.customoptions",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.ProductGifts",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.photo", connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery("delete from cms.review", connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery("delete from catalog.productlist",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.productext",
            connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery(
            "delete from seo.metainfo where type in ('product', 'category', 'brand', 'tag')",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery(
            "delete from catalog.photo where type in ('product', 'category', 'brand')",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Customers))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from Customers.Task", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Customers.ViewedTask",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Customers.TaskGroup",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Customers.ManagerTask",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [Bonus].[Transaction]",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.customer",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.clientcode",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.contact",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.CustomerCertificate",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.CustomerCoupon",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.CustomerGroup where CustomerGroupId<>1",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.CustomerRoleAction",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.Departments",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.RecentlyViewsData",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.OpenIdLinkCustomer",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.RoleAction",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.SmsNotifications",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.Subscription",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.CustomerField",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.CustomerFieldValue",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.CustomerFieldValuesMap",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.ManagerRole",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.Managers",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.CustomerSegment",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.CustomerSegment_Customer",
            connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery(
            "UPDATE Settings.Settings set value = '8b40c4f4-322e-4926-ad39-d2f6d6cd10c1' where name='LicKey'",
            connectionString: _testConnectionString);
        }


        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Orders))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from [order].Certificate",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].[Order]", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderContact",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderCurrency",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderCustomer",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderItems",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderPaymentInfo",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].PaymentDetails",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderPickPoint",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderPriceDiscount",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderCustomOptions",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderHistory",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].StatusHistory",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderTax", connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery("delete from [order].DeletedOrders",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderByRequest",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderConfirmation",
            connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderStatus",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderSource",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.CRM))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from [order].Lead", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].LeadItem", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].LeadCurrency",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].LeadEvent",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from crm.BizProcessRule",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from crm.DealStatus", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from crm.SalesFunnel", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from crm.SalesFunnel_DealStatus",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from crm.SalesFunnel_Manager",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].OrderSource",
            connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery("delete from [order].[Order]", connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'DefaultSalesFunnelId'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'BuyInOneClick_CreateOrder'",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.CMS))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from cms.carousel", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from cms.menu", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from cms.staticpage", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from cms.staticblock", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Settings.News", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Settings.NewsCategory",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from voice.Answer", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from voice.VoiceTheme", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.photo", connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery(
            "delete from catalog.photo where type in ('carousel', 'staticpage', 'menu')",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery(
            "UPDATE Settings.Settings set value = 'true' where name='ShowVotingOnMainPage'",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Payment))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from [order].PaymentMethod",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].PaymentParam",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].PaymentCity",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].PaymentCountry",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Settings.GiftCertificatePayments",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Shipping))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from [order].ShippingCache",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].ShippingCityExcluded",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].ShippingCountry",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].ShippingMethod",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].ShippingParam",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].ShippingPayments",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from [order].ShippingCity",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.ExportFeed))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from settings.ExportFeed",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.ExportFeedSelectedCategories",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.ExportFeedExcludedProducts",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.ExportFeedSettings",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Settings))
        {
            //SQLDataAccess2.ExecuteNonQuery("delete from settings.ProfitPlan", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.Redirect",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.Reseller",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery(
            "delete from settings.TemplateSettings where Template not like '_default' and Template not like 'mobile' and Template not like 'mobile_Modern'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.SettingsSearch",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) ||
            type.HasFlag(ClearType.SettingsSearch))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from settings.SettingsSearch",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Tasks))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from Customers.Task", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Customers.TaskGroup",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Bonuses))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from Bonus.Card", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Bonus.Grade", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Bonus.AdditionBonus",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Bonus.SmsTemplate",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Bonus.SmsLog", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery(
            "UPDATE Settings.Settings set value = 'true' where name='BonusSystem.IsActive'",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Taxes))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.tax", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'DefaultTaxId'",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) ||
            type.HasFlag(ClearType.SettingsProductsPerPage))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'ProductsPerPage'",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Countries))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from customers.Country",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.Region", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.City", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'SellerCountryId'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'SellerRegionId'",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Currencies))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.product", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.category where categoryid <> 0",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.ProductCategories",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.offer", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'DefaultCurrencyISO3'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from Booking.Service", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.Currency", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'YandexMarketCurrency'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'GoogleBaseCurrency'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'YahooShoppingCurrency'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'ShoppingComCurrency'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'ShopzillaCurrency'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'PriceGrabberCurrency'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'AmazonCurrency'",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Redirect))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from settings.Redirect",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.Error404",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.MailFormat))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from settings.MailFormat",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.MailFormatType",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.MailTemplate))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from settings.MailTemplate",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.SocialWidget))
        {
            SQLDataAccess2.ExecuteNonQuery(
            "delete from settings.settings where Name = 'SettingsSocialWidget.IsActive'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery(
            "delete from settings.settings where Name = 'SettingsSocialWidget.IsShowVk'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery(
            "delete from settings.settings where Name = 'SettingsSocialWidget.IsShowFb'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery(
            "delete from settings.settings where Name = 'SettingsSocialWidget.IsShowJivosite'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery(
            "delete from settings.settings where Name = 'SettingsSocialWidget.IsShowCallback'",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Landing))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.Landing", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.LandingBlock", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.LandingColorScheme",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.LandingDeferredEmail",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.LandingDomain",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.LandingEmailTemplate",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.LandingForm", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.LandingSettings",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.LandingSite", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.LandingSiteSettings",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from CMS.LandingSubBlock",
            connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'Phone'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'Email_NewLead'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkVk'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkVkActive'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkFacebook'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkFacebookActive'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkTwitter'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkTwitterActive'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'MobilePhone'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkInstagramm'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkOk'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkTelegram'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkYoutube'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkInstagrammActive'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkOkActive'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkTelegramActive'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.settings where Name = 'LinkYoutubeActive'",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery(
            "UPDATE Settings.Settings set value = 'true' where name='ActiveLandingPage'",
            connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Basic) || type.HasFlag(ClearType.Total) || type.HasFlag(ClearType.Booking))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from booking.Affiliate",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.AffiliateAdditionalTime",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.AffiliateCategory",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.AffiliateManager",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.AffiliateReservationResource",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.AffiliateService",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.AffiliateSmsTemplate",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.AffiliateTimeOfBooking",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.Booking", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.BookingCurrency",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.BookingItems",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.Category", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.ReservationResource",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.ReservationResourceAdditionalTime",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.ReservationResourceService",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.ReservationResourceTag",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.ReservationResourceTagsMap",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.ReservationResourceTimeOfBooking",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from booking.Service", connectionString: _testConnectionString);
        }

        if (type.HasFlag(ClearType.Total))
        {
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.currency", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from catalog.tax", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from settings.GiftCertificateTaxes",
            connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery("delete from customers.city", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.region", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from customers.country",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from dbo.DownloadableContent",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from dbo.Modules", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from dbo.ModuleSettings",
            connectionString: _testConnectionString);

            SQLDataAccess2.ExecuteNonQuery("delete from dbo.SaasData", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from dbo.Modules", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from dbo.OrderSource", connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery("delete from dbo.OrderStatus", connectionString: _testConnectionString);
        }

        Reindex();
    }

    private static void Reindex()
    {
        SQLDataAccess2.ExecuteNonQuery("[Catalog].[SetCategoryHierarchicallyEnabled]", new {CatParent = 0},
        CommandType.StoredProcedure, connectionString: _testConnectionString);

        SQLDataAccess2.ExecuteNonQuery("[Catalog].[PreCalcProductParamsMass]",
        new
        {
            ModerateReviews = SQLDataAccess2.ExecuteScalar<bool>(
            "Select value from settings.settings where name ='ModerateReviewed'",
            connectionString: _testConnectionString),
            OnlyAvailable = SQLDataAccess2.ExecuteScalar<bool>(
            "Select value from settings.settings where name ='ShowOnlyAvalible'",
            connectionString: _testConnectionString),
            ComplexFilter = SQLDataAccess2.ExecuteScalar<bool>(
            "Select value from settings.settings where name ='ComplexFilter'",
            connectionString: _testConnectionString)
        }, CommandType.StoredProcedure, connectionString: _testConnectionString);

        SQLDataAccess2.ExecuteNonQuery("[Catalog].[sp_RecalculateProductsCount]",
        CommandType.StoredProcedure, connectionString: _testConnectionString);
    }

    private static void SetLicKey()
    {
        SQLDataAccess2.ExecuteNonQuery(
        "UPDATE Settings.Settings set value = '8b40c4f4-322e-4926-ad39-d2f6d6cd10c1' where name='LicKey'",
        connectionString: _testConnectionString);
        SQLDataAccess2.ExecuteNonQuery("UPDATE Settings.Settings set value = 'true' where name='ActiveLic'",
        connectionString: _testConnectionString);
    }

    private static void SetDisplayCityBubble()
    {
        SQLDataAccess2.ExecuteNonQuery(
        "UPDATE Settings.Settings set value = 'false' where name='DisplayCityBubble'",
        connectionString: _testConnectionString);
    }

    public static void ShowVoiting()
    {
        SQLDataAccess2.ExecuteNonQuery(
        "UPDATE Settings.Settings set value = 'true' where name='ShowVotingOnMainPage'",
        connectionString: _testConnectionString);
    }

    public static void AppActive()
    {
        SQLDataAccess2.ExecuteNonQuery("UPDATE Settings.Settings set value = 'true' where name='TasksActive'",
        connectionString: _testConnectionString);
        SQLDataAccess2.ExecuteNonQuery(
        "UPDATE Settings.Settings set value = 'true' where name='BonusSystem.IsActive'",
        connectionString: _testConnectionString);
    }

    public static void LandingsActive()
    {
        SQLDataAccess2.ExecuteNonQuery("UPDATE Settings.Settings set value = 'true' where name='ActiveLandingPage'",
        connectionString: _testConnectionString);
    }

    public static void VoitingActive()
    {
        SQLDataAccess2.ExecuteNonQuery(
        "UPDATE Settings.TemplateSettings set value = 'true' where name='VotingVisibility'",
        connectionString: _testConnectionString);
    }

    public static void YandexChannelActive()
    {
        //SQLDataAccess2.ExecuteNonQuery("INSERT INTO Settings.Settings (Name, Value) VALUES ('YandexChannelActive', 'True')", connectionString: _testConnectionString);
        SQLDataAccess2.ExecuteNonQuery(
        "UPDATE Settings.Settings set value = 'true' where name='YandexChannelActive'",
        connectionString: _testConnectionString);
    }

    public static void GoogleChannelActive()
    {
        SQLDataAccess2.ExecuteNonQuery(
        "INSERT INTO Settings.Settings (Name, Value) VALUES ('GoogleChannelActive', 'True')",
        connectionString: _testConnectionString);
    }

    public static void BonusSystemActive()
    {
        SQLDataAccess2.ExecuteNonQuery("UPDATE Settings.Settings set VALUE = 'True' where name = 'BonusAppActive'",
        connectionString: _testConnectionString);
    }

    public static void BookingActive()
    {
        SQLDataAccess2.ExecuteNonQuery("UPDATE Settings.Settings set VALUE = 'True' where name = 'BookingActive'",
        connectionString: _testConnectionString);
    }

    public static void SetShopUrl()
    {
        SQLDataAccess2.ExecuteNonQuery(
        "UPDATE Settings.Settings SET value = 'http://my-shop.ru' where name = 'ShopURL'",
        connectionString: _testConnectionString);
    }

    public static void EnableStore(bool enable)
    {
        SQLDataAccess2.ExecuteNonQuery(
        "UPDATE Settings.Settings set value = '" + enable + "' where name='StoreActive'",
        connectionString: _testConnectionString);
        if (enable)
        {
            SQLDataAccess2.ExecuteNonQuery(
            "INSERT INTO Settings.Settings (Name, Value) VALUES ('StoreScreenShot', '../images/design/preview.jpg')",
            connectionString: _testConnectionString);
            SQLDataAccess2.ExecuteNonQuery(
            "INSERT INTO Settings.Settings (Name, Value) VALUES ('StoreScreenShotMiddle', '../images/design/preview.jpg')",
            connectionString: _testConnectionString);
        }
    }

    public static void SetCustomLogoAndFavicon()
    {
        SQLDataAccess2.ExecuteNonQuery(
        "UPDATE Settings.Settings SET value = 'my_logo.png' where name = 'MainPageLogoFileName'",
        connectionString: _testConnectionString);
        SQLDataAccess2.ExecuteNonQuery(
        "UPDATE Settings.Settings SET value = 'my_favicon.png' where name = 'MainFaviconFileName'",
        connectionString: _testConnectionString);
    }

    public static string GetBaseDownloadPath()
    {
        return _baseDownloadPath.Trim('\\') + "\\";
    }

    public static string GetSitePath()
    {
        return _sitePath.Trim('\\') + "\\";
    }

    public static void LoadData(params string[] files)
    {
        foreach (var file in files)
        {
            DataTable csvData = new();
            string value;

            using TextReader textReader = new StreamReader(ProjectPath + file);
            using var reader = new CsvReader(textReader,
            new CsvConfiguration(CultureInfo.InvariantCulture) {HasHeaderRecord = false, Delimiter = ";"});
            var counter = 0;
            //reading headers
            if (reader.Read())
            {
                for (var i = 0; reader.TryGetField(i, out value); i++)
                {
                    if (value.Contains("[GUID]"))
                    {
                        value = value.Replace("[GUID]", "");
                        var dataColumn = new DataColumn(value, typeof(Guid));
                        csvData.Columns.Add(dataColumn);
                    }
                    else if (value.Contains("[DATE]"))
                    {
                        value = value.Replace("[DATE]", "");
                        var dataColumn = new DataColumn(value, typeof(DateTime));
                        csvData.Columns.Add(dataColumn);
                    }
                    else if (value.Contains("[PHONE]"))
                    {
                        value = value.Replace("[PHONE]", "");
                        var dataColumn = new DataColumn(value, typeof(string));
                        csvData.Columns.Add(dataColumn);
                    }
                    else
                    {
                        var dataColumn = new DataColumn(value);
                        csvData.Columns.Add(dataColumn);
                    }

                    counter++;
                }
            }

            //reading data
            while (reader.Read())
            {
                var fieldData = new object[counter];
                for (var i = 0; reader.TryGetField(i, out value); i++)
                {
                    Guid tempGuid;
                    DateTime tempDate;
                    float tempFloat;
                    if (Guid.TryParse(value, out tempGuid))
                    {
                        fieldData[i] = tempGuid;
                    }
                    else if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture,
                             DateTimeStyles.None, out tempDate))
                    {
                        fieldData[i] = tempDate;
                    }
                    else if (DateTime.TryParseExact(value, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture,
                             DateTimeStyles.None, out tempDate))
                    {
                        fieldData[i] = tempDate;
                    }
                    else if (DateTime.TryParseExact(value, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                             DateTimeStyles.None, out tempDate))
                    {
                        fieldData[i] = tempDate;
                    }
                    else if (value != null && value.IndexOf("+7", StringComparison.Ordinal) == 0 &&
                             value.Length == 12)
                    {
                        fieldData[i] = value;
                    }
                    else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out tempFloat))
                    {
                        fieldData[i] = tempFloat;
                    }

                    else
                    {
                        fieldData[i] = value switch
                        {
                            "NULL" => null,
                            "[TODAY]" => DateTime.Today.ToString(CultureInfo.CurrentCulture),
                            "[TOMORROW]" => DateTime.Today.AddDays(1).ToString(CultureInfo.CurrentCulture),
                            "[YESTERDAY]" => DateTime.Today.AddDays(-1).ToString(CultureInfo.CurrentCulture),
                            "[MONTH_AGO]" => DateTime.Today.AddMonths(-1).ToString(CultureInfo.CurrentCulture),
                            _ => value
                        };
                    }
                }

                csvData.Rows.Add(fieldData);
            }

            string tableName = new FileInfo(file).Name.Replace(".csv", "");

            ExecuteCsvDataToSql(tableName, csvData);
        }
    }

    public static void ClearCache(HttpClient httpClient)
    {
        var response = httpClient.GetStringAsync(_siteUrl.Trim('/') + "/tools/clearcache.ashx").GetAwaiter().GetResult();
        if (response != "ok")
            throw new Exception("Cant clear cache:" + response);
    }


    private static void ReindexLucene(HttpClient httpClient)
    {
        var response =
            httpClient.GetStringAsync(_siteUrl.Trim('/') + "/tools/ReindexLucene.ashx").GetAwaiter().GetResult();
        if (response != "ok")
            throw new Exception("Cant reindex lucene:" + response);
    }


    private static void ExecuteCsvDataToSql(string tableName, DataTable data)
    {
        using (SqlConnection dbConnection = new SqlConnection(_testConnectionString))
        {
            dbConnection.Open();
            using (SqlBulkCopy s = new SqlBulkCopy(dbConnection,
                   SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.CheckConstraints, null))
            {
                s.DestinationTableName = tableName;
                foreach (var column in data.Columns)
                {
                    s.ColumnMappings.Add(column.ToString(), column.ToString());
                }

                s.WriteToServer(data);
            }

            dbConnection.Close();
        }
    }


    public static void RollBackDatabase(bool isRepeated = false)
    {
        //#if DEBUG
        //return;
        //#endif

        string backupName = _backupPath + "etalon_" + DateTime.Now.Ticks + ".bak";
        try
        {
            //File.Copy(_backupPath + "etalon.bak", backupName);
            DbHelper.BackupDatabase(_etalonConnectionString, _etalonDatabase, backupName);

            if (DbHelper.ExistDatabase(_testConnectionString))
            {
                DbHelper.DropDatabase(_testConnectionString);
            }

            DbHelper.RestoreDatabase(_testConnectionString, _testDatabase, backupName, _databasePath);
        }
        catch (Exception)
        {
            if (!isRepeated)
            {
                Thread.Sleep(new Random().Next(1000, 3000));
                RollBackDatabase(true);
            }
            else
            {
                throw;
            }
        }
        finally
        {
            if (File.Exists(backupName))
            {
                File.Delete(backupName);
            }
        }
    }


    private static void ReadConfig()
    {
        XmlDocument document = new XmlDocument();
        document.Load(ProjectPath + "app.config");

        XmlNode listSettings = document.SelectNodes("/configuration/appSettings")?[0];
        if (listSettings is null)
            throw new NullReferenceException("Could not read app.config");

        foreach (XmlNode node in listSettings)
        {
            switch (node?.Attributes["key"].Value)
            {
                case "EtalonDatabaseName":
                    _etalonDatabase = node.Attributes["value"].Value;
                    break;

                case "TestDatabaseName":
                    _testDatabase = node.Attributes["value"].Value;
                    break;

                case "BackupPath":
                    _backupPath = node.Attributes["value"].Value;
                    break;

                case "DatabasePath":
                    _databasePath = node.Attributes["value"].Value;
                    break;

                case "SiteUrl":
                    _siteUrl = node.Attributes["value"].Value;
                    break;

                case "SitePath":
                    _sitePath = node.Attributes["value"].Value;
                    break;

                case "ScreenshotsPath":
                    _baseScreenshotsPath = node.Attributes["value"].Value;
                    break;

                case "DownloadPath":
                    _baseDownloadPath = node.Attributes["value"].Value;
                    break;

                case "LogFile":
                    _logFile = node.Attributes["value"].Value;
                    break;
            }
        }

        XmlNode nodeListConnections = document.SelectNodes("/configuration/connectionStrings")[0];

        foreach (XmlNode node in nodeListConnections)
        {
            switch (node.Attributes["name"].Value)
            {
                case "etalon":
                    _etalonConnectionString = node.Attributes["connectionString"].Value;
                    break;
                case "AdvantConnectionString":
                    _testConnectionString = node.Attributes["connectionString"].Value;
                    break;
            }
        }
    }
}