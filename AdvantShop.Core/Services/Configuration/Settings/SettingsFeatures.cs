using System;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.SalesChannels;

namespace AdvantShop.Configuration
{
    public enum EFeature
    {
        [Localize("Core.Services.Features.EFeature.OfferWeightAndDimensions")]
        [DescriptionKey("Core.Services.Features.EFeature.OfferWeightAndDimensions.Description")]
        OfferWeightAndDimensions,
        
        [Localize("Core.Services.Features.EFeature.FunnelBlocks")]
        [DescriptionKey("Core.Services.Features.EFeature.FunnelBlocks.Description")]
        FunnelBlocks,

        //[Localize("Новый дашборд")]
        //[DescriptionKey("Показывать \"Мои сайты\" и новый дашбоард")]
        //NewDashboard,

        [Localize("Core.Services.Features.EFeature.OfferBarCode")]
        [DescriptionKey("Core.Services.Features.EFeature.OfferBarCode.Description")]
        OfferBarCode,

        //[Localize("Моб. версия админ. панели")]
        //[DescriptionKey("Включает моб. версию админ. панели")]
        //MobileAdmin,
    }

    public class FeaturesService
    {
        public static bool IsEnabled(EFeature feature)
        {
            return SettingsFeatures.EnableExperimentalFeatures && SettingsFeatures.IsFeatureEnabled(feature);
        }
    }

    public class SettingsFeatures
    {
        public static bool EnableExperimentalFeatures
        {
            get => Convert.ToBoolean(SettingProvider.Items["Features.EnableExperimentalFeatures"]);
            set
            {
                SettingProvider.Items["Features.EnableExperimentalFeatures"] = value.ToString();
                CacheManager.RemoveByPattern(SalesChannelService.CacheKey);
            }
        }

        public static bool IsFeatureEnabled(EFeature feature)
        {
            return Convert.ToBoolean(SettingProvider.Items["Features.Enable" + feature.ToString()]);
        }

        public static void SetFeatureEnabled(EFeature feature, bool enabled)
        {
            SettingProvider.Items["Features.Enable" + feature.ToString()] = enabled.ToString();
            CacheManager.RemoveByPattern(SalesChannelService.CacheKey);
        }
    }
}