﻿using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.RussianPost
{
    public class RussianPostGetDocumentsBeforShipment
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public RussianPostGetDocumentsBeforShipment(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public string Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.RussianPost.RussianPost.KeyNameOrderRussianPostIdInOrderAdditionalData);

            if (!string.IsNullOrEmpty(orderAdditionalData))
            {
                var order = OrderService.GetOrder(_orderId);
                if (order != null)
                {
                    var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
                    if (shippingMethod != null &&
                        shippingMethod.ShippingType ==
                        ((ShippingKeyAttribute)
                            typeof(Shipping.RussianPost.RussianPost).GetCustomAttributes(
                                typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        var russianPostMethod = new Shipping.RussianPost.RussianPost(shippingMethod, null, null);

                        var path = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
                        FileHelpers.CreateDirectory(path);

                        var result = russianPostMethod.RussianPostApiService.GetDocumentsBeforShipment(orderAdditionalData.TryParseLong(), path);

                        Errors = russianPostMethod.RussianPostApiService.LastActionErrors;

                        return result;
                    }
                    else
                        Errors.Add("Неверный метод доставки");

                }
                else
                    Errors.Add("Заказ не найден");

            }
            else Errors.Add("Заказ еще не был передан");

            return null;
        }
    }
}
