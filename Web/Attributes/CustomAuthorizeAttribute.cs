using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.Application;


namespace Web.Attributes
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {

        //Custom named parameters for annotation
        //public string ResourceKey { get; set; }
        public string OperationKey { get; set; }

        //Called when access is denied
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //User isn't logged in
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Home", action = "Index" })
                );
            }
            //User is logged in but has no access
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Account", action = "AccessDenied" })
                );
            }
        }

        //Core authentication, called before each action
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var b = LoggedInUser.LoggedIn;
            //Is user logged in?
            if (b)
                //If user is logged in and we need a custom check:
                if (!String.IsNullOrEmpty(OperationKey))
                    return LoggedInUser.IsPageAllowed(OperationKey);
            //Returns true or false, meaning allow or deny. False will call HandleUnauthorizedRequest above
            return b;
        }
    }
}