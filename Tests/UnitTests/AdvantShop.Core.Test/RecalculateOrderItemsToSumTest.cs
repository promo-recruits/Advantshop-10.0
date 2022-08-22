using System;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Test
{
    [TestFixture]
    public class RecalculateOrderItemsToSumTest
    {
        [Test]
        public void Test_NotNullReferenceException()
        {
            //Arrange
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(null);

            //Act
            var items = recalculateOrderItemsToSum.ToSum(0);

            //Assert
            Assert.NotNull(items);
            Assert.IsFalse(items.Any());
        }

        [Test]
        public void Test_PositiveSum()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 100, Amount = 1},
                new OrderItem { Price = 100, Amount = 1},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10);

            //Assert
            Assert.Greater(recalculateOrderItems.Sum(x => x.Price * x.Amount), 0);
        }

        [Test]
        public void Test_ZeroSum()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 100, Amount = 1},
                new OrderItem { Price = 100, Amount = 1},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0);

            //Assert
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Price * x.Amount), 0f);
        }

        [Test]
        public void Test_NegativeSum()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 100, Amount = 1},
                new OrderItem { Price = 100, Amount = 1},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(-10);

            //Assert
            Assert.Less(recalculateOrderItems.Sum(x => x.Price * x.Amount), 0);
        }

        [Test]
        public void Test_AcceptableDifferenceZero()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 30, Amount = 3},
                new OrderItem { Price = 100, Amount = 2},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(290-50);

            //Assert
            var difference = Math.Abs(recalculateOrderItems.Sum(x => x.Price * x.Amount) - 240);
            Assert.AreEqual(difference, 0);
        }

        [Test]
        public void Test_AcceptableDifferenceOneCent()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0.01f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100);

            //Assert
            var difference = Math.Abs(recalculateOrderItems.Sum(x => x.Price * x.Amount) - 100);
            Assert.LessOrEqual(difference, recalculateOrderItemsToSum.AcceptableDifference);
            Assert.GreaterOrEqual(difference, 0);
        }

        [Test]
        public void Test_PriceHaveNotChangeIfDifferenceIsZero()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120);

            //Assert
            Assert.AreEqual(recalculateOrderItems.ElementAt(0).Price, 40);
        }

        [Test]
        public void Test_SourceNotChange()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120);

            //Assert
            Assert.IsFalse(Equals(sourceOrderItems, recalculateOrderItems));

        }

        [Test]
        public void Test_NoChangeAmount()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.NoChangeAmount = true;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100);

            //Assert
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));

        }

        /// <summary>
        /// Тест на раскидывание наценки, которую можно сделать только с разделением позиции
        /// </summary>
        [Test]
        public void Test_SeparateItems()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 167, Amount = 60},
                new OrderItem { Price = 367, Amount = 60},
                new OrderItem { Price = 359, Amount = 30},
                new OrderItem { Price = 130, Amount = 6},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;
            recalculateOrderItemsToSum.NotSeparate = false;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(45_770);

            //Assert
            var difference = Math.Abs(recalculateOrderItems.Sum(x => x.Price * x.Amount) - 45_770);
            Assert.AreEqual(difference, 0);
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count + 1);
        }

        /// <summary>
        /// Тест на раскидыванием наценки, которую можно сделать только с разделением позиции (больше никак на 100%)
        /// </summary>
        [Test]
        public void Test_ToSumOnlyWithSeparateItems()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.NoChangeAmount = true;
            recalculateOrderItemsToSum.AcceptableDifference = 0f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100);

            //Assert
            var difference = Math.Abs(recalculateOrderItems.Sum(x => x.Price * x.Amount) - 100);
            Assert.AreEqual(difference, 0);
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count + 1);
        }

        /// <summary>
        /// Тест на неудачу раскидывания наценки без функции разделения позиции
        /// </summary>
        [Test]
        public void Test_FailWithoutSeparateItems()
        {
            // если тест падает из-за того что удается раскидать скидку из-за новых реализаций алгоритма, нужно 
            // отключить эти функции в блоке настроек RecalculateOrderItemsToSum
            
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 167, Amount = 60},
                new OrderItem { Price = 367, Amount = 60},
                new OrderItem { Price = 359, Amount = 30},
                new OrderItem { Price = 130, Amount = 6},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            // настраиваем
            recalculateOrderItemsToSum.AcceptableDifference = 0f;
            recalculateOrderItemsToSum.NotSeparate = true;
            recalculateOrderItemsToSum.NoChangeAmount = true;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(45_770);

            //Assert
            var difference = Math.Abs(recalculateOrderItems.Sum(x => x.Price * x.Amount) - 45_770);
            Assert.AreNotEqual(difference, 0);
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count);
        }

        /// <summary>
        /// Тест2 на неудачу раскидывания наценки без функции разделения позиции (больше никак не раскидать на 100%)
        /// </summary>
        [Test]
        public void Test_FailWithoutSeparateItems2()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;
            recalculateOrderItemsToSum.NotSeparate = true;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100);

            //Assert
            var difference = Math.Abs(recalculateOrderItems.Sum(x => x.Price * x.Amount) - 100);
            Assert.AreEqual(difference, 0);
            Assert.AreNotEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count);
        }
        
        /// <summary>
        /// Распределение наценки с округлением
        /// <remarks>
        /// https://task.advant.shop/adminv3/tasks#?modal=27983
        /// </remarks>
        /// </summary>
        [Test]
        public void Test_Round()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 167, Amount = 60},
                new OrderItem { Price = 367, Amount = 60},
                new OrderItem { Price = 359, Amount = 30},
                new OrderItem { Price = 130, Amount = 6},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;
            recalculateOrderItemsToSum.RoundNumbers = 1f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(45_770);

            //Assert
            // как цена позиции округляется в заказе, так и сумма позиции также округляется
            Assert.AreEqual(recalculateOrderItems.Sum(x => Math.Round(Math.Round(x.Price, MidpointRounding.AwayFromZero) * x.Amount, MidpointRounding.AwayFromZero)), 45_770);
            Assert.IsTrue(recalculateOrderItems.All(x => x.Price % 1 == 0));
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count + 1);
        }

        /// <summary>
        /// Тест валидации входных цен
        /// </summary>
        [Test]
        public void Test_ValidatePrice()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { Price = 0f, Amount = 60},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            
            //Act, Assert

            recalculateOrderItemsToSum.RoundNumbers = null;
            
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.01f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f);
            sourceOrderItems[0].Price = 10f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f);
            
            
            
            recalculateOrderItemsToSum.RoundNumbers = 0.01f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            sourceOrderItems[0].Price = 0.11f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f);
            sourceOrderItems[0].Price = 10f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f);
            
            
            
            recalculateOrderItemsToSum.RoundNumbers = 0.1f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f); });
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            sourceOrderItems[0].Price = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f); });
            sourceOrderItems[0].Price = 10f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f);
            
            
            
            recalculateOrderItemsToSum.RoundNumbers = 1f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f); });
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            sourceOrderItems[0].Price = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f); });
            sourceOrderItems[0].Price = 10f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f);
            sourceOrderItems[0].Price = 120f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120f);
            sourceOrderItems[0].Price = 1001f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(1001f);
            sourceOrderItems[0].Price = 200f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(200f);
            sourceOrderItems[0].Price = 11000f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(11000f);
         
            
            
            recalculateOrderItemsToSum.RoundNumbers = 10f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f); });
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            sourceOrderItems[0].Price = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f); });
            sourceOrderItems[0].Price = 10f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f);
            sourceOrderItems[0].Price = 120f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120f);
            sourceOrderItems[0].Price = 1001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(1001f); });
            sourceOrderItems[0].Price = 200f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(200f);
            sourceOrderItems[0].Price = 11000f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(11000f);
        
            
            
            recalculateOrderItemsToSum.RoundNumbers = 100f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f); });
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            sourceOrderItems[0].Price = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f); });
            sourceOrderItems[0].Price = 10f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f); });
            sourceOrderItems[0].Price = 120f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120f); });
            sourceOrderItems[0].Price = 1001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(1001f); });
            sourceOrderItems[0].Price = 200f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(200f);
            sourceOrderItems[0].Price = 11000f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(11000f);

            
            
            recalculateOrderItemsToSum.RoundNumbers = 1000f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f); });
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            sourceOrderItems[0].Price = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f); });
            sourceOrderItems[0].Price = 10f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f); });
            sourceOrderItems[0].Price = 100f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100f); });
            sourceOrderItems[0].Price = 120f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120f); });
            sourceOrderItems[0].Price = 1001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(1001f); });
            sourceOrderItems[0].Price = 11000f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(11000f);
        }
    }
}
