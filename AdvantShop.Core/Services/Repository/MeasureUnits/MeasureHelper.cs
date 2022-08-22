//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Repository
{
    public class Measure
    {
        public float[] XYZ { get; set; }
        public float Amount { get; set; }
    }

    public class MeasureHelper
    {
        public static float[] GetDimensions(PreOrder preOrder, List<PreOrderItem> items, 
                                        float defaultHeight = 0, float defaultWidth = 0, float defaultLength = 0, 
                                        float rate = 1)
        {
            if (preOrder.TotalLength != null &&
                preOrder.TotalWidth != null &&
                preOrder.TotalHeight != null)
            {
                return new float[3] {
                    (preOrder.TotalLength.Value == 0 ? defaultLength : preOrder.TotalLength.Value) / rate,
                    (preOrder.TotalWidth.Value == 0 ? defaultWidth : preOrder.TotalWidth.Value) / rate,
                    (preOrder.TotalHeight.Value == 0 ? defaultHeight : preOrder.TotalHeight.Value) / rate
                };
            }

            var dimensions = items.Select(item => new Measure
            {
                XYZ = new[]
                {
                    (item.Length == 0 ? defaultLength : item.Length) / rate,
                    (item.Width == 0 ? defaultWidth : item.Width) / rate,
                    (item.Height == 0 ? defaultHeight : item.Height) / rate,
                },
                Amount = item.Amount
            }).ToList();

            return items.Count == 1 && items[0].Amount == 1 ? dimensions[0].XYZ : GetDimensions(dimensions);
        }

        private static float[] GetDimensions(List<Measure> dimensions)
        {
            var result = new float[3];

            foreach (var item in dimensions)
            {
                item.XYZ = item.XYZ.OrderByDescending(x => x).ToArray();
            }

            foreach (var dim in dimensions)
            {
                if (dim.XYZ[0] >= result[0])
                    result[0] = dim.XYZ[0];

                if (dim.XYZ[1] >= result[1])
                    result[1] = dim.XYZ[1];

                result[2] += dim.XYZ[2] * dim.Amount;
            }
            return result;
        }


        public static float[] GetDimensions(Order order, 
                                            float defaultHeight = 0, float defaultWidth = 0, float defaultLength = 0, 
                                            float rate = 1)
        {
            var preOrder = new PreOrder()
            {
                TotalWeight = order.TotalWeight,
                TotalLength = order.TotalLength,
                TotalWidth = order.TotalWidth,
                TotalHeight = order.TotalHeight
            };
            var items = order.OrderItems.Select(x => new PreOrderItem(x)).ToList();

            return GetDimensions(preOrder, items, defaultHeight, defaultWidth, defaultLength, rate);
        }

        public static float[] GetDimensions(Order order, List<Measure> dimensions)
        {
            if (order.TotalLength != null &&
                order.TotalWidth != null &&
                order.TotalHeight != null)
            {
                return new float[3] { order.TotalLength.Value, order.TotalWidth.Value, order.TotalHeight.Value };
            }

            return GetDimensions(dimensions);
        }

        public static float GetTotalWeight(Order order, List<OrderItem> orderItems, float defaultWeight = 0, float rate = 1)
        {
            var weight = order.TotalWeight != null
                ? order.TotalWeight.Value * rate
                : orderItems.Sum(x => (x.Weight == 0 ? defaultWeight : x.Weight) * rate * x.Amount);

            return weight;
        }

        public static float ConvertUnitToNewAmount(float oldUnit, float oldAmount, float newAmount)
            => oldUnit * oldAmount / newAmount;
    }
}