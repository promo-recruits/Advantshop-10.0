using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Crm.Vk;
using Newtonsoft.Json;

namespace AdvantShop.Configuration
{
    public class SettingsVk
    {
        public static string ApplicationId
        {
            get => SettingProvider.Items["SettingsVk.ApplicationId"];
            set => SettingProvider.Items["SettingsVk.ApplicationId"] = value;
        }

        /// <summary>
        /// Токен с правами только на юзера (потому, что сначала нужно получить группы, а уже потом авторизоваться от имени группы)
        /// </summary>
        public static string TokenUser
        {
            get => SettingProvider.Items["SettingsVk.TokenUser"];
            set => SettingProvider.Items["SettingsVk.TokenUser"] = value;
        }

        /// <summary>
        /// Токен с правами на группу
        /// </summary>
        public static string TokenGroup
        {
            get => SettingProvider.Items["SettingsVk.TokenGroup"];
            set
            {
                SettingProvider.Items["SettingsVk.TokenGroup"] = value;
                JobActivationManager.SettingUpdated();
            }
        }

        /// <summary>
        /// Авторизовались под логином и паролем
        /// </summary>
        public static bool AuthByUser
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsVk.AuthByUser"]);
            set => SettingProvider.Items["SettingsVk.AuthByUser"] = value.ToString();
        }

        public static int TokenUserErrorCount
        {
            get => Convert.ToInt32(SettingProvider.Items["SettingsVk.TokenGroupUserCount"]);
            set => SettingProvider.Items["SettingsVk.TokenGroupUserCount"] = value.ToString();
        }

        public static int TokenGroupErrorCount
        {
            get => Convert.ToInt32(SettingProvider.Items["SettingsVk.TokenGroupErrorCount"]);
            set => SettingProvider.Items["SettingsVk.TokenGroupErrorCount"] = value.ToString();
        }

        public static long UserId
        {
            get => SettingProvider.Items["SettingsVk.UserId"].TryParseLong();
            set
            {
                SettingProvider.Items["SettingsVk.UserId"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }

        public static VkGroup Group
        {
            get
            {
                var str = SettingProvider.Items["SettingsVk.Group"];
                return !string.IsNullOrWhiteSpace(str) ? JsonConvert.DeserializeObject<VkGroup>(str) : null;
            }
            set => SettingProvider.Items["SettingsVk.Group"] = value != null ? JsonConvert.SerializeObject(value) : null;
        }

        public static long? LastMessageId
        {
            get => SettingProvider.Items["SettingsVk.LastMessageId"].TryParseLong(true);
            set => SettingProvider.Items["SettingsVk.LastMessageId"] = value != null ? value.ToString() : null;
        }

        public static long? LastSendedMessageId
        {
            get => SettingProvider.Items["SettingsVk.LastSendedMessageId"].TryParseLong(true);
            set => SettingProvider.Items["SettingsVk.LastSendedMessageId"] = value != null ? value.ToString() : null;
        }
        
        /// <summary>
        /// Уже загружали посты?
        /// </summary>
        public static bool IsMessagesLoaded
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsVk.IsMessagesLoaded"]);
            set => SettingProvider.Items["SettingsVk.IsMessagesLoaded"] = value.ToString();
        }

        /// <summary>
        /// Создавать лид из личных сообщений
        /// </summary>
        public static bool CreateLeadFromMessages
        {
            get
            {
                var value = SettingProvider.Items["SettingsVk.CreateLeadFromMessages"];
                return value == null || Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["SettingsVk.CreateLeadFromMessages"] = value.ToString();
        }

        /// <summary>
        /// Создавать лид из комментариев
        /// </summary>
        public static bool CreateLeadFromComments
        {
            get
            {
                var value = SettingProvider.Items["SettingsVk.CreateLeadFromComments"];
                return value == null || Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["SettingsVk.CreateLeadFromComments"] = value.ToString();
        }

        /// <summary>
        /// Импортировать заказы из VK
        /// </summary>
        public static bool SyncOrdersFromVk
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsVk.SyncOrdersFromVk"]);
            set
            {
                SettingProvider.Items["SettingsVk.SyncOrdersFromVk"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }

        public static string TokenUserCopy
        {
            get => SettingProvider.Items["SettingsVk.TokenUserCopy"];
            set => SettingProvider.Items["SettingsVk.TokenUserCopy"] = value;
        }

        public static string GroupMessageErrorStatus
        {
            get => SettingProvider.Items["SettingsVk.GroupMessageErrorStatus"];
            set => SettingProvider.Items["SettingsVk.GroupMessageErrorStatus"] = value;
        }
    }
}