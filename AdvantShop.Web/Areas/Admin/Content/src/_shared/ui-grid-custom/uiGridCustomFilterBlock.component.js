; (function (ng) {
    'use strict';

    ng.module('uiGridCustomFilter')
      .component('uiGridCustomFilterBlock', {
          templateUrl: ['$element', '$attrs', 'urlHelper', function (tElement, tAttrs, urlHelper) {
              return urlHelper.getAbsUrl('/areas/admin/content/src/_shared/ui-grid-custom/templates/ui-grid-custom-filter-block.html', true);
          }],
          controller: 'UiGridCustomFilterBlockCtrl',
          bindings: {
              item: '<',
              blockType: '<',
              onApply: '&',
              onClose: '&'
          }
      });

})(window.angular);