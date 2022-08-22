; (function (ng) {
    'use strict';

    ng.module('quickview')
        .directive('quickviewTrigger', ['domService', '$location', 'urlHelper', '$parse', function (domService, $location, urlHelper, $parse) {
            return {
                require: ['quickviewTrigger', '^productViewItem'],
                restrict: 'A',
                scope: true,
                controller: 'QuickviewCtrl',
                controllerAs: 'quickview',
                bindToController: true,
                link: function (scope, element, attrs, ctrls) {
                    var hash = $location.hash();
                    var quickviewCtrl = ctrls[0],
                        productViewItemCtrl = ctrls[1];
                    if (hash != null) {
                        var splitedHash = hash.split('?');
                        hash = splitedHash != null ? splitedHash[0] : hash;
                    }
                    var colorId, sizeId;
                    var originalHash = splitedHash[0];
                    if (splitedHash.length > 1) {
                        var addParams = urlHelper.getUrlParamsAsObject(splitedHash[1]);
                        colorId = addParams.colorId;
                        sizeId = addParams.sizeId;
                    }

                    quickviewCtrl.modalId = attrs.modalId != null ? attrs.modalId : null;
                    quickviewCtrl.isSpyAddress = attrs.spyAddress === 'true';

                    if (hash === attrs.modalId) {

                        quickviewCtrl.openFromHash = true;

                        if (attrs.categoryId != null) {
                            quickviewCtrl.modalId = attrs.modalId + '?categoryId=' + attrs.categoryId;
                        }


                        if (quickviewCtrl.siblings == null) {
                            quickviewCtrl.setSiblings(element[0]);
                        }

                        quickviewCtrl.showModal(productViewItemCtrl.productId || parseInt(attrs.productId),
                            colorId || productViewItemCtrl.getSelectedColorId(),
                            attrs.quickviewTypeView,
                            element[0].getAttribute('data-modal-class'),
                            attrs.landingId,
                            attrs.hideShipping,
                            attrs.showLeadButton,
                            attrs.blockId,
                            attrs.showVideo != null ? attrs.showVideo : null,
                            quickviewCtrl.modalId,
                            quickviewCtrl.openFromHash,
                            sizeId,
                            quickviewCtrl.onOpenModal,
                            quickviewCtrl.isSpyAddress,
                            attrs.descriptionMode
                        );

                        //scope.$digest();
                    }

                    element[0].addEventListener('click', function (event) {

                        event.preventDefault();
                        event.stopPropagation();

                        if (quickviewCtrl.siblings == null) {
                            quickviewCtrl.setSiblings(element[0]);
                        }

                        if (attrs.modalId != null && attrs.categoryId != null) {
                            quickviewCtrl.modalId = attrs.modalId + '?categoryId=' + attrs.categoryId;
                        }

                        quickviewCtrl.showModal(productViewItemCtrl.productId || parseInt(attrs.productId),
                            productViewItemCtrl.getSelectedColorId(),
                            attrs.quickviewTypeView,
                            element[0].getAttribute('data-modal-class'),
                            attrs.landingId,
                            attrs.hideShipping,
                            attrs.showLeadButton,
                            attrs.blockId,
                            attrs.showVideo != null ? attrs.showVideo : null,
                            quickviewCtrl.modalId,
                            quickviewCtrl.openFromHash,
                            sizeId,
                            quickviewCtrl.onOpenModal,
                            quickviewCtrl.isSpyAddress,
                            attrs.descriptionMode
                        );

                        scope.$apply();
                    });
                }
            };
        }]);

})(window.angular);