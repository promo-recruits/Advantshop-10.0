; (function (ng) {
    'use strict';

    var BookingCategoriesCtrl = function() {
        var ctrl = this;
    };

    BookingCategoriesCtrl.$inject = [];

    ng.module("bookingCategories", ["listBookingCategories"])
        .controller("BookingCategoriesCtrl", BookingCategoriesCtrl);

})(window.angular);