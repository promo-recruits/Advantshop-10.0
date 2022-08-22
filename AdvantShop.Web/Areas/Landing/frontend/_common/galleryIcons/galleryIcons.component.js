(function (ng) {
    'use strict';

    ng.module('galleryIcons')
        .component('galleryIcons', {
            controller: 'GalleryIconsCtrl',
            templateUrl: 'areas/landing/frontend/_common/galleryIcons/galleryIcons.html',
            bindings: {
                onSelect: '&'
            }
        });

    

})(window.angular);
