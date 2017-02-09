using Nancy;

namespace Cimpress.Nancy.Modules
{
    public class PingModule : VersionModule
    {
        public PingModule(IConfiguration configuration) : base(configuration.Version, "/Ping")
        {
            Get("/", _ => Ping(), name: "Ping");
        }

        public Response Ping()
        {
            return new Response { StatusCode = HttpStatusCode.OK };
        }
    }
}
