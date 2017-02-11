using Nancy;
using Nancy.Bootstrapper;

namespace Cimpress.Nancy.Security
{
    public class AuthRegistrations : Registrations
    {
        public AuthRegistrations(ITypeCatalog typeCatalog) : base(typeCatalog)
        {
            RegisterWithDefault<IAuthValidator>(typeof(OAuth2Validator));
            RegisterWithDefault<IAuthVerifier>(typeof(AuthVerifier));
        }
    }
}
