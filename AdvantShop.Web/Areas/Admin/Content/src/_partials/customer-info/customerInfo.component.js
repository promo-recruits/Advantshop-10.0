; (function (ng) {
    'use strict';


    ng.module('customerInfo')
        .component('customerInfoContainer', {
            templateUrl: '../areas/admin/content/src/_partials/customer-info/templates/customer-info-container.html',
            controller: 'CustomerInfoContainerCtrl'
        })
        .component('customerInfo', {
            templateUrl: '../areas/admin/content/src/_partials/customer-info/templates/customer-info.html',
            controller: 'CustomerInfoCtrl',
            bindings: {
                instance: '<?',
                onInit: '&',
                onOpen: '&',
                onClose: '&'
            }
        })

})(window.angular);