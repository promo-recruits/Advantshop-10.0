; (function (ng) {
    'use strict';
    var CustomerTagsCtrl = function (uiGridConstants, uiGridCustomConfig, $translate, $http, SweetAlert, $window) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.CustomerTags.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="customertags/edit/{{row.entity.Id}}">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.CustomerTags.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name'
                    }
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.CustomerTags.Enabled'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.CustomerTags.Enabled'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.CustomerTags.Active'), value: true }, { label: $translate.instant('Admin.Js.CustomerTags.Inactive'), value: false }]
                    }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.CustomerTags.Sorting'),
                    enableCellEdit: true,
                    width: 100
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<a href="customertags/edit/{{row.entity.Id}}" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a> ' +
                        '<ui-grid-custom-delete url="customertags/deleteTag" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowUrl: 'customertags/edit/{{row.entity.Id}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.CustomerTags.DeleteSelected'),
                        url: 'customertags/deleteTags',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.CustomerTags.AreYouSureDelete'), { title: $translate.instant('Admin.Js.CustomerTags.Deleting') }).then(function (result) {
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

        ctrl.deleteTag = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.CustomerTags.AreYouSureDelete'), { title: $translate.instant('Admin.Js.CustomerTags.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('customerTags/deleteTag', { id: id }).then(function (response) {
                        $window.location.assign('settingscustomers#?tab=customerTags');
                    });
                }
            });
        };
    }

    CustomerTagsCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', '$translate', '$http', 'SweetAlert', '$window'];
    
    ng.module('customerTags', ['uiGridCustom', 'urlHelper'])
        .controller('CustomerTagsCtrl', CustomerTagsCtrl);

})(window.angular);