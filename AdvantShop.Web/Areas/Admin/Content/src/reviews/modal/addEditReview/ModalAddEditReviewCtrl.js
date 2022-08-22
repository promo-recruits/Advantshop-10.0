; (function (ng) {
    'use strict';

    var ModalAddEditReviewCtrl = function ($uibModalInstance, $http, toaster, Upload, $translate, $document, $window) {
        var ctrl = this;

        ctrl.ckeditor = {
            height: 150,
            extraPlugins: 'codemirror,lineheight,autogrow',
            bodyClass: 'm-n textarea-padding',
            toolbar: {},
            toolbarGroups: {},
            resize_enabled: false,
            toolbar_emptyToolbar: { name: 'empty', items: [] },
            autoGrow_minHeight: 150,
            autoGrow_onStartup: true,
            on: {
                instanceReady: function (event) {
                    $document[0].getElementById(event.editor.id + '_top').style.display = 'none';
                },
            },
            disableNativeSpellChecker: false,
            browserContextMenuOnCtrl: false,
            removePlugins: 'language,liststyle,tabletools,scayt,menubutton,contextmenu,tableselection,elementspath'
        };

        ctrl.$onInit = function () {

            var params = ctrl.$resolve;
            ctrl.reviewId = params.reviewId != null ? params.reviewId : 0;
            ctrl.mode = ctrl.reviewId != 0 ? 'edit' : 'add';

            ctrl.isLoading = true;

            ctrl.getFormData().then(function (data) {
                ctrl.filesHelpText = data.filesHelpText;
                if (ctrl.mode == 'add') {
                    ctrl.AddDate = (new Date(Date.now()).toLocaleString()).replace(',', '');
                    ctrl.Checked = true;
                    ctrl.Photos = [];
                    return data;
                } else {
                    return ctrl.getReview(ctrl.reviewId);
                }
            })
                .then(function () {
                    ctrl.isLoading = false;
                });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
        
        ctrl.getFormData = function () {
            return $http.post('reviews/getFormData').then(function (response) {
                return response.data;
            });
        };

        ctrl.getReview = function(reviewId) {
            return $http.get('reviews/getReview', { params: { reviewId: reviewId } }).then(function(response) {

                if (response.data != null && response.data.result) {
                    var data = response.data.obj;

                    ctrl.Name = data.Name;
                    ctrl.Email = data.Email;
                    ctrl.Text = data.Text;
                    ctrl.Checked = data.Checked;
                    ctrl.AddDate = data.AddDateFormatted;
                    ctrl.Ip = data.Ip;
                    //ctrl.Photo = data.PhotoSrc;
                    //ctrl.PhotoName = data.PhotoName;
                    ctrl.Photos = data.Photos;
                    ctrl.ArtNo = data.ArtNo;
                    ctrl.productName = data.ProductName;
                    ctrl.productUrl = data.ProductUrl;

                    return data;

                } else if (response.data.errors != null) {
                    response.data.errors.forEach(function(error) {
                        toaster.pop('error', '', error);
                    });
                    $window.location.assign('reviews');
                }
            });
        };

        ctrl.uploadImage = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop')) {
                if ($files && $files.length) {
                    for (var i = 0; i < $files.length; i++) {
                        ctrl.pushImages($files[i]);
                    }
                }
            }

            if ($invalidFiles.length > 0) {
                toaster.pop('error', '', $translate.instant('Admin.Js.Reviews.FileDoesNotMeet'));
            }
        };

        ctrl.pushImages = function (image) {
            ctrl.Photos.push(image);
        };

        ctrl.deletePhoto = function (photoId, index) {
            if (photoId) {
                $http.post('reviews/deletePhoto', { reviewId: ctrl.reviewId, photoId: photoId }).then(
                    function (response) {
                        if (response.data.result) {
                            //ctrl.Photos = ctrl.Photos.filter(function(photo) { return photo.PhotoId != photoId });
                            ctrl.Photos.splice(index, 1);
                        }
                    });
            } else {
                ctrl.Photos.splice(index, 1);
            }
        };

        ctrl.save = function() {

            var params = {
                reviewId: ctrl.reviewId,
                Name: ctrl.Name,
                Email: ctrl.Email,
                Text: ctrl.Text,
                Checked: ctrl.Checked,
                AddDate: ctrl.AddDate,
                ArtNo: ctrl.ArtNo,
            };

            var url = ctrl.mode == 'add' ? 'reviews/addReview' : 'reviews/updateReview';

            Upload.upload({
                url: url,
                data: ng.extend(params,
                    {
                        rnd: Math.random(),
                    }),
                file: ctrl.Photos.filter(function (image) { return image.name; }) // or list of files (files) for html5 only
            }).then(function(response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Reviews.ChangesSaved'));
                    $uibModalInstance.close('saveColor');
                } else {
                    if (data.error != null) {
                        toaster.pop('error', '', data.error);
                    } else if (data.errors != null) {
                        data.errors.forEach(function(error) {
                            toaster.pop('error', '', error);
                        });
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Reviews.ErrorWhileCreatingEditing'));
                    }
                }
            });
        };
    };

    ModalAddEditReviewCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', 'Upload', '$translate', '$document', '$window'];

    ng.module('uiModal')
        .controller('ModalAddEditReviewCtrl', ModalAddEditReviewCtrl);

})(window.angular);