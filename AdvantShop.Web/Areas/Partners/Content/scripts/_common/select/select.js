; (function (ng) {
    'use strict';

    ng.module('select', [])
        .directive('select', ['$parse', '$timeout', function ($parse, $timeout) {
            return {
                require: '?ngModel',
                restrict: 'E',
                link: function (scope, element, attrs, ctrl) {
                    if (ctrl != null && !attrs.ngOptions && attrs.disabledAutobind == null && (ctrl.$modelValue === undefined || isNaN(ctrl.$modelValue))) {
                        $parse(attrs.ngModel).assign(scope, element.val());
                    }

                    if (attrs.onChange != null && attrs.onChange.length > 0) {

                        var onChangeCallback = $parse(attrs.onChange),
                            onChangeHandler = function (event) {
                                onChangeCallback(scope, { event: event });
                            };

                        element.on('change', onChangeHandler);

                        element.on('$destroy', function () {
                            element.off('change', onChangeHandler);
                        });
                    }
                }
            };
        }])
        .directive('convertToNumber', function () {
            return {
                require: 'ngModel',
                link: function (scope, element, attrs, ngModel) {
                    ngModel.$parsers.push(function (val) {
                        return ng.isArray(val)
                            ? val.map(function (item) {
                                return parseFloat(item, 10);
                            })
                            : parseFloat(val, 10);
                    });
                    ngModel.$formatters.push(function (val) {
                        return ng.isArray(val) ? val.map(function (item) {
                            return item != null ? '' + item : item;
                        }) : val != null ? '' + val : val;
                    });
                }
            };
        })
     .directive('convertToBool', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                ngModel.$parsers.push(function (val) {
                    return val.toLowerCase() === 'true';
                });
                ngModel.$formatters.push(function (val) {
                    return val != null ? (val === true ? 'True' : 'False') : val;
                });
            }
        };
    });

})(window.angular);