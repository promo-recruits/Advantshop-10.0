; (function (ng) {
    'use strict';

    var OrderStatusesCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $uibModal, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'Color',
                    displayName: '',
                    cellTemplate: '<div class="ui-grid-cell-contents"><i class="fa fa-circle" ng-style="{color:\'#\' + COL_FIELD}"></i></div>',
                    width: 40,
                },
                {
                    name: 'StatusName',
                    displayName: $translate.instant('Admin.Js.OrderStatuses.Name'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                            '<ui-modal-trigger data-controller="\'ModalAddEditOrderStatusCtrl\'" controller-as="ctrl" ' +
                                        'template-url="../areas/admin/content/src/orderstatuses/modal/AddEditOrderStatus.html" ' +
                                        'data-resolve="{\'orderStatusId\': row.entity.OrderStatusId}" ' +
                                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                                '<a href="">{{COL_FIELD}}</a> ' +
                            '</ui-modal-trigger>' +
                        '</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderStatuses.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    }
                },
                {
                    name: 'IsDefault',
                    displayName: $translate.instant('Admin.Js.OrderStatuses.DefaultValue'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label">' +
                                        '<input type="checkbox" disabled ng-model="row.entity.IsDefault" class="adv-checkbox-input control-checkbox" data-e2e="switchOnOffSelect" />' +
                                        '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                                    '</label></div>',
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderStatuses.DefaultValue'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'IsDefault',
                        selectOptions: [{ label: $translate.instant('Admin.Js.OrderStatuses.Yes'), value: true }, { label: $translate.instant('Admin.Js.OrderStatuses.No'), value: false }]
                    }
                },
                {
                    name: 'IsCanceled',
                    displayName: $translate.instant('Admin.Js.OrderStatuses.OrderCancel'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label">' +
                                        '<input type="checkbox" disabled ng-model="row.entity.IsCanceled" class="adv-checkbox-input control-checkbox" data-e2e="switchOnOffSelect" />' +
                                        '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                                    '</label></div>',
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderStatuses.OrderCancel'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'IsCanceled',
                        selectOptions: [{ label: $translate.instant('Admin.Js.OrderStatuses.Yes'), value: true }, { label: $translate.instant('Admin.Js.OrderStatuses.No'), value: false }]
                    }
                },
                {
                    name: 'IsCompleted',
                    displayName: $translate.instant('Admin.Js.OrderStatuses.OrderComplete'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label">' +
                                        '<input type="checkbox" disabled ng-model="row.entity.IsCompleted" class="adv-checkbox-input control-checkbox" data-e2e="switchOnOffSelect" />' +
                                        '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                                    '</label></div>',
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderStatuses.OrderComplete'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'IsCompleted',
                        selectOptions: [{ label: $translate.instant('Admin.Js.OrderStatuses.Yes'), value: true }, { label: $translate.instant('Admin.Js.OrderStatuses.No'), value: false }]
                    }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.OrderStatuses.Sorting'),
                    width: 100,
                },
                {
                    name: 'CommandFormatted',
                    displayName: $translate.instant('Admin.Js.OrderStatuses.Command'),
                    width: 200,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderStatuses.Command'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'CommandId',
                        fetch: 'orderstatuses/getcommands'
                    }
                },
                {
                    name: 'CancelForbidden',
                    displayName: $translate.instant('Admin.Js.OrderStatuses.CancelForbidden'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label">' +
                        '<input type="checkbox" disabled ng-model="row.entity.CancelForbidden" class="adv-checkbox-input control-checkbox" data-e2e="switchOnOffSelect" />' +
                        '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                        '</label></div>',
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderStatuses.CancelForbidden'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'CancelForbidden',
                        selectOptions: [{ label: $translate.instant('Admin.Js.OrderStatuses.Yes'), value: true }, { label: $translate.instant('Admin.Js.OrderStatuses.No'), value: false }]
                    }
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openModal(row.entity.OrderStatusId)"></a> ' +
                            '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.CanBeDeleted, row.entity.OrderStatusId, row.entity.CanAddDelete)" ng-class="(!row.entity.CanBeDeleted || !row.entity.CanAddDelete ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')"></a> ' +
                        '</div></div>'
                }
            ];


        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.openModal(row.entity.OrderStatusId);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.OrderStatuses.DeleteSelected'),
                        url: 'orderstatuses/deleteOrderSatuses',
                        field: 'OrderStatusId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.OrderStatuses.AreYouSureDelete'), { title: $translate.instant('Admin.Js.OrderStatuses.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.delete = function (canBeDeleted, orderStatusId, canAddDelete) {

            if (!canAddDelete) {
                SweetAlert.alert($translate.instant('Admin.Js.OrderStatuses.DeletingImpossibleByTariff'), { title: $translate.instant('Admin.Js.OrderStatuses.DeletingImpossible') });
                return;
            }

            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.OrderStatuses.AreYouSureDelete'), { title: $translate.instant('Admin.Js.OrderStatuses.Deleting') }).then(function (result) {
                    if (result === true) {
                        $http.post('orderstatuses/deleteOrderStatus', { 'OrderStatusId': orderStatusId }).then(function (response) {
                            ctrl.grid.fetchData();
                        });
                    }
                });
            } else {
                SweetAlert.alert($translate.instant('Admin.Js.OrderStatuses.StatusUsedInOrders'), { title: $translate.instant('Admin.Js.OrderStatuses.DeletingImpossible') });
            }
        }

        ctrl.openModal = function (orderStatusId) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditOrderStatusCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/orderstatuses/modal/AddEditOrderStatus.html',
                resolve: {
                    orderStatusId: function () {
                        return orderStatusId;
                    }
                }
            }).result.then(function (result) {
                ctrl.grid.fetchData();
                return result;
            }, function (result) {
                ctrl.grid.fetchData();
                return result;
            });
        };
    };

    OrderStatusesCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', '$uibModal', '$translate'];


    ng.module('orderstatuses', ['uiGridCustom', 'urlHelper'])
      .controller('OrderStatusesCtrl', OrderStatusesCtrl);

})(window.angular);