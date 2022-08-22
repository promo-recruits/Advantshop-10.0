; (function (ng) {
    'use strict';

    var ImageSearchCtrl = function ($http) {
        var ctrl = this;

        ctrl.apply = function (params) {

            if (ctrl.PictureUploaderCtrl != null) {
                ctrl.PictureUploaderCtrl.updatePhotoData(params.result.pictureId, params.result.picture);
            }

            if (ctrl.onApply != null) {
                ctrl.onApply(params);
            }
        }
    };

    ImageSearchCtrl.$inject = ['$http'];

    ng.module('product')
        .controller('ImageSearchCtrl', ImageSearchCtrl)
        .component('imageSearch', {
            require: {
                PictureUploaderCtrl: '?^pictureUploader'
            },
            templateUrl: '../areas/admin/content/src/_shared/imageSearch/imageSearch.html',
            controller: 'ImageSearchCtrl',
            bindings: {
                uploadbylinkUrl: '@',
                uploadbylinkParams: '<?',
                selectMode: '@',
                onApply: '&'
            }
        });

})(window.angular);