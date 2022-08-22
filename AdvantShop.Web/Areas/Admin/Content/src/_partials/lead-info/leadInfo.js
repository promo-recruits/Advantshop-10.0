; (function (ng) {
    'use strict';

    var LeadInfoCtrl = function ($uibModalStack, leadInfoService, $window, $timeout) {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({ instance: ctrl });
            }
        };

        ctrl.open = function () {

            ctrl.scrollTop = $window.pageYOffset;

            ctrl.url = 'leads/popup?rnd=' + Math.random() + '&id=' + ctrl.instance.leadId;

            ctrl.isShow = true;

            leadInfoService.setUrlParam(ctrl.instance.leadId);

            if (ctrl.onOpen != null) {
                ctrl.onOpen({ instance: ctrl });
            }
        };

        ctrl.close = function () {
            ctrl.isShow = false;


            leadInfoService.removeUrlParam();

            if (ctrl.onClose != null) {
                ctrl.onClose({ instance: ctrl });
            }

            $timeout(function () {
                $window.scrollTo(0, ctrl.scrollTop);
            }, 0);
        };
    };

    LeadInfoCtrl.$inject = ['$uibModalStack', 'leadInfoService', '$window', '$timeout'];

    ng.module('leadInfo', [])
        .controller('LeadInfoCtrl', LeadInfoCtrl);

})(window.angular);