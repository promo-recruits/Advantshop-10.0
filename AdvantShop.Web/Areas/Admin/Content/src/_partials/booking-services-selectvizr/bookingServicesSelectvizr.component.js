; (function (ng) {
    'use strict';

    ng.module('bookingServicesSelectvizr')
        .component('bookingServicesSelectvizr', {
            templateUrl: '../areas/admin/content/src/_partials/booking-services-selectvizr/templates/booking-services-selectvizr.html',
            controller: 'BookingServicesSelectvizrCtrl',
            transclude: true,
            bindings: {
                selectvizrTreeUrl: '<',
                selectvizrGridUrl: '<',
                selectvizrGridOptions: '<',
                selectvizrGridParams: '<?',
                selectvizrGridItemsSelected: '<?',
                selectvizrOnChange: '&'
            }
        });

})(window.angular);