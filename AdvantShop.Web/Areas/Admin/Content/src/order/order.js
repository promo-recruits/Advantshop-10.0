; (function (ng) {
    'use strict';

    var OrderCtrl = function (uiGridCustomConfig, $http, $httpParamSerializer, toaster, $timeout, $uibModal, $q, SweetAlert, lastStatisticsService, $translate, $window) {

        var ctrl = this;
        var timerChangeCustomer,
            timerProcessAddress;

        ctrl.initOrder = function (orderId, isEditMode, isDraft, customerId, standardPhone) {
            ctrl.orderId = orderId;
            ctrl.isEditMode = isEditMode;
            ctrl.isDraft = isDraft;
            ctrl.customerId = customerId;
            ctrl.standardPhone = standardPhone;
        };

        ctrl.startGridOrderItems = function (isPaied) {
            ctrl.isPaied = isPaied;

            ctrl.gridOrderItemsOptions = ng.extend({}, uiGridCustomConfig, {
                rowHeight: 95,
                columnDefs: [
                    {
                        name: "Position",
                        displayName: $translate.instant('Admin.Js.Order.Position'),
                        cellTemplate: '<div class="ui-grid-cell-contents"><span class="order-grid__css-counter"></span></div>',
                        width: 45,
                        enableSorting: false
                    },
                    {
                        name: 'ImageSrc',
                        displayName: '',
                        cellTemplate: '<div class="ui-grid-cell-contents"><img class="ui-grid-custom-col-img" ng-src="{{row.entity.ImageSrc}}"></div>',
                        width: 100,
                        enableSorting: false,
                        enableCellEdit: false
                    },
                    {
                        name: 'Name',
                        displayName: $translate.instant('Admin.Js.Order.Name'),
                        cellTemplate:
                            '<div class="ui-grid-cell-contents ui-grid-cell-contents-order-items">' +
                            '<div>' +
                            '<div ng-if="row.entity.ProductLink != null"><a href="{{row.entity.ProductLink}}" target="_blank" ng-class="{\'order-item-not-enabled\': !row.entity.Enabled}">{{row.entity.Name}}</a></div> ' +
                            '<div ng-if="row.entity.ProductLink == null">{{row.entity.Name}}</div> ' +
                            '<div class="order-item-artno">' + $translate.instant('Admin.Js.Order.VendorCode') + '{{row.entity.ArtNo}}</div> ' +
                            '<div class="order-item-artno" ng-if="row.entity.BarCode != null && row.entity.BarCode.length > 0">' + $translate.instant('Admin.Js.Order.BarCode') + '{{row.entity.BarCode}}</div> ' +
                            '<div ng-if="row.entity.Color != null && row.entity.Color.length > 0">{{row.entity.Color}}</div>' +
                            '<div ng-if="row.entity.Size != null && row.entity.Size.length > 0">{{row.entity.Size}}</div>' +
                            '<div ng-if="row.entity.CustomOptions != null && row.entity.CustomOptions.length > 0"> <div ng-bind-html="row.entity.CustomOptions"></div> </div>' +
                            '<div ng-if="!row.entity.Available" class="order-notavalable">{{row.entity.AvailableText}}</div>' +
                            '<div ng-if="row.entity.Length != 0 && row.entity.Width != 0 && row.entity.Height != 0">' + $translate.instant('Admin.Js.Order.Dimensions') + ': {{row.entity.Length}} x {{row.entity.Width}} x {{row.entity.Height}} мм </div>' +
                            '<div ng-if="row.entity.Weight != 0">' + $translate.instant('Admin.Js.Order.Weight') + ': {{row.entity.Weight}} кг </div>' +
                            '<div ng-if="!grid.appScope.$ctrl.gridExtendCtrl.isPaied && row.entity.ShowEditCustomOptions"> ' +
                                '<ui-modal-trigger data-controller="\'ModalEditCustomOptionsCtrl\'" data-controller-as="ctrl" ' +
                                                  'data-resolve="{params: { orderItemId: row.entity.OrderItemId, productId: row.entity.ProductId, artno: row.entity.ArtNo}}" ' +
                                                  'data-on-close="grid.appScope.$ctrl.gridExtendCtrl.gridOrderItemUpdate()" ' +
                                                  'template-url="../areas/admin/content/src/order/modal/editCustomOptions/editCustomOptions.html">' +
                                '<a href="" class="order-item-edit-custom-options">изменить доп. опции</a></ui-modal-trigger>' +
                            '</div>' +
                            '</div>' +
                            '</div>',
                        enableCellEdit: false,
                    },
                    {
                        name: 'PriceString',
                        displayName: $translate.instant('Admin.Js.Order.Price'),
                        enableCellEdit: true,
                        width: 100,
                    },
                    {
                        name: 'Amount',
                        displayName: $translate.instant('Admin.Js.Order.Amount'),
                        enableCellEdit: true,
                        width: 60,
                    },
                    {
                        name: 'Cost',
                        displayName: $translate.instant('Admin.Js.Order.Cost'),
                        width: 100,
                    },
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 35,
                        enableSorting: false,
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><div>' +
                            '<div ng-if="!grid.appScope.$ctrl.gridExtendCtrl.isPaied"> <ui-grid-custom-delete url="orders/deleteOrderItem" params="{\'orderId\': row.entity.OrderId, \'orderItemId\': row.entity.OrderItemId }"></ui-grid-custom-delete> </div>' +
                            '<div ng-if="grid.appScope.$ctrl.gridExtendCtrl.isPaied"> <a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.checkStopEdit()" class="ui-grid-custom-service-icon fa fa-times link-invert"></a> </div>' +
                            '</div></div>'
                    }
                ],
                paginationPageSize: 20,
                paginationPageSizes: [20, 50, 100]
            });

            ctrl.gridOrderCertificatesOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'CustomName',
                        displayName: '',
                        cellTemplate: '<div class="ui-grid-cell-contents">' + $translate.instant('Admin.Js.Order.Certificate') + '</div>'
                    },
                    {
                        name: 'CertificateCode',
                        displayName: $translate.instant('Admin.Js.Order.CertificateCode')
                    },
                    {
                        name: 'Sum',
                        displayName: $translate.instant('Admin.Js.Order.Sum')
                    },
                    {
                        name: 'Price',
                        displayName: $translate.instant('Admin.Js.Order.UsedInOrderN')
                    }
                ]
            });

            ctrl.isShowGridOrderItem = true;
        };

        ctrl.gridOrderItemsOnInit = function (grid) {
            ctrl.gridOrderItems = grid;
            ctrl.gridOrderOnFetch();
        };

        ctrl.gridOrderItemsSelectionOnInit = function (selectionCustom) {
            ctrl.selectionCustom = selectionCustom;
        };

        ctrl.gridOrderOnFetch = function () {

            if (ctrl.gridOrderItems == null)
                return;

            var params = ctrl.gridOrderItems.getRequestParams();

            if (params.sorting != null && params.sortingType != null) {
                ctrl.gridOrderItemsSorting = '&sorting=' + params.sorting + '&sortingType=' + params.sortingType;
            } else {
                ctrl.gridOrderItemsSorting = null;
            }
        }

        ctrl.addOrderItems = function (result) {

            if (result == null || result.ids == null || result.ids.length == 0)
                return;

            ctrl.saveDraft().then(function () {
                ctrl.gridOrderItems.isProcessing = true;
                $http.post("orders/addOrderItems", { orderId: ctrl.orderId, offerIds: result.ids })
                    .then(function (response) {
                        if (response.data.result === true) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Order.ProductSuccessfullyAdded'));
                        }
                    })
                    .then(ctrl.gridOrderItemUpdate)
                    .then(ctrl.onChangeHistory)
                    .catch(function () {
                        toaster.pop('error', $translate.instant('Admin.Js.Order.ErrorWhileAddingOrder'));
                    })
                    .finally(function () {
                        ctrl.gridOrderItems.isProcessing = false;
                    });
            });
        };

        ctrl.gridOrderItemUpdate = function () {
            ctrl.gridOrderItems.fetchData();
            ctrl.orderItemsSummaryUpdate();
        };

        ctrl.gridOrderItemDelete = function () {
            ctrl.orderItemsSummaryUpdate();
        };

        ctrl.initOrderItemsSummary = function (orderItemsSummary) {
            ctrl.orderItemsSummary = orderItemsSummary;
        };

        ctrl.orderItemsSummaryUpdate = function () {
            if (ctrl.orderItemsSummary != null) {
                ctrl.orderItemsSummary.getOrderItemsSummary();
            }
        };

        ctrl.changeStatus = function () {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalChangeOrderStatusCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/order/modal/changeOrderStatus/changeOrderStatus.html',
                resolve: {
                    params: {
                        orderId: ctrl.orderId,
                        statusId: ctrl.orderStatus,
                        statusName: $(".orderstatus option:selected").text()
                    }
                }
            }).result.then(function (result) {
                if (result == null)
                    ctrl.orderStatus = ctrl.orderStatusOld;
                else {
                    ctrl.statusComment = result.basis;
                }
                ctrl.orderStatusOld = ctrl.orderStatus;
                ctrl.onChangeStatusHistory();
                //ctrl.modalClose(); //тут при успешном закрытии
                return result;
            }, function (result) {
                if (result === "cancelChangeOrderStatus")
                    ctrl.orderStatus = ctrl.orderStatusOld;
                ctrl.onChangeStatusHistory();
                //ctrl.modalDismiss();  //тут при неудачном закрытии, отмене
                return result;
            });
        };

        ctrl.setPaied = function (checked) {

            $http.post("orders/setPaied", { orderId: ctrl.orderId, paid: checked }).then(function (response) {
                if (response.data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    ctrl.isPaied = checked;
                    ctrl.onChangeHistory();
                }
            });
        };

        ctrl.sendDateToServer = function (date) {
            if (date == null || date == '') {
                return;
            }

            $http.post("orders/setDate", { orderId: ctrl.orderId, date: date }).then(function (response) {
                if (response.data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                }
            });
        }

        ctrl.setDate = function (selectedDates, date, instance) {
            ctrl.sendDateToServer(date);
        };

        ctrl.setDateMobile = function (date) {
            ctrl.sendDateToServer(date);
        }

        ctrl.setManagerConfirmer = function (isManagerConfirmed) {
            $http.post("orders/setManagerConfirmed", { orderId: ctrl.orderId, isManagerConfirmed: isManagerConfirmed }).then(function (response) {
                if (response.data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                }
            });
        };

        ctrl.set1CExportOrder = function (useIn1C) {
            $http.post("orders/setUseIn1C", { orderId: ctrl.orderId, useIn1C: useIn1C }).then(function (response) {
                if (response.data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                }
            });
        }

        ctrl.selectCustomer = function (result) {
            ctrl.getCustomer(result)
                .then(function (result) {
                    return result || $q.reject('error');
                })
                .then(function (result) {
                    if (ctrl.isDraft) {
                        ctrl.saveDraft();
                    } else {
                        $timeout(function () { document.getElementById('orderForm').submit(); });
                    }
                });
        };

        ctrl.changeCustomer = function (orderCustomerForm, timeout) {

            if (timerChangeCustomer) {
                clearTimeout(timerChangeCustomer);
            }

            timerChangeCustomer = setTimeout(function () {
                ctrl.saveDraft().then(function () {
                    orderCustomerForm.$setPristine();
                });
            }, timeout != null ? timeout : 300);
        };

        ctrl.processCity = function (orderCustomerForm, zone) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }

            return timerProcessAddress = $timeout(function () {
                if (zone != null) {
                    ctrl.country = zone.Country;
                    ctrl.region = zone.Region;
                    ctrl.district = zone.District;
                    ctrl.zip = zone.Zip;
                    if (zone.Zip && ctrl.isDraft) { ctrl.changeCustomer(orderCustomerForm, 0); }
                }
                if (zone == null || !zone.Zip) {
                    ctrl.processCustomerContact(zone == null).then(function (data) {
                        if (data.result === true) {
                            ctrl.country = data.obj.Country;
                            ctrl.region = data.obj.Region;
                            ctrl.district = data.obj.District;
                            ctrl.zip = data.obj.Zip;
                        }
                        if (ctrl.isDraft) { ctrl.changeCustomer(orderCustomerForm, 0) };
                    });
                }
            }, zone != null ? 0 : 300);
        };

        ctrl.processAddress = function (orderCustomerForm, data) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }

            return timerProcessAddress = $timeout(function () {
                if (data != null && data.Zip) {
                    ctrl.zip = data.Zip;
                    if (ctrl.isDraft) { ctrl.changeCustomer(orderCustomerForm, 0) };
                } else {
                    ctrl.processCustomerContact().then(function (data) {
                        if (data.result === true) {
                            ctrl.zip = data.obj.Zip;
                        }
                        if (ctrl.isDraft) { ctrl.changeCustomer(orderCustomerForm, 0) };
                    });
                }
            }, data != null ? 0 : 300);
        };

        ctrl.processCustomerContact = function (byCity) {
            var contact = {
                country: ctrl.country,
                region: ctrl.region,
                district: ctrl.district,
                city: ctrl.city,
                zip: ctrl.zip,
                street: ctrl.street,
                house: ctrl.house,
                byCity: byCity
            };
            return $http.post('customers/processCustomerContact', contact).then(function (response) {
                return response.data;
            });
        };


        ctrl.saveChanges = function (form) {
            if (ctrl.isEditMode === true) {
                ctrl.saveOrderInfo(form);
            } else {
                ctrl.saveDraft(form);
            }
        };

        ctrl.updateAdminComment = function (form) {
            var params = {
                orderId: ctrl.orderId,
                adminOrderComment: ctrl.adminOrderComment,
            };

            return $http.post("orders/updateAdminComment", params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    if (form != null) {
                        form.$setPristine();
                    }
                }
                ctrl.onChangeHistory();
                return data;
            });
        };

        ctrl.getCustomer = function (result) {
            if (result == null || result.customerId == null) {
                return false;
            }

            return $http.get("customers/getCustomerWithContact", { params: { customerId: result.customerId } }).then(function (response) {

                var customer = response.data;

                if (customer == null) return false;

                ctrl.customerId = customer.Id;
                ctrl.firstName = ctrl.selectedFirstName = customer.FirstName;
                ctrl.lastName = ctrl.selectedLastName = customer.LastName;
                ctrl.patronymic = customer.Patronymic;
                ctrl.email = customer.Email;
                ctrl.phone = customer.Phone;
                ctrl.standardPhone = customer.StandardPhone;
                ctrl.organization = customer.Organization;

                ctrl.bonusCardNumber = customer.BonusCardNumber;
                ctrl.customerGroup = customer.CustomerGroup;
                var contacts = customer.Contacts;

                if (contacts != null && contacts.length > 0) {
                    var contact = contacts[0];

                    ctrl.country = contact.Country;
                    ctrl.region = contact.Region;
                    ctrl.district = contact.District;
                    ctrl.city = contact.City;
                    ctrl.zip = contact.Zip;
                    ctrl.street = contact.Street;
                    ctrl.entrance = contact.Entrance;
                    ctrl.floor = contact.Floor;
                    ctrl.house = contact.House;
                    ctrl.structure = contact.Structure;
                    ctrl.apartment = contact.Apartment;

                    ctrl.customField1 = contact.CustomField1;
                    ctrl.customField2 = contact.CustomField2;
                    ctrl.customField3 = contact.CustomField3;
                }
                return true;
            });
        };

        // save draft
        ctrl.saveDraft = function (form) {

            if (!ctrl.isDraft) {
                return $q.resolve();
            }

            var orderId = ctrl.orderId;

            var params = {

                orderId: ctrl.orderId,

                orderCustomer: {
                    customerId: ctrl.customerId,
                    firstName: ctrl.firstName,
                    lastName: ctrl.lastName,
                    patronymic: ctrl.patronymic,
                    email: ctrl.email,
                    phone: ctrl.phone,
                    standardPhone: ctrl.standardPhone,
                    country: ctrl.country,
                    region: ctrl.region,
                    district: ctrl.district,
                    city: ctrl.city,
                    zip: ctrl.zip,
                    customField1: ctrl.customField1,
                    customField2: ctrl.customField2,
                    customField3: ctrl.customField3,
                    street: ctrl.street,
                    house: ctrl.house,
                    apartment: ctrl.apartment,
                    structure: ctrl.structure,
                    entrance: ctrl.entrance,
                    floor: ctrl.floor,
                    organization: ctrl.organization
                },

                statusComment: ctrl.statusComment,
                adminOrderComment: ctrl.adminOrderComment,
                orderSourceId: ctrl.orderSourceId,
                managerId: ctrl.managerId,
                trackNumber: ctrl.trackNumber
            };

            return $http.post("orders/saveDraft", params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    if (orderId === 0 && data.orderId !== 0) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.CreatedDraftOrder') + data.orderId);
                        ctrl.orderId = data.orderId;
                        ctrl.customerId = data.customerId;

                        ctrl.gridOrderItems.setParams({ OrderId: data.orderId });
                    } else {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    }

                    if (form != null) {
                        form.$setPristine();
                    }

                    ctrl.isEditMode = true;
                    ctrl.isDraft = true;
                    ctrl.onChangeHistory();
                }

                return data;
            });
        };

        // save order information in edit mode
        ctrl.saveOrderInfo = function (form) {

            if (ctrl.isDraft) {
                return $q.resolve();
            }

            var params = {
                orderId: ctrl.orderId,

                managerId: ctrl.managerId,
                statusComment: ctrl.statusComment,
                adminOrderComment: ctrl.adminOrderComment,
                trackNumber: ctrl.trackNumber,
                orderSourceId: ctrl.orderSourceId
            };

            return $http.post("orders/saveOrderInfo", params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    if (form != null) {
                        form.$setPristine();
                    }
                }
                ctrl.onChangeHistory();
                return data;
            });
        };

        ctrl.updateOrderBonusCard = function () {
            $http.post('orders/updateOrderBonusCard', { orderId: ctrl.orderId }).then(function (response) {
                window.location.reload();
            });
        };

        ctrl.deleteOrder = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Order.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Order.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('orders/deleteOrder', { orderId: ctrl.orderId }).then(function (response) {
                        lastStatisticsService.getLastStatistics();
                        window.location.assign('orders');
                    });
                }
            });
        };

        ctrl.getMapAddress = function () {
            var address = ctrl.country != null ? ctrl.country : "";
            address += (address.length > 0 ? ", " : "") + (ctrl.region != null ? ctrl.region : "");
            address += (address.length > 0 ? ", " : "") + (ctrl.district != null ? ctrl.district : "");
            address += (address.length > 0 ? ", " : "") + (ctrl.city != null ? ctrl.city : "");
            if (ctrl.address != null && ctrl.address !== '') {
                address += (address.length > 0 ? ", " : "") + (ctrl.address != null ? ctrl.address : "");
            } else {
                address += (address.length > 0 ? ", " : "") + (ctrl.street != null ? ctrl.street : "");
                address += (address.length > 0 ? ", " : "") + (ctrl.house != null ? ctrl.house : "");
                address += (address.length > 0 ? ", " : "") + (ctrl.structure != null ? ctrl.structure : "");
            }

            return encodeURIComponent(address);
        };

        ctrl.findCustomers = function (val) {
            if (ctrl.isDraft && val != null && val.length > 1) {
                return $http.get("customers/getCustomersAutocomplete?q=" + val).then(function (response) {
                    return response.data;
                });
            }
        };

        ctrl.selectCustomerByAutocomplete = function ($item, $model, $label, $event) {
            var customerId = $item.value;
            return ctrl.getCustomer({ customerId: customerId });
        };

        ctrl.dateChange = function (date) {
            alert($translate.instant('Admin.Js.Order.ChangeTheOrderDate') + date.toString());
        };

        ctrl.checkStopEdit = function () {
            var result = true;

            if (ctrl.isPaied === true) {
                SweetAlert.alert($translate.instant('Admin.Js.Order.PaidOrderCantBeChanged'), { title: $translate.instant('Admin.Js.Order.ChangingOrder') });
                result = false;
            }

            return result;
        };

        ctrl.gridOnInplaceBeforeApply = function () {
            return ctrl.checkStopEdit();
        };

        ctrl.resetOrderCustomer = function () {
            ctrl.customerId = null;
            ctrl.firstName = ctrl.selectedFirstName = null;
            ctrl.lastName = ctrl.selectedLastName = null;
            ctrl.patronymic = null;
            ctrl.email = null;
            ctrl.phone = null;
            ctrl.standardPhone = null;
            ctrl.country = null;
            ctrl.region = null;
            ctrl.district = null;
            ctrl.city = null;
            ctrl.zip = null;
            ctrl.street = null;
            ctrl.entrance = null;
            ctrl.floor = null;
            ctrl.house = null;
            ctrl.structure = null;
            ctrl.apartment = null;
            ctrl.organization = null;
        };

        ctrl.updateStatus = function () {
            $http.post('customers/updateClientStatus', { id: ctrl.customerId, clientStatus: ctrl.clientStatus }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Customer.ChangesSaved'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.Customer.ErrorWhileSaving'));
                }
                ctrl.onChangeHistory();
            });
        };

        ctrl.changeStatusClient = function (currentStatus) {
            ctrl.clientStatus = ctrl.clientStatus === currentStatus ? 'none' : currentStatus;
            ctrl.updateStatus();
        };

        ctrl.leadEventsOnInit = function (leadEvents) {
            ctrl.leadEvents = leadEvents;
        };

        ctrl.updateLeadEvents = function () {
            ctrl.leadEvents.getLeadEvents();
        };

        ctrl.updateLeadEventsWithDelay = function () {
            setTimeout(ctrl.updateLeadEvents, 800);
        };

        ctrl.statusHistoryOnInit = function (orderStatusHistory) {
            ctrl.orderStatusHistory = orderStatusHistory;
        };

        ctrl.onChangeStatusHistory = function () {
            if (ctrl.orderStatusHistory != null) {
                ctrl.orderStatusHistory.update();
            }
        };

        ctrl.orderHistoryOnInit = function (orderHistory) {
            ctrl.orderHistory = orderHistory;
        };

        ctrl.onChangeHistory = function () {
            if (ctrl.orderHistory != null) {
                ctrl.orderHistory.update();
            }
        };
        /* templateDocx */

        ctrl.createDocx = function () {
            var params = {
                orderId: ctrl.orderId,
                templatesDocx: ctrl.selectedTemplatesDocx.slice()
            };

            var url = 'orders/generateTemplates';
            ctrl.selectedTemplatesDocx = [];

            $window.location.href = url + '?' + $httpParamSerializer(params);
        };

        /* end templateDocx */

        ctrl.editCustomerClose = function () {
            $timeout(function () { $window.location.reload(true); }, 100);
        };

        ctrl.taskGridOnInit = function (taskGrid) {
            ctrl.taskGrid = taskGrid;
        }
    };


    OrderCtrl.$inject = ['uiGridCustomConfig', '$http', '$httpParamSerializer', 'toaster', '$timeout', '$uibModal', '$q', 'SweetAlert', 'lastStatisticsService', '$translate', '$window'];


    ng.module('order', ['uiGridCustom', 'urlHelper', 'ui.bootstrap', 'orderItemsSummary', 'spinbox', 'shipping', 'payment', 'orderStatusHistory', 'orderHistory'])
      .controller('OrderCtrl', OrderCtrl);

})(window.angular);