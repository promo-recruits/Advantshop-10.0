(function (ng) {
    'use strict';

    ng.module('colorSchemeSettings')
        .component('colorSchemeSettings', {
            templateUrl: 'areas/landing/frontend/_common/color-scheme-settings/templates/colorSchemeSettings.html',
            controller: 'ColorSchemeSettingsCtrl',
            bindings: {
                settings: '=',
                onInit: '&',
                onUpdateBackground: '&',
                onUpdateColor: '&',
                fontMain: '<?',
                fontHeight: '<?'
            }
        });
})(window.angular);
