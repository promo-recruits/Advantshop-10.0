﻿using System;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Webhook.Models.Api
{
    public class OrderCustomerModel
    {
        public static OrderCustomerModel FromOrderCustomer(OrderCustomer orderCustomer)
        {
            if (orderCustomer == null)
                return null;

            return new OrderCustomerModel
            {
                CustomerId = orderCustomer.CustomerID,
                FirstName = orderCustomer.FirstName,
                LastName = orderCustomer.LastName,
                Patronymic = orderCustomer.Patronymic,
                Organization = orderCustomer.Organization,
                Email = orderCustomer.Email,
                Phone = orderCustomer.Phone,
                Country = orderCustomer.Country,
                Region = orderCustomer.Region,
                District = orderCustomer.District,
                City = orderCustomer.City,
                Zip = orderCustomer.Zip,
                CustomField1 = orderCustomer.CustomField1,
                CustomField2 = orderCustomer.CustomField2,
                CustomField3 = orderCustomer.CustomField3,
                Street = orderCustomer.Street,
                House = orderCustomer.House,
                Apartment = orderCustomer.Apartment,
                Structure = orderCustomer.Structure,
                Entrance = orderCustomer.Entrance,
                Floor = orderCustomer.Floor,
            };
        }

        public Guid? CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Organization { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Apartment { get; set; }
        public string Structure { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
    }
}
