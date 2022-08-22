using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Statistic;
using AdvantShop.Taxes;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace AdvantShop.ExportImport.Excel
{
    public class ExcelExport
    {
        public const string templateSingleOrder = "~/App_Data/Reports/exportSingleOrder.xlsx";

        private static void RenderOrderItems(ExcelWorksheet worksheet, Order order)
        {
            var i = 1;
            foreach (OrderItem item in order.OrderItems)
            {
                //copy style
                if (i != 1)
                {
                    worksheet.InsertRow(22, 1, 21); //shift down
                                                    //worksheet.Row(19 + i).StyleID = worksheet.Row(20).StyleID;
                }

                var currentRow = i != 1 ? 22 : 21;
                worksheet.Cells[currentRow, 1].Value = item.ArtNo;
                worksheet.Cells[currentRow, 2].Value = item.Name;

                var html = new StringBuilder();
                if (item.Color.IsNotEmpty())
                    html.AppendFormat("{0}: {1}\n", SettingsCatalog.ColorsHeader, item.Color);
                if (item.Size.IsNotEmpty())
                    html.AppendFormat("{0}: {1}\n", SettingsCatalog.SizesHeader, item.Size);
                foreach (EvaluatedCustomOptions ev in item.SelectedOptions)
                    html.AppendFormat("- {0}: {1}\n", ev.CustomOptionTitle, ev.OptionTitle);
                worksheet.Cells[currentRow, 3].Value = html.ToString();

                worksheet.Cells[currentRow, 4].Value = (decimal)PriceService.SimpleRoundPrice(item.Price);
                worksheet.Cells[currentRow, 5].Value = order.OrderCurrency.CurrencySymbol.Trim();
                worksheet.Cells[currentRow, 6].Value = (decimal)item.Amount;
                worksheet.Cells[currentRow, 7].Value = (decimal)PriceService.SimpleRoundPrice(item.Price * item.Amount, order.OrderCurrency);
                worksheet.Cells[currentRow, 8].Value = order.OrderCurrency.CurrencySymbol.Trim();

                i++;
            }
        }

        public static void SingleOrder(string templatePath, string filename, Order order)
        {
            using (var excel = new ExcelPackage(new System.IO.FileInfo(templatePath)))
            {
                var worksheet = excel.Workbook.Worksheets.First();

                worksheet.Name = string.Format("{0} {1}", LocalizationService.GetResource("Core.ExportImport.ExcelSingleOrder.ItemNum"), order.Number);
                //title
                worksheet.Cells[1, 1].Value = string.Format("{0} {1}", LocalizationService.GetResource("Core.ExportImport.ExcelSingleOrder.ItemNum"), order.Number);

                //status
                worksheet.Cells[2, 1].Value = "(" + order.OrderStatus.StatusName + ")";

                // Date
                worksheet.Cells[4, 1].Value = LocalizationService.GetResource("Core.ExportImport.ExcelSingleOrder.Date");
                worksheet.Cells[4, 3].Value = Culture.ConvertDate(order.OrderDate);

                //StatusComment
                worksheet.Cells[5, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.StatusComment");
                worksheet.Cells[5, 3].Value = order.StatusComment;

                //Email
                worksheet.Cells[6, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Email");
                worksheet.Cells[6, 3].Value = order.OrderCustomer.Email;

                //Phone
                worksheet.Cells[7, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Phone");
                worksheet.Cells[7, 3].Value = order.OrderCustomer.Phone;

                worksheet.Cells[9, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Billing");
                worksheet.Cells[9, 3].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Shipping");
                //worksheet.Cells[9, 3].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.ShippingMethod");

                var customerName = order.OrderCustomer.GetFullName();

                worksheet.Cells[10, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.ContactName"), customerName);
                worksheet.Cells[10, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.ContactName"), customerName);

                var shippingMethodName = order.ArchivedShippingName;
                if (order.OrderPickPoint != null)
                    shippingMethodName += order.OrderPickPoint.PickPointAddress.Replace("<br/>", " ");
                //worksheet.Cells[10, 3].Value = "     " + shippingMethodName;


                worksheet.Cells[11, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Country"), order.OrderCustomer.Country);
                worksheet.Cells[11, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Country"), order.OrderCustomer.Country);
                //worksheet.Cells[11, 3].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.PaymentType");

                worksheet.Cells[12, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.City"), order.OrderCustomer.City);
                worksheet.Cells[12, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.City"), order.OrderCustomer.City);
                //worksheet.Cells[12, 3].Value = "     " + order.PaymentMethodName;

                worksheet.Cells[13, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Zone"), order.OrderCustomer.Region);
                worksheet.Cells[13, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Zone"), order.OrderCustomer.Region);


                worksheet.Cells[14, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Zip"), order.OrderCustomer.Zip);
                worksheet.Cells[14, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Zip"), order.OrderCustomer.Zip);

                worksheet.Cells[15, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Address"), order.OrderCustomer.GetCustomerAddress());
                worksheet.Cells[15, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Address"), order.OrderCustomer.GetCustomerAddress());

                worksheet.Cells[16, 1].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Organization"), order.OrderCustomer.Organization);
                worksheet.Cells[16, 3].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Organization"), order.OrderCustomer.Organization);

                if (order.PaymentDetails != null)
                {
                    worksheet.Cells[9, 4].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Payer");
                    worksheet.Cells[10, 4].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Company"), order.PaymentDetails.CompanyName);
                    worksheet.Cells[11, 4].Value = string.Format("     {0} {1}", LocalizationService.GetResource("Admin.Orders.OrderItem.Inn"), order.PaymentDetails.INN);
                }

                worksheet.Cells[18, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.OrderItem");

                worksheet.Cells[20, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Sku");
                worksheet.Cells[20, 2].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.ItemName");
                worksheet.Cells[20, 3].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.CustomOptions");
                worksheet.Cells[20, 4].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Price");
                worksheet.Cells[20, 5].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.ItemAmount");
                worksheet.Cells[20, 7].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.ItemCost");

                //productprice
                var currency = order.OrderCurrency;
                float productPrice = order.OrderItems.Sum(item => PriceService.SimpleRoundPrice(item.Amount * item.Price, currency));
                worksheet.Cells[22, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.ProductsPrice");
                worksheet.Cells[22, 7].Value = (decimal)productPrice;
                worksheet.Cells[22, 8].Value = currency.CurrencySymbol.Trim();

                int summaryRow = 23;
                int styleRow = 23;
                //totalsum
                worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.TotalPrice");
                worksheet.Cells[summaryRow, 7].Value = (decimal)order.Sum;
                worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();

                //comment
                worksheet.Cells[24, 1].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.CustomerComment");
                worksheet.Cells[25, 1].Value = order.CustomerComment;



                //if (order.PaymentCost > 0)
                //{
                //insert before summaryRow row with copy style from styleRow

                worksheet.InsertRow(summaryRow, 1, styleRow);
                if (order.ArchivedPaymentName.IsNotEmpty())
                    worksheet.Cells[summaryRow, 6].Value = "(" + order.ArchivedPaymentName + ")";

                worksheet.InsertRow(summaryRow, 1, styleRow);
                worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Core.ExportImport.ExcelSingleOrder.PaymentExtracharge");
                worksheet.Cells[summaryRow, 7].Value = (decimal)order.PaymentCost;
                worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();

                //}

                worksheet.InsertRow(summaryRow, 1, styleRow);
                if (shippingMethodName.IsNotEmpty())
                    worksheet.Cells[summaryRow, 6].Value = "(" + shippingMethodName + ")";

                worksheet.InsertRow(summaryRow, 1, styleRow);
                worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Core.ExportImport.ExcelSingleOrder.ShippingPrice");
                worksheet.Cells[summaryRow, 7].Value = (decimal)order.ShippingCost;
                worksheet.Cells[summaryRow, 8].Value = order.OrderCurrency.CurrencySymbol.Trim();

                var taxedItems = order.Taxes;
                if (taxedItems.Count > 0)
                {
                    foreach (var tax in taxedItems)
                    {
                        worksheet.InsertRow(summaryRow, 1, styleRow);
                        worksheet.Cells[summaryRow, 6].Value = (tax.ShowInPrice ? LocalizationService.GetResource("Core.Tax.IncludeTax") + " " : "") + tax.Name + ":";
                        worksheet.Cells[summaryRow, 7].Value = (decimal?)tax.Sum;
                        worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                    }
                }
                else
                {
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Taxes");
                    worksheet.Cells[summaryRow, 7].Value = 0;
                    worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                }

                float bonusPrice = order.BonusCost;
                if (bonusPrice > 0)
                {
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Bonuses");
                    worksheet.Cells[summaryRow, 7].Value = -1 * (decimal)bonusPrice;
                    worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                }

                if (order.Certificate != null)
                {
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Certificate");
                    worksheet.Cells[summaryRow, 7].Value = -1 * (decimal)order.Certificate.Price;
                    worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                }

                if (order.OrderDiscount != 0 || order.OrderDiscountValue != 0)
                {
                    var productsIgnoreDiscountPrice = order.OrderItems.Where(item => item.IgnoreOrderDiscount).Sum(item => item.Price * item.Amount);
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Discount");
                    worksheet.Cells[summaryRow, 7].Value = -1 * (decimal)PriceService.SimpleRoundPrice((productPrice - productsIgnoreDiscountPrice) / 100 * order.OrderDiscount + order.OrderDiscountValue);
                    worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                }

                if (order.Coupon != null)
                {
                    // coupon code
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    //insert before summaryRow row with copy style from styleRow
                    worksheet.InsertRow(summaryRow, 1, styleRow);
                    worksheet.Cells[summaryRow, 6].Value = LocalizationService.GetResource("Admin.Orders.OrderItem.Coupon");
                    var productsWithCoupon = order.OrderItems.Where(item => item.IsCouponApplied).Sum(item => item.Price * item.Amount);
                    switch (order.Coupon.Type)
                    {
                        case CouponType.Fixed:
                            worksheet.Cells[summaryRow, 7].Value = -1 * (decimal)PriceService.SimpleRoundPrice(order.Coupon.Value, currency);
                            worksheet.Cells[summaryRow + 1, 6].Value = string.Format("({0})", order.Coupon.Code);
                            break;
                        case CouponType.Percent:
                            worksheet.Cells[summaryRow, 7].Value = -1 * (decimal)PriceService.SimpleRoundPrice(productsWithCoupon * order.Coupon.Value / 100, currency);
                            worksheet.Cells[summaryRow + 1, 6].Value = string.Format("({0} ({1}%))", order.Coupon.Code, PriceFormatService.FormatPriceInvariant(order.Coupon.Value));
                            break;
                    }
                    worksheet.Cells[summaryRow, 8].Value = currency.CurrencySymbol.Trim();
                }

                RenderOrderItems(worksheet, order);

                excel.SaveAs(new FileInfo(filename));
            }
        }

        public static void MultiOrder(List<Order> orders, string filename, string encoding)
        {
            using (var streamWriter = new StreamWriter(filename, false, Encoding.GetEncoding(encoding)))
            using (var writer = new CsvWriter(streamWriter, new CsvConfiguration { Delimiter = ";" }))
            {
                // headers
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.OrderID"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Status"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.OrderDate"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.FIO"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.CustomerEmail"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.CustomerPhone"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.OrderedItems"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.BonusCost"));

                writer.WriteField(LocalizationService.GetResource("Скидка"));
                writer.WriteField(LocalizationService.GetResource("Стоимость доставки"));
                writer.WriteField(LocalizationService.GetResource("Наценка оплаты"));
                writer.WriteField(LocalizationService.GetResource("Купон"));

                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Total"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Currency"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Tax"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Cost"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Profit"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Payment"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Shipping"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.ShippingAddress"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.CustomerComment"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.AdminComment"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.StatusComment"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Payed"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.Manager"));
                writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.CouponCode"));
                writer.WriteField("Google client id");
                writer.WriteField("Yandex client id");
                writer.WriteField("Referral");
                writer.WriteField(LocalizationService.GetResource("Admin.Orders.Orderinfo.LoginPage"));
                writer.WriteField("UTM Source");
                writer.WriteField("UTM Medium");
                writer.WriteField("UTM Campaign");
                writer.WriteField("UTM Content");
                writer.WriteField("UTM Term");

                writer.NextRecord();

                foreach (var order in orders)
                {
                    if (!CommonStatistic.IsRun || CommonStatistic.IsBreaking)
                        return;

                    writer.WriteField(order.Number);
                    writer.WriteField(order.OrderStatus != null ? order.OrderStatus.StatusName : LocalizationService.GetResource("Core.ExportImport.MultiOrder.NullStatus"));
                    writer.WriteField(order.OrderDate.ToString("dd.MM.yyyy HH:mm:ss"));

                    if (order.OrderCustomer != null)
                    {
                        writer.WriteField(order.OrderCustomer.LastName + " " + order.OrderCustomer.FirstName);
                        writer.WriteField(order.OrderCustomer.Email ?? string.Empty);
                        writer.WriteField(order.OrderCustomer.Phone ?? string.Empty);
                    }
                    else
                    {
                        writer.WriteField(LocalizationService.GetResource("Core.ExportImport.MultiOrder.NullCustomer"));
                        writer.WriteField(string.Empty);
                        writer.WriteField(string.Empty);
                    }

                    if (order.OrderCurrency != null)
                    {
                        writer.WriteField(RenderOrderedItems(order.OrderItems, order.OrderCurrency) ?? string.Empty);
                        writer.WriteField(order.BonusCost);

                        writer.WriteField(order.GetOrderDiscountPrice());
                        writer.WriteField(PriceService.RoundPrice(order.ShippingCost, order.OrderCurrency));
                        writer.WriteField(PriceService.RoundPrice(order.PaymentCost, order.OrderCurrency));
                        writer.WriteField(order.GetOrderCouponPrice());

                        writer.WriteField(PriceService.RoundPrice(order.Sum, order.OrderCurrency));
                        writer.WriteField(order.OrderCurrency.CurrencySymbol);
                        writer.WriteField(PriceService.RoundPrice(order.TaxCost, order.OrderCurrency));
                        float totalCost = order.OrderItems.Sum(oi => oi.SupplyPrice * oi.Amount);
                        writer.WriteField(PriceService.RoundPrice(totalCost, order.OrderCurrency));
                        writer.WriteField(PriceService.RoundPrice(order.Sum - order.ShippingCost - order.TaxCost - totalCost, order.OrderCurrency));
                        writer.WriteField(order.PaymentMethodName);
                        writer.WriteField(order.ArchivedShippingName);
                        writer.WriteField(order.OrderCustomer != null
                            ? new List<string>
                            {
                                order.OrderCustomer.Zip,
                                order.OrderCustomer.Country,
                                order.OrderCustomer.Region,
                                order.OrderCustomer.City,
                                order.OrderCustomer.GetCustomerAddress(),
                                order.OrderCustomer.CustomField1,
                                order.OrderCustomer.CustomField2,
                                order.OrderCustomer.CustomField3,
                                order.OrderPickPoint != null ? order.OrderPickPoint.PickPointAddress : string.Empty
                            }.Where(s => s.IsNotEmpty()).AggregateString(", ")
                            : string.Empty);
                        writer.WriteField(order.CustomerComment ?? string.Empty);
                        writer.WriteField(order.AdminOrderComment ?? string.Empty);
                        writer.WriteField(order.StatusComment ?? string.Empty);
                        writer.WriteField(LocalizationService.GetResource(order.Payed ? "Admin.Yes" : "Admin.No"));
                        writer.WriteField(order.Manager!= null ? order.Manager.FullName : string.Empty);
                        writer.WriteField(order.Coupon != null ? order.Coupon.Code : string.Empty);

                        var orderTrafficSource = OrderTrafficSourceService.Get(order.OrderID, TrafficSourceType.Order);
                        if (orderTrafficSource != null)
                        {
                            writer.WriteField(orderTrafficSource.GoogleClientId ?? "");
                            writer.WriteField(orderTrafficSource.YandexClientId ?? "");
                            writer.WriteField(orderTrafficSource.Referrer ?? "");
                            writer.WriteField(orderTrafficSource.Url ?? "");
                            writer.WriteField(orderTrafficSource.utm_source ?? "");
                            writer.WriteField(orderTrafficSource.utm_medium ?? "");
                            writer.WriteField(orderTrafficSource.utm_campaign ?? "");
                            writer.WriteField(orderTrafficSource.utm_content ?? "");
                            writer.WriteField(orderTrafficSource.utm_term ?? "");
                        }
                    }

                    writer.NextRecord();
                    CommonStatistic.RowPosition++;
                }
            }
        }

        private static string RenderOrderedItems(IEnumerable<OrderItem> items, OrderCurrency orderCurrency)
        {
            var res = new StringBuilder();

            foreach (OrderItem orderItem in items)
            {
                res.AppendFormat("[{0} - {1} - {2}{3} - {4}{5}{6}], ", 
                    orderItem.ArtNo, 
                    orderItem.Name, 
                    PriceService.RoundPrice(orderItem.Price * orderItem.Amount, orderCurrency), 
                    orderCurrency.CurrencySymbol, 
                    orderItem.Amount,
                    LocalizationService.GetResource("Core.ExportImport.ExcelOrder.Pieces"),
                    RenderSelectedOptions(orderItem.SelectedOptions, orderItem.Color, orderItem.Size));
            }

            return res.ToString().TrimEnd(new[] { ',', ' ' });
        }

        private static string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist, string color, string size)
        {
            var html = string.Empty;
            if (evlist != null || !string.IsNullOrEmpty(color) || !string.IsNullOrEmpty(size))
            {
                if (!string.IsNullOrEmpty(color))
                    html = SettingsCatalog.ColorsHeader + ": " + color;

                if (!string.IsNullOrEmpty(size))
                    html += (!string.IsNullOrEmpty(html) ? ", " : "") + SettingsCatalog.SizesHeader + ": " + size;

                if (evlist != null)
                {
                    foreach (EvaluatedCustomOptions ev in evlist)
                        html += (!string.IsNullOrEmpty(html) ? ", " : "") + (string.Format("{0}: {1},", ev.CustomOptionTitle, ev.OptionTitle));
                }

                if (!string.IsNullOrEmpty(html))
                    html = " (" + html + ")";
            }

            return html;
        }
    }
}
