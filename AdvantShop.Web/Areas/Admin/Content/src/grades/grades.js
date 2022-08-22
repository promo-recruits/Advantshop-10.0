; (function (ng) {
    'use strict';

    var GradesCtrl = function ($location, $window, $uibModal, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, toaster, $http, $q, SweetAlert, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Grades.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Grades.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name'
                    }
                },
                {
                    name: 'BonusPercent',
                    displayName: $translate.instant('Admin.Js.Grades.Percent'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Grades.Percent'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BonusPercent'
                    }
                },
                {
                    name: 'PurchaseBarrier',
                    displayName: $translate.instant('Admin.Js.Grades.Transition'),
                    headerCellClass: 'grid-text-required',
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Grades.Transition'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'PurchaseBarrier'
                    }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Grades.Sorting'),
                    enableCellEdit: true,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                            '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadGrade(row.entity.Id)"></a> ' +
                            '<ui-grid-custom-delete url="grades/delete" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadGrade(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Grades.DeleteSelected'),
                        url: 'grades/deleteGrade',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Grades.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Grades.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]
            }
        });

        ctrl.loadGrade = function (id) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditGradeCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/grades/modal/addEditGrade/AddEditGrade.html',
                resolve: {
                    id: function () {
                        return id;
                    }
                }
            }).result.then(function (result) {
                ctrl.grid.fetchData();
                return result;
            }, function (result) {
                return result;
            });
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
        
    };

    GradesCtrl.$inject = ['$location', '$window', '$uibModal', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', 'toaster', '$http', '$q', 'SweetAlert', '$translate'];


    ng.module('grades', ['uiGridCustom', 'urlHelper'])
      .controller('GradesCtrl', GradesCtrl);

})(window.angular);