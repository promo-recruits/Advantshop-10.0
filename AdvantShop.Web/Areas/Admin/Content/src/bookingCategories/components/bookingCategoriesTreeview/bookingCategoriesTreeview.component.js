; (function (ng) {
    'use strict';

    ng.module('bookingCategoriesTreeview')
        .component('bookingCategoriesTreeview', {
            templateUrl: '../areas/admin/content/src/bookingCategories/components/bookingCategoriesTreeview/templates/bookingCategoriesTreeview.html',
            controller: 'BookingCategoriesTreeviewCtrl',
            bindings: {
                categoryIdSelected: '@',
                affiliateId: '@',
                onInit: '&'
            }
        });

})(window.angular);