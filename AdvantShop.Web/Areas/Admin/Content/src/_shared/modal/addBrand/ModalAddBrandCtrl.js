; (function (ng) {
    'use strict';

    var ModalAddBrandCtrl = function ($uibModalInstance, uiGridCustomConfig, $translate) {
        var ctrl = this;

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'BrandName',
                    displayName: $translate.instant('Admin.Js.AddBrand.ManufactrerName'),
                },
                {
                    name: 'ProductsCount',
                    displayName: $translate.instant('Admin.Js.AddBrand.QuantityOfProducts'),
                    width: 120,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 100,
                    enableSorting: false,
                    cellTemplate:
                    '<div class="ui-grid-cell-contents"><div>' +
                    '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.choose(row.entity.BrandId, row.entity.BrandName)">' + $translate.instant('Admin.Js.AddCategory.Select') + '</a> ' +
                        '</div></div>'
                }
            ]
        });

        if (ctrl.$resolve.multiSelect === false) {
            ng.extend(ctrl.gridOptions, {
                multiSelect: false,
                modifierKeysToMultiSelect: false,
                enableRowSelection: true,
                enableRowHeaderSelection: false
            });
        }

        ctrl.gridOnInit = function (grid) {
            ctrl.gridBrands = grid;
        };
        
        ctrl.choose = function (brandId, brandName) {
            $uibModalInstance.close({ brandId: brandId, brandName: brandName });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddBrandCtrl.$inject = ['$uibModalInstance', 'uiGridCustomConfig', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddBrandCtrl', ModalAddBrandCtrl);

})(window.angular);