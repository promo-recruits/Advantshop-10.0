; (function (ng) {
    'use strict';

    var CartFullCtrl = function ($rootScope, cartService, moduleService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            cartService.getData().then(function (data) {
                ctrl.cartData = data;
            });
        };

        ctrl.updateAmount = function (value, itemId) {

            var item = {
                Key: itemId,
                Value: value
            };

            cartService.updateAmount([item]).then(function () {
                moduleService.update('fullcartmessage');
            });      
        };

        ctrl.remove = function (shoppingCartItemId) {
            cartService.removeItem(shoppingCartItemId).then(function (result) {
                moduleService.update('fullcartmessage');
                $(document).trigger("cart.remove", result.offerId);
            });
        };

        ctrl.clear = function () {
            cartService.clear().then(function () {
                moduleService.update('fullcartmessage');
                $(document).trigger("cart.clear");
            });
        };

        ctrl.refresh = function () {
            return cartService.getData(false).then(function (data) {
                moduleService.update('fullcartmessage');
                ctrl.cartData = data;
            });
        };
    };

    angular.module('cart')
      .controller('CartFullCtrl', CartFullCtrl);

    CartFullCtrl.$inject = ['$rootScope','cartService', 'moduleService'];

})(window.angular);