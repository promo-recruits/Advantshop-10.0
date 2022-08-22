; (function (ng) {
    'use strict';

    var PhotoViewListCtrl = function () {
        var ctrl = this;
        ctrl.updateActiveElements = function () {
            ctrl.activeNavIndex = 0;
            ctrl.activeItemIndex = 0;
        };

    }

    ng.module(`photoViewList`)
        .controller(`PhotoViewListCtrl`, PhotoViewListCtrl)

})(window.angular);