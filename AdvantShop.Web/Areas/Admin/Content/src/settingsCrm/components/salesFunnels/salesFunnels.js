; (function (ng) {
    'use strict';

    var salesFunnelsCtrl = function ($q, $uibModal, uiGridConstants, uiGridCustomConfig, $http, toaster, SweetAlert, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.SalesFunnels.Name'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                            '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadSalesFunnel(row.entity.Id, $event)">{{COL_FIELD}}</a>' +
                        '</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SalesFunnels.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name'
                    }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.SalesFunnels.SortOrder'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SalesFunnels.SortOrder'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'SortOrder'
                    },
                    width: 200
                },
                {
                    name: 'Enable',
                    displayName: $translate.instant('Admin.Js.SalesFunnels.Activity'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch field-name="Enable" row="row" readonly="row.entity.IsDefaultFunnel"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SalesFunnels.Activity'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.SalesFunnels.TheyActive'), value: true }, { label: $translate.instant('Admin.Js.SalesFunnels.Inactive'), value: false }]
                    },
                    //cellEditableCondition: function ($scope) {
                    //    return !$scope.row.entity.IsDefaultFunnel || !$scope.row.entity.Enable
                    //}
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadSalesFunnel(row.entity.Id, $event)"></a>' +
                            '<ui-grid-custom-delete url="salesFunnels/deleteSalesFunnel" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SalesFunnels.DeleteSelected'),
                        url: 'salesFunnels/deleteSalesFunnels',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.$onInit = function () {

            ctrl.getFormData().then(ctrl.fetch);

            if (ctrl.onInit != null) {
                ctrl.onInit({ salesFunnels: ctrl });
            }
        };

        ctrl.fetch = function () {
            if (ctrl.grid != null) {
                ctrl.grid.fetchData();
            }
        };

        ctrl.getFormData = function () {
            return $http.get('salesFunnels/getFormData').then(function (response) {
                ctrl.canAddSalesFunnel = response.data.canAddSalesFunnel;
            });
        };

        ctrl.onAddEdit = function () {
            if (ctrl.grid != null) {
                ctrl.grid.fetchData();
            }

            if (ctrl.onChange != null) {
                ctrl.onChange();
            }
        };

        ctrl.loadSalesFunnel = function (id, $event) {
            if ($event) {
                $event.preventDefault();
            }

            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditSalesFunnelCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/settingsCrm/components/salesFunnels/modals/addEditSalesFunnel/addEditSalesFunnel.html',
                resolve: { Id: function () { return id; } },
                size: 'middle'
            }).result.then(function (result) {
                ctrl.fetch();
                return result;
            });
        };
    };

    salesFunnelsCtrl.$inject = ['$q', '$uibModal', 'uiGridConstants', 'uiGridCustomConfig', '$http', 'toaster', 'SweetAlert', '$translate'];

    ng.module('salesFunnels', ['as.sortable'])
        .controller('salesFunnelsCtrl', salesFunnelsCtrl)
        .component('salesFunnels', {
            templateUrl: '../areas/admin/content/src/settingsCrm/components/salesFunnels/salesFunnels.html',
            controller: 'salesFunnelsCtrl',
            controllerAs: 'ctrl',
            transclude: true,
            bindings: {
                onInit: '&',
                onChange: '&'
            }
        });

})(window.angular);