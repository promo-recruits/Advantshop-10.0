; (function (ng) {
    'use strict';


    ng.module('bookingSheduler')
        .component('bookingSheduler', {
            templateUrl: ['$element', '$attrs', function (element, attrs) {
                return attrs.templatePath || '../areas/admin/content/src/_shared/booking-sheduler/template/booking-sheduler.html';
            }],
            controller: 'BookingShedulerCtrl',
            bindings: {
                shedulerObj: '<',
                calendarOptions: '<',
                fetchUrl: '<?',
                fetchColumnUrl: '<?',
                shedulerOnInit: '&',
                extendCtrl: '<?',
                shedulerColumnDefs: '<?',
                shedulerOnFilterInit: '&',
                shedulerParams: '<?',
                uid: '@',
                shedulerScrollable: '<?',
                shedulerDraggable: '<?',
                shedulerColumnClasses: '<?',
                shedulerRowClasses: '<?',
                shedulerColumnWrapClasses: '<?',
                slotHeightPx: '<?',
                minSlotHeightPx: '<?',
                compactView: '<?',
                emptyText: '<?'
            }
        });
    
})(window.angular);