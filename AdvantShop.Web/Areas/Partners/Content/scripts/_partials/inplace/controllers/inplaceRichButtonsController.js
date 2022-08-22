; (function (ng) {
    'use strict';

    var InplaceRichButtonsCtrl = function ($element, inplaceService, $scope) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.richCtrl = inplaceService.getRich(ctrl.inplaceRichButtons);

            ctrl.richCtrl.buttons = {
                element: $element,
                ctrl: ctrl
            };
        };

        ctrl.btnSave = function () {
            ctrl.richCtrl.clickedButtons = true;
            ctrl.richCtrl.save();
        };

        ctrl.btnCancel= function () {
            ctrl.richCtrl.clickedButtons = true;
            ctrl.richCtrl.cancel();
        };

        ctrl.destroy = function () {
            $scope.$destroy();
            $element.remove();
        }
    };

    ng.module('inplace')
      .controller('InplaceRichButtonsCtrl', InplaceRichButtonsCtrl);

    InplaceRichButtonsCtrl.$inject = ['$element', 'inplaceService', '$scope'];
})(window.angular);