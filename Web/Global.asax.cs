using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.Script.Serialization;
using Web.Application;
using System.Data.Entity;
using API.Models;
using Web.Codes;

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public const string Cookie_Name = "HYBRID_IDENT";
        protected void Application_Start()
        {
            //Database.SetInitializer<MyDbContext>(new DropCreateDatabaseIfModelChanges<MyDbContext>());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            EmailScheduler.Start();
            log4net.Config.XmlConfigurator.Configure();
        }
        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[Cookie_Name];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket.UserData == "OAuth") return;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                CustomPrincipalSerializedModel serializeModel = serializer.Deserialize<CustomPrincipalSerializedModel>(authTicket.UserData);
                CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);
                newUser.UserId = serializeModel.UserId;
                newUser.DisplayName = serializeModel.DisplayName;
                newUser.Email = serializeModel.Email;
                newUser.UserCategory = serializeModel.UserCategory;
                HttpContext.Current.User = newUser;
            }
        }
    }
}
