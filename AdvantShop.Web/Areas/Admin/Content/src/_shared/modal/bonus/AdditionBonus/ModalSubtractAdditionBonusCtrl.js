; (function (ng) {
    'use strict';

    var ModalSubtractAdditionBonusCtrl = function ($uibModalInstance, $http, $window, toaster, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.cardId = params != null && params.cardId != null ? params.cardId : null;
            ctrl.SendSms = params != null && params.sendSms != undefined ? params.sendSms : true;

            $http.get('cards/getAdditionBonus?cardId='+ ctrl.cardId )
                 .then(function (result) {
                     ctrl.additionBonuses = result.data.obj;
                 },
                function (err) {
                    toaster.pop('error', '', $translate.instant('Admin.Js.AdditionBonus.ErrorGettingBonuses') + err);
                 });

        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.subctractBonus = function () {
            $http.post('cards/subtractAdditionBonus',
            {
                cardId: ctrl.cardId,
                amount: ctrl.amount,
                reason: ctrl.reason,
                additionId: ctrl.additionId,
                sendsms: ctrl.SendSms
            })
                .then(function (result) {
                    var data = result.data.result;
                    if (data === true) {
                        $window.location.assign('cards/edit/' + ctrl.cardId);
                        toaster.pop('success', '', $translate.instant('Admin.Js.AdditionBonus.BonusesAreWrittenOff'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.AdditionBonus.ErrorWhileWritingOffBonuses'));
                    }
                },
                    function (err) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.AdditionBonus.ErrorWhileWritingOffBonuses') + err);
                    });
        };

        ctrl.onChangeSwitch = function (checked) {
            ctrl.SendSms = checked;
        };
    };

    ModalSubtractAdditionBonusCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$q', '$translate'];

    ng.module('uiModal')
        .controller('ModalSubtractAdditionBonusCtrl', ModalSubtractAdditionBonusCtrl);

})(window.angular);