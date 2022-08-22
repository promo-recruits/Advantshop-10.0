﻿; (function (ng) {
    'use strict';

    var ModalCompleteTaskCtrl = function ($translate, $uibModalInstance, lastStatisticsService, tasksService, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.task = ctrl.$resolve.task;
            if (ctrl.task == null) {
                ctrl.close();
                return;
            }
            if (ctrl.task.orderId != null) {
                tasksService.getOrderStatuses(ctrl.task.orderId).then(function (data) {
                    ctrl.orderStatuses = data;
                    for (var i = 0, len = ctrl.orderStatuses.length; i < len; i++) {
                        if (ctrl.orderStatuses[i].selected === true) {
                            ctrl.orderStatusIdCurrent = ctrl.orderStatuses[i].value;
                            ctrl.orderStatusId = ctrl.orderStatuses[i].value;
                        }
                    }
                });
            } else if (ctrl.task.leadId != null) {
                tasksService.getDealStatuses(ctrl.task.leadId).then(function (data) {
                    ctrl.dealStatuses = data;
                    for (var i = 0, len = ctrl.dealStatuses.length; i < len; i++) {
                        if (ctrl.dealStatuses[i].selected === true) {
                            ctrl.dealStatusIdCurrent = ctrl.dealStatuses[i].value;
                            ctrl.dealStatusId = ctrl.dealStatuses[i].value;
                        }
                    }
                });
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.completeTask = function () {
            ctrl.btnSleep = true;
            tasksService.completeTask(ctrl.task.id, ctrl.taskResult, ctrl.orderStatusId, ctrl.dealStatusId).then(function (data) {
                if (data != null && data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.Task') + ' <a  href="tasks/view/' + ctrl.task.id + '">№' + ctrl.task.id + '</a> ' + $translate.instant('Admin.Js.EditTask.Completed') + '<br>' + (data.obj != null && data.obj.orderId != null ? ($translate.instant('Admin.Js.CompleteTask.OrderCreated') + ' <a href="orders/edit/' + data.obj.orderId + '">' + data.obj.orderNumber + '</a>') : ''));
                    $uibModalInstance.close(true);
                    lastStatisticsService.getLastStatistics();
                } else {
                    toaster.error($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.FailedToCompleteTask'));
                    $uibModalInstance.close(false);
                }
                ctrl.btnSleep = false;
            });

        };
    };

    ModalCompleteTaskCtrl.$inject = ['$translate', '$uibModalInstance', 'lastStatisticsService', 'tasksService', 'toaster'];

    ng.module('uiModal')
        .controller('ModalCompleteTaskCtrl', ModalCompleteTaskCtrl);

})(window.angular);