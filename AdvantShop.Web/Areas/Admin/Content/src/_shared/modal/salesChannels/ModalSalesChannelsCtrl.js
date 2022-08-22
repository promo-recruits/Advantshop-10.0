; (function (ng) {
    'use strict';

    var ModalSalesChannelsCtrl = function ($http, $window, SweetAlert, toaster, $timeout, domService, $sce, $document, $translate, $uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.$resolve.data != null) {
                if (ctrl.$resolve.data.selectedChannelTypeStr != null) {
                    ctrl.externalChannelTypeStr = ctrl.$resolve.data.selectedChannelTypeStr;
                }
                ctrl.closeOnComplete = ctrl.$resolve.data.closeOnComplete === true;
            }
            ctrl.getList();
            ctrl.isLoadCarousel = false;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.changeSaleChannel = function (event, item) {
            if (event != null) {
                var chosenSale = domService.closest(event.target, '.card-channel-inner');
                if (chosenSale != null) {
                    ctrl.scrollChosenSale = chosenSale.scrollTop;
                }
            }
            ctrl.selectedChannel = item;
            $timeout(function () {
                ctrl.isLoadCarousel = true;
            });
        }

        ctrl.hiddenAnimatedElements = function (event, className) {
            var currentTarget = event.currentTarget;
            var target = event.target;
            if (target.classList.contains('card-channel-inner') && target.classList.contains('modal-left-animation')) {
                target.style.height = '0px';
                target.style.overflow = 'hidden';

                var innerPage = currentTarget.querySelector('.card-channel-details');
                if (innerPage != null) {
                    innerPage.scrollIntoView(true);
                }
            }

            if (target.classList.contains('card-channel-inner') && target.classList.contains('modal-right-animation')) {
                target.scrollTop = ctrl.scrollChosenSale;
            }
        }

        ctrl.showAnimatedElements = function (event, className) {
            var target = event.target;

            if (target.classList.contains('card-channel-inner') && target.classList.contains('modal-right-animation')) {
                target.style.height = '100%';
                target.style.overflow = 'auto';
            }
        }

        ctrl.addSaleChannel = function () {

            ctrl.addDisabled = true;

            if (ctrl.selectedChannel.Type == 'Module') { //  && ctrl.selectedChannel.Details.Price == 0

                var data = {
                    stringId: ctrl.selectedChannel.ModuleStringId,
                    id: ctrl.selectedChannel.ModuleId,
                    version: ctrl.selectedChannel.ModuleVersion,
                    active: true
                };

                $http.post('modules/installModule', data).then(function(response) {
                    if (response.data.result === true) {
                        $window.location = response.data.url;
                        toaster.pop('success', '', 'Канал продаж установлен');
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Modules.ErrorInstallingModule'));
                    }
                }).finally(function () {
                    ctrl.addDisabled = false;
                });
            } else {
                $http.post('salesChannels/add', { type: ctrl.selectedChannel.Type, moduleStringId: ctrl.selectedChannel.ModuleStringId }).then(function (response) {
                    var data = response.data;
                    if (ctrl.closeOnComplete) {
                        $uibModalInstance.close(true);
                    } else if (data.obj.url == null || ctrl.externalChannelTypeStr != null) {
                        $window.location.reload();
                    } else {
                        $window.location.assign(data.obj.url);
                    }
                    toaster.pop('success', '', 'Канал продаж установлен');

                }).finally(function () {
                    ctrl.addDisabled = false;
                });
            }
        }

        ctrl.removeSaleChannel = function () {
            SweetAlert.confirm('Вы уверены что хотите удалить канал?', { title: 'Удаление' }).then(function () {

                ctrl.removeDisabled = true;

                if (ctrl.selectedChannel.Type == 'Module') {

                    $http.post('modules/uninstallModule', { stringId: ctrl.selectedChannel.ModuleStringId }).then(function(response) {
                        if (response.data.result === true) {
                            toaster.pop('success', '', 'Канал продаж удален');
                            var basePath = document.getElementsByTagName('base')[0].getAttribute('href');
                            $window.location.assign(basePath);
                        }
                    }).finally(function() {
                        ctrl.removeDisabled = false;
                    });

                } else {
                    $http.post('salesChannels/delete', { type: ctrl.selectedChannel.Type, moduleStringId: ctrl.selectedChannel.ModuleStringId }).then(function (response) {
                        var data = response.data;
                        if (data.obj != null && data.obj.url != null) {
                            $window.location.assign(data.obj.url);
                        } else {
                            toaster.pop('success', '', 'Канал продаж удален');
                            var basePath = document.getElementsByTagName('base')[0].getAttribute('href');
                            $window.location.assign(basePath);
                        }
                    }).finally(function () {
                        ctrl.removeDisabled = false;
                    });
                }
            });
        }

        ctrl.getList = function () {
            $http.get('salesChannels/getList').then(function (response) {
                ctrl.channels = response.data;

                ctrl.salesChannelsEnabled = ctrl.channels.filter(function (x) { return x.Enabled });
                ctrl.salesChannelsNotEnabled = ctrl.channels.filter(function (x) { return !x.Enabled });

                if (ctrl.externalChannelTypeStr != null) {
                    var items = ctrl.salesChannelsNotEnabled.filter(function (element) {
                        if (element.TypeStr == ctrl.$resolve.data.selectedChannelTypeStr) {
                            return element;
                        }
                    });
                    if (items != null && items.length > 0) {
                        ctrl.changeSaleChannel(null, items[0]);
                    }
                }
            });
        }

        ctrl.backToMenu = function () {

            var iframeVideo = $document[0].getElementById('funnelVideo');
            if (iframeVideo) {
                iframeVideo.contentWindow.postMessage('{"event":"command","func":"' + 'stopVideo' + '","args":""}', '*');
            }
            ctrl.selectedChannel = null;
            $timeout(function () {
                ctrl.isLoadCarousel = false;
            });
        }

        ctrl.getTrustVideoSrc = function (videoSrc) {
            return ctrl.urlVideo = $sce.trustAsResourceUrl(videoSrc + '?enablejsapi=1&version=3&playerapiid=funnelVideo');
        }

    };



    ModalSalesChannelsCtrl.$inject = ['$http', '$window', 'SweetAlert', 'toaster', '$timeout', 'domService', '$sce', '$document', '$translate', '$uibModalInstance'];

    ng.module('modalSalesChannels', [])
        .controller('ModalSalesChannelsCtrl', ModalSalesChannelsCtrl)
        .directive('animationObserver', function () {
            return {
                restrict: 'A',
                scope: {
                    animationend: '&',
                    animationstart: '&'
                },
                link: function (scope, element) {
                    var eventsEnd = 'animationend webkitAnimationEnd MSAnimationEnd transitionend webkitTransitionEnd';
                    var eventsStart = 'animationstart transitionstart webkitTransitionStart';
                    if (scope.animationend != null) {
                        element.on(eventsEnd, function (event) {
                            scope.animationend({ event: event, element: element });
                        });
                    }

                    if (scope.animationstart != null) {
                        element.on(eventsStart, function (event) {
                            scope.animationstart({ event: event, element: element });
                        });
                    }

                    scope.$on('$destroy', function () {
                        element.off(eventsEnd);
                        element.off(eventsStart);
                    });
                }
            };
        })

})(window.angular);
