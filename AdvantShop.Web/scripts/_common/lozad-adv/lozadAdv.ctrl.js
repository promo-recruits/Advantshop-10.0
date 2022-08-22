import debounceFn from 'debounce-fn';

/*@ngInject*/
export default function LozadAdvCtrl($window, $scope, $element, $parse, $attrs, $document, lozadAdvDefault) {

    const ctrl = this;

    let observer = void 0,
        elementNative,
        options,
        lozadAdv,
        eventReinit;

    ctrl.$postLink = function () {
        elementNative = $element[0];
        options = angular.extend({}, lozadAdvDefault, $parse($attrs.lozadAdvOptions)($scope));
        lozadAdv = $parse($attrs.lozadAdv);
        eventReinit = $parse($attrs.lozadAdvEventReinit)($scope);

        if (options.afterWindowLoaded === true && $document[0].readyState !== 'complete') {
            $window.addEventListener('load', ctrl.loadFn);
        } else {
            ctrl.lozad(elementNative);
        }
    };

    ctrl.onIntersection = function onIntersection(load) {
        return function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting || ctrl.isElementInViewport(entry.target, entry)) {
                    debounceFn(ctrl.scroll.bind(ctrl, entry, observer, load), {wait: 500})();
                }
            });
        };
    };

    ctrl.scroll = function (entry, observer, load) {
        if (ctrl.isElementInViewport(entry.target, entry) === true) {
            observer.unobserve(entry.target);
            load(entry.target);
            lozadAdv($scope);
            $scope.$digest();
        }
    };

    ctrl.isElementInViewport = function isElementInViewport(el, entry) {
        var rect = entry.boundingClientRect;

        var windowHeight = (window.innerHeight || document.documentElement.clientHeight);
        // http://stackoverflow.com/questions/325933/determine-whether-two-date-ranges-overlap
        var vertInView = (rect.top <= windowHeight) && ((rect.top + rect.height) >= 0);
        return vertInView;
    };

    ctrl.lozad = function lozad(element) {
        if ($window.IntersectionObserver) {
            observer = new IntersectionObserver(ctrl.onIntersection(options.load), options);
        }

        if (observer) {
            observer.observe(element);
        }

        if (eventReinit != null) {
            $scope.$on(eventReinit, function () {
                ctrl.reinit();
            });
        }
    };

    ctrl.loadFn = function loadFn() {

        $window.removeEventListener('load', ctrl.loadFn);

        ctrl.lozad(elementNative);
    };

    ctrl.reinit = function reinit() {
        if (observer) {
            observer.unobserve(elementNative);
        }

        ctrl.lozad(elementNative);
    };
}
