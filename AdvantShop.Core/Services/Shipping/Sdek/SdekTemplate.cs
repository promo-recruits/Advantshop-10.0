//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.Sdek
{
    public class SdekTemplate : DefaultCargoParams
    {
        public const string AuthLogin = "authLogin";
        public const string AuthPassword = "authPassword";
        public const string CalculateTariffs = "CalculateTariffs";
        public const string TariffOldParam = "Tariffs";
        public const string CityFrom = "cityFrom";
        public const string CityFromId = "cityFromId";
		public const string DefaultCourierNameContact = "DefaultCourierNameContact";
		public const string DefaultCourierPhone = "DefaultCourierPhone";
        public const string DefaultCourierCity = "DefaultCourieCity";
        public const string DefaultCourierStreet = "DefaultCourierStreet";
		public const string DefaultCourierHouse = "DefaultCourierHouse";
		public const string DefaultCourierFlat = "DefaultCourierFlat";
        public const string DescriptionForSendOrder = "DescriptionForSendOrder";
        public const string DeliveryNote = "DeliveryNote";
        public const string StatusesSync = "StatusesSync";
        public const string StatusCreated = "StatusCreated";
        // public const string StatusDeleted = "StatusDeleted";
        public const string StatusAcceptedAtWarehouseOfSender = "StatusAcceptedAtWarehouseOfSender";
        public const string StatusIssuedForShipmentFromSenderWarehouse = "StatusIssuedForShipmentFromSenderWarehouse";
        public const string StatusReturnedToWarehouseOfSender = "StatusReturnedToWarehouseOfSender";
        public const string StatusDeliveredToCarrierFromSenderWarehouse = "StatusDeliveredToCarrierFromSenderWarehouse";
        public const string StatusSentToTransitWarehouse = "StatusSentToTransitWarehouse";
        public const string StatusMetAtTransitWarehouse = "StatusMetAtTransitWarehouse";
        public const string StatusAcceptedAtTransitWarehouse = "StatusAcceptedAtTransitWarehouse";
        public const string StatusReturnedToTransitWarehouse = "StatusReturnedToTransitWarehouse";
        public const string StatusIssuedForShipmentInTransitWarehouse = "StatusIssuedForShipmentInTransitWarehouse";
        public const string StatusDeliveredToCarrierInTransitWarehouse = "StatusDeliveredToCarrierInTransitWarehouse";
        public const string StatusSentToWarehouseOfRecipient = "StatusSentToWarehouseOfRecipient";
        public const string StatusSentToSenderCity = "StatusSentToSenderCity";
        public const string StatusMetAtSenderCity = "StatusMetAtSenderCity";
        public const string StatusMetAtConsigneeWarehouse = "StatusMetAtConsigneeWarehouse";
        public const string StatusAcceptedAtConsigneeWarehouse_AwaitingDelivery = "StatusAcceptedAtConsigneeWarehouse_AwaitingDelivery";
        public const string StatusAcceptedAtConsigneeWarehouse_AwaitingFenceByClient = "StatusAcceptedAtConsigneeWarehouse_AwaitingFenceByClient";
        public const string StatusIssuedForDelivery = "StatusIssuedForDelivery";
        public const string StatusReturnedToConsigneeWarehouse = "StatusReturnedToConsigneeWarehouse";
        public const string StatusAwarded = "StatusAwarded";
        public const string StatusNotAwarded = "StatusNotAwarded";
        public const string AllowInspection = "AllowInspection";
        public const string ShowPointsAsList = "ShowPointsAsList";
        public const string ShowSdekWidjet = "ShowSdekWidjet";
        public const string ShowAddressComment = "ShowAddressComment";
        public const string YaMapsApiKey = "YaMapsApiKey";
        public const string WithInsure = "WithInsure";

        public const string UseSeller = "UseSeller";
        public const string SellerAddress = "SellerAddress";
        public const string SellerName = "SellerName";
        public const string SellerINN = "SellerINN";
        public const string SellerPhone = "SellerPhone";
        public const string SellerOwnershipForm = "SellerOwnershipForm";
    }

    public class SdekParamsSendOrder
    {
        public SdekParamsSendOrder()
        { }
        public SdekParamsSendOrder(ShippingMethod method)
        {
            Description = method.Params.ElementOrDefault(SdekTemplate.DescriptionForSendOrder);
            if (method.Params.ElementOrDefault(SdekTemplate.UseSeller).TryParseBool())
            {
                SellerAddress = method.Params.ElementOrDefault(SdekTemplate.SellerAddress);
                SellerName = method.Params.ElementOrDefault(SdekTemplate.SellerName);
                SellerINN = method.Params.ElementOrDefault(SdekTemplate.SellerINN);
                SellerPhone = method.Params.ElementOrDefault(SdekTemplate.SellerPhone);
                SellerOwnershipForm = method.Params.ElementOrDefault(SdekTemplate.SellerOwnershipForm);
            }
        }
        public string Description { get; set; }
        public string SellerAddress { get; set; }
        public string SellerName { get; set; }
        public string SellerINN { get; set; }
        public string SellerPhone { get; set; }
        public string SellerOwnershipForm { get; set; }
    }
}