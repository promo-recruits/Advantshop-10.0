; (function (ng) {
    'use strict';

    var ModalSubtractMainBonusCtrl = function ($uibModalInstance, $http, $window, toaster, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.cardId = params != null && params.cardId != null ? params.cardId : null;
            ctrl.SendSms = params != null && params.sendSms != undefined ? params.sendSms : true;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.subtractBonus = function () {
            $http.post('cards/subtractBonus',
                {
                    cardId: ctrl.cardId,
                    amount: ctrl.Amount,
                    reason: ctrl.Reason,
                    sendsms: ctrl.SendSms
                })
                .then(function (result) {
                    var data = result.data.result;
                    if (data === true) {
                        $window.location.assign('cards/edit/' + ctrl.cardId);
                        toaster.pop('success', '', $translate.instant('Admin.Js.MainBonus.BonusesAreWrittenOff'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.MainBonus.ErrorWritingOffBonuses'));
                    }
                },
                    function (err) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.MainBonus.ErrorWritingOffBonuses'));
                    });
        };

        ctrl.onChangeSwitch = function (checked) {
            ctrl.SendSms = checked;
        };
    };

    ModalSubtractMainBonusCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$q', '$translate'];

    ng.module('uiModal')
        .controller('ModalSubtractMainBonusCtrl', ModalSubtractMainBonusCtrl);

})(window.angular);