using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Payment;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Security;
using AdvantShop.Shipping;
using AdvantShop.Trial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Taxes;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Orders
{
    public class MyCheckout
    {
        public CheckoutData Data { get; set; }

        public List<BaseShippingOption> AvailableShippingOptions(List<PreOrderItem> preorderList = null)
        {
            var cart = ShoppingCartService.CurrentShoppingCart;

            var ignoreCartItems = ModulesExecuter.GetIgnoreShippingCartItems();
            if (ignoreCartItems.Count > 0)
            {
                foreach (var item in ignoreCartItems)
                    cart.Remove(item);
            }

            var preOrder = new PreOrder
            {
                CountryDest = this.Data.Contact.Country,
                RegionDest = this.Data.Contact.Region,
                CityDest = this.Data.Contact.City,
                DistrictDest = this.Data.Contact.District,
                ZipDest = this.Data.Contact.Zip,
                ShippingOption = this.Data.SelectShipping,
                PaymentOption = this.Data.SelectPayment,
                Currency = CurrencyService.CurrentCurrency,
                TotalDiscount = cart.TotalDiscount + GetBonusDiscount(Data, cart),
                BonusCardId = this.Data.User.BonusCardId,
                BonusUseIt = this.Data.Bonus.UseIt
            };
            var items = preorderList ?? cart.Select(shpItem => new PreOrderItem(shpItem)).ToList();

            var manager = new ShippingManager(preOrder, items, cart.TotalPrice - cart.TotalDiscount);
            return manager.GetOptions();
        }

        public void UpdateSelectShipping(List<PreOrderItem> preorderList, BaseShippingOption shipping, List<BaseShippingOption> shippingOptions = null)
        {
            this.Data.SelectShipping = shipping;

            var cart = ShoppingCartService.CurrentShoppingCart;

            var ignoreCartItems = ModulesExecuter.GetIgnoreShippingCartItems();
            if (ignoreCartItems.Count > 0)
            {
                foreach (var item in ignoreCartItems)
                    cart.Remove(item);
            }

            List<PreOrderItem> items = null;
            BaseShippingOption option = null;

            if (shippingOptions != null)
            {
                option = (shipping != null ? shippingOptions.FirstOrDefault(x => x.Id == shipping.Id) : null) ??
                         shippingOptions.FirstOrDefault();
            }
            else
            {
                var preOrder = new PreOrder
                {
                    CountryDest = this.Data.Contact.Country,
                    RegionDest = this.Data.Contact.Region,
                    CityDest = this.Data.Contact.City,
                    DistrictDest = this.Data.Contact.District,
                    ZipDest = this.Data.Contact.Zip,
                    ShippingOption = this.Data.SelectShipping,
                    PaymentOption = this.Data.SelectPayment,
                    Currency = CurrencyService.CurrentCurrency,
                    TotalDiscount = cart.TotalDiscount + GetBonusDiscount(Data, cart),
                    BonusUseIt = this.Data.Bonus.UseIt,
                    BonusCardId = this.Data.User.BonusCardId
                };
                items = preorderList ?? cart.Select(shpItem => new PreOrderItem(shpItem)).ToList();

                var manager = new ShippingManager(preOrder, items, cart.TotalPrice - cart.TotalDiscount);
                var options = manager.GetOptions(false);

                option = (shipping != null ? options.FirstOrDefault(x => x.Id == shipping.Id) : null) ??
                         options.FirstOrDefault();
            }

            if (option != null)
            {
                option.Update(shipping);
                if (this.Data.SelectPayment != null)
                    if (option.ApplyPay(this.Data.SelectPayment))
                    {
                        var totalPrice = (cart.TotalPrice - cart.TotalPriceIgnoreDiscount - cart.TotalDiscount).RoundPrice(CurrencyService.CurrentCurrency.Rate, CurrencyService.CurrentCurrency);
                        var modules = AttachedModules.GetModules<IShippingCalculator>();
                        var options = new List<BaseShippingOption> { option };
                        if (items == null)
                            items = preorderList ?? cart.Select(shpItem => new PreOrderItem(shpItem)).ToList();
                        foreach (var module in modules)
                        {
                            if (module != null)
                            {
                                var classInstance = (IShippingCalculator)Activator.CreateInstance(module);
                                classInstance.ProcessOptions(options, items, totalPrice);
                            }
                        }
                    }
            }

            this.Data.SelectShipping = option;
            this.Update();
        }

        public List<BasePaymentOption> AvailablePaymentOptions(List<PreOrderItem> preorderList = null)
        {
            var cart = ShoppingCartService.CurrentShoppingCart;

            var preOrder = new PreOrder()
            {
                CountryDest = this.Data.Contact.Country,
                CityDest = this.Data.Contact.City,
                DistrictDest = this.Data.Contact.District,
                RegionDest = this.Data.Contact.Region,
                BonusCardId = this.Data.User.BonusCardId,
                BonusUseIt = this.Data.Bonus.UseIt,
                ShippingOption = this.Data.SelectShipping,
                TotalDiscount = cart.TotalDiscount + GetBonusDiscount(Data, cart)
            };
            var items = preorderList ?? cart.Select(shpItem => new PreOrderItem(shpItem)).ToList();

            var manager = new PaymentManager(preOrder, items, preorderList == null ? cart : null);
            var result = manager.GetOptions();
            return result;
        }

        public bool UpdateSelectPayment(List<PreOrderItem> preorderList, BasePaymentOption payment, List<BasePaymentOption> paymentOptions = null)
        {
            this.Data.SelectPayment = payment;

            var cart = ShoppingCartService.CurrentShoppingCart;

            var preOrder = new PreOrder
            {
                CountryDest = this.Data.Contact.Country,
                CityDest = this.Data.Contact.City,
                DistrictDest = this.Data.Contact.District,
                RegionDest = this.Data.Contact.Region,
                ShippingOption = this.Data.SelectShipping,
                PaymentOption = this.Data.SelectPayment,
                Currency = CurrencyService.CurrentCurrency,
                TotalDiscount = cart.TotalDiscount + GetBonusDiscount(Data, cart),
                BonusUseIt = this.Data.Bonus.UseIt,
                BonusCardId = this.Data.User.BonusCardId
            };

            var items = preorderList ?? cart.Select(shpItem => new PreOrderItem(shpItem)).ToList();

            BasePaymentOption option = null;
            PaymentManager manager = null;

            if (paymentOptions != null)
            {
                option = (payment != null ? paymentOptions.FirstOrDefault(x => x.Id == payment.Id) : null) ??
                         paymentOptions.FirstOrDefault();
            }
            else
            {
                manager = new PaymentManager(preOrder, items, preorderList == null ? cart : null);
                var options = manager.GetOptions(true);

                option = (payment != null ? options.FirstOrDefault(x => x.Id == payment.Id) : null) ??
                         options.FirstOrDefault();
            }

            if (option != null)
                option.Update(payment);

            this.Data.SelectPayment = option;

            var prevFinalRateShipping = this.Data.SelectShipping != null
                ? this.Data.SelectShipping.FinalRate
                : 0f;

            if (this.Data.SelectShipping != null && this.Data.SelectShipping.ApplyPay(this.Data.SelectPayment))
            {
                // стоимость доставки изменилась, нужно обновить оплату, 
                // т.к. ее нацена зависит и от стоимости доставки (наценка метода оплаты)
                if (prevFinalRateShipping != this.Data.SelectShipping.FinalRate && manager != null)
                    this.Data.SelectPayment = manager.UpdatePaymentByNewShipping(this.Data.SelectPayment, this.Data.SelectShipping);

                var totalPrice = (cart.TotalPrice - cart.TotalPriceIgnoreDiscount - cart.TotalDiscount).RoundPrice(CurrencyService.CurrentCurrency.Rate, CurrencyService.CurrentCurrency);
                var modules = AttachedModules.GetModules<IShippingCalculator>();
                var options = new List<BaseShippingOption> { this.Data.SelectShipping };
                foreach (var module in modules)
                {
                    if (module != null)
                    {
                        var classInstance = (IShippingCalculator)Activator.CreateInstance(module);
                        classInstance.ProcessOptions(options, items, totalPrice);
                    }
                }
                //foreach (var item in options)
                //{
                //    item.Rate = item.Rate.RoundPrice(CurrencyService.CurrentCurrency);
                //}
            }

            this.Update();
            return this.Data.SelectPayment == null;
        }

        public static MyCheckout Factory(Guid customerId)
        {
            var model = new MyCheckout() { Data = OrderConfirmationService.Get(customerId) };

            if (model.Data == null)
            {
                model.Data = new CheckoutData
                {
                    ShopCartHash = ShoppingCartService.CurrentShoppingCart.GetHashCode(),
                    User = { Id = customerId }
                };
                OrderConfirmationService.Add(CustomerContext.CustomerId, model.Data);
            }
            return model;
        }

        public void Update()
        {
            OrderConfirmationService.Update(CustomerContext.CustomerId, Data);
        }

        public void ProcessUser()
        {
            var customer = CustomerContext.CurrentCustomer;

            if (customer.RegistredUser)
            {
                ProcessRegisteredUser(customer);
                return;
            }

            var customerByEmail = !string.IsNullOrEmpty(Data.User.Email) ? CustomerService.GetCustomerByEmail(Data.User.Email) : null;
            if (customerByEmail != null && customerByEmail.Id != Guid.Empty)
            {
                Data.User.Id = customerByEmail.Id;
                if (BonusSystem.IsActive)
                {
                    var bonusCard = BonusSystemService.GetCard(customerByEmail.Id);
                    if (bonusCard != null)
                        Data.User.BonusCardId = bonusCard.CardId;
                }
                return;
            }

            ProcessUnRegisteredUser();
        }

        private void ProcessRegisteredUser(Customer customer)
        {
            try
            {
                var user = Data.User;
                
                if (string.IsNullOrEmpty(user.Email))
                    user.Email = customer.EMail;

                var needUpdateCustomer = false;

                if (!string.IsNullOrWhiteSpace(user.FirstName) && customer.FirstName != user.FirstName)
                {
                    customer.FirstName = user.FirstName;
                    needUpdateCustomer = true;
                }

                if (!string.IsNullOrWhiteSpace(user.LastName) && customer.LastName != user.LastName)
                {
                    customer.LastName = user.LastName;
                    needUpdateCustomer = true;
                }

                if (!string.IsNullOrWhiteSpace(user.Patronymic) && customer.Patronymic != user.Patronymic)
                {
                    customer.Patronymic = user.Patronymic;
                    needUpdateCustomer = true;
                }

                if (!string.IsNullOrWhiteSpace(user.Phone) && customer.Phone != user.Phone)
                {
                    customer.Phone = user.Phone;
                    customer.StandardPhone =
                        !string.IsNullOrEmpty(user.Phone)
                            ? StringHelper.ConvertToStandardPhone(user.Phone)
                            : null;

                    needUpdateCustomer = true;
                }

                if (SettingsCheckout.IsShowBirthDay && user.BirthDay != null && user.BirthDay != customer.BirthDay)
                {
                    customer.BirthDay = user.BirthDay;
                    needUpdateCustomer = true;
                }

                if (customer.BonusCardNumber == null && user.BonusCardId != null)
                {
                    var card = BonusSystemService.GetCard(user.BonusCardId);
                    if (card != null && !card.Blocked)
                    {
                        customer.BonusCardNumber = card.CardNumber;
                        needUpdateCustomer = true;
                    }
                }
                
                if (needUpdateCustomer)
                    CustomerService.UpdateCustomer(customer);

                if (customer.Contacts.Count == 0)
                {
                    var country = !string.IsNullOrWhiteSpace(Data.Contact.Country)
                        ? CountryService.GetCountryByName(Data.Contact.Country)
                        : null;

                    CustomerService.AddContact(new CustomerContact()
                    {
                        Name = StringHelper.AggregateStrings(" ", user.LastName, user.FirstName, user.Patronymic),
                        Country = Data.Contact.Country,
                        CountryId = country?.CountryId ?? 0,
                        Region = Data.Contact.Region,
                        City = Data.Contact.City,
                        District = Data.Contact.District,
                        Zip = Data.Contact.Zip,

                        Street = Data.Contact.Street,
                        House = Data.Contact.House,
                        Apartment = Data.Contact.Apartment,
                        Structure = Data.Contact.Structure,
                        Entrance = Data.Contact.Entrance,
                        Floor = Data.Contact.Floor

                    }, customer.Id);
                }

                if (user.CustomerFields != null)
                {
                    foreach (var customerField in user.CustomerFields)
                    {
                        CustomerFieldService.AddUpdateMap(customer.Id, customerField.Id, customerField.Value ?? "", true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private void ProcessUnRegisteredUser()
        {
            try
            {
                if (!Data.User.WantRegist)
                    Data.User.Password = StringHelper.GeneratePassword(8);

                var customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    Id = CustomerContext.CustomerId,
                    Password = Data.User.Password,
                    FirstName = Data.User.FirstName,
                    LastName = Data.User.LastName,
                    Patronymic = Data.User.Patronymic,
                    Phone = Data.User.Phone,
                    StandardPhone = StringHelper.ConvertToStandardPhone(Data.User.Phone),
                    SubscribedForNews = true,
                    EMail = Data.User.Email,
                    CustomerRole = Role.User,
                    BirthDay = SettingsCheckout.IsShowBirthDay ? Data.User.BirthDay : null
                };

                CustomerService.InsertNewCustomer(customer, Data.User.CustomerFields);
                if (customer.Id == Guid.Empty)
                    return;

                if (Data.User.WantRegist && BonusSystem.IsActive && (!Saas.SaasDataService.IsSaasEnabled || Saas.SaasDataService.CurrentSaasData.BonusSystem))
                {
                    CreateBonusCard(customer);
                }

                Data.User.Id = customer.Id;

                AuthorizeService.SignIn(customer.EMail, customer.Password, false, true);

                var country = !string.IsNullOrWhiteSpace(Data.Contact.Country)
                    ? CountryService.GetCountryByName(Data.Contact.Country)
                    : null;

                var contact = new CustomerContact()
                {
                    Name = customer.GetFullName(),
                    Country = Data.Contact.Country,
                    CountryId = country != null ? country.CountryId : 0,
                    Region = Data.Contact.Region,
                    District = Data.Contact.District,
                    City = Data.Contact.City,
                    Zip = Data.Contact.Zip,

                    Street = Data.Contact.Street,
                    House = Data.Contact.House,
                    Apartment = Data.Contact.Apartment,
                    Structure = Data.Contact.Structure,
                    Entrance = Data.Contact.Entrance,
                    Floor = Data.Contact.Floor
                };

                CustomerService.AddContact(contact, customer.Id);
                
                if (Data.User.WantRegist && !string.IsNullOrEmpty(customer.EMail))
                {
                    var mail = new RegistrationMailTemplate(customer);

                    MailService.SendMailNow(CustomerContext.CustomerId, customer.EMail, mail);
                    MailService.SendMailNow(SettingsMail.EmailForRegReport, mail, replyTo: customer.EMail);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        public Order ProcessOrder(string customData, OrderType orderType, bool isLanding)
        {
            ProcessUser();

            var cart = ShoppingCartService.CurrentShoppingCart;

            var order = CreateOrder(cart, customData, orderType, isLanding);

            var certificate = cart.Certificate;
            if (certificate != null)
            {
                certificate.ApplyOrderNumber = order.Number;
                certificate.Used = true;
                certificate.Enable = true;

                GiftCertificateService.DeleteCustomerCertificate(certificate.CertificateId);
                GiftCertificateService.UpdateCertificateById(certificate);
            }

            var coupon = cart.Coupon;
            if (coupon != null && cart.TotalPrice >= coupon.MinimalOrderPrice)
            {
                coupon.ActualUses += 1;
                CouponService.UpdateCoupon(coupon);
                CouponService.DeleteCustomerCoupon(coupon.CouponID);
            }

            ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart, Data.User.Id);
            ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart, CustomerContext.CustomerId);

            OrderConfirmationService.Delete(CustomerContext.CustomerId);

            return order;
        }

        private Order CreateOrder(ShoppingCart cart, string customData, OrderType orderType, bool isLanding)
        {
            var currency = CurrencyService.CurrentCurrency;
            var orderSource = OrderSourceService.GetOrderSource(OrderType.ShoppingCart);

            if (orderType != OrderType.None)
                orderSource = OrderSourceService.GetOrderSource(orderType);
            else if (Data.LpId != null)
            {
                var lp = new LpService().Get(Data.LpId.Value);
                LpSite site;
                if (lp != null && (site = new LpSiteService().Get(lp.LandingSiteId)) != null)
                    orderSource = OrderSourceService.GetOrderSource(OrderType.LandingPage, site.Id, site.Name);
                else
                    orderSource = OrderSourceService.GetOrderSource(OrderType.LandingPage);
            }
            else if (SettingsDesign.IsSocialTemplate)
                orderSource = OrderSourceService.GetOrderSource(OrderType.SocialNetworks);
            else if (SettingsDesign.IsMobileTemplate)
                orderSource = OrderSourceService.GetOrderSource(OrderType.Mobile);

            var customer = CustomerContext.CurrentCustomer;

            var order = new Order
            {
                OrderCustomer = new OrderCustomer
                {
                    CustomerIP = HttpContext.Current.Request.UserHostAddress,
                    CustomerID = Data.User.Id,
                    FirstName = Data.User.FirstName,
                    LastName = Data.User.LastName,
                    Patronymic = Data.User.Patronymic,
                    Organization = Data.User.Organization,
                    Email = Data.User.Email,
                    Phone = Data.User.Phone,
                    StandardPhone =
                        !string.IsNullOrWhiteSpace(Data.User.Phone)
                            ? StringHelper.ConvertToStandardPhone(Data.User.Phone)
                            : null,

                    Country = Data.Contact.Country,
                    Region = Data.Contact.Region,
                    District = Data.Contact.District,
                    City = Data.Contact.City,
                    Zip = Data.Contact.Zip,
                    CustomField1 = Data.Contact.CustomField1,
                    CustomField2 = Data.Contact.CustomField2,
                    CustomField3 = Data.Contact.CustomField3,

                    Street = Data.Contact.Street,
                    House = Data.Contact.House,
                    Apartment = Data.Contact.Apartment,
                    Structure = Data.Contact.Structure,
                    Entrance = Data.Contact.Entrance,
                    Floor = Data.Contact.Floor
                },
                OrderCurrency = currency,
                OrderStatusId = OrderStatusService.DefaultOrderStatus,
                AffiliateID = 0,
                OrderDate = DateTime.Now,
                CustomerComment = Data.CustomerComment,
                ManagerId = customer.ManagerId,

                GroupName = customer.CustomerGroup.GroupName,
                GroupDiscount = customer.CustomerGroup.GroupDiscount,
                OrderDiscount = cart.DiscountPercentOnTotalPrice,
                OrderSourceId = orderSource.Id,
                CustomData = customData,
                LpId = Data.LpId
            };

            foreach (var orderItem in cart.Select(item => (OrderItem)item))
            {
                order.OrderItems.Add(orderItem);
            }

            order.ShippingMethodId = Data.SelectShipping.MethodId;
            order.PaymentMethodId = Data.SelectPayment.Id;

            order.ArchivedShippingName = Data.SelectShipping.Name;
            order.ArchivedPaymentName = Data.SelectPayment.Name;

            order.OrderPickPoint = Data.SelectShipping.GetOrderPickPoint();

            order.PaymentDetails = Data.SelectPayment.GetDetails();

            order.AvailablePaymentCashOnDelivery = Data.SelectShipping.IsAvailablePaymentCashOnDelivery;
            order.AvailablePaymentPickPoint = Data.SelectShipping.IsAvailablePaymentPickPoint;

            ProcessCertificate(order);
            ProcessCoupon(order);

            var shippingPrice = Data.SelectShipping.FinalRate;
            var paymentPrice = Data.SelectPayment.Rate;

            Card bonusCard = null;
            if (BonusSystem.IsActive)
            {
                bonusCard = BonusSystemService.GetCard(Data.User.BonusCardId);

                if (Data.Bonus.UseIt && bonusCard != null && bonusCard.BonusesTotalAmount > 0)
                {
                    order.BonusCost = BonusSystemService.GetBonusCost(bonusCard, cart, shippingPrice, Data.Bonus.UseIt).BonusPrice;
                }

                if (Data.User.WantBonusCard && bonusCard == null && customer.RegistredUser)
                {
                    CreateBonusCard(customer);
                    bonusCard = BonusSystemService.GetCard(customer.Id);
                }
            }

            order.BonusCardNumber = bonusCard != null && !bonusCard.Blocked ? bonusCard.CardNumber : default(long?);

            order.ShippingCost = shippingPrice;
            var shippingTax = Data.SelectShipping.TaxId.HasValue ? TaxService.GetTax(Data.SelectShipping.TaxId.Value) : null;
            order.ShippingTaxType = shippingTax == null ? TaxType.None : shippingTax.TaxType;
            order.ShippingPaymentMethodType = Data.SelectShipping.PaymentMethodType;
            order.ShippingPaymentSubjectType = Data.SelectShipping.PaymentSubjectType;
            order.PaymentCost = paymentPrice;

            order.OrderID = OrderService.AddOrder(order, new OrderChangedBy(customer));

            OrderStatusService.ChangeOrderStatus(order.OrderID, OrderStatusService.DefaultOrderStatus, LocalizationService.GetResource("Core.OrderStatus.Created"), false);

            if (BonusSystem.IsActive && bonusCard != null && !bonusCard.Blocked)
            {
                BonusSystemService.MakeBonusPurchase(bonusCard.CardNumber, cart, shippingPrice, order);
            }

            PostProcessOrder(order);


            var lpUrl = new GetCrossSellLandingUrl(Data.LpUpId, order, isLanding).Execute();

            if (string.IsNullOrEmpty(lpUrl))
            {
                OrderService.SendOrderMail(order, cart.TotalDiscount, Data.Bonus.BonusPlus, Data.SelectShipping.ForMailTemplate(), Data.SelectPayment.Name);
            }
            else
            {
                LandingHelper.LandingRedirectUrl = lpUrl;
                DeferredMailService.Add(new DeferredMail(order.OrderID, DeferredMailType.Order));
            }

            TrialService.TrackEvent(
                order.OrderItems.Any(x => x.Name.Contains("SM-G900F"))
                    ? TrialEvents.BuyTheProduct
                    : TrialEvents.CheckoutOrder, string.Empty);

            return order;
        }

        private void ProcessCertificate(Order order)
        {
            var certificate = ShoppingCartService.CurrentShoppingCart.Certificate;

            if (certificate != null)
            {
                order.Certificate = new OrderCertificate()
                {
                    Code = certificate.CertificateCode,
                    Price = certificate.Sum
                };
            }
        }

        private void ProcessCoupon(Order order)
        {
            var coupon = ShoppingCartService.CurrentShoppingCart.Coupon;
            if (coupon != null && ShoppingCartService.CurrentShoppingCart.TotalPrice >= coupon.MinimalOrderPrice)
            {
                order.Coupon = new OrderCoupon()
                {
                    Code = coupon.Code,
                    Type = coupon.Type,
                    Value = coupon.GetRate()
                };
            }
        }

        private void CreateBonusCard(Customer customer)
        {
            try
            {
                customer.BonusCardNumber = BonusSystemService.AddCard(new Card { CardId = customer.Id });
                CustomerService.UpdateCustomer(customer);

                if (customer.BonusCardNumber != null)
                {
                    Data.User.BonusCardId = customer.Id;

                    if (HttpContext.Current != null)
                        HttpContext.Current.Session["BonusesForNewCard"] = BonusSystem.BonusesForNewCard;
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private void PostProcessOrder(Order order)
        {
            if (order.Sum == 0)
                OrderService.PayOrder(order.OrderID, true);
        }

        private float GetBonusDiscount(CheckoutData data, ShoppingCart cart)
        {
            if (data.Bonus != null && data.Bonus.UseIt)
            {
                var bonusCost = BonusSystemService.GetBonusCost(cart, 0, data.Bonus.UseIt);
                return bonusCost.BonusPrice;
            }

            return 0.0f;
        }
    }
}