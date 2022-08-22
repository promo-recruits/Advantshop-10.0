; (function (ng) {
    'use strict';

    var ModalAddAdditionBonusCtrl = function ($uibModalInstance, $http, $window, toaster, $q, $translate) {
        var ctrl = this;
        ctrl.isStartExport = false;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.cardId = params != null && params.cardId != null ? params.cardId : null;
            ctrl.filter = params != null && params.filter != null ? params.filter : null;
            ctrl.SendSms = params != null && params.sendSms != undefined ? params.sendSms : true;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.addBonus = function () {
            ctrl.btnLoading = true;
            if (ctrl.cardId != null) {
                ctrl.singleAdd();
            } else {
                ctrl.massAdd();
            }
        };

        ctrl.singleAdd = function () {
            $http.post('cards/addAdditionBonus',
                {
                    cardId: ctrl.cardId,
                    amount: ctrl.amount,
                    reason: ctrl.reason,
                    name: ctrl.name,
                    startDate: ctrl.startDate,
                    endDate: ctrl.endDate,
                    sendsms: ctrl.SendSms
                })
                .then(function (result) {
                    var data = result.data.result;
                    if (data === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.AdditionBonus.BonusesAdded'));
                        $window.location.assign('cards/edit/' + ctrl.cardId);
                        return data;
                    } else {
                        return $q.reject(data);
                    }
                })
                .catch(function () {
                    toaster.pop('error', '', $translate.instant('Admin.Js.AdditionBonus.ErrorAddingBonuses'));
                    ctrl.btnLoading = false;
                });
        };

        ctrl.massAdd = function () {
            $http.post('ExportImportCommon/GetCommonStatistic').then(function (response) {
                if (!response.data.IsRun) {
                    $http.post('cards/addAdditionBonusmass',
                            {
                                amount: ctrl.amount,
                                reason: ctrl.reason,
                                name: ctrl.name,
                                startDate: ctrl.startDate,
                                endDate: ctrl.endDate,
                                sendsms: ctrl.SendSms,
                                FIO: ctrl.FIO,
                                Email: ctrl.Email,
                                MobilePhone: ctrl.MobilePhone,
                                CardNumber: ctrl.CardNumber,
                                GradeId: ctrl.GradeId,
                                BonusAmountFrom: ctrl.BonusAmountFrom,
                                BonusAmountTo: ctrl.BonusAmountTo,
                                SelectMode: ctrl.filter.selectMode,
                                Ids: ctrl.filter.ids
                            })
                        .then(function (result) {
                                //var data = result.data.result;
                                if (result.data.result === true && result.data.obj === true) {
                                    //$window.location.assign('cards/index');
                                    ctrl.isStartExport = true;
                                    toaster.pop('success', '', $translate.instant('Admin.Js.AdditionBonus.AdditionStarted'));
                                } else {
                                    toaster.pop('error', '', $translate.instant('Admin.Js.AdditionBonus.ErrorAddingBonuses'));
                                    ctrl.btnLoading = false;
                                }
                            },
                            function (err) {
                                toaster.pop('error', '', $translate.instant('Admin.Js.AdditionBonus.ErrorAddingBonuses'));
                                ctrl.btnLoading = false;
                            }).finally(function () {
                            //ctrl.btnLoading = false;
                        });
                } else {
                    toaster.error('', $translate.instant('Admin.Js.CommonStatistic.AlreadyRunning') +
                        ' <a href="' + response.data.CurrentProcess + '">' + (response.data.CurrentProcessName || response.data.CurrentProcess) + '</a>');
                    ctrl.btnLoading = false;
                }
            });
        };

        ctrl.onTick = function (data) {
            ctrl.btnLoading = data.IsRun;
        };

        ctrl.onChangeSwitch = function (checked) {
            ctrl.SendSms = checked;
        };
    };

    ModalAddAdditionBonusCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$q', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddAdditionBonusCtrl', ModalAddAdditionBonusCtrl);

})(window.angular);