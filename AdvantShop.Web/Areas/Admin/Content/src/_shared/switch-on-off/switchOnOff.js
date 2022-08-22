; (function (ng) {
    'use strict';

    var idIncrement = 0;

    ng.module('switchOnOff', [])
        .directive('switchOnOff', ['$ocLazyLoad', 'urlHelper', function ($ocLazyLoad, urlHelper) {
            return {
                controller: function () {
                    var ctrl = this;
                    ctrl.change = function (state) {
                        if (ctrl.onChange != null) {
                            ctrl.onChange({ checked: state });
                        }
                    };

                    ctrl.click = function (state, name) {
                        ctrl.checked = !state;
                        ctrl.onClick({ state: ctrl.checked, name: name });
                    };
                },
                bindToController: true,
                templateUrl: function (elem, attrs) {
                    var themeName = attrs.theme;
                    var templatePath = '/areas/admin/content/src/_shared/switch-on-off/templates/';
                    return urlHelper.getAbsUrl(templatePath + (themeName != null && themeName.length > 0 ? themeName : 'default') + '.html', true);
                },
                controllerAs: '$ctrl',
                scope: {
                    checked: '<?',
                    onChange: '&',
                    readonly: '<?',
                    id: '@',
                    onClick: '&',
                    theme: '<?',
                    label: '@'
                },
                link: function (scope, element, attrs, ctrl) {

                    if (ctrl.id == null || ctrl.id.length === 0) {
                        ctrl.id = 'switchOnOff_' + idIncrement;
                        idIncrement = idIncrement + 1;
                    }
                    ctrl.themeName = attrs.theme;
                    var themeCssPath = urlHelper.getAbsUrl('/areas/admin/content/src/_shared/switch-on-off/' + (ctrl.themeName != null && ctrl.themeName.length > 0 ? 'themes/' + ctrl.themeName + '.css' : 'styles/switch-on-off.css'), true);
                    $ocLazyLoad.load(
                        [
                            themeCssPath
                        ],
                        { serie: true }
                    ).then(function () { });

                }
            };
        }]);

})(window.angular);