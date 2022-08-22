; (function (ng) {
    'use strict';

    ng.module('pictureUploader')
        .directive('pictureUploader', ['$templateRequest', 'urlHelper', '$compile', '$sce', function ($templateRequest, urlHelper, $compile, $sce) {
            return {
                bindToController: true,
                controllerAs: '$ctrl',
                transclude: true,
                controller: 'PictureUploaderCtrl',
                scope: {
                    startSrc: '@',
                    pictureId: '@',
                    uploadUrl: '@',
                    uploadParams: '<?',
                    deleteUrl: '@',
                    deleteParams: '<?',
                    uploadbylinkUrl: '@',
                    uploadbylinkParams: '<?',
                    onUpdate: '&',
                    onDelete: '&',
                    startPictureId: '@',
                    uploaderDestination: '@', //название input type=file для тестов
                    fileTypes: '<?' //вариант из pictureUploaderFileTypes
                },
                link: function (scope, element, attrs, ctrl, transclude) {
                    $templateRequest(urlHelper.getAbsUrl('../areas/admin/content/src/_shared/picture-uploader/templates/picture-uploader.html'))
                        .then(function (tpl) {
                            var fragment = document.createDocumentFragment();
                            var innerEl = document.createElement('div');
                            innerEl.innerHTML = tpl;
                            var clone = transclude().clone();
                            var transcludeBlock = innerEl.querySelector('.transclude-block');
                            for (var i = 0; i < clone.length; i++) {
                                fragment.appendChild(clone[i]);
                            }
                            transcludeBlock.appendChild(fragment);

                            var buttonAdd = innerEl.querySelector('.picture-uploader-buttons-add');
                            buttonAdd.setAttribute('data-e2e', 'imgAdd' + ctrl.uploaderDestination);
                            $compile(element.html(innerEl).contents())(scope);
                        });
                }
            };
        }]);


})(window.angular);