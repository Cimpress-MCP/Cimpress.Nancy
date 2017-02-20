using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Demo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy.Owin;

namespace Cimpress.Nancy.Deployment.DotnetCoreBeanstalk.Demo
{
    public class Startup : Cimpress.Nancy.Demo.Startup
    {
        public Startup(IHostingEnvironment env) : base(env)
        {
        }
    }
}
