using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Track
{
    public class TrackService
    {
        private const string UrlTrackEvents = "http://modules.advantshop.net/Event/TrackEvent?licKey={0}&eventName={1}";

        public static void TrackEvent(ETrackEvent @event, string eventKeyPostfix = null)
        {
            var eventAttr = @event.GetAttribute<TrackEventAttribute>();

            string eventKey = null;
            bool sendOnce = false;
            string prefix = null;
            string delimiter = ".";
            if (eventAttr != null)
            {
                if (eventAttr.ShopMode.HasValue && !ModeConfigService.IsModeEnabled(eventAttr.ShopMode.Value))
                    return;
                eventKey = eventAttr.EventKey;
                sendOnce = eventAttr.SendOnce;
                if (eventAttr.Delimiter.IsNotEmpty())
                    delimiter = eventAttr.Delimiter;
                prefix = Trial.TrialService.IsTrialEnabled ? eventAttr.TrialPrefix : null;
            }
            if (eventKey.IsNullOrEmpty())
                eventKey = @event.ToString();
            eventKey = string.Join(delimiter, new List<string> { prefix, eventKey, eventKeyPostfix }.Where(x => x.IsNotEmpty()));

            var currentCustomer = Customers.CustomerContext.CurrentCustomer; 
            if ((currentCustomer != null && currentCustomer.IsVirtual) || 
                @event == ETrackEvent.None ||
                (sendOnce && TrackEventIsCommitted(@event)))
                return;

            var url = string.Format(UrlTrackEvents, SettingsLic.LicKey, eventKey);
            if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.UserHostAddress.IsNotEmpty())
                url += "&ip=" + HttpUtility.UrlEncode(HttpContext.Current.Request.UserHostAddress);

            new System.Threading.Tasks.Task(() =>
            {
                try
                {
                    var request = WebRequest.Create(url);
                    request.Method = "GET";
                    request.Timeout = 1000;
                    var response = request.GetResponse();
                    //new WebClient().DownloadString(url); // default timeout - 100 seconds
                }
                catch (Exception ex)
                {
                    Debug.Log.Warn(ex);
                }
            }).Start();

            if (sendOnce)
                SetTrackEventCommitted(eventKey);
        }

        public static bool TrackEventIsCommitted(ETrackEvent @event)
        {
            // старый код, переход от куки к базе
            //var cookieValue = HttpContext.Current != null && HttpContext.Current.Request != null ? CommonHelper.GetCookieString("committedEvents") : string.Empty;
            //if (!SettingsTracking.TrackedEvents.Any() && cookieValue.IsNotEmpty())
            //{
            //    var cookieEvents = cookieValue.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            //    SettingsTracking.TrackedEvents = cookieEvents;
            //    CommonHelper.DeleteCookie("committedEvents");
            //}
            return TrackEventIsCommitted(@event.ToString());
        }

        public static bool TrackEventIsCommitted(string eventKey)
        {
            return SettingsTracking.TrackedEvents.Any(x => x == eventKey);
        }

        private static void SetTrackEventCommitted(string eventKey)
        {
            var trackedEvents = SettingsTracking.TrackedEvents;
            trackedEvents.Add(eventKey);
            SettingsTracking.TrackedEvents = trackedEvents;
        }
    }
}