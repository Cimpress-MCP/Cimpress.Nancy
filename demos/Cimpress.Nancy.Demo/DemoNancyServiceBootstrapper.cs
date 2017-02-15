using Cimpress.Nancy.Components;
using Cimpress.Nancy.Config;
using Microsoft.Extensions.Configuration;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using IConfiguration = Cimpress.Nancy.Config.IConfiguration;

namespace Cimpress.Nancy.Demo
{
    public class DemoNancyServiceBootstrapper : NancyServiceBootstrapper
    {
        private readonly IConfiguration _configuration = new Configuration();

        public DemoNancyServiceBootstrapper(Microsoft.Extensions.Configuration.IConfiguration aspNetConfig) : base()
        {
            // We want to load the configuration info from the application settings.
            // This will bind the object from the section "Cimpress.Nancy"
            aspNetConfig.GetSection("Cimpress.Nancy").Bind(_configuration);
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

            // Register the Cimpress.Nancy configuration object
            container.Register(_configuration);

            base.ApplicationStartup(container, pipelines);
        }
    }
}
