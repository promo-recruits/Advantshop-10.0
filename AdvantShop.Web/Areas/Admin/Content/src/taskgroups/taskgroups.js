; (function (ng) {
    'use strict';

    var TaskGroupsCtrl = function ($q, $location, $window, $http, $uibModal, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, SweetAlert, $translate, toaster) {

        var ctrl = this;

        ctrl.init = function (isAdmin) {
            var columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Project'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a href=\'projects/{{row.entity.Id}}\' class="link">{{COL_FIELD}}</span></div>'
                }
            ];

            columnDefs.push.apply(columnDefs, [
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Activity'),
                    cellTemplate: '<ui-grid-custom-switch row="row" readonly="' + !isAdmin + '"></ui-grid-custom-switch>',
                    width: 100,
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Order'),
                    type: 'number',
                    enableCellEdit: isAdmin,
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Sorting'),
                        type: 'range',
                        rangeOptions: {
                            from: {name: 'SortingFrom'},
                            to: {name: 'SortingTo'}
                        }
                    }
                }
            ]);

            if (isAdmin) {
                columnDefs.push.apply(columnDefs, [
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 80,
                        enableSorting: false,
                        cellTemplate:
                            '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                                '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt js-task-group-edit" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTaskGroup(row.entity.Id)"></a> ' +
                                '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteTaskGroup(row.entity.CanBeDeleted, row.entity.Id)" class="js-task-group-edit" ' +
                                'ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')"></a> ' +
                                '</div></div>'
                    }
                ]);
            }

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefs,
                uiGridCustom: {
                    rowUrl: 'projects/{{row.entity.Id}}',
                    //rowClick: function ($event, row) {
                    //    ctrl.loadTaskGroup(row.entity.Id);
                    //},
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.Taskgroups.Taskgroups.DeleteSelected'),
                            url: 'taskgroups/deletetaskgroups',
                            field: 'Id',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.Taskgroups.Taskgroups.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Deleting') }).then(function (result) {
                                    return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            }
                        }
                    ]
                }
            });
        }

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.loadTaskGroup = function (id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditTaskGroupCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/taskgroups/modal/addEditTaskGroup.html',
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

        ctrl.deleteTaskGroup = function (canBeDeleted, id) {
            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.Taskgroups.Taskgroups.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Deleting') }).then(function (result) {
                    if (result === true) {
                        $http.post('taskgroups/deletetaskgroup', { Id: id }).then(function (response) {
                            if (response.data.result == true) {
                                ctrl.grid.fetchData();
                            } else {
                                response.data.errors.forEach(function (error) {
                                    toaster.error(error);
                                });
                            }
                        });
                    }
                });
            } else {
                SweetAlert.alert($translate.instant('Admin.Js.Taskgroups.Taskgroups.ProjectHasTasks'), { title: $translate.instant('Admin.Js.Taskgroups.Taskgroups.DeletingIsImpossible') });
            }
        };

    };

    TaskGroupsCtrl.$inject = ['$q', '$location', '$window', '$http', '$uibModal', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', 'SweetAlert', '$translate', 'toaster'];


    ng.module('taskgroups', ['uiGridCustom', 'urlHelper'])
      .controller('TaskGroupsCtrl', TaskGroupsCtrl);

})(window.angular);