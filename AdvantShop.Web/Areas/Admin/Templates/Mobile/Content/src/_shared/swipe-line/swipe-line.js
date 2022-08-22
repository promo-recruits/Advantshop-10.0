; (function () {
    'use strict';

    angular.module('swipeLine', [])
        .directive('swipeLine', function () {
            return {
                controller: ['$element', '$document', '$window', function ($element, $document, $window) {
                    var ctrl = this;

                    var overflowDistance = 50; //насколько мы можем перемотать ещё, после того как показались элементы 
                    var ratio = 30 / 100; //процент, при котором мы доматываем до конца
                    var moveValueCurrent = 0;
                    var moveValueLast = 0;
                    
                    ctrl.$postLink = function () {
                        $element[0].addEventListener('touchstart', function (e) {
                           var e = e || $window.event;//don't know how mobile browsers behave here
                            ctrl.startCoordinates = {
                                x: e.changedTouches[0].clientX,
                                y: e.changedTouches[0].clientY
                            };

                            if (ctrl.swipeLineRightElement != null) {
                                ctrl.maxDistanceRight = ctrl.swipeLineRightElement[0].offsetWidth;
                            } else {
                                return;
                            }

                            $element[0].addEventListener('touchmove', ctrl.moveHandler,);
                            $element[0].addEventListener('touchend', ctrl.endHandler);
                        }, false);
                    }

                    ctrl.moveHandler = function (e) {
                        var e = e || $window.e;

                        if (ctrl.validSwipe(ctrl.startCoordinates, { x: e.changedTouches[0].clientX, y: e.changedTouches[0].clientY }) === false) {
                            return;
                        }

                        e.preventDefault();

                        var diffCalc =  (moveValueLast * -1 ) + ctrl.startCoordinates.x - e.changedTouches[0].clientX;
                        var diff;

                        if (diffCalc > ctrl.maxDistanceRight + overflowDistance) {
                            diff = ctrl.maxDistanceRight + overflowDistance;
                        } else if (diffCalc < overflowDistance * -1) {
                            diff = overflowDistance * -1;
                        } else {
                            diff = diffCalc;
                        }

                        ctrl.move(diff * -1);
                    };

                    ctrl.endHandler = function (e) {
                        var e = e || $window.e;

                        var diff = ctrl.startCoordinates.x - e.changedTouches[0].clientX;
                        var diffOverride;

                        if (diff > ctrl.maxDistanceRight || (moveValueCurrent < 0 && ratio < Math.abs(moveValueCurrent) / ctrl.maxDistanceRight)) {
                            diffOverride = ctrl.maxDistanceRight;
                        } else {
                            diffOverride = 0;
                        }


                        if (diffOverride != null) {
                            ctrl.move(diffOverride * -1, true);
                            moveValueCurrent = diffOverride * -1;
                        } 

                        moveValueLast = moveValueCurrent;

                        $element[0].removeEventListener('touchmove', ctrl.moveHandler);
                        $element[0].removeEventListener('touchend', ctrl.endHandler);
                    };

                    ctrl.runAnimation = function (callback) {
                        function fnOnTransitionend() {
                            $element.removeClass('swipe-line-animate');

                            if (callback != null) {
                                callback();
                            }

                            $element[0].removeEventListener('transitionend', fnOnTransitionend);
                        };

                        $element[0].addEventListener('transitionend', fnOnTransitionend);
                        $element.addClass('swipe-line-animate');
                    };

                    ctrl.move = function (moveValue, useAnimate, callback) {

                        if (useAnimate === true) {
                            ctrl.runAnimation(callback);
                        }

                        $element[0].style.transform = 'translate3d(' + moveValue + 'px, 0, 0)';
                        moveValueCurrent = moveValue;
                    
                    };
                                        
                    ctrl.addSwipeLineRight = function (swipeLineRightElement) {
                        ctrl.swipeLineRightElement = swipeLineRightElement;
                    };

                    ctrl.validSwipe = function (startCoords, coords) {

                        var deltaAlt = Math.abs(coords.y - startCoords.y);
                        var deltaMain = Math.abs(coords.x - startCoords.x);
                        var touchAngle = (Math.atan2(Math.abs(deltaAlt), Math.abs(deltaMain)) * 180) / Math.PI;

                        return touchAngle <= 45;
                    };
                }]
            }
        })
        .directive('swipeLineRight', function () {
            return {
                require: {
                    swipeLine: '^swipeLine'
                },
                bindToController: true,
                controller: ['$element', function ($element) {
                    var ctrl = this;

                    ctrl.$onInit = function () {
                        ctrl.swipeLine.addSwipeLineRight($element);
                    }
                }]
            }
        })
})();