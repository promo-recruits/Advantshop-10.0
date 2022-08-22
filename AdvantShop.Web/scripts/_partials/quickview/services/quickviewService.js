/* @ngInject */
function quickviewService(modalService, $location, urlHelper) {
    var service = this,
        isRender = false,
        modalQuickViewId = null,
        modalsStorage = {},
        data = {};

    service.dialogRender = function (parentScope) {
        modalService.renderModal(modalQuickViewId || 'modalQuickView',
            null,
            '<div data-ng-include="\'/scripts/_partials/quickview/templates/quckviewModal.html\'"></div>',
            null,
            {
                'isOpen': false,
                'modalClass': 'modal-quickview' + ' ' + (parentScope.modalClass || ''),
                'backgroundEnable': true,
                spyAddress: parentScope.spyAddress,
                anchor: modalQuickViewId || 'modalQuickView',
                callbackOpen: parentScope.spyAddress ? 'quickview.onOpenModalCallback()' : ''
            }, { quickview: parentScope });
        modalService.getModal(modalQuickViewId || 'modalQuickView').then(function (modal) {
            modal.modalScope.open();
        });
    };

    service.getUrl = function (productId, colorId, typeView, landingId, hideShipping, showLeadButton, blockId, showVideo, sizeId, descriptionMode) {
        return 'product/productquickview' +
            '?productId=' + productId +
            (colorId != null ? '&color=' + colorId : '') +
            (sizeId != null ? '&size=' + sizeId : '') +
            '&from=' + typeView +
            (landingId != null ? '&landingId=' + landingId : '') +
            (hideShipping != null ? '&hideShipping=' + hideShipping : '') +
            (showLeadButton != null ? '&showLeadButton=' + showLeadButton : '') +
            (blockId != null ? '&blockId=' + blockId : '') +
            (showVideo != null ? '&showVideo=' + showVideo : '') +
            (descriptionMode != null ? '&descriptionMode=' + descriptionMode : '');
    };

    service.dialogOpen = function (itemData, productId, colorId, typeView, modalClass, landingId, hideShipping, showLeadButton, blockId, showVideo, modalId, openFromHash, sizeId, onOpenModalCallback, spyAddress, descriptionMode) {

        data.url = service.getUrl(productId, colorId, typeView, landingId, hideShipping, showLeadButton, blockId, showVideo, sizeId, descriptionMode);
        data.itemData = itemData;
        data.productId = productId;
        data.next = service.next;
        data.prev = service.prev;
        data.modalClass = modalClass;
        data.typeView = typeView;
        data.landingId = landingId;
        data.hideShipping = hideShipping;
        data.showLeadButton = showLeadButton;
        data.blockId = blockId;
        data.showVideo = showVideo;
        data.modalId = modalId;
        data.onOpenModalCallback = onOpenModalCallback;
        data.spyAddress = spyAddress;
        data.descriptionMode = descriptionMode;
        modalQuickViewId = modalId;

        var hash = $location.hash();
        var splitedHash = hash.split('?');
        var originalHash = splitedHash[0];

        if (!modalsStorage[modalId]) {
            service.dialogRender(data);
        } else {
            modalService.open(modalId);
        }
        modalsStorage[modalId] = modalId;
    };

    service.dialogClose = function () {
        modalService.close(modalQuickViewId || 'modalQuickView');
    };

    service.prev = function () {

        var index, indexPrev;

        index = data.itemData.siblings.indexOf(data.productId);
        indexPrev = index - 1;

        if (data.itemData.siblings[indexPrev] != null) {
            data.productId = data.itemData.siblings[indexPrev];
            $location.hash(data.itemData.modalIds[data.productId]);
            data.url = service.getUrl(data.productId, null, data.typeView, data.landingId, data.hideShipping, data.showLeadButton, data.blockId, data.showVideo, null, data.descriptionMode);
        }

    };

    service.next = function () {

        var index, indexNext;

        index = data.itemData.siblings.indexOf(data.productId);

        indexNext = index + 1;

        if (data.itemData.siblings[indexNext] != null) {
            data.productId = data.itemData.siblings[indexNext];
            $location.hash(data.itemData.modalIds[data.productId]);
            data.url = service.getUrl(data.productId, null, data.typeView, data.landingId, data.hideShipping, data.showLeadButton, data.blockId, data.showVideo, null, data.descriptionMode);
        }

    };
};

export default quickviewService;