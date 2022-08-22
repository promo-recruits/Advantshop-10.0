; (function (ng) {
    'use strict';

    var SubblockInplaceCtrl = function (subblockInplaceService) {
        var ctrl = this;

        ctrl.savePicture = function (result) {
            ctrl.settings.src = result.picture;
            ctrl.settings.type = result.type || 'image';
            subblockInplaceService.updateSubBlockSettings(ctrl.sublockId, ctrl.settings);
        };


        ctrl.onLazyLoadChange = function (result) {
            ctrl.settings.lazyLoadEnabled = result;
            subblockInplaceService.updateSubBlockSettings(ctrl.sublockId, ctrl.settings);
        };
    };

    ng.module('subblockInplace')
      .controller('SubblockInplaceCtrl', SubblockInplaceCtrl);

    SubblockInplaceCtrl.$inject = ['subblockInplaceService'];

})(window.angular);