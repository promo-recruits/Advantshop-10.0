; (function (ng) {
    'use strict';

    var ShippingMethodCtrl = function ($location, $window, $uibModal, toaster, SweetAlert, $http, Upload, $translate) {

        var ctrl = this;

        ctrl.init = function(methodId, icon) {
            ctrl.methodId = methodId;
            ctrl.icon = icon;
            ctrl.inputTypeLogin = 'password';
            ctrl.inputTypePassword = 'password';
            ctrl.inputTypeApiKey = 'password';
            ctrl.getAvailableLocations();
            ctrl.getExcludedLocations();
            ctrl.getExcludedByCatalog();
            ctrl.getPayments();
        }

        ctrl.isNoExceptions = function() {
            return (ctrl.ExcludedCities == null || ctrl.ExcludedCities.length === 0) &&
                (ctrl.ExcludedCountry == null || ctrl.ExcludedCountry.length === 0) &&
                (!ctrl.CountExcludedProducts) &&
                (ctrl.ExcludedCategories == null || ctrl.ExcludedCategories.length === 0);
        }

        ctrl.getAvailableLocations = function() {
            $http.get('shippingMethods/getAvailableLocations', { params: { methodId: ctrl.methodId } })
                .then(function(response) {

                    var data = response.data;
                    ctrl.AvailableCountries = data.countries;
                    ctrl.AvailableRegions = data.regions;
                    ctrl.AvailableCities = data.cities;
                });
        }

        ctrl.getExcludedLocations = function () {
            $http.get('shippingMethods/getExcludedLocations', { params: { methodId: ctrl.methodId } })
                .then(function (response) {

                    var data = response.data;
                    ctrl.ExcludedCities = data.cities;
                    ctrl.ExcludedRegions = data.regions;
                    ctrl.ExcludedCountry = data.country;
                });
        }

        ctrl.getExcludedByCatalog = function () {
            $http.get('shippingMethods/getExcludedByCatalog', { params: { methodId: ctrl.methodId } })
                .then(function (response) {

                    var data = response.data;
                    ctrl.CountExcludedProducts = data.countProducts;
                    ctrl.ExcludedCategories = data.categories;
                });
        }

        ctrl.selectExcludedCategories = function (result) {
            ctrl.ExcludedCategories = result.categoryIds;
            ctrl.saveExcludedCategories();
        }

        ctrl.selectExcludedProducts = function () {
            ctrl.getExcludedByCatalog();
        }

        ctrl.resetExcludedCategories = function () {
            ctrl.ExcludedCategories = [];
            ctrl.saveExcludedCategories();
        }

        ctrl.resetExcludedProducts = function () {
            ctrl.resetExcludedProducts();
        }

        ctrl.saveExcludedCategories = function() {
            $http.post('shippingMethods/saveExcludedCategories', { methodId: ctrl.methodId, excludedCategories: ctrl.ExcludedCategories })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    }
                }).then(ctrl.getExcludedByCatalog);
        }

        ctrl.resetExcludedProducts = function() {
            $http.post('shippingMethods/resetExcludedProducts', { methodId: ctrl.methodId })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    }
                }).then(ctrl.getExcludedByCatalog);
        }

        ctrl.deleteAvailableCountry = function(countryId) {
            $http.post('shippingMethods/deleteAvailableCountry', { methodId: ctrl.methodId, countryId: countryId })
                .then(function(response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    }
                }).then(ctrl.getAvailableLocations);
        }

        ctrl.deleteAvailableCity = function (cityId) {
            $http.post('shippingMethods/deleteAvailableCity', { methodId: ctrl.methodId, cityId: cityId })
                .then(function(response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    }
                }).then(ctrl.getAvailableLocations);
        }

        ctrl.deleteExcludedCity = function (cityId) {
            $http.post('shippingMethods/deleteExcludedCity', { methodId: ctrl.methodId, cityId: cityId })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    }
                }).then(ctrl.getExcludedLocations);
        }

        ctrl.deleteAvailableRegion = function (regionId) {
            $http.post('shippingMethods/deleteAvailableRegion', { methodId: ctrl.methodId, regionId: regionId })
                .then(function(response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    }
                }).then(ctrl.getAvailableLocations);
        }

        ctrl.deleteExcludedRegion = function (regionId) {
            $http.post('shippingMethods/deleteExcludedRegion', { methodId: ctrl.methodId, regionId: regionId })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    }
                }).then(ctrl.getExcludedLocations);
        }

        ctrl.deleteExcludedCountry = function (CountryID) {
            $http.post('shippingMethods/DeleteExcludedCountry', { methodId: ctrl.methodId, CountryId: CountryID })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    }
                }).then(ctrl.getExcludedLocations);
        }


        ctrl.addAvailableCountry = function () {
            $http.post('shippingMethods/addAvailableCountry', { methodId: ctrl.methodId, countryName: ctrl.newAvailableCountry })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.newAvailableCountry = '';
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ShippingMethods.CanNotAdd') + ctrl.newAvailableCountry + '" ');
                    }
                }).then(ctrl.getAvailableLocations);
        }

        ctrl.selectAvailableCity = function (item) {
            if (item == null) return;
            ctrl.newAvailableCity = {
                id: item.CityId,
                name: item.City + (item.District && item.District.length ? ', ' + item.District : '') + (item.Region && item.Region.length ? ' (' + item.Region + ')' : '')
            };
        };

        ctrl.addAvailableCity = function () {
            $http.post('shippingMethods/addAvailableCity', { methodId: ctrl.methodId, cityName: ctrl.newAvailableCity.name, cityId: ctrl.newAvailableCity.id })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.newAvailableCity = {};
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ShippingMethods.CanNotAdd') + ctrl.newAvailableCity.name + '" ');
                    }
                }).then(ctrl.getAvailableLocations);
        }

        ctrl.selectExcludedCity = function (item) {
            if (item == null) return;
            ctrl.newExcludedCity = {
                id: item.CityId,
                name: item.City + (item.District && item.District.length ? ', ' + item.District : '') + (item.Region && item.Region.length ? ' (' + item.Region + ')' : '')
            };
        };

        ctrl.addExcludedCity = function () {
            $http.post('shippingMethods/AddExcludedCity', { methodId: ctrl.methodId, cityName: ctrl.newExcludedCity.name, cityId: ctrl.newExcludedCity.id })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.newExcludedCity = {};
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ShippingMethods.CanNotAdd') + ctrl.newExcludedCity.name + '" ');
                    }
                }).then(ctrl.getExcludedLocations);
        };

        ctrl.selectAvailableRegion = function (item) {
            if (item == null) return;
            ctrl.newAvailableRegion = {
                id: item.RegionId,
                name: item.Region + (item.CountryName && item.CountryName.length ? ' (' + item.CountryName + ')' : '')
            };
        };

        ctrl.addAvailableRegion = function () {
            $http.post('shippingMethods/addAvailableRegion', { methodId: ctrl.methodId, regionName: ctrl.newAvailableRegion.name, regionId: ctrl.newAvailableRegion.id })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.newAvailableRegion = {};
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ShippingMethods.CanNotAdd') + ctrl.newAvailableRegion.name + '" ');
                    }
                }).then(ctrl.getAvailableLocations);
        }

        ctrl.selectExcludedRegion = function (item) {
            if (item == null) return;
            ctrl.newExcludedRegion = {
                id: item.RegionId,
                name: item.Region + (item.CountryName && item.CountryName.length ? ' (' + item.CountryName + ')' : '')
            };
        };

        ctrl.addExcludedRegion = function () {
            $http.post('shippingMethods/AddExcludedRegion', { methodId: ctrl.methodId, regionName: ctrl.newExcludedRegion.name, regionId: ctrl.newExcludedRegion.id })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.newExcludedRegion = {};
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ShippingMethods.CanNotAdd') + ctrl.newExcludedRegion.name + '" ');
                    }
                }).then(ctrl.getExcludedLocations);
        };

        ctrl.addExcludedCountry = function () {
            $http.post('shippingMethods/AddExcludedCountry', { methodId: ctrl.methodId, countryName: ctrl.newExcludedCountry })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.newExcludedCountry = '';
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ShippingMethods.CanNotAdd') + ctrl.newExcludedCountry + '" ');
                    }
                }).then(ctrl.getExcludedLocations);
        }

        ctrl.findCity = function(val) {
            return $http.get('cities/getCitiesAutocompleteExt', { params: { q: val, rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };

        ctrl.findRegion = function(val) {
            return $http.get('regions/getRegionsAutocompleteExt', { params: { q: val, rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };

        ctrl.getPayments = function () {
            $http.get('shippingMethods/getPayments', { params: { methodId: ctrl.methodId } })
                .then(function (response) {
                    ctrl.payments = response.data;
                    ctrl.selectedPaymentMethods =
                        ctrl.payments != null
                            ? ctrl.payments.filter(function (x) { return x.Active === true; }).map(function(x) { return x.PaymentMethodId; })
                            : null;
                });
        }


        ctrl.uploadIcon = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                ctrl.sendIcon($file);
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.ShippingMethods.ErrorLoading'), $translate.instant('Admin.Js.ShippingMethods.FileNotMeetRquirements'));
            }
        };

        ctrl.deleteIcon = function () {

            SweetAlert.confirm($translate.instant('Admin.Js.ShippingMethods.AreYouSureDelete'), { title: $translate.instant('Admin.Js.ShippingMethods.Deleting') })
                .then(function (result) {
                    if (result === true) {
                        return $http.post('shippingMethods/deleteIcon', { methodId: ctrl.methodId }).then(function (response) {
                            var data = response.data;
                            if (data.result === true) {
                                ctrl.icon = null;
                                toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ImageDeleted'));
                            } else {
                                toaster.pop('error', $translate.instant('Admin.Js.ShippingMethods.ErrorDeleting'), data.error);
                            }
                        });
                    }
                });
        };

        ctrl.sendIcon = function (file) {
            return Upload.upload({
                url: 'shippingMethods/uploadIcon',
                data: {
                    file: file,
                    methodId: ctrl.methodId,
                    rnd: Math.random(),
                }
            }).then(function (response) {
                var data = response.data;

                if (data.Result === true) {
                    ctrl.icon = data.Picture;
                    toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ImageSaved'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.ShippingMethods.ErrorLoading'), data.error);
                }
            });
        }


        ctrl.deleteMethod = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.ShippingMethods.AreYouSureDelete'), { title: $translate.instant('Admin.Js.ShippingMethods.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('shippingMethods/deleteMethod', { methodId: ctrl.methodId }).then(function (response) {
                        $window.location.assign('settings/shippingMethods');
                    });
                }
            });
        }


        /* sdek */
        ctrl.callSdekCourier = function () {

            var params = ctrl.sdek;
            $http.post('shippingMethods/callSdekCourier', params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', data.msg);
                } else {
                    if (typeof data.msg === 'string') {
                        toaster.pop('error', '', data.msg);
                    } else if (Object.prototype.toString.call(data.msg) === '[object Array]') {
                        for (var i = 0; i < data.msg.length; i++) {
                            toaster.pop('error', '', data.msg[i]);
                        }
                    }
                }
            });
        }


        ctrl.openSdekRegistration = function () {
            $uibModal.open({
                bindToController: true,
                //controller: 'ShippingMethodCtrl',
                //controllerAs: 'ctrl',
                size: "lg",
                templateUrl: '../areas/admin/content/src/shippingMethods/modal/sdekRegistration/SdekRegistration.html',
                //resolve: {
                //    msg: function () { return data.msg; },
                //    title: function () { return data.result === true ? $translate.instant('Admin.Js.PriceRegulation.RegulationOfPrices') : $translate.instant('Admin.Js.PriceRegulation.Error'); }
                //},
                backdrop: 'static'
            });
        }


    };

    ShippingMethodCtrl.$inject = ['$location', '$window', '$uibModal', 'toaster', 'SweetAlert', '$http', 'Upload', '$translate'];


    ng.module('shippingMethod', ['checklist-model', 'angular-inview'])
      .controller('ShippingMethodCtrl', ShippingMethodCtrl);

})(window.angular);