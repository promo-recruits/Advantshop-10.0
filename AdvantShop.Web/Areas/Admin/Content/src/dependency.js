; (function (ng) {
    'use strict';

    var dependencyList = [];

    var dependencyService = function () {
        var service = this;

        service.add = function (items) {
            if (ng.isArray(items) === true) {
                dependencyList = dependencyList.concat(items);
            } else {
                dependencyList.push(items);
            }
        };

        service.get = function () {
            return dependencyList;
        };

    };

    ng.module('dependency', [])
        .service('dependencyService', dependencyService);


})(window.angular);