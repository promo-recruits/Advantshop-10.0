; (function (ng) {
    'use strict';

    var SettingsSmsCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $uibModal, $translate, toaster) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'OrderStatusName',
                    displayName: 'Статус заказа',
                    width: 230,
                    filter: {
                        placeholder: 'Статус заказа',
                        type: uiGridConstants.filter.SELECT,
                        name: 'OrderStatusId',
                        fetch: 'orders/getOrderStatuses'
                    }
                },
                {
                    name: 'SmsText',
                    displayName: 'Текст sms',
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.News.Title'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'SmsText',
                    }
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.News.SheActive'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                    width: 90,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.News.Activity'),
                        name: 'Enabled',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.News.TheyActive'), value: true }, { label: $translate.instant('Admin.Js.News.Inactive'), value: false }]
                    }
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditSmsTemplateOnOrderChangingCtrl\'" controller-as="ctrl" size="lg" ' +
                        'template-url="../areas/admin/content/src/settingsSms/modal/addEditSmsTemplateOnOrderChanging/addEditSmsTemplateOnOrderChanging.html" ' +
                        'data-resolve="{value:{\'id\': row.entity.Id }}"' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a> ' +
                        '</ui-modal-trigger>' +

                        '<ui-grid-custom-delete url="settingsSms/deleteTemplate" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];


        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.News.DeleteSelected'),
                        url: 'settingsSms/deleteTemplates',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.News.AreYouSureDelete'), { title: $translate.instant('Admin.Js.News.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.sendTestSms = function(phone, text) {
            $http.post('settingsSms/sendTestSms', { phone: phone, text: text }).then(function(response) {
                var data = response.data;
                if (data.result) {
                    toaster.pop('success', '', 'Собщение отправлено');
                } else {
                    data.errors.forEach(function (e) {
                        toaster.pop('error', '', e);
                    });
                }
            });
        }
    };

    SettingsSmsCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', '$uibModal', '$translate', 'toaster'];


    ng.module('settingsSms', ['uiGridCustom', 'urlHelper'])
      .controller('SettingsSmsCtrl', SettingsSmsCtrl);

})(window.angular);