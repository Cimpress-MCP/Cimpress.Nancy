using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Config;
using Cimpress.Nancy.Security.AuthenticationHeaders;
using Nancy;

namespace Cimpress.Nancy.Security.Demo.Security
{
    public class AuthVerifier : Nancy.Security.AuthVerifier
    {
        public AuthVerifier(IConfiguration config)
        {
            AuthHeader = new SimpleAuthenticationHeader(config.OptionalParameters["OAuth2ClientId"]);
        }

        public override Response HandleBefore(NancyContext context)
        {
            var response = base.HandleBefore(context);
            if (response != null)
            {
                return response;
            }

            if(!context.CurrentUser.HasClaim("[TEST CLAIM]", "true"))
            {
                return HttpStatusCode.Unauthorized;
            }
            return null;
        }

        public override IAuthenticationHeader AuthHeader { get; }
    }
}
