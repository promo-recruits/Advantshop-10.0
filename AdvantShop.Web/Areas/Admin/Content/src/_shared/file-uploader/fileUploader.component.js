; (function (ng) {
    'use strict';

    ng.module('fileUploader')
        .component('fileUploader', {
            templateUrl: '../areas/admin/content/src/_shared/file-uploader/templates/file-uploader.html',
            controller: 'FileUploaderCtrl',
            bindings: {
                startSrc: '@',
                uploadUrl: '@',
                uploadParams: '<?',
                deleteUrl: '@',
                deleteParams: '<?',
                uploadbylinkUrl: '@',
                uploadbylinkParams: '<?',
                onUpdate: '&',
                onSuccess: '&',
                onBeforeSend: '&',
                accept: '@',
                showResult: '<?',
                disabled: '<?',
                goToFirstStepAfterSucces: '<?',

                titleUploadButton: '@',
                titleUploadLinkButton: '@',
                titleDeleteButton: '@',

                notSendImmediately: '<?',
                onInit: '&'
            }
        });

})(window.angular);