//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;

namespace AdvantShop.Core.Modules
{

    public class ModulesExecuter
    {
        #region PictureModules

        public static void ProcessPhoto(Image image)
        {
            foreach (var cls in AttachedModules.GetModules<IProcessPhoto>())
            {
                var classInstance = (IProcessPhoto)Activator.CreateInstance(cls, null);
                classInstance.DoProcessPhoto(image);
            }
        }

        #endregion

        #region OrderModules

        public static void OrderAdded(IOrder order)
        {
            try
            {
                foreach (var cls in AttachedModules.GetModules<IOrderChanged>())
                {
                    var classInstance = (IOrderChanged)Activator.CreateInstance(cls, null);
                    classInstance.DoOrderAdded(order);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        public static void OrderChangeStatus(int orderId)
        {
            IOrder order = null;

            var modules = AttachedModules.GetModules<IOrderChanged>();
            if (modules.Count > 0)
                order = OrderService.GetOrder(orderId);

            try
            {
                foreach (var cls in modules)
                {
                    var classInstance = (IOrderChanged)Activator.CreateInstance(cls, null);
                    classInstance.DoOrderChangeStatus(order);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        public static void OrderUpdated(IOrder order)
        {
            try
            {
                foreach (var cls in AttachedModules.GetModules<IOrderChanged>())
                {
                    var classInstance = (IOrderChanged) Activator.CreateInstance(cls, null);
                    classInstance.DoOrderUpdated(order);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        public static void OrderDeleted(int orderId)
        {
            try
            {
                foreach (var cls in AttachedModules.GetModules<IOrderChanged>())
                {
                    var classInstance = (IOrderChanged) Activator.CreateInstance(cls, null);
                    classInstance.DoOrderDeleted(orderId);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }


        public static void PayOrder(int orderId, bool payed)
        {
            try
            {
                foreach (var cls in AttachedModules.GetModules<IOrderChanged>())
                {
                    var classInstance = (IOrderChanged) Activator.CreateInstance(cls, null);
                    classInstance.PayOrder(orderId, payed);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        public static void UpdateComments(int orderId)
        {
            foreach (var cls in AttachedModules.GetModules<IOrderChanged>())
            {
                var classInstance = (IOrderChanged)Activator.CreateInstance(cls, null);
                classInstance.UpdateComments(orderId);
            }
        }


        #endregion

        #region CustomerActions

        public static void AddToCart(ShoppingCartItem item, string url = "")
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>().Union(AttachedModules.GetCore<ICustomerAction>()))
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.AddToCart(item, url);
            }
        }
        public static void AddToCompare(ShoppingCartItem item, string url = "")
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>().Union(AttachedModules.GetCore<ICustomerAction>()))
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.AddToCompare(item, url);
            }
        }
        public static void AddToWishList(ShoppingCartItem item, string url = "")
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>().Union(AttachedModules.GetCore<ICustomerAction>()))
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.AddToWishList(item, url);
            }
        }
        public static void Subscribe(Subscription subscription, string objectId = "")
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>().Union(AttachedModules.GetCore<ICustomerAction>()))
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.Subscribe(subscription.Email);
            }

            var modules = AttachedModules.GetModules<ISendMails>();
            foreach (var moduleType in modules)
            {
                var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                if (string.IsNullOrEmpty(objectId))
                {
                    moduleObject.SubscribeEmail(subscription);
                }
                else
                {
                    moduleObject.SubscribeEmail(subscription, objectId);
                }
            }
        }

        public static void UnSubscribe(string email)
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>().Union(AttachedModules.GetCore<ICustomerAction>()))
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.UnSubscribe(email);
            }

            var modules = AttachedModules.GetModules<ISendMails>();
            foreach (var moduleType in modules)
            {
                var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                moduleObject.UnsubscribeEmail(email);
            }

        }

        public static void Search(string searchTerm, int resultsCount)
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>().Union(AttachedModules.GetCore<ICustomerAction>()))
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.Search(searchTerm, resultsCount);
            }
        }

        public static void Registration(Customer customer)
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>().Union(AttachedModules.GetCore<ICustomerAction>()))
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.Register(customer);
            }
        }

        public static void Login(Customer customer)
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>())
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.Login(customer);
            }
        }

        public static void ViewMyAccount(Customer customer)
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>().Union(AttachedModules.GetCore<ICustomerAction>()))
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.ViewMyAccount(customer);
            }
        }

        public static void FilterCatalog()
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>().Union(AttachedModules.GetCore<ICustomerAction>()))
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.FilterCatalog();
            }
        }

        public static void Vote()
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerAction>().Union(AttachedModules.GetCore<ICustomerAction>()))
            {
                var classInstance = (ICustomerAction)Activator.CreateInstance(cls, null);
                classInstance.Vote();
            }
        }
        #endregion

        #region ISendOrderNotifications

        public static void SendNotificationsOnOrderAdded(IOrder order)
        {
            var modules = AttachedModules.GetModules<ISendOrderNotifications>();
            foreach (var cls in modules)
            {
                var classInstance = (ISendOrderNotifications)Activator.CreateInstance(cls, null);
                classInstance.SendOnOrderAdded(order);
            }
        }

        public static void SendNotificationsOnOrderChangeStatus(IOrder order)
        {
            var modules = AttachedModules.GetModules<ISendOrderNotifications>();
            foreach (var cls in modules)
            {
                var classInstance = (ISendOrderNotifications)Activator.CreateInstance(cls, null);
                classInstance.SendOnOrderChangeStatus(order);
            }
        }


        public static bool SendNotificationsHasTemplatesOnChangeStatus(int orderStatusId)
        {
            bool res = false;
            var modules = AttachedModules.GetModules<ISendOrderNotifications>();
            foreach (var cls in modules)
            {
                var classInstance = (ISendOrderNotifications)Activator.CreateInstance(cls, null);
                res |= classInstance.HaveSmsTemplate(orderStatusId);
            }

            return res;
        }


        public static void SendNotificationsOnOrderUpdated(IOrder order)
        {
            var modules = AttachedModules.GetModules<ISendOrderNotifications>();
            foreach (var cls in modules)
            {
                var classInstance = (ISendOrderNotifications)Activator.CreateInstance(cls, null);
                classInstance.SendOnOrderUpdated(order);
            }
        }

        public static void SendNotificationsOnOrderDeleted(int orderId)
        {
            var modules = AttachedModules.GetModules<ISendOrderNotifications>();
            foreach (var cls in modules)
            {
                var classInstance = (ISendOrderNotifications)Activator.CreateInstance(cls, null);
                classInstance.SendOnOrderDeleted(orderId);
            }
        }

        public static void SendNotificationsOnPayOrder(int orderId, bool payed)
        {
            var modules = AttachedModules.GetModules<ISendOrderNotifications>();
            foreach (var cls in modules)
            {
                var classInstance = (ISendOrderNotifications)Activator.CreateInstance(cls, null);
                classInstance.SendOnPayOrder(orderId, payed);
            }
        }

        #endregion

        #region ICustomerChange

        public static void AddCustomer(Customer customer)
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerChange>().Union(AttachedModules.GetCore<ICustomerChange>()))
            {
                var classInstance = (ICustomerChange)Activator.CreateInstance(cls, null);
                classInstance.Add(customer);
            }
        }

        public static void UpdateCustomer(Customer customer)
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerChange>().Union(AttachedModules.GetCore<ICustomerChange>()))
            {
                var classInstance = (ICustomerChange)Activator.CreateInstance(cls, null);
                classInstance.Update(customer);
            }
        }

        public static void UpdateCustomer(Guid customerId)
        {
            Customer customer = null;
            foreach (var cls in AttachedModules.GetModules<ICustomerChange>().Union(AttachedModules.GetCore<ICustomerChange>()))
            {
                var classInstance = (ICustomerChange)Activator.CreateInstance(cls, null);
                classInstance.Update(customer ?? (customer = CustomerService.GetCustomer(customerId)));
            }
        }

        public static void DeleteCustomer(Guid customerId)
        {
            foreach (var cls in AttachedModules.GetModules<ICustomerChange>().Union(AttachedModules.GetCore<ICustomerChange>()))
            {
                var classInstance = (ICustomerChange)Activator.CreateInstance(cls, null);
                classInstance.Delete(customerId);
            }
        }

        #endregion

        #region IContactChange

        public static void AddContact(CustomerContact contact)
        {
            foreach (var cls in AttachedModules.GetModules<IContactChange>().Union(AttachedModules.GetCore<IContactChange>()))
            {
                var classInstance = (IContactChange)Activator.CreateInstance(cls, null);
                classInstance.Add(contact);
            }
        }

        public static void UpdateContact(CustomerContact contact)
        {
            foreach (var cls in AttachedModules.GetModules<IContactChange>().Union(AttachedModules.GetCore<IContactChange>()))
            {
                var classInstance = (IContactChange)Activator.CreateInstance(cls, null);
                classInstance.Update(contact);
            }
        }

        public static void DeleteContact(Guid contactId)
        {
            foreach (var cls in AttachedModules.GetModules<IContactChange>().Union(AttachedModules.GetCore<IContactChange>()))
            {
                var classInstance = (IContactChange)Activator.CreateInstance(cls, null);
                classInstance.Delete(contactId);
            }
        }

        #endregion

        #region Lead

        public static void LeadAdded(Lead lead)
        {
            var modules = AttachedModules.GetModules<ILeadChanged>();
            foreach (var cls in modules)
            {
                var classInstance = (ILeadChanged)Activator.CreateInstance(cls, null);
                classInstance.LeadAdded(lead);
            }
        }

        public static void LeadUpdated(Lead lead)
        {
            var modules = AttachedModules.GetModules<ILeadChanged>();
            foreach (var cls in modules)
            {
                var classInstance = (ILeadChanged)Activator.CreateInstance(cls, null);
                classInstance.LeadUpdated(lead);
            }
        }

        public static void LeadDeleted(int leadId)
        {
            var modules = AttachedModules.GetModules<ILeadChanged>();
            foreach (var cls in modules)
            {
                var classInstance = (ILeadChanged)Activator.CreateInstance(cls, null);
                classInstance.LeadDeleted(leadId);
            }
        }

        #endregion


        #region CheckInfo

        public static bool CheckInfo(HttpContext currentContext, ECheckType checkType, string senderEmail, string senderNickname, string message = "", string phone = "")
        {
            var result = true;
            var modules = AttachedModules.GetModules<ICheckInfo>();
            foreach (var cls in modules)
            {
                var classInstance = (ICheckInfo)Activator.CreateInstance(cls, null);
                result &= classInstance.CheckInfo(currentContext, checkType, senderEmail, senderNickname, message, phone);
            }
            return result;
        }

        //public static bool CheckNewUser(string senderEmail, string senderIp, int submitTime, string senderNickname, string phone)
        //{
        //    var result = true;
        //    var modules = AttachedModules.GetModules<ICheckMessage>();
        //    foreach (var cls in modules)
        //    {
        //        var classInstance = (ICheckUser)Activator.CreateInstance(cls, null);
        //        result &= classInstance.CheckUser(senderEmail, senderIp, submitTime, senderNickname, phone);
        //    }
        //    return result;
        //}
        #endregion

        #region VirtualCategory
        public static Category GetVirtualCategory(Category category)
        {
            foreach (var cls in AttachedModules.GetModules<IVirtualCategory>().Union(AttachedModules.GetCore<IVirtualCategory>()))
            {
                var classInstance = (IVirtualCategory)Activator.CreateInstance(cls, null);
                return classInstance.GetVirtualCategory(category);
            }
            return category;
        }

        public static ICategoryModel GetVirtualCategoryModel(ICategoryModel model)
        {
            foreach (var cls in AttachedModules.GetModules<IVirtualCategory>().Union(AttachedModules.GetCore<IVirtualCategory>()))
            {
                var classInstance = (IVirtualCategory)Activator.CreateInstance(cls, null);
                return classInstance.GetVirtualCategoryModel(model);
            }
            return model;
        }

        public static List<BreadCrumbs> GetVirtualCategoryBreadCrumbs(List<BreadCrumbs> breadCrumbs)
        {
            foreach (var cls in AttachedModules.GetModules<IVirtualCategory>().Union(AttachedModules.GetCore<IVirtualCategory>()))
            {
                var classInstance = (IVirtualCategory)Activator.CreateInstance(cls, null);
                return classInstance.GetVirtualCategoryBreadCrumbs(breadCrumbs);
            }
            return breadCrumbs;
        }

        public static Dictionary<int, KeyValuePair<float, float>> GetRangeIds(Dictionary<int, KeyValuePair<float, float>> rangeIds)
        {
            foreach (var cls in AttachedModules.GetModules<IVirtualCategory>().Union(AttachedModules.GetCore<IVirtualCategory>()))
            {
                var classInstance = (IVirtualCategory)Activator.CreateInstance(cls, null);
                return classInstance.GetRangeIds(rangeIds);
            }
            return rangeIds;
        }

        public static string GetUrlParentCategory(string url)
        {
            foreach (var cls in AttachedModules.GetModules<IVirtualCategory>().Union(AttachedModules.GetCore<IVirtualCategory>()))
            {
                var classInstance = (IVirtualCategory)Activator.CreateInstance(cls, null);
                return classInstance.GetUrlParentCategory(url);
            }
            return url;
        }
        #endregion

        #region IGeoIp

        public static List<Repository.IpZone> GetIpZonesAutocomplete(string q, bool inAdminPart = false)
        {
            foreach (var cls in AttachedModules.GetModules<IGeoIp>())
            {
                var classInstance = (IGeoIp)Activator.CreateInstance(cls, null);
                var ipZones = classInstance.GetIpZonesAutocomplete(q, inAdminPart);
                if (ipZones != null)
                    return ipZones;
            }
            return new List<Repository.IpZone>();
        }

        public static void OnSetZone(Repository.IpZone ipZone)
        {
            foreach (var cls in AttachedModules.GetModules<IGeoIp>())
            {
                var classInstance = (IGeoIp)Activator.CreateInstance(cls, null);
                classInstance.OnSetZone(ipZone);
            }
        }

        #endregion

        #region ISuggestions

        public static void ProcessCheckoutAddress(CheckoutAddressQueryModel address)
        {
            foreach (var cls in AttachedModules.GetModules<ISuggestions>())
            {
                var classInstance = (ISuggestions)Activator.CreateInstance(cls, null);
                classInstance.ProcessCheckoutAddress(address);
            }
        }

        public static void ProcessAddress(SuggestAddressQueryModel model, bool inAdminPart = false)
        {
            foreach (var cls in AttachedModules.GetModules<ISuggestions>())
            {
                var classInstance = (ISuggestions)Activator.CreateInstance(cls, null);
                classInstance.ProcessAddress(model, inAdminPart);
            }
        }

        public static void GetSuggestionsHtmlAttributes(int customerFieldId, Dictionary<string, object> htmlAttributes)
        {
            foreach (var cls in AttachedModules.GetModules<ISuggestions>())
            {
                var instance = (ISuggestions)Activator.CreateInstance(cls, null);
                instance.GetSuggestionsHtmlAttributes(customerFieldId, htmlAttributes);
            }
        }

        #endregion

        #region IIgnoreCheckoutShipping

        public static List<ShoppingCartItem> GetIgnoreShippingCartItems()
        {
            var result = new List<ShoppingCartItem>();
            var modules = AttachedModules.GetModules<IIgnoreCheckoutShipping>();
            foreach (var cls in modules)
            {
                var classInstance = (IIgnoreCheckoutShipping)Activator.CreateInstance(cls, null);

                var ids = classInstance.GetIgnoreShippingCartItems();

                if (ids != null && ids.Count > 0)
                    result.AddRange(ids);
            }
            return result;
        }

        #endregion

        #region IShippingMethod

        public static List<Services.Configuration.ListItemModel> GetDropdownShippings()
        {
            return Caching.CacheManager.Get("ModulesDropdownShippings", () => 
                AttachedModules.GetModules<IShippingMethod>(ignoreActive: true) // из-за ignoreActive можно в кэш закидывать
                .Where(module => module != null)
                .Select(module => (IShippingMethod)Activator.CreateInstance(module))
                .Select(moduleInstance =>
                    new Services.Configuration.ListItemModel
                    {
                        Value = moduleInstance.ShippingKey,
                        Text = moduleInstance.ShippingName
                    })
                .ToList());
        }

        #endregion

     
        #region ITaskChanged
        public static void DoTaskAdded(ITask task, ICustomer managerToNotify)
        {
            try
            {
                foreach (var cls in AttachedModules.GetModules<ITaskChanged>())
                {
                    var classInstance = (ITaskChanged)Activator.CreateInstance(cls, null);
                    classInstance.DoTaskAdded(task, managerToNotify);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        public static void DoTaskChanged(ITask oldTask, ITask newTask, ICustomer managerToNotify)
        {
            try
            {
                foreach (var cls in AttachedModules.GetModules<ITaskChanged>())
                {
                    var classInstance = (ITaskChanged)Activator.CreateInstance(cls, null);
                    classInstance.DoTaskChanged(oldTask, newTask, managerToNotify);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }


        public static void DoTaskCommentAdded(ITask task, IAdminComment comment, ICustomer managerToNotify)
        {
            try
            {
                foreach (var cls in AttachedModules.GetModules<ITaskChanged>())
                {
                    var classInstance = (ITaskChanged)Activator.CreateInstance(cls, null);
                    classInstance.DoTaskCommentAdded(task, comment, managerToNotify);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }
        #endregion
    }
}