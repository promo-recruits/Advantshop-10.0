; (function (ng) {
    'use strict';

    var ProductGiftsCtrl = function ($http, SweetAlert, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            getGifts();
        };

        ctrl.addGifts = function (result) {
            if (result == null || result.ids == null || result.ids.length === 0)
                return;
            $http.post('product/addGifts', {
                productId: ctrl.productId,
                offerIds: result.ids,
                productCount: 1
            }).then(function (response) {
                getGifts();
                toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
            });
        };

        ctrl.deleteGift = function (offerId) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('product/deleteGift', {
                        productId: ctrl.productId,
                        offerId: offerId
                    }).then(function (response) {
                        getGifts();
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    });
                }
            });
        };

        function getGifts() {
            $http.get('product/getGifts', { params: { productId: ctrl.productId } }).then(function (response) {
                ctrl.products = response.data;
            });
        };

        ctrl.updateGift = function (offerId, productCount) {
            $http.post('product/updateGift', {
                productId: ctrl.productId,
                offerId: offerId,
                productCount: productCount
            }).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    getGifts();
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };
    };

    ProductGiftsCtrl.$inject = ['$http', 'SweetAlert', 'toaster', '$translate'];

    ng.module('productGifts', ['offersSelectvizr'])
        .controller('ProductGiftsCtrl', ProductGiftsCtrl)
        .component('productGifts', {
            templateUrl: '../areas/admin/content/src/product/components/productGifts/productGifts.html',
            controller: ProductGiftsCtrl,
            bindings: {
                productId: '@'
            }
      });

})(window.angular);