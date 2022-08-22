; (function (ng) {
    'use strict';

    var PictureUploaderCtrl = function ($http, Upload, toaster, SweetAlert, $translate, pictureUploaderFileTypes) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.src = ctrl.startSrc;
            ctrl.pictureId = ctrl.startPictureId;

            if (ctrl.fileTypes == null) {
                ctrl.fileTypes = 'image';
            }

            ctrl.allowExts = pictureUploaderFileTypes[ctrl.fileTypes];
        };

        ctrl.upload = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (ctrl.uploadParams == null) {
                ctrl.uploadParams = {};
            }

            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                ctrl.send($file);
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.PictureUploader.ErrorWhileLoading'), $translate.instant('Admin.Js.PictureUploader.FileDoesNotMeetRequirements'));
            }
        };

        ctrl.uploadByLink = function (result) {
            if (ctrl.uploadbylinkParams == null) {
                ctrl.uploadbylinkParams = {};
            }
            ctrl.uploadbylinkParams.fileLink = result;
            $http.post(ctrl.uploadbylinkUrl, ctrl.uploadbylinkParams).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    ctrl.src = data.obj.picture;
                    ctrl.pictureId = data.obj.pictureId;
                    toaster.pop('success', '', $translate.instant('Admin.Js.PictureUploader.ImageSaved'));
                } else {
                    toaster.error($translate.instant('Admin.Js.PictureUploader.ErrorWhileLoading'), (data.errors || [data.error])[0]);
                }

                if (ctrl.onUpdate != null) {
                    ctrl.onUpdate({ result: data.obj || {} });
                }
            });
        };

        ctrl.delete = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.PictureUploader.AreYouSureDelete'), { title: $translate.instant('Admin.Js.PictureUploader.Deleting') })
                .then(function (result) {
                    if (result === true) {
                        var objId = (ctrl.uploadParams && ctrl.uploadParams.objId) || (ctrl.uploadbylinkParams && ctrl.uploadbylinkParams.objId);

                        return $http.post(ctrl.deleteUrl, { pictureId: ctrl.pictureId, objId: objId }).then(function (response) {
                            var data = response.data;
                            if (data.result === true) {
                                ctrl.updatePhotoData(null, data.obj.picture);
                                toaster.pop('success', '', $translate.instant('Admin.Js.PictureUploader.ImageDeleted'));

                                if (ctrl.onDelete != null) {
                                    ctrl.onDelete({ result: data.obj || {} });
                                }
                            } else {
                                toaster.error($translate.instant('Admin.Js.PictureUploader.ErrorWhileDeleting'), (data.errors || [data.error])[0]);
                            }
                        });
                    }
                });
        };

        ctrl.send = function (file) {
            return Upload.upload({
                url: ctrl.uploadUrl,
                data: ng.extend(ctrl.uploadParams, {
                    file: file,
                    rnd: Math.random(),
                })
            }).then(function (response) {
                var data = response.data;

                if (data.result === true) {

                    ctrl.updatePhotoData(data.obj.pictureId, data.obj.picture);

                    toaster.pop('success', '', $translate.instant('Admin.Js.PictureUploader.ImageSaved'));
                } else {
                    toaster.error($translate.instant('Admin.Js.PictureUploader.ErrorWhileLoading'), (data.errors || [data.error])[0]);
                }

                if (ctrl.onUpdate != null) {
                    ctrl.onUpdate({ result: data.obj || {} });
                }
            })
        };

        ctrl.updatePhotoData = function (pictureId, src) {
            ctrl.pictureId = pictureId;
            ctrl.src = src;
        };
    };

    PictureUploaderCtrl.$inject = ['$http', 'Upload', 'toaster', 'SweetAlert', '$translate', 'pictureUploaderFileTypes'];

    ng.module('pictureUploader', ['uiModal', 'toaster'])
        .controller('PictureUploaderCtrl', PictureUploaderCtrl)
        .constant('pictureUploaderFileTypes', {
            'image': ["jpg", "gif", "png", "bmp", "jpeg"],
            'favicon': ["ico", "gif", "png"]
        });

})(window.angular);