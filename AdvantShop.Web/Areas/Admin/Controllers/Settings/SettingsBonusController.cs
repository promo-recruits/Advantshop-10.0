﻿using System.Web.Mvc;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.BonusSystem)]
    [SaasFeature(Saas.ESaasProperty.BonusSystem)]
    [SalesChannel(ESalesChannelType.Bonus)]
    public class SettingsBonusController : BaseAdminController
    {
        public ActionResult Index()
        {
            var model = new BonusSettingsModel()
            {
                IsEnabled = BonusSystem.IsEnabled,
                BonusGradeId = BonusSystem.DefaultGrade,
                CardNumFrom = BonusSystem.CardFrom,
                CardNumTo = BonusSystem.CardTo,
                SmsEnabled = BonusSystem.SmsEnabled,
                MaxOrderPercent = BonusSystem.MaxOrderPercent,
                BonusType = BonusSystem.BonusType,
                BonusTextBlock = BonusSystem.BonusTextBlock,
                BonusRightTextBlock = BonusSystem.BonusRightTextBlock,
                ForbidOnCoupon = BonusSystem.ForbidOnCoupon
            };

            var item = model.Grades.Find(x => x.Value == model.BonusGradeId.ToString());
            if (item != null)
                item.Selected = true;

            SetMetaInformation(T("Admin.Settings.Bonus.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsBonusCtrl);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(BonusSettingsModel model)
        {
            if (!ModelState.IsValid)
            {
                ShowErrorMessages();
            }
            else
            {
                BonusSystem.IsEnabled = model.IsEnabled;
                BonusSystem.DefaultGrade = model.BonusGradeId;
                BonusSystem.CardFrom = model.CardNumFrom;
                BonusSystem.CardTo = model.CardNumTo;
                BonusSystem.SmsEnabled = model.SmsEnabled;
                BonusSystem.MaxOrderPercent = model.MaxOrderPercent;
                BonusSystem.BonusType = model.BonusType;
                BonusSystem.BonusTextBlock = model.BonusTextBlock;
                BonusSystem.BonusRightTextBlock = model.BonusRightTextBlock;
                BonusSystem.ForbidOnCoupon = model.ForbidOnCoupon;

                ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
            }

            return Index();
        }
    }
}
