/* @ngInject */
function AddressListCtrl($http, $q, $timeout, addressService, zoneService, modalService) {
    var ctrl = this,
        timerChange,
        processContactTimer;
    
    ctrl.$onInit = function (){
        ctrl.items = [];
        ctrl.form = {};
        ctrl.fields = null;
        ctrl.isLoaded = false;
        
        modalService.getModal('modalAddress').then(function (modal) {
            ctrl.formModal = modal.modalScope._form;
        });

        addressService.getAddresses().then(function (response) {

            ctrl.items = [];

            if (response != null && response !== '' && response.length > 0) {
                ctrl.items.push(response[0]);
            }

            //ng.extend(ctrl.items, response);
            ctrl.addressSelected = ctrl.items[0];

            ctrl.initAddressFn({
                address: ctrl.addressSelected
            });

            ctrl.isLoaded = true;
        });
    }
    
    ctrl.isModalRendered = function (){
        return modalService.hasModal('modalAddress');
    }

    ctrl.change = function (address) {

        if (timerChange != null) {
            clearTimeout(timerChange);
        }

        timerChange = setTimeout(function () {
            ctrl.changeAddressFn({
                address: ctrl.addressSelected
            });
        }, 600);
    };

    ctrl.add = function () {
        ctrl.clearFormData();
        ctrl.buildModal();

        if (ctrl.isModalRendered() === false) {
            addressService.dialogRender('addressList.modalCallbackClose(modalScope)', ctrl);
        } else {
            addressService.dialogOpen();
        }
    };

    ctrl.edit = function (item) {

        ctrl.form.contactId = item.ContactId;
        ctrl.form.fio = item.Name;
        ctrl.form.firstName = item.FirstName;
        ctrl.form.lastName = item.LastName;
        ctrl.form.patronymic = item.Patronymic;
        ctrl.form.countryId = item.CountryId;
        //ctrl.form.country = item.Country; //set in ctrl.buildModal()
        ctrl.form.region = item.Region;
        ctrl.form.city = item.City;
        ctrl.form.district = item.District;
        ctrl.form.zip = item.Zip;
        ctrl.form.street = item.Street;
        ctrl.form.house = item.House;
        ctrl.form.apartment = item.Apartment;
        ctrl.form.structure = item.Structure;
        ctrl.form.entrance = item.Entrance;
        ctrl.form.floor = item.Floor;

        ctrl.buildModal().then(function () {
            if (ctrl.isModalRendered() === false) {
                addressService.dialogRender('addressList.modalCallbackClose', ctrl);
            } else {
                addressService.dialogOpen();
            }
        });
    };

    //ctrl.remove = function (contactId, index) {
    //    addressService.removeAddress(contactId).then(function (response) {
    //        if (response === true) {
    //            ctrl.items.splice(index, 1);
    //        }
    //    });
    //};

    ctrl.buildModal = function () {
        return addressService.getFields(ctrl.isShowName)
            .then(function (response) {
                return ctrl.fields = ctrl.fields || response;
            })
            .then(function (fields) {
                if (!fields.IsShowFullAddress) {
                    ctrl.clearFullAddressData();
                }
                ctrl.initListsMaxHeight();
                if (fields.IsShowCountry === true) {
                    return ctrl.getCountries();
                }
            })
            .then(function (countries) {
                if (countries != null) {
                    return ctrl.getSelectedCountry(countries);
                }
            });
    };

    ctrl.getCountries = function () {

        var countriesDefer = $q.defer(),
            countriesPromise;

        if (ctrl.form.countries != null) {
            countriesPromise = countriesDefer.promise;
            countriesDefer.resolve(ctrl.form.countries);
        } else {
            countriesPromise = $http.get('location/GetCountries').then(function (response) { return ctrl.form.countries = response.data; });
        }

        return countriesPromise;
    };

    ctrl.getSelectedCountry = function (countries) {

        return $q.when(ctrl.form.countryId != null && ctrl.form.countryId !== 0 ? { CountryId: ctrl.form.countryId } : zoneService.getCurrentZone()).then(function (zone) {

            var country;

            for (var i = countries.length - 1; i >= 0; i--) {
                if (countries[i].CountryId === zone.CountryId) {
                    country = countries[i];
                    break;
                }
            }

            return ctrl.form.country = country;
        });
    };

    ctrl.save = function () {
        var obj = ctrl.getObjectForUpdate();

        addressService.addUpdateCustomerContact(obj).then(function (response) {
            var editContact, needSelect;

            if (response !== null) {

                if (obj.ContactId != null) {
                    //editContact = ctrl.items.filter(function (item) { return item.ContactId == response.ContactId })[0];
                    //ng.extend(editContact, response);
                } else {

                    needSelect = ctrl.items.length === 0;

                    ctrl.items.push(response);

                    if (needSelect === true) {
                        ctrl.addressSelected = ctrl.items[0];
                    }
                }

                addressService.getAddresses().then(function (response) {
                    ctrl.items[0] = response[0];
                    ctrl.addressSelected = ctrl.items[0];

                    addressService.dialogClose();
                    ctrl.clearFormData();

                    ctrl.saveAddressFn({
                        address: ctrl.addressSelected
                    });
                });

            }
        });
    };

    ctrl.clearFormData = function () {
        ctrl.form.contactId = null;
        ctrl.form.fio = null;
        ctrl.form.firstName = null;
        ctrl.form.lastName = null;
        ctrl.form.patronymic = null;
        ctrl.form.countryId = null;
        ctrl.form.country = null;
        ctrl.form.region = null;
        ctrl.form.city = null;
        ctrl.form.district = null;
        ctrl.form.street = null;
        ctrl.form.zip = null;
        if (ctrl.formModal != null) {
            ctrl.formModal.$setPristine();
        }
    };

    ctrl.clearFullAddressData = function () {
        ctrl.form.house = '';
        ctrl.form.apartment = '';
        ctrl.form.structure = '';
        ctrl.form.entrance = '';
        ctrl.form.floor = '';
    };

    ctrl.initListsMaxHeight = function () {
        ctrl.addressListMaxHeight = (50 * (ctrl.fields.IsShowCity + ctrl.fields.IsShowDistrict + ctrl.fields.IsShowState + ctrl.fields.IsShowCountry + ctrl.fields.IsShowZip)) || 50;
        if (!ctrl.fields.IsShowFullAddress) {
            ctrl.citiesListMaxHeight = (50 * (ctrl.fields.IsShowState + ctrl.fields.IsShowCountry + ctrl.fields.IsShowZip + 2 * ctrl.fields.IsShowAddress)) || 50;
        }
    };

    ctrl.modalCallbackClose = function (modal) {
        ctrl.clearFormData();
    };

    ctrl.getObjectForUpdate = function () {

        var form = ctrl.form,
            account = {};

        if (form.contactId) {
            account.ContactId = form.contactId;
        }

        if (form.fio) {
            account.Fio = form.fio;
        }

        if (form.firstName) {
            account.FirstName = form.firstName;
        }
        if (form.lastName) {
            account.LastName = form.lastName;
        }
        if (form.patronymic) {
            account.Patronymic = form.patronymic;
        }

        if (form.country) {
            account.CountryId = form.country.CountryId;
            account.Country = form.country.Name;
        }

        if (form.region) {
            account.Region = form.region;
        }

        if (form.district) {
            account.District = form.district;
        }

        if (form.city) {
            account.City = form.city;
        }

        if (form.zip) {
            account.Zip = form.zip;
        }

        account.Street = form.street;
        account.House = form.house;
        account.Apartment = form.apartment;
        account.Structure = form.structure;
        account.Entrance = form.entrance;
        account.Floor = form.floor;
        
        account.IsShowName = ctrl.isShowName;

        return account;
    };

    ctrl.processCity = function (zone, timeout) {
        if (processContactTimer != null) {
            $timeout.cancel(processContactTimer);
        }

        return processContactTimer = $timeout(function () {
            if (zone != null) {
                ctrl.form.region = zone.Region;
                ctrl.form.district = zone.District;
                ctrl.form.countryId = zone.CountryId;
                ctrl.form.zip = zone.Zip;
            }
            ctrl.form.byCity = zone == null;

            addressService.processAddress(ctrl.form).then(function (data) {
                if (data.result === true) {
                    ctrl.form.countryId = data.obj.CountryId;
                    ctrl.form.region = data.obj.Region;
                    ctrl.form.district = data.obj.District;
                    ctrl.form.zip = data.obj.Zip;
                    ctrl.getSelectedCountry(ctrl.form.countries);
                }
            });
        }, timeout != null ? timeout : 700);
    };

    ctrl.processAddress = function (data, timeout) {
        if (!ctrl.fields.UseAddressSuggestions) {
            return;
        }

        if (processContactTimer != null) {
            $timeout.cancel(processContactTimer);
        }

        return processContactTimer = $timeout(function () {
            ctrl.form.byCity = false;
            if (data != null && data.Zip) {
                ctrl.form.zip = data.Zip;
            } else {
                addressService.processAddress(ctrl.form).then(function (data) {
                    if (data.result === true) {
                        ctrl.form.zip = data.obj.Zip;
                    }
                });
            }
        }, timeout != null ? timeout : 700);
    };
    
    ctrl.processCountry = function(){
        ctrl.form.countryId = ctrl.form.country.CountryId;
    };
};

export default AddressListCtrl;