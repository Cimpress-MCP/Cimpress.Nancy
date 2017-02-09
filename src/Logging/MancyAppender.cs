using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace Cimpress.Nancy.Logging
{
    public class MancyAppender : AppenderSkeleton
    {
        private readonly HttpClient _httpClient;
        private readonly string _sumoLogicBaseUri;

        public MancyAppender(string sumoLogicBaseUri)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("UserAgent", "SumoLogicAppender");
            _httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");

            if (!sumoLogicBaseUri.EndsWith("?"))
            {
                sumoLogicBaseUri += '?';
            }
            _sumoLogicBaseUri = sumoLogicBaseUri;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            Task.Factory.StartNew(() =>
            {
                string body;
                using (var textWriter = new StringWriter())
                {
                    try
                    {
                        Layout.Format(textWriter, loggingEvent);
                        body = textWriter.ToString();
                    }
                    catch
                    {
                        body = "Error parsing message";
                    }
                }

                _httpClient.GetAsync(_sumoLogicBaseUri + body);
            });
        }
    }
}