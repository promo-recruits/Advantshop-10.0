import IMask from 'imask';

const presetList = {
    'date': {
        mask: Date,
        lazy: false,
        parse: function (str) {
            var year, month, day;

            if (str.includes('.')) {
                var _str = str.split('.');
                day = _str[0],
                    month = _str[1],
                    year = _str[2];
            } else if (str.includes('-')) {
                var _str = str.split('-');
                day = _str[2],
                    month = _str[1],
                    year = _str[0];
            }

            return new Date(year, month - 1, day);
        },
    },
    'phone': {
        mask: [
            {
                mask: '+0(000)000-00-00', //Россия
                startsWith: '7',
                lazy: true,
            },
            {
                mask: '+000(00)000-00-00', //Украина
                startsWith: '380',
                lazy: true,
            },
            {
                mask: '+000(00)000-00-00', //Беларусь
                startsWith: '375',
                lazy: true,
            }
        ],
        //parse: function (str) {
        //    return str != null && str.length > 0 ? str.replace(/[\s\+]/g, '') : str;
        //},
        dispatch: function (appended, dynamicMasked) {
            var number = (dynamicMasked.value + appended).replace(/\D/g, '');

            var itemFinded = dynamicMasked.compiledMasks.find(function (m) {
                return number.indexOf(m.startsWith) === 0;
            });

            let result = itemFinded || dynamicMasked.compiledMasks[0];

            return result;
        }
    },
    'number': {
        mask: /^\d+$/
    }
};

function getMaskValue(mask, maskControlPreset) {
    return maskControlPreset === 'phone' ? mask.value : mask.unmaskedValue;

}

/*@ngInject*/
function maskDirective($parse, $timeout, $q, maskControlService) {
    return {
        restrict: 'A',
        require: {
            ngModel: 'ngModel'
        },
        bindToController: true,
        controllerAs: 'mask',
        priority: 100,
        controller: ['$scope', '$element', '$attrs', function (scope, element, attrs) {
            const ctrl = this;

            ctrl.$onInit = function () {

                let isComplete = false;
                let timer;

                const modelValueSetter = function (scope, newValue) {
                    // if (timer != null) {
                    //     $timeout.cancel(timer);
                    // }
                    // return timer = $timeout(() => { $parse(attrs.ngModel).assign(scope, newValue) }, 100);
                    return $q.when($parse(attrs.ngModel).assign(scope, newValue));
                };


                const config = maskControlService.getMaskControlConfig();

                if (config.enablePhoneMask === false || $parse(attrs.maskControl)(scope) === false) {
                    return;
                }

                var ngModelValue = $parse(attrs.ngModel)(scope);
                var startValue = ngModelValue != null && (typeof ngModelValue !== 'string' || ngModelValue.length > 0) ? ngModelValue : element.val();
                if (startValue.length > 0 && startValue !== ctrl.ngModel.$modelValue && (ctrl.ngModel.$modelValue == null || isNaN(ctrl.ngModel.$modelValue))) {
                    ctrl.ngModel.$setViewValue(startValue);
                    ctrl.ngModel.$setPristine();
                }

                const preset = attrs.maskControlPreset != null ? presetList[attrs.maskControlPreset] : null;

                const mask = IMask(element[0], Object.assign({}, preset, $parse(attrs.mask)(scope)));

                ctrl.maskOriginal = mask;

                if (attrs.maskControlPreset === 'phone') {
                    updatePlaceholder();
                }

                mask.on('accept', function (event) {
                    if (attrs.maskControlPreset === 'phone') {
                        //если ввели по старой памяти 8 заместо +7 для России
                        if (mask.unmaskedValue.startsWith('8')) {
                            mask.value = '+7' + mask.unmaskedValue.substring(1);
                            //если же сразу начали вводить с 9 для России
                        } else if (mask.unmaskedValue === '9' && event.data === '9') {
                            mask.value = '+79';
                            //ставим курсор после числа 9
                            setTimeout(function () {
                                mask.updateCursor(4);
                            });
                        }
                    }

                    isComplete = false;
                    modelValueSetter(scope, getMaskValue(mask, attrs.maskControlPreset));
                });

                mask.on('complete', function () {
                    isComplete = true;
                    const maskValue = getMaskValue(mask, attrs.maskControlPreset);
                    modelValueSetter(scope, maskValue)
                        .then(() => ctrl.ngModel.$setViewValue(maskValue))
                        .then(() => ctrl.ngModel.$validate());
                });


                ctrl.ngModel.$render = function () {
                    //если ввели по старой памяти 8 заместо +7 для России
                    if (ctrl.ngModel.$modelValue != null && ctrl.ngModel.$modelValue.length === 11 && ctrl.ngModel.$modelValue.charAt(0) === '8') {
                        ctrl.ngModel.$modelValue = '7' + ctrl.ngModel.$modelValue.substring(ctrl.ngModel.$modelValue.length - (ctrl.ngModel.$modelValue.length - 1));
                    }
                    mask.value = ctrl.ngModel.$modelValue || '';
                };


                ctrl.ngModel.$parsers.push(function (value) {
                    return getMaskValue(mask, attrs.maskControlPreset);
                });


                ctrl.ngModel.$formatters.push(function (value) {
                    return value != null || (mask.masked.currentMask != null ? mask.masked.currentMask.lazy === false : mask.masked.lazy === false) ? mask.value : value;
                });

                ctrl.ngModel.$validators.mask = function (modelValue, viewValue) {
                    return mask.masked.isComplete || isComplete || (element[0].getAttribute('required') == null && $parse(attrs.ngRequired)(scope) != true && mask.masked.rawInputValue.length === 0);
                };


                scope.$watch(attrs.ngModel, function (newVal, oldVal) {
                    if (newVal != null && (mask.unmaskedValue == '' || mask.unmaskedValue !== newVal)) {
                        //mask.unmaskedValue = mask.masked.parse(newVal);
                        mask.unmaskedValue = mask.masked.format(mask.masked.parse(newVal));
                    }
                });


                function updatePlaceholder() {
                    var placeholderOldValue = element[0].placeholder;
                    if (placeholderOldValue == null || placeholderOldValue.length === 0) {
                        var currentMask = ctrl.maskOriginal.masked.currentMask;
                        var placeholder = currentMask.mask.replaceAll('0', currentMask.placeholderChar);
                        element[0].setAttribute('placeholder', placeholder);
                    }
                }

                element.on('$destroy', function () {
                    mask.destroy();
                });
            }
        }]
    }
}

/*@ngInject*/
function maskConfigDirective($parse, maskControlService) {
    return {
        restrict: 'A',
        bindToController: true,
        priority: 100,
        controllerAs: 'mask',
        controller: ['$scope', '$element', '$attrs', '$parse', function (scope, element, attrs) {
            if (instance) return;
            const ctrl = this;
            ctrl.$onInit = function () {
                const config = $parse(attrs.maskConfig)(scope);
                maskControlService.setMaskControlConfig(config || {});
            };
        }]
    };
}


export {
    maskDirective,
    maskConfigDirective
};