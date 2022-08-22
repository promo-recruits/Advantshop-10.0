using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Handlers.MyAccount;
using AdvantShop.Orders;
using AdvantShop.ViewModel.Checkout;

namespace AdvantShop.Handlers.Checkout
{
    public class ThankYouPageHandler
    {
        private readonly Order _order;
        private readonly Lead _lead;
        private const int ProductsCount = 12;

        public ThankYouPageHandler(int? orderId, int? leadId)
        {
            if (orderId != null)
                _order = OrderService.GetOrder(orderId.Value);

            if (leadId != null)
                _lead = LeadService.GetLead(leadId.Value);
        }

        public ThankYouPageViewModel Execute()
        {
            if (SettingsThankYouPage.ActionType == EThankYouPageActionType.None ||
                (_order != null && SettingsThankYouPage.ExcludedPaymentIds.Contains(_order.PaymentMethodId)))
                return null;

            var model = new ThankYouPageViewModel
            {
                ActionType = SettingsThankYouPage.ActionType,
                //OrderDetails = _order != null ? new GetOrderDetailsHandler(_order).Get() : null
            };

            switch (model.ActionType)
            {
                case EThankYouPageActionType.JoinGroup:
                    model.SocialNetworks = SettingsThankYouPage.SocialNetworks.Where(x => x.Enabled && x.Link.IsNotEmpty()).ToList();
                    return model;

                case EThankYouPageActionType.ShowProducts:
                    model.ProductIds = GetProductIds();
                    model.NameOfBlockProducts = SettingsThankYouPage.NameOfBlockProducts;
                    return model.ProductIds.Any() ? model : null;

                case EThankYouPageActionType.Share:
                    return model;
            }

            return model;
        }

        private List<int> GetProductIds()
        {
            var productIds = new List<int>();

            if (SettingsThankYouPage.ShowReletedProducts)
            {
                var relatedType = SettingsThankYouPage.ReletedProductsType;

                var items = _order != null
                    ? _order.OrderItems
                    : (_lead != null ? _lead.LeadItems.Select(x => (OrderItem)x).ToList() : null);

                if (items != null)
                    foreach (var item in items.Where(x => x.ProductID.HasValue))
                    {
                        var pr = GetRelatedProducts(item.ProductID.Value, relatedType);
                        if (pr != null && pr.Count > 0)
                        {
                            productIds.AddRange(pr.Select(x => x.ProductId));
                        }
                    }
            }

            var productListType = SettingsThankYouPage.ProductsListType;
            if (SettingsThankYouPage.ShowProductsList && productListType != EProductOnMain.None)
            {
                if (productListType == EProductOnMain.List && SettingsThankYouPage.ProductsListId.HasValue)
                {
                    productIds.AddRange(ProductListService.GetProducts(SettingsThankYouPage.ProductsListId.Value, ProductsCount).Select(x => x.ProductId));
                }
                else
                {
                    productIds.AddRange(ProductOnMain.GetProductsByType(productListType, ProductsCount).Select(x => x.ProductId));
                }
            }

            if (SettingsThankYouPage.ShowSelectedProducts)
            {
                var products = ProductService.GetProductsByIds(SettingsThankYouPage.SelectedProductIds, true);

                productIds.AddRange(products.Select(x => x.ProductId));
            }

            productIds = productIds.Distinct().Take(ProductsCount).ToList();
            productIds.Shuffle();

            return productIds;
        }

        private List<ProductModel> GetRelatedProducts(int productId, RelatedType relatedType)
        {
            var products = new List<ProductModel>();
            var product = ProductService.GetProduct(productId);

            switch (SettingsDesign.RelatedProductSourceType)
            {
                case SettingsDesign.eRelatedProductSourceType.Default:
                    products = ProductService.GetRelatedProducts(product.ProductId, relatedType);
                    break;

                case SettingsDesign.eRelatedProductSourceType.FromCategory:

                    products = ProductService.GetRelatedProducts(product.ProductId, relatedType);
                    if (products == null || products.Count == 0)
                    {
                        products = ProductService.GetRelatedProductsFromCategory(product, relatedType);
                    }
                    break;

                case SettingsDesign.eRelatedProductSourceType.FromModule:
                    var module = AttachedModules.GetModules<IModuleRelatedProducts>().FirstOrDefault();
                    if (module != null)
                    {
                        var instance = (IModuleRelatedProducts)Activator.CreateInstance(module);

                        products = instance.GetRelatedProducts(product, relatedType);
                    }

                    // get default related products
                    if ((products == null || products.Count == 0))
                    {
                        products = ProductService.GetRelatedProducts(product.ProductId, relatedType);
                    }
                    break;
            }
            return products;
        }
    }
}