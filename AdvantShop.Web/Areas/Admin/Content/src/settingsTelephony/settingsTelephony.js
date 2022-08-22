; (function (ng) {
    'use strict';

    var SettingsTelephonyCtrl = function ($http, $q, $window, SweetAlert, toaster, $translate) {

        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getOrderSources().then(function () {
                ctrl.getPhoneOrderSources();
            });
        }

        ctrl.copy = function (data) {
            var input = document.createElement('input');
            input.setAttribute('value', data);
            input.style.opacity = 0;
            document.body.appendChild(input);
            input.select();
            if (document.execCommand('copy')) {
                toaster.pop('success', $translate.instant('Admin.Js.SettingsTelephony.LinkCopiedToClipboard'));
            } else {
                toaster.pop('error', $translate.instant('Admin.Js.SettingsTelephony.FailedToCopyLink'));
            }
            document.body.removeChild(input);
        }

        // phone order sources

        ctrl.getOrderSources = function () {
            return $http.post('orders/getOrderSources').then(function (response) {
                ctrl.orderSources = response.data;
            });
        }

        ctrl.getPhoneOrderSources = function () {
            return $http.post('settingsTelephony/getPhoneOrderSources').then(function (response) {
                ctrl.phoneOrderSources = response.data.obj; 
            });
        }

        ctrl.savePhoneOrderSources = function (phone, orderSourceId) {
            if (phone) {
                ctrl.phoneOrderSources[phone] = orderSourceId;
            }
            $http.post('settingsTelephony/savePhoneOrderSources', { phoneOrderSources: JSON.stringify(ctrl.phoneOrderSources) }).then(function (response) {
                if (response.data.result == true) {
                    toaster.success($translate.instant('Admin.Js.SettingsTelephony.ChangesSaved'));
                } else {
                    toaster.error($translate.instant('Admin.Js.SettingsTelephony.ErrorWhileSaving'));
                }
                ctrl.getPhoneOrderSources();
            });
        }

        ctrl.addPhone = function () {
            if (!ctrl.newPhone || !ctrl.newOrderSourceId) {
                toaster.error($translate.instant('Admin.Js.SettingsTelephony.MissingData'));
                return;
            }
            if (ctrl.phoneOrderSources[ctrl.newPhone]) {
                toaster.error($translate.instant('Admin.Js.SettingsTelephony.BundleWithThisNumber'));
                return;
            }
            ctrl.phoneOrderSources[ctrl.newPhone] = ctrl.newOrderSourceId;
            ctrl.savePhoneOrderSources();
            ctrl.newPhone = ctrl.newOrderSourceId = '';
        }

        ctrl.deletePhone = function (phone) {
            SweetAlert.confirm($translate.instant('Admin.Js.SettingsTelephony.AreYouSureDelete'), { title: $translate.instant('Admin.Js.SettingsTelephony.Deleting') }).then(function (result) {
                if (result === true) {
                    delete ctrl.phoneOrderSources[phone];
                    ctrl.savePhoneOrderSources();
                }
            });
        }

        // telphin

        ctrl.getTelphinExtensions = function () {
            if (!ctrl.telphinAppKey || !ctrl.telphinAppSecret) {
                $window.document.getElementById(ctrl.telphinAppKey ? 'TelphinAppSecret' : 'TelphinAppKey').focus();
                toaster.error($translate.instant('Admin.Js.SettingsTelephony.SpecifyApplicationData'));
                return;
            }
            $http.post('settingsTelephony/getTelphinExtensions').then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.telphinExtensions = data.obj;
                    toaster.success($translate.instant('Admin.Js.SettingsTelephony.DataUpdated'));
                } else {
                    toaster.error($translate.instant('Admin.Js.SettingsTelephony.FailedToRetrieveData'));
                }
            });
        }

        ctrl.addTelphinEvents = function (ext) {
            $http.post('settingsTelephony/addTelphinEvents', { extensionId: ext.id }).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.telphinExtensions = data.obj;
                    toaster.success($translate.instant('Admin.Js.SettingsTelephony.EventNotificationsForAddNumber') + ext.extension + $translate.instant('Admin.Js.SettingsTelephony.Installed'));
                } else {
                    data.errors.forEach(function (error) {
                        toaster.error($translate.instant('Admin.Js.SettingsTelephony.Error'), error);
                    });
                }
            });
        }

        ctrl.deleteTelphinEvents = function (ext) {
            SweetAlert.confirm($translate.instant('Admin.Js.SettingsTelephony.DeleteTelphinEvents'), { title: $translate.instant('Admin.Js.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('settingsTelephony/deleteTelphinEvents', { extensionId: ext.id }).then(function (response) {
                        var data = response.data;
                        if (data.result == true) {
                            ctrl.telphinExtensions = data.obj;
                            toaster.success($translate.instant('Admin.Js.SettingsTelephony.EventNotificationsForAddNumber') + ext.extension + $translate.instant('Admin.Js.SettingsTelephony.WasDeleted'));
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error($translate.instant('Admin.Js.SettingsTelephony.Error'), error);
                            });
                        }
                    });
                }
            });
        }

        ctrl.setTelphinEvents = function (ext) {
            SweetAlert.confirm($translate.instant('Admin.Js.SettingsTelephony.SetDefaultTelphinEvents'), { title: '' }).then(function (result) {
                if (result === true) {
                    $http.post('settingsTelephony/deleteTelphinEvents', { extensionId: ext.id }).then(function () {
                        ctrl.addTelphinEvents(ext);
                    });
                }
            });
        }
        
        ctrl.copyToClipboard = function (id) {
            try {
                window.getSelection().removeAllRanges();
                var link = document.querySelector('#' + id);
                link.select();
                var result = document.execCommand('copy');
                if (result) {
                    toaster.pop('success', '', 'Данные успешно скопированы');
                } else {
                    toaster.pop('error', '', 'Упс! Данные не удалось скопировать');
                }
                window.getSelection().removeAllRanges();
            } catch (error) {
                console.log(error);
            }
        }
    };

    SettingsTelephonyCtrl.$inject = ['$http', '$q', '$window', 'SweetAlert', 'toaster', '$translate'];

    ng.module('settingsTelephony', [])
      .controller('SettingsTelephonyCtrl', SettingsTelephonyCtrl);

})(window.angular);