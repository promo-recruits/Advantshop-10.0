using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Saas;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Attachments;
using AdvantShop.Web.Admin.Handlers.Shared.AdminNotifications;
using AdvantShop.Web.Admin.Handlers.Tasks;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Attachments;
using AdvantShop.Web.Admin.Models.Tasks;
using AdvantShop.Web.Admin.ViewModels.Tasks;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Tasks
{
    [SaasFeature(ESaasProperty.HaveTasks)]
    [Auth(RoleAction.Tasks)]
    [AccessBySettings(Core.Services.Configuration.EProviderSetting.TasksActive, ETypeRedirect.AdminPanel)]
    public partial class TasksController : BaseAdminController
    {
        private const string LastProjectCookieName = "tasks_lastProject";

        private TasksListViewModel GetTasksListViewModel(TasksFilterModel filter)
        {
            var model = new GetIndexModel(filter).Execute();

            SetMetaInformation(T("Admin.Tasks.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.TasksCtrl);

            // временно отключаем
            //if (!BizProcessRuleService.ExistBizProcessRules())
            //    ShowNotification(NotifyType.Notice, T("Admin.Tasks.BizProcessesNotSet"));

            Track.TrackService.TrackEvent(Track.ETrackEvent.Trial_VisitTasks);

            return model;
        }

        public ActionResult Index(TasksFilterModel filter)
        {
            CommonHelper.DeleteCookie(LastProjectCookieName);

            var model = GetTasksListViewModel(filter);

            if (model.TaskGroupId != null && !TaskService.CheckAccessByGroup(model.TaskGroupId.Value))
            {
                ShowMessage(NotifyType.Error, T("Admin.Tasks.Project.NoAccess"));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Project(TasksFilterModel filter)
        {
            CommonHelper.SetCookie(LastProjectCookieName, filter.TaskGroupId.ToString(), new TimeSpan(365, 0, 0, 0), false);

            var model = GetTasksListViewModel(filter);

            if (model.TaskGroupId != null && !TaskService.CheckAccessByGroup(model.TaskGroupId.Value))
            {
                ShowMessage(NotifyType.Error, T("Admin.Tasks.Project.NoAccess"));
                return RedirectToAction("Index");
            }

            return View("Index", model);
        }

        public ActionResult LastProject(TasksFilterModel filter)
        {
            var taskGroupId = CommonHelper.GetCookieString(LastProjectCookieName).TryParseInt(true);
            return taskGroupId.HasValue
                ? RedirectToAction("Project", new { taskGroupId = taskGroupId })
                : RedirectToAction("Index");
        }

        public ActionResult View(int id)
        {
            return Redirect(Url.RouteUrl(new { controller = "Tasks", action = "Index" }) + "#?modal=" + id);
        }

        public JsonResult GetTasks(TasksFilterModel model)
        {
            return Json(new GetTasksHandler(model).Execute());
        }

        #region Kanban

        public JsonResult GetKanban(TasksKanbanFilterModel model)
        {
            return Json(new GetTasksKanbanHandler(model).Execute());
        }

        public JsonResult GetKanbanColumn(TasksKanbanFilterModel model)
        {
            return Json(new GetTasksKanbanHandler(model).GetCards());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeSorting(int id, int? prevId, int? nextId)
        {
            var result = new ChangeTaskSorting(id, prevId, nextId).Execute();
            return Json(new { result });
        }

        #endregion

        public JsonResult GetTask(int id)
        {
            var manager = ManagerService.GetManager(CustomerContext.CustomerId);

            var task = TaskService.GetTask(id, manager == null ? (int?)null : manager.ManagerId);
            if (task == null || task.IsDeferred)
                return JsonError();

            if (!TaskService.CheckAccess(task))
                return JsonError(T("Admin.Tasks.GetTask.NoAccess"));

            var result = new GetTaskModel(task).Execute();

            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddTask(TaskModel model)
        {
            var manager = ManagerService.GetManager(CustomerContext.CustomerId);
            if (manager == null)
                return JsonError(T("Admin.Tasks.AddTask.NoManager"));

            model.AppointedManagerId = manager.ManagerId;
            model.DateAppointed = DateTime.Now;
            var task = new AddTaskHandler(model).Execute();
            if (task == null)
                return JsonError();

            new UploadAttachmentsHandler(task.Id).Execute<TaskAttachment>();

            new TaskAddedNotificationsHandler(task).Execute();

            return Json(new { result = true, id = task.Id });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult EditTask(TaskModel model)
        {
            var manager = ManagerService.GetManager(CustomerContext.CustomerId);
            if (manager == null)
                return JsonError();

            var result = new EditTaskHandler(model).Execute();

            return Json(new { result });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeTaskStatus(int id, TaskStatus status)
        {
            var task = TaskService.GetTask(id);
            var model = (TaskModel)task;
            if (task.Accepted && status != TaskStatus.Completed)
            {
                model.Accepted = false;
                model.ResultFull = string.Empty;
                model.ResultShort = string.Empty;
            }

            model.Status = status;
            var result = new EditTaskHandler(model).Execute();

            return Json(new { result, status = model.StatusString });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeAssignedManager(int id, List<int> managerIds)
        {
            var result = new EditTaskHandler().ChangeAssignedManagers(id, managerIds);
            return Json(new {result});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeObserver(int id, List<int> observerIds)
        {
            var result = new EditTaskHandler().ChangeObservers(id, observerIds);
            return Json(new {result});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CompleteTask(int id, string taskResult, int? orderStatusId, int? dealStatusId)
        {
            var task = TaskService.GetTask(id);
            var model = (TaskModel)task;
            model.Status = TaskStatus.Completed;
            model.ResultFull = taskResult;

            var result = new EditTaskHandler(model).Execute();
            if (result)
            {
                if (task.Order != null && orderStatusId.HasValue)
                {
                    if (task.Order.OrderStatusId != orderStatusId.Value)
                    {
                        OrderStatusService.ChangeOrderStatus(task.OrderId.Value, orderStatusId.Value,
                            LocalizationService.GetResourceFormat("Admin.Tasks.CompleteTask.OrderStatusBasis", task.Id,
                                taskResult.Default("-")));
                        TrialService.TrackEvent(TrialEvents.ChangeOrderStatus, "");
                    }
                }

                if (task.Lead != null && dealStatusId.HasValue && task.Lead.DealStatusId != dealStatusId.Value)
                {
                    var salesFunnel = SalesFunnelService.Get(task.Lead.SalesFunnelId);
                    var dealStatus = DealStatusService.Get(dealStatusId.Value);
                    if (salesFunnel != null && dealStatus != null)
                    {
                        if (dealStatus.Status == SalesFunnelStatusType.FinalSuccess && salesFunnel.FinalSuccessAction == SalesFunnelFinalSuccessAction.CreateOrder)
                        {
                            var order = OrderService.CreateOrder(task.Lead);
                            return order == null ? JsonError() : JsonOk(new { orderId = order.OrderID, orderNumber = order.Number });
                        }

                        task.Lead.DealStatusId = dealStatusId.Value;
                        LeadService.UpdateLead(task.Lead, false);
                    }
                }
            }

            return result ? JsonOk() : JsonError();
        }

        public JsonResult GetDealStatuses(int leadId)
        {
            var lead = LeadService.GetLead(leadId);
            if (lead == null)
                return JsonError();
            return Json(DealStatusService.GetList(lead.SalesFunnelId).Select(x => new SelectItemModel(x.Name, x.Id, x.Id == lead.DealStatusId)));
        }


        public JsonResult GetOrderStatuses(int orderId)
        {
            var order = OrderService.GetOrder(orderId);
            var statuses = OrderStatusService.GetOrderStatuses();

            return Json(statuses.Select(x => new SelectItemModel(x.StatusName, x.StatusID.ToString(), x.StatusID == order.OrderStatusId)));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AcceptTask(int id)
        {
            var task = TaskService.GetTask(id);
            var model = (TaskModel)task;
            model.Status = TaskStatus.Completed;
            model.Accepted = true;
            var result = new EditTaskHandler(model).Execute();

            return Json(new { result });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CopyTask(int id)
        {
            return Json(new CopyTask(id).Execute());
        }

        #region Inplace

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult DeleteTask(int id)
        {
            var modifier = CustomerContext.CurrentCustomer;
            var task = TaskService.GetTask(id);

            var customersToNotify = new List<Customer>();
            foreach (var manager in task.Managers)
                customersToNotify.Add(manager.Customer);
            if (task.AppointedManager != null)
                customersToNotify.Add(task.AppointedManager.Customer);

            new AdminNotificationsHandler().NotifyCustomers(new OnTaskDeletedNotification(task, modifier),
                customersToNotify.Where(x => x != null && x.Id != modifier.Id && x.HasRoleAction(RoleAction.Tasks)).Select(x => x.Id).ToArray());

            TaskService.DeleteTask(id);

            return JsonOk();
        }

        #endregion

        #region Commands

        private void Command(TasksCommand command, Func<int, TasksCommand, bool> func)
        {
            var currentManager = ManagerService.GetManager(CustomerContext.CustomerId);
            if (currentManager == null)
                return;

            command.ManagerId = currentManager.ManagerId;
            command.Customer = CustomerContext.CurrentCustomer;

            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    func(id, command);
                }
            }
            else
            {
                var handler = new GetTasksHandler(command);
                var ids = handler.GetItemsIds("Task.Id");

                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult DeleteTasks(TasksCommand command)
        {
            Command(command, (id, c) =>
            {
                //if (!TaskService.CanDeleteTask(id, c.ManagerId))
                //    return false;
                TaskService.DeleteTask(id);
                return true;
            });
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeTaskStatuses(TasksCommand command, TaskStatus status)
        {
            Command(command, (id, c) =>
            {
                var task = TaskService.GetTask(id);
                if (task == null || !TaskService.CheckAccess(task))
                    return false;
                if (task.Status != status)
                {
                    TaskService.ChangeTaskStatus(id, status);
                    var prev = task.DeepClone();
                    task.Status = status;
                    TaskService.OnTaskChanged(c.Customer, prev, task);
                }
                return true;
            });
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AcceptTasks(TasksCommand command)
        {
            Command(command, (id, c) =>
            {
                TaskService.SetTaskAccepted(id);
                return true;
            });
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult MarkViewed(TasksCommand command)
        {
            Command(command, (id, c) =>
            {
                TaskService.SetTaskViewed(id, c.ManagerId);
                return true;
            });
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult MarkNotViewed(TasksCommand command)
        {
            Command(command, (id, c) =>
            {
                TaskService.SetTaskViewed(id, c.ManagerId, false);
                return true;
            });
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeTaskGroup(TasksCommand command, int selectedGroupId)
        {
            var realGroup = TaskGroupService.GetTaskGroup(selectedGroupId);
            if (realGroup == null)
                return Json(false);

            Command(command, (id, c) =>
            {
                var task = TaskService.GetTask(id);
                if (task == null || !TaskService.CheckAccess(task))
                    return false;
                if(task.TaskGroupId != realGroup.Id && !task.Accepted)
                {
                    task.TaskGroupId = realGroup.Id;
                    TaskService.UpdateTask(task);
                }
                return true;
            });
            return Json(true);
        }

        #endregion

        #region Attachments

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ValidateAttachments()
        {
            var handler = new UploadAttachmentsHandler(null);
            var result = handler.Validate<TaskAttachment>();

            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadAttachments(int taskId)
        {
            var task = TaskService.GetTask(taskId);
            var prevState = task.DeepClone();

            var handler = new UploadAttachmentsHandler(taskId);
            var result = handler.Execute<TaskAttachment>();
            foreach(var item in result)
            {
                if(item.Result)
                    TasksHistoryService.TrackTaskAddAttachment(taskId, item.Attachment.FileName, null);
            }
            TaskService.OnTaskChanged(CustomerContext.CurrentCustomer, prevState, task);
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteAttachment(int id, int taskId)
        {
            var task = TaskService.GetTask(taskId);
            var prevState = task.DeepClone();
            var attachment = AttachmentService.GetAttachment<TaskAttachment>(id);

            var result = AttachmentService.DeleteAttachment<TaskAttachment>(id);
            if (result)
            {
                TasksHistoryService.TrackTaskRemoveAttachment(taskId, attachment.FileName, null);
            }
            TaskService.OnTaskChanged(CustomerContext.CurrentCustomer, prevState, task);
            return Json(new { result });
        }

        public JsonResult GetTaskAttachments(int id)
        {
            return Json(AttachmentService.GetAttachments<TaskAttachment>(id)
                .Select(x => new AttachmentModel
                {
                    Id = x.Id,
                    ObjId = x.ObjId,
                    FileName = x.FileName,
                    FilePath = x.Path,
                    FilePathAdmin = x.PathAdmin,
                    FileSize = x.FileSizeFormatted
                })
            );
        }

        #endregion

        public JsonResult GetTaskPrioritiesSelectOptions()
        {
            return Json(Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>()
                .Select(x => new SelectItemModel(x.Localize(), x.ConvertIntString())));
        }

        public JsonResult GetTaskStatusesSelectOptions()
        {
            return Json(Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>()
                .Select(x => new SelectItemModel(x.Localize(), x.ConvertIntString())));
        }

        public JsonResult GetNotCompletedTaskStatusesSelectOptions()
        {
            return Json(Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>()
                .Where(x => x != TaskStatus.Completed)
                .Select(x => new SelectItemModel(x.Localize(), x.ConvertIntString())));
        }

        //private List<Manager> GetTaskManagersList(Task task, int? taskGroupId = null)
        //{
        //    var managers = ManagerService.GetManagers(RoleAction.Tasks);

        //    if (task != null)
        //    {
        //        managers = managers.Where(x =>
        //                    task.ManagerIds.Contains(x.ManagerId) ||
        //                    TaskService.CheckAccessByGroup(x.Customer, task.TaskGroupId)).ToList();

        //        // Если выключены исполнители или постановщик, то добавляем их в список
        //        foreach (var taskManagerId in task.ManagerIds.Where(x => !managers.Any(m => m.ManagerId == x)))
        //        {
        //            var m = ManagerService.GetManager(taskManagerId);
        //            if (m != null)
        //                managers.Add(m);
        //        }

        //        if (task.AppointedManagerId != null && !managers.Any(x => x.ManagerId == task.AppointedManagerId.Value))
        //        {
        //            var m = ManagerService.GetManager(task.AppointedManagerId.Value);
        //            if (m != null)
        //                managers.Add(m);
        //        }
        //    }
        //    else if (taskGroupId != null)
        //        managers = managers.Where(x => TaskService.CheckAccessByGroup(x.Customer, taskGroupId.Value)).ToList();

        //    return managers;
        //}

        public JsonResult GetTaskFormData(int? id = null, int? taskGroupId = null)
        {
            var groups = TaskGroupService.GetAllTaskGroups();
            var currentManager = ManagerService.GetManager(CustomerContext.CustomerId);

            Task task = null;

            if (id != null)
            {
                task = TaskService.GetTask(id.Value);
                if (task != null && !groups.Any(x => x.Id == task.TaskGroupId))
                    groups.Insert(0, task.TaskGroup);
            }

            var managersAssign = ManagerService.GetAllTaskManagers(taskGroupId: task != null ? task.TaskGroupId : taskGroupId, taskId: id, assigned: id.HasValue);
            var managersAppoint = ManagerService.GetAllTaskManagers(taskGroupId: task != null ? task.TaskGroupId : taskGroupId, taskId: id, appointed: id.HasValue);
            var managersObserve = ManagerService.GetAllTaskManagers(taskGroupId: task != null ? task.TaskGroupId : taskGroupId, taskId: id, observed: id.HasValue);

            return Json(new
            {
                currentManagerId = currentManager != null ? currentManager.ManagerId : (int?)null,
                managersAssign = managersAssign.Select(x => new SelectItemModel(x.FullName, x.ManagerId)),
                managersAppoint = managersAppoint.Select(x => new SelectItemModel(x.FullName, x.ManagerId)),
                managersObserve = managersObserve.Select(x => new SelectItemModel(x.FullName, x.ManagerId)),
                taskGroups = groups.Select(x => new SelectItemModel(x.Name, x.Id)),
                priorities = Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>().Select(x => new SelectItemModel(x.Localize(), x.ConvertIntString())),
                //defaultTaskGroupId = SettingsTasks.DefaultTaskGroup == 0 ? (int?)null : SettingsTasks.DefaultTaskGroup,
                filesHelpText = FileHelpers.GetFilesHelpText(EAdvantShopFileTypes.TaskAttachment),
                leadDealStatuses = task != null && task.Lead != null ? DealStatusService.GetList(task.Lead.SalesFunnelId).Select(x => new SelectItemModel(x.Name, x.Id)) : null,
                reminderTypes = Enum.GetValues(typeof(TaskReminder)).Cast<TaskReminder>().Where(x => x != TaskReminder.NotRemind).Select(x => new SelectItemModel(x.Localize(), x.ConvertIntString())),
                reminderActive = SettingsTasks.ReminderActive
            });
        }

        [HttpGet]
        public JsonResult GetManagers(int? id = null, int? taskGroupId = null)
        {
            //var task = id != null ? TaskService.GetTask(id.Value) : null;
            //var managers = GetTaskManagersList(task, taskGroupId).Select(x => new SelectItemModel(x.FullName, x.ManagerId));

            var managersAssign = ManagerService.GetAllTaskManagers(taskGroupId: taskGroupId, taskId: id, assigned: id.HasValue);
            var managersAppoint = ManagerService.GetAllTaskManagers(taskGroupId: taskGroupId, taskId: id, appointed: id.HasValue);
            var managersObserve = ManagerService.GetAllTaskManagers(taskGroupId: taskGroupId, taskId: id, observed: id.HasValue);

            return Json(new
            {
                managersAssign = managersAssign.Select(x => new SelectItemModel(x.FullName, x.ManagerId)),
                managersAppoint = managersAppoint.Select(x => new SelectItemModel(x.FullName, x.ManagerId)),
                managersObserve = managersObserve.Select(x => new SelectItemModel(x.FullName, x.ManagerId))
            });
        }

        public JsonResult ValidateTaskGroupManager(List<int> managerIds, int taskGroupId)
        {
            var taskGroup = TaskGroupService.GetTaskGroup(taskGroupId);
            if (taskGroup == null)
                return JsonError();

            var errors = new List<string>();

            foreach (var managerId in managerIds)
            {
                var manager = ManagerService.GetManager(managerId);
                if (manager == null)
                {
                    errors.Add("Менеджер не найден");
                    continue;
                }

                if (!TaskService.CheckAccessByGroup(manager.Customer, taskGroupId))
                    errors.Add(string.Format("Менеджер {0} не имеет доступа к проекту \"{1}\"", manager.FullName, taskGroup.Name));
            }

            return errors.Count > 0 ? JsonError(errors.ToArray()) : JsonOk();
        }

        public JsonResult ValidateTaskGroupManagerByRoles(List<int> managerIds, List<int> managerRoleIds, List<int> participantIds)
        {
            var errors = new List<string>();

            foreach (var managerId in managerIds)
            {
                var manager = ManagerService.GetManager(managerId);
                if (manager == null)
                {
                    errors.Add("Менеджер не найден");
                    continue;
                }

                if (!TaskService.CheckAccessByGroup(manager.Customer, managerRoleIds, participantIds))
                    errors.Add($"Менеджер {manager.FullName} не соответствует выбранным ролям или не включен в список участников. Необходимо назначить менеджеру роль, или изменить роли участников, или добавить его в качестве участника.");
            }

            return errors.Count > 0 ? JsonError(errors.ToArray()) : JsonOk();
        }

        public JsonResult ValidateTaskData(int? appointedManagerId, int? taskGroupId)
        {
            var errors = new List<string>();

            if (taskGroupId != null && appointedManagerId != null)
            {
                var manager = ManagerService.GetManager(appointedManagerId.Value);
                if (!TaskService.CheckAccessByGroup(manager.Customer, taskGroupId.Value))
                    errors.Add(string.Format("У менеджера {0} нет прав на выбранный проект.", manager.FullName));
            }

            return errors.Count > 0 ? JsonError(errors.ToArray()) : JsonOk();
        }

        [ChildActionOnly]
        public ActionResult NavMenu()
        {
            var currentManager = ManagerService.GetManager(CustomerContext.CustomerId);
            var groups = TaskGroupService.GetRecentTaskGroups(8, currentManager != null ? currentManager.ManagerId : (int?)null);

            return PartialView(groups);
        }
    }
}
