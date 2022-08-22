//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Caching;
using AdvantShop.SEO;

namespace AdvantShop.Configuration
{
    public class SettingsNews
    {
        public static int NewsMainPageCount
        {
            get => int.Parse(SettingProvider.Items["NewsMainPageCount"]);
            set
            {
                SettingProvider.Items["NewsMainPageCount"] = value.ToString();
                CacheManager.RemoveByPattern(CacheNames.GetNewsForMainPage());
            }
        }

        public static int NewsPerPage
        {
            get => int.Parse(SettingProvider.Items["NewsPerPage"]);
            set => SettingProvider.Items["NewsPerPage"] = value.ToString();
        }

        public static string NewsMetaTitle
        {
            get => SettingProvider.Items[MetaType.News + "MainTitle"];
            set => SettingProvider.Items[MetaType.News + "MainTitle"] = value;
        }

        public static string NewsMetaDescription
        {
            get => SettingProvider.Items[MetaType.News + "MainMetaDescription"];
            set => SettingProvider.Items[MetaType.News + "MainMetaDescription"] = value;
        }

        public static string NewsMetaKeywords
        {
            get => SettingProvider.Items[MetaType.News + "MainMetaKeywords"];
            set => SettingProvider.Items[MetaType.News + "MainMetaKeywords"] = value;
        }

        public static string NewsMetaH1
        {
            get => SettingProvider.Items[MetaType.News + "MainH1"];
            set => SettingProvider.Items[MetaType.News + "MainH1"] = value;
        }

        public static string MainPageText
        {
            get => SettingProvider.Items[MetaType.News + "MainPageText"];
            set => SettingProvider.Items[MetaType.News + "MainPageText"] = value;
        }

        public static bool RssViewNews
        {
            get {
                    bool result = false;
                    bool.TryParse(SettingProvider.Items[MetaType.News + "RssViewNews"], out result);
                    return result;
                }
            set => SettingProvider.Items[MetaType.News + "RssViewNews"] = value.ToString();
        }
    }
}