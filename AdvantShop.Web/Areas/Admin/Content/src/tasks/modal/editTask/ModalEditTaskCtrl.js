; (function (ng) {
    'use strict';

    var ModalEditTaskCtrl = function ($document, $uibModalInstance, $location, $uibModal, SweetAlert, toaster, Upload, tasksService, $q, $translate, $window, $scope, $timeout, leadService) {
        var ctrl = this;
        var ckeditorObj;

        ctrl.$onInit = function () {

            ctrl.localStorageKey = 'adminTaskButtonAction';

            ctrl.formInited = false;
            ctrl.needRefresh = false;

            ctrl.ckeditor = {
                height: 150,
                extraPlugins: 'clicklinkbest,codemirror,lineheight,autolinker,autogrow,pastefromgdocs',
                bodyClass: 'm-n textarea-padding',
                toolbar: {},
                toolbarGroups: {},
                resize_enabled: false,
                toolbar_emptyToolbar: { name: 'empty', items: [] },
                autoGrow_minHeight: 233,
                autoGrow_onStartup: true,
                on: {
                    instanceReady: function (event) {
                        ckeditorObj = event.editor;
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


            ctrl.buttonActions = {
                'save': {
                    'fn': ctrl.chain.bind(ctrl, ctrl.saveTask, ctrl.close),
                    'text': $translate.instant('Admin.Js.Tasks.EditTask.Save'),
                },
                'saveAndInProgress': {
                    'fn': ctrl.chain.bind(ctrl, ctrl.saveTask, ctrl.taskInProgress, ctrl.close),
                    'text': 'Cохранить и начать выполнение',
                },
                'saveAndComplete': {
                    'fn': ctrl.chain.bind(ctrl, ctrl.saveTask, ctrl.completeTask, ctrl.close),
                    'text': 'Сохранить и завершить'
                },
                'saveAndPause':
                {
                    'fn': ctrl.chain.bind(ctrl, ctrl.saveTask, ctrl.taskOpen, ctrl.close),
                    'text': 'Сохранить и приостановить'
                },
                'saveAndAccept':
                {
                    'fn': ctrl.chain.bind(ctrl, ctrl.saveTask, ctrl.acceptTask, ctrl.close),
                    'text': 'Сохранить и принять'
                },
                'saveAndResume':
                {
                    'fn': ctrl.chain.bind(ctrl, ctrl.saveTask, ctrl.taskOpen, ctrl.close),
                    'text': 'Сохранить и возобновить'
                }
            };

            //$location.search('grid', null);
            $location.search('viewed', 'true'); // prevent scroll to page top after modal close
            $location.search('modal', ctrl.$resolve.id);
            $location.search('modalOpened', 'true');
            ctrl.managerIds = [];

            var search = $location.search();
            if (search.needRefresh != null) {
                ctrl.needRefresh = true;
            }

            tasksService.getFormData(ctrl.$resolve.id).then(function (data) {
                if (data != null) {
                    ctrl.currentManagerId = data.currentManagerId;
                    ctrl.managersAssign = data.managersAssign;
                    ctrl.managersAppoint = data.managersAppoint;
                    ctrl.managersObserve = data.managersObserve;
                    ctrl.taskGroups = data.taskGroups;
                    ctrl.priorities = data.priorities;
                    ctrl.filesHelpText = data.filesHelpText;
                    ctrl.leadDealStatuses = data.leadDealStatuses;
                    ctrl.reminderTypes = data.reminderTypes;
                    ctrl.reminderActive = data.reminderActive;

                    ctrl.canAssingToMe = ctrl.managersAssign.some(function (item) {
                        return item.value == ctrl.currentManagerId;
                    });
                }

                return tasksService.getTask(ctrl.$resolve.id).then(function (result) {
                    if (result.result === false) {
                        ctrl.close();
                        if (result.errors != null) {
                            result.errors.forEach(function (err) {
                                toaster.error('', err);
                            });
                        }
                    }

                    ctrl.formInited = true;
                    ctrl.id = result.Id;
                    ctrl.name = result.Name;
                    ctrl.dueDate = result.DueDate;
                    ctrl.description = result.Description;
                    ctrl.managerIds = result.ManagerIds;
                    ctrl.appointedManagerId = result.AppointedManagerId;
                    ctrl.taskGroupId = result.TaskGroupId;
                    ctrl.priority = result.Priority;
                    ctrl.dateAppointedFormatted = result.DateAppointedFormattedFull;
                    ctrl.status = result.StatusString;
                    ctrl.accepted = result.Accepted;
                    ctrl.orderId = result.OrderId;
                    ctrl.orderNumber = result.OrderNumber;
                    ctrl.leadId = result.LeadId;
                    ctrl.leadTitle = result.LeadTitle;
                    ctrl.leadSalesFunnelId = result.LeadSalesFunnelId;
                    ctrl.leadDealStatusId = result.LeadDealStatusId;
                    ctrl.reviewId = result.ReviewId;
                    ctrl.bindedTaskId = result.BindedTaskId;
                    ctrl.clientCustomerId = result.ClientCustomerId;
                    ctrl.clientName = result.ClientName;
                    ctrl.canDelete = result.CanDelete;
                    ctrl.result = result.ResultFull;
                    ctrl.editTaskForm.$setPristine();
                    ctrl.taskUrl = window.location.origin + window.location.pathname.replace('leads', 'tasks') + '#?modal=' + ctrl.id;
                    ctrl.commentsType = result.IsPrivateComments ? 'taskHidden' : 'task';
                    ctrl.isAutomatic = result.IsAutomatic;
                    ctrl.isReadonlyTask = result.IsReadonlyTask;
                    ctrl.remind = result.Remind;
                    ctrl.reminder = result.Remind == false ? ctrl.reminderTypes[0].value : result.Reminder;
                    ctrl.observerIds = result.ObserverIds;
                    ctrl.observingTask = ctrl.observerIds.includes(ctrl.currentManagerId);
                });
            })
                .then(function () {
                    var buttonActionNameDefault = ctrl.getButtonActionNameDefault(ctrl.status);
                    ctrl.buttonActionDefault = ctrl.buttonActions[buttonActionNameDefault];
                    ctrl.buttonActionsCurrent = ctrl.getButtonActionsByState(ctrl.status, buttonActionNameDefault);
                });

            tasksService.getTaskAttachments(ctrl.$resolve.id).then(function (result) {
                ctrl.attachments = result;
            });

            $window.onbeforeunload = ctrl.warningMissingData;

            $scope.$on('modal.closing', function ($event, reason, closed) {
                var defer = $q.defer(),
                    promise;

                if (ctrl.editTaskForm.modified === true && closed === false && (reason == null || reason.notCheck !== true) || ctrl.adminCommentsCtrl.form.text != null && ctrl.adminCommentsCtrl.form.text.length > 0 && closed === false && (reason == null || reason.notCheck !== true)) {
                    $event.preventDefault();
                    promise = $timeout(function () {
                        return SweetAlert.confirm($translate.instant(ctrl.adminCommentsCtrl.form.text != null && ctrl.adminCommentsCtrl.form.text.length > 0 ? 'Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingComment' : 'Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingData'), { title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingTitle') })
                            .then(function (result) {
                                return result;
                            }, 100);
                    });
                } else {
                    promise = defer.promise;

                    defer.resolve(null);
                }

                promise.then(function (result) {
                    if (result != null && result !== false) {
                        if (closed === true) {
                            ctrl.close();
                        } else {
                            ctrl.dismiss(true);
                        }
                    }

                    $location.search('modalOpened', null);
                    $window.onbeforeunload = null;
                });
            });
        };

        ctrl.warningMissingData = function () {
            if (ctrl.editTaskForm.modified === true || ctrl.adminCommentsCtrl.form.text != null && ctrl.adminCommentsCtrl.form.text.length > 0) {
                return $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingData');
            }
        };

        ctrl.getUiSelectCtrl = function (uiSelectCtrl) {
            ctrl.uiSelectCtrl = uiSelectCtrl;
        };

        ctrl.copy = function (data) {
            var input = document.createElement('input');
            input.setAttribute('value', data);
            input.style.opacity = 0;
            document.body.appendChild(input);
            input.select();
            if (document.execCommand('copy')) {
                toaster.success($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.LinkCopiedToClipboard'));
            } else {
                toaster.error($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.FailedToCopyLink'));
            }
            document.body.removeChild(input);
        };

        ctrl.dismiss = function (notCheck) {
            if (notCheck === true) {
                $location.search('modal', null);
            }

            $uibModalInstance.dismiss({ refresh: ctrl.needRefresh, notCheck: notCheck });
        };

        ctrl.close = function () {
            $location.search('needRefresh', null);
            $location.search('modal', null);
            $uibModalInstance.close({ refresh: true });
            return true;
        };

        ctrl.changeStatus = function (status) {
            return tasksService.changeTaskStatus(ctrl.id, status).then(function (response) {
                ctrl.status = response.status;
                if (ctrl.accepted) {
                    ctrl.accepted = false;
                }

                ctrl.needRefresh = true;
                
                var buttonActionNameDefault = ctrl.getButtonActionNameDefault(ctrl.status);
                ctrl.buttonActionDefault = ctrl.buttonActions[buttonActionNameDefault];
                ctrl.buttonActionsCurrent = ctrl.getButtonActionsByState(ctrl.status, buttonActionNameDefault);
                
                if (status == 'open') {
                    toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.Task') + ' <a  href="tasks/view/' + ctrl.id + '">№' + ctrl.id + '</a> ' + $translate.instant('Admin.Js.EditTask.Opened'));
                } else if (status == 'inprogress') {
                    toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.Task') + ' <a  href="tasks/view/' + ctrl.id + '">№' + ctrl.id + '</a> ' + $translate.instant('Admin.Js.EditTask.Started'));
                } else {
                    toaster.success($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.StatusChanged'));
                }

                return response;
            });
        };

        ctrl.changeAssignedManager = function (managerId, single) {
            if (single) {
                ctrl.managerIds = [managerId];
            }
            tasksService.changeAssignedManager(ctrl.id, ctrl.managerIds).then(function (response) {
                ctrl.needRefresh = true;
                toaster.success('', $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.ExecutorChanged', { number: ' <a  href="tasks/view/' + ctrl.id + '">№' + ctrl.id + '</a> '}));
            });
        };

        ctrl.changeObserver = function (managerId, single) {
            if (single) {
                if (ctrl.observingTask)
                    ctrl.observerIds.splice(ctrl.observerIds.indexOf(managerId), 1);
                else
                    ctrl.observerIds.push(managerId);
            }
            tasksService.changeObserver(ctrl.id, ctrl.observerIds).then(function (response) {
                ctrl.needRefresh = true;
                ctrl.observingTask = ctrl.observerIds.includes(ctrl.currentManagerId);
                toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.Task') + ' <a  href="tasks/view/' + ctrl.id + '">№' + ctrl.id + '</a> ' + $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.ObserverChanged'));
            });
        }

        ctrl.completeTask = function () {
            return tasksService.completeTaskShowModal({
                id: ctrl.id,
                name: ctrl.name,
                leadId: ctrl.leadId,
                orderId: ctrl.orderId
            })
                .then(function (result) {
                    return result;
                })
                .catch(function (result) {
                    return $q.reject(result);
                });
        };

        ctrl.acceptTask = function () {
            return tasksService.acceptTask(ctrl.id).then(function (response) {
                toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.Task') + ' <a  href="tasks/view/' + ctrl.id + '">№' + ctrl.id + '</a> ' + $translate.instant('Admin.Js.EditTask.Accepted'));
                return response;
            });
        };

        ctrl.deleteTask = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.Deleting') }).then(function (result) {
                if (result === true) {
                    tasksService.deleteTask(ctrl.id).then(function (data) {
                        if (data.result === true) {
                            ctrl.close();
                            toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.HasBeenDeleted', { number: ' №' + ctrl.id }));
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error(error);
                            });
                        }
                    });
                }
            });
        };

        ctrl.copyTask = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.AreYouSureCopy'), { title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.Copy') }).then(function (result) {
                if (result === true) {
                    tasksService.copyTask(ctrl.id).then(function (data) {
                        if (data.result === true) {
                            toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.TaskHasBeenCopied', { number: ' <a  href="tasks/view/' + ctrl.id + '">№' + ctrl.id + '</a> '}));
                            $location.search('needRefresh', 'true');
                            $location.search('modal', data.taskId);
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error(error);
                            });
                        }
                    });
                }
            });
        };

        ctrl.saveTask = function () {

            ctrl.btnSleep = true;

            return ctrl.validateTaskData().then(function (result) {

                if (!result) {
                    ctrl.btnSleep = false;
                    return;
                }

                return ctrl.validateTaskGroupManager().then(function (result) {
                    if (!result) {
                        ctrl.btnSleep = false;
                        return;
                    }

                    return $timeout(function () { // задержка нужна для ckeditor чтоб проинициализировались данные

                        var objTask = {
                            id: ctrl.id,
                            name: ctrl.name,
                            managerIds: ctrl.managerIds,
                            appointedManagerId: ctrl.appointedManagerId,
                            dueDate: ctrl.dueDate,
                            description: ctrl.description,
                            taskGroupId: ctrl.taskGroupId,
                            priority: ctrl.priority,
                            status: ctrl.status,
                            accepted: ctrl.accepted,
                            resultFull: ctrl.result,
                            remind: ctrl.remind,
                            reminder: ctrl.reminder,
                            observerIds: ctrl.observerIds
                        };

                        return tasksService.editTask(objTask)
                            .then(function (result) {
                                ctrl.showSaveMessage();
                                ctrl.btnSleep = false;
                                return result;
                            });
                    }, 300);

                });

            });
        };

        ctrl.uploadAttachment = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            ctrl.loadingFiles = true;
            if (($event.type === 'change' || $event.type === 'drop') && $files != null && $files.length > 0) {

                Upload.upload({
                    url: 'tasks/uploadAttachments',
                    data: {
                        taskId: ctrl.$resolve.id,
                    },
                    file: $files,
                }).then(function (response) {
                    var data = response.data;
                    for (var i in response.data) {
                        if (data[i].Result === true) {
                            ctrl.attachments.push(data[i].Attachment);
                            toaster.success($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.File') + data[i].Attachment.FileName + $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WasAdded'));
                        }
                        else {
                            toaster.error($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.ErrorLoading'), (data[i].Attachment != null ? data[i].Attachment.FileName + ": " : "") + data[i].Error);
                        }
                    }
                    ctrl.loadingFiles = false;
                });
            } else if ($invalidFiles.length > 0) {
                toaster.error($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.ErrorLoading'), $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.FileNotMeetRequirements'));
                ctrl.loadingFiles = false;
            }
            else {
                ctrl.loadingFiles = false;
            }
        };

        ctrl.deleteAttachment = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.AreYouSureDeleteFile'), { title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.Deleting') }).then(function (result) {
                if (result === true) {
                    tasksService.deleteAttachment(id, ctrl.$resolve.id).then(function (response) {
                        tasksService.getTaskAttachments(ctrl.$resolve.id).then(function (result) {
                            ctrl.attachments = result;
                        });
                    });
                }
            });
        };

        ctrl.flatpickrOnSetup = function (fpItem) {
            ctrl.flatpickr = fpItem;
        };

        ctrl.flatpickrOnChange = function (selectedDates, dateStr, instance) {
            if (selectedDates.length == 0)
                return;
            var date = new Date(selectedDates[0]);
            var minutes = (Math.floor(date.getMinutes() / 10) * 10);
            date.setMinutes(minutes);
            ctrl.dueDate = date.getFullYear() + '-' +
                ('0' + (date.getMonth() + 1)).slice(-2) + '-' +
                ('0' + date.getDate()).slice(-2) + 'T' +
                ('0' + date.getHours()).slice(-2) + ':' +
                ('0' + date.getMinutes()).slice(-2);
        }

        ctrl.validateTaskGroupManager = function () {
            if (ctrl.taskGroupId == null || ctrl.managerIds == null || ctrl.managerIds.length == 0) {
                return $q.resolve(true);
            }
            return tasksService.validateTaskGroupManager(ctrl.managerIds, ctrl.taskGroupId).then(function (data) {
                if (data.errors != null) {
                    data.errors.forEach(function (err) {
                        toaster.error('', err);
                    });
                    return false;
                }
                return true;
            });
        };

        ctrl.validateTaskData = function () {

            return tasksService.validateTaskData(ctrl.appointedManagerId, ctrl.taskGroupId).then(function (data) {
                if (data.errors != null) {
                    data.errors.forEach(function (err) {
                        toaster.error('', err);
                    });
                    return false;
                }
                return true;
            });
        };

        ctrl.getManagers = function () {
            tasksService.getTaskManagers(ctrl.id, ctrl.taskGroupId).then(function (result) {
                if (result != null) {
                    ctrl.managersAssign = result.managersAssign;
                    ctrl.managersAppoint = result.managersAppoint;
                    ctrl.managersObserve = result.managersObserve;

                    ctrl.canAssingToMe = ctrl.managersAssign.some(function (item) {
                        return item.value == ctrl.currentManagerId;
                    });
                }
            });
        };

        ctrl.changeLeadDealStatus = function () {
            if (ctrl.leadId == null || ctrl.leadDealStatusId == null) {
                return;
            }
            leadService.changeDealStatus(ctrl.leadId, ctrl.leadDealStatusId).then(function (data) {
                if (data.result == true) {
                    toaster.success($translate.instant('Admin.Js.Leads.TransactionStageChanged'), data.obj != null && data.obj.orderId != null
                        ? ($translate.instant('Admin.Js.CompleteTask.OrderCreated') + ' <a href="orders/edit/' + data.obj.orderId + '">' + data.obj.orderNumber + '</a>')
                        : '');
                }
                else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.chain = function () {
            return ctrl.chainProcess(arguments, 0);
        };

        ctrl.chainProcess = function (fnList, index) {
            return $q.when(fnList[index] != null ? fnList[index]() : true)
                .then(function () {
                    var newIndex = index + 1;
                    return newIndex < fnList.length ? ctrl.chainProcess(fnList, newIndex) : null;
                });
        };

        ctrl.taskOpen = function () {

            if (ckeditorObj != null) {
                ckeditorObj.setReadOnly(false);
            }

            return ctrl.changeStatus('open');
        };

        ctrl.taskInProgress = function () {
            return ctrl.changeStatus('inprogress');
        };

        ctrl.callAndSaveButtonAction = function (buttonActionName) {

            if (ctrl.buttonActions[buttonActionName] != null) {
                ctrl.buttonActions[buttonActionName].fn();
                var storageData = JSON.parse($window.localStorage.getItem(ctrl.localStorageKey)) || {};

                storageData[ctrl.status] = buttonActionName;

                $window.localStorage.setItem(ctrl.localStorageKey, JSON.stringify(storageData));
            } else {
                throw Error('Not found buttonActionName');
            }
        };

        ctrl.getButtonActionsByState = function (state, actionDefault) {
            var result = {}, buttonActionsCurrent;

            switch (state) {
                case 'open':
                    buttonActionsCurrent = ['save', 'saveAndInProgress', 'saveAndComplete'];
                    break;
                case 'inprogress':
                    buttonActionsCurrent = ['save', 'saveAndPause', 'saveAndComplete'];
                    break;
                case 'completed':
                    buttonActionsCurrent = ['save', 'saveAndAccept', 'saveAndResume'];
                    break;
            }

            if (ctrl.accepted === true) {
                buttonActionsCurrent = ['save', 'saveAndResume'];
            }

            Object.keys(ctrl.buttonActions).forEach(function (key) {
                if (buttonActionsCurrent.indexOf(key) !== -1 && key !== actionDefault) {
                    result[key] = ctrl.buttonActions[key];
                }
            });

            return result;
        };

        ctrl.getButtonActionNameDefault = function (status) {
            var result;
            var storageData = JSON.parse($window.localStorage.getItem(ctrl.localStorageKey));

            result = storageData != null ? storageData[status] || 'save' : 'save';

            return result;
        };

        ctrl.addAdminCommentsCtrl = function (adminCommentsCtrl) {
            ctrl.adminCommentsCtrl = adminCommentsCtrl;
        };

        ctrl.saveAndCheckUnsaveData = function (callbacks) {

            if (ctrl.adminCommentsCtrl.form.text != null && ctrl.adminCommentsCtrl.form.text.length > 0) {
                $timeout(function () {
                    return SweetAlert.confirm($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingComment'), { title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingTitle') })
                        .then(function (result) {
                            if (result != null && result === true) {
                                callbacks.func(callbacks.arg);
                            }
                        }, 100);
                });
            } else {
                callbacks.func(callbacks.arg);
            }


        };

        ctrl.showSaveMessage = function () {
            toaster.pop('success', '', $translate.instant('Admin.Js.Tasks.Tasks.Task') + ' <a  href="tasks/view/' + ctrl.id + '">№' + ctrl.id + '</a> ' + $translate.instant('Admin.Js.EditTask.Saved'));

            return $q.resolve();
        };
    };

    ModalEditTaskCtrl.$inject = ['$document', '$uibModalInstance', '$location', '$uibModal', 'SweetAlert', 'toaster', 'Upload', 'tasksService', '$q', '$translate', '$window', '$scope', '$timeout', 'leadService'];
    ng.module('uiModal')
        .controller('ModalEditTaskCtrl', ModalEditTaskCtrl);

})(window.angular);