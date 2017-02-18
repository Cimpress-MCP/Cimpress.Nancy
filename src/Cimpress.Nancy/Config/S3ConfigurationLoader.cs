using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace Cimpress.Nancy.Config
{
    public class S3ConfigurationLoader : IConfigurationLoader
    {
        private readonly string _s3FileName;
        private readonly string _s3BucketName;
        private readonly RegionEndpoint _region;

        public S3ConfigurationLoader(string s3FileName, string s3BucketName, RegionEndpoint region)
        {
            _s3FileName = s3FileName;
            _s3BucketName = s3BucketName;
            _region = region;
        }

        public async Task<T> LoadConfiguration<T>() where T : IConfiguration
        {
            var configuration = await GetConfigurationObject<T>();

            if (configuration == null)
            {
                return default(T);
            }

            if (string.IsNullOrEmpty(configuration.Version))
            {
                configuration.Version = GetVersion();
            }
            return configuration;
        }

        public virtual string GetVersion()
        {
            return "v0";
        }

        private async Task<T> GetConfigurationObject<T>() where T : IConfiguration
        {
            var configString = await RetrieveConfigFromS3(_s3FileName, _s3BucketName);

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

        private async Task<string> RetrieveConfigFromS3(string s3FileName, string s3BucketName)
        {
            if (!string.IsNullOrEmpty(s3FileName))
            {
                try
                {

                    using (AmazonS3Client client = new AmazonS3Client(_region))
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