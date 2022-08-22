; (function (ng) {
    'use strict';

    var PARAM_KEY = 'partnerIdInfo';

    var partnerInfoService = function ($location, $http, $q) {
        var service = this,
            _container,
            arrayDefers = [];

        service.initContainer = function (container) {
            _container = container;

            arrayDefers.forEach(function (defer) {
                defer.resolve(container);
            });
        };

        service.addInstance = function (partnerId) {
            var defer = $q.defer(),
                promise;
         
            if (_container != null) {
                promise = $q.when(_container);
            } else {
                promise = defer.promise;
                arrayDefers.push(defer);
            }

            return promise.then(function (result) {
                result.addInstance(partnerId);
            });
        };

        service.setUrlParam = function (partnerid) {
            $location.search(PARAM_KEY, partnerid);
        };

        service.removeUrlParam = function (partnerid) {
            $location.search(PARAM_KEY, null);
        };

        service.getUrlParam = function (partnerid) {
            var search = $location.search();
            return search != null && search[PARAM_KEY] != null ? search[PARAM_KEY] : null;
        };
    };

    partnerInfoService.$inject = ['$location','$http', '$q'];

    ng.module('partnerInfo')
        .service('partnerInfoService', partnerInfoService);

})(window.angular);