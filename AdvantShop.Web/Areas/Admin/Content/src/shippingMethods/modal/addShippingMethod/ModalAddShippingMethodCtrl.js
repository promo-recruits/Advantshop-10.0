﻿; (function (ng) {
    'use strict';

    var ModalAddShippingMethodCtrl = function ($uibModalInstance, $http, toaster, $translate) {
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
            return $http.get('shippingMethods/getTypesList').then(function (response) {
                ctrl.types = response.data;
            });
        }

        ctrl.save = function () {
            if (ctrl.isProgress === true) {
                return;
            }
            ctrl.isProgress = true;

            $http.post('shippingMethods/addShippingMethod', { name: ctrl.name, type: ctrl.type.value, description: ctrl.description }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.DeliveryMethodsAdded'));

                    window.location = 'shippingmethods/edit/' + data.id;

                } else {
                    toaster.pop('error', '', data.error || $translate.instant('Admin.Js.ShippingMethods.ErrorAddingShippingMethods'));
                    ctrl.isProgress = false;
                }
            });
        }
    };

    ModalAddShippingMethodCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddShippingMethodCtrl', ModalAddShippingMethodCtrl);

})(window.angular);