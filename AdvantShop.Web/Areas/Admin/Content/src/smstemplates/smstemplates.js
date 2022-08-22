; (function (ng) {
    'use strict';

    var SmsTemplatesCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, toaster, $http, $q, SweetAlert, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'SmsType',
                    displayName: $translate.instant('Admin.Js.Smstemplates.TypeOfTemplate'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><ui-modal-trigger data-controller="\'ModalAddEditSmsTemplateCtrl\'" controller-as="ctrl" size="md" backdrop="static"' +
                              'template-url="../areas/admin/content/src/_shared/modal/bonus/smstemplate/addeditsmstemplate.html"' +
                              'resolve="{params:{ SmsTypeId:row.entity.SmsTypeId}}"><div class="ui-grid-cell-contents ui-grid-custom-pointer"><a ng-href="">{{COL_FIELD}}</a></div></ui-modal-trigger></div>'
                },
                {
                    name: 'SmsBody',
                    displayName: $translate.instant('Admin.Js.Smstemplates.Messages'),
                    enableCellEdit: false,
                    cellTemplate: '<div class="ui-grid-cell-contents"><ui-modal-trigger data-controller="\'ModalAddEditSmsTemplateCtrl\'" controller-as="ctrl" size="md" backdrop="static"' +
                              'template-url="../areas/admin/content/src/_shared/modal/bonus/smstemplate/addeditsmstemplate.html"' +
                              'resolve="{params:{ SmsTypeId:row.entity.SmsTypeId}}"><div class="ui-grid-cell-contents ui-grid-custom-pointer">{{COL_FIELD}}' + '</div></ui-modal-trigger></div>'
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-modal-trigger on-close="grid.appScope.$ctrl.gridExtendCtrl.gridUpdate();" data-controller="\'ModalAddEditSmsTemplateCtrl\'" controller-as="ctrl" size="md" backdrop="static"' +
                              'template-url="../areas/admin/content/src/_shared/modal/bonus/smstemplate/addeditsmstemplate.html"' +
                              'resolve="{params:{ SmsTypeId:row.entity.SmsTypeId}}"><a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a></ui-modal-trigger>' +
                            '<ui-grid-custom-delete url="smstemplates/deletesmstemplate" params="{\'id\': row.entity.SmsTypeId}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            enableSorting: false,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Smstemplates.DeleteSelected'),
                        url: 'smstemplates/DeleteSmsTemplateMass',
                        field: 'SmsTypeId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Smstemplates.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Smstemplates.Deleting') }).then(function (result) {
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

        ctrl.gridUpdate = function () {
            ctrl.grid.fetchData()
        }

        var columnlogDefs = [
                {
                name: 'Phone',
                displayName: $translate.instant('Admin.Js.Smstemplates.Number'),
                    enableSorting: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Smstemplates.Number'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Phone'
                    }
                },
                {
                    name: 'Body',
                    displayName: $translate.instant('Admin.Js.Smstemplates.Message'),
                    enableCellEdit: false,
                    enableSorting: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Smstemplates.Message'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Body'
                    }
                },
                 {
                     name: 'State',
                     displayName: $translate.instant('Admin.Js.Smstemplates.Status'),
                     enableCellEdit: false
                 },
                  {
                      name: 'Created_Str',
                      displayName: $translate.instant('Admin.Js.Smstemplates.Created'),
                      enableCellEdit: false
                  },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    //cellTemplate:
                    //    '<div class="ui-grid-cell-contents"><div>' +
                    //        '<ui-modal-trigger data-controller="\'ModalAddEditSmsTemplateCtrl\'" controller-as="ctrl" size="md" backdrop="static"' +
                    //          'template-url="../areas/admin/content/src/_shared/modal/bonus/smstemplate/addeditsmstemplate.html"' +
                    //          'resolve="{params:{ SmsTypeId:row.entity.SmsTypeId}}"><a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a></ui-modal-trigger>' +
                    //        '<ui-grid-custom-delete url="smstemplates/deletesmstemplate" params="{\'id\': row.entity.SmsTypeId}"></ui-grid-custom-delete>' +
                    //    '</div></div>'
                }
        ];

        ctrl.gridlogOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnlogDefs,
            rowHeight: 100
        });

        ctrl.gridlogOnInit = function (gridlog) {
            ctrl.gridlog = gridlog;
        };

        ctrl.deleteSmsTemplate = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Smstemplates.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Smstemplates.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('smstemplates/deleteSmsTemplate', { id: id }).then(function (response) {
                        ctrl.gridUpdate();
                    });
                }
            });
        }

    };

    SmsTemplatesCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', 'toaster', '$http', '$q', 'SweetAlert', '$translate'];


    ng.module('smstemplates', ['uiGridCustom', 'urlHelper'])
      .controller('SmsTemplatesCtrl', SmsTemplatesCtrl);

})(window.angular);