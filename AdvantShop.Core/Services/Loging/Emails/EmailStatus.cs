using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public enum EmailStatus
    {
        None,

        [Localize("Core.Loging.EmailStatus.PrepareSend")]
        [DescriptionKey("Core.Loging.EmailStatus.PrepareSend.Description")]
        PrepareSend,

        [Localize("Core.Loging.EmailStatus.Sending")]
        [DescriptionKey("Core.Loging.EmailStatus.Sending.Description")]
        Sending,

        [Localize("Core.Loging.EmailStatus.Sent")]
        [DescriptionKey("Core.Loging.EmailStatus.Sent.Description")]
        Sent,               

        [Localize("Core.Loging.EmailStatus.Delivered")]
        [DescriptionKey("Core.Loging.EmailStatus.Delivered.Description")]
        Delivered,

        [Localize("Core.Loging.EmailStatus.Opened")]
        [DescriptionKey("Core.Loging.EmailStatus.Opened.Description")]
        Opened,

        [Localize("Core.Loging.EmailStatus.Clicked")]
        [DescriptionKey("Core.Loging.EmailStatus.Clicked.Description")]
        Clicked,

        [Localize("Core.Loging.EmailStatus.Unsubscribed")]
        [DescriptionKey("Core.Loging.EmailStatus.Unsubscribed.Description")]
        Unsubscribed,

        [Localize("Core.Loging.EmailStatus.SoftBounced")]
        [DescriptionKey("Core.Loging.EmailStatus.SoftBounced.Description")]
        SoftBounced,

        [Localize("Core.Loging.EmailStatus.HardBounced")]
        [DescriptionKey("Core.Loging.EmailStatus.HardBounced.Description")]
        HardBounced,

        [Localize("Core.Loging.EmailStatus.Spam")]
        [DescriptionKey("Core.Loging.EmailStatus.Spam.Description")]
        Spam,

        [Localize("Core.Loging.EmailStatus.Error")]
        [DescriptionKey("Core.Loging.EmailStatus.Error.Description")]
        Error,
    }


    public enum AdvantShopEmailErrorStatus
    {
        None,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.Unsubscribed")]
        Unsubscribed,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.Invalid")]
        Invalid,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.Duplicate")]
        Duplicate,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.TemporaryUnavailable")]
        TemporaryUnavailable,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.PermanentUnavailable")]
        PermanentUnavailable,

        [Localize("Core.Loging.AdvantShopEmailErrorStatus.InternalServerError")]
        InternalServerError,
    }
}
