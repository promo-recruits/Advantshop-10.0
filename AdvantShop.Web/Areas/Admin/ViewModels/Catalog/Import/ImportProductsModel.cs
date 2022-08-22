using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Saas;

namespace AdvantShop.Web.Admin.ViewModels.Catalog.Import
{
    public class ImportProductsModel : BaseImportModel
    {
        public ImportProductsModel() : base()
        {
            ImportRemains = Enum.GetValues(typeof(EImportRemainsType)).Cast<EImportRemainsType>()
                .Select(enumItem => new SelectListItem { Value = enumItem.StrName(), Text = enumItem.Localize() }).ToList();
        }

        public string PropertySeparator { get; set; }

        public string PropertyValueSeparator { get; set; }

        public bool DisableProducts { get; set; }

        public string ImportRemainsType { get; set; }

        public List<SelectListItem> ImportRemains { get; set; }

        public SaasData CurrentSaasData { get; set; }

        public bool IsStartExport { get; set; }

        public bool OnlyUpdateProducts { get; set; }

        public bool UpdatePhotos { get; set; }

        public bool CsvV2 { get; set; }
    }
}
