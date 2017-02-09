using System.Collections.Generic;
using Nancy;

namespace Cimpress.Nancy.Logging
{
    public class ErrorLogMessage : ILogMessage
    {
        public string ResponseReason { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StackTrace { get; set; }
        public string Host { get; set; }
        public int CallDuration { get; set; }
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}