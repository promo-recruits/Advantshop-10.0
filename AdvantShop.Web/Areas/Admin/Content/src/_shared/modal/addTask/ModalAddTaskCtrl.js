; (function (ng) {
    'use strict';

    var ModalAddTaskCtrl = function ($document, $uibModalInstance, $filter, $q, Upload, toaster, tasksService, $window, lastStatisticsService, $translate, $scope, $timeout, SweetAlert) {
        var ctrl = this;

        ctrl.ckeditor = {
            height: 150,
            extraPlugins: 'clicklinkbest,codemirror,lineheight,autolinker,autogrow',
            bodyClass: 'm-n textarea-padding',
            toolbar: {},
            toolbarGroups: {},
            resize_enabled: false,
            toolbar_emptyToolbar: { name: 'empty', items: [] },
            autoGrow_minHeight: 233,
            autoGrow_onStartup: true,
            on: {
                instanceReady: function (event) {
                    $document[0].getElementById(event.editor.id + '_top').style.display = 'none';
                },
                focus: function (event) {
                    if (ctrl.uiSelectCtrl) {
                        ctrl.uiSelectCtrl.close();
                    }

                    if (ctrl.flatpickr) {
                        ctrl.flatpickr.close();
                    }
                }
            },
            disableNativeSpellChecker: false,
            browserContextMenuOnCtrl: false,
            removePlugins: 'language,liststyle,tabletools,scayt,menubutton,contextmenu,tableselection,elementspath'
        };

        ctrl.getUiSelectCtrl = function (uiSelectCtrl) {
            ctrl.uiSelectCtrl = uiSelectCtrl;
        };

        ctrl.flatpickrOnSetup = function (fpItem) {
            ctrl.flatpickr = fpItem;
        };

        ctrl.flatpickrOnChange = function (selectedDates, dateStr, instance) {
            if (selectedDates.length == 0) {
                return;
            }
            var date = new Date(selectedDates[0]);
            var minutes = (Math.floor(date.getMinutes() / 10) * 10);
            date.setMinutes(minutes);
            ctrl.dueDate = date.getFullYear() + '-' +
                ('0' + (date.getMonth() + 1)).slice(-2) + '-' +
                ('0' + date.getDate()).slice(-2) + 'T' +
                ('0' + date.getHours()).slice(-2) + ':' +
                ('0' + date.getMinutes()).slice(-2);
        }

        ctrl.$onInit = function () {

            var params = ctrl.$resolve;
            ctrl.redirectToTasks = params.redirectToTasks || false;
            if (params.bindTo != null) {
                switch (params.bindTo.type.toLowerCase()) {
                    case 'order':
                        ctrl.orderId = params.bindTo.objId;
                        break;
                    case 'lead':
                        ctrl.leadId = params.bindTo.objId;
                        break;
                    case 'customer':
                        ctrl.clientCustomerId = params.bindTo.objId;
                        break;
                }
            }

            tasksService.getFormData(null, params.taskGroupId).then(function (data) {
                if (data != null) {
                    ctrl.managersAssign = data.managersAssign;
                    ctrl.currentManagerId = data.currentManagerId,
                    ctrl.taskGroups = data.taskGroups;
                    ctrl.priorities = data.priorities;
                    ctrl.filesHelpText = data.filesHelpText;
                    ctrl.taskGroupId = params.taskGroupId; //|| data.defaultTaskGroupId || null;
                    ctrl.reminderTypes = data.reminderTypes;
                    ctrl.remind = false;
                    ctrl.reminder = ctrl.reminderTypes[0].value;
                    ctrl.reminderActive = data.reminderActive;

                    if (ctrl.taskGroupId != null) {
                        var items = ctrl.taskGroups.filter(function (x) { return x.value == ctrl.taskGroupId; });
                        if (items != null && items.length == 0) {
                            ctrl.taskGroupId = '';
                        }
                    }

                    if (ctrl.priorities.length > 1) {
                        ctrl.priority = ctrl.priorities[1].value;
                    } else if (ctrl.priorities.length > 0) {
                        ctrl.priority = ctrl.priorities[0].value;
                    }

                    ctrl.canAssingToMe = ctrl.managersAssign.some(function (item) {
                        return item.value == ctrl.currentManagerId;
                    });
                }

                if (params.userData != null) {
                    ctrl.managerIds = [params.userData.assignedManager];
                }

                $timeout(function () {
                    ctrl.formAddTask.$setPristine();
                });
            });

            ctrl.attachments = [];
            ctrl.managerIds = [];

            $window.onbeforeunload = ctrl.warningMissingData;

            $scope.$on('modal.closing', function ($event, reason, closed) {
                var defer = $q.defer(),
                    promise;

                if (ctrl.formAddTask.modified === true && closed === false && (reason == null || reason.notCheck !== true)) {
                    $event.preventDefault();
                    promise = $timeout(function () {
                        return SweetAlert.confirm($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingData'), { title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingTitle') })
                               .then(function (result) {
                                   return result;
                               }, 100);
                    });
                } else {
                    promise = defer.promise;

                    defer.resolve(null);
                }

                promise.then(function (result) {
                    if (result !== false) {
                        if (closed === false) {
                            ctrl.close(true);
                        }
                    }

                    $window.onbeforeunload = null;
                })
            });

        };

        ctrl.warningMissingData = function () {
            if (ctrl.formAddTask.modified === true) {
                return $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingData');
            }
        };

        ctrl.close = function (notCheck) {
            $uibModalInstance.dismiss({ notCheck: notCheck });
        };

        ctrl.addAttachments = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {

            ctrl.loadingFiles = true;
            if (($event.type === 'change' || $event.type === 'drop') && $files != null && $files.length > 0) {
                for (var i = 0, len = ctrl.attachments.length; i < len; i++) {
                    if ($filter('filter')($files, { name: ctrl.attachments[i].name }, true)[0] != null) {
                        toaster.pop('error', $translate.instant('Admin.Js.AddTask.Error'), ctrl.attachments[i].name + $translate.instant('Admin.Js.AddTask.FileAlreadyExist'));
                        $files = $filter('filter')($files, function (file) { return file.name !== ctrl.attachments[i].name; });
                        ctrl.loadingFiles = false;
                        return;
                    }
                }
                Upload.upload({
                    url: 'tasks/validateAttachments',
                    data: {},
                    file: $files,
                }).then(function (response) {
                    var data = response.data;
                    for (var i = 0, len = data.length; i < len; i++) {
                        if (data[i].Result === true) {
                            ctrl.attachments.push($files[i]);
                        }
                        else {
                            toaster.pop('error', $translate.instant('Admin.Js.AddTask.Error'), (data[i].Attachment != null ? data[i].Attachment.FileName + ": " : "") + data[i].Error);
                        }
                    }
                    ctrl.loadingFiles = false;
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.AddTask.ErrorLoading'), $translate.instant('Admin.Js.AddTask.FileDoesNotMeet'));
                ctrl.loadingFiles = false;
            }
            else {
                ctrl.loadingFiles = false;
            }
        };

        ctrl.deleteAttachment = function (index) {
            ctrl.attachments.splice(index, 1);
        };

        ctrl.addTask = function () {

            ctrl.validateTaskGroupManager().then(function (result) {
                if (!result) {
                    ctrl.btnLoading = false;
                    return;
                }

                setTimeout(function () {  // задержка нужна для ckeditor чтоб проинициализировались данные

                    Upload.upload({
                        url: 'tasks/addTask',
                        data: {
                            name: ctrl.name,
                            managerIds: ctrl.managerIds,
                            dueDate: ctrl.dueDate,
                            description: ctrl.description,
                            taskGroupId: ctrl.taskGroupId,
                            priority: ctrl.priority,
                            orderId: ctrl.orderId,
                            leadId: ctrl.leadId,
                            clientCustomerId: ctrl.clientCustomerId,
                            remind: ctrl.remind,
                            reminder: ctrl.reminder
                        },
                        file: ctrl.attachments,
                    })
                        .then(function (response) {
                            var data = response.data;
                            if (data.result === true) {
                                toaster.pop('success', '', $translate.instant('Admin.Js.AddTask.Task') + ' <a href="tasks/view/' + data.id + '">№' + data.id + '</a> ' + $translate.instant('Admin.Js.AddTask.Added'));

                                $window.onbeforeunload = null;

                                if (ctrl.redirectToTasks && $window.location.pathname.split('/').pop() != 'tasks') {
                                    $window.location.assign('tasks');
                                }

                                $uibModalInstance.close();

                                lastStatisticsService.getLastStatistics();
                            } else {
                                if (data.errors != null) {
                                    data.errors.forEach(function(err) {
                                        toaster.pop('error', '', err);
                                    });
                                } else {
                                    toaster.pop('error', $translate.instant('Admin.Js.AddTask.Error'), data.Error);
                                }
                            }
                        }).then(function () {
                            ctrl.btnLoading = false;
                        });

                }, 300);
            });
        };

        ctrl.validateTaskGroupManager = function () {
            if (ctrl.taskGroupId == null || ctrl.managerIds == null || ctrl.managerIds.length == 0) {
                return $q.resolve(true);
            }
            return tasksService.validateTaskGroupManager(ctrl.managerIds, ctrl.taskGroupId).then(function (data) {
                if (data.errors != null) {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                    return false;
                }
                return true;
            });
        };

        ctrl.getManagers = function () {
            tasksService.getTaskManagers(null, ctrl.taskGroupId).then(function (result) {
                if (result != null) {
                    ctrl.managersAssign = result.managersAssign;
                    ctrl.canAssingToMe = ctrl.managersAssign.some(function (item) {
                        return item.value == ctrl.currentManagerId;
                    });
                }
            });
        };
    };

    ModalAddTaskCtrl.$inject = ['$document', '$uibModalInstance', '$filter', '$q', 'Upload', 'toaster', 'tasksService', '$window', 'lastStatisticsService', '$translate', '$scope', '$timeout', 'SweetAlert'];

    ng.module('uiModal')
        .controller('ModalAddTaskCtrl', ModalAddTaskCtrl);

})(window.angular);