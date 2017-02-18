using System.Collections.Generic;

namespace Cimpress.Nancy.Config
{
    public interface IConfiguration
    {
        string Version { get; set; }
        Dictionary<string, string> OptionalParameters { get; set; }
    }
}