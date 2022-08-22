using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Extensions;
using AdvantShop.Handlers.Checkout;
using AdvantShop.Helpers;
using AdvantShop.Models.Checkout;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Track;
using AdvantShop.Trial;
using AdvantShop.ViewModel.Checkout;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using BotDetect.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Controllers
{
    public partial class CheckoutController : BaseClientController
    {
        #region Checkout

        // GET: /checkout
        public ActionResult Index()
        {
            var cart = ShoppingCartService.CurrentShoppingCart;
            if (!cart.CanOrder)
                return RedirectToRoute("Cart");

            var showConfirmButtons =
                AttachedModules.GetModules<IShoppingCart>()
                    .Select(t => (IShoppingCart)Activator.CreateInstance(t))
                    .Aggregate(true, (current, module) => current & module.ShowConfirmButtons);

            if (!showConfirmButtons)
                return RedirectToRoute("Cart");

            if (MobileHelper.IsMobileEnabled() && !SettingsMobile.IsFullCheckout)
                return Redirect("mobile/checkoutmobile/index");

            var model = new GetCheckoutPage().Execute(cart);

            SetNgController(NgControllers.NgControllersTypes.CheckOutCtrl);
            SetMetaInformation(T("Checkout.Index.CheckoutTitle"));
            SetNoFollowNoIndex();

            WriteLog("", Url.AbsoluteRouteUrl("Checkout"), ePageType.order);

            return View(model);
        }

        public ActionResult Lp(int? lpId, int? lpUpId, CheckoutLpMode? mode, string products)
        {
            if (!string.IsNullOrWhiteSpace(products))
                ShoppingCartService.AddShoppingCartItems(products);

            var model = new GetCheckoutPage().Execute(ShoppingCartService.CurrentShoppingCart, lpId, lpUpId, mode);

            model.IsLanding = true;

            SetNgController(NgControllers.NgControllersTypes.CheckOutCtrl);
            SetMetaInformation(T("Checkout.Index.CheckoutTitle"));

            WriteLog("", Url.AbsoluteRouteUrl("Checkout"), ePageType.order);

            return View("Index", "_LayoutEmpty", model);
        }

        // POST: Confirm order
        [HttpPost, ValidateAntiForgeryToken]
        [CaptchaValidation("CaptchaCode", "CaptchaSource")]
        public ActionResult IndexPost(CheckoutPostModel model)
        {
            var cart = ShoppingCartService.CurrentShoppingCart;
            var currentCustomer = CustomerContext.CurrentCustomer;

            if (!cart.CanOrder && !model.IsLanding)
                return RedirectToRoute("Cart");

            if (SettingsMain.EnableCaptchaInCheckout)
            {
                if (!ModelState.IsValidField("CaptchaCode"))
                {
                    ShowMessage(NotifyType.Error, T("Captcha.Wrong"));
                    return RedirectToReferrerOnPost("Index");
                }
                MvcCaptcha.ResetCaptcha("CaptchaSource");
            }

            var orderCode = "";
            var current = MyCheckout.Factory(currentCustomer.Id);

            if (ShoppingCartService.CurrentShoppingCart.GetHashCode() != current.Data.ShopCartHash)
                return RedirectToReferrerOnPost("Index");

            if (current.Data.User.WantRegist && SettingsCheckout.IsShowEmail)
            {
                if (!ValidationHelper.IsValidEmail(current.Data.User.Email))
                {
                    ShowMessage(NotifyType.Error, T("User.Registration.ErrorCustomerEmailIsWrong"));
                    return RedirectToReferrerOnPost("Index");
                }

                if (!currentCustomer.RegistredUser && CustomerService.GetCustomerByEmail(current.Data.User.Email) != null)
                {
                    ShowMessage(NotifyType.Error, string.Format(LocalizationService.GetResource("User.Registration.ErrorCustomerExist"), "forgotpassword"));
                    return RedirectToReferrerOnPost("Index");
                }
            }

            var valid = current.Data.SelectShipping.Validate();
            if (!valid.IsValid)
            {
                ShowMessage(NotifyType.Error, valid.ErrorMessage);
                return RedirectToReferrerOnPost("Index");
            }

            if (cart.Coupon != null && !currentCustomer.RegistredUser && current.Data.User.Email.IsNotEmpty())
            {
                var customerByEmail = CustomerService.GetCustomerByEmail(current.Data.User.Email);
                if (customerByEmail != null && !CouponService.CanApplyCustomerCoupon(cart.Coupon, customerByEmail.Id))
                {
                    CouponService.DeleteCustomerCoupon(cart.Coupon.CouponID, currentCustomer.Id);
                    ShowMessage(NotifyType.Error, T("Checkout.CheckoutCart.CouponNotApplied"));
                    return RedirectToReferrerOnPost("Index");
                }
            }

            Order order = null;

            try
            {
                // игнорируем базовый индекс населенного пункта из ipzone, если индекс не выводится в клиентке
                if (!SettingsCheckout.IsShowZip && current.Data.Contact.Zip.IsNotEmpty())
                    current.Data.Contact.Zip = null;
                
                var allow = ModulesExecuter.CheckInfo(System.Web.HttpContext.Current, ECheckType.Order, current.Data.User.Email, current.Data.User.FirstName, message: current.Data.CustomerComment, phone: current.Data.User.Phone);
                if (!allow)
                {
                    ShowMessage(NotifyType.Error, T("Common.SpamCheckFailed"));
                    return RedirectToAction("Index");
                }

                order = current.ProcessOrder(model.CustomData, model.OrderType, model.IsLanding);
                orderCode = order.Code.ToString();
                TempData["orderid"] = order.OrderID.ToString();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                ShowMessage(NotifyType.Error, "Error");
                return RedirectToAction("Index");
            }

            this.ToTempData();

            if (!string.IsNullOrEmpty(LandingHelper.LandingRedirectUrl))
                return Redirect(LandingHelper.LandingRedirectUrl);

            if (current.Data.LpId != null)
                return RedirectToRoute("CheckoutSuccess", new { code = orderCode, mode = "lp" });

            TrackService.TrackEvent(SettingsDesign.IsMobileTemplate ? ETrackEvent.Core_Orders_OrderCreated_Mobile : ETrackEvent.Core_Orders_OrderCreated_Desktop);
            TrackService.TrackEvent(ETrackEvent.Trial_AddOrderFromClientSide);

            return RedirectToRoute("CheckoutSuccess", new { code = orderCode });
        }

        // GET: /checkout/success
        public ActionResult Success(string code, string mode, string lid)
        {
            this.ToContext();

            if (!string.IsNullOrEmpty(lid))
                return RedirectToAction("BuyInOneClickSuccess", new { id = lid, area = "" });

            if (string.IsNullOrWhiteSpace(code))
                return Error404();

            SetNgController(NgControllers.NgControllersTypes.CheckOutSuccessCtrl);
            SetMetaInformation(T("Checkout.Index.CheckoutTitle"));

            var isLanding = mode == "lp";
            var tempOrderId = TempData["orderid"] != null ? TempData["orderid"].ToString() : string.Empty;

            var order = OrderService.GetOrderByCode(code);
            if (order == null || (order.OrderID.ToString() != tempOrderId && mode != "lp"))
                return View("OrderComplete", new CheckoutSuccess() { IsEmptyLayout = isLanding, IsLanding = isLanding });

            var model = new CheckoutSuccessHandler().Get(order);

            model.IsEmptyLayout = model.IsLanding = isLanding;

            var tagManager = GoogleTagManagerContext.Current;
            tagManager.CreateTransaction(order);

            TrialService.TrackEvent(TrialEvents.CheckoutOrder, order.OrderID.ToString());

            WriteLog("", Url.AbsoluteRouteUrl("CheckoutSuccess"), ePageType.purchase);

            return View("Success", model.IsEmptyLayout ? "_LayoutEmpty" : "_Layout", model);
        }

        public JsonResult GetPaymentInfo(int orderid)
        {
            var order = OrderService.GetOrder(orderid);
            var confirmed = !SettingsCheckout.ManagerConfirmed || order.ManagerConfirmed;
            var processType = order?.PaymentMethod?.ProcessType ?? ProcessType.None;

            var willProceedToPayment = SettingsCheckout.ProceedToPayment && confirmed && processType != ProcessType.None;
            
            // костыль для счетов
            if (willProceedToPayment
               && processType == ProcessType.Javascript
               && (order?.PaymentMethod?.PaymentKey == AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(SberBank))
                   || order?.PaymentMethod?.PaymentKey == AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Bill))
                   || order?.PaymentMethod?.PaymentKey == AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(BillUa))
                   || order?.PaymentMethod?.PaymentKey == AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(BillBy))
                   || order?.PaymentMethod?.PaymentKey == AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(BillKz))
                   || order?.PaymentMethod?.PaymentKey == AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Check))))
            {
                willProceedToPayment = false;
            }
            
            return Json(new
            {
                proceedToPayment = SettingsCheckout.ProceedToPayment,
                willProceedToPayment = willProceedToPayment
            });
        }

        [ChildActionOnly]
        public ActionResult CheckoutUser(bool? isLanding)
        {
            return PartialView(new CheckoutUserHandler(isLanding).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetCheckoutUser()
        {
            return JsonOk(new CheckoutUserHandler(false).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckoutUserPost(CheckoutUser customer)
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);
            current.Data.User.Email = 
                !string.IsNullOrEmpty(customer.Email) 
                    ? HttpUtility.HtmlEncode(customer.Email) 
                    : (CustomerContext.CurrentCustomer.RegistredUser ? CustomerContext.CurrentCustomer.EMail : null);
            current.Data.User.FirstName = HttpUtility.HtmlEncode(customer.FirstName);
            current.Data.User.LastName = HttpUtility.HtmlEncode(customer.LastName);
            current.Data.User.Patronymic = HttpUtility.HtmlEncode(customer.Patronymic);
            current.Data.User.Phone = HttpUtility.HtmlEncode(customer.Phone);
            current.Data.User.WantRegist = customer.WantRegist;
            current.Data.User.Password = customer.Password;
            current.Data.User.CustomerFields = customer.CustomerFields;
            current.Data.User.BirthDay = customer.BirthDay;
            current.Update();

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveWantBonusCard(bool wantBonusCard)
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);
            current.Data.User.WantBonusCard = wantBonusCard;
            current.Update();

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckoutProcessContactPost(CheckoutAddressQueryModel address)
        {
            if (address == null || address.City.IsNullOrEmpty())
                return JsonError();

            IpZone zone;
            if ((address.ByCity || address.Region.IsNullOrEmpty()) && (zone = IpZoneService.GetZoneByCity(address.City, null)) != null)
            {
                address.District = zone.District;
                address.Region = zone.Region;
                address.Country = zone.CountryName;
                address.Zip = SettingsCheckout.IsShowZip ? zone.Zip : null; // в zone только базовый индекс населенного пункта, если индекс не выводится в клиентке, не проставлять значение из zone
            }
            ModulesExecuter.ProcessCheckoutAddress(address);

            return JsonOk(address);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckoutContactPost(CheckoutAddress address)
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);

            if (current.Data.Contact == address)
                return Json(true);

            current.Data.Contact = address;

            /* Перенос в CheckoutShippingJson
            if (!current.Data.HideShippig)
            {
                var options = current.AvailableShippingOptions();

                if (current.Data.SelectShipping == null || !options.Any(x => x.Id == current.Data.SelectShipping.Id))
                    current.Data.SelectShipping = null;
            }*/

            current.Update();

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckoutShippingJson(List<PreOrderItem> preorderList = null)
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);

            var options = new List<BaseShippingOption>();

            if (!current.Data.HideShippig)
            {
                options = current.AvailableShippingOptions(preorderList);

                if (current.Data.SelectShipping == null || !options.Any(x => x.Id == current.Data.SelectShipping.Id))
                    current.Data.SelectShipping = null;

                current.UpdateSelectShipping(preorderList, current.Data.SelectShipping, options);
            }
            else
            {
                options.Add(current.Data.SelectShipping);
            }

            return Json(new { selectShipping = current.Data.SelectShipping, option = options });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckoutShippingPost(BaseShippingOption shipping, List<PreOrderItem> preorderList = null)
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);

            if (!current.Data.HideShippig)
                current.UpdateSelectShipping(preorderList, shipping);

            return Json(new { selectShipping = current.Data.SelectShipping });
        }

        // Shipping lazy load
        public JsonResult GetShippingData(int methodId, Dictionary<string, object> data)
        {
            var shippingMethod = ShippingMethodService.GetShippingMethod(methodId);
            if (shippingMethod == null)
                return Json(null);

            var type = ReflectionExt.GetTypeByAttributeValue<Core.Common.Attributes.ShippingKeyAttribute>(typeof(BaseShipping), atr => atr.Value, shippingMethod.ShippingType);
            if (!type.GetInterfaces().Contains(typeof(IShippingLazyData)))
                return Json(null);

            var shipping = (BaseShipping)Activator.CreateInstance(type, shippingMethod, null, null);
            return Json(((IShippingLazyData)shipping).GetLazyData(data));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckoutPaymentJson(List<PreOrderItem> preorderList = null)
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);
            var options = current.AvailablePaymentOptions(preorderList);

            var cookiePayment = CommonHelper.GetCookie("payment");
            if (cookiePayment != null && !string.IsNullOrEmpty(cookiePayment.Value))
            {
                var paymentId = cookiePayment.Value.TryParseInt();
                current.Data.SelectPayment = options.FirstOrDefault(x => x.Id == paymentId);
                CommonHelper.DeleteCookie("payment");
            }

            current.UpdateSelectPayment(preorderList, current.Data.SelectPayment, options);
            return Json(new { selectPayment = current.Data.SelectPayment, option = options });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckoutPaymentPost(BasePaymentOption payment, List<PreOrderItem> preorderList = null)
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);
            var temp = current.UpdateSelectPayment(preorderList, payment);
            return Json(temp);
        }

        [ChildActionOnly]
        public ActionResult CheckoutShippingAddress()
        {
            var customer = CustomerContext.CurrentCustomer;

            var hasAddresses = customer.Contacts.Count > 0 && !string.IsNullOrEmpty(customer.Contacts[0].Street);
            var hasCustomShippingFields = SettingsCheckout.IsShowCustomShippingField1 ||
                                          SettingsCheckout.IsShowCustomShippingField2 ||
                                          SettingsCheckout.IsShowCustomShippingField3;

            if (hasAddresses && !hasCustomShippingFields)
                return new EmptyResult();

            if (!hasCustomShippingFields &&
                (!SettingsCheckout.IsShowAddress || hasAddresses) &&
                (!SettingsCheckout.IsShowZip || SettingsCheckout.ZipDisplayPlace))
                return new EmptyResult();

            var current = MyCheckout.Factory(CustomerContext.CustomerId);
            var model = new CheckoutShippingAddressViewModel()
            {
                AddressContact = current.Data.Contact,
                HasAddresses = hasAddresses,
                HasCustomShippingFields = hasCustomShippingFields
            };

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult CheckoutBonus()
        {
            var model = new CheckoutBonusHandler().Execute();
            if (model == null)
                return new EmptyResult();

            if (!model.HasCard && !CustomerContext.CurrentCustomer.RegistredUser)
                return new EmptyResult();

            return PartialView(model);
        }

        public JsonResult CheckoutBonusAutorizePost(long cardNumber)
        {
            if (!BonusSystem.IsActive)
                return Json(false);

            var card = BonusSystemService.GetCard(cardNumber);
            if (card != null)
            {
                var current = MyCheckout.Factory(CustomerContext.CustomerId);
                current.Data.User.BonusCardId = card.CardId;
                current.Update();
            }
            return Json(true);
        }

        public JsonResult CheckoutBonusApplyPost(bool isApply)
        {
            if (!BonusSystem.IsActive)
                return Json(new { result = false });

            if (isApply && BonusSystem.ForbidOnCoupon && ShoppingCartService.CurrentShoppingCart.Coupon != null)
            {
                return Json(new { result = false, msg = T("Checkout.Checkout.BonusNotAppliedWithCoupon") });// "Нельзя применить бонусы при использование купона" });
            }

            var current = MyCheckout.Factory(CustomerContext.CustomerId);
            current.Data.Bonus = current.Data.Bonus ?? new CheckoutBonus();
            current.Data.Bonus.UseIt = isApply;
            current.Update();
            return Json(new { result = true });
        }

        [ChildActionOnly]
        public ActionResult CheckoutCoupon()
        {
            var show = SettingsCheckout.DisplayPromoTextbox &&
                (CustomerContext.CurrentCustomer.CustomerGroupId == CustomerGroupService.DefaultCustomerGroup || SettingsCheckout.EnableGiftCertificateService);

            if (!show)
                return new EmptyResult();

            return PartialView();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult CheckoutCouponApplied()
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);

            //if (current.Data.Bonus.UseIt && BonusSystem.ForbidOnCoupon)
            //{
            //    return Json("Нельзя применить купон при использование бонусов");
            //}

            current.Data.ShopCartHash = ShoppingCartService.CurrentShoppingCart.GetHashCode();
            current.Update();

            return Json(true);
        }

        public JsonResult CheckoutCartJson()
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);

            var shippingPrice = current.Data.SelectShipping != null ? current.Data.SelectShipping.FinalRate : 0;
            var paymentCost = current.Data.SelectPayment != null ? current.Data.SelectPayment.Rate : 0;
            var currency = CurrencyService.CurrentCurrency;

            var model = new CheckoutCartHandler().Get(current.Data, ShoppingCartService.CurrentShoppingCart, shippingPrice, paymentCost, currency);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CommentPost(string message)
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);
            current.Data.CustomerComment = HttpUtility.HtmlEncode(message);
            current.Update();
            return Json(true);
        }

        #endregion Checkout

        #region Buy in one click

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckoutBuyInOneClick(BuyOneInClickJsonModel model)
        {
            if (SettingsMain.EnableCaptchaInBuyInOneClick &&
                !MvcCaptcha.Validate("CaptchaSourceBuyInOneClick", model.CaptchaCode, model.CaptchaSource))
            {
                return Json(new BuyOneClickResult { error = T("Checkout.CheckoutBuyInOneClick.CapchaWrong") });
            }

            var allow = ModulesExecuter.CheckInfo(System.Web.HttpContext.Current, ECheckType.Order, model.Email, model.Name, message: model.Comment, phone: model.Phone);
            if (!allow)
                return Json(new BuyOneClickResult { error = T("Common.SpamCheckFailed") });

            var returnModel = new BuyInOneClickHandler(model).Create();
            if (returnModel != null)
                TempData["orderid"] = returnModel.orderId;

            MvcCaptcha.ResetCaptcha("CaptchaSourceBuyInOneClick");

            return Json(returnModel);
        }

        public JsonResult CheckoutBuyInOneClickFields()
        {
            var suggestionsModule = AttachedModules.GetModules<ISuggestions>().Select(x => (ISuggestions)Activator.CreateInstance(x)).FirstOrDefault();

            var obj = new
            {
                SettingsCheckout.IsShowBuyInOneClickName,
                SettingsCheckout.IsShowBuyInOneClickEmail,
                SettingsCheckout.IsShowBuyInOneClickPhone,
                SettingsCheckout.IsShowBuyInOneClickComment,
                SettingsCheckout.BuyInOneClickName,
                SettingsCheckout.BuyInOneClickEmail,
                SettingsCheckout.BuyInOneClickPhone,
                SettingsCheckout.BuyInOneClickComment,
                SettingsCheckout.IsRequiredBuyInOneClickName,
                SettingsCheckout.IsRequiredBuyInOneClickEmail,
                SettingsCheckout.IsRequiredBuyInOneClickPhone,
                SettingsCheckout.IsRequiredBuyInOneClickComment,
                BuyInOneClickFinalText = T("Checkout.BuyInOneClickFinalText"),
                SettingsCheckout.BuyInOneClickFirstText,
                SettingsCheckout.BuyInOneClickButtonText,
                SettingsCheckout.BuyInOneClickLinkText,
                SettingsCheckout.IsShowUserAgreementText,
                SettingsCheckout.UserAgreementText,
                SettingsCheckout.AgreementDefaultChecked,
                UseFullNameSuggestions = suggestionsModule != null && suggestionsModule.SuggestFullNameInClient,
                SuggestFullNameUrl = suggestionsModule != null ? suggestionsModule.SuggestFullNameUrl : null,
                SettingsMain.EnableCaptchaInBuyInOneClick,
                CaptchaCode = T("Captcha.Code")
            };

            return Json(obj);
        }

        public JsonResult CheckoutBuyInOneClickCustomer()
        {
            var customer = CustomerContext.CurrentCustomer;

            return Json(customer != null
                ? new
                {
                    name = string.Join(" ", new[] { customer.FirstName, customer.LastName }.Where(x => x.IsNotEmpty())),
                    email = customer.EMail,
                    phone = customer.Phone
                }
                : new
                {
                    name = "",
                    email = "",
                    phone = ""
                });
        }

        public ActionResult BuyInOneClickSuccess(int id)
        {
            var lead = LeadService.GetLead(id);
            if (lead == null)
                return Error404();

            var model = new BuyInOneClickSuccess() { Lead = lead };

            SetMetaInformation(T("Checkout.BuyInOneClickSuccess.CheckoutTitle"));

            return View(model);
        }

        #endregion Buy in one click

        #region Print Order

        public ActionResult PrintOrder(PrintOrderModel printOrder)
        {
            if (string.IsNullOrWhiteSpace(printOrder.Code))
                return Error404();

            var order = OrderService.GetOrderByCode(printOrder.Code);
            if (order == null)
                return Error404();

            var model = new PrintOrderHandler(order, printOrder).Execute();

            SettingsDesign.IsMobileTemplate = false;

            return View(model);
        }

        #endregion Print Order

        #region Billing page

        public ActionResult Billing(string code, string number, string hash, string payCode)
        {
            if (((String.IsNullOrWhiteSpace(code) && String.IsNullOrWhiteSpace(number)) || String.IsNullOrWhiteSpace(hash)) &&
                String.IsNullOrWhiteSpace(payCode))
                return Error404();

            Order order = null;
            if (payCode.IsNotEmpty())
                order = OrderService.GetOrderByPayCode(payCode);
            else
            {
                if (code.IsNotEmpty())
                    order = OrderService.GetOrderByCode(code);
                if (number.IsNotEmpty() && order == null) //для старых ссылок на оплату, которы по номеру открывались
                    order = OrderService.GetOrderByNumber(number);
                if (order != null && hash != OrderService.GetBillingLinkHash(order))
                    return Error404();
            }

            if (order == null || order.IsDraft)
                return Error404();

            var model = new BillingViewModel
            {
                Order = order,
                IsMobile = SettingsDesign.IsMobileTemplate,
                Header =
                    T("Checkout.Billing.BillingTitle") + " " + order.Number +
                    (order.Payed
                        ? " - " + T("Core.Orders.Order.OrderPaied")
                        : order.OrderStatus.IsCanceled ? T("Core.Crm.LeadStatus.NotClosedDeal") : "")
            };

            SetNgController(NgControllers.NgControllersTypes.BillingCtrl);
            SetMetaInformation(model.Header);

            SettingsDesign.IsMobileTemplate = false;

            return View(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult BillingPaymentJson(int orderId)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return Json("error");

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);

            BaseShippingOption shipping = null;

            if (shippingMethod != null)
            {
                shipping = new BaseShippingOption(shippingMethod, 0f);
            }
            else if (order.ArchivedShippingName == LocalizationService.GetResource("Core.Orders.GiftCertificate.DeliveryByEmail"))
            {
                shipping = new BaseShippingOption() { Name = LocalizationService.GetResource("Core.Orders.GiftCertificate.DeliveryByEmail") };
            }

            if (shipping == null)
                shipping = new BaseShippingOption() { Name = order.ArchivedShippingName, Rate = order.ShippingCost };

            if (shipping == null)
                return Json("Доставка не выбрана");

            var preOrder = new PreOrder()
            {
                CountryDest = order.OrderCustomer.Country,
                CityDest = order.OrderCustomer.City,
                DistrictDest = order.OrderCustomer.District,
                ZipDest = order.OrderCustomer.Zip,
                ShippingOption = shipping,
                Currency = order.OrderCurrency
            };
            var items = order.OrderItems.Select(x => new PreOrderItem(x)).ToList();

            var manager = new PaymentManager(preOrder, items, null);
            var options = manager.GetOptions();

            if (order.PaymentDetails != null)
            {
                foreach (var option in options)
                    option.SetDetails(order.PaymentDetails);
            }

            BasePaymentOption selectedPayment = null;
            if (options != null)
                selectedPayment = options.FirstOrDefault(x => x.Id == order.PaymentMethodId) ?? options.FirstOrDefault();

            return Json(new { selectPayment = selectedPayment, option = options });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult BillingPaymentPost(BasePaymentOption payment, int orderId)
        {
            var order = OrderService.GetOrder(orderId);

            if (order == null || order.Payed || order.OrderStatus.IsCanceled)
                return Json(null);

            var paymentMethod = PaymentService.GetPaymentMethod(payment.Id);
            if (paymentMethod == null)
                return Json(null);

            order.PaymentMethodId = paymentMethod.PaymentMethodId;
            order.ArchivedPaymentName = paymentMethod.Name;
            order.PaymentCost = paymentMethod.GetExtracharge(order);

            order.PaymentDetails = payment.GetDetails();

            OrderService.UpdatePaymentDetails(order.OrderID, order.PaymentDetails);
            OrderService.UpdateOrderMain(order);

            OrderService.RefreshTotal(order);
            order = OrderService.GetOrder(order.OrderID);

            return Json(new { proceedToPayment = SettingsCheckout.ProceedToPayment });
        }

        public JsonResult BillingCartJson(int orderId)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return Json("error");

            var model = new BillingCartHandler().Get(order);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion Billing page

        #region Check order

        [ChildActionOnly]
        public ActionResult CheckOrderBlock()
        {
            if (!SettingsDesign.CheckOrderVisibility)
                return new EmptyResult();

            return PartialView();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckOrder(string ordernumber)
        {
            if (string.IsNullOrEmpty(ordernumber))
                return Json(null);

            var statusInfo = OrderService.GetStatusInfo(ordernumber);
            if (statusInfo == null)
                return Json(new CheckOrderModel() {Error = T("Checkout.CheckOrder.StatusCommentNotFound")});

            var order = OrderService.GetOrderByNumber(ordernumber);
            if (order == null)
                return Json(new CheckOrderModel() {Error = T("Checkout.CheckOrder.StatusCommentNotFound")});

            var model = new CheckOrderModel
            {
                StatusName = statusInfo.Hidden ? statusInfo.PreviousStatus : statusInfo.Status,
                StatusComment = statusInfo.Comment,
                OrderNumber = order.Number
            };

            if (order.ShippingMethod != null)
            {
                var shippingType = ReflectionExt.GetTypeByAttributeValue<Core.Common.Attributes.ShippingKeyAttribute>(typeof(BaseShipping), atr => atr.Value, order.ShippingMethod.ShippingType);
                if (shippingType.GetInterfaces().Contains(typeof(IShippingSupportingTheHistoryOfMovement)))
                {
                    var shipping = (IShippingSupportingTheHistoryOfMovement)Activator.CreateInstance(shippingType, order.ShippingMethod, null, null);
                    if (shipping.ActiveHistoryOfMovement)
                    {
                        var historyOfMovement = shipping.GetHistoryOfMovement(order);
                        var pointInfo = shipping.GetPointInfo(order);

                        model.ShippingHistory = new Models.MyAccount.ShippingHistoryOfMovementInfo
                        {
                            HistoryOfMovement = historyOfMovement != null ? historyOfMovement.Select(Models.MyAccount.HistoryOfMovementModel.CreateBy).ToList() : null,
                            PointInfo = pointInfo != null ? Models.MyAccount.PointInfoModel.CreateBy(pointInfo) : null
                        };
                    }
                }
            }

            return Json(model);
        }

        #endregion Check order

        #region Address

        public ActionResult AddressModal()
        {
            return PartialView(new CheckoutAddressViewModel());
        }

        #endregion Address

        #region ThankYouPage

        [ChildActionOnly]
        public ActionResult ThankYouPage(int? orderId, int? leadId)
        {
            var model = new ThankYouPageHandler(orderId, leadId).Execute();
            if (model == null)
                return new EmptyResult();

            return PartialView(model);
        }

        #endregion ThankYouPage

        #region DDeivery crutch

        public ActionResult DdeliveryRequest(int id, bool nppOption, string url, Dictionary<string, string> data)
        {
            var ddeliveryShipping = ShippingMethodService.GetShippingMethod(id);

            if (ddeliveryShipping == null)
                return new HttpUnauthorizedResult();

            var apiKey = ddeliveryShipping.Params.ElementOrDefault(Shipping.DDelivery.DDeliveryTemplate.ApiKey);
            var token = ddeliveryShipping.Params.ElementOrDefault(Shipping.DDelivery.DDeliveryTemplate.Token);
            var shopId = ddeliveryShipping.Params.ElementOrDefault(Shipping.DDelivery.DDeliveryTemplate.ShopId);
            url = url.Replace(":key", apiKey);

            var urlParams = string.Empty;
            if (url.Contains("calculator.json"))
            {
                var index = 0;
                foreach (var key in Request.QueryString.AllKeys)
                {
                    if (key == "data[apply_sdk_settings]")
                    {
                        urlParams += "apply_sdk_settings=" + Request.QueryString[key] + "&";
                    }
                    if (key == "data[city_to]")
                    {
                        urlParams += "city_to=" + Request.QueryString[key] + "&";
                    }
                    if (key == "data[item_count]")
                    {
                        urlParams += "item_count=" + Request.QueryString[key] + "&";
                    }
                    if (key == "data[side1]")
                    {
                        urlParams += "side1=" + Request.QueryString[key] + "&";
                    }
                    if (key == "data[side2]")
                    {
                        urlParams += "side2=" + Request.QueryString[key] + "&";
                    }
                    if (key == "data[side3]")
                    {
                        urlParams += "side3=" + Request.QueryString[key] + "&";
                    }
                    if (key == "data[weight]")
                    {
                        urlParams += "weight=" + Request.QueryString[key] + "&";
                    }
                    if (key == "data[price_declared]")
                    {
                        urlParams += "price_declared=" + Request.QueryString[key] + "&";
                        if (nppOption)
                        {
                            urlParams += "price_payment=" + Request.QueryString[key] + "&";
                            urlParams += "is_payment=1&";
                        }
                    }
                    /////////////////////////////////////////////////////
                    if (key == "data[products][" + index + "][price_declared]")
                    {
                        urlParams += "[products][" + index + "][price_declared]=" + Request.QueryString[key] + "&";
                    }
                    if (key == "data[products][" + index + "][count]")
                    {
                        urlParams += "[products][" + index + "][count]=" + Request.QueryString[key] + "&";
                    }
                    if (key == "data[products][" + index + "][discount]")
                    {
                        urlParams += "[products][" + index + "][discount]=" + Request.QueryString[key] + "&";
                    }
                    if (key == "data[products][" + index + "][nds]")
                    {
                        urlParams += "[products][" + index + "][nds]=" + Request.QueryString[key] + "&";
                        index++;
                    }
                }

                //foreach (KeyValuePair<string, string> pair in data)
                //{
                //    urlParams += pair.Key + "=" + pair.Value + "&";
                //    // do something with entry.Value or entry.Key
                //}
            }
            else if (url.Contains("index-by-address.json") || url.Contains("phone.json"))
            {
                foreach (var keyValue in data)
                {
                    urlParams += keyValue.Key + "=" + keyValue.Value + "&";
                }
            }

            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + token },
                { "shop-id", shopId }
            };
            var result = Core.Services.Helpers.RequestHelper.MakeRequest<string>(url + "?" + urlParams, headers: headers, method: Core.Services.Helpers.ERequestMethod.GET);
            //var result = RequestHelper.MakeRequest<string>(url, method: ERequestMethod.GET);
            return Content(result);

            //var dataString = Request.QueryString.ToString().Contains("data")
            //    ? //"data[apply_sdk_settings]=1&data[city_to]=151184&data[stopSubmit]=true&data[userFullName]=&data[userPhone]=&data[itemCount]=1&data[width]=10.0&data[height]=15.0&data[length]=5.0&data[weight]=0.3&data[nppOption]=false&data[products][0][name]=Суши Хамачи&data[products][0][vendorCode]=10500&data[products][0][price]=1000.0&data[products][0][count]=1"//Request.QueryString.ToString().Remove(0, Request.QueryString.ToString().IndexOf("data"))
            //    "data[apply_sdk_settings]=1&data[city_to]=151184&data[products][0][price_declared]=500&data[products][0][count]=1&data[products][0][discount]=0&data[products][0][nds]=0&data[price_declared]=0"
            //    : Request.QueryString.ToString();

            //var result = RequestHelper.MakeRequest<string>(url, System.Web.HttpUtility.UrlEncode(dataString), method: Request.HttpMethod.ToLower() == "get" ? ERequestMethod.GET : ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);

            //return Content(result);
        }

        #endregion DDeivery crutch

        #region Pay

        public ActionResult PayRedirect(string code)
        {
            var order = OrderService.GetOrderByCode(code);
            if (order == null || order.Payed)
                return View("PayRedirectError", (object)T("Checkout.PayRedirectError.OrderNotFound"));

            if (order.PaymentMethod == null || order.PaymentMethod.ProcessType != ProcessType.ServerRequest)
                return View("PayRedirectError", (object)T("Checkout.PayRedirectError.PaymentMethodNotFound"));

            var shouldBeConfirmedByManager = SettingsCheckout.ManagerConfirmed && !order.ManagerConfirmed;
            if (shouldBeConfirmedByManager)
                return View("PayRedirectError", (object)T("Checkout.PayRedirectError.NotConfirmedByManager"));

            var url = order.PaymentMethod.ProcessServerRequest(order);
            if (string.IsNullOrEmpty(url))
                return View("PayRedirectError", (object)T("Checkout.PayRedirectError.LinkNotAvailable"));

            return Redirect(url);
        }
        
        public ActionResult PayRedirectForMokka(string code)
        {
            var order = OrderService.GetOrderByCode(code);
            if (order == null || order.Payed)
                return View("PayRedirectErrorMokka", (object)T("Checkout.PayRedirectError.OrderNotFound"));

            if (order.PaymentMethod == null || !(order.PaymentMethod is Mokka))
                return View("PayRedirectErrorMokka", (object)T("Checkout.PayRedirectError.PaymentMethodNotFound"));

            var shouldBeConfirmedByManager = SettingsCheckout.ManagerConfirmed && !order.ManagerConfirmed;
            if (shouldBeConfirmedByManager)
                return View("PayRedirectErrorMokka", (object)T("Checkout.PayRedirectError.NotConfirmedByManager"));

            var url = order.PaymentMethod.ProcessServerRequest(order);
            if (string.IsNullOrEmpty(url))
                return View("PayRedirectErrorMokka", (object)T("Checkout.PayRedirectError.LinkNotAvailable"));

            return Redirect(url);
        }

        public ActionResult GetOrderPay(Models.Checkout.OrderPayModel model)
        {
            var order = 
                model.OrderCode.IsNotEmpty() 
                    ? OrderService.GetOrderByCode(model.OrderCode) 
                    : null;
            // order = order ??
            //         (model.OrderId.HasValue
            //             ? OrderService.GetOrder(model.OrderId.Value)
            //             : null);

            var paymentMethod =
                model.PaymentMethodId.HasValue
                        ? PaymentService.GetPaymentMethod(model.PaymentMethodId.Value)
                        : order?.PaymentMethod;

            if (
                order == null
                || order.Payed
                || order.OrderStatus.IsCanceled
                || paymentMethod is null
                || (SettingsCheckout.ManagerConfirmed && !order.ManagerConfirmed)
               )
                return new EmptyResult();
            
            if (paymentMethod is ICreditPaymentMethod creditPaymentMethod 
                && creditPaymentMethod.ActiveCreditPayment 
                && (creditPaymentMethod.MinimumPrice > order.Sum.ConvertCurrency(order.OrderCurrency, paymentMethod.PaymentCurrency ?? order.OrderCurrency)
                    || creditPaymentMethod.MaximumPrice < order.Sum.ConvertCurrency(order.OrderCurrency, paymentMethod.PaymentCurrency ?? order.OrderCurrency)))
                return new EmptyResult();

            
            var viewModel =
                new OrderPayHandler(
                        order,
                        paymentMethod,
                        model.PageWithPaymentButton)
                    .Execute();
            
            if (viewModel == null)
                return new EmptyResult();

            if (
                viewModel.ViewPath != null &&
                ViewEngineCollection.FindPartialView(ControllerContext, viewModel.ViewPath)?.View != null
                )
                return PartialView(viewModel.ViewPath, viewModel);
            return PartialView("OrderPay/_Common", viewModel);
        }

        #endregion Pay
    }
}