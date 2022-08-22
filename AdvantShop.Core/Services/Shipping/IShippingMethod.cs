//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Orders;

namespace AdvantShop.Shipping
{
    public interface IShippingOption
    {
        OptionValidationResult Validate();
        string ForMailTemplate();
    }
    
    public interface IShipping
    {
        IEnumerable<BaseShippingOption> GetOptions();
    }

    /// <summary>
    /// Реализация синхронизации статусов
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingSupportingSyncOfOrderStatus
    {
        void SyncStatusOfOrder(Order order);
        void SyncStatusOfOrders(IEnumerable<Order> orders);

        bool SyncByAllOrders { get; }

        bool StatusesSync { get; }
    }

    /// <summary>
    /// Реализация предоставления информации доставкой о передвижении заказа
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingSupportingTheHistoryOfMovement
    {
        bool ActiveHistoryOfMovement { get; }
        List<HistoryOfMovement> GetHistoryOfMovement(Order order);
        PointInfo GetPointInfo(Order order);
    }

    public class HistoryOfMovement
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }

    public class PointInfo
    {
        public string Address { get; set; }
        public string TimeWork { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }
    }

    public interface IShippingLazyData
    {
        object GetLazyData(Dictionary<string, object> data);
    }

    /// <summary>
    /// Реализуется при необходимости выполнения фоновых задач
    /// <para>Задача запускается раз в час. Необходимость менее редкого выполнения контролируется самим методом доставки</para>
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    // internal - чтобы модули не имели возможности реализации (для них имеется IModuleTask)
    internal interface IShippingWithBackgroundMaintenance
    {
        void ExecuteJob();
    }

    /// <summary>
    /// Наследование интерфейса делает доступным метод для выбора в насройках платежки COD
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingSupportingPaymentCashOnDelivery { }


    /// <summary>
    /// Наследование интерфейса делает доступным метод для выбора в насройках платежки PickPoint
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingSupportingPaymentPickPoint { }

    /// <summary>
    /// Наследование интерфейса приводит к отключению функционала наценки
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingNoUseExtracharge { }

    /// <summary>
    /// Наследование интерфейса уведомляет об отсутсвии зависимости метода достаки от конкретной валюты
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingNoUseCurrency { }

    /// <summary>
    /// Наследование интерфейса уведомляет об отсутсвии поддержки увеличения времени доставки
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingNoUseExtraDeliveryTime { }

    public static class BaseShippingExtensions
    {
        public static string KeyAttribute(this BaseShipping obj)
        {
            return AttributeHelper.GetAttributeValue<ShippingKeyAttribute, string>(obj);
        }
    }
}