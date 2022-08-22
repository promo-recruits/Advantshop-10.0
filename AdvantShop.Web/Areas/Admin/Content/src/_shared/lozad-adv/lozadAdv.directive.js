; (function (ng) {
    'use strict';

    ng.module('lozadAdv')
        .constant('lozadAdvDefault',{
            rootMargin: '0px',
            threshold: 0,
            afterWindowLoaded: true,
            load: function load(element) {
                if (element.dataset.src) {
                    element.src = element.dataset.src;
                }
                if (element.dataset.srcset) {
                    element.srcset = element.dataset.srcset;
                }
                if (element.dataset.backgroundImage) {
                    element.style.backgroundImage = 'url(' + element.dataset.backgroundImage + ')';
                }
            }
        })
        .directive('lozadAdv', [function () {
            return {
                controller: 'LozadAdvCtrl',
                bindToController: true,
                controllerAs: 'lozadAdv',
                scope: true
            };
    }]);

})(window.angular);


