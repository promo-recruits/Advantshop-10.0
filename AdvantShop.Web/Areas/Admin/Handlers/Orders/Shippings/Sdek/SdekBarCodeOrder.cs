using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Sdek.Api;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Sdek
{
    public class SdekBarCodeOrder
    {
        private readonly int _orderId;
        public List<string> Errors;

        public SdekBarCodeOrder(int orderId)
        {
            _orderId = orderId;
        }

        public Tuple<string, string> Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return null;
            
            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "Sdek")
                return null;
            
            try
            {
                var sdekOrderUuid = OrderService.GetOrderAdditionalData(order.OrderID, Shipping.Sdek.Sdek.KeyNameSdekOrderUuidInOrderAdditionalData);
                var sdekOrderNumber = OrderService.GetOrderAdditionalData(order.OrderID, Shipping.Sdek.Sdek.KeyNameDispatchNumberInOrderAdditionalData);
                var sdek = new Shipping.Sdek.Sdek(shippingMethod, null, null);
                
                var createBarCodeOrderResult = sdek.SdekApiService20.CreateBarCodeOrder(new CreateBarCodeOrder()
                {
                    Orders = new List<CreatePrintFormOrder>()
                    {
                        new CreatePrintFormOrder()
                        {
                            OrderUuid = sdekOrderUuid.TryParseGuid(true),
                            CdekNumber = sdekOrderNumber.TryParseInt(true)
                        }
                    }
                });

                if (createBarCodeOrderResult?.Entity?.Uuid != null)
                {
                    if (createBarCodeOrderResult.Requests.First().State != "INVALID")
                    {
                        GetBarCodeOrderResult getBarCodeOrderResult;
                        int countRequests = 0;
                        do 
                        {
                            getBarCodeOrderResult = sdek.SdekApiService20.GetBarCodeOrder(createBarCodeOrderResult.Entity.Uuid);
                            countRequests++;
                        }  while (getBarCodeOrderResult != null && getBarCodeOrderResult?.Entity?.Statuses?.All(x => x.Code != "INVALID" && x.Code != "READY") == true && countRequests < 10);

                        if (getBarCodeOrderResult?.Entity?.Url.IsNotEmpty() == true)
                        {
                            var path = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
                            FileHelpers.CreateDirectory(path);
                            
                            var fullFilePath = sdek.SdekApiService20.DownloadFile(getBarCodeOrderResult.Entity.Url, path);
                        
                            if (!File.Exists(fullFilePath))
                                return null;

                            return new Tuple<string, string>(fullFilePath, Path.GetFileName(fullFilePath));
                        } 
                        else if (getBarCodeOrderResult?.Requests != null)
                        {
                            Errors = getBarCodeOrderResult.Requests
                                .Where(x => x.Errors != null)
                                .SelectMany(request => request.Errors.Select(error => error.Message))
                                .ToList();
                        }
                    }
                }
                else if (sdek.SdekApiService20.LastActionErrors != null)
                {
                    Errors = sdek.SdekApiService20.LastActionErrors;
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }
    }
}