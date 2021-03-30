using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using API.Models;

namespace Web.Application
{
    public static class LoggedInUser
    {
        public static bool LoggedIn
        {
            get
            {
                return !string.IsNullOrEmpty(UserName);
            }
        }

        public static string UserName
        {
            get
            {
                string result = "";
                CustomPrincipal _principal = HttpContext.Current.User as CustomPrincipal;
                if (_principal != null && _principal.HasIdentity)
                {
                    result = _principal.UserId;
                }
                return result;
            }
        }

        public static List<Menu> Privileges
        {
            get
            {
                List<Menu> result = new List<Menu>();
                using (MyDbContext _db = new MyDbContext())
                {
                    User _user = _db.Users.SingleOrDefault(a => a.UserID == UserName);
                    if (_user != null)
                        result = _user.GetPrivileges();
                }
                return result;
            }
        }

        public static string DisplayName
        {
            get
            {
                string result = "";
                CustomPrincipal _principal = HttpContext.Current.User as CustomPrincipal;
                if (_principal != null && _principal.HasIdentity)
                {
                    result = _principal.DisplayName;
                }
                return result;
            }
        }
        public static string UserId
        {
            get
            {
                string result = "0";
                if (!string.IsNullOrEmpty((HttpContext.Current.User as CustomPrincipal).UserId))
                {
                    result = (HttpContext.Current.User as CustomPrincipal).UserId;
                }
                return result;
            }
        }


        public static string Email
        {
            get
            {
                string result = "";
                if (!string.IsNullOrEmpty((HttpContext.Current.User as CustomPrincipal).Email))
                {
                    result = (HttpContext.Current.User as CustomPrincipal).Email;
                }
                return result;
            }
        }

        public static string UserCategory {
            get
            {
                string result = "";
                if (!string.IsNullOrEmpty((HttpContext.Current.User as CustomPrincipal).UserCategory))
                {
                    result = (HttpContext.Current.User as CustomPrincipal).UserCategory;
                }
                return result;
            }
        }

        public static bool IsPageAllowed(string _OperationKey)
        {
            bool flag = false;
            if (LoggedIn)
            {
                List<string> exceptions = new List<string> { "home/index", "account/accessdenied", "home/error" };
                if (String.IsNullOrEmpty(UserName))
                {
                    return false;
                }

                if (exceptions.Contains(_OperationKey.ToLower()))
                {
                    return true;
                }
                using (MyDbContext _db = new MyDbContext())
                {
                    var _user = _db.Users.SingleOrDefault(u => u.UserID == UserName);
                    if (_user != null)
                    {
                        var privileges = _user.GetPrivileges();
                        if (privileges != null)
                        {
                            string _link = _OperationKey;
                            if (!_link.StartsWith("/"))
                                _link = "/" + _link;
                            int _count = privileges.Count(a => a.SubMenus.Count(b => b.Link.ToLower() == _link.ToLower()) > 0);
                            flag = _count > 0;
                        }
                    }
                }
            }
            return flag;
        }
    }
}
