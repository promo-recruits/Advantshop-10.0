; (function (ng) {
    'use strict';
    window.addEventListener('load', function () {
        var portBtns = document.querySelectorAll('.trigger-port');
        if (portBtns.length != 0) {
            for (var i = 0; i < portBtns.length; i++) {
                portBtns[i].data = i;
            }
        }
    });

    var ModuleCtrl = function ($http, toaster, $translate, SweetAlert) {

        var ctrl = this;
            ctrl.tab = 0;

        ctrl.onInit = function () {
            ctrl.activeImport = false;
        };
        
        ctrl.changeEnabled = function () {
            $http.post('modules/changeEnabled', { stringId: ctrl.stringId, enabled: ctrl.enabled }).then(function (response) {
                if (response.data.result === true) {
                    if (!response.data.obj.SaasAndPaid || ctrl.enabled) {
                        toaster.pop('success', '', ctrl.enabled ? $translate.instant('Admin.Js.Module.ModuleIsActivated') : $translate.instant('Admin.Js.Module.ModuleIsNotActive'));
                    } else {
                        SweetAlert.info('', { title: '', html: $translate.instant('Admin.Js.Modules.DeactivatedAndPayable')});
                    }
                } else {
                    ctrl.enabled = false;
                    toaster.pop('error', '', $translate.instant('Admin.Js.Module.ErrorWhileSaving'));
                }
            });
        }

        ctrl.setTab = function(newTab) {
            ctrl.tab = newTab;
        }

        ctrl.isSet = function (tabNum) {
            return ctrl.tab === tabNum;
        };
    }


    ModuleCtrl.$inject = ['$http', 'toaster', '$translate', 'SweetAlert'];

    ng.module('module', [])
      .controller('ModuleCtrl', ModuleCtrl);

})(window.angular);