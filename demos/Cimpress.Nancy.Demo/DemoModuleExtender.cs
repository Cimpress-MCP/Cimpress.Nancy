using Cimpress.Nancy.Components;
using Nancy;

namespace Cimpress.Nancy.Demo
{
    public class DemoModuleExtender : IVersionModuleExtender
    {
        public void After(NancyContext context)
        {
            if (context.Response.StatusCode == HttpStatusCode.Unauthorized)
            {
                context.Response.ReasonPhrase = "Authentication Failed";
            }
        }

        public Response Before(NancyContext context)
        {
            Response response = null;
            var user = context.CurrentUser;
            if (user == null)
            {
                response = HttpStatusCode.Unauthorized;
                response = response.WithHeader("WWW-Authenticate", "Demo Secret Key");
            }

            return response;
        }

        public int Priority => 10;
    }
}
