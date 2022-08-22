; (function (ng) {
    'use strict';

    ng.module('triggers')
        .component('triggerEdit', {
            templateUrl: '../areas/admin/content/src/triggers/components/triggerEdit/templates/triggerEdit.html',
            controller: 'TriggerEditCtrl',
            controllerAs: 'ctrl',
            bindings: {
                id: '<?',
                eventType: '<?'
            }
        });

})(window.angular);