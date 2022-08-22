//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Shipping.Grastin;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Core.UrlRewriter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Shipping.Grastin
{
    public class GrastinPointDeliveryMapOption : BaseShippingOption, PointDelivery.IPointDeliveryMapOption
    {
        public GrastinPointDeliveryMapOption()
        {
        }

        public GrastinPointDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public BaseShippingPoint SelectedPoint { get; set; }
        public PointDelivery.MapParams MapParams { get; set; }
        public PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }

        private string _pickpointId;
        public string PickpointId
        {
            get
            {
                return _pickpointId;
            }
            set
            {
                _pickpointId = value;
                _pickpointAdditionalDataObj = null;
            }
        }

        private GrastinEventWidgetData _pickpointAdditionalDataObj;
        [JsonIgnore]
        public GrastinEventWidgetData PickpointAdditionalDataObj
        {
            get
            {
                if (_pickpointAdditionalDataObj == null && !string.IsNullOrEmpty(PickpointId) && PickpointId.Contains("#"))
                {
                    _pickpointAdditionalDataObj = new GrastinEventWidgetData
                    {
                        CityFrom = CityFrom,
                        CityTo = CityTo,
                        PickPointId = PickpointId.Split('#')[1],
                        DeliveryType = EnDeliveryType.PickPoint
                    };
                    _pickpointAdditionalDataObj.Partner = (EnPartner)Enum.Parse(typeof(EnPartner), PickpointId.Split('#')[0], true);
                }


                return _pickpointAdditionalDataObj;
            }
        }

        [JsonIgnore]
        public List<Core.Services.Shipping.Grastin.Api.ISelfpickupGrastin> GrastinPoints { get; set; }

        [JsonIgnore]
        public List<Core.Services.Shipping.Grastin.Api.SelfpickupBoxberry> BoxberryPoints { get; set; }
        [JsonIgnore]
        public bool ShowDrivingDescriptionPoint { get; set; }
        public bool IsAvailableCashOnDelivery { get; set; }
        public override bool IsAvailablePaymentPickPoint { get { return PickpointAdditionalDataObj != null && PickpointAdditionalDataObj.DeliveryType == EnDeliveryType.PickPoint && IsAvailableCashOnDelivery; } }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public string CityTo { get; set; }
        public string CityFrom { get; set; }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as GrastinPointDeliveryMapOption;
            if (opt != null && opt.Id == this.Id && opt.CityTo == this.CityTo && (GrastinPoints != null || BoxberryPoints != null))
            {
                this.PickpointId = opt.PickpointId;
                this.YaSelectedPoint = opt.YaSelectedPoint;
                //this.PickpointAdditionalDataObj = opt.PickpointAdditionalDataObj;

                if (PickpointAdditionalDataObj != null)
                {
                    if ((PickpointAdditionalDataObj.Partner == EnPartner.Grastin || PickpointAdditionalDataObj.Partner == EnPartner.Partner) &&
                        GrastinPoints != null)
                    {
                        var grastinPoint = GrastinPoints.FirstOrDefault(x => x.Id == opt.PickpointAdditionalDataObj.PickPointId);
                        SelectedPoint = grastinPoint != null
                            ? new BaseShippingPoint
                            {
                                Id = grastinPoint.Id.GetHashCode(),
                                Code = grastinPoint.Id,
                                Address = grastinPoint.ToString(),
                                Description = string.Join("<br>", new[] { grastinPoint.TimeTable, grastinPoint.Phone, ShowDrivingDescriptionPoint ? grastinPoint.DrivingDescription : null }.Where(x => !string.IsNullOrEmpty(x)))
                            }
                            : null;
                    }
                    else if (PickpointAdditionalDataObj.Partner == EnPartner.Boxberry && BoxberryPoints != null)
                    {
                        var boxberryPoint = BoxberryPoints.FirstOrDefault(x => x.Id == opt.PickpointAdditionalDataObj.PickPointId);
                        SelectedPoint = boxberryPoint != null
                            ? new BaseShippingPoint
                            {
                                Id = boxberryPoint.Id.GetHashCode(),
                                Code = boxberryPoint.Id,
                                Address = boxberryPoint.Name,
                                Description = string.Join("<br>", new[] { boxberryPoint.Schedule, ShowDrivingDescriptionPoint ? boxberryPoint.DrivingDescription : null }.Where(x => !string.IsNullOrEmpty(x)))
                            }
                            : null;
                        //if (boxberryPoint != null)
                        //    IsAvailableCashOnDelivery = boxberryPoint.FullPrePayment;
                    }

                    HideAddressBlock = PickpointAdditionalDataObj == null ||
                                       (PickpointAdditionalDataObj.DeliveryType == EnDeliveryType.PickPoint &&
                                        PickpointAdditionalDataObj.Partner != EnPartner.RussianPost);
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId) && PickpointAdditionalDataObj != null
                ? new OrderPickPoint
                {
                    PickPointId = PickpointAdditionalDataObj.PickPointId,
                    PickPointAddress = SelectedPoint != null ?  SelectedPoint.Address : null,
                    AdditionalData = JsonConvert.SerializeObject(PickpointAdditionalDataObj)
                }
                : null;
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption != null && payOption.GetType() == typeof(PickPointOption))
                Rate = PriceCash;
            else
            {
                Rate = BasePrice;
            }
            return true;
        }

        public override string GetDescriptionForPayment()
        {
            var diff = PriceCash - BasePrice;
            if (diff <= 0)
                return string.Empty;

            return string.Format("Стоимость доставки увеличится на {0}", diff.RoundPrice().FormatPrice());
        }

    }
}
