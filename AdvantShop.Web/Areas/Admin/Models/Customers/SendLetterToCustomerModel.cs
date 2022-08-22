using System;
using System.Collections.Generic;
using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Customers
{
    public class GetLetterToCustomerModel
    {
        public string CustomerId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        
        public int TemplateId { get; set; }

        public string ReId { get; set; }
    }

    public class GetLetterToCustomerResult
    {
        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }


        public GetLetterToCustomerResult()
        {
            Subject = "";
            Text = "";
        }
    }


    public class SendLetterToCustomerModel
    {
        public Guid? CustomerId { get; set; }
        public List<Guid> CustomerIds { get; set; }
        public List<int> SubscriptionIds { get; set; }
        public string Email { get; set; }

        public string Subject { get; set; }
        public string Text { get; set; }

        public string PageType { get; set; }
    }

    public class SendLetterToCustomerItemModel
    {
        public string Email { get; set; }
        public Guid? CustomerId { get; set; }
        public Customer Customer { get; set; }

        public SendLetterToCustomerItemModel()
        {
            
        }

        public SendLetterToCustomerItemModel(Customer c)
        {
            CustomerId = c.Id;
            Email = c.EMail;
            Customer = c;
        }

        public SendLetterToCustomerItemModel(AdvantShop.Customers.Subscription s)
        {
            CustomerId = s.CustomerId;
            Email = s.Email;
            Customer = s.CustomerId.HasValue ? new Customer
            {
                FirstName = s.FirstName,
                LastName = s.LastName
            } : null;
        }
    }
}
