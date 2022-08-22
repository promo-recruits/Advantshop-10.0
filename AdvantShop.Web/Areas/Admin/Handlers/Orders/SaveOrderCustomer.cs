using System;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class SaveOrderCustomer
    {
        private readonly OrderCustomerModel _orderCustomer;
        private readonly Order _order;

        public SaveOrderCustomer(OrderCustomerModel orderCustomer)
        {
            _orderCustomer = orderCustomer;
            _order = OrderService.GetOrder(_orderCustomer.OrderId);
        }

        public bool Execute()
        {
            try
            {
                _order.IsFromAdminArea = true;

                if (_order.OrderCustomer == null)
                    _order.OrderCustomer = new OrderCustomer();

                if (_orderCustomer != null)
                {
                    if (_orderCustomer.OrderCustomer != null)
                    {
                        _order.OrderCustomer.CustomerID = _orderCustomer.OrderCustomer.CustomerID;
                        if (_order.OrderCustomer.CustomerID == Guid.Empty)
                            _order.OrderCustomer.CustomerID = Guid.NewGuid();

                        _order.OrderCustomer.FirstName = _orderCustomer.OrderCustomer.FirstName;
                        _order.OrderCustomer.LastName = _orderCustomer.OrderCustomer.LastName;
                        _order.OrderCustomer.Patronymic = _orderCustomer.OrderCustomer.Patronymic;
                        _order.OrderCustomer.Email = _orderCustomer.OrderCustomer.Email;
                        _order.OrderCustomer.Organization = _orderCustomer.OrderCustomer.Organization;

                        if (!string.IsNullOrWhiteSpace(_order.OrderCustomer.Email))
                        {
                            var c = CustomerService.GetCustomerByEmail(_order.OrderCustomer.Email);
                            if (c != null && c.Id != Guid.Empty)
                                _order.OrderCustomer.CustomerID = c.Id;
                        }

                        _order.OrderCustomer.Phone = _orderCustomer.OrderCustomer.Phone;
                        _order.OrderCustomer.StandardPhone =
                            StringHelper.ConvertToStandardPhone(_order.OrderCustomer.Phone, true, true);

                        _order.OrderCustomer.Country = _orderCustomer.OrderCustomer.Country;
                        _order.OrderCustomer.Region = _orderCustomer.OrderCustomer.Region;
                        _order.OrderCustomer.District = _orderCustomer.OrderCustomer.District;
                        _order.OrderCustomer.City = _orderCustomer.OrderCustomer.City;
                        _order.OrderCustomer.Zip = _orderCustomer.OrderCustomer.Zip;
                        //_order.OrderCustomer.Address = _orderCustomer.OrderCustomer.Address;
                        _order.OrderCustomer.CustomField1 = _orderCustomer.OrderCustomer.CustomField1;
                        _order.OrderCustomer.CustomField2 = _orderCustomer.OrderCustomer.CustomField2;
                        _order.OrderCustomer.CustomField3 = _orderCustomer.OrderCustomer.CustomField3;

                        _order.OrderCustomer.Street = _orderCustomer.OrderCustomer.Street;
                        _order.OrderCustomer.House = _orderCustomer.OrderCustomer.House;
                        _order.OrderCustomer.Apartment = _orderCustomer.OrderCustomer.Apartment;
                        _order.OrderCustomer.Structure = _orderCustomer.OrderCustomer.Structure;
                        _order.OrderCustomer.Entrance = _orderCustomer.OrderCustomer.Entrance;
                        _order.OrderCustomer.Floor = _orderCustomer.OrderCustomer.Floor;

                        var currentCustomer = CustomerService.GetCustomer(_orderCustomer.OrderCustomer.CustomerID);
                        var group = currentCustomer != null ? currentCustomer.CustomerGroup : CustomerGroupService.GetCustomerGroup();
                        _order.GroupDiscount = group.GroupDiscount;
                        _order.GroupName = group.GroupName;
                    }
                }

                var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);

                var updateModules = !_order.IsDraft;

                OrderService.UpdateOrderCustomer(_order.OrderCustomer, changedBy, updateModules);
                OrderService.UpdateOrderMain(_order, updateModules: updateModules, changedBy: changedBy, trackChanges: updateModules);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("SaveOrderCustomer error", ex);
            }

            return false;
        }
    }
}
