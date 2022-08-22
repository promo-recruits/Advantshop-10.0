; (function (ng) {
    'use strict';

    ng.module('changeHistory')
        .component('changeHistory', {
            templateUrl: '../areas/admin/content/src/_partials/change-history/templates/changeHistory.html',
            controller: 'ChangeHistoryCtrl',
            bindings: {
                objId: '<',
                objType: '<',
                type: '@',
                hide: '='
            }
        });

})(window.angular);