; (function (ng) {
    'use strict';

    var DomainsManageCtrl = function ($document, $timeout, urlHelper) {

        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.switchOnDomainsManage();
            ctrl.addOnLoadIframe();
        };

        ctrl.$postLink = function () {
            ctrl.pageIsReady = true;


            var selectedValueDomainBinding = urlHelper.getUrlParamByName('selectedValueDomainBinding');
            if (selectedValueDomainBinding != null && selectedValueDomainBinding.length > 0 ) {
                ctrl.connectYourDomain('#iframeDomainsManage', selectedValueDomainBinding);
            }

            var openFunnelId = urlHelper.getUrlParamByName('openFunnel');
            if (openFunnelId != null && openFunnelId.length > 0) {
                ctrl.sendMessage('#iframeDomainsManage', 'openFunnel', openFunnelId);
            }
        };

        ctrl.switchOnDomainsManage = function () {
            ctrl.iframeType = 'domainsManage';
        };

        ctrl.switchOnPay = function () {
            ctrl.iframeType = 'pay';
            doPostMessageDeleteCallback('domainDataLoaded');
        };

        ctrl.connectYourDomain = function (iframeId, selectedValue) {
            if (ctrl.iframeType !== 'domainsManage') {
                ctrl.switchOnDomainsManage();
            }

            $timeout(function () {

                doPostMessageWait('domainDataLoaded', function () {
                    doPostMessage($document[0].querySelector(iframeId), JSON.stringify({ name: 'connectYourDomain', selectedValue: selectedValue }));
                });
            }, 100);  
        };

        ctrl.sendMessage = function (iframeId, name, selectedValue) {
            if (ctrl.iframeType !== 'domainsManage') {
                ctrl.switchOnDomainsManage();
            }

            $timeout(function () {

                doPostMessageWait('domainDataLoaded', function () {
                    doPostMessage($document[0].querySelector(iframeId), JSON.stringify({ name: name, selectedValue: selectedValue }));
                });
            }, 100);
        };

        ctrl.addOnLoadIframe = function () {
            window.onLoadIframeHandler = function () {
                ctrl.loadedIframe = true;
            };
        };
    };

    DomainsManageCtrl.$inject = ['$document','$timeout', 'urlHelper'];

    ng.module('domainsManage', [])
      .controller('DomainsManageCtrl', DomainsManageCtrl);

})(window.angular);