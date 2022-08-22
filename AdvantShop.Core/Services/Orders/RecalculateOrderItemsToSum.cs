using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Orders
{
    public class RecalculateOrderItemsToSum
    {
        private readonly IList<OrderItem> _sourceItems;

        /// <summary>
        /// Допустимое отклонение
        /// </summary>
        public float AcceptableDifference { get; set; }

        /// <summary>
        /// Не менять кол-во у позиций
        /// </summary>
        public bool NoChangeAmount { get; set; }

        /// <summary>
        /// Не разделять позиции на части
        /// </summary>
        public bool NotSeparate { get; set; }

        /// <summary>
        /// Округление до какого значения
        /// </summary>
        public float? RoundNumbers { get; set; }

        public RecalculateOrderItemsToSum(IEnumerable<OrderItem> items)
        {
            AcceptableDifference = 0f;

            _sourceItems = items == null
                ? new List<OrderItem>()
                : items as IList<OrderItem> ?? items.ToList();
        }

        /// <exception cref="ArgumentException"></exception>
        public IList<OrderItem> ToSum(float sum)
        {
            return ToSum(sum, out _);
        }

        /// <exception cref="ArgumentException"></exception>
        public IList<OrderItem> ToSum(float sum, out float difference)
        {
            if (!_ValidatePrice(sum))
                throw new ArgumentException(nameof(sum),
                    $"Не корректные входные данные: параметр {nameof(sum)}={sum} не соответствует указанной точности {RoundNumbers}.");
            if (_sourceItems == null || _sourceItems.Count == 0)
            {
                difference = 0f;
                return _sourceItems;
            }
            
            if (_sourceItems.Any(x => !_ValidatePrice(x.Price)))
                throw new ArgumentException(nameof(_sourceItems),
                    $"Не корректные входные данные: цены позиций не соответствует указанной точности {RoundNumbers}.");

            var resultItem = 
                _sourceItems
                    .DeepClone();// изменения не должны касаться исходных данных

            // частный случай приведения к 0
            if (sum == 0f)
                resultItem = (IList<OrderItem>)resultItem.ResetPriceToZero();

            if (!IsAcceptableDifference(resultItem, sum))
                // без изменения кол-ва
                _ToSum(sum, resultItem);

            if (!NoChangeAmount &&
                !IsAcceptableDifference(resultItem, sum) && 
                _sourceItems.Any(item => item.Amount % 1 > 0))
            {
                // предыдущие распределения не сработали
                // округляем кол-во до целых и пробуем снова

                resultItem = (IList<OrderItem>)_sourceItems
                    .DeepClone()// изменения не должны касаться исходных данных
                    .CeilingAmountToInteger();

                _ToSum(sum, resultItem);
            }

            if (!NoChangeAmount &&
                !IsAcceptableDifference(resultItem, sum))
            {
                // предыдущие распределения не сработали
                // приводим кол-во к единице и пробуем снова

                resultItem = (IList<OrderItem>)_sourceItems
                    .DeepClone()// изменения не должны касаться исходных данных
                    .SetAmountToOne();

                _ToSum(sum, resultItem);
            }

            difference = GetDifference(resultItem, sum);
            return resultItem;
        }

        private float GetDifference(IList<OrderItem> orderItems, float sum)
        {
            return (float)Math.Round(sum - orderItems.Sum(x => (float)Math.Round(x.Amount * x.Price, 2)), 2);
        }

        private bool IsAcceptableDifference(IList<OrderItem> items, float sum)
            => Math.Abs(GetDifference(items, sum)) <= AcceptableDifference;


        private void _ToSum(float sum, IList<OrderItem> items)
        {
            _DivideDifference(GetDifference(items, sum), items);

            if (!IsAcceptableDifference(items, sum))
                if (sum > 0f) // метода не сработает, если нужно привести к отрицательной сумме, поэтому не вызываем
                    _SetDifferenceToItem(GetDifference(items, sum), items, sum);
        }

        /// <summary>
        /// Раскидываем разницу по позициям
        /// </summary>
        /// <param name="difference">разница</param>
        /// <param name="items">позиции</param>
        private void _DivideDifference(float difference, IList<OrderItem> items)
        {
            var productsTotal = items.Sum(x => (float)Math.Round(x.Amount * x.Price, 2));

            foreach (var item in items)
            {    
                if (RoundNumbers.HasValue)
                    item.Price = (float)(Math.Round((item.Price + item.Price / productsTotal * difference) / RoundNumbers.Value, 0, MidpointRounding.AwayFromZero) * Math.Round(RoundNumbers.Value, 2));
                else
                    item.Price = (float)Math.Round(item.Price + item.Price / productsTotal * difference, 2);
            }
        }

        /// <summary>
        /// Разницу скидываем на одну позицию
        /// </summary>
        /// <param name="difference">разница</param>
        /// <param name="items">позиции</param>
        /// <param name="sum">сумма к которой необходимо привести</param>
        private void _SetDifferenceToItem(float difference, IList<OrderItem> items, float sum)
        {
            foreach (var item in items
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                // в приоритет ставим элементы с кол-вом равным 1
                .OrderByDescending(x => x.Amount == 1f)
                // далее по приоритету с целым кол-вом
                .ThenByDescending(x => x.Amount % 1 == 0))
            {
                var newPrice = RoundNumbers.HasValue
                    ? (float)(Math.Round((item.Price + difference / item.Amount) / RoundNumbers.Value, 0, MidpointRounding.AwayFromZero) * Math.Round(RoundNumbers.Value, 2))
                    : (float)Math.Round(item.Price + difference / item.Amount, 2);
                if (newPrice > 0f)
                {
                    item.Price = newPrice;
                    break;
                }
            }

            // разделяем одну позицию на две, чтобы скинуть на нее разницу
            if (!NotSeparate && !IsAcceptableDifference(items, sum))
            {
                difference = GetDifference(items, sum);

                var absDifference = Math.Abs(difference);
                var itemsForSeparate = items
                    // берем впервую очередь с большей ценой
                    .OrderByDescending(x => x.Price)
                    // с большим кол-вом (стараясь не разделять 1.5 на 1 и 0.5)
                    .ThenByDescending(x => x.Amount)
                    // только с кол-вом больше 1
                    .Where(x => x.Amount > 1f)
                    // чтобы цена не ушла в минус
                    .Where(x => x.Price > absDifference)
                    .ToList();

                if (itemsForSeparate.Count > 0)
                {
                    var currentItem = 0;
                    do
                    {
                        var separateItem = itemsForSeparate[currentItem];
                        var pristineAmount = separateItem.Amount; // чтобы не потерять точность при мат.операциях
                        var newItem = separateItem.DeepClone();
                        newItem.Amount = 1f;
                        separateItem.Amount -= newItem.Amount;

                        var newPrice = RoundNumbers.HasValue
                            ? (float)(Math.Round((newItem.Price + difference) / RoundNumbers.Value, 0, MidpointRounding.AwayFromZero) * Math.Round(RoundNumbers.Value, 2))
                            : (float)Math.Round(newItem.Price + difference, 2);
                        if (newPrice > 0f)
                            newItem.Price = newPrice;
                        
                        int indexSeparateItem = int.MinValue;
                        for (var index = 0; index < items.Count; index++)
                        {
                            if (ReferenceEquals(items[index], separateItem))
                            {
                                indexSeparateItem = index;
                                break;
                            }
                        }
                        items.Insert(indexSeparateItem + 1, newItem);

                        if (!IsAcceptableDifference(items, sum))
                        {
                            // откатываем
                            separateItem.Amount = pristineAmount;
                            items.RemoveAt(indexSeparateItem + 1);
                        }

                        currentItem++;
                    } while (currentItem < itemsForSeparate.Count && !IsAcceptableDifference(items, sum));
                }
            }
        }

        /// <summary>
        /// Валидирует, что цена соответствует указанной точности
        /// </summary>
        /// <param name="value">значение</param>
        /// <returns></returns>
        private bool _ValidatePrice(float value)
        {
            if (!RoundNumbers.HasValue)
                return true;

            var invariantCulture = System.Globalization.CultureInfo.InvariantCulture;
            var valueStr = value.ToString(invariantCulture);
            var RoundNumbersStr = RoundNumbers.Value.ToString(invariantCulture);
            var decimalSeparator = invariantCulture.NumberFormat.NumberDecimalSeparator;

		
            var indexValueStr = valueStr.IndexOf(decimalSeparator);
            var indexRoundNumbersStr = RoundNumbersStr.IndexOf(decimalSeparator);

            return indexRoundNumbersStr < 0 
                ? value % RoundNumbers.Value == 0
                : indexValueStr < 0 || valueStr.Substring(indexValueStr).Length <= RoundNumbersStr.Substring(indexRoundNumbersStr).Length;
        }
    }
}
