﻿; (function (ng) {
    'use strict';

    ng.module('gradientPicker')
        .component('gradientPicker', {
            templateUrl: 'areas/landing/frontend/_common/gradient-picker/templates/gradientPicker.html',
            controller: 'GradientPickerCtrl',
            bindings: {
                onUpdate: '&',
                startColor: '<',
                middleColor: '<',
                endColor: '<'
            }
        });
})(window.angular);