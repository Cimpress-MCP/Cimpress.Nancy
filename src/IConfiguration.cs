using System.Collections.Generic;

namespace Cimpress.Nancy
{
    public interface IConfiguration
    {
        string Version { get; set; }
        IDictionary<string, string> OptionalParameters { get; set; }
    }
}