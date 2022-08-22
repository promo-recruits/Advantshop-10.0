; (function (ng) {
    'use strict';
    
    var SettingsCustomersCtrl = function ($uibModal, $q, $location, uiGridConstants, uiGridCustomConfig, SweetAlert, $translate) {

        var ctrl = this;

        ctrl.editValues = function (field) {
            ctrl.field = field;
            // при перезагрузке страницы из редактирования значений в урле остается gridCustomerFieldValues
            $location.search('gridCustomerFieldValues', null);
        };

        ctrl.back = function () {
            ctrl.field = null;
            ctrl.gridCustomerFieldValues.clearParams();
        };

        // #region CustomerFields
        var columnDefsCustomerFields = [
            {
                name: 'Name',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.Name'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCustomers.Name'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Name',
                }
            },
            {
                name: 'FieldTypeFormatted',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.Type'),
                width: 150,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCustomers.Type'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'FieldType',
                    fetch: 'customerFields/getCustomerFieldTypes'
                }
            },
            {
                name: 'HasValues',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.FieldSettings'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link" ng-if="row.entity.HasValues" ng-click="grid.appScope.$ctrl.gridExtendCtrl.editValues(row.entity)">' + $translate.instant('Admin.Js.SettingsCustomers.ListOfValues') + '</span></div>',
                width: 140,
                enableSorting: false
            },
            {
                name: 'Required',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.Obligatory'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row" field-name="Required" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                width: 100,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCustomers.Obligatory'),
                    name: 'Required',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [{ label: $translate.instant('Admin.Js.SettingsCustomers.Yes'), value: true }, { label: $translate.instant('Admin.Js.SettingsCustomers.No'), value: false }]
                }
            },
            {
                name: 'SortOrder',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.SortingOrder'),
                type: 'number',
                width: 100,
                enableCellEdit: true
            },
            {
                name: 'ShowInRegistration',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.RequestFromBuyerInRegistration'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row" field-name="ShowInRegistration" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                width: 130,
                visible: 1440,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCustomers.RequestFromBuyerInRegistration'),
                    name: 'ShowInRegistration',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [{ label: $translate.instant('Admin.Js.SettingsCustomers.Yes'), value: true }, { label: $translate.instant('Admin.Js.SettingsCustomers.No'), value: false }]
                }
            },
            {
                name: 'ShowInCheckout',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.RequestFromBuyerInCheckout'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row" field-name="ShowInCheckout" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                width: 140,
                visible: 1440,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCustomers.RequestFromBuyerInCheckout'),
                    name: 'ShowInCheckout',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [{ label: $translate.instant('Admin.Js.SettingsCustomers.Yes'), value: true }, { label: $translate.instant('Admin.Js.SettingsCustomers.No'), value: false }]
                }
            },
            {
                name: 'Enabled',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.Actively'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                width: 75,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCustomers.Activity'),
                    name: 'Enabled',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [{ label: $translate.instant('Admin.Js.SettingsCustomers.Active'), value: true }, { label: $translate.instant('Admin.Js.SettingsCustomers.Inactive'), value: false }]
                }
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadCustomerField(row.entity.Id)"></a> ' +
                    '<ui-grid-custom-delete url="customerFields/delete" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                    '</div></div>'
            }
        ];

        ctrl.gridCustomerFieldsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsCustomerFields,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadCustomerField(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SettingsCustomers.DeleteSelected'),
                        url: 'customerFields/deleteItems',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsCustomers.AreYouSureDelete'), { title: $translate.instant('Admin.Js.SettingsCustomers.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]
            }
        });

        ctrl.gridCustomerFieldsOnInit = function (grid) {
            ctrl.gridCustomerFields = grid;
        };

        ctrl.loadCustomerField = function (id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditCustomerFieldCtrl',
                controllerAs: 'ctrl',
                size: 'middle',
                templateUrl: '../areas/admin/content/src/settingsCustomers/modal/addEditCustomerField/AddEditCustomerField.html',
                resolve: {
                    id: function () {
                        return id;
                    }
                }
            }).result.then(function (result) {
                ctrl.gridCustomerFields.fetchData();
                return result;
            }, function (result) {
                return result;
            });
        };
        // #endregion

        // #region CustomerFieldValues
        var columnDefsCustomerFieldValues = [
            {
                name: 'Value',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.Value'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCustomers.Value'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Value',
                }
            },
            {
                name: 'SortOrder',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.SortingOrder'),
                type: 'number',
                width: 100,
                enableCellEdit: true
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadCustomerFieldValue(row.entity.Id)"></a> ' +
                    '<ui-grid-custom-delete url="customerFieldValues/delete" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                    '</div></div>'
            }
        ];

        ctrl.gridCustomerFieldValuesOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsCustomerFieldValues,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadCustomerFieldValue(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SettingsCustomers.DeleteSelected'),
                        url: 'customerFieldValues/deleteItems',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsCustomers.AreYouSureDelete'), { title: $translate.instant('Admin.Js.SettingsCustomers.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]
            }
        });

        ctrl.gridCustomerFieldValuesOnInit = function (grid) {
            ctrl.gridCustomerFieldValues = grid;
        };

        ctrl.loadCustomerFieldValue = function (id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditCustomerFieldValueCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/settingsCustomers/modal/addEditCustomerFieldValue/AddEditCustomerFieldValue.html',
                resolve: {
                    id: function () {
                        return id;
                    }
                }
            }).result.then(function (result) {
                ctrl.gridCustomerFieldValues.fetchData();
                return result;
            }, function (result) {
                return result;
            });
        };
        // #endregion
    };

    SettingsCustomersCtrl.$inject = ['$uibModal', '$q', '$location', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', '$translate'];

    ng.module('settingsCustomers', ['as.sortable', 'vkAuth'])
      .controller('SettingsCustomersCtrl', SettingsCustomersCtrl);

})(window.angular);