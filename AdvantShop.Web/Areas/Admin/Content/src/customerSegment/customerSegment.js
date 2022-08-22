; (function (ng) {
    'use strict';

    var CustomerSegmentCtrl = function ($http, $window, SweetAlert, uiGridCustomConfig, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.CustomerSegments.Customer'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                            '<a ng-href="customers/view/{{row.entity.CustomerId}}">' +
                                '{{row.entity.Organization != null && row.entity.Organization.length > 0 ? row.entity.Organization : row.entity.Name }}' +
                            '</a>' +
                        '</div>',
                },
                {
                    name: 'Phone',
                    displayName: $translate.instant('Admin.Js.CustomerSegment.Phone'),
                },
                {
                    name: 'Email',
                    displayName: 'Email',
                },
                {
                    name: 'OrdersCount',
                    displayName: $translate.instant('Admin.Js.CustomerSegment.NumberOfPaidOrders'),
                    width: 150,
                    type: 'number',
                },
                {
                    name: 'RegistrationDateTimeFormatted',
                    displayName: $translate.instant('Admin.Js.CustomerSegment.DateOfRegistration'),
                    width: 150,
                }
            ];
        
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowUrl: 'customers/view/{{row.entity.CustomerId}}'
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };


        ctrl.deleteSegment = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.CustomerSegments.AreYouSureDelete'), { title: $translate.instant('Admin.Js.CustomerSegments.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('customerSegments/deleteSegment', { id: id }).then(function (response) {
                        $window.location.assign('customers#?customersTab=segments');
                    });
                }
            });
        };

        ctrl.initCategories = function (selectedCategories) {
            ctrl.selectedCategories = selectedCategories === null || selectedCategories == '' ? null : JSON.parse(selectedCategories);
            ctrl.getCategories();
        };

        ctrl.getCategories = function () {
            $http.get('customerSegments/getCategories').then(function (response) {
                ctrl.Categories = response.data.categories;
            });
        };

        ctrl.initCities = function (selectedCities) {
            ctrl.selectedCities = selectedCities === null || selectedCities == '' ? null : JSON.parse(selectedCities);
            ctrl.getCities();
        };

        ctrl.getCities = function () {
            $http.get('customerSegments/getCities').then(function (response) {
                ctrl.Cities = response.data.cities;
            });
        };

        ctrl.initCountries = function (selectedCountries) {
            ctrl.selectedCountries = selectedCountries === null || selectedCountries == '' ? null : JSON.parse(selectedCountries);
            ctrl.getCountries();
        };

        ctrl.getCountries = function () {
            $http.get('customerSegments/getCountries').then(function (response) {
                ctrl.Countries = response.data.countries;
            });
        };

        ctrl.export = function () {
            ctrl.grid.export();
        };

        ctrl.getCustomerIds = function (segmentId) {
            $http.get('customerSegments/getCustomerIdsBySegment', { params: { id: segmentId, itemsPerPage: 1000000 } }).then(function (response) {
                var data = response.data;
                if (data != null && data.DataItems != null) {
                    ctrl.customerIds = data.DataItems.map(function (x) { return x.CustomerId; });
                }
            });
        };

        ctrl.sendSmsNotEnabled = function () {
            SweetAlert
                .confirm($translate.instant('Admin.Js.CustomerSegment.SmsModuleIsNotConnected') + '<br/>' + $translate.instant('Admin.Js.CustomerSegments.YouCan') + '<a href="modules/market" target="_blank">' + $translate.instant('Admin.Js.CustomerSegment.ConnectTheModule') + '</a>' + $translate.instant('Admin.Js.CustomerSegment.SmsInforming'), { title: "" })
                .then(function (result) {
                });
        };
    };

    CustomerSegmentCtrl.$inject = ['$http', '$window', 'SweetAlert', 'uiGridCustomConfig', '$translate'];

    ng.module('customerSegment', ['uiGridCustom'])
      .controller('CustomerSegmentCtrl', CustomerSegmentCtrl);

})(window.angular);