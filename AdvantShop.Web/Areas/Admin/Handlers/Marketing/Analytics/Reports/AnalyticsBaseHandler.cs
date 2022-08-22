﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class AnalyticsBaseHandler
    {
        private readonly float _currencyValue = CurrencyService.CurrentCurrency.Rate;

        protected EGroupDateBy Filter(string groupFormatString)
        {
            if (groupFormatString == null)
                groupFormatString = "dd";

            switch (groupFormatString)
            {
                case "dd":
                    return EGroupDateBy.Day;
                case "wk":
                    return EGroupDateBy.Week;
                case "mm":
                    return EGroupDateBy.Month;
                default:
                    return EGroupDateBy.Day;
            }
        }

        protected Dictionary<DateTime, float> GetByDays(Dictionary<DateTime, float> list, DateTime startDate, DateTime endDate)
        {
            IDictionary<DateTime, float> sortedList =
                list.Count > 0
                    ? (IDictionary<DateTime, float>)new SortedDictionary<DateTime, float>(list) // performance
                    : list;
            var resultList = new Dictionary<DateTime, float>();

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            while (startDate <= endDate)
            {
                resultList.Add(startDate, sortedList.ContainsKey(startDate) ? sortedList[startDate] / _currencyValue : 0f);

                startDate = startDate.AddDays(1);
            }

            return resultList;
        }

        protected Dictionary<DateTime, float> GetByWeeks(Dictionary<DateTime, float> list, DateTime startDate, DateTime endDate)
        {
            var resultList = new Dictionary<DateTime, float>();

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            var nextDate = startDate;
            var prevDate = DateTime.MinValue;

            while (nextDate <= endDate)
            {
                var value = list.Where(x => x.Key > prevDate && x.Key <= nextDate).Sum(x => x.Value);
                resultList.Add(nextDate, (value / _currencyValue));

                prevDate = nextDate;
                nextDate = nextDate.DayOfWeek != 0
                            ? nextDate.AddDays(7 - (int)nextDate.DayOfWeek + 1)
                            : nextDate.AddDays(7);
            }

            if (prevDate != endDate)
            {
                var lastValue = list.Where(x => x.Key > prevDate && x.Key <= endDate).Sum(x => x.Value);
                resultList.Add(endDate, lastValue / _currencyValue);
            }

            return resultList;
        }

        protected Dictionary<DateTime, float> GetByMonths(Dictionary<DateTime, float> list, DateTime startDate, DateTime endDate)
        {
            var resultList = new Dictionary<DateTime, float>();

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            var nextDate = startDate;
            var prevDate = DateTime.MinValue;

            while (nextDate <= endDate)
            {
                var value = list.Where(x => x.Key > prevDate && x.Key <= nextDate).Sum(x => x.Value);
                resultList.Add(nextDate, value / _currencyValue);

                prevDate = nextDate;
                nextDate = nextDate.AddMonths(1);
                if (nextDate.Day != 1)
                    nextDate = new DateTime(nextDate.Year, nextDate.Month, 1);
            }

            if (prevDate != endDate)
            {
                var lastValue = list.Where(x => x.Key > prevDate && x.Key <= endDate).Sum(x => x.Value);
                resultList.Add(endDate, lastValue / _currencyValue);
            }

            return resultList;
        }
    }
}
