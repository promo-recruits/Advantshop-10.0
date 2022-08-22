; (function (ng) {
    'use strict';
    var LeadCtrl = function (uiGridCustomConfig, $http, toaster, $timeout, $uibModal, $q, SweetAlert, $window, Upload, leadService, $translate) {

        var ctrl = this;

        ctrl.init = function (Id, form, readOnly) {
            ctrl.instance = {};
            ctrl.instance.Id = Id;
            ctrl.instance.lead = ctrl.instance.lead || {};
            ctrl.instance.lead.Id = Id;
            ctrl.instance.lead.customer = ctrl.instance.lead.customer || {};
            ctrl.getAttachments();
            ctrl.formLead = form;
            ctrl.readOnly = readOnly;
        }

        ctrl.gridLeadItemsOptions = ng.extend({}, uiGridCustomConfig, {
            rowHeight: 90,
            columnDefs: [
                {
                    name: 'ImageSrc',
                    displayName: '',
                    cellTemplate: '<div class="ui-grid-cell-contents"><img class="ui-grid-custom-col-img" ng-src="{{row.entity.ImageSrc}}"></div>',
                    width: 100,
                    enableSorting: false,
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Lead.Name'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents ui-grid-cell-contents-order-items">' +
                            '<div ng-if="row.entity.ProductLink != null"><a href="{{row.entity.ProductLink}}" target="_blank">{{row.entity.Name}}</a></div> ' +
                            '<div ng-if="row.entity.ProductLink == null">{{row.entity.Name}}</div> ' +

                            '<div class="order-item-artno">' + $translate.instant('Admin.Js.Lead.VendorCode') + '{{row.entity.ArtNo}}</div> ' +
                            '<div class="order-item-artno" ng-if="row.entity.BarCode != null && row.entity.BarCode.length > 0">' + $translate.instant('Admin.Js.Lead.BarCode') + '{{row.entity.BarCode}}</div> ' +
                            '<div ng-if="row.entity.Color != null && row.entity.Color.length > 0">{{row.entity.Color}}</div>' +
                            '<div ng-if="row.entity.Size != null && row.entity.Size.length > 0">{{row.entity.Size}}</div>' +
                            '<div ng-if="row.entity.CustomOptions != null && row.entity.CustomOptions.length > 0"> <div ng-bind-html="row.entity.CustomOptions"></div> </div>' +
                        '</div>',
                },
                {
                    name: 'Dimensions',
                    displayName: $translate.instant('Admin.Js.Lead.Dimensions'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents" ng-if="row.entity.Length != 0 && row.entity.Width != 0 && row.entity.Height != 0">' +
                        '{{row.entity.Length}} x {{row.entity.Width}} x {{row.entity.Height}} {{\'Admin.Js.Lead.Mm\'|translate}}' +
                    '</div>',
                    enableCellEdit: false,
                    width: 80,
                },
                {
                    name: 'Weight',
                    cellTemplate: '<div class="ui-grid-cell-contents" ng-if="row.entity.Weight != 0">{{row.entity.Weight}} {{\'Admin.Js.Lead.Kg\'|translate}} </div>',
                    displayName: $translate.instant('Admin.Js.Lead.Weight'),
                    enableCellEdit: false,
                    width: 60,
                },
                {
                    name: 'Price',
                    displayName: $translate.instant('Admin.Js.Lead.Price'),
                    enableCellEdit: true,
                    cellEditableCondition: function () {
                        return ctrl.readOnly === true ? false : true;
                    },
                    width: 100,
                },
                {
                    name: 'Amount',
                    displayName: $translate.instant('Admin.Js.Lead.Amount'),
                    enableCellEdit: true,
                    cellEditableCondition: function () {
                        return ctrl.readOnly === true ? false : true;
                    },
                    width: 60,
                },
                {
                    name: 'Cost',
                    displayName: $translate.instant('Admin.Js.Lead.Cost'),
                    width: 90,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 35,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-grid-custom-delete url="leads/deleteLeadItem" params="{\'LeadId\': row.entity.LeadId, \'leadItemId\': row.entity.LeadItemId }"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ],
        });


        ctrl.gridLeadItemsOnInit = function (grid) {
            ctrl.gridLeadItems = grid;
            ctrl.updateReadOnlyStatus();
        };


        ctrl.addLeadItems = function (result) {

            if (result == null || result.ids == null || result.ids.length == 0)
                return;

            leadService.addLeadItems(ctrl.instance.lead.Id, result.ids)
                .then(function (data) {
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Lead.ItemSuccessfullyAdded'));
                    }
                })
                .then(ctrl.gridLeadItemUpdate)
                .then(ctrl.updateLeadEventsWithDelay);
        }

        ctrl.gridLeadItemUpdate = function () {
            ctrl.gridLeadItems.fetchData();
            ctrl.leadItemsSummaryUpdate();
        }

        ctrl.gridLeadItemDelete = function () {
            ctrl.leadItemsSummaryUpdate();
        }

        ctrl.initLeadItemsSummary = function (leadItemsSummary) {
            ctrl.leadItemsSummary = leadItemsSummary;
            ctrl.leadItemsSummaryUpdate(true);
        }

        ctrl.leadItemsSummaryUpdate = function (firstTime) {
            if (ctrl.leadItemsSummary != null) {
                ctrl.leadItemsSummary.getLeadItemsSummary()
                    .then(function (data) {
                        if (data != null) {
                            ctrl.hasProducts = data.ProductsCost > 0;
                            if (ctrl.hasProducts || (data.ProductsCost === 0 && ctrl.ProductsCost !== 0 && ctrl.ProductsCost != null)) {
                                ctrl.instance.lead.sum = ctrl.sum = data.SumValueFormat;
                                ctrl.ProductsCost = data.ProductsCost;
                            }
                        }
                    });

                if (firstTime != true) {
                    ctrl.updateLeadEventsWithDelay();
                }
            }
        }

        ctrl.onCompleteLead = function (result, close) {
            if (result === true && close) {
                close();
            }
        }

        ctrl.createPaymentLink = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Lead.CreatePaymentLinkConfirm'), { title: '' }).then(function (result) {
                if (result === true) {
                    leadService.createPaymentLink(ctrl.instance.lead.Id).then(function (data) {
                        if (data.result === true && data.orderId != null) {
                            ctrl.orderId = data.orderId;

                            $uibModal.open({
                                bindToController: true,
                                controller: 'ModalGetBillingLinkCtrl',
                                controllerAs: 'ctrl',
                                templateUrl: '../areas/admin/content/src/order/modal/getBillingLink/getBillingLink.html',
                                resolve: { params: { orderId: ctrl.orderId } }
                            });
                        }
                    });
                }
            });
        }

        ctrl.deleteLead = function (onDelete) {
            SweetAlert.confirm($translate.instant('Admin.Js.Lead.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Lead.Deleting') })
                .then(function (result) {
                    if (result === true) {
                        leadService.deleteLead(ctrl.instance.lead.Id).then(function (data) {
                            if (data.result === true) {
                                if (onDelete) {
                                    onDelete();
                                } else {
                                    $window.location.assign('leads');
                                }
                            } else {
                                data.errors.forEach(function (error) {
                                    toaster.error(error);
                                });
                            }
                        });
                    }
                });
        }

        /* attachments */
        ctrl.getAttachments = function () {
            leadService.getAttachments(ctrl.instance.lead.Id)
                .then(function (data) {
                    ctrl.attachments = data;
                });
        }

        ctrl.uploadAttachment = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            ctrl.loadingFiles = true;

            if (($event.type === 'change' || $event.type === 'drop') && $files != null && $files.length > 0) {

                leadService.uploadAttachment(ctrl.instance.lead.Id, $files)
                .then(function (data) {
                    for (var i in data) {
                        if (data[i].Result === true) {
                            ctrl.attachments.push(data[i].Attachment);
                            toaster.pop('success', '', $translate.instant('Admin.Js.Lead.File') + data[i].Attachment.FileName + $translate.instant('Admin.Js.Lead.Added'));
                            ctrl.updateLeadEvents();
                        }
                        else {
                            toaster.pop('error', $translate.instant('Admin.Js.Lead.ErrorLoading'), (data[i].Attachment != null ? data[i].Attachment.FileName + ": " : "") + data[i].Error);
                        }
                    }
                    ctrl.loadingFiles = false;
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.Lead.ErrorLoading'), $translate.instant('Admin.Js.Lead.FileDoesNotMeet'));
                ctrl.loadingFiles = false;
            }
            else {
                ctrl.loadingFiles = false;
            }
        };

        ctrl.deleteAttachment = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Lead.AreYouSureDeleteFile'), { title: $translate.instant('Admin.Js.Lead.Deleting') }).then(function (result) {
                if (result === true) {
                    leadService.deleteAttachment(ctrl.instance.lead.Id, id).then(function () {
                        ctrl.getAttachments();
                        ctrl.updateLeadEvents();
                    });
                }
            });
        }
        /* end attachments */

        ctrl.leadEventsOnInit = function (leadEvents) {
            ctrl.leadEvents = leadEvents;
        }

        ctrl.updateLeadEvents = function () {
            ctrl.leadEvents.getLeadEvents();
        }

        ctrl.updateLeadEventsWithDelay = function () {
            setTimeout(ctrl.updateLeadEvents, 800);
        }

        ctrl.taskGridOnOnit = function (taskGrid) {
            ctrl.taskGrid = taskGrid;
        }

        ctrl.updateTasks = function () {
            ctrl.updateLeadEventsWithDelay();
            leadService.getLeadInfo(ctrl.instance.lead.Id).then(function (data) {
                if (data == null)
                    return;
                ctrl.instance.lead.dealStatusId = data.Lead.DealStatusId.toString();
                ctrl.statuses = data.Statuses;
            });
        };

        ctrl.saveLead = function (lead) {
            return leadService.saveLead(lead).then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Lead.ChangesSuccessfullySaved'));
                    ctrl.updateLeadEvents();
                    ctrl.formLead.$setPristine();
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Lead.ErrorWhileSaving'));
                }
            });
        };

        ctrl.updateTitle = function (value) {

            ctrl.instance.lead.title = value;

            ctrl.saveLead(ctrl.instance);
        };

        ctrl.onAfterAddLead = function () {

        };

        ctrl.saveCustomerField = function () {
            ctrl.saveLead(ctrl.instance);
        };

        ctrl.saveLeadField = function () {
            ctrl.saveLead(ctrl.instance);
        };

        ctrl.changeLeadStatus = function (prevStatusId) {

            ctrl.saveLead(ctrl.instance);

            if (prevStatusId !== ctrl.instance.lead.dealStatusId &&
                ctrl.instance.lead.dealStatusId === ctrl.finalStatusId && ctrl.instance.lead.dealStatusId === ctrl.CanceledStatusId && ctrl.FinalSuccessActions == 0) {

                SweetAlert.confirm($translate.instant('Admin.Js.Lead.CreateAnOrder'), { title: "" }).then(function (result) {
                    if (result === true) {
                        ctrl.createOrder();
                    }
                });
            }
            ctrl.updateReadOnlyStatus();
        }

        ctrl.changeSalesFunnel = function() {
            ctrl.instance.leadfieldsJs = [];
            ctrl.saveLead(ctrl.instance).then(function () {
                leadService.getLeadInfo(ctrl.instance.lead.Id).then(function (data) {
                    if (data == null)
                        return;

                    ctrl.instance.lead.dealStatusId = data.Lead.DealStatusId.toString();
                    ctrl.statuses = data.Statuses;
                    ctrl.finalStatusId = data.FinalStatusId;
                    ctrl.canceledStatusId = data.CanceledStatusId;
                    ctrl.updateReadOnlyStatus();
                    if (ctrl.leadFieldsReloadFn) {
                        ctrl.leadFieldsReloadFn();
                    }
                });
            });
        }

        ctrl.updateReadOnlyStatus = function () {
            if (ctrl.instance.lead.dealStatusId == ctrl.finalStatusId || ctrl.instance.lead.dealStatusId == ctrl.canceledStatusId ) {
                ctrl.readOnly = true;
                ctrl.gridLeadItems.hideColumn('_serviceColumn');
                ctrl.gridLeadItemUpdate();
            } else {
                ctrl.readOnly = false;
                ctrl.gridLeadItems.showColumn('_serviceColumn');
                ctrl.gridLeadItemUpdate();
            }
        }

        ctrl.addSocialUser = function (type, link) {

            if (ctrl.btnSocialAdding != null)
                return;

            var url = '';
            switch (type) {
                case 'vk':
                    url = 'vk/addVkUser';
                    break;
                case 'facebook':
                    url = 'facebook/addFacebookUser';
                    break;
                case 'instagram':
                    url = 'instagram/addInstagramUser';
                    break;
                case 'ok':
                    url = 'ok/addOkUser';
                    break;
            }

            ctrl.btnSocialAdding = type;

            $http.post(url, { customerId: ctrl.instance.lead.customer.customerId, link: link }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Lead.ChangesSaved'));
                    location.reload();
                } else {
                    if (data.errors != null) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', '', error);
                        });
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Lead.ErrorWhileSaving'));
                    }
                }
            }).finally(function () {
                ctrl.btnSocialAdding = null;
            });
        }

        ctrl.deleteSocialLink = function (type) {
            SweetAlert.confirm($translate.instant('Admin.Js.Customer.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Customer.Deleting') }).then(function (result) {
                if (result === true) {

                    var url = '';
                    switch (type) {
                        case 'vk':
                            url = 'vk/deleteVkLink';
                            break;
                        case 'facebook':
                            url = 'facebook/deleteLink';
                            break;
                        case 'instagram':
                            url = 'instagram/deleteLink';
                            break;
                        case 'ok':
                            url = 'ok/deleteOkLink';
                            break;
                    }

                    $http.post(url, { customerId: ctrl.instance.lead.customer.customerId }).then(function (response) {
                        var data = response.data;
                        if (data) {
                            $window.location.reload(true);
                        }
                    });
                }
            });
        }

        ctrl.filterEvents = function (filterBy) {
            ctrl.leadEvents.filterType = filterBy;
        }


        ctrl.saveAddress = function () {
            var params = {
                leadId: ctrl.instance.lead.Id,
                country: ctrl.instance.lead.Country,
                region: ctrl.instance.lead.Region,
                district: ctrl.instance.lead.District,
                city: ctrl.instance.lead.City,
                zip: ctrl.instance.lead.Zip
            }
            $http.post('leads/saveShippingCity', params).then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.Lead.ChangesSuccessfullySaved'));
                ctrl.leadItemsSummaryUpdate();
            });
        }
    }

    LeadCtrl.$inject = ['uiGridCustomConfig', '$http', 'toaster', '$timeout', '$uibModal', '$q', 'SweetAlert', '$window', 'Upload', 'leadService', '$translate', 'urlHelper'];

    ng.module('lead', ['uiGridCustom', 'leadItemsSummary', 'leadEvents'])
      .controller('LeadCtrl', LeadCtrl);

})(window.angular);