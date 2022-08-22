﻿; (function (ng) {

    'use strict';

    var SubBlockGalleryConstructorCtrl = function (blocksConstructorService, pictureLoaderService, toaster) {
        var ctrl = this;


        ctrl.onUploadPictureSubblock = function (item, result) {
            return ng.extend(item, blocksConstructorService.mapPictureField(item, result));
        };

        ctrl.onUploadPicture = function (items, result, index) {
            var isNew = items[index] == null;
            var itemUpdated = ng.extend(items[index] || {}, blocksConstructorService.updatePictureFields(items[index], result));

            if (isNew === true) {
                items.push(itemUpdated);
            }

            return itemUpdated;
        };

        ctrl.delete = function (lpId, blockId, picture, parameters, deleteUrl, items, index) {

            pictureLoaderService.delete(lpId, blockId, picture, parameters, deleteUrl)
                .then(function () {
                    items.splice(index, 1);
                })
                .catch(function () {
                    toaster.pop('error', 'Ошибка при удалении изображения');
                });
        };
    };

    ng.module('blocksConstructor')
        .controller('SubBlockGalleryConstructorCtrl', SubBlockGalleryConstructorCtrl);

    SubBlockGalleryConstructorCtrl.$inject = ['blocksConstructorService', 'pictureLoaderService', 'toaster'];

})(window.angular);