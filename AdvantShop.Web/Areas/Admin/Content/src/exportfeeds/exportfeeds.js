; (function (ng) {
    'use strict';

    var ExportFeedsCtrl = function (
        $interval,
        $q,
        $translate,
        $window,
        advTrackingService,
        exportfeedsService,
        SweetAlert,
        toaster,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService) {

        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.exportFeedId = 0;
            ctrl.exportFeedFields = [];
            ctrl.defaultFields = [];
            ctrl.CommonSettings = {};
            ctrl.AdvancedSettings = {};
            ctrl.CurrentExportType = '';
            ctrl.showGridGlobalDeliveryCosts = true;
        }

        ctrl.init = function (exportFeedId, currentExportType) {
            ctrl.exportFeedId = exportFeedId;
            ctrl.CurrentExportType = currentExportType;
            ctrl.gridYandexPromoCodesOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'Name',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoName'),
                        width: 250
                    },
                    {
                        name: 'Description',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoDescription')
                    },
                    {
                        name: 'PromoUrl',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoUrl'),
                        width: 200
                    },
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 80,
                        enableSorting: false,
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-modal-trigger data-controller="\'ModalAddEditYandexPromoCodeCtrl\'" controller-as="ctrl" size="middle"' +
                            'template-url="../areas/admin/content/src/exportfeeds/modal/addEditYandexPromoCode/addEditYandexPromoCode.html"' +
                            'data-resolve="{value: { ExportFeedId: ' + exportFeedId + ', PromoID: row.entity.PromoID } }"' +
                            'on-close="grid.appScope.$ctrl.gridExtendCtrl.AddEditYandexPromoCode(result)">' +
                            '<a href="" data-e2e="btnEdit">' +
                            '<span class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></span>' +
                            '</a>' +
                            '</ui-modal-trigger>' +
                            '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteYandexPromoCode(row.entity)" class= "ui-grid-custom-service-icon fa fa-times link-invert" >' +
                            '</a >' +
                            '</div></div>'
                    }
                ],
                data: [],
                enableSorting: false,
            });

            ctrl.gridYandexPromoFlashOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'Name',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoName'),
                        width: 200
                    },
                    {
                        name: 'Description',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoDescription')
                    },
                    {
                        name: 'StartDate',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoStartDate'),
                        width: 150,
                        cellFilter: 'date:\'dd.MM.yyyy HH:mm\''
                    },
                    {
                        name: 'ExpirationDate',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoExpirationDate'),
                        width: 150,
                        cellFilter: 'date:\'dd.MM.yyyy HH:mm\''
                    },
                    {
                        name: 'PromoUrl',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoUrl'),
                        width: 200
                    },
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 80,
                        enableSorting: false,
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-modal-trigger data-controller="\'ModalAddEditYandexPromoFlashCtrl\'" controller-as="ctrl" size="middle"' +
                            'template-url="../areas/admin/content/src/exportfeeds/modal/addEditYandexPromoFlash/addEditYandexPromoFlash.html"' +
                            'data-resolve="{value: { ExportFeedId: ' + exportFeedId + ', PromoID: row.entity.PromoID } }"' +
                            'on-close="grid.appScope.$ctrl.gridExtendCtrl.AddEditYandexPromoFlash(result)">' +
                            '<a href="" data-e2e="btnEdit">' +
                            '<span class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></span>' +
                            '</a>' +
                            '</ui-modal-trigger>' +
                            '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteYandexPromoFlash(row.entity)" class= "ui-grid-custom-service-icon fa fa-times link-invert" >' +
                            '</a >' +
                            '</div></div>'
                    }
                ],
                data: [],
                enableSorting: false,
            });

            ctrl.gridYandexPromoGiftOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'Name',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoName'),
                        width: 200
                    },
                    {
                        name: 'Description',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoDescription')
                    },
                    {
                        name: 'StartDate',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoStartDate'),
                        width: 150,
                        cellFilter: 'date:\'dd.MM.yyyy HH:mm\''
                    },
                    {
                        name: 'ExpirationDate',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoExpirationDate'),
                        width: 150,
                        cellFilter: 'date:\'dd.MM.yyyy HH:mm\''
                    },
                    {
                        name: 'PromoUrl',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoUrl'),
                        width: 200
                    },
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 80,
                        enableSorting: false,
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-modal-trigger data-controller="\'ModalAddEditYandexPromoGiftCtrl\'" controller-as="ctrl" size="middle"' +
                            'template-url="../areas/admin/content/src/exportfeeds/modal/addEditYandexPromoGift/addEditYandexPromoGift.html"' +
                            'data-resolve="{value: { ExportFeedId: ' + exportFeedId + ', PromoID: row.entity.PromoID } }"' +
                            'on-close="grid.appScope.$ctrl.gridExtendCtrl.AddEditYandexPromoGift(result)">' +
                            '<a href="" data-e2e="btnEdit">' +
                            '<span class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></span>' +
                            '</a>' +
                            '</ui-modal-trigger>' +
                            '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteYandexPromoGift(row.entity)" class= "ui-grid-custom-service-icon fa fa-times link-invert" >' +
                            '</a >' +
                            '</div></div>'
                    }
                ],
                data: [],
                enableSorting: false,
            });

            ctrl.gridYandexPromoNPlusMOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'Name',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoName'),
                        width: 200
                    },
                    {
                        name: 'Description',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoDescription')
                    },
                    {
                        name: 'StartDate',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoStartDate'),
                        width: 150,
                        cellFilter: 'date:\'dd.MM.yyyy HH:mm\''
                    },
                    {
                        name: 'ExpirationDate',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoExpirationDate'),
                        width: 150,
                        cellFilter: 'date:\'dd.MM.yyyy HH:mm\''
                    },
                    {
                        name: 'PromoUrl',
                        displayName: $translate.instant('Admin.Js.ExportFeeds.PromoUrl'),
                        width: 200
                    },
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 80,
                        enableSorting: false,
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-modal-trigger data-controller="\'ModalAddEditYandexPromoNPlusMCtrl\'" controller-as="ctrl" size="middle"' +
                            'template-url="../areas/admin/content/src/exportfeeds/modal/addEditYandexPromoNPlusM/addEditYandexPromoNPlusM.html"' +
                            'data-resolve="{value: { ExportFeedId: ' + exportFeedId + ', PromoID: row.entity.PromoID } }"' +
                            'on-close="grid.appScope.$ctrl.gridExtendCtrl.AddEditYandexPromoNPlusM(result)">' +
                            '<a href="" data-e2e="btnEdit">' +
                            '<span class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></span>' +
                            '</a>' +
                            '</ui-modal-trigger>' +
                            '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteYandexPromoNPlusM(row.entity)" class= "ui-grid-custom-service-icon fa fa-times link-invert" >' +
                            '</a >' +
                            '</div></div>'
                    }
                ],
                data: [],
                enableSorting: false,
            });
        };

        ctrl.initExportFields = function (defaultFields) {
            ctrl.defaultFields = defaultFields;
        };

        ctrl.saveExportFeed = function (quietSuccess) {
            ctrl.validateFileName();
            if (ctrl.CommonSettings.JobStartHour < 0 || ctrl.CommonSettings.JobStartHour > 23 || ctrl.CommonSettings.JobStartMinute < 0 || ctrl.CommonSettings.JobStartMinute > 59) {
                toaster.error('', $translate.instant('Admin.Js.ExportFeeds.JobStartHourMinuteError'));
                return $q.resolve(false);
            }
            if (ctrl.gridGlobalDeliveryOptions != null && ctrl.gridGlobalDeliveryOptions.data != null) { // ctrl.showGridGlobalDeliveryCosts
                ctrl.AdvancedSettings.GlobalDeliveryCost = JSON.stringify(ctrl.gridGlobalDeliveryOptions.data);
            }

            if (ctrl.gridYandexPromoCodesOptions != null && ctrl.gridYandexPromoCodesOptions.data != null
                || ctrl.gridYandexPromoFlashOptions != null && ctrl.gridYandexPromoFlashOptions.data != null
                || ctrl.gridYandexPromoGiftOptions != null && ctrl.gridYandexPromoGiftOptions.data != null
                || ctrl.gridYandexPromoNPlusMOptions != null && ctrl.gridYandexPromoNPlusMOptions.data != null) {

                ctrl.AdvancedSettings.Promos = JSON.stringify(ctrl.gridYandexPromoCodesOptions.data
                    .concat(ctrl.gridYandexPromoFlashOptions.data)
                    .concat(ctrl.gridYandexPromoGiftOptions.data)
                    .concat(ctrl.gridYandexPromoNPlusMOptions.data));
            }

            if (ctrl.LocalDeliveryOption != null) {
                ctrl.AdvancedSettings.LocalDeliveryOption =
                    JSON.stringify({
                        Days: ctrl.LocalDeliveryOption.Days,
                        OrderBefore: ctrl.LocalDeliveryOption.OrderBefore
                    });
            }

            return exportfeedsService.saveExportFeedSettings(ctrl.exportFeedId, ctrl.CommonSettings.Name, ctrl.CommonSettings.Description, ctrl.CommonSettings, JSON.stringify(ctrl.AdvancedSettings)).then(function (data) {
                var defer = $q.defer(),
                    promise = defer.promise;
                var saveSettingsSucess = data.result;
                if (!saveSettingsSucess) {
                    toaster.error('', $translate.instant('Admin.Js.ExportFeeds.ErrorWhileSavingSettings'));
                }
                if (ctrl.exportFeedFields.length > 0) {
                    promise = exportfeedsService.saveExportFeedFields(ctrl.exportFeedId, ctrl.exportFeedFields);
                } else {
                    defer.resolve({ result: true });
                }

                return promise.then(function (data) {
                    if (!data.result) {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ExportFeeds.ErrorWhileSavingExportFields'));
                    }

                    if (!quietSuccess && saveSettingsSucess && data.result) {
                        toaster.success('', $translate.instant('Admin.Js.ExportFeeds.ChangesSaved'));
                    }

                    ctrl.exportFeedForm.modified = false;
                    ctrl.exportFeedForm.$setPristine();

                    return saveSettingsSucess && data.result;
                });
            });
        };

        ctrl.deleteExport = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.ExportFeeds.AreYouSureDelete'), { title: $translate.instant('Admin.Js.ExportFeeds.Deleting') }).then(function (result) {
                if (result === true) {
                    exportfeedsService.deleteExport(id).then(function (data) {
                        if (data.result) {
                            $window.location.assign('exportfeeds/index' + ctrl.CurrentExportType);
                        }
                        else {
                            toaster.error('', $translate.instant('Admin.Js.ExportFeeds.CouldNotDeleteExport'));
                        }
                    });
                }
            });
        }

        // #region ChoiceOfProducts

        var timerTree;
        ctrl.treeCallbacks = {
            select_node: function (event, data) {

                if (timerTree != null) {
                    clearTimeout(timerTree);
                }

                timerTree = setTimeout(function () {
                    var tree = ng.element(event.target).jstree(true);
                    var selectedNodes = tree.get_selected(true);
                    var selected = selectedNodes.map(function (item) {
                        return { categoryId: item.id, opened: item.state.opened };
                    });
                    exportfeedsService.addCategoriesToExport(ctrl.exportFeedId, selected).then(function (response) { });


                    if (data.event == null || data.event.parentEvent !== 'load_node') {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ExportFeeds.CategoryAddedToExport'));
                    }
                }, 600);
            },

            deselect_node: function (event, data) {

                if (timerTree != null) {
                    clearTimeout(timerTree);
                }

                timerTree = setTimeout(function () {
                    var tree = ng.element(event.target).jstree(true);
                    var selectedNodes = tree.get_selected(true);
                    var selected = selectedNodes.map(function (item) {
                        return { categoryId: item.id, opened: item.state.opened };
                    });

                    exportfeedsService.addCategoriesToExport(ctrl.exportFeedId, selected).then(function (response) { });

                    if (data.event == null || data.event.parentEvent !== 'load_node') {
                        toaster.pop('success', '', $translate.instant('Admin.Js.ExportFeeds.CategoryDeletedFromExport'));
                    }
                }, 600);
            },
        };

        ctrl.exportAllProductsClick = function () {
            ctrl.exportAllProducts = true;
            toaster.success($translate.instant('Admin.Js.ExportFeeds.WholeCatalogIsUnloaded'));
        };

        // #endregion

        // #region ChoiceOfFields

        ctrl.setNoneExportFeedFields = function () {
            for (var i = 0; i < ctrl.exportFeedFields.length; i++) {
                ctrl.exportFeedFields[i] = 'None';
            }
        }

        ctrl.setDefaultExportFeedFields = function () {
            for (var i = 0; i < ctrl.defaultFields.length; i++) {
                ctrl.exportFeedFields[i] = ctrl.defaultFields[i];
            }
        }

        // #endregion

        // #region Starting/Processing Export

        ctrl.progressValue = 0;
        ctrl.progressTotal = 0;
        ctrl.progressPercent = 0;
        ctrl.progressCurrentProcess = "";
        ctrl.progressCurrentProcessName = "";
        ctrl.IsRun = true;
        ctrl.FileName = "";

        ctrl.stop = 0;

        ctrl.initProgress = function () {

            ctrl.stop = $interval(function () {

                exportfeedsService.getCommonStatistic().then(function (response) {
                    ctrl.IsRun = response.IsRun;
                    if (!response.IsRun) {
                        $interval.cancel(ctrl.stop);
                        ctrl.FileName = response.FileName.indexOf('?') != -1 ? response.FileName : response.FileName + "?rnd=" + Math.random();
                    }
                    ctrl.ErrorsCount = response.Error;
                    ctrl.IsZip = response.ZipFile;
                    ctrl.progressTotal = response.Total;
                    ctrl.progressValue = response.Processed;
                    ctrl.progressCurrentProcess = response.CurrentProcess;
                    ctrl.progressCurrentProcessName = response.CurrentProcessName;
                });

            }, 100);
        }

        ctrl.startExport = function () {
            ctrl.saveExportFeed(true).then(function (result) {
                if (result) {
                    ctrl.startProcessExport();
                }
            });
        };

        ctrl.startProcessExport = function() {
            exportfeedsService.getCommonStatistic().then(function (response) {
                if (!response.IsRun) {
                    var actionName = ctrl.CurrentExportType == 'csv' || ctrl.CurrentExportType == 'csv2' ? "export" : "export" + ctrl.CurrentExportType;
                    $window.location.assign(`exportfeeds/${actionName}/${ctrl.exportFeedId}`);
                } else {
                    toaster.error('', $translate.instant('Admin.Js.CommonStatistic.AlreadyRunning') +
                        ' <a href="' + response.CurrentProcess + '">' + (response.CurrentProcessName || response.CurrentProcess) + '</a>');
                }
            });
        };
        // #endregion

        // #region Settings/AdvancedSettings

        ctrl.gridGlobalDeliveryOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Cost',
                    displayName: $translate.instant('Admin.Js.ExportFeeds.Cost')
                },
                {
                    name: 'Days',
                    displayName: $translate.instant('Admin.Js.ExportFeeds.DeliveryTime')
                },
                {
                    name: 'OrderBefore',
                    displayName: $translate.instant('Admin.Js.ExportFeeds.TimeToWhich')
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 40,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteGlobalOptionCost(row.entity)" class="ui-grid-custom-service-icon fa fa-times link-invert"></a></div>'
                }
            ],
            enableSorting: false,
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.gridGlobalDeliveryCosts = grid;
            ctrl.showGridGlobalDeliveryCosts = grid.gridOptions.data != null && grid.gridOptions.data.length > 0;
        };

        ctrl.gridOnFetch = function (grid) {
            ctrl.showGridGlobalDeliveryCosts = grid.gridOptions.data != null && grid.gridOptions.data.length > 0;
        }

        ctrl.setGridOptionsData = function (data) {
            ctrl.gridGlobalDeliveryOptions.data = data;
        }

        ctrl.deleteGlobalOptionCost = function (row) {
            var indexDelete = ctrl.gridGlobalDeliveryOptions.data.indexOf(row);
            ctrl.gridGlobalDeliveryOptions.data.splice(indexDelete, 1);

            ctrl.exportFeedForm.modified = true;
        }



        ctrl.yandexAddGlobalDeliveryCost = function (result) {
            ctrl.gridGlobalDeliveryOptions.data.push(result);
            ctrl.exportFeedForm.modified = true;
        }

        ctrl.gridYandexPromoCodesOnInit = function (grid) {
            ctrl.gridYandexPromoCodes = grid;
            ctrl.showGridYandexPromoCodes = grid.gridOptions.data != null && grid.gridOptions.data.length > 0;
        };

        ctrl.gridYandexPromoCodesOnFetch = function (grid) {
            ctrl.showGridYandexPromoCodes = grid.gridOptions.data != null && grid.gridOptions.data.length > 0;
        }

        ctrl.setGridYandexPromoCodesOptionsData = function (data) {
            ctrl.gridYandexPromoCodesOptions.data = data;
        }

        ctrl.AddEditYandexPromoCode = function (result) {
            var indexPromo = ctrl.gridYandexPromoCodesOptions.data.map(function (e) { return e.PromoID; }).indexOf(result.PromoID);
            if (indexPromo == -1) {
                ctrl.gridYandexPromoCodesOptions.data.push(result);
            } else {
                ctrl.gridYandexPromoCodesOptions.data[indexPromo] = result;
            }
            ctrl.exportFeedForm.modified = true;
            ctrl.saveExportFeed();
            ctrl.trackEditYandexPromoEvent();
        }

        ctrl.deleteYandexPromoCode = function (row) {
            SweetAlert.confirm("Вы уверены, что хотите удалить промоакцию?", { title: "Удаление" }).then(function (result) {
                if (result === true) {
                    var indexDelete = ctrl.gridYandexPromoCodesOptions.data.indexOf(row);
                    ctrl.gridYandexPromoCodesOptions.data.splice(indexDelete, 1);
                    ctrl.exportFeedForm.modified = true;
                    ctrl.saveExportFeed();
                    ctrl.trackEditYandexPromoEvent();
                }
            });

        }



        ctrl.gridYandexPromoFlashOnInit = function (grid) {
            ctrl.gridYandexPromoFlash = grid;
            ctrl.showGridYandexPromoFlash = grid.gridOptions.data != null && grid.gridOptions.data.length > 0;
        };

        ctrl.gridYandexPromoFlashOnFetch = function (grid) {
            ctrl.showGridYandexPromoFlash = grid.gridOptions.data != null && grid.gridOptions.data.length > 0;
        }

        ctrl.setGridYandexPromoFlashOptionsData = function (data) {
            ctrl.gridYandexPromoFlashOptions.data = data;
        }

        ctrl.AddEditYandexPromoFlash = function (result) {
            var indexPromo = ctrl.gridYandexPromoFlashOptions.data.map(function (e) { return e.PromoID; }).indexOf(result.PromoID);
            if (indexPromo == -1) {
                ctrl.gridYandexPromoFlashOptions.data.push(result);
            } else {
                ctrl.gridYandexPromoFlashOptions.data[indexPromo] = result;
            }
            ctrl.exportFeedForm.modified = true;
            ctrl.saveExportFeed();
            ctrl.trackEditYandexPromoEvent();
        }

        ctrl.deleteYandexPromoFlash = function (row) {
            SweetAlert.confirm("Вы уверены, что хотите удалить промоакцию?", { title: "Удаление" }).then(function (result) {
                if (result === true) {
                    var indexDelete = ctrl.gridYandexPromoFlashOptions.data.indexOf(row);
                    ctrl.gridYandexPromoFlashOptions.data.splice(indexDelete, 1);
                    ctrl.exportFeedForm.modified = true;
                    ctrl.saveExportFeed();
                    ctrl.trackEditYandexPromoEvent();
                }
            });
        }



        ctrl.gridYandexPromoGiftOnInit = function (grid) {
            ctrl.gridYandexPromoGift = grid;
            ctrl.showGridYandexPromoGift = grid.gridOptions.data != null && grid.gridOptions.data.length > 0;
        };

        ctrl.gridYandexPromoGiftOnFetch = function (grid) {
            ctrl.showGridYandexPromoGift = grid.gridOptions.data != null && grid.gridOptions.data.length > 0;
        }

        ctrl.setGridYandexPromoGiftOptionsData = function (data) {
            ctrl.gridYandexPromoGiftOptions.data = data;
        }

        ctrl.AddEditYandexPromoGift = function (result) {
            var indexPromo = ctrl.gridYandexPromoGiftOptions.data.map(function (e) { return e.PromoID; }).indexOf(result.PromoID);
            if (indexPromo == -1) {
                ctrl.gridYandexPromoGiftOptions.data.push(result);
            } else {
                ctrl.gridYandexPromoGiftOptions.data[indexPromo] = result;
            }
            ctrl.exportFeedForm.modified = true;
            ctrl.saveExportFeed();
            ctrl.trackEditYandexPromoEvent();
        }

        ctrl.deleteYandexPromoGift = function (row) {
            SweetAlert.confirm("Вы уверены, что хотите удалить промоакцию?", { title: "Удаление" }).then(function (result) {
                if (result === true) {
                    var indexDelete = ctrl.gridYandexPromoGiftOptions.data.indexOf(row);
                    ctrl.gridYandexPromoGiftOptions.data.splice(indexDelete, 1);
                    ctrl.exportFeedForm.modified = true;
                    ctrl.saveExportFeed();
                    ctrl.trackEditYandexPromoEvent();
                }
            });
        }



        ctrl.gridYandexPromoNPlusMOnInit = function (grid) {
            ctrl.gridYandexPromoNPlusM = grid;
            ctrl.showGridYandexPromoNPlusM = grid.gridOptions.data != null && grid.gridOptions.data.length > 0;
        };

        ctrl.gridYandexPromoNPlusMOnFetch = function (grid) {
            ctrl.showGridYandexPromoNPlusM = grid.gridOptions.data != null && grid.gridOptions.data.length > 0;
        }

        ctrl.setGridYandexPromoNPlusMOptionsData = function (data) {
            ctrl.gridYandexPromoNPlusMOptions.data = data;
        }

        ctrl.AddEditYandexPromoNPlusM = function (result) {
            var indexPromo = ctrl.gridYandexPromoNPlusMOptions.data.map(function (e) { return e.PromoID; }).indexOf(result.PromoID);
            if (indexPromo == -1) {
                ctrl.gridYandexPromoNPlusMOptions.data.push(result);
            } else {
                ctrl.gridYandexPromoNPlusMOptions.data[indexPromo] = result;
            }
            ctrl.exportFeedForm.modified = true;
            ctrl.saveExportFeed();
            ctrl.trackEditYandexPromoEvent();
        }

        ctrl.deleteYandexPromoNPlusM = function (row) {
            SweetAlert.confirm("Вы уверены, что хотите удалить промоакцию?", { title: "Удаление" }).then(function (result) {
                if (result === true) {
                    var indexDelete = ctrl.gridYandexPromoNPlusMOptions.data.indexOf(row);
                    ctrl.gridYandexPromoNPlusMOptions.data.splice(indexDelete, 1);
                    ctrl.exportFeedForm.modified = true;
                    ctrl.saveExportFeed();
                    ctrl.trackEditYandexPromoEvent();
                }
            });
        }

        // #endregion

        ctrl.validateFileName = function () {
            var regex = /(\\|\:|\*|\?|\"|\<|\>|\||\;|\#|\&|\%|\+|\~)/g;
            ctrl.CommonSettings.FileName = ctrl.CommonSettings.FileName.replace(regex, '');
        };

        ctrl.trackEditYandexPromoEvent = function () {
            advTrackingService.trackEvent('Shop_ExportFeeds_YandexMarket_EditPromo');
        };

        // #region TrashBin
        //ctrl.gridGlobalDeliveryOptions = {};

        //ctrl.onChange = function (categoryId, ids, selectMode) {

        //    var itemIndex;

        //    for (var i = 0, len = ctrl.data.length; i < len; i++) {
        //        if (ctrl.data[i].categoryId === categoryId) {
        //            itemIndex = i;
        //            break;
        //        }
        //    }

        //    if (itemIndex != null) {
        //        ctrl.data[itemIndex].ids = ids;
        //        ctrl.data[itemIndex].selectMode = selectMode;
        //    } else {
        //        ctrl.data.push({
        //            categoryId: categoryId,
        //            ids: ids,
        //            selectMode: selectMode
        //        })
        //    }
        //};
        //ctrl.initAvitoAdvancedSettings()
        //{
        //    return $http.post('exportfeeds/get', { 'exportFeedId': exportFeedId }).then(function (response) {
        //        return response.data;
        //    });
        //}
        // #endregion
    };

    ExportFeedsCtrl.$inject = [
        '$interval',
        '$q',
        '$translate',
        '$window',
        'advTrackingService',
        'exportfeedsService',
        'SweetAlert',
        'toaster',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService'];

    ng.module('exportfeeds', ['urlHelper'])
        .controller('ExportFeedsCtrl', ExportFeedsCtrl);

})(window.angular);