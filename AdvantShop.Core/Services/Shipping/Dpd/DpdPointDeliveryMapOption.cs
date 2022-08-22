﻿//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Dpd
{
    public class DpdPointDeliveryMapOption : BaseShippingOption, PointDelivery.IPointDeliveryMapOption
    {
        public DpdPointDeliveryMapOption()
        {
        }

        public DpdPointDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public DpdCalculateOption CalculateOption { get; set; }
        [JsonIgnore]
        public List<DpdPoint> CurrentPoints { get; set; }
        public BaseShippingPoint SelectedPoint { get; set; }
        public PointDelivery.MapParams MapParams { get; set; }
        public PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }
        public string PickpointId { get; set; }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as DpdPointDeliveryMapOption;
            if (opt != null && opt.Id == this.Id)
            {
                if (this.CurrentPoints != null && this.CurrentPoints.Any(x => x.Code == opt.PickpointId))
                {
                    this.PickpointId = opt.PickpointId;
                    this.YaSelectedPoint = opt.YaSelectedPoint;
                    this.SelectedPoint = this.CurrentPoints.FirstOrDefault(x => x.Code == opt.PickpointId);
                }
                else
                {
                    this.PickpointId = null;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId)
             ? new OrderPickPoint
             {
                 PickPointId = PickpointId,
                 PickPointAddress = SelectedPoint != null ? SelectedPoint.Address : null,
                 AdditionalData = JsonConvert.SerializeObject(CalculateOption)
             }
             : null;
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption != null && payOption.GetType() == typeof(CashOnDeliverytOption))
                Rate = PriceCash;
            else
                Rate = BasePrice;
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
