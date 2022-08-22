(function () {
    'use strict';

    var RegistrationPageCtrl = function ($http, $q, $timeout, $translate, $window, toaster) {

        var vm = this;

        vm.submit = function () {

            vm.checkCaptcha()
                .then(function () {
                    return $http.post('user/registrationjson', {
                        firstName: vm.fname,
                        lastname: vm.lastname,
                        patronymic: vm.patronymic,
                        email: vm.email,
                        phone: vm.phone,
                        birthday: vm.birthday,
                        password: vm.pass,
                        passwordConfirm: vm.passagain,
                        wantBonusCard: vm.wantBonusCard,
                        newsSubscription: vm.subscr,
                        agree: vm.agreement,
                        customerFields: vm.CustomerFields,
                        lpId: vm.lpId
                    })
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        $window.location.assign(response.data.obj);
                    } else {
                        return $q.reject(response.data.errors);
                    }
                })
                .catch(function (result) {
                    toaster.pop('error', Array.isArray(result) ? result.join('<br>') : result);
                });
        }

        vm.checkCaptcha = function () {
            if (typeof (CaptchaSource) != 'undefined') {
                CaptchaSource.InputId = 'CaptchaCode';

                return $http.get(CaptchaSource.ValidationUrl + '&i=' + CaptchaSource.GetInputElement().value)
                    .then(function (result) {
                        $timeout(function () { CaptchaSource.ReloadImage(); }, 1000);
                        CaptchaSource.GetInputElement().value = '';

                        if (result.data === true) {
                            return $q.resolve();
                        } else {
                            return $q.reject($translate.instant('Js.Captcha.Wrong'));
                        }
                    });
            } else {
                return $q.resolve();
            }
        }
    }

    RegistrationPageCtrl.$inject = ['$http', '$q', '$timeout', '$translate', '$window', 'toaster'];

    angular.module('registrationPage')
        .controller('RegistrationPageCtrl', RegistrationPageCtrl);

})();

