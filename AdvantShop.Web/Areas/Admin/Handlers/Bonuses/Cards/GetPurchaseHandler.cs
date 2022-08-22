﻿using System;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Bonuses.Cards;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Bonuses.Cards
{
    public class GetPurchaseHandler : AbstractHandler<PurcharseFilterModel, int, Purchase>
    {
        public GetPurchaseHandler(PurcharseFilterModel filterModel) : base(filterModel)
        {
        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select("Id", "OrderId", "CreateOn", "PurchaseAmount",
                          "PurchaseFullAmount", "CashAmount", "MainBonusAmount",
                          "AdditionBonusAmount", "NewBonusAmount", "Comment", "Status");
            paging.From("[Bonus].[Purchase]");
            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            if (FilterModel.CardId!= Guid.Empty)
            {
                paging.Where("CardId = {0}", FilterModel.CardId);
            }
            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            paging.OrderByDesc("CreateOn");
            return paging;
        }
    }
}
