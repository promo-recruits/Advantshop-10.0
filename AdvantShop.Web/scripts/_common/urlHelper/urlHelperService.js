﻿; (function (ng) {
    'use strict';


    var urlHelperService = function ($window, urlHelperConfig) {

        var service = this,
            tagBaseHref = document.getElementsByTagName('base')[0].getAttribute('href'),
            regexDomain = new RegExp('^(?:[a-z]+:)?//', 'i');

        
        service.getUrlParam = function (paramName, toLower) {
            paramName = toLower !== false ? paramName.toLowerCase() : paramName;
            var query = toLower !== false ? $window.location.search.substring(1).toLowerCase() : $window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                if (pair[0] == paramName) { return pair[1]; }
            }
            return null;
        };

        service.getUrlParamByName = function (name) {
            var url = $window.location.href.toLowerCase();
            name = name.toLowerCase().replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        };

        service.getUrlParamDictionaryByNameFunc = function (fn, paramsAsObject) {
            if (!paramsAsObject) return;

            var result = [];

            var paramsName = Object.keys(paramsAsObject);

            for (var i = 0; i < paramsName.length; i++) {
                if (fn(paramsName[i]) === true) {
                    result.push({ name: paramsName[i], value: paramsAsObject[paramsName[i]] });
                }
            }

            return result;
        };

        service.getUrlParamsAsObject = function (string) {
            const seachParams = new URLSearchParams(string);
            const obj = {};
            seachParams.forEach(function (key, value) {
                obj[value] = key;
            });

            return obj;
        };

        service.getBaseHref = function () {
            return tagBaseHref;
        };

        service.hasDomain = function (url) {
            return regexDomain.test(url);
        };

        service.getAbsUrl = function (url, excludeAdmin) {

            var base = service.getBaseHref(),
                basePrepare;

            if((excludeAdmin != null && excludeAdmin === true) || urlHelperConfig.isAdmin === false){
                basePrepare = base.replace(urlHelperConfig.adminPath, '');
            } else {
                basePrepare = base;
            }

            if (service.hasDomain(url) === false) {

                //убераем впереди слеш
                if (url.charAt(0) === '/') {
                    url = url.substring(1);
                }

                url = basePrepare + url;
            }

            return url;
        };

        service.getUrl = function (url, excludeAdmin) {

            var base = service.getBaseHref(),
                basePrepare = base.replace(/.*\/\/[^\/]*/, '');

            if ((excludeAdmin != null && excludeAdmin === true) || urlHelperConfig.isAdmin === false) {
                basePrepare = basePrepare.replace(urlHelperConfig.adminPath, '');
            }

            if (service.hasDomain(url) === false) {
                if (url.charAt(0) === '/') {
                    url = url.substring(1);
                }
                url = basePrepare + url;
            }
            return url;
        };

        service.paramsToString = function (object) {
            //var result = "";
            var result = [];
            for(var key in object)
            {
                if(object.hasOwnProperty(key))
                {
                    //result += key + "=" + object[key] + "&";
                    result.push(key + "=" + object[key]);
                }
            }
                       
            return result.join("&");
        };

        service.updateQueryStringParameter = function (uri, key, value) {
            const _uri = new URL(uri);
            const seachParams = new URLSearchParams(_uri.search);

            if (value == null) {
                seachParams.delete(key);
            } else if (seachParams.has(key)) {
                seachParams.set(key, value);
            } else {
                seachParams.append(key, value);
            }

            return _uri.href.split('?')[0] + '?' + seachParams.toString();
        };

        service.setLocationQueryParams = function (key, value, replace) {
            var url = service.updateQueryStringParameter($window.location.href, key, value);
            history[replace ? 'replaceState' : 'pushState']({ url: url }, '', url);
        };

        service.getHashFromString = function (string, withHash) {
            var hash = string.split('#')[1];
            return withHash ? '#' + hash : hash;
        };
    };

    urlHelperService.$inject = ['$window', 'urlHelperConfig'];

    angular.module('urlHelper', [])
        .service('urlHelper', urlHelperService)
        .constant('urlHelperConfig', {
            isAdmin: false,
            adminPath: /adminv2\/|adminv3\//g
        });

})(window.angular);