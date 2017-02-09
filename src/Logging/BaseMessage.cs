namespace Cimpress.Nancy.Logging
{
    public class BaseMessage
    {
        public string Message { get; set; }
        public string CorrelationId { get; set; }
        public ILogMessage Info { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
