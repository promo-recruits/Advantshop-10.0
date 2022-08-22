; (function (ng) {
    'use strict';

    ng.module('productView')
        .directive('productViewItem', ['productViewService', 'domService', 'windowService', '$parse', function (productViewService, domService, windowService, $parse) {
          return {
              require: ['^productViewItem'],
              restrict: 'A',
              controller: 'ProductViewItemCtrl',
              controllerAs: 'productViewItem',
              bindToController: true,
              scope: true,
              link: function (scope, element, attrs, ctrls) {

                  var productViewItemCtrl = ctrls[0],
                      timerHover;

                  productViewItemCtrl.productId = parseInt(attrs.productId);

                  if (attrs.offer != null) {
                      productViewItemCtrl.offer = $parse(attrs.offer)(scope);
                  }

                  if (attrs.onChangeColor != null) {
                      productViewItemCtrl.onChangeColor = attrs.onChangeColor;
                  }

                  if (attrs.maxPhotoView != null) {
                      productViewItemCtrl.maxPhotoView = parseFloat(attrs.maxPhotoView);
                  }


                  if (attrs.onlyPhotoWithColor != null) {
                      productViewItemCtrl.onlyPhotoWithColor = attrs.onlyPhotoWithColor === 'true';
                  }
                 
                  productViewItemCtrl.offerId = parseInt(attrs.offerId);

                  //productViewItemCtrl.getOffersProduct(productViewItemCtrl.productId);

                  productViewService.addCallback('setView', function (viewMode) {

                      productViewItemCtrl.viewMode = viewMode;

                      setTimeout(function () {

                          var colorsViewerCarousel = productViewItemCtrl.getControl('colorsViewerCarousel');

                          if (colorsViewerCarousel != null) {
                              colorsViewerCarousel.update();
                          }

                          scope.$digest();

                      }, 50);

                  });


                  element[0].addEventListener('mouseenter', function () {

                      if (timerHover != null) {
                          clearTimeout(timerHover);
                      }

                      timerHover = setTimeout(function () {
                          productViewItemCtrl.enter();
                          scope.$digest();
                      }, 100);
                  });

                  element[0].addEventListener('mouseleave', function () {

                      clearTimeout(timerHover);

                      productViewItemCtrl.leave();

                      scope.$digest();
                  });

                  element[0].addEventListener('touchstart', function () {
                      productViewItemCtrl.enter();
                      scope.$digest();
                  });

                  windowService.addCallback('touchstart', function (eventObj) {
                      var isClickedMe = domService.closest(eventObj.event.target, element[0]) != null;

                      if (isClickedMe === false) {
                          productViewItemCtrl.leave();

                          scope.$digest();
                      }
                  });
              }
          }
      }]);

    ng.module('productView')
      .directive('productViewCarouselPhotos', function () {
          return {
              require: ['^productViewCarouselPhotos', '^productViewItem'],
              restrict: 'A',
              scope: {
                  photoHeight: '@',
                  photoWidth: '@',
                  changePhoto: '&'
              },
              replace: true,
              templateUrl: '/scripts/_partials/product-view/templates/photos.html',
              controller: 'ProductViewCarouselPhotosCtrl',
              controllerAs: 'photosCarousel',
              bindToController: true,
              link: function (scope, element, attrs, ctrl) {

                  var carouselPhotosCtrl = ctrl[0],
                      productViewItemCtrl = ctrl[1];

                  carouselPhotosCtrl.parentScope = productViewItemCtrl;

                  productViewItemCtrl.addControl('photosCarousel', carouselPhotosCtrl);
              }
          }
      });

    ng.module('productView')
        .directive('productViewChangeMode', ['productViewService', 'viewList', function (productViewService, viewList) {
          return {
              restrict: 'A',
              scope: true,
              controller: 'ProductViewChangeModeCtrl',
              controllerAs: 'changeMode',
              bindToController: true,
              link: function (scope, element, attrs, ctrl) {
                  ctrl.name = attrs.name;
                  ctrl.currentViewList = viewList[attrs.viewListName || 'desktop'];
                  ctrl.isMobile = attrs.isMobile === 'true';
                  ctrl.isReadyViewMode = false;
                  if (ctrl.isMobile === true) {
                      ctrl.current = productViewService.getViewFromCookie('mobile_viewmode', ctrl.currentViewList);
                  } else {
                      ctrl.current = attrs.viewMode;
                  }

                  ctrl.isReadyViewMode = true;
              }
          };
      }]);

    ng.module('productView')
        .directive('productViewMode', ['productViewService', 'viewList', 'viewPrefix', function (productViewService, viewList, viewPrefix) {
        return {
            restrict: 'A',
            scope: true,
            controller: 'ProductViewModeCtrl',
            controllerAs: 'productViewMode',
            bindToController: true,
            link: function (scope, element, attrs, ctrl) {

                ctrl.isMobile = attrs.isMobile === 'true';
                ctrl.currentViewList = viewList[attrs.viewListName || 'desktop'];
                ctrl.currentViewPrefix = viewPrefix[attrs.viewListName || 'desktop'];

                if (ctrl.isMobile === true) {
                    ctrl.viewName = productViewService.getViewFromCookie('mobile_viewmode', ctrl.currentViewList);
                    element[0].classList.add('products-view-' + ctrl.currentViewPrefix + ctrl.viewName);
                } else {
                    ctrl.viewName = attrs.current;
                }

                productViewService.addCallback('setView', onChangeMode);

                function onChangeMode(view) {
                    view.viewList.forEach(function (item) {
                        element[0].classList.remove('products-view-' + ctrl.currentViewPrefix + item);
                    });

                    element[0].classList.add('products-view-' + ctrl.currentViewPrefix + view.viewName);

                    ctrl.viewName = view.viewName;
                }

            }
        };
    }]);

})(window.angular);