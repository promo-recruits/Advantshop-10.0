; (function (ng) {
    'use strict';

    ng.module('leads')
        .directive('leadsList', ['$compile', '$timeout', function ($compile, $timeout) {
            return {
                controller: 'LeadsListCtrl',
                controllerAs: '$ctrl',
                //template: '<div bind-html-compile="$ctrl.data"></div>',
                transclude: true,
                bindToController: true,
                replace: true,
                scope: true
                //scope: {
                //    excludeLeadListId: '<?'
                //}
            };
        }]);

    ng.module('leads')
        .directive('leadsListSources', function () {
            return {
                controller: 'LeadsListSourcesCtrl',
                controllerAs: '$ctrl',                
                templateUrl: '../areas/admin/content/src/leads/components/leadsListSources/leadsListSources.html',
                bindToController: true,
                scope: {
                    leadsListId: '@'                    
                }
            };
        });
    
    ng.module('leads')
        .directive('leadsListChart', function () {
            return {
                controller: 'LeadsListChartCtrl',
                controllerAs: '$ctrl',
                templateUrl: '../areas/admin/content/src/leads/components/leadsListChart/leadsListChart.html',
                bindToController: true,
                scope: {
                    leadsListId: '@'
                }
            };
        });

})(window.angular);