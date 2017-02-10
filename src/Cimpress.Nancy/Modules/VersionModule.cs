using Cimpress.Nancy.Components;
using Nancy;

namespace Cimpress.Nancy.Modules
{
    public class VersionModule : NancyModule
    {

        public VersionModule(IConfiguration configuration, IComponentManager componentManager) : this(configuration.Version, string.Empty, componentManager)
        {

        }

        public VersionModule(string version, string route, IComponentManager componentManager) : base(version + route)
        {
            foreach (var extender in componentManager.GetModuleExtenders())
            {
                Before.AddItemToEndOfPipeline(extender.Before);
                After.AddItemToEndOfPipeline(extender.After);
            }

            Get("/", _ => HttpStatusCode.NotFound);
        }
    }
}
