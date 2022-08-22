using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Localization;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Marketing.Coupons;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Marketing.Coupons;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Marketing
{
    [Auth(RoleAction.Settings)]
    public partial class CouponsController : BaseAdminController
    {
        #region Coupons

        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Coupons.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.CouponsCtrl);

            return View();
        }

        public JsonResult GetCoupons(CouponsFilterModel model)
        {
            return Json(new GetCoupons(model).Execute());
        }

        #region Commands

        private void Command(CouponsFilterModel model, Func<int, CouponsFilterModel, bool> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                {
                    func(id, model);
                }
            }
            else
            {
                var handler = new GetCoupons(model);
                var ids = handler.GetItemsIds();

                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCoupons(CouponsFilterModel model)
        {
            Command(model, (id, c) =>
            {
                CouponService.DeleteCoupon(id);
                return true;
            });
            return Json(true);
        }

        #endregion

        #endregion

        #region Coupon

        [HttpGet]
        public JsonResult GetCouponData()
        {
            var data = new
            {
                types = Enum.GetValues(typeof(CouponType)).Cast<CouponType>().Select(x => new
                {
                    label = x.Localize(),
                    value = (int)x,
                }),
                currencies = CurrencyService.GetAllCurrencies(true).Select(x => new
                {
                    label = x.Name,
                    value = x.Iso3,
                }),
                dateNow = Culture.ConvertShortDate(DateTime.Now)
            };

            return Json(data);
        }

        public JsonResult GetTypes()
        {
            return
                Json(
                    Enum.GetValues(typeof(CouponType))
                        .Cast<CouponType>()
                        .Select(x => new SelectItemModel<int>(x.Localize(), (int) x)));
        }

        [HttpGet]
        public JsonResult GetCoupon(int couponId)
        {
            var coupon = CouponService.GetCoupon(couponId);

            var model = new CouponModel()
            {
                CouponId = coupon.CouponID,
                Code = coupon.Code,
                Value = coupon.Value,
                Type = coupon.Type,
                PossibleUses = coupon.PossibleUses,
                ExpirationDate = coupon.ExpirationDate,
                AddingDate = coupon.AddingDate,
                CurrencyIso3 = coupon.CurrencyIso3,
                Enabled = coupon.Enabled,
                MinimalOrderPrice = coupon.MinimalOrderPrice,
                IsMinimalOrderPriceFromAllCart = coupon.IsMinimalOrderPriceFromAllCart,
                ActualUses = coupon.ActualUses,
                CategoryIds = coupon.CategoryIds,
                ProductsIds = coupon.ProductsIds,
                TriggerActionId = coupon.TriggerActionId,
                TriggerId = coupon.TriggerId,
                Mode = coupon.Mode,
                Days = coupon.Days,
                StartDate = coupon.StartDate,
                ForFirstOrder = coupon.ForFirstOrder
            };
            var partner = PartnerService.GetPartnerByCoupon(couponId);
            if (partner != null)
            {
                model.PartnerId = partner.Id;
                model.PartnerName = partner.Name;
            }

            return Json(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCoupon(CouponModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var coupon = CouponService.GetCoupon(model.CouponId);
                    coupon.Code = model.Code.Trim();
                    coupon.Value = model.Value;
                    coupon.Type = model.Type;
                    coupon.ExpirationDate = model.ExpirationDate != null
                        ? (DateTime?)new DateTime(model.ExpirationDate.Value.Year, model.ExpirationDate.Value.Month, model.ExpirationDate.Value.Day).AddHours(23).AddMinutes(59).AddSeconds(59)
                        : null;
                    coupon.PossibleUses = model.PossibleUses;
                    coupon.MinimalOrderPrice = model.MinimalOrderPrice;
                    coupon.IsMinimalOrderPriceFromAllCart = model.IsMinimalOrderPriceFromAllCart;
                    coupon.Enabled = model.Enabled;
                    coupon.CurrencyIso3 = model.CurrencyIso3;
                    coupon.Days = model.Days;
                    coupon.StartDate = model.StartDate;
                    coupon.ForFirstOrder = model.ForFirstOrder;

                    CouponService.UpdateCoupon(coupon);

                    CouponService.DeleteAllCategoriesFromCoupon(coupon.CouponID);

                    if (model.CategoryIds != null)
                    {
                        foreach (var categoryId in model.CategoryIds)
                            CouponService.AddCategoryToCoupon(coupon.CouponID, categoryId);
                    }

                    CouponService.DeleteAllProductsFromCoupon(coupon.CouponID);

                    if (model.ProductsIds != null)
                    {
                        foreach (var productId in model.ProductsIds)
                            CouponService.AddProductToCoupon(coupon.CouponID, productId);
                    }

                    return JsonOk(coupon.CouponID);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex.Message, ex);
                }
            }

            var errors = "";

            foreach (var modelState in ViewData.ModelState.Values)
                foreach (var error in modelState.Errors)
                    errors += " " + error.ErrorMessage;

            return Json(new { result = false, errors = errors });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceCoupon(CouponModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var coupon = CouponService.GetCoupon(model.CouponId);
                    coupon.Code = model.Code.Trim();
                    coupon.Value = model.Value;
                    coupon.MinimalOrderPrice = model.MinimalOrderPrice;
                    coupon.IsMinimalOrderPriceFromAllCart = model.IsMinimalOrderPriceFromAllCart;
                    coupon.Enabled = model.Enabled;

                    CouponService.UpdateCoupon(coupon);
                    
                    return Json(new { result = true });
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex.Message, ex);
                }
            }

            var errors = "";

            foreach (var modelState in ViewData.ModelState.Values)
                foreach (var error in modelState.Errors)
                    errors += " " + error.ErrorMessage;

            return Json(new { result = false, errors = errors });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCoupon(int couponId)
        {
            CouponService.DeleteCoupon(couponId);
            return Json(new { result = true });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCoupon(CouponModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var coupon = new Coupon();
                    coupon.Code = model.Code.Trim();
                    coupon.Value = model.Value;
                    coupon.Type = model.Type;
                    coupon.ExpirationDate = model.ExpirationDate != null
                        ? (DateTime?)new DateTime(model.ExpirationDate.Value.Year, model.ExpirationDate.Value.Month, model.ExpirationDate.Value.Day).AddHours(23).AddMinutes(59).AddSeconds(59)
                        : null;
                    coupon.PossibleUses = model.PossibleUses;
                    coupon.MinimalOrderPrice = model.MinimalOrderPrice;
                    coupon.IsMinimalOrderPriceFromAllCart = model.IsMinimalOrderPriceFromAllCart;
                    coupon.Enabled = model.Enabled;
                    coupon.CurrencyIso3 = model.CurrencyIso3;
                    coupon.TriggerActionId = model.TriggerActionId;
                    coupon.TriggerId = model.TriggerId;
                    coupon.Mode = model.Mode;
                    coupon.AddingDate = DateTime.Now;
                    coupon.Days = model.Days;
                    coupon.StartDate = model.StartDate;
                    coupon.ForFirstOrder = model.ForFirstOrder;

                    CouponService.AddCoupon(coupon);

                    if (model.CategoryIds != null)
                    {
                        foreach (var categoryId in model.CategoryIds)
                            CouponService.AddCategoryToCoupon(coupon.CouponID, categoryId);
                    }

                    if (model.ProductsIds != null)
                    {
                        foreach (var productId in model.ProductsIds)
                            CouponService.AddProductToCoupon(coupon.CouponID, productId);
                    }

                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Discounts_CouponCreated);

                    return JsonOk(coupon.CouponID);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex.Message, ex);
                }
            }

            var errors = "";

            foreach (var modelState in ViewData.ModelState.Values)
                foreach (var error in modelState.Errors)
                    errors += " " + error.ErrorMessage;

            return Json(new { result = false, errors = errors });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ResetCouponCategories(int couponId)
        {
            CouponService.DeleteAllCategoriesFromCoupon(couponId);
            return Json(new {result = true});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ResetCouponProducts(int couponId)
        {
            CouponService.DeleteAllProductsFromCoupon(couponId);
            return Json(new { result = true });
        }

        [HttpGet]
        public JsonResult GetCouponCode()
        {
            return Json(new { code = Strings.GetRandomString(8) });
        }

        #endregion

        #region Triggers 

        [HttpGet]
        public JsonResult GetCouponsByTriggerAction(int triggerActionId)
        {
            return Json(CouponService.GetCouponsByTriggerAction(triggerActionId));
        }

        [HttpGet]
        public JsonResult GetCouponByTrigger(int triggerId)
        {
            return Json(CouponService.GetCouponByTrigger(triggerId));
        }

        #endregion
    }
}
