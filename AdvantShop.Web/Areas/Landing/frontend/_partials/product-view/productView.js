; (function (ng) {
    'use strict';

    ng.module('productView', [])
        .constant('viewList', {
            desktop: ['tile', 'list', 'table'],
            mobile: ['tile', 'list', 'single'],
            mobileModern: ['tile', 'list', 'single']
        })
        .constant('viewPrefix', {
            desktop: '',
            mobile: 'mobile-',
            mobileModern: 'mobile-modern-'
        });
})(window.angular);