; (function (ng) {
    'use strict';


    var SettingsSystemLocationCityCtrl = function ($q, uiGridConstants, uiGridCustomConfig, SweetAlert, $translate) {

        var ctrl = this;

        ctrl.$onInit = function () {
            var columnDefsCity = [
                    {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.City'),
                        enableCellEdit: false,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.SettingsSystem.City'),
                            type: uiGridConstants.filter.INPUT,
                            name: 'Name'
                        }
                    },
                    {
                        name: 'DisplayInPopup',
                        displayName: $translate.instant('Admin.Js.SettingsSystem.BasicCity'),
                        enableCellEdit: true,
                        type: 'checkbox',
 cellTemplate: '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                        width: 80,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.SettingsSystem.BasicCity'),
                            name: 'DisplayInPopup',
                            type: uiGridConstants.filter.SELECT,
                            selectOptions: [{ label: $translate.instant('Admin.Js.SettingsSystem.Yes'), value: true }, { label: $translate.instant('Admin.Js.SettingsSystem.No'), value: false }]
                        }
                    },
                    {
                        name: 'PhoneNumber',
                        displayName: $translate.instant('Admin.Js.SettingsSystem.PhoneNumber'),
                        type: uiGridConstants.filter.INPUT,
                        enableCellEdit: true,
                        width: 150,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.SettingsSystem.PhoneNumber'),
                            type: uiGridConstants.filter.INPUT,
                            name: 'PhoneNumber'
                        }
                    },
                    {
                        name: 'MobilePhoneNumber',
                        displayName: $translate.instant('Admin.Js.SettingsSystem.PhoneNumberInMobileVersion'),
                        type: uiGridConstants.filter.INPUT,
                        enableCellEdit: true,
                        width: 150,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.SettingsSystem.PhoneNumberInMobileVersion'),
                            type: uiGridConstants.filter.INPUT,
                            name: 'MobilePhoneNumber'
                        }
                    },
                    {
                        name: 'Zip',
                        displayName: $translate.instant('Admin.Js.SettingsSystem.Zip'),
                        type: uiGridConstants.filter.INPUT,
                        enableCellEdit: true,
                        width: 100,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.SettingsSystem.Zip'),
                            type: uiGridConstants.filter.INPUT,
                            name: 'Zip'
                        }
                    },
                    {
                        name: 'CitySort',
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
                                            '<ui-modal-trigger data-controller="\'ModalAddEditCitysCtrl\'" controller-as="ctrl" ' +
                                            'template-url="../areas/admin/content/src/settingsSystem/location/modal/addEditCitys/addEditCitys.html" ' +
                                            'data-resolve="{\'entity\': row.entity}" ' +
                                            'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                                            '<a ng-href="" class="ui-grid-custom-service-icon fas fa-pencil-alt">{{COL_FIELD}}</a>' +
                                       '</ui-modal-trigger>' +
                                       '<ui-grid-custom-delete url="Cities/DeleteCity" params="{\'Ids\': row.entity.CityId}"></ui-grid-custom-delete></div></div>'
                    }
            ];

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefsCity,
                uiGridCustom: {
                    rowUrl: '', //'countryregioncity/edit/{{row.entity.CountryId}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.DeleteSelected'),
                            url: 'Cities/DeleteCity',
                            field: 'CityId',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.AreYouSureDelete'), { title: $translate.instant('Admin.Js.SettingsSystem.Deleting') }).then(function (result) {
                                    return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            }
                        },
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.ShowWhenSelectingACity'),
                            url: 'Cities/ActivateCity',
                            field: 'CityId'
                        },
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.NotShowWhenSelectingACity'),
                            url: 'Cities/DisableCity',
                            field: 'CityId'
                        }
                    ]
                }
            });
        }

    }

    SettingsSystemLocationCityCtrl.$inject = ['$q', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', '$translate'];


    ng.module('settingsSystem')
      .controller('SettingsSystemLocationCityCtrl', SettingsSystemLocationCityCtrl);

})(window.angular);