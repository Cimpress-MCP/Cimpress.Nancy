using Cimpress.Nancy.Components;
using Nancy;

namespace Cimpress.Nancy.Modules
{
    public class PingModule : NancyModule
    {
        public PingModule() : base("/Ping")
        {
            Get("/", _ => Ping(), name: "Ping");
        }

        public Response Ping()
        {
            return new Response { StatusCode = HttpStatusCode.OK };
        }
    }
}
