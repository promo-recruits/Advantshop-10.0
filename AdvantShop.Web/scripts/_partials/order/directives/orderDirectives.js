
function orderHistoryDirective() {
    return {
        restrict: 'A',
        scope: {
            mode: '=',
            onChangeView: '&',
            titleText: '@',
            isLp: '<?'
        },
        bindToController: true,
        controller: 'OrderHistoryCtrl',
        controllerAs: 'orderHistory',
        replace: true,
        template: function (elem, attr) {
            if (attr.isMobile) {
                return '<div data-ng-switch="orderHistory.mode"><div data-order-history-items data-ng-switch-when="all"></div><div data-order-history-details data-is-mobile="true" data-is-lp="orderHistory.isLp" data-ng-switch-when="details"></div></div>';
            } else {
                return '<div><div data-ng-if="orderHistory.isGetOrdersData"><div data-ng-switch="orderHistory.mode"><div data-order-history-items data-ng-switch-when="all"></div><div data-order-history-details data-is-lp="orderHistory.isLp" data-ng-switch-when="details"></div></div></div><div class="order-history__spinner svg-spinner" data-ng-if="!orderHistory.isGetOrdersData"></div></div>';
            }
        },
    };
};

function orderHistoryItemsDirective() {
    return {
        require: '^orderHistory',
        restrict: 'A',
        scope: {
            isLp: '<?'
        },
        replace: true,
        templateUrl: '/scripts/_partials/order/templates/items.html',
        link: function (scope, element, attrs, ctrl) {
            scope.parentScope = ctrl;
        }
    };
};

function orderHistoryDetailsDirective() {
    return {
        require: '^orderHistory',
        restrict: 'A',
        scope: {
            isLp: '<?'
        },
        replace: true,
        templateUrl: function (elem, attr) {
            if (attr.isMobile) {
                return '/scripts/_partials/order/templates/mobileDetails.html';
            } else {
                return '/scripts/_partials/order/templates/details.html';
            }
        },
        link: function (scope, element, attrs, ctrl) {
            scope.parentScope = ctrl;
        }
    };
};

function orderPayDirective() {
    return {
        restrict: 'AE',
        scope: {
            orderCode: '<?',
            // orderId: '<?',
            paymentMethodId: '<?',
            pageWithPaymentButton: '@'
        },
        bindToController: true,
        controller: 'OrderPayCtrl',
        controllerAs: 'orderPay',
        template: function (elem, attr) {
            return '<div data-ng-include="\'/checkout/getorderpay?ordercode=\'+orderPay.orderCode+\'&orderid=\'+orderPay.orderId+\'&paymentmethodid=\'+orderPay.paymentMethodId+\'&pagewithpaymentbutton=\'+orderPay.pageWithPaymentButton+\'&rnd=\'+orderPay.rnd"></div>';
        },
    };
};

export {
    orderHistoryDirective,
    orderHistoryItemsDirective,
    orderHistoryDetailsDirective,
    orderPayDirective
}

