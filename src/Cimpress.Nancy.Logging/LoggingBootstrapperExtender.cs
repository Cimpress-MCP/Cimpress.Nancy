using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Components;
using log4net;
using Nancy;
using Nancy.TinyIoc;
using Newtonsoft.Json;

namespace Cimpress.Nancy.Logging
{
    public class LoggingBootstrapperExtender : IBootstrapperExtender
    {
        private const string StartTimeString = "StartTime";
        private const string CorrelationIdString = "CorrelationId";

        private INancyLogger _logger;
        private NancyServiceBootstrapper _bootstrapper;

        public LoggingBootstrapperExtender(INancyLogger logger)
        {
            Priority = 100;
            _logger = logger;
        }

        public void Initialize(NancyServiceBootstrapper bootstrapper, TinyIoCContainer container)
        {
            _bootstrapper = bootstrapper;
            _logger.Configure(new Dictionary<string, string>
            {
                { "ApplicationName", _bootstrapper.ApplicationName },
                { "EnvironmentName", _bootstrapper.EnvironmentName },
                { "LoggingBaseUri", _bootstrapper.SumoLogicBaseUri }
            });
        }

        public void OnAfterRequest(NancyContext context, IDictionary<string, object> logData)
        {
            var request = context.Request;
            var response = context.Response;
            var duration = int.MinValue;
            if (context.Items.ContainsKey(StartTimeString))
            {
                duration = (int)DateTime.UtcNow.Subtract((DateTime)context.Items[StartTimeString]).TotalMilliseconds;
            }

            var correlationId = string.Empty;
            if (context.Items.ContainsKey(CorrelationIdString))
            {
                correlationId = (string) context.Items[CorrelationIdString];
            }
            
            var stream = new MemoryStream();
            response.Contents(stream);
            stream.Position = 0;
            var responseBody = new StreamReader(stream).ReadToEnd();
            var responseIsJson = response.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(responseBody);
            var bodyObject = responseIsJson ? JsonConvert.DeserializeObject(responseBody) : responseBody;

            logData["Host"] = Environment.MachineName;
            logData["Body"] = bodyObject;
            logData["CallDuration"] = duration;
            logData["StatusCode"] = response.StatusCode;
            logData["ResponseReason"] = response.ReasonPhrase;

            _logger.Info(new BaseMessage
            {
                Message = request.Path,
                CorrelationId = correlationId,
                Info = logData
            });

            context.Response.Headers.Add("CorrelationId", correlationId);
        }

        public Response OnBeforeRequest(NancyContext context, IDictionary<string, object> logData)
        {

            var startTime = DateTime.UtcNow;
            context.Items.Add(StartTimeString, startTime);

            var correlationId = GetCorrelationId(context);
            context.Items.Add(CorrelationIdString, correlationId);

            var request = context.Request;

            var formString = JsonConvert.SerializeObject(request.Form.ToDictionary());
            request.Body.Position = 0;
            var bodyString = new StreamReader(request.Body).ReadToEnd();
            request.Body.Position = 0;

            var isBodyJson = string.Equals(request.Headers.ContentType ?? string.Empty, "application/json", StringComparison.OrdinalIgnoreCase);

            var bodyObject = new object();
            try
            {
                if (isBodyJson)
                {
                    bodyObject = JsonConvert.DeserializeObject(bodyString);
                }
            }
            catch (Exception)
            {
                isBodyJson = false;
            }

            logData["Host"] = Environment.MachineName;
            logData["Form"] = JsonConvert.DeserializeObject(formString);
            logData["Headers"] = request.Headers;
            logData["Body"] = isBodyJson ? bodyObject : bodyString;
            logData["Method"] = request.Method;

            _logger.Debug(new BaseMessage
            {
                Message = request.Path,
                CorrelationId = correlationId,
                Info = logData
            });
            return null;
        }

        public void OnError(NancyContext context, Exception ex, Response newResponse, IDictionary<string, object> logData)
        {
            var duration = int.MinValue;
            if (context.Items.ContainsKey(StartTimeString))
            {
                duration = (int)DateTime.UtcNow.Subtract((DateTime)context.Items[StartTimeString]).TotalMilliseconds;
            }

            var correlationId = string.Empty;
            if (context.Items.ContainsKey(CorrelationIdString))
            {
                correlationId = (string) context.Items[CorrelationIdString];
            }

            logData["Host"] = Environment.MachineName;
            logData["StackTrace"] = ex.ToString();
            logData["CallDuration"] = duration;

            _logger.Error(new BaseMessage
            {
                Message = context.Request.Path,
                CorrelationId = correlationId,
                Info = logData
            });

            newResponse.Headers.Add("CorrelationId", correlationId);
        }

        public int Priority { get; }

        private string GetCorrelationId(NancyContext ctx)
        {
            var correlationId = ctx.Request.Headers["CorrelationId"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            return correlationId;
        }
    }
}
