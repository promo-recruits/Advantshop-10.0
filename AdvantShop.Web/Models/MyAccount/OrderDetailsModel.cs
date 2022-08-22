using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Payment;
using AdvantShop.Shipping;

namespace AdvantShop.Models.MyAccount
{
    public class OrderDetailsItemModel
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public float Amount { get; set; }
        public string ArtNo { get; set; }
        public int? Id { get; set; }
        public string TotalPrice { get; set; }
        public string Photo { get; set; }
        public string Url { get; set; }
        public string ColorHeader { get; set; }
        public string SizeHeader { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public List<EvaluatedCustomOptions> CustomOptions { get; set; }
        public float Width { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
    }

    public class OrderDetailsCertificateModel
    {
        public string Code { get; set; }
        public string Price { get; set; }
    }

    public class OrderDetailsPaymentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OrderDetailsModel
    {
        public string Email { get; set; }
        public int OrderID { get; set; }
        public string Number { get; set; }
        public Guid Code { get; set; }
        public string StatusName { get; set; }
        public List<OrderDetailsItemModel> OrderItems { get; set; }
        public List<OrderDetailsCertificateModel> OrderCertificates { get; set; }
        public CustomerContact BillingAddress { get; set; }
        public string ShippingName { get; set; }
        public CustomerContact ShippingInfo { get; set; }
        public string ArchivedShippingName { get; set; }
        public string ShippingAddress { get; set; }
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public string TotalDiscountPrice { get; set; }
        public string ProductsPrice { get; set; }
        public float TotalDiscount { get; set; }
        public string CertificatePrice { get; set; }
        public string TotalPrice { get; set; }
        public string Coupon{ get; set; }
        public string ShippingPrice { get; set; }
        public string PaymentPrice { get; set; }
        public string PaymentPriceText { get; set; }
        public string CustomerComment { get; set; }
        public List<OrderDetailsPaymentModel> Payments { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
        public bool Canceled { get; set; }
        public bool Payed { get; set; }
        public string Bonus { get; set; }
        public string TaxesNames { get; set; }
        public string TaxesPrice { get; set; }
        public string TrackNumber { get; set; }
        public bool StatusCancelForbidden { get; set; }
        public ShippingHistoryOfMovementInfo ShippingHistory { get; set; }
        public string OrderDate { get; set; }
    }

    public class ShippingHistoryOfMovementInfo
    {
        public List<HistoryOfMovementModel> HistoryOfMovement { get; set; }
        public PointInfoModel PointInfo { get; set; }
    }

    public class HistoryOfMovementModel
    {
        public static HistoryOfMovementModel CreateBy(HistoryOfMovement historyOfMovement)
        {
            return new HistoryOfMovementModel
            {
                Code = historyOfMovement.Code,
                Name = historyOfMovement.Name,
                Comment = historyOfMovement.Comment,
                Date = historyOfMovement.Date,
                DateString = historyOfMovement.Date.ToString(Configuration.SettingsMain.ShortDateFormat) + " " + historyOfMovement.Date.ToString("HH:mm"),
            };
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
    }

    public class PointInfoModel
    {
        public static PointInfoModel CreateBy(PointInfo pointInfo)
        {
            return new PointInfoModel
            {
                Address = pointInfo.Address,
                TimeWork = pointInfo.TimeWork,
                Phone = pointInfo.Phone,
                Comment = pointInfo.Comment,
            };
        }

        public string Address { get; set; }
        public string TimeWork { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }
    }
}