; (function (ng) {
    'use strict';

    var TasksCtrl = function (
        $cookies,
        $location,
        $q,
        $rootScope,
        $uibModal,
        $window,
        adminWebNotificationsEvents,
        adminWebNotificationsService,
        lastStatisticsService,
        SweetAlert,
        tasksService,
        toaster,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        urlHelper,
        $translate,
        $uibModalStack) {

        var ctrl = this;

        ctrl.init = function (useKanban, selectTasks, prefilter, taskGroupId, isAdmin) {
            ctrl.useKanban = useKanban;
            // for grid
            ctrl.prefilter = prefilter || '';
            // for kanban
            ctrl.selectTasks = selectTasks || 'all';

            ctrl.isAdmin = isAdmin;
            ctrl.taskGroupId = taskGroupId;
            ctrl.gridParams = {};
            if (!ctrl.useKanban) {
                ctrl.gridParams.filterby = prefilter;
            } else {
                ctrl.selectTasks = //ctrl.gridParams.selectTasks =
                    ctrl.selectTasks;
            }
            if (taskGroupId)
                ctrl.gridParams.TaskGroupId = taskGroupId;

            var selectionOptions = [
                {
                    template:
                        '<ui-modal-trigger data-controller="\'ModalChangeTaskStatusesCtrl\'" controller-as="ctrl" data-resolve=\"{params:$ctrl.getSelectedParams(\'Id\'), canAccept:' + (ctrl.prefilter == 'completed') + '}\" ' +
                        'template-url="../areas/admin/content/src/tasks/modal/changeTaskStatuses/changeTaskStatuses.html" ' +
                        'data-on-close="$ctrl.gridOnAction()">' + $translate.instant('Admin.Js.Tasks.Tasks.ChangeStatusForSelected') + '</ui-modal-trigger>'
                },
                {
                    text: $translate.instant('Admin.Js.Tasks.Tasks.MarkAsViewed'),
                    url: 'tasks/markviewed',
                    field: 'Id'
                },
                {
                    text: $translate.instant('Admin.Js.Tasks.Tasks.MarkAsNotViewed'),
                    url: 'tasks/marknotviewed',
                    field: 'Id'
                },
                {
                    template:
                        '<ui-modal-trigger data-controller="\'ModalChangeTaskGroupCtrl\'" controller-as="ctrl" data-resolve=\"{params:$ctrl.getSelectedParams(\'Id\')}\" ' +
                        'template-url="../areas/admin/content/src/tasks/modal/changeTaskGroup/changeTaskGroup.html" ' +
                        'data-on-close="$ctrl.gridOnAction()">' + $translate.instant('Admin.Js.Tasks.Tasks.ChangeTaskGroup') + '</ui-modal-trigger>'
                }
            ];
            if (ctrl.isAdmin) {
                selectionOptions.splice(0, 0, {
                    text: $translate.instant('Admin.Js.Tasks.Tasks.DeleteSelected'),
                    url: 'tasks/deletetasks',
                    field: 'Id',
                    before: function () {
                        return SweetAlert.confirm($translate.instant('Admin.Js.Tasks.Tasks.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Tasks.Tasks.Deleting') }).then(function (result) {
                            return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                        });
                    }
                });
            }

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                enableExpandAll: false,
                enableGridMenu: true,
                columnDefs: ctrl.getColumnDefs(useKanban, selectTasks, prefilter, taskGroupId),
                uiGridCustom: {
                    rowClick: function ($event, row) {
                        ctrl.loadTask(row.entity.Id);
                    },
                    groupByField: ctrl.taskGroupId == null ? 'TaskGroupName' : null,
                    selectionOptions: selectionOptions,
                    rowClasses: function (row) {
                        var classes = '';
                        if (!row.entity.Viewed || row.entity.NewCommentsCount > 0)
                            classes += 'ui-grid-custom-row-bold ';
                        //if (row.entity.Overdue)
                        //    classes += 'ui-grid-custom-row-red ';
                        //if (row.entity.InProgress)
                        //    classes += 'ui-grid-custom-row-blue ';
                        if (row.entity.Completed)
                            classes += 'ui-grid-custom-row-linethrough ';
                        return classes;
                    }
                }
            });
        }

        ctrl.getColumnDefs = function (useKanban, selectTasks, prefilter, taskGroupId) {

            // visible columns
            var columnDefs = [
                {
                    name: 'Viewed',
                    displayName: '',
                    enableHiding: false,
                    width: 25,
                    cellTemplate: '<div class="ui-grid-cell-contents"><span ng-if="!row.entity.Viewed" class="fa fa-circle text-warning" title="' + $translate.instant('Admin.Js.Tasks.Tasks.NotViewed') + '"> </span></div>'
                },
                {
                    name: 'Id',
                    displayName: '№',
                    width: 60
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.Task'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a href="tasks#?modal={{row.entity.Id}}" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTask(row.entity.Id, $event)">{{COL_FIELD}}</a> <span ng-if="row.entity.NewCommentsCount > 0" class="badge badge-pink" title="' + $translate.instant('Admin.Js.Tasks.Tasks.CountNewComments') + '">{{row.entity.NewCommentsCount}}</span></div>'
                },
                {
                    name: 'PriorityFormatted',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.Priority'),
                    width: 110
                },
                {
                    name: 'DateAppointedFormatted',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.DateOfCreation'),
                    width: 125,
                    cellTemplate: '<div class="ui-grid-cell-contents">{{COL_FIELD}}</div>'
                },
                {
                    name: 'DueDateFormatted',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.Deadline'),
                    width: 125,
                    visible: 1441,
                    cellTemplate: '<div class="ui-grid-cell-contents" ng-class="{\'ui-grid-cell-red\': row.entity.Overdue}">{{COL_FIELD}}</div>'
                },
                {
                    name: 'StatusFormatted',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.Status'),
                    width: 110,
                    cellTemplate: '<div class="ui-grid-cell-contents" ng-class="{\'ui-grid-cell-blue\': row.entity.InProgress}">{{COL_FIELD}}</div>',
                },
                {
                    name: 'Managers',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.Executor'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents ui-grid-cell-contents--flex-nowrap js-grid-not-clicked">' +
                        '<sidebar-user-trigger customer-id="manager.CustomerId" class="ui-grid-cell-contents" ng-repeat="manager in row.entity.Managers track by $index">' +
                        '<div class="ui-grid-cell-avatar" ng-if="manager.AvatarSrc != null"><img ng-src="{{manager.AvatarSrc}}" alt="{{manager.FullName}}" title="{{manager.FullName}}"/></div>' +
                        '<a href="" class="text-decoration-invert" ng-if="row.entity.Managers.length == 1">{{manager.FullName}}</a>' +
                        '</sidebar-user-trigger>' +
                        '</div>',
                    //width: 200,
                },
                {
                    name: 'AppointedName',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.TaskManager'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents js-grid-not-clicked">' +
                        '<sidebar-user-trigger customer-id="row.entity.AppointedCustomerId" ng-if="row.entity.AppointedCustomerId != null" class="ui-grid-cell-contents">' +
                        '<div class="ui-grid-cell-avatar" ng-if="row.entity.AppointedCustomerAvatarSrc != null"><img ng-src="{{row.entity.AppointedCustomerAvatarSrc}}"/></div>' +
                        '<a href="" class="text-decoration-invert">{{COL_FIELD}}</a>' +
                        '</sidebar-user-trigger>' +
                        '</div>',
                    //width: 200,
                },
                {
                    name: '_serviceColumn',
                    enableHiding: false,
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents ui-grid-custom-ignore-row-style js-grid-not-clicked"><div><a href="javascript:void(0);" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTask(row.entity.Id, $event)"></a>' +
                        (ctrl.isAdmin ? '<ui-grid-custom-delete url="tasks/deletetask" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete></div></div>' : '')
                }];

            // filters
            if (taskGroupId == null) {
                columnDefs.push({
                    name: '_noopColumnTaskGroups',
                    enableHiding: false,
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Project'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'TaskGroupId',
                        fetch: 'taskgroups/getTaskGroupsSelectOptions'
                    }
                });
            }
            columnDefs.push.apply(columnDefs,
                [{
                    name: '_noopColumnDateCreated',
                    enableHiding: false,
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Created'),
                        type: 'datetime',
                        term: {
                            from: new Date((new Date()).setMonth((new Date()).getMonth() - 1)),
                            to: new Date()
                        },
                        datetimeOptions: {
                            from: { name: 'DateCreatedFrom' },
                            to: { name: 'DateCreatedTo' }
                        }
                    }
                },
                {
                    name: '_noopColumnPriorities',
                    enableHiding: false,
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Priority'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Priority',
                        fetch: 'tasks/getTaskPrioritiesSelectOptions'
                    }
                },
                {
                    name: '_noopColumnDueDateFormatted',
                    enableHiding: false,
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Deadline'),
                        type: 'datetime',
                        term: {
                            from: new Date((new Date()).setMonth((new Date()).getMonth() - 1)),
                            to: new Date()
                        },
                        datetimeOptions: {
                            from: { name: 'DueDateFrom' },
                            to: { name: 'DueDateTo' }
                        }
                    }
                },
                {
                    name: '_noopColumnViewed',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Viewed'),
                        name: 'Viewed',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.Tasks.Tasks.Yes'), value: 'true' }, { label: $translate.instant('Admin.Js.Tasks.Tasks.No'), value: 'false' }]
                    }
                }]);

            if ((useKanban && selectTasks != 'assignedtome') || (!useKanban && prefilter != 'assignedtome')) {
                columnDefs.push({
                    name: '_noopColumnAssigned',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Executor'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'AssignedManagerId',
                        fetch: 'managers/getAllTaskManagers?includeEmpty=true&assigned=true'
                    }
                });
            }
            if ((useKanban && selectTasks != 'appointedbyme') || (!useKanban && prefilter != 'appointedbyme')) {
                columnDefs.push({
                    name: '_noopColumnAppointed',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.TaskManager'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'AppointedManagerId',
                        fetch: 'managers/getAllTaskManagers?appointed=true'
                    }
                });
            }
            if (!useKanban && prefilter != 'completed' && prefilter != 'accepted') {
                columnDefs.push({
                    name: '_noopColumnStatuses',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Status'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Status',
                        fetch: prefilter == '' || prefilter == 'none' || prefilter == 'assignedtome'
                            ? 'tasks/getNotCompletedTaskStatusesSelectOptions'
                            : 'tasks/getTaskStatusesSelectOptions'
                    }
                });
            }
            if ((useKanban && selectTasks != 'observedbyme') || (!useKanban && prefilter != 'observedbyme')) {
                columnDefs.push({
                    name: '_noopColumnObserved',
                    enableHiding: false,
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.TaskObserver'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'ObserverId',
                        fetch: 'managers/getAllTaskManagers?observed=true'
                    }
                });
            }
            //if (prefilter == 'completed') {
            //    ctrl.gridOptions.uiGridCustom.selectionOptions.push({
            //        text: 'Принять выделенные',
            //        url: 'tasks/accepttasks',
            //        field: 'Id'
            //    });
            //}

            return columnDefs;
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.fetchData = function (ignoreHistory) {
            if (!ctrl.useKanban) {
                ctrl.grid.fetchData(ignoreHistory);
            } else {
                ctrl.kanban.fetchData();
            }
        };

        ctrl.refresh = function (reload) {
            if (reload) {
                ctrl.reload();
            } else {
                ctrl.fetchData();
            }
        };

        ctrl.reload = function (query) {
            var loc = $window.location.href.split('#')[0];
            var url = loc.split('?')[0] + (query || '');
            url += (url.indexOf('?') === -1 ? '?' : '&') + 'rnd=' + Math.random();
            $window.location.href = url;
        };

        ctrl.modalDismiss = ctrl.modalClose = function (result, refresh) {
            if (!ctrl.useKanban || !result || result.refresh) {
                ctrl.fetchData();
            }
            $location.search('modal', null);
            //$location.search('modalShow', null);
        };

        ctrl.changeView = function (view) {
            ctrl.setCookie('tasks_viewmode', view);
            ctrl.reload(view == 'grid' && ctrl.selectTasks != 'all' ? '?filterby=' + ctrl.selectTasks : '');
        };

        ctrl.toggleViewTasks = function (selectTasks) {
            //ctrl.gridParams.selectTasks = selectTasks;
            ctrl.setCookie('tasks_mykanban', selectTasks);

            ctrl.gridOptions.columnDefs.length = 0;
            ctrl.gridOptions.columnDefs.push.apply(ctrl.gridOptions.columnDefs, ctrl.getColumnDefs(ctrl.useKanban, selectTasks, ctrl.prefilter, ctrl.taskGroupId));

            ctrl.kanbanFilter.updateColumns();
            ctrl.kanban.resetColumnsData();
            ctrl.fetchData();
        };

        ctrl.setCookie = function (name, value) {
            var date = new Date();
            date.setFullYear(date.getFullYear() + 1)
            $cookies.put(name, value, { expires: date });
        };

        ctrl.loadTask = function (id, $event) {
            if ($event) {
                $event.preventDefault();
            }

            $uibModalStack.dismissAll('open other modal');

            $uibModal.open({
                animation: false,
                bindToController: true,
                controller: 'ModalEditTaskCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/tasks/modal/editTask/editTask.html',
                resolve: {
                    id: function () {
                        return id;
                    }
                },
                size: 'xs-8',
                backdrop: 'static',
                windowClass: 'modal__window--scrollbar-no',
            }).result.then(function (result) {
                ctrl.modalClose(result);
                return result;
            }, function (result) {
                ctrl.modalDismiss(result);
                return result;
            });
        };

        ctrl.newTask = function () {

            $uibModalStack.dismissAll('open other modal');

            $uibModal.open({
                animation: false,
                bindToController: true,
                controller: 'ModalAddTaskCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/_shared/modal/addTask/addTask.html',
                size: 'xs-8 modal-md-6',
                backdrop: 'static'
            }).result.then(function (result) {
                ctrl.refresh(!ctrl.useKanban);
                return result;
            }, function (result) {
                $location.search('modalNewTask', null);
                return result;
            });
        };


        /************ Kanban  +  filter **************/

        ctrl.sortableOptions = {
            containment: '#kanban',
            containerPositioning: 'relative',
            additionalPlaceholderClass: 'kanban__placeholder',
            itemMoved: function (event) {
                var status;
                var defer = $q.defer();
                switch (event.dest.sortableScope.$parent.column.Id) {
                    case 'New':
                        status = 'Open';
                        defer.resolve();
                        break;
                    case 'InProgress':
                        status = 'InProgress';
                        defer.resolve();
                        break;
                    case 'Done':
                        tasksService.completeTaskShowModal({
                            id: event.source.itemScope.modelValue.Id,
                            name: event.source.itemScope.modelValue.Name,
                            leadId: event.source.itemScope.modelValue.LeadId,
                            orderId: event.source.itemScope.modelValue.OrderId
                        })
                            .then(function () {
                                status = 'Completed';
                                defer.resolve({ changeStatus: false });
                                return status;
                            })
                            .catch(function () {
                                return defer.reject();
                            });
                        break;
                    //case 'Accepted':
                    //    tasksService.completeTask(event.source.itemScope.modelValue.Id).then(function (data) {
                    //        if (data != null && data.result === true) {
                    //            lastStatisticsService.getLastStatistics();
                    //        }
                    //    });
                    //    break;
                    default:
                        throw ('no status to change');
                }

                defer.promise.then(function (params) {
                    var deferAfter = $q.defer();
                    if (params != null && params.changeStatus === false) {
                        deferAfter.resolve();
                    } else {
                        tasksService.changeTaskStatus(event.source.itemScope.modelValue.Id, status).then(function (response) {
                            deferAfter.resolve(true);
                        });
                    }
                    deferAfter.promise.then(function (showToaster) {
                        if (showToaster) {
                            toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.Task') + ' <a  href="tasks/view/' + event.source.itemScope.modelValue.Id + '">№' + event.source.itemScope.modelValue.Id + '</a> ' + $translate.instant('Admin.Js.EditTask.TaskSaved'));
                        }
                        event.source.sortableScope.$parent.column.TotalCardsCount -= 1;
                        event.dest.sortableScope.$parent.column.TotalCardsCount += 1;
                        ctrl.onOrderChanged(event);
                    });
                })
                    .catch(function () {
                        event.dest.sortableScope.removeItem(event.dest.index);
                        event.source.itemScope.sortableScope.insertItem(event.source.index, event.source.itemScope.card);
                    });
            },
            orderChanged: function (event) {
                ctrl.onOrderChanged(event, true);
            }
        };

        ctrl.onOrderChanged = function (event, showMessage) {
            var current = event.source.itemScope.card,
                prev = event.dest.sortableScope.modelValue[event.dest.index - 1],
                next = event.dest.sortableScope.modelValue[event.dest.index + 1];
            // высокий приоритет выводится выше, задачу со средним или низким приоритетом не вставлять между задачами с высоким приоритетом и наоборот
            if (prev != null && (prev.Priority == 2 || current.Priority == 2) && prev.Priority != current.Priority) {
                prev = null;
            }
            if (next != null && (next.Priority == 2 || current.Priority == 2) && next.Priority != current.Priority) {
                next = null;
            }
            tasksService.changeSorting(current.Id, prev != null ? prev.Id : null, next != null ? next.Id : null).then(function (data) {
                if (showMessage && data != null && data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.ChangesSaved'));
                }
            });
        };

        ctrl.kanbanOnInit = function (kanban) {
            ctrl.kanban = kanban;
        };

        ctrl.kanbanOnFilterInit = function (filter) {
            ctrl.kanbanFilter = filter;
        };

        ctrl.$onInit = function () {

            var locationSearch = $location.search();

            if (locationSearch != null) {

                if (locationSearch.modal != null) {
                    ctrl.loadTask(locationSearch.modal);
                } else if (locationSearch.modalNewTask) {
                    ctrl.newTask();
                }
            }

            $rootScope.$on('$locationChangeSuccess', function (event, newUrl, oldUrl) {
                var params = $location.search(); 
                if (newUrl !== oldUrl && params != null && params.modal != null && params.modalOpened !== 'true') { //&& $location.search().modalShow != null
                    ctrl.loadTask($location.search().modal);
                }
            });

            adminWebNotificationsService.addListener(adminWebNotificationsEvents.updateTasks, function () { ctrl.fetchData(true); });
        };


        ctrl.editTaskGroupClose = function (result) {
            if (result != null) {
                ctrl.taskGroupName = result.name;
            }
        };


        ctrl.goToTab = function (event, link) {
            var params = ctrl.grid.getParams();

            delete params.filterby;

            ctrl.grid.setParams(params);
            event.preventDefault();

            setTimeout(function () {
                $window.location.assign(link + $window.location.hash);
            });
        };
    };

    TasksCtrl.$inject = [
        '$cookies',
        '$location',
        '$q',
        '$rootScope',
        '$uibModal',
        '$window',
        'adminWebNotificationsEvents',
        'adminWebNotificationsService',
        'lastStatisticsService',
        'SweetAlert',
        'tasksService',
        'toaster',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'urlHelper',
        '$translate',
        '$uibModalStack'];

    ng.module('tasks', ['uiGridCustom', 'adminComments', 'urlHelper'])
        .controller('TasksCtrl', TasksCtrl);

})(window.angular);