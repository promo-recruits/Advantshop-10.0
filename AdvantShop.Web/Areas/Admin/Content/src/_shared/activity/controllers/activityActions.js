; (function (ng) {
    'use strict';

    var ActivityActionsCtrl = function ($http) {
        var ctrl = this;

        ctrl.$onInit = function() {
            $http.get('activity/getActions', { params: { customerId: ctrl.customerId } }).then(function(response) {
                ctrl.items = response.data.DataItems;
            });

            if (ctrl.onInit != null) {
                ctrl.onInit({ activityActions: ctrl });
            }
        };
    };

    ActivityActionsCtrl.$inject = ['$http'];

    ng.module('activity')
        .component('activityActions', {
            templateUrl: '../areas/admin/content/src/_shared/activity/templates/activity-actions.html',
            controller: ActivityActionsCtrl,
            bindings: {
                customerId: '<?',
                onInit: '&',
            }
        });

})(window.angular);