/*@ngInject*/
function MobileMenuRootCtrl($element, $q, $scope, domService) {
    var ctrl = this;
    var level = 0;
    var el = $element[0];
    var mobileMenuSelected;
    var mobileMenuItemDictionary = {};

    ctrl.$onInit = function () {
        ctrl.styles = {};
    };

    ctrl.$postLink = function () {
        if (mobileMenuSelected != null) {
            mobileMenuSelected.showOnLoad();
        }

        $element[0].addEventListener('click', function (event) {
            var trigger = domService.closest(event.target, '[data-mobile-menu-item-trigger]', '[data-mobile-menu]');
            if (trigger != null && mobileMenuItemDictionary[trigger.dataset.mobileMenuItemTrigger]) {
                mobileMenuItemDictionary[trigger.dataset.mobileMenuItemTrigger](event);

                $scope.$digest();
            }
        });
    };

    ctrl.move = function (isNext, customLevel) {
        var defer = $q.defer();

        el.parentNode.scrollTo(0, 0);

        var fn = function () {
            el.removeEventListener('transitionend', fn);
            defer.resolve();
        };

        el.addEventListener('transitionend', fn);

        if (customLevel != null) {
            level = customLevel;
        } else {
            level += isNext === true ? 1 : -1;
        }

        var value = -100 * level + '%';
        ctrl.styles.transform = 'translateX(' + value + ')';

        return defer.promise;
    };

    ctrl.addMobileMenuSelected = function (mobileMenu) {
        mobileMenuSelected = mobileMenu;
    };

    ctrl.addMobileMenuItem = function (id, mobileMenuItemClickFn) {
        mobileMenuItemDictionary[id] = mobileMenuItemClickFn;
    }
};

export default MobileMenuRootCtrl;