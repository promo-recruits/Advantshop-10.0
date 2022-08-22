; (function (ng) {
    'use strict';

    var PARAM_KEY = 'customerIdInfo';

    var customerInfoService = function ($location, $http, $q) {
        var service = this,
            _container,
            arrayDefers = [];

        service.initContainer = function (container) {
            _container = container;

            arrayDefers.forEach(function (defer) {
                defer.resolve(container);
            });
        };

        service.addInstance = function (instance) {
            var defer = $q.defer(),
                promise;
         
            if (_container != null) {
                promise = $q.when(_container);
            } else {
                promise = defer.promise;
                arrayDefers.push(defer);
            }

            return promise.then(function (result) {
                result.addInstance(instance);
            });
        };

        service.setUrlParam = function (customerid) {
            $location.search(PARAM_KEY, customerid);
        };

        service.removeUrlParam = function (customerid) {
            $location.search(PARAM_KEY, null);
        };

        service.getUrlParam = function (customerid) {
            var search = $location.search();
            return search != null && search[PARAM_KEY] != null ? search[PARAM_KEY] : null;
        };
    };

    customerInfoService.$inject = ['$location','$http', '$q'];

    ng.module('customerInfo')
        .service('customerInfoService', customerInfoService);

})(window.angular);