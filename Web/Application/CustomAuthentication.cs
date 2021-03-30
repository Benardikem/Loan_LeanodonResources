using System;
using System.Web;
using System.Web.Security;
using System.Linq;
using System.Web.Script.Serialization;
using API.Models;

namespace Web.Application
{
    public class CustomAuthentication
    {
        public static void SetAuthCookies(string userName, string fullName)
        {
            FormsAuthenticationTicket identityTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddDays(60), true, fullName);
            string encryptedIdentityTicket = FormsAuthentication.Encrypt(identityTicket);
            var identityCookie = new HttpCookie(MvcApplication.Cookie_Name, encryptedIdentityTicket);
            identityCookie.Expires = DateTime.Now.AddDays(60);
            HttpContext.Current.Response.Cookies.Add(identityCookie);
            FormsAuthentication.SetAuthCookie(userName, false);
        }

        public static void CreateAuthenticationTicket(string username, bool persistent)
        {
            using (MyDbContext _db = new MyDbContext())
            {
                User user = _db.Users.SingleOrDefault(a => a.UserID == username);
                if (user != null)
                {
                    CustomPrincipalSerializedModel serializeModel = new CustomPrincipalSerializedModel();
                    serializeModel.Email = user.Email;
                    serializeModel.UserName = user.UserID;
                    serializeModel.DisplayName = user.DisplayName;
                    serializeModel.UserId = user.UserID.ToString();
                    serializeModel.UserCategory = user.UserCategory;
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string userData = serializer.Serialize(serializeModel);
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                      1, username, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.Minutes), persistent, userData);
                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie faCookie = new HttpCookie(MvcApplication.Cookie_Name, encTicket);
                    HttpContext.Current.Response.Cookies.Add(faCookie);
                }
            }
        }
    }
}