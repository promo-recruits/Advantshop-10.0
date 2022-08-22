; (function (ng) {
    'use strict';

    var ModalAddEditProductListCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;
        
        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.id != null ? params.id : 0;
            ctrl.mode = ctrl.id != 0 ? "edit" : "add";

            if (ctrl.mode == "add") {
                ctrl.sortOrder = 0;
                ctrl.enabled = true;
            } else {
                $http.get('productLists/getProductList', { params: { id: ctrl.id }}).then(function (response) {
                    var data = response.data;
                    ctrl.name = data.Name;
                    ctrl.sortOrder = data.SortOrder;
                    ctrl.enabled = data.Enabled;
                    ctrl.description = data.Description;

                    var meta = data.Meta;
                    if (meta != null && meta.ObjId) {
                        ctrl.H1 = meta.H1;
                        ctrl.Title = meta.Title;
                        ctrl.MetaKeywords = meta.MetaKeywords;
                        ctrl.MetaDescription = meta.MetaDescription;
                        ctrl.MetaId = meta.MetaId;
                        ctrl.Type = meta.Type;
                        ctrl.ObjId = meta.ObjId;

                        ctrl.defaultMeta = false;

                    } else {
                        ctrl.defaultMeta = true;
                    }
                });
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {

            var url = ctrl.mode == "add" ? 'productLists/addProductList' : 'productLists/updateProductList';
            var params = {
                id: ctrl.id,
                name: ctrl.name,
                sortOrder: ctrl.sortOrder,
                enabled: ctrl.enabled,
                description: ctrl.description,
                Meta: {
                    H1: ctrl.defaultMeta ? '' : ctrl.H1,
                    Title: ctrl.defaultMeta ? '' : ctrl.Title,
                    MetaKeywords: ctrl.defaultMeta ? '' : ctrl.MetaKeywords,
                    MetaDescription: ctrl.defaultMeta ? '' : ctrl.MetaDescription,
                    MetaId: ctrl.MetaId,
                    Type: ctrl.Type,
                    ObjId: ctrl.ObjId == undefined ? ctrl.id : ctrl.ObjId
                }
            };

            $http.post(url, params).then(function (response) {
                if (ctrl.mode == "add") {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Productlists.ListOfProductsAdded'));
                } else {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Productlists.ChangesSaved'));
                }
                $uibModalInstance.close({id: response.data.obj});
            });
            
        };
    };

    ModalAddEditProductListCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddEditProductListCtrl', ModalAddEditProductListCtrl);

})(window.angular);