; (function (ng) {

    'use strict';

    var authService = function ($http) {
        var service = this;

        service.login = function (email, password, captchaCode, captchaSource) {
            return $http.post('/user/loginjson', { email: email, password: password, captchaCode: captchaCode, captchaSource: captchaSource }).then(function (response) {
                return response.data;
            });
        }

        service.getCaptchaHtml = function (ngModel) {
            return $http.post('/commonExt/getCaptchaHtml', { ngModel: ngModel }).then(function (response) {
                return response.data;
            });
        }
    };

    ng.module('auth')
      .service('authService', authService);

    authService.$inject = ['$http'];

})(window.angular);