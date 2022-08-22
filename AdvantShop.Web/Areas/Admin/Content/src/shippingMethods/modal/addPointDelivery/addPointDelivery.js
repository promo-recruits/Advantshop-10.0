; (function (ng) {
    'use strict';

    var ModalAddPointDeliveryCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.mode = (ctrl.$resolve ? ctrl.$resolve.point : null) ? 'edit' : 'add';
            ctrl.point = (ctrl.$resolve.point ? ng.extend({}, ctrl.$resolve.point) : null) || { PointX: 0.0, PointY: 0.0 };
            ctrl.point.position = ctrl.point.PointX + ', ' + ctrl.point.PointY;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            var coordinats = ctrl.point.position.replace(/\[|\]/g, '').split(',');
            ctrl.point.PointX = parseFloat(coordinats[0].trim());
            ctrl.point.PointY = parseFloat(coordinats[1].trim());
            delete ctrl.point.position;

            $uibModalInstance.close(ctrl.point);
        };
    };

    ModalAddPointDeliveryCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal')
        .controller('ModalAddPointDeliveryCtrl', ModalAddPointDeliveryCtrl);

})(window.angular);