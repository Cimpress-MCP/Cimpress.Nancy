using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Components;
using Cimpress.Nancy.Config;
using log4net;
using Nancy;
using Nancy.TinyIoc;
using Newtonsoft.Json;

namespace Cimpress.Nancy.Logging
{
    public class LoggingBootstrapperExtender : IBootstrapperExtender
    {
        public static readonly string BodySizeLimitConfigKey = "LoggingBodySizeLimit";

        private const string CorrelationIdString = "CorrelationId";

        private INancyLogger _logger;

        private int _loggedBodySizeLimit = -1;

        public LoggingBootstrapperExtender(INancyLogger logger, IConfiguration config)
        {
            Priority = 100;
            _logger = logger;

            string loggedBodySizeLimit;
            if(config.OptionalParameters.TryGetValue(BodySizeLimitConfigKey, out loggedBodySizeLimit))
            {
                int.TryParse(loggedBodySizeLimit, out _loggedBodySizeLimit);
            }
        }

        public void Initialize(NancyServiceBootstrapper bootstrapper, TinyIoCContainer container)
        {
        }

        public void OnAfterRequest(NancyContext context, IDictionary<string, object> logData)
        {
            var request = context.Request;
            var response = context.Response;

            if (context.Items.ContainsKey(NancyServiceBootstrapper.StartTimeString))
            {
                var startTime = (DateTime)context.Items[NancyServiceBootstrapper.StartTimeString];
                var endTime = DateTime.UtcNow;
                logData[NancyServiceBootstrapper.StartTimeString] = startTime;
                logData[NancyServiceBootstrapper.EndTimeString] = endTime;
                logData["CallDuration"] = (int)endTime.Subtract(startTime).TotalMilliseconds;
            }

            var correlationId = string.Empty;
            if (context.Items.ContainsKey(CorrelationIdString))
            {
                correlationId = (string) context.Items[CorrelationIdString];
            }

            string responseBody;
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                stream.Position = 0;
                using (var streamReader = new StreamReader(stream))
                {
                    responseBody = streamReader.ReadToEnd();
                }
            }

            var responseIsJson = response.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(responseBody);
            var bodyObject = responseIsJson ? JsonConvert.DeserializeObject(responseBody) : responseBody;

            //If the body is too large, just log the first x characters (and don't return the JSON object if applicable) 
            if(_loggedBodySizeLimit > -1 && responseBody.Length > _loggedBodySizeLimit)
            {
                bodyObject = responseBody.Substring(0, _loggedBodySizeLimit);
            }

            logData["Host"] = Environment.MachineName;
            logData["Body"] = bodyObject;
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
            var correlationId = GetCorrelationId(context);
            context.Items.Add(CorrelationIdString, correlationId);

            var request = context.Request;

            var formString = JsonConvert.SerializeObject(request.Form.ToDictionary());
            request.Body.Position = 0;
            string bodyString;
            using (var streamReader = new StreamReader(request.Body))
            {
                bodyString = streamReader.ReadToEnd();
            }
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

            //If the body is too large, just log the first x characters (and don't return the JSON object if applicable) 
            if (_loggedBodySizeLimit > -1 && bodyString.Length > _loggedBodySizeLimit)
            {
                bodyString = bodyString.Substring(0, _loggedBodySizeLimit);
                isBodyJson = false;
            }

            var query = (IDictionary<string, object>)context.Request.Query;
            var queryLog = query.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value.ToString()));

            logData["Host"] = Environment.MachineName;
            logData["Form"] = JsonConvert.DeserializeObject(formString);
            logData["Headers"] = request.Headers;
            logData["Query"] = queryLog;
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
            if (context.Items.ContainsKey(NancyServiceBootstrapper.StartTimeString))
            {
                var startTime = (DateTime)context.Items[NancyServiceBootstrapper.StartTimeString];
                var endTime = DateTime.UtcNow;
                logData[NancyServiceBootstrapper.StartTimeString] = startTime;
                logData[NancyServiceBootstrapper.EndTimeString] = endTime;
                logData["CallDuration"] = (int)endTime.Subtract(startTime).TotalMilliseconds;
            }

            var correlationId = string.Empty;
            if (context.Items.ContainsKey(CorrelationIdString))
            {
                correlationId = (string) context.Items[CorrelationIdString];
            }

            logData["Host"] = Environment.MachineName;
            logData["StackTrace"] = ex.ToString();

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
