; (function (ng) {
    'use strict';

    var TriggerAnalyticsCtrl = function ($http, toaster) {

        var ctrl = this;

        ctrl.$onInit = function () {
            
        };

        ctrl.init = function (triggerId) {
            ctrl.triggerId = triggerId;

            var dateTo = new Date();
            var dateFrom = new Date(dateTo);
            dateFrom.setDate(dateFrom.getDate() - 14);
            ctrl.dateFrom = dateFrom;
            ctrl.dateTo = dateTo;

            ctrl.fetch();
        };

        ctrl.fetch = function () {
            var params = {
                triggerId: ctrl.triggerId,
                dateFrom: ctrl.dateFrom,
                dateTo: ctrl.dateTo
            };
            $http.get('emailings/getTriggerEmailingsAnalytics', { params: params }).then(function (response) {
                ctrl.Data = response.data.obj;
            });
        }
    };

    TriggerAnalyticsCtrl.$inject = ['$http', 'toaster'];

    ng.module('triggerAnalytics', ['emailingAnalytics'])
        .controller('TriggerAnalyticsCtrl', TriggerAnalyticsCtrl);

})(window.angular);