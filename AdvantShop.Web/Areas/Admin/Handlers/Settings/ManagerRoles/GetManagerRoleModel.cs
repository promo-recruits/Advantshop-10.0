﻿using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Settings.ManagerRoles;

namespace AdvantShop.Web.Admin.Handlers.Settings.ManagerRoles
{
    public class GetManagerRoleModel
    {
        private readonly ManagerRole _managerRole;

        public GetManagerRoleModel(ManagerRole managerRole)
        {
            _managerRole = managerRole;
        }

        public AdminManagerRoleModel Execute()
        {
            var model = new AdminManagerRoleModel
            {
                Id = _managerRole.Id,
                Name = _managerRole.Name,
                SortOrder = _managerRole.SortOrder,
            };

            return model;
        }
    }
}
