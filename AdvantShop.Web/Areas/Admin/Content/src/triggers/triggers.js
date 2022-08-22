; (function (ng) {
    'use strict';

    var TriggersCtrl = function (toaster, triggersService, SweetAlert, $translate, uiGridCustomConfig, uiGridConstants, $q, $uibModal, $http) {

        var ctrl = this;
        ctrl.gridCategoriesInited = false;

        ctrl.$onInit = function () {
            ctrl.gridTriggersOptions =  ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'Name',
                        displayName: $translate.instant('Admin.Js.Triggers.Name'),
                        cellTemplate: '<div class="ui-grid-cell-contents"><a class="link-invert word-break" ng-href="triggers/edit/{{row.entity.Id}}" title="{{COL_FIELD}}">{{COL_FIELD}}</a></div>',
                        enableCellEdit: false,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.Triggers.Name'),
                            type: uiGridConstants.filter.INPUT,
                            name: 'uiGridCustomConfig'
                        }
                    },
                    {
                        name: 'Description',
                        displayName: $translate.instant('Admin.Js.Triggers.Description'),
                        enableCellEdit: false,
                        enableSorting: false
                    },
                    {
                        name: 'CategoryName',
                        displayName: $translate.instant('Admin.Js.Triggers.CategoryName'),
                        cellTemplate: '<div class="ui-grid-cell-contents">{{COL_FIELD || \'Общая\'}}</div>',
                        enableCellEdit: false
                    },
                    {
                        name: 'Enabled',
                        displayName: $translate.instant('Admin.Js.Catalog.Activ'),
                        enableCellEdit: false,
                        cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                        filter: {
                            placeholder: $translate.instant('Admin.Js.Catalog.Activity'),
                            type: uiGridConstants.filter.SELECT,
                            selectOptions: [{ label: $translate.instant('Admin.Js.Catalog.TheyActive'), value: true }, { label: $translate.instant('Admin.Js.Catalog.Inactive'), value: false }]
                        }
                    },
                    {
                        name: 'CategoryId',
                        enableCellEdit: false,
                        visible: false,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.Triggers.Category'),
                            type: uiGridConstants.filter.SELECT,
                            name: 'CategoryId',
                            fetch: 'triggers/getCategoriesList'
                        }
                    },
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 75,
                        enableSorting: false,
                        cellTemplate: '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked"><a ng-href="triggers/edit/{{row.entity.Id}}" class="ui-grid-custom-service-icon fas fa-pencil-alt"></a><ui-grid-custom-delete url="triggers/deleteTrigger" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete></div></div>'
                    }
                ],
                uiGridCustom: {
                    rowUrl: 'triggers/edit/{{row.entity.Id}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.Catalog.DeleteSelected'),
                            url: 'triggers/deleteTriggers',
                            field: 'Id',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.Catalog.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Catalog.Deleting') }).then(function (result) {
                                    return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            }
                        },
                        {
                            text: $translate.instant('Admin.Js.Catalog.MakeActive'),
                            url: 'triggers/activateTriggers',
                            field: 'Id'
                        },
                        {
                            text: $translate.instant('Admin.Js.Catalog.MakeInactive'),
                            url: 'triggers/disableTriggers',
                            field: 'Id'
                        }
                    ]
                }
            });
            ctrl.gridCategoriesOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'Name',
                        displayName: 'Название',
                        enableCellEdit: true,
                        //cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>'
                    },
                    {
                        name: 'SortOrder',
                        displayName: 'Порядок',
                        type: 'number',
                        enableCellEdit: true,
                        width: 150,
                        filter: {
                            placeholder: 'Сортировка',
                            type: 'range',
                            rangeOptions: {
                                from: { name: 'SortingFrom' },
                                to: { name: 'SortingTo' }
                            }
                        }
                    },
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 80,
                        enableSorting: false,
                        cellTemplate:
                            '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                            '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt js-task-group-edit" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadCategory(row.entity.Id)"></a> ' +
                            '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteCategory(row.entity.Id)" class="js-task-group-edit ui-grid-custom-service-icon fa fa-times link-invert"></a> ' +
                            '</div></div>'
                    }
                ],
                uiGridCustom: {
                    rowClick: function ($event, row) {
                        ctrl.loadCategory(row.entity.Id);
                    },
                    selectionOptions: [
                        {
                            text: 'Удалить выделенные',
                            url: 'triggers/deleteCategories',
                            field: 'Id',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.TriggerCategories.Categories.AreYouSureDelete'), { title: $translate.instant('Admin.Js.TriggerCategories.Categories.Deleting') }).then(function (result) {
                                    return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            }
                        }
                    ]
                }
            });
        };

        ctrl.deleteTrigger = function (id) {
            SweetAlert.confirm('Вы уверены что хотите удалить?', { title: $translate.instant('Admin.Js.GridCustomComponent.Deleting') })
                .then(function () {
                    triggersService.deleteTrigger(id).then(function (result) {
                        toaster.pop('success', '', 'Изменения успешно сохранены.');

                        ctrl.getTriggers();
                    });
                });
        };

        ctrl.gridCategoriesOnInit = function (grid) {
            ctrl.grid = grid;
            ctrl.gridCategoriesInited = true;
        };

        ctrl.loadCategory = function (id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditCategoryCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/triggers/modal/addEditCategory/addEditCategory.html',
                resolve: {
                    id: function () {
                        return id;
                    }
                }
            }).result.then(function (result) {
                ctrl.grid.fetchData();
                return result;
            }, function (result) {
                return result;
            });
        };

        ctrl.deleteCategory = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.TriggerCategories.Categories.AreYouSureDelete'), { title: $translate.instant('Admin.Js.TriggerCategories.Categories.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('triggers/deleteCategory', { Id: id }).then(function (response) {
                        if (response.data.result === true) {
                            ctrl.grid.fetchData();
                        } else {
                            response.data.errors.forEach(function (error) {
                                toaster.error(error);
                            });
                        }
                    });
                }
            });
        };
    };

    TriggersCtrl.$inject = ['toaster', 'triggersService', 'SweetAlert', '$translate', 'uiGridCustomConfig', 'uiGridConstants', '$q', '$uibModal', '$http'];


    ng.module('triggers', [])
        .controller('TriggersCtrl', TriggersCtrl);

})(window.angular);