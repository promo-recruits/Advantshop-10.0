﻿/* @ngInject */
function ShippingListCtrl($anchorScroll, $location, shippingService) {
        var ctrl = this;
        var watchersFn = [];

        //ctrl.isProgress = null;

        $anchorScroll.yOffset = 50;

        //ctrl.selectedItemIndex = 0;
        //ctrl.collapsed = true;

        ctrl.visibleItems = Number.POSITIVE_INFINITY;

        ctrl.changeShipping = function (shipping, index) {

            if (index != null) {
                ctrl.selectedItemIndex = index;
            }

            ctrl.change({
                shipping: shipping,
                newShipping: ctrl.newShipping
            });
        };

        ctrl.changeShippingControl = function (shipping) {

            for (var i = ctrl.items.length - 1; i >= 0; i--) {
                if (ctrl.items[i] === shipping) {
                    ctrl.selectShipping = shipping;
                    ctrl.selectedItemIndex = i;
                    break;
                }
            }

            ctrl.change({
                shipping: shipping,
                customShipping: ctrl.customShipping
            });
        };

        ctrl.focusEditPrice = function (shipping, index) {
            ctrl.selectShipping = shipping;
            ctrl.selectedItemIndex = index;
            
            ctrl.focus({
                shipping: shipping,
                customShipping: ctrl.customShipping
            });
        };

        ctrl.calc = function (index) {
            var selectItemPos = index + 1;

            ctrl.selectedItemIndex = index;

            ctrl.visibleItems = selectItemPos > ctrl.countVisibleItems ? selectItemPos : ctrl.countVisibleItems;

            return selectItemPos;
        };

        ctrl.toggleVisible = function () {

            var selectItemPos = ctrl.calc(ctrl.selectedItemIndex);

            if (ctrl.collapsed === true) {
                ctrl.visibleItems = ctrl.items.length;
                ctrl.collapsed = false;
            } else {

                if (selectItemPos === ctrl.items.length) {
                    return;
                }

                ctrl.visibleItems = selectItemPos > ctrl.countVisibleItems ? selectItemPos : ctrl.countVisibleItems;
                ctrl.collapsed = true;

                $location.hash(ctrl.anchor);
                $anchorScroll();
            }
        };

        ctrl.setSelectedIndex = function (index) {

            var selectItemPos = ctrl.calc(index);

            if (selectItemPos === ctrl.items.length) {
                ctrl.collapsed = false;
            } else {
                ctrl.collapsed = true;
            }
        };

        ctrl.addCallbackOnLoad = function (fn) {
            watchersFn.push(fn);
        };

        ctrl.processCallbacks = function () {
            var params = arguments;
            watchersFn.forEach(function (fn) {
                fn(params);
            });
        };
        
        ctrl.isTemplateReady = function (item){
            return ctrl.isProgress !== true && item.Template && shippingService.isTemplateReady(item.Template) !== true;
        }
    };

export default ShippingListCtrl;