using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Loging.Emails
{
    partial class ActivityEmailNullLoger : IEmailLoger
    {
        public virtual void LogEmail(EmailLogItem email)
        {
        }

        public virtual List<EmailLogItem> GetEmails(Guid customerId, string email)
        {
            return null;
        }
    }
}
