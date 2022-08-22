using System;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{

    public static class VkProgress
    {
        private static int _total;
        private static int _current;
        private static string _errorMessage;

        public static void Start(int total)
        {
            _total = total;
            _errorMessage = null;
            _current = 0;
        }

        public static void Inc()
        {
            _current += 1;
        }

        public static void Error(string error)
        {
            _errorMessage = error;
        }

        public static Tuple<int, int, string> State()
        {
            return new Tuple<int, int, string>(_total, _current, _errorMessage);
        }
    }
}
