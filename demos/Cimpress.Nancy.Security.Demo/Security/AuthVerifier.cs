using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Config;
using Cimpress.Nancy.Security.AuthenticationHeaders;

namespace Cimpress.Nancy.Security.Demo.Security
{
    public class AuthVerifier : Nancy.Security.AuthVerifier
    {
        public AuthVerifier(IConfiguration config)
        {
            AuthHeader = new SimpleAuthenticationHeader(config.OptionalParameters["OAuth2ClientId"]);
        }

        public override IAuthenticationHeader AuthHeader { get; }
    }
}
