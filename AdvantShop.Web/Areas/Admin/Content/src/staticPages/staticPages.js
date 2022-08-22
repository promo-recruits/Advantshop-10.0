; (function (ng) {
    'use strict';

    var StaticPagesCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $uibModal, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'PageName',
                    displayName: $translate.instant('Admin.Js.StaticPages.PageTitle'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a href="staticpages/edit/{{row.entity.StaticPageId}}">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.StaticPages.Title'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'PageName',
                    }
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.StaticPages.Activ'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.StaticPages.Activity'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.StaticPages.Active'), value: true }, { label: $translate.instant('Admin.Js.StaticPages.Inactive'), value: false }]
                    }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.StaticPages.Sorting'),
                    width: 150,
                    enableCellEdit: true,
                },
                {
                    name: 'ModifyDateFormatted',
                    displayName: $translate.instant('Admin.Js.StaticPages.Changed'),
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.StaticPages.DateOfChange'),
                        type: 'datetime',
                        term: {
                            from: new Date((new Date()).setMonth((new Date()).getMonth() - 1)),
                            to: new Date()
                        },
                        datetimeOptions: {
                            from: {
                                name: 'ModifyDateFrom'
                            },
                            to: {
                                name: 'ModifyDateTo'
                            }
                        }
                    }
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<a ng-href="staticpages/edit/{{row.entity.StaticPageId}}" class="ui-grid-custom-service-icon fas fa-pencil-alt"></a>' +
                            '<ui-grid-custom-delete url="staticpages/deleteStaticPage" params="{\'StaticPageId\': row.entity.StaticPageId}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        var SETTINGSTAB_SEARCH_NAME = 'staticPageTab';

        ctrl.$onInit = function () {
            var search = $location.search();
            ctrl.settingsTab = (search != null && search[SETTINGSTAB_SEARCH_NAME]) || 'pages';
        };

        ctrl.changeSettingsTab = function (tab) {
            ctrl.settingsTab = tab;
            $location.search(SETTINGSTAB_SEARCH_NAME, tab);
        };


        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.StaticPage.DeleteSelected'),
                        url: 'staticpages/deleteStaticPages',
                        field: 'StaticPageId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.StaticPages.AreYouSureDelete'), { title: $translate.instant('Admin.Js.StaticPages.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
    };

    StaticPagesCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', '$uibModal', '$translate'];


    ng.module('staticPages', ['uiGridCustom', 'urlHelper'])
      .controller('StaticPagesCtrl', StaticPagesCtrl);

})(window.angular);