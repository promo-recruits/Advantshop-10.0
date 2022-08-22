/*@ngInject*/
function MobileMenuItemCtrl($attrs, $compile, $q, $http, $scope) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.id = $attrs.mobileMenuItem;
        ctrl.isSelected = false;
        ctrl.hasChild = $attrs.hasChild != null && $attrs.hasChild === 'true';
        ctrl.isLoadedChilds = false;
        ctrl.isProgressLoadChilds = false;

        ctrl.parentMobileMenu.addMenuItem(ctrl, ctrl.isSelected);

        ctrl.mobileMenuRoot.addMobileMenuItem(ctrl.id, ctrl.click);
    };

    ctrl.addMenuChild = function (menuChild) {
        ctrl.menuChild = menuChild;
    };

    ctrl.click = function (event) {

        if (ctrl.isProgressLoadChilds) {
            return;
        }

        event.stopPropagation();

        if (ctrl.menuChild != null) {
            if (ctrl.menuChild.checkOpen() === true) {
                ctrl.menuChild.back();
            } else {
                ctrl.menuChild.open()
                    .then(ctrl.isLoadedChilds === false && ctrl.menuChild.list != null && ctrl.menuChild.list.length > 0 ? ctrl.loadChilds : $q.resolve);
            }
        }
    };

    ctrl.loadChilds = function () {

        ctrl.isProgressLoadChilds = true;

        return $http.get('./mobile/catalog/catalogmenu', { params: { categoryId: ctrl.id } })
            .then(function (response) {
                var menuSubmenu = angular.element(response.data);

                menuSubmenu.attr('is-open', 'true');

                ctrl.menuChild.$element.replaceWith(menuSubmenu);

                $compile(menuSubmenu)($scope);

                return menuSubmenu;
            })
            .finally(function () {
                ctrl.isLoadedChilds = true;
                ctrl.isProgressLoadChilds = false;
            });
    };
};

export default MobileMenuItemCtrl;