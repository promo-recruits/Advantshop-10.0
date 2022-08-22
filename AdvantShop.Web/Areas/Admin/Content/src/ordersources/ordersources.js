; (function (ng) {
    'use strict';

    var OrderSourcesCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $uibModal, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.OrderSources.Name'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                            '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openModal(row.entity.Id)">{{COL_FIELD}}</a> ' +
                        '</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderSources.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    }
                },
                {
                    name: 'TypeFormatted',
                    displayName: $translate.instant('Admin.Js.OrderSources.Group'),
                    width: 220,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<span class="">{{COL_FIELD}}</span>' +
                            '<a ng-if="row.entity.TypeString == \'LandingPage\' && row.entity.ObjId != null" href="funnels/site/{{row.entity.ObjId}}" target="_blank" class="fas fa-edit text-decoration-none link-invert p-l-xs" title="{{\'Admin.Js.OrderSources.GoToFunnel\' | translate}}"></a>' +
                        '</div></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderSources.Group'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Type',
                        fetch: 'ordersources/getTypes'
                    }

                },
                {
                    name: 'Main',
                    displayName: $translate.instant('Admin.Js.OrderSources.MainInGroup'),
                    width: 90,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                            '<label class="ui-grid-custom-edit-field adv-checkbox-label"> ' +
                                '<input type="checkbox" class="adv-checkbox-input" data-e2e="switchOnOffInput" ng-model="row.entity.Main" disabled /> ' +
                                '<span class="adv-checkbox-emul" data-e2e="switchOnOffSelect"></span> ' +
                            '</label>' +
                        '</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderSources.MainInGroup'),
                        name: 'Main',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.Yes'), value: 'true' }, { label: $translate.instant('Admin.Js.No'), value: 'false' }]
                    }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.OrderSources.SortingOrder'),
                    width: 90,
                },
                {
                    name: 'OrdersCount',
                    displayName: $translate.instant('Admin.Js.OrderSources.AmountOfOrders'),
                    width: 90,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderSources.AmountOfOrders'),
                        type: 'range',
                        rangeOptions: {
                            from: { name: 'OrdersCountFrom' },
                            to: { name: 'OrdersCountTo' }
                        }
                    }
                },
                {
                    name: 'LeadsCount',
                    displayName: $translate.instant('Admin.Js.OrderSources.CountOfLeads'),
                    width: 90,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OrderSources.CountOfLeads'),
                        type: 'range',
                        rangeOptions: {
                            from: { name: 'LeadsCountFrom' },
                            to: { name: 'LeadsCountTo' }
                        }
                    }
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openModal(row.entity.Id)"></a> ' +
                            ' <a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.CanBeDeleted, row.entity.Id)" ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')"></a> ' +
                        '</div></div>'
                }
            ];


        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.openModal(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.OrderSources.DeleteSelected'),
                        url: 'ordersources/deleteOrderSources',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.OrderSources.AreYouSureDelete'), { title: $translate.instant('Admin.Js.OrderSources.Deleting') }).then(function (result) {
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

        ctrl.delete = function (canBeDeleted, id) {

            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.OrderSources.AreYouSureDelete'), { title: $translate.instant('Admin.Js.OrderSources.Deleting') }).then(function (result) {
                    if (result === true) {
                        $http.post('ordersources/deleteOrderSource', { 'id': id }).then(function (response) {
                            ctrl.grid.fetchData();
                        });
                    }
                });
            } else {
                SweetAlert.alert($translate.instant('Admin.Js.OrderSources.DeletingImpossible'), { title: $translate.instant('Admin.Js.OrderSources.DeletingIsImpossible') });
            }
        }

        ctrl.openModal = function (id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditOrderSourceCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/ordersources/modal/AddEditOrderSource.html',
                resolve: {
                    id: function () {
                        return id;
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

    OrderSourcesCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', '$uibModal', '$translate'];


    ng.module('ordersources', ['uiGridCustom', 'urlHelper'])
      .controller('OrderSourcesCtrl', OrderSourcesCtrl);

})(window.angular);