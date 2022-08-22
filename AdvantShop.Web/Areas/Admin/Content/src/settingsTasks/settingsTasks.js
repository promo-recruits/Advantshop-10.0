; (function (ng) {
    'use strict';

    var SettingsTasksCtrl = function ($http, $q, $uibModal, uiGridCustomConfig, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'EventName',
                    displayName: $translate.instant('Admin.Js.SettingsTasks.Event'),
                    enableSorting: false
                },
                {
                    name: 'ManagerFilterHTML',
                    displayName: $translate.instant('Admin.Js.SettingsTasks.Employee'),
                    width: 250,
                    enableSorting: false,
                    cellTemplate: '<div class="ui-grid-cell-contents">' +
                        '<div ng-if="row.entity.ManagerFilter.Comparers.length > 0" ng-repeat="comparer in row.entity.ManagerFilter.Comparers | limitTo : row.entity.ManagerFilter.Comparers.length === 3 ? 3 : 2 ">' +
                    '<b ng-if="row.entity.ManagerFilter.Comparers.length > 1" ng-bind="($index + 1) + \'.\'"></b> <span ng-bind-html="grid.appScope.$ctrl.gridExtendCtrl.getManagerFilterHtml(comparer, row.entity.EventTypeString)"></span>' +
                    '</div><div ng-if="row.entity.ManagerFilter.Comparers.length > 3"><span class="span-link">' + $translate.instant('Admin.Js.SettingsTasks.More') + '</span></div></div>'
                },
                {
                    name: 'TaskDueDateIntervalFormatted',
                    displayName: $translate.instant('Admin.Js.SettingsTasks.TimeForExecution'),
                    enableSorting: false
                },
                {
                    name: 'TaskCreateIntervalFormatted',
                    displayName: $translate.instant('Admin.Js.SettingsTasks.TermOfSettingTask'),
                    enableSorting: false
                },
                {
                    name: 'Priority',
                    displayName: $translate.instant('Admin.Js.SettingsTasks.RulePriority'),
                    enableSorting: false
                },
                //{
                //    name: '_taskServiceColumn',
                //    displayName: 'Текст задачи',
                //    enableSorting: false,
                //    cellTemplate: '<div class="ui-grid-cell-contents"><a href="">Настроить</a></div>'
                //},
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                        '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadRule(row.entity.Id, row.entity.EventTypeString)"></a> ' +
                        '<ui-grid-custom-delete url="bizprocessrules/delete" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadRule(row.entity.Id, row.entity.EventTypeString);
                }
            }
        });

        ctrl.gridOrderCreatedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridOrderStatusChangedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridLeadCreatedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridLeadStatusChangedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridCallMissedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridReviewAddedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridMessageReplyRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridTaskCreatedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridTaskStatusChangedRulesOptions = angular.copy(ctrl.gridOptions);

        ctrl.loadRule = function (id, eventType) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditRuleCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/settingsTasks/modal/addEditRule/addEditRule.html',
                resolve: {
                    id: function () {
                        return id;
                    },
                    event: {
                        type: eventType
                    }
                },
                size: 'lg',
                backdrop: 'static'
            }).result.then(function (result) {
                ctrl.modalClose(result);
                return result;
            }, function (result) {
                return result;
            });
        }

        ctrl.modalClose = function (result) {
            switch (result.eventType) {
                case 'OrderCreated':
                    ctrl.gridOrderCreatedRules.fetchData();
                    break;
                case 'OrderStatusChanged':
                    ctrl.gridOrderStatusChangedRules.fetchData();
                    break;
                case 'LeadCreated':
                    ctrl.gridLeadCreatedRules.fetchData();
                    break;
                case 'LeadStatusChanged':
                    ctrl.gridLeadStatusChangedRules.fetchData();
                    break;
                case 'CallMissed':
                    ctrl.gridCallMissedRules.fetchData();
                    break;
                case 'ReviewAdded':
                    ctrl.gridReviewAddedRules.fetchData();
                    break;
                case 'MessageReply':
                    ctrl.gridMessageReplyRules.fetchData();
                    break;
                case 'TaskCreated':
                    ctrl.gridTaskCreatedRules.fetchData();
                    break;
                case 'TaskStatusChanged':
                    ctrl.gridTaskStatusChangedRules.fetchData();
                    break;
            }
        }

        ctrl.getManagerFilterHtml = function (comparer, eventType) {
            var result;
            if (comparer.FilterType == 2)
                result = '<b>' + comparer.CustomerName + '</b>';
            else if (comparer.FilterType == 3) {
                switch (eventType) {
                    case 'OrderCreated':
                    case 'OrderStatusChanged':
                        result = '<b>' + $translate.instant('Admin.Js.SettingsTasks.OrderManager') + '</b>';
                        break;
                    case 'LeadCreated':
                    case 'LeadStatusChanged':
                        result = '<b>' + $translate.instant('Admin.Js.SettingsTasks.LeadManager') + '</b>';
                        break;
                    case 'MessageReply':
                        result = '<b>' + $translate.instant('Admin.Js.SettingsTasks.BuyersManager') + '</b>';
                        break;
                    case 'TaskCreated':
                    case 'TaskStatusChanged':
                        result = '<b>' + $translate.instant('Admin.Js.SettingsTasks.TaskManager') + '</b>';
                        break;
                }
            } else {
                result = '<b>' + comparer.FilterTypeName + '</b>';
                var rules = [];
                if (comparer.ManagerRoleId != null)
                    rules.push($translate.instant('Admin.Js.SettingsTasks.Role') + comparer.ManagerRoleName);
                if (comparer.City)
                    rules.push($translate.instant('Admin.Js.SettingsTasks.City') + comparer.City);
                if (rules.length > 0) {
                    result += ' <span>(' + rules.join(', ') + ')</span>'
                }
            }
            return result;
        };

        ctrl.onChangeStateOffOn = function (checked) {
            ctrl.TasksActive = checked;
        };

        ctrl.onChangeReminderStateOffOn = function (cheched) {
            ctrl.ReminderActive = cheched;
        };
    };
    SettingsTasksCtrl.$inject = ['$http', '$q', '$uibModal', 'uiGridCustomConfig', '$translate'];

    ng.module('settingsTasks', [])
      .controller('SettingsTasksCtrl', SettingsTasksCtrl);

})(window.angular);