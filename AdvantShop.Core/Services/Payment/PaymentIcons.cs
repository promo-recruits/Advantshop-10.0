﻿//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using AdvantShop.FilePath;

namespace AdvantShop.Payment
{
    public static class PaymentIcons
    {
        private static readonly Dictionary<string, string> ExternalMap = new Dictionary<string, string>
        {
            {"alfabankkupilegko", "alfabank"},
            {"alfabankua", "alfabank"},
            {"billby", "bill"},
            {"billkz", "bill"},
            {"billua", "bill"},
            {"intellectmoneymainprotocol", "intellectmoney"},
            {"interkassa2", "interkassa"},
            {"paypalexpresscheckout", "paypal"},
            {"qiwikassa", "qiwi"},
            {"rbkmoney2", "rbkmoney"},
            {"walletonecheckout", "walletone"},
            {"yandexkassa", "yookassa"},
        };

        public static string GetPaymentIcon(string paymentKey, string iconName, string name)
        {
            if (!string.IsNullOrWhiteSpace(iconName) && File.Exists(FoldersHelper.GetPathAbsolut(FolderType.PaymentLogo, iconName)))
            {
                return FoldersHelper.GetPath(FolderType.PaymentLogo, iconName, false);
            }

            var folderPath = FoldersHelper.GetPath(FolderType.PaymentLogo, string.Empty, false);

            var paymentKeyLower = paymentKey.ToLower();
            
            if (paymentKey == nameof(UniversalPayGate))
            {
                paymentKeyLower = name.ToLower();
            }
            
            if (ExternalMap.ContainsKey(paymentKeyLower))
            {
                paymentKeyLower = ExternalMap[paymentKeyLower];
            }
            
            var fileName = $"{paymentKeyLower}.svg";

            if (File.Exists(FoldersHelper.GetPathAbsolut(FolderType.PaymentLogo, fileName)))
            {
                return folderPath + fileName;
            }

            return folderPath + "cash.svg";
        }
    }
}