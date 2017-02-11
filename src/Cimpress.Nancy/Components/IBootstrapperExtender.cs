using System;
using System.Collections.Generic;
using Nancy;
using Nancy.TinyIoc;

namespace Cimpress.Nancy.Components
{
    public interface IBootstrapperExtender
    {
        void Initialize(NancyServiceBootstrapper bootstrapper, TinyIoCContainer container);
        void OnAfterRequest(NancyContext context, IDictionary<string, object> logData);
        Response OnBeforeRequest(NancyContext context, IDictionary<string, object> logData);
        void OnError(NancyContext context, Exception ex, Response newResponse, IDictionary<string, object> logData);
        int Priority { get; }
    }
}
