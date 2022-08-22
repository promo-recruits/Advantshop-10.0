﻿using System;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Bonuses.Sms;
using AdvantShop.Core.Services.Bonuses.Sms.Template;
using AdvantShop.Core.SQL;
using Quartz;

namespace AdvantShop.Core.Services.Bonuses.Model.Rules
{
    [DisallowConcurrentExecution]
    public class CleanExpiredBonusRule : BaseRule
    {
        public int DayBefore { get; set; }
        public bool NeedSms { get; set; }

        public override void Process(IJobExecutionContext context)
        {
            var bdrule = CustomRuleService.Get(ERule.CleanExpiredBonus);
            if (bdrule == null || !bdrule.Enabled)
                return;

            var rule = BaseRule.Get(bdrule) as CleanExpiredBonusRule;
            if (rule == null)
                return;

            DayBefore = rule.DayBefore;
            NeedSms = rule.NeedSms;
            SmsNotification();
            ProcessBonuses();
        }

        public void SmsNotification()
        {
            if (!NeedSms) return;
            var curDate = DateTime.Today.AddDays(DayBefore);
            var result =
                SQLDataAccess2.ExecuteReadIEnumerable<CleanExpiredBonusRuleDto>(
                    "select ad.Id as BonusId, card.CardId, card.BonusAmount, customer.StandardPhone " +
                    "from Bonus.AdditionBonus ad " +
                    "inner join Bonus.card card on card.CardId=ad.CardId " +
                    "inner join Customers.Customer customer on card.CardId=customer.CustomerID " +
                    "where EndDate is not null and EndDate <=  @date and Status <> @status " +
                    "and (ad.NotifiedAboutExpiry is null or ad.NotifiedAboutExpiry <> 1)",
                    new {date = curDate, status = EAdditionBonusStatus.Remove, rule = ERule.CleanExpiredBonus })
                    .ToList();

            var companyname = SettingsMain.ShopName;
            foreach (var item in result)
            {
                var tmpSum = AdditionBonusService.ActualSum(item.CardId);
                RuleLogService.Add(new RuleLog
                {
                    CardId = item.CardId,
                    RuleType = ERule.CleanExpiredBonus,
                    Created = DateTime.Today
                });

                var balance = item.BonusAmount + tmpSum;

                SmsService.Process(item.StandardPhone, ESmsType.OnCleanExpiredBonus,
                    new OnCleanExpiredBonusTempalte
                    {
                        CompanyName = companyname,
                        Balance = balance,
                        DayLeft = DayBefore
                    });

                var additionBonus = AdditionBonusService.Get(item.BonusId);
                additionBonus.NotifiedAboutExpiry = true;
                AdditionBonusService.Update(additionBonus);
            }
        }

        public void ProcessBonuses()
        {
            var curDate = DateTime.Today;
            var result =
                SQLDataAccess2.ExecuteReadIEnumerable<CleanExpiredBonusRuleDto>(
                    "select ad.Id as BonusId, card.CardId, card.BonusAmount, customer.StandardPhone " +
                    "from Bonus.AdditionBonus ad " +
                    "inner join Bonus.card card on card.CardId=ad.CardId " +
                    "inner join Customers.Customer customer on card.CardId=customer.CustomerID " +
                    "where EndDate is not null and EndDate <=  @date and Status <> @status ",
                    new {date = curDate, status = EAdditionBonusStatus.Remove, rule = ERule.CleanExpiredBonus })
                    .ToList();

            foreach (var item in result)
            {
                var tmpSum = AdditionBonusService.ActualSum(item.CardId);
                var bonus = AdditionBonusService.Get(item.BonusId);
                bonus.Status = EAdditionBonusStatus.Remove;
                tmpSum -= bonus.Amount;
                var transLog = Transaction.Factory(item.CardId, bonus.Amount, "удаление истекшего", EOperationType.SubtractAdditionBonus, tmpSum);
                TransactionService.Create(transLog);
                AdditionBonusService.Update(bonus);
            }
        }

        private class CleanExpiredBonusRuleDto
        {
            public int BonusId { get; set; }
            public Guid CardId { get; set; }
            public long StandardPhone { get; set; }
            public decimal BonusAmount { get; set; }
        }
    }
}
