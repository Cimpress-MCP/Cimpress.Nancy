using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Cimpress.Nancy.Authentication;
using Cimpress.Nancy.Logging;
using log4net;
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
        private const string StartTimeString = "StartTime";
        private const string CorrelationIdString = "CorrelationId";
        private readonly ILog _logger;
        private IAuthValidator _authValidator;

        protected NancyServiceBootstrapper() : base()
        {
            SetJsonAppender();
            _logger = CreateLogger();
        }

        protected virtual string SumoLogicBaseUri
        {
            get { return string.Empty; }
        }

        protected virtual string ApplicationName
        {
            get { return string.Empty; }
        }

        protected virtual string EnvironmentName
        {
            get { return string.Empty; }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.Register<ILog>(_logger);
            var authProvider = container.Resolve<IAuthVerifier>() ?? new AuthVerifier();
            AuthenticationExtensions.Verifier = authProvider;

            _authValidator = container.Resolve<IAuthValidator>();

            SetupPipelineHandlers(pipelines);
            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.AddDirectory("Swagger","Views");
            base.ConfigureConventions(nancyConventions);
        }

        private void SetJsonAppender()
        {
            var appender = new SumologicAppender(SumoLogicBaseUri)
            {
                Layout = new JsonLayout(EnvironmentName)
            };
            var repo = LogManager.CreateRepository(ApplicationName);
            log4net.Config.BasicConfigurator.Configure(repo, appender);
        }

        private ILog CreateLogger()
        {
            var envName = $"{ApplicationName}.{EnvironmentName}";
            var leLogger = LogManager.GetLogger(ApplicationName, envName);
            leLogger.Info(new BaseMessage
            {
                Message = "Service started at " + DateTime.Now
            });
            return leLogger;
        }

        private void SetupPipelineHandlers(IPipelines pipelines)
        {
            pipelines.BeforeRequest += OnBeforeRequest;

            pipelines.AfterRequest += OnAfterRequest;

            pipelines.OnError += OnError;
        }

        protected virtual RequestLogMessage ModifyRequestLogMessage(RequestLogMessage logMessage, NancyContext ctx)
        {
            return logMessage;
        }

        protected virtual ResponseLogMessage ModifyResponseLogMessage(ResponseLogMessage logMessage, NancyContext ctx)
        {
            return logMessage;
        }

        protected virtual ErrorLogMessage ModifyErrorLogMessage(ErrorLogMessage logMessage, NancyContext ctx)
        {
            return logMessage;
        }

        private Response OnBeforeRequest(NancyContext ctx)
        {
            var startTime = DateTime.UtcNow;
            ctx.Items.Add(StartTimeString, startTime);

            var correlationId = GetCorrelationId(ctx);
            ctx.Items.Add(CorrelationIdString, correlationId);

            var request = ctx.Request;

            var currentUser = _authValidator.GetUserFromContext(ctx);
            ctx.CurrentUser = currentUser;

            var formString = JsonConvert.SerializeObject(request.Form.ToDictionary());
            request.Body.Position = 0;
            var bodyString = new StreamReader(request.Body).ReadToEnd();
            request.Body.Position = 0;

            var isBodyJson = string.Equals(request.Headers.ContentType, "application/json", StringComparison.OrdinalIgnoreCase);

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

            _logger.Debug(new BaseMessage
            {
                Message = request.Path,
                CorrelationId = correlationId,
                Info = ModifyRequestLogMessage(new RequestLogMessage
                {
                    Host = Environment.MachineName,
                    Form = JsonConvert.DeserializeObject(formString),
                    Headers = request.Headers,
                    Body = isBodyJson ? bodyObject : bodyString,
                    Method = ctx.Request.Method,
                    User = currentUser,
                    AdditionalData = new Dictionary<string, object>()
                }, ctx)
            });
            return null;
        }

        private string GetCorrelationId(NancyContext ctx)
        {
            var correlationId = ctx.Request.Headers["CorrelationId"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            return correlationId;
        }

        private void OnAfterRequest(NancyContext ctx)
        {
            var request = ctx.Request;
            var response = ctx.Response;
            var duration = int.MinValue;
            if (ctx.Items.ContainsKey(StartTimeString))
            {
                duration = (int)DateTime.UtcNow.Subtract((DateTime)ctx.Items[StartTimeString]).TotalMilliseconds;
            }

            var correlationId = string.Empty;
            if (ctx.Items.ContainsKey(CorrelationIdString))
            {
                correlationId = (string) ctx.Items[CorrelationIdString];
            }
            
            var stream = new MemoryStream();
            response.Contents(stream);
            stream.Position = 0;
            var responseBody = new StreamReader(stream).ReadToEnd();
            var responseIsJson = response.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(responseBody);
            var bodyObject = responseIsJson ? JsonConvert.DeserializeObject(responseBody) : responseBody;

            _logger.Info(new BaseMessage
            {
                Message = request.Path,
                CorrelationId = correlationId,
                Info = ModifyResponseLogMessage(new ResponseLogMessage
                {
                    Host = Environment.MachineName,
                    Body = bodyObject,
                    ResponseReason = response.ReasonPhrase,
                    StatusCode = response.StatusCode,
                    CallDuration = duration,
                    AdditionalData = new Dictionary<string, object>()
                }, ctx)
            });
            ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            ctx.Response.Headers.Add("Access-Control-Allow-Headers", "APIKEY,Authorization,Access-Control-Allow-Origin,Content-Type,Accept");
            ctx.Response.Headers.Add("Access-Control-Allow-Methods", "OPTIONS,POST,GET,DELETE,PUT");
            ctx.Response.Headers.Add("CorrelationId", correlationId);
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
            var duration = int.MinValue;
            if (ctx.Items.ContainsKey(StartTimeString))
            {
                duration = (int)DateTime.UtcNow.Subtract((DateTime)ctx.Items[StartTimeString]).TotalMilliseconds;
            }

            var correlationId = string.Empty;
            if (ctx.Items.ContainsKey(CorrelationIdString))
            {
                correlationId = (string) ctx.Items[CorrelationIdString];
            }

            _logger.Error(new BaseMessage
            {
                Message = request.Path,
                CorrelationId = correlationId,
                Info = ModifyErrorLogMessage(new ErrorLogMessage
                {
                    Host = Environment.MachineName,
                    ResponseReason = responseReason,
                    StatusCode = responseStatus,
                    StackTrace = ex.ToString(),
                    CallDuration = duration,
                    AdditionalData = new Dictionary<string, object>()
                }, ctx)
            });

            var newResponse = new Response { ReasonPhrase = responseReason, StatusCode = responseStatus };
            newResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            newResponse.Headers.Add("CorrelationId", correlationId);
            return newResponse;
        }
    }
}
