; (function (ng) {
    'use strict';

    //TODO: вынести работу с DOMом

    var isTouchDevice = 'ontouchstart' in document.documentElement;

    var TransformerCtrl = function ($window, $element, $scope, transformerService) {

        var ctrl = this,
            container,
            containerStartRect,
            elBoxIndents,
            elWrapBoxIndents,
            elementStartRect,
            mutableItems = [];

        ctrl.styles = {};

        ctrl.isTouchDevice = isTouchDevice;

        ctrl.init = function (containerElement) {

            transformerService.addInStorage(ctrl);

            ctrl.addContainer(containerElement);

            ctrl.calc();

            if (ctrl.onInit != null) {
                ctrl.onInit($scope, { transformer: ctrl });
            }
        };

        ctrl.addContainer = function (containerElement) {
            var temp,
                tempEl;

            container = containerElement;
            temp = container.getBoundingClientRect();
            tempEl = $element[0].getBoundingClientRect();

            elBoxIndents = {
                paddingLeft: parseFloat($($element[0]).css('padding-left')),
                marginLeft: parseFloat($($element[0]).css('margin-left'))
            };

            elWrapBoxIndents = {
                paddingLeft: parseFloat($element.parent().css('padding-left')),
                marginLeft: parseFloat($element.parent().css('margin-left'))
            };

            elementStartRect = {
                top: tempEl.top,
                bottom: tempEl.bottom,
                height: tempEl.height
            };

            containerStartRect = {
                top: temp.top + $window.pageYOffset,
                bottom: temp.bottom + $window.pageYOffset,
                right: temp.right + $window.pageXOffset
            };
        };

        ctrl.calc = function () {
            var freezeNew,
                containerComputedStyle = $window.getComputedStyle(container),
                containerRect = container.getBoundingClientRect(),
                elementRect = $element[0].getBoundingClientRect(),
                offsetByParents = transformerService.getOffsetByParents(ctrl),
                scrollOverNew = $window.pageYOffset + ctrl.offsetTop + offsetByParents > containerStartRect[ctrl.stickyPosition];

            var containerPaddingLeft = parseFloat(containerComputedStyle['padding-left']);
            var containerPaddingRight = parseFloat(containerComputedStyle['padding-right']);

            ctrl.styles.width = containerRect.width - containerPaddingLeft - containerPaddingRight;

            if (ctrl.limitPos === true) {
                if (scrollOverNew === false) {
                    //default state
                    freezeNew = false;
                    ctrl.styles.transform = 'translate3d(0, 0, 0)';
                    ctrl.styles[ctrl.stickyPosition] = 'auto';
                    ctrl.expandMutableItems();
                } else {

                    if (containerRect.height + containerStartRect.top <= elementRect.height + window.pageYOffset + ctrl.offsetTop + offsetByParents) {
                        //bottom sticky
                        freezeNew = true;
                        ctrl.styles.transform = 'translate3d(0, ' + (containerRect.height - elementRect.height) + 'px, 0)';
                        ctrl.styles[ctrl.stickyPosition] = 'auto';
                    } else {
                        //fixed
                        freezeNew = false;
                        ctrl.styles.transform = 'translate3d(0, 0, 0)';
                        ctrl.styles[ctrl.stickyPosition] = ctrl.offsetTop + offsetByParents + 'px';

                        var containerBoxIndents = containerPaddingLeft + parseFloat(containerComputedStyle['margin-left']);

                        if (containerRect.left != 0 && containerRect.left > 0 && scrollOverNew) {
                            ctrl.styles.left = containerRect.left + elBoxIndents.paddingLeft + elBoxIndents.marginLeft + containerBoxIndents + elWrapBoxIndents.marginLeft + elWrapBoxIndents.paddingLeft;
                        } else {
                            ctrl.styles.left = ctrl.elStartRect;
                        }

                        if (elementRect.height + ctrl.offsetTop > $window.innerHeight) {
                            ctrl.collapseMutableItems(elementRect.height + containerStartRect.top - $window.innerHeight);
                        } else if (ctrl.offsetTop + elementStartRect.height < $window.innerHeight) {
                            ctrl.expandMutableItems();
                        }
                    }
                }
            } else {
                ctrl.styles[ctrl.stickyPosition] = offsetByParents || 0;
            }
            

            if (freezeNew === true) {
                ctrl.scrollOver = false;
            } else {
                ctrl.scrollOver = scrollOverNew;
            }

            ctrl.freeze = freezeNew;

            if (ctrl.scrollOver === true) {
                $element[0].classList.remove('transformer-scroll-default');
                $element[0].classList.add('transformer-scroll-over');
                $element[0].classList.add('transformer-scroll-over--' + ctrl.stickyPosition);
            } else if (ctrl.scrollOver === false) {
                $element[0].classList.remove('transformer-scroll-over');
                $element[0].classList.add('transformer-scroll-default');
                ctrl.styles.backgroundColor = null;
                ctrl.styles[ctrl.stickyPosition] = 'auto';
            }

            if (ctrl.freeze) {
                $element[0].classList.add('transformer-freeze');
            } else {
                $element[0].classList.remove('transformer-freeze');
            }

            $element.css(ctrl.styles);
        };

        ctrl.windowScroll = function () {

            if (ctrl.initialize !== true) {
                ctrl.wait === true;
                return;
            }

            ctrl.calc();

            $scope.$digest();
        };

        ctrl.addMutableItem = function (element) {
            mutableItems.push({
                el: element,
                height: element.offsetHeight
            });
        };

        ctrl.collapseMutableItems = function (dim) {
            var sumMutable = 0;

            for (var i = 0, len = mutableItems.length; i < len; i++) {
                if (sumMutable < dim) {
                    sumMutable += mutableItems[i].height;
                    mutableItems[i].el.classList.add('transformer-hidden');
                } else {
                    break;
                }
            }
        };

        ctrl.expandMutableItems = function () {
            for (var i = 0, len = mutableItems.length; i < len; i++) {
                mutableItems[i].el.classList.remove('transformer-hidden');
            }
        };

        ctrl.getBottomPoint = function () {
            return ctrl.offsetTop + transformerService.getOffsetByParents(ctrl) + $element[0].offsetHeight;
        };
    };

    ng.module('transformer')
        .controller('TransformerCtrl', TransformerCtrl);

    TransformerCtrl.$inject = ['$window', '$element', '$scope', 'transformerService'];

})(window.angular);