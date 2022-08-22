﻿using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.IPTelephony
{
    public enum EOperatorType
    {
        [Localize("Core.IPTelephony.EOperatorType.None")]
        None,

        [Localize("Core.IPTelephony.EOperatorType.Sipuni")]
        Sipuni,

        [Localize("Core.IPTelephony.EOperatorType.Mango")]
        Mango,

        [Localize("Core.IPTelephony.EOperatorType.Telphin")]
        Telphin,

        [Localize("Core.IPTelephony.EOperatorType.Zadarma")]
        Zadarma,

        [Localize("Core.IPTelephony.EOperatorType.Yandex")]
        Yandex
    }

    public enum ECallButtonType
    {
        [Localize("Core.IPTelephony.ECallButtonType.Small")]
        Small,
        [Localize("Core.IPTelephony.ECallButtonType.Big")]
        Big
    }

    public class IPTelephonyOperator
    {
        public virtual EOperatorType Type
        {
            get { return EOperatorType.None; }
        }

        protected IPTelephonyOperator()
        {

        }

        public virtual string GetRecordLink(int callId)
        {
            return string.Empty;
        }

        public virtual CallBack.CallBack CallBack
        {
            get { return null; }
        }

        public static IPTelephonyOperator Current
        {
            get
            {
                return GetByType(SettingsTelephony.CurrentIPTelephonyOperatorType);
            }
        }

        public static IPTelephonyOperator GetByType(EOperatorType type)
        {
            switch (type)
            {
                case EOperatorType.Sipuni:
                    return new Sipuni.Sipuni();
                case EOperatorType.Telphin:
                    return new Telphin.Telphin();
                case EOperatorType.Mango:
                    return new Mango.Mango();
                case EOperatorType.Zadarma:
                    return new Zadarma.Zadarma();
                case EOperatorType.Yandex:
                    return new Yandex.YandexTelephony();
                default:
                    return new IPTelephonyOperator();
            }
        }
    }
}
