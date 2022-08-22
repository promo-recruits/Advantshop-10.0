; (function (ng) {
    'use strict';

    ng.module('productsSelectvizr')
      .component('productsSelectvizr', {
          templateUrl: ['$element', '$attrs', 'urlHelper', function (tElement, tAttrs, urlHelper) {
              return urlHelper.getAbsUrl('/areas/admin/content/src/_partials/products-selectvizr/templates/products-selectvizr.html', true) + "?rnd=" + Math.random();
          }],
          controller: 'ProductsSelectvizrCtrl',
          transclude: true,
          bindings: {
              selectvizrTreeUrl: '<',
              selectvizrTreeItemsSelected: '<?',
              selectvizrGridUrl: '<',
              selectvizrGridOptions: '<',
              selectvizrGridParams: '<?',
              selectvizrGridInplaceUrl: '<?',
              selectvizrGridItemsSelected: '<?',
              selectvizrOnChange: '&',
              selectvizrOnInit: '&',
              selectvizrTreeSearch: '<?'
          }
      });

})(window.angular);