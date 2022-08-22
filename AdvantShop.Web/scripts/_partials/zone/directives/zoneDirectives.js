; (function (ng) {
    'use strict';

    angular.module('zone')
        .directive('zoneDialogTrigger', ['zoneService', function (zoneService) {
            return {
                restrict: 'A',
                scope: {},
                link: function (scope, element, attrs, ctrl) {
                    element.on('click', function (e) {
                        e.stopPropagation();
                        scope.$apply(zoneService.zoneDialogOpen);
                    });
                }
            };
        }]);

    angular.module('zone')
        .directive('zoneDialog', function () {
            return {
                restrict: 'A',
                scope: {},
                replace: true,
                templateUrl: '/scripts/_partials/zone/templates/dialog.html',
                controller: 'ZoneCtrl',
                controllerAs: 'zone',
                bindToController: true
            }
        });

    angular.module('zone')
        .directive('zoneCurrent', ['zoneService', function (zoneService) {
            return {
                restrict: 'A',
                scope: true,
                link: function (scope, element, attrs, ctrl) {

                    var startVal = new Function('return ' + attrs.startVal)();

                    scope.zone = {};

                    //if (attrs.startCity != null) {
                    //    scope.zone.City = attrs.startCity;
                    //}

                    if (startVal != null) {
                        angular.extend(scope.zone, zoneService.trustZone(startVal));
                    }

                    zoneService.addUpdateList(scope);

                    zoneService.getCurrentZone().then(function (data) {
                        scope.zone = zoneService.trustZone(data);
                    });
                }
            }
        }]);


    angular.module('zone')
        .directive('zonePopover', function () {
            return {
                restrict: 'A',
                scope: true,
                controller: 'ZonePopoverCtrl',
                controllerAs: 'zonePopover'
            };
        });
    angular.module('zone')
        .directive('zoneAddCallback', ['zoneService', '$parse', function (zoneService, $parse) {
            return {
                restrict: 'A',
                scope: true,
                controller: 'ZonePopoverCtrl',
                controllerAs: 'zonePopover',
                link: function (scope, element, attrs, ctrl) {
                    const objCallback = $parse(attrs.zoneAddCallback)(scope);
                    if (objCallback != null && objCallback.callback != null && objCallback.callbackName != null) {
                        zoneService.addCallback(objCallback.callbackName, objCallback.callback);
                    }
                }
            };
        }]);

})(window.angular);