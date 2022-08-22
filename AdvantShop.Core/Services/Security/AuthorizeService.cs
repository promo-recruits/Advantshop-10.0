//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using System.Web.Security;
using AdvantShop.Core.Common;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;

namespace AdvantShop.Security
{
    public class AuthorizeService
    {
        private const string Spliter = ":";

        public static Customer GetAuthenticatedCustomer()
        {
            if (HttpContext.Current == null) return null;

            var formsCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (formsCookie != null)
            {
                try
                {
                    var formsAuthenticationTicket = FormsAuthentication.Decrypt(formsCookie.Value);
                    if (formsAuthenticationTicket != null)
                    {
                        var token = formsAuthenticationTicket.Name;
                        var words = token.Split(new[] { Spliter }, StringSplitOptions.RemoveEmptyEntries);
                        if (words.Length != 2) return null;
                        var email = words[0];
                        var passHash = words[1];
                        return string.IsNullOrEmpty(email)
                            ? null
                            : CustomerService.GetCustomerByEmailAndPassword(email, passHash, true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
            return null;
        }

        public static bool SignIn(string email, string password, bool isHash, bool createPersistentCookie, out Customer customer)
        {
            customer = null;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return false;

            var isDebug = Secure.IsDebugAccount(email, password);
            if (isDebug)
            {
                CustomerContext.IsDebug = true;

                customer = new Customer()
                {
                    CustomerRole = Role.Administrator,
                    IsVirtual = true,
                    Enabled = true
                };
                HttpContext.Current.Items["CustomerContext"] = customer;

                Secure.AddUserLog("sa", true, true);
                return true;
            }

            var oldCustomerId = CustomerContext.CurrentCustomer.Id;

            customer = CustomerService.GetCustomerByEmailAndPassword(email, password, isHash);
            if (customer == null)
                return false;

            Secure.AddUserLog(customer.EMail, true, customer.IsAdmin);
            ShoppingCartService.MergeShoppingCarts(oldCustomerId, customer.Id);
            CustomerContext.SetCustomerCookie(customer.Id);
            FormsAuthentication.SetAuthCookie(email + Spliter + customer.Password, createPersistentCookie);

            return true;
        }

        public static bool SignIn(string email, string password, bool isHash, bool createPersistentCookie)
        {
            Customer customer;
            return SignIn(email, password, isHash, createPersistentCookie, out customer);
        }

        public static void SignOut()
        {
            CustomerContext.IsDebug = false;
            CustomerContext.DeleteCustomerCookie();
            
            //удаляем куку после выхода из аккаунта чтобы не привязывались другие пользователи
            PartnerService.ClearReferralCookie();
            
            FormsAuthentication.SignOut();
        }

    }
}