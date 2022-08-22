﻿; (function (ng) {
    'use strict';

    ng.module('adminComments')
        .component('adminComments', {
            templateUrl: '../areas/admin/content/src/_partials/admin-comments/templates/admin-comments.html',
            controller: 'AdminCommentsCtrl',
            bindings: {
                objId: '<',
                objUrl: '<?',
                type: '<?',
                formVisible: '=',
                onInit: '&'
            }
        })
        .component('adminCommentItem', {
            require: {
                parent: '^adminComments'
            },
            controller: 'AdminCommentsItemCtrl',
            templateUrl: '../areas/admin/content/src/_partials/admin-comments/templates/admin-comment-item.html',
            bindings: {
                comment: '<',
                objId: '<',
                type: '<?'
            }
        })
        .component('adminCommentsForm', {
            require: {
                parent: '^adminComments'
            },
            controller: 'AdminCommentsFormCtrl',
            templateUrl: '../areas/admin/content/src/_partials/admin-comments/templates/admin-comments-form.html',
            bindings: {
                visibleFormCancelButton: '=',
                adminCommentId: '=',
                submitFn: '&',
                cancelFn: '&',
                formVisible: '='
            }
        });

})(window.angular);