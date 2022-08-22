; (function (ng) {
    'use strict';

    ng.module('notificationMessage')
        .directive('notificationMessage', ['toaster', '$timeout', function (toaster, $timeout) {
            return {
                controller: 'NotificationMessageCtrl',
                controllerAs: '$ctrl',
                scope: true,
                bindToController: true
            };
        }]);

})(window.angular);