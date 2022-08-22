; (function (ng) {
	'use strict';

    ng.module('ngYearCalendar', [])
        .directive('yearCalendar', ['$ocLazyLoad',
            function ($ocLazyLoad) {
                return {
                    scope: {
                        calendarOptions: '<',
                        calendarOnInit: '&'
                    },
                    link: function (scope, element) {
                        $ocLazyLoad.load(
                            [
                                '../areas/admin/content/vendors/year-calendar/bootstrap-year-calendar.min.css',
                                '../areas/admin/content/vendors/year-calendar/bootstrap-year-calendar.custom.css',
                                '../areas/admin/content/vendors/year-calendar/bootstrap-year-calendar.min.js',
                                '../areas/admin/content/vendors/year-calendar/bootstrap-year-calendar.ru.js'
                            ],
                            { serie: true }
                        ).then(function() {
                            var option = ng.extend({ language: document.documentElement.lang }, scope.calendarOptions || {});
                            var calendarObj = $(element).calendar(option);

                            if (scope.calendarOnInit != null) {
                                scope.calendarOnInit({ calendar: calendarObj });
                            }
                        });
                    }
                }
            }
        ]);

})(window.angular);