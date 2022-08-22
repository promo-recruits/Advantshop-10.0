using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.ExportImport.Excel;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Saas;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Marketing.Analytics;
using AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports;
using AdvantShop.Web.Admin.Models.Marketing.Analytics;
using AdvantShop.Web.Admin.ViewModels.Analytics;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Admin.Handlers.Analytics;

namespace AdvantShop.Web.Admin.Controllers.Marketing
{
    [Auth(RoleAction.Analytics)]
    [SaasFeature(ESaasProperty.DeepAnalytics)]
    public partial class AnalyticsController : BaseAdminController
    {
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public ActionResult Page()
        {
            return RedirectToAction(SaasDataService.IsEnabledFeature(ESaasProperty.DeepAnalytics) ? "Index" : "ExportProducts");
        }

        #region Analytics report

        public ActionResult Index()
        {
            var model = new AnalyticsReportModel();

            SetMetaInformation(T("Admin.Marketing.ConsolidatedError"));
            SetNgController(NgControllers.NgControllersTypes.AnalyticsReportCtrl);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Reports_ViewReports);

            return View(model);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetVortex(string datefrom, string dateto)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            var result = new VortexHandler(from, to).Execute();
            return Json(result);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetProfit(string type, string datefrom, string dateto, bool? paid, int? orderStatus, bool useShippingCost, string groupFormatString)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            object result = null;

            var handler = new OrderStatictsHandler(from, to, orderStatus, paid, useShippingCost, groupFormatString);
            switch (type)
            {
                case "sum":
                    result = handler.GetOrdersSum();
                    break;
                case "count":
                    result = handler.GetOrdersCount();
                    break;

            }
            return Json(result);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetAvgCheck(string type, string datefrom, string dateto, bool? paid, int? orderStatus, string groupFormatString)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            object result = null;

            var handler = new AvgCheckHandler(from, to, orderStatus, paid, groupFormatString);
            switch (type)
            {
                case "avg":
                    result = handler.GetAvgCheck();
                    break;
                case "city":
                    result = handler.GetAvgCheckByCity();
                    break;
            }

            return Json(result);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetOrders(string type, string datefrom, string dateto, bool? paid, int? orderStatus, string groupFormatString)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            object result = null;

            var handler = new OrdersByHandler(from, to, orderStatus, paid, groupFormatString);
            switch (type)
            {
                case "payments":
                    result = handler.GetPayments();
                    break;
                case "shippings":
                    result = handler.GetShippings();
                    break;
                case "shippingsGroupedByName":
                    result = handler.GetShippings(true);
                    break;
                case "statuses":
                    result = handler.GetStatuses();
                    break;
                case "sources":
                    result = handler.GetOrderTypes();
                    break;
                case "repeatorders":
                    result = handler.GetRepeatOrders();
                    break;
            }

            return Json(result);
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetTelephony(string type, string datefrom, string dateto, string groupFormatString)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            object result = null;

            var handler = new TelephonyHandler(from, to, groupFormatString);
            switch (type)
            {
                case "in":
                    result = handler.GetCallsCount(ECallType.In);
                    break;
                case "missed":
                    result = handler.GetCallsCount(ECallType.Missed);
                    break;
                case "out":
                    result = handler.GetCallsCount(ECallType.Out);
                    break;
                case "avgtime":
                    result = handler.GetAvgDuration();
                    break;
            }

            return Json(result);
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetRfm(string datefrom, string dateto)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            var handler = new RfmAnalysisHandler(from, to);
            return Json(handler.GetData());
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetRfmCommonData(string datefrom, string dateto)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            var handler = new RfmCommonDataHandler(from, to);
            return Json(handler.GetData());
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetManagers(string datefrom, string dateto)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            var result = new ManagersHandler(from, to).Execute();
            return Json(result);
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetAbcxyzAnalysis(string datefrom, string dateto)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            var result = new AbcxyzAnalysisHandler(from, to).Execute();
            return Json(result);
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetProductStatisticsName(int productId)
        {
            var p = ProductService.GetProduct(productId);
            return Json(new { name = p != null ? p.Name : "" });
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetProductStatistics(string type, string datefrom, string dateto, int productId, bool? paid, string groupFormatString)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            object result = null;

            var handler = new ProductStatisticsHandler(from, to, productId, paid, groupFormatString);
            switch (type)
            {
                case "sum":
                    result = handler.GetSum();
                    break;
                case "count":
                    result = handler.GetCount();
                    break;
            }

            return Json(result);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetProductStatisticsList(ProductOrdersStatisticsFilterModel filter)
        {
            return Json(new ProductOrdersStatistics(filter).Execute());
        }

        #endregion

        #region Analytics Filter

        [Auth(RoleAction.Catalog, RoleAction.Customers)]
        public ActionResult AnalyticsFilter(AnalyticsFilterModel data)
        {
            data.From = data.From.TryParseDateTime().ToString("dd-MM-yyyy");
            data.To = data.To.TryParseDateTime().ToString("dd-MM-yyyy");

            SetMetaInformation(T("Admin.Marketing.ConsolidatedError"));
            SetNgController(NgControllers.NgControllersTypes.AnalyticsFilterCtrl);

            return View(data);
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetAnalyticsFilterAbcxyz(AnalyticsFilterModel data, BaseFilterModel filter)
        {
            var from = data.From.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = data.To.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);
            
            return Json(new GetAnalyticsFilterAbcxyz(filter, from, to, data.Group).Execute());
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetAnalyticsFilterRfm(AnalyticsFilterModel data, BaseFilterModel filter)
        {
            var from = data.From.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = data.To.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);
            
            return Json(new GetAnalyticsFilterRfm(new BaseFilterModel(), data.Group, from, to).Execute());
        }

        #endregion

        #region ExportOrders

        [Auth(RoleAction.Orders)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public ActionResult ExportOrders()
        {
            SetNgController(NgControllers.NgControllersTypes.AnalyticsCtrl);
            SetMetaInformation(T("Admin.Marketing.Analytics.ExportOrders"));

            var model = new GetExportOrdersModel().Execute();
            return View(model);
        }

        [Auth(RoleAction.Orders)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public JsonResult GetExportOrdersSettings()
        {
            var model = new ExportOrdersModel();

            var encodings = new Dictionary<string, string>();
            foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
            {
                encodings.Add(enumItem.StrName(), enumItem.StrName());
            }

            var orderStatuses = new Dictionary<int, string>();
            foreach (var status in OrderStatusService.GetOrderStatuses())
            {
                orderStatuses.Add(status.StatusID, status.StatusName);
            }

            model.PaidStatuses = new Dictionary<bool, string>
            {
                {true, T("Admin.Marketing.Paid") },
                {false, T("Admin.Marketing.NotPaid") }
            };

            model.Shippings = new Dictionary<int, string>();
            foreach (var shipping in ShippingMethodService.GetAllShippingMethods())
            {
                model.Shippings.Add(shipping.ShippingMethodId, shipping.Name);
            }

            model.Shipping = model.Shippings.Any() ? model.Shippings.ElementAt(0).Key : 0;

            model.Paid = true;

            model.Encoding = EncodingsEnum.Windows1251.StrName();
            model.Encodings = encodings;
            model.OrderStatuses = orderStatuses;
            model.Status = orderStatuses != null && orderStatuses.Count > 0 ? orderStatuses.FirstOrDefault().Key : 0;
            model.DateFrom = DateTime.Now.AddMonths(-3);
            model.DateTo = DateTime.Now;

            return JsonOk(model);
        }

        [Auth(RoleAction.Orders)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        [HttpPost]
        public JsonResult ExportOrders(ExportOrdersModel settings)
        {
            new ExportOrdersHandler(settings).Execute();
            return Json(new { result = true });
        }

        #endregion

        #region ExportOrder to xlsx
        [SaasFeature(Saas.ESaasProperty.HaveExcel)]
        [Auth(RoleAction.Orders)]
        public ActionResult ExportOrder(int orderId)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return Content(T("Admin.Marketing.OrderNotFound"));

            try
            {
                order.OrderItems = order.OrderItems.OrderByDescending(x => x.Name).ToList();
                if (order.OrderItems.Count > 1)
                {
                    var temp = order.OrderItems[order.OrderItems.Count - 1];
                    var orderItems = order.OrderItems.Where(x => x.OrderItemID != temp.OrderItemID).OrderByDescending(x => x.Name).ToList();
                    order.OrderItems.Clear();
                    order.OrderItems.Add(temp);
                    order.OrderItems.AddRange(orderItems.ToArray());
                }

                string strPath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
                FileHelpers.CreateDirectory(strPath);

                var filename = string.Format("Order{0}.xlsx", order.Number.RemoveSymbols());
                var templatePath = Server.MapPath(ExcelExport.templateSingleOrder);

                ExcelExport.SingleOrder(templatePath, strPath + filename, order);

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_ExportExcel);

                return File(strPath + filename, "application/octet-stream", filename);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return Content("error");
        }

        #endregion

        #region ExportProducts

        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        [Auth(RoleAction.Orders)]
        public ActionResult ExportProducts()
        {
            SetNgController(NgControllers.NgControllersTypes.AnalyticsCtrl);
            SetMetaInformation(T("Admin.Marketing.Analytics.ExportProducts"));

            var encodings = new Dictionary<string, string>();
            foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
            {
                encodings.Add(enumItem.StrName(), enumItem.StrName());
            }

            var separators = new Dictionary<string, string>();
            foreach (var enumItem in (SeparatorsEnum[])Enum.GetValues(typeof(SeparatorsEnum)))
            {
                separators.Add(enumItem.StrName(), enumItem.Localize());
            }

            return View(new ExportProductsModel
            {
                Encoding = EncodingsEnum.Windows1251.StrName(),
                Encodings = encodings,
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
                Separators = separators
            });
        }

        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        [Auth(RoleAction.Orders)]
        [HttpPost]
        public JsonResult GetExportProductsSettings()
        {
            var encodings = new Dictionary<string, string>();
            foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
            {
                encodings.Add(enumItem.StrName(), enumItem.StrName());
            }

            var separators = new Dictionary<string, string>();
            foreach (var enumItem in (SeparatorsEnum[])Enum.GetValues(typeof(SeparatorsEnum)))
            {
                separators.Add(enumItem.StrName(), enumItem.Localize());
            }

            var model = new ExportProductsModel
            {
                Encoding = EncodingsEnum.Windows1251.StrName(),
                Encodings = encodings,
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
                Separators = separators,
                DateFrom = DateTime.Now.AddMonths(-3),
                DateTo = DateTime.Now
            };
            return JsonOk(model);
        }

        [HttpPost]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        [Auth(RoleAction.Orders)]
        public ActionResult ExportProducts(ExportProductsModel settings)
        {
            new ExportProductsHandler(settings).Execute();
            return Json(new { result = true });
        }

        #endregion

        #region ExportCustomers

        [Auth(RoleAction.Customers)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public ActionResult ExportCustomers()
        {
            SetNgController(NgControllers.NgControllersTypes.AnalyticsCtrl);
            SetMetaInformation(T("Admin.Marketing.Analytics.ExportCustomers"));

            var encodings = new Dictionary<string, string>();
            foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
            {
                encodings.Add(enumItem.StrName(), enumItem.StrName());
            }

            var customerGroups = new Dictionary<int, string>() { { -1, T("Admin.Marketing.All") } };
            foreach (var group in CustomerGroupService.GetCustomerGroupList())
            {
                customerGroups.Add(group.CustomerGroupId, group.GroupName);
            }

            var separators = new Dictionary<string, string>();
            foreach (var enumItem in (SeparatorsEnum[])Enum.GetValues(typeof(SeparatorsEnum)))
            {
                separators.Add(enumItem.StrName(), enumItem.Localize());
            }

            return View(new ExportCustomersModel
            {
                Group = -1,
                Encoding = EncodingsEnum.Windows1251.StrName(),
                PropertySeparator = ";",
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),

                Encodings = encodings,
                Groups = customerGroups,
                Separators = separators
            });
        }

        [HttpPost]
        [Auth(RoleAction.Customers)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public JsonResult GetExportCustomersSettings()
        {
            var encodings = new Dictionary<string, string>();
            foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
            {
                encodings.Add(enumItem.StrName(), enumItem.StrName());
            }

            var customerGroups = new Dictionary<int, string>() { { -1, T("Admin.Marketing.All") } };
            foreach (var group in CustomerGroupService.GetCustomerGroupList())
            {
                customerGroups.Add(group.CustomerGroupId, group.GroupName);
            }

            var separators = new Dictionary<string, string>();
            foreach (var enumItem in (SeparatorsEnum[])Enum.GetValues(typeof(SeparatorsEnum)))
            {
                separators.Add(enumItem.StrName(), enumItem.Localize());
            }

            var model = new ExportCustomersModel
            {
                Group = -1,
                Encoding = EncodingsEnum.Windows1251.StrName(),
                PropertySeparator = ";",
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
                Encodings = encodings,
                Groups = customerGroups,
                Separators = separators,
                DateFrom = DateTime.Now.AddMonths(-3),
                DateTo = DateTime.Now,
            };
            return JsonOk(model);
        }

        [HttpPost]
        [Auth(RoleAction.Customers)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public JsonResult ExportCustomers(ExportCustomersModel settings)
        {
            new ExportCustomersHandler(settings).Execute();
            return Json(new { result = true });
        }

        #endregion

        [Auth(RoleAction.Customers)]
        public ActionResult SearchQueries()
        {
            SetMetaInformation(T("Admin.Analytics.SearchQueries"));
            SetNgController(NgControllers.NgControllersTypes.SearchQueriesCtrl);

            return View();
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetSearchQueriesStatistic(SearchQueriesFilterModel model)
        {
            return Json(new GetSearchQueriesStatistic(model).Execute());
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetResultCountRangeForPaging(SearchQueriesFilterModel command)
        {
            var handler = new GetSearchQueriesStatistic(command);
            var resultCount =
                handler.GetItemsIds<SearchQueriesResultCountRangeModel>("Max(ResultCount) as Max, Min(ResultCount) as Min")
                    .FirstOrDefault();

            return Json(new { from = resultCount != null ? resultCount.Min : 0, to = resultCount != null ? resultCount.Max : 10000000 });
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetTopCustomers(string from, string to)
        {
            var fromDate = from.TryParseDateTime(new DateTime(2000, 1, 1));
            var toDate = to.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            var model = new GetTopCustomersHandler(fromDate, toDate).Execute();
            return Json(new { DataItems = model });
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetCustomerGroups(string datefrom, string dateto, string groupFormatString)
        {
            var from = datefrom.TryParseDateTime(new DateTime(2000, 1, 1));
            var to = dateto.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            object result = new GetCustomersGroupHandler(from, to, groupFormatString).GetGroups();

            return Json(result);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetOrdersData(string from, string to, bool? paid, int? orderStatus)
        {

            var fromDate = from.TryParseDateTime(new DateTime(2000, 1, 1));
            var toDate = to.TryParseDateTime(new DateTime(2100, 1, 1)).AddDays(1);

            var model = new GetOrdersDataHandler(fromDate, toDate, paid, orderStatus).Execute();
            return Json(model);
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetCatalogData()
        {
            var model = new GetProductsDataHandler().Execute();
            return Json(model);
        }
        
    }
}
