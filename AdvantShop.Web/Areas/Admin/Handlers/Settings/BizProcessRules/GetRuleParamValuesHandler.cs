using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models;

namespace AdvantShop.Web.Admin.Handlers.Settings.BizProcessRules
{
    public class GetRuleParamValuesHandler
    {
        private readonly EBizProcessEventType _eventType;
        private readonly int _fieldType;
        private readonly int? _fieldObjId;

        public GetRuleParamValuesHandler(EBizProcessEventType eventType, int fieldType, int? fieldObjId)
        {
            _eventType = eventType;
            _fieldType = fieldType;
            _fieldObjId = fieldObjId;
        }

        public List<SelectItemModel> Execute()
        {
            switch (_eventType)
            {
                case EBizProcessEventType.OrderCreated:
                case EBizProcessEventType.OrderStatusChanged:
                    switch (_fieldType)
                    {
                        case (int)EOrderFieldType.CustomerGroup:
                            return GetCustomerGroupsList();
                        case (int)EOrderFieldType.CustomerField:
                            return GetCustomerFieldValuesList();
                        case (int)EOrderFieldType.OrderSource:
                            return GetOrderSourcesList();
                        case (int)EOrderFieldType.PaymentMethod:
                            return PaymentService.GetAllPaymentMethods(false).Select(x => new SelectItemModel(x.Name, x.PaymentMethodId)).ToList();
                        case (int)EOrderFieldType.ShippingMethod:
                            return ShippingMethodService.GetAllShippingMethods().Select(x => new SelectItemModel(x.Name, x.ShippingMethodId)).ToList();
                        case (int)EOrderFieldType.Manager:
                            return GetManagersList();
                        default:
                            return null;
                    }
                case EBizProcessEventType.LeadCreated:
                case EBizProcessEventType.LeadStatusChanged:
                    switch (_fieldType)
                    {
                        case (int)ELeadFieldType.CustomerGroup:
                            return GetCustomerGroupsList();
                        case (int)ELeadFieldType.CustomerField:
                            return GetCustomerFieldValuesList();
                        case (int)ELeadFieldType.Source:
                            return GetOrderSourcesList();
                        case (int)ELeadFieldType.SalesFunnel:
                            return SalesFunnelService.GetList().Select(x => new SelectItemModel(x.Name, x.Id)).ToList();
                        default:
                            return null;
                    }
                case EBizProcessEventType.CallMissed:
                    switch (_fieldType)
                    {
                        case (int)ECallFieldType.CustomerGroup:
                            return GetCustomerGroupsList();
                        case (int)ECallFieldType.CustomerField:
                            return GetCustomerFieldValuesList();
                        default:
                            return null;
                    }
                case EBizProcessEventType.TaskCreated:
                case EBizProcessEventType.TaskStatusChanged:
                    switch (_fieldType)
                    {
                        case (int)ETaskFieldType.AssignedManager:
                        case (int)ETaskFieldType.AppointedManager:
                            return GetManagersList();
                        case (int)ETaskFieldType.Priority:
                            return Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>().Select(x => new SelectItemModel(x.Localize(), x.ConvertIntString())).ToList();
                        case (int)ETaskFieldType.TaskGroup:
                            return TaskGroupService.GetAllTaskGroups().Select(x => new SelectItemModel(x.Name, x.Id)).ToList();
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

        private List<SelectItemModel> GetCustomerGroupsList()
        {
            return CustomerGroupService.GetCustomerGroupList().Select(x => new SelectItemModel(x.GroupName, x.CustomerGroupId.ToString())).ToList();
        }

        private List<SelectItemModel> GetCustomerFieldValuesList()
        {
            var customerField = _fieldObjId.HasValue ? CustomerFieldService.GetCustomerField(_fieldObjId.Value) : null;
            return customerField != null && customerField.FieldType == CustomerFieldType.Select
                    ? CustomerFieldService.GetCustomerFieldValues(customerField.Id).Select(x => new SelectItemModel(x.Value, x.Value)).ToList()
                    : null;
        }

        private List<SelectItemModel> GetOrderSourcesList()
        {
            return OrderSourceService.GetOrderSources().Select(x => new SelectItemModel(x.Name, x.Id)).ToList();
        }

        private List<SelectItemModel> GetManagersList()
        {
            return ManagerService.GetManagersList().Select(x => new SelectItemModel(x.FullName, x.ManagerId)).ToList();
        }
    }
}