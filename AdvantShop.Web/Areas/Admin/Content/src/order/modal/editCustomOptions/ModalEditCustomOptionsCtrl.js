; (function (ng) {
    'use strict';

    var ModalEditCustomOptionsCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.orderItemId = params.orderItemId;
            ctrl.productId = params.productId;
            ctrl.artno = params.artno;
        };

        ctrl.customOptionsInitFn = function (customOptions) {
            ctrl.customOptions = customOptions;
        };

        ctrl.customOptionsChange = function () {
        };

        ctrl.save = function () {
            var params = {
                orderItemId: ctrl.orderItemId,
                artno: ctrl.artno,
                customOptionsXml: ctrl.customOptions.xml
            };

            $http.post('orders/changeOrderItemCustomOptions', params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.success('', 'Доп. опция сохранена');
                    $uibModalInstance.close();
                } else {
                    data.errors.forEach(function (error) {
                        ctrl.error = error;
                    });
                }
            });
        }
        
        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalEditCustomOptionsCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalEditCustomOptionsCtrl', ModalEditCustomOptionsCtrl);

})(window.angular);