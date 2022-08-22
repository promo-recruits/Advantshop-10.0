﻿; (function (ng) {
    'use strict';

    var ModalAddEditNewsCategoryCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;
        
        ctrl.$onInit = function () {

            var params = ctrl.$resolve;
            ctrl.NewsCategoryId = params.id != null ? params.id : 0;
            ctrl.mode = ctrl.NewsCategoryId != 0 && ctrl.NewsCategoryId != undefined && ctrl.NewsCategoryId != null ? "edit" : "add";

            if (ctrl.mode == "edit") {
                ctrl.getNewsCategory(ctrl.NewsCategoryId);
            }
            else {
                ctrl.id = 0;
                ctrl.IsDefaultMeta = true;
            }
        };

        ctrl.getNewsCategory = function (ID) {
            $http.get('NewsCategory/GetNewsCategoryItem', { params: { ID: ID, rnd: Math.random() } }).then(function (response) {
                var category = response.data.newsCategory;
                if (category != null) {

                    ctrl.Name = category.Name;
                    ctrl.SortOrder = category.SortOrder;
                    ctrl.UrlPath = category.UrlPath;
                }
                var meta = response.data.meta;
                if (meta != null) {
                    ctrl.IsDefaultMeta = false;
                    ctrl.H1 = meta.H1;
                    ctrl.Title = meta.Title;
                    ctrl.MetaKeywords = meta.MetaKeywords;
                    ctrl.MetaDescription = meta.MetaDescription;
                    ctrl.MetaId = meta.MetaId;
                    ctrl.Type = meta.Type;
                    ctrl.ObjId = meta.ObjId;
                } else {
                    ctrl.IsDefaultMeta = true;
                }
            });
        }
        
        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
        
        ctrl.saveNewsCategory = function () {

            ctrl.btnSleep = true;

            var params = {
                NewsCategoryId: ctrl.NewsCategoryId,
                Name: ctrl.Name,
                SortOrder: ctrl.SortOrder,
                UrlPath: ctrl.UrlPath,
                Meta: {
                    H1: ctrl.H1,
                    Title: ctrl.Title,
                    MetaKeywords: ctrl.MetaKeywords,
                    MetaDescription: ctrl.MetaDescription,
                    MetaId: ctrl.MetaId,
                    Type: ctrl.Type,
                    ObjId: ctrl.ObjId == undefined ? ctrl.NewsCategoryId : ctrl.ObjId
                },
                IsDefaultMeta: ctrl.IsDefaultMeta,
                rnd: Math.random()
            };

            var url = ctrl.mode == "add" ? 'NewsCategory/AddNewsCategory' : 'NewsCategory/EditNewsCategory';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop("success", "", $translate.instant('Admin.Js.NewsCategory.ChangesSaved'));
                    $uibModalInstance.close('saveNewsCategory');
                } else {
                    toaster.pop("error", $translate.instant('Admin.Js.NewsCategory.Error'), $translate.instant('Admin.Js.NewsCategory.ErrorWhileCreating'));
                    ctrl.btnSleep = false;
                }
            });
        }
    };

    ModalAddEditNewsCategoryCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddEditNewsCategoryCtrl', ModalAddEditNewsCategoryCtrl);

})(window.angular);