using System.IO;
using System.Linq;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Statistic;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportCategories
{
    public class StartingExportCategoriesHandler
    {
        private readonly string _fileName = "export_categories";
        private readonly string _fileExtention = ".csv";

        public string Execute(bool useCommonStatistic)
        {
            //delete old
            foreach (var item in Directory.GetFiles(FoldersHelper.GetPathAbsolut(FolderType.PriceTemp)).Where(f => f.Contains(_fileName)))
            {
                FileHelpers.DeleteFile(item);
            }

            var exportFeedCsvOptions = new ExportFeedCsvOptions()
            {
                FileName = FoldersHelper.GetPathRelative(FolderType.PriceTemp, _fileName, false),
                FileExtention = "csv",
                CsvEnconing = ExportFeedCsvCategorySettings.CsvEnconing,
                CsvSeparator = ExportFeedCsvCategorySettings.CsvSeparator,
            };

            CsvExportCategories.Factory(
                ExportFeedCsvCategoryService.GetCsvCategories(ExportFeedCsvCategorySettings.FieldMapping),
                exportFeedCsvOptions,
                ExportFeedCsvCategorySettings.FieldMapping,
                ExportFeedCsvCategoryService.GetCsvCategoriesCount(),
                useCommonStatistic
                ).Process();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_ExportCategories);

            return UrlService.GetAbsoluteLink(FoldersHelper.PhotoFoldersPath[FolderType.PriceTemp] + _fileName + _fileExtention);
        }
    }
}
