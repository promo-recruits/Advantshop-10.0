//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Admin;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.SEO;

namespace AdvantShop.Core.UrlRewriter
{
    public enum ParamType
    {
        None,
        Product,
        Category,
        StaticPage,
        News,
        Brand,
        NewsCategory,
        Tag
    }

    public static class UrlService
    {
        public class UrlStruct
        {
            public int ObjId { get; set; }
            public string UrlPath { get; set; }
            public ParamType Type { get; set; }
        }

        private const string ProductsWord = "products";
        private const string CategoriesWord = "categories";
        private const string PagesWord = "pages";
        private const string NewsWord = "news";
        private const string NewscategoryWord = "newscategory";
        private const string ManufacturersWord = "manufacturers";

       public enum ESocialType
        {
            none = 0,
            vk = 1,
            fb = 2,
            ok = 3
        }

        public static readonly List<string> ExtentionNotToRedirect = new List<string>
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".gif",
                ".bmp",
                ".ico",
                ".css",
                ".js",
                //".html",
                ".htc",
                ".ashx",
                ".xls",
                ".xlsx",
                ".xml",
                ".yml",
                ".txt",
                ".zip",
                ".pdf",
                ".swf",
                ".tpl",
		".axd",
                ".csv",
            };

        public static readonly List<string> ExtentionOpenInBrowser = new List<string>
        {
            ".csv",
            ".xml",
            ".yml"
        };

        public static readonly Dictionary<ParamType, string> NamesAndPages = new Dictionary<ParamType, string>
                                                                                         {
                                                                                             {ParamType.StaticPage, "StaticPageView.aspx"},
                                                                                             {ParamType.Category, "Catalog.aspx"},
                                                                                             {ParamType.Product, "Details.aspx"},
                                                                                             {ParamType.NewsCategory, "News.aspx"},
                                                                                             {ParamType.News, "NewsView.aspx"},
                                                                                             {ParamType.Brand, "BrandView.aspx"},
                                                                                             {ParamType.None, string.Empty}
                                                                                         };

        public static readonly Dictionary<ParamType, string> NamesAndIds = new Dictionary<ParamType, string>
                                                                                         {
                                                                                             {ParamType.StaticPage, "StaticPageId"},
                                                                                             {ParamType.Category, "CategoryId"},
                                                                                             {ParamType.Product, "ProductId"},
                                                                                             {ParamType.NewsCategory, "NewsCategoryId"},
                                                                                             {ParamType.News, "NewsId"},
                                                                                             {ParamType.Brand, "BrandId"},
                                                                                             {ParamType.Tag, "Id"},
                                                                                             {ParamType.None, string.Empty}
                                                                                         };

        public static readonly Dictionary<ParamType, string> NamesAndWords = new Dictionary<ParamType, string>
                                                                                         {
                                                                                             {ParamType.StaticPage, PagesWord},
                                                                                             {ParamType.Category, CategoriesWord},
                                                                                             {ParamType.Product, ProductsWord},
                                                                                             {ParamType.NewsCategory, NewscategoryWord},
                                                                                             {ParamType.News, NewsWord},
                                                                                             {ParamType.Brand, ManufacturersWord},
                                                                                             {ParamType.None, string.Empty}
                                                                                         };
        public static readonly Dictionary<ParamType, string> NamesAndDb = new Dictionary<ParamType, string>
                                                                                         {
                                                                                             {ParamType.StaticPage, "CMS.StaticPage"},
                                                                                             {ParamType.Category, "Catalog.Category"},
                                                                                             {ParamType.Product, "Catalog.Product"},
                                                                                             {ParamType.NewsCategory, "Settings.NewsCategory"},
                                                                                             {ParamType.News, "Settings.News"},
                                                                                             {ParamType.Brand, "Catalog.Brand"},
                                                                                             {ParamType.Tag, "Catalog.Tag"},
                                                                                             {ParamType.None, string.Empty}
                                                                                         };
        public static readonly List<string> UnAvailableWords = new List<string>
                                                                                 {
                                                                                     ProductsWord,
                                                                                     CategoriesWord,
                                                                                     PagesWord,
                                                                                     NewscategoryWord,
                                                                                     NewsWord,
                                                                                     ManufacturersWord,
                                                                                    "admin",
                                                                                    "adminv2",
                                                                                    "areas",
                                                                                    "bin",
                                                                                    "combine",
                                                                                    "content",
                                                                                    "fonts",
                                                                                    "images",
                                                                                    "landings",
                                                                                    "modules",
                                                                                    "pictures",
                                                                                    "scripts",
                                                                                    "styles",
                                                                                    "templates",
                                                                                    "tools",
                                                                                    "userfiles",
                                                                                    "vendors",
                                                                                    "views"
                                                                                 };
        
        /// <summary>
        /// Warning!!! if we can't urlpath on databing
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objId"></param>
        /// <returns></returns>
        public static string GetLinkDB(ParamType type, int objId)
        {
            var objUrl = GetObjUrlFromDb(type, objId);
            return GetLink(type, objUrl, objId);
        }

        /// <summary>
        /// get url from db by id and type
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetObjUrlFromDb(ParamType type, int objId)
        {
            if (type == ParamType.None) return string.Empty;
            return SQLDataAccess.ExecuteScalar<string>(string.Format("select urlPath from {0} where {1}=@id", NamesAndDb[type], NamesAndIds[type]),
                                                        CommandType.Text, new SqlParameter { ParameterName = "@id", Value = objId });
        }

        /// <summary>
        /// create url-string
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="objUrl"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetLink(ParamType type, string objUrl, int objId)
        {
            return GetLink(type, objUrl, objId, string.Empty);
        }

        /// <summary>
        /// return url link
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="objUrl"></param>
        /// <param name="type"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string GetLink(ParamType type, string objUrl, int objId, string query)
        {
            return string.IsNullOrEmpty(objUrl)
                ? NamesAndPages[type] + '?' + NamesAndIds[type] + "=" + objId + (string.IsNullOrEmpty(query) ? string.Empty : '&' + query)
                : NamesAndWords[type] + '/' + HttpUtility.UrlEncode(objUrl) + (string.IsNullOrEmpty(query) ? string.Empty : '?' + query);
        }

        public static string GetAbsoluteLink(string link)
        {
            if (link.Contains("http://") || link.Contains("https://")) return link;

            if (HttpContext.Current == null)
                return link;

            return string.Format("{0}/{1}", (HttpContext.Current.Request.ApplicationPath == "/" ? string.Empty : HttpContext.Current.Request.ApplicationPath).ToLower(), link.TrimStart('/'));
        }

        public static string GetAdminAbsoluteLink(string link)
        {
            if (link.Contains("http://") || link.Contains("https://")) return link;
            return string.Format("{0}/{1}", (HttpContext.Current.Request.ApplicationPath == "/" ? "/admin" : HttpContext.Current.Request.ApplicationPath + "/admin"), link.TrimStart('/'));
        }

        /// <summary>
        /// Get if url is avalible
        /// </summary>
        /// <param name="objUrl"></param>
        /// <param name="objId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAvailableUrl(int objId, ParamType type, string objUrl)
        {
            if (string.IsNullOrWhiteSpace(objUrl)) return true;
            var temp = objUrl.ToLower();
            // find in unavalible words
            if (UnAvailableWords.FirstOrDefault(x => x == temp) != null) return false;
            //find  in database
            if (GetUrlCount(temp, type, objId) > 0) return false;

            return true;
        }

        public static bool IsAvailableUrl(ParamType type, string objUrl)
        {
            if (string.IsNullOrWhiteSpace(objUrl)) return true;
            // find in unavalible words
            var temp = objUrl.ToLower();
            if (UnAvailableWords.FirstOrDefault(x => x == temp) != null) return false;
            //find  in database
            if (GetUrlCount(temp, type, 0) > 0) return false;

            return true;
        }

        /// <summary>
        /// Get count of objUrl in database by type
        /// </summary>
        /// <param name="objUrl"></param>
        /// <param name="type"></param>
        /// <param name="objId"></param>
        /// <returns></returns>
        private static int GetUrlCount(string objUrl, ParamType type, int objId)
        {
            return SQLDataAccess.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM {0} WHERE UrlPath=@UrlPath AND {1} <> @id", NamesAndDb[type], NamesAndIds[type]),
                                                    CommandType.Text,
                                                    new SqlParameter { ParameterName = "@UrlPath", Value = objUrl },
                                                    new SqlParameter { ParameterName = "@id", Value = objId }
                                                    );
        }

        public static bool IsDebugUrl(string url)
        {
            // Add here more adress if you need it
            return url.Contains("/tools/") ||
                   url.Contains("/techdemos/") ||
                   url.Contains("/content/info/") ||
                   url.Contains("/.well-known/");

        }

        public static ESocialType IsSocialUrl(string url)
        {
            foreach (ESocialType item in new ESocialType[] {ESocialType.vk, ESocialType.fb})
            {
                if (url.StartsWith("https://" + item + ".") || url.StartsWith("http://" + item + "."))
                    return item;
            }

            return ESocialType.none;
        }

        public static void RedirectTo(HttpApplication app, UrlStruct param)
        {
            var query = NamesAndIds[param.Type] + "=" + param.ObjId;
            app.Context.RewritePath("~/" + NamesAndPages[param.Type], "", string.IsNullOrEmpty(app.Request.Url.Query) ? query : query + "&" + app.Request.QueryString);
        }

        public static string GetRedirect301(string fromUrl, string reqAbsoluteUri)
        {
            var index = reqAbsoluteUri.IndexOf('?');
            var absoluteUri = index > 0
                                ? reqAbsoluteUri.Substring(0, index).ToLower() + reqAbsoluteUri.Substring(index).ToLower()
                                : reqAbsoluteUri.ToLower();

            absoluteUri = HttpUtility.UrlDecode(absoluteUri);

            var redirect = RedirectSeoService.GetByInputUrl(fromUrl, absoluteUri);
            if (redirect == null)
                return null;

            var uri = new Uri(redirect.RedirectFrom, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
            {
                redirect.RedirectFrom = GetUrl(redirect.RedirectFrom);
            }
            uri = new Uri(redirect.RedirectTo, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
            {
                redirect.RedirectTo = GetUrl(redirect.RedirectTo);
            }

            string location = string.Empty;
            if (!string.IsNullOrEmpty(redirect.ProductArtNo))
            {
                var product = Catalog.ProductService.GetProduct(redirect.ProductArtNo);

                location = product != null ? GetAbsoluteLink(GetLink(ParamType.Product, product.UrlPath, product.ProductId))
                                           : absoluteUri.Replace(redirect.RedirectFrom, redirect.RedirectTo);
            }
            else
            {
                var absoluteUrlEncoded = absoluteUri.Split('/').Select(HttpUtility.UrlDecode).AggregateString("/").ToLower();
                var absoluteUrlPathEncoded = absoluteUri.Split('/').Select(HttpUtility.UrlPathEncode).AggregateString("/").ToLower();
                if (absoluteUrlEncoded.Contains(redirect.RedirectFrom) || absoluteUrlEncoded.Replace(" ", "+").Contains(redirect.RedirectFrom))
                {
                    location = absoluteUrlEncoded.Replace(redirect.RedirectFrom, redirect.RedirectTo);
                }
                if (absoluteUrlPathEncoded.Contains(redirect.RedirectFrom) || absoluteUrlPathEncoded.Replace(" ", "+").Contains(redirect.RedirectFrom))
                {
                    location = absoluteUrlPathEncoded.Replace(redirect.RedirectFrom, redirect.RedirectTo);
                }
                else if (absoluteUri.Contains(redirect.RedirectFrom))
                {
                    location = absoluteUri.Replace(redirect.RedirectFrom, redirect.RedirectTo);
                }
            }
            return location.Replace("*", string.Empty);
        }

        public static string GetAvailableValidUrl(int objId, ParamType type, string prevUrl)
        {
            var j = 1;
            var transformUrl = !SettingsMain.EnableCyrillicUrl
                ? StringHelper.TransformUrl(StringHelper.Translit(prevUrl))
                : StringHelper.TransformUrl(prevUrl ?? "");

            if (transformUrl.Length > 150)
                transformUrl = transformUrl.Substring(0, 150);

            var url = transformUrl;
            if (url.IsNullOrEmpty())
                url = transformUrl = type.ToString().ToLower();

            while (!IsAvailableUrl(objId, type, url))
            {
                url = StringHelper.TransformUrl(transformUrl);
                url =
                    (url.Length + j.ToString().Length + 1 > 150
                        ? url.Substring(0, url.Length - j.ToString().Length - 1)
                        : url) +
                    "-" + j++;
            }

            return url;
        }

        public static bool IsValidUrl(string url, ParamType type)
        {
            var pattern = !SettingsMain.EnableCyrillicUrl ? "^[a-zA-Z0-9_\\-]+$" : "^[a-zA-Zа-яА-Я0-9_\\-]+$";
            var reg = new Regex(pattern);

            return reg.IsMatch(url) && IsAvailableUrl(type, url);
        }

        public static string GenerateBaseUrl()
        {
            if (HttpContext.Current.Items["BaseUrl"] != null)
                return (string)HttpContext.Current.Items["BaseUrl"];

            var request = HttpContext.Current.Request;
            var requestUrl = request.Url;

            var port = requestUrl.IsDefaultPort ? string.Empty : (":" + Convert.ToString(requestUrl.Port, CultureInfo.InvariantCulture));
            var baseUrl = ((IsSecureConnection(request) ? "https" : "http") + Uri.SchemeDelimiter + requestUrl.Host + port).ToLower();

            HttpContext.Current.Items["BaseUrl"] = baseUrl;

            return baseUrl;
        }

        public static string GetUrl()
        {
            if (HttpContext.Current == null)
                return SettingsMain.SiteUrl + "/";

            var request = HttpContext.Current.Request;
            var url = request.ApplicationPath == "/" ? request.ApplicationPath : request.ApplicationPath + "/";

            return GenerateBaseUrl() + url.ToLower(); 
        }
        
        public static string GetUrl(string path)
        {
            return GetUrl() + path;
        }

        /// <summary>
        /// Admin webforms base url
        /// </summary>
        /// <returns></returns>
        public static string GetAdminBaseUrl()
        {
            if (HttpContext.Current == null)
                return SettingsMain.SiteUrl + "/adminv2/";

            return GenerateBaseUrl() + HttpContext.Current.Request.Url.PathAndQuery;
        }

        public static string GetAdminUrl(bool baseUrlfromSettings = false, bool useAdminAreaTemplates = true)
        {
            if (HttpContext.Current == null || baseUrlfromSettings == true)
                return SettingsMain.SiteUrl + "/adminv2/";

            var request = HttpContext.Current.Request;
            var requestUrl = request.Url;

            var port = requestUrl.IsDefaultPort ? string.Empty : (":" + Convert.ToString(requestUrl.Port, CultureInfo.InvariantCulture));
            var url = request.ApplicationPath == "/" ? request.ApplicationPath : request.ApplicationPath + "/";

            var adminUrl = (IsSecureConnection(request) ? "https" : "http") + Uri.SchemeDelimiter + requestUrl.Host + port + url;

            if (useAdminAreaTemplates && AdminAreaTemplate.Current != null)
                adminUrl += AdminAreaTemplate.Current + "/";
            else
                adminUrl+= "adminv2/";

            return adminUrl;
        }

        public static string GetAdminUrl(string path, bool baseUrlfromSettings = false)
        {
            return GetAdminUrl(baseUrlfromSettings) + path;
        }

        public static string GetAdminStaticPath()
        {
            return "areas/admin/content/";
        }

        public static string GetAdminStaticUrl()
        {
            if (HttpContext.Current == null)
                return GetAdminStaticPath();

            var request = HttpContext.Current.Request;
            var requestUrl = request.Url;

            var port = requestUrl.IsDefaultPort ? string.Empty : (":" + Convert.ToString(requestUrl.Port, CultureInfo.InvariantCulture));
            var url = request.ApplicationPath == "/" ? request.ApplicationPath : request.ApplicationPath + "/";

            return (IsSecureConnection(request) ? "https" : "http") + Uri.SchemeDelimiter + requestUrl.Host + port + url + GetAdminStaticPath();
        }

        public static string GetCanonicalUrl()
        {
            if (HttpContext.Current == null)
                return SettingsMain.SiteUrl + "/";
            
            return (GenerateBaseUrl() + HttpContext.Current.Request.Url.AbsolutePath).ToLower();
        }

        public static string GetCurrentUrl(string url = null)
        {
            if (HttpContext.Current == null)
                return SettingsMain.SiteUrl + "/";

            return HttpContext.Current.Request.RawUrl + url;
        }

        public static bool IsSecureConnection(HttpRequest request)
        {
            if (request == null)
                return false;

            return request.IsSecureConnection || request.Headers["x-forwarded-proto"] == "https";
        }

        public static bool IsSecureConnection(HttpRequestBase request)
        {
            if (request == null)
                return false;

            return request.IsSecureConnection || request.Headers["x-forwarded-proto"] == "https";
        }

        public static bool IsIpBanned(string ip)
        {
            var ipList = SettingsGeneral.BannedIp;
            return ipList.IsNotEmpty() && ipList.Split(",").Contains(ip);
        }

        public static bool IsCurrentUrl(string url)
        {
            if (HttpContext.Current == null || url.IsNullOrEmpty())
                return false;
            var currentUri = HttpContext.Current.Request.Url;
            if (currentUri.LocalPath == "/")
                return false;

            url = url.ToLower();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = GetUrl(url.TrimStart('/'));

            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri) && uri.Host != currentUri.Host)
                return false;

            var path = uri != null ? uri.LocalPath : url;
            return path.IsNotEmpty() && path.Equals(currentUri.LocalPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}