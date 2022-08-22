; (function (ng) {
    'use strict';

    var ModalEmailSettingsUpdateAddressCtrl = function ($uibModalInstance, $http, toaster, $translate, SweetAlert) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.value || {};
            ctrl.email = params.email;
            ctrl.emailsInvalid = [{
                value: '@mail.ru',
                error: $translate.instant('Admin.Js.MailSettings.MailServiceDoesNotSupportTransactions')
            }];
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function (email) {

            var emailsError = ctrl.emailsInvalid.filter(function (item) { return email.indexOf(item.value) !== -1; });

            if (emailsError != null && emailsError.length > 0) {
                return SweetAlert.alert(emailsError.map(function (item) { return item.error; }).join('<br>'), { title: $translate.instant('Admin.Js.AdminWebNotifications.Attention') });
            } else {
                $http.post('/SettingsMail/UpdateAddress', { email: email })
                    .then(function (response) {
                        if (response.data.result) {
                            $uibModalInstance.close(response.data.obj);
                            toaster.pop('success', '', $translate.instant('Admin.Js.MainPageProducts.ChangesSaved'));
                        } else {
                            toaster.pop('error', $translate.instant('Admin.Js.MailSettings.Error'), response.data.errors.join('<br>'));
                        }
                    })
                    .catch(function (err) {
                        toaster.pop('error', $translate.instant('Admin.Js.MailSettings.Error'));
                    });
            }
        };
    };

    ModalEmailSettingsUpdateAddressCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', 'SweetAlert'];

    ng.module('uiModal')
        .controller('ModalEmailSettingsUpdateAddressCtrl', ModalEmailSettingsUpdateAddressCtrl);

})(window.angular);