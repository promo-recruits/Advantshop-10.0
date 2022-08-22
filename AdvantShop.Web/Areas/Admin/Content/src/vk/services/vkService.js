; (function (ng) {
    'use strict';

    var vkService = function ($http) {
        var service = this;
        
        service.getVkSettings = function () {
            return $http.get('vk/getVkSettings').then(function (response) {
                return response.data;
            });
        }

        service.saveAuthVkUser = function (params) {
            return $http.post('vk/saveAuthVkUser', params).then(function (response) {
                return params;
            });
        }

        service.getGroups = function () {
            return $http.post('vk/getVkGroups').then(function (response) {
                return response.data;
            });
        }

        service.saveAuthVkGroup = function (params) {
            return $http.post('vk/saveAuthVkGroup', params).then(function (response) {
                return response.data;
            });
        }

        service.deleteGroup = function (params) {
            return $http.post('vk/deleteGroup', params).then(function (response) {
                return response.data;
            });
        }


        service.authVkByLoginPassword = function (login, password) {
            return $http.post('vk/authByLoginAndPassword', {login: login, password: password}).then(function (response) {
                return response.data;
            });
        }

        service.deleteVkByLoginPassword = function () {
            return $http.post('vk/deleteVkByLoginPassword').then(function (response) {
                return response.data;
            });
        }

        service.saveSettings = function (id, createLeadFromMessages, createLeadFromComments, syncOrdersFromVk) {
            return $http.post('vk/saveSettings',{
                        id: id,
                        createLeadFromMessages: createLeadFromMessages,
                        createLeadFromComments: createLeadFromComments,
                        syncOrdersFromVk: syncOrdersFromVk
                    }).then(function(response) {
                    return response.data;
                });
        }

        service.removeChannel = function() {
            return $http.post('salesChannels/delete', {type: 'vk'}).then(function (response) {
                return response.data;
            });
        }
    };

    vkService.$inject = ['$http'];

    ng.module('vkAuth')
        .service('vkService', vkService);

})(window.angular);