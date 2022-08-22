using System.Linq;
using AdvantShop.Core.Services.Api;
using AdvantShop.Customers;

namespace AdvantShop.Areas.Api.Models.Customers
{

    public class GetCustomerResponse : CustomerModel, IApiResponse
    {
        public GetCustomerResponse()
        { 
        }

        public GetCustomerResponse(Customer customer)
        {
            Id = customer.Id;
            Email = customer.EMail;
            Phone = customer.StandardPhone;
            FirstName = customer.FirstName ?? "";
            LastName = customer.LastName ?? "";
            Patronymic = customer.Patronymic ?? "";
            Organization = customer.Organization ?? "";
            SubscribedForNews = customer.SubscribedForNews;
            BirthDay = customer.BirthDay;
            AdminComment = customer.AdminComment ?? "";
            Contact = customer.Contacts != null && customer.Contacts.Count > 0
                ? new CustomerContactModel(customer.Contacts[0])
                : new CustomerContactModel();

            ManagerId = customer.ManagerId;
            GroupId = customer.CustomerGroupId;

            Fields = CustomerFieldService.GetCustomerFieldsWithValue(customer.Id).Select(x =>
                new CustomerFieldModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value
                }).ToList();
        }
    }
}