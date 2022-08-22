using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Models.Dashboard;

namespace AdvantShop.Web.Admin.Handlers.Dashboard
{
    public class GetCreateSiteModel
    {
        private readonly string _mode;

        public GetCreateSiteModel(string mode)
        {
            _mode = mode;
        }

        public CreateSiteModel Execute()
        {
            var model = new CreateSiteModel()
            {
                Mode = _mode,
                Categories = new List<CreateSiteCategory>()
            };

            if (_mode == "store")
            {
                model.Categories.Add(new CreateSiteCategory()
                {
                    Name = LpSiteCategory.Store.Localize(),
                    Type = LpSiteCategory.Store
                });
            }
            else
            {
                var store = SalesChannelService.GetByType(ESalesChannelType.Store);

                foreach (LpSiteCategory category in Enum.GetValues(typeof(LpSiteCategory)))
                {
                    if (category == LpSiteCategory.Store && store.Enabled)
                        continue;

                    model.Categories.Add(new CreateSiteCategory()
                    {
                        Name = category.Localize(),
                        Type = category
                    });
                }
            }

            return model;
        }
    }
}
