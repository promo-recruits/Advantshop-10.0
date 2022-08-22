/* @ngInject */
function MyAccountCtrl($http, $window, $location, toaster, $translate) {

    var ctrl = this,
        activeTabHeader;

    ctrl.orderHistoryMode = 'all';
    ctrl.commonInfo = {};

    ctrl.showContent = function (tabHeader, fromUrl) {
        if (fromUrl) {
            ctrl.showTabs = false;
        }
        activeTabHeader = tabHeader.headerTab;
    };

    ctrl.changeTempEmail = function (email) {
        $http.post("myaccount/updatecustomeremail", { email: email }).then(function (response) {

            if (response.data === true) {
                ctrl.modalWrongNewEmail = false;
                $window.location.reload(true);
            } else {
                ctrl.modalWrongNewEmail = true;
            }
        });
    };

    ctrl.onChangeOrderHistoryMode = function (orderHistoryCtrl, ordernumber) {
        ctrl.orderHistoryCtrl = orderHistoryCtrl;
        $location.search('mode', orderHistoryCtrl.mode);
        $location.search('ordernumber', ordernumber);
        if (orderHistoryCtrl.mode === 'details') {
            ctrl.myaccountTitlePageText = activeTabHeader;
        }
    };

    ctrl.backToFromTabs = function () {

        if (ctrl.orderHistoryMode === 'all') {
            ctrl.showTabs = true;
            $location.search('tab', null);
        } else {
            ctrl.orderHistoryCtrl.changeModeAll();
            ctrl.myaccountTitlePageText = null;
        }
        $location.search('mode', null);
        $location.search('ordernumber', null);
    };

    ctrl.saveUser = function () {
        $http.post("myaccount/saveUserInfo", { userInfo: ctrl.user }).then(function (response) {
            let data = response.data;
            if (data != null && data.result) {
                toaster.pop('success', '', $translate.instant('Js.MyAccount.ChangesSaved'));
            } else if (data != null && data.errors != null) {
                data.errors.forEach(function(err){
                    toaster.pop('error', '', err);
                });
            } else {
                toaster.pop('error', '', $translate.instant('Js.MyAccount.ChangesNotSaved'));
            }
        });
    }

    ctrl.changePassword = function() {
        $http.post("myaccount/changePassword", ctrl.password).then(function (response) {

            if (response.data.result) {
                toaster.pop('success', '', $translate.instant('Js.MyAccount.ChangesSaved'));
            } else if (response.data.errors != null) {
                response.data.errors.forEach(function(err) {
                    toaster.pop('error', '', err);
                })
            } else {
                toaster.pop('error', '', $translate.instant('Js.MyAccount.ChangesNotSaved'));
            }
        });
    }
};

export default MyAccountCtrl;