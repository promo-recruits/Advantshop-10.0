; (function (ng) {
    'use strict';

    var ModalAddEditRegionsCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {

            ctrl.entity = ng.copy(ctrl.$resolve != null ? (ctrl.$resolve.entity != null ? ctrl.$resolve.entity : {}) : {});

            ctrl.mode = ctrl.entity.RegionId  != null && ctrl.entity.RegionId != 0 ? 'edit' : 'add';
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.saveRegions = function () {

            ctrl.btnSleep = true;

            var url = ctrl.mode == 'add' ? 'Regions/AddRegion' : 'Regions/EditRegion';

            $http.post(url, ctrl.entity).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop("success", "", $translate.instant('Admin.Js.SettingsSystem.ChangesSaved'));
                    $uibModalInstance.close('saveRegions');
                    ctrl.entity = null;
                } else {
                    toaster.pop("error", $translate.instant('Admin.Js.SettingsSystem.Error'), $translate.instant('Admin.Js.SettingsSystem.ErrorCreatingRegion'));
                }
            }).finally(function () {
                ctrl.btnSleep = false;
            });
        }
    };

    ModalAddEditRegionsCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddEditRegionsCtrl', ModalAddEditRegionsCtrl);

})(window.angular);