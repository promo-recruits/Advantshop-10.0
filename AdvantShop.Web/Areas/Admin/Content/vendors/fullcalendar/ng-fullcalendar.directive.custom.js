/*!
 * angular-fullcalendar
 * https://github.com/JavyMB/angular-fullcalendar#readme
 * Version: 1.0.1 - 2017-10-06T14:28:29.825Z
 * License: ISC
 */


(function () { 
'use strict';
angular.module('angular-fullcalendar',[])
    .value('CALENDAR_DEFAULTS',{
        locale:'ru'
    })
    .directive('fullCalendar', ['CALENDAR_DEFAULTS', '$ocLazyLoad', fcDirectiveFn]);

function fcDirectiveFn(CALENDAR_DEFAULTS, $ocLazyLoad) {
    return {
        scope : {
            eventSource: '=ngModel',
            options: '=fcOptions',
            fcGetOptions: '&',
            fcIsWatchEventSource: '<',
            fcIsWatchOptions: '<',
            fcOnInit: '&',
            fcOnDestroy: '&'
        },
        link:function (scope, elm) {
            $ocLazyLoad.load(
                [
                    '../areas/admin/content/vendors/fullcalendar/fullcalendar.min.css',
                    '../areas/admin/content/vendors/moment/moment.min.js',
                    '../areas/admin/content/vendors/moment/locale/ru.js',
                    '../areas/admin/content/vendors/fullcalendar/fullcalendar.min.js',
                    '../areas/admin/content/vendors/fullcalendar/locale/ru.js'
                ],
                { serie: true }
            ).then(function() {
                var calendar;
                init();
                if (scope.fcIsWatchEventSource === undefined || scope.fcIsWatchEventSource) {
                    scope.$watch('eventSource', watchDirective, true);
                }
                if (scope.fcIsWatchOptions === undefined || scope.fcIsWatchOptions) {
                    scope.$watch('options', watchDirective, true);
                }
                scope.$on('$destroy', function () { destroy();});
                function init() {
                    if (!calendar) {
                        calendar = $(elm).html('');
                    }

                    if (scope.options) {
                        calendar.fullCalendar(getOptions(scope.options));
                    } else if (scope.fcGetOptions) {
                        calendar.fullCalendar(getOptions(scope.fcGetOptions()));
                    } else {
                        calendar.fullCalendar(getOptions({}));
                    }

                    if (scope.fcOnInit != null) {
                        scope.fcOnInit({ calendar: calendar.data('fullCalendar') });
                    }
                }
                function destroy() {
                  if(calendar && calendar.fullCalendar){
                      calendar.fullCalendar('destroy');
                  }

                  if (scope.fcOnDestroy != null) {
                      scope.fcOnDestroy();
                  }
                }
                function getOptions(options) {
                    return angular.extend({}, CALENDAR_DEFAULTS,{
                        events:scope.eventSource
                    },options);
                }
                function watchDirective(newOptions,oldOptions) {
                    if (newOptions !== oldOptions) {
                        destroy();
                        init();
                    } else if ((newOptions && angular.isUndefined(calendar))) {
                        init();
                    }
                }
            });
        }
    };

}

}());