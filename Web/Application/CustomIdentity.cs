using System.Security.Principal;

namespace Web.Application
{
    public class CustomIdentity : GenericIdentity
    {
        private string _fullName;

        public CustomIdentity(string userName, string fullName) : base (userName)
        {
            _fullName = fullName;
        }

        public bool HasIdentity
        {
            get { return !string.IsNullOrWhiteSpace(_fullName); }
        }

        public string FullName
        {
            get { return _fullName; }
        }

        public static CustomIdentity EmptyIdentity
        {
            get { return new CustomIdentity("", ""); }
        }
        
    }
}