; (function (ng) {
    'use strict';

    var cartToolbar,
        timerPopoverHide;

    var CartAddCtrl = function ($document, $q, $timeout, $window, cartConfig, cartService, moduleService, popoverService, SweetAlert, $translate, domService) {

        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.source != null && ctrl.source === 'mobile') {
                cartService.setStateInfo(true);
            }
        };

        ctrl.addItem = function (event, offerId, productId, amount, attributesXml, payment, href, mode, lpId, lpUpId, lpEntityId, lpEntityType, lpBlockId, hideShipping, offerIds, modeFrom, lpButtonName) {

            event.preventDefault();

            var isValid = ctrl.cartAddValid(),
                deferNoop = $q.defer();

            if (isValid === true || isValid == null) {

                if (ctrl.isLoading == true) {
                    deferNoop.resolve(null);
                    return deferNoop.promise;
                }

                ctrl.isLoading = true;

                return cartService.addItem(offerId, productId, amount, attributesXml, payment, mode, lpId, lpEntityId, lpEntityType, lpBlockId, { lpUpId: lpUpId, hideShipping: hideShipping, offerIds: offerIds, modeFrom: modeFrom, lpButtonName: lpButtonName })
                    .then(function (result) {
                        if (result[0].status == 'redirect') {
                            if (result[0].url != null && result[0].url.length > 0) {
                                $window.location.assign(result[0].url);
                            }
                            else {
                                $window.location.assign(href);
                            }
                        } else {

                            $(document).trigger("add_to_cart", href);
                            $(document).trigger("cart.add", [offerId, productId, amount, attributesXml, result['0'].cartId, event.target]);
                            $(document).trigger("cart.addv2", [productId, result['0'].cartId, result['0'].CartItem, event.target]);

                            moduleService.update(['minicartmessage', 'fullcartmessage']).then(ctrl.popoverModule);
                        }
                        return result;
                    })
                    .then(function (result) {
                        if (ctrl.source != null && ctrl.source === 'mobile' && result[0].status !== 'redirect') {
                            cartService.showInfoWithDebounce();
                        }

                        return result;
                    })
                    .finally(function () {
                        ctrl.isLoading = false;
                    });
            } else {
                deferNoop.resolve(null);
                return deferNoop.promise;
            }
        };

        ctrl.popoverModule = function (content) {
            if (moduleService.getModule('minicartmessage') != null && content[0].trim().length > 0) {

                $timeout(function () {
                    popoverService.getPopoverScope('popoverCartToolbar').then(function (popoverScope) {

                        cartToolbar = cartToolbar || document.getElementById('cartToolbar');

                        popoverScope.active(cartToolbar);

                        popoverScope.updatePosition(cartToolbar);

                        if (timerPopoverHide != null) {
                            $timeout.cancel(timerPopoverHide);
                        }

                        timerPopoverHide = $timeout(function () {
                            popoverScope.deactive();
                        }, 5000);
                    });
                }, 0);
            }
        };
    };

    angular.module('cart')
        .controller('CartAddCtrl', CartAddCtrl);

    CartAddCtrl.$inject = ['$document', '$q', '$timeout', '$window', 'cartConfig', 'cartService', 'moduleService', 'popoverService', 'SweetAlert', '$translate', 'domService'];

})(window.angular);