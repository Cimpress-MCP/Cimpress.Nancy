using System;
using System.IO;
using System.Net;
using log4net.Core;
using log4net.Layout;
using Newtonsoft.Json;

namespace Cimpress.Nancy.Logging
{
    public class MancyLayout : LayoutSkeleton
    {
        private readonly string _environment;
        private readonly string _hostname;

        public MancyLayout(string environment)
        {
            _environment = environment;
            _hostname = Dns.GetHostName();
        }

        public JsonSerializer JsonSerializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public override void ActivateOptions() { }

        //Do not write out exception message after the json to the writer
        public override bool IgnoresException { get { return false; } }

        public override void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            JsonSerializer.Serialize(writer, new MancyJsonLog
            {
                Host = _hostname,
                Environment = _environment,
                TimeStamp = DateTimeOffset.Now.ToString("o"),
                Data = loggingEvent.MessageObject,
                MessageHash = loggingEvent.RenderedMessage.GetHashCode(),
                RenderedMessage = loggingEvent.RenderedMessage,
                ExceptionData = loggingEvent.ExceptionObject,
                Level = loggingEvent.Level.DisplayName
            });
        }
    }
}
