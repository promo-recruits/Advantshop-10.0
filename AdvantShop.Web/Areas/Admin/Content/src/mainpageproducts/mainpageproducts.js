﻿; (function (ng) {
    'use strict';

    var MainPageProductsCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, urlHelper, mainpageproductsService, $q, SweetAlert, toaster, $translate, $http) {
        var ctrl = this,
            type = urlHelper.getUrlParam('type'),
            deferProductList,
            columnDefs = [
                {
                    name: '_noopColumnArtNo',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Catalog.VendorCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ArtNo',
                    }
                },
                {
                    name: '_noopColumnName',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Catalog.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    }
                },
                {
                    name: '_noopColumnEnabled',
                    visible: false,
                    filter: {
                        name: 'Enabled',
                        placeholder: $translate.instant('Admin.Js.Catalog.Activity'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.Catalog.TheyActive'), value: true }, { label: $translate.instant('Admin.Js.Catalog.Inactive'), value: false }]
                    }
                },

                {
                    name: 'ProductArtNo',
                    displayName: $translate.instant('Admin.Js.MainPageProducts.VendorCode'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a class="link-invert" ng-href="product/edit/{{row.entity.ProductId}}">{{COL_FIELD}}</a></div>',
                    width: 100
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.MainPageProducts.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="product/edit/{{row.entity.ProductId}}">{{COL_FIELD}}</a></div>',
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.MainPageProducts.Order'),
                    width: 100,
                    type: 'number',
                    enableCellEdit: true,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<a ng-href="product/edit/{{row.entity.ProductId}}" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a>' +
                        '<ui-grid-custom-delete url="mainpageproducts/deletefromlist" params="{\'ProductId\': row.entity.ProductId, type: (grid.appScope.$ctrl.gridExtendCtrl.selectedList != null ? grid.appScope.$ctrl.gridExtendCtrl.selectedList.TypeStr : \'' + type + '\')}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.MainPageProducts.DeleteSelectedFromList'),
                        url: 'mainpageproducts/deleteProductsFromList',
                        field: 'ProductId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.MainPageProducts.AreYouSureDelete'), { title: $translate.instant('Admin.Js.MainPageProducts.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },
                    {
                        template: '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalMoveProductInOtherCategoryCtrl\'" controller-as="ctrl" ' +
                            'data-resolve=\"{params:$ctrl.getSelectedParams(\'ProductId\')}\" template-url="../areas/admin/content/src/_shared/modal/moveProductInOtherCategory/moveProductInOtherCategory.html">' +
                            $translate.instant('Admin.Js.Catalog.AddProductsAnotherCategory') + '</ui-modal-trigger>'
                    },
                    {
                        text: $translate.instant('Admin.Js.Catalog.MakeActive'),
                        url: 'catalog/activateproducts',
                        field: 'ProductId'
                    },
                    {
                        text: $translate.instant('Admin.Js.Catalog.MakeInactive'),
                        url: 'catalog/disableproducts',
                        field: 'ProductId'
                    },
                    {
                        template: '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalCopyProductCtrl\'" controller-as="ctrl" ' +
                            'data-resolve=\"{product:$ctrl.getSelectedParams(\'ProductId\'), name: $ctrl.getSelectedParams(\'Name\')}\" template-url="../areas/admin/content/src/product/modal/copyProduct/copyProduct.html">' +
                            $translate.instant('Admin.Js.Catalog.CreateCopyOfProduct') + '</ui-modal-trigger>'
                    },
                    {
                        template: '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalAddRemovePropertyToProductsCtrl\'" controller-as="ctrl" ' +
                            'data-resolve=\"{params:$ctrl.getSelectedParams(\'ProductId\'), mode:{remove: false}}\" template-url="../areas/admin/content/src/_shared/modal/addRemovePropertyToProducts/addRemovePropertyToProducts.html" size="md">' +
                            $translate.instant('Admin.Js.Catalog.AddPropertyToProducts') + '</ui-modal-trigger>'
                    },
                    {
                        template: '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalAddRemovePropertyToProductsCtrl\'" controller-as="ctrl" ' +
                            'data-resolve=\"{params:$ctrl.getSelectedParams(\'ProductId\'), mode:{remove: true}}\" template-url="../areas/admin/content/src/_shared/modal/addRemovePropertyToProducts/addRemovePropertyToProducts.html" size="md">' +
                            $translate.instant('Admin.Js.Catalog.RemovePropertyFromProducts') + '</ui-modal-trigger>'
                    }
                ]
            }
        });

        ctrl.$onInit = function () {
            ctrl.gridUniqueId = 'gridMainPageProducts';
        };

        ctrl.addProductsModal = function (result) {
            var params = {
                type: ctrl.selectedList != null ? ctrl.selectedList.TypeStr : type
            };

            mainpageproductsService.addProducts(ng.extend(params, result)).then(function (data) {
                var _result;

                if (data.result === true) {
                    _result = ctrl.grid.fetchData();

                    if (ctrl.catalogLeftMenu != null) {
                        ctrl.catalogLeftMenu.updateData();
                    }
                } else {
                    return $q.reject(new Error());
                }
                return _result;
            }).catch(function () {
                toaster.pop('error', $translate.instant('Admin.Js.MainPageProducts.ErrorAddingProducts'));
            });
        };

        ctrl.onInitGrid = function (grid) {
            ctrl.grid = grid;
            //if (ctrl.selectedList != null) {
            //    ctrl.grid.setParams({ type: ctrl.selectedList.TypeStr });
            //    ctrl.grid.fetchData();
            //}
        };

        ctrl.initCatalogLeftMenu = function (catalogLeftMenu) {
            ctrl.catalogLeftMenu = catalogLeftMenu;
        };

        ctrl.onGridDeleteItem = function () {
            if (ctrl.catalogLeftMenu != null) {
                ctrl.catalogLeftMenu.updateData();
            }
        };

        ctrl.init = function (typeStr, listId) {
            if (listId != null && listId > 0) {
                typeStr = 'list';
                ctrl.showMode = 'list';
            }

            ctrl.getItemByType(typeStr, listId).then(function (data) {
                if (listId != null) {
                    ctrl.mapDataForList(data);
                }
            });
        };

        ctrl.getItemByType = function (type, id) {
            return $http.get('mainpageProductsStore/getItemByType', { params: { type: type, id: id, rnd: Math.random() } })
                .then(function (response) {
                    return ctrl.selectedList = response.data;
                });
        };

        /* product lists */
        ctrl.initProductLists = function (productLists) {
            ctrl.productLists = productLists;

            if (deferProductList != null) {
                deferProductList.resolve(productLists);
            }
        };

        ctrl.updateProductLists = function () {
            return ctrl.productLists.fetch().then(function (data) {
                return ctrl.getItemByType('list', ctrl.selectedList.Id);
            });
        };

        ctrl.getProductList = function () {
            if (ctrl.productLists != null) {
                return $q.resolve(ctrl.productLists)
            } else {
                deferProductList = $q.defer();
                return deferProductList.promise;
            }
        }

        ctrl.onChangeList = function (list) {
            if (list == null) {
                ctrl.updateProductLists().then(function () {
                    ctrl.getItemByType('best', null);
                });
                return;
            }

            ctrl.selectedList = list;

            ctrl.getItemByType('list', list.Id).then(function () {
                ctrl.showMode = 'list';

                ctrl.mapDataForList(ctrl.selectedList);

                if (ctrl.productlistsCtrl.gridProducts != null) {
                    ctrl.productlistsCtrl.gridProducts.clearParams();
                    ctrl.productlistsCtrl.gridProducts.setParams({ listId: ctrl.selectedList.Id });
                    ctrl.productlistsCtrl.gridProducts.fetchData();
                }
            });
        };

        ctrl.mapDataForList = function (data) {
            return ctrl.getProductList().then(function () {
                ctrl.productLists.listId = data.Id;
                ctrl.productlistsCtrl.listId = data.Id;
                return data;
            });
        };

        /* best, new, sale */
        ctrl.changeByType = function (typeStr) {
            ctrl.getItemByType(typeStr, null).then(function () {
                ctrl.showMode = null;
                ctrl.productLists.listId = null;

                if (ctrl.grid != null) {
                    ctrl.grid.clearParams();
                    ctrl.gridUniqueId = 'gridMainPageProducts' + typeStr;
                    ctrl.grid.gridUniqueId = ctrl.gridUniqueId;
                    ctrl.grid.setParams({ type: typeStr });
                    ctrl.grid.fetchData();
                }
            });
        };

        ctrl.changeEnabled = function () {
            $http.post('mainpageProductsStore/changeEnabled', { type: ctrl.selectedList.TypeStr, enabled: ctrl.selectedList.Enabled, id: ctrl.selectedList.Id })
                .then(function (response) {
                    if (response.data != null && response.data.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.onAddList = function (result) {
            ctrl.updateProductLists().then(function (data) {
                ctrl.onChangeList({ Id: result.id });
            });
        };

        ctrl.changeDisplayLatestProductsInNewOnMainPageEnabled = function () {
            $http.post('mainpageProductsStore/changeDisplayLatestProductsInNewOnMainPageEnabled', { enabled: ctrl.selectedList.DisplayLatestProductsInNewOnMainPage })
                .then(function (response) {
                    if (response.data != null && response.data.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.changeShuffleList = function () {
            $http.post('mainpageProductsStore/changeShuffleList', { type: ctrl.selectedList.TypeStr, shuffleList: ctrl.selectedList.ShuffleList, id: ctrl.selectedList.Id })
                .then(function (response) {
                    if (response.data != null && response.data.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };
    };

    MainPageProductsCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', 'urlHelper', 'mainpageproductsService', '$q', 'SweetAlert', 'toaster', '$translate', '$http'];

    ng.module('mainpageproducts', ['uiGridCustom', 'productsSelectvizr', 'productListsMenu'])
        .controller('MainPageProductsCtrl', MainPageProductsCtrl);
})(window.angular);