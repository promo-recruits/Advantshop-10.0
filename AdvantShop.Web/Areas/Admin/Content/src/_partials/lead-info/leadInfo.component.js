; (function (ng) {
    'use strict';


    ng.module('leadInfo')
        .component('leadInfoContainer', {
            templateUrl: '../areas/admin/content/src/_partials/lead-info/templates/lead-info-container.html',
            controller: 'LeadInfoContainerCtrl'
        })
        .component('leadInfo', {
            templateUrl: '../areas/admin/content/src/_partials/lead-info/templates/lead-info.html',
            controller: 'LeadInfoCtrl',
            bindings: {
                instance: '<?',
                onInit: '&',
                onOpen: '&',
                onClose: '&'
            }
        })

})(window.angular);