(function (root, factory) {
    'use strict';
    root['angular-flatpickr'] = factory(root.angular, root.flatpickr);
}(this, function (angular, flatpickr) {

    'use strict';

var maskIsCompleteOptional = function (maskControl) {
    return maskControl == null || maskControl.maskOriginal.masked.isComplete;
}

var ngModelFormatFn = function (flatpickr, ngFlatpickrFilter, maskControl) {
    return function (value) {

        var result;

        if (value == null || value === '' || (maskControl != null && maskControl.maskOriginal.masked.isComplete === false)) {
            result = value;
        }
        else {
            if (angular.isString(value) === true && flatpickr.config.startDateFormat == null) {
                console.warn('Option "startDateFormat" required');
            }

            result = ngFlatpickrFilter(value, flatpickr.config.dateFormat, flatpickr.config.startDateFormat);
        }

        return result;
    };
};

var ngModelParseFn = function (flatpickr, ngFlatpickrFilter, maskControl) {
    return function (value) {
        return value == null || value.length === 0 || (maskControl != null && maskControl.maskOriginal.masked.isComplete === false) ? value : ngFlatpickrFilter(value, flatpickr.config.startDateFormat, flatpickr.config.dateFormat);
    };
};


var ngFlatpickr = angular.module('angular-flatpickr', []);


ngFlatpickr.constant('ngFlatpickrDefaultOptions', {
    time_24hr: true,
    allowInput: true
});

    ngFlatpickr.directive('ngFlatpickr', ['$document', '$filter', '$timeout', 'domService', 'ngFlatpickrDefaultOptions', function ($document, $filter, $timeout, domService, ngFlatpickrDefaultOptions) {
    return {
        require: {
            ngModel: '?ngModel',
            maskControl: '?maskControl'
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

            ctrl.addNgFlatpickrInput = function (ngFlatpickrInput) {
                ctrl.ngFlatpickrInput = ngFlatpickrInput;
            };

            ctrl.processNgModel = function (flatpickr, ngModel, maskControl) {
                var ngModelParseFnCurrent;
                var ngModelFormatFnCurrent;

                ngModelFormatFnCurrent = ngModelFormatFn(flatpickr, $filter('ngFlatpickr'), maskControl);
                ngModelParseFnCurrent = ngModelParseFn(flatpickr, $filter('ngFlatpickr'), maskControl);

                ngModel.$formatters.push(ngModelFormatFnCurrent);
                ngModel.$parsers.push(ngModelParseFnCurrent);

                ngModel.$render = function () {
                    if (maskIsCompleteOptional(maskControl)) {
                        flatpickr.setDate(ngModel.$viewValue, false);
                    }
                };

                if (ngModel.$viewValue != null && angular.isString(ngModel.$viewValue) === true && ngModel.$viewValue.length > 0 && maskIsCompleteOptional(maskControl)) {
                    flatpickr.setDate($filter('ngFlatpickr')(ngModel.$viewValue, flatpickr.config.dateFormat, flatpickr.config.startDateFormat), false);
                }

                return function () {
                    var indexFormatFn;
                    var indexParseFn;

                    if (ngModelFormatFnCurrent) {
                        indexFormatFn = ngModel.$formatters.indexOf(ngModelFormatFnCurrent);

                        if (indexFormatFn !== -1) {
                            ngModel.$formatters.splice(indexFormatFn, 1);
                        }
                    }

                    if (ngModelParseFnCurrent) {
                        indexParseFn = ngModel.$parsers.indexOf(ngModelParseFnCurrent);

                        if (indexParseFn !== -1) {
                            ngModel.$parsers.splice(indexParseFn, 1);
                        }
                    }
                }
            }

            ctrl.bindElementEvents = function (inputElement, maskControl) {
                if (inputElement != null) {
                    var documentClick = function (event) {
                        if (ctrl.flatpickr.isOpen && ctrl.flatpickr.input !== event.target && domService.closest(event.target, ctrl.flatpickr.calendarContainer) == null) {
                            ctrl.flatpickr.setDate(inputElement.val(), true);
                        }
                    };

                    inputElement.on('keyup', function () {
                        ctrl.needUpdate = true;
                    });

                    $document[0].addEventListener('mousedown', documentClick);

                    inputElement.on('blur', function (event) {
                        var el = this;
                        if (ctrl.needUpdate === true && maskIsCompleteOptional(maskControl)) {
                            $timeout(function () {
                                ctrl.flatpickr.setDate(el.value, true);
                            }, 10);
                        } else {
                            //отменяем вызов события потери фокуса у flatpickr, так как он упадает из-за плейсхолдера маски
                            event.stopImmediatePropagation();
                        }
                    });
                    return function () {
                        inputElement.off();
                        $document[0].removeEventListener('mousedown', documentClick);
                    }
                } else {
                    return function () { };
                }
            }

            function removeMaskPlaceholder(input, ngModel, maskControl) {
                var startVal;

                if ((isNaN(ngModel.$modelValue) || ngModel.$modelValue == null) && input != null && maskControl != null) {
                    startVal = input.val();
                    input.val('');

                    return function () {
                        setTimeout(() => input.val(startVal), 0);
                    }
                } else {
                    return function () { }
                }
            }

            ctrl.$postLink = function () {
                var input = ctrl.ngFlatpickrInput != null ? ctrl.ngFlatpickrInput.$element : $element[0].tagName.toLowerCase() === 'input' ? $element : null;
                var maskControl = ctrl.ngFlatpickrInput != null && ctrl.ngFlatpickrInput.maskControl != null ? ctrl.ngFlatpickrInput.maskControl : ctrl.maskControl;
                var ngModel = ctrl.ngFlatpickrInput != null ? ctrl.ngFlatpickrInput.ngModel : ctrl.ngModel;
                var options = angular.extend({}, ngFlatpickrDefaultOptions, ctrl.fpOpts || {});


                if (typeof options.appendTo === 'string') {
                    options.appendTo = document.querySelector(options.appendTo);

                }

                options.onChange = function (selectedDates, dateStr, instance) {
                    ctrl.needUpdate = false;

                    if (ctrl.fpOnChange != null) {
                        $timeout(function () {
                            ctrl.fpOnChange({ selectedDates: selectedDates, dateStr: dateStr, instance: instance });
                        });
                    }
                };

                options.onValueUpdate = function (selectedDates, dateStr, instance) {
                    //когда инпут пустой и щелкаешь по нему первый раз, то вставляется дата
                    if (ngModel != null && ngModel.modelValue == null) {

                        if (maskControl != null) {
                            maskControl.maskOriginal.value = dateStr;
                        }
                        ngModel.$setViewValue(dateStr);
                    }

                    //if (maskControl != null) {
                    //    maskControl.maskOriginal.updateValue();
                    //}

                };

                options.onOpen = function (selectedDates, dateStr, instance) {
                    ctrl.needUpdate = false;

                    if (ctrl.fpOnOpen != null) {
                        $timeout(function () {
                            ctrl.fpOnOpen({ selectedDates: selectedDates, dateStr: dateStr, instance: instance });
                        });
                    }
                };

                //убираем плейсхолдер от маски на время иначе календарь берет его как значение и падает
                var revertMaskPlaceholder = removeMaskPlaceholder(input, ngModel, maskControl);
                var destroyElementEvents = ctrl.bindElementEvents(input, maskControl);

                ctrl.flatpickr = new flatpickr($element[0], options);

                var destroyNgModel = ctrl.processNgModel(ctrl.flatpickr, ngModel, maskControl);

                //возвращаем плейсхолдер
                revertMaskPlaceholder();

                // destroy the flatpickr instance when the dom element is removed
                $element.on('$destroy', function () {
                    ctrl.flatpickr.destroy();
                    destroyNgModel();
                    destroyElementEvents();
                });

                if (ctrl.fpOnSetup) {
                    ctrl.fpOnSetup({
                        fpItem: ctrl.flatpickr
                    });
                }
            };
        }],
    };
}]);

ngFlatpickr.directive('ngFlatpickrInput', ['$filter', '$timeout', function ($filter, $timeout) {
    return {
        require: {
            ngModel: 'ngModel',
            ngFlatpickr: '^ngFlatpickr',
            maskControl: '?maskControl'
        },
        restrict: 'A',
        bindToController: true,
        controllerAs: 'ngFlatpickrInput',
        controller: ['$element', function ($element) {
            var ctrl = this;

            ctrl.$onInit = function () {
                ctrl.$element = $element;
                ctrl.ngFlatpickr.addNgFlatpickrInput(ctrl);
            };

            ctrl.$postLink = function () {
                $element.attr('data-input', '');
            }
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
