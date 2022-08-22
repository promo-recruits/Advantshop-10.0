; (function (ng) {
    'use strict';

    var LeadInfoContainerCtrl = function ($document, $scope, leadInfoService, domService) {
        var ctrl = this,
            containerContent;

        ctrl.items = [];

        ctrl.$onInit = function () {
            leadInfoService.initContainer(ctrl);

            var leadIdInfoFromUrl = leadInfoService.getUrlParam();

            if (leadIdInfoFromUrl != null) {
                leadInfoService.addInstance({
                    leadId: leadIdInfoFromUrl
                });
            }
        };

        ctrl.initItem = function (instance) {
            instance.open();

            ctrl.contentCompress();
        };

        ctrl.closeItem = function (item) {

            var index;

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
                item.onClose();
            }

            if (ctrl.items.length === 0) {
                ctrl.contentFree();
            }
        }

        ctrl.onCloseItem = function (item) {
            ctrl.closeItem(item.instance);
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

    LeadInfoContainerCtrl.$inject = ['$document','$scope', 'leadInfoService', 'domService'];

    ng.module('leadInfo')
        .controller('LeadInfoContainerCtrl', LeadInfoContainerCtrl);

})(window.angular);