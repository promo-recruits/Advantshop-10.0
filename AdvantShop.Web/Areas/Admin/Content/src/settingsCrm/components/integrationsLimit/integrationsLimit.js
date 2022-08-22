; (function (ng) {
    'use strict';

    var integrationsLimitCtrl = function ($http, toaster, SweetAlert, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
        };
    };

    integrationsLimitCtrl.$inject = ['$http', 'toaster', 'SweetAlert', '$translate'];

    ng.module('integrationsLimit', [])
        .controller('integrationsLimitCtrl', integrationsLimitCtrl)
        .component('integrationsLimit', {
            templateUrl: '../areas/admin/content/src/settingsCrm/components/integrationsLimit/integrationsLimit.html',
            controller: 'integrationsLimitCtrl',
            bindings: {
                limit: '<',
                count: '<'
            }
        });

})(window.angular);