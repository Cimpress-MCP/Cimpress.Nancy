using System.Collections.Generic;
using Cimpress.Nancy.Components;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Cimpress.Nancy.Config;

namespace Cimpress.Nancy.Security.Demo
{
    public class DemoNancyServiceBootstrapper : NancyServiceBootstrapper
    {
        public DemoNancyServiceBootstrapper() : base()
        {
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            IConfiguration configuration = new Configuration();
            configuration.OptionalParameters = new Dictionary<string, string>
            {
                ["OAuth2Issuer"] = "[Pass root url of issuer here]",
                ["OAuth2SecretKey"] = "[Pass application's secret key here]",
                ["OAuth2ClientId"] = "[Pass application's client id here]",
                ["OAuth2JwksLocation"] = "[Pass auth0 tenant's jwks url here]",
                ["OAuth2JwksAudience"] = "[Pass auth0 tenant's audience here]"
            };
            container.Register(configuration);
            base.ApplicationStartup(container, pipelines);
        }
    }
}
