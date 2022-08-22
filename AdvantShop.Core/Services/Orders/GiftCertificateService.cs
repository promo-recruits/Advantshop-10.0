//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Security;

namespace AdvantShop.Orders
{
    public class GiftCertificateService
    {

        #region add, get, update, delete

        private static GiftCertificate GetFromReader(SqlDataReader reader)
        {
            return new GiftCertificate
            {
                CertificateId = SQLDataHelper.GetInt(reader, "CertificateID"),
                CertificateCode = SQLDataHelper.GetString(reader, "CertificateCode"),
                FromName = SQLDataHelper.GetString(reader, "FromName"),
                ToName = SQLDataHelper.GetString(reader, "ToName"),
                ApplyOrderNumber = SQLDataHelper.GetString(reader, "ApplyOrderNumber"),
                OrderId = SQLDataHelper.GetNullableInt(reader, "OrderID"),
                Sum = SQLDataHelper.GetFloat(reader, "Sum"),
                Used = SQLDataHelper.GetBoolean(reader, "Used"),
                Enable = SQLDataHelper.GetBoolean(reader, "Enable"),
                CertificateMessage = SQLDataHelper.GetString(reader, "Message"),
                ToEmail = SQLDataHelper.GetString(reader, "ToEmail"),
                CreationDate = SQLDataHelper.GetDateTime(reader, "CreationDate"),
            };
        }


        public static GiftCertificate GetCertificateById(int certificateId)
        {
            return SQLDataAccess.ExecuteReadOne<GiftCertificate>("[Order].[sp_GetCertificateById]",
                                                                                        CommandType.StoredProcedure, GetFromReader,
                                                                                        new SqlParameter("@CertificateID", certificateId));
        }
        public static GiftCertificate GetCertificateByCode(string certificateCode)
        {
            return SQLDataAccess.ExecuteReadOne<GiftCertificate>("[Order].[sp_GetCertificateByCode]",
                                                                                        CommandType.StoredProcedure, GetFromReader,
                                                                                        new SqlParameter("@CertificateCode", certificateCode));
        }

        public static List<GiftCertificate> GetOrderCertificates(int orderId)
        {
            return SQLDataAccess.ExecuteReadList<GiftCertificate>("[Order].[sp_GetOrderCertificates]",
                                                                                        CommandType.StoredProcedure, GetFromReader,
                                                                                        new SqlParameter("@OrderID", orderId));
        }


        public static int AddCertificate(GiftCertificate certificate)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Order].[sp_AddCertificate]", CommandType.StoredProcedure,
                                                      new SqlParameter("@CertificateCode", certificate.CertificateCode),
                                                      new SqlParameter("@ApplyOrderNumber", String.IsNullOrEmpty(certificate.ApplyOrderNumber) ? (object)DBNull.Value : certificate.ApplyOrderNumber),
                                                      new SqlParameter("@OrderID", certificate.OrderId == 0 ? (object)DBNull.Value : certificate.OrderId),
                                                      new SqlParameter("@FromName", certificate.FromName),
                                                      new SqlParameter("@ToName", certificate.ToName),
                                                      new SqlParameter("@Used", certificate.Used),
                                                      new SqlParameter("@Enable", certificate.Enable),
                                                      new SqlParameter("@Sum", certificate.Sum),
                                                      new SqlParameter("@Message", certificate.CertificateMessage),
                                                      new SqlParameter("@ToEmail", certificate.ToEmail)
                );
        }

        public static void UpdateCertificateById(GiftCertificate certificate)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateCertificateById]", CommandType.StoredProcedure,
                new SqlParameter("@CertificateId", certificate.CertificateId),
                new SqlParameter("@CertificateCode", certificate.CertificateCode),
                new SqlParameter("@OrderID", certificate.OrderId == 0 || !certificate.OrderId.HasValue ? (object)DBNull.Value : certificate.OrderId),
                new SqlParameter("@ApplyOrderNumber", String.IsNullOrEmpty(certificate.ApplyOrderNumber) ? (object)DBNull.Value : certificate.ApplyOrderNumber),
                new SqlParameter("@FromName", certificate.FromName),
                new SqlParameter("@ToName", certificate.ToName),
                new SqlParameter("@Used", certificate.Used),
                new SqlParameter("@Enable", certificate.Enable),
                new SqlParameter("@Sum", certificate.Sum),
                new SqlParameter("@Message", certificate.CertificateMessage),
                new SqlParameter("@ToEmail", certificate.ToEmail));
        }

        public static void DeleteCertificateById(int certificateId)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_DeleteCertificateById]", CommandType.StoredProcedure, new SqlParameter("@CertificateId", certificateId));
        }
        #endregion

        #region CustomerSertificate
        public static GiftCertificate GetCustomerCertificate()
        {
            return GetCustomerCertificate(CustomerContext.CustomerId);
        }

        public static GiftCertificate GetCustomerCertificate(Guid customerId)
        {
            var certificate = SQLDataAccess.ExecuteReadOne<GiftCertificate>
                ("Select * From [Order].[Certificate] Where CertificateID = (Select CertificateID From Customers.CustomerCertificate Where CustomerID = @CustomerID)",
                    CommandType.Text, GetFromReader,
                    new SqlParameter("@CustomerID", customerId));

            if (certificate != null)
            {
                if (certificate.Paid && !certificate.Used)
                    return certificate;

                DeleteCustomerCertificate(certificate.CertificateId);
                return null;
            }
            return null;
        }

        public static void DeleteCustomerCertificate(int certificateId)
        {
            DeleteCustomerCertificate(certificateId, CustomerContext.CustomerId);
        }

        public static void DeleteCustomerCertificate(int certificateId, Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Customers.CustomerCertificate Where CertificateID = @CertificateID and CustomerID = @CustomerID",
                CommandType.Text,
                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@CertificateID", certificateId));
        }


        public static void AddCustomerCertificate(int certificateId)
        {
            AddCustomerCertificate(certificateId, CustomerContext.CustomerId);
        }

        public static void AddCustomerCertificate(int certificateId, Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO Customers.CustomerCertificate (CustomerID, CertificateID) VALUES (@CustomerID, @CertificateID)",
                CommandType.Text,
                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@CertificateID", certificateId));
        }

        #endregion

        public static string GenerateCertificateCode()
        {
            var code = String.Empty;
            while (String.IsNullOrEmpty(code) || IsExistCertificateCode(code) || CouponService.IsExistCouponCode(code))
            {
                code = @"C-" + Strings.GetRandomString(8);
            }
            return code;
        }

        public static bool IsExistCertificateCode(string code)
        {
            return SQLDataHelper.GetInt(
                SQLDataAccess.ExecuteScalar(
                    "Select COUNT(CertificateID) From [Order].[Certificate] Where CertificateCode = @CertificateCode",
                    CommandType.Text,
                    new SqlParameter("@CertificateCode", code))) > 0;
        }

        public static List<GiftCertificate> GetCertificates()
        {
            List<GiftCertificate> certificates = SQLDataAccess.ExecuteReadList<GiftCertificate>("Select * From [Order].[Certificate]", CommandType.Text, GetFromReader);
            return certificates;
        }

        public static float GetCertificatePriceById(int id)
        {
            return SQLDataHelper.GetFloat(SQLDataAccess.ExecuteScalar("Select Sum From [Order].[Certificate] Where CertificateId = @CertificateId",
                                                                        CommandType.Text,
                                                                        new SqlParameter("@CertificateId", id)));
        }

        public static void SendCertificateMails(GiftCertificate certificate)
        {
            var certificateMailTemplate = new CertificateMailTemplate(certificate.CertificateCode, certificate.FromName,
                                                                      certificate.ToName,
                                                                      PriceFormatService.FormatPrice(certificate.Sum),
                                                                      certificate.CertificateMessage);

            MailService.SendMailNow(CustomerContext.CustomerId, certificate.ToEmail, certificateMailTemplate);
            MailService.SendMailNow(Guid.Empty, SettingsMail.EmailForOrders, certificateMailTemplate, replyTo: certificate.ToEmail);
        }

        public static void DeleteCertificatePaymentMethods()
        {
            SQLDataAccess.ExecuteNonQuery("Delete FROM [Settings].[GiftCertificatePayments]", CommandType.Text);
        }

        public static void SaveCertificatePaymentMethods(List<int> paymentMethodsIds)
        {
            DeleteCertificatePaymentMethods();

            foreach (var paymentMethodsId in paymentMethodsIds)
            {
                SQLDataAccess.ExecuteScalar(
                    "INSERT INTO [Settings].[GiftCertificatePayments] ([PaymentID]) VALUES (@PaymentID)",
                    CommandType.Text,
                    new SqlParameter("@PaymentID", paymentMethodsId));
            }
        }

        public static List<int> GetCertificatePaymentMethodsID()
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                "SELECT * FROM [Settings].[GiftCertificatePayments]",
                CommandType.Text, "PaymentID");
        }

        public static Order CreateCertificateOrder(GiftCertificateOrderModel model)
        {
            var certificate = model.GiftCertificate;
            var customer = CustomerContext.CurrentCustomer;

            if (!customer.RegistredUser)
            {
                var existCustomer = CustomerService.GetCustomerByEmail(model.EmailFrom);
                if (existCustomer != null)
                {
                    customer = existCustomer;
                }
                else
                {
                    var c = new Customer(CustomerGroupService.DefaultCustomerGroup)
                    {
                        Id = CustomerContext.CustomerId,
                        Password = StringHelper.GeneratePassword(8),
                        FirstName = model.GiftCertificate != null ? model.GiftCertificate.FromName : null,
                        EMail = model.EmailFrom,
                        CustomerRole = Role.User,
                        Phone = model.Phone,
                        StandardPhone = StringHelper.ConvertToStandardPhone(model.Phone)
                    };

                    CustomerService.InsertNewCustomer(c);

                    AuthorizeService.SignIn(c.EMail, c.Password, false, true);

                    customer = c;
                }
            }

            TaxElement tax;
            var taxValue = TaxService.CalculateCertificateTax(certificate.Sum, out tax);

            var taxOverPay = taxValue.HasValue && !tax.ShowInPrice ? taxValue.Value : 0f;

            var orderSum = certificate.Sum + taxOverPay;

            var payment = PaymentService.GetPaymentMethod(model.PaymentId);
            //var paymentPrice = payment.Extracharge == 0 ? 0 : (payment.ExtrachargeType == ExtrachargeType.Fixed ? payment.Extracharge : payment.Extracharge / 100 * certificate.Sum + taxOverPay);
            
            var paymentPrice = payment.GetExtracharge(orderSum);


            var currency = CurrencyService.CurrentCurrency;
            var orderSource = OrderSourceService.GetOrderSource(OrderType.ShoppingCart);

            var order = new Order
            {
                OrderDate = DateTime.Now,
                OrderCustomer = new OrderCustomer
                {
                    CustomerID = customer.Id,
                    Email = model.EmailFrom,
                    FirstName = customer.RegistredUser ? customer.FirstName : certificate.FromName,
                    LastName = customer.RegistredUser ? customer.LastName : string.Empty,
                    CustomerIP = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : string.Empty,
                    Phone = customer.Phone,
                    StandardPhone = customer.StandardPhone
                },
                GroupName = customer.CustomerGroup.GroupName,
                GroupDiscount = customer.CustomerGroup.GroupDiscount,
                OrderCurrency = currency,
                OrderStatusId = OrderStatusService.DefaultOrderStatus,
                AffiliateID = 0,
                ArchivedShippingName = LocalizationService.GetResource("Core.Orders.GiftCertificate.DeliveryByEmail"),
                PaymentMethodId = model.PaymentId,
                ArchivedPaymentName = payment.Name,
                PaymentDetails = null,
                Sum = orderSum + paymentPrice,
                PaymentCost = paymentPrice,
                OrderCertificates = new List<GiftCertificate>
                {
                    certificate
                },
                TaxCost = taxValue ?? 0f,
                Taxes = taxValue.HasValue 
                    ? new List<OrderTax> {new OrderTax { TaxId = tax.TaxId, Name = tax.Name, ShowInPrice = tax.ShowInPrice, Sum = taxValue.Value }}
                    : new List<OrderTax>(),
                OrderSourceId = orderSource.Id
            };

            order.PaymentDetails = new PaymentDetails() { Phone = model.Phone };

            var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);

            order.OrderID = OrderService.AddOrder(order, changedBy);

            OrderStatusService.ChangeOrderStatus(order.OrderID, OrderStatusService.DefaultOrderStatus,
                LocalizationService.GetResource("Core.Orders.GiftCertificate.Created"));

            var email = model.EmailFrom;

            var customerSb = new StringBuilder();
            const string format = "<div class='l-row'><div class='l-name vi cs-light' style='color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 65px;'>{0}:</div><div class='l-value vi' style='display: inline-block; margin: 5px 0;'>{1}</div></div>";
            customerSb.AppendFormat(format, "Email", model.EmailFrom);

            var htmlOrderTable = OrderService.GenerateCertificatesHtml(order.OrderCertificates, currency, order.PaymentCost, order.TaxCost);

            var orderMailTemplate = new NewOrderMailTemplate(order.Number, order.Code.ToString(), email,
                                                             customerSb.ToString(),
                                                             order.ArchivedShippingName,
                                                             order.ArchivedPaymentName,
                                                             htmlOrderTable,
                                                             currency.Iso3, order.Sum.ToString(),
                                                             order.CustomerComment,
                                                             OrderService.GetBillingLinkHash(order),
                                                             customer.FirstName,
                                                             customer.LastName,
                                                             order.Manager != null ? order.Manager.FullName : string.Empty,
                                                             order.PaymentDetails != null ? order.PaymentDetails.INN : string.Empty,
                                                             order.PaymentDetails != null ? order.PaymentDetails.CompanyName : string.Empty,
                                                             string.Empty,
                                                             order.OrderCustomer.City,
                                                             order.OrderCustomer.GetCustomerAddress(),
                                                             order.PayCode, order.OrderID);

            MailService.SendMailNow(CustomerContext.CustomerId, email, orderMailTemplate);
            MailService.SendMailNow(Guid.Empty, SettingsMail.EmailForOrders, orderMailTemplate, replyTo: email);

            return order;
        }
    }
}