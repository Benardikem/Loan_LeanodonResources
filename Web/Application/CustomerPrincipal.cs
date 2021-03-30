using System.Security.Principal;
using System.Web.Security;

namespace Web.Application
{
    public interface ICustomPrincipal : IPrincipal
    {
        string DisplayName { get; set; }
    }
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }

        public CustomPrincipal(string username)
        {
            this.Identity = new GenericIdentity(username);
        }

        public bool IsInRole(string role)
        {
            return Identity != null && Identity.IsAuthenticated &&
               !string.IsNullOrWhiteSpace(role) && Roles.IsUserInRole(Identity.Name, role);
        }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
public string UserCategory { get; set; }
        public bool HasIdentity
        {
            get { return !string.IsNullOrWhiteSpace(this.Identity.Name); }
        }
        //public List<Menu> Menus { get; set; }
    }

    public class CustomPrincipalSerializedModel
    {
        public string DisplayName { get; set; }
        public string Email { get;  set; }
        public string UserName { get;  set; }
        public string UserId { get; set; }
        public string UserCategory { get; set; }
        //public List<Menu> Menus { get; set; }
        //public string ContractorId { get; set; }
    }
}