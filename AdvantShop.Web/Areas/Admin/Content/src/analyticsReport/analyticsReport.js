; (function (ng) {
    'use strict';

    var AnalyticsReportCtrl = function ($location) {

        var ctrl = this;
        var chartsStorage = {};


        ctrl.showTab = function (tab) {
            //ctrl.showPaid = false;
            //ctrl.showOrderStatus = false;

            if (ctrl.selectedTab !== tab) {
                ctrl.selectedTab = tab;
                $location.search('analyticsReportTab', tab);
            }
        };

        ctrl.updateData = function () {
            ctrl.showTab(ctrl.selectedTab);       

            if (ctrl.selectedTab == "telephony") {
                ctrl.recalcTelephonyCallLog(ctrl.dateFrom, ctrl.dateTo);
            } else if (ctrl.selectedTab == "orders") {
                ctrl.recalcForOrdersTab(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.orderStatus);
            } else if (ctrl.selectedTab == 'product') {
                ctrl.productreport.recalc(ctrl.dateFrom, ctrl.dateTo, ctrl.productreportProductId, ctrl.paid);
            } else if (chartsStorage[ctrl.selectedTab] != null) {
                chartsStorage[ctrl.selectedTab](ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.orderStatus);
            }
        };

        ctrl.$onInit = function () {
        };

        ctrl.init = function (defaultTab) {
            var urlParams = $location.search();

            if (urlParams != null && urlParams.analyticsReportTab != null && urlParams.analyticsReportTab.length > 0) {
                ctrl.selectedTab = urlParams.analyticsReportTab;
            } else {
                ctrl.selectedTab = defaultTab || 'orders';
            }
        };

        ctrl.onInitVortex = function (vortex, dateFrom, dateTo) {
            ctrl.vortex = vortex;

            ctrl.showPaid = false;
            ctrl.showOrderStatus = false;
            ctrl.showDateFrom = true;
            ctrl.showDateTo = true;

            ctrl.vortex.recalc(dateFrom, dateTo);

            chartsStorage['vortex'] = ctrl.vortex.recalc;
        };

        ctrl.onInitProfit = function (profit, dateFrom, dateTo, paid, orderStatus) {
            ctrl.profit = profit;

            ctrl.showPaid = true;
            ctrl.showOrderStatus = true;
            ctrl.showDateFrom = true;
            ctrl.showDateTo = true;

            ctrl.profit.recalc(dateFrom, dateTo, paid, orderStatus);
            //chartsStorage['orders'] = ctrl.profit.recalc;
        };

        ctrl.onInitAvgcheck = function (avgcheck, dateFrom, dateTo, paid, orderStatus) {
            ctrl.avgcheck = avgcheck;

            ctrl.showPaid = true;
            ctrl.showOrderStatus = true;
            ctrl.showDateFrom = true;
            ctrl.showDateTo = true;

            ctrl.avgcheck.recalc(dateFrom, dateTo, paid, orderStatus);

            //chartsStorage['orders'] = ctrl.avgcheck.recalc;
        };

        ctrl.onInitOrders = function (orders, dateFrom, dateTo, paid, orderStatus) {
            ctrl.orders = orders;

            ctrl.showPaid = true;
            ctrl.showOrderStatus = true;
            ctrl.showDateFrom = true;
            ctrl.showDateTo = true;

            ctrl.orders.recalc(dateFrom, dateTo, paid, orderStatus);

            chartsStorage['orders'] = ctrl.orders.recalc;
        };

        ctrl.recalcForOrdersTab = function (dateFrom, dateTo, paid, orderStatus) {
            ctrl.profit.recalc(dateFrom, dateTo, paid, orderStatus);
            ctrl.orders.recalc(dateFrom, dateTo, paid, orderStatus);
            ctrl.avgcheck.recalc(dateFrom, dateTo, paid, orderStatus);
        };

        ctrl.onInitAbcxyz = function (abcxyz, dateFrom, dateTo) {
            ctrl.abcxyz = abcxyz;

            ctrl.showPaid = false;
            ctrl.showOrderStatus = false;
            ctrl.showDateFrom = true;
            ctrl.showDateTo = true;

            ctrl.abcxyz.recalc(dateFrom, dateTo);
            chartsStorage['abcxyz'] = ctrl.abcxyz.recalc;
        };

        ctrl.onInitRfm = function (rfm, dateFrom, dateTo) {
            ctrl.rfm = rfm;

            ctrl.showPaid = false;
            ctrl.showOrderStatus = false;
            ctrl.showDateFrom = true;
            ctrl.showDateTo = true;

            ctrl.rfm.recalc(dateFrom, dateTo);
            chartsStorage['rfm'] = ctrl.rfm.recalc;
        };

        ctrl.onInitTelephony = function (telephony, dateFrom, dateTo) {
            ctrl.telephony = telephony;

            ctrl.showPaid = false;
            ctrl.showOrderStatus = false;
            ctrl.showDateFrom = true;
            ctrl.showDateTo = true;

            ctrl.telephony.recalc(dateFrom, dateTo);
            chartsStorage['telephony'] = ctrl.telephony.recalc;
        };

        ctrl.onInitTelephonyCallLog = function (telephonyCallLog, dateFrom, dateTo) {
            ctrl.telephonyCallLog = telephonyCallLog;
            
            ctrl.showDateFrom = true;
            ctrl.showDateTo = true;

            ctrl.telephonyCallLog.recalc(dateFrom, dateTo);
        };

        ctrl.recalcTelephonyCallLog = function (dateFrom, dateTo, paid, orderStatus) {
            ctrl.telephony.recalc(dateFrom, dateTo);
            ctrl.telephonyCallLog.recalc(dateFrom, dateTo);
        };

        ctrl.onInitManagers = function (managers, dateFrom, dateTo) {
            ctrl.managers = managers;

            ctrl.showPaid = false;
            ctrl.showOrderStatus = false;
            ctrl.showDateFrom = true;
            ctrl.showDateTo = true;

            ctrl.managers.recalc(dateFrom, dateTo);
            chartsStorage['managers'] = ctrl.managers.recalc;
        };

        ctrl.onInitProductReport = function (productreport, dateFrom, dateTo, paid, productId) {
            ctrl.productreport = productreport;
            if (ctrl.productreportProductId == null) { 
                ctrl.productreportProductId = productId;
            }

            ctrl.showPaid = true;
            ctrl.showOrderStatus = false;
            ctrl.showDateFrom = true;
            ctrl.showDateTo = true;

            ctrl.productreport.recalc(dateFrom, dateTo, ctrl.productreportProductId, paid);

            //chartsStorage['product'] = ctrl.productreport.recalc;
        };

        ctrl.onChangeProductReport = function (productId) {
            ctrl.productreportProductId = productId;  // сохраняем выбраный товар в текущем контроллере, при смене таба затрется или выставится из request
        }

        ctrl.onInitEmailMailing = function (emailings) {

            ctrl.emailings = emailings;

            ctrl.showPaid = false;
            ctrl.showOrderStatus = false;
            ctrl.showDateFrom = false;
            ctrl.showDateTo = false;
        };

        ctrl.onInitSearchRequests = function (searchRequests) {

            ctrl.searchRequests = searchRequests;

            ctrl.showPaid = false;
            ctrl.showOrderStatus = false;
            ctrl.showDateFrom = false;
            ctrl.showDateTo = false;
        };

        ctrl.onInitExportExcel = function () {
            ctrl.showPaid = false;
            ctrl.showOrderStatus = false;
            ctrl.showDateFrom = false;
            ctrl.showDateTo = false;
        };
    };

    AnalyticsReportCtrl.$inject = ['$location'];

    ng.module('analyticsReport', [])
      .controller('AnalyticsReportCtrl', AnalyticsReportCtrl);

})(window.angular);