using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Crm.Vk.VkMarket;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;

namespace AdvantShop.Web.Admin.Models.VkMarkets
{
    public class VkCategoryModel
    {
        public int Id { get; set; }

        public long VkId { get; set; }

        public long VkCategoryId { get; set; }
        public string VkCategoryName { get; set; }

        public string Name { get; set; }

        public string Categories { get; set; }
        public List<int> CategoryIds { get; set; }

        public int SortOrder { get; set; }

        public VkCategoryModel()
        {
            
        }

        public VkCategoryModel(VkCategory vkCategory, List<VkMarketCategory> marketCategories)
        {
            Id = vkCategory.Id;
            VkId = vkCategory.VkId;
            VkCategoryId = vkCategory.VkCategoryId;
            Name = vkCategory.Name;
            SortOrder = vkCategory.SortOrder;

            if (marketCategories != null)
            {
                var marketCategory = marketCategories.Find(x => x.Id == VkCategoryId);
                if (marketCategory != null)
                    VkCategoryName = marketCategory.Name;
            }

            var cats = new VkCategoryService().GetLinkedCategories(Id);

            Categories = cats != null ? String.Join(", ", cats.Select(x => x.Name)) : null;
            CategoryIds = cats != null ? cats.Select(x => x.CategoryId).ToList() : null;
        }
    }
}
