using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class StartImportCategoriesHandler : BaseStartImportHandler
    {
        public StartImportCategoriesHandler(ImportCategoriesModel model, string inputFilePath) : base(model, inputFilePath)
        {
        }

        protected override void ValidateData()
        {
            if (!FieldMapping.ContainsKey(CategoryFields.CategoryId.StrName()) && 
                !FieldMapping.ContainsKey(CategoryFields.ExternalId.StrName()) && 
                !FieldMapping.ContainsKey(CategoryFields.Name.StrName()))
            {
                throw new BlException(LocalizationService.GetResource("Admin.ImportCategories.Errors.FieldsRequired"));
            }
        }

        protected override void Handle()
        {
            var importCategories = new CsvImportCategories(InputFilePath, HaveHeader, ColumnSeparator, Encoding, FieldMapping);
            importCategories.ProcessThroughACommonStatistic("import#?importTab=importCategories", LocalizationService.GetResource("Admin.ImportCategories.ProcessName"));

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_ImportCategories);
        }
    }
}
