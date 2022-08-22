using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Web.Admin.Models.Tasks
{
    // EnumIgnore - not shown in admin part
    public enum TasksPreFilterType
    {
        [Localize("Admin.Models.Tasks.TasksPreFilterType.None")]
        None = 0,
        [Localize("Admin.Models.Tasks.TasksPreFilterType.AssignedToMe")]
        AssignedToMe = 1,
        [Localize("Admin.Models.Tasks.TasksPreFilterType.AppointedByMe")]
        AppointedByMe = 2,
        [Localize("Admin.Models.Tasks.TasksPreFilterType.Completed")]
        Completed = 3,
        [Localize("Admin.Models.Tasks.TasksPreFilterType.Accepted")]
        Accepted = 4,
        
        [EnumIgnore]
        Order = 5,
        [EnumIgnore]
        Lead = 6,

        [Localize("Admin.Models.Tasks.TasksPreFilterType.ObservedByMe")]
        ObservedByMe = 7,

        [EnumIgnore]
        Customer = 8,
    }
}
