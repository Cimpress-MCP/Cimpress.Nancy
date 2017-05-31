using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimpress.Nancy.Components;
using Cimpress.Nancy.Config;
using log4net;

namespace Cimpress.Nancy.Logging
{
    public class SumologicLogger : INancyLogger
    {
        public static readonly string ApplicationNameConfigKey = "ApplicationName";
        public static readonly string EnvironmentNameConfigKey = "EnvironmentName";
        public static readonly string SumologicBaseUriConfigKey = "SumologicBaseUri";

        private ILog _logger;

        private string _applicationName;
        private string _environmentName;
        private string _sumoLogicBaseUri;

        public SumologicLogger(IConfiguration configuration)
        {
            var result = configuration.OptionalParameters.TryGetValue(ApplicationNameConfigKey, out _applicationName);
            result &= configuration.OptionalParameters.TryGetValue(EnvironmentNameConfigKey, out _environmentName);
            result &= configuration.OptionalParameters.TryGetValue(SumologicBaseUriConfigKey, out _sumoLogicBaseUri);

            if (!result)
            {
                Console.WriteLine($"Error: {"Missing option(s) to configure SumologicLogger"}");
                return;
            }

            SetJsonAppender();

            var envName = $"{_applicationName}.{_environmentName}";
            var logger = LogManager.GetLogger(_applicationName, envName);
            logger.Info(new BaseMessage
            {
                Message = "Service started at " + DateTime.Now
            });
            _logger = logger;
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
