function demoModalDirective() {
    return {
        restrict: 'A',
        scope: {
            demoModalUrl: '@',
            demoModalId: '@'
        },
        controller: 'DemoCtrl',
        controllerAs: 'demoModal',
        bindToController: true,
        templateUrl: '/scripts/_partials/demo/templates/demoModal.html'
    };
};

export {
    demoModalDirective
}