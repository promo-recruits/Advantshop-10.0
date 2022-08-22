(function (ng) {
    'use strict';

    ng.module('galleryCloud')
        .component('galleryCloud', {
            controller: 'GalleryCloudCtrl',
            templateUrl: 'areas/landing/frontend/_common/galleryCloud/galleryCloud.html',
            bindings: {
                onSelect: '&'
            }
        });

    

})(window.angular);
