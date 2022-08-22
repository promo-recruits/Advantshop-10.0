﻿using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository;
using AdvantShop.Saas;
using AdvantShop.Shipping;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Orders;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.Boxberry;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.DDelivery;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.Grastin;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.Hermes;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.OzonRocket;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.Pec;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.PecEasyway;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.PickPoint;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.RussianPost;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.Sdek;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.Shiptor;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.YandexDelivery;
using AdvantShop.Web.Admin.Handlers.Orders.Shippings.YandexNewDelivery;
using AdvantShop.Web.Admin.Models.Orders;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;
using AdvantShop.Web.Admin.ViewModels.Orders;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Controllers.Orders
{
    [Auth(RoleAction.Orders)]
    public partial class OrdersController : BaseAdminController
    {
        #region Orders List

        public ActionResult Index(OrdersFilterModel filter)
        {
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var order = OrderService.GetOrder(filter.Search.TryParseInt()) ?? OrderService.GetOrderByNumber(filter.Search);
                if (order != null)
                    return RedirectToAction("Edit", new { id = order.OrderID });
            }

            var model = new OrdersViewModel()
            {
                PreFilter = filter.StatusId == null ? filter.FilterBy : default(OrdersPreFilterType?),
                EnableMangers = SettingsCheckout.EnableManagersModule &&
                    (!SaasDataService.IsSaasEnabled || (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveCrm)),
                StatusId = filter.StatusId,
                OrderStatuses = OrderStatusService.GetOrderStatuses().Where(x => x.ShowInMenu).ToList()
            };

            SetMetaInformation(T("Admin.Orders.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.OrdersCtrl);

            return View("List", model);
        }

        /// <summary>
        /// Orders Paging
        /// </summary>
        public JsonResult GetOrders(OrdersFilterModel model)
        {
            return Json(new GetOrdersHandler(model).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteOrder(int orderId)
        {
            OrderService.DeleteOrder(orderId);
            return JsonOk();
        }

        #region Commands

        private void Command(OrdersFilterModel command, Action<int, OrdersFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetOrdersHandler(command).GetItemsIds("[Order].OrderID");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteOrders(OrdersFilterModel command)
        {
            Command(command, (id, c) => OrderService.DeleteOrder(id));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult MarkPaid(OrdersFilterModel command)
        {
            Command(command, (id, c) => OrderService.PayOrder(id, true));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult MarkNotPaid(OrdersFilterModel command)
        {
            Command(command, (id, c) => OrderService.PayOrder(id, false));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeStatus(OrdersFilterModel command, int newOrderStatusId, string statusBasis)
        {
            Command(command, (id, c) => OrderStatusService.ChangeOrderStatus(id, newOrderStatusId, statusBasis));
            return JsonOk();
        }

        public JsonResult GetOrderStatuses()
        {
            var statuses = OrderStatusService.GetOrderStatuses();

            return Json(statuses.Select(x => new { label = x.StatusName, value = x.StatusID.ToString(), }));
        }

        public JsonResult GetOrderPaymentMethods()
        {
            var methods = SQLDataAccess.Query<string>("SELECT distinct Name FROM [Order].[PaymentMethod]").ToList();

            return Json(methods.Select(method => new { label = method, value = method }));
        }

        public JsonResult GetOrderShippingMethods()
        {
            var methods = OrderService.GetShippingMethodNamesFromOrder();

            return Json(methods.Select(method => new { label = method, value = method }));
        }

        public JsonResult GetOrderSources()
        {
            var sources = OrderSourceService.GetOrderSources();

            return Json(sources.Select(x => new { label = x.Name, value = x.Id }));
        }

        #endregion Commands

        #endregion Orders List

        #region Add | Edit Order

        public ActionResult Add(string customerId, string phone)
        {
            var model = new GetOrder(customerId, phone).Execute();

            SetMetaInformation(T("Admin.Orders.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.OrderCtrl);

            return View("AddEdit", model);
        }

        public ActionResult Edit(int id)
        {
            var model = new GetOrder(true, id).Execute();
            if (model == null)
                return RedirectToAction("Index");

            SetMetaInformation(T(!model.Order.IsDraft ? "Admin.Orders.AddEdit.OrderTitle" : "Admin.Orders.AddEdit.OrderDraftTitle", model.Order.Number));
            SetNgController(NgControllers.NgControllersTypes.OrderCtrl);

            return View("AddEdit", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(OrderModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new UpdateOrder(model).Execute();
                if (result)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return RedirectToAction("Edit", new { id = model.OrderId });
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.Orders.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.OrderCtrl);

            return RedirectToAction("Edit", new { id = model.OrderId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(OrderModel model)
        {
            if (ModelState.IsValid)
            {
                var orderId = 0;
                var order = model.OrderId != 0
                                ? OrderService.GetOrder(model.OrderId)
                                : null;
                if (order == null)
                {
                    var result = new SaveOrderDraft(model.Order).Execute();
                    if (result != null)
                        orderId = result.OrderId;
                }
                else
                {
                    model.Order.IsDraft = false;
                    var result = new UpdateOrder(model).Execute();

                    if (result)
                        orderId = order.OrderID;
                }

                if (orderId != 0)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return RedirectToAction("Edit", new { id = orderId });
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.Orders.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.OrderCtrl);

            return View("AddEdit", model);
        }

        public ActionResult PopupOrderCustomer(int? orderId)
        {
            var model = orderId.HasValue ? new GetOrder(true, orderId.Value).Execute() : null;
            if (model == null)
                return new EmptyResult();

            return PartialView("_PopupOrderCustomer", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveCustomer(OrderCustomerModel model)
        {
            var result = new SaveOrderCustomer(model).Execute();
            return result ? JsonOk() : JsonError("Ошибка при сохранении");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveDraft(OrderDraftModel model)
        {
            var result = new SaveOrderDraft(model).Execute();
            return Json(new { result = true, orderId = result.OrderId, customerId = result.CustomerId });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddOrderFromCart(Guid customerId)
        {
            return ProcessJsonResult(new AddOrderFromCart(customerId));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateOrderBonusCard(int orderId)
        {
            return ProcessJsonResult(new UpdateOrderTotal(orderId, null));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdatePaymentDetails(int orderId, PaymentDetails paymentDetails)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return JsonError();

            if (order.PaymentDetails == null)
                order.PaymentDetails = new PaymentDetails();

            order.PaymentDetails.INN = paymentDetails.INN;
            order.PaymentDetails.CompanyName = paymentDetails.CompanyName;
            order.PaymentDetails.Phone = paymentDetails.Phone;
            order.PaymentDetails.Contract = paymentDetails.Contract;
            order.PaymentDetails.IsCashOnDeliveryPayment = paymentDetails.IsCashOnDeliveryPayment;
            order.PaymentDetails.IsPickPointPayment = paymentDetails.IsPickPointPayment;

            OrderService.UpdatePaymentDetails(order.OrderID, order.PaymentDetails);

            return JsonOk();
        }

        #region OrderItems

        [HttpGet]
        public JsonResult GetOrderItems(OrderItemsFilterModel model)
        {
            return Json(new GetOrderItems(model).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddOrderItems(int orderId, List<int> offerIds)
        {
            var order = OrderService.GetOrder(orderId);

            if (order == null || offerIds == null || offerIds.Count == 0 || !OrderService.CheckAccess(order))
                return Json(new { result = false });

            var saveChanges = new AddOrderItems(order, offerIds).Execute();

            var result = saveChanges && new UpdateOrderItems(order, resetOrderCargoParams: true).Execute();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderItemAdded);

            return Json(new { result });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateOrderItem(OrderItemModel model, string priceString)
        {
            var order = OrderService.GetOrder(model.OrderId);
            if (order == null)
                return JsonError();

            var orderItem = order.OrderItems.Find(x => x.OrderItemID == model.OrderItemId);
            if (orderItem == null)
                return JsonError();

            if (!string.IsNullOrEmpty(priceString))
            {
                orderItem.Price = priceString.Replace(" ", "").TryParseFloat();
            }
            var resetOrderCargoParams = orderItem.Amount != model.Amount;
            orderItem.Amount = model.Amount;

            var result = new UpdateOrderItems(order, resetOrderCargoParams).Execute();

            return Json(new { result });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteOrderItem(int orderid, int orderItemId)
        {
            var order = OrderService.GetOrder(orderid);
            if (order == null)
                return JsonError();

            var index = order.OrderItems.FindIndex(x => x.OrderItemID == orderItemId);
            if (index == -1)
                return JsonError();

            order.OrderItems.RemoveAt(index);

            var result = new UpdateOrderItems(order, resetOrderCargoParams: true).Execute();

            return Json(new { result });
        }

        public JsonResult GetOrderItemsSummary(int orderId)
        {
            return Json(new GetOrderItemsSummary(orderId).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeOrderItemCustomOptions(int orderItemId, string customOptionsXml, string artno)
        {
            var orderItem = OrderService.GetOrderItem(orderItemId);
            if (orderItem == null)
                return JsonError("Позиция не найдена");

            var order = OrderService.GetOrder(orderItem.OrderID);
            if (order == null)
                return JsonError("Заказ не найден");

            var item = order.OrderItems.Find(x => x.OrderItemID == orderItemId);

            var saveChanges = new ChangeOrderItemCustomOptions(item, order.OrderCurrency, customOptionsXml, artno).Execute();

            var result = saveChanges && new UpdateOrderItems(order).Execute();

            return Json(new { result });
        }

        #endregion OrderItems

        #region OrderCertificates

        public JsonResult GetOrderCertificates(int orderId)
        {
            var items = GiftCertificateService.GetOrderCertificates(orderId).Select(x => new
            {
                x.CertificateId,
                x.CertificateCode,
                x.Sum,
                x.ApplyOrderNumber
            });
            return Json(new { DataItems = items });
        }

        #endregion OrderCertificates

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateDimesions(int orderId, float? width, float? height, float? length)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return JsonError();

            if (width == null || height == null || length == null)
            {
                order.TotalWidth = order.TotalHeight = order.TotalLength = null;
            }
            else
            {
                order.TotalWidth = width;
                order.TotalHeight = height;
                order.TotalLength = length;
            }

            OrderService.UpdateOrderMain(order, trackChanges: !order.IsDraft);

            var dimensions = MeasureHelper.GetDimensions(order);

            return JsonOk(new
            {
                length = dimensions[0],
                width = dimensions[1],
                height = dimensions[2],
                IsNotEditedDimensions = order.TotalWidth == null && order.TotalHeight == null && order.TotalLength == null,
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateWeight(int orderId, float? weight)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return JsonError();

            order.TotalWeight = weight;

            OrderService.UpdateOrderMain(order, trackChanges: !order.IsDraft);

            var totalWeight = MeasureHelper.GetTotalWeight(order, order.OrderItems);

            return JsonOk(new
            {
                weight = totalWeight,
                IsNotEditedWeight = order.TotalWeight == null,
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCustomerComment(int orderId, string customerComment)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return JsonError();

            order.CustomerComment = customerComment.DefaultOrEmpty();
            OrderService.UpdateOrderMain(order, trackChanges: !order.IsDraft);

            return JsonOk();
        }

        #region Shippings

        [HttpGet]
        public JsonResult GetShippings(int id, string country, string city, string region, string zip, string district)
        {
            return Json(new GetShippings(id, country, city, district, region, zip).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CalculateShipping(int id, string country, string city, string district, string region, string zip, BaseShippingOption shipping)
        {
            var model = new GetShippings(id, country, city, district, region, zip, shipping, getAll: false, applyPay: false).Execute();

            var option = model.Shippings != null ? model.Shippings.FirstOrDefault(x => x.Id == shipping.Id) : null;

            if (option != null)
            {
                option.Update(shipping);

                var order = id != 0 ? OrderService.GetOrder(id) : null;
                if (order != null)
                {
                    var preCoast = order.OrderItems.Sum(x => x.Amount * x.Price) -
                                    (order.GetOrderDiscountPrice() + order.BonusCost) + option.FinalRate;

                    var paymentOption = order.PaymentMethod != null
                        ? order.PaymentMethod.GetOption(option, preCoast)
                        : null;

                    if (paymentOption != null)
                        option.ApplyPay(paymentOption);
                }
            }

            if (option != null)
                option.ManualRate = option.FinalRate;

            if (option == null && shipping.MethodId == 0)
                option = shipping;

            return Json(new { selectShipping = option });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveShipping(int id, string country, string city, string district, string region, string zip, BaseShippingOption shipping)
        {
            var order = OrderService.GetOrder(id);
            if (order == null || shipping == null)
                return JsonError();

            BaseShippingOption option;

            if (shipping.IsCustom == false)
            {
                var model = new GetShippings(id, country, city, district, region, zip, shipping).Execute();

                option = model.Shippings != null ? model.Shippings.FirstOrDefault(x => x.Id == shipping.Id) : null;
                if (option == null)
                    return JsonError("Выбранный метод не найден");

                option.Update(shipping);

                var preCoast = order.OrderItems.Sum(x => x.Amount * x.Price) -
                                (order.GetOrderDiscountPrice() + order.BonusCost) + option.FinalRate;

                var paymentOption = order.PaymentMethod != null
                    ? order.PaymentMethod.GetOption(option, preCoast)
                    : null;

                if (paymentOption != null)
                    option.ApplyPay(paymentOption);

                option.ManualRate = shipping.ManualRate;
            }
            else
            {
                option = shipping;
            }

            new SaveShipping(order, country, city, district, region, option).Execute();

            return JsonOk();
        }

        [HttpGet]
        public JsonResult GetDeliveryTime(int id)
        {
            var order = OrderService.GetOrder(id);
            if (order == null)
                return JsonError();

            return Json(new
            {
                DeliveryDate = order.DeliveryDate != null ? order.DeliveryDate.Value.ToString("dd.MM.yyyy") : "",
                DeliveryTime = order.DeliveryTime
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveDeliveryTime(int id, string deliveryDate, string deliveryTime)
        {
            var order = OrderService.GetOrder(id);
            if (order == null)
                return JsonError();

            order.DeliveryDate = !string.IsNullOrWhiteSpace(deliveryDate) ? deliveryDate.TryParseDateTime() : default(DateTime?);
            order.DeliveryTime = deliveryTime;

            var trackChanges = !order.IsDraft;

            OrderService.UpdateOrderMain(order, updateModules: false, trackChanges: trackChanges);

            return JsonOk();
        }

        #endregion Shippings

        #region Payments

        [HttpGet]
        public JsonResult GetPayments(int orderId, string country, string city, string region, string district)
        {
            return Json(new { payments = new GetPayments(orderId, country, city, region, district).Execute() });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SavePayment(int orderId, string country, string city, string district, string region, BasePaymentOption payment)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null || payment == null)
                return JsonError();

            new SavePayment(order, country, city, district, region, payment).Execute();

            return JsonOk();
        }

        #endregion Payments

        #region Discount

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeDiscount(int orderId, float orderDiscount, bool isValue)
        {
            if (!isValue && (orderDiscount < 0 || orderDiscount > 100))
                return JsonError();

            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return JsonError();

            if (!isValue)
            {
                order.OrderDiscount = orderDiscount;
                order.OrderDiscountValue = 0;
            }
            else
            {
                order.OrderDiscount = 0;
                order.OrderDiscountValue = orderDiscount;
            }

            new UpdateOrderTotal(order).Execute();

            return JsonOk();
        }

        #endregion Discount

        #region Bonuses

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UseBonuses(int orderId, float bonusesAmount)
        {
            return ProcessJsonResult(new UpdateOrderTotal(orderId, bonusesAmount));
        }

        #endregion Bonuses

        #region Status

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeOrderStatus(int orderId, int statusId, string basis)
        {
            var order = OrderService.GetOrder(orderId);
            var status = OrderStatusService.GetOrderStatus(statusId);

            if (order == null || status == null || !OrderService.CheckAccess(order) || order.OrderStatusId == statusId)
                return JsonError();

            if (!string.IsNullOrEmpty(basis))
            {
                OrderService.UpdateStatusComment(orderId, basis, trackChanges: !order.IsDraft);
            }
            else
            {
                basis = order.StatusComment;
            }

            OrderStatusService.ChangeOrderStatus(orderId, statusId, basis);

            TrialService.TrackEvent(TrialEvents.ChangeOrderStatus, "");
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderStatusChanged);

            return Json(new
            {
                result = true,
                color = status.Color,
                basis = basis,
                isNotifyUserEmail = !status.Hidden && order.OrderCustomer.Email.IsNotEmpty(),
                isNotifyUserSms = !status.Hidden && order.OrderCustomer.Phone.IsNotEmpty() &&
                                    SmsNotifier.HasSmsTemplateOnOrderStatus(status.StatusID)
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult NotifyStatusChanged(int orderId, string type)
        {
            return Json(new { result = new NotifyStatusChanged(orderId, type).Exectute() });
        }

        #endregion Status

        #region Paied

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetPaied(int orderId, bool paid)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null || !OrderService.CheckAccess(order))
                return JsonError();

            OrderService.PayOrder(orderId, paid, trackChanges: !order.IsDraft);

            return JsonOk();
        }

        #endregion Paied

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetDate(int orderId, DateTime date)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null || !OrderService.CheckAccess(order))
                return JsonError();

            if (date == DateTime.MinValue)
            {
                var d = DateTime.Now;
                date = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Kind);
            }

            order.OrderDate = date;

            OrderService.UpdateOrderMain(order, !order.IsDraft, changedBy: new OrderChangedBy(CustomerContext.CurrentCustomer), trackChanges: !order.IsDraft);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult setManagerConfirmed(int orderId, bool isManagerConfirmed)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null || !OrderService.CheckAccess(order))
                return JsonError();

            order.ManagerConfirmed = isManagerConfirmed;

            OrderService.UpdateOrderMain(order, !order.IsDraft, changedBy: new OrderChangedBy(CustomerContext.CurrentCustomer), trackChanges: !order.IsDraft);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderConfirmedByManager);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetUseIn1C(int orderId, bool useIn1C)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null || !OrderService.CheckAccess(order) || !Settings1C.Enabled)
                return JsonError();

            order.UseIn1C = useIn1C;

            OrderService.UpdateOrderMain(order, !order.IsDraft, changedBy: new OrderChangedBy(CustomerContext.CurrentCustomer), trackChanges: !order.IsDraft);

            return JsonOk();
        }

        #region Save status and admin comments, tracknumber

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveOrderInfo(int orderId, int? managerId, string statusComment, string adminOrderComment, string trackNumber, int orderSourceId)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null || !OrderService.CheckAccess(order))
                return JsonError();

            var isChangeManager = order.ManagerId != managerId;

            order.ManagerId = managerId;

            order.StatusComment = statusComment.DefaultOrEmpty();
            OrderService.UpdateStatusComment(order.OrderID, order.StatusComment);

            order.AdminOrderComment = adminOrderComment.DefaultOrEmpty();
            order.TrackNumber = trackNumber.DefaultOrEmpty();

            var orderSource = OrderSourceService.GetOrderSource(orderSourceId);
            if (orderSource != null)
                order.OrderSourceId = orderSourceId;

            OrderService.UpdateOrderMain(order);

            if (isChangeManager)
            {
                if (managerId.HasValue)
                    OrderService.SendSetOrderManagerMail(order.OrderID, order.ManagerId.Value);
                BizProcessExecuter.OrderManagerAssigned(order);
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateAdminComment(int orderId, string adminOrderComment)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null || !OrderService.CheckAccess(order))
                return JsonError();

            if (order.AdminOrderComment != adminOrderComment.DefaultOrEmpty())
            {
                OrderService.UpdateAdminOrderComment(orderId, adminOrderComment, trackChanges: !order.IsDraft);
            }

            return JsonOk();
        }

        #endregion Save status and admin comments, tracknumber

        #region Status History

        [HttpGet]
        public JsonResult GetOrderStatusHistory(int orderId)
        {
            var items =
                OrderStatusService.GetOrderStatusHistory(orderId).OrderByDescending(item => item.Date).Select(x => new
                {
                    Date = Localization.Culture.ConvertDate(x.Date),
                    x.PreviousStatus,
                    x.NewStatus,
                    x.CustomerName,
                    x.Basis
                });
            return Json(new { DataItems = items });
        }

        #endregion Status History

        #region Order History

        [HttpGet]
        public JsonResult GetOrderHistory(int orderId)
        {
            var items =
                OrderHistoryService.GetList(orderId).Select(x => new
                {
                    ModificationTime = x.ModificationTime,
                    ModificationTimeFormatted = Localization.Culture.ConvertDate(x.ModificationTime),
                    x.Parameter,
                    x.ParameterDescription,
                    x.OldValue,
                    x.NewValue,
                    x.ManagerId,
                    x.ManagerName,
                    IsEmployee = x.CustomerRole == Role.Administrator || x.CustomerRole == Role.Moderator
                });
            return Json(new { DataItems = items });
        }

        #endregion Order History

        #region Cpoupon

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeCoupon(int orderId, string couponCode)
        {
            if (couponCode.IsNullOrEmpty())
                return JsonError();

            var coupon = CouponService.GetCouponByCode(couponCode);
            if (coupon == null)
                return JsonError();

            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return JsonError();

            order.Coupon = new OrderCoupon()
            {
                Code = coupon.Code,
                Type = coupon.Type,
                Value = coupon.Value
            };
            foreach (var item in order.OrderItems)
            {
                if (item.ProductID.HasValue && CouponService.IsCouponAppliedToProduct(coupon.CouponID, item.ProductID.Value))
                {
                    item.IsCouponApplied = true;
                }
            }
            var oldOrderItems = OrderService.GetOrderItems(order.OrderID);
            OrderService.AddUpdateOrderItems(order.OrderItems, oldOrderItems, order, trackChanges: !order.IsDraft);

            coupon.ActualUses += 1;
            CouponService.UpdateCoupon(coupon);

            new UpdateOrderTotal(order).Execute();

            return JsonOk();
        }

        public JsonResult RemoveCoupon(int orderId)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return JsonError();

            if (order.Coupon != null)
            {
                var coupon = CouponService.GetCouponByCode(order.Coupon.Code);
                if (coupon != null)
                {
                    coupon.ActualUses -= 1;
                    CouponService.UpdateCoupon(coupon);
                }
            }

            order.Coupon = null;

            new UpdateOrderTotal(order).Execute();

            return JsonOk();
        }

        #endregion Cpoupon

        #region Certificate

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeCertificate(int orderId, string code)
        {
            if (code.IsNullOrEmpty())
                return JsonError();

            var certificate = GiftCertificateService.GetCertificateByCode(code);
            if (certificate == null)
                return JsonError();

            if (certificate.Used)
                return JsonError(T("Admin.Orders.CerticateUsed"));

            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return JsonError();

            order.Certificate = new OrderCertificate()
            {
                Code = certificate.CertificateCode,
                Price = certificate.Sum
            };

            certificate.ApplyOrderNumber = order.Number;
            certificate.Used = true;

            GiftCertificateService.UpdateCertificateById(certificate);

            new UpdateOrderTotal(order).Execute();

            return JsonOk();
        }

        public JsonResult RemoveCertificate(int orderId)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return JsonError();

            if (order.Certificate != null)
            {
                var certificate = GiftCertificateService.GetCertificateByCode(order.Certificate.Code);
                if (certificate != null)
                {
                    certificate.ApplyOrderNumber = null;
                    certificate.Used = false;

                    GiftCertificateService.UpdateCertificateById(certificate);
                }
            }

            order.Certificate = null;

            new UpdateOrderTotal(order).Execute();

            return JsonOk();
        }

        #endregion Certificate

        #region TemplatesDocx

        public ActionResult GenerateTemplates(GenerateTemplatesDocxModel model)
        {
            var handler = new GenerateTemplatesDocx(model);
            var result = handler.Execute();

            if (result != null)
            {
                var resultData = (Tuple<string, string>)result;
                return FileDeleteOnUpload(resultData.Item1, "application/octet-stream", System.IO.Path.GetFileName(resultData.Item1), () => Helpers.FileHelpers.DeleteDirectory(resultData.Item2));
            }

            if (handler.Errors != null)
                foreach (var error in handler.Errors)
                    ShowMessage(NotifyType.Error, error);
            else
                ShowMessage(NotifyType.Error, "Не удалось создать документ(ы)");

            return RedirectToAction("Edit", new { id = model.OrderId });
            // return JsonError(handler.Errors.ToArray());
        }

        #endregion TemplatesDocx

        [ChildActionOnly]
        public ActionResult ClientInfo(OrderModel orderModel)
        {
            if (!orderModel.IsEditMode)
                return new EmptyResult();

            var model = new GetClientInfo(orderModel).Execute();
            return PartialView("_ClientInfo", model);
        }

        #endregion Add | Edit Order

        #region Send Billing Link

        public JsonResult GetBillingLink(GetBillingLinkModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var order = model.Order;
            var hash = OrderService.GetBillingLinkHash(order);
            var billingLink = UrlService.GetUrl("checkout/billing?code=" + order.Code + "&hash=" + hash);

            return JsonOk(new
            {
                link = billingLink,
                shortLink = order.PayCode.IsNotEmpty() ? UrlService.GetUrl("pay/" + order.PayCode) : null,
                showSendToCustomerLink = order.OrderCustomer != null && !string.IsNullOrEmpty(order.OrderCustomer.Email)
            });
        }

        public JsonResult GenerateShortBillingLink(GetBillingLinkModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var order = model.Order;
            if (order.PayCode.IsNullOrEmpty())
                order.PayCode = OrderService.GeneratePayCode(order.OrderID);
            var shortLink = UrlService.GetUrl("pay/" + order.PayCode);

            return JsonOk(shortLink);
        }

        public JsonResult GetBillingLinkMailTemplate(GetBillingLinkMailModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var order = model.Order;
            var hash = OrderService.GetBillingLinkHash(order);
            var billingLink = UrlService.GetUrl("checkout/billing?code=" + order.Code + "&hash=" + hash);

            var orderTable =
                OrderService.GenerateOrderItemsHtml(order.OrderItems, order.OrderCurrency,
                    order.OrderItems.Sum(x => x.Price * x.Amount),
                    order.OrderDiscount, order.OrderDiscountValue, order.Coupon,
                    order.Certificate, order.TotalDiscount, order.ShippingCost,
                    order.PaymentCost, order.TaxCost, order.BonusCost, 0);

            var mailTemplate =
                new BillingLinkMailTemplate(order.OrderID, order.Number, order.Code.ToString(),
                    order.OrderCustomer.FirstName, OrderService.GenerateCustomerContactsHtml(order.OrderCustomer),
                    hash, "", orderTable, order.Manager != null ? order.Manager.FullName : string.Empty, order.PayCode,
                    order.ArchivedShippingName + (order.OrderPickPoint != null ? " (" + order.OrderPickPoint.PickPointAddress + ")" : ""));
            mailTemplate.BuildMail();

            return Json(new { result = true, link = billingLink, subject = mailTemplate.Subject, text = mailTemplate.Body });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendBillingLink(SendBillingLinkMailModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var order = model.Order;

            MailService.SendMailNow(order.OrderCustomer.CustomerID, order.OrderCustomer.Email, model.Subject, model.Text, true, (int)MailType.OnBillingLink);
            MailService.SendMailNow(Guid.Empty, SettingsMail.EmailForOrders, model.Subject, model.Text, true, (int)MailType.OnBillingLink, order.OrderCustomer.Email);

            return Json(new { result = true, message = T("Admin.Orders.PaymentLinkSent") });
        }

        #endregion Send Billing Link

        #region Shippings

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetOrderTrackNumber(int orderId)
        {
            var trackNumber = OrderService.GetOrderTrackNumber(orderId);
            return JsonOk(trackNumber);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateYandexDeliveryOrder(int orderId)
        {
            var model = new CreateYandexDeliveryOrder(orderId).Execute();
            return Json(new { result = model.Result, error = model.Error, message = model.Message });
        }

        #region Sdek

        public ActionResult GetOrderActionsSdek(int orderId)
        {
            var model = new SdekOrderActions(orderId).Execute();

            return PartialView("_OrderActionsSdek", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateSdekOrder(int orderId)
        {
            var model = new CreateSdekOrder(orderId).Execute();
            return Json(new { result = model.Result, error = model.Error, message = model.Message });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SdekDeleteOrder(int orderId)
        {
            var model = new SdekDeleteOrder(orderId).Execute();
            return Json(new { result = model.Result, error = model.Error, message = model.Message });
        }

        public ActionResult SdekOrderPrintForm(int orderId)
        {
            var handler = new SdekOrderPrintForm(orderId);
            var result = handler.Execute();

            if (result == null)
                return Content(handler.Errors == null
                    ? "Не удалось получить файл"
                    : string.Join("\\ ", handler.Errors));

            return FileDeleteOnUpload(result.Item1, "application/pdf"/*, result.Item2*/);
        }

        public ActionResult SdekBarCodeOrder(int orderId)
        {
            var handler = new SdekBarCodeOrder(orderId);
            var result = handler.Execute();

            if (result == null)
                return Content(handler.Errors == null
                    ? "Не удалось получить файл"
                    : string.Join("\\ ", handler.Errors));

            return FileDeleteOnUpload(result.Item1, "application/pdf"/*, result.Item2*/);
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SdekChangeDispatchNumber(int orderId, string dispatchNumber)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null || !OrderService.CheckAccess(order))
                return JsonError("Заказ не найден");

            if (dispatchNumber.IsNullOrEmpty())
                return JsonError("Укажите номер заказа в СДЭК");

            OrderService.AddUpdateOrderAdditionalData(orderId, Shipping.Sdek.Sdek.KeyNameDispatchNumberInOrderAdditionalData, dispatchNumber);
            return JsonOk();
        }

        #endregion Sdek

        #region Boxberry

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateBoxberryOrder(int orderId)
        {
            var model = new BoxberryCreateOrder(orderId).Execute();
            return Json(new { result = model.Result, error = model.Error, message = model.Message });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteBoxberryOrder(int orderId)
        {
            var model = new BoxberryDeleteOrder(orderId).Execute();
            return Json(new { result = model.Result, error = model.Error, message = model.Message });
        }

        #endregion Boxberry

        #region Grastin

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GrastinSendRequestForMark(int orderId)
        {
            if (ModelState.IsValid)
            {
                var handler = new GrastinSendRequestForMark(orderId);

                var result = handler.Execute();
                if (!string.IsNullOrEmpty(result))
                    return JsonOk(new { FileName = System.IO.Path.GetFileName(result) }, T("Admin.Orders.MarkingReceived"));
                else if (handler.Errors != null)
                    return JsonError(handler.Errors.ToArray());
            }
            return JsonError();
        }

        public ActionResult GrastinOrderPrintMark(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(FilePath.FoldersHelper.GetPathAbsolut(FilePath.FolderType.PriceTemp) + fileName))
                return Content("");

            return FileDeleteOnUpload(System.IO.Path.Combine(FilePath.FoldersHelper.GetPathAbsolut(FilePath.FolderType.PriceTemp), fileName), "application/octet-stream", fileName);
        }

        #endregion Grastin

        #region DDelivery

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateDDeliveryOrder(int orderId)
        {
            var model = new DDeliveryCreateOrder(orderId).Execute();
            return Json(new { result = model.Result, error = model.Error, message = model.Message });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CanselDDeliveryOrder(int orderId)
        {
            var model = new DDeliveryCanselOrder(orderId).Execute();
            return Json(new { result = model.Result, error = model.Error, message = model.Message });
        }

        public ActionResult DDeliveryOrderInfo(int orderId)
        {
            var result = new DDeliveryOrderInfo(orderId).Execute();

            if (result == null)
                return Content("");

            return FileDeleteOnUpload(result.Item1, "application/octet-stream", result.Item2);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult IsExistDDeliveryOrder(int orderId)
        {
            var model = new DDeliveryIsExistOrder(orderId).Execute();
            return Json(new { result = model.Result, error = model.Error, message = model.Message });
        }

        #endregion DDelivery

        #region RussianPost

        public ActionResult GetOrderActionsRussianPost(int orderId)
        {
            var model = new RussianPostOrderActions(orderId).Execute();

            return PartialView("_OrderActionsRussianPost", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateRussianPostOrder(int orderId, string additionalAction, string additionalActionData)
        {
            string additionalActionResult;
            object additionalActionDataResult;

            var handler = new RussianPostCreateOrder(orderId, additionalAction, additionalActionData);
            var result = handler.Execute(out additionalActionResult, out additionalActionDataResult);

            if (result)
            {
                return JsonOk(null, "Заказ передан");
            }
            else if (additionalActionResult != null)
            {
                return JsonOk(new { additionalAction = additionalActionResult, additionalActionData = additionalActionDataResult, errors = handler.Errors });
            }
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось передать заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось передать заказ");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteRussianPostOrder(int orderId)
        {
            var handler = new RussianPostDeleteOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ удален из Почты России");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось удалить заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось удалить заказ");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RussianPostGetDocumentsBeforShipment(int orderId)
        {
            if (ModelState.IsValid)
            {
                var handler = new RussianPostGetDocumentsBeforShipment(orderId);

                var result = handler.Execute();
                if (!string.IsNullOrEmpty(result))
                    return JsonOk(new { FileName = System.IO.Path.GetFileName(result) }, "Документы получены");
                else if (handler.Errors != null)
                    return JsonError(handler.Errors.ToArray());
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RussianPostGetDocuments(int orderId)
        {
            if (ModelState.IsValid)
            {
                var handler = new RussianPostGetDocumentsBeforShipment(orderId);

                var result = handler.Execute();
                if (!string.IsNullOrEmpty(result))
                    return JsonOk(new { FileName = System.IO.Path.GetFileName(result) }, "Документы получены");
                else if (handler.Errors != null)
                    return JsonError(handler.Errors.ToArray());
            }
            return JsonError();
        }

        public ActionResult RussianPostGetFileDocuments(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(FilePath.FoldersHelper.GetPathAbsolut(FilePath.FolderType.PriceTemp) + fileName))
                return Content("");

            return FileDeleteOnUpload(FilePath.FoldersHelper.GetPathAbsolut(FilePath.FolderType.PriceTemp) + fileName, "application/octet-stream", fileName);
        }

        #endregion RussianPost

        #region Shiptor

        public ActionResult GetOrderActionsShiptor(int orderId)
        {
            var model = new ShiptorOrderActions(orderId).Execute();

            return PartialView("_OrderActionsShiptor", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateShiptorOrder(int orderId)
        {
            var handler = new ShiptorCreateOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ передан");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось передать заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось передать заказ");
        }

        #endregion Shiptor

        #region YandexNewDelivery

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateYandexNewDeliveryOrder(int orderId)
        {
            var model = new CreateYandexNewDeliveryOrder(orderId).Execute();
            return Json(new { result = model.Result, error = model.Error, message = model.Message });
        }

        #endregion YandexNewDelivery

        #region Hermes

        public ActionResult GetOrderActionsHermes(int orderId)
        {
            var model = new HermesOrderActions(orderId).Execute();

            return PartialView("_OrderActionsHermes", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateHermesOrderStandart(int orderId)
        {
            var handler = new HermesCreateOrderStandart(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ передан");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось передать заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось передать заказ");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateHermesOrderVsd(int orderId)
        {
            var handler = new HermesCreateOrderVsd(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ передан");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось передать заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось передать заказ");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateHermesOrderDrop(int orderId)
        {
            var handler = new HermesCreateOrderDrop(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ передан");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось передать заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось передать заказ");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteHermesOrder(int orderId)
        {
            var handler = new HermesDeleteOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ удален из Hermes");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось удалить заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось удалить заказ");
        }

        #endregion Hermes

        #region PecEasyway

        public ActionResult GetOrderActionsPecEasyway(int orderId)
        {
            var model = new PecEasywayOrderActions(orderId).Execute();

            return PartialView("_OrderActionsPecEasyway", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreatePecEasywayOrder(int orderId)
        {
            var handler = new PecEasywayCreateOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ передан");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось передать заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось передать заказ");
        }

        public ActionResult GetPecEasywayOrderLabel(int orderId)
        {
            var html = new PecEasywayGetOrderLabel(orderId).Execute();

            return Content(html);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CancelPecEasywayOrder(int orderId)
        {
            var handler = new PecEasywayCancelOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ отменен в ПЭК:EASYWAY");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось отменить заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось отменить заказ");
        }

        #endregion PecEasyway

        #region Pec

        public ActionResult GetOrderActionsPec(int orderId)
        {
            var model = new PecOrderActions(orderId).Execute();

            return PartialView("_OrderActionsPec", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreatePecOrder(int orderId)
        {
            var handler = new PecCreateOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ передан");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось передать заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось передать заказ");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CancelPecOrder(int orderId)
        {
            var handler = new PecCancelOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ отменен в ПЭК");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось отменить заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось отменить заказ");
        }

        #endregion Pec

        #region PickPoint

        public ActionResult GetOrderActionsPickPoint(int orderId)
        {
            var model = new PickPointOrderActions(orderId).Execute();

            return PartialView("_OrderActionsPickPoint", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreatePickPointOrder(int orderId)
        {
            var handler = new PickPointCreateOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ передан");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось передать заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось передать заказ");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePickPointOrder(int orderId)
        {
            var handler = new PickPointDeleteOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ удален из PickPoint");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось удалить заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось удалить заказ");
        }

        public ActionResult GetPickPointMakeLabel(int orderId)
        {
            var result = new GetPickPointMakeLabel(orderId).Execute();

            if (result == null)
                return Content("");

            return FileDeleteOnUpload(result.Item1, "application/pdf"/*, result.Item2*/);
        }

        public ActionResult GetPickPointMakeZebraLabel(int orderId)
        {
            var result = new GetPickPointMakeZebraLabel(orderId).Execute();

            if (result == null)
                return Content("");

            return FileDeleteOnUpload(result.Item1, "application/pdf"/*, result.Item2*/);
        }

        #endregion PickPoint

  #region OzonRocket
     
        public ActionResult GetOrderActionsOzonRocket(int orderId)
        {
            var model = new OzonRocketOrderActions(orderId).Execute();

            return PartialView("_OrderActionsOzonRocket", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateOzonRocketOrder(int orderId)
        {
            var handler = new OzonRocketCreateOrUpdateOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ передан");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось передать заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось передать заказ");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CancelOzonRocketOrder(int orderId)
        {
            var handler = new OzonRocketCancelOrder(orderId);
            var result = handler.Execute();
            if (result)
                return JsonOk(null, "Заказ отменен в Ozon Rocket");
            else if (handler.Errors != null)
            {
                handler.Errors.Add("Неудалось отменить заказ");
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError("Неудалось отменить заказ");
        }

        #endregion

        #endregion Shippings

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DisableDesktopAppNotification(string appName)
        {
            if (string.Equals(appName, "viber", StringComparison.OrdinalIgnoreCase))
            {
                SettingsAdmin.ShowViberDesktopAppNotification = false;
            }
            if (string.Equals(appName, "whatsapp", StringComparison.OrdinalIgnoreCase))
            {
                SettingsAdmin.ShowWhatsAppDesktopAppNotification = false;
            }
            return JsonOk();
        }
    }
}
