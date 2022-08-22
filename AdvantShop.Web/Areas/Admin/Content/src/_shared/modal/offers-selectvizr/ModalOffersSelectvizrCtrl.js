; (function (ng) {
    'use strict';

    var ModalOffersSelectvizrCtrl = function ($uibModalInstance, uiGridCustomConfig, uiGridConstants, $http, domService, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.selectvizrTreeUrl = 'catalog/categoriestree';
            ctrl.selectvizrGridUrl = 'catalog/getOffersCatalog';

            ctrl.selectvizrGridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                  {
                      name: 'ArtNo',
                      displayName: $translate.instant('Admin.Js.OfferSelect.VendorCode'),
                      width: 100,
                      enableSorting: false,
                      visible: 500
                  },
                  {
                      name: 'Name',
                      displayName: $translate.instant('Admin.Js.OffersSelect.Name'),
                      enableSorting: false,
                  },
                  {
                      name: 'ColorName',
                      displayName: $translate.instant('Admin.Js.OffersSelect.Color'),
                      width: 100,
                      enableSorting: false,
                      visible: 900
                  },
                  {
                      name: 'SizeName',
                      displayName: $translate.instant('Admin.Js.OffersSelect.Size'),
                      width: 100,
                      enableSorting: false,
                      visible: 1210
                  },
                  {
                      name: 'PriceFormatted',
                      displayName: $translate.instant('Admin.Js.OffersSelect.Price'),
                      width: 120,
                      enableSorting: false,
                  },
                  {
                      name: 'Amount',
                      displayName: $translate.instant('Admin.Js.OffersSelect.ProductCount'),
                      width: 100,
                      enableSorting: false,
                      visible: 1250
                  }
                ],

                showTreeExpandNoChildren: false,
                uiGridCustom: {
                    rowClick: function ($event, row, grid) {
                        if (row.treeNode.children && row.treeNode.children.length > 0 && domService.closest($event.target, '.ui-grid-tree-base-row-header-buttons') == null) {
                            grid.gridApi.treeBase.toggleRowTreeState(row);
                        }
                    },
                    rowClasses: function (row) {
                        return row.treeNode.children == null || row.treeNode.children.length === 0 ? 'ui-grid-custom-prevent-pointer' : '';
                    }
                }
            });

            uiGridCustomConfig.enableHorizontalScrollbar = 1;

            if (ctrl.$resolve.multiSelect === false) {
                ng.extend(ctrl.selectvizrGridOptions, {
                    multiSelect: false,
                    modifierKeysToMultiSelect: false,
                    enableRowSelection: true,
                    enableRowHeaderSelection: false
                });
            }
        };

        ctrl.onChange = function (categoryId, ids, selectMode) {
            ctrl.data = {
                categoryId: categoryId,
                ids: ids,
                selectMode: selectMode
            }
        };

        ctrl.gridOnFetch = function (grid) {
            if (grid != null && grid.gridOptions != null && grid.gridOptions.data != null && grid.gridOptions.data.length > 0) {
                for (var i = 0, len = grid.gridOptions.data.length; i < len; i++) {
                    if (grid.gridOptions.data[i].Main === true) {
                        grid.gridOptions.data[i].$$treeLevel = 0;
                    }
                }
            }
        }

        ctrl.select = function () {
            if (ctrl.data.selectMode == "all") {
                $http.get('catalog/getCatalogOfferIds', { params: ctrl.data }).then(function (response) {
                    if (response.data != null) {
                        ctrl.data.selectMode = "none";
                        ctrl.data.ids = response.data.ids.filter(function (item) {
                            return ctrl.data.ids.indexOf(item) === -1;
                        });
                    }
                    $uibModalInstance.close(ctrl.data);
                });
            } else {
                $uibModalInstance.close(ctrl.data);
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalOffersSelectvizrCtrl.$inject = ['$uibModalInstance', 'uiGridCustomConfig', 'uiGridConstants', '$http', 'domService', '$translate'];

    ng.module('uiModal')
        .controller('ModalOffersSelectvizrCtrl', ModalOffersSelectvizrCtrl);

})(window.angular);