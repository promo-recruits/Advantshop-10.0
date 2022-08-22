; (function (ng) {
    'use strict';
    ng.module('photoViewList', [])
        .directive('photoViewList', ['domService', function (domService) {
            return {
                scope: true,
                controller: 'PhotoViewListCtrl',
                controllerAs: 'photoViewList',
                bindToController: true,
                link: function (scope, element, attrs, ctrl) {
                    element[0].classList.add('photo-view-list');
                }
            }
        }]);

    ng.module('photoViewList')
        .directive('photoViewListItem', [ function () {
            return {
                scope: true,
                require: ['^photoViewList'],
                bindToController: true,
                link: function (scope, element, attrs, ctrls) {
                    var photoViewListCtrl = ctrls[0];
                    element[0].classList.add('photo-view-list__item');
                    if (element[0].parentNode != null && !element[0].parentNode.classList.contains('photo-view-list__item-wrap')) {
                        element[0].parentNode.classList.add('photo-view-list__item-wrap');
                    }
                    photoViewListCtrl.activeItemIndex = 0;
                    photoViewListCtrl.isActiveElement = true;

                }
            }
        }]);
    ng.module('photoViewList')
        .directive('photoViewListNav', [ function () {
            return {
                scope: true,
                require: ['^photoViewList'],
                bindToController: true,
                link: function (scope, element, attrs, ctrls) {
                    var photoViewListCtrl = ctrls[0];
                    element[0].classList.add('photo-view-list__nav');
                    photoViewListCtrl.activeNavIndex = 0;
                    photoViewListCtrl.isActiveElementNav = true;

                    element[0].addEventListener('mouseover', function (e) {
                        var target = e.target;
                        var indexActiveNav = target.getAttribute('data-nav-index');
                        photoViewListCtrl.activeNavIndex = parseFloat(indexActiveNav);
                        photoViewListCtrl.activeItemIndex = parseFloat(indexActiveNav);
                        scope.$apply();
                    }, true);

                }
            }
        }]);

})(window.angular);