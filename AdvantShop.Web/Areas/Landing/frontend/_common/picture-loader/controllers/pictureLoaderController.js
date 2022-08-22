; (function (ng) {
    'use strict';

    var PictureLoaderCtrl = function (pictureLoaderService, $scope, Cropper, $timeout, $http, $q, toaster, Upload, pictureLoaderStates, galleryIconsService) {
        var ctrl = this;
        var ignorePathImages = ['images', 'frontend/images'];

        var file, fileName, data, result, methodSource, methodsSave, methodsSavePostCallbacks;

        function showCropper() { $scope.$broadcast(ctrl.showEvent); }
        function hideCropper() { $scope.$broadcast(ctrl.hideEvent); }

        ctrl.$onInit = function () {
            ctrl.isShowedProgress = false;

            ctrl.cropperOptions = {
                viewMode: 1,
                maximize: true,
                preview: '.preview-container'
            };

            ctrl.cropperProxy = 'cropper.first';
            ctrl.showEvent = 'show';
            ctrl.hideEvent = 'hide';


            ctrl.cropperOptions = ng.extend(ctrl.cropperOptions, ctrl.cropperParams || {});

            var cropperCropFn = ctrl.cropperOptions.crop;
            var cropperReadyFn = ctrl.cropperOptions.ready;
            var cropperCropendFn = ctrl.cropperOptions.cropend;

            ctrl.cropperOptions.crop = function (dataNew) {
                data = dataNew;
                ctrl.widthPicture = Math.ceil(dataNew.detail.width);
                ctrl.heightPicture = Math.ceil(dataNew.detail.height);

                if (cropperCropFn != null) {
                    cropperCropFn.apply(this, arguments);
                }
            };

            ctrl.cropperOptions.ready = function (e) {
                ctrl.crop(ctrl.result, file, data.detail);

                if (cropperReadyFn != null) {
                    cropperReadyFn.apply(this, arguments);;
                }
            };

            ctrl.cropperOptions.cropend = function (e) {
                ctrl.crop(ctrl.result, file, data.detail);

                if (cropperCropendFn != null) {
                    cropperCropendFn.apply(this, arguments);
                }
            };

            if (ctrl.galleryIconsEnabled === true) {
                galleryIconsService.preloadData();
            }

            if (ctrl.onInit != null) {
                ctrl.onInit({ pictureLoader: ctrl });
            }

            if (ctrl.onChangeState != null) {
                ctrl.onChangeState({ state: pictureLoaderStates.init, pictureLoader: ctrl });
            }
        };

        ctrl.showProgress = function () {
            ctrl.isShowedProgress = true;
        };

        ctrl.hideProgress = function () {
            ctrl.isShowedProgress = false;
        };

        ctrl.uploadFile = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if ($file != null) {

                if (ctrl.onChangeState != null) {
                    ctrl.onChangeState({ state: pictureLoaderStates.start, pictureLoader: ctrl });
                }

                ctrl.showProgress();

                methodSource = {
                    type: 'file',
                    data: {
                        $file: $file,
                        ext: pictureLoaderService.getExt($file.name)
                    }
                };

                Upload.imageDimensions($file)
                    .then(function (dimensions) {
                        ctrl.widthPicture = dimensions.width;
                        ctrl.heightPicture = dimensions.height;
                    });


                Cropper.encode(file = $file)
                    .then(function (dataUrl) {
                        ctrl.dataUrlPicture = dataUrl;
                        ctrl.isShowCropper = true;
                        $timeout(showCropper);

                        pictureLoaderService.setVisibleFooter(true);

                        ctrl.hideProgress();
                    });
            }

            if ($invalidFiles != null) {
                for (var i = 0, len = $invalidFiles.length; i < len; i++) {
                    toaster.pop({
                        type: 'error',
                        title: 'Ошибка при выборе файле',
                        body: $invalidFiles[i].$error === 'maxSize' ? 'Превышен лимит размера файла ' + $invalidFiles[i].name + ' в 10Мб.<br><a class="link-decoration-none" target="_blank" href="https://www.advantshop.net/help/pages/reduce-photos">Ссылка на рекомендации по уменьшению размера изображения</a>' : 'Некорректный формат файла ' + $invalidFiles[i].name,
                        bodyOutputType: 'trustedHtml'
                    });
                }
            }
        };

        ctrl.uploadByUrl = function (url) {

            if (ctrl.onChangeState != null) {
                ctrl.onChangeState({ state: pictureLoaderStates.start, pictureLoader: ctrl });
            }

            ctrl.showProgress();

            return pictureLoaderService.getBase64PictureByUrl(url)
                .then(function (data) {

                    if (data.result === false) {
                        return $q.reject(data.error);
                    }

                    methodSource = {
                        type: 'url',
                        data: {
                            url: url,
                            ext: pictureLoaderService.getExt(url)
                        }
                    };

                    var img = new Image();

                    img.onload = function () {
                        ctrl.widthPicture = this.naturalWidth;
                        ctrl.heightPicture = this.naturalHeight;
                    };

                    img.src = url;

                    ctrl.dataUrlPicture = data.picture;
                    ctrl.isShowCropper = true;
                    $timeout(showCropper);

                    pictureLoaderService.setVisibleFooter(true);

                    return data;
                })
                .catch(function (err) {
                    toaster.pop('error', 'Ошибка при загрузке файла по ссылке', err);
                })
                .finally(function () {
                    ctrl.hideProgress();
                });
        };

        ctrl.showGalleryCloud = function () {
            pictureLoaderService.showGalleryCloud(function galleryCloudOnSelect(photo) {
                return ctrl.uploadByUrl(photo.Src.Landscape);
            });
        };

        ctrl.showGalleryIcons = function () {
            return pictureLoaderService.showGalleryIcons(function galleryIconsOnSelect(svg) {

                ctrl.onUploadIcon({
                    result: {
                        picture: svg,
                        type: 'svg'
                    }
                });

                pictureLoaderService.closeModal();

                return svg;
            });
        };

        ctrl.crop = function (result, file, data) {

            var fileGeneratedFromBase64 = null;

            if (file == null) {
                fileGeneratedFromBase64 = Cropper.decode(ctrl.dataUrlPicture);
            }

            Cropper.crop(file || fileGeneratedFromBase64, data)
                .then(Cropper.encode)
                .then(function (base64String) {
                    var prevType = methodSource.type !== 'crop' ? methodSource.type : methodSource.prevType,
                        ext = methodSource.data.ext;

                    methodSource = {
                        type: 'crop',
                        prevType: prevType,
                        data: {
                            base64String: base64String,
                            ext: ext
                        }
                    };
                });
        };


        ctrl.lazyLoadChange = function (lazyLoadState) {
            if (ctrl.onLazyLoadChange != null) {
                ctrl.onLazyLoadChange({ result: lazyLoadState });
            }
        };

        ctrl.delete = function () {

            $q.when(ctrl.type === 'svg' || pictureLoaderService.delete(ctrl.lpId, ctrl.blockId, ctrl.current, ctrl.parameters, ctrl.deleteUrl || 'landinginplace/removepicture'))
                .then(function (result) {

                toaster.pop('success', 'Изображение успешно удалено');

                if (ctrl.onDelete != null) {
                    ctrl.onDelete({ result: result });
                }
            });
 
        };

        methodsSave = {
            'file': function (data) {
                return pictureLoaderService.uploadFile(ctrl.lpId, ctrl.blockId, ctrl.maxWidthPicture, ctrl.maxHeightPicture, ctrl.parameters, data.$file, ctrl.uploadUrlFile || 'landinginplace/uploadpicture', ctrl.current != null && ignorePathImages.some(function (el) { return ctrl.current.indexOf(el) === -1 }) ? ctrl.current : null);
            },
            'url': function (data) {
                return pictureLoaderService.uploadByUrl(ctrl.lpId, ctrl.blockId, ctrl.maxWidthPicture, ctrl.maxHeightPicture, ctrl.parameters, data.url, ctrl.uploadUrlByAddress || 'landinginplace/uploadPictureByUrl', ctrl.current != null && ignorePathImages.some(function (el) { return ctrl.current.indexOf(el) === -1 }) ? ctrl.current : null);
            },
            'crop': function (data) {
                return pictureLoaderService.uploadCropped(ctrl.lpId, ctrl.blockId, ctrl.maxWidthPicture, ctrl.maxHeightPicture, ctrl.parameters, data.base64String, data.ext, ctrl.uploadUrlCropped || 'landinginplace/uploadPictureCropped', ctrl.current != null && ignorePathImages.some(function (el) { return ctrl.current.indexOf(el) === -1 }) ? ctrl.current : null);
            }
        };

        methodsSavePostCallbacks = {
            'file': function (data) {
                return $q.resolve(data)
                    .then(function (data) {
                        if (data.result === false) {
                            return $q.reject(data);
                        } else {
                            return data;
                        }
                    })
                    .then(function (result) {

                        ctrl.result = result;

                        ctrl.current = result.picture;

                        if (ctrl.onUploadFile != null) {
                            ctrl.onUploadFile({ result: result });
                        }

                        return result;
                    })
                    .catch(function (data) {
                        if (data.error != null && data.error.length > 0) {
                            toaster.pop('error', data.error);
                        }
                    });
            },
            'url': function (data) {
                return $q.resolve(data)
                    .then(function (result) {

                        ctrl.result = result;

                        ctrl.current = result.picture;

                        if (ctrl.onUploadByUrl != null) {
                            ctrl.onUploadByUrl({ result: result });
                        }

                        if (result != null && result.result) {
                            ctrl.showUploadByUrl = false;
                        }

                        return result;
                    })
                    .catch(function (data) {
                        if (data.error != null && data.error.length > 0) {
                            toaster.pop('error', data.error);
                        }
                    });
            },
            'crop': function (data) {
                return $q.resolve(data)
                    .then(function (result) {
                        if (methodSource.prevType === 'file' && ctrl.onUploadFile != null) {
                            ctrl.onUploadFile({ result: result });
                        } else if (methodSource.prevType === 'url' && ctrl.onUploadByUrl != null) {
                            ctrl.onUploadByUrl({ result: result });
                        }

                        return result;
                    })
                    .catch(function (data) {
                        if (data.error != null && data.error.length > 0) {
                            toaster.pop('error', data.error);
                        }
                    });
            }
        };

        ctrl.apply = function (callback) {
            if (ctrl.useExternalSave === true) {
                if (ctrl.externalSave != null) {
                    $q.when(ctrl.externalSave({ pictureLoader: ctrl, saveFn: ctrl.save.bind(ctrl, callback), base64String: methodSource.data.base64String }))
                        .then(methodsSavePostCallbacks[methodSource.type])
                        .then(function (result) {
                            if (ctrl.onChangeState != null) {
                                ctrl.onChangeState({ state: pictureLoaderStates.apply, pictureLoader: ctrl });
                            }

                            return result;
                        });
                }
            } else {
                $q.when(ctrl.save(callback))
                    .then(function (result) {

                        if (ctrl.onChangeState != null) {
                            ctrl.onChangeState({ state: pictureLoaderStates.apply, pictureLoader: ctrl});
                        }

                        return result;
                    });
            }
        };

        ctrl.save = function (callback) {
            var result;

            if (methodSource != null) {
                result = methodsSave[methodSource.type](methodSource.data)
                    .then(methodsSavePostCallbacks[methodSource.type])
                    .then(function (result) {
                        callback(result);

                        return result;
                    });
            } else {
                result = $q.when(callback());
            }

            return result;
        };
    };

    ng.module('pictureLoader')
        .controller('PictureLoaderCtrl', PictureLoaderCtrl);

    PictureLoaderCtrl.$inject = ['pictureLoaderService', '$scope', 'Cropper', '$timeout', '$http', '$q', 'toaster', 'Upload', 'pictureLoaderStates', 'galleryIconsService'];

})(window.angular);