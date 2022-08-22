; (function () {
    'use strict';

    var isMobile = document.documentElement.classList.contains('mobile-version');

    angular.module('isMobile', [])
        .directive('isMobile', ['$document', '$parse', function ($document, $parse) {
            return {
                priority: 10,
                scope: true,
                controllerAs: 'isMobile',
                controller: function () {
                    var ctrl = this;

                    ctrl.$onInit = function () {
                        ctrl.value = isMobile;
                    };
                },
                bintToController: true
            }
        }]);

})();