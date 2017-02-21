using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Config;

namespace Cimpress.Nancy.Security.Demo.Security
{
    public class AuthVerifier : Nancy.Security.AuthVerifier
    {
        public AuthVerifier(IConfiguration config)
        {
            AuthHeader = config.OptionalParameters["OAuth2ClientId"];
        }

        public override string AuthHeader { get; }
    }
}
