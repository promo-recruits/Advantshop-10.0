; (function (ng) {
    'use strict';

    var SalesChannelsCtrl = function ($window, SweetAlert, $http, toaster) {
        var ctrl = this;

        ctrl.removeChannel = function (type) {
            SweetAlert.confirm('Вы уверены что хотите удалить канал?', { title: 'Удаление' })
                .then(function () {
                    $http.post('salesChannels/delete', { type: type }).then(function (response) {
                        toaster.pop('success', '', 'Канал продаж удален');

                        var basePath = document.getElementsByTagName('base')[0].getAttribute('href');
                        $window.location.assign(basePath);
                    });
                });
        }

        ctrl.addSalesChannel = function (type) {
            $http.post('salesChannels/add', { type: type }).then(function (response) {
                $window.location.reload();
            });
        }
    };

    SalesChannelsCtrl.$inject = ['$window', 'SweetAlert', '$http', 'toaster'];


    ng.module('salesChannels', [])
      .controller('SalesChannelsCtrl', SalesChannelsCtrl);

})(window.angular);