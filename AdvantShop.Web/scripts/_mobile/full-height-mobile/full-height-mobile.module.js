import './full-height-mobile.scss';

const moduleName = 'fullHeightMobile';

angular.module(moduleName, [])
    .directive('fullHeightMobile', ['$window', '$document', function ($window, $document) {

        return {
            controller: function FullHeightMobileCtrl() {

                var ctrl = this;
                var root = $document[0].querySelector(':root');

                ctrl.onOrientationChangeHandler = function () {
                    root.style.setProperty('--min-full-height', $window.innerHeight + 'px');
                };

            },
            link: function (scope, element, attrs, fullHeightMobileCtrl) {
                fullHeightMobileCtrl.onOrientationChangeHandler();

                $window.addEventListener("resize", fullHeightMobileCtrl.onOrientationChangeHandler);
            }
        };

    }]);

export default moduleName;

