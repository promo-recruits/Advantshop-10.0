; (function (ng) {
    'use strict';

    var WishlistControlCtrl = function (wishlistService) {
        var ctrl = this;

        ctrl.dirty = false;

        ctrl.add = function (offerId, state) {
            return wishlistService.add(offerId, state);
        };

        ctrl.remove = function (offerId, state) {
            return wishlistService.remove(offerId, state);
        };

        ctrl.change = function (offerId, state) {

            ctrl.dirty = true;

            if (ctrl.isAdded) {
                ctrl.add(offerId, state);
            } else {
                ctrl.remove(offerId, state);
            }
        };

        ctrl.checkStatus = function (offerId) {
            wishlistService.getStatus(offerId).then(function (isAdded) {
                ctrl.isAdded = isAdded;
            });
        };
    };


    angular.module('wishlist')
      .controller('WishlistControlCtrl', WishlistControlCtrl);

    WishlistControlCtrl.$inject = ['wishlistService'];

})(window.angular);