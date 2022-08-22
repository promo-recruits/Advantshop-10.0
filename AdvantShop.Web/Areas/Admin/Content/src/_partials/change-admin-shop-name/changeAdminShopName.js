; (function (ng) {
    'use strict';

    var ChangeAdminShopNameCtrl = function ($rootScope) {
        var ctrl = this;

        ctrl.$onInit = function () {

            $rootScope.$on('adminShopNameUpdated', function (event, data) {
                if (data != null && data.shopName) {
                    ctrl.shopname = data.shopName;
                }
            });
        };

        ctrl.save = function (result) {
            ctrl.shopname = result.name;
        };
    };

    ChangeAdminShopNameCtrl.$inject = ['$rootScope'];

    ng.module('changeAdminShopName', [])
        .controller('ChangeAdminShopNameCtrl', ChangeAdminShopNameCtrl);

})(window.angular);