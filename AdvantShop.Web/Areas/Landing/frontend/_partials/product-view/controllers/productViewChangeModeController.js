﻿; (function (ng) {
    'use strict';

    var ProductViewChangeModeCtrl = function (productViewService, viewList) {
        var ctrl = this;

        ctrl.setView = function (name, view, isMobile) {
            ctrl.current = view;
            productViewService.setView(name, view, ctrl.currentViewList, isMobile);
        };


        ctrl.toggle = function (name) {
            var index = ctrl.currentViewList.indexOf(ctrl.current);
            var nextViewIndex = index !== -1 ?  index + 1 : 0;
            ctrl.setView(name, ctrl.currentViewList[nextViewIndex < ctrl.currentViewList.length ? nextViewIndex : 0], ctrl.currentViewList, ctrl.isMobile);
        };
    };

    ng.module('productView')
      .controller('ProductViewChangeModeCtrl', ProductViewChangeModeCtrl);

    ProductViewChangeModeCtrl.$inject = ['productViewService', 'viewList'];

})(window.angular);