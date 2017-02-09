using System.Collections.Generic;
using Cimpress.Nancy.Authentication;
using Nancy;

namespace Cimpress.Nancy.Logging
{
    public class RequestLogMessage : ILogMessage
    {
        public object Form { get; set; }
        public RequestHeaders Headers { get; set; }
        public object Body { get; set; }
        public string Host { get; set; }
        public string Method { get; set; }
        public UserIdentity User { get; set; }
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}