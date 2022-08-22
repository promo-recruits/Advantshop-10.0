; (function (ng) {
    'use strict';

    var CategoriesBlockCtrl = function (SweetAlert, catalogService, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {

            ctrl.categoriesSelected = [];

            ctrl.fetch();
        };

        ctrl.fetch = function () {
           return catalogService.getCategories(ctrl.categoryId, ctrl.categorysearch).then(function (result) {
               return ctrl.categories = result;
            });
        };

        ctrl.toggleSelectedAll = function (selectAll) {
            if (selectAll === true) {
                ctrl.categoriesSelected = ctrl.categories.map(function (item) { return item.CategoryId; });
            } else {
                ctrl.categoriesSelected = [];
            }
        };

        ctrl.deleteCategories = function (ids) {
            SweetAlert.confirm($translate.instant('Admin.Js.CategoriesBlock.AreYouSureDeleteCategories'), { title: $translate.instant('Admin.Js.CategoriesBlack.DeletingCategories'), showLoaderOnConfirm: true })
               .then(function (result) {
                   if (result === true) {
                       catalogService.deleteCategories(ids).then(function () {
                           ctrl.categoriesSelected = [];
                       })
                       .then(ctrl.fetch)
                       .then(function () {
                           if (ctrl.onDelete != null) {
                               ctrl.onDelete();
                           }
                       });
                   }
               });
        };

        ctrl.deleteCategory = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.CategoriesBlock.AreYouSureDeleteCategory'), { title: $translate.instant('Admin.Js.CategoriesBlock.DeletingCategory'), showLoaderOnConfirm: true, confirmButtonColor: "#2d9cee", cancelButton: "#ffffff" })
               .then(function (result) {
                   if (result === true) {
                       catalogService.deleteCategories([id]).then(function () {
                           ctrl.categoriesSelected = [];
                       })
                       .then(ctrl.fetch)
                       .then(function () {
                           if (ctrl.onDelete != null) {
                               ctrl.onDelete();
                           }
                       });
                   }
               });
        }

        ctrl.sortableOptions = {
            orderChanged: function (event) {
                var categoryId = event.source.itemScope.category.CategoryId,
                    prevCategory = ctrl.categories[event.dest.index - 1],
                    nextCategory = ctrl.categories[event.dest.index + 1];

                catalogService.changeCategorySortOrder(categoryId, prevCategory != null ? prevCategory.CategoryId : null, nextCategory != null ? nextCategory.CategoryId : null).then(function () {
                    toaster.pop("success", "", $translate.instant('Admin.Js.CategoriesBlock.ChangesSaved'));
                });
            }
        };
    };


    CategoriesBlockCtrl.$inject = ['SweetAlert', 'catalogService', 'toaster', '$translate'];

    ng.module('categoriesBlock', ['ng-sweet-alert', 'checklist-model', 'as.sortable'])
        .controller('CategoriesBlockCtrl', CategoriesBlockCtrl);

})(window.angular);