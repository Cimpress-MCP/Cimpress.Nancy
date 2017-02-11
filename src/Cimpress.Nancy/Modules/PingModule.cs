using Cimpress.Nancy.Components;
using Nancy;

namespace Cimpress.Nancy.Modules
{
    public class PingModule : VersionModule
    {
        public PingModule(IConfiguration configuration, IComponentManager componentManager) : base(configuration.Version, "/Ping", componentManager)
        {
            Get("/", _ => Ping(), name: "Ping");
        }

        public Response Ping()
        {
            return new Response { StatusCode = HttpStatusCode.OK };
        }
    }
}
