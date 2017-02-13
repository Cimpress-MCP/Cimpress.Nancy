using Cimpress.Nancy.Components;
using Cimpress.Nancy.Modules;
using Nancy;

namespace Cimpress.Nancy.Demos.Modules
{
    public class DemoModule : VersionModule
    {
        public DemoModule(IConfiguration configuration, IComponentManager componentManager) : base(configuration, componentManager)
        {
            Get("/demo", _ => HttpStatusCode.OK, null, "Demo");
        }
    }
}
