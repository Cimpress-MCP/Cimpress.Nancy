using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Components;
using Nancy;
using Nancy.TinyIoc;

namespace Cimpress.Nancy.Demo
{
    public class DemoBootstrapperExtender : IBootstrapperExtender
    {
        private readonly INancyLogger _logger;

        public DemoBootstrapperExtender(INancyLogger logger)
        {
            _logger = logger;
        }

        public void Initialize(NancyServiceBootstrapper bootstrapper, TinyIoCContainer container)
        {

        }

        public void OnAfterRequest(NancyContext context, IDictionary<string, object> logData)
        {
            _logger.Info(logData);
        }

        public Response OnBeforeRequest(NancyContext context, IDictionary<string, object> logData)
        {
            _logger.Info(logData);
            return null;
        }

        public void OnError(NancyContext context, Exception ex, Response newResponse, IDictionary<string, object> logData)
        {
            _logger.Info(logData);
        }

        public int Priority => 10;
    }
}
