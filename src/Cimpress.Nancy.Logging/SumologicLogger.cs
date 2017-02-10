using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Components;
using log4net;

namespace Cimpress.Nancy.Logging
{
    public class SumologicLogger : INancyLogger
    {
        private ILog _leLogger;

        private string _applicationName;
        private string _environmentName;
        private string _sumoLogicBaseUri;

        public void Configure(IDictionary<string, string> options)
        {
            _applicationName = options["ApplicationName"];
            _environmentName = options["EnvironmentName"];
            _sumoLogicBaseUri = options["LoggingBaseUri"];

            SetJsonAppender();

            var envName = $"{_applicationName}.{_environmentName}";
            var leLogger = LogManager.GetLogger(_applicationName, envName);
            leLogger.Info(new BaseMessage
            {
                Message = "Service started at " + DateTime.Now
            });
            _leLogger = leLogger;
        }

        public void Debug(object data)
        {
            _leLogger?.Debug(data);
        }

        public void Info(object data)
        {
            _leLogger?.Info(data);
        }

        public void Error(object data)
        {
            _leLogger?.Error(data);
        }
        private void SetJsonAppender()
        {
            var appender = new SumologicAppender(_sumoLogicBaseUri)
            {
                Layout = new JsonLayout(_environmentName)
            };
            var repo = LogManager.CreateRepository(_applicationName);
            log4net.Config.BasicConfigurator.Configure(repo, appender);
        }
    }
}
