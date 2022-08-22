//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.ShippingYandexNewDelivery
{
    public class YandexNewPointDeliveryMapOption : BaseShippingOption, PointDelivery.IPointDeliveryMapOption
    {
        public YandexNewPointDeliveryMapOption()
        {
        }

        public YandexNewPointDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        [JsonIgnore]
        public List<YandexNewPoint> CurrentPoints { get; set; }
        public BaseShippingPoint SelectedPoint { get; set; }
        public PointDelivery.MapParams MapParams { get; set; }
        public PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }
        public string PickpointId { get; set; }
        public string PickpointCompany { get; set; }
        public YandexNewDeliveryAdditionalData PickpointAdditionalData { get; set; }

        private string PostfixName
        {
            get { return !string.IsNullOrEmpty(PickpointCompany) && (string.IsNullOrEmpty(base.Name) || !base.Name.EndsWith(string.Format(" ({0})", PickpointCompany))) ? string.Format(" ({0})", PickpointCompany) : string.Empty; }
        }

        public new string Name
        {
            get { return base.Name + PostfixName; }
            set { base.Name = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(PostfixName) && value.EndsWith(PostfixName) ? value.Replace(PostfixName, string.Empty) : value; }
        }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as YandexNewPointDeliveryMapOption;
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
                 PickPointAddress = SelectedPoint != null 
                    ? string.Format("{0}{1}{2}",
                            PickpointCompany,
                            PickpointCompany.IsNotEmpty() && SelectedPoint.Address.IsNotEmpty() ? " " : string.Empty,
                            SelectedPoint.Address)
                    : null,
                 AdditionalData = JsonConvert.SerializeObject(PickpointAdditionalData)
             }
             : null;
        }
    }
}
