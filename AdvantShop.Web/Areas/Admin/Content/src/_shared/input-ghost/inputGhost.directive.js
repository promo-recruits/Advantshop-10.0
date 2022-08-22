; (function (ng) {
    'use strict';

    ng.module('inputGhost')
      .directive('inputGhost', function () {
          return {
              require: {
                  'ngModel': 'ngModel'
              },
              controller: 'InputGhostCtrl',
              controllerAs: 'inputGhost',
              bindToController: true,
              compile: function (cElement, cAttrs) {
                  cElement[0].classList.add('input-ghost');

                  return function (scope, element, attrs, ctrls) {
                      var inputGhostCtrl = ctrls[0],
                          ngModelCtrl = ctrls[1];

                      scope.$watch('inputGhost.ngModel.$viewValue', function (newVal, oldVal) {
                          attrs.$set('size', newVal == null || newVal.length === 1 ? 1 : newVal.length);
                      });
                  }
              }
          }
      });

})(window.angular);