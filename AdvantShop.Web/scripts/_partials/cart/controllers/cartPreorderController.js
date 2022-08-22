; (function (ng) {
    'use strict';

    var CartPreorderCtrl = function ($window) {
        var ctrl = this;

        ctrl.getPreOrderUrl = function () {
            var params = [];
            if (ctrl.amount != null && ctrl.amount != '') {
                params.push("amount=" + ctrl.amount);
            }
            if (ctrl.attributesHash != null && ctrl.attributesHash != '') {
                params.push("optionsHash=" + ctrl.attributesHash);
            }
            return "preorder" + (ctrl.lp || '') + '/'  + ctrl.offerId + (params.length ? "?" + params.join('&') : '');
        };
    };

    CartPreorderCtrl.$inject = ['$window'];

    angular.module('cart')
      .controller('CartPreorderCtrl', CartPreorderCtrl);


})(window.angular);