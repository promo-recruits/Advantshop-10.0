; (function (ng) {
    'use strict';

    var TagsCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, toaster, $http, $q, SweetAlert, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Tags.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="tags/edit/{{row.entity.Id}}">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tags.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name'
                    }
                },
                {
                    name: 'UrlPath',
                    displayName: $translate.instant('Admin.Js.Tags.SynonymForUrl'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tags.SynonymForUrl'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Url'
                    }
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.Tags.Activ'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tags.Activity'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.Tags.Active'), value: true }, { label: $translate.instant('Admin.Js.Tags.Inactive'), value: false }]
                    }
                },
                {
                    name: 'VisibilityForUsers',
                    displayName: $translate.instant('Admin.Js.Tags.Visibility'),
                      enableCellEdit: false,
                      cellTemplate: '<ui-grid-custom-switch row="row" field-name="VisibilityForUsers"></ui-grid-custom-switch>',
                      width: 100,
                      filter: {
                          placeholder: $translate.instant('Admin.Js.Tags.Visibility'),
                          type: uiGridConstants.filter.SELECT,
                          selectOptions: [{ label: $translate.instant('Admin.Js.Tags.Yes'), value: true }, { label: $translate.instant('Admin.Js.Tags.No'), value: false }]
                      }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Tags.Sorting'),
                    enableCellEdit: true,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<a href="tags/edit/{{row.entity.Id}}" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a> ' +
                            '<ui-grid-custom-delete url="tags/deleteTag" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowUrl: 'tags/edit/{{row.entity.Id}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Tags.DeleteSelected'),
                        url: 'tags/deleteTags',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Tags.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Tags.Deleting') }).then(function (result) {
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

        ctrl.setActive = function (active, id) {
            if (id <= 0) return;

            $http.post('tags/setTagActive', { id: id, active: active }).then(function (response) {
                if (response.data === true) {
                    ctrl.Enabled = active;
                    toaster.pop('success', $translate.instant('Admin.Js.Tags.ChangesSaved'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.Tags.ErrorChangingActivity'), "");
                }
            });
        };

        ctrl.deleteTag = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Tags.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Tags.Deleting') }).then(function (result) {
                if (result === true) {
                    $http.post('tags/deleteTag', { id: id }).then(function (response) {
                        $window.location.assign('tags');
                    });
                }
            });
        }
    };

    TagsCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', 'toaster', '$http', '$q', 'SweetAlert', '$translate'];


    ng.module('tags', ['uiGridCustom', 'urlHelper'])
      .controller('TagsCtrl', TagsCtrl);

})(window.angular);