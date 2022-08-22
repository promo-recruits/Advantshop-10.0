; (function (ng) {
    'use strict';

    var CategoryCtrl = function ($http, uiGridCustomConfig, toaster, Upload, SweetAlert, $translate) {

        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.showGridPropertyGroups = true;
        }

        ctrl.gridPropertyGroupsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Category.PropertyGroups')
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 40,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div><ui-grid-custom-delete url="category/deleteGroupFromCategory" params="{\'groupId\': row.entity.PropertyGroupId, \'categoryId\': row.entity.CategoryId }"></ui-grid-custom-delete></div></div>'
                }
            ],
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.gridPropertyGroups = grid;
            ctrl.showGridPropertyGroups = grid.gridOptions.data.length > 0;
        };

        ctrl.gridOnFetch = function (grid) {
            ctrl.showGridPropertyGroups = grid.gridOptions.data.length > 0;
        }


        ctrl.changeCategory = function (result) {
            ctrl.ParentCategoryId = result.categoryId;
            ctrl.ParentCategoryName = result.categoryName;
        }


        ctrl.PictureId = 0;
        ctrl.IconId = 0;
        ctrl.MiniPictureId = 0;

        ctrl.updateMiniImage = function (result) {
            ctrl.MiniPictureId = result.pictureId;
        };

        ctrl.updateIconImage = function (result) {
            ctrl.IconId = result.pictureId;
        };

        ctrl.updateImage = function (result) {
            ctrl.PictureId = result.pictureId;
        };

        // load tags
        ctrl.loadTags = function (categoryId, form) {
            $http.get('category/getTags', { params: { categoryId: categoryId } })
                .then(function (response) {
                    ctrl.tags = response.data.tags;
                    ctrl.selectedTags = response.data.selectedTags;
                })
                .then(function () {
                    form.$setPristine();
                })
        };

        ctrl.tagTransform = function (newTag) {
            return { value: newTag };
        };

        ctrl.deleteCategory = function (categoryId) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Deleting') }).then(function (result) {
                if (result) {
                    $http.post('category/delete', { id: categoryId }).then(function (response) {
                        if (response.data.result === true) {
                            if (response.data.needRedirect) {
                                window.location = 'catalog?categoryid=' + response.data.id;
                            }
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.Category.ErrorWhileDeleting'), "");
                        }
                    });
                }
            });
        };


        // region automap categories

        ctrl.addAutomapCategories = function (categoryId, categoryIds) {
            if (categoryIds == null || categoryIds.length == 0) {
                return;
            }

            $http.post('category/addAutomapCategories', { categoryId: categoryId, categoryIds: categoryIds }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.Category.AutomapCategoriesAdded'));
                    ctrl.getAutomapCategories(categoryId);
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.getAutomapCategories = function (categoryId) {
            return $http.get('category/getAutomapCategories', { params: { categoryId: categoryId }}).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.automapCategories = data.obj.automapCategories || [];
                    ctrl.automapAction = data.obj.automapAction;
                    ctrl.mainAutomapCategory = data.obj.mainAutomapCategory;
                    ctrl.automapCategoriesIds = ctrl.automapCategories.map(function (ac) {
                        return ac.CategoryId;
                    });
                }
            });
        };

        ctrl.setMainAutomapCategory = function (categoryId, automapCategoryId, setValue, quiet) {
            if (typeof (setValue) != 'undefined') {
                ctrl.mainAutomapCategory = setValue;
            }
            $http.post('category/setMainAutomapCategory', {
                categoryId: categoryId,
                automapCategoryId: automapCategoryId
            }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    !quiet && toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.setCategoryAutomapAction = function (categoryId, automapAction, prevAutomapAction, setValue, quiet) {
            if (typeof (setValue) != 'undefined') {
                ctrl.automapAction = setValue;
            }
            $http.post('category/setCategoryAutomapAction', {
                categoryId: categoryId,
                automapAction: prevAutomapAction != null ? automapAction | prevAutomapAction : automapAction
            }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    !quiet && toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.deleteAutomapCategory = function (categoryId, automapCategoryId) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('category/deleteAutomapCategory', { categoryId: categoryId, automapCategoryId: automapCategoryId }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            toaster.success('', $translate.instant('Admin.Js.Category.AutomapCategoryDeleted'));
                            ctrl.getAutomapCategories(categoryId).then(function () {
                                if (ctrl.automapCategories.length == 0) {
                                    ctrl.setCategoryAutomapAction(categoryId, 8, null, 8, true);
                                    ctrl.mainAutomapCategory = null;
                                }
                            });
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        };

        ctrl.deleteAutomapCategories = function (categoryId) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('category/deleteAutomapCategories', { categoryId: categoryId }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            toaster.success('', $translate.instant('Admin.Js.Category.AutomapCategoriesDeleted'));
                            ctrl.getAutomapCategories(categoryId).then(function () {
                                if (ctrl.automapCategories.length == 0) {
                                    ctrl.setCategoryAutomapAction(categoryId, 8, null, 8, true);
                                    ctrl.mainAutomapCategory = null;
                                }
                            });
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        };

        // endregion

    };

    CategoryCtrl.$inject = ['$http', 'uiGridCustomConfig', 'toaster', 'Upload', 'SweetAlert', '$translate'];

    ng.module('category', ['angular-inview', 'uiGridCustom', 'urlGenerator', 'uiModal'])
        .controller('CategoryCtrl', CategoryCtrl);

})(window.angular);