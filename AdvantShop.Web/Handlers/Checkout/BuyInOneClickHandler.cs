﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.App.Landing.Domain;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Models.Checkout;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.Security;
using AdvantShop.Shipping;
using AdvantShop.Taxes;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Track;

namespace AdvantShop.Handlers.Checkout
{
    public class BuyInOneClickHandler
    {
        private readonly BuyOneInClickJsonModel _model;

        public BuyInOneClickHandler(BuyOneInClickJsonModel jsonModel)
        {
            _model = jsonModel;
        }

        public BuyOneClickResult Create()
        {
            var amount = _model.Amount;

            if (!IsValidModel())
            {
                return new BuyOneClickResult() { error = LocalizationService.GetResource("Checkout.BuyInOneClick.WrongData") };
            }

            var orderItems = new List<OrderItem>();
            float discountPercentOnTotalPrice = 0;
            float totalPrice = 0;
            float finalPrice = 0;
            float minimumOrderPrice = CustomerGroupService.GetMinimumOrderPrice();

            OrderCertificate orderCertificate = null;
            OrderCoupon orderCoupon = null;

            var customer = CustomerContext.CurrentCustomer;

            if (_model.Page == BuyInOneclickPage.Product || _model.Page == BuyInOneclickPage.LandingPage || _model.Page == BuyInOneclickPage.PreOrder)
            {
                Offer offer;

                if (_model.OfferId == null || _model.OfferId == 0)
                {
                    var p = ProductService.GetProduct(Convert.ToInt32(_model.ProductId));
                    if (p == null || p.Offers.Count == 0)
                        return new BuyOneClickResult() { error = LocalizationService.GetResource("Checkout.BuyInOneClick.Error") };

                    offer = p.Offers.First();
                }
                else
                {
                    offer = OfferService.GetOffer((int)_model.OfferId);
                }

                List<EvaluatedCustomOptions> listOptions = null;
                var selectedOptions = HttpUtility.UrlDecode(_model.AttributesXml);
                if (selectedOptions.IsNotEmpty())
                    listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions, offer.Product.Currency.Rate);


                var productPrice = PriceService.GetFinalPrice(offer, customer, selectedOptions); //(offer.RoundedPrice - (offer.RoundedPrice * offer.Product.Discount / 100)) * amount;
                totalPrice = PriceService.GetFinalPrice(productPrice * amount);
                discountPercentOnTotalPrice = OrderService.GetDiscount(totalPrice);
                finalPrice = PriceService.GetFinalPrice(totalPrice, new Discount(discountPercentOnTotalPrice, 0f));

                if (_model.Page != BuyInOneclickPage.LandingPage && _model.Page != BuyInOneclickPage.PreOrder)
                {
                    if (totalPrice < minimumOrderPrice)
                    {
                        return new BuyOneClickResult()
                        {
                            error =
                                string.Format(LocalizationService.GetResource("Cart.Error.MinimalOrderPrice"),
                                    PriceFormatService.FormatPrice(minimumOrderPrice),
                                    PriceFormatService.FormatPrice(minimumOrderPrice - totalPrice))
                        };
                    }

                    var errorAvailable = GetAvalible(offer, amount);

                    if (!string.IsNullOrEmpty(errorAvailable))
                        return new BuyOneClickResult() { error = errorAvailable };
                }


                var orderItem = new OrderItem
                {
                    ProductID = offer.ProductId,
                    Name = offer.Product.Name,
                    ArtNo = offer.ArtNo,
                    Price = productPrice,
                    Amount = amount,
                    SupplyPrice = offer.SupplyPrice,
                    SelectedOptions = listOptions,
                    Weight = offer.GetWeight(),
                    Color = offer.Color != null ? offer.Color.ColorName : string.Empty,
                    Size = offer.Size != null ? offer.Size.SizeName : string.Empty,
                    PhotoID = offer.Photo != null ? offer.Photo.PhotoId : (int?)null,
                    Width = offer.GetWidth(),
                    Length = offer.GetLength(),
                    Height = offer.GetHeight(),
                    PaymentMethodType = offer.Product.PaymentMethodType,
                    PaymentSubjectType = offer.Product.PaymentSubjectType
                };

                var tax = offer.Product.TaxId != null ? TaxService.GetTax(offer.Product.TaxId.Value) : null;
                if (tax != null)
                {
                    orderItem.TaxId = tax.TaxId;
                    orderItem.TaxName = tax.Name;
                    orderItem.TaxRate = tax.Rate;
                    orderItem.TaxShowInPrice = tax.ShowInPrice;
                    orderItem.TaxType = tax.TaxType;
                }

                orderItems = new List<OrderItem>() { orderItem };

                if (offer.Product.HasGifts())
                {
                    foreach (var gift in OfferService.GetProductGifts(offer.ProductId))
                    {
                        var item = new OrderItem
                        {
                            ProductID = gift.ProductId,
                            Name = gift.Product.Name,
                            ArtNo = gift.ArtNo,
                            Price = 0,
                            Amount = SettingsCheckout.MultiplyGiftsCount ? amount : 1,
                            Color = gift.Color != null ? gift.Color.ColorName : string.Empty,
                            Size = gift.Size != null ? gift.Size.SizeName : string.Empty,
                            PhotoID = gift.Photo != null ? gift.Photo.PhotoId : (int?)null,
                            Weight = gift.GetWeight(),
                            Width = gift.GetWidth(),
                            Length = gift.GetLength(),
                            Height = gift.GetHeight(),
                            PaymentMethodType = gift.Product.PaymentMethodType,
                            PaymentSubjectType = gift.Product.PaymentSubjectType
                        };

                        tax = offer.Product.TaxId != null ? TaxService.GetTax(gift.Product.TaxId.Value) : null;
                        if (tax != null)
                        {
                            item.TaxId = tax.TaxId;
                            item.TaxName = tax.Name;
                            item.TaxRate = tax.Rate;
                            item.TaxShowInPrice = tax.ShowInPrice;
                            item.TaxType = tax.TaxType;
                        }

                        orderItems.Add(item);
                    }
                }
            }
            else if (_model.Page == BuyInOneclickPage.Cart || _model.Page == BuyInOneclickPage.Checkout)
            {
                var shoppingCart = ShoppingCartService.CurrentShoppingCart;
                discountPercentOnTotalPrice = shoppingCart.DiscountPercentOnTotalPrice;
                totalPrice = shoppingCart.TotalPrice;
                finalPrice = shoppingCart.TotalPrice - shoppingCart.TotalDiscount;

                if (totalPrice < minimumOrderPrice)
                {
                    return new BuyOneClickResult()
                    {
                        error = string.Format(LocalizationService.GetResource("Cart.Error.MinimalOrderPrice"),
                            PriceFormatService.FormatPrice(minimumOrderPrice),
                            PriceFormatService.FormatPrice(minimumOrderPrice - totalPrice))
                    };
                }

                foreach (var item in shoppingCart)
                {
                    var errorAvailable = GetAvalible(item.Offer, item.Amount);

                    if (!string.IsNullOrEmpty(errorAvailable))
                        return new BuyOneClickResult() { error = errorAvailable };
                }

                orderItems.AddRange(shoppingCart.Select(item => (OrderItem)item));

                var certificate = shoppingCart.Certificate;
                var coupon = shoppingCart.Coupon;

                if (certificate != null)
                {
                    orderCertificate = new OrderCertificate()
                    {
                        Code = certificate.CertificateCode,
                        Price = certificate.Sum
                    };
                }
                if (coupon != null && shoppingCart.TotalPrice >= coupon.MinimalOrderPrice)
                {
                    orderCoupon = new OrderCoupon()
                    {
                        Code = coupon.Code,
                        Type = coupon.Type,
                        Value = coupon.Value
                    };
                }
            }

            if (string.IsNullOrEmpty(_model.LastName) && !string.IsNullOrEmpty(_model.Name))
            {
                var fio = _model.Name.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if (fio.Length == 2)
                {
                    _model.Name = fio[0];
                    _model.LastName = fio[1];
                }
            }

            var currency = CurrencyService.CurrentCurrency;

            BuyOneClickResult result = null;

            var crmEnable = (!SaasDataService.IsSaasEnabled ||
                             (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveCrm));

            try
            {
                if ((!SettingsCheckout.BuyInOneClickCreateOrder && crmEnable && _model.OrderType != OrderType.PreOrder) ||
                    (_model.OrderType == OrderType.PreOrder && SettingsCheckout.OutOfStockAction == eOutOfStockAction.Lead))
                {
                    var orderSource = OrderSourceService.GetOrderSource(_model.OrderType);

                    var lead = CreateLead(customer, currency, orderItems, orderCertificate, orderCoupon,
                                            totalPrice, discountPercentOnTotalPrice, orderSource.Id);

                    var url = LandingHelper.LandingRedirectUrl ?? UrlService.GetUrl("checkout/buyinoneclicksuccess/" + lead.Id);

                    result = new BuyOneClickResult()
                    {
                        orderId = lead.Id,
                        url = url,
                        doGo = true,
                        redirectToUrl = !string.IsNullOrEmpty(LandingHelper.LandingRedirectUrl)
                    };
                }
                else
                {
                    var order = CreateOrder(customer, currency, orderItems, orderCertificate, orderCoupon, orderItems.Sum(x => x.Price * x.Amount), discountPercentOnTotalPrice, _model.OrderType, finalPrice);

                    var url = LandingHelper.LandingRedirectUrl ?? UrlService.GetUrl("checkout/success/" + order.Code);

                    result = new BuyOneClickResult()
                    {
                        orderId = order.OrderID,
                        orderNumber = order.Number,
                        code = order.Code.ToString(),
                        url = url,
                        doGo = true,
                        redirectToUrl = !string.IsNullOrEmpty(LandingHelper.LandingRedirectUrl)
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            if (result != null && (_model.Page == BuyInOneclickPage.Cart || _model.Page == BuyInOneclickPage.Checkout))
            {
                ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart, CustomerContext.CustomerId);
            }

            return result;
        }

        #region Help methods

        private bool IsValidModel()
        {
            var valid = true;
            valid &= _model.Page != BuyInOneclickPage.None;

            if (SettingsCheckout.IsShowBuyInOneClickName &&
                SettingsCheckout.IsRequiredBuyInOneClickName)
            {
                valid &= _model.Name.IsNotEmpty();
            }

            if (SettingsCheckout.IsShowBuyInOneClickEmail &&
                SettingsCheckout.IsRequiredBuyInOneClickEmail)
            {
                valid &= _model.Email.IsNotEmpty();
            }

            if (SettingsCheckout.IsShowBuyInOneClickPhone &&
                SettingsCheckout.IsRequiredBuyInOneClickPhone)
            {
                valid &= _model.Phone.IsNotEmpty();
            }

            return valid;
        }

        private string GetAvalible(Offer offer, float amount)
        {
            if (!offer.Product.Enabled || !offer.Product.CategoryEnabled)
                return LocalizationService.GetResource("Cart.Error.NotAvailable") + " 0 " + offer.Product.Unit;

            if ((SettingsCheckout.AmountLimitation) && (amount > offer.Amount))
                return LocalizationService.GetResource("Cart.Error.NotAvailable") + " " + offer.Amount + " " + offer.Product.Unit;

            if (amount > offer.Product.MaxAmount)
                return LocalizationService.GetResource("Cart.Error.MaximumOrder") + " " + offer.Product.MaxAmount + " " + offer.Product.Unit;

            if (amount < offer.Product.MinAmount)
                return LocalizationService.GetResource("Cart.Error.MinimumOrder") + " " + offer.Product.MinAmount + " " + offer.Product.Unit;

            return string.Empty;
        }

        private Order CreateOrder(Customer customer, Currency currency, List<OrderItem> orderItems,
                                    OrderCertificate orderCertificate, OrderCoupon orderCoupon, float totalPrice,
                                    float discountPercentOnTotalPrice, OrderType orderType, float finalPrice)
        {
            if (!customer.RegistredUser)
            {
                var newcustomer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    Id = CustomerContext.CustomerId,
                    Password = StringHelper.GeneratePassword(8),
                    EMail = _model.Email.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Email) : "",
                    Phone = _model.Phone.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Phone) : "",
                    StandardPhone = StringHelper.ConvertToStandardPhone(_model.Phone),

                    FirstName = _model.Name.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Name) : "",
                    LastName = _model.LastName.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.LastName) : "",
                    CustomerRole = Role.User,
                };

                newcustomer.Id = CustomerService.InsertNewCustomer(newcustomer);
                if (newcustomer.Id != Guid.Empty)
                {
                    AuthorizeService.SignIn(newcustomer.EMail, newcustomer.Password, false, true);
                    customer = newcustomer;
                }
            }

            var orderSource = OrderSourceService.GetOrderSource(_model.OrderType);

            var order = new Order
            {
                CustomerComment = HttpUtility.HtmlEncode(_model.Comment),
                OrderDate = DateTime.Now,
                OrderCurrency = currency,
                OrderCustomer = new OrderCustomer
                {
                    CustomerID = customer.Id,
                    Email = _model.Email.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Email) : customer.EMail,
                    FirstName = _model.Name.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Name) : "",
                    LastName = _model.LastName.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.LastName) : "",
                    Phone = _model.Phone.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Phone) : "",
                    StandardPhone = StringHelper.ConvertToStandardPhone(_model.Phone),
                    CustomerIP = HttpContext.Current.Request.UserHostAddress,

                    Country = IpZoneContext.CurrentZone.CountryName,
                    Region = IpZoneContext.CurrentZone.Region,
                    District = IpZoneContext.CurrentZone.District,
                    City = IpZoneContext.CurrentZone.City,
                },

                OrderStatusId = OrderStatusService.DefaultOrderStatus,
                AffiliateID = 0,
                GroupName = customer.CustomerGroup.GroupName,
                GroupDiscount = customer.CustomerGroup.GroupDiscount,
                OrderItems = orderItems,
                OrderDiscount = discountPercentOnTotalPrice,
                Certificate = orderCertificate,
                Coupon = orderCoupon,
                OrderSourceId = orderSource.Id
            };

            var shipping = ShippingMethodService.GetShippingMethod(SettingsCheckout.BuyInOneClickDefaultShippingMethod);

            if (shipping != null)
            {

                var currentZone = IpZoneContext.CurrentZone;
                var mainCountry = CountryService.GetCountry(SettingsMain.SellerCountryId);
                var preOrder = new PreOrder()
                {
                    CountryDest = currentZone.CountryName ?? (mainCountry != null ? mainCountry.Name : string.Empty),
                    RegionDest = currentZone.Region,
                    DistrictDest = currentZone.District,
                    CityDest = currentZone.City ?? SettingsMain.City,
                    Currency = CurrencyService.CurrentCurrency,
                    ZipDest = currentZone.Zip
                };
                var items = order.OrderItems.Select(shpItem => new PreOrderItem(shpItem)).ToList();

                var shippingManager = new ShippingManager(preOrder, items, finalPrice);
                var shippingRate = shippingManager.GetOptions().FirstOrDefault(sh => sh.MethodId == shipping.ShippingMethodId);
                if (shippingRate != null)
                {
                    order.ShippingCost = shippingRate.FinalRate;
                }

                order.ArchivedShippingName = shipping.Name;
                var shippingTax = shipping.TaxId.HasValue ? TaxService.GetTax(shipping.TaxId.Value) : null;
                order.ShippingTaxType = shippingTax == null ? TaxType.None : shippingTax.TaxType;
                order.ShippingMethodId = shipping.ShippingMethodId;
            }


            var payment = Payment.PaymentService.GetPaymentMethod(SettingsCheckout.BuyInOneClickDefaultPaymentMethod);
            if (payment != null)
            {
                order.PaymentMethodId = payment.PaymentMethodId;
                order.ArchivedPaymentName = payment.Name;
                order.PaymentCost = payment.GetExtracharge(totalPrice);
            }

            Card bonusCard = null;
            if (BonusSystem.IsActive)
            {
                bonusCard = BonusSystemService.GetCard(customer.Id);
                if (bonusCard != null && !bonusCard.Blocked)
                    order.BonusCardNumber = bonusCard.CardNumber;
            }

            var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);

            order.OrderID = OrderService.AddOrder(order, changedBy);
            OrderStatusService.ChangeOrderStatus(order.OrderID, OrderStatusService.DefaultOrderStatus, LocalizationService.GetResource("Core.OrderStatus.Created"));

            if (BonusSystem.IsActive && bonusCard != null && !bonusCard.Blocked)
            {
                BonusSystemService.MakeBonusPurchase(bonusCard.CardNumber, OrderService.GetOrder(order.OrderID));
            }

            if (order.OrderID != 0)
            {
                var lpUrl = new GetCrossSellLandingUrl(order).Execute();

                if (string.IsNullOrEmpty(lpUrl))
                {
                    var orderTable = OrderService.GenerateOrderItemsHtml(order.OrderItems, currency, totalPrice,
                        discountPercentOnTotalPrice, 0, orderCoupon, orderCertificate,
                        order.TotalDiscount, 0, 0, order.TaxCost, 0, 0);

                    var mail =
                        _model.OrderType != OrderType.PreOrder
                            ? new BuyInOneClickMailTemplate(order, orderTable)
                            : new PreorderMailTemplate(order, orderTable);

                    if (_model.Email.IsNotEmpty())
                        MailService.SendMailNow(CustomerContext.CustomerId, _model.Email, mail);

                    MailService.SendMailNow(SettingsMail.EmailForOrders, mail, replyTo: _model.Email);
                }
                else
                {
                    LandingHelper.LandingRedirectUrl = lpUrl;
                    DeferredMailService.Add(new DeferredMail(order.OrderID, DeferredMailType.Order));
                }


                if (orderCoupon != null)
                {
                    var coupon = CouponService.GetCouponByCode(orderCoupon.Code);
                    if (coupon != null)
                    {
                        coupon.ActualUses += 1;
                        CouponService.UpdateCoupon(coupon);
                        CouponService.DeleteCustomerCoupon(coupon.CouponID);
                    }
                }

                if (orderCertificate != null)
                {
                    var certificate = GiftCertificateService.GetCertificateByCode(orderCertificate.Code);
                    if (certificate != null)
                    {
                        certificate.ApplyOrderNumber = order.Number;
                        certificate.Used = true;
                        certificate.Enable = true;

                        GiftCertificateService.DeleteCustomerCertificate(certificate.CertificateId);
                        GiftCertificateService.UpdateCertificateById(certificate);
                    }
                }
            }

            TrackService.TrackEvent(SettingsDesign.IsMobileTemplate
                ? ETrackEvent.Core_Orders_OrderCreated_Mobile
                : ETrackEvent.Core_Orders_OrderCreated_Desktop);
            TrackService.TrackEvent(ETrackEvent.Trial_AddOrderFromClientSide);

            return order;
        }

        private Lead CreateLead(Customer customer, Currency currency, List<OrderItem> orderItems,
                                OrderCertificate orderCertificate, OrderCoupon orderCoupon, float totalPrice,
                                float discountPercentOnTotalPrice, int orderSourceId)
        {
            var lead = new Lead()
            {
                Email = _model.Email.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Email) : customer.EMail,
                FirstName = _model.Name.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Name) : "",
                LastName = _model.LastName.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.LastName) : "",
                Phone = _model.Phone.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Phone) : "",
                LeadItems = new List<LeadItem>(),
                LeadCurrency = currency,

                Country = IpZoneContext.CurrentZone.CountryName,
                Region = IpZoneContext.CurrentZone.Region,
                District = IpZoneContext.CurrentZone.District,
                City = IpZoneContext.CurrentZone.City,
                Zip = IpZoneContext.CurrentZone.Zip,

                OrderSourceId = orderSourceId,
                Comment = _model.Comment.IsNotEmpty()
                    ? HttpUtility.HtmlEncode(_model.Comment).Replace("\n", "<br/>")
                    : "",

                SalesFunnelId = SettingsCrm.DefaultBuyInOneClickSalesFunnelId,
                Discount = discountPercentOnTotalPrice,

                CustomerId = customer.RegistredUser ? customer.Id : default(Guid?),
                Customer =
                    !customer.RegistredUser
                        ? new Customer(CustomerGroupService.DefaultCustomerGroup)
                        {
                            Id = CustomerContext.CustomerId,
                            Password = StringHelper.GeneratePassword(8),
                            FirstName = _model.Name.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Name) : "",
                            LastName = _model.LastName.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.LastName) : "",
                            Phone = _model.Phone.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Phone) : "",
                            StandardPhone = StringHelper.ConvertToStandardPhone(_model.Phone),
                            EMail = _model.Email.IsNotEmpty() ? HttpUtility.HtmlEncode(_model.Email) : "",
                            CustomerRole = Role.User,
                        }
                        : null
            };
            
            foreach (var orderItem in orderItems)
                lead.LeadItems.Add((LeadItem)orderItem);

            lead.Sum = lead.LeadItems.Sum(x => x.Price * x.Amount) - lead.GetTotalDiscount(lead.LeadCurrency);

            LeadService.AddLead(lead);

            if (!customer.RegistredUser && lead.Customer != null && lead.Customer.Id != Guid.Empty)
            {
                AuthorizeService.SignIn(lead.Customer.EMail, lead.Customer.Password, false, true);
            }

            var lpUrl = new GetCrossSellLandingUrl(lead).Execute();
            if (!string.IsNullOrEmpty(lpUrl))
                LandingHelper.LandingRedirectUrl = lpUrl;


            TrackService.TrackEvent(SettingsDesign.IsMobileTemplate
                ? ETrackEvent.Core_Leads_LeadCreated_Mobile
                : ETrackEvent.Core_Leads_LeadCreated_Desktop);

            return lead;
        }

        #endregion
    }
}