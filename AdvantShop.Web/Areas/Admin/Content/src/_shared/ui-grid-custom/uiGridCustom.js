; (function (ng) {
    'use strict';

    //TO DO: remove attribute data-e2e-col-index

    var UiGridCustomCtrl = function ($element, $document, $interpolate, $location, $scope, $timeout, $q, $window, $locale, uiGridConstants, i18nService, uiGridEditService, uiGridCustomService, uiGridCustomConfig, uiGridCustomParamsConfig, toaster, domService, $translate, $transclude, gridUtil) {
        var ctrl = this,
            isFirstPageLoad = true,
            gridApiReady = $q.defer(),
            selectionCustomReady = $q.defer(),
            locationWatch,
            historyItems = [],
            paramsOnFirstInit = {};

        ctrl.firstLoading = true;
        ctrl.isProcessing = false;
        ctrl.tableHeight = null;
        ctrl.overrideControl = false;

        ctrl.$onInit = function () {

            if (uiGridCustomService.validateId(ctrl.gridUniqueId) === false) {
                throw new Error('Invalid value "gridUniqueId"' + (ctrl.gridUniqueId ? ': ' + ctrl.gridUniqueId : ''));
            }

            ctrl.isMobile = $document[0].documentElement.classList.contains('mobile-version');

            if (ctrl.isMobile === true) {
                ctrl.gridFilterTemplateUrl = '/areas/admin/content/src/_shared/ui-grid-custom/templates/ui-grid-custom-filter--mobile.html';
                ctrl.gridFilterHiddenTotalItemsCount = true;
                ctrl.searchAutofocus = false;
                ctrl.gridOptions.showHeader = false;
            }

            if (ctrl.isMobile) {
                ctrl.overrideControl = $transclude.isSlotFilled('overrideControl');
            }
            
            i18nService.setCurrentLang($locale.id.split('-')[0]);
            uiGridCustomService.addInStorage(ctrl.gridUniqueId);

            $element.on('$destroy', function () {
                uiGridCustomService.removeFromStorage(ctrl.gridUniqueId);

                ctrl.locationWatchUnreg();

                $location.search(ctrl.gridUniqueId, null);

                $element.off();
            });

            var optionsFromStorage = ctrl.getDataItemFromStorage();

            if (optionsFromStorage != null) {
                if (optionsFromStorage.sorting != null) {
                    var searchName = optionsFromStorage.sorting.toLowerCase();

                    for (var i = 0, len = ctrl.gridOptions.columnDefs.length; i < len; i++) {
                        if (ctrl.gridOptions.columnDefs[i].name.toLowerCase() === searchName) {
                            ctrl.gridOptions.columnDefs[i].sort = ctrl.gridOptions.columnDefs[i].sort || {};
                            ctrl.gridOptions.columnDefs[i].sort.direction = optionsFromStorage.sortingType;
                            break;
                        }
                    }
                }

                if (optionsFromStorage.paginationPageSize != null) {
                    ng.extend(ctrl.gridOptions, { paginationPageSize: optionsFromStorage.paginationPageSize });
                }

                if (optionsFromStorage.enableHiding != null) {
                    var keysColumns = Object.keys(optionsFromStorage.enableHiding);
                    var column;
                    for (var j = 0, lenJ = keysColumns.length; j < lenJ; j++) {
                        column = ctrl.gridOptions.columnDefs.filter(function (col) { return col.name === keysColumns[j] });

                        if (column.length === 1) {
                            column[0].visible = optionsFromStorage.enableHiding[keysColumns[j]];
                        }

                    }
                }
            }

            if (ctrl.overrideControl !== true && ctrl.isMobile !== true) {
                ctrl.addListenerForColDefsMediaQueries(ctrl.findColDefsWithMediaQueries());
            }

            ctrl._params = ng.extend({}, uiGridCustomParamsConfig, ctrl.gridParams, { paginationCurrentPage: 1, paginationPageSize: ctrl.gridOptions.paginationPageSize }, ctrl.getDataItemFromStorage());

            ctrl.gridFilterEnabled = ctrl.gridFilterEnabled != null ? ctrl.gridFilterEnabled : true;
            ctrl.gridPaginationEnabled = ctrl.gridPaginationEnabled != null ? ctrl.gridPaginationEnabled : true;
            ctrl.gridSelectionEnabled = ctrl.gridSelectionEnabled != null ? ctrl.gridSelectionEnabled : true;

            ctrl.gridEmptyText = ctrl.gridEmptyText != null ? ctrl.gridEmptyText : $translate.instant('Admin.Js.GridCustom.NoWritingsFound');

            var oltherOnRegisterApi = ctrl.gridOptions.onRegisterApi || function () { };

            if (ctrl.overrideControl !== true) {
                ctrl.gridOptions.onRegisterApi = function (gridApi) {
                    oltherOnRegisterApi(gridApi);

                    ctrl.bindGridApi(gridApi);
                };
            }

            ctrl.optionsFromUrl();

            if (ctrl.gridOptions != null && ctrl.gridOptions.rowEntitySave == null) {
                ctrl.gridOptions.saveRowIdentity = function (rowEntity) {
                    var result = null;

                    if (ctrl.gridRowIdentificator != null) {
                        result = rowEntity[ctrl.gridRowIdentificator];
                    } else {
                        result = ctrl.defaultRowEntitySave(rowEntity);
                    }

                    return result;
                };
            }

            return $q.when(ctrl.gridOnPreinit != null ? ctrl.gridOnPreinit({ grid: ctrl }) : null).then(function () {

                ctrl.addHistoryItem();

                return ctrl.fetchData().then(function () {
                    isFirstPageLoad = false;

                    if (ctrl.gridOnInit != null) {
                        ctrl.gridOnInit({ grid: ctrl });
                    }

                    paramsOnFirstInit.gridOptions = ng.copy(ctrl.gridOptions);
                    paramsOnFirstInit._params = ng.copy(ctrl._params);

                    return ctrl;
                });
            });
        };

        ctrl.update = function () {
            ctrl.optionsFromUrl();

            return $q.when(ctrl.gridOnPreinit != null ? ctrl.gridOnPreinit({ grid: ctrl }) : null).then(function () {
                return ctrl.fetchData(true).then(function () {
                    isFirstPageLoad = false;

                    if (ctrl.gridOnInit != null) {
                        ctrl.gridOnInit({ grid: ctrl });
                    }

                    return ctrl;
                });
            });
        };

        ctrl.locationWatch = function () {

            if (locationWatch != null) {
                locationWatch();
            }

            locationWatch = $scope.$on('$locationChangeSuccess', function () {

                var newParams = JSON.stringify($location.search()[ctrl.gridUniqueId]);

                if (ctrl.getLastHistoryItem() !== newParams) {
                    //$timeout(function () {

                    if (newParams == null) {
                        ctrl.gridOptions = paramsOnFirstInit.gridOptions;
                        ctrl._params = paramsOnFirstInit._params;
                    }

                    ctrl.backHistory();

                    ctrl.update();
                    //}, 100);
                }
            });
        };

        ctrl.addHistoryItem = function (item) {

            if (historyItems.indexOf(item) === -1) {
                historyItems.push(item);
            }

            return item;
        };

        ctrl.backHistory = function () {
            historyItems.splice(-1, 1);
        };

        ctrl.getLastHistoryItem = function () {
            return historyItems.length > 0 ? historyItems.slice(-1)[0] : historyItems[0];
        };

        ctrl.locationWatchUnreg = function () {
            locationWatch != null && locationWatch();
        };

        ctrl.updateTableHeight = function () {
            var headerHeight = 31,
                rowsLength = ctrl.gridOptions != null && ctrl.gridOptions.data != null && ctrl.gridOptions.data.length > 0 ? ctrl.gridOptions.data.length : 1;

            ctrl.tableHeight = (rowsLength * ctrl.gridOptions.rowHeight + headerHeight) + 'px';
        };

        ctrl.setParamsByUrl = function () {
            if (isFirstPageLoad === false && ctrl.gridPreventStateInHash !== true) {
                uiGridCustomService.setParamsByUrl(ctrl.gridUniqueId, ctrl._params);
            } else if (isFirstPageLoad === false && ctrl.gridPreventStateInHash === true) {
                ctrl.update();
            }
        };

        ctrl.fetchData = function (ignoreHistory) {

            ctrl.locationWatchUnreg();

            ctrl.setStateProcess(true);

            var defer = $q.defer();

            if (ctrl.gridUrl != null && ctrl.gridUrl.length > 0) {
                uiGridCustomService.getData(ctrl.gridUrl, uiGridCustomService.convertToServerParams(ctrl._params))
                    .then(function (result) {
                        defer.resolve(result);
                    })
                    .catch(function (response) {
                        defer.reject(response);
                    });
            } else {
                defer.resolve(ctrl.gridOptions);
            }

            return defer.promise
                .then(function (result) {
                    return ctrl.gridOptions.paginationCurrentPage > result.TotalPageCount && result.TotalPageCount != 0 ? ctrl.paginationChange(result.TotalPageCount, ctrl.gridOptions.paginationPageSize, ctrl.gridOptions.paginationPageSizes) : result;
                })
                .then(function (result) {
                    ng.extend(ctrl.gridOptions, uiGridCustomService.convertToClientParams(ctrl._params, true), result);

                    gridApiReady.promise.then(ctrl.checkSelection).then(function () {
                        $timeout(function () {
                            ctrl.restoreState(true);
                        }, 0);
                    });

                    ctrl.setStateProcess(false);

                    ctrl.firstLoading = false;

                    if (ctrl.gridOnFetch != null) {
                        ctrl.gridOnFetch({ grid: ctrl });
                    }

                    return result;
                })
                .then(function (result) {
                    if (ctrl.gridSelectionEnabled === true && ctrl.isMobile === false) {
                        return selectionCustomReady.promise.then(function () {
                            return ctrl.selectionSelectItemsFromOutside(result);
                        });
                    } else {
                        return result;
                    }
                })
                .catch(function (error) {
                    if (error.status !== -1) {
                        ctrl.gridOptions.data = [];
                        toaster.error($translate.instant('Admin.Js.GridCustom.ErrorWhileLoadingData'));
                        ctrl.error = $translate.instant('Admin.Js.GridCustom.ErrorWhileLoadingData');
                        ctrl.setStateProcess(false);
                    }

                    return error;
                })
                .finally(function () {

                    if (ignoreHistory !== true && ctrl.getLastHistoryItem() !== JSON.stringify($location.search()[ctrl.gridUniqueId])) {
                        ctrl.addHistoryItem(JSON.stringify($location.search()[ctrl.gridUniqueId]));
                    }

                    setTimeout(function () {
                        ctrl.locationWatch();
                    }, 0);

                    ctrl.updateTableHeight();
                });
        };

        //#region filter
        ctrl.filterInit = function (filter) {
            ctrl.filter = filter;
            if (ctrl.gridOnFilterInit != null) {
                ctrl.gridOnFilterInit({ filter: filter });
            }
        };

        ctrl.filterApply = function (params, item) {

            if (ng.isArray(params) === false) {
                throw new Error('Parameter "params" should be array')
            }

            for (var i = 0, len = params.length; i < len; i++) {
                ctrl._params[params[i].name] = params[i].value;
            }

            ctrl.gridOptions.paginationCurrentPage = 1;
            ctrl._params.paginationCurrentPage = 1;

            ctrl.setParamsByUrl();
        };

        ctrl.filterRemove = function (name, item) {

            if (item.filter.type === 'range') {
                delete ctrl._params[item.filter.rangeOptions.from.name];
                delete ctrl._params[item.filter.rangeOptions.to.name];
            } if (item.filter.type === 'datetime') {
                delete ctrl._params[item.filter.datetimeOptions.from.name];
                delete ctrl._params[item.filter.datetimeOptions.to.name];
            } else if (item.filter.type === 'date') {
                delete ctrl._params[item.filter.dateOptions.from.name];
                delete ctrl._params[item.filter.dateOptions.to.name];
            } else {
                delete ctrl._params[name];
            }

            ctrl.gridOptions.paginationCurrentPage = 1;
            ctrl._params.paginationCurrentPage = 1;

            ctrl.setParamsByUrl();
        };
        //#endregion

        //#region selection

        ctrl.selectionSelectItemsFromOutside = function (result) {
            return $timeout(function () {

                //if (ctrl.selectionCustom != null && (ctrl.gridOptions.data == null || ctrl.gridOptions.data.length === 0)) {
                //    ctrl.selectionCustom.clearSelectedRows();
                //} else {

                result.data.filter(function (rowEntity) {
                    return ctrl.gridSelectionItemsSelectedFn({ rowEntity: rowEntity });
                })
                    .forEach(function (item) {
                        ctrl.gridApi.selection.selectRow(item);
                    });
                ctrl.selectionOnChange(ctrl.gridApi.selection.getSelectedGridRows());
                //}

                return result;
            }, 100);
        };

        ctrl.selectionOnInit = function (selectionCustom) {
            ctrl.selectionCustom = selectionCustom;
            if (ctrl.gridSelectionOnInit != null) {
                ctrl.gridSelectionOnInit({ selectionCustom: selectionCustom });
            }

            if (ctrl.gridOptions.data == null || ctrl.gridOptions.data.length === 0 && ctrl.gridApi.core.getVisibleRows(ctrl.gridApi.grid).length === 0) {
                ctrl.selectionCustom.clearSelectedRows();
            } else {
                ctrl.selectionSelectItemsFromOutside(ctrl.gridOptions);
            }
            selectionCustomReady.resolve(selectionCustom);
        };

        ctrl.selectionUpdate = function (response) {

            //ctrl.resetState();
            //ctrl.selectionCustom.clearSelectedRows();

            ctrl.fetchData()
                .then(function () {
                    ctrl.setParamsByUrl();

                    if (ctrl.gridOptions.data == null || ctrl.gridOptions.data.length === 0) {
                        ctrl.selectionCustom.clearSelectedRows();
                    }
                });

            if (response != null && response.data.result === false) {
                if (response.data.errors != null && response.data.errors.length > 0) {
                    response.data.errors.forEach(function (item) {
                        toaster.error(item);
                    });
                }
            }

            if (ctrl.gridSelectionMassApply != null) {
                ctrl.gridSelectionMassApply();
            }

        };

        ctrl.checkSelection = function () {

            var defer = $q.defer();

            if (ctrl.selectionCustom != null && ctrl.selectionCustom.getIsSelectedAll() === true) {
                $timeout(function () {
                    //ctrl.gridApi.selection.selectAllRows();

                    var rows = ctrl.gridApi.core.getVisibleRows(ctrl.gridApi.grid);

                    if (rows != null && rows.length > 0) {
                        rows.forEach(function (item) {
                            item.isSelected = ctrl.selectionCustom.unselectedRows.length > 0 ? ctrl.selectionCustom.indexOfUnselected(item.entity) === -1 : true;
                        });

                        ctrl.saveInStorageRows(rows);
                    } else {
                        ctrl.selectionCustom.clearSelectedRows();
                    }

                    defer.resolve(true);
                });
            } else {
                defer.resolve(false);
            }

            return defer.promise;
        };

        ctrl.selectionOnChange = function (rows) {

            ctrl.saveInStorageRows(rows);

            if (ctrl.gridSelectionOnChange != null) {
                ctrl.gridSelectionOnChange({ rows: rows });
            }
        };

        //#endregion

        ctrl.saveInStorageRows = function (rows) {
            var row,
                rowIdentity;

            for (var i = 0, len = rows.length; i < len; i++) {

                row = rows[i];
                rowIdentity = ctrl.gridOptions.saveRowIdentity(row.entity);

                if ((ctrl.selectionCustom.getIsSelectedAll() === false || ctrl.selectionCustom.unselectedRows.length > 0) && ctrl.storageStates != null && ctrl.storageStates.selection != null && ctrl.storageStates.selection.length > 0) {
                    for (var j = 0, lenj = ctrl.storageStates.selection.length; j < lenj; j++) {
                        if (rowIdentity === ctrl.storageStates.selection[j].row) {
                            if ((ctrl.selectionCustom.getIsSelectedAll() === true && ctrl.selectionCustom.indexOfUnselected(row.entity) !== -1) || (ctrl.selectionCustom.getIsSelectedAll() === false && row.isSelected === false)) {
                                ctrl.storageStates.selection[j] = null;
                            }
                        }
                    }

                    for (var k = 0, lenk = ctrl.storageStates.selection.length; k < lenk; k++) {
                        if (ctrl.storageStates.selection[k] == null) {
                            ctrl.storageStates.selection.splice(k, 1);
                        }
                    }
                }
            }

            ctrl.saveState();
        };

        ctrl.setSwitchEnabled = function (rowEntity, state, fieldName) {
            var oldValue = rowEntity[fieldName],
                newValue = state;
            rowEntity[fieldName] = state;

            uiGridCustomService.applyInplaceEditing(ctrl.gridInplaceUrl, ng.extend({}, uiGridCustomService.removeDuplicate(rowEntity, ctrl._params), rowEntity)).then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.GridCustom.ChangesSaved'));

                    if (ctrl.gridOnInplaceApply != null) {
                        ctrl.gridOnInplaceApply({ rowEntity: rowEntity, colDef: { name: fieldName }, newValue: newValue, oldValue: oldValue });
                    }
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.GridCustom.Error'), data.error != null ? data.error : $translate.instant('Admin.Js.GridCustom.ErrorWhileSaving'));
                }
            });
        };

        ctrl.paginationChange = function (paginationCurrentPage, paginationPageSize, paginationPageSizes) {

            ctrl.gridOptions.paginationCurrentPage = paginationCurrentPage;
            ctrl.gridOptions.paginationPageSize = paginationPageSize;

            ctrl._params.paginationCurrentPage = paginationCurrentPage;
            ctrl._params.paginationPageSize = paginationPageSize;

            ctrl.setParamsByUrl();
            ctrl.saveDataInStorage({
                paginationPageSize: paginationPageSize
            });



            return ctrl.fetchData();
        };

        ctrl.clickRow = function ($event, row, fn, url) {


            if (['a', 'input', 'textarea', 'button'].indexOf($event.target.tagName.toLowerCase()) !== -1
                || domService.closest($event.target, ['.ui-select-choices-row-inner', '.js-grid-not-clicked', '.ui-select-container'], $element[0]) != null
                || $event.target.querySelector('.js-grid-not-clicked') != null)
                return;

            if (fn != null) {
                fn($event, row, ctrl);
            }

            if (url != null && url.length > 0) {
                $window.location.assign($interpolate(url)({ row: row }));
            }
        };

        ctrl.setParams = function (params) {
            ng.extend(ctrl._params = ctrl._params || {}, uiGridCustomService.convertToClientParams(params));
            if (ctrl.gridPreventStateInHash !== true) {
                uiGridCustomService.setParamsByUrl(ctrl.gridUniqueId, ctrl._params);
            }
        };

        ctrl.clearParams = function () {
            ctrl._params = null;
            uiGridCustomService.clearParams(ctrl.gridUniqueId);
        };

        ctrl.setStateProcess = function (value) {
            ctrl.isProcessing = value;
        };

        ctrl.selectionOnRequestBefore = function () {
            ctrl.setStateProcess(true);
        };

        ctrl.clearSelectionInStorage = function () {
            if (ctrl.storageStates != null && ctrl.storageStates.selection != null) {
                ctrl.storageStates.selection.length = 0;
            }

        };

        //#region state
        ctrl.saveState = function () {

            var saveData = ctrl.gridApi.saveState.save(),
                prop,
                itemInProp,
                index;

            ctrl.storageStates = ctrl.storageStates || {};

            for (var key in saveData) {
                if (saveData.hasOwnProperty(key) === true) {

                    prop = saveData[key];

                    if (ng.isArray(prop) === true) {
                        for (var i = 0, len = prop.length; i < len; i++) {

                            itemInProp = prop[i];

                            if (ctrl.storageStates[key] != null) {
                                for (var j = 0, lenj = ctrl.storageStates[key].length; j < lenj; j++) {
                                    if ((ctrl.compareState[key] != null && ctrl.compareState[key](ctrl.storageStates[key][j], itemInProp)) || ng.equals(ctrl.storageStates[key][j], itemInProp) === true) {
                                        index = j;
                                        break;
                                    }
                                }
                            } else {
                                ctrl.storageStates[key] = [];
                            }

                            if (index != null && index !== -1) {
                                ctrl.storageStates[key][index] = itemInProp;
                            } else {
                                ctrl.storageStates[key].push(itemInProp);
                            }

                            index = null;
                        }
                    } else {
                        ctrl.storageStates[key] = saveData[key];
                    }
                }
            }

            return ctrl.storageStates;
        };

        ctrl.restoreState = function (onlySelection) {
            var result;

            if (ctrl.storageStates != null) {
                result = ctrl.gridApi.saveState.restore(null, onlySelection ? { selection: ctrl.storageStates.selection } : ctrl.storageStates)
            }

            return result;
        };

        ctrl.resetState = function () {
            ctrl.storageStates = {};
        };

        ctrl.compareState = {
            'selection': function (obj, otherObj) {
                return obj.row === otherObj.row;
            }
        }
        //#endregion

        ctrl.export = function () {
            uiGridCustomService.export(ctrl.gridUrl, ctrl.getRequestParams());
        };

        ctrl.getRequestParams = function () {
            return uiGridCustomService.convertToServerParams(ctrl._params);
        };

        ctrl.getParams = function () {
            return ctrl._params;
        };

        ctrl.bindGridApi = function (gridApi) {
            var destroyEditingAfter, destroySort, destroyColumnVisibilityChanged;

            ctrl.gridApi = gridApi;

            destroyEditingAfter = gridApi.uiGridEditCustom.on.change($scope, function (rowEntity, colDef, newValue, oldValue, callback) {
                var resultBefore;

                if (ctrl.gridOnInplaceBeforeApply != null) {
                    resultBefore = ctrl.gridOnInplaceBeforeApply({ rowEntity: rowEntity, colDef: colDef, newValue: newValue, oldValue: oldValue });

                    if (resultBefore === false) {
                        rowEntity[colDef.name] = oldValue;
                        return;
                    }
                }

                if (ctrl.gridInplaceUrl != null && ctrl.gridInplaceUrl.length > 0) {
                    uiGridCustomService.applyInplaceEditing(ctrl.gridInplaceUrl, ng.extend({}, uiGridCustomService.removeDuplicate(rowEntity, ctrl._params), rowEntity))
                        .then(function (data) {

                            if (data.result === true) {

                                toaster.pop('success', '', $translate.instant('Admin.Js.GridCustom.ChangesSaved'));

                                if (data.entity != null) {
                                    ng.extend(rowEntity, data.entity);
                                }

                                if (ctrl.gridOnInplaceApply != null) {
                                    ctrl.gridOnInplaceApply({ rowEntity: rowEntity, colDef: colDef, newValue: newValue, oldValue: oldValue });
                                }

                                if (callback != null) {
                                    callback(rowEntity, colDef, newValue, oldValue);
                                }

                            } else {
                                if (data.errors != null) {
                                    data.errors.forEach(function (error) {
                                        toaster.pop('error', $translate.instant('Admin.Js.GridCustom.Error'), error);
                                    });
                                } else {
                                    toaster.pop('error', $translate.instant('Admin.Js.GridCustom.Error'), data.error != null ? data.error : $translate.instant('Admin.Js.GridCustom.ErrorWhileSaving'));
                                }
                            }
                        })
                        .catch(function (response) {
                            toaster.error($translate.instant('Admin.Js.GridCustom.ErrorWhileUpdatingData'));
                            return response;
                        });
                }
            });

            destroySort = gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                if (sortColumns.length > 0) {
                    ctrl._params.sorting = sortColumns[0].name;
                    ctrl._params.sortingType = sortColumns[0].sort.direction;
                    ctrl.saveDataInStorage({ sorting: sortColumns[0].name, sortingType: sortColumns[0].sort.direction });
                } else {
                    delete ctrl._params.sorting;
                    delete ctrl._params.sortingType;
                    ctrl.saveDataInStorage({ sorting: null, sortingType: null });
                }

                ctrl.setParamsByUrl();
            });

            // ROWS RENDER

            $scope.$on('uiGridCustomAutoResize', function () {
                if (ctrl.gridApi) {
                    var prevWidth = ctrl.gridApi.grid.gridWidth;
                    var prevHeight = ctrl.gridApi.grid.gridHeight;
                    var width = gridUtil.elementWidth(ctrl.gridApi.grid.element);
                    var height = gridUtil.elementWidth(ctrl.gridApi.grid.element);

                    if (width > 0 && height > 0) {
                        ctrl.gridApi.grid.gridWidth = width;
                        ctrl.gridApi.grid.gridHeight = height;
                        ctrl.gridApi.grid.queueGridRefresh()
                            .then(function () {
                                ctrl.gridApi.core.raise.gridDimensionChanged(prevHeight, prevWidth, height, width);
                            });
                    }
                }
            });

            $element.on('$destroy', function () {
                uiGridCustomService.removeFromStorage(ctrl.gridUniqueId);

                destroyEditingAfter();
                destroySort();
                destroyColumnVisibilityChanged();

                gridApi.grid.appScope.$destroy();
            });


            destroyColumnVisibilityChanged = gridApi.core.on.columnVisibilityChanged($scope, function (changedColumn) {
                var storage = ctrl.getDataItemFromStorage();
                var enableHidingDictionary = storage != null ? storage.enableHiding || {} : {};
                enableHidingDictionary[changedColumn.colDef.name] = changedColumn.colDef.visible;
                ctrl.saveDataInStorage({ enableHiding: enableHidingDictionary });
            });

            gridApiReady.resolve(gridApi);
        };


        ctrl.optionsFromUrl = function () {
            var gridParamsByUrl = uiGridCustomService.getParamsByUrl(ctrl.gridUniqueId);

            if (gridParamsByUrl != null) {

                //#region set sorting on page load from url
                if (gridParamsByUrl.sorting != null) {
                    for (var i = 0, len = ctrl.gridOptions.columnDefs.length; i < len; i += 1) {
                        if (ctrl.gridOptions.columnDefs[i].name === gridParamsByUrl.sorting) {
                            ctrl.gridOptions.columnDefs[i].sort = {
                                direction: uiGridConstants[gridParamsByUrl.sortingType.toUpperCase()]
                            };
                            break;
                        }
                    }
                }
                //#endregion

                ng.extend(ctrl.gridOptions, uiGridCustomService.convertToClientParams(gridParamsByUrl));
                ng.extend(ctrl._params, uiGridCustomService.convertToClientParams(gridParamsByUrl));
            }
        };

        ctrl.defaultRowEntitySave = function (rowEntity) {
            //эта функция отвечает за генерацию уникального id для строки
            //нам нужно удалить поле $$hashKey в rowEntity, которое добавляет сам ангуляр
            //так как хэш будет каждый раз разный

            //для того чтобы не изменять искомый объект и не ломать логику ангуляра клонируем объект
            var clone = JSON.parse(JSON.stringify(rowEntity));

            //удаляем уникальный хэш
            delete clone.$$hashKey;

            return JSON.stringify(clone);
        };

        ctrl.getKey = function () {
            return $window.location.pathname + '::' + ctrl.gridUniqueId;
        };

        ctrl.getDataItemFromStorage = function () {
            return uiGridCustomService.getDataItimFromStorageByKey(ctrl.getKey());
        };

        ctrl.saveDataInStorage = function (data) {
            uiGridCustomService.saveDataInStorage(ctrl.getKey(), data);
        };

        ctrl.hideColumn = function (columnName) {
            ctrl.toggleVisibleColumn(columnName, false);
        };

        ctrl.showColumn = function (columnName) {
            ctrl.toggleVisibleColumn(columnName, true);
        };

        ctrl.toggleVisibleColumn = function (columnName, isVisible) {
            var columns = ctrl.gridOptions.columnDefs.filter(function (col) { return col.name === columnName });

            if (columns != null && columns.length > 0) {
                columns[0].visible = isVisible;
            }
        };

        ctrl.deleteItem = function (rowEntity) {

            if (ctrl.gridApi != null && ctrl.gridApi.selection != null) {
                ctrl.gridApi.selection.unSelectRow(rowEntity, null);
            }

            return ctrl.fetchData(true)
                .then(function () {
                    if (ctrl.gridOnDelete != null) {
                        ctrl.gridOnDelete();
                    }
                });
        };

        ctrl.findColDefsWithMediaQueries = function () {
            var colDefs = ctrl.gridOptions.columnDefs.filter(function (col) { return col.visible != null && (typeof col.visible === 'number' || col.visible.breakpoint != null); });
            var mqObj = null;

            if (colDefs.length > 0) {
                mqObj = {};
                var key, isNumber;
                colDefs.forEach(function (col) {

                    isNumber = typeof col.visible === 'number';

                    key = isNumber === true ? col.visible : col.visible.breakpoint;

                    mqObj[key] = mqObj[key] || {};
                    mqObj[key].cols = mqObj[key].cols || [];
                    mqObj[key].cols.push(col.name);

                    if (isNumber === false) {
                        mqObj[key].customFn = col.visible.customFn;
                    }
                });
            }

            return mqObj;
        };

        ctrl.processColDefByMediaQueries = function (windowWidth, colDefsMediaQueries) {
            Object.keys(colDefsMediaQueries).forEach(function (breakpoint) {
                Object.keys(colDefsMediaQueries).forEach(function (item) {
                    colDefsMediaQueries[breakpoint].cols.forEach(function (colName) {

                        var breakpointActive = windowWidth > parseFloat(breakpoint);

                        $q.when(colDefsMediaQueries[breakpoint].customFn != null ? colDefsMediaQueries[breakpoint].customFn(breakpointActive) : breakpointActive)
                            .then(function (isMatch) {
                                ctrl.toggleVisibleColumn(colName, isMatch != null ? isMatch : breakpointActive);
                            });
                    });
                });
            });
        };

        ctrl.addListenerForColDefsMediaQueries = function (colDefsMediaQueries) {
            if (colDefsMediaQueries != null) {

                ctrl.processColDefByMediaQueries($window.innerWidth, colDefsMediaQueries);

                var resizeFn = debounce(function () {
                    ctrl.processColDefByMediaQueries($window.innerWidth, colDefsMediaQueries);
                    ctrl.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
                }, 700);

                $window.addEventListener('resize', resizeFn);
            }
        };
    };

    UiGridCustomCtrl.$inject = ['$element', '$document', '$interpolate', '$location', '$scope', '$timeout', '$q', '$window', '$locale', 'uiGridConstants', 'i18nService', 'uiGridEditService', 'uiGridCustomService', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'toaster', 'domService', '$translate', '$transclude', 'gridUtil'];

    ng.module('uiGridCustom', [
        'ui.grid',
        'ui.grid.edit',
        'ui.grid.selection',
        'ui.grid.cellNav',
        'ui.grid.autoResize',
        'ui.grid.grouping',
        'ui.grid.treeView',
        'ui.grid.saveState',
        'uiGridCustomFilter',
        'uiGridCustomPagination',
        'uiGridCustomSelection',
        'uiGridCustomEdit',
        'switchOnOff',
        'toaster',
        'dom'])
        .controller('UiGridCustomCtrl', UiGridCustomCtrl);


    function debounce(func, ms) {

        var timer;

        return function () {

            if (timer != null) {
                clearTimeout(timer);
            }

            var vm = this;
            var args = arguments;

            timer = setTimeout(function () {
                func.apply(vm, args);
            }, ms);
        };
    }

})(window.angular);