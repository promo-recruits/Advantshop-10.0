﻿using System.Linq;
using AdvantShop.Core.Services.Crm.Vk.VkMarket;
using AdvantShop.Web.Admin.Models.VkMarkets;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.VkMarkets
{
    public class GetVkCategories
    {
        private readonly VkCategoryService _vkCategoryService = new VkCategoryService();
        private readonly VkMarketApiService _vkMarketApiService = new VkMarketApiService();


        public FilterResult<VkCategoryModel> Execute()
        {
            var marketCategories = _vkMarketApiService.GetMarketCategories();

            var categories =
                _vkCategoryService.GetList().Select(x => new VkCategoryModel(x, marketCategories)).ToList();
            

            var model = new FilterResult<VkCategoryModel>
            {
                TotalItemsCount = categories.Count,
                TotalPageCount = 1,
                DataItems = categories,
                TotalString = "Всего: " + categories.Count
            };

            return model;
        }
    }
}
