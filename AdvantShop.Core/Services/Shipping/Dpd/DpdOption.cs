﻿using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Dpd
{
    public class DpdOption : BaseShippingOption
    {
        public BaseShippingPoint SelectedPoint { get; set; }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public List<DpdPoint> ShippingPoints { get; set; }
        public DpdCalculateOption CalculateOption { get; set; }

        public DpdOption()
        {
        }

        public DpdOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
        }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/DpdOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as DpdOption;

            if (opt != null && opt.Id == this.Id && opt.ShippingPoints != null)
            {
                this.SelectedPoint = opt.SelectedPoint != null && this.ShippingPoints != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints.FirstOrDefault();
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = SelectedPoint != null ? SelectedPoint.Code : string.Empty,
                PickPointAddress = SelectedPoint != null ? SelectedPoint.Address : string.Empty,
                AdditionalData = JsonConvert.SerializeObject(CalculateOption)
            };
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
