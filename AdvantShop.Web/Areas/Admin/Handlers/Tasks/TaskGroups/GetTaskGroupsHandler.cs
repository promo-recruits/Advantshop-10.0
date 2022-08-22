﻿using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Tasks.TaskGroups;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Tasks.TaskGroups
{
    public class GetTaskGroupsHandler
    {
        private readonly TaskGroupsFilterModel _filterModel;
        private SqlPaging _paging;

        public GetTaskGroupsHandler(TaskGroupsFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<TaskGroupModel> Execute()
        {
            var model = new FilterResult<TaskGroupModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.TaskGroups.Grid.TotalString", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<TaskGroupModel>();
            
            return model;
        }

        public List<int> GetItemsIds(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<int>(fieldName);
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "Id",
                "Name",
                "SortOrder",
                "Enabled",
                "(select count(Id) FROM Customers.Task WHERE TaskGroupId = TaskGroup.Id)".AsSqlField("TasksCount")
                );

            _paging.From("[Customers].[TaskGroup]");

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filterModel.Search))
            {
                _paging.Where("Name LIKE '%' + {0} + '%'", _filterModel.Search);
            }
            if (_filterModel.SortingFrom.HasValue)
            {
                _paging.Where("SortOrder >= {0}", _filterModel.SortingFrom.Value);
            }
            if (_filterModel.SortingTo.HasValue)
            {
                _paging.Where("SortOrder <= {0}", _filterModel.SortingTo.Value);
            }

            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsModerator)
            {
                // если у группы нет ролей или роль группы и пользователя пересекаются
                _paging.Where(
                    "(" +
                        "(Select Count(*) From [Customers].[TaskGroupManagerRole] Where TaskGroupManagerRole.TaskGroupId = TaskGroup.Id) = 0" +
                        " OR Exists ( " +
                            "Select 1 From [Customers].[TaskGroupManagerRole] " +
                            "Where TaskGroupManagerRole.TaskGroupId = TaskGroup.Id and TaskGroupManagerRole.ManagerRoleId in (Select ManagerRoleId From Customers.ManagerRolesMap Where ManagerRolesMap.[CustomerId] = {0}) " +
                        ")" +
                    ")",
                    customer.Id);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy("SortOrder");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower();

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sorting);
                }
                else
                {
                    _paging.OrderByDesc(sorting);
                }
            }
        }
    }
}