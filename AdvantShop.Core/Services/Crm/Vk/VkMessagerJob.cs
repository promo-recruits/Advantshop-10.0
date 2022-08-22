using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using Quartz;
using VkNet.Exception;

namespace AdvantShop.Core.Services.Crm.Vk
{
    [DisallowConcurrentExecution]
    public class VkMessagerJob : IJob
    {
        private readonly VkApiService _vkService = new VkApiService();

        public void Execute(IJobExecutionContext context)
        {
            if (VkMessagerJobState.IsRun)
            {
                context.LogWarning("VkMessagerJobState.IsRun is still true");
                return;
            }

            try
            {
                VkMessagerJobState.IsRun = true;

                _vkService.GetLastMessagesByApi();
            }
            catch (BlException ex)
            {
                Debug.Log.Error(ex);
                context.LogError(ex.Message);
                StopJob();
                ShowNotification();
            }
            catch (UserAuthorizationFailException ex)
            {
                Debug.Log.Error(ex);
                context.LogError(ex.Message);
                StopJob(true);
                ShowNotification();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                context.LogError(ex.Message);
            }
            finally
            {
                VkMessagerJobState.IsRun = false;
            }
        }

        private void StopJob(bool cleanSettings = false)
        {
            if (cleanSettings)
            {
                SettingsVk.TokenGroup = null;
                SettingsVk.Group = null;
            }
            TaskManager.TaskManagerInstance().RemoveTask(nameof(VkMessagerJob), TaskManager.ModuleGroup);
        }

        private void ShowNotification()
        {
            var customers = CustomerService.GetCustomersbyRole(Role.Administrator);
            foreach (var customer in customers)
            {
                AdminInformerService.Add(new AdminInformer(AdminInformerType.Error, 0, null)
                {
                    Title = "Ошибка авторизации в интеграции с ВКонтакте. Переподключитесь к группе.",
                    PrivateCustomerId = customer.Id
                });
            }
        }
    }
}
