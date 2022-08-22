; (function (ng) {
    'use strict';

    var ModalAddEditMailAnswerTemplateCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {

            var params = ctrl.$resolve.value || {};
            ctrl.TemplateId = params.TemplateId != null ? params.TemplateId : 0;
            ctrl.mode = ctrl.TemplateId !== 0 ? "edit" : "add";
            

            if (ctrl.TemplateId !== 0) {
                ctrl.getMailAnswerTemplate(ctrl.TemplateId);
            }
            else {
                ctrl.SortOrder = 0;
                ctrl.Active = true;
            }
        };

        ctrl.getMailAnswerTemplate = function (id) {
            return $http.get('settingsMail/getMailAnswerTemplate', { params: { id: id } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.Name = data.Name;
                    ctrl.Active = data.Active;
                    ctrl.Subject = data.Subject;
                    ctrl.Body = data.Body;
                    ctrl.SortOrder = data.SortOrder;
                    ctrl.TemplateId = data.TemplateId;
                }
                else {
                    toaster.pop("error", $translate.instant('Admin.Js.MailSettings.Error'), response.data.error);
                }
            });
        }

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            var params = {
                TemplateId: ctrl.TemplateId,
                Name: ctrl.Name,
                Subject: ctrl.Subject,
                Active: ctrl.Active,
                Body: ctrl.Body,                
                SortOrder: ctrl.SortOrder,
            };

            var url = ctrl.mode === "add" ? 'settingsMail/AddMailTemplate' : 'settingsMail/EditMailTemplate';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop("success", "", $translate.instant('Admin.Js.MainPageProducts.ChangesSaved'));
                    $uibModalInstance.close('saveSize');
                } else {
                    toaster.pop("error", $translate.instant('Admin.Js.MailSettings.Error'), data.errors || $translate.instant('Admin.Js.MailSettings.ErrorWhileCreatingEditing'));
                }
            });
        }
    };

    ModalAddEditMailAnswerTemplateCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddEditMailAnswerTemplateCtrl', ModalAddEditMailAnswerTemplateCtrl);

})(window.angular);