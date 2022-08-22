﻿; (function (ng) {
    'use strict';

    var ModalAddPaymentMethodCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {

            ctrl.getTypes().then(function () {
                ctrl.type = ctrl.types[0];
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getTypes = function () {
            return $http.get('paymentMethods/getTypesList').then(function (response) {
                ctrl.types = response.data;
            });
        }

        ctrl.save = function () {
            if (ctrl.isProgress === true) {
                return;
            }
            ctrl.isProgress = true;

            $http.post('paymentMethods/addPaymentMethod', { name: ctrl.name, type: ctrl.type.value, description: ctrl.description, code: ctrl.type.code }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.PaymentMethods.PaymentMethodAdded'));

                    window.location = 'paymentmethods/edit/' + data.id;

                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.PaymentMethods.ErrorAddingPaymentMethod'));
                    ctrl.isProgress = false;
                }
            });
        }
    };

    ModalAddPaymentMethodCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddPaymentMethodCtrl', ModalAddPaymentMethodCtrl);

})(window.angular);