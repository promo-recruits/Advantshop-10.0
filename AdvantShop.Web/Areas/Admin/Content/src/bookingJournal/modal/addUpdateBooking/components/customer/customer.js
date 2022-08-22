; (function(ng) {
    'use strict';

    var BookingCustomerCtrl = function ($http, $q, toaster, $timeout) {
        var ctrl = this;

        ctrl.$onInit = function() {
            if (ctrl.params.customerId) {

                ctrl.getCustomer(ctrl.params.customerId).then(function () {
                    if (ctrl.params.phone) {
                        ctrl.customer.phone = ctrl.params.phone;
                    }
                    ctrl.changedCustomer();
                });

            } else {

                if (ctrl.params.phone) {
                    ctrl.customer.phone = ctrl.params.phone;
                }
            }

            if (ctrl.onInit != null) {
                ctrl.onInit({ bookingCustomer: ctrl});
            }
        };

        ctrl.findCustomer = function (val) {
            if (ctrl.mode === 'add' || !ctrl.customer.customerId) {
                return $http.get('customers/getCustomersAutocomplete', { params: { q: val, rnd: Math.random() } }).then(function (response) {
                    return response.data;
                });
            }
            return null;
        };

        ctrl.selectCustomer = function (result) {
            if (result != null) {
                ctrl.getCustomer(result.customerId || result.CustomerId)
                    .then(function (result) {

                        ctrl.changedCustomer();
                        return result || $q.reject('error');
                    });
            }
        };

        ctrl.clearCustomer = function () {
            ctrl.customer.lastName = null;
            ctrl.customer.firstName = null;
            ctrl.customer.patronymic = null;
            ctrl.customer.organization = null;
            ctrl.customer.phone = null;
            ctrl.customer.standardPhone = null;
            ctrl.customer.email = null;
            ctrl.customer.birthday = null;
            ctrl.customer.customerId = null;

            ctrl.changedCustomer();
        };

        ctrl.changedCustomer = function () {
            ctrl.getCustomerFields();
            ctrl.getCustomerSocial();
            $timeout(function () {
                if (ctrl.bookingEvents) {
                    ctrl.bookingEvents.getLeadEvents();
                }
            });
        };

        ctrl.getCustomerSocial = function () {
            $http.get("booking/getCustomerSocial", { params: { customerId: ctrl.customer.customerId } }).then(function (response) {
                ctrl.customer.social = response.data;
            });
        };

        ctrl.callGetgCustomerFields = function () {
            return ctrl.getCustomerFields();
        };

        ctrl.getCustomerFields = function () {
            return ctrl.getFunctionGetCustomerFields().then(function () { return ctrl.getCustomerFieldsFn(); });
        };

        ctrl.onCustomerFieldsInit = function (reloadFn) {
            ctrl.getCustomerFieldsFn = reloadFn || function () { };
            if (ctrl.functionGetCustomerFieldsPromise) {
                ctrl.functionGetCustomerFieldsPromise.resolve();
            }
        };

        ctrl.getFunctionGetCustomerFields = function () {
            if (ctrl.getCustomerFieldsFn) {
                return $q.resolve();
            } else {
                ctrl.functionGetCustomerFieldsPromise = ctrl.functionGetCustomerFieldsPromise
                    ? ctrl.functionGetCustomerFieldsPromise
                    : $q.defer();
                return ctrl.functionGetCustomerFieldsPromise.promise;
            }
        };

        ctrl.getCustomer = function (customerId) {
            if (customerId == null) {
                return $q.defer().resolve(false);
            }

            return $http.get("customers/getCustomerWithContact", { params: { customerId: customerId } }).then(function (response) {

                var customer = response.data;

                if (customer == null) return false;

                ctrl.customer.customerId = customer.Id;
                ctrl.customer.firstName = customer.FirstName;
                ctrl.customer.lastName = customer.LastName;
                ctrl.customer.patronymic = customer.Patronymic;
                ctrl.customer.organization = customer.Organization;
                ctrl.customer.email = customer.Email;
                ctrl.customer.phone = customer.Phone;
                ctrl.customer.standardPhone = customer.StandardPhone;
                ctrl.customer.birthday = customer.BirthDay;

                return true;
            });
        };

        ctrl.addSocialUser = function(type, link) {

            if (ctrl.btnSocialAdding != null)
                return;

            var url = '';
            switch (type) {
            case 'vk':
                url = 'vk/addVkUser';
                break;
            case 'facebook':
                url = 'facebook/addFacebookUser';
                break;
            case 'instagram':
                url = 'instagram/addInstagramUser';
                break;
            }

            ctrl.btnSocialAdding = type;

            $http.post(url, { customerId: ctrl.customer.customerId, link: link }).then(function(response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', 'Изменения сохранены');
                    ctrl.getCustomerSocial();
                } else {
                    if (data.errors != null) {
                        data.errors.forEach(function(error) {
                            toaster.pop('error', '', error);
                        });
                    } else {
                        toaster.pop('error', 'Ошибка при сохранении');
                    }
                }
            }).finally(function() {
                ctrl.btnSocialAdding = null;
            });
        };

        ctrl.filterEvents = function (filterBy) {
            ctrl.bookingEvents.filterType = filterBy;
        };

        ctrl.updateBookingEvents = function() {
            ctrl.bookingEvents.getLeadEvents();
        };

        ctrl.updateBookingEventsWithDelay = function() {
            setTimeout(ctrl.updateBookingEvents, 800);
        };
    };

    BookingCustomerCtrl.$inject = ['$http', '$q', 'toaster', '$timeout'];

    ng.module('bookingCustomer', [])
        .controller('BookingCustomerCtrl', BookingCustomerCtrl)
        .component('bookingCustomer', {
            templateUrl: '../areas/admin/content/src/bookingJournal/modal/addUpdateBooking/components/customer/customer.html',
            controller: 'BookingCustomerCtrl',
            bindings: {
                onInit: '&',
                params: '<?',
                customer: '<',
                mode: '<',
                canBeEditing: '<?',
                bookingEvents: '<'
            }
        });
})(window.angular);