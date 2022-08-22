﻿; (function (ng) {
    'use strict';

    var ModalAddEditSizeCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {

            var params = ctrl.$resolve;
            ctrl.sizeId = params.sizeId != null ? params.sizeId : 0;
            ctrl.mode = ctrl.sizeId != 0 ? "edit" : "add";

            if (ctrl.mode == "add") {
                ctrl.sortOrder = 0;
            } else {
                ctrl.getSize(ctrl.sizeId);
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
        
        ctrl.getSize = function (sizeId) {
            $http.get('sizes/getSize', { params: { sizeId: sizeId } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.sizeName = data.SizeName;
                    ctrl.sortOrder = data.SortOrder;
                }
            });
        }

        ctrl.save = function () {

            var params = {
                sizeId: ctrl.sizeId,
                sizeName: ctrl.sizeName,
                sortOrder: ctrl.sortOrder,
            };

            var url = ctrl.mode == "add" ? 'sizes/addSize' : 'sizes/updateSize';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop("success", "", $translate.instant('Admin.Js.Sizes.ChangesSaved'));
                    $uibModalInstance.close('saveSize');
                } else {
                    toaster.pop("error", $translate.instant('Admin.Js.Sizes.Error'), $translate.instant('Admin.Js.Sizes.ErrorAddingEditing'));
                }
            });
        }
    };

    ModalAddEditSizeCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddEditSizeCtrl', ModalAddEditSizeCtrl);

})(window.angular);