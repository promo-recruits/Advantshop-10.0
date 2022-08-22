using AdvantShop.Core.Services.Crm;
using AdvantShop.Web.Admin.Models.Tasks.TaskGroups;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Tasks.TaskGroups
{
    public class GetTaskGroupModel : AbstractCommandHandler<TaskGroupModel>
    {
        private readonly TaskGroup _taskGroup;

        public GetTaskGroupModel(TaskGroup taskGroup)
        {
            _taskGroup = taskGroup;
        }

        protected override TaskGroupModel Handle()
        {
            var model = new TaskGroupModel
            {
                Id = _taskGroup.Id,
                Name = _taskGroup.Name,
                SortOrder = _taskGroup.SortOrder,
                Enabled = _taskGroup.Enabled,
                IsPrivateComments = _taskGroup.IsPrivateComments,
                ManagerIds = TaskGroupService.GetTaskGroupManagerIds(_taskGroup.Id),
                ManagerRoleIds = TaskGroupService.GetTaskGroupManagerRoleIds(_taskGroup.Id),
            };

            return model;
        }
    }
}
