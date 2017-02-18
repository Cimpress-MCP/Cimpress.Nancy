using System.Collections.Generic;
using Cimpress.Nancy.Components;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Cimpress.Nancy.Swagger;
using Nancy.Swagger.Services;
using Cimpress.Nancy.Config;

namespace Cimpress.Nancy.Swagger.Demo
{
    public class DemoNancyServiceBootstrapper : NancyServiceBootstrapper
    {
        public DemoNancyServiceBootstrapper() : base()
        {
            SwaggerMetadataProvider.SetInfo("Cimpress.Nancy.Swagger.Demo", "v0", "A test swagger document");
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            IConfiguration configuration = new Configuration();
            configuration.OptionalParameters = new Dictionary<string, string>
            {
                [SwaggerModule.SwaggerUiEndpointUrlKey] = "/swagger-ui/"
            };
            container.Register(configuration);
            base.ApplicationStartup(container, pipelines);
        }
    }
}
