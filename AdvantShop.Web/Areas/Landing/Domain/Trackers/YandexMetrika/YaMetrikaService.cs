using System;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Domain.Trackers.YandexMetrika
{
    public class YaMetrikaService
    {
        public string YaMetrikaHeadScript(string counterId, string jsScript, bool collectip = true)
        {
            if (string.IsNullOrEmpty(counterId) || string.IsNullOrEmpty(jsScript))
                return null;

            return string.Format(
                "<script>window.yaCounterId=\"{0}\"; window.dataLayer = window.dataLayer || []; {1}</script>\n {2}\n",
                counterId,
                collectip ? "var yaParams={ip_adress: '" + System.Web.HttpContext.Current.Request.UserHostAddress + "'};" : "", 
                jsScript);
        }

        public string CheckoutFinalStepScript(string counterId, string jsScript, IOrder order)
        {
            if (string.IsNullOrEmpty(counterId) || string.IsNullOrEmpty(jsScript))
                return null;

            var ecOrder = OrderService.GetOrder(order.OrderID);

            var actionField = new
            {
                Id = order.Number,
                Shipping = ecOrder.ShippingCostWithDiscount > 0 ? (int)ecOrder.ShippingCostWithDiscount : 0,
                Coupon = order.Coupon != null ? order.Coupon.Code : null
            };

            var products = order.OrderItems.Select(x => new
            {
                Id = x.ArtNo,
                Name = x.Name,
                Price = (int)x.Price,
                Category = x.ProductID != null ? GetCategory(x.ProductID.Value) : null,
                Brand = x.ProductID != null ? GetBrand(x.ProductID.Value) : null,
                Quantity = Convert.ToInt32(x.Amount)
            });

            var result =
                string.Format(
                    "\n<script> \n window.dataLayer.push({{\"ecommerce\": {{ \"purchase\": \n {{ \"actionField\": {0}, \n  \"products\":{1} }} \n }} \n }}); </script>\n ",
                    JsonConvert.SerializeObject(actionField),
                    JsonConvert.SerializeObject(products));

            return result;
        }


        private string GetCategory(int productId)
        {
            var categories = ProductService.GetCategoriesByProductId(productId);
            if (categories.Count > 5)
                categories = categories.Skip(categories.Count - 5).ToList();

            return String.Join("/", categories.Select(x => x.Name));
        }

        private string GetBrand(int productId)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null || product.Brand == null)
                return null;

            return product.Brand.Name;
        }


    }
}
