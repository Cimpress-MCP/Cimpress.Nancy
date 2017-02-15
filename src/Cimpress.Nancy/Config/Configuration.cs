using System.Collections.Generic;

namespace Cimpress.Nancy.Config
{
    public class Configuration : IConfiguration
    {
        public string Version { get; set; }
        public Dictionary<string, string> OptionalParameters { get; set; } 
    }
}