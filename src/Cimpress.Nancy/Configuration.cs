using System.Collections.Generic;

namespace Cimpress.Nancy
{
    public class Configuration : IConfiguration
    {
        public string Version { get; set; }
        public IDictionary<string, string> OptionalParameters { get; set; } 
    }
}