//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Configuration
{
    public class SettingsGeneral
    {
        private static readonly Object ThisLock = new Object();

        public static string AbsoluteUrlPath
        {
            get
            {
                // check Handler instead of Request (Request may throw error "Request is not available in this context")
                if (HttpContext.Current != null && HttpContext.Current.Handler != null)
                {
                    if (HttpContext.Current.Request != null)
                        return (HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath).ToLower();
                }
                return SettingsMain.SiteUrl.ToLower();
            }
        }
        

        private static string _absolutePath;

        public static string AbsolutePath
        {
            get
            {
                if (_absolutePath.IsNullOrEmpty())
                    SetAbsolutePath(System.Web.Hosting.HostingEnvironment.MapPath("~/"));
                return _absolutePath;
            }
        }

        public static void SetAbsolutePath(string st)
        {
            lock (ThisLock)
            {
                _absolutePath = st;
            }
        }

        public static string SiteVersion => SettingProvider.GetConfigSettingValue("PublicVersion");

        public static string SiteVersionDev => SettingProvider.GetConfigSettingValue("Version");

        public static string Release => SettingProvider.GetConfigSettingValue("Release");

        public static string CurrentSaasId
        {
            get => SettingProvider.Items["LicKey"];
            set => SettingProvider.Items["LicKey"] = value;
        }

        public static string CsvSeparator
        {
            get => SettingProvider.Items["CsvSeparator"];
            set => SettingProvider.Items["CsvSeparator"] = value;
        }

        public static string CsvEnconing
        {
            get => SettingProvider.Items["CsvEnconing"];
            set => SettingProvider.Items["CsvEnconing"] = value;
        }

        public static string CsvColumSeparator
        {
            get => SettingProvider.Items["CsvColumSeparator"];
            set => SettingProvider.Items["CsvColumSeparator"] = value;
        }

        public static string CsvPropertySeparator
        {
            get => SettingProvider.Items["CsvPropertySeparator"];
            set => SettingProvider.Items["CsvPropertySeparator"] = value;
        }

        public static bool CsvExportNoInCategory
        {
            get => SettingProvider.Items["CsvExportNoInCategory"].TryParseBool();
            set => SettingProvider.Items["CsvExportNoInCategory"] = value.ToString();
        }

        public static string BannedIp
        {
            get => SettingProvider.Items["BannedIp"];
            set => SettingProvider.Items["BannedIp"] = value;
        }

        public static bool BackupPhotosBeforeDeleting => SettingProvider.Items["BackupPhotosBeforeDeleting"].TryParseBool();

        public static bool DisableXFrameOptionsHeader => SettingProvider.Items["DisableXFrameOptionsHeader"].TryParseBool();

        public static string LastBundlesCleanup
        {
            get => SettingProvider.Items["LastBundlesCleanup"];
            set => SettingProvider.Items["LastBundlesCleanup"] = value;
        }
    }
}

