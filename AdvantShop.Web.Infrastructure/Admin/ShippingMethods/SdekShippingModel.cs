using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Orders;
using AdvantShop.Shipping.Sdek;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("Sdek")]
    public class SdekShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string AuthLogin
        {
            get { return Params.ElementOrDefault(SdekTemplate.AuthLogin); }
            set { Params.TryAddValue(SdekTemplate.AuthLogin, value.DefaultOrEmpty()); }
        }
        public string AuthPassword
        {
            get { return Params.ElementOrDefault(SdekTemplate.AuthPassword); }
            set { Params.TryAddValue(SdekTemplate.AuthPassword, value.DefaultOrEmpty()); }
        }

        public string CityFrom
        {
            get { return Params.ElementOrDefault(SdekTemplate.CityFrom); }
            set
            {
                Params.TryAddValue(SdekTemplate.CityFrom, value.DefaultOrEmpty());
                
                // var methodId = ShippingMethodId;
                //
                // if (methodId == 0 && HttpContext.Current != null && HttpContext.Current.Request["shippingmethodid"] != null)
                //     methodId = HttpContext.Current.Request["shippingmethodid"].TryParseInt();
                //
                // var method = Shipping.ShippingMethodService.GetShippingMethod(methodId);
                // var sdek = new Shipping.Sdek.Sdek(method, null, null);
                // Params.TryAddValue(SdekTemplate.CityFromId, SdekService.GetSdekCityId(value.DefaultOrEmpty(), string.Empty, string.Empty, string.Empty, sdek.SdekApiService20, out _, allowedObsoleteFindCity: true).ToString());
            }
        }

        public string CityFromId
        {
            get { return Params.ElementOrDefault(SdekTemplate.CityFromId); }
            set { Params.TryAddValue(SdekTemplate.CityFromId, value.DefaultOrEmpty()); }
        }
        
        public string[] Tariff
        {
            get
            {
                return Params.ContainsKey(SdekTemplate.CalculateTariffs)
                  ? (Params.ElementOrDefault(SdekTemplate.CalculateTariffs) ?? string.Empty).Split(",")
                  : (Params.ElementOrDefault(SdekTemplate.TariffOldParam) ?? string.Empty).Split(",");
            }
            set { Params.TryAddValue(SdekTemplate.CalculateTariffs, value != null ? string.Join(",", value) : string.Empty); }
        }

        public string DeliveryNote
        {
            get { return Params.ElementOrDefault(SdekTemplate.DeliveryNote) ?? "1"; }
            set { Params.TryAddValue(SdekTemplate.DeliveryNote, value.TryParseInt().ToString()); }
        }

        public bool StatusesSync
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(SdekTemplate.StatusesSync, value.ToString()); }
        }

        public bool WithInsure
        {
            get { return Params.ElementOrDefault(SdekTemplate.WithInsure).TryParseBool(); }
            set { Params.TryAddValue(SdekTemplate.WithInsure, value.ToString()); }
        }

        public bool AllowInspection
        {
            get { return Params.ElementOrDefault(SdekTemplate.AllowInspection).TryParseBool(); }
            set { Params.TryAddValue(SdekTemplate.AllowInspection, value.ToString()); }
        }

        public bool ShowPointsAsList
        {
            get { return Params.ElementOrDefault(SdekTemplate.ShowPointsAsList).TryParseBool(); }
            set { Params.TryAddValue(SdekTemplate.ShowPointsAsList, value.ToString()); }
        }

        public bool ShowSdekWidjet
        {
            get { return Params.ElementOrDefault(SdekTemplate.ShowSdekWidjet).TryParseBool(); }
            set { Params.TryAddValue(SdekTemplate.ShowSdekWidjet, value.ToString()); }
        }

        public bool ShowAddressComment
        {
            get { return Params.ElementOrDefault(SdekTemplate.ShowAddressComment).TryParseBool(); }
            set { Params.TryAddValue(SdekTemplate.ShowAddressComment, value.ToString()); }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(SdekTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(SdekTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public bool UseSeller
        {
            get { return Params.ElementOrDefault(SdekTemplate.UseSeller).TryParseBool(); }
            set { Params.TryAddValue(SdekTemplate.UseSeller, value.ToString()); }
        }

        public string SellerAddress
        {
            get { return Params.ElementOrDefault(SdekTemplate.SellerAddress); }
            set { Params.TryAddValue(SdekTemplate.SellerAddress, value.DefaultOrEmpty()); }
        }

        public string SellerName
        {
            get { return Params.ElementOrDefault(SdekTemplate.SellerName); }
            set { Params.TryAddValue(SdekTemplate.SellerName, value.DefaultOrEmpty()); }
        }

        public string SellerINN
        {
            get { return Params.ElementOrDefault(SdekTemplate.SellerINN); }
            set { Params.TryAddValue(SdekTemplate.SellerINN, value.DefaultOrEmpty()); }
        }

        public string SellerPhone
        {
            get { return Params.ElementOrDefault(SdekTemplate.SellerPhone); }
            set { Params.TryAddValue(SdekTemplate.SellerPhone, value.DefaultOrEmpty()); }
        }

        public string SellerOwnershipForm
        {
            get { return Params.ElementOrDefault(SdekTemplate.SellerOwnershipForm); }
            set { Params.TryAddValue(SdekTemplate.SellerOwnershipForm, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> SellerOwnershipForms
        {
            get
            {
                var forms = new List<SelectListItem>
                {
                    new SelectListItem() { Text = "", Value = "" },
                    new SelectListItem() { Text = "Акционерное общество", Value = "9" },
                    new SelectListItem() { Text = "Закрытое акционерное общество", Value = "61" },
                    new SelectListItem() { Text = "Индивидуальный предприниматель", Value = "63" },
                    new SelectListItem() { Text = "Открытое акционерное общество", Value = "119" },
                    new SelectListItem() { Text = "Общество с ограниченной ответственностью", Value = "137" },
                    new SelectListItem() { Text = "Публичное акционерное общество", Value = "147" },
                };

                return forms;
            }
        }

        public string StatusCreated
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusCreated); }
            set { Params.TryAddValue(SdekTemplate.StatusCreated, value.DefaultOrEmpty()); }
        }

        // public string StatusDeleted
        // {
        //     get { return Params.ElementOrDefault(SdekTemplate.StatusDeleted); }
        //     set { Params.TryAddValue(SdekTemplate.StatusDeleted, value.DefaultOrEmpty()); }
        // }

        public string StatusAcceptedAtWarehouseOfSender
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtWarehouseOfSender); }
            set { Params.TryAddValue(SdekTemplate.StatusAcceptedAtWarehouseOfSender, value.DefaultOrEmpty()); }
        }

        public string StatusIssuedForShipmentFromSenderWarehouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusIssuedForShipmentFromSenderWarehouse); }
            set { Params.TryAddValue(SdekTemplate.StatusIssuedForShipmentFromSenderWarehouse, value.DefaultOrEmpty()); }
        }

        public string StatusReturnedToWarehouseOfSender
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusReturnedToWarehouseOfSender); }
            set { Params.TryAddValue(SdekTemplate.StatusReturnedToWarehouseOfSender, value.DefaultOrEmpty()); }
        }

        public string StatusDeliveredToCarrierFromSenderWarehouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusDeliveredToCarrierFromSenderWarehouse); }
            set { Params.TryAddValue(SdekTemplate.StatusDeliveredToCarrierFromSenderWarehouse, value.DefaultOrEmpty()); }
        }

        public string StatusSentToTransitWarehouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusSentToTransitWarehouse); }
            set { Params.TryAddValue(SdekTemplate.StatusSentToTransitWarehouse, value.DefaultOrEmpty()); }
        }

        public string StatusMetAtTransitWarehouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusMetAtTransitWarehouse); }
            set { Params.TryAddValue(SdekTemplate.StatusMetAtTransitWarehouse, value.DefaultOrEmpty()); }
        }

        public string StatusAcceptedAtTransitWarehouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtTransitWarehouse); }
            set { Params.TryAddValue(SdekTemplate.StatusAcceptedAtTransitWarehouse, value.DefaultOrEmpty()); }
        }

        public string StatusReturnedToTransitWarehouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusReturnedToTransitWarehouse); }
            set { Params.TryAddValue(SdekTemplate.StatusReturnedToTransitWarehouse, value.DefaultOrEmpty()); }
        }

        public string StatusIssuedForShipmentInTransitWarehouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusIssuedForShipmentInTransitWarehouse); }
            set { Params.TryAddValue(SdekTemplate.StatusIssuedForShipmentInTransitWarehouse, value.DefaultOrEmpty()); }
        }

        public string StatusDeliveredToCarrierInTransitWarehouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusDeliveredToCarrierInTransitWarehouse); }
            set { Params.TryAddValue(SdekTemplate.StatusDeliveredToCarrierInTransitWarehouse, value.DefaultOrEmpty()); }
        }

        public string StatusSentToSenderCity
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusSentToSenderCity); }
            set { Params.TryAddValue(SdekTemplate.StatusSentToSenderCity, value.DefaultOrEmpty()); }
        }

        public string StatusSentToWarehouseOfRecipient
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusSentToWarehouseOfRecipient); }
            set { Params.TryAddValue(SdekTemplate.StatusSentToWarehouseOfRecipient, value.DefaultOrEmpty()); }
        }

        public string StatusMetAtSenderCity
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusMetAtSenderCity); }
            set { Params.TryAddValue(SdekTemplate.StatusMetAtSenderCity, value.DefaultOrEmpty()); }
        }

        public string StatusMetAtConsigneeWarehouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusMetAtConsigneeWarehouse); }
            set { Params.TryAddValue(SdekTemplate.StatusMetAtConsigneeWarehouse, value.DefaultOrEmpty()); }
        }

        public string StatusAcceptedAtConsigneeWarehouse_AwaitingDelivery
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingDelivery); }
            set { Params.TryAddValue(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingDelivery, value.DefaultOrEmpty()); }
        }

        public string StatusAcceptedAtConsigneeWarehouse_AwaitingFenceByClient
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingFenceByClient); }
            set { Params.TryAddValue(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingFenceByClient, value.DefaultOrEmpty()); }
        }

        public string StatusIssuedForDelivery
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusIssuedForDelivery); }
            set { Params.TryAddValue(SdekTemplate.StatusIssuedForDelivery, value.DefaultOrEmpty()); }
        }

        public string StatusReturnedToConsigneeWarehouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusReturnedToConsigneeWarehouse); }
            set { Params.TryAddValue(SdekTemplate.StatusReturnedToConsigneeWarehouse, value.DefaultOrEmpty()); }
        }

        public string StatusAwarded
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusAwarded); }
            set { Params.TryAddValue(SdekTemplate.StatusAwarded, value.DefaultOrEmpty()); }
        }

        public string StatusNotAwarded
        {
            get { return Params.ElementOrDefault(SdekTemplate.StatusNotAwarded); }
            set { Params.TryAddValue(SdekTemplate.StatusNotAwarded, value.DefaultOrEmpty()); }
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

        public List<SelectListItem> ListTariffs
        {
            get
            {
                var selectedTariff = Tariff.ToList();

                var tariffs = SdekTariffs.Tariffs.Select(x => new SelectListItem() { Text = x.Name, Value = x.TariffId.ToString(), Selected = selectedTariff.Contains(x.TariffId.ToString()) }).ToList();

                //if (!tariffs.Any(x => x.Selected))
                //{
                //    tariffs[0].Selected = true;
                //    Tariff = new[] { tariffs[0].Value };
                //}

                return tariffs;
            }
        }

        public string DefaultCourierCity
        {
            get { return Params.ElementOrDefault(SdekTemplate.DefaultCourierCity) ?? SettingsMain.City; }
            set { Params.TryAddValue(SdekTemplate.DefaultCourierCity, value.DefaultOrEmpty()); }
        }

        public string DefaultCourierStreet
        {
            get { return Params.ElementOrDefault(SdekTemplate.DefaultCourierStreet); }
            set { Params.TryAddValue(SdekTemplate.DefaultCourierStreet, value.DefaultOrEmpty()); }
        }

        public string DefaultCourierHouse
        {
            get { return Params.ElementOrDefault(SdekTemplate.DefaultCourierHouse); }
            set { Params.TryAddValue(SdekTemplate.DefaultCourierHouse, value.DefaultOrEmpty()); }
        }

        public string DefaultCourierFlat
        {
            get { return Params.ElementOrDefault(SdekTemplate.DefaultCourierFlat); }
            set { Params.TryAddValue(SdekTemplate.DefaultCourierFlat, value.DefaultOrEmpty()); }
        }

        public string DefaultCourierNameContact
        {
            get { return Params.ElementOrDefault(SdekTemplate.DefaultCourierNameContact); }
            set { Params.TryAddValue(SdekTemplate.DefaultCourierNameContact, value.DefaultOrEmpty()); }
        }

        public string DefaultCourierPhone
        {
            get { return Params.ElementOrDefault(SdekTemplate.DefaultCourierPhone); }
            set { Params.TryAddValue(SdekTemplate.DefaultCourierPhone, value.DefaultOrEmpty()); }
        }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
