; (function (ng) {
    'use strict';

    var ModalAddEditCarouselCtrl = function ($uibModalInstance, $http, toaster, Upload, $translate) {
        var ctrl = this;
        
        ctrl.$onInit = function () {
            ctrl.CarouselID = 0;
        };

        
        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.updateImage = function(result){
            ctrl.ImageSrc = result.pictureName;
        }

        ctrl.saveCarousel = function () {

            ctrl.btnSleep = true;

            if (ctrl.ImageSrc == undefined || ctrl.ImageSrc == null || ctrl.ImageSrc == '') {
                toaster.pop('error', $translate.instant('Admin.Js.Carousel.ImageNotUploaded'), $translate.instant('Admin.Js.Carousel.PleaseUploadAnImage'));
                ctrl.btnSleep = false;
                return;
            }

            var params = {
                CarouselID: ctrl.CarouselID,
                CaruselUrl: ctrl.CaruselUrl,
                DisplayInOneColumn: ctrl.DisplayInOneColumn,
                DisplayInTwoColumns: ctrl.DisplayInTwoColumns,
                DisplayInMobile: ctrl.DisplayInMobile,
                Blank: ctrl.Blank,
                SortOrder: ctrl.SortOrder,
                Enabled: ctrl.Enabled,
                ImageSrc: ctrl.ImageSrc,
                Description: ctrl.Description,
                rnd: Math.random()
            };

            var url = 'Carousel/AddCarousel';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop("success", "", $translate.instant('Admin.Js.Carousel.ChangesSaved'));
                    $uibModalInstance.close('saveCarousel');
                } else {
                    toaster.pop("error", $translate.instant('Admin.Js.Carousel.Error'), $translate.instant('Admin.Js.Carousel.ErrorWhileAddingImage'));
                    ctrl.btnSleep = false;
                }
            });
        }
    };

    ModalAddEditCarouselCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', 'Upload', '$translate'];

    ng.module('uiModal')
        .controller('ModalAddEditCarouselCtrl', ModalAddEditCarouselCtrl);

})(window.angular);