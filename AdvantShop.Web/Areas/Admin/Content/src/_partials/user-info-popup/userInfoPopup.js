; (function (ng) {
    'use strict';

    var userInfoPopupCtrl = function ($uibModal, $http) {
        var ctrl = this;

        ctrl.$onInit = function () {

            ctrl.getUserData()
                .then(function (data) {
                    if (data != null && data.Show === true) {
                        if (swal.isVisible()) { // скрыть sweetalert, если открыты
                            swal.close();
                        }
                        return ctrl.showStepOne(data)
                                   .then(function (result) {
                                       if (ctrl.onFinish != null) {
                                           ctrl.onFinish({});
                                       }

                                       return result;
                                   })
                        //.then(ctrl.showStepTwo);
                    }
                })
                .finally(function () {
                    if (ctrl.onClose != null) {
                        ctrl.onClose({});
                    }
                });
        };

        ctrl.getUserData = function () {
            return $http.get('home/getUserInformation', { params: { rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };

        ctrl.showStepOne = function (data) {
            return $uibModal.open({
                windowClass: "user-info-popup",
                bindToController: true,
                controller: 'ModalUserInfoPopupCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/_partials/user-info-popup/modals/modalUserInfoPopup.html',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    params: data
                }
            })
            .result
        };

        ctrl.showStepTwo = function () {
            return $uibModal.open({
                windowClass: "user-info-popup",
                bindToController: true,
                controller: 'ModalUserInfoPopupInviteCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/_partials/user-info-popup/modals/modalUserInfoPopupInvite.html',
                backdrop: 'static',
                keyboard: false
            }).result;
        };
    };

    userInfoPopupCtrl.$inject = ['$uibModal', '$http'];

    ng.module('userInfoPopup', [])
        .controller('userInfoPopupCtrl', userInfoPopupCtrl);

})(window.angular);