; (function (ng) {
    'use strict';

    ng.module('offersSelectvizr')
      .component('offersSelectvizr', {
          templateUrl: ['$element', '$attrs', 'urlHelper', function (tElement, tAttrs, urlHelper) {
              return urlHelper.getAbsUrl('/areas/admin/content/src/_partials/offers-selectvizr/templates/offers-selectvizr.html', true);
          }],
          controller: 'OffersSelectvizrCtrl',
          transclude: true,
          bindings: {
              selectvizrTreeUrl: '<',
              selectvizrGridUrl: '<',
              selectvizrGridOptions: '<',
              selectvizrGridParams: '<?',
              selectvizrOnChange: '&',
              selectvizrGridOnFetch: '&',
              selectvizrTreeSearch: '<?'
          }
      });

})(window.angular);