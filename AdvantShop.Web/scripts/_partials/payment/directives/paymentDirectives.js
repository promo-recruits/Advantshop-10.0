/* @ngInject */
function paymentListDirective(urlHelper) {
    return {
        restrict: 'A',
        scope: {
            items: '=',
            selectPayment: '=',
            countVisibleItems: '=',
            change: '&',
            anchor: '@',
            isProgress: '=?',
            iconWidth: '@',
            iconHeight: '@',
            enablePhoneMask: '<?'
        },
        controller: 'PaymentListCtrl',
        controllerAs: 'paymentList',
        bindToController: true,
        replace: true,
        templateUrl: function () {
            return urlHelper.getAbsUrl('/scripts/_partials/payment/templates/paymentList.html', true);
        }
    };
};

/* @ngInject */
function paymentTemplateDirective() {
    return {
        restrict: 'A',
        scope: {
            templateUrl: '=',
            payment: '=',
            changeControl: '&',
            enablePhoneMask: '<?'
        },
        controller: 'PaymentTemplateCtrl',
        controllerAs: 'paymentTemplate',
        bindToController: true,
        replace: true,
        template: '<div data-ng-include="paymentTemplate.templateUrl"></div>'
    };
};

export {
    paymentListDirective,
    paymentTemplateDirective
}