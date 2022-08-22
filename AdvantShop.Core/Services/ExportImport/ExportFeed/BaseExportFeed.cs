//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using System;
using System.Web;
using System.Collections.Generic;

using AdvantShop.FilePath;
using AdvantShop.Configuration;
using AdvantShop.Catalog;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using System.Linq;
using AdvantShop.Statistic;

namespace AdvantShop.ExportImport
{
    public abstract class BaseExportFeed
    {
        protected readonly bool UseCommonStatistic;

        protected string tempPrefix = ".temp";

        protected readonly List<ProductDiscount> ProductDiscountModels;

        protected BaseExportFeed()
        {
            var discountModule = AttachedModules.GetModules<IDiscount>().FirstOrDefault();
            if (discountModule != null)
            {
                var classInstance = (IDiscount)Activator.CreateInstance(discountModule);
                ProductDiscountModels = classInstance.GetProductDiscountsList();
            }
        }

        protected BaseExportFeed(bool useCommonStatistic) : this()
        {
            UseCommonStatistic = useCommonStatistic;
        }

        public abstract string Export(int exportFeedId);

        public abstract string Export(IEnumerable<ExportFeedCategories> categories, IEnumerable<ExportFeedProductModel> products, ExportFeedSettings options, int categoriesCount, int productsCount);

        public abstract void SetDefaultSettings(int exportFeedId);

        public abstract List<string> GetAvailableVariables();

        public abstract List<string> GetAvailableFileExtentions();

        public abstract List<ExportFeedCategories> GetCategories(int exportFeedId);

        public abstract List<ExportFeedProductModel> GetProducts(int exportFeedId);

        public abstract int GetProductsCount(int exportFeedId);

        public abstract int GetCategoriesCount(int exportFeedId);


        public virtual string GetDownloadableExportFeedFileLink(int exportFeedId)
        {
            var settings = ExportFeedSettingsProvider.GetSettings(exportFeedId);            
            return SettingsMain.SiteUrl + "/" + settings.FileFullName + "?rnd=" + (new Random()).Next();
        }

        protected string GetImageProductPath(string photoPath)
        {
            if (string.IsNullOrEmpty(photoPath))
                photoPath = "";

            photoPath = photoPath.Trim().ToLower();

            if (photoPath.Contains("http://") || photoPath.Contains("https://"))
                return HttpUtility.HtmlEncode(photoPath);

            return SettingsMain.SiteUrl.TrimEnd('/') + "/" + FoldersHelper.GetImageProductPathRelative(ProductImageType.Big, photoPath, false);
        }

        protected string GetAdditionalUrlTags(ExportFeedProductModel row, string urlTags)
        {
            if (string.IsNullOrEmpty(urlTags))
            {
                return string.Empty;
            }

            urlTags = urlTags.Replace("#STORE_NAME#", HttpUtility.UrlEncode(SettingsMain.ShopName));
            urlTags = urlTags.Replace("#STORE_URL#", HttpUtility.UrlEncode(SettingsMain.SiteUrl));
            urlTags = urlTags.Replace("#PRODUCT_NAME#", HttpUtility.UrlEncode(row.Name));
            urlTags = urlTags.Replace("#PRODUCT_ID#", row.ProductId.ToString());
            urlTags = urlTags.Replace("#OFFER_ID#", row.OfferId.ToString());
            urlTags = urlTags.Replace("#PRODUCT_ARTNO#", HttpUtility.UrlEncode(row.ArtNo));
            return urlTags;
        }

        #region CommonStatistic

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (UseCommonStatistic)
                commonStatisticAction();
        }

        protected void CsSetFileName(string fileName)
        {
            DoForCommonStatistic(() => CommonStatistic.FileName = fileName);
        }

        protected void CsSetTotalRow(int count)
        {
            DoForCommonStatistic(() => CommonStatistic.TotalRow = count);
        }

        protected void CsNextRow()
        {
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }

        protected void CsRowError()
        {
            DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
        }

        protected void CsRowError(string message)
        {
            DoForCommonStatistic(() =>
            {
                CommonStatistic.WriteLog(message);
                CommonStatistic.TotalErrorRow++;
            });

        }

        #endregion
    }
}