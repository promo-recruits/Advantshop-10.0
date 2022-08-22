; (function (ng) {
    'use strict';
    var ModulesCtrl = function (modulesService, $http, toaster, $window, $translate, SweetAlert, advTrackingService) {

        var ctrl = this;

        ctrl.gridParams = {};

        ctrl.columns = [];

        ctrl.dataLoaded = false;

        ctrl.onInit = function () {
            ctrl.getModules().then(function () {
                ctrl.filterApply([{ name: 'search', value: ctrl.filterStart }]);

                ctrl.needUpdateModules = ctrl.modulesData.some(function (item) {
                    return item.Version != item.CurrentVersion;
                })

            });
            ctrl.filterParams = {};
        };

        ctrl.gridSearchPlaceholder = ctrl.gridSearchPlaceholder || $translate.instant('Admin.Js.Modules.EnterTextToSearch');

        ctrl.filterModal = function (value) {

            var result = true;

            for (var key in ctrl.filterParams) {

                if (ctrl.filterParams.hasOwnProperty(key)) {

                    var term = ctrl.filterParams[key].filter.term;

                    if (term != null) {

                        if (ctrl.filterParams[key].filter.type === 'range') {

                            if (term.from != null && term.from > value[key]) {
                                result = false;
                                break;
                            }

                            if (term.to != null && term.to < value[key]) {
                                result = false;
                                break;
                            }

                        } else if (ctrl.filterParams[key].filter.type === 'input') {

                            if (term != null && term !== "") {
                                if (value[key].toLowerCase().indexOf(term.toLowerCase()) === -1) {

                                    if (value["StringId"].toLowerCase().indexOf(term.toLowerCase()) === -1) {
                                        result = false;
                                        break;
                                    }
                                }
                            }

                        } else if (ctrl.filterParams[key].filter.type === 'select') {

                            if (term != null && value[key] !== term) {
                                result = false;
                                break;
                            }
                        }

                    }

                }
            }

            return result;
        };

        ctrl.filterColumnDefs = [
            {
                filter: {
                    placeholder: $translate.instant('Admin.Js.Modules.Name'),
                    type: 'input',
                    name: 'Name',
                }
            },
            {
                filter: {
                    name: 'Enabled',
                    placeholder: $translate.instant('Admin.Js.Modules.Activity'),
                    type: 'select',
                    selectOptions: [{ label: $translate.instant('Admin.Js.Modules.TheyActive'), value: true }, { label: $translate.instant('Admin.Js.Modules.TheyInactive'), value: false }]
                }
            }
        ];

        ctrl.filterApply = function (params, item) {
            if (params == null) return;

            var obj = params.filter(function (x) { return x.name === "search"; })[0];

            if (obj != null && obj.name === 'search') {
                ctrl.filterParams['Name'] = {
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Modules.Name'),
                        type: 'input',
                        term: obj.value,
                        name: 'Name',
                    }
                }
            } else if (item != null) {
                //ctrl.filterParams[item.filter.type === 'range' ? item.filter.name : name] = item;
                ctrl.filterParams[item.filter.name] = item;
            }

            ctrl.modulesData = ctrl.modulesMaster.filter(ctrl.filterModal);
        };

        ctrl.filterRemove = function (name, item) {

            if (item.filter.type === 'range') {
                delete ctrl.gridParams[item.filter.rangeOptions.from.name];
                delete ctrl.gridParams[item.filter.rangeOptions.to.name];
            } if (item.filter.type === 'datetime') {
                delete ctrl.gridParams[item.filter.datetimeOptions.from.name];
                delete ctrl.gridParams[item.filter.datetimeOptions.to.name];
            } else {
                delete ctrl.gridParams[name];
            }
        };

        ctrl.getModules = function () {
            return modulesService[ctrl.pageType ? 'getLocalModules' : 'getMarketModules']().then(function (modulesData) {
                ctrl.dataLoaded = true;
                ctrl.modulesMaster = ng.copy(modulesData);
                ctrl.modulesData = ng.copy(modulesData);

                return modulesData;
            });
        };

        ctrl.installModule = function (module) {

            SweetAlert.info(null, {
                title: '<i class="fa fa-spinner fa-spin"></i>&nbsp;' + $translate.instant('Admin.Js.Modules.ModuleInstalling'),
                showConfirmButton: false,
                allowOutsideClick: false,
                allowEscapeKey: false,
            })

            $http.post('modules/installModule', { stringId: module.StringId, id: module.Id, version: module.Version })
                .then(function (response) {
                    if (response.data.result === true) {
                        $window.location = response.data.url;
                        toaster.pop('success', '', $translate.instant('Admin.Js.Modules.ModuleIsInstalled'));
                    } else {
                        swal.close();
                        toaster.pop('error', '', $translate.instant('Admin.Js.Modules.ErrorInstallingModule'));
                    }
                    if (ctrl.previewPage === true) {
                        advTrackingService.trackEvent('SalesChannels_Interest', module.StringId);
                    }
                })
                .catch(function () {
                    swal.close();
                });
        }

        ctrl.updateModule = function (module) {
            SweetAlert.info(null, {
                title: '<i class="fa fa-spinner fa-spin"></i>&nbsp;' + $translate.instant('Admin.Js.Modules.ModuleUpdating'),
                showConfirmButton: false,
                allowOutsideClick: false,
                allowEscapeKey: false,
            })

            $http.post('modules/updateModule', { stringId: module.StringId, id: module.Id, version: module.Version })
                .then(function (response) {
                    $window.location.reload(true);
                })
                .catch(function () {
                    swal.close();
                });
        }

        ctrl.updateAllModules = function () {
            SweetAlert.info(null, {
                title: '<i class="fa fa-spinner fa-spin"></i>&nbsp;' + $translate.instant('Admin.Js.Modules.ModuleUpdating'),
                showConfirmButton: false,
                allowOutsideClick: false,
                allowEscapeKey: false,
            })
            $http.post('modules/updateAllModules', { modules: ctrl.modulesData })
                .then(function (response) {
                    $window.location.reload(true);
                })
                .catch(function () {
                    swal.close();
                });
            //.finally(function () {
            //    toaster.pop('success', '', $translate.instant('Admin.Js.Modules.ModulesUpdated'));
            //});
        }

        ctrl.changeEnabled = function (module) {

            $http.post('modules/changeEnabled', { stringId: module.StringId, enabled: module.Enabled }).then(function (response) {

                if (response.data.result === true) {
                    if (!response.data.obj.SaasAndPaid || module.Enabled) {
                        toaster.pop('success', '', module.Enabled ? $translate.instant('Admin.Js.Modules.ModuleIsActivated') : $translate.instant('Admin.Js.ModuleIsNotActive'));
                    } else {
                        SweetAlert.info('', { title: '', html: $translate.instant('Admin.Js.Modules.DeactivatedAndPayable') });
                    }
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Modules.ErrorChangingActivity'));
                }
            });
        }

        ctrl.uninstallModule = function (module) {

            if (ctrl.moduleInProcess != null) {
                return;
            }

            ctrl.moduleInProcess = module;

            $http.post('modules/uninstallModule', { stringId: module.StringId })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Modules.ModuleWasDeleted'));

                        $window.location.reload(true);
                    }
                })
                .finally(function () {
                    ctrl.moduleInProcess = null;
                })
        }

        ctrl.IsVisibleUpdateAll = function () {
            for (var i = 0; i < ctrl.modulesData.length; i++) {
                if (!ctrl.modulesData[i].IsLocalVersion && !ctrl.modulesData[i].IsCustomVersion && ctrl.modulesData[i].IsInstall && ctrl.modulesData[i].Active && ctrl.modulesData[i].Version != ctrl.modulesData[i].CurrentVersion) {
                    return true;
                }
            }
            return false;
        }
    }


    ModulesCtrl.$inject = ['modulesService', '$http', 'toaster', '$window', '$translate', 'SweetAlert', 'advTrackingService'];

    ng.module('modules', [])
      .controller('ModulesCtrl', ModulesCtrl);

})(window.angular);