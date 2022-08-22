using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Seo
{
    public class GetSeoSettings
    {
        public SEOSettingsModel Execute()
        {
            var model = new SEOSettingsModel
            {
                BrandItemDefaultH1 = SettingsSEO.BrandItemH1,
                BrandItemDefaultTitle = SettingsSEO.BrandItemMetaTitle,
                BrandItemDefaultMetaKeywords = SettingsSEO.BrandItemMetaKeywords,
                BrandItemDefaultMetaDescription = SettingsSEO.BrandItemMetaDescription,

                BrandsDefaultTitle = SettingsSEO.BrandMetaTitle,
                BrandsDefaultMetaKeywords = SettingsSEO.BrandMetaKeywords,
                BrandsDefaultMetaDescription = SettingsSEO.BrandMetaDescription,

                CategoriesDefaultH1 = SettingsSEO.CategoryMetaH1,
                CategoriesDefaultTitle = SettingsSEO.CategoryMetaTitle,
                CategoriesDefaultMetaKeywords = SettingsSEO.CategoryMetaKeywords,
                CategoriesDefaultMetaDescription = SettingsSEO.CategoryMetaDescription,

                TagsDefaultH1 = SettingsSEO.TagsMetaH1,
                TagsDefaultTitle = SettingsSEO.TagsMetaTitle,
                TagsDefaultMetaKeywords = SettingsSEO.TagsMetaKeywords,
                TagsDefaultMetaDescription = SettingsSEO.TagsMetaDescription,

                DefaultTitle = SettingsSEO.DefaultMetaTitle,
                DefaultMetaKeywords = SettingsSEO.DefaultMetaKeywords,
                DefaultMetaDescription = SettingsSEO.DefaultMetaDescription,

                NewsDefaultH1 = SettingsSEO.NewsMetaH1,
                NewsDefaultTitle = SettingsSEO.NewsMetaTitle,
                NewsDefaultMetaKeywords = SettingsSEO.NewsMetaKeywords,
                NewsDefaultMetaDescription = SettingsSEO.NewsMetaDescription,

                ProductsDefaultH1 = SettingsSEO.ProductMetaH1,
                ProductsDefaultTitle = SettingsSEO.ProductMetaTitle,
                ProductsDefaultMetaKeywords = SettingsSEO.ProductMetaKeywords,
                ProductsDefaultMetaDescription = SettingsSEO.ProductMetaDescription,
                ProductsDefaultAdditionalDescription = SettingsSEO.ProductAdditionalDescription,

                StaticPageDefaultH1 = SettingsSEO.StaticPageMetaH1,
                StaticPageDefaultTitle = SettingsSEO.StaticPageMetaTitle,
                StaticPageDefaultMetaKeywords = SettingsSEO.StaticPageMetaKeywords,
                StaticPageDefaultMetaDescription = SettingsSEO.StaticPageMetaDescription,

                CategoryNewsDefaultH1 = SettingsNews.NewsMetaH1,
                CategoryNewsDefaultTitle = SettingsNews.NewsMetaTitle,
                CategoryNewsDefaultMetaKeywords = SettingsNews.NewsMetaKeywords,
                CategoryNewsDefaultMetaDescription = SettingsNews.NewsMetaDescription,

                MainPageProductsDefaultH1 = SettingsSEO.MainPageProductsMetaH1,
                MainPageProductsDefaultTitle = SettingsSEO.MainPageProductsMetaTitle,
                MainPageProductsDefaultMetaKeywords = SettingsSEO.MainPageProductsMetaKeywords,
                MainPageProductsDefaultMetaDescription = SettingsSEO.MainPageProductsMetaDescription,


                OpenGraphEnabled = SettingsSEO.OpenGraphEnabled,
                OpenGraphFbAdmins = SettingsSEO.OpenGraphFbAdmins,
                EnableCyrillicUrl = SettingsMain.EnableCyrillicUrl,

                GoogleAnalyticsNumber = SettingsSEO.GoogleAnalyticsNumber,
                GoogleAnalyticsEnableDemogrReports = SettingsSEO.GoogleAnalyticsEnableDemogrReports,
                GoogleAnalyticsSendOrderOnStatusChanged = SettingsSEO.GoogleAnalyticsSendOrderOnStatusChanged,
                GoogleAnalyticsOrderStatusIdToSend = SettingsSEO.GoogleAnalyticsOrderStatusIdToSend,
                GoogleAnalyticsEnabled = SettingsSEO.GoogleAnalyticsEnabled,

                GoogleAnalyticsApiEnabled = SettingsSEO.GoogleAnalyticsApiEnabled,
                GoogleAnalyticsAccountID = SettingsSEO.GoogleAnalyticsAccountID,
                GoogleClientId = SettingsOAuth.GoogleClientId,
                GoogleClientSecret = SettingsOAuth.GoogleClientSecret,


                UseGTM = SettingsSEO.UseGTM,
                GTMContainerId = SettingsSEO.GTMContainerID,
                GTMOfferIdType = SettingsSEO.GTMOfferIdType,

                EnableRedirect301 = SettingsSEO.Enabled301Redirects
            };

            if (!model.OrderStatuses.Any(x => x.Value == model.GoogleAnalyticsOrderStatusIdToSend.ToString()))
            {
                model.GoogleAnalyticsOrderStatusIdToSend = 0;
                model.OrderStatuses.Insert(0, new SelectListItem { Value = "0", Text = "Не указан" });
            }

            try
            {
                var filePath = HttpContext.Current.Server.MapPath("~/robots.txt");
                if (File.Exists(filePath))
                {
                    using (var streamReader = new StreamReader(filePath))
                        model.RobotsText = streamReader.ReadToEnd();
                }
                else
                {
                    using (File.Create(filePath)) { } //nothing here, just  create file
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return model;
        }
    }
}
