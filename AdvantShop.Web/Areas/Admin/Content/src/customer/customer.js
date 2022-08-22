; (function (ng) {
    'use strict';

    var CustomerCtrl = function ($http, SweetAlert, $window, toaster, $translate, $timeout) {

        var ctrl = this,
            timerSaveChanges,
            callbacksOnSave = [],
            timerParseStandartPhone,
            timerProcessAddress;

        ctrl.isProcessGetTags = true;

        ctrl.initCustomer = function (customerId, isEditMode, standardPhone, partnerId, orderId) {
            ctrl.instance = {};
            ctrl.instance.isEditMode = isEditMode;
            ctrl.instance.customerId = customerId;
            ctrl.instance.partnerId = partnerId;
            ctrl.instance.orderId = orderId;
            ctrl.instance.customer = {};
            ctrl.instance.customer.customerId = customerId;
            ctrl.instance.customer.standardPhone = standardPhone;
        };

        ctrl.processCity = function (zone) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }

            return timerProcessAddress = $timeout(function () {
                if (zone != null) {
                    ctrl.instance.customerContact.country = zone.Country;
                    ctrl.instance.customerContact.region = zone.Region;
                    ctrl.instance.customerContact.district = zone.District;
                    ctrl.instance.customerContact.zip = zone.Zip;
                }
                if (zone == null || !zone.Zip) {
                    ctrl.processCustomerContact(zone == null).then(function (data) {
                        if (data.result === true) {
                            ctrl.instance.customerContact.country = data.obj.Country;
                            ctrl.instance.customerContact.region = data.obj.Region;
                            ctrl.instance.customerContact.district = data.obj.District;
                            ctrl.instance.customerContact.zip = data.obj.Zip;
                        }
                    });
                }
            }, zone != null ? 0 : 700);
        };

        ctrl.processAddress = function (data) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }

            return timerProcessAddress = $timeout(function () {
                if (data != null && data.Zip) {
                    ctrl.instance.customerContact.zip = data.Zip;
                } else {
                    ctrl.processCustomerContact().then(function (data) {
                        if (data.result === true) {
                            ctrl.instance.customerContact.zip = data.obj.Zip;
                        }
                    });
                }
            }, data != null ? 0 : 700);
        };

        ctrl.processCustomerContact = function (byCity) {
            var contact = {
                country: ctrl.instance.customerContact.country,
                region: ctrl.instance.customerContact.region,
                district: ctrl.instance.customerContact.district,
                city: ctrl.instance.customerContact.city,
                zip: ctrl.instance.customerContact.zip,
                street: ctrl.instance.customerContact.street,
                house: ctrl.instance.customerContact.house,
                byCity: byCity
            };
            return $http.post('customers/processCustomerContact', contact).then(function (response) {
                return response.data;
            });
        };

        ctrl.delete = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Customer.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Customer.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('customers/deleteCustomer', { customerId: ctrl.instance.customer.customerId }).then(function (response) {
                        var data = response.data;
                        if (data) {
                            $window.location.assign('customers');
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.Customer.UserCantBeDeleted'));
                        }
                    });
                }
            });
        };

        ctrl.createOrderFromCart = function () {
            ctrl.addingOrder = false;
            $http.post('orders/addOrderFromCart', { customerId: ctrl.instance.customer.customerId }).then(function (response) {
                var data = response.data;
                if (data.result == true && response.data.obj != 0) {
                    toaster.pop("success", "", $translate.instant('Admin.Js.Customer.DraftOrder') + response.data.obj);
                    $window.location.assign('orders/edit/' + response.data.obj);
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.Customer.ErrorCreatingOrder'), data.errors != null ? '</br>' + data.errors.join('</br>') : '');
                    ctrl.addingOrder = false;
                }
            });
        };

        ctrl.saveCustomer = function (form) {
            //if (timerSaveChanges != null) {
            //    clearTimeout(timerSaveChanges);
            //}

            //timerSaveChanges = setTimeout(function () {
            ctrl.instance.tags = ctrl.selectedTags.map(function (obj) { return obj.value; });

            $http.post('customers/savePopup', ctrl.instance)
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Customer.ChangesSuccessfullySaved'));

                        if (!ctrl.instance.isEditMode && data.obj != null) {
                            if (ctrl.instance.partnerId != null) {
                                $window.location.assign('partners/view/' + ctrl.instance.partnerId + '?customerId=' + data.obj + '#?partnerTab=customers'); // customerId to reload page
                            } else {
                                $window.location.assign('customers/view/' + data.obj);
                            }
                        } else {
                            form.$setPristine();
                        }

                        if (callbacksOnSave.length > 0) {
                            for (var i = 0, len = callbacksOnSave.length; i < len; i++) {
                                callbacksOnSave[i]();
                            }
                        }

                    } else {
                        data.errors.forEach(function (e) {
                            toaster.pop('error', '', e);
                        });
                    }
                });
            //}, 500);
        };

        ctrl.parseStandartPhone = function (phone) {

            if (timerParseStandartPhone != null) {
                clearTimeout(timerParseStandartPhone);
            }

            timerParseStandartPhone = setTimeout(function () {
                $http.post('customers/getStandartPhone', { phone: phone })
                    .then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            ctrl.instance.customer.standardPhone = data.obj;
                        } else {
                            ctrl.instance.customer.standardPhone = null;
                        }
                    });
            }, 300);
        };


        ctrl.addCallbackOnSave = function (callback) {
            callbacksOnSave.push(callback);
        };

        ctrl.deleteVkLink = function() {
            SweetAlert.confirm($translate.instant('Admin.Js.Customer.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Customer.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('vk/deleteVkLink', { customerId: ctrl.instance.customer.customerId }).then(function (response) {
                        var data = response.data;
                        if (data) {
                            $window.location.reload(true);
                        }
                    });
                }
            });
        }

        ctrl.deleteOkLink = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Customer.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Customer.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('ok/deleteOkLink', { customerId: ctrl.instance.customer.customerId }).then(function (response) {
                        var data = response.data;
                        if (data) {
                            $window.location.reload(true);
                        }
                    });
                }
            });
        }

        //#region Tags
        ctrl.tagTransform = function (newTag) {
            return { value: newTag };
        };

        ctrl.getTags = function (form) {
            ctrl.isProcessGetTags = true;

            $http.get('customers/getTags', { params: { customerId: ctrl.instance.customer.customerId } })
                .then(function (response) {
                    ctrl.tags = response.data.tags;
                    ctrl.selectedTags = response.data.selectedTags;

                    return response.data;
                })
                .then(function (data) {
                    return $timeout(function () {
                        if(form != null)
                            form.$setPristine();
                        return data;
                    }, 0);
                })
                .then(function (data) {
                    return $timeout(function () {
                        ctrl.isProcessGetTags = false;
                        return data;
                    }, 500)
                });
        }
        //#endregion
    };

    CustomerCtrl.$inject = ['$http', 'SweetAlert', '$window', 'toaster', '$translate', '$timeout'];

    ng.module('customer', ['uiGridCustom', 'customerOrders', 'customerLeads', 'customerBookings'])
        .controller('CustomerCtrl', CustomerCtrl);

})(window.angular);