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
        private ILog _logger;

        private string _applicationName;
        private string _environmentName;
        private string _sumoLogicBaseUri;

        public void Configure(IDictionary<string, string> options)
        {
            var result = options.TryGetValue("ApplicationName", out _applicationName);
            result &= options.TryGetValue("EnvironmentName", out _environmentName);
            result &= options.TryGetValue("LoggingBaseUri", out _sumoLogicBaseUri);

            if (!result)
            {
                Console.WriteLine($"Error: {"Missing option(s) to configure SumologicLogger"}");
                return;
            }

            SetJsonAppender();

            var envName = $"{_applicationName}.{_environmentName}";
            var leLogger = LogManager.GetLogger(_applicationName, envName);
            leLogger.Info(new BaseMessage
            {
                Message = "Service started at " + DateTime.Now
            });
            _logger = leLogger;
        }

        public void Debug(object data)
        {
            _logger?.Debug(data);
        }

        public void Info(object data)
        {
            _logger?.Info(data);
        }

        public void Error(object data)
        {
            _logger?.Error(data);
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
