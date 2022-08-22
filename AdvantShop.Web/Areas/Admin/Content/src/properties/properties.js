; (function (ng) {
    'use strict';

    var PropertiesCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, toaster, $q, SweetAlert, $translate, urlHelper, $http) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Properties.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents">'+
                                        '<ui-modal-trigger data-controller="\'ModalAddEditPropertyCtrl\'" controller-as="ctrl" ' +
                                        'template-url="../areas/admin/content/src/properties/modal/addEditProperty/addEditProperty.html" ' +
                                        'data-resolve="{\'propertyId\': row.entity.PropertyId}" ' +
                                        'data-on-close="grid.appScope.$ctrl.fetchData()" size="lg"> ' +
                                        '<a ng-href="">{{COL_FIELD}}</a>' +
                                   '</ui-modal-trigger></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Properties.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name'
                    }
                },
                {
                    name: 'GroupName',
                    displayName: $translate.instant('Admin.Js.Properties.Group'),
                },
                {
                    name: 'UseInFilter',
                    displayName: $translate.instant('Admin.Js.Properties.ShowInFilter'),
                    width: 80,
                    enableCellEdit: true,
                    type: 'checkbox',
 cellTemplate: '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Properties.ShowingInFilter'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'UseInFilter',
                        selectOptions: [{ label: $translate.instant('Admin.Js.Properties.Yes'), value: true }, { label: $translate.instant('Admin.Js.Properties.No'), value: false }]
                    }
                },
                {
                    name: 'UseInDetails',
                    displayName: $translate.instant('Admin.Js.Properties.ShowInProductCard'),
                    width: 80,
                    enableCellEdit: true,
                    type: 'checkbox',
 cellTemplate: '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Properties.ShowingInProductCard'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'UseInDetails',
                        selectOptions: [{ label: $translate.instant('Admin.Js.Properties.Yes'), value: true }, { label: $translate.instant('Admin.Js.Properties.No'), value: false }]
                    }
                },
                {
                    name: 'UseInBrief',
                    displayName: $translate.instant('Admin.Js.Properties.ShowInTheBrief'),
                    width: 80,
                    enableCellEdit: true,
                    type: 'checkbox',
 cellTemplate: '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Properties.ShowingInTheBrief'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'UseInBrief',
                        selectOptions: [{ label: $translate.instant('Admin.Js.Properties.Yes'), value: true }, { label: $translate.instant('Admin.Js.Properties.No'), value: false }]
                    }
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Properties.SortOrder'),
                    width: 80,
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Properties.Sorting'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'SortingFrom'
                            },
                            to: {
                                name: 'SortingTo'
                            }
                        }
                    },
                },
                {
                    name: 'ProductsCount',
                    displayName: $translate.instant('Admin.Js.Properties.UsedForProducts'),
                    width: 80,
                    cellTemplate: '<div class="ui-grid-cell-contents"> <a ng-href="catalog?showMethod=AllProducts#?grid=%7B%22PropertyId%22:{{row.entity.PropertyId}},%22PropertyValueId%22:null%7D">{{ COL_FIELD }}</a></div>'
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 100,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +

                            '<ui-modal-trigger data-controller="\'ModalAddEditPropertyCtrl\'" controller-as="ctrl" ' +
                                        'template-url="../areas/admin/content/src/properties/modal/addEditProperty/addEditProperty.html" ' +
                                        'data-resolve="{\'propertyId\': row.entity.PropertyId}" ' +
                                        'data-on-close="grid.appScope.$ctrl.fetchData()" size="lg"> ' +
                                '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a> ' +
                            '</ui-modal-trigger>' +
                            '<a ng-href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.showPropertyValues(row.entity.PropertyId)" class="link-invert ui-grid-custom-service-icon fa fa-list"></a>' +
                            '<ui-grid-custom-delete url="properties/deleteProperty" params="{\'propertyId\': row.entity.PropertyId}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];


        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                //rowUrl: 'propertyValues?propertyId={{row.entity.PropertyId}}',
                rowClick: function($event, row, ctrl) {
                    ctrl.gridExtendCtrl.showPropertyValues(row.entity.PropertyId);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Properties.DeleteSelected'),
                        url: 'properties/deleteproperties',
                        field: 'PropertyId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Properties.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Properties.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },
                    {
                        template: '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalChangePropertyGroupCtrl\'" controller-as="ctrl" ' +
                        'data-resolve=\"{params:$ctrl.getSelectedParams(\'PropertyId\')}\" template-url="../areas/admin/content/src/properties/modal/changePropertyGroup/changePropertyGroup.html"> ' +
                        $translate.instant('Admin.Js.Properties.ChangeTheGroup') + '</ui-modal-trigger>'
                    },
                    {
                        text: $translate.instant('Admin.Js.Properties.OutputToFilter'),
                        url: 'properties/useInFilter',
                        field: 'PropertyId'
                    },
                    {
                        text: $translate.instant('Admin.Js.Properties.DoNotOutputToFilter'),
                        url: 'properties/notUseInFilter',
                        field: 'PropertyId'
                    },

                    {
                        text: $translate.instant('Admin.Js.Properties.UseInProductCard'),
                        url: 'properties/useInDetails',
                        field: 'PropertyId'
                    },
                    {
                        text: $translate.instant('Admin.Js.Properties.DoNotUseInProductCard'),
                        url: 'properties/notUseInDetails',
                        field: 'PropertyId'
                    },

                    {
                        text: $translate.instant('Admin.Js.Properties.UseInBrief'),
                        url: 'properties/useInBrief',
                        field: 'PropertyId'
                    },
                    {
                        text: $translate.instant('Admin.Js.Properties.DoNotUseInBrief'),
                        url: 'properties/notUseInBrief',
                        field: 'PropertyId'
                    },
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.$onInit = function () {

            ctrl.selectedGroupId = urlHelper.getUrlParam('groupId');
            ctrl.getGroup();

            var searchNgParams = $location.search();
            if (searchNgParams != null && searchNgParams.propertyId != null) {
                ctrl.showPropertyValues(searchNgParams.propertyId);
            }
        };

        ctrl.getGroup = function() {
            if (ctrl.selectedGroupId == 0) {
                ctrl.selectedGroupId = null;
            }

            if (ctrl.selectedGroupId == null) {
                ctrl.selectedGroupName = $translate.instant('Admin.Js.Properties.GroupName'); //'Все свойства'

            } else if (ctrl.selectedGroupId == -1) {
                ctrl.selectedGroupName = 'Свойства без группы';

            } else {
                $http.get('properties/getGroup?groupId=' + ctrl.selectedGroupId).then(function (response) {
                    var data = response.data;
                    if (data != null) {
                        ctrl.selectedGroup = data;
                        ctrl.selectedGroupName = data.Name;
                    }
                });
            }
        }

        ctrl.initPropertyGroups = function (propertyGroups) {
            ctrl.propertyGroups = propertyGroups;
        };

        ctrl.updatePropertyGroups = function (result) {
            toaster.pop('success', '', $translate.instant('Admin.Js.Properties.PropertyGroupCreated'));
            ctrl.propertyGroups.fetch();
        }

        ctrl.onChangeGroup = function (groupId) {
            ctrl.selectedGroupId = groupId;
            ctrl.showMode = null;

            ctrl.getGroup();
            ctrl.grid.setParams({ groupId: ctrl.selectedGroupId });
            ctrl.grid.fetchData();
        };

        ctrl.showPropertyValues = function (propertyId) {
            ctrl.showMode = 'propertyValues';
            ctrl.selectedPropertyId = propertyId;
            $location.search('propertyId', propertyId);
            //ctrl.propertyvalues.showPropertyValues(propertyId);
        };

        ctrl.back = function () {
            ctrl.showMode = null;
            $location.search('propertyId', null);
        };

    };

    PropertiesCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', 'toaster', '$q', 'SweetAlert', '$translate', 'urlHelper', '$http'];


    ng.module('properties', ['uiGridCustom', 'urlHelper', 'propertyGroups'])
      .controller('PropertiesCtrl', PropertiesCtrl);

})(window.angular);