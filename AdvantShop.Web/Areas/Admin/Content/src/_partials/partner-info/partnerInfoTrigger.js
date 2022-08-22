; (function (ng) {
    'use strict';

    var PartnerInfoTriggerCtrl = function (partnerInfoService) {
        var ctrl = this;

        ctrl.openByTrigger = function () {
            partnerInfoService.addInstance(ctrl);
        };

        ctrl.close = function() {
            if (ctrl.onClose != null) {
                ctrl.onClose();
            }
        }
    };

    PartnerInfoTriggerCtrl.$inject = ['partnerInfoService'];

    ng.module('partnerInfo')
        .controller('PartnerInfoTriggerCtrl', PartnerInfoTriggerCtrl);

})(window.angular);