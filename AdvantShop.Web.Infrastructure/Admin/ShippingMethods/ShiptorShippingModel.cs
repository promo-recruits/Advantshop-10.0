using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Shiptor;
using AdvantShop.Shipping.Shiptor.Api;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("Shiptor")]
    public class ShiptorShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string ApiKey
        {
            get { return Params.ElementOrDefault(ShiptorTemplate.ApiKey); }
            set { Params.TryAddValue(ShiptorTemplate.ApiKey, value.DefaultOrEmpty()); }
        }

        public string PaymentCodCardId
        {
            get { return Params.ElementOrDefault(ShiptorTemplate.PaymentCodCardId); }
            set { Params.TryAddValue(ShiptorTemplate.PaymentCodCardId, value.TryParseInt(true).ToString() ?? string.Empty); }
        }

        public List<SelectListItem> PaymentsCod
        {
            get
            {
                var paymentsCod = PaymentService.GetAllPaymentMethods(false)
                    .Where(x => x.PaymentKey.Equals("CashOnDelivery", System.StringComparison.OrdinalIgnoreCase))
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.PaymentMethodId.ToString() })
                    .ToList();

                paymentsCod.Insert(0, new SelectListItem() { Text = "-", Value = string.Empty });

                paymentsCod.Where(x => x.Value == PaymentCodCardId).ForEach(x => x.Selected = true);

                return paymentsCod;
            }
        }

        public bool WithInsure
        {
            get { return Params.ElementOrDefault(ShiptorTemplate.WithInsure).TryParseBool(); }
            set { Params.TryAddValue(ShiptorTemplate.WithInsure, value.ToString()); }
        }

        public string PickupName
        {
            get { return Params.ElementOrDefault(ShiptorTemplate.PickupName) ?? "Самовывоз"; }
            set { Params.TryAddValue(ShiptorTemplate.PickupName, value.DefaultOrEmpty()); }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(ShiptorTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(ShiptorTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public bool StatusesSync
        {
            get { return Params.ElementOrDefault(ShiptorTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(ShiptorTemplate.StatusesSync, value.ToString()); }
        }

        private Dictionary<string, int?> _statusesReference;
        public Dictionary<string, int?> StatusesReference
        {
            get
            {
                var statusesReference = Params.ElementOrDefault(ShiptorTemplate.StatusesReference);
                if (!string.IsNullOrEmpty(statusesReference))
                {
                    string[] arr = null;
                    _statusesReference =
                        statusesReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToDictionary(x => (arr = x.Split(','))[0],
                                x => arr.Length > 1 ? arr[1].TryParseInt(true) : null);
                }
                else
                    _statusesReference = new Dictionary<string, int?>();

                return _statusesReference;
            }
            set
            {
            }
        }

        public Dictionary<string, string> StatusesReferenceVal
        {
            get { return null; }
            set
            {
                var statusesReference = "";
                foreach(var key in value.Keys)
                {
                    if (value[key].IsNotEmpty())
                        statusesReference += string.Format("{0},{1};", key, value[key]);
                }

                Params.TryAddValue(ShiptorTemplate.StatusesReference, statusesReference);
            }
        }

        private Dictionary<string, string> _statuses;
        public Dictionary<string, string> Statuses
        {
            get
            {
                if (_statuses == null)
                {
                    if (ApiKey.IsNotEmpty())
                    {
                        var shiptorCheckoutApiService = new ShiptorCheckoutApiService(ApiKey);
                        var shiptorStatuses = shiptorCheckoutApiService.GetStatusList();
                        if (shiptorStatuses != null)
                            _statuses = shiptorStatuses.Values.ToDictionary(x => x.Code, x => x.Name);
                    }

                    if (_statuses == null) {
                        _statuses = new Dictionary<string, string>();
                    }
                }

                return _statuses;
            }
        }

        public List<SelectListItem> ListStatuses
        {
            get
            {
                var statuses = OrderStatusService.GetOrderStatuses()
                    .Select(x => new SelectListItem() { Text = x.StatusName, Value = x.StatusID.ToString() }).ToList();

                statuses.Insert(0, new SelectListItem() { Text = "", Value = "" });

                return statuses;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                yield return new ValidationResult("Укажите виджет ключ", new[] { "ApiKey" });
            }
        }
    }
}
