; (function (ng) {
    'use strict';

    ng.module('uiGridCustomPagination')
      .component('uiGridCustomPagination', {
          templateUrl: ['$element', '$attrs', 'urlHelper', function (tElement, tAttrs, urlHelper) {
              return urlHelper.getAbsUrl('/areas/admin/content/src/_shared/ui-grid-custom/templates/ui-grid-custom-pagination.html', true);
          }],
          controller: 'UiGridCustomPaginationCtrl',
          bindings: {
              gridTotalItems: '<',
              gridPaginationPageSize: '<',
              gridPaginationPageSizes: '<',
              gridPaginationCurrentPage: '<',
              onChange: '&'
          },
          transclude: true
      });

})(window.angular);