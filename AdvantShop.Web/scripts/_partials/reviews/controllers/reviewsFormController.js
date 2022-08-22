/* @ngInject */
function ReviewsFormCtrl($timeout, toaster, $translate, $http) {
    var ctrl = this;

    ctrl.nameFocus = ctrl.emailFocus = ctrl.textFocus = false;
    ctrl.images = [];

    ctrl.selectedImage = function (files) {
        if (files && files.length) {
            for (var i = 0; i < files.length; i++) {
                ctrl.pushImages(files[i]);
            }
        }
    };

    ctrl.pushImages = function (image) {
        ctrl.images.push(image || {});
    };

    ctrl.deleteImage = function (index) {
        ctrl.images.splice(index, 1);
    };

    ctrl.submit = function () {
        if (ctrl.isShowUserAgreementText && !ctrl.agreement) {
            toaster.pop('error', $translate.instant('Js.Subscribe.ErrorAgreement'));
            return;
        }

        ctrl.images = ctrl.images.filter(function (image) { return image.name; });

        var sendResult = false;

        if (typeof (CaptchaSource) != "undefined") {
            CaptchaSource.InputId = "CaptchaCode";

            ctrl.captchaCode = CaptchaSource.GetInputElement().value;
            ctrl.captchaSource = CaptchaSource.InstanceId;

            $http.get(CaptchaSource.ValidationUrl + '&i=' + CaptchaSource.GetInputElement().value)
                .then(function (result) {
                    if (result.data === true) {
                        ctrl.submitFn({ form: ctrl });
                        sendResult = true;
                    } else {
                        toaster.pop('error', $translate.instant('Js.Captcha.Wrong'));
                    }
                })
                .then(function () {
                    $timeout(function () { CaptchaSource.ReloadImage(); }, 1000);
                    CaptchaSource.GetInputElement().value = '';

                    if ((ctrl.moderate == null || ctrl.moderate == false) && sendResult === true) {
                        $translate(['Js.Reviews.SuccessTitle', 'Js.Reviews.SuccessMessage']).then(function (translations) {
                            toaster.success(translations['Js.Reviews.SuccessTitle'], translations['Js.Reviews.SuccessMessage']);
                        });
                    }
                });
        }
        else {
            ctrl.submitFn({ form: ctrl });
            sendResult = true;

            if ((ctrl.moderate == null || ctrl.moderate == false) && sendResult === true) {
                $translate(['Js.Reviews.SuccessTitle', 'Js.Reviews.SuccessMessage']).then(function (translations) {
                    toaster.success(translations['Js.Reviews.SuccessTitle'], translations['Js.Reviews.SuccessMessage']);
                });
            }
        }
    };

    ctrl.reset = function () {
        //formScope.name = '';
        //formScope.email = '';
        ctrl.text = '';
        ctrl.images = [];
        ctrl.agreement = false;
        ctrl.form.$setPristine();
    };

    ctrl.setAutofocus = function () {
        ctrl.nameFocus = ctrl.emailFocus = ctrl.textFocus = false;

        $timeout(function () {
            if (ctrl.name == null || ctrl.name.length === 0) {
                ctrl.nameFocus = true;
            } else if (ctrl.email == null || ctrl.email.length === 0) {
                ctrl.emailFocus = true;
            }
            else if (ctrl.text == null || ctrl.text.length === 0) {
                ctrl.textFocus = true;
            }
        }, 0);
    };
};

export default ReviewsFormCtrl;