﻿using System;
using System.IO;
using AdvantShop.Core.Services.Shipping.DDelivery;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.DDelivery
{
    public class DDeliveryOrderInfo
    {
        private readonly int _orderId;

        public DDeliveryOrderInfo(int orderId)
        {
            _orderId = orderId;
        }

        public Tuple<string, string> Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return null;//new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "DDelivery")
                return null;//new CommandResult() { Error = "Order shipping method is not 'DDelivery' type" };


            var dDeliveryService = new DDeliveryService();

            var ddeliveryOrderNumber = dDeliveryService.GetDDeliveryOrderNumber(order.OrderID);
            if (string.IsNullOrEmpty(ddeliveryOrderNumber))
            {
                return null;//new CommandResult() { Result = false, Message = "Заказ не найден", Error = "Заказ не найден" };
            }

            try
            {
                var dDelivery = new Shipping.DDelivery.DDelivery(shippingMethod, null, null);
                var result = dDelivery.GetOrderInfo(ddeliveryOrderNumber);

                var fileName = string.Format("OrderInfoReport_{0}.txt", DateTime.Now.ToShortDateString());
                var fullFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + "\\" + fileName;

                if (result.Success && result.Data != null)
                {
                    using (var streamWriter = new StreamWriter(fullFilePath))
                    {
                        streamWriter.WriteLine("Сайт :" + result.Data.Site);
                        streamWriter.WriteLine("ID заказа :" + result.Data.OrderId);
                        streamWriter.WriteLine("Тип доставки :" + result.Data.Type.ToString());
                        streamWriter.WriteLine("Компания доставки :" + result.Data.Company);
                        streamWriter.WriteLine("Статус :" + result.Data.Status);
                        streamWriter.WriteLine("Трек номер :" + result.Data.TrackNumber);
                        streamWriter.WriteLine("Адресс для проверки :" + result.Data.TrackingUrl);
                        if (result.Data.Point != null)
                        {
                            streamWriter.WriteLine("Пункт выдачи :");
                            streamWriter.WriteLine("\tНименование :\t" + result.Data.Point.Name);
                            streamWriter.WriteLine("\tКомпания доставки :\t" + result.Data.Point.DeliveryCompanyName);
                            streamWriter.WriteLine("\tОписание входа :\t" + result.Data.Point.DescriptionIn);
                            streamWriter.WriteLine("\tОписание выхода :\t" + result.Data.Point.DescriptionOut);
                            streamWriter.WriteLine("\tАдрес :\t" + result.Data.Point.Adress);
                            //streamWriter.WriteLine("\tРассписание :\t" + result.Data.Point.Schedule);
                            streamWriter.WriteLine("\tИмеется ли примерочная ? :\t" + (result.Data.Point.IsFitting == true ? "Да" : "Нет"));
                            streamWriter.WriteLine("\tОплата наличными возможна ? :\t" + (result.Data.Point.IsCash == true ? "Да" : "Нет"));
                            streamWriter.WriteLine("\tОплата картой возможна ? :\t" + (result.Data.Point.IsCard == true ? "Да" : "Нет"));
                        }
                    }
                }

                return new Tuple<string, string>(fullFilePath, fileName);
                //new CommandResult() { Result = result.Success, Message = result.Message, Obj = result.Data, Error = !result.Success ? result.Message : "" };

            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;//new CommandResult() { Error = "Не удалось получить инофрмацию о заказе: " + ex.Message };
            }
        }
    }
}
