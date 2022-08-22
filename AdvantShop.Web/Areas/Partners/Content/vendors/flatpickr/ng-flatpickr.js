(function (root, factory) {
    'use strict';
    root['angular-flatpickr'] = factory(root.angular, root.flatpickr);
}(this, function (angular, flatpickr) {

    'use strict';

    var ngModelFormatFn = function (flatpickr, ngFlatpickrFilter) {
        return function (value) {

            var result;

            if (value == null || value === '') {
                result = value;
            } else {
                if (angular.isString(value) === true && flatpickr.config.startDateFormat == null) {
                    console.warn('Option "startDateFormat" required');
                }

                result = ngFlatpickrFilter(value, flatpickr.config.dateFormat, flatpickr.config.startDateFormat);
            }

            return result;
        };
    };

    var ngModelParseFn = function (flatpickr, ngFlatpickrFilter) {
        return function (value) {
            return value == null || value.length === 0 ? value : ngFlatpickrFilter(value, flatpickr.config.startDateFormat, flatpickr.config.dateFormat);
        };
    };


    var ngFlatpickr = angular.module('angular-flatpickr', []);


    ngFlatpickr.constant('ngFlatpickrDefaultOptions', {
        time_24hr: true,
        allowInput: true
    });

    ngFlatpickr.directive('ngFlatpickr', ['$q', '$filter', '$timeout', 'ngFlatpickrDefaultOptions', function ($q, $filter, $timeout, ngFlatpickrDefaultOptions) {
        return {
            require: {
                ngModel: '?ngModel'
            },
            restrict: 'A',
            scope: {
                fpOpts: '<?',
                fpOnSetup: '&',
                fpOnChange: '&',
                fpOnOpen: '&'
            },
            bindToController: true,
            controllerAs: 'ngFlatpickr',
            controller: ['$element', '$scope', function ($element, $scope) {
                var ctrl = this;

                var defer = $q.defer();

                ctrl.whenNgFlatpickrInitilazed = function () {
                    return defer.promise;
                };

                ctrl.addNgFlatpickrInput = function (ngFlatpickrInput) {
                    ctrl.ngFlatpickrInput = ngFlatpickrInput;
                };

                ctrl.$postLink = function () {

                    var ngModelParseFnCurrent;
                    var ngModelFormatFnCurrent;

                    var options = angular.extend({}, ngFlatpickrDefaultOptions, ctrl.fpOpts || {});

                    options.onChange = function (selectedDates, dateStr, instance) {
                        ctrl.needUpdate = false;

                        if (ctrl.fpOnChange != null) {
                            $timeout(function () {
                                ctrl.fpOnChange({ selectedDates: selectedDates, dateStr: dateStr, instance: instance });
                            });
                        }
                    };

                    options.onValueUpdate = function (selectedDates, dateStr, instance) {
                        var ngModel = ctrl.ngModel || ctrl.ngFlatpickrInput.ngModel;

                        //когда инпут пустой и щелкаешь по нему первый раз, то вставл€етс€ дата
                        if (ngModel != null && ngModel.modelValue == null) {
                            ngModel.$setViewValue(dateStr);
                        }
                    };

                    options.onOpen = function (selectedDates, dateStr, instance) {
                        ctrl.needUpdate = false;

                        if (ctrl.fpOnOpen != null) {
                            $timeout(function () {
                                ctrl.fpOnOpen({ selectedDates: selectedDates, dateStr: dateStr, instance: instance });
                            });
                        }
                    };

                    ctrl.flatpickr = new flatpickr($element[0], options);

                    if (ctrl.fpOnSetup) {
                        ctrl.fpOnSetup({
                            fpItem: ctrl.flatpickr
                        });
                    }

                    // destroy the flatpickr instance when the dom element is removed
                    $element.on('$destroy', function () {
                        ctrl.flatpickr.destroy();
                        var indexFormatFn;
                        var indexParseFn;

                        if (ngModelFormatFnCurrent) {
                            indexFormatFn = ctrl.ngModel.$formatters.indexOf(ngModelFormatFnCurrent);

                            if (indexFormatFn !== -1) {
                                ctrl.ngModel.$formatters.splice(indexFormatFn, 1);
                            }
                        }

                        if (ngModelParseFnCurrent) {
                            indexParseFn = ctrl.ngModel.$parsers.indexOf(ngModelParseFnCurrent);

                            if (indexParseFn !== -1) {
                                ctrl.ngModel.$parsers.splice(indexParseFn, 1);
                            }
                        }
                    });

                    if (ctrl.ngModel != null) {
                        ngModelFormatFnCurrent = ngModelFormatFn(ctrl.flatpickr, $filter('ngFlatpickr'));
                        ngModelParseFnCurrent = ngModelParseFn(ctrl.flatpickr, $filter('ngFlatpickr'));

                        ctrl.ngModel.$formatters.push(ngModelFormatFnCurrent);
                        ctrl.ngModel.$parsers.push(ngModelParseFnCurrent);

                        ctrl.ngModel.$render = function () {
                            ctrl.flatpickr.setDate(ctrl.ngModel.$viewValue, false);
                        };

                        if (ctrl.ngModel.$viewValue != null && angular.isString(ctrl.ngModel.$viewValue) === true && ctrl.ngModel.$viewValue.length > 0) {
                            flatpickr.setDate($filter('ngFlatpickr')(ctrl.ngModel.$viewValue, flatpickr.config.dateFormat, flatpickr.config.startDateFormat), false);
                        }
                    }

                    if ($element[0].tagName.toLowerCase() === 'input') {
                        $element.on('keyup', function () {
                            ctrl.needUpdate = true;
                        });

                        $element.on('blur', function () {
                            var el = this;

                            $timeout(function () {
                                if (ctrl.needUpdate === true) {
                                    flatpickr.setDate(el.value, true);
                                }
                            }, 10);
                        });
                    }

                    defer.resolve(ctrl.flatpickr);
                };
            }],
        };
    }]);

    ngFlatpickr.directive('ngFlatpickrInput', ['$filter', '$timeout', function ($filter, $timeout) {
        return {
            require: {
                ngModel: 'ngModel',
                ngFlatpickr: '^ngFlatpickr'
            },
            restrict: 'A',
            bindToController: true,
            controllerAs: 'ngFlatpickrInput',
            controller: ['$element', function ($element) {
                var ctrl = this;

                ctrl.$postLink = function () {

                    $element.attr('data-input', '');

                    ctrl.ngFlatpickr.whenNgFlatpickrInitilazed().then(function (flatpickr) {

                        var ngModelFormatFnCurrent;
                        var ngModelParseFnCurrent;

                        ngModelFormatFnCurrent = ngModelFormatFn(flatpickr, $filter('ngFlatpickr'));
                        ngModelParseFnCurrent = ngModelParseFn(flatpickr, $filter('ngFlatpickr'));

                        ctrl.ngModel.$formatters.push(ngModelFormatFnCurrent);
                        ctrl.ngModel.$parsers.push(ngModelParseFnCurrent);

                        $element.on('$destroy', function () {
                            var indexFormatFn;
                            var indexParseFn;

                            if (ngModelFormatFnCurrent) {
                                indexFormatFn = ctrl.ngModel.$formatters.indexOf(ngModelFormatFnCurrent);

                                if (indexFormatFn !== -1) {
                                    ctrl.ngModel.$formatters.splice(indexFormatFn, 1);
                                }
                            }

                            if (ngModelParseFnCurrent) {
                                indexParseFn = ctrl.ngModel.$parsers.indexOf(ngModelParseFnCurrent);

                                if (indexParseFn !== -1) {
                                    ctrl.ngModel.$parsers.splice(indexParseFn, 1);
                                }
                            }
                        });

                        ctrl.ngModel.$render = function () {
                            flatpickr.setDate(ctrl.ngModel.$viewValue, false);
                        };

                        $element.on('keyup', function () {
                            ctrl.ngFlatpickr.needUpdate = true;
                        });

                        $element.on('blur', function () {
                            var el = this;

                            $timeout(function () {
                                if (ctrl.ngFlatpickr.needUpdate === true) {
                                    flatpickr.setDate(el.value, true);
                                }
                            }, 10);
                        });

                        if (ctrl.ngModel.$viewValue != null && angular.isString(ctrl.ngModel.$viewValue) === true && ctrl.ngModel.$viewValue.length > 0) {
                            flatpickr.setDate($filter('ngFlatpickr')(ctrl.ngModel.$viewValue, flatpickr.config.dateFormat, flatpickr.config.startDateFormat), false);
                        }

                        ctrl.ngFlatpickr.addNgFlatpickrInput(ctrl);
                    });
                };
            }]
        };
    }]);

    ngFlatpickr.filter('ngFlatpickr', function () {
        return function (value, format, formatParse) {
            var valueParsed;

            if (value == null) {
                return value;
            }

            if (angular.isString(value) === true) {
                if (formatParse == null || formatParse.length === 0) {
                    console.warn('Missing required filter parameter "formatParse" for parse date string');
                } else {
                    valueParsed = flatpickr.parseDate(value, formatParse);
                }
            } else if (angular.isDate(value) === true) {
                valueParsed = value;
            } else {
                console.warn('Unsupport variable type for filter ngFlatpickr');
                return value;
            }

            return flatpickr.formatDate(valueParsed || value, format);
        };
    });

    return ngFlatpickr;

}));
