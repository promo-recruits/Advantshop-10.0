using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public interface IEmailLoger : IAdvantShopLoger
    {
        void LogEmail(EmailLogItem email);

        List<EmailLogItem> GetEmails(Guid customerId, string email);
    }
}
