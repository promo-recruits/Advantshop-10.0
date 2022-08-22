using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Tasks.TaskGroups;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Tasks.TaskGroups;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Tasks
{
    [SaasFeature(ESaasProperty.HaveTasks)]
    [Auth(RoleAction.Tasks)]
    [AccessBySettings(Core.Services.Configuration.EProviderSetting.TasksActive, ETypeRedirect.AdminPanel)]
    public partial class TaskGroupsController : BaseAdminController
    {
        public JsonResult GetTaskGroups(TaskGroupsFilterModel model)
        {
            return Json(new GetTaskGroupsHandler(model).Execute());
        }

        [Auth]
        public JsonResult GetTaskGroup(int id)
        {
            var group = TaskGroupService.GetTaskGroup(id);
            if (group == null || !TaskService.CheckAccessByGroup(group.Id))
                return JsonError();

            return ProcessJsonResult(new GetTaskGroupModel(group));
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult AddTaskGroup(TaskGroupModel model)
        {
            return ProcessJsonResult(new AddEditTaskGroup(model, false));
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult UpdateTaskGroup(TaskGroupModel model)
        {
            return ProcessJsonResult(new AddEditTaskGroup(model, true));
        }

        public JsonResult GetFormData(int? taskGroupId)
        {
            var managers = ManagerService.GetManagers(RoleAction.Tasks);

            if (taskGroupId != null)
            {
                var managerIds = TaskGroupService.GetTaskGroupManagerIds(taskGroupId.Value);

                foreach (var managerId in managerIds.Where(x => !managers.Any(m => m.ManagerId == x)))
                {
                    var manager = ManagerService.GetManager(managerId);
                    if (manager != null)
                        managers.Add(manager);
                }
            }
            return Json(new
            {
                managers = managers.Select(x => new SelectItemModel(x.FullName, x.ManagerId)),
                managerRoles = ManagerRoleService.GetManagerRoles().Select(x => new SelectItemModel(x.Name, x.Id)),
            });
        }

        #region Inplace

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult InplaceTaskGroup(TaskGroupModel model)
        {
            if (model.Id != 0)
            {
                var taskGroup = TaskGroupService.GetTaskGroup(model.Id);
                if (taskGroup == null)
                    return Json(new { result = false });
                taskGroup.SortOrder = model.SortOrder;
                taskGroup.Enabled = model.Enabled;
                TaskGroupService.UpdateTaskGroup(taskGroup);
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult DeleteTaskGroup(int id)
        {
            TaskGroupService.DeleteTaskGroup(id);
            return JsonOk();
        }

        #endregion

        #region Commands

        private void Command(TaskGroupsFilterModel command, Func<int, TaskGroupsFilterModel, bool> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    func(id, command);
                }
            }
            else
            {
                var handler = new GetTaskGroupsHandler(command);
                var ids = handler.GetItemsIds("Id");

                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult DeleteTaskGroups(TaskGroupsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                TaskGroupService.DeleteTaskGroup(id);
                return true;
            });
            return Json(true);
        }

        #endregion

        public JsonResult GetTaskGroupsSelectOptions()
        {
            var taskGroups = TaskGroupService.GetAllTaskGroups();
            return Json(taskGroups.Select(x => new SelectItemModel(x.Name, x.Id.ToString())));
        }
    }
}
