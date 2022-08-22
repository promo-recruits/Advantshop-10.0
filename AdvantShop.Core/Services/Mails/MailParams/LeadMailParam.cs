//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Localization;
using AdvantShop.Orders;

namespace AdvantShop.Mails
{
    public abstract class BaseLeadMailTemplate : MailTemplate
    {
        private readonly Lead _lead;

        private readonly string _name;
        private readonly string _phone;
        private readonly string _email;
        private readonly string _organization;
        private readonly string _orderTable;
        private readonly string _shippingName;
        private readonly string _description;
        private readonly string _dealStatus;
        private readonly string _managerName;
        private readonly string _additionalCustomerFields;
        private readonly string _salesFunnelName;
        private readonly string _orderSourceName;
        private readonly string _attachments;

        private const string rowFormat = "<div class='l-row'><div class='l-name vi cs-light' style='color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;'>{0}:</div><div class='l-value vi' style='display: inline-block; margin: 5px 0;'>{1}</div></div>";

        public BaseLeadMailTemplate(Lead lead)
        {
            _lead = lead;


            _name = new List<string> { _lead.LastName, _lead.FirstName, _lead.Patronymic }.Where(x => x.IsNotEmpty()).AggregateString(" ");

            if (_name.IsNullOrEmpty() && _lead.Customer != null)
                _name = new List<string> { _lead.Customer.LastName, _lead.Customer.FirstName, _lead.Customer.Patronymic }.Where(x => x.IsNotEmpty()).AggregateString(" ");

            _phone = _lead.Phone.IsNullOrEmpty() ? (_lead.Customer != null ? _lead.Customer.Phone : null) : _lead.Phone;
            _email = _lead.Email.IsNullOrEmpty() ? (_lead.Customer != null ? _lead.Customer.EMail : null) : _lead.Email;
            _organization = _lead.Organization.IsNullOrEmpty() ? (_lead.Customer != null ? _lead.Customer.Organization : null) : _lead.Organization;

            _orderTable = _lead.LeadItems.Count > 0 ? LeadService.GenerateHtmlLeadItemsTable(_lead.LeadItems, _lead.LeadCurrency) : string.Empty;
            _shippingName = _lead.ShippingName + (_lead.ShippingPickPoint.IsNotEmpty() ? "<br />" + _lead.ShippingPickPoint : string.Empty);
            _description = _lead.Description.IsNotEmpty() ? _lead.Description.Replace("\r\n", "<br />").Replace("\n", "<br />") : string.Empty;
            _dealStatus = _lead.DealStatus != null ? _lead.DealStatus.Name : string.Empty;

            var manager = _lead.ManagerId.HasValue ? ManagerService.GetManager(_lead.ManagerId.Value) : null;
            _managerName = manager != null ? manager.FullName : string.Empty;

            _additionalCustomerFields = string.Empty;
            if (_lead.Customer != null)
            {
                foreach (var customerField in CustomerFieldService.GetMappedCustomerFieldsWithValue(_lead.Customer.Id).Where(x => !string.IsNullOrEmpty(x.Value)))
                    _additionalCustomerFields += string.Format(rowFormat, customerField.Name, customerField.Value);
            }

            var salesFunnel = SalesFunnelService.Get(_lead.SalesFunnelId);
            _salesFunnelName = salesFunnel != null ? salesFunnel.Name : string.Empty;

            var orderSource = OrderSourceService.GetOrderSource(_lead.OrderSourceId);
            _orderSourceName = orderSource != null ? orderSource.Name : string.Empty;

            var attachments = AttachmentService.GetAttachments<LeadAttachment>(_lead.Id);
            _attachments = attachments.Select(x => GetLinkHTML(x.Path, x.FileName)).AggregateString(", ");
        }

        protected override string FormatString(string formatedStr)
        {
            var sb = new StringBuilder(formatedStr);

            sb.Replace("#STORE_NAME#", SettingsMain.ShopName);
            sb.Replace("#LEAD_ID#", _lead.Id.ToString());
            sb.Replace("#NAME#", _name);
            sb.Replace("#PHONE#", _phone);
            sb.Replace("#EMAIL#", _email);
            sb.Replace("#ORGANIZATION#", _organization);
            sb.Replace("#ORDERTABLE#", _orderTable);
            sb.Replace("#SHIPPINGMETHOD#", _shippingName);
            sb.Replace("#CITY#",
                _lead.Customer != null && _lead.Customer.Contacts != null && _lead.Customer.Contacts.Count > 0
                    ? _lead.Customer.Contacts[0].City
                    : _lead.City);
            sb.Replace("#DISTRICT#",
                _lead.Customer != null && _lead.Customer.Contacts != null && _lead.Customer.Contacts.Count > 0
                    ? _lead.Customer.Contacts[0].District
                    : _lead.District);
            sb.Replace("#COUNTRY#",
                _lead.Customer != null && _lead.Customer.Contacts != null && _lead.Customer.Contacts.Count > 0
                    ? _lead.Customer.Contacts[0].Country
                    : _lead.Country);
            sb.Replace("#REGION#",
                _lead.Customer != null && _lead.Customer.Contacts != null && _lead.Customer.Contacts.Count > 0
                    ? _lead.Customer.Contacts[0].Region
                    : _lead.Region);
            sb.Replace("#COMMENTS#", _lead.Comment);
            sb.Replace("#DESCRIPTION#", _description);
            sb.Replace("#DEAL_STATUS#", _dealStatus);
            sb.Replace("#DATE#", Culture.ConvertDate(_lead.CreatedDate));
            sb.Replace("#MANAGER_NAME#", _managerName);
            sb.Replace("#ADDITIONALCUSTOMERFIELDS#", _additionalCustomerFields);
            sb.Replace("#LEADS_LIST#", _salesFunnelName);
            sb.Replace("#SOURCE#", _orderSourceName);
            sb.Replace("#LEAD_ATTACHMENTS#", _attachments);
            sb.Replace("#LEAD_URL#", UrlService.GetAdminUrl("leads#?leadIdInfo=" + _lead.Id));

            return sb.ToString();
        }

        private static string GetLinkHTML(string url, string name)
        {
            return string.Format("<a href=\"{0}\">{1}</a>", url, name);
        }
    }

    public class LeadMailTemplate : BaseLeadMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnLead; }
        }

        public LeadMailTemplate(Lead lead) : base(lead)
        {
        }
    }

    public class LeadAssignedMailTemplate : BaseLeadMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnLeadAssigned; }
        }

        public LeadAssignedMailTemplate(Lead lead) : base(lead)
        {
        }
    }

    public class LeadChangedMailTemplate : BaseLeadMailTemplate
    {
        private readonly string _changesTable;
        private readonly string _modifier;

        public override MailType Type
        {
            get { return MailType.OnLeadChanged; }
        }

        public LeadChangedMailTemplate(Lead lead, string changesTable, string modifier) : base(lead)
        {
            _changesTable = changesTable;
            _modifier = modifier;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#MODIFIER#", _modifier);
            formatedStr = formatedStr.Replace("#CHANGES_TABLE#", _changesTable);

            formatedStr = base.FormatString(formatedStr);

            return formatedStr;
        }
    }

    public class LeadCommentAddedMailTemplate : BaseLeadMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnLeadCommentAdded; }
        }

        private readonly string _author;
        private readonly string _comment;

        public LeadCommentAddedMailTemplate(Lead lead, string author, string comment) : base(lead)
        {
            _author = author;
            _comment = comment;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = base.FormatString(formatedStr);
            formatedStr = formatedStr.Replace("#AUTHOR#", _author);
            formatedStr = formatedStr.Replace("#COMMENT#", _comment);
            return formatedStr;
        }
    }

}