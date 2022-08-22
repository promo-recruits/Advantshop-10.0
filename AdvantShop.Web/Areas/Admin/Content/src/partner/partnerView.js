; (function (ng) {
    'use strict';

    var PartnerViewCtrl = function ($http, SweetAlert, $window, toaster, $translate) {

        var ctrl = this;
        
        ctrl.init = function (partnerId) {
            ctrl.partnerId = partnerId;

            localStorage.removeItem('admin-URLtab');

            ctrl.getPartnerView();
        };

        ctrl.getPartnerView = function () {
            $http.get('partners/getView', { params: { id: ctrl.partnerId } }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.instance = data.obj;
                } else {
                    window.location.assign('partners');
                }
            });
        };

        ctrl.delete = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('partners/deletePartner', { id: ctrl.partnerId }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            $window.location.assign('partners');
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        };

        ctrl.updateAdminComment = function (comment) {
            $http.post('partners/updateAdminComment', { partnerId: ctrl.partnerId, comment: comment }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.addPartnerCoupon = function (couponId) {
            $http.post('partners/addPartnerCoupon', { partnerId: ctrl.partnerId, couponId: couponId }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.getPartnerView();
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        }

        ctrl.deletePartnerCoupon = function (couponId) {
            SweetAlert.confirm('Вы уверены, что хотите удалить купон партнера?', { title: $translate.instant('Admin.Js.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('coupons/deleteCoupon', { couponId: couponId }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            ctrl.getPartnerView();
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        }

        ctrl.onMoneyProcessed = function () {
            ctrl.getPartnerView();
            if (ctrl.gridTransactions) {
                ctrl.gridTransactions.fetchData();
            }
        };

        ctrl.isNullOrEmpty = function (str) {
            return str == null || str.length == 0;
        };
    };

    PartnerViewCtrl.$inject = ['$http', 'SweetAlert', '$window', 'toaster', '$translate'];


    ng.module('partnerView', ['uiGridCustom', 'partnerCustomers', 'partnerTransactions', 'partnerActReports'])
      .controller('PartnerViewCtrl', PartnerViewCtrl);

})(window.angular);