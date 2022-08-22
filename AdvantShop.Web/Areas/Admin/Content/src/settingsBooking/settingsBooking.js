; (function (ng) {
    'use strict';

    var SettingsBookingCtrl = function ($http, $q, uiGridConstants, uiGridCustomConfig, SweetAlert, toaster) {
        var ctrl = this;

        var columnDefsBookingTags = [
        {
            name: 'Name',
            displayName: "Название",
            enableCellEdit: true
        },
        {
            name: 'SortOrder',
            displayName: "Порядок",
            type: 'number',
            width: 150,
            enableCellEdit: true
        },
        {
            name: '_serviceColumn',
            displayName: '',
            width: 80,
            enableSorting: false,
            cellTemplate:
                '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                //'<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTag(row.entity.Id)"></a> ' +
                '<ui-grid-custom-delete url="settingsBooking/deleteTag" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                '</div></div>'
        }
        ];

        ctrl.gridBookingTagsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsBookingTags,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: 'Удалить выделенные',
                        url: 'settingsBooking/deleteTags',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm('Вы уверены, что хотите удалить?', { title: 'Удаление' }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]
            }
        });

        ctrl.gridBookingTagsOnInit = function (grid) {
            ctrl.gridBookingTags = grid;
        };

        ctrl.onChangeStateOffOn = function (checked) {
            ctrl.BookingActive = checked;
        };
    };

    SettingsBookingCtrl.$inject = ['$http', '$q', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', 'toaster'];

    ng.module('settingsBooking', [])
      .controller('SettingsBookingCtrl', SettingsBookingCtrl);

})(window.angular);