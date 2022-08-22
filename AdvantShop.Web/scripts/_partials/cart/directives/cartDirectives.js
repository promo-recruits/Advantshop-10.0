; (function (ng) {
    'use strict';

    angular.module('cart')
        .directive('cartMini', ['cartService', function (cartService) {
            return {
                restrict: 'A',
                scope: true,
                controller: 'CartMiniCtrl',
                controllerAs: 'cartMini',
                bindToController: true
            };
        }]);

    angular.module('cart')
        .directive('cartMiniTrigger', function () {
            return {
                require: '^cartMini',
                restrict: 'A',
                scope: {},
                link: function (scope, element, attrs, ctrl) {
                    element.on('click', function (event) {
                        ctrl.triggerClick(event);
                        scope.$apply();
                    });
                }
            };
        });

    angular.module('cart')
        .directive('cartMiniList', ['$window', function ($window) {
            return {
                require: ['cartMiniList', '^cartMini'],
                restrict: 'EA',
                scope: {
                    cartData: '=',
                    isMobile: '<?',
                    isShowRemove: '<?'
                },
                replace: true,
                controller: 'CartMiniListCtrl',
                controllerAs: 'cartMiniList',
                bindToController: true,
                templateUrl: '/scripts/_partials/cart/templates/cart-mini.html',
                link: function (scope, element, attrs, ctrls) {
                    var cartMiniList = ctrls[0],
                        cartMini = ctrls[1];

                    cartMiniList.initialized = true;

                    cartMini.addMinicartList(cartMiniList);

                    if (cartMiniList.isMobile !== true) {
                        element[0].addEventListener('mouseenter', function () {
                            cartMiniList.clearTimerClose();
                            scope.$digest();
                        });

                        element[0].addEventListener('mouseleave', function () {
                            cartMiniList.startTimerClose();
                            scope.$digest();
                        });
                    }

                }
            };
        }]);


    angular.module('cart')
        .directive('cartFull', function () {
            return {
                restrict: 'EA',
                scope: {
                    photoWidth: '@'
                },
                controller: 'CartFullCtrl',
                controllerAs: 'cartFull',
                bindToController: true,
                replace: true,
                templateUrl: '/scripts/_partials/cart/templates/cart-full.html'
            };
        });

    angular.module('cart')
        .directive('cartMobileFull', function () {
            return {
                restrict: 'EA',
                scope: {},
                controller: 'CartMobileFullCtrl',
                controllerAs: 'cartMFull',
                bindToController: true,
                replace: true,
                templateUrl: '/scripts/_partials/cart/templates/cart-mobile-full.html'
            };
        });

    angular.module('cart')
        .directive('cartAdd', function () {
            return {
                restrict: 'EA',
                scope: {
                    offerId: '=',
                    productId: '=',
                    amount: '=',
                    attributesXml: '=',
                    payment: '=',
                    href: '@',
                    cartAddValid: '&',
                    mode: '@',
                    lpId: '@',
                    lpUpId: '@',
                    lpEntityId: '@',
                    lpEntityType: '@',
                    lpBlockId: '@',
                    lpButtonName: '@',
                    hideShipping: '@',
                    offerIds: '=',
                    source: '@',
                    modeFrom: '@'
                },
                controller: 'CartAddCtrl',
                controllerAs: 'cartAdd',
                bindToController: true,
                link: function (scope, element, attrs, ctrl) {
                    element[0].addEventListener('click', function (event) {
                        ctrl.addItem(event, ctrl.offerId, ctrl.productId, ctrl.amount, ctrl.attributesXml, ctrl.payment, ctrl.href,
                            ctrl.mode, ctrl.lpId, ctrl.lpUpId, ctrl.lpEntityId, ctrl.lpEntityType, ctrl.lpBlockId, ctrl.hideShipping,
                            ctrl.offerIds, ctrl.modeFrom, ctrl.lpButtonName);
                    });
                }
            };
        });

    angular.module('cart')
        .directive('cartPreorder', function () {
            return {
                replace: true,
                transclude: true,
                restrict: 'EA',
                scope: {
                    offerId: '=',
                    amount: '=',
                    attributesHash: '=',
                    cartPreorderValid: '&',
                    lp: '@'
                },
                template: '<a data-ng-click="cartPreorder.cartPreorderValid() === false ? $event.preventDefault(): null" data-ng-href="{{cartPreorder.getPreOrderUrl()}}" data-ng-transclude></a>',
                controller: 'CartPreorderCtrl',
                controllerAs: 'cartPreorder',
                bindToController: true
            };
        });

    angular.module('cart')
        .directive('cartCount', ['$sce', function ($sce) {
            return {
                restrict: 'A',
                scope: true,
                controller: 'CartCountCtrl',
                controllerAs: 'cartCount',
                bindToController: true,
                link: function (scope, element, attrs, ctrl, transclude) {
                    ctrl.type = attrs.type;
                    var startValue = element.html();
                    ctrl.startValue = $sce.trustAsHtml(startValue);
                }
            };
        }]);

    angular.module('cart')
        .directive('cartConfirm', function () {
            return {
                restrict: 'A',
                scope: true,
                controller: 'CartConfirmCtrl',
                controllerAs: 'cartConfirm',
                bindToController: true
            };
        });
})(angular);