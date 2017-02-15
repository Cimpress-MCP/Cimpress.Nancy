using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace Cimpress.Nancy.Config
{
    public class FileConfigurationLoader : IConfigurationLoader
    {
        private readonly string _configFilePath;

        public FileConfigurationLoader(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public Task<T> LoadConfiguration<T>() where T : IConfiguration
        {
            var configuration = GetConfigurationObject<T>();

            if (configuration == null)
            {
                return Task.FromResult(default(T));
            }

            if (string.IsNullOrEmpty(configuration.Version))
            {
                configuration.Version = GetVersion();
            }
            return Task.FromResult(configuration);
        }

        public virtual string GetVersion()
        {
            return "v0";
        }

        private T GetConfigurationObject<T>() where T : IConfiguration
        {
            var configString = File.Exists(_configFilePath) ? File.ReadAllText(_configFilePath) : string.Empty;

            if (string.IsNullOrEmpty(configString))
            {
                return default(T);
            }

            var config = JsonConvert.DeserializeObject<T>(configString);

            if (config.OptionalParameters == null)
            {
                config.OptionalParameters = new Dictionary<string, string>();
            }
            return config;
        }
    }
}