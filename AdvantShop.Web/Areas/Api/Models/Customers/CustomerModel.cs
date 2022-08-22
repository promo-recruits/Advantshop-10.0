using System;
using System.Collections.Generic;
using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Customers
{
    public class CustomerModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public long? Phone { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("patronymic")]
        public string Patronymic { get; set; }

        [JsonProperty("organization")]
        public string Organization { get; set; }

        [JsonProperty("subscribedForNews")]
        public bool? SubscribedForNews { get; set; }

        [JsonProperty("birthday")]
        public DateTime? BirthDay { get; set; }

        [JsonProperty("adminComment")]
        public string AdminComment { get; set; }

        [JsonProperty("managerId")]
        public int? ManagerId { get; set; }

        [JsonProperty("groupId")]
        public int? GroupId { get; set; }

        [JsonProperty("password"), JsonIgnore]
        public string Password { get; set; }

        [JsonProperty("contact", NullValueHandling = NullValueHandling.Ignore)]
        public CustomerContactModel Contact { get; set; }

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<CustomerFieldModel> Fields { get; set; }
    }

    public class CustomerContactModel
    {
        [JsonProperty("contactId")]
        public Guid ContactId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("house")]
        public string House { get; set; }

        [JsonProperty("apartment")]
        public string Apartment { get; set; }

        [JsonProperty("structure")]
        public string Structure { get; set; }

        [JsonProperty("entrance")]
        public string Entrance { get; set; }

        [JsonProperty("floor")]
        public string Floor { get; set; }

        public CustomerContactModel()
        {
        }

        public CustomerContactModel(CustomerContact contact)
        {
            ContactId = contact.ContactId;
            Name = contact.Name;
            Country = contact.Country;
            City = contact.City;
            District = contact.District;
            Region = contact.Region;
            Zip = contact.Zip;
            Street = contact.Street;
            House = contact.House;
            Apartment = contact.Apartment;
            Structure = contact.Structure;
            Entrance = contact.Entrance;
            Floor = contact.Floor;
        }
    }

    public class CustomerFieldModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}