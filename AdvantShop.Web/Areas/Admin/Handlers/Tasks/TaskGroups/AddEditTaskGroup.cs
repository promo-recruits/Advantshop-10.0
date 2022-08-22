using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Web.Admin.Models.Tasks.TaskGroups;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Tasks.TaskGroups
{
    public class AddEditTaskGroup : AbstractCommandHandler<int>
    {
        private readonly TaskGroupModel _model;
        private bool _editMode;

        public AddEditTaskGroup(TaskGroupModel model, bool editMode)
        {
            _model = model;
            _editMode = editMode;
        }

        protected override void Validate()
        {
            if (_model.Name.IsNullOrEmpty())
                throw new BlException("Укажите название");

            if (_editMode && !TaskService.CheckAccessByGroup(_model.Id))
                throw new BlException("Нет доступа");
        }

        protected override int Handle()
        {
            var taskGroup = new TaskGroup
            {
                Name = _model.Name,
                SortOrder = _model.SortOrder,
                Enabled = _model.Enabled,
                IsPrivateComments = _model.IsPrivateComments
            };

            if (_editMode)
            {
                taskGroup.Id = _model.Id;
                TaskGroupService.UpdateTaskGroup(taskGroup);
                TaskGroupService.ClearTaskGroupManagers(taskGroup.Id);
                TaskGroupService.ClearTaskGroupManagerRoles(taskGroup.Id);
            }
            else
            {
                taskGroup.Id = TaskGroupService.AddTaskGroup(taskGroup);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Tasks_TaskProjectCreated);
            }
            if (_model.ManagerIds != null)
            {
                foreach (var managerId in _model.ManagerIds)
                    TaskGroupService.AddTaskGroupManager(taskGroup.Id, managerId);
            }
            if (_model.ManagerRoleIds != null)
            {
                foreach (var managerRoleId in _model.ManagerRoleIds)
                    TaskGroupService.AddTaskGroupManagerRole(taskGroup.Id, managerRoleId);
            }

            return taskGroup.Id;
        }
    }
}
