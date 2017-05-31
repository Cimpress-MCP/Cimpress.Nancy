using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Cimpress.Nancy.Components;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using HttpStatusCode = Nancy.HttpStatusCode;

namespace Cimpress.Nancy
{
    public abstract class NancyServiceBootstrapper : DefaultNancyBootstrapper
    {
        private IComponentManager _componentManager;

        protected NancyServiceBootstrapper() : base()
        {
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            _componentManager = container.Resolve<IComponentManager>();

            SetupPipelineHandlers(pipelines);
            base.ApplicationStartup(container, pipelines);

            foreach (var extender in _componentManager.GetBootstrapperExtenders())
            {
                extender.Initialize(this, container);
            }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.AddDirectory("Swagger","Views");
            base.ConfigureConventions(nancyConventions);
        }

        private void SetupPipelineHandlers(IPipelines pipelines)
        {
            pipelines.BeforeRequest += OnBeforeRequest;

            pipelines.AfterRequest += OnAfterRequest;

            pipelines.OnError += OnError;
        }

        private Response OnBeforeRequest(NancyContext ctx)
        {
            IDictionary<string, object> logData = new Dictionary<string, object>();
            Response result = null;
            foreach (var extender in _componentManager.GetBootstrapperExtenders())
            {
                //The last response returned by an extender will be the one sent.
                result = extender.OnBeforeRequest(ctx, logData) ?? result;
            }
            return result;
        }

        private void OnAfterRequest(NancyContext ctx)
        {
            IDictionary<string, object> logData = new Dictionary<string, object>();
            foreach (var extender in _componentManager.GetBootstrapperExtenders())
            {
                extender.OnAfterRequest(ctx, logData);
            }

            ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            ctx.Response.Headers.Add("Access-Control-Allow-Headers", "APIKEY,Authorization,Access-Control-Allow-Origin,Content-Type,Accept");
            ctx.Response.Headers.Add("Access-Control-Allow-Methods", "OPTIONS,POST,GET,DELETE,PUT");
        }

        private dynamic OnError(NancyContext ctx, Exception ex)
        {
            var request = ctx.Request;
            var webException = ex as WebException;
            string responseReason;
            HttpStatusCode responseStatus;
            if (webException == null)
            {
                responseReason = "Internal Server Error";
                responseStatus = HttpStatusCode.InternalServerError;
            }
            else
            {
                var response = webException.Response as HttpWebResponse;
                responseReason = new StreamReader(response.GetResponseStream()).ReadToEnd();
                responseStatus = (HttpStatusCode)response.StatusCode;
            }

            IDictionary<string, object> logData = new Dictionary<string, object>();
            logData["ResponseReason"] = responseReason;
            logData["StatusCode"] = responseStatus;

            var newResponse = new Response { ReasonPhrase = responseReason, StatusCode = responseStatus };
            newResponse.Headers.Add("Access-Control-Allow-Origin", "*");


            foreach (var extender in _componentManager.GetBootstrapperExtenders())
            {
                extender.OnError(ctx, ex, newResponse, logData);
            }

            return newResponse;
        }
    }
}
