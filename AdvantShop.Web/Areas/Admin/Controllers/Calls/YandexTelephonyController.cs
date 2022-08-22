using System;
using System.IO;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Core.Services.IPTelephony.Yandex;
using AdvantShop.Saas;
using Call = AdvantShop.Core.Services.IPTelephony.Call;

namespace AdvantShop.Web.Admin.Controllers.Calls
{
    public class YandexTelephonyController : BaseCallController
    {
        private static readonly Object SyncObject = new Object();

        public ActionResult PushNotification(YandexNotification model)
        {
            lock (SyncObject)
            {
                if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveTelephony)
                    return Content("no telephony");

                LogNotification();

                var request = ControllerContext.RequestContext.HttpContext.Request;
                request.InputStream.Seek(0, SeekOrigin.Begin);
                //AdvantShop.Statistic.CommonStatistic.WriteLog("stream: " + (new StreamReader(request.InputStream)).ReadToEnd());
                if (request.Headers["Echo"].IsNotEmpty()) // валидация ссылки
                {
                    ControllerContext.RequestContext.HttpContext.Response.AddHeader("Echo", request.Headers["Echo"]);
                    return Content(string.Format("Echo: {0}", request.Headers["Echo"]));
                }

                if (model == null || model.Body == null || SettingsTelephony.CurrentIPTelephonyOperatorType != EOperatorType.Yandex)
                    return Content("error");

                var call = CallService.GetCall(model.Body.Id, EOperatorType.Yandex) ?? new Call
                {
                    CallId = model.Body.Id,
                    SrcNum = model.Body.From,
                    DstNum = model.Body.To,
                    Extension = model.Body.Extension,
                    CallDate = model.Timestamp,
                    OperatorType = EOperatorType.Yandex
                };

                switch (model.EventType)
                {
                    case EYandexEventType.IncomingCall:
                        call.Type = ECallType.In;
                        call.Phone = model.Body.From;
                        break;
                    case EYandexEventType.IncomingCallRinging:
                        call.Type = ECallType.In;
                        if (model.Body.Extension.IsNotEmpty())
                            call.Extension = model.Body.Extension;
                        SendIncomingCallNotify(call);
                        break;
                    case EYandexEventType.IncomingCallConnected:
                        call.Type = ECallType.In;
                        if (model.Body.Extension.IsNotEmpty())
                            call.Extension = model.Body.Extension;
                        call.CallAnswerDate = model.Timestamp;
                        break;
                    case EYandexEventType.IncomingCallCompleted:
                        call.IsComplete = true;
                        if (!call.CallAnswerDate.HasValue)
                        {
                            call.Type = ECallType.Missed;
                            ProcessMissedCall(call);
                        }
                        else
                        {
                            call.Type = ECallType.In;
                            call.Duration = (int)(model.Timestamp - call.CallAnswerDate.Value).TotalSeconds;
                        }
                        SendCallEndedNotify(call);
                        break;

                    case EYandexEventType.OutgoingCall:
                    case EYandexEventType.CallbackCall:
                        call.Type = ECallType.Out;
                        call.Phone = model.Body.To;
                        if (model.Body.Extension.IsNotEmpty())
                            call.Extension = model.Body.Extension;
                        break;
                    case EYandexEventType.OutgoingCallConnected:
                    case EYandexEventType.CallbackCallConnected:
                        call.Type = ECallType.Out;
                        call.CallAnswerDate = model.Timestamp;
                        if (model.Body.Extension.IsNotEmpty())
                            call.Extension = model.Body.Extension;
                        break;
                    case EYandexEventType.OutgoingCallCompleted:
                    case EYandexEventType.CallbackCallCompleted:
                        call.Type = ECallType.Out;
                        if (call.CallAnswerDate.HasValue)
                            call.Duration = (int)(model.Timestamp - call.CallAnswerDate.Value).TotalSeconds;
                        call.IsComplete = true;
                        break;
                }

                if (call.Id == 0)
                    call.Id = CallService.AddCall(call);
                else
                    CallService.UpdateCall(call);

                CallService.ProcessCall(call);

                return Content("ok");
            }
        }
    }
}
