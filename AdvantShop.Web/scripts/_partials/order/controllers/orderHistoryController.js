/* @ngInject */
function OrderHistoryCtrl($sce, $window, orderService, windowService, toaster, $translate, SweetAlert, $location) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.setViewFromUrl();

        ctrl.isGetOrdersData = false;
        orderService.getOrders().then(function (orders) {
            ctrl.items = orders;
            ctrl.mode = 'all';
        }).finally(() => {
            ctrl.isGetOrdersData = true;
        });

    };

    ctrl.changeModeAll = function () {
        ctrl.mode = 'all';
    };

    ctrl.changeModeDetails = function () {
        ctrl.mode = 'details';
    };

    ctrl.prepareDetails = function (order) {

        var paymentSelected;

        ctrl.orderDetails = order;

        if (ctrl.orderDetails.Payments != null && ctrl.orderDetails.Payments.length > 0) {

            for (var i = 0, l = ctrl.orderDetails.Payments.length; i < l; i++) {
                if (ctrl.orderDetails.Payments[i].Id == ctrl.orderDetails.PaymentMethodId) {
                    paymentSelected = ctrl.orderDetails.Payments[i];
                    break;
                }
            }

            if (paymentSelected == null) {
                paymentSelected = ctrl.orderDetails.Payments[0];
            }

            ctrl.orderDetails.paymentSelected = paymentSelected;
            ctrl.orderDetails.PaymentForm = $sce.trustAsHtml(ctrl.orderDetails.PaymentForm);
        }

        return ctrl.orderDetails;
    };

    ctrl.view = function (ordernumber) {

        return orderService.getOrderDetails(ordernumber).then(function (order) {
            ctrl.prepareDetails(order);
            ctrl.changeModeDetails();
            if (ctrl.onChangeView != null) {
                ctrl.onChangeView({ orderHistoryCtrl: ctrl, ordernumber: ordernumber });
            }
            setTimeout(function () {
                $window.scrollTo(0, 0);
            });
        });
    };


    ctrl.cancelOrder = function (ordernumber) {

        return SweetAlert.confirm($translate.instant('Js.Order.AreYouWantCancelOrder'), { title: $translate.instant('Js.Order.OrderCancel') }).then(function (result) {
            if (result.value === true) {
                return orderService.cancelOrder(ordernumber)
                    .then(orderService.getOrderDetails.bind(orderService, ordernumber))
                    .then(function (order) {
                        return ctrl.orderDetails = order;
                    });
            }
        });
    };

    ctrl.print = function (ordernumber) {
        windowService.print('PrintOrder/' + ordernumber, 'printOrder', 'menubar=no,location=no,resizable=yes,scrollbars=yes');
    };

    ctrl.changePaymentMethod = function (ordernumber, paymentId) {
        return orderService.changePaymentMethod(ordernumber, paymentId).then(function (response) {
            if (response != null) {
                return orderService.getOrderDetails(ordernumber).then(ctrl.prepareDetails);
            }
        });
    };

    ctrl.changeOrderComment = function (ordernumber, customercomment) {
        return orderService.changeOrderComment(ordernumber, customercomment).then(function (response) {
            if (response === true) {
                toaster.pop('success', '', $translate.instant('Js.Order.CommentSaved'));
            }
            else {
                toaster.pop('error', '', $translate.instant('Js.Order.CommentNotSaved'));
            }
        });
    };

    ctrl.setViewFromUrl = function () {
        if ($location.search().mode != null && $location.search().ordernumber != null) {
            ctrl.view($location.search().ordernumber);
            ctrl.mode = $location.search().mode;
        }
    };

};
export default OrderHistoryCtrl;