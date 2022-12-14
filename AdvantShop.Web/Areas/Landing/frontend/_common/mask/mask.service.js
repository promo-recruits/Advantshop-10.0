(function () {
    'use strict';

    var maskControlService = function () {
        var service = this;
        var maskControlConfig = {};

        service.setMaskControlConfig = function (config) {
            maskControlConfig = Object.assign({}, config);
        };

        service.getMaskControlConfig = function () {
            return maskControlConfig;
        };
    }

    angular.module('mask')
        .service('maskControlService', maskControlService);

})();
