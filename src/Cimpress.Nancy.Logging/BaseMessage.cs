using System.Collections.Generic;

namespace Cimpress.Nancy.Logging
{
    public class BaseMessage
    {
        public string Message { get; set; }
        public string CorrelationId { get; set; }
        public IDictionary<string, object> Info { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
