; (function (ng) {
    'use strict';

    var PartnersPayoutReportsCtrl = function (uiGridConstants, uiGridCustomConfig, toaster, $http, $translate, $window) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'FileName',
                    displayName: 'Файл',
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="partnersReport/payoutReport/{{row.entity.Id}}">{{COL_FIELD}}</a></div>'
                },
                {
                    name: 'PeriodToFormatted',
                    displayName:'Период'
                },
                {
                    name: 'DateCreatedFormatted',
                    displayName: 'Создан'
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 50,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-grid-custom-delete url="partnersReport/deletePayoutReport" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.getPayoutsReport = function () {
            return $http.post('partnersReport/getPayoutsReport').then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.grid.fetchData();
                    $window.location.assign(data.obj.url);
                } else {
                    toaster.error('', 'Не удалось сформировать отчет');
                }
            });
        };
    };

    PartnersPayoutReportsCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', 'toaster', '$http', '$translate', '$window'];


    ng.module('partnersPayoutReports', ['uiGridCustom'])
      .controller('PartnersPayoutReportsCtrl', PartnersPayoutReportsCtrl);

})(window.angular);