using Cimpress.Nancy.Security.AuthenticationHeaders;
using Nancy;

namespace Cimpress.Nancy.Security
{
    public class AuthVerifier : IAuthVerifier
    {
        public AuthVerifier()
        {
        }

        public Response HandleBefore(NancyContext context)
        {
            Response response = null;
            var user = context.CurrentUser as UserIdentity;
            if (user == null || !user.Valid)
            {
                response = HttpStatusCode.Unauthorized;
                if (AuthHeader != null)
                {
                    AuthHeader.AddHeader(response, context.Request);
                }
            }

            return response;
        }

        public void HandleAfter(NancyContext context)
        {
            if (context.Response.StatusCode == HttpStatusCode.Unauthorized)
            {
                context.Response.ReasonPhrase = "Authentication Failed";
            }
        }

        public virtual IAuthenticationHeader AuthHeader => null;
    }
}