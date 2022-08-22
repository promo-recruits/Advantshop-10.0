﻿; (function (ng) {

    'use strict';

    var catalogFilterService = function () {

        var service = this,
            filterStorage;



        service.buildUrl = function (filterSelectedItems) {
            var result = [], obj;

            for (var item in filterSelectedItems) {

                if (ng.isArray(filterSelectedItems[item]) === true) {
                    obj = item + '=' + filterSelectedItems[item].map(function (val) {
                        return ng.isArray(val) ? val.join(',') : val;
                    }).join('-');
                } else {
                    obj = item + '=' + filterSelectedItems[item];
                }

                result.push(obj);
            }

            return result.join('&');
        };

        service.getSelectedData = function (filterData) {

            if (filterData == null) {
                return null;
            }

            var arraySelected,
                selectedItems,
                item,
                nameRangeMin, nameRangeMax;

            for (var i = filterData.length - 1; i >= 0; i--) {

                item = filterData[i];

                if (item == null)
                    continue;

                arraySelected = arraySelected || {};

                //ищем выбранные значения
                if (item.Control == 'select' || item.Control == 'selectSearch') {
                    if (item.Selected != null && item.Selected.Id !== '0') {
                        selectedItems = item.Selected.Id;
                    }
                }
                else {
                    selectedItems = item.Values.filter(function (item) { return item.Selected }).map(function (item) { return item.Id; });
                }

                //добавляем эти значения в массив
                if (selectedItems != null && selectedItems.length > 0) {

                    arraySelected[item.Type] = arraySelected[item.Type] || [];

                    if (item.Type != 'prop') {
                        arraySelected[item.Type] = arraySelected[item.Type].concat(selectedItems);
                    } else {
                        arraySelected[item.Type].push(selectedItems);
                    }
                }

                //добавляем текст для поиска
                if (item.Control == 'input' && item.Text.length > 0) {
                    arraySelected["q"] = [item.Text];
                }

                //добавляем значения из ползунков 
                if (item.Control == 'range' && item.dirty) {

                    if (item.Type == 'price') {
                        nameRangeMin = 'pricefrom';
                        nameRangeMax = 'priceto';

                        arraySelected[nameRangeMin] = item.Values[0].CurrentMin;
                        arraySelected[nameRangeMax] = item.Values[0].CurrentMax;

                    } else {
                        var propRangeItem = {
                            id: item.Values[0].Id,
                            min: item.Values[0].CurrentMin,
                            max: item.Values[0].CurrentMax
                        };

                        arraySelected['propRange'] = arraySelected['propRange'] || [];
                        arraySelected['propRange'].push(propRangeItem);
                    }
                }
            }

            return arraySelected;
        };

        service.parseSearchString = function (str) {
            var index = str.indexOf('?'),
                strNormalize = str,
                parameters = {},
                arrayKeyValues,
                temp;

            if (index > -1) {
                strNormalize = strNormalize.substring(index + 1);
            }

            arrayKeyValues = strNormalize.split('&');

            for (var i = arrayKeyValues.length - 1; i >= 0; i--) {
                temp = arrayKeyValues[i].split('=');

                if (temp.length === 2) {
                    parameters[decodeURIComponent(temp[0])] = decodeURIComponent(temp[1]);
                }
            }

            return parameters;
        };

        service.saveFilterData = function (filter) {
            filterStorage = filter;
        };

        service.getFilterData = function () {
            return filterStorage;
        };
    };

    ng.module('catalogFilter')
      .service('catalogFilterService', catalogFilterService);

    catalogFilterService.$inject = [];

})(window.angular);