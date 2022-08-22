﻿namespace AdvantShop.Core.Services.Crm.Facebook
{
    public class FacebookJobState
    {
        private static readonly object SyncObject = new object();

        private static bool _isRun = false;
        public static bool IsRun
        {
            get
            {
                lock (SyncObject)
                {
                    return _isRun;
                }
            }
            set
            {
                lock (SyncObject)
                {
                    _isRun = value;
                }
            }
        }
    }
}
