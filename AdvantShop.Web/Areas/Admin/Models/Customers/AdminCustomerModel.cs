using System;
using AdvantShop.Customers;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Customers
{
    public partial class AdminCustomerModel
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ManagerId { get; set; }
        public string Rating { get; set; }
        public int LastOrderId { get; set; }
        public string LastOrderNumber { get; set; }
        public float OrdersSum { get; set; }
        public int OrdersCount { get; set; }
        public string Location { get; set; }
        public string ManagerName { get; set; }

        public DateTime? BirthDay { get; set; }

        public string BirthDayFormatted
        {
            get { return BirthDay != null ? Culture.ConvertDateWithoutHours(BirthDay.Value) : null; }
        }

        public DateTime RegistrationDateTime { get; set; }

        public string RegistrationDateTimeFormatted
        {
            get { return Culture.ConvertDate(RegistrationDateTime); }
        }

        public bool CanBeDeleted
        {
            get { return CustomerService.CanDelete(CustomerId); } 
        }

        public int GroupId { get; set; }

        #region Export fields

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }

        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }

        public long? CardNumber { get; set; }

        #endregion
    }
}
