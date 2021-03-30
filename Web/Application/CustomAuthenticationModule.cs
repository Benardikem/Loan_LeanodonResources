using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Web.Application
{
    public class CustomAuthenticationModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnAuthenticateRequest;
        }

        public void OnAuthenticateRequest(object sender, EventArgs e)
        {
            string identityCookieName = MvcApplication.Cookie_Name;;
            HttpApplication app = (HttpApplication)sender;

            // Get the authentication cookie
            HttpCookie identityCookie = app.Context.Request.Cookies[identityCookieName];

            // If the cookie can't be found don't issue custom authentication
            if (identityCookie == null)
                return;

            // decrypt identity ticket
            FormsAuthenticationTicket identityTicket = (FormsAuthenticationTicket)null;
            try
            {
                identityTicket = FormsAuthentication.Decrypt(identityCookie.Value);
            }
            catch
            {
                app.Context.Request.Cookies.Remove(identityCookieName);
                return;
            }

            string name = "";
            HttpCookie authCookie = app.Context.Request.Cookies[MvcApplication.Cookie_Name];
            if (authCookie != null)
            {
                try
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (authTicket.Name != identityTicket.Name) return;
                    name = authTicket.Name;
                }
                catch
                {
                    app.Context.Request.Cookies.Remove(MvcApplication.Cookie_Name);
                    return;
                }
            }

            var customIdentity = new CustomIdentity(name, identityTicket.UserData);
            var userPrincipal = new GenericPrincipal(customIdentity, new string[0]);
            app.Context.User = userPrincipal;
        }

        public void Dispose()
        {
        }
    }
}