; (function (ng) {
    'use strict';

    var SettingsUsersCtrl = function ($uibModal, $http, $q, uiGridConstants, uiGridCustomConfig, SweetAlert, toaster, $translate) {

        var ctrl = this;
        ctrl.gridUsersInited = false;

        // #region Users
        var columnDefsUsers = [
            {
                name: 'PhotoSrc',
                headerCellClass: 'ui-grid-custom-header-cell-center',
                displayName: $translate.instant('Admin.Js.SettingsUsers.Photo'),
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><span class="ui-grid-custom-flex-center ui-grid-custom-link-for-img">' +
                        '<img class="ui-grid-custom-col-img" ng-src="{{row.entity.PhotoSrc}}"></span></div>',
                width: 80,
                enableSorting: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsUsers.Photo'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'HasPhoto',
                    selectOptions: [{ label: $translate.instant('Admin.Js.SettingsUsers.WithPhoto'), value: true }, { label: $translate.instant('Admin.Js.SettingsUser.WithoutPhoto'), value: false }]
                }
            },
            {
                name: 'FullName',
                displayName: $translate.instant('Admin.Js.SettingsUsers.FullName'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsUsers.FullName'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'FullName',
                }
            },
            {
                name: 'Email',
                displayName: 'Email',
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                filter: {
                    placeholder: 'Email',
                    type: uiGridConstants.filter.INPUT,
                    name: 'Email',
                }
            },
            {
                name: 'DepartmentName',
                displayName: $translate.instant('Admin.Js.SettingsUsers.Department')
            },
            {
                name: '_noopColumnDepartments',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsUsers.Department'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'DepartmentId',
                    fetch: 'departments/getDepartmentsSelectOptions'
                }
            },
            {
                name: 'Roles',
                displayName: $translate.instant('Admin.Js.SettingsUsers.Role'),
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsUsers.Role'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'Role',
                    fetch: 'users/getRoles'
                }
            },
            {
                name: 'Permissions',
                displayName: $translate.instant('Admin.Js.SettingsUsers.Permissions'),
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsUsers.Permissions'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'Permission',
                    fetch: 'users/getPermissions'
                }
            },
            {
                name: 'Enabled',
                displayName: $translate.instant('Admin.Js.SettingsUsers.Active'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                width: 90,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsUsers.Activity'),
                    name: 'Enabled',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [{ label: $translate.instant('Admin.Js.SettingsUsers.AreActive'), value: true }, { label: $translate.instant('Admin.Js.SettingsUsers.Inactive'), value: false }]
                }
            },
            {
                name: 'SortOrder',
                displayName: $translate.instant('Admin.Js.SettingUsers.Sorting'),
                enableCellEdit: true,
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadUser(row.entity.CustomerId)"></a> ' +
                    '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteUser(row.entity.CanBeDeleted, row.entity.CustomerId, row.entity.CantDeleteMessage)" ' +
                               'class="ui-grid-custom-service-icon fa fa-times link-invert"></a> ' +
                               //'ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')"></a> ' +
                    '</div></div>'
            }
        ];

        ctrl.gridUsersOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsUsers,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadUser(row.entity.CustomerId);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SettingsUsers.DeleteSelected'),
                        url: 'users/deleteusers',
                        field: 'CustomerId',
                        before: function () {
                            var selectedOptions = ctrl.gridUsers.selectionCustom.getRowsFromStorage();
                            var canDelete = true;
                            var message = '';

                            selectedOptions.forEach(function (el) {
                                if (!el.CanBeDeleted && message === '') {
                                    canDelete = false;
                                    message = el.CantDeleteMessage;
                                }
                            });

                            if (canDelete) {
                                return SweetAlert.confirm($translate.instant('Admin.Js.SettingsUsers.AreYouSureDelete'), { title: $translate.instant('Admin.Js.SettingsUsers.Deleting') }).then(function (result) {
                                    return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            }
                            else {
                                SweetAlert.error('', { title: $translate.instant('Admin.Js.SettingsUsers.DeletingIsImpossible'), html: message });
                                return false;
                            }
                        }
                    }
                ]
            }
        });

        ctrl.gridUsersOnInit = function (grid) {
            ctrl.gridUsers = grid;
            ctrl.getSaasDataInformation();
        };

        ctrl.changeManagersPageState = function () {
            $http.post('users/changeManagersPageState', { state: ctrl.showManagersPage }).then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsUsers.ChangesSaved'));
            });
        };

        ctrl.changeEnableManagersModuleState = function () {
            $http.post('users/changeEnableManagersModuleState', { state: ctrl.enableManagersModule }).then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsUsers.ChangesSaved'));
            });
        };

        ctrl.loadUser = function (id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditUserCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/settings/modal/addEditUser/AddEditUser.html',
                resolve: {
                    params: {
                        customerId: id
                    }
                }
            }).result.then(function (result) {
                ctrl.gridUsers.fetchData();
                return result;
            }, function (result) {
                ctrl.getSaasDataInformation();
                ctrl.gridUsers.fetchData();
                return result;
            });
        };

        ctrl.deleteUser = function (canBeDeleted, customerId, message) {
            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.SettingsUsers.AreYouSureDelete'), { title: $translate.instant('Admin.Js.SettingsUsers.Deleting') }).then(function (result) {
                    if (result === true) {
                        $http.post('users/deleteuser', { customerId: customerId }).then(function (response) {
                            var data = response.data;
                            if (data != null && data.result === false) {
                                data.errors.forEach(function (error) {
                                    toaster.error($translate.instant('Admin.Js.SettingsUsers.Error'), error);
                                });
                            }
                            ctrl.gridUsers.fetchData();
                            ctrl.getSaasDataInformation();
                        });
                    }
                });
            } else {
                SweetAlert.error('', { title: $translate.instant('Admin.Js.SettingsUsers.DeletingIsImpossible'), html: message });
            }
        };
        // #endregion


        // #region Departments
        var columnDefsDepartments = [
            {
                name: 'Name',
                displayName: $translate.instant('Admin.Js.SettingsUsers.Name'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>'
            },
            {
                name: 'Sort',
                displayName: $translate.instant('Admin.Js.SettingsUsers.SortingOrder'),
                type: 'number',
                width: 150,
                enableCellEdit: true
            },
            {
                name: 'Enabled',
                displayName: $translate.instant('Admin.Js.SettingsUsers.Active'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                width: 90,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsUsers.Activity'),
                    name: 'Enabled',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [{ label: $translate.instant('Admin.Js.SettingsUsers.AreActive'), value: true }, { label: $translate.instant('Admin.Js.SettingsUsers.Inactive'), value: false }]
                }
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadDepartment(row.entity.DepartmentId)"></a> ' +
                    '<ui-grid-custom-delete url="departments/deletedepartment" params="{\'departmentId\': row.entity.DepartmentId}"></ui-grid-custom-delete>' +
                    '</div></div>'
            }
        ];

        ctrl.gridDepartmentsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsDepartments,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadDepartment(row.entity.DepartmentId);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SettingsUsers.DeleteSelected'),
                        url: 'departments/deletedepartments',
                        field: 'DepartmentId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsUsers.AreYouSureDelete'), { title: $translate.instant('Admin.Js.SettingsUsers.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]
            }
        });

        ctrl.gridDepartmentsOnInit = function (grid) {
            ctrl.gridDepartments = grid;
        };

        ctrl.loadDepartment = function (id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditDepartmentCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/settings/modal/addEditDepartment/AddEditDepartment.html',
                resolve: {
                    departmentId: function () {
                        return id;
                    }
                }
            }).result.then(function (result) {
                ctrl.gridDepartments.fetchData();
                return result;
            }, function (result) {
                return result;
            });
        };
        // #endregion


        // #region ManagerRoles
        var columnDefsManagerRoles = [
            {
                name: 'Name',
                displayName: $translate.instant('Admin.Js.SettingsUsers.Name'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsUsers.Name'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Name',
                }
            },
            {
                name: 'SortOrder',
                displayName: $translate.instant('Admin.Js.SettingsUsers.Order'),
                type: 'number',
                width: 150,
                enableCellEdit: true
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadManagerRole(row.entity.Id)"></a> ' +
                    '<ui-grid-custom-delete url="managerRoles/deleteManagerRole" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                    '</div></div>'
            }
        ];

        ctrl.gridManagerRolesOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsManagerRoles,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadManagerRole(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SettingsUsers.DeleteSelected'),
                        url: 'managerRoles/deleteManagerRoles',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsUsers.AreYouSureDelete'), { title: $translate.instant('Admin.Js.SettingsUsers.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]
            }
        });

        ctrl.gridManagerRolesOnInit = function (grid) {
            ctrl.gridManagerRoles = grid;
        };

        ctrl.loadManagerRole = function (id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditManagerRoleCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/settings/modal/addEditManagerRole/AddEditManagerRole.html',
                resolve: {
                    id: function () {
                        return id;
                    }
                }
            }).result.then(function (result) {
                ctrl.gridManagerRoles.fetchData();
                return result;
            }, function (result) {
                return result;
            });
        };
        // #endregion
        
        ctrl.getSaasDataInformation = function () {
            return $http.post('users/GetSaasDataInformation', {}).then(function (response) {
                var data = response.data;
                if (data != null && data.result === false) {
                    toaster.pop("error", $translate.instant('Admin.Js.SettingsUsers.Error'), data.error);
                }

                ctrl.managersLimitation = data.obj.ManagersLimitation;
                ctrl.managersLimit = data.obj.ManagersLimit;

                ctrl.employeesCount = data.obj.EmployeesCount;
                ctrl.employeesLimit = data.obj.EmployeesLimit;
                ctrl.enableEmployees = data.obj.EnableEmployees;

                //ctrl.showManagersPage = data.obj.ShowManagersPage;
                ctrl.enableManagersModule = data.obj.EnableManagersModule;
                ctrl.gridUsersInited = true;

                return data;
            });
        };
    };

    SettingsUsersCtrl.$inject = ['$uibModal', '$http', '$q', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', 'toaster', '$translate'];

    ng.module('settingsUsers', ['ngFileUpload'])
      .controller('SettingsUsersCtrl', SettingsUsersCtrl);

})(window.angular);