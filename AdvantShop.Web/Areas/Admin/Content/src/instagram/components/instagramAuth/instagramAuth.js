; (function (ng) {
    'use strict';

    var instagramAuthCtrl = function ($http, toaster, SweetAlert, instagramAuthService, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getSettings();
        };

        ctrl.getSettings = function() {
            instagramAuthService.getInstagramSettings().then(function (data) {
                ctrl.settings = data;
            });
        }

        ctrl.save = function () {

            var login = ctrl.settings.login;

            if (ctrl.settings.login == null || ctrl.settings.login.length == 0 ||
                ctrl.settings.password == null || ctrl.settings.password.length == 0) {
                toaster.pop('error', '', 'Заполните обязательные поля');
                return;
            }

            if (login.indexOf('@') != -1 && login[0] != '@') {
                toaster.pop('error', '', 'Укажите логин вида @mylogin');
                return;
            }

            instagramAuthService.saveLoginSettings({ login: ctrl.settings.login, password: ctrl.settings.password })
                .then(function(data) {

                    if (data.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.SettingsSuccessfulySaved'));
                        ctrl.getSettings();
                        if (ctrl.onAddDelInstagram) {
                            ctrl.onAddDelInstagram();
                        }
                    } else {
                        data.errors.forEach(function(error) {
                            toaster.pop('error', '', error);
                        });

                        if (data.obj.IsChallengeRequired == true) {
                            ctrl.ApiPath = data.obj.ApiPath;
                            ctrl.IsChallengeRequired = data.obj.IsChallengeRequired;
                        }
                    }
                });
        }

        ctrl.deActivate = function() {
            instagramAuthService.deActivate().then(function () {
                ctrl.getSettings();
                if (ctrl.onAddDelInstagram) {
                    ctrl.onAddDelInstagram();
                }
            });
        }

        ctrl.saveSettings = function () {
            instagramAuthService.saveSettings(ctrl.settings.salesFunnelId, ctrl.settings.createLeadFromDirectMessages, ctrl.settings.createLeadFromComments).then(function (data) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
            });
        }


        ctrl.requireChallengeCode = function(choice) {
            instagramAuthService.requireChallengeCode(ctrl.ApiPath, choice).then(function (data) {

                if (data.result) {
                    ctrl.IsChallengeRequired = false;
                    ctrl.IsChallengeRequiredCode = true;

                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error);
                    });
                }
            });
        }

        ctrl.sendChallengeCode = function() {
            instagramAuthService.sendChallengeCode(ctrl.ApiPath, ctrl.code).then(function (data) {

                if (data.result) {

                    ctrl.IsChallengeRequiredCode = false;

                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.SettingsSuccessfulySaved'));
                    ctrl.getSettings();
                    if (ctrl.onAddDelInstagram) {
                        ctrl.onAddDelInstagram();
                    }

                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error);
                    });
                }
            });
        }


    };

    instagramAuthCtrl.$inject = ['$http', 'toaster', 'SweetAlert', 'instagramAuthService', '$translate'];

    ng.module('instagramAuth', [])
        .controller('instagramAuthCtrl', instagramAuthCtrl)
        .component('instagramAuth', {
            templateUrl: '../areas/admin/content/src/instagram/components/instagramAuth/instagramAuth.html',
            controller: 'instagramAuthCtrl',
            bindings: {
                saasData: '<',
                onAddDelInstagram: '&'
            }
        });

})(window.angular);