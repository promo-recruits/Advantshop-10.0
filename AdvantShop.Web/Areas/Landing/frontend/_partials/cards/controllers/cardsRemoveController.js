; (function (ng) {
    'use strict';

    var CardsRemoveCtrl = function (cardsService, $translate) {
        var ctrl = this;

        ctrl.remove = function (type) {

            var request;

            switch (type) {
                case 'coupon':
                    request = cardsService.deleteCoupon();
                    break;
                case 'certificate':
                    request = cardsService.deleteCertificate();
                    break;
                default:
                    throw Error($translate.instant('Js.Cards.NotFoundTypeToRemove'));
            }

            request.then(function () {
                ctrl.applyFn();
            });
        };
    };

    ng.module('cards')
      .controller('CardsRemoveCtrl', CardsRemoveCtrl);

    CardsRemoveCtrl.$inject = ['cardsService', '$translate'];

})(window.angular);