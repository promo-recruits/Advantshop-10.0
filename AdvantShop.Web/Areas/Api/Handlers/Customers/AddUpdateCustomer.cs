using System;
using System.Web;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Customers
{
    public class AddUpdateCustomer : AbstractCommandHandler<AddUpdateCustomerResponse>
    {
        #region Ctor

        private readonly AddUpdateCustomerModel _model;
        private readonly bool _isEditMode;
        private Customer _customer;

        public AddUpdateCustomer(AddUpdateCustomerModel model) : this(Guid.Empty, model){}

        public AddUpdateCustomer(Guid id, AddUpdateCustomerModel model)
        {
            _model = model;
            _model.Id = id;
            _isEditMode = id != Guid.Empty;
        }

        #endregion

        protected override void Validate()
        {
            if (!ValidationHelper.IsValidEmail(_model.Email))
                throw new BlException("Не валидный email");

            if (!_isEditMode)
            {
                if (!string.IsNullOrWhiteSpace(_model.Email) && CustomerService.IsEmailExist(_model.Email))
                    throw new BlException("Пользователь с таким email уже существует");

                if (_model.Phone != null)
                {
                    var standardPhone = StringHelper.ConvertToStandardPhone(_model.Phone.ToString(), true, true);
                    var phone = standardPhone != null ? standardPhone.ToString() : _model.Phone.ToString();

                    if (CustomerService.IsPhoneExist(phone, standardPhone))
                        throw new BlException("Пользователь с таким телефоном уже существует");
                }
            }
            else
            {
                _customer = CustomerService.GetCustomer(_model.Id);
                if (_customer == null)
                    throw new BlException("Покупатель не найден");

                if (_customer.EMail != _model.Email && !string.IsNullOrWhiteSpace(_model.Email) && CustomerService.IsEmailExist(_model.Email))
                    throw new BlException("Пользователь с таким email уже существует");

                if (_model.Phone != null)
                {
                    var standardPhone = StringHelper.ConvertToStandardPhone(_model.Phone.ToString(), true, true);
                    var phone = standardPhone != null ? standardPhone.ToString() : _model.Phone.ToString();

                    if (_customer.StandardPhone != standardPhone && CustomerService.IsPhoneExist(phone, standardPhone))
                        throw new BlException("Пользователь с таким телефоном уже существует");
                }
            }
        }

        private AddUpdateCustomerResponse AddUpdate()
        {
            var customer = _customer ??
                           new Customer(CustomerGroupService.DefaultCustomerGroup) {CustomerRole = Role.User};

            customer.EMail = _model.Email.EncodeOrEmpty();

            if (_model.FirstName != null)
                customer.FirstName = _model.FirstName.EncodeOrEmpty();

            if (_model.LastName != null)
                customer.LastName = _model.LastName.EncodeOrEmpty();

            if (_model.Patronymic != null)
                customer.Patronymic = _model.Patronymic.EncodeOrEmpty();

            if (_model.Organization != null)
                customer.Organization = _model.Organization.EncodeOrEmpty();

            if (_model.Phone != null)
            {
                var standardPhone = StringHelper.ConvertToStandardPhone(_model.Phone.ToString(), true, true);
                var phone = standardPhone != null ? standardPhone.ToString() : _model.Phone.ToString();
                
                customer.Phone = standardPhone != null ? standardPhone.ToString() : phone;
                customer.StandardPhone = standardPhone;
            }

            if (_model.SubscribedForNews != null)
                customer.SubscribedForNews = _model.SubscribedForNews.Value;

            if (_model.BirthDay != null)
                customer.BirthDay = _model.BirthDay;

            if (_model.AdminComment != null)
                customer.AdminComment = _model.AdminComment.EncodeOrEmpty();

            if (_model.ManagerId != null && ManagerService.GetManager(_model.ManagerId.Value) != null)
                customer.ManagerId = _model.ManagerId;

            if (_model.GroupId != null && CustomerGroupService.GetCustomerGroup(_model.GroupId.Value) != null)
                customer.CustomerGroupId = _model.GroupId.Value;

            if (!_isEditMode)
            {
                customer.Password = _model.Password.DefaultOrEmpty();

                customer.Id = CustomerService.InsertNewCustomer(customer);

                if (customer.Id == Guid.Empty)
                    throw new BlException("Не удалось создать пользователя");

                if (_model.PartnerId.HasValue && PartnerService.GetPartner(_model.PartnerId.Value) != null)
                    PartnerService.AddBindedCustomer(new BindedCustomer { CustomerId = customer.Id, PartnerId = _model.PartnerId.Value });

                //var mail = new RegistrationMailTemplate(customer);
                //MailService.SendMailNow(SettingsMail.EmailForRegReport, mail, replyTo: customer.EMail);
            }
            else
            {
                CustomerService.UpdateCustomer(customer);
            }

            if (_model.Fields != null)
            {
                foreach (var field in _model.Fields)
                    CustomerFieldService.AddUpdateMap(customer.Id, field.Id, field.Value ?? "", !_isEditMode);
            }

            if (_model.Contact != null)
            {
                var contact = CustomerService.GetCustomerContact(_model.Contact.ContactId) ??
                              new CustomerContact() { CustomerGuid = customer.Id };

                contact.Name = !string.IsNullOrEmpty(contact.Name.EncodeOrEmpty())
                    ? contact.Name.EncodeOrEmpty()
                    : (customer.FirstName + " " + customer.LastName).Trim();
                contact.City = _model.Contact.City.EncodeOrEmpty();
                contact.District = _model.Contact.District.EncodeOrEmpty();
                contact.Zip = _model.Contact.Zip.EncodeOrEmpty();

                contact.Country = _model.Contact.Country.EncodeOrEmpty();
                var country = CountryService.GetCountryByName(contact.Country);
                contact.CountryId = country?.CountryId ?? 0;

                contact.Region = _model.Contact.Region.EncodeOrEmpty();
                contact.RegionId = RegionService.GetRegionIdByName(contact.Region);

                contact.Street = _model.Contact.Street.EncodeOrEmpty();
                contact.House = _model.Contact.House.EncodeOrEmpty();
                contact.Apartment = _model.Contact.Apartment.EncodeOrEmpty();
                contact.Structure = _model.Contact.Structure.EncodeOrEmpty();
                contact.Entrance = _model.Contact.Entrance.EncodeOrEmpty();
                contact.Floor = _model.Contact.Floor.EncodeOrEmpty();

                if (contact.ContactId == Guid.Empty)
                {
                    CustomerService.AddContact(contact, customer.Id);
                }
                else
                {
                    CustomerService.UpdateContact(contact);
                }
            }

            return new AddUpdateCustomerResponse() { Id = customer.Id };
        }

        protected override AddUpdateCustomerResponse Handle()
        {
            try
            {
                return AddUpdate();
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
                throw;
            }
        }
    }
}