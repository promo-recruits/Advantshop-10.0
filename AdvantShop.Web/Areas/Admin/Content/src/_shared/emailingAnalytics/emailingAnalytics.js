; (function (ng) {
    'use strict';

    var EmailingAnalyticsCtrl = function ($http, $httpParamSerializer) {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.hideFlatpickr != true) {
                ctrl.fetch();
            } else {
                ctrl.dataLoaded = true;
            }
        };

        ctrl.fetch = function () {
            ctrl.dataLoaded = false;

            $http.get(ctrl.emailDataUrl, { params: { id: ctrl.emailingId, dateFrom: ctrl.emailDateFrom, dateTo: ctrl.emailDateTo } }).then(function(response) {
                    ctrl.chartData = response.data.obj.ChartData;
                    ctrl.statusesData = response.data.obj.StatusesData;

                    if (ctrl.onChangeDate != null) {
                        ctrl.onChangeDate({ dateFrom: ctrl.emailDateFrom, dateTo: ctrl.emailDateTo });
                    }
            }).finally(function() {
                ctrl.dataLoaded = true;
            });
        }

        ctrl.getStatusUrlParams = function (statusName) {
            var dateParams = '';
            if (ctrl.emailDateFrom != '' && ctrl.emailDateFrom != undefined) {
                if (ctrl.emailDateFrom instanceof Date)
                    ctrl.emailDateFrom = ctrl.emailDateFrom.toISOString().split("T")[0];
                if (ctrl.hideFlatpickr)
                    ctrl.emailDateFrom = ctrl.emailDateFrom.split('T')[0].replace('"', '');
                dateParams += '"DateFrom":"' + ctrl.emailDateFrom.split('.').reverse().join('-') + '"';
            }
            if (ctrl.emailDateTo != '' && ctrl.emailDateTo != undefined) {
                if (ctrl.emailDateTo instanceof Date)
                    ctrl.emailDateTo = ctrl.emailDateTo.toISOString().split("T")[0];
                if (ctrl.hideFlatpickr)
                    ctrl.emailDateTo = ctrl.emailDateTo.split('T')[0].replace('"', '');
                dateParams += (dateParams == '' ? '"DateFrom":"",' : ',') + '"DateTo":"' + ctrl.emailDateTo.split('.').reverse().join('-') + '"';
            }
            var statusParams = '';
            if (statusName) {
                statusParams = '"Statuses":"' + statusName + '"';
            }
            if (dateParams != '' || statusParams != '') {
                //return '#?' + ctrl.gridName + '={' + dateParams + (dateParams == '' || statusParams == '' ? '' : ',') + statusParams + '}';
                return '{' + dateParams + (dateParams == '' || statusParams == '' ? '' : ',') + statusParams + '}';
            }
            return '{}';
        };

        ctrl.getStatusUrlParamsForMVC = function (statusName) {
            var data = JSON.parse(ctrl.getStatusUrlParams(statusName));
            return $httpParamSerializer(data);
        };

    }

    EmailingAnalyticsCtrl.$inject = ['$http', '$httpParamSerializer'];

    ng.module('emailingAnalytics', [])
        .controller('EmailingAnalyticsCtrl', EmailingAnalyticsCtrl)
        .component('emailingAnalytics', {
            templateUrl: '../areas/admin/content/src/_shared/emailingAnalytics/templates/emailingAnalytics.html',
            controller: 'EmailingAnalyticsCtrl',
            bindings: {
                emailingId: '@',
                emailSubject: '@',
                sendTime: '@',
                chartData: '<',
                statusesData: '<',
                emailLogUrl: '<?',
                emailComeBackUrl: '<?',
                emailDataUrl: '@',
                emailDateFrom: '<?',
                emailDateTo: '<?',
                hideFlatpickr: '<',
                gridName: '@',
                emailComeBackClick: '&',
                emailLogClick: '&',
                onChangeDate: '&',
                hideComeBackLink: '<'
            }
      });

})(window.angular);