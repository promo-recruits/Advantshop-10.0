; (function (ng) {
    'use strict';

    var ProductViewModeCtrl = function (productViewService) {
    };

    ng.module('productView')
      .controller('ProductViewModeCtrl', ProductViewModeCtrl);

    ProductViewModeCtrl.$inject = ['productViewService'];

})(window.angular);