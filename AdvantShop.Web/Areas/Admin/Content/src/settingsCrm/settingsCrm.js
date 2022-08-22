; (function (ng) {
    'use strict';

    var SettingsCrmCtrl = function ($http, toaster, $translate, $window, leadsService) {

        var ctrl = this;

        ctrl.$onInit = function () {
            //ctrl.getSaasData();
            ctrl.getSalesFunnels();
            ctrl.getOrderStatuses();
        };

        ctrl.salesFunnelsOnInit = function (salesFunnels) {
            ctrl.salesFunnels = salesFunnels;
        };

        ctrl.getSalesFunnels = function () {
            return $http.get('salesFunnels/getSalesFunnels').then(function (response) {
                return ctrl.funnels = response.data;
            });
        };

        ctrl.getOrderStatuses = function () {
            return $http.get('settingsCrm/getOrderStatuses').then(function (response) {
                return ctrl.statuses = response.data;
            });
        };

        ctrl.updateFunnels = function () {
            ctrl.getSalesFunnels();

            leadsService.updateList();
        };

        ctrl.saveDefaultSalesFunnelId = function () {
            $http.post('settingsCrm/saveDefaultSalesFunnelId', { id: ctrl.DefaultSalesFunnelId }).then(function (response) {
                if (response.data.result) {
                    toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSuccessfullySaved'));
                }
            });
        };

        ctrl.saveOrderStatusIdFromLead = function () {
            $http.post('settingsCrm/saveOrderStatusIdFromLead', { id: ctrl.OrderStatusIdFromLead }).then(function (response) {
                if (response.data.result) {
                    toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSuccessfullySaved'));
                }
            });
        };

        ctrl.setCrmActive = function (active) {
            $http.post('settingsCrm/setCrmActive', { active: active }).then(function (response) {
                if (response.data.result) {
                    toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSuccessfullySaved'));
                    $window.location.reload();
                }
            });
        }

        //ctrl.getSaasData = function () {
        //    return $http.get('settingsCrm/getIntegrationsData').then(function (response) {
        //        if (response.data != null) {
        //            ctrl.saasData = {
        //                limit: response.data.limit,
        //                count: response.data.count,
        //                limitRiched: response.data.limitRiched
        //            };
        //        }
        //    });
        //};
    };

    SettingsCrmCtrl.$inject = ['$http', 'toaster', '$translate', '$window', 'leadsService'];


    ng.module('settingsCrm', ['dealStatuses', 'facebookAuth', 'salesFunnels', 'integrationsLimit', 'leadFieldsList'])
      .controller('SettingsCrmCtrl', SettingsCrmCtrl);

})(window.angular);