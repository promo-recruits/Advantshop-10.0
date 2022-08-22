using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AdvantShop.Core.Common
{
    public enum RetryMode
    {
        AllError,
        LastError
    }

    public static class RetryHelper
    {

        /// <summary>
        /// Repeat action ned times before throw exception
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryInterval">in seconds</param>
        /// <param name="retryCount">count of repeats</param>
        public static void Do(Action action, int retryInterval = 1, int retryCount = 5, RetryMode mode = RetryMode.AllError, Action actionOnFail = null)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, retryCount, mode, actionOnFail);
        }

        /// <summary>
        /// Repeat action ned times before throw exception
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryInterval">in seconds</param>
        /// <param name="retryCount">count of repeats</param>
        public static T Do<T>(Func<T> action, int retryInterval = 1, int retryCount = 5, RetryMode mode = RetryMode.AllError, Action actionOnFail = null)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                        Thread.Sleep(retryInterval * 1000);
                    return action();
                }
                catch (Exception ex)
                {
                    if (mode == RetryMode.LastError && retry == (retryCount - 1))
                    {
                        throw;
                    }
                    exceptions.Add(ex);
                }
                if (actionOnFail != null)
                {
                    try
                    {
                        actionOnFail();
                    }
                    catch (Exception) { }
                }
            }
            throw new AggregateException(exceptions);
        }

        public static async Task DoAsync(Func<Task> task, int retryInterval = 1, int retryCount = 3, RetryMode mode = RetryMode.AllError)
        {
            var exceptions = new List<Exception>();
            for (int attempted = 0; attempted < retryCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        await Task.Delay(retryInterval * 1000);
                    }

                    await task();
                    return;
                }
                catch (Exception ex)
                {
                    if (mode == RetryMode.LastError && attempted == (retryCount - 1))
                    {
                        throw;
                    }
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }

        public static async Task<T> DoAsync<T>(Func<Task<T>> task, int retryInterval = 1, int retryCount = 3, RetryMode mode = RetryMode.AllError)
        {
            var exceptions = new List<Exception>();
            for (int attempted = 0; attempted < retryCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        await Task.Delay(retryInterval * 1000);
                    }
                    return await task();
                }
                catch (Exception ex)
                {
                    if (mode == RetryMode.LastError && attempted == (retryCount - 1))
                    {
                        throw;
                    }
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }
    }
}
