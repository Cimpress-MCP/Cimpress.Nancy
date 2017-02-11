using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy;
using Cimpress.Nancy.Components;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Cimpress.Nancy.Demo
{
    public class DemoNancyServiceBootstrapper : NancyServiceBootstrapper
    {
        public DemoNancyServiceBootstrapper() : base()
        {
            
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.Register<INancyLogger, DemoLogger>();
            base.ApplicationStartup(container, pipelines);
        }
    }
}
