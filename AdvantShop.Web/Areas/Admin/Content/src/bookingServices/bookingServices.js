;(function(ng) {
    'use strict';

    var BookingServicesCtrl = function ($http, $q, uiGridConstants, uiGridCustomConfig, SweetAlert, toaster, $uibModal, $translate) {
        var ctrl = this;
        ctrl.gridServicesInited = false;
        ctrl.jstreeCategoriesInited = false;

        var columnDefsServices = [
            {
                name: 'ArtNo',
                displayName: $translate.instant('Admin.Js.BookingServices.ArtNo'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                width: 80,
                filter: {
                    placeholder: $translate.instant('Admin.Js.BookingServices.ArtNo'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'ArtNo'
                }
            },
            {
                name: 'PhotoSrc',
                headerCellCalss: 'ui-grid-custom-header-cell-center',
                displayName: $translate.instant('Admin.Js.BookingServices.Photo'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="ui-grid-custom-flex-center ui-grid-custom-link-for-img">' +
                    '<img ng-if="COL_FIELD != null && COL_FIELD.length > 0" class="ui-grid-custom-col-img" ng-src="{{COL_FIELD}}"></span></div>',
                width: 80,
                enableSorting: false,
                visible: 1441,
                filter: {
                    placeholder: $translate.instant('Admin.Js.BookingServices.Photo'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'HasPhoto',
                    selectOptions: [{ label: $translate.instant('Admin.Js.BookingServices.WithPhoto'), value: true }, { label: $translate.instant('Admin.Js.BookingServices.WithoutPhoto'), value: false }]
                }
            },
            {
                name: 'Name',
                displayName: $translate.instant('Admin.Js.BookingServices.Name'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                //filter: {
                //    placeholder: 'Название',
                //    type: uiGridConstants.filter.INPUT,
                //    name: 'Name'
                //}
            },
            {
                name: 'PriceString',
                displayName: $translate.instant('Admin.Js.BookingServices.Price'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                width: 100
            },
            {
                name: 'SortOrder',
                displayName: $translate.instant('Admin.Js.BookingServices.Order'),
                type: 'number',
                enableCellEdit: true,
                width: 80,
                filter: {
                    placeholder: $translate.instant('Admin.Js.BookingServices.Sorting'),
                    type: 'range',
                    rangeOptions: {
                        from: {
                            name: 'SortingFrom'
                        },
                        to: {
                            name: 'SortingTo'
                        }
                    }
                },
            },
            {
                name: 'BindAffiliate',
                displayName: $translate.instant('Admin.Js.BookingServices.Bind'),
                cellTemplate: '<ui-grid-custom-switch row="row" class="js-grid-not-clicked" field-name="BindAffiliate"></ui-grid-custom-switch>',
                width: 90,
                filter: {
                    name: 'HasAffiliate',
                    placeholder: $translate.instant('Admin.Js.BookingServices.BindToAffiliate'),
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [{ label: $translate.instant('Admin.Js.BookingServices.Binded'), value: true }, { label: $translate.instant('Admin.Js.BookingServices.NotBinded'), value: false }]
                }
            },
            {
                name: 'Enabled',
                displayName: $translate.instant('Admin.Js.BookingServices.Active'),
                cellTemplate: '<ui-grid-custom-switch row="row" class="js-grid-not-clicked" field-name="Enabled"></ui-grid-custom-switch>',
                width: 90,
                filter: {
                    name: 'Enabled',
                    placeholder: $translate.instant('Admin.Js.BookingServices.Activity'),
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [{ label: $translate.instant('Admin.Js.BookingServices.AreActive'), value: true }, { label: $translate.instant('Admin.Js.BookingServices.NotActive'), value: false }]
                }
            },
            {
                name: '_serviceColumn',
                enableHiding: false,
                displayName: '',
                enableSorting: false,
                width: 80,
                cellTemplate: '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadService(row.entity.Id)"></a> ' +
                    '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteService(row.entity.Id)" ' +
                               'class="ui-grid-custom-service-icon fa fa-times link-invert"></a> ' +
                    '</div></div>'
            }
        ];

        ctrl.gridServicesOptions = ng.extend({}, uiGridCustomConfig, {
            enableGridMenu: true,
            columnDefs: columnDefsServices,
            uiGridCustom: {
                rowClick: function($event, row) {
                    ctrl.loadService(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.BookingServices.DeleteSelected'),
                        url: 'bookingService/deleteServices',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.BookingServices.AreYouSureDelete'), { title: $translate.instant('Admin.Js.BookingServices.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },
                    {
                        text: $translate.instant('Admin.Js.BookingServices.BindToAffiliate'),
                        url: 'bookingService/bindAffiliate',
                        field: 'Id'
                    },
                    {
                        text: $translate.instant('Admin.Js.BookingServices.UnbindToAffiliate'),
                        url: 'bookingService/unBindAffiliate',
                        field: 'Id'
                    }
                ]
            }
        });

        ctrl.gridServicesOnInit = function(grid) {
            ctrl.gridServices = grid;
            ctrl.gridServicesInited = true;
        };

        ctrl.loadService = function(id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddUpdateBookingServiceCtrl',
                controllerAs: 'ctrl',
                size: 'lg',
                backdrop: 'static',
                templateUrl: '../areas/admin/content/src/bookingServices/modals/addUpdateBookingService/addUpdateBookingService.html',
                resolve: {
                    params: {
                        id: id
                    }
                }
            }).result.then(function (result) {
                ctrl.gridServices.fetchData();
                return result;
            }, function (result) {
                ctrl.gridServices.fetchData();
                return result;
            });
        };

        ctrl.deleteService = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.BookingServices.AreYouSureDelete'), { title: $translate.instant('Admin.Js.BookingServices.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('bookingService/deleteService', { id: id }).then(function (response) {
                        if (response.data.result === true) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.BookingServices.ChangesSaved'));
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.BookingServices.FailedToDeleteService'));
                        }
                        ctrl.gridServices.fetchData();
                    });
                }
            });
        };

        ctrl.onServiceAddedOrUpdated = function() {
            ctrl.gridServices.fetchData();
        };

        ctrl.initCategoriesTreeview = function(jstree) {
            ctrl.jstree = jstree;
            ctrl.jstreeCategoriesInited = true;
        }

        ctrl.onCategoryAddedEdit = function () {
            ctrl.jstree.refresh();
        };
    };

    BookingServicesCtrl.$inject = ['$http', '$q', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', 'toaster', '$uibModal', '$translate'];

    ng.module("bookingServices", ['uiGridCustom', 'bookingCategoriesTreeview'])
        .controller("BookingServicesCtrl", BookingServicesCtrl);

})(window.angular)