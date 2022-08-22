using System;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class SaveOrderDraftModel
    {
        public int OrderId { get; set; }
        public Guid CustomerId { get; set; }
    }

    public class SaveOrderDraft
    {
        private readonly OrderDraftModel _draftOrder;
        private readonly Order _order;

        public SaveOrderDraft(OrderDraftModel draftOrder)
        {
            _draftOrder = draftOrder;
            _order = OrderService.GetOrder(_draftOrder.OrderId) ?? new Order() {IsDraft = true};
        }

        public SaveOrderDraft(Order order)
        {
            _order = order;
            _order.IsDraft = false;
        }


        public SaveOrderDraftModel Execute()
        {
            if (_order.OrderID == 0)
            {
                _order.OrderDate = DateTime.Now;
                _order.OrderCurrency = CurrencyService.CurrentCurrency;
                _order.OrderStatusId = OrderStatusService.DefaultOrderStatus;
            }

            try
            {
                _order.IsFromAdminArea = true;

                if (_order.OrderCustomer == null)
                    _order.OrderCustomer = new OrderCustomer();

                if (_draftOrder != null)
                {
                    _order.StatusComment = _draftOrder.StatusComment;
                    _order.AdminOrderComment = _draftOrder.AdminOrderComment;
                    _order.OrderSourceId = _draftOrder.OrderSourceId;
                    _order.ManagerId = _draftOrder.ManagerId;
                    _order.TrackNumber = _draftOrder.TrackNumber;

                    if (_draftOrder.OrderCustomer != null)
                    {
                        _order.OrderCustomer.CustomerID = _draftOrder.OrderCustomer.CustomerID;
                        if (_order.OrderCustomer.CustomerID == Guid.Empty)
                            _order.OrderCustomer.CustomerID = Guid.NewGuid();

                        _order.OrderCustomer.FirstName = _draftOrder.OrderCustomer.FirstName;
                        _order.OrderCustomer.LastName = _draftOrder.OrderCustomer.LastName;
                        _order.OrderCustomer.Patronymic = _draftOrder.OrderCustomer.Patronymic;
                        _order.OrderCustomer.Email = _draftOrder.OrderCustomer.Email;
                        _order.OrderCustomer.Organization = _draftOrder.OrderCustomer.Organization;

                        if (!string.IsNullOrWhiteSpace(_order.OrderCustomer.Email))
                        {
                            var c = CustomerService.GetCustomerByEmail(_order.OrderCustomer.Email);
                            if (c != null && c.Id != Guid.Empty)
                                _order.OrderCustomer.CustomerID = c.Id;
                        }

                        _order.OrderCustomer.Phone = _draftOrder.OrderCustomer.Phone;
                        _order.OrderCustomer.StandardPhone =
                            StringHelper.ConvertToStandardPhone(_order.OrderCustomer.Phone, true, true);

                        _order.OrderCustomer.Country = _draftOrder.OrderCustomer.Country;
                        _order.OrderCustomer.Region = _draftOrder.OrderCustomer.Region;
                        _order.OrderCustomer.District = _draftOrder.OrderCustomer.District;
                        _order.OrderCustomer.City = _draftOrder.OrderCustomer.City;
                        _order.OrderCustomer.Zip = _draftOrder.OrderCustomer.Zip;
                        //_order.OrderCustomer.Address = _draftOrder.OrderCustomer.Address;
                        _order.OrderCustomer.CustomField1 = _draftOrder.OrderCustomer.CustomField1;
                        _order.OrderCustomer.CustomField2 = _draftOrder.OrderCustomer.CustomField2;
                        _order.OrderCustomer.CustomField3 = _draftOrder.OrderCustomer.CustomField3;

                        _order.OrderCustomer.Street = _draftOrder.OrderCustomer.Street;
                        _order.OrderCustomer.House = _draftOrder.OrderCustomer.House;
                        _order.OrderCustomer.Apartment = _draftOrder.OrderCustomer.Apartment;
                        _order.OrderCustomer.Structure = _draftOrder.OrderCustomer.Structure;
                        _order.OrderCustomer.Entrance = _draftOrder.OrderCustomer.Entrance;
                        _order.OrderCustomer.Floor = _draftOrder.OrderCustomer.Floor;

                        var currentCustomer = CustomerService.GetCustomer(_draftOrder.OrderCustomer.CustomerID);
                        var group = currentCustomer != null ? currentCustomer.CustomerGroup : CustomerGroupService.GetCustomerGroup();
                        _order.GroupDiscount = group.GroupDiscount;
                        _order.GroupName = group.GroupName;
                    }
                }

                var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);
                var isNewOrder = false;

                if (_order.OrderID == 0)
                {
                    _order.OrderID = OrderService.AddOrder(_order, changedBy, trackChanges: !_order.IsDraft);
                    isNewOrder = !_order.IsDraft;
                }
                else
                {
                    var updateModules = false;

                    var prevOrder = OrderService.GetOrder(_order.OrderID);

                    if (!_order.IsDraft && prevOrder != null && prevOrder.IsDraft)
                        isNewOrder = updateModules = true;

                    OrderService.UpdateOrderMain(_order, updateModules: updateModules, changedBy: changedBy, trackChanges: false);
                    OrderService.UpdateOrderCustomer(_order.OrderCustomer, changedBy, false);
                    OrderService.UpdateStatusComment(_order.OrderID, _order.StatusComment);
                }
                
                if (isNewOrder)
                {
                    OrderStatusService.ChangeOrderStatus(_order.OrderID, OrderStatusService.DefaultOrderStatus,
                        LocalizationService.GetResource("Core.OrderStatus.CreatedFromAdmin"), false);

                    Core.Services.Bonuses.Model.Card bonusCard = null;
                    if (BonusSystem.IsActive && (bonusCard = _order.GetOrderBonusCard()) != null && !bonusCard.Blocked)
                    {
                        BonusSystemService.MakeBonusPurchase(bonusCard.CardNumber, _order);
                    }
                }

                return new SaveOrderDraftModel()
                {
                    OrderId = _order.OrderID,
                    CustomerId = _order.OrderCustomer != null ? _order.OrderCustomer.CustomerID : Guid.NewGuid()
                };
            }
            catch (Exception ex)
            {
                Debug.Log.Error("Update order draft", ex);
            }

            return null;
        }
    }
}
