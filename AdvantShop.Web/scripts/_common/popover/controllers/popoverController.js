/* @ngInject */
function PopoverCtrl($q, $element, $timeout, popoverService, popoverConfig) {
    var ctrl = this;

    ctrl.$onInit = function () {
        var popoverShowOnLoad = ctrl.popoverShowOnLoad(),
            popoverOverlayEnabled = ctrl.popoverOverlayEnabled(),
            popoverIsFixed = ctrl.popoverIsFixed(),
            popoverIsCanHover = ctrl.popoverIsCanHover();

        ctrl.popoverShowOnLoad = popoverShowOnLoad != null ? popoverShowOnLoad : popoverConfig.popoverShowOnLoad;
        ctrl.popoverOverlayEnabled = popoverOverlayEnabled != null ? popoverOverlayEnabled : popoverConfig.popoverOverlayEnabled;
        ctrl.popoverIsFixed = popoverShowOnLoad != null ? popoverIsFixed : popoverConfig.popoverIsFixed;
        ctrl.popoverIsCanHover = popoverIsCanHover != null ? popoverIsCanHover : popoverConfig.popoverIsCanHover;

        popoverService.addStorage(ctrl.id, ctrl);
    };

    ctrl.updatePosition = function (targetElement) {
        ctrl.position = popoverService.getPosition($element[0], targetElement || ctrl.controlElement[0], ctrl.popoverPosition, ctrl.popoverIsFixed);
    };

    ctrl.active = function (targetElement) {
        ctrl.popoverIsShow = true;

        return $timeout(function () {
            ctrl.updatePosition(targetElement);
            ctrl.popoverPosition = ctrl.position.position;

            if (ctrl.popoverOverlayEnabled === true) {
                popoverService.showOverlay(ctrl.id);
            }
        }, 0);
    };

    ctrl.deactive = function () {
        ctrl.popoverIsShow = false;

        if (ctrl.popoverOverlayEnabled === true) {
            popoverService.getPopoverOverlay().then(function (overlayScope) {
                overlayScope.overlayHide();
            });
        }
    };

    ctrl.toggle = function () {

        if (ctrl.popoverIsShow === true) {
            ctrl.deactive();
        } else {
            ctrl.active();
        }
    };

    ctrl.getClasses = function () {

        var result = [];

        result.push('adv-popover-position-' + ctrl.popoverPosition);

        if (ctrl.popoverIsFixed === true) {
            result.push('adv-popover-fixed');
        }

        return result;
    };
};

export default PopoverCtrl;