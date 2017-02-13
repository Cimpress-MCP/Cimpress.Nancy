using Cimpress.Nancy.Components;
using Nancy;
using System.Linq;

namespace Cimpress.Nancy.Demos
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
                if (!string.IsNullOrEmpty(context.Request.Headers?["Authorization"].First()))
                {
                    response = response.WithHeader("WWW-Authenticate", "Demo Secret Key");
                }
            }

            return response;
        }

        public int Priority => 10;
    }
}
