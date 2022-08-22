; (function (ng) {
    'use strict';

    var domService = function () {
        var service = this;

        service.closest = function (element, selector, elementLimit) {

            if (element == null) {
                return null;
            }

            element = ng.isDefined(element[0]) ? element[0] : element;
            selector = ng.isElement(selector) && ng.isDefined(selector[0]) ? selector[0] : selector;

            var parent = element;

            if (parent == null) {
                return null;
            }
            if (elementLimit != null && typeof (elementLimit) === 'string') {
                elementLimit = service.closest(element, elementLimit);
            } else {
                elementLimit = elementLimit || document.body;
            }

            while (parent != elementLimit && parent != document && parent != null) {

                if (ng.isString(selector)) {
                    if (service.matches(parent, selector) === true) {
                        return parent;
                    }
                } else {
                    if (parent == selector) {
                        return parent;
                    }
                }

                parent = parent.parentNode;
            }

            return null;
        };

        service.prevAll = function (element) {

            var prevElement, result = [];

            element = ng.isDefined(element[0]) ? element[0] : element;


            prevElement = element.previousElementSibling;

            while (prevElement != null) {

                result.push(prevElement);

                prevElement = prevElement.previousElementSibling;
            }

            return result;
        };

        service.nextAll = function (element) {

            var nextElement, result = [];

            element = ng.isDefined(element[0]) ? element[0] : element;

            nextElement = element.nextElementSibling;

            while (nextElement != null) {

                result.push(nextElement);

                nextElement = nextElement.nextElementSibling;
            }

            return result;
        };

        service.matches = function (element, selector) {
            var fn = Element.prototype.matches || Element.prototype.matchesSelector ||
                Element.prototype.webkitMatchesSelector ||
                Element.prototype.mozMatchesSelector ||
                Element.prototype.msMatchesSelector;


            return fn.call(element, selector);
        };

        service.parent = function (element, selector, elementLimit) {
            var elementNative = ng.isDefined(element[0]) ? element[0] : element;
            return elementNative.parentElement != null ? service.closest(elementNative.parentNode, selector, elementLimit) : null;
        };
    };

    ng.module('dom')
      .service('domService', domService);

})(window.angular);