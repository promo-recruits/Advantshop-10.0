﻿using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.Users
{
    public class AdminUsersFilterResult : FilterResult<AdminUserModel>
    {
        public int ManagersCount { get; set; }
    }
}
