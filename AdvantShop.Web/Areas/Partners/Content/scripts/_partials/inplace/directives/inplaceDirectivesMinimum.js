; (function (ng) {
    'use strict';

    ng.module('inplace')
      .directive('inplaceStart', ['$window', '$compile', '$rootScope', 'inplaceService', function ($window, $compile, $rootScope, inplaceService) {
          return {
              restrict: 'A',
              scope: {},
              link: function (scope, element, attrs, ctrl) {
                  var selector = '[data-inplace-rich]';
                  var mq = $window.matchMedia('(min-width: 75em)');

                  if (mq.matches) {
                      init();
                  }

                  mq.addListener(function (result) {
                      result.matches ? init() : destroy();
                  });


                  function init() {
                      var objs = document.querySelectorAll(selector);

                      if (objs != null && objs.length > 0) {
                          Array.prototype.slice.call(objs).forEach(function (item) {
                              var _item = ng.element(item);
                              var _scope = _item.scope() || scope;
                              _item.addClass('inplace-initialized');
                              $compile(item)(_scope.$new());
                          })
                      }
                  }

                  function destroy() {
                      inplaceService.destroyAll();

                      var objs = document.querySelectorAll(selector);

                      if (objs != null && objs.length > 0) {
                          Array.prototype.slice.call(objs).forEach(function (item) {
                              var _item = ng.element(item);
                              _item.removeClass('inplace-initialized');
                          })
                      }
                  }
              }
          };
      }]);

})(window.angular);