using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace Cimpress.Nancy
{
    public class ConfigurationLoader : IConfigurationLoader
    {
        public T LoadConfiguration<T>(T configuration) where T : IConfiguration
        {
            configuration.Version = GetVersion();
            return configuration;
        }

        public virtual string GetVersion()
        {
            return "v0";
        }

        public async Task<T> GetConfigurationObject<T>(string configFile, string s3FileName, string s3BucketName) where T : IConfiguration
        {
            var configString = File.Exists(configFile) ? File.ReadAllText(configFile) : await RetrieveConfigFromS3(s3FileName, s3BucketName);

            var config = JsonConvert.DeserializeObject<T>(configString);

            if (config.OptionalParameters == null)
            {
                config.OptionalParameters = new Dictionary<string, string>();
            }
            return config;
        }

        private async Task<string> RetrieveConfigFromS3(string s3FileName, string s3BucketName)
        {
            var region = Amazon.RegionEndpoint.EUWest1;
            if (!string.IsNullOrEmpty(s3FileName))
            {
                try
                {

                    using (AmazonS3Client client = new AmazonS3Client(region))
                    {
                        var objectRequest = new GetObjectRequest
                        {
                            BucketName = s3BucketName,
                            Key = s3FileName,
                        };
                        var response = await client.GetObjectAsync(objectRequest);
                        using (var streamReader = new StreamReader(response.ResponseStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return string.Empty;
        }
    }
}