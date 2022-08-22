; (function (ng) {
    'use strict';

    var CustomerInfoTriggerCtrl = function (customerInfoService) {
        var ctrl = this;

        ctrl.openByTrigger = function () {
            customerInfoService.addInstance(ctrl);
        };

        ctrl.close = function (instance) {
            if (ctrl.onClose != null) {
                ctrl.onClose({ instance: instance});
            }
        };
    };

    CustomerInfoTriggerCtrl.$inject = ['customerInfoService'];

    ng.module('customerInfo')
        .controller('CustomerInfoTriggerCtrl', CustomerInfoTriggerCtrl);

})(window.angular);