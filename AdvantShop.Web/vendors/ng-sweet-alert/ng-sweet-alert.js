/**
 *  * @description directive for sweet alert 
 * @author Tushar Borole
 * @createDate 18/04/2015
 * @version 1.0.3
 * @lastmodifiedDate 06/18/2015
 */



(function () {
    'use strict';


    // Check we have sweet alert js included
    if (angular.isUndefined(window.Sweetalert2)) {
        throw "Please inlcude sweet alert js and css from http://t4t5.github.io/sweetalert/";
    }

    var Swal = Swal || window.Sweetalert2;

    angular
        .module('ng-sweet-alert', [])
        .directive('sweetalert', sweetalert)
        .factory('SweetAlert', sweetalert_service);

    sweetalert.$inject = ['$parse'];

    /* @ngInject */
    function sweetalert($parse) {
        // Usage:
        //
        // Creates:
        //
        var directive = {
            link: link
        };
        return directive;

        function link(scope, element, attrs, controller) {
            var sweetElement = angular.element(element);
            sweetElement.click(function () {
                var sweetOptions = scope.$eval(attrs.sweetOptions);
                var sweetConfirmOption = scope.$eval(attrs.sweetConfirmOption);
                var sweetCancelOption = scope.$eval(attrs.sweetCancelOption);

                Swal.fire(sweetOptions).then(function (isConfirm) {
                    if (isConfirm) {
                        if (sweetConfirmOption) Swal.fire(sweetConfirmOption);
                        if (attrs.sweetOnConfirm) scope.$evalAsync(attrs.sweetOnConfirm);

                    } else {
                        if (sweetCancelOption) Swal.fire(sweetCancelOption);
                        if (attrs.sweetOnCancel) scope.$evalAsync(attrs.sweetOnCancel);
                    }
                });

            });

        }
    }

    // Use SweetAlert as service
    //
    // swal() gets two arguments;
    // first argument is parameters Objects (with default values).
    // second argument is Callback function when clicking on "OK"/"Cancel", which is a promise.
    // register to the promise (using 'then') and handle the resolve / reject according to your business logic.
    //
    // Add 'SweetAlert' to your directive / controller / ect)
    // Use SweetAlert.confirm(msg, options) / SweetAlert.alert(msg, options) / SweetAlert.info(msg, options) / SweetAlert.success(msg, options)
    // pass arguments:
    // msg; String - The message to be displayed in the alert / confirm box (mandatory).
    // options; Object (optinal):
    //   title: String - the title of the box.
    //   type: String - "warning" / "info" / "error" / "success" / "" (empty string will not display a graphic icon).
    //   showCancelButton: Boolean - shows the "cancel" button (true will behave like confirm dialog, false will behave like alert dialog).
    // Use returned promise;
    //
    // SweetAlert.confirm("Are you sure?", {title : "Careful now!"})
    //           .then(function(p) { do something on success },
    //                 function(p) { do something on fail }
    //           );
    //
    // SweetAlert.success("You have successfully completed our poll!", {title: "Good job!"});

    sweetalert_service.$inject = ['$q'];

    function sweetalert_service($q) {
        function swal_alert(message, options) {
            return swal_confirm(message, angular.extend({
                title: "Alert",
                html: message,
                icon: "warning",
                type: "warning",
                showCancelButton: false
            }, options));
        }

        function swal_info(message, options) {
            return swal_alert(message, angular.extend({
                icon: "info",
                type: "info"
            }, options));
        }

        function swal_success(message, options) {
            return swal_alert(message, angular.extend({
                icon: "success",
                type: "success"
            }, options));
        }

        function swal_error(message, options) {
            return swal_alert(message, angular.extend({
                icon: "error",
                type: "error"
            }, options));
        }

        function swal_confirm(message, options) {
            var defered = $q.defer();
            var optionsNew = angular.extend({
                title: "Alert",
                html: message,
                icon: "warning",
                type: "warning",
                showCancelButton: true
            }, options);

            Swal.fire(optionsNew)
                .then(function (r) {
                    defered.resolve(r);
                }, function (e) {
                    defered.reject(e);
                });

            return defered.promise;
        }
        return {
            alert: swal_alert,
            confirm: swal_confirm,
            info: swal_info,
            success: swal_success,
            error: swal_error
        };
    }

})();

