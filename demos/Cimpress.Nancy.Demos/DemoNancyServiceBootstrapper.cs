using Cimpress.Nancy.Components;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Cimpress.Nancy.Demos
{
    public class DemoNancyServiceBootstrapper : NancyServiceBootstrapper
    {
        public DemoNancyServiceBootstrapper() : base()
        {
            
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // Specify we want to use our DemoLogger implementation
            // And not worry about the container using the DefaultNancyLogger
            container.Register<INancyLogger, DemoLogger>();

            // This is not actually needed as the container will automatically
            // detect all I*Extender implementations and register them at startup.
            // When explicitly registered this way, however, the container 
            // should only wire up the specified implementations
            container.Register<IVersionModuleExtender, DemoModuleExtender>();
            container.Register<IBootstrapperExtender, DemoBootstrapperExtender>();
            base.ApplicationStartup(container, pipelines);
        }
    }
}
