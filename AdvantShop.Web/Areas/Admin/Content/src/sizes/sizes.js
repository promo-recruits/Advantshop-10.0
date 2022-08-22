; (function (ng) {
    'use strict';

    var SizesCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, toaster, SweetAlert, $http, $q, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'SizeName',
                    displayName: $translate.instant('Admin.Js.Sizes.Name'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Sizes.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'SizeName'
                    }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Sizes.SortOrder'),
                    width: 100,
                    enableCellEdit: true,
                },
                {
                    name: 'ProductsCount',
                    displayName: $translate.instant('Admin.Js.Sizes.UsedForProducts'),
                    width: 90,
                    cellTemplate: '<div class="ui-grid-cell-contents"> <a ng-href="catalog?showMethod=AllProducts#?grid=%7B%22SizeId%22:{{row.entity.SizeId}}%7D">{{ COL_FIELD }}</a></div>'
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +

                            '<ui-modal-trigger data-controller="\'ModalAddEditSizeCtrl\'" controller-as="ctrl" ' +
                                        'template-url="../areas/admin/content/src/sizes/modal/addEditSize/addEditSize.html" ' +
                                        'data-resolve="{\'sizeId\': row.entity.SizeId}" ' +
                                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                                '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a> ' +
                            '</ui-modal-trigger>' +
                            
                            ' <a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.CanBeDeleted, row.entity.SizeId)" ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')"></a> ' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Sizes.DeleteSelected'),
                        url: 'sizes/deleteSizes',
                        field: 'SizeId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Sizes.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Sizes.Deleting') }).then(function (result) {
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
        
        ctrl.delete = function (canBeDeleted, sizeId) {

            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.Sizes.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Sizes.Deleting') }).then(function (result) {
                    if (result === true) {
                        $http.post('sizes/deleteSize', { 'sizeId': sizeId }).then(function (response) {
                            ctrl.grid.fetchData();
                        });
                    }
                });
            } else {
                SweetAlert.alert($translate.instant('Admin.Js.Sizes.SizesCanNotBeDeleted'), { title: $translate.instant('Admin.Js.Sizes.DeletingIsImpossible') });
            }
        }
    };

    SizesCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', 'toaster', 'SweetAlert', '$http', '$q', '$translate'];


    ng.module('sizes', ['uiGridCustom', 'urlHelper'])
      .controller('SizesCtrl', SizesCtrl);

})(window.angular);