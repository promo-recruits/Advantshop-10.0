using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Customers;
using AdvantShop.Models.BonusSystemModule;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    public partial class BonusesController : BaseClientController
    {
        #region Help methods

        //private string IsValidCardData(string firstName, string lastName, long? phone, string birthDay = null)
        //{
        //    var isValid = firstName.IsNotEmpty() && lastName.IsNotEmpty();

        //    if (birthDay != null)
        //        isValid &= birthDay.IsNotEmpty();

        //    if (!isValid)
        //        return T("Bonuses.ErrorRequired");

        //    if (phone == null || phone == 0)
        //        return T("Bonuses.ErrorPhone");

        //    return string.Empty;
        //}

        //private void UpdateCustomer(long cardNumber, bool isCheckout)
        //{
        //    var customer = CustomerContext.CurrentCustomer;

        //    if (customer.RegistredUser)
        //    {
        //        customer.BonusCardNumber = cardNumber;
        //        CustomerService.UpdateCustomer(customer);
        //    }
        //    else if (!isCheckout)
        //    {
        //        Session["bonuscard"] = cardNumber;
        //    }
        //    //else save in checkout data other ajax request
        //}

        #endregion

        public ActionResult GetBonusCard()
        {
            if (!BonusSystem.IsActive)
                return Error404();
            
            var breadCrumbs = new List<BreadCrumbs>()
            {
                new BreadCrumbs(T("MainPage"), Url.AbsoluteRouteUrl("Home")),
                new BreadCrumbs(T("Module.BonusSystem.GetBonusCardTitle"), Url.AbsoluteRouteUrl("GetBonusCardRoute"))
            };

            var model = new GetBonusCardViewModel
            {
                BreadCrumbs = breadCrumbs,
                BonusTextBlock = BonusSystem.BonusTextBlock,
                BonusRightTextBlock = BonusSystem.BonusRightTextBlock,
                Grades = BonusSystem.IsActive && BonusSystem.BonusShowGrades
                        ? BonusSystemService.GetGrades()
                        : null
            };

            SetMetaInformation(T("Module.BonusSystem.GetBonusCardTitle"));

            SetNgController(Web.Infrastructure.Controllers.NgControllers.NgControllersTypes.BonusPageCtrl);

            return View(model);
        }

        public JsonResult BonusJson()
        {
            var customer = CustomerContext.CurrentCustomer;

            var bonusCard = BonusSystemService.GetCard(customer.Id);

            if (bonusCard == null)
            {
                var checkoutData = OrderConfirmationService.Get(customer.Id);
                if (checkoutData != null && checkoutData.User.BonusCardId != null)
                {
                    bonusCard = BonusSystemService.GetCard(checkoutData.User.BonusCardId.Value);
                }
            }

            if (bonusCard == null && Session["bonuscard"] != null)
            {
                bonusCard = BonusSystemService.GetCard((long)Session["bonuscard"]);
            }
            
            return Json(bonusCard != null
                ? new
                {
                    bonus = new
                    {
                        CardNumber = bonusCard.CardNumber,
                        BonusAmount = bonusCard.BonusesTotalAmount,
                        BonusPercent = bonusCard.Grade.BonusPercent,
                        Blocked = bonusCard.Blocked
                    },
                    bonusText =
                        string.Format("{0} ({1} {2} {3})", T("Bonuses.ByBonusCard"), T("Bonuses.YourBonuses"),
                            bonusCard.BonusesTotalAmount.ToString("F1"),
                            Strings.Numerals((float)bonusCard.BonusesTotalAmount, T("Bonuses.Bonuses0"),
                                T("Bonuses.Bonuses1"), T("Bonuses.Bonuses2"), T("Bonuses.Bonuses5")))
                }
                : null);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateBonusCard()
        {
            var customer = CustomerContext.CurrentCustomer;
            if (!customer.RegistredUser)
                return Json(new { result = false, error = T("Bonuses.UserNotRegistred") });

            var bonusCard = BonusSystemService.GetCard(customer.Id);
            if (bonusCard != null)
                return Json(new { result = false, error = T("Bonuses.BonusCardAlreadyRegistered") });

            customer.BonusCardNumber = BonusSystemService.AddCard(new Card { CardId = customer.Id });
            CustomerService.UpdateCustomer(customer);

            return Json(new {result = true});
        }
    }
}