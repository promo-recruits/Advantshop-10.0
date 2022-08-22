using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Orders;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetOrderItems
    {
        private OrderItemsFilterModel _filterModel;
        private readonly UrlHelper _urlHelper;
        private SqlPaging _paging;

        public GetOrderItems(OrderItemsFilterModel model)
        {
            _filterModel = model;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public FilterResult<OrderItemModel> Execute()
        {
            var model = new FilterResult<OrderItemModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено товаров: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
                return model;
            
            model.DataItems = new List<OrderItemModel>();

            var currency = OrderService.GetOrderCurrency(_filterModel.OrderId) ?? CurrencyService.CurrentCurrency;
            var pageItems = _paging.PageItemsList<OrderItem>();
            foreach(var pageItem in pageItems)
            {
                var p = pageItem.ProductID != null ? ProductService.GetProduct(pageItem.ProductID.Value) : null;
                var selectedOptions = OrderService.GetOrderCustomOptionsByOrderItemId(pageItem.OrderItemID);
                var showEditCustomOptions = p != null && (selectedOptions != null && selectedOptions.Count > 0 ||
                                                          CustomOptionsService.GetCustomOptionsByProductId(p.ProductId).Any());
                var modelItem = new OrderItemModel()
                {
                    OrderItemId = pageItem.OrderItemID,
                    OrderId = pageItem.OrderID,
                    ImageSrc = pageItem.Photo.ImageSrcSmall(),
                    ArtNo = pageItem.ArtNo,
                    Name = pageItem.Name,
                    ProductId = p != null ? p.ProductId : default(int?),
                    ProductLink = p != null ? _urlHelper.Action("Edit", "Product", new { id = p.ProductId }) : null,

                    Color = !string.IsNullOrEmpty(pageItem.Color) ? SettingsCatalog.ColorsHeader + ": " + pageItem.Color : "",
                    Size = !string.IsNullOrEmpty(pageItem.Size) ? SettingsCatalog.SizesHeader + ": " + pageItem.Size : "",
                    CustomOptions = selectedOptions != null ? RenderCustomOptions(selectedOptions) : null,
                    ShowEditCustomOptions = showEditCustomOptions,

                    Price = pageItem.Price,
                    Amount = pageItem.Amount,
                    Cost = PriceService.SimpleRoundPrice(pageItem.Price * pageItem.Amount, currency).FormatPrice(currency),
                    Width = pageItem.Width,
                    Height = pageItem.Height,
                    Length = pageItem.Length,
                    Enabled = p != null ? p.Enabled : false,
                    Weight = pageItem.Weight,
                    BarCode = pageItem.BarCode
                };

                if (pageItem.TypeItem == TypeOrderItem.Product)
                {
                    var offer = OfferService.GetOffer(pageItem.ArtNo);
                    if (offer == null || pageItem.Amount > offer.Amount)
                    {
                        var amount = offer == null ? 0 : offer.Amount;

                        modelItem.Available = false;
                        modelItem.AvailableText =
                            string.Format(LocalizationService.GetResource("Admin.Orders.GetOrderItems.AvailableLimit"),
                                amount);
                    }
                    else if (pageItem.Amount <= offer.Amount)
                    {
                        modelItem.Available = true;
                        modelItem.AvailableText =
                            LocalizationService.GetResource("Admin.Orders.GetOrderItems.Available");
                    }
                }
                else if (pageItem.TypeItem == TypeOrderItem.BookingService)
                {
                    modelItem.Available = true;
                }

                model.DataItems.Add(modelItem);
            };
            
            return model;
        }

        public void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "OrderItemID",
                "OrderID",
                "PhotoID",
                "ArtNo",
                "Name",
                "ProductID",
                "Color",
                "Size",
                "Price",
                "Amount",
                "Width",
                "Height",
                "Length",
                "Weight",
                "Price * Amount".AsSqlField("Cost"),
                "BookingServiceId",
                "TypeItem",
                "BarCode");
            _paging.From("[Order].[OrderItems]");
            _paging.Where("OrderID = {0}", _filterModel.OrderId);

            Sorting();
        }

        public void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
                return;

            var sorting = _filterModel.Sorting.ToLower();
            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sorting);
                }
                else
                {
                    _paging.OrderByDesc(sorting);
                }
            }
        }
        
        public static string RenderCustomOptions(List<EvaluatedCustomOptions> evlist)
        {
            if (evlist == null || evlist.Count == 0)
                return "";

            var html = new StringBuilder("");
            foreach (var ev in evlist)
            {
                html.AppendFormat(
                    "<div class=\"orderitem-option\"><span class=\"orderitem-option-name\">{0}:</span> <span class=\"orderitem-option-value\">{1} {2}</span></div>",
                    ev.CustomOptionTitle, ev.OptionTitle, ev.FormatPrice);
            }
            html.Append("");

            return html.ToString();
        }
    }
}
