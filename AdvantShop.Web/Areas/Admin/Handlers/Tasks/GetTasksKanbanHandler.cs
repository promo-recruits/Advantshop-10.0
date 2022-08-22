using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Tasks;

namespace AdvantShop.Web.Admin.Handlers.Tasks
{
    public class GetTasksKanbanHandler
    {
        private readonly TasksKanbanFilterModel _filter;
        private int _currentManagerId;
        private Customer _currentCustomer;

        public GetTasksKanbanHandler(TasksKanbanFilterModel filterModel)
        {
            _filter = filterModel;
            _currentCustomer = CustomerContext.CurrentCustomer;
            var currentManager = ManagerService.GetManager(_currentCustomer.Id);
            if (currentManager != null)
                _currentManagerId = currentManager.ManagerId;
        }

        public TasksKanbanModel Execute()
        {
            var model = new TasksKanbanModel
            {
                Name = "Kanban"
            };

            if (_filter.TaskGroupId != null && !TaskService.CheckAccessByGroup(_filter.TaskGroupId.Value))
                return model;

            model.AssignedToMeTasksCount = TaskService.GetAssignedTasksCount(_currentManagerId, _filter.TaskGroupId);
            model.AppointedByMeTasksCount = TaskService.GetAppointedTasksCount(_currentManagerId, _filter.TaskGroupId);
            model.ObservedByMeTasksCount = TaskService.GetObservedTasksCount(_currentManagerId, _filter.TaskGroupId);
            if (!_filter.Columns.Any())
            {
                _filter.Columns = new List<TasksKanbanColumnFilterModel>
                {
                    new TasksKanbanColumnFilterModel(ETasksKanbanColumn.New.ToString()),
                    new TasksKanbanColumnFilterModel(ETasksKanbanColumn.InProgress.ToString()),
                    new TasksKanbanColumnFilterModel(ETasksKanbanColumn.Done.ToString()),
                    //new TasksKanbanColumnFilterModel(ETasksKanbanColumn.Accepted.ToString())
                };
            }
            foreach (var filterColumn in _filter.Columns)
            {
                var paging = GetPaging(filterColumn);
                if (paging == null)
                    continue;
                var column  = new TasksKanbanColumnModel
                {
                    Id = filterColumn.Id,
                    Name = filterColumn.Type.Localize(),
                    Page = filterColumn.Page,
                    CardsPerColumn = filterColumn.CardsPerColumn,
                    TotalCardsCount = paging.TotalRowsCount,
                    TotalPagesCount = paging.PageCount(paging.TotalRowsCount, filterColumn.CardsPerColumn),
                };
                var statusColor = filterColumn.Type.ColorValue();
                if (statusColor.IsNotEmpty())
                {
                    column.HeaderStyle.Add("color", statusColor);
                    column.CardStyle.Add("border-top-color", statusColor);
                }

                if (column.TotalPagesCount >= filterColumn.Page || filterColumn.Page == 1)
                    column.Cards = paging.PageItemsList<TaskKanbanModel>();

                model.Columns.Add(column);
            }

            return model;
        }

        public List<TaskKanbanModel> GetCards()
        {
            var result = new List<TaskKanbanModel>();
            if (_filter.ColumnId.IsNullOrEmpty() || !_filter.Columns.Any(x => x.Id == _filter.ColumnId))
                return result;

            var paging = GetPaging(_filter.Columns.FirstOrDefault(x => x.Id == _filter.ColumnId), false);

            return paging != null ? paging.PageItemsList<TaskKanbanModel>() : new List<TaskKanbanModel>();
        }

        private SqlPaging GetPaging(TasksKanbanColumnFilterModel columnFilter, bool allCards = true)
        {
            var type = columnFilter.Id.TryParseEnum<ETasksKanbanColumn>();
            if (type == ETasksKanbanColumn.None)
                return null;

            var paging = new SqlPaging()
            {
                ItemsPerPage = allCards ? columnFilter.CardsPerColumn * columnFilter.Page : columnFilter.CardsPerColumn,
                CurrentPageIndex = allCards ? 1 : columnFilter.Page
            };

            paging.Select(
                "Task.Id",
                "Task.TaskGroupId",
                "Task.Name",
                "Task.Status",
                "Task.Accepted",
                "Task.Priority",
                "Task.DueDate",
                "Task.DateAppointed",
                "Task.LeadId",
                "Task.OrderId",
                "AppointedCustomer.CustomerID".AsSqlField("AppointedCustomerId"),
                "AppointedCustomer.FirstName + ' ' + AppointedCustomer.LastName".AsSqlField("AppointedName"),
                (!_currentCustomer.IsAdmin ? "(case when IsPrivateComments = 0 then CRM.GetTaskManagersJson(Task.Id) else '' end )" : "CRM.GetTaskManagersJson(Task.Id)").AsSqlField("ManagersJson"),
                "TaskGroup.Name".AsSqlField("TaskGroupName"),
                "ViewedTask.ViewDate",
                ("(case when Task.Priority = " + (int)TaskPriority.Critical + " then 0 when Task.Priority = " + (int)TaskPriority.High + " then 1 else 2 end)").AsSqlField("PrioritySort"),
                ("(case when ViewedTask.ViewDate is not null OR TaskManager.ManagerId IS NULL then 1 else 0 end)").AsSqlField("Viewed"),

                ("(select count(AdminComment.Id) FROM CMS.AdminComment WHERE ObjId = Task.Id AND (Type = 'Task' " + (CustomerContext.CurrentCustomer.IsAdmin ? " OR Type = 'TaskHidden'" : "") + ") " +
                "AND Deleted = 0 AND AdminComment.CustomerId <> '" + _currentCustomer.Id.ToString() + "' " +
                "AND (ViewedTask.ViewDate is null OR AdminComment.DateCreated > ViewedTask.ViewDate))").AsSqlField("NewCommentsCount"),

                "AppointedCustomer.Avatar".AsSqlField("AppointedCustomerAvatar")
                );

            paging.From("[Customers].[Task]");
            paging.Left_Join("Customers.Managers as AppointedManager ON Task.AppointedManagerId = AppointedManager.ManagerId");
            paging.Left_Join("Customers.Customer as AppointedCustomer ON AppointedCustomer.CustomerID = AppointedManager.CustomerId");
            paging.Inner_Join("Customers.TaskGroup ON Task.TaskGroupId = TaskGroup.Id");
            paging.Left_Join("Customers.ViewedTask ON Task.Id = ViewedTask.TaskId AND ViewedTask.ManagerId = " + _currentManagerId.ToString());
            paging.Left_Join("Customers.TaskManager ON Task.Id = TaskManager.TaskId AND TaskManager.ManagerId = " + _currentManagerId.ToString());

            paging.Where("Task.IsDeferred = 0");
            Sorting(paging);
            Filter(paging, columnFilter);

            return paging;
        }

        private void Filter(SqlPaging paging, TasksKanbanColumnFilterModel columnFilter)
        {
            _filter.Accepted = false;

            var selectTasks = CommonHelper.GetCookieString("tasks_mykanban").TryParseEnum<ETasksKanbanViewTasks>();
            switch (selectTasks)
            {
                case ETasksKanbanViewTasks.All:
                    _filter.AppointedManagerId = _filter.AppointedManagerId != null ? _filter.AppointedManagerId : null;
                    _filter.AssignedManagerId = _filter.AssignedManagerId != null ? _filter.AssignedManagerId : null;

                    break;
                case ETasksKanbanViewTasks.AssignedToMe:
                    _filter.AssignedManagerId = _currentManagerId;
                    break;
                case ETasksKanbanViewTasks.AppointedByMe:
                    _filter.AppointedManagerId = _currentManagerId;
                    break;
                case ETasksKanbanViewTasks.ObservedByMe:
                    _filter.ObserverId = _currentManagerId;
                    break;
                default:
                    return;
            }

            switch (columnFilter.Type)
            {
                case ETasksKanbanColumn.New:
                    _filter.Status = TaskStatus.Open;
                    break;
                case ETasksKanbanColumn.InProgress:
                    _filter.Status = TaskStatus.InProgress;
                    break;
                case ETasksKanbanColumn.Done:
                    _filter.Status = TaskStatus.Completed;
                    break;
                case ETasksKanbanColumn.Accepted:
                    _filter.Accepted = true;
                    break;
                default:
                    return;
            }
            if (!string.IsNullOrWhiteSpace(_filter.Search))
            {
                if (_filter.Search.IsInt())
                    paging.Where("Task.Id = {0}", _filter.Search.TryParseInt());
                else
                    paging.Where("(Task.Name LIKE '%' + {0} + '%' OR Task.Description LIKE '%' + {0} + '%')", _filter.Search);
            }
            if (!string.IsNullOrWhiteSpace(_filter.Name))
            {
                paging.Where("Task.Name LIKE '%' + {0} + '%'", _filter.Search);
            }
            if (_filter.Status.HasValue)
            {
                paging.Where("Task.Status = {0}", _filter.Status.Value);
            }
            if (_filter.TaskGroupId.HasValue)
            {
                paging.Where("Task.TaskGroupId = {0}", _filter.TaskGroupId.Value);
            }
            if (_filter.Accepted.HasValue)
            {
                paging.Where("Task.Accepted = {0}", _filter.Accepted.Value);
            }
            if (_filter.Viewed.HasValue)
            {
                if (_filter.Viewed.Value)
                    paging.Where("(ViewedTask.ViewDate is not null OR TaskManager.ManagerId IS NULL)");
                else
                    paging.Where("ViewedTask.ViewDate is null AND TaskManager.ManagerId IS NOT NULL");
            }
            if (_filter.Priority.HasValue)
            {
                paging.Where("Task.Priority = {0}", _filter.Priority.Value);
            }
            if (_filter.AppointedManagerId.HasValue)
            {
                paging.Where("Task.AppointedManagerId = {0}", _filter.AppointedManagerId.Value);
            }
            if (_filter.AssignedManagerId.HasValue)
            {
                if (_filter.AssignedManagerId.Value == -1)
                    paging.Where("NOT EXISTS(SELECT * FROM Customers.Taskmanager WHERE TaskId = Task.Id)");
                else
                    paging.Where("EXISTS(SELECT * FROM Customers.Taskmanager WHERE TaskId = Task.Id AND ManagerId = {0})", _filter.AssignedManagerId.Value);
            }
            if (_filter.DueDateFrom.HasValue)
            {
                paging.Where("Task.DueDate >= {0}", _filter.DueDateFrom.Value);
            }
            if (_filter.DueDateTo.HasValue)
            {
                paging.Where("Task.DueDate <= {0}", _filter.DueDateTo.Value);
            }
            if (_filter.DateCreatedFrom.HasValue)
            {
                paging.Where("Task.DateAppointed >= {0}", _filter.DateCreatedFrom.Value);
            }
            if (_filter.DateCreatedTo.HasValue)
            {
                paging.Where("Task.DateAppointed <= {0}", _filter.DateCreatedTo.Value);
            }

            if (_filter.ObserverId.HasValue)
            {
                paging.Where("EXISTS(SELECT * FROM Customers.TaskObserver WHERE TaskId = Task.Id AND ManagerId = {0})", _filter.ObserverId.Value);
            }

            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersTaskConstraint == ManagersTaskConstraint.Assigned)
                    {
                        paging.Where("EXISTS(SELECT * FROM Customers.TaskManager WHERE TaskId = Task.Id AND (ManagerId = {0} OR Task.AppointedManagerId = {0}))", manager.ManagerId);
                    }
                    else if (SettingsManager.ManagersTaskConstraint == ManagersTaskConstraint.AssignedAndFree)
                    {
                        paging.Where("(EXISTS(SELECT * FROM Customers.TaskManager WHERE TaskId = Task.Id AND (ManagerId = {0} OR Task.AppointedManagerId = {0})) OR " +
                                     "NOT EXISTS(SELECT * FROM Customers.TaskManager WHERE TaskId = Task.Id))", manager.ManagerId);
                    }

                    // если у группы нет ролей или роль группы и пользователя пересекаются
                    paging.Where(
                        "(" +
                            "(Select Count(*) From [Customers].[TaskGroupManagerRole] Where TaskGroupManagerRole.TaskGroupId = Task.TaskGroupId) = 0" +
                            " OR Exists ( " +
                                "Select 1 From [Customers].[TaskGroupManagerRole] " +
                                "Where TaskGroupManagerRole.TaskGroupId = Task.TaskGroupId and TaskGroupManagerRole.ManagerRoleId in (Select ManagerRoleId From Customers.ManagerRolesMap Where ManagerRolesMap.[CustomerId] = {0}) " +
                            ")" +
                        ")",
                        customer.Id);
                }
            }
        }

        private void Sorting(SqlPaging paging)
        {
            paging.OrderBy("PrioritySort");
            paging.OrderBy("Task.SortOrder");
        }
    }
}