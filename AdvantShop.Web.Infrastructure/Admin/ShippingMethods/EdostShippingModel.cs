﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Payment;
using AdvantShop.Shipping.Edost;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("Edost")]
    public class EdostShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string ShopId
        {
            get { return Params.ElementOrDefault(EdostTemplate.ShopId); }
            set { Params.TryAddValue(EdostTemplate.ShopId, value.DefaultOrEmpty()); }
        }

        public string Password
        {
            get { return Params.ElementOrDefault(EdostTemplate.Password); }
            set { Params.TryAddValue(EdostTemplate.Password, value.DefaultOrEmpty()); }
        }
        public bool EnabledCod
        {
            get { return Params.ElementOrDefault(EdostTemplate.EnabledCOD).TryParseBool(); }
            set { Params.TryAddValue(EdostTemplate.EnabledCOD, value.ToString()); }
        }
        public bool EnabledPickPoint
        {
            get { return Params.ElementOrDefault(EdostTemplate.EnabledPickPoint).TryParseBool(); }
            set { Params.TryAddValue(EdostTemplate.EnabledPickPoint, value.ToString()); }
        }
        public int ShipIdCod
        {
            get { return Params.ElementOrDefault(EdostTemplate.ShipIdCOD, "0").TryParseInt(); }
            set
            {
                var paymentMethodId = value;

                if (EnabledCod)
                {
                    var payment = PaymentService.GetPaymentMethod(paymentMethodId);
                    if (payment == null)
                    {
                        var methodId = ShippingMethodId;

                        if (methodId == 0 && HttpContext.Current != null && HttpContext.Current.Request["shippingmethodid"] != null)
                            methodId = HttpContext.Current.Request["shippingmethodid"].TryParseInt();

                        var payMethod = new CashOnDelivery
                        {
                            Name = LocalizationService.GetResource("Admin.ShippingMethods.EdostShippingMethodModel.CashOnDeliveryName"),
                            Enabled = true,
                            ShippingMethodId = methodId
                        };
                        var paymentId = PaymentService.AddPaymentMethod(payMethod);

                        Params.TryAddValue(EdostTemplate.ShipIdCOD, paymentId.ToString());
                    }
                }
                else
                {
                    if (paymentMethodId != 0)
                        PaymentService.DeletePaymentMethod(paymentMethodId);

                    Params.TryAddValue(EdostTemplate.ShipIdCOD, "");
                }
            }
        }

        public int ShipIdPickPoint
        {
            get { return Params.ElementOrDefault(EdostTemplate.ShipIdPickPoint).TryParseInt(); }
            set
            {
                var paymentMethodId = value;

                if (EnabledPickPoint)
                {
                    var payment = PaymentService.GetPaymentMethod(paymentMethodId);
                    if (payment == null)
                    {
                        var methodId = ShippingMethodId;

                        if (methodId == 0 && HttpContext.Current != null && HttpContext.Current.Request["shippingmethodid"] != null)
                            methodId = HttpContext.Current.Request["shippingmethodid"].TryParseInt();

                        var payMethod = new PickPoint
                        {
                            Name = LocalizationService.GetResource("Admin.ShippingMethods.EdostShippingMethodModel.OrderPickPointMessage"),
                            Enabled = true,
                            ShippingMethodId = methodId
                        };
                        var paymentId = PaymentService.AddPaymentMethod(payMethod);

                        Params.TryAddValue(EdostTemplate.ShipIdPickPoint, paymentId.ToString());
                    }
                }
                else
                {
                    if (paymentMethodId != 0)
                        PaymentService.DeletePaymentMethod(paymentMethodId);

                    Params.TryAddValue(EdostTemplate.ShipIdPickPoint, "");
                }
            }
        }

        public string Rate
        {
            get { return Params.ElementOrDefault(EdostTemplate.Rate, "1"); }
            set { Params.TryAddValue(EdostTemplate.Rate, value.TryParseFloat().ToString()); }
        }

        public EDemensionsUnit DemensionsUnit
        {
            get { return Params.ElementOrDefault(EdostTemplate.DemensionsUnit, EDemensionsUnit.Centimeters.ToString()).TryParseEnum<EDemensionsUnit>(); }
            set { Params.TryAddValue(EdostTemplate.DemensionsUnit, value.ToString()); }
        }
        public Dictionary<string, string> DemensionsUnitList
        {
            get
            {
                return Enum.GetValues(typeof(EDemensionsUnit))
                     .Cast<object>()
                     .ToDictionary(k => ((Enum)k).ToString(), v => ((Enum)v).StrName());
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ShopId))
            {
                yield return new ValidationResult("Укажите идентификатор магазина", new[] { "ShopId" });
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Укажите пароль", new[] { "Password" });
            }
            if (DemensionsUnit == EDemensionsUnit.None)
            {
                yield return new ValidationResult("Выберите единицы измерения");
            }
        }
    }
}
