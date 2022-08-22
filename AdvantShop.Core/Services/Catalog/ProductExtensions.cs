using AdvantShop.Catalog;

namespace AdvantShop.Core.Services.Catalog
{
    public static class ProductExtensions
    {
        public static float GetWeight(this Offer offer)
        {
            return offer.Weight ?? 0;
        }

        public static float GetLength(this Offer offer)
        {
            return offer.Length ?? 0;
        }


        public static float GetWidth(this Offer offer)
        {
            return offer.Width ?? 0;
        }

        public static float GetHeight(this Offer offer)
        {
            return offer.Height ?? 0;
        }

        public static string GetDimensions(this Offer offer, string separator = "x")
        {
            return offer.GetLength() + separator + offer.GetWidth() + separator + offer.GetHeight();
        }
    }
}
