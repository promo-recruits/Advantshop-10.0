; (function (ng) {
    'use strict';

    var ModalAddEditSmsTemplateCtrl = function ($uibModalInstance, $http, $window, toaster, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.SmsTypeId = params != null && params.SmsTypeId != null ? params.SmsTypeId : null;
            ctrl.IsNew = params != null && params.IsNew != null ? params.IsNew : false;

            $http.get('smstemplates/AddEditSmsTemplate', { params: { smsTypeId: ctrl.SmsTypeId || 'None', IsNew: ctrl.IsNew } })
                 .then(function (result) {
                     ctrl.SmsTypes = result.data.obj.SmsTypes;
                     ctrl.AvalibleVarible = result.data.obj.AvalibleVarible;
                     ctrl.SmsBody = result.data.obj.SmsBody;
                 },
                function (err) {
                    toaster.pop('error', '', $translate.instant('Admin.Js.SmsTemplate.ErrorRetrievingTemplate') + err);
                 });

        };

        ctrl.refresh = function () {
            $http.get('smstemplates/AddEditSmsTemplate', { params: { smsTypeId: ctrl.SmsTypeId || 'None', IsNew: ctrl.IsNew } })
                 .then(function (result) {
                     ctrl.SmsTypes = result.data.obj.SmsTypes;
                     ctrl.AvalibleVarible = result.data.obj.AvalibleVarible;
                 },
                 function (err) {
                     toaster.pop('error', '', $translate.instant('Admin.Js.SmsTemplate.ErrorRetrievingTemplate') + err);
                 });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.saveTemplate = function () {
            ctrl.btnLoading = true;
            $http.post('smstemplates/addeditsmstemplate',
                {
                    SmsTypeId: ctrl.SmsTypeId,
                    SmsBody: ctrl.SmsBody,
                    IsNew: ctrl.IsNew
                })
                .then(function (result) {
                    var data = result.data.result;
                    if (data === true) {
                        toaster.pop('success', '', ctrl.IsNew ? $translate.instant('Admin.Js.SmsTemplate.TemplateAdded') : $translate.instant('Admin.Js.SmsTemplate.TemplateEditedSuccessfully'));
                        $uibModalInstance.close();
                    } else {

                        var message = result.data.errors.reduce(function (old, current) {
                           return old += current;
                        }, '');

                        toaster.pop('error', message);
                    }
                },
                function (err) {
                    toaster.pop('error', '', ctrl.IsNew ? $translate.instant('Admin.Js.SmsTemplate.ErrorWhileAddingTemplate') : $translate.instant('Admin.Js.SmsTemplate.ErrorEditingTemplate'));
                    }).finally(function () {
                        ctrl.btnLoading = false;
                    });;
        };
    };

    ModalAddEditSmsTemplateCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$q', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddEditSmsTemplateCtrl', ModalAddEditSmsTemplateCtrl);

})(window.angular);