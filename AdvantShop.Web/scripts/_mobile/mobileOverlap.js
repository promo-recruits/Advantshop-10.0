//#region module

; (function (ng) {
    'use strict';

    angular.module('mobileOverlap', ['ngCookies']);

})(window.angular);

//#endregion

//#region controller

; (function (ng) {
    'use strict';

    var mobileOverlapCtrl = function ($location, $cookies, $timeout, $http) {

        var ctrl = this;

        ctrl.goToDesktop = function () {
            $cookies.remove('deviceMode');
            $cookies.put('deviceMode', 'desktop');
            ctrl.resetLastModified().then((data) => {
                window.location = $location.absUrl();
            }).catch((error) => {
                console.error(error);
            });
        };

        ctrl.goToMobile = function () {
            $cookies.remove('deviceMode');
            $cookies.put('deviceMode', 'mobile');
            ctrl.resetLastModified().then((data) => {
                window.location = $location.absUrl();
            }).catch((error) => {
                console.error(error);
            });;
        };

        ctrl.stayOnDesktop = function () {
            $cookies.put('deviceMode', 'desktop');
            document.documentElement.classList.remove('mobile-redirect-panel');
            //$element.remove();
        };

        ctrl.stayOnMobile = function () {
            $cookies.put('deviceMode', 'mobile');
            document.documentElement.classList.remove('desktop-redirect-panel');
            //$element.remove();
        };

        ctrl.resetLastModified = function () {//чистим cache
            return $http.post('/common/resetLastModified');
        };
    };

    angular.module('mobileOverlap')
      .controller('mobileOverlapCtrl', mobileOverlapCtrl);

    mobileOverlapCtrl.$inject = ['$location', '$cookies', '$timeout', '$http'];

})(window.angular);

//#endregion