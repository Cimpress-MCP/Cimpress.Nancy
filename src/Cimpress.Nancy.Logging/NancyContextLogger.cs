using Nancy;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cimpress.Nancy.Logging
{
    public class NancyContextLogger
    {
        private const string DurationsKey = "Durations";
        private const string MetadataKey = "Metadata";

        private readonly ConcurrentDictionary<string, long> _durations;
        private readonly ConcurrentDictionary<string, object> _metadata;

        public NancyContextLogger(NancyContext context)
        {
            var logData = NancyServiceBootstrapper.GetLogData(context);

            if (!logData.ContainsKey(DurationsKey))
            {
                logData[DurationsKey] = new ConcurrentDictionary<string, long>();
            }
            _durations = (ConcurrentDictionary<string, long>) logData[DurationsKey];

            if (!logData.ContainsKey(MetadataKey))
            {
                logData[MetadataKey] = new ConcurrentDictionary<string, object>();
            }
            _metadata = (ConcurrentDictionary<string, object>) logData[MetadataKey];
        }

        public void AddMetadata(string key, object value)
        {
            _metadata[key] = value;
        }

        public void LogDuration(Action action, string operation)
        {
            LogDuration(() => { action(); return 0; }, operation);
        }

        public T LogDuration<T>(Func<T> myFunc, string operation)
        {
            var sw = Stopwatch.StartNew();
            T result;
            try
            {
                result = myFunc();
            }
            finally
            {
                _durations[operation] = sw.ElapsedMilliseconds;
            }
            return result;
        }

        public async Task<T> LogDuration<T>(Task<T> task, string operation)
        {
            var sw = Stopwatch.StartNew();
            T result;
            try
            {
                result = await task.ConfigureAwait(false);
            }
            finally
            {
                _durations[operation] = sw.ElapsedMilliseconds;
            }
            return result;
        }

        public async Task LogDuration(Task task, string operation)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await task.ConfigureAwait(false);
            }
            finally
            {
                _durations[operation] = sw.ElapsedMilliseconds;
            }
        }
    }
}
