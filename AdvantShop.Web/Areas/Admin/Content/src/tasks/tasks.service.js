; (function (ng) {
    'use strict';

    var tasksService = function ($http, $uibModal, toaster, $translate) {
        var service = this;

        service.getTasks = function () {
            return $http.get('tasks/getTasks').then(function (response) {
                return response.data;
            });
        }

        service.getFormData = function (id, taskGroupId) {
            return $http.get('tasks/getTaskFormData', { params: { id: id, taskGroupId: taskGroupId } }).then(function (response) {
                return response.data;
            });
        }

        service.getTaskGroups = function () {
            return $http.get('taskgroups/getTaskGroupsSelectOptions').then(function (response) {
                return response.data;
            });
        }

        service.getTaskPriorities = function () {
            return $http.get('tasks/getTaskPrioritiesSelectOptions').then(function (response) {
                return response.data;
            });
        }

        service.getTaskStatuses = function () {
            return $http.get('tasks/getTaskStatusesSelectOptions').then(function (response) {
                return response.data;
            });
        }

        service.getTaskAttachments = function (id) {
            return $http.post('tasks/getTaskAttachments', { id: id }).then(function (response) {
                return response.data;
            });
        }

        service.getTask = function (id) {
            return $http.post('tasks/getTask', { id: id }).then(function (response) {
                return response.data;
            });
        }

        service.deleteTask = function (id) {
            return $http.post('tasks/deleteTask', { id: id }).then(function (response) {
                return response.data;
            });
        }

        service.addTask = function (params) {
            return $http.post('tasks/addTask', params).then(function (response) {
                return response.data;
            });
        }

        service.editTask = function (params) {
            return $http.post('tasks/editTask', params).then(function (response) {
                return response.data;
            });
        }

        service.changeTaskStatus = function (id, status) {
            return $http.post('tasks/changeTaskStatus', { id: id, status: status }).then(function (response) {
                return response.data;
            });
        }

        service.changeAssignedManager = function (id, managerIds) {
            return $http.post('tasks/changeAssignedManager', { id: id, managerIds: managerIds }).then(function (response) {
                return response.data;
            });
        }

        service.changeTaskStatuses = function (params) {
            return $http.post('tasks/changeTaskStatuses', params).then(function (response) {
                return response.data;
            });
        }

        service.completeTask = function (id, result, orderStatusId, dealStatusId) {
            return $http.post('tasks/completeTask', { id: id, taskResult: result, orderStatusId: orderStatusId, dealStatusId: dealStatusId }).then(function (response) {
                return response.data;
            });
        }

        service.getOrderStatuses = function (orderId) {
            return $http.post('tasks/getOrderStatuses', { orderId: orderId }).then(function (response) {
                return response.data;
            });
        }

        service.getDealStatuses = function (leadId) {
            return $http.post('tasks/getDealStatuses', { leadId: leadId }).then(function (response) {
                return response.data;
            });
        }

        service.acceptTask = function (id) {
            return $http.post('tasks/acceptTask', { id: id }).then(function (response) {
                return response.data;
            });
        }

        service.acceptTasks = function (params) {
            return $http.post('tasks/accepttasks', params).then(function (response) {
                return response.data;
            });
        }

        service.changeSorting = function (id, prevId, nextId) {
            return $http.post('tasks/changeSorting', { id: id, prevId: prevId, nextId: nextId }).then(function (response) {
                return response.data;
            });
        }

        service.deleteAttachment = function (id, taskId) {
            return $http.post('tasks/deleteAttachment', { id: id, taskId: taskId }).then(function (response) {
                return response.data;
            });
        }

        service.loadTask = function (id) {
            return $uibModal.open({
                bindToController: true,
                controller: 'ModalEditTaskCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/tasks/modal/editTask/editTask.html',
                resolve: {
                    id: function () {
                        return id;
                    }
                },
                size: 'lg',
                backdrop: 'static'
            });
        };

        service.getHistory = function(id) {
            return $http.get('tasks/getHistory', { params: { id: id }}).then(function (response) {
                return response.data;
            });
        }

        service.validateTaskGroupManager = function (managerIds, taskGroupId) {
            return $http.get('tasks/validateTaskGroupManager', { params: { managerIds: managerIds, taskGroupId: taskGroupId } }).then(function (response) {
                return response.data;
            });
        }

        service.validateTaskGroupManagerByRoles = function (managerIds, managerRoleIds) {
            return $http.get('tasks/validateTaskGroupManagerByRoles', { params: { managerIds: managerIds, managerRoleIds: managerRoleIds } }).then(function (response) {
                return response.data;
            });
        }

        service.validateTaskData = function (appointedManagerId, taskGroupId) {
            return $http.get('tasks/validateTaskData', { params: { appointedManagerId: appointedManagerId, taskGroupId: taskGroupId } }).then(function (response) {
                return response.data;
            });
        }

        service.getTaskManagers = function (id, taskGroupId) {
            return $http.get('tasks/getManagers', { params: { id: id, taskGroupId: taskGroupId } }).then(function (response) {
                return response.data;
            });
        }

        service.copyTask = function (id) {
            return $http.post('tasks/copyTask', { id: id }).then(function (response) {
                return response.data;
            });
        }

        service.completeTaskShowModal = function (taskData) {
            return $uibModal.open({
                bindToController: true,
                controller: 'ModalCompleteTaskCtrl',
                controllerAs: 'ctrl',
                templateUrl: '../areas/admin/content/src/tasks/modal/completeTask/completeTask.html',
                windowClass: 'modal--strecth',
                resolve: {
                    task: taskData
                }
            }).result;
        }

        service.changeTaskGroup = function (params) {
            return $http.post('tasks/changeTaskGroup', params).then(function (response) {
                return response.data;
            });
        }

        service.changeObserver = function (id, observerIds) {
            return $http.post('tasks/changeObserver', { id: id, observerIds: observerIds }).then(function (response) {
                return response.data;
            });
        }
    };

    tasksService.$inject = ['$http', '$uibModal', 'toaster', '$translate'];

    ng.module('tasks')
        .service('tasksService', tasksService);

})(window.angular);