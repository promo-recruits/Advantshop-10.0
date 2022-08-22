; (function (ng) {
    'use strict';

    var inplaceService = function ($http) {
        var service = this,
            storage = {
                rich: {}
            };

        service.addRich = function (id, obj) {
            storage.rich[id] = obj;
        };

        service.getRich = function (id) {
            return storage.rich[id];
        };

        service.save = function (url, params) {
            return $http.post(url, ng.extend(params, { rnd: Math.random() }));
        };

        service.destroyAll = function () {
            Object.keys(storage).forEach(function (keyDirective) {
                Object.keys(storage[keyDirective]).forEach(function (keyItem) {
                    if (storage[keyDirective][keyItem].destroy != null) {
                        storage[keyDirective][keyItem].destroy();
                        delete storage[keyDirective][keyItem];
                    }
                });
            });
        }
    };

    ng.module('inplace')
      .service('inplaceService', inplaceService);

    inplaceService.$inject = ['$http'];

})(window.angular);