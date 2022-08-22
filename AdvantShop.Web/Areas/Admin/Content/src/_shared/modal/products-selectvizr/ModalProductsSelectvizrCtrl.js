; (function (ng) {
    'use strict';

    var ModalProductsSelectvizrCtrl = function ($uibModalInstance, uiGridCustomConfig, uiGridConstants, $http, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.selectvizrTreeUrl = 'catalog/categoriestree';
            ctrl.selectvizrGridUrl = 'catalog/getcatalog';
            ctrl.data = [];
            ctrl.itemsSelected = ctrl.$resolve != null && ctrl.$resolve.value != null ? ng.copy(ctrl.$resolve.value.itemsSelected) : null;

            ctrl.selectvizrGridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'ProductArtNo',
                        displayName: $translate.instant('Admin.Js.ProductSelect.VendorCode'),
                        width: 100,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.ProductSelect.VendorCode'),
                            type: uiGridConstants.filter.INPUT,
                            name: 'ArtNo',
                        }
                    },
                    {
                        name: 'Name',
                        displayName: $translate.instant('Admin.Js.ProductSelect.Name'),
                        filter: {
                            placeholder: $translate.instant('Admin.Js.ProductSelect.Name'),
                            type: uiGridConstants.filter.INPUT,
                            name: 'Name',
                        }
                    },
                    //{
                    //    visible: false,
                    //    name: 'PhotoSrc',
                    //    filter: {
                    //        placeholder: 'Изображение',
                    //        type: uiGridConstants.filter.SELECT,
                    //        name: 'HasPhoto',
                    //        selectOptions: [{ label: 'С фотографией', value: true }, { label: 'Без фотографии', value: false }]
                    //    }
                    //},
                    {
                        visible: false,
                        name: 'BrandId',
                        filter: {
                            placeholder: $translate.instant('Admin.Js.ProductSelect.Manufacturer'),
                            type: uiGridConstants.filter.SELECT,
                            name: 'BrandId',
                            fetch: 'catalog/getBrandList'
                        }
                    },
                    {
                        visible: false,
                        name: 'Price',
                        filter: {
                            placeholder: $translate.instant('Admin.Js.ProductSelect.Price'),
                            type: 'range',
                            rangeOptions: {
                                from: {
                                    name: 'PriceFrom'
                                },
                                to: {
                                    name: 'PriceTo'
                                },
                            },
                            fetch: 'catalog/getpricerangeforpaging'
                        }
                    },
                    {
                        visible: false,
                        name: 'Amount',
                        filter: {
                            placeholder: $translate.instant('Admin.Js.ProductSelect.Quantity'),
                            type: 'range',
                            rangeOptions: {
                                from: {
                                    name: 'AmountFrom'
                                },
                                to: {
                                    name: 'AmountTo'
                                }
                            },
                            fetch: 'catalog/getamountrangeforpaging'
                        }
                    },
                    {
                        visible: true,
                        name: 'Enabled',
                        displayName: 'Активность',
                        enableCellEdit: false,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.ProductSelect.Activity'),
                            type: uiGridConstants.filter.SELECT,
                            selectOptions: [{ label: $translate.instant('Admin.Js.ProductSelect.TheyActive'), value: true }, { label: $translate.instant('Admin.Js.ProductSelect.Inactive'), value: false }]
                        },
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label">' +
                            '<input type="checkbox" disabled ng-model="row.entity.Enabled" class="adv-checkbox-input control-checkbox" data-e2e="switchOnOffSelect" />' +
                            '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                            '</label></div>'
                    }

                ],
                enableFullRowSelection: true
            });

            if (ctrl.$resolve.multiSelect === false) {
                ng.extend(ctrl.selectvizrGridOptions, {
                    multiSelect: false,
                    modifierKeysToMultiSelect: false,
                    enableRowSelection: true,
                    enableRowHeaderSelection: true
                });
            }
        };

        ctrl.onChange = function (categoryId, gridParams) {

            if (gridParams == null) {
                return;
            }

            var itemIndex;
            for (var i = 0, len = ctrl.data.length; i < len; i++) {
                if (ctrl.data[i].categoryId === categoryId) {
                    itemIndex = i;
                    break;
                }
            }

            if (itemIndex != null) {
                ng.extend(ctrl.data[itemIndex], gridParams);
                //ctrl.data[itemIndex].ids = gridParams.ids;
                //ctrl.data[itemIndex].selectMode = gridParams.selectMode;
            } else {
                ctrl.data.push(
                    ng.extend({ categoryId: categoryId }, gridParams)
                    //{ categoryId: categoryId, ids: gridParams.ids, selectMode: gridParams.selectMode }
                )
            }
        };

        ctrl.select = function () {

            var promiseArray;

            ctrl.data.forEach(function (dataItem) {
                if (dataItem.selectMode == "all") {
                    var promise = $http.get('catalog/getCatalogIds', { params: dataItem }).then(function (response) {
                        if (response.data != null) {
                            dataItem.selectMode = 'none';
                            dataItem.ids = response.data.ids.filter(function (item) {
                                return dataItem.ids.indexOf(item) === -1;
                            });
                        }

                        return dataItem;
                    });

                    promiseArray = promiseArray || [];

                    promiseArray.push(promise);
                }
            });


            $q.all(promiseArray || ctrl.data).then(function (data) {
                var allIds = data.reduce(function (previousValue, currentValue) {
                    return previousValue.concat(currentValue.ids);
                }, [])

                var uniqueItems = [];

                allIds.concat(ctrl.itemsSelected || []).forEach(function (item) {
                    uniqueItems.indexOf(item) === -1 ? uniqueItems.push(item) : null;
                });


                $uibModalInstance.close({ ids: uniqueItems });
            })
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalProductsSelectvizrCtrl.$inject = ['$uibModalInstance', 'uiGridCustomConfig', 'uiGridConstants', '$http', '$q', '$translate'];

    ng.module('uiModal')
        .controller('ModalProductsSelectvizrCtrl', ModalProductsSelectvizrCtrl);

})(window.angular);