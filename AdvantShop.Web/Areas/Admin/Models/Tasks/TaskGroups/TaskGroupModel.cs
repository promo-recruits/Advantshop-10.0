using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Tasks.TaskGroups
{
    public partial class TaskGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public bool IsPrivateComments { get; set; }
        public int TasksCount { get; set; }
        public List<int> ManagerIds { get; set; }
        public List<int> ManagerRoleIds { get; set; }

        public bool CanBeDeleted
        {
            get { return TasksCount == 0; }
        }
    }
}
