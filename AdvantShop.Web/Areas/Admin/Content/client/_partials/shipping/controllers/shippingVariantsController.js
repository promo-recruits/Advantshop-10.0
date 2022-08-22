; (function (ng) {
    'use strict';

    var ShippingVariantsCtrl = function ($http, zoneService) {
        var ctrl = this;

        ctrl.getData = function (offerId, amount, customOptions, zip) {

            ctrl.isProgress = true;

            return $http.post('productExt/getshippings', { offerId: offerId, amount: amount, customOptions: customOptions, zip: zip, rnd: Math.random() }).then(function (response) {
                if (response.data != null) {
                    ctrl.items = response.data.Shippings;
                    if (response.data.AdvancedObj) {
                        ctrl.showZip = response.data.AdvancedObj.ShowZip;
                    }
                }
                else {
                    ctrl.items = [];
                }
                ctrl.isProgress = false;

                return response.data;
            });
        };
        if (ctrl.type === "display") {
            ctrl.getData(ctrl.startOfferId, ctrl.startAmount, ctrl.startSvCustomOptions, ctrl.zip);
        }
        if (ctrl.type === "none") {
            ctrl.getData(ctrl.startOfferId, ctrl.startAmount, ctrl.startSvCustomOptions, ctrl.zip);
        }



        if (ctrl.type === "Always") {
            ctrl.getData(ctrl.startOfferId, ctrl.startAmount, ctrl.startSvCustomOptions, ctrl.zip);
        }
        
        ctrl.update = function () {
            ctrl.getData(ctrl.offerId, ctrl.amount, ctrl.svCustomOptions, ctrl.zip);
        };

        ctrl.calcShippings = function () {
            ctrl.type = "Always";
            ctrl.getData(ctrl.startOfferId, ctrl.startAmount, ctrl.startSvCustomOptions, ctrl.zip);
        }

        zoneService.addCallback('set', function (data) {
            ctrl.zip = data.Zip;
            ctrl.getData(ctrl.offerId, ctrl.amount, ctrl.svCustomOptions, ctrl.zip);
        });

        ctrl.initFn({ shippingVariants: ctrl });
    };

    ng.module('shipping')
      .controller('ShippingVariantsCtrl', ShippingVariantsCtrl);

    ShippingVariantsCtrl.$inject = ['$http', 'zoneService'];

})(window.angular);