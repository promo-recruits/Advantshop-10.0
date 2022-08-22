using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetFieldsFromCategoriesCsvFile : BaseGetFieldsFromCsvFileHandler<object>
    {
        public GetFieldsFromCategoriesCsvFile(ImportCategoriesModel model, string outputFilePath) : base(model, outputFilePath)
        {
        }

        protected override void LoadData()
        {
            CsvRows = new CsvImportCategories(OutputFilePath, false, ColumnSeparator, Encoding, null, useCommonStatistic: false).ReadFirstRecord();
        }

        protected override object HandleData()
        {
            Headers.RemoveAll(header => header == CategoryFields.CategoryHierarchy.StrName());

            foreach (CategoryFields item in Enum.GetValues(typeof(CategoryFields)))
            {
                if(item == CategoryFields.CategoryHierarchy)
                    continue;
                AllFields.Add(item.StrName().ToLower(), item.Localize());
            }

            return new { FirstItem, AllFields, Headers };
        }
    }
}
