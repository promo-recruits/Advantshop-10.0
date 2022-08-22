; (function(ng) {
    'use strict';

    ng.module('triggers')
        .component('triggerActionSendRequest', {
            templateUrl: '../areas/admin/content/src/triggers/components/triggerActionSendRequest/templates/triggerActionSendRequest.html',
            controller: 'TriggerActionSendRequestCtrl',
            controllerAs: 'ctrl',
            bindings: {
                eventType: '=',
                action: '=',
                sendRequestParameters: '='
            }
        });

})(window.angular);