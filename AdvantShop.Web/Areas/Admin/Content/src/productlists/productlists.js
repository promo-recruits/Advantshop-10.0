; (function (ng) {
    'use strict';

    var ProductListsCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, $http, $q, SweetAlert, toaster, $translate) {

        var ctrl = this;
        
        /* product lists */
        ctrl.gridProductListsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.ProductLists.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="productlists/products/{{row.entity.Id}}">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductLists.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.ProductLists.SortOrder'),
                    width: 80
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.ProductLists.Activity'),
                    width: 100,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label">' +
                            '<input type="checkbox" disabled ng-model="row.entity.Enabled" class="adv-checkbox-input control-checkbox" data-e2e="switchOnOffSelect" />' +
                            '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                        '</label></div>'
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 96,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked">' +
                            
                            '<ui-modal-trigger data-controller="\'ModalAddEditProductListCtrl\'" controller-as="ctrl" size="lg" ' +
                                        'template-url="../areas/admin/content/src/productlists/modal/addEditProductList/addEditProductList.html" ' +
                                        'data-resolve="{\'id\': row.entity.Id}" ' +
                                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                                '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a> ' +
                            '</ui-modal-trigger>' +

                            '<ui-grid-custom-delete url="productLists/deleteProductList" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ],
            uiGridCustom: {
                rowUrl: 'productlists/products/{{row.entity.Id}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Productlists.DeleteSelected'),
                        url: 'productlists/deleteProductLists',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Productlists.AreYouSureDelete'), { title: $translate.instant('Admin.Js.ProductLists.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.gridProductLists = grid;
        };



        /* products */
        ctrl.gridProductsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'ProductArtNo',
                    displayName: $translate.instant('Admin.Js.Productlists.VendorCode'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a class="link-invert" ng-href="product/edit/{{row.entity.ProductId}}">{{COL_FIELD}}</a></div>',
                    width: 100
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.ProductLists.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="product/edit/{{row.entity.ProductId}}">{{COL_FIELD}}</a></div>',
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.ProductLists.SortOrder'),
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
                        '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked">' +
                            '<a ng-href="product/edit/{{row.entity.ProductId}}" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a>' +
                            '<ui-grid-custom-delete url="productlists/deletefromlist" params="{\'listId\': row.entity.ListId, \'ProductId\': row.entity.ProductId}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ],
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Productlists.DeleteSelectedFromTheList'),
                        url: 'productlists/deleteProductsFromList',
                        field: 'ProductId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Productlists.AreYouSureDelete'), { title: $translate.instant('Admin.Js.ProductLists.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },
                ]
            }
        });

        ctrl.addProductsModal = function (result) {
            $http.post('productlists/addproducts', ng.extend({ listId: ctrl.listId }, result)).then(function (response) {
                var data = response.data,
                    _result;
                if (data.result === true) {
                    _result = ctrl.gridProducts.fetchData();
                } else {
                    _result = $q.reject()
                }

                return _result;

            }).catch(function () {
                toaster.pop('error', $translate.instant('Admin.Js.PrpductLists.ErrorWhileAddingProducts'))
            });
        };

        ctrl.gridProductsOnInit = function (grid) {
            ctrl.gridProducts = grid;
        };

        ctrl.initCatalogLeftMenu = function (catalogLeftMenu) {
            ctrl.catalogLeftMenu = catalogLeftMenu;
        };

        ctrl.onGridDeleteItem = function () {
            if (ctrl.catalogLeftMenu != null) {
                ctrl.catalogLeftMenu.updateData();
            }
        };

        ctrl.onAddList = function () {
            ctrl.gridProductLists.fetchData();
            if (ctrl.catalogLeftMenu != null) {
                ctrl.catalogLeftMenu.updateData();
            }
        };
    };

    ProductListsCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', '$http', '$q', 'SweetAlert', 'toaster', '$translate'];
    
    ng.module('productlists', ['uiGridCustom'])
      .controller('ProductListsCtrl', ProductListsCtrl);

})(window.angular);