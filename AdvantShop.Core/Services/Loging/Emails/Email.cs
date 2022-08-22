﻿using System;
using System.Linq;
using AdvantShop.Localization;
using MailKit;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public class EmailLogItem
    {
        public DateTime CreateOn { get; set; }
        public DateTime? Updated { get; set; }        
        public Guid CustomerId { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EmailStatus Status { get; set; }
        public string ShopId { get; set; }
    }

    public class EmailImap
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public string DateStr { get { return Culture.ConvertDate(Date); } }
        public string From { get; set; }
        public string FromEmail { get; set; }
        public string To { get; set; }
        public string ToEmail { get; set; }

        public string TextBody { get; set; }
        public string HtmlBody { get; set; }

        public string Folder { get; set; }

        public EmailImap()
        {
            
        }

        public EmailImap(IMessageSummary summary, string folder)
        {
            var envelope = summary.Envelope;

            Id = summary.UniqueId.ToString();
            Folder = folder;

            if (envelope != null)
            {
                Subject = envelope.Subject;

                Date = summary.InternalDate.HasValue
                    ? summary.InternalDate.Value.LocalDateTime
                    : envelope.Date.HasValue ? envelope.Date.Value.LocalDateTime : DateTime.Now;

                //Date = envelope.Date.HasValue
                //    ? TimeZone.CurrentTimeZone.ToLocalTime(envelope.Date.Value.UtcDateTime)
                //    : summary.InternalDate.HasValue ? TimeZone.CurrentTimeZone.ToLocalTime(summary.InternalDate.Value.UtcDateTime) : DateTime.Now;

                From = envelope.From != null ? envelope.From.ToString() : "";

                if (envelope.From != null)
                {
                    var mailBox = envelope.From.Mailboxes.FirstOrDefault();
                    if (mailBox != null)
                        FromEmail = mailBox.Address;
                }

                To = envelope.To != null ? envelope.To.ToString() : "";
                if (envelope.To != null)
                {
                    var mailBox = envelope.To.Mailboxes.FirstOrDefault();
                    if (mailBox != null)
                        ToEmail = mailBox.Address;
                }
            }
        }

        public EmailImap(UniqueId uid, MimeMessage msg, string folder)
        {
            Id = uid.ToString();
            Folder = folder;

            Subject = msg.Subject;
            Date = msg.Date.LocalDateTime;
            From = msg.From.ToString();

            if (msg.From != null)
            {
                var mailBox = msg.From.Mailboxes.FirstOrDefault();
                if (mailBox != null)
                    FromEmail = mailBox.Address;
            }

            To = msg.To.ToString();
            TextBody = msg.TextBody;
            HtmlBody = msg.HtmlBody;
        }
    }

    public class EmailImapShort
    {
        
    }
}
