using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Controllers.Shared;

namespace AdvantShop.Web.Admin.Attributes
{
    [Flags]
    public enum EAuthErrorType
    {
        None = 0,
        Json = 1,
        Action = 2,
        View = 4,
        PartialView = 8
    }

    public class AuthAttribute : ActionFilterAttribute
    {
        private List<RoleAction> _rolesActionKeys;
        private readonly EAuthErrorType _errorType;

        public AuthAttribute()
        {
            _rolesActionKeys = new List<RoleAction>();
            _errorType = EAuthErrorType.Json | EAuthErrorType.Action | EAuthErrorType.View;
        }

        public AuthAttribute(params RoleAction[] keys)
        {
            _rolesActionKeys = new List<RoleAction>(keys);
            _errorType = EAuthErrorType.Json | EAuthErrorType.Action | EAuthErrorType.View;
        }

        public AuthAttribute(EAuthErrorType errorType, params RoleAction[] keys)
        {
            _errorType = errorType;
            _rolesActionKeys = new List<RoleAction>(keys);
        }
        public List<RoleAction> RolesAction
        {
            get { return _rolesActionKeys; }
            set { _rolesActionKeys = value; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsModerator && !HasRole(customer))
            {
                var controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
                var action = filterContext.RouteData.Values["action"].ToString().ToLower();

                if (!(controller.Equals("account") && action.Equals("login")))
                {
                    if (!filterContext.IsChildAction)
                    {
                        var roleAcion = _rolesActionKeys.Any() ? _rolesActionKeys[0] : RoleAction.None;
                        var request = filterContext.RequestContext.HttpContext.Request;
                        if (request.IsAjaxRequest() && _errorType.HasFlag(EAuthErrorType.Json))
                            filterContext.Result = new ServiceController().RoleAccessIsDeniedJson(roleAcion);
                        else if (_errorType.HasFlag(EAuthErrorType.PartialView))
                            filterContext.Result = (PartialViewResult)new ServiceController().RoleAccessIsDenied(roleAcion, partial: true);
                        else
                            filterContext.Result = (ViewResult)new ServiceController().RoleAccessIsDenied(roleAcion);
                    }
                    else
                    {
                        filterContext.Result = new EmptyResult();
                    }
                }
            }
        }

        private bool HasRole(Customer customer)
        {
            var customerRolesActions = RoleActionService.GetCustomerRoleActionsByCustomerId(customer.Id);

            foreach (var actionKey in _rolesActionKeys)
            {
                if (actionKey != RoleAction.None && customerRolesActions.Any(item => item.Role == actionKey))
                    return true;
            }

            return false;
        }
    }
}
