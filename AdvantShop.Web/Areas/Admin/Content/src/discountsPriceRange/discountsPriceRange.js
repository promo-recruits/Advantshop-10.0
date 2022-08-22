; (function (ng) {
    'use strict';

    var DiscountsPriceRangeCtrl = function (uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, toaster, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'PriceRange',
                    displayName: $translate.instant('Admin.Js.PriceRange.OrderAmountOver'),
                    enableCellEdit: true,
                    type: 'number',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.PriceRange.OrderAmountOver'),
                        type: 'number',
                        name: 'PriceRange',
                    }
                },
                {
                    name: 'PercentDiscount',
                    displayName: $translate.instant('Admin.Js.PriceRange.Discount'),
                    enableCellEdit: true,
                    type: 'number',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.PriceRange.Discount'),
                        type: 'number',
                        name: 'PercentDiscount',
                    }
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-modal-trigger data-controller="\'ModalAddEditDiscountsPriceRangeCtrl\'" controller-as="ctrl" ' +
                                        'template-url="../areas/admin/content/src/discountsPriceRange/modal/addEditDiscountsPriceRange/AddEditDiscountsPriceRange.html" ' +
                                        'data-resolve="{\'OrderPriceDiscountId\': row.entity.OrderPriceDiscountId}" ' +
                                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                                '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a> ' +
                            '</ui-modal-trigger>' +
                            '<ui-grid-custom-delete url="discountsPriceRange/deleteItem" params="{\'OrderPriceDiscountId\': row.entity.OrderPriceDiscountId}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];


        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.PriceRange.DeleteSelected'),
                        url: 'discountsPriceRange/deleteItems',
                        field: 'OrderPriceDiscountId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.PriceRange.AreYouSureDelete'), { title: $translate.instant('Admin.Js.PriceRange.Deleting') }).then(function (result) {
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

        ctrl.changeDiscountsState = function() {
            $http.post('discountsPriceRange/enableDiscounts', { state: ctrl.enableDiscounts }).then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.PriceRange.ChangesSuccessfullySaved'));
            });
        };
    };

    DiscountsPriceRangeCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', 'toaster', '$translate'];


    ng.module('discountsPriceRange', ['uiGridCustom', 'urlHelper'])
      .controller('DiscountsPriceRangeCtrl', DiscountsPriceRangeCtrl);

})(window.angular);