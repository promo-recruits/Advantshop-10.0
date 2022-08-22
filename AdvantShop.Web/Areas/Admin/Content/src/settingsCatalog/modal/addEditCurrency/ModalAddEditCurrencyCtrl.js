﻿; (function (ng) {
    'use strict';

    var ModalAddEditCurrencyCtrl = function ($uibModalInstance, $http, $filter, toaster, $translate) {
        var ctrl = this;
        ctrl.formInited = false;

        ctrl.$onInit = function () {

            var params = ctrl.$resolve;
            ctrl.id = params.id != null ? params.id : 0;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';


            if (ctrl.mode == 'add') {
                ctrl.Name = '';
                ctrl.Rate = 1;
                ctrl.RoundNumbers = -1;
                ctrl.formInited = true;
            } else {
                ctrl.getCurrency(ctrl.id);
            }

        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getCurrency = function (id) {
            $http.get('settingsCatalog/getCurrency', { params: { id: id, rnd: Math.random() } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.id = data.CurrencyId
                    ctrl.Name = data.Name;
                    ctrl.Rate = data.Rate;
                    ctrl.EnablePriceRounding = data.EnablePriceRounding;
                    ctrl.IsCodeBefore = data.IsCodeBefore;
                    ctrl.Iso3 = data.Iso3;
                    ctrl.NumIso3 = data.NumIso3;
                    ctrl.RoundNumbers = data.RoundNumbers;
                    ctrl.Symbol = data.Symbol;
                }
                ctrl.form.$setPristine();
                ctrl.formInited = true;
            });
        }

        ctrl.save = function () {

            ctrl.btnSleep = true;

            var params = {
                CurrencyId: ctrl.id,
                Name: ctrl.Name,
                Rate: ctrl.Rate,
                EnablePriceRounding: ctrl.EnablePriceRounding,
                IsCodeBefore: ctrl.IsCodeBefore,
                Iso3: ctrl.Iso3,
                NumIso3: ctrl.NumIso3,
                RoundNumbers: ctrl.RoundNumbers,
                Symbol: ctrl.Symbol,
                rnd: Math.random()
            };

            var url = ctrl.mode == 'add' ? 'settingsCatalog/addCurrency' : 'settingsCatalog/updateCurrency';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop('success', '', ctrl.mode == 'add' ? $translate.instant('Admin.Js.SettingsCatalog.CurrencyAdded') : $translate.instant('Admin.Js.SettingsCatalog.ChangesSaved'));
                    $uibModalInstance.close('saveCurrency');
                } else {
                    if (data.errors != null) {
                        data.errors.forEach(function(error) {
                            toaster.pop('error', '', error);
                        });
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.SettingsCatalog.Error'), $translate.instant('Admin.Js.SettingsCatalog.ErrorWhile') + (ctrl.mode == 'add' ? $translate.instant('Admin.Js.SettingsCatalog.WhileCreating') : $translate.instant('Admin.Js.SettingsCatalog.WhileEditing')));
                    }
                    ctrl.btnSleep = false;
                }
            });
        }
    };

    ModalAddEditCurrencyCtrl.$inject = ['$uibModalInstance', '$http', '$filter', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddEditCurrencyCtrl', ModalAddEditCurrencyCtrl);

})(window.angular);