﻿; (function (ng) {
    'use strict';

    var PartnerCustomersCtrl = function ($http, uiGridCustomConfig, $translate, toaster) {
        var ctrl = this;

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'FullName',
                    displayName: $translate.instant('Admin.Js.PartnerCustomers.FullName'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> ' +
                            '<div class="m-l-xs"><a href="customers/view/{{row.entity.CustomerId}}" target="_blank">{{row.entity.FullName}}</a></div> ' +
                        '</div>'
                },
                {
                    name: 'Email',
                    displayName: $translate.instant('Admin.Js.PartnerCustomers.Email')
                },
                {
                    name: 'Phone',
                    displayName: $translate.instant('Admin.Js.PartnerCustomers.Phone')
                },
                {
                    name: 'Location',
                    displayName: $translate.instant('Admin.Js.PartnerCustomers.Location')
                },
                {
                    name: 'PaidOrdersCount',
                    displayName: $translate.instant('Admin.Js.PartnerCustomers.PaidOrdersCount')
                },
                {
                    name: 'PaidOrdersSumFormatted',
                    displayName: $translate.instant('Admin.Js.PartnerCustomers.PaidOrdersSum')
                },
                {
                    name: 'DateCreatedFormatted',
                    displayName: $translate.instant('Admin.Js.PartnerCustomers.DateCreated')
                },
                {
                    name: '_serviceColumnDetails',
                    displayName: '',
                    width: 35,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked">' +
                            '<ui-modal-trigger ng-if="row.entity.HasDetails" class="dropdown-menu-link js-menu-link" ' +
                                'controller="grid.appScope.$ctrl.gridExtendCtrl.modalBindedCustomerDetails" ' +
                                'data-resolve="{params: {data: row.entity}}" ' +
                                'template-url="../areas/admin/content/src/partner/modals/bindedCustomerDetails/bindedCustomerDetails.html">' +
                                '<a href="" title="Подробнее" class="ui-grid-custom-service-icon fa fa-eye link-invert"></a>' +
                            '</ui-modal-trigger>' +
                        '</div></div>'
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 35,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked">' +
                            '<ui-grid-custom-delete url="partners/unbindCustomer" params="{customerId: row.entity.CustomerId }"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ],
        });

        ctrl.bindCustomer = function (customer) {
            if (customer == null || customer.customerId == null) {
                return false;
            }
            $http.post('partners/bindCustomer', { partnerId: ctrl.partnerId, customerId: customer.customerId }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.gridPartnerCustomers.fetchData();
                    if (ctrl.onBindCustomer) {
                        ctrl.onBindCustomer();
                    }
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.modalBindedCustomerDetails = function () {
            var detailsCtrl = this;

            detailsCtrl.$onInit = function () {
                detailsCtrl.data = detailsCtrl.$resolve.params.data;
            };
        };

    };

    PartnerCustomersCtrl.$inject = ['$http', 'uiGridCustomConfig', '$translate', 'toaster'];

    ng.module('partnerCustomers', ['uiGridCustom'])
        .controller('PartnerCustomersCtrl', PartnerCustomersCtrl)
        .component('partnerCustomers', {
            templateUrl: '../areas/admin/content/src/partner/components/partnerCustomers/partnerCustomers.html',
            controller: PartnerCustomersCtrl,
            bindings: {
                partnerId: '<?',
                onBindCustomer: '&'
            }
      });

})(window.angular);