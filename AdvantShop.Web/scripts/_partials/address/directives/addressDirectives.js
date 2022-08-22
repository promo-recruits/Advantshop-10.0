function addressListDirective() {
    return {
        restrict: 'A',
        scope: {
            type: '@', //change or view
            initAddressFn: '&',
            changeAddressFn: '&',
            saveAddressFn: '&',
            isShowFullAddress: '<?',
            isShowName: '<?'
        },
        controller: 'AddressListCtrl',
        controllerAs: 'addressList',
        bindToController: true,
        replace: true,
        templateUrl: '/scripts/_partials/address/templates/list.html'
    };
}

export { addressListDirective };