; (function (ng) {
    'use strict';

    var ProductViewItemCtrl = function ($q, $timeout, productViewService, $translate, $scope, $parse) {

        var ctrl = this, controls = {}, needCarouselUpdate = false, requestGetPhotosInPending = false, isPhotosStorageMutation = false, photosStorage;
        ctrl.photosVisible = false;
        ctrl.photos = []; 
        ctrl.picture = {};
        ctrl.promisesGetPhotos = [];
        ctrl.colorSelected = null;

        ctrl.getOffersProduct = function (productId) {
            return productViewService.getOfferId(productId).then(function (result) {
                if (result != null) {
                    return result.Offers;
                }
            });
        };

        ctrl.getPhotos = function (productId) {
            var defer = $q.defer(),
                promise;

            if (requestGetPhotosInPending === false && (photosStorage == null || needCarouselUpdate === true)) {
                requestGetPhotosInPending = true;
                promise = productViewService.getPhotos(ctrl.productId || productId).then(function (photos) {
                    isPhotosStorageMutation = true;

                    for (var i = 0, len = ctrl.promisesGetPhotos.length; i < len; i++) {
                        ctrl.promisesGetPhotos[i].resolve(photos);
                    }

                    ctrl.promisesGetPhotos.length = 0;

                    requestGetPhotosInPending = false; 

                    return photosStorage = photos;
                });
            } else if (requestGetPhotosInPending === true) {
                promise = defer.promise;
                ctrl.promisesGetPhotos.push(defer);
            } else {
                promise = defer.promise;
                defer.resolve(photosStorage);
            }

            return promise;
        };

        ctrl.numberals = function (num) {
            if (num <= 0) return ctrl.textNumberals = num + ' ' + $translate.instant('Js.ProductView.Photos0');
            num = num % 100;
            var nums = num % 10;
            if (num > 10 && num < 20) return ctrl.textNumberals = num + ' ' + $translate.instant('Js.ProductView.Photos5');
            if (nums > 1 && nums < 5) return ctrl.textNumberals = num + ' ' + $translate.instant('Js.ProductView.Photos2');
            return nums === 1 ? ctrl.textNumberals = num + ' ' + $translate.instant('Js.ProductView.Photos1')
            : ctrl.textNumberals = num + ' ' + $translate.instant('Js.ProductView.Photos5');
        };

        ctrl.fill = function (photos) {
            if (ctrl.getControl('colorsViewer') != null) {
                ctrl.photos = ctrl.filterPhotos(photosStorage == null && isPhotosStorageMutation === false ? ctrl.getPhotos() : photos, ctrl.getControl('colorsViewer').colorSelected.ColorId, ctrl.onlyPhotoWithColor);
            } else {
                ctrl.photos = photos;

            }

            if (ctrl.photos.length === 0) {
                ctrl.photos.length = 0;
                ctrl.photos.push.apply(ctrl.photos, ctrl.getMainPhoto(photos));
            }
            if (ctrl.maxPhotoView != null) {
                ctrl.photos = ctrl.photos.slice(0, ctrl.maxPhotoView);
            }

            ctrl.numberals(ctrl.photos.length);
            return photos;
        };

        ctrl.process = function (productId) {

            return ctrl.getPhotos(productId).then(function (photos) {

                ctrl.fill(photos);


                $timeout(function () {

                    ctrl.carouselInit = true;

                    if (needCarouselUpdate === true && ctrl.getControl('photosCarousel') != null && ctrl.getControl('photosCarousel').carousel != null) {
                        ctrl.getControl('photosCarousel').carousel.update();
                        needCarouselUpdate = false;
                    }
                }, 0);

                return photos;
            });
        };

        ctrl.clearPhotos = function () {
            photosStorage = null;
            needCarouselUpdate = true;
        };

        ctrl.enter = function () {

            ctrl.photosVisible = true;

            ctrl.process();
        };

        ctrl.leave = function () {
            ctrl.photosVisible = false;
            ctrl.carouselInit = false;
        };

        ctrl.changePhoto = function (photo) {
            ctrl.picture = photo;
        };

        ctrl.initColors = function(colorsViewer) {
            ctrl.addControl('colorsViewer', colorsViewer);

            if (colorsViewer.changeStartSelectedColor != null) {
                setTimeout(function() { colorsViewer.selectColorById(colorsViewer.changeStartSelectedColor); }, 500);
            }
        };

        ctrl.getSelectedColorId = function () {
            var colorsViewer = ctrl.getControl('colorsViewer'),
                colorId;

            if (colorsViewer != null && colorsViewer.colorSelected != null && colorsViewer.getDirtyState() === true) {
                colorId = colorsViewer.colorSelected.ColorId;
            }

            return colorId;
        };

        ctrl.initColorsCarousel = function (carousel) {
            ctrl.addControl('colorsViewerCarousel', carousel);
        };

        ctrl.changeColor = function (color) {

            ctrl.getOffersProduct(ctrl.productId).then(function (result) {
                ctrl.productOffers = result;
                if (ctrl.productOffers != null && ctrl.productOffers.length > 0) {
                    ctrl.selectedProductOffer = ctrl.productOffers.filter(function (offer) {
                        return offer.Color.ColorId === color.ColorId;
                    });
                    if (ctrl.selectedProductOffer != null && ctrl.selectedProductOffer.length > 0) {

                        var o = null;
                        for (var i = 0; i < ctrl.selectedProductOffer.length; i++) {
                            if (ctrl.selectedProductOffer[i].Amount > 0) {
                                o = ctrl.selectedProductOffer[i];
                                break;
                            }
                        }

                        ctrl.offer = o == null ? ctrl.selectedProductOffer[0] : o;
                    }
                }
                var defaultPhoto;
                if (photosStorage == null && isPhotosStorageMutation === false) {
                    ctrl.getPhotos().then(function (result) {
                        ctrl.photos = ctrl.filterPhotos(result, color.ColorId, ctrl.getControl('colorsViewer') != null ? ctrl.onlyPhotoWithColor : false);
                        defaultPhoto = ctrl.getMainPhoto(ctrl.photos);
                        //if (ctrl.photos.length === 0) {
                        //    ctrl.photos = defaultPhoto;
                        //}
                        //if (ctrl.maxPhotoView != null) {
                        //    ctrl.photos = ctrl.photos.slice(0, ctrl.maxPhotoView);
                        //}
                        ctrl.setColor(defaultPhoto);
                    });
                } else {
                    defaultPhoto = ctrl.getMainPhoto(photosStorage != null && photosStorage.length !== 0 ? photosStorage : ctrl.photos);
                    ctrl.photos = ctrl.filterPhotos(photosStorage, color.ColorId, ctrl.getControl('colorsViewer') != null ? ctrl.onlyPhotoWithColor : false);

                    ctrl.setColor(defaultPhoto);
                }
                if (ctrl.onChangeColor != null) {
                    $parse(ctrl.onChangeColor)($scope);
                }

                if (ctrl.photos.length === 0) {
                    ctrl.photos.length = 0;
                    ctrl.photos.push.apply(ctrl.photos, defaultPhoto);
                    //ctrl.photos = ctrl.photos.concat(defaultPhoto);
                }

                if (ctrl.maxPhotoView != null) {
                    ctrl.photos = ctrl.photos.slice(0, ctrl.maxPhotoView);
                }

            }).catch(function (error) {
                console.error(error);
            });

        };

        ctrl.setColor = function (defaultPhoto) {

            ctrl.picture = ctrl.photos.length === 0 && defaultPhoto != null ? defaultPhoto[0] : ctrl.photos[0];

            ctrl.numberals(ctrl.photos.length);
            var photosCarousel = ctrl.getControl('photosCarousel');
            if (photosCarousel != null && photosCarousel.carousel != null) {
                $timeout(function () {
                    photosCarousel.carousel.options.indexActive = 0;
                    photosCarousel.carousel.update();
                });
            }

            if (ctrl.photoViewer != null) {
                ctrl.photoViewer.reinit();
            }
        };

        ctrl.addControl = function (name, scope) {
            controls[name] = scope;
        };

        ctrl.getControl = function (name) {
            return controls[name];
        };

        ctrl.filterPhotos = function (photos, colorId, onlyColorPhoto) {
            return photos.filter(function (item) {
                if (onlyColorPhoto) {
                    return item.ColorID === colorId;
                }
                return item.ColorID === colorId || item.ColorID == null;
            });
        };

        ctrl.getUrl = function (url) {
            var result = url,
                colorId = ctrl.getSelectedColorId();

            if (colorId != null) {
                result = result + '?color=' + colorId;
            }

            return result;
        };

        ctrl.addPhotoViewer = function (photoViewer) {
            ctrl.photoViewer = photoViewer;
        };

        ctrl.getPictureByViewMode = function (photos, defaultViewMode, lazyLoadMode) {
            var result;

            if (lazyLoadMode === 'Carousel' && ctrl.isCarouselImgVisible !== true || lazyLoadMode === 'Default' && ctrl.isImgVisible !== true) {
                return null;
            }

            if (ctrl.viewMode != null) {
                result = ctrl.getPictureBySize(ctrl.viewMode.viewName === 'single' ? 'PathMiddle' : 'PathSmall', Object.keys(ctrl.picture).length > 0 ? ctrl.picture : photos[0]);
            } else {
                result = ctrl.getPictureBySize(defaultViewMode === 'small' ? 'PathSmall' : 'PathMiddle', Object.keys(ctrl.picture).length > 0 ? ctrl.picture : photos[0]);
            }

            return result; 
        };

        ctrl.lazyLoadImgInCarousel = function () {
            ctrl.isCarouselImgVisible = true;
        };

        ctrl.lazyLoadImg = function () {
            ctrl.isImgVisible = true;
        };

        ctrl.getPictureBySize = function (size, photos) {
            return photos[size];
        };

        ctrl.getMainPhoto = function (photos) {
            return photos.filter(function (photo) {
                return photo.Main === true;
            });
        }
    };

    ng.module('productView')
      .controller('ProductViewItemCtrl', ProductViewItemCtrl);

    ProductViewItemCtrl.$inject = ['$q', '$timeout', 'productViewService', '$translate', '$scope', '$parse'];

})(window.angular);