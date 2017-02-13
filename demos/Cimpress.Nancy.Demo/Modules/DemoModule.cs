using Cimpress.Nancy.Components;
using Cimpress.Nancy.Modules;
using Nancy;

namespace Cimpress.Nancy.Demo.Modules
{
    public class DemoModule : VersionModule
    {
        public DemoModule(IComponentManager componentManager) : base(string.Empty, "/demo", componentManager)
        {
            Get("/", _ => HttpStatusCode.OK, null, "Demo");
        }
    }
}
