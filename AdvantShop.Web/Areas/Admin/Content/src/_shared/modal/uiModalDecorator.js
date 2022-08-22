; (function (ng) {
    'use strict';
    ng.module('uiModal').config(['$provide', function ($provide) {
        $provide.decorator('$uibModal', ['$delegate', function ($delegate) {
            var originalWarn = $delegate.open;
            $delegate.open = function () {
                if (navigator.userAgent.toLowerCase().match(/(ipad|iphone)/)) {
                    window.scrollTo(0, 0);
                }
                return originalWarn.apply($delegate, arguments);
            };
            return $delegate;
        }]);

        $provide.decorator('uibModalTranscludeDirective', ['$delegate', '$animate', '$document', function ($delegate, $animate, $document) {
            var directive = $delegate[0];
            var originalLink = directive.link;

            delete directive.link;

            directive.compile = function (cElement, cAttrs) {
                return function (scope, element, attrs, controller, transclude) {
                    element[0].addEventListener('mousedown', function (event) {
                        scope.$parent.isMouseDownContent = true;
                    });

                    originalLink.apply(this, arguments);
                };
            };

            return $delegate;
        }]);

        $provide.decorator('uibModalWindowDirective', ['$delegate', '$uibModalStack', '$q', '$animateCss', '$document', function ($delegate, $modalStack, $q, $animateCss, $document) {
            var directive = $delegate[0];
            var originalLink = directive.link;

            directive.compile = function (cElement, cAttrs) {
                return function (scope, element, attrs, controller, transclude) {

                    originalLink.apply(this, arguments);

                    var originalClose = scope.close;

                    element.off('click', scope.close);

                    scope.close = function () {
                        if (scope.$parent.isMouseDownContent !== true) {
                            originalClose.apply(scope, arguments);
                        }
                    };

                    element.on('click', scope.close);

                    $document[0].addEventListener('click', function (event) {
                        scope.$parent.isMouseDownContent = null;
                    });
                };
            };

            delete directive.link;

            return $delegate;
        }]);

    }]);

})(window.angular);