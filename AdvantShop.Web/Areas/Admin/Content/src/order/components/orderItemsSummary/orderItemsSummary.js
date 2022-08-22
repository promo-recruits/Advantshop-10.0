; (function (ng) {
    'use strict';

    var OrderItemsSummaryCtrl = function ($http, $timeout, toaster, SweetAlert, $translate, $uibModal) {
        var ctrl = this, popoverShippingTimer, popoverPaymentTimer;

        ctrl.$onInit = function () {

            ctrl.grastinActionsUrl = 'grastin/getorderactions?orderid=' + ctrl.orderId;
            ctrl.russianPostActionsUrl = 'orders/getOrderActionsRussianPost?orderid=' + ctrl.orderId;
            ctrl.shiptorActionsUrl = 'orders/getOrderActionsShiptor?orderid=' + ctrl.orderId;
            ctrl.sdekActionsUrl = 'orders/getOrderActionsSdek?orderid=' + ctrl.orderId;
            ctrl.hermesActionsUrl = 'orders/getOrderActionsHermes?orderid=' + ctrl.orderId;
            ctrl.pecEasywayActionsUrl = 'orders/getOrderActionsPecEasyway?orderid=' + ctrl.orderId;
            ctrl.pecActionsUrl = 'orders/getOrderActionsPec?orderid=' + ctrl.orderId;
            ctrl.pickPointActionsUrl = 'orders/getOrderActionsPickPoint?orderid=' + ctrl.orderId;
            ctrl.ozonRocketActionsUrl = 'orders/getOrderActionsOzonRocket?orderid=' + ctrl.orderId;

            //ctrl.toggleselectCurrencyLabel('1');

            ctrl.CheckDdeliveryOrder();

            ctrl.getOrderItemsSummary();

            if (ctrl.onInit != null) {
                ctrl.onInit({ orderItemsSummary: ctrl });
            }
        };

        ctrl.toggleselectCurrencyLabel = function (val) {
            ctrl.typeDiscountPercent = val === '1' ? true : false;
            ctrl.selectCurrency = val;
        };

        ctrl.getOrderItemsSummary = function () {
            $http.get('orders/getOrderItemsSummary', { params: { orderId: ctrl.orderId } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.Summary = data;
                    ctrl.toggleselectCurrencyLabel(ctrl.Summary.OrderDiscount > 0 ? '1' : '0');
                }
            });
        };

        ctrl.changeShipping = function (result) {

            ctrl.getOrderItemsSummary();
            ctrl.grastinUpdateActions();
            ctrl.russianPostUpdateActions();
            ctrl.shiptorUpdateActions();
            ctrl.sdekUpdateActions();
            ctrl.hermesUpdateActions();
            ctrl.pecEasywayUpdateActions();
            ctrl.pecUpdateActions();
            ctrl.pickPointUpdateActions();
            ctrl.ozonRocketUpdateActions();

            if (result != null) {
                toaster.pop('success', '', $translate.instant('Admin.Js.Order.ShippingMethodSaved'));
            }
        };


        ctrl.changePayment = function (result) {
            ctrl.getOrderItemsSummary();

            if (result != null) {
                toaster.pop('success', '', $translate.instant('Admin.Js.Order.PaymentMethodSaved'));
            }
        };


        /* shippings */

        ctrl.getOrderTrackNumber = function() {
            $http.post('orders/getOrderTrackNumber', { orderId: ctrl.orderId }).then(function(response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.trackNumber = data.object;
                }
            });
        };

        ctrl.baseShippingRequest = function (url, params, flag, onSuccess, onError) {
            if (ctrl[flag]) {
                return;
            }
            ctrl[flag] = true;
            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    if (data.message) {
                        toaster.success('', data.message);
                    }

                    ctrl.getOrderTrackNumber();

                    if (onSuccess) {
                        onSuccess(data);
                    }
                } else {
                    if (data.errors) {
                        data.errors.forEach(function (error) {
                            toaster.error('', error);
                        });
                    } else if (data.error){
                        toaster.error('', data.error);
                    }
                    if (onError) {
                        onError(data);
                    }
                }
            }).finally(function () {
                ctrl[flag] = false;
                toaster.clear(toasterWait);
            });
            var toasterWait = toaster.pop('wait', '', 'Задача выполняется');
        };

        ctrl.createYandexDeliveryOrder = function () {
            ctrl.baseShippingRequest('orders/createYandexDeliveryOrder', { orderId: ctrl.orderId }, 'sendingCreateYandexDeliveryOrder');
        };

        ctrl.createYandexNewDeliveryOrder = function () {
            ctrl.baseShippingRequest('orders/createYandexNewDeliveryOrder', { orderId: ctrl.orderId }, 'sendingCreateYandexNewDeliveryOrder');
        };

        ctrl.createCheckoutRuOrder = function () {
            ctrl.baseShippingRequest('orders/createCheckoutRuOrder', { orderId: ctrl.orderId }, 'sendingCreateCheckoutRuOrder');
        };

        ctrl.sdekOrderPrintForm = function () {
            window.location = 'orders/sdekOrderPrintForm?orderId=' + ctrl.orderId;
        };

        ctrl.createSdekOrder = function () {
            ctrl.baseShippingRequest('orders/createSdekOrder', { orderId: ctrl.orderId }, 'sendingCreateSdekOrder',
                function (data) {
                    ctrl.grastinUpdateActionsAndSummary();
                });
        };

        ctrl.sdekDeleteOrder = function () {
            ctrl.baseShippingRequest('orders/sdekDeleteOrder', { orderId: ctrl.orderId }, 'sendingSdekDeleteOrder',
                function (data) {
                    ctrl.grastinUpdateActionsAndSummary();
                });
        };

        ctrl.createBoxberryOrder = function () {
            ctrl.baseShippingRequest('orders/createBoxberryOrder', { orderId: ctrl.orderId }, 'sendingCreateBoxberryOrder');
        };

        ctrl.deleteBoxberryOrder = function () {
            ctrl.baseShippingRequest('orders/deleteBoxberryOrder', { orderId: ctrl.orderId }, 'sendingDeleteBoxberryOrder');
        };

        ctrl.grastinOrderPrintMark = function () {
            ctrl.baseShippingRequest('orders/grastinSendRequestForMark', { orderId: ctrl.orderId }, 'sendingGrastinOrderPrintMark',
                function (data) {
                    window.location = "orders/GrastinOrderPrintMark?filename=" + encodeURIComponent(data.obj.FileName);
                });
        };

        ctrl.createShiptorOrder = function () {
            ctrl.baseShippingRequest('orders/createShiptorOrder', { orderId: ctrl.orderId }, 'sendingCreateShiptorOrder',
                function (data) {
                    ctrl.grastinUpdateActionsAndSummary();
                });
        };

        ctrl.createRussianPostOrder = function (additionalAction, additionalActionData) {
            ctrl.baseShippingRequest('orders/createRussianPostOrder', { orderId: ctrl.orderId, additionalAction: additionalAction, additionalActionData: additionalActionData }, 'sendingCreateRussianPostOrder',
                function (data) {
                    if (data.obj) {
                        if (data.obj.additionalAction) {
                            if (data.obj.additionalAction === 'fill_additional_data_for_customs_declaration') {
                                $uibModal.open({
                                    bindToController: true,
                                    controller: 'ModalRussianPostCustomsDeclarationProductDataCtrl',
                                    controllerAs: 'ctrl',
                                    size: 'lg',
                                    backdrop: 'static',
                                    templateUrl: '../areas/admin/content/src/order/modal/shippings/russianPost/customsDeclarationProductData/customsDeclarationProductData.html',
                                    resolve: {
                                        params: {
                                            products: data.obj.additionalActionData.Products
                                        }
                                    }
                                }).result.then(function (result) {
                                    if (result && result.products) {
                                        ctrl.createRussianPostOrder(data.obj.additionalAction, JSON.stringify(result.products));
                                    }
                                    return result;
                                });
                            }
                        }
                        if (data.obj.errors) {
                            data.obj.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        } else if (data.obj.error) {
                            toaster.error('', data.obj.error);
                        }

                    } else {
                        ctrl.grastinUpdateActionsAndSummary();
                    }
                });
        };

        ctrl.deleteRussianPostOrder = function () {
            ctrl.baseShippingRequest('orders/deleteRussianPostOrder', { orderId: ctrl.orderId }, 'sendingDeleteRussianPostOrder',
                function (data) {
                    ctrl.grastinUpdateActionsAndSummary();
                });
        };

        ctrl.russianPostGetDocumentsBeforShipment = function () {
            ctrl.baseShippingRequest('orders/russianPostGetDocumentsBeforShipment', { orderId: ctrl.orderId }, 'sendingRussianPostGetDocumentsBeforShipment',
                function (data) {
                    window.location = "orders/russianPostGetFileDocuments?filename=" + encodeURIComponent(data.obj.FileName);
                });
        };

        ctrl.russianPostGetDocuments = function () {
            ctrl.baseShippingRequest('orders/russianPostGetDocuments', { orderId: ctrl.orderId }, 'sendingRussianPostGetDocuments',
                function (data) {
                    window.location = "orders/russianPostGetFileDocuments?filename=" + encodeURIComponent(data.obj.FileName);
                });
        };

        ctrl.createHermesOrderStandart = function () {
            ctrl.baseShippingRequest('orders/createHermesOrderStandart', { orderId: ctrl.orderId }, 'sendingCreateHermesOrderStandart',
                function (data) {
                    ctrl.updateActionsAndSummary();
                });
        };

        ctrl.createHermesOrderVSD = function () {
            ctrl.baseShippingRequest('orders/createHermesOrderVsd', { orderId: ctrl.orderId }, 'sendingCreateHermesOrderVsd',
                function (data) {
                    ctrl.updateActionsAndSummary();
                });
        };

        ctrl.createHermesOrderDrop = function () {
            ctrl.baseShippingRequest('orders/createHermesOrderDrop', { orderId: ctrl.orderId }, 'sendingCreateHermesOrderDrop',
                function (data) {
                    ctrl.updateActionsAndSummary();
                });
        };

        ctrl.deleteHermesOrder = function () {
            ctrl.baseShippingRequest('orders/deleteHermesOrder', { orderId: ctrl.orderId }, 'sendingDeleteHermesOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                });
        };

        ctrl.createPecEasywayOrder = function () {
            ctrl.baseShippingRequest('orders/createPecEasywayOrder', { orderId: ctrl.orderId }, 'sendingCreatePecEasywayOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                });
        };

        ctrl.cancelPecEasywayOrder = function () {
            ctrl.baseShippingRequest('orders/cancelPecEasywayOrder', { orderId: ctrl.orderId }, 'sendingCancelPecEasywayOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                });
        };

        ctrl.createPecOrder = function () {
            ctrl.baseShippingRequest('orders/createPecOrder', { orderId: ctrl.orderId }, 'sendingCreatePecOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                });
        };

        ctrl.cancelPecOrder = function () {
            ctrl.baseShippingRequest('orders/cancelPecOrder', { orderId: ctrl.orderId }, 'sendingCancelPecOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                });
        };

        ctrl.createPickPointOrder = function () {
            ctrl.baseShippingRequest('orders/createPickPointOrder', { orderId: ctrl.orderId }, 'sendingCreatePickPointOrder',
                function (data) {
                    ctrl.pickPointUpdateActions();
                });
        };

        ctrl.deletePickPointOrder = function () {
            ctrl.baseShippingRequest('orders/deletePickPointOrder', { orderId: ctrl.orderId }, 'sendingDeletePickPointOrder',
                function (data) {
                    ctrl.pickPointUpdateActions();
                });
        };

        ctrl.createDdeliveryOrder = function () {
            ctrl.baseShippingRequest('orders/createDDeliveryOrder', { orderId: ctrl.orderId }, 'sendingCreateDdeliveryOrder',
                function (data) {
                    ctrl.CheckDdeliveryOrder();
                });
        };

        ctrl.createOzonRocketOrder = function () {
            ctrl.baseShippingRequest('orders/createOzonRocketOrder', { orderId: ctrl.orderId }, 'sendingCreateOzonRocketOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                });
        };

        ctrl.cancelOzonRocketOrder = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Order.CancelOzonRocketOrder'), { title: 'Ozon Rocket' }).then(function (result) {
                if (result === true) {
                    ctrl.baseShippingRequest('orders/cancelOzonRocketOrder', {orderId: ctrl.orderId}, 'sendingCancelOzonRocketOrder',
                        function (data) {
                            ctrl.updateActionsAndSummary();
                        });
                }
            });
        };

        ctrl.getDdeliveryOrderInfo = function () {
            window.location = 'orders/ddeliveryOrderInfo?orderId=' + ctrl.orderId;
            //$http.post('orders/DDeliveryOrderInfo', { orderId: ctrl.orderId }).then(function (response) {
            //    var data = response.data;
            //    if (data.result === true) {
            //        toaster.pop('success', '', data.message);
            //    } else {
            //        toaster.pop('error', '', data.error);
            //    }
            //});
        };

        ctrl.canselDdeliveryOrder = function () {
            ctrl.baseShippingRequest('orders/CanselDDeliveryOrder', { orderId: ctrl.orderId }, 'sendingCancelDdeliveryOrder',
                function (data) {
                    ctrl.CheckDdeliveryOrder();
                });
        };

        ctrl.CheckDdeliveryOrder = function () {
            ctrl.isExistDdeliveryOrder = false;
            $http.post('orders/IsExistDDeliveryOrder', { orderId: ctrl.orderId }).then(function (response) {
                var data = response.data;
                ctrl.isExistDdeliveryOrder = data.result;
            });
        };

        ctrl.grastinUpdateActions = function () {
            ctrl.grastinActionsUrl = 'grastin/getorderactions?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };

        ctrl.russianPostUpdateActions = function () {
            ctrl.russianPostActionsUrl = 'orders/getOrderActionsRussianPost?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };

        ctrl.sdekUpdateActions = function () {
            ctrl.sdekActionsUrl = 'orders/getOrderActionsSdek?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };

        ctrl.sdekUpdatedDispatchNumber = function () {
            ctrl.sdekUpdateActions();
        };

        ctrl.shiptorUpdateActions = function () {
            ctrl.shiptorActionsUrl = 'orders/getOrderActionsShiptor?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };

        ctrl.hermesUpdateActions = function () {
            ctrl.hermesActionsUrl = 'orders/getOrderActionsHermes?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };

        ctrl.pecEasywayUpdateActions = function () {
            ctrl.pecEasywayActionsUrl = 'orders/getOrderActionsPecEasyway?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };

        ctrl.pecUpdateActions = function () {
            ctrl.pecActionsUrl = 'orders/getOrderActionsPec?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };

        ctrl.pickPointUpdateActions = function () {
            ctrl.pickPointActionsUrl = 'orders/getOrderActionsPickPoint?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };

        ctrl.ozonRocketUpdateActions = function () {
            ctrl.ozonRocketActionsUrl = 'orders/getOrderActionsOzonRocket?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.updateActionsAndSummary = function () {
            ctrl.getOrderItemsSummary();
            ctrl.grastinUpdateActions();
            ctrl.russianPostUpdateActions();
            ctrl.shiptorUpdateActions();
            ctrl.sdekUpdateActions();
            ctrl.hermesUpdateActions();
            ctrl.pecEasywayUpdateActions();
            ctrl.pecUpdateActions();
            ctrl.pickPointUpdateActions();
            ctrl.ozonRocketUpdateActions();
        };

        ctrl.grastinUpdateActionsAndSummary = function () {
            ctrl.updateActionsAndSummary();
        };

        /* end shippings */

        /* discount */
        ctrl.discountPopoverOpen = function () {
            ctrl.OrderDiscountNew = ctrl.typeDiscountPercent ? ctrl.Summary.OrderDiscount : ctrl.Summary.ProductsDiscountPrice;
            ctrl.discountPopoverIsOpen = true;
        };

        ctrl.discountPopoverClose = function () {
            ctrl.discountPopoverIsOpen = false;
        };

        ctrl.discountPopoverToggle = function () {
            ctrl.discountPopoverIsOpen === true ? ctrl.discountPopoverClose() : ctrl.discountPopoverOpen();
        };

        ctrl.changeDiscount = function (discount) {

            if (ctrl.orderId === 0) return;

            $http.post("orders/changeDiscount", { orderId: ctrl.orderId, orderDiscount: discount, isValue: ctrl.selectCurrency === "0" }).then(function (response) {
                if (response.data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.DiscountSaved'));
                    if (ctrl.selectCurrency === "1") {
                        ctrl.Summary.OrderDiscount = discount;
                    } else {
                        ctrl.Summary.OrderDiscountValue = discount;
                    }
                }

                return response.data;
            }).finally(function () {
                ctrl.getOrderItemsSummary();
                ctrl.discountPopoverClose();
            });
        };

        ctrl.getPaymentDetailsLink = function (withoutStamp) {
            var link = ctrl.Summary.PrintPaymentDetailsLink;

            if (ctrl.Summary.PaymentDetails != null) {
                if (ctrl.Summary.PaymentDetails.INN != null && ctrl.Summary.PaymentDetails.INN.length > 0) {
                    link += '&bill_INN=' + ctrl.Summary.PaymentDetails.INN;
                }

                if (ctrl.Summary.PaymentDetails.CompanyName != null && ctrl.Summary.PaymentDetails.CompanyName.length > 0) {
                    link += '&bill_CompanyName=' + ctrl.Summary.PaymentDetails.CompanyName;
                }

                if (ctrl.Summary.PaymentDetails.Contract != null && ctrl.Summary.PaymentDetails.Contract.length > 0) {
                    link += '&bill_Contract=' + ctrl.Summary.PaymentDetails.Contract;
                }

                if (withoutStamp) {
                    link += '&withoutStamp=true';
                }
            }

            return link;
        };


        /* bonuses */
        ctrl.bonusesPopoverOpen = function () {
            ctrl.bonusesPopoverIsOpen = true;
        };

        ctrl.bonusesPopoverClose = function () {
            ctrl.bonusesPopoverIsOpen = false;
        };

        ctrl.bonusesPopoverToggle = function () {
            ctrl.bonusesPopoverIsOpen === true ? ctrl.bonusesPopoverClose() : ctrl.bonusesPopoverOpen();
        };

        ctrl.useBonuses = function (bonusesAmount) {
            SweetAlert.confirm($translate.instant('Admin.Js.Order.WriteOffBonuses'), { title: $translate.instant('Admin.Js.Order.WritingOffBonuses') }).then(function (result) {
                if (result === true) {
                    $http.post('orders/useBonuses', { orderId: ctrl.orderId, bonusesAmount: bonusesAmount }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                        } else {
                            toaster.pop('error', '', data.error);
                        }
                    }).finally(function () {
                        ctrl.getOrderItemsSummary();
                        ctrl.bonusesPopoverClose();
                    });
                }
            });
        }

        ctrl.popoverShippingOpen = function () {

            if (popoverShippingTimer != null) {
                $timeout.cancel(popoverShippingTimer);
            }

            ctrl.popoverShippingIsOpen = true;
        };

        ctrl.popoverShippingClose = function () {

            popoverShippingTimer = $timeout(function () {
                ctrl.popoverShippingIsOpen = false;
            }, 500);
        };

        ctrl.popoverPaymentOpen = function () {

            if (popoverPaymentTimer != null) {
                $timeout.cancel(popoverPaymentTimer);
            }

            ctrl.popoverPaymentIsOpen = true;
        };

        ctrl.popoverPaymentClose = function () {

            popoverPaymentTimer = $timeout(function () {
                ctrl.popoverPaymentIsOpen = false;
            }, 500);
        };

        ctrl.savePaymentDetails = function () {
            var params = ng.extend(ctrl.Summary.PaymentDetails, { orderId: ctrl.orderId });

            $http.post('orders/updatePaymentDetails', params)
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    }
                });
        }

        /* coupons */

        ctrl.couponsPopoverOpen = function () {
            ctrl.couponsPopoverIsOpen = true;
        }

        ctrl.couponsPopoverClose = function () {
            ctrl.couponsPopoverIsOpen = false;
        };
        
        ctrl.couponsPopoverToggle = function () {
            ctrl.couponsPopoverIsOpen === true ? ctrl.couponsPopoverClose() : ctrl.couponsPopoverOpen();
        };

        ctrl.changeCoupon = function (couponCode) {

            if (ctrl.orderId === 0) return;

            $http.post("orders/changeCoupon", { orderId: ctrl.orderId, couponCode: couponCode }).then(function (response) {
                if (response.data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.CouponSaved'));
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Order.CouponSavingError'));
                }
                return response.data;
            }).finally(function () {
                ctrl.getOrderItemsSummary();
                ctrl.couponsPopoverClose();
            });
        }

        ctrl.removeCoupon = function () {
            $http.post("orders/removeCoupon", { orderId: ctrl.orderId }).then(function (response) {
                if (response.data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.CouponRemoved'));
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Order.CouponRemoveError'));
                }
            }).finally(function () {
                ctrl.getOrderItemsSummary();
                ctrl.couponsPopoverClose();
            });
        };

        /* certificate */

        ctrl.certificatePopoverOpen = function () {
            ctrl.certificatePopoverIsOpen = true;
        }

        ctrl.certificatePopoverClose = function () {
            ctrl.certificatePopoverIsOpen = false;
        };

        ctrl.certificatePopoverToggle = function () {
            ctrl.certificatePopoverIsOpen === true ? ctrl.certificatePopoverClose() : ctrl.certificatePopoverOpen();
        };

        ctrl.changeCertificate = function (code) {

            if (ctrl.orderId === 0) return;

            $http.post("orders/changeCertificate", { orderId: ctrl.orderId, code: code }).then(function (response) {
                if (response.data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Order.CertificateSavingError'));
                }
                return response.data;
            }).finally(function () {
                ctrl.getOrderItemsSummary();
                ctrl.certificatePopoverClose();
            });
        }

        ctrl.removeCertificate = function () {
            $http.post("orders/removeCertificate", { orderId: ctrl.orderId }).then(function (response) {
                if (response.data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.OrderCertificateRemoveError'));
                }
            }).finally(function () {
                ctrl.getOrderItemsSummary();
                ctrl.certificatePopoverClose();
            });
        };

        /* end certificate */

        ctrl.demensionsStartEdit = function () {
            ctrl.demensionsBackup = {
                height: ctrl.Summary.TotalHeight,
                width: ctrl.Summary.TotalWidth,
                length: ctrl.Summary.TotalLength
            };

            ctrl.demensionsEdit = true;
        };

        ctrl.demensionsCancelEdit = function () {

            ctrl.Summary.TotalHeight = ctrl.demensionsBackup.height;
            ctrl.Summary.TotalWidth = ctrl.demensionsBackup.width;
            ctrl.Summary.TotalLength = ctrl.demensionsBackup.length;

            ctrl.demensionsEdit = false;
        };

        ctrl.demensionsApplyEdit = function () {

            var listValues = [ctrl.Summary.TotalWidth, ctrl.Summary.TotalHeight, ctrl.Summary.TotalLength];

            if (listValues.every(function (item) { return item != null && item !== ''; }) === false && listValues.every(function (item) { return item == null || item === ''; }) === false) {
                toaster.pop('error', $translate.instant('Admin.Js.OrdersItemsSummary.InvalidDimensionsValues'));
                return;
            }

            $http.post('orders/updateDimesions', { orderId: ctrl.orderId, width: ctrl.Summary.TotalWidth, height: ctrl.Summary.TotalHeight, length: ctrl.Summary.TotalLength }).then(function (response) {
                var data = response.data;
                if (data.result === true) {

                    ctrl.Summary.TotalWidth = data.obj.width;
                    ctrl.Summary.TotalHeight = data.obj.height;
                    ctrl.Summary.TotalLength = data.obj.length;
                    ctrl.Summary.IsNotEditedDimensions = data.obj.IsNotEditedDimensions;

                    toaster.pop('success', '', $translate.instant('Admin.Js.OrdersItemsSummary.DimensionsSaved'));
                    ctrl.demensionsEdit = false;

                    ctrl.getOrderItemsSummary();

                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.OrdersItemsSummary.ErrorDimensionsSaved'));
                }
            });
        };

        ctrl.weightStartEdit = function () {
            ctrl.weightBackup = ctrl.Summary.TotalWeight;
            ctrl.weightEdit = true;
        };

        ctrl.weightCancelEdit = function () {
            ctrl.Summary.TotalWeight = ctrl.weightBackup;
            ctrl.weightEdit = false;
        };

        ctrl.weightApplyEdit = function () {
            $http.post('orders/updateWeight', { orderId: ctrl.orderId, weight: ctrl.Summary.TotalWeight }).then(function (response) {
                var data = response.data;
                if (data.result === true) {

                    ctrl.Summary.TotalWeight = data.obj.weight;
                    ctrl.Summary.IsNotEditedWeight = data.obj.IsNotEditedWeight;

                    toaster.pop('success', '', $translate.instant('Admin.Js.OrdersItemsSummary.WeightSaved'));
                    ctrl.weightEdit = false;

                    ctrl.getOrderItemsSummary();
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.OrdersItemsSummary.ErrorWeightSaved'));
                }
            });
        };

        ctrl.customerCommentStartEdit = function () {
            ctrl.customerCommentBackup = ctrl.Summary.CustomerComment;
            ctrl.customerCommentEdit = true;
        };

        ctrl.customerCommentApplyEdit = function () {

            $http.post('orders/updateCustomerComment', { orderId: ctrl.orderId, customerComment: ctrl.Summary.CustomerComment }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    ctrl.customerCommentEdit = false;
                }
            });
        };

        ctrl.customerCommentCancelEdit = function () {
            ctrl.Summary.CustomerComment = ctrl.customerCommentBackup;
            ctrl.customerCommentEdit = false;
        };
    };

    OrderItemsSummaryCtrl.$inject = ['$http', '$timeout', 'toaster', 'SweetAlert', '$translate', '$uibModal'];

    ng.module('orderItemsSummary', [])
        .controller('OrderItemsSummaryCtrl', OrderItemsSummaryCtrl)
        .component('orderItemsSummary', {
            templateUrl: '../areas/admin/content/src/order/components/orderItemsSummary/orderItemsSummary.html',
            controller: OrderItemsSummaryCtrl,
            transclude: {
                'footerLeft': '?footerLeft'
            },
            bindings: {
                orderId: '=',
                onInit: '&',
                country: '=',
                region: '=',
                district: '=',
                city: '=',
                zip: '=',
                isEdit: '<',
                onStopEdit: '&',
                statusComment: '@',
                adminComment: '@',
                trackNumber: '='
            }

        });

})(window.angular);