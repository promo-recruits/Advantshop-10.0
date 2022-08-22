﻿using System.Collections.Generic;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;

namespace AdvantShop.ViewModel.PaymentReceipt
{
    public class BillKzViewModel
    {
        public string Contract { get; set; }
        public string Kbe { get; set; }
        public string BinIin { get; set; }
        public string PayeesBank { get; set; }
        public string Bik { get; set; }
        public string Knp { get; set; }
        public string Iik { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string Vendor { get; set; }
        public string Buyer { get; set; }
        public string CompanyName { get; set; }
        public string Contractor { get; set; }
        public string PosContractor { get; set; }
        public string TotalKop { get; set; }

        public string StampImageName { get; set; }

        public string ShippingCost { get; set; }
        public string PaymentCost { get; set; }
        public string ProductsCost { get; set; }
        public string DiscountCost { get; set; }
        public string TotalCost { get; set; }
        public string TotalCostCurrency { get; set; }
        public string IntPartPrice { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public List<AdvantShop.Orders.GiftCertificate> OrderCertificates { get; set; }
        public List<OrderTax> Taxes { get; set; } 

        public OrderCurrency OrderCurrency { get; set; }
        public Currency RenderCurrency { get; set; }
    }
}