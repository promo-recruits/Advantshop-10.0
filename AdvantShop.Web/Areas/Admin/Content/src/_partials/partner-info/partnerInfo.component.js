; (function (ng) {
    'use strict';


    ng.module('partnerInfo')
        .component('partnerInfoContainer', {
            templateUrl: '../areas/admin/content/src/_partials/partner-info/templates/partner-info-container.html',
            controller: 'PartnerInfoContainerCtrl'
        })
        .component('partnerInfo', {
            templateUrl: '../areas/admin/content/src/_partials/partner-info/templates/partner-info.html',
            controller: 'PartnerInfoCtrl',
            bindings: {
                instance: '<?',
                onInit: '&',
                onOpen: '&',
                onClose: '&'
            }
        })

})(window.angular);