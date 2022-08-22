using System;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.IPTelephony.Yandex
{
    public enum EYandexEventType
    {
        /// <summary>
        /// Появление нового входящего звонка со внешнего номера {From} на бизнес номер {To}
        /// </summary>
        IncomingCall,
        /// <summary>
        /// Начало дозвона до пользователя с указанным добавочным номером {Extension}
        /// </summary>
        IncomingCallRinging,
        /// <summary>
        /// Неуспешная попытка дозвона до пользователя с добавочным номером {Extension}
        /// </summary>
        IncomingCallStopRinging,
        /// <summary>
        /// Успешное соединение с пользователем с добавочным номером {Extension}
        /// </summary>
        IncomingCallConnected,
        /// <summary>
        /// Завершение входящего звонка
        /// </summary>
        IncomingCallCompleted,

        /// <summary>
        /// Начало исходящего звонка с бизнес номера {From} на внешний номер {To} пользователем с добавочным номером {Extension}
        /// </summary>
        OutgoingCall,
        /// <summary>
        /// Начало разговора при исходящем звонке
        /// </summary>
        OutgoingCallConnected,
        /// <summary>
        /// Завершение исходящего звонка
        /// </summary>
        OutgoingCallCompleted,

        /// <summary>
        /// Появление заявки на обратный звонок с бизнес номера {From} на внешний номер {To}
        /// </summary>
        CallbackCall,
        /// <summary>
        /// Начало дозвона до пользователя с указанным добавочным номером {Extension} при обратном звонке
        /// </summary>
        CallbackCallRinging,
        /// <summary>
        /// Неуспешная попытка дозвона до пользователя с добавочным номером {Extension} при обратном звонке
        /// </summary>
        CallbackCallStopRinging,
        /// <summary>
        /// Пользователь с добавочным номером {Extension} соединился с номером {To} при обратном звонке
        /// </summary>
        CallbackCallConnected,
        /// <summary>
        /// Завершение обратного звонка
        /// </summary>
        CallbackCallCompleted,
    }

    public class YandexNotification
    {
        public YandexNotificationBody Body { get; set; }

        public Guid Guid { get; set; }

        public string ApiKey { get; set; }

        public DateTime Timestamp { get; set; }

        public EYandexEventType EventType { get; set; }
    }

    public class YandexNotificationBody
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Extension { get; set; }
    }
}
