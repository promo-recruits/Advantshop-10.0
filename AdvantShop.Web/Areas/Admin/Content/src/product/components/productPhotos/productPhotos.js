; (function (ng) {
    'use strict';

    var ProductPhotosCtrl = function ($http, $q, toaster, productPhotosService, SweetAlert, Upload, $timeout, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {

            ctrl.load();

            if (ctrl.onInit != null) {
                ctrl.onInit({ productPhotos: ctrl });
            }
        };


        ctrl.load = function () {
            return productPhotosService.getPhotoColors(ctrl.productId).then(function (data) {
                ctrl.photoColors = data;
                ctrl.allPhotoColors = data.slice();
                ctrl.allPhotoColors.unshift({ value: -1, label: $translate.instant('Admin.Js.Product.All') });
                ctrl.filterColorId = ctrl.allPhotoColors[0].value;
                return ctrl.getPhotos();
            });
        }

        ctrl.changeMainPhoto = function (photoId) {
            productPhotosService.changeMainPhoto(photoId).then(function (response) {
                if (response) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                }
                return response;
            })
                .then(ctrl.getPhotos)
                .then(ctrl.findMainPhoto)
                .then(function (mainPhoto) {
                    if (ctrl.onChangeMainPhoto != null) {
                        ctrl.onChangeMainPhoto({ mainPhoto: mainPhoto });
                    }
                });
        }

        ctrl.editPhoto = function (photoId, alt, colorId) {
            productPhotosService.editPhoto(photoId, alt, colorId).then(function (response) {
                if (response) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                }
                ctrl.getPhotos();
            });
        }

        ctrl.changePhotoColor = function (photoId, colorId) {
            productPhotosService.changePhotoColor(photoId, colorId).then(function (response) {
                if (response) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                }
            }).then(ctrl.getPhotos);
        }

        ctrl.deletePhoto = function (photoId) {
            SweetAlert.confirm($translate.instant('Admin.Js.Product.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Product.Deleting') }).then(function (result) {
                if (result === true) {
                    productPhotosService.deletePhoto(photoId)
                        .then(ctrl.getPhotos)
                        .then(ctrl.findMainPhoto)
                        .then(function (mainPhoto) {

                            toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));

                            if (ctrl.onDeletePhoto != null) {
                                ctrl.onDeletePhoto({ mainPhoto: mainPhoto });
                            }
                        });
                }
            });
        }

        ctrl.sortableOptions = {
            containerPositioning: 'relative',
            containment: '#productPhotosSortable',
            orderChanged: function (event) {
                var photoId = event.source.itemScope.item.PhotoId,
                    prevPhoto = ctrl.photos[event.dest.index - 1],
                    nextPhoto = ctrl.photos[event.dest.index + 1];

                productPhotosService.changePhotoSortOrder(ctrl.productId, photoId, prevPhoto != null ? prevPhoto.PhotoId : null, nextPhoto != null ? nextPhoto.PhotoId : null).then(function () {
                    toaster.pop("success", '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                    ctrl.getPhotos();
                });
            }
        }

        ctrl.upload = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Upload.upload({
                    url: 'product/uploadphotos',
                    data: {
                        files: $files,
                        productId: ctrl.productId
                    }
                }).then(function (response) {
                    if (response.data.result === true) {
                        ctrl.getPhotos()
                            .then(ctrl.findMainPhoto)
                            .then(function (mainPhoto) {
                                if (ctrl.onUploadPhoto != null) {
                                    ctrl.onUploadPhoto({ mainPhoto: mainPhoto });
                                }
                            });
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.UploadingPhotoIsComplete'));
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Product.ErrorWhileUploadingPhotos'), response.data.error);
                    }
                }, function (error) {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileUploadingPhotos'));
                });
            } else if ($invalidFiles.length > 0) {
                if ($invalidFiles[0] != null && $invalidFiles[0].$errorMessages != null && $invalidFiles[0].$errorMessages.maxSize == true) {
                    toaster.pop('error', $translate.instant('Admin.Js.Product.ErrorWhileUploading'), $translate.instant('Admin.Js.Product.FileDoesNotMeetSizeRequirements'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.Product.ErrorWhileUploading'), $translate.instant('Admin.Js.Product.FileDoesNotMeetRequirements'));
                }
            }
        };

        ctrl.uploadByLink = function (result) {
            productPhotosService.uploadPhoto(ctrl.productId, result).then(function (data) {

                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.UploadingPhotoIsComplete'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.Product.ErrorWhileUploadingPhotos'), data.error);
                }

                ctrl.getPhotos()
                    .then(ctrl.findMainPhoto)
                            .then(function (mainPhoto) {
                                if (ctrl.onUploadPhoto != null) {
                                    ctrl.onUploadPhoto({ mainPhoto: mainPhoto });
                                }
                            });
            });
        }

        ctrl.updateListPhotos = function () {
                ctrl.getPhotos()
                    .then(ctrl.findMainPhoto)
                            .then(function (mainPhoto) {
                                if (ctrl.onUploadPhoto != null) {
                                    ctrl.onUploadPhoto({ mainPhoto: mainPhoto });
                                }
                            });
        };

        ctrl.filterPhotos = function () {
            if (ctrl.filterColorId === -1) {
                ctrl.photos = ctrl.allPhotos;
            } else {
                ctrl.photos = ctrl.allPhotos.filter(function (x) { return x.ColorId === ctrl.filterColorId; });
            }
            return ctrl.photos;
        }

        ctrl.findMainPhoto = function (photos) {
            var mainPhoto;
            for (var i = 0, len = photos.length; i < len; i++) {
                if (photos[i].Main === true) {
                    mainPhoto = photos[i];
                    break;
                }
            }

            return mainPhoto;
        };

        ctrl.getPhotos = function () {
            return productPhotosService.getPhotos(ctrl.productId).then(function (photos) {
                ctrl.photos = photos;
                ctrl.allPhotos = photos;

                return photos;
            }).then(ctrl.filterPhotos);
        }
    };

    ProductPhotosCtrl.$inject = ['$http', '$q', 'toaster', 'productPhotosService', 'SweetAlert', 'Upload', '$timeout', '$translate'];

    ng.module('productPhotos', ['as.sortable'])
        .controller('ProductPhotosCtrl', ProductPhotosCtrl)
        .component('productPhotos', {
            templateUrl: '../areas/admin/content/src/product/components/productPhotos/productPhotos.html',
            controller: 'ProductPhotosCtrl',
            bindings: {
                productId: '@',
                showGoogleImageSearch: '<?',
                showImageSearch: '<?',
                onChangeMainPhoto: '&',
                onDeletePhoto: '&',
                onUploadPhoto: '&',
                onInit: '&'
            }
        });

})(window.angular);