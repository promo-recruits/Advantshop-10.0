; (function (ng) {
    'use strict';

    ng.module('validation')
        .filter('validationUnique', function () {
            return function (collection) {
                var result = {};
                var tempList = [];
                var data = unboxing(collection);

                Object.keys(data).forEach(function (key) {
                    tempList = [];

                    data[key].reduce(function (accumulator, currentValue) {
                        if (isUnique(currentValue, accumulator) === true) {
                            accumulator.push(currentValue);
                        }
                        return accumulator;
                    }, tempList);

                    result[key] = tempList;
                });

                return result;
            };
        });

    function unboxing(collection) {
        var result = {};

        Object.keys(collection).forEach(function (key) {
            collection[key].forEach(function (item) {
                if (item.constructor.name === 'FormController') {
                    result = merge(result, unboxing(item.$error));
                } else {
                    result[key] = result[key] || [];
                    result[key].push(item);
                }
            });
        });

        return result;
    }


    function merge(source, dest) {
        var cloneDest = ng.copy(dest);

        Object.keys(source).forEach(function (key) {
            if (cloneDest[key] != null) {
                source[key] = source[key].concat(cloneDest[key]);
                delete cloneDest[key];
            }
        });

        Object.keys(cloneDest).forEach(function (key) {
            source[key] = cloneDest[key];
        });

        return source;
    }

    function isUnique(error, array) {
        return error.validationInputText != null && array.some(function (item) { return item.validationInputText === error.validationInputText; }) !== true;
    }

})(window.angular);