; (function (ng) {
    'use strict';

    var OrdersCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, urlHelper, $q, SweetAlert, lastStatisticsService, adminWebNotificationsService, adminWebNotificationsEvents, $translate, $scope) {

        var ctrl = this;

        ctrl.init = function (showManagers) {

            adminWebNotificationsService.addListener(adminWebNotificationsEvents.updateOrders, function () { ctrl.gridUpdate(); });

            ctrl.showManagers = showManagers;

            var columnDefs = [
                {
                    name: '_noopColumnNumber',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.NumberOfOrder'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Number',
                    }
                },
                {
                    name: '_noopColumnStatuses',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.Status'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'OrderStatusId',
                        fetch: 'orders/getorderstatuses'
                    }
                },
                {
                    name: '_noopColumnSum',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.Cost'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'PriceFrom'
                            },
                            to: {
                                name: 'PriceTo'
                            },
                        }
                    }
                },
                {
                    name: '_noopColumnIsPaid',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.Payment'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'IsPaid',
                        selectOptions: [{ label: $translate.instant('Admin.Js.Orders.Yes'), value: true }, { label: $translate.instant('Admin.Js.Orders.No'), value: false }]
                    }
                },
                {
                    name: '_noopColumnName',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.CustomersFullName'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerName'
                    }
                },
                {
                    name: '_noopColumnPhone',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.CustomersPhone'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerPhone'
                    }
                },
                {
                    name: '_noopColumnEmail',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.CustomersEmail'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerEmail'
                    }
                },
                {
                    name: '_noopColumnCity',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.CustomersCity'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerCity'
                    }
                },
                {
                    name: '_noopColumnProduct',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.NameOrVendorCodeOfProduct'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ProductNameArtNo',
                    }
                },
            ];

            if (ctrl.showManagers) {
                columnDefs.push({
                    name: '_noopColumnManager',
                    visible: false,
                    enableHiding: false,
                    enabled: ctrl.showManagers,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.Manager'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'ManagerId',
                        fetch: 'managers/getManagersSelectOptions?includeEmpty=true'
                    }
                });
            }

            columnDefs = columnDefs.concat([
                {
                    name: '_noopColumnShippings',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.ShippingMethods'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ShippingMethod',
                        //fetch: 'orders/getordershippingmethods'
                    }
                },
                {
                    name: '_noopColumnPayments',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.PaymentMethod'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'PaymentMethod',
                        fetch: 'orders/getorderpaymentmethods'
                    }
                },
                {
                    name: '_noopColumnSources',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.OrderSource'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'OrderSourceId',
                        fetch: 'orders/getordersources'
                    }
                },
                {
                    name: 'Number',
                    displayName: $translate.instant('Admin.Js.Orders.Number'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a ng-href="orders/edit/{{row.entity.OrderId}}">{{COL_FIELD}}</a></div>',
                    width: 90
                },
                {
                    name: 'StatusName',
                    displayName: $translate.instant('Admin.Js.Orders.Status'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><i class="fa fa-circle m-r-xs" style="color:#{{row.entity.Color}}"></i> {{row.entity.StatusName}}</div>',
                },
                {
                    name: 'BuyerName',
                    displayName: $translate.instant('Admin.Js.Orders.Customer'),
                },
                {
                    name: 'OrderItems',
                    displayName: $translate.instant('Admin.Js.Orders.OrderItems'),
                    width: 230,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div ng-repeat="item in row.entity.OrderItems | limitTo:5 track by $index">' +
                            '<div class="m-b-xs" ng-bind="item"></div>' +
                        '</div><div ng-if="row.entity.OrderItems.length > 5">и другие</div></div>',
                    visible: false
                },
                {
                    name: 'SumFormatted',
                    displayName: $translate.instant('Admin.Js.Orders.Amount'),
                    width: 100,
                },
                {
                    name: 'IsPaid',
                    displayName: $translate.instant('Admin.Js.Orders.Payment'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div class="adv-checkbox-label">' +
                        '<input type="checkbox" ng-model="row.entity.IsPaid" readonly class="adv-checkbox-input control-checkbox pointer-events-none" data-e2e="switchOnOffSelect" />' +
                        '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                        '</div></div>',
                    width: 65,
                    headerCellClass: 'ui-grid-text-center',
                    cellClass: 'ui-grid-text-center'
                },
                {
                    name: 'PaymentMethod',
                    displayName: $translate.instant('Admin.Js.Orders.PaymentMethod'),
                    visible: 1367
                },
                {
                    name: 'ShippingMethod',
                    displayName: $translate.instant('Admin.Js.Orders.ShippingMethods'),
                    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.ShippingMethod != null && row.entity.ShippingMethod.length > 0 ? row.entity.ShippingMethod : row.entity.ShippingMethodName}}</div>',
                    visible: 1367
                },
                {
                    name: 'ManagerName',
                    displayName: $translate.instant('Admin.Js.Orders.Manager'),
                    visible: ctrl.showManagers && 1441,
                    enableHiding: ctrl.showManagers
                },
                {
                    name: 'AdminOrderComment',
                    displayName: $translate.instant('Admin.Js.Orders.AdminOrderComment'),
                    visible: ctrl.showManagers && 1601,
                    enableHiding: ctrl.showManagers 
                },
                {
                    name: 'OrderDateFormatted',
                    displayName: $translate.instant('Admin.Js.Orders.DateAndTime'),
                    width: 114,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.DateAndTime'),
                        type: 'datetime',
                        term: {
                            from: new Date((new Date()).setMonth((new Date()).getMonth() - 1)),
                            to: new Date()
                        },
                        datetimeOptions: {
                            from: {
                                name: 'OrderDateFrom'
                            },
                            to: {
                                name: 'OrderDateTo'
                            }
                        }
                    }
                },
                {
                    name: '_noopColumnCouponCode',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.CouponCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'CouponCode',
                    }
                },
                {
                    name: '_noopColumnDeliveryDate',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.DeliveryDate'),
                        type: 'date',
                        term: {
                            from: new Date((new Date()).setMonth((new Date()).getMonth() - 1)),
                            to: new Date()
                        },
                        dateOptions: {
                            from: { name: 'DeliveryDateFrom' },
                            to: { name: 'DeliveryDateTo' }
                        }
                    }
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    enableHiding: false,
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked">' +
                        '<a ng-href="orders/edit/{{row.entity.OrderId}}" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a>' +
                        '<ui-grid-custom-delete url="orders/deleteorder" params="{\'OrderId\': row.entity.OrderId }"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ]);


            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                enableGridMenu: true,
                columnDefs: columnDefs,
                uiGridCustom: {
                    rowUrl: 'orders/edit/{{row.entity.OrderId}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.Orders.DeleteSelected'),
                            url: 'orders/deleteorders',
                            field: 'OrderId',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.Orders.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Orders.Deleting') }).then(function (result) {
                                    return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            },
                            after: function () {
                                lastStatisticsService.getLastStatistics();
                            },
                        },
                        {
                            template:
                                '<ui-modal-trigger data-controller="\'ModalChangeOrderStatusesCtrl\'" controller-as="ctrl" data-resolve=\"{params:$ctrl.getSelectedParams(\'OrderId\')}\" ' +
                                'template-url="../areas/admin/content/src/orders/modal/ChangeOrderStatuses.html" ' +
                                'data-on-close="$ctrl.gridOnAction()">' +
                                $translate.instant('Admin.Js.Orders.ChangeStatusToSelected') + '</ui-modal-trigger>'
                        },
                        {
                            text: $translate.instant('Admin.Js.Orders.MarkAsPaid'),
                            url: 'orders/markpaid',
                            field: 'OrderId'
                        },
                        {
                            text: $translate.instant('Admin.Js.Orders.MarkAsUnpaid'),
                            url: 'orders/marknotpaid',
                            field: 'OrderId'
                        }
                    ]
                }
            });
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.gridUpdate = function(){
            ctrl.grid.fetchData(true);
        };
    };

    OrdersCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', 'urlHelper', '$q', 'SweetAlert', 'lastStatisticsService', 'adminWebNotificationsService', 'adminWebNotificationsEvents', '$translate', '$scope'];


    ng.module('orders', ['uiGridCustom', 'urlHelper'])
      .controller('OrdersCtrl', OrdersCtrl);

})(window.angular);