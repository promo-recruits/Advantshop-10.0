using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Models.ProductDetails;
using AdvantShop.ViewModel.Catalog;

namespace AdvantShop.Handlers.ProductDetails
{
    public class GetRelatedProductsHandler
    {
        #region Fields

        private readonly Product _product;
        private readonly RelatedType _type;
        private readonly bool _isMobile;
        #endregion

        public GetRelatedProductsHandler(Product product, RelatedType type, bool isMobile)
        {
            _product = product;
            _type = type;
            _isMobile = isMobile;
        }

        public RelatedProductsViewModel Get()
        {
            if (_product == null)
                return null;

            var products = new List<ProductModel>();
            var html = "";
            //todo move to service and create separate method for string result
            switch (SettingsDesign.RelatedProductSourceType)
            {
                case SettingsDesign.eRelatedProductSourceType.Default:
                    products = ProductService.GetRelatedProducts(_product.ProductId, _type);
                    break;

                case SettingsDesign.eRelatedProductSourceType.FromCategory:

                    products = ProductService.GetRelatedProducts(_product.ProductId, _type);
                    if (products == null || products.Count == 0)
                    {
                        products = ProductService.GetRelatedProductsFromCategory(_product, _type);
                    }
                    break;

                case SettingsDesign.eRelatedProductSourceType.FromModule:
                    var module = AttachedModules.GetModules<IModuleRelatedProducts>().FirstOrDefault();
                    if (module != null)
                    {
                        var instance = (IModuleRelatedProducts)Activator.CreateInstance(module);

                        products = instance.GetRelatedProducts(_product, _type);

                        if (products == null || products.Count == 0)
                            html = instance.GetRelatedProductsHtml(_product, _type);
                    }

                    // get default related products
                    if ((products == null || products.Count == 0) && string.IsNullOrEmpty(html))
                    {
                        products = ProductService.GetRelatedProducts(_product.ProductId, _type);
                    }
                    break;
            }
            

            var productModels = new List<ProductModel>();

            if (products != null && products.Count > 0)
            {
                foreach (var product in products)
                    product.BriefDescription = null;

                productModels = products.Where(p => p.ProductId != _product.ProductId).ToList();
            }

            var model = new RelatedProductsViewModel()
            {
                Products = new ProductViewModel(productModels, _isMobile),
                Html = html,
                RelatedType = _type.ToString().ToLower()
            };

            model.Products.DisplayPhotoPreviews = false;

            return model;
        }
               
    }
}