; (function (ng) {
    'use strict';

    var ModalAddEditSmsAnswerTemplateCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {

            var params = ctrl.$resolve.params || {};
            ctrl.TemplateId = params.id != null ? params.id : 0;
            ctrl.mode = ctrl.TemplateId !== 0 ? "edit" : "add";


            if (ctrl.TemplateId !== 0) {
                ctrl.getSmsAnswerTemplate(ctrl.TemplateId);
            }
            else {
                ctrl.SortOrder = 0;
                ctrl.Active = true;
            }
        };

        ctrl.getSmsAnswerTemplate = function (id) {
            return $http.get('settingsMail/getSmsAnswerTemplate', { params: { id: id } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.Name = data.Name;
                    ctrl.Active = data.Active;
                    ctrl.Text = data.Text;
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
                Active: ctrl.Active,
                Text: ctrl.Text,
                SortOrder: ctrl.SortOrder,
            };

            var url = ctrl.mode === "add" ? 'settingsMail/AddSmsTemplate' : 'settingsMail/EditSmsTemplate';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop("success", "", $translate.instant('Admin.Js.MainPageProducts.ChangesSaved'));
                    $uibModalInstance.close();
                } else {
                    toaster.pop("error", $translate.instant('Admin.Js.MailSettings.Error'), data.errors || $translate.instant('Admin.Js.MailSettings.ErrorWhileCreatingEditing'));
                }
            });
        }
    };

    ModalAddEditSmsAnswerTemplateCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddEditSmsAnswerTemplateCtrl', ModalAddEditSmsAnswerTemplateCtrl);

})(window.angular);