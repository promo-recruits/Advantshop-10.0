﻿using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.ViewModel.Checkout;
using AdvantShop.ViewModel.Checkout.OrderPay;

namespace AdvantShop.Handlers.Checkout
{
    public class OrderPayHandler
    {
        private readonly Order _order;
        private readonly PaymentMethod _paymentMethod;
        private readonly PageWithPaymentButton _pageWithPaymentButton;

        public OrderPayHandler(Order order, PaymentMethod paymentMethod, PageWithPaymentButton pageWithPaymentButton)
        {
            _order = order;
            _paymentMethod = paymentMethod;
            _pageWithPaymentButton = pageWithPaymentButton;
        }

        public OrderPayModel Execute()
        {
            if (_order is null || _paymentMethod is null || _paymentMethod.ProcessType == ProcessType.None)
                return null;

            var paymentType = AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(_paymentMethod.GetType());
            var typeModel = ReflectionExt.GetTypeByAttributeValue<PaymentOrderPayModelAttribute>(
                typeof(OrderPayModel), 
                atr => atr.Value, paymentType);
            
            var model = typeModel is null 
                ? new OrderPayModel()
                : (OrderPayModel)Activator.CreateInstance(typeModel);
            
            model.Order = _order;
            model.PaymentMethod = _paymentMethod;
            model.PageWithPaymentButton = _pageWithPaymentButton;
            model.PaymentType = paymentType;

            if (_paymentMethod.ProcessType == ProcessType.FormPost)
            {
                model.PaymentForm = _paymentMethod.GetPaymentForm(_order);
                model.ButtonOnClick = $"document.{model.PaymentForm.FormName}.submit();";
            }

            if (_paymentMethod.ProcessType == ProcessType.ServerRequest)
                model.ButtonHref = UrlService.GetUrl("checkout/payredirect/" + _order.Code);
            

            if (_paymentMethod.ProcessType == ProcessType.Javascript)
            {
                model.Javascript = _paymentMethod.ProcessJavascript(_order);
                model.ButtonOnClick = _paymentMethod.ProcessJavascriptButton(_order);
            }
            
            model.ButtonOnClick = 
                model.ButtonOnClick == string.Empty 
                    ? null 
                    : model.ButtonOnClick;

            return model;
        }
    }
}