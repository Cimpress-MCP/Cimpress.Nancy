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
                if (!string.IsNullOrEmpty(AuthHeader))
                {
                    response = response.WithHeader("WWW-Authenticate", AuthHeader);
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

        public virtual string AuthHeader => string.Empty;
    }
}