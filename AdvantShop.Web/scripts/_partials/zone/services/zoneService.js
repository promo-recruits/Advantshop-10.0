﻿; (function (ng) {
    'use strict';

    var zoneService = function ($http, $cacheFactory, $q, $sce, $timeout, modalService) {
        var service = this,
            isRenderDialog = false,
            updateList = [],
            queryList = [],
            callbacks = {},
            cache = $cacheFactory('zonesCache');

        service.getDataForPopup = function () {
            return $http.get('location/getdataforpopup').then(function (response) {
                return response.data;
            });
        };

        service.getZones = function (countryId) {

            return service.getZonesFromCache(countryId).then(function (response) {
                if (response == null) {
                    return service.getZonesFromDB(countryId);
                } else {
                    return response;
                }
            });
        };

        service.getZonesFromCache = function (countryId) {

            var zones = cache.get('zones'),
                zone = angular.isDefined(zones) ? zones[countryId] : null;

            return $q.when(zone);
        };

        service.getZonesFromDB = function (countryId) {
            return $http.get('/location/getcities', { params: { countryId: countryId || 0 } }).then(function (response) {

                var zones = cache.get('zones') || {};
                zones[countryId || 0] = response.data;
                cache.put('zones', zones);

                return response.data;
            });
        };

        service.setCurrentZone = function (city, obj, countryId, region, country, zip, district) {
            var params = {
                city: city,
                countryId: countryId,
                regionName: region,
                countryName: country,
                zip: zip,
                district: district
            };
            if (obj != null)
                params.cityId = obj.CityId;

            return $http.post('/location/setzone', params).then(function (response) {

                var currentFromCache = cache.get('currentZone'),
                    obj = angular.isDefined(currentFromCache) ? angular.extend(currentFromCache, response.data) : response.data;

                if (obj.Phone != null) {
                    obj.Phone = $sce.trustAsHtml(obj.Phone);
                }

                service.processUpdateList(obj);

                service.processCallback('set', obj);

                return cache.put('currentZone', obj);
            });
        };

        service.getCurrentZone = function () {

            var currentFromCache = cache.get('currentZone');

            if (currentFromCache != null) {
                return $q.when(currentFromCache);
            }

            if (queryList.length > 0) {

                var defer = $q.defer();
                queryList.push(defer);

                return defer.promise;
            }

            queryList.push($q.defer());


            return $http.post('/location/getcurrentzone').then(function (response) {


                response.data.Phone = $sce.trustAsHtml(response.data.Phone);

                cache.put('currentZone', response.data.current);

                for (var i = queryList.length - 1; i >= 0; i--) {
                    queryList[i].resolve(cache.get('currentZone'));
                }

                queryList.length = 0;

                return $q.when(cache.get('currentZone'));



            });

        };

        service.approveZone = function () {
            return $http.post('/location/approveZone');
        };

        service.zonePopoverShown = function () {
            return $http.post('/common/resetLastModified');
        };

        service.zoneDialogOpen = function () {

            if (isRenderDialog === false) {
                modalService.renderModal(
                    'zoneDialog',
                    undefined,
                    '<div data-zone-dialog></div>',
                    undefined,
                    {
                        'isOpen': true,
                        'modalClass': 'zone-dialog'
                    }
                );
                isRenderDialog = true;
            } else {
                modalService.open('zoneDialog');
            }
        };

        service.zoneDialogClose = function () {

            if (isRenderDialog === true) {
                modalService.close('zoneDialog');
                modalService.startWorking();
            }
        };

        service.sliceCitiesForDialog = function (cities) {
            var columnsSize = 4,
                citiesLength = cities.length;

            var itemsSize = Math.ceil(citiesLength / columnsSize),
                newArray = [];

            for (var i = 0; i < columnsSize; i++) {
                newArray.push(cities.slice(i * itemsSize, (i + 1) * itemsSize));
            }

            return newArray;
        };

        service.getCitiesForAutocomplete = function (cityName) {
            return $http.get('/location/getcitiesautocomplete', { params: { q: cityName } }).then(function (response) {
                return response.data;
            });
        };

        service.addUpdateList = function (scope) {
            updateList.push(scope);
        };

        service.addCallback = function (eventName, func) {
            callbacks[eventName] = callbacks[eventName] || [];
            callbacks[eventName].push(func);
        };

        service.processCallback = function (eventName, data) {
            if (callbacks[eventName] != null) {
                for (var i = 0, l = callbacks[eventName].length; i < l; i++) {
                    callbacks[eventName][i](data);
                }
            }
        };

        service.processUpdateList = function (data) {

            var dataTrusted = service.trustZone(data),
                zoneCurrentItem;

            for (var i = updateList.length - 1; i >= 0; i--) {

                zoneCurrentItem = updateList[i].zone;

                zoneCurrentItem = angular.isDefined(zoneCurrentItem) ? angular.extend(zoneCurrentItem, dataTrusted) : dataTrusted;
            }
        };

        service.trustZone = function (zone) {
            if (zone.Phone != null && typeof (zone.Phone) === 'string') {
                zone.Phone = $sce.trustAsHtml(zone.Phone);
            }

            return zone;
        };
    };

    angular.module('zone')
      .service('zoneService', zoneService);

    zoneService.$inject = ['$http', '$cacheFactory', '$q', '$sce', '$timeout', 'modalService'];

})(window.angular);