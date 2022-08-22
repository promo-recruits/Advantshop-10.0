using System;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsSms
    {
        public static string AdminPhone
        {
            get => SettingProvider.Items["AdminPhone"];
            set => SettingProvider.Items["AdminPhone"] = value;
        }
        
        public static bool SendSmsToCustomerOnNewOrder
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToCustomerOnNewOrder"]);
            set => SettingProvider.Items["SendSmsToCustomerOnNewOrder"] = value.ToString();
        }

        public static bool SendSmsToAdminOnNewOrder
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToAdminOnNewOrder"]);
            set => SettingProvider.Items["SendSmsToAdminOnNewOrder"] = value.ToString();
        }

        public static bool SendSmsToAdminOnNewLead
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToAdminOnNewLead"]);
            set => SettingProvider.Items["SendSmsToAdminOnNewLead"] = value.ToString();
        }

        public static string SmsTextOnNewOrder
        {
            get => SettingProvider.Items["SmsTextOnNewOrder"];
            set => SettingProvider.Items["SmsTextOnNewOrder"] = value;
        }

        public static string SmsTextOnNewLead
        {
            get => SettingProvider.Items["SmsTextOnNewLead"];
            set => SettingProvider.Items["SmsTextOnNewLead"] = value;
        }


        public static bool SendSmsToCustomerOnOrderStatusChanging
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToCustomerOnOrderStatusChanging"]);
            set => SettingProvider.Items["SendSmsToCustomerOnOrderStatusChanging"] = value.ToString();
        }

        public static bool SendSmsToAdminOnOrderStatusChanging
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToAdminOnOrderStatusChanging"]);
            set => SettingProvider.Items["SendSmsToAdminOnOrderStatusChanging"] = value.ToString();
        }

        public static string ActiveSmsModule
        {
            get => SettingProvider.Items["ActiveSmsModule"];
            set => SettingProvider.Items["ActiveSmsModule"] = value;
        }
    }
}
