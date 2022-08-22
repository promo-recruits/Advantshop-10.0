; (function (ng) {
    'use strict';

    var dependencyList = [
        'pascalprecht.translate',
        'ngCookies',
        'ngFileUpload',
        'mask',
        'ngSanitize',
        'angular-bind-html-compile',
        //'angular-cache',
        'angular-ladda',
        'autocompleter',
        'autofocus',
        'carousel',
        'cookiesPolicy',
        'countdown',
        'currency',
        'defaultButton',
        'dom',
        //'ext',
        'harmonica',
        'input',
        'urlHelper',
        'modal',
        'module',
        'ngInputModified',
        'oc.lazyLoad',
        'popover',
        'readmore',
        'select',
        'submenu',
        'scrollToTop',
        'spinbox',
        'tabs',
        'toaster',
        'transformer',
        'ui-rangeSlider',
        'windowExt',
        'zoomer',
        'magnificPopup',
        'mouseoverClassToggler',
        'validation',
        'internationalPhoneNumber',

        'ui.bootstrap.popover',
        'ui.bootstrap',
        'angular-flatpickr',

        //controllers of pages
        'customers',
        'forgotPassword',
        'home',
        'rewards',
        'settings',
        'qazy'
    ];

    var dependencyService = function () {
        var service = this;

        service.get = function () {
            return dependencyList;
        };
    };


    ng.module('dependency', [])
        .service('dependencyService', dependencyService);


})(window.angular);