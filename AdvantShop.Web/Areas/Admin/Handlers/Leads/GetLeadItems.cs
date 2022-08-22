using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Crm.Leads;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetLeadItems
    {
        private readonly int _leadId;
        private readonly string _sorting;
        private readonly string _sortingType;
        private readonly UrlHelper _urlHelper;

        public GetLeadItems(int leadId, string sorting, string sortingType)
        {
            _leadId = leadId;
            _sorting = sorting;
            _sortingType = sortingType;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public List<LeadItemModel> Execute()
        {
            var leadItems = new List<LeadItemModel>();

            var currency = LeadService.GetLeadCurrency(_leadId) ?? CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);

            foreach (var item in LeadService.GetLeadItems(_leadId))
            {
                var p = item.ProductId != null ? ProductService.GetProduct(item.ProductId.Value) : null;

                var leadItem = new LeadItemModel()
                {
                    LeadItemId = item.LeadItemId,
                    LeadId = item.LeadId,
                    ImageSrc = item.Photo.ImageSrcSmall(),
                    ArtNo = item.ArtNo,
                    Name = item.Name,
                    ProductLink = p != null ? _urlHelper.Action("Edit", "Product", new { id = p.ProductId }) : null,

                    Color = !string.IsNullOrEmpty(item.Color) ? SettingsCatalog.ColorsHeader + ": " + item.Color : "",
                    Size = !string.IsNullOrEmpty(item.Size) ? SettingsCatalog.SizesHeader + ": " + item.Size : "",
                    Price = item.Price,
                    Amount = item.Amount,
                    Cost = (item.Price*item.Amount).FormatPrice(currency),
                    Width = item.Width,
                    Length = item.Length,
                    Height = item.Height,
                    Weight = item.Weight,

                    BarCode = item.BarCode,

                    CustomOptions = RenderCustomOptions(CustomOptionsService.DeserializeFromJson(item.CustomOptionsJson, currency.CurrencyValue))
                };

                //var offer = OfferService.GetOffer(item.ArtNo);
                //if (offer == null || item.Amount > offer.Amount)
                //{
                //    var amount = offer == null ? 0 : offer.Amount;

                //    leadItem.Available = false;
                //    leadItem.AvailableText = string.Format(LocalizationService.GetResource("Admin.Orders.GetOrderItems.AvailableLimit"), amount);
                //}
                //else if (item.Amount <= offer.Amount)
                //{
                //    leadItem.Available = true;
                //    leadItem.AvailableText = LocalizationService.GetResource("Admin.Orders.GetOrderItems.Available");
                //}

                leadItems.Add(leadItem);
            }

            if (!string.IsNullOrEmpty(_sorting))
            {
                bool orderBy = !(_sortingType != null && _sortingType.ToLower() == "desc");
                    
                switch (_sorting.ToLower())
                {
                    case "name":
                        leadItems = orderBy
                            ? leadItems.OrderBy(x => x.Name).ToList()
                            : leadItems.OrderByDescending(x => x.Name).ToList();
                        break;

                    case "price":
                        leadItems = orderBy
                            ? leadItems.OrderBy(x => x.Price).ToList()
                            : leadItems.OrderByDescending(x => x.Price).ToList();
                        break;

                    case "amount":
                        leadItems = orderBy
                            ? leadItems.OrderBy(x => x.Amount).ToList()
                            : leadItems.OrderByDescending(x => x.Amount).ToList();
                        break;

                    //case "available":
                    //    leadItems = orderBy
                    //        ? leadItems.OrderBy(x => x.Available).ToList()
                    //        : leadItems.OrderByDescending(x => x.Available).ToList();
                    //    break;

                    case "cost":
                        leadItems = orderBy
                            ? leadItems.OrderBy(x => x.Price * x.Amount).ToList()
                            : leadItems.OrderByDescending(x => x.Price * x.Amount).ToList();
                        break;
                }
            }

            return leadItems;
        }

        public static string RenderCustomOptions(List<EvaluatedCustomOptions> evlist)
        {
            if (evlist == null || !evlist.Any())
                return string.Empty;

            var html = new StringBuilder();
            foreach (var ev in evlist)
            {
                html.AppendFormat(
                    "<div class=\"orderitem-option\"><span class=\"orderitem-option-name\">{0}:</span> <span class=\"orderitem-option-value\">{1} {2}</span></div>",
                    ev.CustomOptionTitle, ev.OptionTitle, ev.FormatPrice);
            }

            return html.ToString();
        }
    }
}
