; (function (ng) {
    'use strict';

    var UiGridCustomFilterBlockCtrl = function ($timeout, $filter, $http) {
        var ctrl = this,
            timer;

        ctrl.$onInit = function () {
            ctrl.blockUrl = '../areas/admin/content/src/_shared/ui-grid-custom/templates/filter-types/' + ctrl.blockType + '.html';
        };

        ctrl.$postLink = function () {
            if (ctrl.blockType === 'datetime') {
                ctrl.applyDatetime(ctrl.item);
            } else if (ctrl.blockType === 'date') {
                ctrl.applyDate(ctrl.item);
            } else if (ctrl.blockType === 'range' && ctrl.item.filter.rangeOptions && ctrl.item.filter.term != null) {
                ctrl.apply([{ name: ctrl.item.filter.rangeOptions.from.name, value: ctrl.item.filter.term.from }, { name: ctrl.item.filter.rangeOptions.to.name, value: ctrl.item.filter.term.to }], ctrl.item, true);
            } else if ((ctrl.blockType === 'input' || ctrl.blockType === 'number') && ctrl.item.filter.term != null) {
                ctrl.apply([{ name: ctrl.item.filter.name, value: ctrl.item.filter.term }], ctrl.item, true);
            }
        };

        ctrl.close = function (name, item) {
            ctrl.onClose({ name: name, item: item });
        };

        ctrl.apply = function (params, item, debounce) {

            if (debounce === true) {

                if (timer != null) {
                    $timeout.cancel(timer);
                }

                timer = $timeout(function () {
                    ctrl.onApply({ params: params, item: item });
                }, 300);
            } else {
                ctrl.onApply({ params: params, item: item });
            }
        };

        ctrl.applyDate = function (item) {
            ctrl.apply([{ name: item.filter.dateOptions.from.name, value: item.filter.term.from},
                        { name: item.filter.dateOptions.to.name, value: item.filter.term.to}], item, true);
        };

        ctrl.applyDatetime = function (item) {
            ctrl.apply([{ name: item.filter.datetimeOptions.from.name, value: item.filter.term.from },
                { name: item.filter.datetimeOptions.to.name, value: item.filter.term.to }], item, true);
        };
        
        ctrl.filterSearch = function(search, item) {
            
            if (!item.filter.dynamicSearch || search == null || item.filter.fetch == null) {
                return;
            }
            
            if (item.filter.prevSearch == null || item.filter.prevSearch === search){
                item.filter.prevSearch = search;
                return;
            }
            
            let params = {
                q: search
            };
            params[item.filter.name || item.name] = item.filter.term;
            
            if (item.filter.dynamicSearchRelations != null && item.filter.dynamicSearchRelations.length > 0){
                for(let i = 0; i < item.filter.dynamicSearchRelations.length; i++){
                    let key = item.filter.dynamicSearchRelations[i];
                    params[key] = ctrl.getGridValue(key);
                }
            }
            
            $http.get(item.filter.fetch, { params: params }).then(function (response) {
                if (response.data != null) {
                    item.filter.selectOptions = response.data;
                    item.filter.prevSearch = search;
                }
            });
        }

        ctrl.getGridValue = function(key) {

            try {
                key = key.toLowerCase();
                let grid = {};                
                const gridString = decodeURI(window.location.hash.substring(2).toLowerCase()); 
                
                eval(gridString);
                
                for (let name in grid){
                    if (name == key)
                        return grid[name];
                }
            } catch (e){ 
                //console.log(e);
            }
            return null;
        }
    };

    UiGridCustomFilterBlockCtrl.$inject = ['$timeout', '$filter', '$http'];

    ng.module('uiGridCustomFilter')
        .controller('UiGridCustomFilterBlockCtrl', UiGridCustomFilterBlockCtrl);

})(window.angular);