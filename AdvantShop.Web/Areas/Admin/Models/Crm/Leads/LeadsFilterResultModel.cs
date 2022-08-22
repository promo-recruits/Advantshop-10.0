using System;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Helpers;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class LeadsFilterResultModel
    {
        public int Id { get; set; }

        public Guid? CustomerId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPatronymic { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Organization { get; set; }

        public string FullName
        {
            get
            {
                var name = StringHelper.AggregateStrings(" ", LastName, FirstName, Patronymic);

                if (string.IsNullOrWhiteSpace(name) && CustomerId != null)
                    name = StringHelper.AggregateStrings(" ", CustomerLastName, CustomerFirstName, CustomerPatronymic);

                if (!string.IsNullOrEmpty(Organization))
                    name += !string.IsNullOrEmpty(name) ? " (" + Organization + ")" : Organization;

                return name;
            }
        }

        public string ManagerName { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public string CreatedDateFormatted { get { return Culture.ConvertDate(CreatedDate); } }

        public float Sum { get; set; }

        public string SumFormatted
        {
            get
            {
                return PriceFormatService.FormatPrice(Sum, CurrencyValue, CurrencySymbol, CurrencyCode, IsCodeBefore);
            }
        }
        public float ProductsSum { get; set; }
        public float ProductsCount { get; set; }
        public string DealStatusName { get; set; }
        public string SalesFunnelName { get; set; }
        public int SalesFunnelId { get; set; }


        public string CurrencyCode { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }


        // additional fields for export to CSV
        //public string Location { get; set; }
        public DateTime? CustomerBirthDay { get; set; }
        //public DateTime? CustomerRegistrationDateTime { get; set; }
        //public int LastOrderId { get; set; }
        //public string LastOrderNumber { get; set; }
        //public int OrdersCount { get; set; }
        //public float OrdersSum { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }

        public string Comment { get; set; }
    }
}
