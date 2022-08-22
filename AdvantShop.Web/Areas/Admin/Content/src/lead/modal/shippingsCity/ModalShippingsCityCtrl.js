; (function (ng) {
    'use strict';

    var ModalShippingsCityCtrl = function ($uibModalInstance, toaster, $http) {
        var ctrl = this;
        
        ctrl.$onInit = function () {
            var params = ctrl.$resolve.obj;
            ctrl.leadId = params.leadId;

            ctrl.getShippingCity();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getShippingCity = function () {
            $http.get('leads/getShippingCity', { params: { leadId: ctrl.leadId } }).then(function (response) {
                ctrl.data = response.data;
            });
        }

        ctrl.save = function () {
            ctrl.data.leadId = ctrl.leadId;
            $http.post('leads/saveShippingCity', ctrl.data).then(function (response) {
                var data = response.data;
                $uibModalInstance.close(data != null ? data.obj : null);
            });
        }

    };

    ModalShippingsCityCtrl.$inject = ['$uibModalInstance', 'toaster', '$http'];

    ng.module('uiModal')
        .controller('ModalShippingsCityCtrl', ModalShippingsCityCtrl);

})(window.angular);