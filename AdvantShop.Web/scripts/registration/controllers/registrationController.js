
class RegistrationPageCtrl {
    /*@ngInject*/
    constructor($http, $q, $timeout, $translate, $window, toaster) {
        this.$http = $http;
        this.$q = $q;
        this.$timeout = $timeout;
        this.$translate = $translate;
        this.toaster = toaster;
        this.$window = $window;
    }

    submit() {

        this.checkCaptcha()
            .then(result => {
                return this.$http.post('user/registrationjson', {
                    firstName: this.fname,
                    lastname: this.lastname,
                    patronymic: this.patronymic,
                    email: this.email,
                    phone: this.phone,
                    birthday: this.birthday,
                    password: this.pass,
                    passwordConfirm: this.passagain,
                    wantBonusCard: this.wantBonusCard,
                    newsSubscription: this.subscr,
                    agree: this.agreement,
                    customerFields: this.CustomerFields,
                    lpId: this.lpId
                })
            })
            .then(response => {
                if (response.data.result === true) {
                    this.$window.location.assign(response.data.obj);
                } else {
                    return this.$q.reject(response.data.errors);
                }
            })
            .catch(result => {
                this.toaster.pop('error', '', Array.isArray(result) ? result.join('<br>') : result);
            });
    }

    checkCaptcha() {
        if (typeof (CaptchaSource) != 'undefined') {
            CaptchaSource.InputId = 'CaptchaCode';

            return this.$http.get(CaptchaSource.ValidationUrl + '&i=' + CaptchaSource.GetInputElement().value)
                .then(result => {
                    this.$timeout(function () { CaptchaSource.ReloadImage(); }, 1000);
                    CaptchaSource.GetInputElement().value = '';

                    if (result.data === true) {
                        return this.$q.resolve();
                    } else {
                        return this.$q.reject(this.$translate.instant('Js.Captcha.Wrong'));
                    }
                });
        } else {
            return this.$q.resolve();
        }
    }
}


export default RegistrationPageCtrl;