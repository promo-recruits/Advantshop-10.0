//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using System;
using System.Collections.Generic;

namespace AdvantShop.Shipping.NovaPoshta
{

    [ShippingKey("NovaPoshta")]
    public class NovaPoshta : BaseShippingWithWeight/*AndCache*/, IShippingSupportingPaymentCashOnDelivery
    {
        private readonly string _apiKey;
        private readonly enNovaPoshtaDeliveryType _deliveryType;
        private readonly string _cityFrom;

        public override string[] CurrencyIso3Available { get { return new[] { "UAH" }; } }

        public NovaPoshta(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items)
            : base(method, preOrder, items)
        {
            _apiKey = _method.Params.ElementOrDefault(NovaPoshtaTemplate.APIKey);
            _cityFrom = _method.Params.ElementOrDefault(NovaPoshtaTemplate.CityFrom);
            _deliveryType = (enNovaPoshtaDeliveryType)_method.Params.ElementOrDefault(NovaPoshtaTemplate.DeliveryType).TryParseInt((int)enNovaPoshtaDeliveryType.WarehouseDoors);
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var result = new List<BaseShippingOption>();

            if (_preOrder.CityDest.IsNotEmpty() && _cityFrom.IsNotEmpty())
            {
                var cityTo = _preOrder.CityDest;
                var areaTo = _preOrder.RegionDest;
                var totalWeight = GetTotalWeight();

                // todo: возможно город ищется не потому справочнику (для курьерской доставки)
                // т.к. заявлено: Стоит учитывать, справочник выгружается только с населенными пунктами где есть отделения "Нова Пошта" и можно оформить доставку на отделение, а также на доставку по адресу.
                var senderCity = NovaPoshtaService.GetCity(_apiKey, _cityFrom, "");
                var recipientCity = NovaPoshtaService.GetCity(_apiKey, cityTo, areaTo);

                if (senderCity != null && recipientCity != null)
                {
                    var delivery = NovaPoshtaService.GetDocumentDeliveryDate(_apiKey, senderCity.Ref, recipientCity.Ref, _deliveryType, DateTime.Now);
                    var price = NovaPoshtaService.GetDocumentPrice(_apiKey, senderCity.Ref, recipientCity.Ref, _deliveryType, totalWeight, _totalPrice);
                    var days = (delivery - DateTime.Today).Days + _method.ExtraDeliveryTime;

                    var toWarehouse = _deliveryType == enNovaPoshtaDeliveryType.DoorsWarehouse ||
                        _deliveryType == enNovaPoshtaDeliveryType.WarehouseWarehouse;

                    var warehouses = toWarehouse
                        ? GetWarehousePoints(_apiKey, recipientCity.Ref.ToString(), totalWeight)
                        : null;

                    var option = new NovaPoshtaOptions(_method, _totalPrice)
                    {
                        //Name = _method.Name + (toWarehouse ? " (пункты выдачи)" : " (курьером)"),
                        DeliveryTime = days + " " + Strings.Numerals(days, "нет", "день", "дня", "дней"),
                        Rate = price,
                        ShippingPoints = toWarehouse ? warehouses : null,
                        HideAddressBlock = toWarehouse,
                    };

                    result.Add(option);
                }
            }

            return result;
        }

        private List<NovaPoshtaPoint> GetWarehousePoints(string apiKey, string cityRef, float totalWeight)
        {
            var listShippingPoints = new List<NovaPoshtaPoint>();
            foreach (var warehouse in NovaPoshtaService.GetWarehouses(_apiKey, cityRef)
                .Where(x => x.TotalMaxWeightAllowed == 0f || totalWeight <= x.TotalMaxWeightAllowed)
                .OrderBy(x => x.ShortAddressRu))
            {
                listShippingPoints.Add(new NovaPoshtaPoint
                {
                    Id = warehouse.SiteKey,
                    Code = warehouse.Ref,
                    Address = warehouse.ShortAddressRu,
                    //AddressComment = sdekPoint.AddressComment,
                    Description = string.Join("<br/>", new[] { warehouse.Phone }.Where(x => x.IsNotEmpty())),
                    PointX = warehouse.Latitude,
                    PointY = warehouse.Longitude,
                });
            }
            return listShippingPoints;
        }

        //protected override int GetHashForCache()
        //{
        //    var dimensions = GetDimensions(_preOrder, _items);

        //    var length = dimensions[0];
        //    var width = dimensions[1];
        //    var height = dimensions[2];

        //    var postData = string.Format("auth{0},senderCity{1}recipientCity{2}mass{3}height{4}width{5}depth{6}",
        //                                    _apiKey, _cityFrom, _preOrder.CityDest, GetTotalWeight().ToString("F3"),
        //                                    height.ToString("F2"), width.ToString("F2"), length.ToString("F2"));
        //    var hash = postData.GetHashCode();
        //    return _method.ShippingMethodId ^ hash;
        //}
    }
}