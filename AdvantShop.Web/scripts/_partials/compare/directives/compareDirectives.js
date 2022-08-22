/* @ngInject */
function compareControlDirective(compareService) {
    return {
        restrict: 'A',
        scope: true,
        controller: 'CompareCtrl',
        controllerAs: 'compare',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            ctrl.dirty = true;
            if (attrs.compareControl != null) {
                compareService.addCompareScope(parseInt(attrs.compareControl), ctrl);
            }
        }
    };
};

function compareCountDirective() {
    return {
        restrict: 'A',
        scope: true,
        controller: 'CompareCountCtrl',
        controllerAs: 'compareCount',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            ctrl.countObj.count = parseInt(attrs.startCount, 10);
        }
    };
};

/* @ngInject */
function compareRemoveAllDirective(compareService) {
    return {
        restrict: 'A',
        scope: true,
        link: function (scope, element, attrs) {
            element.on('click', function (event) {
                event.preventDefault();
                compareService.removeAll();
            });
        }
    };
};

/* @ngInject */
function compareRemoveDirective(compareService) {
    return {
        restrict: 'A',
        scope: true,
        link: function (scope, element, attrs) {
            element.on('click', function (event) {
                event.preventDefault();
                compareService.remove(attrs.compareRemove);
            });
        }
    };
};

export {
    compareControlDirective,
    compareCountDirective,
    compareRemoveAllDirective,
    compareRemoveDirective
}