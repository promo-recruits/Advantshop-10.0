; (function (ng) {
    'use strict';

    ng.module('videoFileUploader')
        .directive('videoFileUploader', [ function () {
            return {
                restrict: 'A',
                transclude: true,
                controller: 'VideoFileUploaderCtrl',
                bindToController: true,
                controllerAs: 'videoFileUploader',
                templateUrl: './areas/landing/frontend/_common/video-file-uploader/video-file-uploader.html',
                scope: {
                    settings: '<',
                    pattern: '<?',
                    accept: '@',
                    upload: '&',
                    showProgress: '<?',
                    drop: '<?',
                    dropSize: '<?',
                    uploadUrl: '@',
                    deleteUrl: '@',
                    urlListVideo: '@'
                },
                link: function (scope, element, attrs, ctrl) {
                    ctrl.pattern = ctrl.pattern || 'video/*';
                    ctrl.accept = ctrl.accept || '.webm';
                    ctrl.dropSize = ctrl.dropSize || { width: 200, height: 300 };
                }
            };
        }]);

})(window.angular);

