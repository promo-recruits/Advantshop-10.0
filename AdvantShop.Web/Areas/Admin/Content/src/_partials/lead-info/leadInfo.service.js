; (function (ng) {
    'use strict';

    var PARAM_KEY = 'leadIdInfo';

    var leadInfoService = function ($location, $http, $q) {
        var service = this,
            _container,
            arrayDefers = [];

        service.initContainer = function (container) {
            _container = container;

            arrayDefers.forEach(function (defer) {
                defer.resolve(container);
            });
        };

        service.addInstance = function (leadid) {
            var defer = $q.defer(),
                promise;
         
            if (_container != null) {
                promise = $q.when(_container);
            } else {
                promise = defer.promise;
                arrayDefers.push(defer);
            }

            return promise.then(function (result) {
                result.addInstance(leadid);
            });
        };

        service.setUrlParam = function (leadid) {
            $location.search(PARAM_KEY, leadid);
        };

        service.removeUrlParam = function (leadid) {
            $location.search(PARAM_KEY, null);
        };

        service.getUrlParam = function (leadid) {
            var search = $location.search();
            return search != null && search[PARAM_KEY] != null ? search[PARAM_KEY] : null;
        };
    };

    leadInfoService.$inject = ['$location','$http', '$q'];

    ng.module('leadInfo')
        .service('leadInfoService', leadInfoService);

})(window.angular);