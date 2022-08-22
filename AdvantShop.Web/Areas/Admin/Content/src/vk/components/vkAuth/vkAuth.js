; (function (ng) {
    'use strict';

    var vkAuthCtrl = function ($http, toaster, SweetAlert, vkService, $translate, $window) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getSettings();
        };

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

                //ctrl.salesFunnels = data.salesFunnels;
                //ctrl.salesFunnelId = data.salesFunnelId;

                //ctrl.createLeadFromMessages = data.createLeadFromMessages;
                //ctrl.createLeadFromComments = data.createLeadFromComments;
            });
        }

        // Авторизация в vk с правами пользователя, чтобы получить список групп
        ctrl.authVk = function() {
            var w = 700;
            var h = 525;

            var left = (screen.width / 2) - (w / 2);
            var top = (screen.height / 2) - (h / 2);

            var url = 'https://oauth.vk.com/authorize?client_id=' + ctrl.clientId +
                '&display=page' +
                '&redirect_uri=' + ctrl.redirectUrl +
                '&scope=offline,wall,groups,market,photos&response_type=token&v=5.64';

            var win = window.open(url, '', 'width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
            win.focus();

            var timer = window.setInterval(function () {
                try {
                    if (win.document.URL.indexOf(ctrl.redirectUrl) !== -1) {

                        var accessToken = '';
                        var userId = '';
                        var urlParts = win.document.URL.split('#')[1].split('&');

                        for (var i = 0; i < urlParts.length; i++) {

                            if (urlParts[i].indexOf('access_token') !== -1) {
                                accessToken = urlParts[i].replace('access_token=', '');
                            } else if (urlParts[i].indexOf('user_id') !== -1) {
                                userId = urlParts[i].replace('user_id=', '');
                            }
                        }

                        win.close();
                        window.clearInterval(timer);

                        return vkService
                            .saveAuthVkUser({ clientId: ctrl.clientId, accessToken: accessToken, userId: userId })
                            .then(function(data) {

                                ctrl.clienId = data.clientId;
                                ctrl.accessToken = data.accessToken;
                                ctrl.userId = data.userId;

                                vkService.getGroups().then(function (groups) {
                                    ctrl.groups = groups;
                                    if (ctrl.groups != null && ctrl.groups.length > 0) {
                                        ctrl.selectedGroup = ctrl.groups[0];
                                    }
                                });
                            });
                    }

                } catch (e) {
                    console.log(e);
                }

            }, 100);
        }

        // Авторизация в vk с правами пользователя, чтобы получить список групп
        ctrl.authGroup = function () {

            if (ctrl.selectedGroup == null) {
                return;
            }

            var group = ctrl.selectedGroup;

            var w = 700;
            var h = 525;

            var left = (screen.width / 2) - (w / 2);
            var top = (screen.height / 2) - (h / 2);

            var url = 'https://oauth.vk.com/authorize?client_id=' + ctrl.clientId +
                '&group_ids=' + group.Id +
                '&display=page' +
                '&redirect_uri=' + ctrl.redirectUrl +
                '&scope=messages,manage&response_type=token&v=5.64';

            var win = window.open(url, '', 'width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
            win.focus();

            var timer = window.setInterval(function () {
                try {
                    if (win.document.URL.indexOf(ctrl.redirectUrl) !== -1) {

                        var accessToken = '';
                        var urlParts = win.document.URL.split('#')[1].split('&');

                        for (var i = 0; i < urlParts.length; i++) {
                            if (urlParts[i].indexOf('access_token') !== -1) {
                                accessToken = urlParts[i].split('=')[1];
                            }
                        }

                        win.close();
                        window.clearInterval(timer);

                        return vkService
                            .saveAuthVkGroup({ group: group, accessToken: accessToken })
                            .then(function (data) {
                                if (data.result === true) {

                                    // TODO !
                                    //ctrl.getSettings();
                                    $window.location.reload(true);

                                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.Group') + group.Name + $translate.instant('Admin.Js.SettingsCrm.GroupIsConnected'));
                                    if (ctrl.onAddDelVk) {
                                        ctrl.onAddDelVk();
                                    }
                                } else {
                                    data.errors.forEach(function (error) {
                                        if (error) {
                                            toaster.error('', error);
                                        }
                                    });
                                }
                            });
                    }

                } catch (e) {
                    console.log(e);
                }

            }, 100);
        }

        //ctrl.deleteGroup = function() {
        //    vkService.deleteGroup().then(function () {
        //        ctrl.getSettings();
        //        if (ctrl.onAddDelVk) {
        //            ctrl.onAddDelVk();
        //        }
        //    });
        //}

        //ctrl.authVkByLoginPassword = function() {
        //    vkService.authVkByLoginPassword(ctrl.login, ctrl.password).then(function(data) {
        //        if (data.result === true) {
        //            ctrl.authByUser = true;
        //            toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.AuthSuccessfulSetupComplete'));
        //        } else {
        //            toaster.pop('error', '', $translate.instant('Admin.Js.SettingsCrm.FailedLogIn'));
        //        }
        //    });
        //}

        ctrl.saveSettings = function() {
            vkService.saveSettings(ctrl.salesFunnelId, ctrl.createLeadFromMessages, ctrl.createLeadFromComments).then(function (data) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
            });
        }

    };

    vkAuthCtrl.$inject = ['$http', 'toaster', 'SweetAlert', 'vkService', '$translate', '$window'];

    ng.module('vkAuth', [])
        .controller('vkAuthCtrl', vkAuthCtrl)
        .component('vkAuth', {
            templateUrl: '../areas/admin/content/src/vk/components/vkAuth/vkAuth.html',
            controller: 'vkAuthCtrl',
            bindings: {
                redirectUrl: '<?',
                onAddDelVk: '&'
            }
        });

})(window.angular);