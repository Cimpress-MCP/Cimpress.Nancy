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
        public const string StartTimeString = "StartTime";
        public const string EndTimeString = "EndTime";

        private IComponentManager _componentManager;

        protected string CorsHeaders = "APIKEY,Authorization,Access-Control-Allow-Origin,Content-Type,Accept";
        protected string CorsMethods = "OPTIONS,POST,GET,DELETE,PUT,PATCH";
        protected string CorsOrigins = "*";

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
            ctx.Items.Add(StartTimeString, DateTime.UtcNow);
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
            AddCorsHeaders(ctx.Response);
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
            AddCorsHeaders(newResponse);

            foreach (var extender in _componentManager.GetBootstrapperExtenders())
            {
                extender.OnError(ctx, ex, newResponse, logData);
            }

            return newResponse;
        }

        private void AddCorsHeaders(Response response)
        {
            response.Headers.Add("Access-Control-Allow-Origin", CorsOrigins);
            response.Headers.Add("Access-Control-Allow-Headers", CorsHeaders);
            response.Headers.Add("Access-Control-Allow-Methods", CorsMethods);
        }
    }
}
