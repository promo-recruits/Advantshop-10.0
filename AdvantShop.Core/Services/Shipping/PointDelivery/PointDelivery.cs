using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.PointDelivery
{
    [ShippingKey("PointDelivery")]
    public class PointDelivery : BaseShipping, IShippingLazyData, IShippingNoUseExtraDeliveryTime
    {        
        private readonly List<DeliveryPointShipping> _points;
        private readonly EnTypePoints _typePoints;
        private readonly float _rate;
        private readonly string _deliveryTime;
        private readonly string _yaMapsApiKey;

        public override bool CurrencyAllAvailable { get { return true; } }

        public PointDelivery(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : base(method, preOrder, items)
        {
            _typePoints = (EnTypePoints)method.Params.ElementOrDefault(PointDeliveryTemplate.TypePoints).TryParseInt();
            _rate = _method.Params.ElementOrDefault(PointDeliveryTemplate.ShippingPrice).TryParseFloat();
            _deliveryTime = _method.Params.ElementOrDefault(PointDeliveryTemplate.DeliveryTime);
            _yaMapsApiKey = _method.Params.ElementOrDefault(PointDeliveryTemplate.YaMapsApiKey);

            var oldPoints = method.Params.ElementOrDefault(PointDeliveryTemplate.Points);
            if (!string.IsNullOrEmpty(oldPoints))
                _points = oldPoints.Split(';').Select((x, index) => new DeliveryPointShipping
                {
                    Id = index,
                    Address = x
                }).ToList();
            else
            {
                var newPoints = method.Params.ElementOrDefault(PointDeliveryTemplate.NewPoints);
                _points = !string.IsNullOrEmpty(newPoints)
                    ? JsonConvert.DeserializeObject<List<DeliveryPointShipping>>(newPoints)
                    : new List<DeliveryPointShipping>();
            }

            _points.Where(x => x.Id == 0).ForEach(x => x.Id = string.Format("{0}-{1}", _method.ShippingMethodId, x.Address).GetHashCode());
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var _options = new List<BaseShippingOption>();

            if (_typePoints == EnTypePoints.List || _preOrder.IsFromAdminArea || _yaMapsApiKey.IsNullOrEmpty())
                _options.Add(new PointDeliveryOption(_method, _totalPrice)
                {
                    ShippingPoints = _points,
                    Rate = _rate,
                    DeliveryTime = _deliveryTime
                });
            else
            {
                var option = new PointDeliveryMapOption(_method, _totalPrice)
                {
                    MapPoints = _points,
                    Rate = _rate,
                    DeliveryTime = _deliveryTime,
                    MapParams = new MapParams(),
                    PointParams = new PointParams()
                };
                SetMapData(option);

                _options.Add(option);
            }

            return _options;
        }

        private void SetMapData(PointDeliveryMapOption option)
        {
            string lang = "en_US";
            switch (Localization.Culture.Language)
            {
                case Localization.Culture.SupportLanguage.Russian:
                    lang = "ru_RU";
                    break;
                case Localization.Culture.SupportLanguage.English:
                    lang = "en_US";
                    break;
                case Localization.Culture.SupportLanguage.Ukrainian:
                    lang = "uk_UA";
                    break;
            }
            option.MapParams.Lang = lang;
            option.MapParams.YandexMapsApikey = _yaMapsApiKey;
            option.MapParams.Destination = string.Join(", ", new[] { _preOrder.CountryDest, _preOrder.RegionDest, _preOrder.CityDest}.Where(x => x.IsNotEmpty()));

            option.PointParams.IsLazyPoints = option.MapPoints.Count > 30;
            option.PointParams.PointsByDestination = false;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = null;
            }
            else
            {
                option.PointParams.Points = GetYaPoints(option.MapPoints);
            }
        }

        public FeatureCollection GetYaPoints(List<DeliveryPointShipping> points)
        {
            return new FeatureCollection
            {
                Features = points.Select(p =>
                    new Feature {
                        Id = p.Id,
                        Geometry = new PointGeometry { PointX = p.PointX, PointY = p.PointY },
                        Options = new PointOptions { Preset = "islands#dotIcon" },
                        Properties = new PointProperties
                        {
                            BalloonContentHeader = p.Address,
                            HintContent = p.Address,
                            BalloonContentBody = 
                                string.Format("{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{2}')\">Выбрать</a>",
                                                p.Description,
                                                p.Description.IsNotEmpty() ? "<br>" : "",
                                                p.Id)
                        }
                    }).ToList()
            };
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            return GetYaPoints(_points);
        }
    }


    public enum EnTypePoints
    {
        /// <summary>
        /// Список
        /// </summary>
        [Localize("Список")]
        List = 0,

        /// <summary>
        /// Карта
        /// </summary>
        [Localize("Карта")]
        Map = 1,
    }

}