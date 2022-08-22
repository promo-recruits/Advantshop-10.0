; (function (ng) {
    'use strict';

    ng.module('backgroundPicker')
        .component('backgroundPicker', {
            templateUrl: 'areas/landing/frontend/_common/background-picker/templates/backgroundPicker.html',
            controller: 'BackgroundPickerCtrl',
            bindings: {
                onUpdate: '&',
                colors: '<',
                colorSelected: '<?',
                onInit: '&'
            }
        });
})(window.angular);