using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;
using CsvHelper;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetExampleBrandsFileHandler
    {
        private readonly string _columnSeparator;
        private readonly string _encoding;
        private readonly string _outputFilePath;
        private Dictionary<EBrandFields, string> _brandsExample;

        public GetExampleBrandsFileHandler(ImportBrandsModel model, string outputFilePath)
        {
            _columnSeparator = model.ColumnSeparator.IsNullOrEmpty() &&
                               string.Equals(model.ColumnSeparator, SeparatorsEnum.Custom.ToString(),
                                   StringComparison.InvariantCultureIgnoreCase)
                ? model.CustomColumnSeparator
                : model.ColumnSeparator;
            _encoding = model.Encoding;
            _outputFilePath = outputFilePath;
        }

        private CsvWriter InitWriter()
        {
            var streamWriter = new StreamWriter(_outputFilePath, false, Encoding.GetEncoding(_encoding));
            var writer = new CsvWriter(streamWriter);
            writer.Configuration.Delimiter = _columnSeparator;
            return writer;
        }

        public void Execute()
        {
            if (_columnSeparator.IsNullOrEmpty())
                throw new BlException(LocalizationService.GetResource("Admin.Import.Errors.NotColumnSeparator"));

            if (File.Exists(_outputFilePath))
                File.Delete(_outputFilePath);

            GetBrandExample();
            using (var csv = InitWriter())
            {
                foreach (EBrandFields item in Enum.GetValues(typeof(EBrandFields)))
                {
                    if (item == EBrandFields.None || !_brandsExample.ContainsKey(item))
                        continue;
                    csv.WriteField(item.StrName().ToLower());
                }

                csv.NextRecord();

                foreach (EBrandFields item in Enum.GetValues(typeof(EBrandFields)))
                {
                    if (item == EBrandFields.None || !_brandsExample.ContainsKey(item))
                        continue;
                    csv.WriteField(_brandsExample[item]);
                }

                csv.NextRecord();
            }
        }

        private void GetBrandExample()
        {
            _brandsExample = new Dictionary<EBrandFields, string>
            {
                {EBrandFields.Name, "Apple"},
                {
                    EBrandFields.Description,
                    @"<p>Apple Inc. &mdash; ???????????????????????? ????????????????????, ?????????????????????????? ???????????????????????? ?? ???????????????????? ??????????????????????, ????????????????????????, ??????????????????, ???????????????????????? ??????????????????????. ???????? ???? ???????????????? ?? ?????????????? ???????????????????????? ?????????????????????? ?? ?????????????????????? ?????????????????????????? ???????????????????????? ???????????? ?? ?????????????????????? ??????????????????????.<br /><br />????????-???????????????? &mdash; ?? ??????????????????, ???????? ????????????????????. ?????????????????? ?????????????????????????? ?????????????????????? ?? ?????????????????????? ??????????????, ???????????????????? Apple ?????????????? ???????????????????? ??????????????????, ?????????????????? ?? ??????????????, ?? ?????????????????? ?????????????????????????????? ??????????????????????. ?? ?????? 2011 ???????? ???????????????? ?????????? Apple ???????? ???????????????? ?????????? ?????????????? ?????????????? ?? ???????? (c ?????????????? ?? $153,3 ????????) ?? ???????????????? ???????????????????????????? ???????????????????????????????????? ?????????????????? Millward Brown.</p>"
                },
                {
                    EBrandFields.BriefDescription,
                    @"<p>Apple Inc. &mdash; ???????????????????????? ????????????????????, ?????????????????????????? ???????????????????????? ?? ???????????????????? ??????????????????????, ????????????????????????, ??????????????????, ???????????????????????? ??????????????????????. ???????? ???? ???????????????? ?? ?????????????? ???????????????????????? ?????????????????????? ?? ?????????????????????? ?????????????????????????? ???????????????????????? ???????????? ?? ?????????????????????? ??????????????????????. ????????-???????????????? &mdash; ?? ??????????????????, ???????? ????????????????????.</p>"
                },
                {EBrandFields.Enabled, "True"},
                {EBrandFields.UrlPath, "apple"},
                {EBrandFields.BrandSiteUrl, "apple.com"},
                {EBrandFields.CountryId, "196"},
                {EBrandFields.CountryName, "??????"},
                {EBrandFields.CountryOfManufactureId, "196"},
                {EBrandFields.CountryOfManufactureName, "??????"},
                {EBrandFields.Photo, "https://www.apple.com/ac/globalnav/5/ru_RU/images/globalnav/apple/image_large.svg"}
            };
        }
    }
}
