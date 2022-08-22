using System;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Shared.Vk;
using AdvantShop.Web.Admin.Models.Crm;
using AdvantShop.Web.Admin.Models.Shared.Socials;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Vk)]
    [SalesChannel(ESalesChannelType.Vk)]
    public partial class VkController : BaseAdminController
    {
        #region Ctor

        private readonly VkApiService _vkService;

        public VkController()
        {
            _vkService = new VkApiService();
        }

        #endregion

        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Vk.Index.Title"));

            return View();
        }
        
        #region Settings 

        public JsonResult GetVkSettings()
        {
            var group = SettingsVk.Group;

            return Json(new
            {
                clientId = SettingsVk.ApplicationId,
                groups = !string.IsNullOrEmpty(SettingsVk.TokenUser) ? _vkService.GetUserGroups() : null,
                group = group,
                authByUser = SettingsVk.AuthByUser,
                salesFunnels = SalesFunnelService.GetList(),
                salesFunnelId = SettingsCrm.DefaultVkSalesFunnelId,
                createLeadFromMessages = SettingsVk.CreateLeadFromMessages,
                createLeadFromComments = SettingsVk.CreateLeadFromComments,
                syncOrdersFromVk = SettingsVk.SyncOrdersFromVk,
                groupMessageErrorStatus = SettingsVk.GroupMessageErrorStatus
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult SaveAuthVkUser(string clientId, string accessToken, string userId)
        {
            if (SettingsVk.ApplicationId.IsNullOrEmpty() && clientId.IsNotEmpty())
                Track.TrackService.TrackEvent(Track.ETrackEvent.SalesChannels_ConnectAttempt_Vk);

            SettingsVk.ApplicationId = clientId;
            SettingsVk.TokenUser = accessToken;
            SettingsVk.UserId = Convert.ToInt64(userId);

            return JsonOk();
        }
        
        public JsonResult GetVkGroups()
        {
            return Json(_vkService.GetUserGroups());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveAuthVkGroup(VkGroup group, string accessToken)
        {
            if (!VkApiService.IsVkActive() && Core.Services.Crm.SocialNetworkService.IntegrationsLimitRiched())
                return JsonError(T("Admin.SettingsCrm.SocialNetworks.LimitRiched"));

            if (group == null ||
                group.Id == 0 ||
                string.IsNullOrWhiteSpace(group.Name) ||
                string.IsNullOrWhiteSpace(accessToken))
            {
                return JsonError();
            }

            SettingsVk.Group = group;
            SettingsVk.TokenGroup = accessToken;
            SettingsVk.TokenGroupErrorCount = 0;

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Vk_GroupConnected);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteGroup()
        {
            SettingsVk.Group = null;
            SettingsVk.TokenGroup = null;
            SettingsVk.TokenUser = null;
            SettingsVk.TokenUserErrorCount = 0;
            SettingsVk.TokenGroupErrorCount = 0;
            SettingsVk.LastMessageId = null;
            SettingsVk.LastSendedMessageId = null;
            SettingsVk.IsMessagesLoaded = false;
            SettingsVk.AuthByUser = false;

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveSettings(SaveVkSettingsModel model)
        {
            SettingsCrm.DefaultVkSalesFunnelId = model.Id;
            SettingsVk.CreateLeadFromMessages = model.CreateLeadFromMessages;
            SettingsVk.CreateLeadFromComments = model.CreateLeadFromComments;
            SettingsVk.SyncOrdersFromVk = model.SyncOrdersFromVk;
            
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult AuthByLoginAndPassword(string login, string password)
        {
            var result = _vkService.TryAuthByUser((ulong)SettingsVk.ApplicationId.TryParseLong(), login, password);

            return result ? JsonOk() : JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult DeleteVkByLoginPassword()
        {
            SettingsVk.AuthByUser = false;
            SettingsVk.TokenUser = SettingsVk.TokenUserCopy;

            return JsonOk();
        }

        #endregion

        [HttpGet]
        public JsonResult GetCustomerMessages(Guid customerId)
        {
            if (!VkApiService.IsVkActive())
                return Json(null);

            return Json(VkService.GetCustomerMessages(customerId));
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendVkMessage(long userId, string message)
        {
            return ProcessJsonResult(() => _vkService.SendMessageByGroup(userId, message.Replace("\r\n", "<br>")));
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendVkMessageByCustomers(SocialSendMessageModel model)
        {
            var sendedCount = new SendVkMessage(model).Execute();
            return JsonOk(sendedCount);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendVkMessageToWall(int id, string message)
        {
            return ProcessJsonResult(() => _vkService.SendMessageToWall(id, message.Replace("\r\n", "<br>")));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddVkUser(Guid customerId,  string link)
        {
            try
            {
                var result = _vkService.AddVkUserByLink(customerId, link);
                return result ? JsonOk() : JsonError();
            }
            catch (BlException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return JsonError();
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteVkLink(Guid customerId)
        {
            _vkService.DeleteVkUser(customerId);
            return JsonOk();
        }
    }
}
