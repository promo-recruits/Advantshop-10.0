; (function (ng) {
    'use strict';

    ng.module('shipping')
      .directive('shippingList', ['urlHelper', '$timeout', function (urlHelper, $timeout) {
          return {
              restrict: 'A',
              scope: {
                  items: '=',
                  selectShipping: '=',
                  countVisibleItems: '=',
                  change: '&',
                  focus: '&',
                  anchor: '@',
                  isProgress: '=?',
                  contact: '<?',
                  isCanAddCustom: '<?',
                  customShipping: '<?',
                  iconWidth: '@',
                  iconHeight: '@',
                  editPrice: '<?',
                  isAdmin: '<?'
              },
              controller: 'ShippingListCtrl',
              controllerAs: 'shippingList',
              bindToController: true,
              replace: true,
              templateUrl: function () {
                  return urlHelper.getAbsUrl('/scripts/_partials/shipping/templates/shippingList.html', true);
              },
              link: function (scope, element, attrs, ctrl) {
                  scope.$watch('shippingList.items', function (newValue, oldValue) {
                      $timeout(function () {
                          if (newValue !== oldValue) {
                              ctrl.processCallbacks();
                          }
                      }, 50);
                  })
              }
          };
      }]);

    ng.module('shipping')
      .directive('shippingTemplate', [function () {
          return {
              restrict: 'A',
              scope: {
                  templateUrl: '=',
                  shipping: '=',
                  isSelected: '=',
                  changeControl: '&',
                  contact: '<?',
                  isAdmin: '<?'
              },
              controller: 'ShippingTemplateCtrl',
              controllerAs: 'shippingTemplate',
              bindToController: true,
              replace: true,
              template: '<div data-ng-include="shippingTemplate.templateUrl"></div>'
          };
      }]);


    ng.module('shipping')
  .directive('shippingVariants', function () {
      return {
          restrict: 'A',
          scope: {
              type: '@',
              offerId: '=',
              amount: '=',
              svCustomOptions: '=',
              startOfferId: '@',
              startAmount: '@',
              startSvCustomOptions: '@',
              zip: '@',
              initFn: '&'
          },
          controller: 'ShippingVariantsCtrl',
          controllerAs: 'shippingVariants',
          bindToController: true,
          replace: true,
          templateUrl: '/scripts/_partials/shipping/templates/shippingVariants.html'
      };
  });

})(window.angular);