//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace AdvantShop.Mails
{
    public abstract class MailTemplate
    {
        public abstract MailType Type { get; }

        public string Subject { get; set; }
        public string Body { get; set; }
        protected bool IsBuilt { get; set; }

        public MailTemplate BuildMail()
        {
            if (!IsBuilt)
            {
                BuildMail(Type.ToString());
                IsBuilt = true;
            }
            return this;
        }

        public void BuildMail(string mailType)
        {
            var mailFormat = MailFormatService.GetByType(mailType);
            if (mailFormat != null)
                BuildMail(mailFormat.FormatSubject, mailFormat.FormatText);
            else
            {
                BuildMail("", "");
                Debug.Log.Warn("Mail type '" + mailType + "' not found");
            }
        }

        public void BuildMail(string subject, string mailBody)
        {
            var logo = SettingsMain.LogoImageName.IsNotEmpty()
                           ? String.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                                           SettingsMain.SiteUrl.Trim('/') + '/' +
                                           FoldersHelper.GetPathRelative(FolderType.Pictures, SettingsMain.LogoImageName, false),
                                           SettingsMain.ShopName)
                           : string.Empty;

            Body = !string.IsNullOrEmpty(mailBody)
                        ? FormatString(mailBody).Replace("#LOGO#", logo).Replace("#MAIN_PHONE#", SettingsMain.Phone)
                        : string.Empty;

            Subject = !string.IsNullOrEmpty(subject)
                        ? FormatString(subject)
                        : string.Empty;
        }

        public string FormatValue(string formatedStr)
        {
            if (string.IsNullOrEmpty(formatedStr))
                return string.Empty;

            var logo = SettingsMain.LogoImageName.IsNotEmpty()
                ? String.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                    SettingsMain.SiteUrl.Trim('/') + '/' +
                    FoldersHelper.GetPathRelative(FolderType.Pictures, SettingsMain.LogoImageName, false),
                    SettingsMain.ShopName)
                : string.Empty;

            var formatedString = string.Copy(formatedStr);

            return FormatString(formatedString).Replace("#LOGO#", logo).Replace("#MAIN_PHONE#", SettingsMain.Phone);
        }

        public string FormatValue(string formatedStr, Coupon couponTemplate, string triggerCouponCode)
        {
            var value = FormatValue(formatedStr);

            if (couponTemplate != null && value.Contains("#GENERATED_COUPON_CODE#"))
            {
                var generatedCoupon = CouponService.GenerateCoupon(couponTemplate);
                if (generatedCoupon != null)
                    value = value.Replace("#GENERATED_COUPON_CODE#", generatedCoupon.Code);
            }

            if (!string.IsNullOrEmpty(triggerCouponCode))
                value = value.Replace("#TRIGGER_COUPON#", triggerCouponCode);

            return value;
        }

        protected virtual string FormatString(string formatedStr)
        {
            return string.Empty;
        }
    }

    public class EmptyMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.None;}
        }

        public EmptyMailTemplate()
        {
            IsBuilt = true;
        }
    }

    public class RegistrationMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnRegistration; }
        }

        private readonly string _shopUrl;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _regDate;
        private readonly string _password;
        private readonly string _subsrcibe;
        private readonly string _customerEmail;
        private readonly string _phone;
        private readonly string _patronymic;

        public RegistrationMailTemplate(Customer customer)
        {
            _shopUrl = SettingsMain.SiteUrl;
            _firstName = customer.FirstName;
            _lastName = customer.LastName;
            _regDate= Localization.Culture.ConvertDate(DateTime.Now);
            _password = customer.Password;
            _subsrcibe = customer.SubscribedForNews
                ? LocalizationService.GetResource("User.Registration.Yes")
                : LocalizationService.GetResource("User.Registration.No");
            _customerEmail = customer.EMail;
            _phone = customer.Phone;
            _patronymic = SettingsCheckout.IsShowPatronymic ? customer.Patronymic : "";
        }

        public RegistrationMailTemplate(string shopUrl, string firstName, string lastName, string regDate,
                                        string password, string subsrcibe, string customerEmail, string phone, string patronymic)
        {
            _shopUrl = shopUrl;
            _firstName = firstName;
            _lastName = lastName;
            _regDate = regDate;
            _password = password;
            _subsrcibe = subsrcibe;
            _customerEmail = customerEmail;
            _phone = phone;
            _patronymic = patronymic;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#EMAIL#", _customerEmail);
            formatedStr = formatedStr.Replace("#FIRSTNAME#", _firstName);
            formatedStr = formatedStr.Replace("#LASTNAME#", _lastName);
            formatedStr = formatedStr.Replace("#PHONE#", _phone);
            formatedStr = formatedStr.Replace("#REGDATE#", _regDate);
            formatedStr = formatedStr.Replace("#PASSWORD#", _password);
            formatedStr = formatedStr.Replace("#SUBSRCIBE#", _subsrcibe);
            formatedStr = formatedStr.Replace("#SHOPURL#", _shopUrl);
            formatedStr = formatedStr.Replace("#PATRONYMIC#", _patronymic);

            return formatedStr;
        }
    }

    public class PwdRepairMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnPwdRepair; }
        }

        private readonly string _recoveryCode;
        private readonly string _email;
        private readonly string _link;

        public PwdRepairMailTemplate(string recoveryCode, string email, string link)
        {
            _recoveryCode = recoveryCode;
            _email = email;
            _link = link;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#EMAIL#", _email);
            formatedStr = formatedStr.Replace("#RECOVERYCODE#", _recoveryCode);
            formatedStr = formatedStr.Replace("#LINK#", _link);
            return formatedStr;
        }
    }

    public class NewOrderMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnNewOrder; }
        }

        private readonly string _customerContacts;
        private readonly string _shippingMethod;
        private readonly string _paymentType;
        private readonly string _orderTable;
        private readonly string _currentCurrencyCode;
        private readonly string _totalPrice;
        private readonly string _comments;
        private readonly string _email;
        private readonly string _number;
        private readonly string _orderCode;
        private readonly string _hash;
        private readonly string _firstName;
        private readonly string _lastName;

        private readonly string _managerName;
        private readonly string _inn;
        private readonly string _companyName;

        private readonly string _additionalCustomerFields;

        private readonly string _city;
        private readonly string _address;
        private string _payCode;
        private readonly int _orderId;

        public NewOrderMailTemplate(string number, string orderCode, string email, string customerContacts,
                                    string shippingMethod, string paymentType, string orderTable,
                                    string currentCurrencyCode, string totalPrice, string comments,
                                    string hash, string firstName, string lastName, string managerName, string inn, 
                                    string companyName, string additionalCustomerFields,
                                    string city, string address, string payCode, int orderId)
        {
            _number = number;
            _orderCode = orderCode;
            _email = email;
            _customerContacts = customerContacts;
            _shippingMethod = shippingMethod;
            _paymentType = paymentType;
            _orderTable = orderTable;
            _currentCurrencyCode = currentCurrencyCode;
            _totalPrice = totalPrice;
            _comments = comments;
            _hash = hash;
            _firstName = firstName;
            _lastName = lastName;
            _managerName = managerName;

            _inn = inn;
            _companyName = companyName;
            _additionalCustomerFields = additionalCustomerFields;
            _city = city;
            _address = address;
            _payCode = payCode;
            _orderId = orderId;
        }

        protected override string FormatString(string formatedStr)
        {
            var sb = new StringBuilder(formatedStr);

            sb.Replace("#ORDER_ID#", _number);
            sb.Replace("#NUMBER#", _number);
            sb.Replace("#EMAIL#", _email);
            sb.Replace("#CUSTOMERCONTACTS#", _customerContacts);
            sb.Replace("#SHIPPINGMETHOD#", _shippingMethod);
            sb.Replace("#PAYMENTTYPE#", _paymentType);
            sb.Replace("#ORDERTABLE#", _orderTable);
            sb.Replace("#CURRENTCURRENCYCODE#", _currentCurrencyCode);
            sb.Replace("#TOTALPRICE#", _totalPrice);
            sb.Replace("#COMMENTS#", _comments);
            sb.Replace("#BILLING_LINK#", SettingsMain.SiteUrl.Trim('/') + "/checkout/billing?code=" + _orderCode + "&hash=" + _hash);
            sb.Replace("#FIRSTNAME#", _firstName);
            sb.Replace("#LASTNAME#", _lastName);

            sb.Replace("#MANAGER_NAME#", _managerName);

            sb.Replace("#INN#", string.IsNullOrEmpty(_inn) ? string.Empty : LocalizationService.GetResource("Core.Payment.PaymentDetails.INN") + ": " + _inn);
            sb.Replace("#COMPANYNAME#", string.IsNullOrEmpty(_companyName) ? string.Empty : LocalizationService.GetResource("Core.Payment.PaymentDetails.CompanyName") + ": " + _companyName);
            sb.Replace("#ADDITIONALCUSTOMERFIELDS#", _additionalCustomerFields);

            sb.Replace("#CITY#", _city);
            sb.Replace("#ADDRESS#", _address);

            if (formatedStr.Contains("#BILLING_SHORTLINK#"))
            {
                if (string.IsNullOrEmpty(_payCode))
                    _payCode = OrderService.GeneratePayCode(_orderId);

                sb.Replace("#BILLING_SHORTLINK#", SettingsMain.SiteUrl.Trim('/') + "/pay/" + _payCode);
            }

            return sb.ToString();
        }
    }

    public class OrderStatusMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnChangeOrderStatus; }
        }

        private readonly string _orderStatus;
        private readonly string _statusComment;
        private readonly string _number;
        private readonly string _orderTable;
        private readonly string _trackNumber;
        private readonly string _managerName;
        private readonly string _shipppingMethod;
        private readonly string _paymentType;
        private readonly string _totalPrice;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _city;
        private readonly string _address;
        private readonly string _orderCode;
        private readonly string _hash;
        private string _payCode;
        private readonly int _orderId;

        public OrderStatusMailTemplate(Order order)
        {
            var orderItemsHtml = OrderService.GenerateOrderItemsHtml(order.OrderItems, order.OrderCurrency,
                order.OrderItems.Sum(oi => oi.Price * oi.Amount),
                order.OrderDiscount, order.OrderDiscountValue,
                order.Coupon, order.Certificate,
                order.TotalDiscount,
                order.ShippingCost, order.PaymentCost,
                order.TaxCost,
                order.BonusCost,
                0);

            _orderStatus = order.OrderStatus.StatusName;
            _statusComment = order.StatusComment.Replace("\r\n", "<br />");
            _number = order.Number;
            _orderTable = orderItemsHtml;
            _trackNumber = !string.IsNullOrEmpty(order.TrackNumber) ? order.TrackNumber : string.Empty;
            _managerName = order.Manager != null ? order.Manager.FullName : string.Empty;

            _shipppingMethod = order.ArchivedShippingName +
                               (order.OrderPickPoint != null ? " " + order.OrderPickPoint.PickPointAddress : "");

            _paymentType = order.PaymentMethodName;
            _totalPrice = order.Sum.ToString();
            _firstName = order.OrderCustomer.FirstName;
            _lastName = order.OrderCustomer.LastName;
            _city = order.OrderCustomer.City;
            _address = order.OrderCustomer.GetCustomerAddress();

            _orderCode = order.Code.ToString();
            _hash = OrderService.GetBillingLinkHash(order);
            _payCode = order.PayCode;
            _orderId = order.OrderID;
        }

        public OrderStatusMailTemplate(string orderStatus, string statusComment, string number, string orderTable,
                                       string trackNumber, string managerName, string shipppingMethod,
                                       string orderCode, string hash, string payCode, int orderId)
        {
            _orderStatus = orderStatus;
            _statusComment = statusComment;
            _number = number;
            _orderTable = orderTable;
            _trackNumber = trackNumber;
            _managerName = managerName;
            _shipppingMethod = shipppingMethod;
            _orderCode = orderCode;
            _hash = hash;
            _payCode = payCode;
            _orderId = orderId;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDERID#", _number);
            formatedStr = formatedStr.Replace("#ORDERSTATUS#", _orderStatus);
            formatedStr = formatedStr.Replace("#STATUSCOMMENT#", _statusComment);
            formatedStr = formatedStr.Replace("#NUMBER#", _number);
            formatedStr = formatedStr.Replace("#ORDERTABLE#", _orderTable);
            formatedStr = formatedStr.Replace("#TRACKNUMBER#", _trackNumber);
            formatedStr = formatedStr.Replace("#MANAGER_NAME#", _managerName);
            formatedStr = formatedStr.Replace("#SHIPPINGMETHOD#", _shipppingMethod);
            formatedStr = formatedStr.Replace("#PAYMENTTYPE#", _paymentType);
            formatedStr = formatedStr.Replace("#TOTALPRICE#", _totalPrice);
            formatedStr = formatedStr.Replace("#FIRSTNAME#", _firstName);
            formatedStr = formatedStr.Replace("#LASTNAME#", _lastName);
            formatedStr = formatedStr.Replace("#CITY#", _city);
            formatedStr = formatedStr.Replace("#ADDRESS#", _address);

            formatedStr = formatedStr.Replace("#BILLING_LINK#", SettingsMain.SiteUrl + "/checkout/billing?code=" + _orderCode + "&hash=" + _hash);

            if (formatedStr.Contains("#BILLING_SHORTLINK#"))
            {
                if (string.IsNullOrEmpty(_payCode))
                    _payCode = OrderService.GeneratePayCode(_orderId);

                formatedStr = formatedStr.Replace("#BILLING_SHORTLINK#", SettingsMain.SiteUrl + "/pay/" + _payCode);
            }

            return formatedStr;
        }
    }

    public class FeedbackMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnFeedback; }
        }

        private readonly string _shopUrl;
        private readonly string _shopName;
        private readonly string _userName;
        private readonly string _userEmail;
        private readonly string _userPhone;
        private readonly string _subjectMessage;
        private readonly string _textMessage;
        private readonly string _orderNumber;

        public FeedbackMailTemplate(string shopUrl, string shopName, string userName, string userEmail,
                                    string userPhone, string subjectMessage, string textMessage, string orderNumber)
        {
            _shopUrl = shopUrl;
            _shopName = shopName;
            _userName = userName;
            _userEmail = userEmail;
            _userPhone = userPhone;
            _subjectMessage = subjectMessage;
            _textMessage = textMessage;
            _orderNumber = orderNumber;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#SHOPURL#", _shopUrl);
            formatedStr = formatedStr.Replace("#STORE_NAME#", _shopName);
            formatedStr = formatedStr.Replace("#USERNAME#", _userName);
            formatedStr = formatedStr.Replace("#USEREMAIL#", _userEmail);
            formatedStr = formatedStr.Replace("#USERPHONE#", _userPhone);
            formatedStr = formatedStr.Replace("#SUBJECTMESSAGE#", _subjectMessage);
            formatedStr = formatedStr.Replace("#TEXTMESSAGE#", _textMessage);
            formatedStr = formatedStr.Replace("#ORDERNUMBER#", _orderNumber);
            return formatedStr;
        }
    }

    public class ProductDiscussMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnProductDiscuss; }
        }

        private readonly string _sku;
        private readonly string _productName;
        private readonly string _productLink;
        private readonly string _author;
        private readonly string _date;
        private readonly string _text;
        private readonly string _deleteLink;
        private readonly string _email;

        public ProductDiscussMailTemplate(string sku, string productName, string productLink, string author, string date,
                                          string text, string deleteLink, string email)
        {
            _sku = sku;
            _productName = productName;
            _productLink = productLink;
            _author = author;
            _date = date;
            _text = text;
            _deleteLink = deleteLink;
            _email = email;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#PRODUCTNAME#", _productName);
            formatedStr = formatedStr.Replace("#PRODUCTLINK#", _productLink);
            formatedStr = formatedStr.Replace("#USERMAIL#", _email);
            formatedStr = formatedStr.Replace("#SKU#", _sku);
            formatedStr = formatedStr.Replace("#AUTHOR#", _author);
            formatedStr = formatedStr.Replace("#DATE#", _date);
            formatedStr = formatedStr.Replace("#DELETELINK#", _deleteLink);
            formatedStr = formatedStr.Replace("#TEXT#", _text);
            return formatedStr;
        }
    }

    public class ProductDiscussAnswerMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnProductDiscussAnswer; }
        }

        private readonly string _sku;
        private readonly string _productName;
        private readonly string _productLink;
        private readonly string _author;
        private readonly string _date;
        private readonly string _previousMsgText;
        private readonly string _answerText;

        public ProductDiscussAnswerMailTemplate(string sku, string productName, string productLink, string author, string date,
                                          string previousMsgText, string answerText)
        {
            _sku = sku;
            _productName = productName;
            _productLink = productLink;
            _author = author;
            _date = date;
            _previousMsgText = previousMsgText;
            _answerText = answerText;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#PRODUCTNAME#", _productName);
            formatedStr = formatedStr.Replace("#PRODUCTLINK#", _productLink);
            formatedStr = formatedStr.Replace("#SKU#", _sku);
            formatedStr = formatedStr.Replace("#AUTHOR#", _author);
            formatedStr = formatedStr.Replace("#DATE#", _date);
            formatedStr = formatedStr.Replace("#ANSWER_TEXT#", _answerText);
            formatedStr = formatedStr.Replace("#PREVIOUS_MSG_TEXT#", _previousMsgText);
            return formatedStr;
        }
    }

    public class OrderByRequestMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnOrderByRequest; }
        }

        private readonly string _orderByRequestId;
        private readonly string _artNo;
        private readonly string _productName;
        private readonly string _quantity;
        private readonly string _userName;
        private readonly string _email;
        private readonly string _phone;
        private readonly string _comment;

        private readonly string _color;
        private readonly string _size;
        private readonly string _options;

        public OrderByRequestMailTemplate(string orderByRequestId, string artNo, string productName, string quantity,
                                          string userName, string email, string phone, string comment, string color,
                                          string size, string options)
        {
            _orderByRequestId = orderByRequestId;
            _artNo = artNo;
            _productName = productName;
            _quantity = quantity;
            _userName = userName;
            _email = email;
            _phone = phone;
            _comment = comment;
            _color = color;
            _size = size;
            _options = options;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDERID#", _orderByRequestId);
            formatedStr = formatedStr.Replace("#ARTNO#", _artNo);
            formatedStr = formatedStr.Replace("#PRODUCTNAME#", _productName);
            formatedStr = formatedStr.Replace("#QUANTITY#", _quantity);
            formatedStr = formatedStr.Replace("#USERNAME#", _userName);
            formatedStr = formatedStr.Replace("#EMAIL#", _email);
            formatedStr = formatedStr.Replace("#PHONE#", _phone);
            formatedStr = formatedStr.Replace("#COMMENT#", _comment);

            formatedStr = formatedStr.Replace("#COLOR#", _color);
            formatedStr = formatedStr.Replace("#SIZE#", _size);
            formatedStr = formatedStr.Replace("#OPTIONS#", _options);
            return formatedStr;
        }
    }

    public class LinkByRequestMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnSendLinkByRequest; }
        }

        private readonly string _orderByRequestId;
        private readonly string _userName;
        private readonly string _artNo;
        private readonly string _productName;
        private readonly string _quantity;
        private readonly string _code;
        private readonly string _color;
        private readonly string _size;
        private readonly string _comment;

        public LinkByRequestMailTemplate(string orderByRequestId, string artNo, string productName, string quantity,
                                             string code, string userName, string comment, string color, string size)
        {
            _orderByRequestId = orderByRequestId;
            _artNo = artNo;
            _productName = productName;
            _quantity = quantity;
            _userName = userName;
            _comment = comment;
            _color = color;
            _size = size;
            _code = code;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#NUMBER#", _orderByRequestId);
            formatedStr = formatedStr.Replace("#USERNAME#", _userName);
            formatedStr = formatedStr.Replace("#ARTNO#", _artNo);
            formatedStr = formatedStr.Replace("#PRODUCTNAME#", _productName);
            formatedStr = formatedStr.Replace("#QUANTITY#", _quantity);
            formatedStr = formatedStr.Replace("#LINK#", SettingsMain.SiteUrl + "/preorder/linkbycode?code=" + _code);

            formatedStr = formatedStr.Replace("#COLOR#", _color);
            formatedStr = formatedStr.Replace("#SIZE#", _size);

            formatedStr = formatedStr.Replace("#COMMENT#", _comment);

            return formatedStr;
        }
    }

    public class FailureByRequestMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnSendFailureByRequest; }
        }

        private readonly string _orderByRequestId;
        private readonly string _userName;
        private readonly string _artNo;
        private readonly string _productName;
        private readonly string _quantity;
        private readonly string _color;
        private readonly string _size;
        private readonly string _comment;

        public FailureByRequestMailTemplate(string orderByRequestId, string artNo, string productName,
                                                string quantity, string userName, string comment, string color, string size)
        {
            _orderByRequestId = orderByRequestId;
            _artNo = artNo;
            _productName = productName;
            _quantity = quantity;
            _userName = userName;
            _comment = comment;
            _color = color;
            _size = size;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#NUMBER#", _orderByRequestId);
            formatedStr = formatedStr.Replace("#USERNAME#", _userName);
            formatedStr = formatedStr.Replace("#ARTNO#", _artNo);
            formatedStr = formatedStr.Replace("#PRODUCTNAME#", _productName);
            formatedStr = formatedStr.Replace("#QUANTITY#", _quantity);

            formatedStr = formatedStr.Replace("#COLOR#", _color);
            formatedStr = formatedStr.Replace("#SIZE#", _size);
            formatedStr = formatedStr.Replace("#COMMENT#", _comment);

            return formatedStr;
        }
    }

    public class CertificateMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnSendGiftCertificate; }
        }

        private readonly string _certificateCode;
        private readonly string _fromName;
        private readonly string _toName;
        private readonly string _sum;
        private readonly string _message;

        public CertificateMailTemplate(string certificateCode, string fromName, string toName, string sum,
                                       string message)
        {
            _certificateCode = certificateCode;
            _fromName = fromName;
            _toName = toName;
            _sum = sum;
            _message = message;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#CODE#", _certificateCode);
            formatedStr = formatedStr.Replace("#FROMNAME#", _fromName);
            formatedStr = formatedStr.Replace("#TONAME#", _toName);
            formatedStr = formatedStr.Replace("#LINK#", StringHelper.ToPuny(SettingsMain.SiteUrl));
            formatedStr = formatedStr.Replace("#SUM#", _sum);
            formatedStr = formatedStr.Replace("#MESSAGE#", _message);

            return formatedStr;
        }
    }

    public class BuyInOneClickMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnBuyInOneClick; }
        }

        private readonly string _number;
        private readonly string _orderCode;
        private readonly string _name;
        private readonly string _phone;
        private readonly string _email;
        private readonly string _comment;
        private readonly string _orderTable;
        private readonly string _hash;
        private readonly string _managerName;

        public BuyInOneClickMailTemplate(string number, string orderCode, string name, string phone, string comment,
                                         string orderTable, string hash, string managerName, string email = "")
        {
            _number = number;
            _orderCode = orderCode;
            _name = name;
            _phone = phone;
            _email = email;
            _comment = comment;
            _orderTable = orderTable;
            _hash = hash;
            _managerName = managerName;
        }

        public BuyInOneClickMailTemplate(Order order, string orderTable)
        {
            _number = order.Number;
            _orderCode = order.Code.ToString();
            _name = order.OrderCustomer.FirstName;
            _phone = order.OrderCustomer.Phone;
            _comment = order.CustomerComment;
            _orderTable = orderTable;
            _hash = OrderService.GetBillingLinkHash(order);
            _managerName = order.Manager != null ? order.Manager.FullName : "";
            _email = order.OrderCustomer.Email;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDER_ID#", _number);
            formatedStr = formatedStr.Replace("#NUMBER#", _number);
            formatedStr = formatedStr.Replace("#NAME#", _name);
            formatedStr = formatedStr.Replace("#COMMENTS#", _comment);
            formatedStr = formatedStr.Replace("#PHONE#", _phone);
            formatedStr = formatedStr.Replace("#EMAIL#", _email);
            formatedStr = formatedStr.Replace("#ORDERTABLE#", _orderTable);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);
            formatedStr = formatedStr.Replace("#BILLING_LINK#",
                SettingsMain.SiteUrl + "/checkout/billing?code=" + _orderCode + "&hash=" + _hash);
            formatedStr = formatedStr.Replace("#MANAGER_NAME#", _managerName);

            return formatedStr;
        }
    }

    public class PreorderMailTemplate : BuyInOneClickMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnPreOrder; }
        }

        public PreorderMailTemplate(string number, string orderCode, string name, string phone, string comment,
                                    string orderTable, string hash, string managerName, string email = "")
            : base(number, orderCode, name, phone, comment, orderTable, hash, managerName, email)
        {
        }

        public PreorderMailTemplate(Order order, string orderTable) : base(order, orderTable)
        {
        }
    }

    public class BillingLinkMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnBillingLink; }
        }

        private readonly int _orderId;
        private readonly string _firstName;
        private readonly string _hash;
        private readonly string _comment;
        private readonly string _orderTable;
        private readonly string _customerContacts;
        private readonly string _orderNumber;
        private readonly string _orderCode;
        private readonly string _managerName;
        private readonly string _shippingMethod;
        private string _payCode;


        public BillingLinkMailTemplate(int orderId, string orderNumber, string orderCode, string firstName,
                                       string customerContacts, string hash, string comment, string orderTable, 
                                       string managerName, string payCode, string shippingMethod)
        {
            _orderId = orderId;
            _orderNumber = orderNumber;
            _orderCode = orderCode;
            _firstName = firstName;
            _hash = hash;
            _comment = comment;
            _orderTable = orderTable;
            _customerContacts = customerContacts;
            _managerName = managerName;
            _payCode = payCode;
            _shippingMethod = shippingMethod;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDER_ID#", _orderNumber);
            formatedStr = formatedStr.Replace("#FIRSTNAME#", _firstName);
            formatedStr = formatedStr.Replace("#COMMENTS#", _comment);
            formatedStr = formatedStr.Replace("#BILLING_LINK#",
                                                SettingsMain.SiteUrl + "/checkout/billing?code=" + _orderCode + "&hash=" + _hash);
            formatedStr = formatedStr.Replace("#ORDERTABLE#", _orderTable);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);
            formatedStr = formatedStr.Replace("#CUSTOMERCONTACTS#", _customerContacts);
            formatedStr = formatedStr.Replace("#MANAGER_NAME#", _managerName);
            formatedStr = formatedStr.Replace("#SHIPPINGMETHOD#", _shippingMethod);

            if (formatedStr.Contains("#BILLING_SHORTLINK#"))
            {
                if (string.IsNullOrEmpty(_payCode))
                    _payCode = OrderService.GeneratePayCode(_orderId);

                formatedStr = formatedStr.Replace("#BILLING_SHORTLINK#", SettingsMain.SiteUrl + "/pay/" + _payCode);
            }

            return formatedStr;
        }
    }

    public class SetOrderManagerMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnSetOrderManager; }
        }

        private readonly string _managerName;
        private readonly int _orderId;

        public SetOrderManagerMailTemplate(string managerName, int orderId)
        {
            _managerName = managerName;
            _orderId = orderId;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#SHOPURL#", SettingsMain.SiteUrl);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);
            formatedStr = formatedStr.Replace("#MANAGER_NAME#", _managerName);
            formatedStr = formatedStr.Replace("#ORDER_ID#", _orderId.ToString());
            formatedStr = formatedStr.Replace("#ORDER_URL#", UrlService.GetAdminUrl("orders/edit/" + _orderId));
            return formatedStr;
        }
    }

    public class ChangeUserCommentTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnChangeUserComment; }
        }

        private readonly string _orderId;
        private readonly string _orderUserComment;
        private readonly string _number;
        private readonly string _managerName;


        public ChangeUserCommentTemplate(string orderId, string orderUserComment, string number, string managerName)
        {
            _orderId = orderId;
            _orderUserComment = orderUserComment;
            _number = number;
            _managerName = managerName;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDER_ID#", _orderId);
            formatedStr = formatedStr.Replace("#ORDER_USER_COMMENT#", _orderUserComment);
            formatedStr = formatedStr.Replace("#NUMBER#", _number);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);
            formatedStr = formatedStr.Replace("#MANAGER_NAME#", _managerName);

            return formatedStr;
        }
    }

    public class PayOrderTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnPayOrder; }
        }

        private readonly string _orderId;
        private readonly string _pay;
        private readonly string _number;
        private readonly string _sum;
        private readonly string _managerName;

        private readonly string _shippingMethod;
        private readonly string _paymentType;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _city;
        private readonly string _address;

        public PayOrderTemplate(Order order, bool pay)
        {
            _orderId = order.OrderID.ToString();
            _number = order.Number;
            _pay = pay
                ? LocalizationService.GetResource("Core.Orders.Order.PaySpend").ToLower()
                : LocalizationService.GetResource("Core.Orders.Order.PayCancel").ToLower();
            
            _sum = order.Sum.ToString();
            _managerName = order.Manager != null ? order.Manager.FullName : "";

            _shippingMethod = order.ArchivedShippingName +
                               (order.OrderPickPoint != null ? " " + order.OrderPickPoint.PickPointAddress : "");
            _paymentType = order.PaymentMethodName;

            if (order.OrderCustomer != null)
            {
                _firstName = order.OrderCustomer.FirstName;
                _lastName = order.OrderCustomer.LastName;
                _city = order.OrderCustomer.City;
                _address = order.OrderCustomer.GetCustomerAddress();
            }
        }

        public PayOrderTemplate(string orderId, string number, string pay, string sum, string managerName)
        {
            _orderId = orderId;
            _pay = pay;
            _number = number;
            _sum = sum;
            _managerName = managerName;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#ORDER_ID#", _orderId);
            formatedStr = formatedStr.Replace("#PAY_STATUS#", _pay);
            formatedStr = formatedStr.Replace("#NUMBER#", _number);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);
            formatedStr = formatedStr.Replace("#SUM#", _sum);
            formatedStr = formatedStr.Replace("#MANAGER_NAME#", _managerName);

            formatedStr = formatedStr.Replace("#SHIPPINGMETHOD#", _shippingMethod);
            formatedStr = formatedStr.Replace("#PAYMENTTYPE#", _paymentType);
            formatedStr = formatedStr.Replace("#FIRSTNAME#", _firstName);
            formatedStr = formatedStr.Replace("#LASTNAME#", _lastName);
            formatedStr = formatedStr.Replace("#CITY#", _city);
            formatedStr = formatedStr.Replace("#ADDRESS#", _address);

            return formatedStr;
        }
    }

    public class SendToCustomerTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnSendToCustomer; }
        }

        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _patronymic;
        private readonly string _text;
        private readonly string _trackNumber;
        private readonly string _managerName;


        public SendToCustomerTemplate(string firstName, string lastName, string patronymic, string text, string trackNumber, string managerName)
        {
            _firstName = firstName;
            _lastName = lastName;
            _patronymic = patronymic;
            _text = text;
            _trackNumber = trackNumber;
            _managerName = managerName;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#TEXT#", _text);
            formatedStr = formatedStr.Replace("#FIRSTNAME#", _firstName);
            formatedStr = formatedStr.Replace("#LASTNAME#", _lastName);
            formatedStr = formatedStr.Replace("#PATRONYMIC#", _patronymic);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);
            formatedStr = formatedStr.Replace("#TRACKNUMBER#", _trackNumber);
            formatedStr = formatedStr.Replace("#MANAGER_NAME#", _managerName);
            return formatedStr;
        }
    }

    public class UserRegisteredMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnUserRegistered; }
        }

        private readonly string _email;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _regDate;
        private readonly string _hash;

        public UserRegisteredMailTemplate(string email, string firstName, string lastName, string regDate, string hash)
        {
            _email = email;
            _firstName = firstName;
            _lastName = lastName;
            _regDate = regDate;
            _hash = hash;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#EMAIL#", _email);
            formatedStr = formatedStr.Replace("#FIRSTNAME#", _firstName);
            formatedStr = formatedStr.Replace("#LASTNAME#", _lastName);
            formatedStr = formatedStr.Replace("#REGDATE#", _regDate);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);
            formatedStr = formatedStr.Replace("#STORE_URL#", SettingsMain.SiteUrl);
            formatedStr = formatedStr.Replace("#LINK#", UrlService.GetAdminUrl("account/setpassword?email=" + _email + "&hash=" + _hash));
            return formatedStr;
        }
    }

    public class UserPasswordRepairMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnUserPasswordRepair; }
        }

        private readonly string _email;
        private readonly string _hash;

        public UserPasswordRepairMailTemplate(string email, string hash)
        {
            _email = email;
            _hash = hash;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#EMAIL#", _email);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);
            formatedStr = formatedStr.Replace("#STORE_URL#", SettingsMain.SiteUrl);
            formatedStr = formatedStr.Replace("#LINK#", UrlService.GetAdminUrl("account/forgotpassword?email=" + _email + "&hash=" + _hash));
            return formatedStr;
        }
    }

    public class OrderCommentAddedMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnOrderCommentAdded; }
        }

        private readonly string _author;
        private readonly string _comment;
        private readonly string _orderLink;
        private readonly string _orderNumber;

        public OrderCommentAddedMailTemplate(string author, string comment, string orderId, string orderNumber)
        {
            _author = author;
            _comment = comment;
            _orderLink = orderId.IsNotEmpty() ? UrlService.GetAdminUrl("orders/edit/" + orderId) : string.Empty;
            _orderNumber = orderNumber;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#AUTHOR#", _author);
            formatedStr = formatedStr.Replace("#COMMENT#", _comment);
            formatedStr = formatedStr.Replace("#ORDER_LINK#", _orderLink);
            formatedStr = formatedStr.Replace("#ORDER_NUMBER#", _orderNumber);
            return formatedStr;
        }
    }

    public class CustomerCommentAddedMailTemplate : MailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnCustomerCommentAdded; }
        }

        private readonly string _author;
        private readonly string _comment;
        private readonly string _customerLink;
        private readonly string _customerName;

        public CustomerCommentAddedMailTemplate(string author, string comment, string customerId, string customerName)
        {
            _author = author;
            _comment = comment;
            _customerLink = customerId.IsNotEmpty() ? UrlService.GetAdminUrl("customers/view/" + customerId) : string.Empty;
            _customerName = customerName;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#AUTHOR#", _author);
            formatedStr = formatedStr.Replace("#COMMENT#", _comment);
            formatedStr = formatedStr.Replace("#CUSTOMER_LINK#", _customerLink);
            formatedStr = formatedStr.Replace("#CUSTOMER#", _customerName);
            return formatedStr;
        }
    }

    public class MissedCallMailTemplate : MailTemplate
    {
        private readonly string _phone;

        public override MailType Type
        {
            get { return MailType.OnMissedCall; }
        }

        public MissedCallMailTemplate(string phone)
        {
            _phone = phone;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#PHONE#", _phone);
            return formatedStr;
        }
    }
}