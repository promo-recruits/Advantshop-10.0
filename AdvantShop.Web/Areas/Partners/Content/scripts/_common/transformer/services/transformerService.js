; (function (ng) {
    'use strict';

    var transformerService = function () {
        var service = this;
        var storage = [];

        service.addInStorage = function (transformer) {
            var sortOrder = transformer.sortOrder || 0;
            var isPasted = false;

            if (storage.length > 0) {
                for (var i = 0, len = storage.length; i < len; i++) {
                    if (sortOrder > storage[i].sortOrder) {
                        storage[i].splice(i, 0, transformer);
                        isPasted = true;
                        break;
                    }
                }
            } 

            if (isPasted === false) {
                storage.push(transformer);
            }
        };

        service.getOffsetByParents = function (self) {
            var result = 0;
            for (var i = 0, len = storage.length; i < len; i++) {

                if (storage[i] === self) {
                    break;
                } else if (storage[i].scrollOver === true || storage[i].freeze === true) {
                    result += storage[i].getBottomPoint();
                }
            }

            return result;
        };
    };

    ng.module('transformer')
        .service('transformerService', transformerService);

    transformerService.$inject = [];

})(window.angular);