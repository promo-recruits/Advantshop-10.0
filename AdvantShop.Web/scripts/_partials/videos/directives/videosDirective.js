function videosDirective() {
    return {
        restrict: 'A',
        controller: 'VideosCtrl',
        controllerAs: 'videos',
        scope: {
            productId: '@',
            onReceive: '&'
        },
        bindToController: true,
        replace: true,
        templateUrl: '/scripts/_partials/videos/templates/videosTemplate.html',
    };
};

export {
    videosDirective
};