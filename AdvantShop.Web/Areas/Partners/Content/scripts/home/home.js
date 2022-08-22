; (function (ng) {

    'use strict';

    var HomeCtrl = function ($http, toaster) {
        var ctrl = this;

        ctrl.chartOptions = {
            maintainAspectRatio: false,
            tooltips: {
                callbacks: {
                    label: function (tooltipItem, data) {
                        var datasetItem = data.datasets[tooltipItem.datasetIndex];
                        return datasetItem.label + ': ' + datasetItem.data[tooltipItem.index].label;
                    }
                }
            }
        };

        ctrl.initChart = function (dateFrom, dateTo) {
            ctrl.dateFrom = dateFrom;
            ctrl.dateTo = dateTo;
            ctrl.fetchChartData();
        };

        ctrl.fetchChartData = function () {
            $http.post('home/getAccrualChartData', { from: ctrl.dateFrom, to: ctrl.dateTo }).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.accrualSum = data.obj.sum;
                    ctrl.chartData = data.obj.chartData;
                } else {
                    toaster.error('', (data.errors || [])[0] || 'Не удалось получить данные');
                }
            });
        };
    };

    HomeCtrl.$inject = ['$http', 'toaster'];

    ng.module('home', [])
      .controller('HomeCtrl', HomeCtrl);

})(window.angular);

