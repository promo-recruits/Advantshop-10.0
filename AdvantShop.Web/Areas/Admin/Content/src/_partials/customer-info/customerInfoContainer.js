; (function (ng) {
    'use strict';

    var CustomerInfoContainerCtrl = function ($scope, customerInfoService, domService) {
        var ctrl = this,
            containerContent;

        ctrl.items = [];

        ctrl.$onInit = function () {
            customerInfoService.initContainer(ctrl);

            var customerIdInfoFromUrl = customerInfoService.getUrlParam();

            if (customerIdInfoFromUrl != null) {
                customerInfoService.addInstance({
                    customerId: customerIdInfoFromUrl
                });
            }
        };

        ctrl.initItem = function (instance) {
            instance.open();

            ctrl.contentCompress();
        };

        ctrl.closeItem = function (customerInfo) {

            var index;

            var item = customerInfo.instance;

            for (var i = 0, len = ctrl.items.length; i < len; i++) {
                if (item === ctrl.items[i]) {
                    index = i;
                    break;
                }
            }

            if (index != null) {
                ctrl.items.splice(index, 1);
            }

            if (item.onClose != null) {
                item.onClose({ instance: customerInfo });
            }

            if (ctrl.items.length === 0) {
                ctrl.contentFree();
            }
        };

        ctrl.onCloseItem = function (customerInfo) {
            ctrl.closeItem(customerInfo);
        };

        ctrl.addInstance = function (instance) {
            ctrl.items.push(instance);
        };

        ctrl.contentCompress = function () {
            containerContent = containerContent || document.getElementById('wrapper');
            containerContent.classList.add('lead-info--compress');
        };

        ctrl.contentFree = function () {
            containerContent = containerContent || document.getElementById('wrapper');

            containerContent.classList.remove('lead-info--compress');
        };

        //ctrl.clickOut = function (event) {
        //    if (ctrl.items.length > 0 && domService.closest(event.target, '.js-lead-info-container') == null && domService.closest(event.target, 'lead-info-trigger') == null) {
        //        ctrl.items.length = 0;
        //        $scope.$digest();
        //    }
        //}
    };

    CustomerInfoContainerCtrl.$inject = ['$scope', 'customerInfoService', 'domService'];

    ng.module('customerInfo')
        .controller('CustomerInfoContainerCtrl', CustomerInfoContainerCtrl);

})(window.angular);