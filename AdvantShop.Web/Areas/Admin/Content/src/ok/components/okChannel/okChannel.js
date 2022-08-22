; (function (ng) {
    'use strict';

    var okChannelCtrl = function ($window, toaster, SweetAlert, okService, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.isLoadded = false;
            ctrl.getSettings();
        }

        ctrl.removeChannel = function () {
            SweetAlert.confirm("Вы уверены, что хотите отключить канал?", { title: "Удаление" }).then(function (result) {
                if (result === true) {
                    okService.removeChannel().then(function () {
                        var basePath = document.getElementsByTagName('base')[0].getAttribute('href');
                        $window.location.assign(basePath);
                    });
                }
            });
        }

        ctrl.removeBinding = function () {
            okService.removeBinding().then(function () {
                $window.location.reload(true);
            });
        }

        ctrl.getSettings = function () {
            okService.getOkSettings().then(function (data) {
                ctrl.groupId = data.groupId;
                ctrl.groupName = data.groupName;
                ctrl.salesFunnels = data.salesFunnels;
                ctrl.salesFunnelId = data.salesFunnelId;
                ctrl.subscribeToMessages = data.subscribeToMessages;
                
                ctrl.isLoadded = true;
            });
        }
        
        ctrl.changeSaleFunnel = function () {
            return okService.changeSaleFunnel({ id: ctrl.salesFunnelId }).then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.OkChannel.ChangesSaved'));
                }
            });
        }
        
        ctrl.toggleSubscriptionToMessages = function (){
            return okService.toggleSubscriptionToMessages(ctrl.subscribeToMessages).then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.OkChannel.ChangesSaved'));
                } else {
                    toaster.pop('error', '', 'Ошибка при сохранении');
                }
            });
        }
    };

    okChannelCtrl.$inject = ['$window', 'toaster', 'SweetAlert', 'okService', '$translate'];

    ng.module('okChannel', [])
        .controller('okChannelCtrl', okChannelCtrl)
        .component('okChannel', {
            templateUrl: '../areas/admin/content/src/ok/components/okChannel/okChannel.html',
            controller: 'okChannelCtrl',
        });

})(window.angular);