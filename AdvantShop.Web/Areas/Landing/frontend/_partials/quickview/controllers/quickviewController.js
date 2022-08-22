; (function (ng) {
    'use strict';

    var QuickviewCtrl = function (quickviewService, cartService, cartConfig, domService, $location, urlHelper) {
        var ctrl = this,
            colorsAndSize = {},
            triggers = [];

        ctrl.addTrigger = function (productId) {
            triggers.push(productId);
        };

        ctrl.showModal = function (productId, colorId, typeView, modalClass, landingId, hideShipping, showLeadButton, blockId, showVideo, modalId, openFromHash, sizeId, onOpenModalCallback, spyAddress, descriptionMode) {
            quickviewService.dialogOpen(ctrl, productId, colorId, typeView, modalClass, landingId, hideShipping, showLeadButton, blockId, showVideo, modalId, openFromHash, sizeId, onOpenModalCallback, spyAddress, descriptionMode);
            ctrl.cartAddTriggerName = 'quckview_' + Date.now();
            cartService.addCallback(cartConfig.callbackNames.add, ctrl.hideModal, ctrl.cartAddTriggerName);
        };

        ctrl.hideModal = function () {
            quickviewService.dialogClose();
            cartService.removeCallback(cartConfig.callbackNames.add, ctrl.cartAddTriggerName);
        };

        ctrl.setSiblings = function (element) {

            var items, sibling, id, modalId;

            ctrl.siblings = [];
            ctrl.modalIds = {};

            items = domService.closest(element, '.js-products-view-block').parentNode.children;

            for (var i = 0, len = items.length - 1; i <= len; i++) {

                sibling = items[i].querySelector('.js-products-view-item');

                if (sibling != null) {

                    id = parseInt(sibling.getAttribute('data-product-id'));
                    modalId = sibling.getAttribute('data-modal-id');

                    if (ng.isNumber(id)) {
                        ctrl.siblings.push(id);
                        ctrl.modalIds[id] = modalId;
                    }
                }
            }
        };

        ctrl.onChangeSizeAndColor = function (data) {
            if (ctrl.isSpyAddress && data != null) {
                var hash = $location.hash();
                var urlParams = hash.split('?');
                var originalHash = urlParams.shift();

                //var urlParamsFromHash = urlHelper.getUrlParamsAsObject(urlParams.join('&'));

                if (data.ColorId != null) {
                    colorsAndSize.colorId = data.ColorId;
                }
                if (data.SizeId != null) {
                    colorsAndSize.sizeId = data.SizeId;
                }
                $location.hash((originalHash || ctrl.modalId) + '?' + urlHelper.paramsToString(Object.assign({}, colorsAndSize))); //, urlParamsFromHash
            }

        };

        ctrl.onOpenModal = function () {
            if (ctrl.productCtrl != null) {
                ctrl.onChangeSizeAndColor(ctrl.productCtrl.colorSelected);
                ctrl.onChangeSizeAndColor(ctrl.productCtrl.sizeSelected);
            }
        };

        ctrl.addProductCtrl = function (productCtrl) {
            ctrl.productCtrl = productCtrl;
        };

    };

    ng.module('quickview')
        .controller('QuickviewCtrl', QuickviewCtrl);

    QuickviewCtrl.$inject = ['quickviewService', 'cartService', 'cartConfig', 'domService', '$location', 'urlHelper'];

})(window.angular);