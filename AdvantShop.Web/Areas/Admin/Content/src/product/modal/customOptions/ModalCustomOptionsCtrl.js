; (function (ng) {
    'use strict';

    var ModalCustomOptionsCtrl = function ($uibModalInstance, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.productId = params.productId;
            ctrl.rnd = Math.random() * 10000;
        };
        
        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function() {

            ctrl.btnLoading = true;

            var iframe = document.getElementById("customOptions");

            var doc = iframe.document;
            if (iframe.contentDocument) {
                doc = iframe.contentDocument;
            }
            else if (iframe.contentWindow) {
                doc = iframe.contentWindow.document;
            }

            doc.getElementsByClassName("save-customoptions")[0].click();

            ctrl.btnLoading = false;

            toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
        }
    };

    ModalCustomOptionsCtrl.$inject = ['$uibModalInstance', 'toaster', '$translate'];

    ng.module('uiModal')
        .controller('ModalCustomOptionsCtrl', ModalCustomOptionsCtrl);

})(window.angular);