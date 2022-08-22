﻿; (function (ng) {
    'use strict';

    ng.module('uiModal')
      //.component('uiModalTrigger', {
      //    template: '<span ng-click="$ctrl.open()" ng-transclude></span>',
      //    controller: 'UiModalTriggerCtrl',
      //    transclude: true,
      //    bindings: {
      //        controller: '<',
      //        resolve: '<?',
      //        controllerAs: '@',
      //        template: '@',
      //        templateUrl: '@',
      //        size: '@',
      //        backdrop: '@',
      //        windowClass: '@',
      //        onClose: '&',
      //        onDismiss: '&',
      //        onBeforeOpen: '&',
      //        keyboard: '<?'
      //    }
      //})
        .directive('uiModalTrigger', function () {
            return {
                controller: 'UiModalTriggerCtrl',
                bindToController: true,
                scope: {
                    controller: '<',
                    resolve: '<?',
                    controllerAs: '@',
                    template: '@',
                    templateUrl: '@',
                    size: '@',
                    backdrop: '@',
                    windowClass: '@',
                    onClose: '&',
                    onDismiss: '&',
                    onBeforeOpen: '&',
                    keyboard: '<?',
                    animation: '<?',
                    openedClass: '@' 
                },
                link: function (scope, element, attrs, ctrl) {
                    element.on('click', function () {
                        ctrl.open();
                        scope.$digest();
                    });
                }
            };
        })
        .component('uiModalCross', {
            template: '<div class="close" ng-click="$ctrl.close()"></div>',
            bindings: {
                closeFn: '&'
            },
            controller: ['$uibModalStack', '$attrs', function ($uibModalStack, $attrs) {
                this.close = function () {
                    if ($attrs.closeFn != null) {
                        this.closeFn();
                    } else {
                        $uibModalStack.getTop().key.dismiss('crossClick');
                    }
                }
            }]
        })
        .component('uiModalDismiss', {
            transclude: true,
            template: '<span ng-click="$ctrl.close()" ng-transclude></span>',
            controller: ['$uibModalStack', function ($uibModalStack) {
                this.close = function () {
                    $uibModalStack.getTop().key.dismiss('crossClick');
                }
            }]
        })

})(window.angular);