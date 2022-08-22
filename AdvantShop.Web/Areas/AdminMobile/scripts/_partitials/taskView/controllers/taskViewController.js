; (function (ng) {
    'use strict';

    var TaskViewCtrl = function ($http) {

        var taskCtrl = this;
        taskCtrl.resultCode = -1;

        taskCtrl.changeStatus = function () {
            $http.post("/adminmobile/Tasks/ChangeStatus", { 'taskId': taskCtrl.taskId, 'status': taskCtrl.status })
                .then(function (response) {
                    taskCtrl.statusSaved = response.data != null && response.data.ResultCode === 0;
                    alert("Статус задачи сохранен");
                });
        }
    };

    ng.module("tasksView")
        .controller("TaskViewController", TaskViewCtrl);

    TaskViewCtrl.$inject = ['$http'];

})(window.angular);
