; (function (ng) {
    'use strict';

    var ModalAddExportFeedCtrl = function ($uibModalInstance, $http, $window, $translate, toaster) {
        var ctrl = this;
        
        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.exportfeedTypes = [];

            $http.post('exportfeeds/GetAvalableTypes', { type: params.type }).then(function (response) {
                var data = response.data;
                ctrl.exportfeedTypes = response.data.obj;
                if (data.result != true || !ctrl.exportfeedTypes || ctrl.exportfeedTypes.length == 0) {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.AddExportFeed.ErrorGettingTypes'));
                } else {
                    ctrl.type = ctrl.exportfeedTypes[0];
                }
            });
        };

        ctrl.add = function () {

            var params = {
                name: ctrl.name,
                description: ctrl.description,
                type: ctrl.type.value
            };

            $http.post('exportfeeds/add', params).then(function (response) {
                if (response.data.result == true) {
                    $uibModalInstance.close();
                    $window.location.assign(
                        `exportfeeds/exportfeed${response.data.obj.typeUrlPostfix}/${response.data.obj.id}`);
                } else {
                    toaster.error('', $translate.instant('Admin.Js.AddExportFeed.ErrorCreatingNewExport'));
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddExportFeedCtrl.$inject = ['$uibModalInstance', '$http', '$window', '$translate', 'toaster'];

    ng.module('uiModal')
        .controller('ModalAddExportFeedCtrl', ModalAddExportFeedCtrl);

})(window.angular);