using System;
using System.Collections.Generic;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Web.Admin.Models.Marketing.Emailings;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Emailings
{
    public class GetTriggerAnalyticsHandler : BaseGetEmailingAnalyticsHandler<Dictionary<Guid, EmailingAnalyticsModel>>
    {
        private int _triggerId;
        private DateTime _dateFrom;
        private DateTime _dateTo;

        public GetTriggerAnalyticsHandler(int triggerId, DateTime dateFrom, DateTime dateTo)
        {
            _triggerId = triggerId;
            _dateFrom = dateFrom.Date;
            _dateTo = dateTo.Date;
        }

        public override Dictionary<Guid, EmailingAnalyticsModel> Execute()
        {
            var cacheName = CacheNames.AdvantShopMail + string.Format("Trigger_{0}_{1}_{2}", _triggerId, _dateFrom.Ticks, _dateTo.Ticks);
            return CacheManager.Get(cacheName, () =>
            {
                var data = AdvantShopMailService.GetTriggerAnalytics(_triggerId, _dateFrom, _dateTo);

                var result = new Dictionary<Guid, EmailingAnalyticsModel>();
                foreach (var item in data)
                {
                    if (item.EmailingId != null)
                        result.Add(item.EmailingId.Value, GetModel(item));
                }
                return result;
            });
        }
    }
}
