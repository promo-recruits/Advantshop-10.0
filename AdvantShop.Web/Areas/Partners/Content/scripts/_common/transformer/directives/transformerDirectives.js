; (function (ng) {
    'use strict';

    ng.module('transformer')
        .directive('transformer', ['$document', '$window', '$parse', function ($document, $window, $parse) {
            return {
                restrict: 'A',
                controller: 'TransformerCtrl',
                controllerAs: 'transformer',
                bindToController: true,
                scope: true,
                link: function (scope, element, attrs, ctrl) {
                    //options
                    ctrl.responsiveOptions = $parse(attrs.responsiveOptions)(scope);
                    ctrl.limitPos = attrs.limitPos != null;
                    ctrl.offsetTop = attrs.offsetTop != null ? parseFloat(attrs.offsetTop) : 0;
                    ctrl.sortOrder = attrs.sortOrder != null ? parseFloat(attrs.sortOrder) : null;
                    ctrl.onInit = attrs.onInit != null ? $parse(attrs.onInit) : null;
                    //init media query
                    ctrl.responsiveMediaQuery = ctrl.responsiveOptions != null ? $window.matchMedia(ctrl.responsiveOptions) : null;

                    function start() {

                        ctrl.elStartRect = element[0].getBoundingClientRect();

                        var containerLimit = document.getElementById(attrs.containerLimit),
                            parent = element.parent();

                        parent.css('minHeight', parent.height());

                        element.addClass(ctrl.isTouchDevice === true ? 'transformer-touch' : 'transformer-notouch');

                        ctrl.init(containerLimit);

                        ctrl.initialize = true;

                        if (ctrl.wait === true) {
                            ctrl.calc();
                            scope.$digest();
                        }

                        $window.addEventListener('scroll', ctrl.windowScroll.bind(ctrl));
                        $window.addEventListener('resize', ctrl.windowScroll.bind(ctrl));

                        return function () {
                            $window.removeEventListener('scroll', ctrl.windowScroll.bind(ctrl));
                            $window.removeEventListener('resize', ctrl.windowScroll.bind(ctrl));
                            parent.css('minHeight', null);

                            Object.keys(ctrl.styles).forEach(function (cssKey) {
                                ctrl.styles[cssKey] = null;
                            });

                            element.css(ctrl.styles);
                            element.removeClass(ctrl.isTouchDevice === true ? 'transformer-touch' : 'transformer-notouch');
                            element.removeClass('transformer-scroll-default');
                            element.removeClass('transformer-scroll-over');
                            element.removeClass('transformer-freeze');
                        };
                    }

                    function load() {
                        var destroyFn;

                        if (ctrl.responsiveMediaQuery != null) {
                            ctrl.responsiveMediaQuery.addListener(function (obj) {
                                if (obj.matches === true) {
                                    destroyFn = start();
                                } else if (destroyFn != null) {
                                    destroyFn();
                                }
                            });

                            if (ctrl.responsiveMediaQuery.matches === true) {
                                destroyFn = start();
                            } else if (destroyFn != null) {
                                destroyFn();
                            }
                        } else {
                            start();
                        }
                    }

                    if ($document[0].readyState !== 'complete') {
                        $window.addEventListener('load', load);
                    } else {
                        load();
                    }

                }
            };
      }])
      .directive('transformerMutable', ['$window', function () {
          return {
              require: '^transformer',
              restrict: 'A',
              link: function (scope, element, attrs, transformerCtrl) {
                  transformerCtrl.addMutableItem(element[0]);
              }
          };
      }]);

})(window.angular);