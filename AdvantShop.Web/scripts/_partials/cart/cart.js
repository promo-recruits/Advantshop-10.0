; (function (ng) {
    'use strict';

    angular.module('cart', [])
      .constant('cartConfig', {
          callbackNames: {
              get: 'get',
              update:'update',
              remove:'remove',
              add: 'add',
              clear: 'clear',
              open: 'open',
          },
          cartMini: {
              delayHide: 3000
          }
      });

})(window.angular);