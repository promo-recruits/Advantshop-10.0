//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using Newtonsoft.Json;

namespace AdvantShop.Customers
{
    [Serializable]
    public class Customer : ICustomer, ITriggerObject
    {
        public Customer()
        {
            Id = Guid.Empty;
            Enabled = true;
        }

        public Customer(int groupId) : this()
        {
            this.CustomerGroupId = groupId;
        }

        public Customer(bool isRegistered) : this()
        {
            RegistredUser = isRegistered;
        }

        public Guid Id { get; set; }

        public int InnerId { get; set; }

        private int _customerGroupId = 0;

        public int CustomerGroupId
        {
            get
            {
                if (_customerGroupId != 0)
                    return _customerGroupId;
                return (_customerGroupId = CustomerGroupService.DefaultCustomerGroup);
            }
            set { _customerGroupId = value; }
        }

        public string Password { get; set; }

        [Compare("Core.Customers.Customer.FirstName")]
        public string FirstName { get; set; }

        [Compare("Core.Customers.Customer.LastName")]
        public string LastName { get; set; }

        [Compare("Core.Customers.Customer.Patronymic")]
        public string Patronymic { get; set; }

        [Compare("Core.Customers.Customer.Organization")]
        public string Organization { get; set; }

        [Compare("Core.Customers.Customer.Phone")]
        public string Phone { get; set; }

        public long? StandardPhone { get; set; }

        public DateTime RegistrationDateTime { get; set; }

        [Compare("Core.Customers.Customer.Email")]
        public string EMail { get; set; }

        private bool? _subscribedForNews;
        public bool SubscribedForNews
        {
            get
            {
                return _subscribedForNews != null
                    ? _subscribedForNews.Value
                    : (_subscribedForNews = SubscriptionService.IsSubscribe(EMail)).Value;
            }
            set { _subscribedForNews = value; }
        }

        public long? BonusCardNumber { get; set; }

        public string AdminComment { get; set; }

        public int Rating { get; set; }

        public Role CustomerRole { get; set; }

        private CustomerGroup _customerGroup;
        public CustomerGroup CustomerGroup
        {
            get { return _customerGroup ?? (_customerGroup = CustomerGroupService.GetCustomerGroup(CustomerGroupId)) 
                                        ?? (_customerGroup = CustomerGroupService.GetDefaultCustomerGroup()) 
                                        ?? new CustomerGroup() { CustomerGroupId = CustomerGroupService.DefaultCustomerGroup }; }
        }

        private List<CustomerContact> _contacts;
        public List<CustomerContact> Contacts
        {
            get { return _contacts ?? (_contacts = CustomerService.GetCustomerContacts(Id)); }
            set { _contacts = value; }
        }
        
        public bool IsAdmin
        {
            get { return CustomerRole == Role.Administrator; }
        }


        public bool IsModerator
        {
            get { return CustomerRole == Role.Moderator; }
        }

        private bool? _isManager;
        public bool IsManager
        {
            get
            {
                if (!_isManager.HasValue)
                    _isManager = ManagerService.CustomerIsManager(Id);

                return _isManager.Value;
            }
        }

        public bool HasRoleAction(RoleAction key)
        {
            return IsAdmin || IsVirtual || (IsModerator && RoleActionService.HasCustomerRoleAction(Id, key));
        }

        public bool IsBuyer
        {
            get { return (!RegistredUser && !IsVirtual) || (!IsAdmin && !IsModerator && !IsManager); }
        }

        public bool CanBeDeleted
        {
            get { return CustomerService.CanDelete(Id); }
        }

        public bool RegistredUser { get; protected set; }

        public bool IsVirtual { get; set; }

        /// <summary>
        /// Customer's manager ID
        /// </summary>
        public int? ManagerId { get; set; }

        public bool Enabled { get; set; }

        public Guid? HeadCustomerId { get; set; }

        public DateTime? BirthDay { get; set; }

        public string BirthDayFormatted
        {
            get { return BirthDay != null ? Culture.ConvertDateWithoutHours(BirthDay.Value) : null; }
        }

        public string City { get; set; }

        public string Avatar { get; set; }

        public string Code { get; set; }
        
        private Manager _manager;

        /// <summary>
        /// Customer's manager
        /// </summary>
        [JsonIgnore]
        public Manager Manager
        {
            get { return _manager ?? (_manager = ManagerId.HasValue ? ManagerService.GetManager(ManagerId.Value) : null); }
        }

        public int SortOrder { get; set; }

        public CustomerClientStatus ClientStatus { get; set; }

        public string RegisteredFrom { get; set; }

        public TriggerProcessObject GetTriggerProcessObject()
        {
            return new TriggerProcessObject()
            {
                EntityId = InnerId,
                Email = EMail,
                Phone = StandardPhone ?? StringHelper.ConvertToStandardPhone(Phone) ?? 0,
                CustomerId = Id,
            };
        }

        private IList<Tag> _tag;
        public IList<Tag> Tags
        {
            get { return _tag ?? (_tag = TagService.Gets(Id,  true)); }
            set { _tag = value; }
        }
    }


    public enum CustomerClientStatus
    {
        None = 0,
        Vip = 1,
        Bad = 2
    }
}