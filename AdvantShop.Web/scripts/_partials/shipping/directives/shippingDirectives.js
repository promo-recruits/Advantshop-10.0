/* @ngInject */
function shippingListDirective(urlHelper, $timeout) {
    return {
        restrict: 'A',
        scope: {
            items: '=',
            selectShipping: '=',
            countVisibleItems: '=',
            change: '&',
            focus: '&',
            anchor: '@',
            isProgress: '=?',
            contact: '<?',
            isCanAddCustom: '<?',
            customShipping: '<?',
            iconWidth: '@',
            iconHeight: '@',
            editPrice: '<?',
            isAdmin: '<?'
        },
        controller: 'ShippingListCtrl',
        controllerAs: 'shippingList',
        bindToController: true,
        replace: true,
        templateUrl: function () {
            return urlHelper.getAbsUrl('/scripts/_partials/shipping/templates/shippingList.html', true);
        },
        link: function (scope, element, attrs, ctrl) {
            scope.$watch('shippingList.items', function (newValue, oldValue) {
                $timeout(function () {
                    if (newValue !== oldValue) {
                        ctrl.processCallbacks();
                    }
                }, 50);
            })
        }
    };
};

function shippingTemplateDirective() {
    return {
        restrict: 'A',
        scope: {
            templateUrl: '=',
            shipping: '=',
            isSelected: '=',
            changeControl: '&',
            contact: '<?',
            isAdmin: '<?'
        },
        controller: 'ShippingTemplateCtrl',
        controllerAs: 'shippingTemplate',
        bindToController: true,
        replace: true,
        template: '<div data-ng-include="shippingTemplate.templateUrl"></div>'
    };
};

function shippingVariantsDirective() {
    return {
        restrict: 'A',
        scope: {
            type: '@',
            offerId: '=',
            amount: '=',
            svCustomOptions: '=',
            startOfferId: '@',
            startAmount: '@',
            startSvCustomOptions: '@',
            zip: '@',
            initFn: '&'
        },
        controller: 'ShippingVariantsCtrl',
        controllerAs: 'shippingVariants',
        bindToController: true,
        replace: true,
        templateUrl: '/scripts/_partials/shipping/templates/shippingVariants.html'
    };
};

export {
    shippingListDirective,
    shippingTemplateDirective,
    shippingVariantsDirective
}
