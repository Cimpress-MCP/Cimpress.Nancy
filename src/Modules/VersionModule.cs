using Cimpress.Nancy.Authentication;
using Nancy;

namespace Cimpress.Nancy.Modules
{
    public class VersionModule : NancyModule
    {
        public VersionModule(IConfiguration configuration) : this(configuration.Version, string.Empty)
        {

        }

        public VersionModule(string version, string route) : base(version + route)
        {
            this.RequiresCustomAuthentication();

            Get("/", _ => HttpStatusCode.NotFound);
        }
    }
}
