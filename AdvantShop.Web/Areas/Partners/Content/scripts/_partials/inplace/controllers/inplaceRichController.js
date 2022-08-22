; (function (ng) {
    'use strict';

    var InplaceRichCtrl = function (inplaceService, $q, $scope, $element) {
        var ctrl = this;

        //ctrl.inplaceParams = ctrl.inplaceParams();

        ctrl.active = function () {
            ctrl.isShow = true;
            ctrl.startContent = ctrl.editor.getData();
        };

        ctrl.destroy = function () {
            if (ctrl.buttons) {
                ctrl.buttons.element.remove();
            }

            if (ctrl.editor) {
                ctrl.editor.destroy();
                $scope.$destroy();
            }

            $element.removeAttr('contenteditable');
        }

        ctrl.save = function () {

            var content = ctrl.editor.getData(), params;

            if (ctrl.startContent === content) {
                return;
            }

            if (ctrl.inplaceUrl != null) {
                params = ng.extend(ctrl.getParams(), { content: content });

                inplaceService.save(ctrl.inplaceUrl, params)
                    .finally(function () {
                        ctrl.isShow = false;

                        if (ctrl.inplaceOnSave != null) {
                            ctrl.inplaceOnSave({ value: content, $scope: $scope });
                        }
                    });
            } else {

                if (ctrl.inplaceOnSave != null) {
                    ctrl.inplaceOnSave({ value: content, $scope: $scope });
                }
            }
        };

        ctrl.cancel = function () {
            ctrl.isShow = false;
            ctrl.editor.setData(ctrl.startContent);
        };
    };

    ng.module('inplace')
      .controller('InplaceRichCtrl', InplaceRichCtrl);

    InplaceRichCtrl.$inject = ['inplaceService', '$q', '$scope', '$element'];

})(window.angular);