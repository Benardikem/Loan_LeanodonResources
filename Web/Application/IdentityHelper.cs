using System.Web;

namespace Web.Application
{
    public class IdentityHelper
    {
        public static CustomIdentity CurrentUser
        {
            get
            {
                if (HttpContext.Current.User.Identity is CustomIdentity)
                    return (CustomIdentity)HttpContext.Current.User.Identity;

                return CustomIdentity.EmptyIdentity;
            }
        }
    }
}