; (function (ng) {
    'use strict';

    angular.module(`harmonica`)
        .directive(`harmonica`, ['$window', `$document`, function ($window, $document) {
        return {
            restrict: 'EA',
            scope: true,
            controller: 'HarmonicaCtrl',
            controllerAs: 'harmonica',
            bindToController: true,
            compile: function (cElement, cAttrs) {

                cElement.append(`<li data-harmonica-tile ${cAttrs.harmonicaTileOnOpen ? `data-on-open="${cAttrs.harmonicaTileOnOpen}"` : ``}></li>`);

                return function (scope, element, attrs, ctrl) {

                    var mq;
                    
                    if (attrs.harmonicaMatchMedia != null && attrs.harmonicaMatchMedia.length > 0) {
                        mq = $window.matchMedia(attrs.harmonicaMatchMedia);

                        mq.addListener(function (obj) {
                            setTimeout(function () {
                                if (obj.matches === true) {
                                    ctrl.start();
                                } else {
                                    ctrl.stop();
                                }

                                scope.$digest();
                            }, 100);
                        });

                        if (mq.matches === true) {
                            if ($document[0].readyState == `complete`) {
                                ctrl.start();
                            } else {
                                $window.addEventListener(`load`, function start() {
                                    $window.removeEventListener(`load`, start);
                                    ctrl.start();
                                });
                            }
                        } else {
                            ctrl.stop();
                        }
                    } else {

                        if ($document[0].readyState == `complete`) {
                            ctrl.start();
                        } else {
                            $window.addEventListener(`load`, function start() {
                                $window.removeEventListener(`load`, start);
                                ctrl.start();
                            });
                        }

                        
                    }

                    $window.addEventListener('resize', function () { update() });

                    function update() {
                        if (ctrl.active === true) {
                            var index = ctrl.calc();
                            ctrl.setVisible(index);
                            scope.$digest();
                        }
                    }
                };
            }
        };
    }]);

    angular.module('harmonica')
    .directive('harmonicaItem', function () {
        return {
            //require: '^harmonica',
            require: {
                harmonicaCtrl: '^harmonica'
            },
            restrict: 'EA',
            scope: true,
            bindToController: true,
            controller: ['$element', '$scope', function ($element, $scope) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    ctrl.harmonicaCtrl.addItem($element, $scope);


                    $scope.$watch('isVisibleInMenu', function (newValue, oldValue) {
                        $element[newValue === false ? 'addClass' : 'removeClass']('ng-hide');
                    });
                };
            }]
        }
    });

    angular.module('harmonica')
    .directive('harmonicaLink', function () {
        return {
            //require: '^harmonica',
            require: {
                harmonicaCtrl: '^harmonica'
            },
            restrict: 'EA',
            scope: true,
            bindToController: true,
            controller: ['$attrs', '$element', '$scope', function ($attrs, $element, $scope) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    ctrl.harmonicaCtrl.addLink($element.attr('href'), $element.text(), $attrs.linkClassesInTile, $attrs.linkTarget, $scope);
                };
            }]
        }
    });

    angular.module('harmonica')
    .directive('harmonicaTile', [function () {
        return {
            //require: ['ctrl', '^harmonica'],
            require: {
                harmonicaCtrl: '^harmonica'
            },
            restrict: 'EA',
            scope: {
                onOpen: '&'
            },
            replace: true,
            controller: 'HarmonicaTileCtrl',
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: 'scripts/_common/harmonica/templates/tile.html'
        }
    }]);

})(angular);