; (function (ng) {
    'use strict';

    var vkChannelCtrl = function ($window, toaster, SweetAlert, vkService, $translate, vkMarketService, advTrackingService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.isLoadded = false;
            ctrl.getSettings();
            ctrl.getReports();
        };

        ctrl.removeChannel = function() {
            SweetAlert.confirm("Вы уверены, что хотите отключить канал?", { title: "Удаление" }).then(function (result) {
                if (result === true) {
                    vkService.removeChannel().then(function () {
                        var basePath = document.getElementsByTagName('base')[0].getAttribute('href');
                        $window.location.assign(basePath);
                    });
                }
            });
        }


        ctrl.getSettings = function() {
            vkService.getVkSettings().then(function (data) {
                ctrl.clientId = data.clientId;
                ctrl.groups = data.groups;
                if (ctrl.groups != null && ctrl.groups.length > 0) {
                    ctrl.selectedGroup = ctrl.groups[0];
                }

                ctrl.group = data.group;
                ctrl.groupId = data.group != null ? data.group.Id : null;
                ctrl.groupName = data.group != null ? data.group.Name : null;
                ctrl.groupScreenName = data.group != null ? data.group.ScreenName : null;
                ctrl.authByUser = data.authByUser;

                ctrl.salesFunnels = data.salesFunnels;
                ctrl.salesFunnelId = data.salesFunnelId;

                ctrl.createLeadFromMessages = data.createLeadFromMessages;
                ctrl.createLeadFromComments = data.createLeadFromComments;
                ctrl.syncOrdersFromVk = data.syncOrdersFromVk;
                ctrl.groupMessageErrorStatus = data.groupMessageErrorStatus;

                ctrl.isLoadded = true;
                ctrl.isPreviewShow = ctrl.groupId == null || ctrl.groupName == null;
            });
        }

        ctrl.deleteGroup = function () {
            SweetAlert.confirm("Вы уверены, что хотите удалить привязку?", { title: "Удаление" }).then(function (result) {
                if (result === true) {
                    vkService.deleteGroup().then(function () {
                        ctrl.getSettings();
                        if (ctrl.onAddDelVk) {
                            ctrl.onAddDelVk();
                        }
                    });
                }
            });
        }

        ctrl.authVkByLoginPassword = function() {
            vkService.authVkByLoginPassword(ctrl.login, ctrl.password).then(function(data) {
                if (data.result === true) {
                    ctrl.authByUser = true;
                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.AuthSuccessfulSetupComplete'));
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.SettingsCrm.VkFailedLogIn'));
                }
            });
        }

        ctrl.deleteVkByLoginPassword = function() {
            SweetAlert.confirm("Вы уверены, что хотите удалить привязку к личной странице?", { title: "Удаление" }).then(function (result) {
                if (result === true) {
                    vkService.deleteVkByLoginPassword().then(function () {
                        ctrl.getSettings();
                    });
                }
            });
        }

        ctrl.saveSettings = function() {
            vkService.saveSettings(ctrl.salesFunnelId, ctrl.createLeadFromMessages, ctrl.createLeadFromComments, ctrl.syncOrdersFromVk).then(function (data) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
            });
        }

        ctrl.export = function () {
            vkMarketService.export().then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', 'Начался экспорт товаров в ВКонтате. Длительность переноса зависит от кол-ва товаров и фотографий.');
                    ctrl.IsExportRun = true;

                    ctrl.getExportProgress();
                    ctrl.getReportsTimeout();
                    
                } else if (data.errors != null) {
                    data.errors.forEach(function (e) {
                        toaster.pop('error', '', e);
                    });
                    ctrl.getExportProgress();
                }
            });
        };

        ctrl.getExportProgress = function () {
            vkMarketService.getExportProgress().then(function (data) {
                ctrl.Error = data.Error;
                ctrl.Total = data.Total;
                ctrl.Current = data.Current;
                
                if (ctrl.Total > 0) {
                    ctrl.Percent = ctrl.Total > 0 ? parseInt(100 / ctrl.Total * ctrl.Current) : 0;

                    if (ctrl.Current === ctrl.Total) {
                        toaster.pop('success', '', 'Экспорт закончен');
                        ctrl.IsExportRun = false;
                        return;
                    }
                }
                setTimeout(ctrl.getExportProgress, 500);
            });
        }

        ctrl.getReportsTimeout = function () {
            setTimeout(function () {
                ctrl.getReports().then(function () {
                    if (ctrl.Percent != 100) {
                        ctrl.getReportsTimeout();
                    }
                });
            }, 3000);
        }

        ctrl.getReports = function() {
            return vkMarketService.getReports().then(function (data) {
                ctrl.Reports = data.reports;
            });
        }

        ctrl.deleteAllProducts = function () {
            SweetAlert.confirm("Вы уверены, что хотите удалить?", { title: "Удаление" }).then(function (result) {
                if (result === true) {
                    vkMarketService.deleteAllProducts().then(function (data) {
                        toaster.pop('success', '', 'Удаление началось');
                    });
                }
            });
        }

        ctrl.connectVk = function () {
            ctrl.isPreviewShow = false;
            advTrackingService.trackEvent('SalesChannels_Interest', 'vk');
        };
    };

    vkChannelCtrl.$inject = ['$window', 'toaster', 'SweetAlert', 'vkService', '$translate', 'vkMarketService', 'advTrackingService'];

    ng.module('vkChannel', [])
        .controller('vkChannelCtrl', vkChannelCtrl)
        .component('vkChannel', {
            templateUrl: '../areas/admin/content/src/vk/components/vkChannel/vkChannel.html',
            controller: 'vkChannelCtrl',
            bindings: {
                redirectUrl: '=',
                onAddDelVk: '&'
            }
        });

})(window.angular);