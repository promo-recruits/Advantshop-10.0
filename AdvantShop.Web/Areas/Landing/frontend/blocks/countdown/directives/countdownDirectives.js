; (function (ng) {
    'use strict';

    ng.module('countdown')
      .directive('countdown', function () {
          return {
              restrict: 'A',
              scope: {
                  endTime: '=',
                  endTimeUtc: '=',
                  isShowDays: '<?',
                  isLoop: '<?',
                  onFinish: '&'
              },
              replace: true,
              controller: 'CountdownCtrl',
              controllerAs: 'countdown',
              bindToController: true,
              templateUrl: function (element, attrs) {
                  return attrs.templateUrl || '/areas/landing/frontend/blocks/countdown/templates/countdown.html'
              }
          }
      });
})(window.angular);