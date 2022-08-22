; (function (ng) {
    'use strict';

    var CustomerInfoCtrl = function ($uibModalStack, customerInfoService, urlHelper) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.isChangeContent = false;
            if (ctrl.onInit != null) {
                ctrl.onInit({ instance: ctrl });
            }
        };

        ctrl.open = function () {

            var orderId = urlHelper.getUrlParam('orderId');

            ctrl.url = ctrl.instance.customerId != null && ctrl.instance.customerId.length > 0
                ? 'customers/popup?rnd=' + Math.random() + '&id=' + ctrl.instance.customerId
                : 'customers/popupAdd?rnd=' + Math.random()
                    + (orderId != null ? '&orderId=' + orderId : '')
                    + (ctrl.instance.partnerId != null ? '&partnerId=' + ctrl.instance.partnerId : '');

            ctrl.isShow = true;

            customerInfoService.setUrlParam(ctrl.instance.customerId);

            if (ctrl.onOpen != null) {
                ctrl.onOpen({ instance: ctrl });
            }
        };

        ctrl.close = function () {
            ctrl.isShow = false;

            customerInfoService.removeUrlParam();

            if (ctrl.onClose != null) {
                ctrl.onClose({ instance: ctrl });
            }
        };

        ctrl.setState = function () {
            ctrl.isChangeContent = true;
        };
    };

    CustomerInfoCtrl.$inject = ['$uibModalStack', 'customerInfoService', 'urlHelper'];

    ng.module('customerInfo', [])
        .controller('CustomerInfoCtrl', CustomerInfoCtrl);

})(window.angular);