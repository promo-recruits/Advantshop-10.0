; (function (ng) {
    'use strict';


    var SettingsSystemLocationRegionCtrl = function ($q, uiGridConstants, uiGridCustomConfig, SweetAlert, $translate) {

        var ctrl = this;

        ctrl.$onInit = function () {
            var columnDefsRegion = [
                    {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.Region'),
                        enableCellEdit: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.onSelect({id : row.entity.RegionId, name: row.entity.Name})">{{COL_FIELD}}</a></div>',
                        filter: {
                            placeholder: $translate.instant('Admin.Js.SettingsSystem.Region'),
                            type: uiGridConstants.filter.INPUT,
                            name: 'Name'
                        }
                    },
                    {
                        name: 'RegionCode',
                        displayName: $translate.instant('Admin.Js.SettingsSystem.RegionCode'),
                        type: uiGridConstants.filter.INPUT,
                        enableCellEdit: true,
                        width: 150,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.SettingsSystem.RegionCode'),
                            type: uiGridConstants.filter.INPUT,
                            name: 'RegionCode'
                        }
                    },
                    {
                        name: 'SortOrder',
                        displayName: $translate.instant('Admin.Js.SettingsSystem.Order'),
                        enableCellEdit: true,
                        type: 'number',
                        width: 100,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.SettingsSystem.Order'),
                            type: 'range',
                            rangeOptions: {
                                from: {
                                    name: 'SortingFrom'
                                },
                                to: {
                                    name: 'SortingTo'
                                }
                            }
                        }
                    },
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 75,
                        enableSorting: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><div class="">' +
                                            '<ui-modal-trigger data-controller="\'ModalAddEditRegionsCtrl\'" controller-as="ctrl" ' +
                                            'template-url="../areas/admin/content/src/settingsSystem/location/modal/addEditRegion/addEditRegions.html" ' +
                                            'data-resolve="{\'entity\': row.entity}" ' +
                                            'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                                            '<a ng-href="" class="ui-grid-custom-service-icon fas fa-pencil-alt">{{COL_FIELD}}</a>' +
                                       '</ui-modal-trigger>' +
                                       '<ui-grid-custom-delete url="Regions/DeleteRegion" params="{\'Ids\': row.entity.RegionId}"></ui-grid-custom-delete></div></div>'
                    }
            ];

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefsRegion,
                uiGridCustom: {
                    rowUrl: '', //'countryregioncity/edit/{{row.entity.CountryId}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.DeleteSelected'),
                            url: 'Regions/DeleteRegion',
                            field: 'RegionId',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.AreYouSureDelete'), { title: $translate.instant('Admin.Js.SettingsSystem.Deleting') }).then(function (result) {
                                    return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            }
                        }
                    ]
                }
            });
        }

    }

    SettingsSystemLocationRegionCtrl.$inject = ['$q', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', '$translate'];


    ng.module('settingsSystem')
      .controller('SettingsSystemLocationRegionCtrl', SettingsSystemLocationRegionCtrl);

})(window.angular);