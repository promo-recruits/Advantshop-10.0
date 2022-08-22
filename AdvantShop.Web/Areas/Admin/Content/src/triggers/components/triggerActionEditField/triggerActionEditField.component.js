; (function(ng) {
    'use strict';

    ng.module('triggers')
        .component('triggerActionEditField', {
            templateUrl: '../areas/admin/content/src/triggers/components/triggerActionEditField/templates/triggerActionEditField.html',
            controller: 'TriggerActionEditFieldCtrl',
            controllerAs: 'ctrl',
            bindings: {
                eventType: '=',
                action: '=',
                fields: '<',
                isLicense: '<'
            }
        });

})(window.angular);