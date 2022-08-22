; (function (ng) {
    'use strict';

    var StaticBlockCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $uibModal, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'Key',
                    displayName: $translate.instant('Admin.Js.StaticBlock.AccessKey'),
                    enableSorting: true,
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.StaticBlock.AccessKey'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Key'
                    },
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-modal-trigger data-controller="\'ModalAddEditStaticBlockCtrl\'" controller-as="ctrl" size="middle" ' +
                                    'template-url="../areas/admin/content/src/staticBlock/modal/addEditStaticBlock/addEditStaticBlock.html" ' +
                                    'data-resolve="{\'id\': row.entity.StaticBlockId}" ' +
                                    'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                                    '<a href="">{{COL_FIELD}}</a>' +
                            '</ui-modal-trigger>' +
                        '</div></div>'
                },
                {
                    name: 'InnerName',
                    displayName: $translate.instant('Admin.Js.StaticBlock.Name'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.StaticBlock.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'InnerName'
                    }
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.StaticBlock.Activ'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.StaticBlock.Activity'),
                        name: 'Enabled',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.StaticBlock.Active'), value: true }, { label: $translate.instant('Admin.Js.StaticBlock.Inactive'), value: false }]
                    }
                },
                {
                    name: 'AddedFormatted',
                    displayName: $translate.instant('Admin.Js.StaticBlock.DateOfAdding'),
                    width: 150,
                    enableCellEdit: false,
                },

                {
                    name: 'ModifiedFormatted',
                    displayName: $translate.instant('Admin.Js.StaticBlock.ModificatonDate'),
                    width: 165,
                    enableCellEdit: false,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-modal-trigger data-controller="\'ModalAddEditStaticBlockCtrl\'" controller-as="ctrl" size="middle" ' +
                                    'template-url="../areas/admin/content/src/staticBlock/modal/addEditStaticBlock/addEditStaticBlock.html" ' +
                                    'data-resolve="{\'id\': row.entity.StaticBlockId}" ' +
                                    'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                                    '<a href="" class="ui-grid-custom-service-icon fas fa-pencil-alt news-category-pointer">{{COL_FIELD}}</a>' +
                            '</ui-modal-trigger>' +
                            //'<ui-grid-custom-delete url="StaticBlock/deleteItem" params="{\'id\': row.entity.StaticBlockId}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];


        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    /*{
                        text: $translate.instant('Admin.Js.StaticBlock.DeleteSelected'),
                        url: 'staticBlock/delete',
                        field: 'StaticBlockId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.StaticBlock.AreYouSureDelete'), { title: $translate.instant('Admin.Js.StaticBlock.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },*/
                    {
                        text: $translate.instant('Admin.Js.StaticBlock.MakeActive'),
                        url: 'staticBlock/active',
                        field: 'StaticBlockId'
                    },
                    {
                        text: $translate.instant('Admin.Js.StaticBlock.MakeInactive'),
                        url: 'staticBlock/deactive',
                        field: 'StaticBlockId'
                    }
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
    };

    StaticBlockCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', '$uibModal', '$translate'];


    ng.module('staticBlock', ['uiGridCustom', 'urlHelper'])
      .controller('StaticBlockCtrl', StaticBlockCtrl);

})(window.angular);