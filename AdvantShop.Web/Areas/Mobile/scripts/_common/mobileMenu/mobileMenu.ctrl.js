/*@ngInject*/
function MobileMenuCtrl($attrs, $element, $timeout) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.list = [];

        if (ctrl.parentMobileMenuItem != null) {
            ctrl.parentMobileMenuItem.addMenuChild(ctrl);
        }

        ctrl.$element = $element;

        ctrl.isOpen = $attrs.isOpen === 'true';
    };

    ctrl.$postLink = function () {

    };

    ctrl.addMenuItem = function (mobileMenuItem, isSelected) {
        ctrl.list.push(mobileMenuItem);

        if (isSelected === true) {
            ctrl.menuRoot.addMobileMenuSelected(ctrl);
        }
    };

    ctrl.back = function () {
        ctrl.toggleSiblings(true);

        return ctrl.menuRoot.move(false)
            .then(function () {
                ctrl.isOpen = false;
                return ctrl;
            });
    };

    ctrl.open = function () {

        ctrl.isOpen = true;

        //таймер, так как переменная в шаблоне не успела примениться
        return $timeout(function () {
            return ctrl.menuRoot.move(true)
                .then(function () {
                    ctrl.toggleSiblings(false);

                    return ctrl;
                });
        });

    };

    ctrl.checkOpen = function () {
        return ctrl.isOpen === true;
    };

    ctrl.toggleSiblings = function (isVisible) {
        if (ctrl.parentMobileMenuItem != null && ctrl.parentMobileMenuItem.parentMobileMenu != null) {
            ctrl.parentMobileMenuItem.parentMobileMenu.list.forEach(function (item) {
                if (item !== ctrl.parentMobileMenuItem) {
                    item.isHidden = !isVisible;
                }
            });
        }
    };

    ctrl.showOnLoad = function () {
        var parentMobileMenuItem,
            levelCount = 0;

        ctrl.isOpen = true;

        parentMobileMenuItem = ctrl.parentMobileMenuItem;

        ctrl.toggleSiblings(false);

        while (parentMobileMenuItem != null && parentMobileMenuItem.parentMobileMenu != null) {

            if (parentMobileMenuItem != null && parentMobileMenuItem.parentMobileMenu != null) {
                parentMobileMenuItem.parentMobileMenu.toggleSiblings(false);
            }

            parentMobileMenuItem.parentMobileMenu.isOpen = true;
            parentMobileMenuItem = parentMobileMenuItem.parentMobileMenuItem;
            levelCount += 1;
        }

        ctrl.menuRoot.move(true, levelCount);

    };
};

export default MobileMenuCtrl;