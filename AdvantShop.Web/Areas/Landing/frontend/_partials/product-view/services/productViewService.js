﻿; (function (ng) {
    'use strict';

    var productViewService = function ($http, $q, $cookies, $window) {
        var service = this,
            productViewTransformers = {},
            queue = {},
            callbacks = {};

        service.getPhotos = function (productId) {
            return $http.get('productExt/getphotos', { params: { productId: productId, rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };

        service.getView = function (name) {

            var defer = $q.defer();

            if (productViewTransformers[name] == null) {
                queue[name] = defer;
            } else {
                defer.resolve(productViewTransformers[name]);
            }

            return defer.promise.finally(function () {
                delete queue[name];
            });
        };

        service.addCallback = function (name, func) {
            callbacks[name] = callbacks[name] || [];
            callbacks[name].push(func);
        };

        service.pricessCallback = function (name, data) {
            if (callbacks[name] != null) {
                for (var i = 0, len = callbacks[name].length - 1; i <= len; i++) {
                    callbacks[name][i](data);
                }
            }
        };

        service.setView = function (name, view, viewList, isMobile) {

            if (isMobile) {
                $cookies.put('mobile_viewmode', view);
            } else {
                if ($window.location.pathname.indexOf('/search') !== -1) {
                    $cookies.put('search_viewmode', view);
                } else {
                    $cookies.put('viewmode', view);
                }
            }

            productViewTransformers[name] = productViewTransformers[name] || {};

            productViewTransformers[name].viewName = view;
            productViewTransformers[name].viewList = viewList;

            if (queue[name] != null) {
                queue[name].resolve(productViewTransformers[name]);
            }

            service.pricessCallback('setView', productViewTransformers[name]);

            return productViewTransformers[name];
        };

        service.getViewFromCookie = function (cookieName, viewList) {
            var value = $cookies.get(cookieName);

            var item;

            for (var i = 0; i < viewList.length; i++) {
                if (viewList[i].indexOf(value) !== -1) {
                    item = viewList[i];
                    break;
                }
            }

            return item != null ? item : viewList[0];
        };

        service.getOfferId = function (productId, colorId, sizeId) {
            return $http.get('productExt/GetOffers', { params: { productId: productId, colorId: colorId, sizeId: sizeId, rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };
    };

    ng.module('productView')
      .service('productViewService', productViewService);

    productViewService.$inject = ['$http', '$q', '$cookies', '$window'];

})(window.angular);