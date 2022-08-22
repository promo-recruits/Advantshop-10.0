using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedSettingsAvitoModel : IValidatableObject
    {
        public ExportFeedSettingsAvitoModel(ExportFeedAvitoOptions exportFeedAvitoOptions)
        {
            Currency = exportFeedAvitoOptions.Currency;
            PublicationDateOffset = exportFeedAvitoOptions.PublicationDateOffset;
            DurationOfPublicationInDays = exportFeedAvitoOptions.DurationOfPublicationInDays;
            PaidPublicationOption = exportFeedAvitoOptions.PaidPublicationOption;
            PaidServices = exportFeedAvitoOptions.PaidServices;
            ManagerName = exportFeedAvitoOptions.ManagerName;
            Phone = exportFeedAvitoOptions.Phone;
            Address = exportFeedAvitoOptions.Address;
            EmailMessages = exportFeedAvitoOptions.EmailMessages;
            ExportNotAvailable = exportFeedAvitoOptions.ExportNotAvailable;
            ProductDescriptionType = exportFeedAvitoOptions.ProductDescriptionType;
            DefaultAvitoCategory = exportFeedAvitoOptions.DefaultAvitoCategory;
            UnloadProperties = exportFeedAvitoOptions.UnloadProperties;
        }

        public string Currency { get; set; }
        public int PublicationDateOffset { get; set; }
        public int DurationOfPublicationInDays { get; set; }
        public EPaidPublicationOption PaidPublicationOption { get; set; }
        public EPaidServices PaidServices { get; set; }
        public string ManagerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool EmailMessages { get; set; }
        public bool ExportNotAvailable { get; set; }
        public string ProductDescriptionType { get; set; }
        public string DefaultAvitoCategory { get; set; }
        public bool UnloadProperties { get; set; }


        public Dictionary<string, string> ListPaidPublicationOption
        {
            get
            {
                return new Dictionary<string, string> {
                    { EPaidPublicationOption.Package.StrName(),EPaidPublicationOption.Package.Localize()},
                    { EPaidPublicationOption.PackageSingle.StrName(),EPaidPublicationOption.PackageSingle.Localize() },
                    { EPaidPublicationOption.Single.StrName(),EPaidPublicationOption.Single.Localize() }
                };
            }
        }

        public Dictionary<string, string> ListPaidServices
        {
            get
            {
                return new Dictionary<string, string> {
                    { EPaidServices.Free.StrName(),EPaidServices.Free.Localize()},
                    { EPaidServices.Highlight.StrName(),EPaidServices.Highlight.Localize() },
                    { EPaidServices.Premium.StrName(),EPaidServices.Premium.Localize() },
                    { EPaidServices.PushUp.StrName(),EPaidServices.PushUp.Localize() },
                    { EPaidServices.QuickSale.StrName(),EPaidServices.QuickSale.Localize() },
                    { EPaidServices.TurboSale.StrName(),EPaidServices.TurboSale.Localize() },
                    { EPaidServices.VIP.StrName(),EPaidServices.VIP.Localize() }
                };
            }
        }

        public Dictionary<string, string> ProductDescriptionTypeList
        {
            get
            {
                return new Dictionary<string, string> {
                    { "short", LocalizationService.GetResource("Admin.ExportFeed.Settings.BriefDescription") },
                    { "full", LocalizationService.GetResource("Admin.ExportFeed.Settings.FullDescription") }
                    //,{ "none", LocalizationService.GetResource("Admin.ExportFeed.Settings.DontUseDescription") }
                };
            }
        }

        public Dictionary<string, string> Currencies
        {
            get
            {
                var currencyList = new Dictionary<string, string>();
                foreach (var item in CurrencyService.GetAllCurrencies().Where(item => ExportFeedAvito.AvailableCurrencies.Contains(item.Iso3)).ToList())
                {
                    currencyList.Add(item.Iso3, item.Name);
                }
                return currencyList;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ManagerName))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "ShopName" });
            }
            if (string.IsNullOrEmpty(Phone))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "CompanyName" });
            }
        }
    }
}
