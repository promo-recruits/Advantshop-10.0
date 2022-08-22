﻿; (function (ng) {
    'use strict';

    var CustomerGroupsCtrl = function ($window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'GroupName',
                    displayName: $translate.instant('Admin.Js.Customergroups.NameOfGroup'),
                    enableCellEdit: true
                },
                {
                    name: 'GroupDiscount',
                    displayName: $translate.instant('Admin.Js.Customergroups.Discount'),
                    enableCellEdit: true
                },
                {
                    name: 'MinimumOrderPrice',
                    displayName: $translate.instant('Admin.Js.Customergroups.MinimumOrderAmount'),
                    enableCellEdit: true
                },
                {
                    name: '_customersColumn',
                    displayName: '',
                    cellTemplate: '<div class="ui-grid-cell-contents"> <a href="customers#?gridCustomers={%22GroupId%22:%22{{row.entity.CustomerGroupId}}%22}" class="link" target="_blank">' + $translate.instant('Admin.Js.Customergroups.ViewCustomers') + '</a> </div>',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 55,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            //'<ui-grid-custom-delete data-ng-show="{{row.entity.CanBeDeleted}}" url="customergroups/deletecustomergroup" params="{\'CustomerGroupId\': row.entity.CustomerGroupId}"></ui-grid-custom-delete>' +
                            ' <a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.CanBeDeleted, row.entity.CustomerGroupId)" ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')"></a> ' +
                        '</div></div>'
                }
            ];


        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Customergroups.DeleteSelected'),
                        url: 'customergroups/deletecustomergroups',
                        field: 'CustomerGroupId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Customergroups.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Customergroups.Deleting') }).then(function (result) {
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

        ctrl.delete = function (canBeDeleted, groupId) {

            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.Customergroups.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Customergroups.Deleting') }).then(function (result) {
                    if (result === true) {
                        $http.post('customergroups/deletecustomergroup', { 'CustomerGroupId': groupId }).then(function (response) {
                            ctrl.grid.fetchData();
                        });
                    }
                });
            } else {
                SweetAlert.alert($translate.instant('Admin.Js.Customergroups.CanNotDeleteDefaultGroup'), { title: $translate.instant('Admin.Js.Customergroups.DeletingImpossible') });
            }
        }
    };

    CustomerGroupsCtrl.$inject = ['$window', 'uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', '$translate'];


    ng.module('customergroups', ['uiGridCustom', 'urlHelper'])
      .controller('CustomerGroupsCtrl', CustomerGroupsCtrl);

})(window.angular);