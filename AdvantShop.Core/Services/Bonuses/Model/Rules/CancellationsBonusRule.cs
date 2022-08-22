using System;
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
    public class CancellationsBonusRule : BaseRule
    {
        public int SmsDayBefore { get; set; }
        public int AgeCard { get; set; }
        public bool NotSendSms { get; set; }

        public override void Process(IJobExecutionContext context)
        {
            var bdrule = CustomRuleService.Get(ERule.CancellationsBonus);
            if (bdrule == null || !bdrule.Enabled) return;
            ProcessNotification(bdrule);
            ProcessCard(bdrule);
        }

        private void ProcessNotification(CustomRule rule)
        {
            var cancellationsBonusRule = BaseRule.Get(rule) as CancellationsBonusRule;
            if (cancellationsBonusRule == null) return;

            var today = DateTime.Today;
            var startDay = today.AddMonths(-cancellationsBonusRule.AgeCard).AddDays(cancellationsBonusRule.SmsDayBefore);
            var endDay = today.AddMonths(-cancellationsBonusRule.AgeCard).AddDays(cancellationsBonusRule.SmsDayBefore).AddDays(1);

            var result = SQLDataAccess2.ExecuteReadIEnumerable<CancellationsBonusRuleDto>(
                "select card.CardId,customer.StandardPhone from Bonus.Card card " +
                "inner join Customers.Customer customer on card.CardId=customer.CustomerID " +
                "where " +
                "((card.DateLastWipeBonus is null  and card.CreateOn < @endDay) or (card.DateLastWipeBonus is not null and card.DateLastWipeBonus < @endDay)) " +
                "and (card.DateLastNotifyBonusWipe is null or card.DateLastNotifyBonusWipe < @endDay)",
                new {today, startDay, endDay, rule = ERule.CancellationsBonus}).ToList();
            var companyname = SettingsMain.ShopName;

            foreach (var item in result)
            {
                SendAndLog(item.CardId, item.StandardPhone, cancellationsBonusRule.SmsDayBefore, cancellationsBonusRule.NotSendSms, companyname);
            }
        }

        private void SendAndLog(Guid cardId, long cellPhone, int dayLeft, bool notSendSms, string companyname)
        {
            var card = CardService.Get(cardId);
            var balance = card.BonusAmount + AdditionBonusService.ActualSum(cardId);
            if (balance <= 0) return;

            var r = new RuleLog
            {
                CardId = card.CardId,
                RuleType = ERule.CancellationsBonus,
                Created = DateTime.Today
            };
            RuleLogService.Add(r);

            card.DateLastNotifyBonusWipe = DateTime.Now;

            if (!notSendSms)
            {
                SmsService.Process(cellPhone, ESmsType.OnCancellationsBonus,
                    new OnCancellationsBonusTempalte
                    {
                        CompanyName = companyname,
                        Balance = balance,
                        DayLeft = dayLeft
                    });
            }
            CardService.Update(card);

        }

        private void ProcessCard(CustomRule rule)
        {
            var cancellationsBonusRule = BaseRule.Get(rule) as CancellationsBonusRule;
            if (cancellationsBonusRule == null) return;
            var today = DateTime.Today;
            var startDay = today.AddMonths(-cancellationsBonusRule.AgeCard);
            var endDay = today.AddMonths(-cancellationsBonusRule.AgeCard).AddDays(1);

            var result = SQLDataAccess2.ExecuteReadIEnumerable<Guid>(
                    "select card.CardId from Bonus.Card card where " +
                    "((card.DateLastWipeBonus is null  and card.CreateOn < @endDay) or (card.DateLastWipeBonus is not null and card.DateLastWipeBonus < @endDay))",
                    new {today, startDay, endDay, rule = ERule.CancellationsBonus})
                .ToList();

            foreach (var item in result)
            {
                ClearBonus(item, rule.Name);
            }
        }

        private void ClearBonus(Guid cardId, string basis)
        {
            var card = CardService.Get(cardId);
            var tmpSum = AdditionBonusService.ActualSum(cardId);
            var balance = card.BonusAmount + tmpSum;
            if (balance <= 0) return;

            var transLog = Transaction.Factory(card.CardId, card.BonusAmount, basis, EOperationType.SubtractMainBonus, 0);
            TransactionService.Create(transLog);
            card.BonusAmount = 0;
            card.DateLastWipeBonus = DateTime.Now;

            foreach (var item in AdditionBonusService.Actual(cardId))
            {
                tmpSum = tmpSum - item.Amount;
                transLog = Transaction.Factory(card.CardId, item.Amount, basis, EOperationType.SubtractMainBonus, tmpSum);
                TransactionService.Create(transLog);
                item.Status = EAdditionBonusStatus.Remove;
                AdditionBonusService.Update(item);
            }
            CardService.Update(card);
        }

        private class CancellationsBonusRuleDto
        {
            public Guid CardId { get; set; }
            public long StandardPhone { get; set; }
        }
    }
}