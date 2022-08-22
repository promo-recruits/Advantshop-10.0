; (function (ng) {
    'use strict';

    angular.module('wishlist')
        .directive('wishlistControl', ['wishlistService', function (wishlistService) {
          return {
              restrict: 'A',
              scope: true,
              controller: 'WishlistControlCtrl',
              controllerAs: 'wishlistControl',
              bindToController: true,
              link: function (scope, element, attrs, ctrl) {
                  if (attrs.wishlistControl != null) {
                      wishlistService.addWishlistScope(parseInt(attrs.wishlistControl), ctrl);
                  }
              }
          };
      }]);

    angular.module('wishlist')
  .directive('wishlistCount', function () {
      return {
          restrict: 'A',
          scope: true,
          controller: 'WishlistCountCtrl',
          controllerAs: 'wishlistCount',
          bindToController: true,
          link: function (scope, element, attrs, ctrl) {
              ctrl.countObj.count = parseInt(attrs.startCount, 10);
          }
      };
  });

    angular.module('wishlist')
      .directive('wishlistWrapper', ['wishlistService', 'domService', function (wishlistService, domService) {
          return {
              restrict: 'A',
              scope: true,
              link: function (scope, element, attrs, ctrl) {
                  var items = element[0].querySelectorAll(attrs.wishlistWrapper),
                      dirRemove;

                  if (items != null && items.length > 0) {


                      dirRemove = document.createElement('a');
                      dirRemove.href = 'javascript:void(0);';
                      dirRemove.className = 'js-wishlist-remove wishlist-remove icon-cancel-circled-before link-text-decoration-none cs-l-5';

                      for (var i = items.length - 1 ; i >= 0; i--) {
                          items[i].appendChild(dirRemove.cloneNode());

                          items[i].addEventListener('click', function (event) {

                              var item = this;

                              if (event.target.classList.contains('js-wishlist-remove')) {
                                  wishlistService.remove(item.getAttribute('data-offer-id')).then(function () {

                                      var blockForDelete = domService.closest(item, '.js-products-view-block');

                                      blockForDelete.parentNode.removeChild(blockForDelete);
                                  });
                              }
                          });
                      }
                  }
              }
          };
      }]);

})(window.angular);