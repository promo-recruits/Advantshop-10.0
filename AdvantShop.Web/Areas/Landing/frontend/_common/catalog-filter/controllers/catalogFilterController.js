; (function (ng) {

    'use strict';

    var isIE = /Windows Phone|iemobile|WPDesktop/.test(navigator.userAgent);

    var CatalogFilterCtrl = function ($http, $window, $timeout, popoverService, domService, catalogFilterService, catalogFilterAdvPopoverOptionsDefault) {

        var ctrl = this,
            pageParameters,
            timerPopoverHide,
            timerRange;

        ctrl.$onInit = function () {

            pageParameters = catalogFilterService.parseSearchString($window.location.search);

            ctrl.isIE = isIE;

            ctrl.countVisibleCollapse = ctrl.countVisibleCollapse() || 10;

            ctrl.collapsed = true;
            ctrl.isRenderBlock = false;
            ctrl.isLoaded = false;

            ctrl.itemsOptions = [];

            ctrl.advPopoverOptions = ng.extend({}, catalogFilterAdvPopoverOptionsDefault, ctrl.advPopoverOptions);

            ctrl.init();

            if (ctrl.onInit != null) {
                ctrl.onInit({ catalogFilter: ctrl });
            }
        };

        ctrl.init = function() {
            ctrl.getFilterData().then(function (catalogFilterData) {

                ctrl.catalogFilterData = catalogFilterData.map(function (filter) {
                    filter.dirty = false;
                    return filter;
                });
                ctrl.isRenderBlock = catalogFilterData != null && catalogFilterData.length > 0;
                ctrl.isLoaded = true;
                if (ctrl.onFilterInit) {
                    ctrl.onFilterInit({ visible: ctrl.isRenderBlock });
                }

                //Fill all <select> ng-model by properly <option> on filter initialization
                var selectIndex;
                for (var i = 0, length = catalogFilterData.length; i < length; i++) {
                    if (catalogFilterData[i] != null && (catalogFilterData[i].Control == "select" || catalogFilterData[i].Control == "selectSearch")) {
                        for (var j = 0, len = catalogFilterData[i].Values.length; j < len; j++) {
                            if (catalogFilterData[i].Values[j].Selected) {
                                selectIndex = j;
                                break;
                            }
                        }
                        catalogFilterData[i].Selected = catalogFilterData[i].Values[selectIndex];
                        selectIndex = -1;
                    } else {
                        ctrl.itemsOptions[i] = {
                            countVisibleItems: ctrl.countVisibleCollapse,
                            collapsed: true
                        };
                    }
                }

                catalogFilterService.saveFilterData(catalogFilterData);
            });
        }

        ctrl.getCssClassForContent = function (controlType) {

            var cssClasses = {};

            cssClasses['catalog-filter-block-content-' + controlType] = true;

            return cssClasses;
        };

        ctrl.inputKeypress = function ($event, indexFilter) {
            if (timerPopoverHide != null) {
                $timeout.cancel(timerPopoverHide);
            }

            timerPopoverHide = $timeout(function () {
                var element = $event.currentTarget.parentNode;

                if (element != null) {
                    ctrl.changeItem(element, indexFilter);
                }
            }, 1200);
        };

        ctrl.clickCheckbox = function ($event, indexFilter) {

            var element = domService.closest($event.target, '.catalog-filter-row');

            if (element != null) {
                ctrl.changeItem(element, indexFilter);
            }
        };

        ctrl.clickSelect = function ($event, indexFilter) {

            var element = $event.currentTarget.parentNode.parentNode;

            if (element != null) {
                ctrl.changeItem(element, indexFilter);
            }
        };

        ctrl.clickRangeDown = function (event, originalEvent, indexFilter) {
            ctrl.rangeElementClicked = originalEvent.target;
        };

        ctrl.clickRange = function (event, indexFilter) {
            if (timerRange != null) {
                $timeout.cancel(timerRange);
            }

            timerRange = $timeout(function () {
                var element = domService.closest(event.target, '.js-range-slider-block');

                if (element != null) {
                    ctrl.changeItem(element, indexFilter);
                }

                ctrl.rangeElementClicked = null;

            }, 500);
        };

        ctrl.changeColor = function (event, indexFilter) {
            var element = domService.closest(event.target, '.js-color-viewer');

            if (element != null) {
                ctrl.changeItem(element, indexFilter);
            }
        };

        ctrl.changeItem = function (element, indexFilter) {
            var selectedItems, params;
            if (indexFilter != null) {
                ctrl.catalogFilterData[indexFilter].dirty = true;
            }
        };

        ctrl.toggleVisible = function (totalItems, index) {
            ctrl.itemsOptions[index].countVisibleItems = ctrl.itemsOptions[index].collapsed === true ? totalItems : ctrl.countVisibleCollapse;
            ctrl.itemsOptions[index].collapsed = !ctrl.itemsOptions[index].collapsed;
        };

        ctrl.reset = function () {
            ctrl.filterSelectedData = null;
            ctrl.filter();

            ctrl.init();
        };

        ctrl.submit = function () {
            ctrl.filterSelectedData = catalogFilterService.getSelectedData(ctrl.catalogFilterData);
            ctrl.filter();
        };

        ctrl.getFilterCount = function (filterString) {
            return $http.get(ctrl.urlCount + (filterString != null && filterString.length > 0 ? '?' + filterString : ''), { params: ng.extend(ctrl.parameters(), { rnd: Math.random() }) }).then(function (response) {
                return response.data;
            });
        };

        ctrl.getFilterData = function () {

            var params = {
                hideFilterByPrice: ctrl.hideFilterByPrice,
                hideFilterByBrand: ctrl.hideFilterByBrand,
                hideFilterByColor: ctrl.hideFilterByColor,
                hideFilterBySize: ctrl.hideFilterBySize,
                hideFilterByProperty: ctrl.hideFilterByProperty
            };

            return $http.get(ctrl.url, { params: ng.extend(pageParameters, ctrl.parameters(), params, { rnd: Math.random() }) }).then(function (response) {
                return response.data;
            });
        };
    };

    ng.module('catalogFilter')
      .controller('CatalogFilterCtrl', CatalogFilterCtrl);

    CatalogFilterCtrl.$inject = ['$http', '$window', '$timeout', 'popoverService', 'domService', 'catalogFilterService', 'catalogFilterAdvPopoverOptionsDefault'];

})(window.angular);