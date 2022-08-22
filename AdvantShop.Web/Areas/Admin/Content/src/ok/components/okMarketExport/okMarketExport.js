; (function (ng) {
    'use strict';

    var okMarketExportCtrl = function ($http, toaster, uiGridCustomConfig, SweetAlert, $q, $uibModal, $translate, okMarketService) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.okMarketExport.Grid.Name'),
                    enableSorting: false
                },
                {
                    name: 'Categories',
                    displayName: $translate.instant('Admin.Js.okMarketExport.Grid.Categories'),
                    enableSorting: false
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<a href="" class="link-invert ui-grid-custom-service-icon fa fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openModal(row.entity.Id)"></a> ' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.Id, row.entity.OkCatalogId)" class="ui-grid-custom-service-icon fa fa-times link-invert"></a> ' +
                        '</div></div>'
                }
            ];

        ctrl.$onInit = function () {
            ctrl.getExportReports();
            ctrl.IsExportRun = false;
            ctrl.getExportState();
            ctrl.isDeleting = false;
            ctrl.getDeleteState();
        };

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.openModal(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: 'Удалить выделенные',
                        url: 'okMarket/deleteCatalogs',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm("Вы уверены, что хотите удалить? Каталоги и товары в них будут удалены из OK тоже.", { title: "Удаление" }).then(function (result) {
                                if (result === true) {
                                    ctrl.isDeleting = true;
                                    setTimeout(function () { ctrl.getDeleteState(); }, 500);
                                }
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.openModal = function (id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditOkCatalogCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/ok/components/okMarketExport/modals/modalAddEditOkCatalog/modalAddEditOkCatalog.html',
                resolve: {
                    id: function () {
                        return id;
                    }
                }
            }).result.then(function (result) {
                ctrl.grid.fetchData();
                return result;
            }, function (result) {
                ctrl.grid.fetchData();
                return result;
            });
        };

        ctrl.delete = function (id, OkCatalogId) {
            SweetAlert.confirm("Вы уверены, что хотите удалить? При удалении каталога удалится каталог в ОК и товары в нем.", { title: "Удаление" }).then(function (result) {
                if (result === true) {
                    toaster.pop('success', '', 'Удаление каталога и товаров началось');
                    ctrl.grid.isProcessing = true;
                    ctrl.isDeleting = true;
                    setTimeout(function () { ctrl.getDeleteState(); }, 500);

                    $http.post('okMarket/deleteCatalog', { 'Id': id, 'OkCatalogId': OkCatalogId }).then(function () {
                        ctrl.grid.fetchData();
                        ctrl.grid.isProcessing = false;
                    });
                }
            });
        }

        ctrl.export = function () {
            okMarketService.export().then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.OkChannel.ExportStarted'));
                    ctrl.IsExportRun = true;
                    ctrl.grid.isProcessing = true;

                    ctrl.getExportProgress();
                    ctrl.getExportReportsTimeout();
                } else {
                    data.errors.forEach(function (e) {
                        toaster.pop('error', '', e);
                    });
                }
            });
        }
        
        ctrl.getExportProgress = function () {
            return okMarketService.getExportProgress().then(function (data) {
                ctrl.Total = data.Total;
                ctrl.Current = data.Current;
                ctrl.Percent = ctrl.Total > 0 ? parseInt(100 / ctrl.Total * ctrl.Current) : 0;
                ctrl.IsExportRun = data.IsRun;
                if (ctrl.grid != null) {
                    ctrl.grid.isProcessing = data.IsRun;
                }

                if (!ctrl.IsExportRun && ctrl.Total > 0) {
                    toaster.pop('success', '', 'Экспорт закончен');
                } else {
                    setTimeout(function () { ctrl.getExportProgress(); }, 500);
                }
            });
        }
        
        ctrl.getExportState = function () {
            okMarketService.getExportState().then(function (data) {
                ctrl.IsExportRun = data.isRun;
                if (ctrl.IsExportRun)
                    ctrl.getExportProgress();
            });
        }
        
        ctrl.getExportReportsTimeout = function () {
            setTimeout(function () {
                ctrl.getExportReports().then(function () {
                    if (ctrl.Percent != 100) {
                        ctrl.getExportReportsTimeout();
                    }
                });
            }, 3000);
        }

        ctrl.getExportReports = function () {
            return okMarketService.getExportReports().then(function (data) {
                ctrl.ExportReports = data.reports;
            });
        }

        ctrl.getDeleteState = function () {
            return okMarketService.getDeleteState().then(function (data) {
                ctrl.isDeleting = data.isRun;
                if (ctrl.grid != null) {
                    ctrl.grid.isProcessing = data.isRun;
                }

                if (ctrl.isDeleting) {
                    setTimeout(function () { ctrl.getDeleteState(); }, 500);
                } else if (ctrl.grid != null) {
                    ctrl.grid.fetchData();
                }
            });
        }
    };

    okMarketExportCtrl.$inject = ['$http', 'toaster', 'uiGridCustomConfig', 'SweetAlert', '$q', '$uibModal', '$translate', 'okMarketService'];


    ng.module('okMarketExport', ['uiGridCustom'])
        .controller('okMarketExportCtrl', okMarketExportCtrl)
        .component('okMarketExport', {
            templateUrl: '../areas/admin/content/src/ok/components/okMarketExport/okMarketExport.html',
            controller: 'okMarketExportCtrl'
        });

})(window.angular);