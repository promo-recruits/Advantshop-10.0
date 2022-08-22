namespace AdvantShop.Core.Common.Extensions
{
    public static class Objects
    {
        public static string AsString(this object val)
        {
            var t = val as string;
            return t ?? "";
        }

        public static float AsFloat(this object val)
        {
            if (val is float)
                return (float)val;
            return 0F;
        }

        public static float? AsNullableFloat(this object val)
        {
            if (val == null)
                return null;

            if (val is float)
                return (float)val;

            return 0F;
        }
    }
}