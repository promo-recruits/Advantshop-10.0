; (function (ng) {
    'use strict';

    var PartnerInfoCtrl = function ($uibModalStack, partnerInfoService, urlHelper) {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({ instance: ctrl });
            }
        };

        ctrl.open = function () {

            ctrl.url = ctrl.instance.partnerId != null
                        ? 'partners/popup?rnd=' + Math.random() + '&id=' + ctrl.instance.partnerId
                        : 'partners/popupAdd?rnd=' + Math.random();

            ctrl.isShow = true;

            partnerInfoService.setUrlParam(ctrl.instance.partnerId);

            if (ctrl.onOpen != null) {
                ctrl.onOpen({ instance: ctrl });
            }
        };

        ctrl.close = function () {
            ctrl.isShow = false;

            partnerInfoService.removeUrlParam();

            if (ctrl.onClose != null) {
                ctrl.onClose({ instance: ctrl });
            }
        };
    };

    PartnerInfoCtrl.$inject = ['$uibModalStack', 'partnerInfoService', 'urlHelper'];

    ng.module('partnerInfo', [])
        .controller('PartnerInfoCtrl', PartnerInfoCtrl);

})(window.angular);